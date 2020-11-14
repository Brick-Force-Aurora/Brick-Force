using _Emulator;
using System.Collections.Generic;
using UnityEngine;

public class BrickBoomer : WeaponFunction
{
	public int maxAmmo = -1;

	private SecureInt curAmmoSecure;

	protected int maxAmmoInst;

	public ImageFont ammoFont;

	public float rateOfFire = 5f;

	public Texture2D vCrossHair;

	public Texture2D hCrossHair;

	public GameObject muzzleFire;

	public Texture2D icon;

	public GameObject targetBox;

	private float deltaTime = float.NegativeInfinity;

	private GameObject target;

	private RaycastHit hitBrick;

	private float distance = float.PositiveInfinity;

	private Transform muzzle;

	private GameObject muzzleFxInstance;

	private bool copyRight;

	private BattleChat battleChat;

	private string statusMessage = string.Empty;

	private float statusDelta;

	private float statusMessageLimit = 5f;

	private int curAmmo
	{
		get
		{
			return curAmmoSecure.Get();
		}
		set
		{
			curAmmoSecure.Set(value);
		}
	}

	public int CurAmmo => curAmmo;

	private void Awake()
	{
		maxAmmoInst = maxAmmo;
		curAmmoSecure.Init(0);
	}

	private void OnDestroy()
	{
		curAmmoSecure.Release();
	}

	public override void Reset(bool bDefenseRespwan = false)
	{
		curAmmoSecure.Reset();
		curAmmo = NoCheat.Instance.HideVal(NoCheat.WATCH_DOG.MAIN_AMMO, maxAmmoInst);
		NoCheat.Instance.Sync(NoCheat.WATCH_DOG.MAIN_AMMO, curAmmo);
	}

