using _Emulator;
using UnityEngine;

public class CannonController : MonoBehaviour
{
	private int brickSeq = -1;

	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	public Texture2D heatBg;

	public Texture2D heatGauge;

	public Texture2D heatGaugeFrame;

	public float AtkPow = 100f;

	public float Rigidity = 0.68f;

	public Weapon.BY weaponBy = Weapon.BY.VULCAN;

	public float Range = 100f;

	public Texture2D vCrossHair;

	public Texture2D hCrossHair;

	public Accuracy accuracy;

	public GameObject fpsLink;

	public GameObject tpsLink;

	public GameObject axle;

	public Transform[] muzzles;

	public GameObject[] muzzleFxInstances;

	public float controllableDistance = 1f;

	public float tryingTime = 5f;

	public float xSpeed = 270f;

	public float ySpeed = 120f;

	public float cameraSpeedFactor = 1f;

	public float yMinLimit = -75f;

	public float yMaxLimit = 75f;

	public float fireVolume = 1f;

	public AudioClip fireSound;

	public GameObject muzzleFire;

	public float recoilPitch = 2f;

	public float heatMax = 10f;

	public float heatUpSpeed = 1.5f;

	public float heatDownSpeed = 1f;

	public float overHeatDelay = 3f;

	private bool overHeated;

	public float overHeatTime;

	public float rateOfFire = 750f;

	private bool cyclic;

	private float deltaTime;

	private LocalController localController;

	private int shooter = -1;

	private float pitchUp;

	private float sumOfPitchUp;

	private float heat;

	private int fireCount;

	private float lastNotified;

	private float x;

	private float y;

	private Camera cam;

	private Vector3 ntfdDir = Vector3.forward;

	private Animation bipAni;

	protected float crossEffectTime;

	private AudioSource audioSource;

	public int BrickSeq => brickSeq;

	public int Shooter => shooter;

	public bool CheckControllable(Transform reference)
	{
		if (null == fpsLink)
		{
			return false;
		}
		return Direction.IsForward(fpsLink.transform.position, reference) && Vector3.Distance(fpsLink.transform.position, reference.position) < controllableDistance;
	}

	private void VerifyCamera()
	{
		if (null == cam)
		{
			GameObject gameObject = GameObject.Find("Main Camera");
			if (null != gameObject)
			{
				cam = gameObject.GetComponent<Camera>();
			}
		}
	}

