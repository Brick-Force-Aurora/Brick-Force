using UnityEngine;

public class FlashBang : HandBomb
{
	private float ammoTime;

	public float radius;

	public float continueTime;

	private void Start()
	{
		Modify();
		UpgradeMaxAmmo();
		Reset();
		detonatorTime = 0f;
		detonating = false;
		throwing = false;
	}

	private void Modify()
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
		WeaponFunction component = GetComponent<WeaponFunction>();
		if (null != component)
		{
			WpnMod wpnMod = WeaponModifier.Instance.Get((int)component.weaponBy);
			if (wpnMod != null)
			{
				maxAmmo = wpnMod.maxAmmo;
				explosionTime = wpnMod.explosionTime;
				speedFactor = wpnMod.fSpeedFactor;
				throwForce = wpnMod.fThrowForce;
				radius = wpnMod.radius;
			}
		}
		WpnModEx ex = WeaponModifier.Instance.GetEx((int)component.weaponBy);
		if (ex != null)
		{
			persistTime = ex.persistTime;
			continueTime = ex.continueTime;
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
				throwForce += value;
			}
			num = 7;
			grade = item.upgradeProps[num].grade;
			if (grade > 0)
			{
				radius = PimpManager.Instance.getValue((int)tWeapon.upgradeCategory, num, grade - 1);
				GlobalVars.Instance.pimpFBRadius = PimpManager.Instance.getValue((int)tWeapon.upgradeCategory, num, grade - 1);
			}
			num = 8;
			grade = item.upgradeProps[num].grade;
			if (grade > 0)
			{
				continueTime = PimpManager.Instance.getValue((int)tWeapon.upgradeCategory, num, grade - 1);
				GlobalVars.Instance.pimpFBContinueTime = PimpManager.Instance.getValue((int)tWeapon.upgradeCategory, num, grade - 1);
			}
		}
	}

	private void OnGUI()
	{
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		GUI.depth = (int)guiDepth;
		GUI.enabled = !DialogManager.Instance.IsModal;
		DrawCrossHair();
		DrawAmmo();
		DrawDetonating();
		GUI.enabled = true;
		GUI.skin = skin;
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
				P2PManager.Instance.SendPEER_ENABLE_HANDBOMB(MyInfoManager.Instance.Seq, base.CurAmmo > 0);
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
					GameObject gameObject = GameObject.Find("Me");
					if (gameObject != null)
					{
						LocalController component = gameObject.GetComponent<LocalController>();
						if (component != null)
						{
							component.IDidSomething();
						}
					}
					if (explosion != null)
					{
						Object.Instantiate((Object)explosion, base.transform.position, Quaternion.Euler(0f, 0f, 0f));
					}
					GlobalVars.Instance.SwitchFlashbang(bVis: true, base.transform.position);
					P2PManager.Instance.SendPEER_PROJECTILE_KABOOM(MyInfoManager.Instance.Seq, -2);
					Restart();
				}
			}
		}
	}

	public override void Throw()
	{
		if ((bool)detonating)
		{
			UseAmmo();
			VoiceManager.Instance.Play("KG409_throw_3");
			Ray ray = cam.ScreenPointToRay(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f));
			Vector3 vector = ray.origin + Vector3.up * 0.5f;
			Vector3 eulerAngles = base.transform.rotation.eulerAngles;
			GameObject gameObject = Object.Instantiate((Object)GetComponent<Weapon>().BulletOrBody, vector, base.transform.rotation) as GameObject;
			if (null != gameObject)
			{
				Rigidbody component = gameObject.GetComponent<Rigidbody>();
				if (null != component)
				{
					component.AddForce(ray.direction * throwForce, ForceMode.Impulse);
				}
				PrjFlashBang component2 = gameObject.GetComponent<PrjFlashBang>();
				if (null != component2)
				{
					component2.weaponBY = weaponBy;
					component2.Index = base.CurAmmo;
					component2.DetonatorTime = detonatorTime;
					component2.ExplosionTime = explosionTime;
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
