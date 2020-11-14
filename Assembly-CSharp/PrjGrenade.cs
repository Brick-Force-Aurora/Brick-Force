using UnityEngine;

public class PrjGrenade : Projectile
{
	private float radius;

	private float atkPow;

	private float rigidity;

	private Weapon.BY weaponBy;

	private Weapon.BY weaponByForChild;

	private int durability;

	private int durabilityMax;

	public float Radius
	{
		set
		{
			radius = value;
		}
	}

	public float AtkPow
	{
		set
		{
			atkPow = value;
		}
	}

	public float Rigidity
	{
		set
		{
			rigidity = value;
		}
	}

	public Weapon.BY WeaponBy
	{
		set
		{
			weaponBy = value;
		}
	}

	public Weapon.BY WeaponByForChild
	{
		set
		{
			weaponByForChild = value;
		}
	}

	public int Durability
	{
		set
		{
			durability = value;
		}
	}

	public int DurabilityMax
	{
		set
		{
			durabilityMax = value;
		}
	}

	private float CalcPowFrom(Vector3 position)
	{
		float num = Vector3.Distance(base.transform.position, position);
		if (num > radius)
		{
			return 0f;
		}
		float num2 = (radius - num) / radius;
		return atkPow * num2;
	}

	private void CheckMyself()
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
				if (Vector3.Distance(position, base.transform.position) < radius)
				{
					int num = 0;
					for (int i = 0; i < 3; i++)
					{
						position.y += 0.3f;
						if (!Physics.Linecast(base.transform.position, position, out RaycastHit _, layerMask))
						{
							int num2 = Mathf.FloorToInt(0.3f * (float)(i + 1) * CalcPowFrom(position));
							if (num2 > 0)
							{
								num += GlobalVars.Instance.applyDurabilityDamage(durability, durabilityMax, num2);
							}
						}
					}
					if (num > 0)
					{
						component.GetHit(MyInfoManager.Instance.Seq, num, 1f, (int)weaponBy, -1, autoHealPossible: true, checkZombie: false);
					}
				}
			}
		}
	}

	private void CheckBoxmen()
	{
		HitPart[] array = ExplosionUtil.CheckBoxmen(base.transform.position, radius, includeFriendly: false);
		for (int i = 0; i < array.Length; i++)
		{
			PlayerProperty[] allComponents = Recursively.GetAllComponents<PlayerProperty>(array[i].transform, includeInactive: false);
			if (allComponents.Length == 1)
			{
				int num = Mathf.FloorToInt(array[i].damageFactor * CalcPowFrom(array[i].transform.position));
				if (num > 0)
				{
					num = GlobalVars.Instance.applyDurabilityDamage(durability, durabilityMax, num);
					allComponents[0].Desc.accumDamaged += num;
				}
			}
		}
		GameObject[] array2 = BrickManManager.Instance.ToGameObjectArray();
		for (int j = 0; j < array2.Length; j++)
		{
			int num2 = (int)weaponBy;
			TPController component = array2[j].GetComponent<TPController>();
			if (component != null && !MyInfoManager.Instance.IsBelow12() && component.IsChild)
			{
				num2 = (int)weaponByForChild;
			}
			PlayerProperty component2 = array2[j].GetComponent<PlayerProperty>();
			if (null != component2 && component2.Desc.accumDamaged > 0)
			{
				P2PManager.Instance.SendPEER_BOMBED(MyInfoManager.Instance.Seq, component2.Desc.Seq, component2.Desc.accumDamaged, rigidity, num2);
				component2.Desc.accumDamaged = 0;
			}
		}
	}

	private void CheckMonster()
	{
		HitPart[] array = ExplosionUtil.CheckMon(base.transform.position, radius, includeFriendly: false);
		for (int i = 0; i < array.Length; i++)
		{
			MonProperty[] allComponents = Recursively.GetAllComponents<MonProperty>(array[i].transform, includeInactive: false);
			if (allComponents.Length == 1 && (MyInfoManager.Instance.Slot >= 4 || !allComponents[i].Desc.bRedTeam) && (MyInfoManager.Instance.Slot < 4 || allComponents[i].Desc.bRedTeam))
			{
				int num = Mathf.FloorToInt(array[i].damageFactor * CalcPowFrom(array[i].transform.position));
				if (num > 0)
				{
					MonManager.Instance.Hit(allComponents[0].Desc.Seq, num, 1f, (int)weaponBy, Vector3.zero, Vector3.zero, -1);
				}
			}
		}
	}

	private void CheckDestructibles()
	{
		BrickProperty[] array = ExplosionUtil.CheckDestructibles(base.transform.position, radius);
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

	private void Start()
	{
		if (BuildOption.Instance.Props.useUskWeaponTex)
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
	}

	private void Kaboom()
	{
		if (!BuildOption.Instance.IsNetmarble && !BuildOption.Instance.IsDeveloper)
		{
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
		}
		else if (explosion != null)
		{
			Object.Instantiate((Object)explosion, base.transform.position, Quaternion.Euler(0f, 0f, 0f));
		}
		P2PManager.Instance.SendPEER_PROJECTILE_KABOOM(MyInfoManager.Instance.Seq, base.Index);
		if (null != projectileAlert)
		{
			projectileAlert.RemoveMine(base.Index);
		}
		CheckMyself();
		CheckBoxmen();
		CheckMonster();
		CheckDestructibles();
		Object.Destroy(base.transform.gameObject);
	}

	private void Update()
	{
		Rigidbody component = GetComponent<Rigidbody>();
		if (!(null != component) || !component.isKinematic)
		{
			deltaTime += Time.deltaTime;
			base.DetonatorTime += Time.deltaTime;
			if (base.DetonatorTime > explosionTime)
			{
				Kaboom();
			}
			else if (deltaTime > BuildOption.Instance.Props.SendRate)
			{
				deltaTime = 0f;
				P2PManager.Instance.SendPEER_PROJECTILE_FLY(MyInfoManager.Instance.Seq, base.Index, base.transform.position, base.transform.rotation.eulerAngles, radius);
				if (null != projectileAlert)
				{
					projectileAlert.TrackMine(base.Index, weaponBy, base.transform.position, radius);
				}
			}
		}
	}

	private void FixedUpdate()
	{
		Rigidbody component = GetComponent<Rigidbody>();
		if (component != null)
		{
			component.AddTorque(Vector3.right * 1000f, ForceMode.Acceleration);
		}
	}
}
