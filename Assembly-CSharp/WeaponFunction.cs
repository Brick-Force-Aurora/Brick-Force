using UnityEngine;

[RequireComponent(typeof(Weapon))]
public class WeaponFunction : MonoBehaviour
{
	private long itemSeq = -1L;

	protected Camera cam;

	protected CameraController camCtrl;

	protected Camera fpCam;

	protected LocalController localCtrl;

	private bool isFirstPerson = true;

	public bool applyUsk = true;

	public int category;

	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	public float speedFactor = 1f;

	public Vector3 firstPersonOffset = new Vector3(0f, -1.36f, 0.2f);

	public Vector3 secondPersonOffset = new Vector3(-0.6f, -1.36f, 0.2f);

	public Weapon.BY weaponBy = Weapon.BY.M16;

	public Weapon.BY weaponByForChild = Weapon.BY.M16;

	public Texture2D ammoBg;

	protected float crossEffectTime;

	public Transform transformFever;

	public GameObject objFever;

	public bool feverAutoFire1Click;

	public bool feverAutoFire2Click;

	public long ItemSeq
	{
		get
		{
			return itemSeq;
		}
		set
		{
			itemSeq = value;
		}
	}

	public bool IsFirstPerson
	{
		get
		{
			return isFirstPerson;
		}
		set
		{
			isFirstPerson = value;
		}
	}

	public bool ApplyUsk
	{
		get
		{
			return applyUsk;
		}
		set
		{
			applyUsk = value;
		}
	}

	public int Category
	{
		get
		{
			return category;
		}
		set
		{
			category = value;
		}
	}

	public float SpeedFactor
	{
		get
		{
			return speedFactor;
		}
		set
		{
			speedFactor = value;
		}
	}

	public Vector3 FirstPersonOffset
	{
		get
		{
			return firstPersonOffset;
		}
		set
		{
			firstPersonOffset = value;
		}
	}

	public Vector3 SecondPersonOffset
	{
		get
		{
			return secondPersonOffset;
		}
		set
		{
			secondPersonOffset = value;
		}
	}

	public Weapon.BY WeaponBy
	{
		get
		{
			return weaponBy;
		}
		set
		{
			weaponBy = value;
		}
	}

	public Texture2D AmmoBg
	{
		get
		{
			return ammoBg;
		}
		set
		{
			ammoBg = value;
		}
	}

	protected bool VerifyCamera()
	{
		if (null == cam)
		{
			GameObject gameObject = GameObject.Find("Main Camera");
			if (null != gameObject)
			{
				cam = gameObject.GetComponent<Camera>();
				camCtrl = gameObject.GetComponent<CameraController>();
			}
			GameObject gameObject2 = GameObject.Find("First Person Camera");
			if (null != gameObject2)
			{
				fpCam = gameObject2.GetComponent<Camera>();
			}
		}
		return cam != null && fpCam != null && camCtrl != null;
	}

	protected bool VerifyLocalController()
	{
		if (null == localCtrl)
		{
			LocalController[] allComponents = Recursively.GetAllComponents<LocalController>(base.transform, includeInactive: false);
			if (allComponents.Length != 1)
			{
				Debug.LogError("Local controller should be unique for fps object, but it has multiple local controllers or no local controllers");
			}
			if (allComponents.Length > 0)
			{
				localCtrl = allComponents[0];
			}
		}
		return localCtrl != null;
	}

	public virtual void SetDrawn(bool draw)
	{
	}

	public virtual void Reset(bool bDefenseRespwan = false)
	{
	}

