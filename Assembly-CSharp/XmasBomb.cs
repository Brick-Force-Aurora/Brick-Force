using _Emulator;
using UnityEngine;

public class XmasBomb : HandBomb
{
	public GameObject smoke;

	public float Rigidity = 0.5f;

	public float Radius = 5f;

	public float AtkPow = 100f;

	private float ammoTime;

	public float _AtkPow
	{
		get
		{
			return AtkPow;
		}
		set
		{
			AtkPow = value;
		}
	}

	public float _Rigidity
	{
		get
		{
			return Rigidity;
		}
		set
		{
			Rigidity = value;
		}
	}

	public float _Radius
	{
		get
		{
			return Radius;
		}
		set
		{
			Radius = value;
		}
	}

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
		Modify();
		UpgradeMaxAmmo();
		Reset();
		detonatorTime = 0f;
		detonating = false;
		throwing = false;
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

	private void DrawCrossHairXmasBomb()
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
				Vector2 vector = new Vector2((float)((Screen.width - 8) / 2), (float)(Screen.height / 2 - 28));
				TextureUtil.DrawTexture(new Rect(vector.x, vector.y, 8f, 8f), vCrossHair, ScaleMode.StretchToFill, alphaBlend: true);
				vector = new Vector2((float)((Screen.width - 8) / 2), (float)(Screen.height / 2 - 20));
				TextureUtil.DrawTexture(new Rect(vector.x, vector.y, 8f, 8f), vCrossHair, ScaleMode.StretchToFill, alphaBlend: true);
			}
			if (null != hCrossHair)
			{
				Vector2 vector = new Vector2((float)(Screen.width / 2 - 8), (float)((Screen.height - 8) / 2 - 20));
				TextureUtil.DrawTexture(new Rect(vector.x, vector.y, 8f, 8f), hCrossHair, ScaleMode.StretchToFill, alphaBlend: true);
				vector = new Vector2((float)(Screen.width / 2), (float)((Screen.height - 8) / 2 - 20));
				TextureUtil.DrawTexture(new Rect(vector.x, vector.y, 8f, 8f), hCrossHair, ScaleMode.StretchToFill, alphaBlend: true);
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
			DrawCrossHairXmasBomb();
			DrawAmmo();
			DrawDetonating();
			GUI.enabled = true;
			GUI.skin = skin;
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
			if ((bool)detonating)
			{
				detonatorTime += Time.deltaTime;
				if (detonatorTime > explosionTime)
				{
					UseAmmo();
					if (!BuildOption.Instance.Props.useUskMuzzleEff || !base.ApplyUsk)
					{
						if (explosion != null)
						{
							Object.Instantiate((Object)explosion, base.transform.position, Quaternion.Euler(0f, 0f, 0f));
						}
					}
					else if (GlobalVars.Instance.explosionUsk != null)
					{
						Object.Instantiate((Object)GlobalVars.Instance.explosionUsk, base.transform.position, Quaternion.Euler(0f, 0f, 0f));
					}
					if (smoke != null)
					{
						Object.Instantiate((Object)smoke, base.transform.position, Quaternion.Euler(0f, 0f, 0f));
					}
					P2PManager.Instance.SendPEER_PROJECTILE_KABOOM(MyInfoManager.Instance.Seq, -1);
					if (localCtrl.IsUserDamaged())
					{
						localCtrl.GetHit(MyInfoManager.Instance.Seq, 1000, 1f, -10, -1, autoHealPossible: true, checkZombie: false);
					}
					else
					{
						localCtrl.GetHit(MyInfoManager.Instance.Seq, 1000, 1f, (int)weaponBy, -1, autoHealPossible: true, checkZombie: false);
					}
					CheckBoxmen();
					CheckMonster();
					CheckDestructibles();
					Restart();
				}
			}
		}
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

	private void CheckBoxmen()
	{
		HitPart[] array = ExplosionUtil.CheckBoxmen(base.transform.position, Radius, includeFriendly: false);
		for (int i = 0; i < array.Length; i++)
		{
			PlayerProperty[] allComponents = Recursively.GetAllComponents<PlayerProperty>(array[i].transform, includeInactive: false);
			if (allComponents.Length == 1)
			{
				int num = Mathf.FloorToInt(array[i].damageFactor * CalcPowFrom(array[i].transform.position));
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
					Item item = MyInfoManager.Instance.GetItemBySequence(component.ItemSeq);
					if (item == null)
					{
						item = MyInfoManager.Instance.GetUsingEquipByCode(tWeapon.code);
					}
					num = GlobalVars.Instance.applyDurabilityDamage(item?.Durability ?? (-1), tWeapon.durabilityMax, num);
					P2PManager.Instance.SendPEER_BOMBED(MyInfoManager.Instance.Seq, allComponents[0].Desc.Seq, num, Rigidity, (int)weaponBy);
				}
			}
		}
	}

	private void CheckMonster()
	{
		HitPart[] array = ExplosionUtil.CheckMon(base.transform.position, Radius, includeFriendly: false);
		for (int i = 0; i < array.Length; i++)
		{
			MonProperty[] allComponents = Recursively.GetAllComponents<MonProperty>(array[i].transform, includeInactive: false);
			if (allComponents.Length == 1 && (MyInfoManager.Instance.Slot >= 4 || !allComponents[i].Desc.bRedTeam) && (MyInfoManager.Instance.Slot < 4 || allComponents[i].Desc.bRedTeam))
			{
				int num = Mathf.FloorToInt(array[i].damageFactor * CalcPowFrom(array[i].transform.position));
				num += AddUpgradedDamagei(category);
				if (num > 0)
				{
					MonManager.Instance.Hit(allComponents[0].Desc.Seq, num, 1f, (int)weaponBy, Vector3.zero, Vector3.zero, -1);
				}
			}
		}
	}

	private void CheckDestructibles()
	{
		BrickProperty[] array = ExplosionUtil.CheckDestructibles(base.transform.position, Radius);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Hit((int)CalcPowFrom(array[i].transform.position));
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

	private float CalcAtkPow()
	{
		float atkPow = AtkPow;
		return atkPow + WantedManager.Instance.GetWantedAtkPowBoost(MyInfoManager.Instance.Seq, atkPow);
	}

	public override void Throw()
	{
		if ((bool)detonating)
		{
			UseAmmo();
			GameObject gameObject = GameObject.Find("Me");
			if (gameObject != null)
			{
				LocalController component = gameObject.GetComponent<LocalController>();
				if (component != null)
				{
					component.IDidSomething();
				}
			}
			VoiceManager.Instance.Play("BKG400_throw_5");
			Ray ray = cam.ScreenPointToRay(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f));
			Vector3 vector = ray.origin + Vector3.up * 0.5f;
			Vector3 eulerAngles = base.transform.rotation.eulerAngles;
			GameObject gameObject2 = Object.Instantiate((Object)GetComponent<Weapon>().BulletOrBody, vector, base.transform.rotation) as GameObject;
			if (null != gameObject2)
			{
				Rigidbody component2 = gameObject2.GetComponent<Rigidbody>();
				if (null != component2)
				{
					component2.AddForce(ray.direction * throwForce, ForceMode.Impulse);
				}
				PrjXmasBomb component3 = gameObject2.GetComponent<PrjXmasBomb>();
				if (null != component3)
				{
					WeaponFunction component4 = GetComponent<WeaponFunction>();
					if (null == component4)
					{
						Debug.LogError("wpnFunc == null");
					}
					TWeapon tWeapon = (TWeapon)GetComponent<Weapon>().tItem;
					if (tWeapon == null)
					{
						Debug.LogError("wpn == null");
					}
					Item item = MyInfoManager.Instance.GetItemBySequence(component4.ItemSeq);
					if (item == null)
					{
						item = MyInfoManager.Instance.GetUsingEquipByCode(tWeapon.code);
					}
					component3.Index = base.CurAmmo;
					component3.AtkPow = CalcAtkPow();
					component3.Rigidity = Rigidity;
					component3.WeaponBy = weaponBy;
					component3.WeaponByForChild = weaponByForChild;
					component3.Radius = Radius;
					component3.Durability = (item?.Durability ?? (-1));
					component3.DurabilityMax = tWeapon.durabilityMax;
					component3.ApplyUsk = applyUsk;
					component3.DetonatorTime = detonatorTime;
					component3.ExplosionTime = explosionTime;
				}
				P2PManager.Instance.SendPEER_PROJECTILE(MyInfoManager.Instance.Seq, base.CurAmmo, vector, eulerAngles);
			}
			detonating = false;
			ShowGrenade(body: false, clip: false);
			if (base.CurAmmo <= 0)
			{
				P2PManager.Instance.SendPEER_ENABLE_HANDBOMB(MyInfoManager.Instance.Seq, enable: false);
			}
		}
	}
}
