using UnityEngine;

public class SmokeGrenade : HandBomb
{
	private float ammoTime;

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
		if (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.BUNGEE)
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
					throwForce = wpnMod.fThrowForce;
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
						throwForce += value;
					}
					num = 8;
					grade = item.upgradeProps[num].grade;
					if (grade > 0)
					{
						float num2 = persistTime = PimpManager.Instance.getValue((int)tWeapon.upgradeCategory, num, grade - 1);
					}
				}
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
			if (CanThrow() && !(bool)detonating && custom_inputs.Instance.GetButtonDown("K_FIRE1"))
			{
				RemoveSafetyClip();
				localCtrl.DoThrowAnimation(throwAnimation);
				P2PManager.Instance.SendPEER_THROW();
			}
		}
	}

	public override void Throw()
	{
		if ((bool)detonating)
		{
			UseAmmo();
			VoiceManager.Instance.Play("BKGS440_throw_2");
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
				PrjSmoke component2 = gameObject.GetComponent<PrjSmoke>();
				if (null != component2)
				{
					component2.Index = base.CurAmmo;
					component2.DetonatorTime = detonatorTime;
					component2.ExplosionTime = explosionTime;
					component2.PersistTime = persistTime;
				}
				P2PManager.Instance.SendPEER_PROJECTILE(MyInfoManager.Instance.Seq, base.CurAmmo, vector, eulerAngles);
			}
			detonating = false;
			ShowGrenade(body: false, clip: false);
			if (base.CurAmmo <= 0)
			{
				P2PManager.Instance.SendPEER_ENABLE_HANDBOMB(MyInfoManager.Instance.Seq, enable: false);
				if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BUNGEE)
				{
					GameObject gameObject2 = GameObject.Find("Me");
					if (null != gameObject2)
					{
						LocalController component3 = gameObject2.GetComponent<LocalController>();
						if (component3 != null)
						{
							component3.ReturnBuildGun();
						}
					}
				}
			}
		}
	}
}