	public virtual void setFever(bool isOn)
	{
		if (isOn)
		{
			if (objFever != null)
			{
				Object.Destroy(objFever);
				objFever = null;
			}
			Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
			int num = 0;
			while (transformFever == null && num < componentsInChildren.Length)
			{
				if (componentsInChildren[num].name.Contains("Dummy_fire_effect"))
				{
					transformFever = componentsInChildren[num];
					break;
				}
				num++;
			}
			if (transformFever != null && objFever == null)
			{
				objFever = (Object.Instantiate((Object)GlobalVars.Instance.fxFeverGun, transformFever.position, Quaternion.Euler(0f, 0f, 0f)) as GameObject);
				Recursively.SetLayer(objFever.GetComponent<Transform>(), LayerMask.NameToLayer("FPWeapon"));
				objFever.transform.position = transformFever.position;
				objFever.transform.parent = transformFever;
				objFever.transform.localRotation = Quaternion.Euler(90f, 90f, 0f);
			}
			else
			{
				Debug.LogError("Not found. Dummy_fire_effect.");
			}
		}
		else if (objFever != null)
		{
			Object.Destroy(objFever);
			objFever = null;
			transformFever = null;
			feverAutoFire1Click = false;
			feverAutoFire2Click = false;
		}
	}

	public virtual bool IsFullAmmo()
	{
		return true;
	}

	public virtual float AddUpgradedDamagef(int cat)
	{
		if (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.MISSION)
		{
			return 0f;
		}
		if (DefenseManager.Instance.upgradeAtkVal >= 0)
		{
			switch (cat)
			{
			case 0:
				return DefenseManager.Instance.GetUpgradeAtkTable().HeavyAtkVal;
			case 1:
				return DefenseManager.Instance.GetUpgradeAtkTable().AssultAtkVal;
			case 2:
				return DefenseManager.Instance.GetUpgradeAtkTable().SniperAtkVal;
			case 3:
				return DefenseManager.Instance.GetUpgradeAtkTable().SubmachineAtkVal;
			case 4:
				return DefenseManager.Instance.GetUpgradeAtkTable().HandgunAtkVal;
			case 6:
				return DefenseManager.Instance.GetUpgradeAtkTable().SpecialAtkVal;
			default:
				return 0f;
			}
		}
		return 0f;
	}

	public virtual float AddUpgradedShockf(int cat)
	{
		if (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.MISSION)
		{
			return 0f;
		}
		if (DefenseManager.Instance.upgradeShockVal >= 0)
		{
			switch (cat)
			{
			case 0:
				return DefenseManager.Instance.GetUpgradeShockTable().HeavyAtkVal;
			case 1:
				return DefenseManager.Instance.GetUpgradeShockTable().AssultAtkVal;
			case 2:
				return DefenseManager.Instance.GetUpgradeShockTable().SniperAtkVal;
			case 3:
				return DefenseManager.Instance.GetUpgradeShockTable().SubmachineAtkVal;
			case 4:
				return DefenseManager.Instance.GetUpgradeShockTable().HandgunAtkVal;
			case 6:
				return DefenseManager.Instance.GetUpgradeShockTable().SpecialAtkVal;
			default:
				return 0f;
			}
		}
		return 0f;
	}

	public virtual float NextUpgradedShockf(int cat)
	{
		if (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.MISSION)
		{
			return 0f;
		}
		if (DefenseManager.Instance.upgradeShockVal + 1 >= 0)
		{
			switch (cat)
			{
			case 0:
				return DefenseManager.Instance.GetNextUpgradeShockTable().HeavyAtkVal;
			case 1:
				return DefenseManager.Instance.GetNextUpgradeShockTable().AssultAtkVal;
			case 2:
				return DefenseManager.Instance.GetNextUpgradeShockTable().SniperAtkVal;
			case 3:
				return DefenseManager.Instance.GetNextUpgradeShockTable().SubmachineAtkVal;
			case 4:
				return DefenseManager.Instance.GetNextUpgradeShockTable().HandgunAtkVal;
			case 6:
				return DefenseManager.Instance.GetNextUpgradeShockTable().SpecialAtkVal;
			default:
				return 0f;
			}
		}
		return 0f;
	}