	protected void UseAmmo()
	{
		curAmmo = NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.MAIN_AMMO, curAmmo) - 1;
		if (curAmmo < 0)
		{
			curAmmo = 0;
		}
		curAmmo = NoCheat.Instance.HideVal(NoCheat.WATCH_DOG.MAIN_AMMO, curAmmo);
		NoCheat.Instance.Sync(NoCheat.WATCH_DOG.MAIN_AMMO, curAmmo);
		if (curAmmo == 0 && RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BUNGEE)
		{
			GameObject gameObject = GameObject.Find("Me");
			if (null != gameObject)
			{
				LocalController component = gameObject.GetComponent<LocalController>();
				if (component != null)
				{
					component.ReturnBuildGun();
				}
			}
		}
	}

	public override bool IsFullAmmo()
	{
		return curAmmo >= maxAmmoInst;
	}

	protected void DrawAmmo()
	{
		if (MyInfoManager.Instance.isGuiOn && null != ammoBg && null != icon)
		{
			Rect position = new Rect((float)(Screen.width - ammoBg.width - 1), (float)(Screen.height - ammoBg.height - 1), (float)ammoBg.width, (float)ammoBg.height);
			TextureUtil.DrawTexture(position, ammoBg);
			float num = 40f;
			float num2 = (float)icon.width * num / (float)icon.height;
			TextureUtil.DrawTexture(new Rect(position.x + (position.width - num2) / 2f, position.y + 10f, num2, num), icon, ScaleMode.StretchToFill);
			ammoFont.Print(new Vector2((float)(Screen.width - 10), (float)(Screen.height - 12)), NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.MAIN_AMMO, curAmmo));
		}
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn)
		{
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			DrawCrossHair();
			DrawAmmo();
			PaletteManager.Instance.Use();
			GUI.enabled = true;
		}
	}

	private void InitializeAnimation()
	{
		base.animation.wrapMode = WrapMode.Loop;
		base.animation["fire"].layer = 1;
		base.animation["fire"].wrapMode = WrapMode.Once;
		base.animation["idle"].layer = 1;
		base.animation["idle"].wrapMode = WrapMode.Loop;
		base.animation.CrossFade("idle");
	}

	private void VerifyBattleChat()
	{
		if (null == battleChat)
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				battleChat = gameObject.GetComponent<BattleChat>();
			}
		}
	}

	public bool IsCoolDown()
	{
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

	private void Start()
	{
		muzzle = null;
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
		int num = 0;
		while (muzzle == null && num < componentsInChildren.Length)
		{
			if (componentsInChildren[num].name.Contains("Dummy_fire_effect"))
			{
				muzzle = componentsInChildren[num];
			}
			num++;
		}
		if (null == muzzle)
		{
			Debug.LogError("Muzzle is null ");
		}
		InitializeAnimation();
		CreateComposerTarget();
		copyRight = false;
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.MAP_EDITOR && UserMapInfoManager.Instance.GetCur() != null)
		{
			copyRight = true;
		}
	}

	private void CreateComposerTarget()
	{
		if (null == target)
		{
			target = (Object.Instantiate((Object)targetBox) as GameObject);
			if (null != target)
			{
				target.GetComponent<ComposerTarget>().ShowTarget(show: false);
			}
		}
	}

	private void DestroyComposerTarget()
	{
		if (null != target)
		{
			Object.Destroy(target);
			target = null;
		}
	}

	private void OnDisable()
	{
		DestroyComposerTarget();
	}

	private void OnEnable()
	{
		CreateComposerTarget();
	}

	private void CreateMuzzleFire()
	{
		if (!(null == muzzleFire) && !(null == muzzle))
		{
			if (muzzleFxInstance == null)
			{
				GameObject gameObject = Object.Instantiate((Object)muzzleFire) as GameObject;
				Recursively.SetLayer(gameObject.GetComponent<Transform>(), LayerMask.NameToLayer("FPWeapon"));
				gameObject.transform.position = muzzle.position;
				gameObject.transform.parent = muzzle;
				gameObject.transform.localRotation = Quaternion.Euler(90f, 90f, 0f);
				muzzleFxInstance = gameObject;
			}
			ParticleEmitter particleEmitter = null;
			int childCount = muzzleFxInstance.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = muzzleFxInstance.transform.GetChild(i);
				particleEmitter = child.GetComponent<ParticleEmitter>();
				if ((bool)particleEmitter)
				{
					particleEmitter.Emit();
				}
			}
		}
	}

	private void Restart()
	{
	}

	protected void DrawCrossHair()
	{
		if (Screen.lockCursor)
		{
			GUI.depth = 35;
			Color color = GUI.color;
			GUI.color = Config.instance.crosshairColor;
			if (null != vCrossHair)
			{
				Vector2 vector = new Vector2((float)((Screen.width - 8) / 2), (float)(Screen.height / 2 - 8));
				TextureUtil.DrawTexture(new Rect(vector.x, vector.y, 8f, 8f), vCrossHair, ScaleMode.StretchToFill, alphaBlend: true);
				vector = new Vector2((float)((Screen.width - 8) / 2), (float)(Screen.height / 2));
				TextureUtil.DrawTexture(new Rect(vector.x, vector.y, 8f, 8f), vCrossHair, ScaleMode.StretchToFill, alphaBlend: true);
			}
			if (null != hCrossHair)
			{
				Vector2 vector = new Vector2((float)(Screen.width / 2 - 8), (float)((Screen.height - 8) / 2));
				TextureUtil.DrawTexture(new Rect(vector.x, vector.y, 8f, 8f), hCrossHair, ScaleMode.StretchToFill, alphaBlend: true);
				vector = new Vector2((float)(Screen.width / 2), (float)((Screen.height - 8) / 2));
				TextureUtil.DrawTexture(new Rect(vector.x, vector.y, 8f, 8f), hCrossHair, ScaleMode.StretchToFill, alphaBlend: true);
			}
			GUI.color = color;
			string text = string.Empty;
			if (null != localCtrl)
			{
				float horzAngle = localCtrl.GetHorzAngle();
				if ((292.5 <= (double)horzAngle && horzAngle <= 360f) || (0f <= horzAngle && horzAngle <= 67.5f))
				{
					text += "N";
				}
				else if (112.5f <= horzAngle && horzAngle <= 247.5f)
				{
					text += "S";
				}
				if (22.5f <= horzAngle && horzAngle <= 157.5f)
				{
					text += "E";
				}
				else if (202.5f <= horzAngle && horzAngle <= 337.5f)
				{
					text += "W";
				}
			}
			if (distance != float.PositiveInfinity && hitBrick.transform != null)
			{
				BrickProperty hitBrickProperty = BrickManager.Instance.GetHitBrickProperty(hitBrick.transform.gameObject, hitBrick.normal, hitBrick.point);
				if (null != hitBrickProperty)
				{
					text += ": ";
					text = text + distance.ToString("0.#") + "m";
				}
			}
			LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), (float)(Screen.height / 2 + 16)), text, "Label", Color.white, Color.black, TextAnchor.MiddleCenter);
		}
	}

	private void ShowTarget(bool show)
	{
		if (!(null == target))
		{
			if (show)
			{
				BrickInst hitBrickInst = BrickManager.Instance.GetHitBrickInst(hitBrick.transform.gameObject, hitBrick.normal, hitBrick.point);
				if (hitBrickInst == null)
				{
					show = false;
				}
				else
				{
					Vector3 vector = new Vector3((float)(int)hitBrickInst.PosX, (float)(int)hitBrickInst.PosY, (float)(int)hitBrickInst.PosZ);
					GameObject brickObjectByPos = BrickManager.Instance.GetBrickObjectByPos(vector);
					BoxCollider boxCollider = null;
					if (brickObjectByPos != null)
					{
						boxCollider = brickObjectByPos.GetComponent<BoxCollider>();
					}
					if (null != boxCollider)
					{
						target.GetComponent<ComposerTarget>().CenterAndSize(boxCollider.center, boxCollider.size);
						target.transform.position = vector;
						target.transform.rotation = Rot.ToQuaternion(hitBrickInst.Rot);
					}
					else
					{
						target.GetComponent<ComposerTarget>().CenterAndSize(new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f));
						target.transform.position = vector;
						target.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
					}
				}
			}
			target.GetComponent<ComposerTarget>().ShowTarget(show);
		}
	}

	private bool CheckAimed()
	{
		bool result = false;
		int layerMask = (1 << LayerMask.NameToLayer("Chunk")) | (1 << LayerMask.NameToLayer("Edit Layer")) | (1 << LayerMask.NameToLayer("BndWall"));
		Ray ray = cam.ScreenPointToRay(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f));
		if (!Physics.Raycast(ray, out RaycastHit hitInfo, GetComponent<Weapon>().range, layerMask))
		{
			distance = float.PositiveInfinity;
		}
		else
		{
			if (hitInfo.transform.gameObject.layer != LayerMask.NameToLayer("BndWall"))
			{
				result = true;
				hitBrick = hitInfo;
			}
			distance = Vector3.Distance(cam.transform.position, hitInfo.point);
		}
		return result;
	}

	private byte GetRotFromCameraDir()
	{
		Vector3 from = cam.transform.TransformDirection(Vector3.forward);
		from.y = 0f;
		from = from.normalized;
		float[] array = new float[4]
		{
			Vector3.Angle(from, Vector3.back),
			Vector3.Angle(from, Vector3.left),
			Vector3.Angle(from, Vector3.forward),
			Vector3.Angle(from, Vector3.right)
		};
		float num = 360f;
		int num2 = -1;
		for (int i = 0; i < 4; i++)
		{
			if (num > array[i])
			{
				num2 = i;
				num = array[i];
			}
		}
		return (byte)num2;
	}

	private byte GetNewBricksRot(Brick newBrick, Vector3 normal)
	{
		byte result = 0;
		if (newBrick.directionable)
		{
			result = (byte)((!(normal == Vector3.forward)) ? ((normal == Vector3.right) ? 1 : ((normal == Vector3.back) ? 2 : ((!(normal == Vector3.left)) ? GetRotFromCameraDir() : 3))) : 0);
		}
		return result;
	}

	private Vector3 GetNewBricksPos(Brick newBrick, Vector3 normal, Vector3 point)
	{
		Vector3 a = Brick.ToBrickCoord(normal, point);
		if (normal == Vector3.up)
		{
			return a + normal;
		}
		if (normal == Vector3.down)
		{
			return a + (float)newBrick.vert * normal;
		}
		int num = Mathf.CeilToInt((float)newBrick.horz / 2f);
		return a + (float)num * normal;
	}

	private bool IsDefenseBrick(int seq)
	{
		if (seq == 134 || seq == 135 || seq == 136)
		{
			return true;
		}
		return false;
	}

	private bool CheckNeighborXZ(Vector3 pos)
	{
		Vector3 pos2 = pos;
		pos2.x -= 3f;
		BrickInst byPos = BrickManager.Instance.GetByPos(pos2);
		if (byPos != null && IsDefenseBrick(byPos.Template))
		{
			return true;
		}
		pos2 = pos;
		pos2.x += 3f;
		byPos = BrickManager.Instance.GetByPos(pos2);
		if (byPos != null && IsDefenseBrick(byPos.Template))
		{
			return true;
		}
		pos2 = pos;
		pos2.z -= 3f;
		byPos = BrickManager.Instance.GetByPos(pos2);
		if (byPos != null && IsDefenseBrick(byPos.Template))
		{
			return true;
		}
		pos2 = pos;
		pos2.z += 3f;
		byPos = BrickManager.Instance.GetByPos(pos2);
		if (byPos != null && IsDefenseBrick(byPos.Template))
		{
			return true;
		}
		return false;
	}

	private void DelFireSound()
	{
		AudioSource component = GetComponent<AudioSource>();
		AudioClip fireSound = GetComponent<Weapon>().fireSound;
		if (null != component && null != fireSound)
		{
			component.PlayOneShot(fireSound);
		}
	}

	private void MissFireSound()
	{
		if (!BuildOption.Instance.Props.brickSoundChange)
		{
			AudioSource component = GetComponent<AudioSource>();
			AudioClip dryFireSound = GetComponent<Weapon>().dryFireSound;
			if (null != component && null != dryFireSound)
			{
				component.PlayOneShot(dryFireSound);
			}
		}
	}

	private void FireSound()
	{
		if (!BuildOption.Instance.Props.brickSoundChange)
		{
			AudioSource component = GetComponent<AudioSource>();
			AudioClip fireSound = GetComponent<Weapon>().fireSound;
			if (null != component && null != fireSound)
			{
				component.PlayOneShot(fireSound);
			}
		}
	}

	private void CheckFire()
	{
		if (!GlobalVars.Instance.blockDelBrick && (custom_inputs.Instance.GetButtonDown("K_FIRE1") || custom_inputs.Instance.GetButtonDown("K_FIRE2")) && UserMapInfoManager.Instance.CheckAuth(showMessage: true))
		{
			BrickProperty hitBrickProperty = BrickManager.Instance.GetHitBrickProperty(hitBrick.transform.gameObject, hitBrick.normal, hitBrick.point);
			bool flag = false;
			if (!IsCoolDown() && hitBrickProperty != null)
			{
				Brick brick = BrickManager.Instance.GetBrick(hitBrickProperty.Index);
				if (brick != null && brick.IsEnable(RoomManager.Instance.CurrentRoomType))
				{
					if (brick.function == Brick.FUNCTION.SCRIPT && !copyRight)
					{
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("SCRIPTABLE_DEVELOPER_ONLY"));
					}
					else if (CSNetManager.Instance.Sock != null && !CSNetManager.Instance.Sock.WaitingAck)
					{
						deltaTime = 0f;
						flag = true;
						DelFireSound();
						CreateMuzzleFire();
						P2PManager.Instance.SendPEER_COMPOSE(isDel: true);
						Vector3 position = hitBrickProperty.transform.position;
						List<int> list = new List<int>();
						for (int i = -1; i < 2; i++)
						{
							for (int j = -1; j < 2; j++)
							{
								for (int k = -1; k < 2; k++)
								{
									Vector3 pos = position;
									pos.x += (float)i;
									pos.y += (float)j;
									pos.z += (float)k;
									GameObject brickObjectByPos = BrickManager.Instance.GetBrickObjectByPos(pos);
									if (null != brickObjectByPos)
									{
										BrickProperty component = brickObjectByPos.GetComponent<BrickProperty>();
										if (component != null)
										{
											list.Add(component.Seq);
										}
									}
								}
							}
						}
						P2PManager.Instance.SendPEER_COMPOSE(isDel: false);
						CSNetManager.Instance.Sock.SendCS_BATCH_DEL_BRICK_REQ(list.ToArray());
						base.animation.CrossFade("fire");
						UseAmmo();
					}
				}
			}
			if (!flag)
			{
				MissFireSound();
			}
		}
	}

	private void Update()
	{
		VerifyBattleChat();
		VerifyCamera();
		VerifyLocalController();
		deltaTime += Time.deltaTime;
		if (Screen.lockCursor && BrickManager.Instance.IsLoaded)
		{
			if (!CheckAimed())
			{
				ShowTarget(show: false);
			}
			else
			{
				ShowTarget(show: true);
				CheckBuildGun();
			}
			if (statusMessage.Length > 0)
			{
				statusDelta += Time.deltaTime;
				if (statusDelta > statusMessageLimit)
				{
					statusDelta = 0f;
					statusMessage = string.Empty;
				}
			}
		}
	}

	private bool CanFire()
	{
		return null != localCtrl && localCtrl.CanControl() && curAmmo > 0;
	}

	private void CheckBuildGun()
	{
		if (CanFire())
		{
			CheckFire();
		}
	}
}
