using _Emulator;
using UnityEngine;

public class SenseBomb : HandBomb
{
	public float Rigidity = 0.5f;

	public float Radius = 5f;

	public float AtkPow = 100f;

	private bool isUnlock = true;

	private float ammoTime;

	private GameObject installingEff;

	private bool installing;

	private float dtWaitBeam;

	private float maxWaitBeam = 2f;

	public Texture2D xmark;

	private bool isBeamTest;

	private Ray BeamRay;

	private float rayRange = 2f;

	private GameObject beamObj;

	private GameObject bombObj;

	private Vector3 vBomb;

	private Vector3 vBombNormal;

	private bool realExplosion;

	private void Start()
	{
		if (BuildOption.Instance.Props.useUskWeaponTex && applyUsk)
		{
			MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				if (meshRenderer.material.mainTexture != null && UskManager.Instance.Get(meshRenderer.material.mainTexture.name) != null)
				{
					meshRenderer.material.mainTexture = UskManager.Instance.Get(meshRenderer.material.mainTexture.name);
				}
			}
		}
		base.transform.localRotation = Quaternion.Euler(90f, 270f, 0f);
		Modify();
		UpgradeMaxAmmo();
		Reset();
		detonatorTime = 0f;
		detonating = false;
		throwing = false;
		dtWaitBeam = 0f;
	}

	private void Modify()
	{
		WeaponFunction component = GetComponent<WeaponFunction>();
		if (null != component)
		{
			WpnMod wpnMod = WeaponModifier.Instance.Get((int)component.weaponBy);
			if (wpnMod != null)
			{
				maxAmmo = wpnMod.maxAmmo;
				explosionTime = wpnMod.explosionTime;
				speedFactor = wpnMod.fSpeedFactor;
				AtkPow = wpnMod.fAtkPow;
				Rigidity = wpnMod.fRigidity;
				throwForce = wpnMod.fThrowForce;
				Radius = wpnMod.radius;
			}
			WpnModEx ex = WeaponModifier.Instance.GetEx((int)component.weaponBy);
			if (ex != null)
			{
				persistTime = ex.persistTime;
			}
			TWeapon tWeapon = (TWeapon)GetComponent<Weapon>().tItem;
			Item item = MyInfoManager.Instance.GetItemBySequence(component.ItemSeq);
			if (item == null)
			{
				item = MyInfoManager.Instance.GetUsingEquipByCode(tWeapon.code);
			}
			if (item != null)
			{
				int num = 0;
				int grade = item.upgradeProps[num].grade;
				if (grade > 0)
				{
					float value = PimpManager.Instance.getValue((int)tWeapon.upgradeCategory, num, grade - 1);
					AtkPow += value;
				}
				num = 6;
				grade = item.upgradeProps[num].grade;
				if (grade > 0)
				{
					float value2 = PimpManager.Instance.getValue((int)tWeapon.upgradeCategory, num, grade - 1);
					throwForce += value2;
				}
				num = 7;
				grade = item.upgradeProps[num].grade;
				if (grade > 0)
				{
					float value3 = PimpManager.Instance.getValue((int)tWeapon.upgradeCategory, num, grade - 1);
					Radius += value3;
				}
			}
		}
	}

	protected void DrawSenseBombCrossHair()
	{
		if (Screen.lockCursor)
		{
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
			if (isUnlock && base.CurAmmo > 0)
			{
				GUI.color = new Color(1f, 1f, 1f, 0.5f);
				Vector2 vector = new Vector2((float)((Screen.width - 64) / 2), (float)((Screen.height - 64) / 2));
				TextureUtil.DrawTexture(new Rect(vector.x, vector.y, 64f, 64f), xmark, ScaleMode.StretchToFill, alphaBlend: true);
			}
			GUI.color = color;
		}
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn)
		{
			GUISkin skin = GUI.skin;
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			DrawSenseBombCrossHair();
			DrawAmmo();
			GUI.enabled = true;
			GUI.skin = skin;
		}
	}

	public void collideTest()
	{
		activeBeam();
		if (isBeamTest)
		{
			int layerMask = (1 << LayerMask.NameToLayer("BoxMan")) | (1 << LayerMask.NameToLayer("Mon"));
			realExplosion = false;
			if (Physics.Raycast(BeamRay, out RaycastHit hitInfo, rayRange, layerMask))
			{
				GameObject gameObject = hitInfo.transform.gameObject;
				if (gameObject.layer == LayerMask.NameToLayer("BoxMan"))
				{
					PlayerProperty[] allComponents = Recursively.GetAllComponents<PlayerProperty>(gameObject.transform, includeInactive: false);
					PlayerProperty playerProperty = null;
					if (allComponents.Length != 1)
					{
						Debug.LogError("PlayerProperty should be unique for a box man, but it has multiple PlayerProperty components or non ");
					}
					if (allComponents.Length > 0)
					{
						playerProperty = allComponents[0];
					}
					if (playerProperty != null && !playerProperty.IsHostile())
					{
						return;
					}
				}
				CheckBoxmen(vBomb);
				CheckMonster(vBomb);
				if (realExplosion)
				{
					CheckDestructibles(vBomb);
					if (!BuildOption.Instance.Props.useUskMuzzleEff || !applyUsk)
					{
						if (explosion != null)
						{
							Object.Instantiate((Object)explosion, vBomb, Quaternion.Euler(0f, 0f, 0f));
						}
					}
					else if (GlobalVars.Instance.explosionUsk != null)
					{
						Object.Instantiate((Object)GlobalVars.Instance.explosionUsk, vBomb, Quaternion.Euler(0f, 0f, 0f));
					}
					detonating = false;
					isBeamTest = false;
					throwing = false;
					Object.DestroyImmediate(beamObj);
					Object.DestroyImmediate(bombObj);
					CSNetManager.Instance.Sock.SendCS_GADGET_ACTION_REQ(MyInfoManager.Instance.SenseBombSeq, -1);
				}
			}
		}
	}

	private void Update()
	{
		if (BrickManager.Instance.IsLoaded)
		{
			NoCheat.Instance.KillCheater(NoCheat.WATCH_DOG.SPECIAL_AMMO, base.CurAmmo);
			ammoTime += Time.deltaTime;
			if (ammoTime > 1f)
			{
				ammoTime = 0f;
				if (!MyInfoManager.Instance.IsSpectator)
				{
					P2PManager.Instance.SendPEER_ENABLE_HANDBOMB(MyInfoManager.Instance.Seq, base.CurAmmo > 0);
				}
			}
			VerifyCamera();
			VerifyLocalController();
			UpdateCrossEffect();
			if (CanThrow())
			{
				if (!(bool)detonating && custom_inputs.Instance.GetButtonDown("K_FIRE1"))
				{
					RemoveSafetyClip();
				}
				if ((bool)detonating && custom_inputs.Instance.GetButtonUp("K_FIRE1"))
				{
					localCtrl.DoThrowAnimation(throwAnimation);
					P2PManager.Instance.SendPEER_THROW();
				}
			}
			updateCrossHair();
		}
	}

	private float CalcAtkPow()
	{
		float atkPow = AtkPow;
		return atkPow + WantedManager.Instance.GetWantedAtkPowBoost(MyInfoManager.Instance.Seq, atkPow);
	}

	private float CalcPowFrom(Vector3 position)
	{
		float num = Vector3.Distance(base.transform.position, position);
		if (num > Radius)
		{
			return 0f;
		}
		float num2 = (Radius - num) / Radius;
		return CalcAtkPow() * num2;
	}

	private float CalcPowFrom(Vector3 BombPos, Vector3 position)
	{
		float num = Vector3.Distance(BombPos, position);
		if (num > Radius)
		{
			return 0f;
		}
		float num2 = (Radius - num) / Radius;
		return CalcAtkPow() * num2;
	}

	private void CheckMyself(Vector3 boomPos, float DamageRadius)
	{
		int layerMask = (1 << LayerMask.NameToLayer("Chunk")) | (1 << LayerMask.NameToLayer("Brick")) | (1 << LayerMask.NameToLayer("BoxMan")) | (1 << LayerMask.NameToLayer("InvincibleArmor")) | (1 << LayerMask.NameToLayer("Bomb")) | (1 << LayerMask.NameToLayer("InstalledBomb"));
		GameObject gameObject = GameObject.Find("Me");
		if (null == gameObject)
		{
			Debug.LogError("Fail to find Me");
		}
		else
		{
			LocalController component = gameObject.GetComponent<LocalController>();
			if (null == component)
			{
				Debug.LogError("Fail to get LocalController component for Me");
			}
			else
			{
				Vector3 position = gameObject.transform.position;
				if (Vector3.Distance(position, boomPos) < DamageRadius)
				{
					int num = 0;
					for (int i = 0; i < 3; i++)
					{
						position.y += 0.3f;
						if (!Physics.Linecast(base.transform.position, position, out RaycastHit _, layerMask))
						{
							int num2 = Mathf.FloorToInt(0.3f * (float)(i + 1) * CalcPowFrom(boomPos, position));
							if (num2 > 0)
							{
								num += num2;
							}
						}
					}
					if (num > 0)
					{
						component.GetHit(MyInfoManager.Instance.Seq, num, 1f, (int)weaponBy, -1, autoHealPossible: true, checkZombie: false);
						realExplosion = true;
					}
				}
			}
		}
	}

	private void CheckBoxmen(Vector3 boomPos)
	{
		HitPart[] array = ExplosionUtil.CheckBoxmen(boomPos, Radius, includeFriendly: false);
		for (int i = 0; i < array.Length; i++)
		{
			PlayerProperty[] allComponents = Recursively.GetAllComponents<PlayerProperty>(array[i].transform, includeInactive: false);
			if (allComponents.Length == 1)
			{
				int num = Mathf.FloorToInt(array[i].damageFactor * CalcPowFrom(boomPos, array[i].transform.position));
				if (num > 0)
				{
					WeaponFunction component = GetComponent<WeaponFunction>();
					if (null == component)
					{
						Debug.LogError("wpnFunc == null");
					}
					TWeapon tWeapon = (TWeapon)GetComponent<Weapon>().tItem;
					if (tWeapon == null)
					{
						Debug.LogError("wpn == null");
					}
					Item usingEquipByCode = MyInfoManager.Instance.GetUsingEquipByCode(tWeapon.code);
					num = GlobalVars.Instance.applyDurabilityDamage(usingEquipByCode?.Durability ?? (-1), tWeapon.durabilityMax, num);
					allComponents[0].Desc.accumDamaged += num;
				}
			}
		}
		GameObject[] array2 = BrickManManager.Instance.ToGameObjectArray();
		for (int j = 0; j < array2.Length; j++)
		{
			PlayerProperty component2 = array2[j].GetComponent<PlayerProperty>();
			if (null != component2 && component2.Desc.accumDamaged > 0)
			{
				P2PManager.Instance.SendPEER_BOMBED(MyInfoManager.Instance.Seq, component2.Desc.Seq, component2.Desc.accumDamaged, Rigidity, (int)weaponBy);
				component2.Desc.accumDamaged = 0;
				realExplosion = true;
			}
		}
	}

	private void CheckMonster(Vector3 boomPos)
	{
		HitPart[] array = ExplosionUtil.CheckMon(base.transform.position, Radius, includeFriendly: false);
		for (int i = 0; i < array.Length; i++)
		{
			MonProperty[] allComponents = Recursively.GetAllComponents<MonProperty>(array[i].transform, includeInactive: false);
			if (allComponents.Length == 1 && (MyInfoManager.Instance.Slot >= 4 || !allComponents[i].Desc.bRedTeam) && (MyInfoManager.Instance.Slot < 4 || allComponents[i].Desc.bRedTeam))
			{
				int num = Mathf.FloorToInt(array[i].damageFactor * CalcPowFrom(boomPos, array[i].transform.position));
				num += AddUpgradedDamagei(category);
				if (num > 0)
				{
					MonManager.Instance.Hit(allComponents[0].Desc.Seq, num, 1f, (int)weaponBy, Vector3.zero, Vector3.zero, -1);
					realExplosion = true;
				}
			}
		}
	}

	private void CheckDestructibles(Vector3 boomPos)
	{
		BrickProperty[] array = ExplosionUtil.CheckDestructibles(base.transform.position, Radius);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Hit((int)CalcPowFrom(boomPos, array[i].transform.position));
			if (array[i].HitPoint <= 0)
			{
				CSNetManager.Instance.Sock.SendCS_DESTROY_BRICK_REQ(array[i].Seq);
			}
			else
			{
				P2PManager.Instance.SendPEER_BRICK_HITPOINT(array[i].Seq, array[i].HitPoint);
			}
		}
	}

	public void SelfKaboom()
	{
		if (isBeamTest)
		{
			if (!BuildOption.Instance.Props.useUskMuzzleEff || !applyUsk)
			{
				if (explosion != null)
				{
					Object.Instantiate((Object)explosion, vBomb, Quaternion.Euler(0f, 0f, 0f));
				}
			}
			else if (GlobalVars.Instance.explosionUsk != null)
			{
				Object.Instantiate((Object)GlobalVars.Instance.explosionUsk, vBomb, Quaternion.Euler(0f, 0f, 0f));
			}
			detonating = false;
			isBeamTest = false;
			throwing = false;
			Object.DestroyImmediate(beamObj);
			Object.DestroyImmediate(bombObj);
		}
	}

	private void activeBeam()
	{
		if (installing)
		{
			dtWaitBeam += Time.deltaTime;
			if (dtWaitBeam > maxWaitBeam)
			{
				Object.DestroyImmediate(installingEff);
				beamObj = (Object.Instantiate((Object)((!IsRed()) ? GlobalVars.Instance.SenseBeam2 : GlobalVars.Instance.SenseBeam), vBomb, Quaternion.Euler(0f, 0f, 0f)) as GameObject);
				beamObj.transform.up = vBombNormal;
				beamObj.transform.localScale = new Vector3(1f, 2f, 1f);
				isBeamTest = true;
				installing = false;
			}
		}
	}

	protected void FireSound()
	{
		GetComponent<Weapon>().FireSound();
	}

	protected void GadgetSound(Weapon.GADGET gadget)
	{
		GetComponent<Weapon>().GadgetSound(gadget);
	}

	private void updateCrossHair()
	{
		int layerMask = (1 << LayerMask.NameToLayer("InstalledBomb")) | (1 << LayerMask.NameToLayer("Brick")) | (1 << LayerMask.NameToLayer("Chunk"));
		Ray ray = cam.ScreenPointToRay(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f));
		if (Physics.Raycast(ray, out RaycastHit _, Radius, layerMask))
		{
			isUnlock = false;
		}
		else
		{
			isUnlock = true;
		}
	}

	private bool IsRed()
	{
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.MISSION)
		{
			if (MyInfoManager.Instance.Slot < 4)
			{
				return true;
			}
			return false;
		}
		if (MyInfoManager.Instance.Slot < 8)
		{
			return true;
		}
		return false;
	}

	public override void Throw()
	{
		if ((bool)detonating)
		{
			if (isBeamTest)
			{
				CSNetManager.Instance.Sock.SendCS_GADGET_ACTION_REQ(MyInfoManager.Instance.SenseBombSeq, -1);
				if (!BuildOption.Instance.Props.useUskMuzzleEff || !applyUsk)
				{
					if (explosion != null)
					{
						Object.Instantiate((Object)explosion, vBomb, Quaternion.Euler(0f, 0f, 0f));
					}
				}
				else if (GlobalVars.Instance.explosionUsk != null)
				{
					Object.Instantiate((Object)GlobalVars.Instance.explosionUsk, vBomb, Quaternion.Euler(0f, 0f, 0f));
				}
				detonating = false;
				isBeamTest = false;
				throwing = false;
				Object.Destroy(beamObj);
				Object.Destroy(bombObj);
			}
			int layerMask = (1 << LayerMask.NameToLayer("Brick")) | (1 << LayerMask.NameToLayer("Chunk"));
			Ray ray = cam.ScreenPointToRay(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f));
			if (Physics.Raycast(ray, out RaycastHit hitInfo, Radius, layerMask))
			{
				GameObject gameObject = hitInfo.transform.gameObject;
				BrickProperty brickProperty = null;
				GameObject brickObjectByPos = BrickManager.Instance.GetBrickObjectByPos(Brick.ToBrickCoord(hitInfo.normal, hitInfo.point));
				if (null != brickObjectByPos)
				{
					brickProperty = brickObjectByPos.GetComponent<BrickProperty>();
				}
				if (null != brickProperty && brickProperty.Index != 104 && brickProperty.Index != 105)
				{
					UseAmmo();
					GameObject gameObject2 = GameObject.Find("Me");
					if (gameObject2 != null)
					{
						LocalController component = gameObject2.GetComponent<LocalController>();
						if (component != null)
						{
							component.IDidSomething();
						}
					}
					if (gameObject.layer == LayerMask.NameToLayer("Brick") || gameObject.layer == LayerMask.NameToLayer("Chunk"))
					{
						bombObj = (Object.Instantiate((Object)GlobalVars.Instance.SenseBomb, hitInfo.point, Quaternion.Euler(0f, 0f, 0f)) as GameObject);
						bombObj.transform.up = hitInfo.normal;
						BeamRay = new Ray(hitInfo.point, hitInfo.normal);
						vBomb = hitInfo.point;
						vBombNormal = hitInfo.normal;
						installing = true;
						installingEff = (Object.Instantiate((Object)GlobalVars.Instance.installingEff, hitInfo.point + vBombNormal * 0.14f, Quaternion.Euler(0f, 0f, 0f)) as GameObject);
						int num = MyInfoManager.Instance.Slot * 10 + base.CurAmmo;
						SockTcp sock = CSNetManager.Instance.Sock;
						int gadget = num;
						Vector3 point = hitInfo.point;
						float x = point.x;
						Vector3 point2 = hitInfo.point;
						float y = point2.y;
						Vector3 point3 = hitInfo.point;
						float z = point3.z;
						Vector3 normal = hitInfo.normal;
						float x2 = normal.x;
						Vector3 normal2 = hitInfo.normal;
						float y2 = normal2.y;
						Vector3 normal3 = hitInfo.normal;
						sock.SendCS_INSTALL_GADGET_REQ(gadget, x, y, z, x2, y2, normal3.z);
					}
					dtWaitBeam = 0f;
					detonating = false;
					ShowGrenade(body: false, clip: false);
					FireSound();
					if (base.CurAmmo <= 0)
					{
						P2PManager.Instance.SendPEER_ENABLE_HANDBOMB(MyInfoManager.Instance.Seq, enable: false);
					}
					isUnlock = false;
				}
			}
			else
			{
				isUnlock = true;
			}
		}
	}

	private void OnDisable()
	{
		if (ZombieVsHumanManager.Instance.IsZombie(MyInfoManager.Instance.Seq))
		{
			if (!BuildOption.Instance.Props.useUskMuzzleEff || !applyUsk)
			{
				if (explosion != null)
				{
					Object.Instantiate((Object)explosion, vBomb, Quaternion.Euler(0f, 0f, 0f));
				}
			}
			else if (GlobalVars.Instance.explosionUsk != null)
			{
				Object.Instantiate((Object)GlobalVars.Instance.explosionUsk, vBomb, Quaternion.Euler(0f, 0f, 0f));
			}
			detonating = false;
			isBeamTest = false;
			throwing = false;
			Object.DestroyImmediate(beamObj);
			Object.DestroyImmediate(bombObj);
			CSNetManager.Instance.Sock.SendCS_GADGET_ACTION_REQ(MyInfoManager.Instance.SenseBombSeq, -1);
		}
	}
}