	public virtual int AddUpgradedChargei(int cat)
	{
		if (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.MISSION)
		{
			return 0;
		}
		if (DefenseManager.Instance.upgradeChargeVal >= 0)
		{
			switch (cat)
			{
			case 0:
				return (int)DefenseManager.Instance.GetUpgradeChargeTable().HeavyAtkVal;
			case 1:
				return (int)DefenseManager.Instance.GetUpgradeChargeTable().AssultAtkVal;
			case 2:
				return (int)DefenseManager.Instance.GetUpgradeChargeTable().SniperAtkVal;
			case 3:
				return (int)DefenseManager.Instance.GetUpgradeChargeTable().SubmachineAtkVal;
			case 4:
				return (int)DefenseManager.Instance.GetUpgradeChargeTable().HandgunAtkVal;
			case 6:
				return (int)DefenseManager.Instance.GetUpgradeChargeTable().SpecialAtkVal;
			default:
				return 0;
			}
		}
		return 0;
	}

	public virtual int NextUpgradedChargei(int cat)
	{
		if (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.MISSION)
		{
			return 0;
		}
		if (DefenseManager.Instance.upgradeChargeVal + 1 >= 0)
		{
			switch (cat)
			{
			case 0:
				return (int)DefenseManager.Instance.GetNextUpgradeChargeTable().HeavyAtkVal;
			case 1:
				return (int)DefenseManager.Instance.GetNextUpgradeChargeTable().AssultAtkVal;
			case 2:
				return (int)DefenseManager.Instance.GetNextUpgradeChargeTable().SniperAtkVal;
			case 3:
				return (int)DefenseManager.Instance.GetNextUpgradeChargeTable().SubmachineAtkVal;
			case 4:
				return (int)DefenseManager.Instance.GetNextUpgradeChargeTable().HandgunAtkVal;
			case 6:
				return (int)DefenseManager.Instance.GetNextUpgradeChargeTable().SpecialAtkVal;
			default:
				return 0;
			}
		}
		return 0;
	}

	public virtual int AddUpgradedDamagei(int cat)
	{
		if (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.MISSION)
		{
			return 0;
		}
		if (DefenseManager.Instance.upgradeAtkVal >= 0)
		{
			switch (cat)
			{
			case 0:
				return (int)DefenseManager.Instance.GetUpgradeAtkTable().HeavyAtkVal;
			case 1:
				return (int)DefenseManager.Instance.GetUpgradeAtkTable().AssultAtkVal;
			case 2:
				return (int)DefenseManager.Instance.GetUpgradeAtkTable().SniperAtkVal;
			case 3:
				return (int)DefenseManager.Instance.GetUpgradeAtkTable().SubmachineAtkVal;
			case 4:
				return (int)DefenseManager.Instance.GetUpgradeAtkTable().HandgunAtkVal;
			case 6:
				return (int)DefenseManager.Instance.GetUpgradeAtkTable().SpecialAtkVal;
			default:
				return 0;
			}
		}
		return 0;
	}

	public virtual int NextUpgradedDamagei(int cat)
	{
		if (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.MISSION)
		{
			return 0;
		}
		if (DefenseManager.Instance.upgradeAtkVal + 1 >= 0)
		{
			switch (cat)
			{
			case 0:
				return (int)DefenseManager.Instance.GetNextUpgradeAtkTable().HeavyAtkVal;
			case 1:
				return (int)DefenseManager.Instance.GetNextUpgradeAtkTable().AssultAtkVal;
			case 2:
				return (int)DefenseManager.Instance.GetNextUpgradeAtkTable().SniperAtkVal;
			case 3:
				return (int)DefenseManager.Instance.GetNextUpgradeAtkTable().SubmachineAtkVal;
			case 4:
				return (int)DefenseManager.Instance.GetNextUpgradeAtkTable().HandgunAtkVal;
			case 6:
				return (int)DefenseManager.Instance.GetNextUpgradeAtkTable().SpecialAtkVal;
			default:
				return 0;
			}
		}
		return 0;
	}

	public void SetShootEnermyEffect()
	{
		crossEffectTime = 0.3f;
	}

	public void UpdateCrossEffect()
	{
		crossEffectTime -= Time.deltaTime;
	}

	public virtual void SetDetonating(bool set)
	{
	}
}