	private void DoMuzzleFire()
	{
		if (muzzles != null && muzzles.Length > 0 && !(muzzleFire == null))
		{
			int num = fireCount % muzzles.Length;
			if (muzzleFxInstances[num] == null)
			{
				GameObject gameObject = Object.Instantiate((Object)muzzleFire) as GameObject;
				gameObject.transform.position = muzzles[num].position;
				gameObject.transform.parent = muzzles[num];
				gameObject.transform.localRotation = Quaternion.Euler(90f, 90f, 0f);
				muzzleFxInstances[num] = gameObject;
			}
			ParticleEmitter particleEmitter = null;
			int childCount = muzzleFxInstances[num].transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = muzzleFxInstances[num].transform.GetChild(i);
				particleEmitter = child.GetComponent<ParticleEmitter>();
				if ((bool)particleEmitter)
				{
					particleEmitter.Emit();
				}
			}
		}
	}

	private void DoFireSound()
	{
		audioSource = GetComponent<AudioSource>();
		if (null != audioSource && null != fireSound)
		{
			audioSource.PlayOneShot(fireSound);
		}
	}

	public void DoFireAnimation()
	{
		if (!(null == bipAni))
		{
			if (!bipAni.IsPlaying("fire"))
			{
				bipAni.Play("fire");
			}
			else
			{
				float length = bipAni["fire"].length;
				bipAni["fire"].time = length / 4f;
			}
		}
	}

	private void InitializeAnimation()
	{
		bipAni = axle.GetComponentInChildren<Animation>();
		if (null == bipAni)
		{
			Debug.LogError("ERROR, Fail to get animation component for cannon");
		}
		else
		{
			bipAni.wrapMode = WrapMode.Loop;
			bipAni["idle"].layer = 1;
			bipAni["idle"].wrapMode = WrapMode.Loop;
			bipAni["fire"].layer = 1;
			bipAni["fire"].wrapMode = WrapMode.Once;
			bipAni.CrossFade("idle");
		}
	}

	private void Start()
	{
		localController = null;
		brickSeq = -1;
		shooter = -1;
		InitializeAnimation();
		audioSource = GetComponent<AudioSource>();
		BrickProperty component = base.transform.parent.GetComponent<BrickProperty>();
		if (null == component)
		{
			Debug.LogError("ERROR, Fail to get brick sequence number for cannon controller");
		}
		else
		{
			brickSeq = component.Seq;
		}
		if (null != axle)
		{
			SaveAxleAngle();
		}
		muzzleFxInstances = new GameObject[muzzles.Length];
		for (int i = 0; i < muzzles.Length; i++)
		{
			muzzleFxInstances[i] = null;
		}
	}

	private void SaveAxleAngle()
	{
		Vector3 eulerAngles = axle.transform.eulerAngles;
		x = eulerAngles.y;
		y = eulerAngles.x;
		if (y > yMaxLimit)
		{
			y -= 360f;
		}
	}

	public void SetShooter(int player)
	{
		if (shooter == MyInfoManager.Instance.Seq)
		{
			shooter = -1;
			localController = null;
			Recursively.ChangeLayer(base.transform, LayerMask.NameToLayer("FpsBrick"), LayerMask.NameToLayer("Brick"));
		}
		else if (shooter >= 0)
		{
			GameObject gameObject = BrickManManager.Instance.Get(shooter);
			if (null != gameObject)
			{
				gameObject.BroadcastMessage("OnLeaveCannon");
				gameObject.transform.parent = null;
			}
			SaveAxleAngle();
		}
		shooter = player;
		if (shooter == MyInfoManager.Instance.Seq)
		{
			GameObject gameObject2 = GameObject.Find("Me");
			if (null != gameObject2)
			{
				localController = gameObject2.GetComponent<LocalController>();
			}
			if (null == gameObject2 || null == localController || localController.IsDead)
			{
				localController = null;
				shooter = -1;
			}
			else
			{
				Recursively.ChangeLayer(base.transform, LayerMask.NameToLayer("Brick"), LayerMask.NameToLayer("FpsBrick"));
				gameObject2.BroadcastMessage("OnGetCannon", this);
				cyclic = false;
				heat = 0f;
				overHeatTime = 0f;
				overHeated = false;
				SaveAxleAngle();
				lastNotified = 0f;
			}
		}
		else if (shooter >= 0)
		{
			TPController tPController = null;
			GameObject gameObject3 = BrickManManager.Instance.Get(shooter);
			if (null != gameObject3)
			{
				tPController = gameObject3.GetComponent<TPController>();
			}
			if (null == gameObject3 || null == tPController || tPController.IsDead)
			{
				shooter = -1;
			}
			else
			{
				gameObject3.BroadcastMessage("OnGetCannon", this);
				gameObject3.transform.parent = tpsLink.transform;
				gameObject3.transform.position = tpsLink.transform.position;
				gameObject3.transform.rotation = tpsLink.transform.rotation;
			}
		}
	}

	private float GetHeatRatio()
	{
		return heat / heatMax;
	}

	private void DrawHeatGauge()
	{
		Rect position = new Rect((float)(Screen.width - heatBg.width - 1), (float)(Screen.height - heatBg.height - 1), (float)heatBg.width, (float)heatBg.height);
		TextureUtil.DrawTexture(position, heatBg);
		Texture2D texture2D = TItemManager.Instance.GetWeaponBy((int)weaponBy);
		if (texture2D != null && heatGaugeFrame != null && heatGauge != null)
		{
			float num = 54f;
			float num2 = (float)texture2D.width * num / (float)texture2D.height;
			TextureUtil.DrawTexture(new Rect(position.x + (position.width - num2) / 2f, position.y + 10f, num2, num), texture2D, ScaleMode.StretchToFill);
			Rect position2 = new Rect(position.x + (position.width - 91f) / 2f, (float)(Screen.height - 26), 91f, 14f);
			TextureUtil.DrawTexture(position2, heatGaugeFrame, ScaleMode.StretchToFill);
			Color color = GUI.color;
			float heatRatio = GetHeatRatio();
			GUI.color = Color.Lerp(Color.white, Color.red, heatRatio);
			float num3 = overHeatTime - (float)Mathf.FloorToInt(overHeatTime);
			if (!overHeated || num3 > 0.2f)
			{
				TextureUtil.DrawTexture(new Rect(position2.x + 3f, position2.y + 2f, (float)heatGauge.width * heatRatio, (float)heatGauge.height), heatGauge);
			}
			GUI.color = color;
		}
	}

	public void CheckSoundOnOff()
	{
		if (null != audioSource)
		{
			if (GlobalVars.Instance.mute)
			{
				audioSource.volume = 0f;
			}
			else
			{
				audioSource.volume = fireVolume;
			}
		}
	}

	private bool IsCoolDown()
	{
		if (overHeated)
		{
			return true;
		}
		if (deltaTime < 0f)
		{
			return false;
		}
		float num = rateOfFire / 60f;
		if (num <= 0f)
		{
			return true;
		}
		float num2 = 1f / num;
		return deltaTime < num2;
	}

	private bool CanFire()
	{
		return localController.CanControl();
	}

	private void Fire()
	{
		if (!IsCoolDown())
		{
			fireCount++;
			deltaTime = 0f;
			pitchUp += recoilPitch;
			DoFireAnimation();
			DoFireSound();
			DoMuzzleFire();
			accuracy.MakeInaccurate(aimAccurateMore: false);
			Shoot();
		}
	}

	private void Shoot()
	{
		Vector2 vector = accuracy.CalcDeflection();
		int layerMask = (1 << LayerMask.NameToLayer("Chunk")) | (1 << LayerMask.NameToLayer("Brick")) | (1 << LayerMask.NameToLayer("BoxMan")) | (1 << LayerMask.NameToLayer("Mon")) | (1 << LayerMask.NameToLayer("InvincibleArmor")) | (1 << LayerMask.NameToLayer("Bomb")) | (1 << LayerMask.NameToLayer("InstalledBomb"));
		Ray ray = cam.ScreenPointToRay(new Vector3(vector.x, vector.y, 0f));
		if (!Application.loadedLevelName.Contains("Tutor"))
		{
			P2PManager.Instance.SendPEER_CANNON_FIRE(BrickSeq, shooter, ray.origin, ray.direction);
		}
		if (Physics.Raycast(ray, out RaycastHit hitInfo, Range, layerMask))
		{
			GameObject gameObject = hitInfo.transform.gameObject;
			if (gameObject.layer == LayerMask.NameToLayer("Brick") || gameObject.layer == LayerMask.NameToLayer("Chunk"))
			{
				GameObject gameObject2 = null;
				BrickProperty brickProperty = null;
				GameObject original = null;
				Texture2D mark = null;
				if (gameObject.layer == LayerMask.NameToLayer("Brick"))
				{
					BrickProperty[] allComponents = Recursively.GetAllComponents<BrickProperty>(gameObject.transform, includeInactive: false);
					if (allComponents.Length > 0)
					{
						brickProperty = allComponents[0];
					}
				}
				else
				{
					gameObject2 = BrickManager.Instance.GetBrickObjectByPos(Brick.ToBrickCoord(hitInfo.normal, hitInfo.point));
					if (null != gameObject2)
					{
						brickProperty = gameObject2.GetComponent<BrickProperty>();
					}
				}
				if (null != brickProperty)
				{
					P2PManager.Instance.SendPEER_HIT_BRICK(brickProperty.Seq, brickProperty.Index, hitInfo.point, hitInfo.normal, isBullet: true);
					mark = BrickManager.Instance.GetBulletMark(brickProperty.Index);
					original = BrickManager.Instance.GetBulletImpact(brickProperty.Index);
					Brick brick = BrickManager.Instance.GetBrick(brickProperty.Index);
					if (brick != null && brick.destructible)
					{
						brickProperty.Hit((int)AtkPow);
						if (brickProperty.HitPoint <= 0)
						{
							CSNetManager.Instance.Sock.SendCS_DESTROY_BRICK_REQ(brickProperty.Seq);
							mark = null;
							original = null;
							if (brickProperty.Index == 115 || brickProperty.Index == 193)
							{
								ExplosionUtil.CheckMyself(gameObject2.transform.position, GlobalVars.Instance.BoomDamage, GlobalVars.Instance.BoomRadius, -3);
								ExplosionUtil.CheckBoxmen(gameObject2.transform.position, GlobalVars.Instance.BoomDamage, GlobalVars.Instance.BoomRadius, -3, Rigidity);
								ExplosionUtil.CheckMonster(gameObject2.transform.position, GlobalVars.Instance.BoomDamage, GlobalVars.Instance.BoomRadius);
								ExplosionUtil.CheckDestructibles(gameObject2.transform.position, GlobalVars.Instance.BoomDamage, GlobalVars.Instance.BoomRadius);
							}
						}
						else
						{
							P2PManager.Instance.SendPEER_BRICK_HITPOINT(brickProperty.Seq, brickProperty.HitPoint);
						}
					}
				}
				if (null != gameObject2 && null != mark)
				{
					GameObject gameObject3 = Object.Instantiate((Object)BrickManager.Instance.bulletMark, hitInfo.point, Quaternion.FromToRotation(Vector3.forward, -hitInfo.normal)) as GameObject;
					BulletMark component = gameObject3.GetComponent<BulletMark>();
					component.GenerateDecal(mark, gameObject, gameObject2);
				}
				if (null != original)
				{
					Object.Instantiate((Object)original, hitInfo.point, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
				}
			}
			else if (gameObject.layer == LayerMask.NameToLayer("BoxMan"))
			{
				PlayerProperty[] allComponents2 = Recursively.GetAllComponents<PlayerProperty>(gameObject.transform, includeInactive: false);
				TPController[] allComponents3 = Recursively.GetAllComponents<TPController>(gameObject.transform, includeInactive: false);
				if (allComponents2.Length != 1)
				{
					Debug.LogError("PlayerProperty should be unique for a box man, but it has multiple PlayerProperty components or non ");
				}
				if (allComponents3.Length != 1)
				{
					Debug.LogError("TPController should be unique for a box man, but it has multiple TPController components or non ");
				}
				PlayerProperty playerProperty = null;
				TPController tPController = null;
				if (allComponents2.Length > 0)
				{
					playerProperty = allComponents2[0];
				}
				if (allComponents3.Length > 0)
				{
					tPController = allComponents3[0];
				}
				if (playerProperty != null && tPController != null)
				{
					int num = 0;
					HitPart component2 = gameObject.GetComponent<HitPart>();
					if (component2 != null)
					{
						bool flag = false;
						if (component2.part == HitPart.TYPE.HEAD)
						{
							int layerMask2 = 1 << LayerMask.NameToLayer("Brain");
							if (Physics.Raycast(ray, out RaycastHit hitInfo2, Range, layerMask2))
							{
								if (playerProperty.Desc.IsLucky())
								{
									flag = true;
								}
								else
								{
									component2 = hitInfo2.transform.gameObject.GetComponent<HitPart>();
								}
							}
						}
						if (component2.GetHitImpact() != null)
						{
							GameObject original2 = component2.GetHitImpact();
							if (flag && null != component2.luckyImpact)
							{
								original2 = component2.luckyImpact;
							}
							Object.Instantiate((Object)original2, hitInfo.point, Quaternion.Euler(0f, 0f, 0f));
						}
						num = (int)(AtkPow * component2.damageFactor);
						if (!playerProperty.IsHostile())
						{
							num = 0;
						}
						P2PManager.Instance.SendPEER_HIT_BRICKMAN(MyInfoManager.Instance.Seq, playerProperty.Desc.Seq, (int)component2.part, hitInfo.point, hitInfo.normal, flag, 0, ray.direction);
						P2PManager.Instance.SendPEER_SHOOT(MyInfoManager.Instance.Seq, playerProperty.Desc.Seq, num, Rigidity, (int)weaponBy, (int)component2.part, flag, rateOfFire);
					}
					tPController.GetHit(num, playerProperty.Desc.Seq);
				}
			}
			else if (gameObject.layer == LayerMask.NameToLayer("Mon"))
			{
				MonProperty[] allComponents4 = Recursively.GetAllComponents<MonProperty>(gameObject.transform, includeInactive: false);
				MonProperty monProperty = null;
				if (allComponents4.Length > 0)
				{
					monProperty = allComponents4[0];
				}
				if (monProperty != null)
				{
					HitPart component3 = gameObject.GetComponent<HitPart>();
					if (component3 != null && (MyInfoManager.Instance.Slot >= 4 || !monProperty.Desc.bRedTeam) && (MyInfoManager.Instance.Slot < 4 || monProperty.Desc.bRedTeam))
					{
						if (component3.GetHitImpact() != null)
						{
							Object.Instantiate((Object)component3.GetHitImpact(), hitInfo.point, Quaternion.Euler(0f, 0f, 0f));
						}
						if (monProperty.Desc.Xp > 0)
						{
							int num2 = (int)(AtkPow * component3.damageFactor);
							num2 += DefenseManager.Instance.AddAtkPower;
							if (monProperty.Desc.bHalfDamage)
							{
								num2 /= 2;
							}
							MonManager.Instance.Hit(monProperty.Desc.Seq, num2, 0f, (int)weaponBy, hitInfo.point, hitInfo.normal, -1);
						}
					}
				}
			}
			else if (gameObject.layer == LayerMask.NameToLayer("InvincibleArmor") || gameObject.layer == LayerMask.NameToLayer("Bomb") || gameObject.layer == LayerMask.NameToLayer("InstalledBomb"))
			{
				GameObject impact = VfxOptimizer.Instance.GetImpact(gameObject.layer);
				if (null != impact)
				{
					Object.Instantiate((Object)impact, hitInfo.point, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
					P2PManager.Instance.SendPEER_HIT_IMPACT(gameObject.layer, hitInfo.point, hitInfo.normal);
				}
			}
		}
	}

	private void CheckFire1()
	{
		if (CanFire())
		{
			if (custom_inputs.Instance.GetButtonDown("K_FIRE1"))
			{
				cyclic = true;
				Fire();
			}
			if (custom_inputs.Instance.GetButtonUp("K_FIRE1"))
			{
				cyclic = false;
			}
			if (!overHeated)
			{
				if (!cyclic)
				{
					heat -= heatDownSpeed * Time.deltaTime;
				}
				else
				{
					Fire();
					heat += heatUpSpeed * Time.deltaTime;
				}
				if (heat > heatMax)
				{
					overHeatTime = 0f;
					overHeated = true;
				}
				heat = Mathf.Clamp(heat, 0f, heatMax);
			}
		}
	}

	private void DrawCrossHair()
	{
		float num = (float)Screen.width * accuracy.Inaccurate;
		Vector2 vector = new Vector2(((float)Screen.width - num) / 2f, ((float)Screen.height - num) / 2f);
		Color color = GUI.color;
		if (crossEffectTime > 0f)
		{
			GUI.color = Color.red;
		}
		else
		{
			GUI.color = Config.instance.crosshairColor;
		}
		if (null != vCrossHair)
		{
			vector = new Vector2((float)((Screen.width - 8) / 2), (float)(Screen.height / 2) - num / 2f - 8f);
			TextureUtil.DrawTexture(new Rect(vector.x, vector.y, 8f, 8f), vCrossHair, ScaleMode.StretchToFill, alphaBlend: true);
			vector = new Vector2((float)((Screen.width - 8) / 2), (float)(Screen.height / 2) + num / 2f);
			TextureUtil.DrawTexture(new Rect(vector.x, vector.y, 8f, 8f), vCrossHair, ScaleMode.StretchToFill, alphaBlend: true);
		}
		if (null != hCrossHair)
		{
			vector = new Vector2((float)(Screen.width / 2) - num / 2f - 8f, (float)((Screen.height - 8) / 2));
			TextureUtil.DrawTexture(new Rect(vector.x, vector.y, 8f, 8f), hCrossHair, ScaleMode.StretchToFill, alphaBlend: true);
			vector = new Vector2((float)(Screen.width / 2) + num / 2f, (float)((Screen.height - 8) / 2));
			TextureUtil.DrawTexture(new Rect(vector.x, vector.y, 8f, 8f), hCrossHair, ScaleMode.StretchToFill, alphaBlend: true);
		}
		GUI.color = color;
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn && shooter == MyInfoManager.Instance.Seq)
		{
			GUISkin skin = GUI.skin;
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			DrawHeatGauge();
			if (CanFire())
			{
				DrawCrossHair();
			}
			GUI.enabled = true;
			GUI.skin = skin;
		}
	}

	private void Update()
	{
		if (shooter == MyInfoManager.Instance.Seq)
		{
			VerifyCamera();
			if (overHeated)
			{
				overHeatTime += Time.deltaTime;
				if (overHeatTime > overHeatDelay)
				{
					overHeatTime = 0f;
					overHeated = false;
				}
			}
			deltaTime += Time.deltaTime;
			CheckFire1();
			accuracy.MakeAccurate(aimAccurate: true);
			UpdateCrossEffect();
		}
	}

	public void NotifyMove()
	{
		if (!Application.loadedLevelName.Contains("Tutor"))
		{
			P2PManager.Instance.SendPEER_CANNON_MOVE(brickSeq, shooter, x, y);
		}
	}

	private void LateUpdate()
	{
		if (shooter == MyInfoManager.Instance.Seq)
		{
			if (Screen.lockCursor)
			{
				float num = 0f;
				if (sumOfPitchUp > 0f)
				{
					num = y - Mathf.Lerp(y, y - sumOfPitchUp, 2f * Time.deltaTime);
					sumOfPitchUp -= num;
					if (sumOfPitchUp < 0f)
					{
						sumOfPitchUp = 0f;
					}
				}
				sumOfPitchUp += pitchUp;
				x += Input.GetAxis("Mouse X") * xSpeed * cameraSpeedFactor * 0.02f;
				if (GlobalVars.Instance.reverseMouse)
				{
					y += Input.GetAxis("Mouse Y") * ySpeed * cameraSpeedFactor * 0.02f - pitchUp + num;
				}
				else
				{
					y -= Input.GetAxis("Mouse Y") * ySpeed * cameraSpeedFactor * 0.02f + pitchUp - num;
				}
				pitchUp = 0f;
				y = Mathf.Clamp(y, yMinLimit, yMaxLimit);
			}
			axle.transform.rotation = Quaternion.Euler(y, x, 0f);
			lastNotified += Time.deltaTime;
			Vector3 normalized = axle.transform.TransformDirection(Vector3.forward).normalized;
			if ((lastNotified > 0.3f || Mathf.Abs(Vector3.Angle(ntfdDir, normalized)) > 1f) && lastNotified > BuildOption.Instance.Props.SendRate)
			{
				lastNotified = 0f;
				ntfdDir = normalized;
				NotifyMove();
			}
		}
	}

	public void Move(float x, float y)
	{
		if (null != axle)
		{
			axle.transform.rotation = Quaternion.Euler(y, x, 0f);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
	}

	public void SetShootEnermyEffect()
	{
		crossEffectTime = 0.3f;
	}

	public void UpdateCrossEffect()
	{
		crossEffectTime -= Time.deltaTime;
	}
}
