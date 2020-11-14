using _Emulator;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class HandBomb : WeaponFunction
{
	public int maxAmmo = -1;

	private SecureInt curAmmoSecure;

	public GameObject explosion;

	public Texture2D vCrossHair;

	public Texture2D hCrossHair;

	public string throwAnimation;

	public float throwForce = 15f;

	public ImageFont ammoFont;

	public float explosionTime = 3f;

	public float persistTime;

	protected float detonatorTime;

	protected ObscuredBool detonating = false;

	protected bool throwing;

	protected bool drawn;

	protected int maxAmmoInst;

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

	public int MaxAmmo
	{
		get
		{
			return maxAmmo;
		}
		set
		{
			maxAmmo = value;
		}
	}

	public GameObject Explosion
	{
		get
		{
			return explosion;
		}
		set
		{
			explosion = value;
		}
	}

	public Texture2D VCrossHair
	{
		get
		{
			return vCrossHair;
		}
		set
		{
			vCrossHair = value;
		}
	}

	public Texture2D HCrossHair
	{
		get
		{
			return hCrossHair;
		}
		set
		{
			hCrossHair = value;
		}
	}

	public string ThrowAnimation
	{
		get
		{
			return throwAnimation;
		}
		set
		{
			throwAnimation = value;
		}
	}

	public float ThrowForce
	{
		get
		{
			return throwForce;
		}
		set
		{
			throwForce = value;
		}
	}

	public ImageFont AmmoFont
	{
		get
		{
			return ammoFont;
		}
		set
		{
			ammoFont = value;
		}
	}

	public float ExplosionTime
	{
		get
		{
			return explosionTime;
		}
		set
		{
			explosionTime = value;
		}
	}

	public float PersistTime
	{
		get
		{
			return persistTime;
		}
		set
		{
			persistTime = value;
		}
	}

	private void Awake()
	{
		curAmmoSecure.Init(0);
		drawn = false;
	}

	private void OnDestroy()
	{
		curAmmoSecure.Release();
	}

	protected void UpgradeMaxAmmo()
	{
		float num = 0f;
		float num2 = 0f;
		if (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.BUNGEE)
		{
			num = MyInfoManager.Instance.SumFunctionFactor("special_ammo_inc");
			num2 = MyInfoManager.Instance.SumFunctionFactor("special_ammo_add");
		}
		bool flag = false;
		int num3 = 0;
		if (num > 0f)
		{
			num3 = Mathf.FloorToInt((float)maxAmmo * num);
			flag = true;
		}
		if (num2 > 0f)
		{
			num3 += Mathf.FloorToInt(num2);
			flag = true;
		}
		if (!flag)
		{
			maxAmmoInst = maxAmmo;
		}
		else
		{
			maxAmmoInst = maxAmmo + num3;
		}
	}

	public override void Reset(bool bDefenseRespwan = false)
	{
		curAmmoSecure.Reset();
		curAmmo = NoCheat.Instance.HideVal(NoCheat.WATCH_DOG.SPECIAL_AMMO, maxAmmoInst);
		NoCheat.Instance.Sync(NoCheat.WATCH_DOG.SPECIAL_AMMO, curAmmo);
		if (drawn)
		{
			Restart();
		}
	}

	public bool AddBonusBomb(int add)
	{
		curAmmo++;
		maxAmmo = curAmmo;
		return true;
	}

	public bool Charge()
	{
		if (IsFullAmmo())
		{
			return false;
		}
		Reset();
		return true;
	}

	protected void UseAmmo()
	{
		curAmmo = NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.SPECIAL_AMMO, curAmmo) - 1;
		if (curAmmo < 0)
		{
			curAmmo = 0;
		}
		curAmmo = NoCheat.Instance.HideVal(NoCheat.WATCH_DOG.SPECIAL_AMMO, curAmmo);
		NoCheat.Instance.Sync(NoCheat.WATCH_DOG.SPECIAL_AMMO, curAmmo);
	}

	public override bool IsFullAmmo()
	{
		return curAmmo >= maxAmmoInst;
	}

	protected void Restart()
	{
		detonatorTime = 0f;
		detonating = false;
		throwing = false;
		if (curAmmo > 0)
		{
			ShowGrenade(body: true, clip: true);
		}
		else
		{
			ShowGrenade(body: false, clip: false);
		}
	}

	public override void SetDrawn(bool draw)
	{
		drawn = draw;
		if (drawn)
		{
			Restart();
		}
		else
		{
			CancelOngoingProcess();
		}
	}

	private void CancelOngoingProcess()
	{
		detonatorTime = 0f;
		detonating = false;
		throwing = false;
	}

	protected void DrawCrossHair()
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
			GUI.color = color;
		}
	}

	protected void RemoveSafetyClip()
	{
		if (GetComponent<Weapon>().CatridgeOrClip != null)
		{
			GameObject gameObject = Object.Instantiate((Object)GetComponent<Weapon>().CatridgeOrClip, base.transform.position, base.transform.rotation) as GameObject;
			if (null != gameObject)
			{
				Rigidbody component = gameObject.GetComponent<Rigidbody>();
				if (null != component)
				{
					component.AddForce(Vector3.left * 0.2f, ForceMode.Impulse);
					component.AddTorque(Vector3.up * 0.2f, ForceMode.Impulse);
				}
			}
		}
		detonatorTime = 0f;
		detonating = true;
		ShowGrenade(body: true, clip: false);
	}

	protected void ShowGrenade(bool body, bool clip)
	{
		SkinnedMeshRenderer[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].name.Contains("body"))
			{
				componentsInChildren[i].enabled = body;
			}
			if (componentsInChildren[i].name.Contains("clip"))
			{
				componentsInChildren[i].enabled = clip;
			}
			if (componentsInChildren[i].name.Contains("water"))
			{
				componentsInChildren[i].enabled = body;
			}
		}
		MeshRenderer[] componentsInChildren2 = GetComponentsInChildren<MeshRenderer>();
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			if (componentsInChildren2[j].name.Contains("body"))
			{
				componentsInChildren2[j].enabled = body;
			}
			if (componentsInChildren2[j].name.Contains("clip"))
			{
				componentsInChildren2[j].enabled = clip;
			}
			if (componentsInChildren2[j].name.Contains("sensebomb1"))
			{
				componentsInChildren2[j].enabled = clip;
			}
		}
	}

	protected bool CanThrow()
	{
		return localCtrl.CanControl() && !throwing && curAmmo > 0 && drawn;
	}

	protected void DrawAmmo()
	{
		if (MyInfoManager.Instance.isGuiOn)
		{
			Texture2D weaponBy = TItemManager.Instance.GetWeaponBy((int)base.weaponBy);
			if (null != ammoBg && null != weaponBy)
			{
				Rect position = new Rect((float)(Screen.width - ammoBg.width - 1), (float)(Screen.height - ammoBg.height - 1), (float)ammoBg.width, (float)ammoBg.height);
				TextureUtil.DrawTexture(position, ammoBg);
				float num = 40f;
				float num2 = (float)weaponBy.width * num / (float)weaponBy.height;
				TextureUtil.DrawTexture(new Rect(position.x + (position.width - num2) / 2f, position.y + 10f, num2, num), weaponBy, ScaleMode.StretchToFill);
				ammoFont.Print(new Vector2((float)(Screen.width - 10), (float)(Screen.height - 12)), NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.SPECIAL_AMMO, curAmmo));
			}
		}
	}

	protected void DrawDetonating()
	{
		if (MyInfoManager.Instance.isGuiOn && (bool)detonating)
		{
			Texture2D weaponBy = TItemManager.Instance.GetWeaponBy((int)base.weaponBy);
			if (null != weaponBy)
			{
				Rect position = new Rect((float)(Screen.width / 2 - GlobalVars.Instance.uiBoomGaugeBg.width / 2), (float)(Screen.height - 200), (float)GlobalVars.Instance.uiBoomGaugeBg.width, (float)GlobalVars.Instance.uiBoomGaugeBg.height);
				TextureUtil.DrawTexture(position, GlobalVars.Instance.uiBoomGaugeBg, ScaleMode.StretchToFill);
				float num = detonatorTime / explosionTime;
				Rect position2 = new Rect((float)(Screen.width / 2 - GlobalVars.Instance.uiBoomGaugeBg.width / 2), (float)(Screen.height - 200), (float)GlobalVars.Instance.uiBoomGaugeBg.width * num, (float)GlobalVars.Instance.uiBoomGaugeBg.height);
				TextureUtil.DrawTexture(position2, GlobalVars.Instance.uiBoomGaugeBar, new Rect(0f, 0f, num, 1f));
				float num2 = 62f;
				Rect position3 = new Rect(position2.x - (float)GlobalVars.Instance.uiBoom.width, (float)(Screen.height - 200) - num2, (float)GlobalVars.Instance.uiBoom.width, (float)GlobalVars.Instance.uiBoom.height);
				TextureUtil.DrawTexture(position3, GlobalVars.Instance.uiBoom);
			}
		}
	}

	public void ThrowStart()
	{
		throwing = true;
	}

	public void ThrowEnd()
	{
		Restart();
	}

	public virtual void Throw()
	{
	}
}
