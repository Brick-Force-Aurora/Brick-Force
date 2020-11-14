using System.Collections.Generic;
using UnityEngine;

public class Gun : WeaponFunction
{
	public int maxAmmo = -1;

	private SecureInt curAmmoSecure;

	private int pickedAmmo;

	public Magazine magazine;

	protected NoCheat.WATCH_DOG wdAmmo;

	protected NoCheat.WATCH_DOG wdMagazine;

	protected NoCheat.WATCH_DOG wdAmmo2;

	public float AtkPow = 10f;

	public float Rigidity = 0.5f;

	public float rateOfFire = 5f;

	public float recoilPitch = 1f;

	public float recoilYaw;

	public LAUNCHER Launcher = LAUNCHER.NONE;

	private Dictionary<int, MissleInfo> dicMis;

	private Transform misMuzzleTrans;

	private Transform misSmokeTrans;

	private GameObject misObj;

	private GameObject misSmokeEff;

	private Vector3 misDir = Vector3.zero;

	public float misSpeed = 10f;

	private float dtMissile;

	private bool isMisFired;

	private float dtCollide;

	private Vector3 misPrePos;

	public float ThrowForce = 10f;

	public int maxLauncherAmmo = 3;

	private bool leftHand;

	private bool twoHands;

	private SecureInt maxLauncherAmmoGameSecure;

	private int pickedAmmo2;

	public Texture2D iconLauncher;

	public AudioClip fireSoundSpecial;

	public AudioClip flyingSoundSpecial;

	private Ray shootRay;

	public float Radius2ndWpn = 5f;

	public int Damage2ndWpn = 100;

	public float recoilPitch2ndWpn = 1f;

	public float recoilYaw2ndWpn;

	public bool IsMuzzle = true;

	public GameObject shootObj;

	public float Radius1stWpn = 5f;

	public bool canCyclic = true;

	public string fireAnimation;

	public GameObject muzzleFire;

	private GameObject muzzleFxInstance;

	private GameObject muzzleFxInstance2;

	public ImageFont magazineFont;

	public ImageFont ammoFont;

	public int crdMagazineX = 40;

	public bool BackShotPosition;

	private bool drawn;

	protected bool reloading;

	protected bool cyclic;

	protected float deltaTime = float.NegativeInfinity;

	public bool semiAuto;

	public int semiAutoMaxCyclicAmmo;

	private int curSemiAutoMaxCyclicAmmo;

	private Transform muzzle;

	private int maxAmmoInst;

	private TutoInput tutoInput;

	private float backward = 0.3f;

	private bool IsFontScaled;

	private bool IsFontBig = true;

	private float curFontScale = 0.7f;

	private float minFontScale = 0.7f;

	private float maxFontScale = 2.5f;

	private float deltaTimeFontScale;

	private int missileUniqSeq = -1;

	private int hitMonSeq = -1;

	private int hitBrickSeq = -1;

	private bool IsBrickDead;

	private Vector3 hitPoint = Vector3.zero;

	private int misSeq;

	public int curAmmo
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

	public int PickedAmmo
	{
		get
		{
			return pickedAmmo;
		}
		set
		{
			pickedAmmo = value;
		}
	}

	public bool LeftHand
	{
		get
		{
			return leftHand;
		}
		set
		{
			leftHand = value;
		}
	}

	public bool TwoHands
	{
		get
		{
			return twoHands;
		}
		set
		{
			twoHands = value;
		}
	}

	public int maxLauncherAmmoGame
	{
		get
		{
			return maxLauncherAmmoGameSecure.Get();
		}
		set
		{
			maxLauncherAmmoGameSecure.Set(value);
		}
	}

	public int PickedAmmo2
	{
		get
		{
			return pickedAmmo2;
		}
		set
		{
			pickedAmmo2 = value;
		}
	}

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

	public Magazine _Magazine
	{
		get
		{
			return magazine;
		}
		set
		{
			magazine = value;
		}
	}

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

	public float RateOfFire
	{
		get
		{
			return rateOfFire;
		}
		set
		{
			rateOfFire = value;
		}
	}

	public float RecoilPitch
	{
		get
		{
			return recoilPitch;
		}
		set
		{
			recoilPitch = value;
		}
	}

	public float RecoilYaw
	{
		get
		{
			return recoilYaw;
		}
		set
		{
			recoilYaw = value;
		}
	}

	public LAUNCHER _Launcher
	{
		get
		{
			return Launcher;
		}
		set
		{
			Launcher = value;
		}
	}

	public float MisSpeed
	{
		get
		{
			return misSpeed;
		}
		set
		{
			misSpeed = value;
		}
	}

	public float _ThrowForce
	{
		get
		{
			return ThrowForce;
		}
		set
		{
			ThrowForce = value;
		}
	}

	public int MaxLauncherAmmo
	{
		get
		{
			return maxLauncherAmmo;
		}
		set
		{
			maxLauncherAmmo = value;
		}
	}

	public Texture2D IconLauncher
	{
		get
		{
			return iconLauncher;
		}
		set
		{
			iconLauncher = value;
		}
	}

	public AudioClip FireSoundSpecial
	{
		get
		{
			return fireSoundSpecial;
		}
		set
		{
			fireSoundSpecial = value;
		}
	}

	public AudioClip FlyingSoundSpecial
	{
		get
		{
			return flyingSoundSpecial;
		}
		set
		{
			flyingSoundSpecial = value;
		}
	}

	public float _Radius2ndWpn
	{
		get
		{
			return Radius2ndWpn;
		}
		set
		{
			Radius2ndWpn = value;
		}
	}

	public int _Damage2ndWpn
	{
		get
		{
			return Damage2ndWpn;
		}
		set
		{
			Damage2ndWpn = value;
		}
	}

	public float RrecoilPitch2ndWpn
	{
		get
		{
			return recoilPitch2ndWpn;
		}
		set
		{
			recoilPitch2ndWpn = value;
		}
	}

	public float RecoilYaw2ndWpn
	{
		get
		{
			return recoilYaw2ndWpn;
		}
		set
		{
			recoilYaw2ndWpn = value;
		}
	}

	public bool isMuzzle
	{
		get
		{
			return IsMuzzle;
		}
		set
		{
			IsMuzzle = value;
		}
	}

	public GameObject ShootObj
	{
		get
		{
			return shootObj;
		}
		set
		{
			shootObj = value;
		}
	}

	public float _Radius1stWpn
	{
		get
		{
			return Radius1stWpn;
		}
		set
		{
			Radius1stWpn = value;
		}
	}

	public bool CanCyclic
	{
		get
		{
			return canCyclic;
		}
		set
		{
			canCyclic = value;
		}
	}

	public string FireAnimation
	{
		get
		{
			return fireAnimation;
		}
		set
		{
			fireAnimation = value;
		}
	}

	public GameObject MuzzleFire
	{
		get
		{
			return muzzleFire;
		}
		set
		{
			muzzleFire = value;
		}
	}

	public ImageFont MagazineFont
	{
		get
		{
			return magazineFont;
		}
		set
		{
			magazineFont = value;
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

	public int CrdMagazineX
	{
		get
		{
			return crdMagazineX;
		}
		set
		{
			crdMagazineX = value;
		}
	}

	public bool SemiAuto
	{
		get
		{
			return semiAuto;
		}
		set
		{
			semiAuto = value;
		}
	}

	public int SemiAutoMaxCyclicAmmo
	{
		get
		{
			return semiAutoMaxCyclicAmmo;
		}
		set
		{
			semiAutoMaxCyclicAmmo = value;
		}
	}

	private void UpgradeMaxAmmo()
	{
		float num = 0f;
		switch (GetComponent<Weapon>().slot)
		{
		case Weapon.TYPE.MAIN:
			num = MyInfoManager.Instance.SumFunctionFactor("main_ammo_inc");
			break;
		case Weapon.TYPE.AUX:
			num = MyInfoManager.Instance.SumFunctionFactor("aux_ammo_inc");
			break;
		}
		maxAmmoInst = maxAmmo + Mathf.FloorToInt((float)maxAmmo * num);
	}

	public bool AddBonusAmmo(int add)
	{
		if (curAmmo == maxAmmoInst - magazine.max)
		{
			return false;
		}
		curAmmo += add;
		if (curAmmo > maxAmmoInst - magazine.max)
		{
			curAmmo = maxAmmoInst - magazine.max;
		}
		NoCheat.Instance.Sync(wdAmmo, curAmmo);
		return true;
	}

	public bool AddBonusAmmoDF(int add)
	{
		bool result = magazine.addAmmo(add);
		NoCheat.Instance.Sync(wdMagazine, magazine.Cur);
		return result;
	}

	private void InitializeAnimation()
	{
		base.animation.wrapMode = WrapMode.Loop;
		base.animation["empty"].layer = 1;
		base.animation["empty"].wrapMode = WrapMode.ClampForever;
		base.animation["fire"].layer = 1;
		base.animation["fire"].wrapMode = WrapMode.Once;
		base.animation["idle"].layer = 1;
		base.animation["idle"].wrapMode = WrapMode.Loop;
		if (Launcher != LAUNCHER.NONE)
		{
			base.animation["bigfire"].layer = 1;
			base.animation["bigfire"].wrapMode = WrapMode.Once;
		}
		base.animation.CrossFade("idle");
	}

	public override void Reset(bool bDefenseRespwan = false)
	{
		curAmmoSecure.Reset();
		maxLauncherAmmoGameSecure.Reset();
		if (!bDefenseRespwan)
		{
			curAmmo = maxAmmoInst;
			magazine.Reset();
			curAmmo = magazine.Reload(curAmmo);
			maxLauncherAmmoGame = NoCheat.Instance.HideVal(NoCheat.WATCH_DOG.MAIN_AMMO2, maxLauncherAmmo);
			if (!leftHand)
			{
				NoCheat.Instance.Sync(wdAmmo, curAmmo);
				NoCheat.Instance.Sync(wdMagazine, magazine.Cur);
			}
			if (wdAmmo2 == NoCheat.WATCH_DOG.MAIN_AMMO2)
			{
				NoCheat.Instance.Sync(wdAmmo2, maxLauncherAmmoGame);
			}
		}
		Restart();
	}

	public void AddAmmo(float percent)
	{
		if (!leftHand)
		{
			GlobalVars.Instance.cheatBlock = false;
			int num = (int)((float)maxAmmoInst * percent);
			int num2 = curAmmo + NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.MAIN_MAGAZINE, magazine.Cur);
			num2 += num;
			if (num2 > maxAmmoInst)
			{
				num2 = maxAmmoInst;
			}
			GlobalVars.Instance.IsRecognition = true;
			GlobalVars.Instance.recogniVal = num;
			GlobalVars.Instance.recogniteType = 1;
			magazine.Reset();
			curAmmo = magazine.Reload(num2);
			NoCheat.Instance.Sync(wdAmmo, curAmmo);
			NoCheat.Instance.Sync(wdMagazine, magazine.Cur);
			IsFontScaled = true;
			IsFontBig = true;
			curFontScale = 0.7f;
			deltaTimeFontScale = 0f;
			GlobalVars.Instance.cheatBlock = true;
		}
	}

	public bool Charge()
	{
		int num = maxAmmoInst - magazine.max;
		int num2 = Mathf.Min(magazine.max, num - curAmmo);
		if (num2 <= 0)
		{
			return false;
		}
		curAmmo += num2;
		if (!leftHand)
		{
			NoCheat.Instance.Sync(wdAmmo, curAmmo);
		}
		return true;
	}

	public override bool IsFullAmmo()
	{
		return magazine.Cur + curAmmo >= maxAmmoInst;
	}

	private void Restart()
	{
		cyclic = false;
		reloading = false;
		Scope component = GetComponent<Scope>();
		if (null != component)
		{
			component.SetAiming(_aiming: true);
			if (component.IsZooming())
			{
				component.ToggleScoping(forceApply: true);
			}
		}
		GetComponent<Aim>().SetAiming(_aiming: true);
		if (magazine.Empty)
		{
			base.animation.Play("empty");
		}
		else
		{
			base.animation.Play("idle");
		}
		if (!leftHand && pickedAmmo > 0)
		{
			curAmmo = pickedAmmo;
			pickedAmmo = 0;
			magazine.Reset();
			curAmmo = magazine.Reload(curAmmo);
			maxLauncherAmmoGame = pickedAmmo2;
			pickedAmmo2 = 0;
			NoCheat.Instance.Sync(wdAmmo, curAmmo);
			NoCheat.Instance.Sync(wdMagazine, magazine.Cur);
			maxLauncherAmmoGame = NoCheat.Instance.HideVal(NoCheat.WATCH_DOG.MAIN_AMMO2, maxLauncherAmmoGame);
			if (wdAmmo2 == NoCheat.WATCH_DOG.MAIN_AMMO2)
			{
				NoCheat.Instance.Sync(wdAmmo2, maxLauncherAmmoGame);
			}
			GlobalVars.Instance.cheatBlock = false;
		}
	}

	private void Awake()
	{
		curAmmoSecure.Init(0);
		maxLauncherAmmoGameSecure.Init(3);
		switch (GetComponent<Weapon>().slot)
		{
		case Weapon.TYPE.MAIN:
			wdAmmo = NoCheat.WATCH_DOG.MAIN_AMMO;
			wdMagazine = NoCheat.WATCH_DOG.MAIN_MAGAZINE;
			wdAmmo2 = NoCheat.WATCH_DOG.MAIN_AMMO2;
			break;
		case Weapon.TYPE.AUX:
			wdAmmo = NoCheat.WATCH_DOG.AUX_AMMO;
			wdMagazine = NoCheat.WATCH_DOG.AUX_MAGAZINE;
			wdAmmo2 = NoCheat.WATCH_DOG.NUM;
			break;
		}
	}

	private void OnDestroy()
	{
		if (dicMis != null && dicMis.Count > 0)
		{
			foreach (KeyValuePair<int, MissleInfo> dicMi in dicMis)
			{
				Object.DestroyImmediate(dicMi.Value.obj);
			}
			dicMis.Clear();
		}
		if (misSmokeEff != null)
		{
			Object.DestroyImmediate(misSmokeEff);
		}
		curAmmoSecure.Release();
		maxLauncherAmmoGameSecure.Release();
	}

	private void Start()
	{
		dicMis = new Dictionary<int, MissleInfo>();
		if (BuildOption.Instance.Props.useUskWeaponTex && applyUsk)
		{
			SkinnedMeshRenderer[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
			{
				if (skinnedMeshRenderer.material.mainTexture != null && UskManager.Instance.Get(skinnedMeshRenderer.material.mainTexture.name) != null)
				{
					skinnedMeshRenderer.material.mainTexture = UskManager.Instance.Get(skinnedMeshRenderer.material.mainTexture.name);
				}
			}
		}
		if (!BuildOption.Instance.IsNetmarble && !BuildOption.Instance.IsDeveloper && BuildOption.Instance.Props.useUskMuzzleEff && applyUsk)
		{
			muzzleFire = GlobalVars.Instance.muzzlefireUsk1;
		}
		muzzle = null;
		Transform[] componentsInChildren2 = GetComponentsInChildren<Transform>();
		int num = 0;
		while (muzzle == null && num < componentsInChildren2.Length)
		{
			if (componentsInChildren2[num].name.Contains("Dummy_fire_effect"))
			{
				muzzle = componentsInChildren2[num];
			}
			num++;
		}
		componentsInChildren2 = GetComponentsInChildren<Transform>();
		int num2 = 0;
		while (misMuzzleTrans == null && num2 < componentsInChildren2.Length)
		{
			if (componentsInChildren2[num2].name.Contains("Dummy_fire_missile"))
			{
				misMuzzleTrans = componentsInChildren2[num2];
			}
			num2++;
		}
		Modify();
		UpgradeMaxAmmo();
		InitializeAnimation();
		Reset();
		SetBackword();
		tutoInput = GameObject.Find("Main").GetComponent<TutoInput>();
	}

	protected virtual void Modify()
	{
		WpnMod wpnMod = WeaponModifier.Instance.Get((int)weaponBy);
		if (wpnMod != null)
		{
			maxAmmo = wpnMod.maxAmmo;
			magazine.max = wpnMod.maxMagazine;
			speedFactor = wpnMod.fSpeedFactor;
			AtkPow = wpnMod.fAtkPow;
			Rigidity = wpnMod.fRigidity;
			rateOfFire = wpnMod.fRateOfFire;
			recoilPitch = wpnMod.fRecoilPitch;
			recoilYaw = wpnMod.recoilYaw;
		}
		WpnModEx ex = WeaponModifier.Instance.GetEx((int)weaponBy);
		if (ex != null)
		{
			misSpeed = ex.misSpeed;
			ThrowForce = ex.throwForce;
			maxLauncherAmmo = ex.maxLauncherAmmo;
			Radius2ndWpn = ex.radius2ndWpn;
			Damage2ndWpn = ex.damage2ndWpn;
			recoilPitch2ndWpn = ex.recoilPitch2ndWpn;
			recoilYaw2ndWpn = ex.recoilYaw2ndWpn;
			Radius1stWpn = ex.Radius1stWpn;
			semiAutoMaxCyclicAmmo = ex.semiAutoMaxCyclicAmmo;
		}
		WeaponFunction component = GetComponent<WeaponFunction>();
		TWeapon tWeapon = (TWeapon)GetComponent<Weapon>().tItem;
		if (null != component && tWeapon != null)
		{
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
				num = 2;
				grade = item.upgradeProps[num].grade;
				if (grade > 0)
				{
					float value2 = PimpManager.Instance.getValue((int)tWeapon.upgradeCategory, num, grade - 1);
					recoilPitch += value2;
				}
				num = 3;
				grade = item.upgradeProps[num].grade;
				if (grade > 0)
				{
					float value3 = PimpManager.Instance.getValue((int)tWeapon.upgradeCategory, num, grade - 1);
					rateOfFire += value3;
				}
				num = 4;
				grade = item.upgradeProps[num].grade;
				if (grade > 0)
				{
					float value4 = PimpManager.Instance.getValue((int)tWeapon.upgradeCategory, num, grade - 1);
					maxAmmo += (int)value4;
				}
			}
		}
	}

	protected void CreateMuzzleFire()
	{
		if (!(null == muzzleFire) && !(null == muzzle))
		{
			if (muzzleFxInstance == null)
			{
				GameObject gameObject = Object.Instantiate((Object)muzzleFire) as GameObject;
				Recursively.SetLayer(gameObject.GetComponent<Transform>(), LayerMask.NameToLayer("FPWeapon"));
				gameObject.transform.position = muzzle.position;
				gameObject.transform.parent = muzzle;
				gameObject.transform.localRotation = (leftHand ? Quaternion.Euler(-90f, 90f, 0f) : Quaternion.Euler(90f, 90f, 0f));
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

	private Vector2 CalcDeflection()
	{
		Scope component = GetComponent<Scope>();
		if (null != component && component.IsZooming())
		{
			return component.CalcDeflection();
		}
		return GetComponent<Aim>().CalcDeflection();
	}

	protected void Shoot()
	{
		if (!(cam == null))
		{
			GameObject gameObject = GameObject.Find("Me");
			if (gameObject != null)
			{
				LocalController component = gameObject.GetComponent<LocalController>();
				if (component != null)
				{
					component.IDidSomething();
				}
			}
			Vector2 vector = CalcDeflection();
			int layerMask = (1 << LayerMask.NameToLayer("Chunk")) | (1 << LayerMask.NameToLayer("Brick")) | (1 << LayerMask.NameToLayer("BoxMan")) | (1 << LayerMask.NameToLayer("Mon")) | (1 << LayerMask.NameToLayer("InvincibleArmor")) | (1 << LayerMask.NameToLayer("Bomb")) | (1 << LayerMask.NameToLayer("InstalledBomb"));
			Ray ray = cam.ScreenPointToRay(new Vector3(vector.x, vector.y, 0f));
			HIT_KIND hIT_KIND = HIT_KIND.NONE;
			int num = -1;
			int num2 = -1;
			int damage = 0;
			float rigidity = 0f;
			bool destructable = false;
			int num3 = -1;
			int hitPart = -1;
			bool lucky = false;
			int layer = -1;
			if (!GlobalVars.Instance.applyNewP2P)
			{
				P2PManager.Instance.SendPEER_FIRE(MyInfoManager.Instance.Seq, (int)GetComponent<Weapon>().slot, 0, ray.origin, ray.direction);
			}
			if (Physics.Raycast(ray, out RaycastHit hitInfo, GetComponent<Weapon>().range, layerMask))
			{
				GameObject gameObject2 = hitInfo.transform.gameObject;
				if (gameObject2.layer == LayerMask.NameToLayer("Brick") || gameObject2.layer == LayerMask.NameToLayer("Chunk"))
				{
					GameObject gameObject3 = null;
					BrickProperty brickProperty = null;
					GameObject gameObject4 = null;
					Texture2D texture2D = null;
					if (gameObject2.layer == LayerMask.NameToLayer("Brick"))
					{
						BrickProperty[] allComponents = Recursively.GetAllComponents<BrickProperty>(gameObject2.transform, includeInactive: false);
						if (allComponents.Length > 0)
						{
							brickProperty = allComponents[0];
						}
					}
					else
					{
						gameObject3 = BrickManager.Instance.GetBrickObjectByPos(Brick.ToBrickCoord(hitInfo.normal, hitInfo.point));
						if (null != gameObject3)
						{
							brickProperty = gameObject3.GetComponent<BrickProperty>();
						}
					}
					if (null != brickProperty)
					{
						hIT_KIND = HIT_KIND.BRICK;
						if (!GlobalVars.Instance.applyNewP2P)
						{
							if (shootObj != null)
							{
								P2PManager.Instance.SendPEER_HIT_BRICK(brickProperty.Seq, curAmmo, hitInfo.point, ray.direction, isBullet: false);
							}
							else
							{
								P2PManager.Instance.SendPEER_HIT_BRICK(brickProperty.Seq, brickProperty.Index, hitInfo.point, hitInfo.normal, isBullet: true);
							}
						}
						num = brickProperty.Seq;
						num2 = brickProperty.Index;
						texture2D = BrickManager.Instance.GetBulletMark(brickProperty.Index);
						gameObject4 = BrickManager.Instance.GetBulletImpact(brickProperty.Index);
						Brick brick = BrickManager.Instance.GetBrick(brickProperty.Index);
						if (brick != null && brick.destructible)
						{
							destructable = true;
							brickProperty.Hit((int)CalcAtkPow(Vector3.Distance(hitInfo.point, base.transform.position)));
							if (brickProperty.HitPoint <= 0)
							{
								CSNetManager.Instance.Sock.SendCS_DESTROY_BRICK_REQ(brickProperty.Seq);
								texture2D = null;
								gameObject4 = null;
								if ((brickProperty.Index == 115 || brickProperty.Index == 193) && gameObject3 != null)
								{
									ExplosionUtil.CheckMyself(gameObject3.transform.position, GlobalVars.Instance.BoomDamage, GlobalVars.Instance.BoomRadius, -3);
									ExplosionUtil.CheckBoxmen(gameObject3.transform.position, GlobalVars.Instance.BoomDamage, GlobalVars.Instance.BoomRadius, -3, Rigidity);
									ExplosionUtil.CheckMonster(gameObject3.transform.position, GlobalVars.Instance.BoomDamage, GlobalVars.Instance.BoomRadius);
									ExplosionUtil.CheckDestructibles(gameObject3.transform.position, GlobalVars.Instance.BoomDamage, GlobalVars.Instance.BoomRadius);
								}
							}
							else
							{
								damage = brickProperty.HitPoint;
								if (!GlobalVars.Instance.applyNewP2P)
								{
									P2PManager.Instance.SendPEER_BRICK_HITPOINT(brickProperty.Seq, brickProperty.HitPoint);
								}
								GameObject impact = VfxOptimizer.Instance.GetImpact(gameObject2.layer);
								if (null != impact)
								{
									Object.Instantiate((Object)impact, hitInfo.point, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
									layer = gameObject2.layer;
									if (!GlobalVars.Instance.applyNewP2P)
									{
										P2PManager.Instance.SendPEER_HIT_IMPACT(gameObject2.layer, hitInfo.point, hitInfo.normal);
									}
								}
							}
						}
					}
					if (shootObj != null)
					{
						GameObject gameObject5 = Object.Instantiate((Object)shootObj) as GameObject;
						if (gameObject5 != null)
						{
							gameObject5.transform.position = hitInfo.point;
							gameObject5.transform.forward = ray.direction;
							CheckBrickDead component2 = gameObject5.GetComponent<CheckBrickDead>();
							if (component2 != null)
							{
								component2.brickID = brickProperty.Seq;
							}
						}
					}
					else if (null != gameObject3 && null != texture2D)
					{
						GameObject gameObject6 = Object.Instantiate((Object)BrickManager.Instance.bulletMark, hitInfo.point, Quaternion.FromToRotation(Vector3.forward, -hitInfo.normal)) as GameObject;
						if (gameObject6 != null)
						{
							BulletMark component3 = gameObject6.GetComponent<BulletMark>();
							if (component3 != null)
							{
								component3.GenerateDecal(texture2D, gameObject2, gameObject3);
							}
						}
					}
					if (null != gameObject4)
					{
						Object.Instantiate((Object)gameObject4, hitInfo.point, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
					}
				}
				else if (gameObject2.layer == LayerMask.NameToLayer("BoxMan"))
				{
					PlayerProperty[] allComponents2 = Recursively.GetAllComponents<PlayerProperty>(gameObject2.transform, includeInactive: false);
					TPController[] allComponents3 = Recursively.GetAllComponents<TPController>(gameObject2.transform, includeInactive: false);
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
						hIT_KIND = HIT_KIND.HUMAN;
						int num4 = 0;
						HitPart component4 = gameObject2.GetComponent<HitPart>();
						if (component4 != null)
						{
							bool flag = false;
							if (component4.part == HitPart.TYPE.HEAD)
							{
								Scope component5 = GetComponent<Scope>();
								int layerMask2 = 1 << LayerMask.NameToLayer("Brain");
								if (null != component5)
								{
									layerMask2 = 1 << LayerMask.NameToLayer("CoreBrain");
								}
								if (Physics.Raycast(ray, out RaycastHit hitInfo2, GetComponent<Weapon>().range, layerMask2))
								{
									if (playerProperty.Desc.IsLucky())
									{
										flag = true;
									}
									else
									{
										component4 = hitInfo2.transform.gameObject.GetComponent<HitPart>();
									}
								}
							}
							if (component4.GetHitImpact() != null)
							{
								GameObject gameObject7 = component4.GetHitImpact();
								if (flag && null != component4.luckyImpact)
								{
									gameObject7 = component4.luckyImpact;
								}
								if (gameObject7 != null)
								{
									Object.Instantiate((Object)gameObject7, hitInfo.point, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
								}
							}
							num4 = (int)(CalcAtkPow(Vector3.Distance(hitInfo.point, base.transform.position)) * component4.damageFactor);
							if (!playerProperty.IsHostile())
							{
								num4 = 0;
							}
							if (shootObj != null)
							{
								GameObject gameObject8 = Object.Instantiate((Object)shootObj) as GameObject;
								if (gameObject8 != null)
								{
									gameObject8.transform.position = hitInfo.point;
									gameObject8.transform.forward = ray.direction;
									ParentFollow component6 = gameObject8.GetComponent<ParentFollow>();
									if (component6 != null)
									{
										component6.IsHuman = true;
										component6.HitParent = component4.transform;
										component6.ParentSeq = playerProperty.Desc.Seq;
									}
								}
							}
							WeaponFunction component7 = GetComponent<WeaponFunction>();
							if (null == component7)
							{
								Debug.LogError("wpnFucn == null");
							}
							TWeapon tWeapon = (TWeapon)GetComponent<Weapon>().tItem;
							if (tWeapon == null)
							{
								Debug.LogError("wpn == null");
							}
							Item item = MyInfoManager.Instance.GetItemBySequence(component7.ItemSeq);
							if (item == null)
							{
								item = MyInfoManager.Instance.GetUsingEquipByCode(tWeapon.code);
							}
							num4 = GlobalVars.Instance.applyDurabilityDamage(item?.Durability ?? (-1), tWeapon.durabilityMax, num4);
							num3 = playerProperty.Desc.Seq;
							hitPart = (int)component4.part;
							damage = num4;
							rigidity = Rigidity;
							lucky = flag;
							if (!GlobalVars.Instance.applyNewP2P)
							{
								P2PManager.Instance.SendPEER_HIT_BRICKMAN(MyInfoManager.Instance.Seq, playerProperty.Desc.Seq, (int)component4.part, hitInfo.point, hitInfo.normal, flag, curAmmo, ray.direction);
								P2PManager.Instance.SendPEER_SHOOT(MyInfoManager.Instance.Seq, playerProperty.Desc.Seq, num4, Rigidity, (int)weaponBy, (int)component4.part, flag, rateOfFire);
							}
						}
						tPController.GetHit(num4, playerProperty.Desc.Seq);
					}
				}
				else if (gameObject2.layer == LayerMask.NameToLayer("Mon"))
				{
					MonProperty[] allComponents4 = Recursively.GetAllComponents<MonProperty>(gameObject2.transform, includeInactive: false);
					MonController[] allComponents5 = Recursively.GetAllComponents<MonController>(gameObject2.transform, includeInactive: false);
					MonProperty monProperty = null;
					MonController x = null;
					if (allComponents4.Length > 0)
					{
						monProperty = allComponents4[0];
					}
					if (allComponents5.Length > 0)
					{
						x = allComponents5[0];
					}
					if (monProperty != null && x != null)
					{
						if ((MyInfoManager.Instance.Slot < 4 && monProperty.Desc.bRedTeam) || (MyInfoManager.Instance.Slot >= 4 && !monProperty.Desc.bRedTeam))
						{
							return;
						}
						HitPart component8 = gameObject2.GetComponent<HitPart>();
						if (component8 != null)
						{
							if (monProperty.Desc.typeID == 2)
							{
								aiHide aiHide = (aiHide)MonManager.Instance.GetAIClass(monProperty.Desc.Seq, monProperty.Desc.tblID);
								if (aiHide != null)
								{
									if (aiHide.IsHide)
									{
										return;
									}
									if (aiHide.CanApply)
									{
										aiHide.setHide(bSet: true);
									}
								}
							}
							if (component8.GetHitImpact() != null)
							{
								Object.Instantiate((Object)component8.GetHitImpact(), hitInfo.point, Quaternion.Euler(0f, 0f, 0f));
							}
							if (monProperty.Desc.Xp <= 0)
							{
								return;
							}
							hIT_KIND = HIT_KIND.MON;
							int num5 = (int)(CalcAtkPow(Vector3.Distance(hitInfo.point, base.transform.position)) * component8.damageFactor);
							num5 += AddUpgradedDamagei(category);
							if (monProperty.Desc.bHalfDamage)
							{
								num5 /= 2;
							}
							rigidity = monProperty.Desc.rigidity;
							hitMonSeq = monProperty.Desc.Seq;
							damage = num5;
							if (shootObj != null)
							{
								GameObject gameObject9 = Object.Instantiate((Object)shootObj) as GameObject;
								if (gameObject9 != null)
								{
									gameObject9.transform.position = hitInfo.point;
									gameObject9.transform.forward = ray.direction;
									CheckMonDead component9 = gameObject9.GetComponent<CheckMonDead>();
									if (component9 != null)
									{
										component9.MonID = monProperty.Desc.Seq;
									}
									ParentFollow component10 = gameObject9.GetComponent<ParentFollow>();
									if (component10 != null)
									{
										component10.IsHuman = false;
										component10.HitParent = component8.transform;
										component10.ParentSeq = monProperty.Desc.Seq;
									}
								}
							}
							monProperty.Desc.rigidity = Rigidity + AddUpgradedShockf(category);
							if (monProperty.Desc.rigidity > 1f)
							{
								monProperty.Desc.rigidity = 1f;
							}
							MonManager.Instance.Hit(monProperty.Desc.Seq, num5, monProperty.Desc.rigidity, (int)weaponBy, hitInfo.point, ray.direction, curAmmo);
						}
					}
				}
				else if (gameObject2.layer == LayerMask.NameToLayer("InvincibleArmor") || gameObject2.layer == LayerMask.NameToLayer("Bomb") || gameObject2.layer == LayerMask.NameToLayer("InstalledBomb"))
				{
					GameObject impact2 = VfxOptimizer.Instance.GetImpact(gameObject2.layer);
					if (null != impact2)
					{
						hIT_KIND = HIT_KIND.OTHER;
						Object.Instantiate((Object)impact2, hitInfo.point, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
						layer = gameObject2.layer;
						if (!GlobalVars.Instance.applyNewP2P)
						{
							P2PManager.Instance.SendPEER_HIT_IMPACT(gameObject2.layer, hitInfo.point, hitInfo.normal);
						}
					}
				}
			}
			if (GlobalVars.Instance.applyNewP2P)
			{
				FirePacket firePacket = new FirePacket(MyInfoManager.Instance.Seq, (int)GetComponent<Weapon>().slot, curAmmo, ray.origin, ray.direction);
				switch (hIT_KIND)
				{
				case HIT_KIND.BRICK:
				{
					BrickManDesc[] array2 = BrickManManager.Instance.ToDescriptorArray();
					for (int j = 0; j < array2.Length; j++)
					{
						if (array2[j].Seq != MyInfoManager.Instance.Seq)
						{
							GameObject player = BrickManManager.Instance.Get(array2[j].Seq);
							switch (GeniusPacketSend.check3rdPersonSendOrNoSend(player, base.transform.position, hitInfo.point, ray.direction, hitInfo.normal, num, GetComponent<Weapon>().range, possibleCan: false))
							{
							case GeniusPacketSend.SEND_PACKET_LEVEL.ALL:
							{
								bool isBullet = false;
								if (shootObj == null)
								{
									firePacket.usID = (ushort)num2;
									isBullet = true;
								}
								HitBrickPacket cls3 = new HitBrickPacket(firePacket, isBullet, num, hitInfo.point, hitInfo.normal, destructable, layer, damage);
								P2PManager.Instance.SendPEER_HIT_BRICK_W(array2[j].Seq, cls3);
								break;
							}
							case GeniusPacketSend.SEND_PACKET_LEVEL.EXCEPT_EFFECT:
								P2PManager.Instance.SendPEER_FIRE_W(array2[j].Seq, firePacket);
								break;
							case GeniusPacketSend.SEND_PACKET_LEVEL.ONLY_SOUND:
							{
								FireSndPacket cls2 = new FireSndPacket(MyInfoManager.Instance.Seq, (int)GetComponent<Weapon>().slot);
								P2PManager.Instance.SendPEER_FIRE_ACTION_W(array2[j].Seq, cls2);
								break;
							}
							}
						}
					}
					break;
				}
				case HIT_KIND.HUMAN:
				{
					HitManPacket hitManPacket = new HitManPacket(firePacket, num3, hitPart, hitInfo.point, hitInfo.normal, damage, rigidity, (int)weaponBy, lucky);
					Camera camera = null;
					GameObject gameObject10 = GameObject.Find("Main Camera");
					if (null != gameObject10)
					{
						camera = gameObject10.GetComponent<Camera>();
						camera.enabled = false;
					}
					GameObject hitman = BrickManManager.Instance.Get(num3);
					BrickManDesc[] array = BrickManManager.Instance.ToDescriptorArray();
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i].Seq != MyInfoManager.Instance.Seq)
						{
							GameObject gameObject11 = BrickManManager.Instance.Get(array[i].Seq);
							TPController component11 = gameObject11.GetComponent<TPController>();
							if (component11 != null && !MyInfoManager.Instance.IsBelow12() && component11.IsChild)
							{
								hitManPacket.weaponBy = (ushort)weaponByForChild;
							}
							AudioSource component12 = gameObject11.GetComponent<AudioSource>();
							if (component12 != null)
							{
								component12.PlayOneShot(GlobalVars.Instance.playerHit);
							}
							switch (GeniusPacketSend.checkHITMAN(hitman, gameObject11, base.transform.position))
							{
							case GeniusPacketSend.SEND_PACKET_LEVEL.ALL:
								P2PManager.Instance.SendPEER_HIT_BRICKMAN_W(array[i].Seq, hitManPacket);
								break;
							case GeniusPacketSend.SEND_PACKET_LEVEL.EXCEPT_EFFECT:
								P2PManager.Instance.SendPEER_FIRE_W(array[i].Seq, firePacket);
								break;
							case GeniusPacketSend.SEND_PACKET_LEVEL.ONLY_SOUND:
								P2PManager.Instance.SendPEER_FIRE_W(array[i].Seq, firePacket);
								break;
							}
						}
					}
					camera.enabled = true;
					break;
				}
				case HIT_KIND.MON:
				{
					HitMonPacket cls = new HitMonPacket(firePacket, hitMonSeq, damage, rigidity, hitInfo.point);
					P2PManager.Instance.SendPEER_MON_HIT_NEW(cls);
					break;
				}
				case HIT_KIND.OTHER:
				{
					HitImpactPacket hitImpact = new HitImpactPacket(firePacket, layer, hitInfo.point, hitInfo.normal);
					P2PManager.Instance.SendPEER_HIT_IMPACT_NEW(hitImpact);
					break;
				}
				default:
					P2PManager.Instance.SendPEER_FIRE_NEW(firePacket);
					break;
				}
			}
		}
	}

	private bool updateCollideLauncher2(Vector3 p1, Vector3 p2)
	{
		bool result = false;
		GameObject gameObject = GameObject.Find("Me");
		if (gameObject != null)
		{
			LocalController component = gameObject.GetComponent<LocalController>();
			if (component != null)
			{
				component.IDidSomething();
			}
		}
		int layerMask = (1 << LayerMask.NameToLayer("Chunk")) | (1 << LayerMask.NameToLayer("Brick")) | (1 << LayerMask.NameToLayer("BoxMan")) | (1 << LayerMask.NameToLayer("Mon")) | (1 << LayerMask.NameToLayer("InvincibleArmor")) | (1 << LayerMask.NameToLayer("Bomb")) | (1 << LayerMask.NameToLayer("InstalledBomb"));
		Vector3 direction = p2 - p1;
		direction.Normalize();
		float maxDistance = Vector3.Distance(p2, p1);
		if (Physics.SphereCast(p1, 0.01f, direction, out RaycastHit hitInfo, maxDistance, layerMask))
		{
			WeaponFunction component2 = GetComponent<WeaponFunction>();
			if (null == component2)
			{
				Debug.LogError("wpnFunc == null");
			}
			TWeapon tWeapon = (TWeapon)GetComponent<Weapon>().tItem;
			if (tWeapon == null)
			{
				Debug.LogError("wpn == null");
			}
			Item item = MyInfoManager.Instance.GetItemBySequence(component2.ItemSeq);
			if (item == null)
			{
				item = MyInfoManager.Instance.GetUsingEquipByCode(tWeapon.code);
			}
			int boomDamage = GlobalVars.Instance.applyDurabilityDamage(item?.Durability ?? (-1), tWeapon.durabilityMax, CalcDamage2ndWpn());
			ExplosionUtil.CheckMyself(hitInfo.point, boomDamage, Radius2ndWpn, (int)weaponBy);
			ExplosionUtil.CheckBoxmen(hitInfo.point, boomDamage, Radius2ndWpn, (int)weaponBy, Rigidity);
			ExplosionUtil.CheckMonster(hitInfo.point, boomDamage, Radius2ndWpn);
			ExplosionUtil.CheckDestructibles(hitInfo.point, boomDamage, Radius2ndWpn);
			misObj.transform.position = hitInfo.point;
			result = true;
		}
		return result;
	}

	private bool updateCollideLauncher1(bool collideEnter, Vector3 p1, Vector3 p2)
	{
		bool result = false;
		if (IsCrossbowType())
		{
			GameObject gameObject = GameObject.Find("Me");
			if (gameObject != null)
			{
				LocalController component = gameObject.GetComponent<LocalController>();
				if (component != null)
				{
					component.IDidSomething();
				}
			}
			hitMonSeq = -1;
			hitBrickSeq = -1;
			IsBrickDead = false;
			Vector3 vector = p2 - p1;
			vector.Normalize();
			float maxDistance = Vector3.Distance(p2, p1);
			int layerMask = (1 << LayerMask.NameToLayer("Chunk")) | (1 << LayerMask.NameToLayer("Brick")) | (1 << LayerMask.NameToLayer("BoxMan")) | (1 << LayerMask.NameToLayer("Mon")) | (1 << LayerMask.NameToLayer("InvincibleArmor")) | (1 << LayerMask.NameToLayer("Bomb")) | (1 << LayerMask.NameToLayer("InstalledBomb"));
			if (Physics.Raycast(p1, vector, out RaycastHit hitInfo, maxDistance, layerMask))
			{
				GameObject gameObject2 = hitInfo.transform.gameObject;
				hitPoint = hitInfo.point;
				if (gameObject2.layer == LayerMask.NameToLayer("Brick") || gameObject2.layer == LayerMask.NameToLayer("Chunk"))
				{
					GameObject gameObject3 = null;
					BrickProperty brickProperty = null;
					GameObject gameObject4 = null;
					if (gameObject2.layer == LayerMask.NameToLayer("Brick"))
					{
						BrickProperty[] allComponents = Recursively.GetAllComponents<BrickProperty>(gameObject2.transform, includeInactive: false);
						if (allComponents.Length > 0)
						{
							brickProperty = allComponents[0];
						}
					}
					else
					{
						gameObject3 = BrickManager.Instance.GetBrickObjectByPos(Brick.ToBrickCoord(hitInfo.normal, hitInfo.point));
						if (null != gameObject3)
						{
							brickProperty = gameObject3.GetComponent<BrickProperty>();
						}
					}
					if (null != brickProperty)
					{
						gameObject4 = BrickManager.Instance.GetBulletImpact(brickProperty.Index);
						Brick brick = BrickManager.Instance.GetBrick(brickProperty.Index);
						if (brick != null)
						{
							if (brick.destructible)
							{
								hitBrickSeq = brickProperty.Seq;
								P2PManager.Instance.SendPEER_HIT_BRICK(hitBrickSeq, missileUniqSeq, hitInfo.point, vector, isBullet: false);
								brickProperty.Hit((int)CalcAtkPow(Vector3.Distance(hitInfo.point, base.transform.position)));
								if (brickProperty.HitPoint <= 0)
								{
									CSNetManager.Instance.Sock.SendCS_DESTROY_BRICK_REQ(brickProperty.Seq);
									gameObject4 = null;
									IsBrickDead = true;
									if (brickProperty.Index == 115 || brickProperty.Index == 193)
									{
										ExplosionUtil.CheckMyself(gameObject3.transform.position, GlobalVars.Instance.BoomDamage, GlobalVars.Instance.BoomRadius, -3);
										ExplosionUtil.CheckBoxmen(gameObject3.transform.position, GlobalVars.Instance.BoomDamage, GlobalVars.Instance.BoomRadius, -3, Rigidity);
										ExplosionUtil.CheckMonster(gameObject3.transform.position, GlobalVars.Instance.BoomDamage, GlobalVars.Instance.BoomRadius);
										ExplosionUtil.CheckDestructibles(gameObject3.transform.position, GlobalVars.Instance.BoomDamage, GlobalVars.Instance.BoomRadius);
									}
								}
								else
								{
									P2PManager.Instance.SendPEER_BRICK_HITPOINT(brickProperty.Seq, brickProperty.HitPoint);
									GameObject impact = VfxOptimizer.Instance.GetImpact(gameObject2.layer);
									if (null != impact)
									{
										Object.Instantiate((Object)impact, hitInfo.point, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
										P2PManager.Instance.SendPEER_HIT_IMPACT(gameObject2.layer, hitInfo.point, hitInfo.normal);
									}
								}
							}
							else
							{
								P2PManager.Instance.SendPEER_HIT_BRICK(-1, missileUniqSeq, hitInfo.point, vector, isBullet: false);
							}
							result = true;
						}
					}
					if (null != gameObject4)
					{
						Object.Instantiate((Object)gameObject4, hitInfo.point, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
					}
				}
				else if (gameObject2.layer == LayerMask.NameToLayer("BoxMan"))
				{
					PlayerProperty[] allComponents2 = Recursively.GetAllComponents<PlayerProperty>(gameObject2.transform, includeInactive: false);
					TPController[] allComponents3 = Recursively.GetAllComponents<TPController>(gameObject2.transform, includeInactive: false);
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
						HitPart component2 = gameObject2.GetComponent<HitPart>();
						if (component2 != null)
						{
							bool flag = false;
							if (component2.part == HitPart.TYPE.HEAD)
							{
								Scope component3 = GetComponent<Scope>();
								int layerMask2 = 1 << LayerMask.NameToLayer("Brain");
								if (null != component3)
								{
									layerMask2 = 1 << LayerMask.NameToLayer("CoreBrain");
								}
								if (Physics.Raycast(p1, vector, out RaycastHit hitInfo2, maxDistance, layerMask2))
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
								GameObject original = component2.GetHitImpact();
								if (flag && null != component2.luckyImpact)
								{
									original = component2.luckyImpact;
								}
								Object.Instantiate((Object)original, hitInfo.point, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
							}
							num = (int)(CalcAtkPow(Vector3.Distance(hitInfo.point, base.transform.position)) * component2.damageFactor);
							if (!playerProperty.IsHostile())
							{
								num = 0;
							}
							WeaponFunction component4 = GetComponent<WeaponFunction>();
							if (null == component4)
							{
								Debug.LogError("wpnFucn == null");
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
							num = GlobalVars.Instance.applyDurabilityDamage(item?.Durability ?? (-1), tWeapon.durabilityMax, num);
							P2PManager.Instance.SendPEER_HIT_BRICKMAN(MyInfoManager.Instance.Seq, playerProperty.Desc.Seq, (int)component2.part, hitInfo.point, hitInfo.normal, flag, missileUniqSeq, vector);
							P2PManager.Instance.SendPEER_SHOOT(MyInfoManager.Instance.Seq, playerProperty.Desc.Seq, num, Rigidity, (int)weaponBy, (int)component2.part, flag, rateOfFire);
						}
						GlobalVars.Instance.hitParent = component2.transform;
						GlobalVars.Instance.hitBirckman = playerProperty.Desc.Seq;
						tPController.GetHit(num, playerProperty.Desc.Seq);
						result = true;
					}
				}
				else if (gameObject2.layer == LayerMask.NameToLayer("Mon"))
				{
					MonProperty[] allComponents4 = Recursively.GetAllComponents<MonProperty>(gameObject2.transform, includeInactive: false);
					MonController[] allComponents5 = Recursively.GetAllComponents<MonController>(gameObject2.transform, includeInactive: false);
					MonProperty monProperty = null;
					MonController x = null;
					if (allComponents4.Length > 0)
					{
						monProperty = allComponents4[0];
					}
					if (allComponents5.Length > 0)
					{
						x = allComponents5[0];
					}
					if (monProperty != null && x != null)
					{
						if (MyInfoManager.Instance.Slot < 4 && monProperty.Desc.bRedTeam)
						{
							return false;
						}
						if (MyInfoManager.Instance.Slot >= 4 && !monProperty.Desc.bRedTeam)
						{
							return false;
						}
						HitPart component5 = gameObject2.GetComponent<HitPart>();
						if (component5 != null)
						{
							if (monProperty.Desc.typeID == 2)
							{
								aiHide aiHide = (aiHide)MonManager.Instance.GetAIClass(monProperty.Desc.Seq, monProperty.Desc.tblID);
								if (aiHide != null)
								{
									if (aiHide.IsHide)
									{
										return true;
									}
									if (aiHide.CanApply)
									{
										aiHide.setHide(bSet: true);
									}
								}
							}
							if (component5.GetHitImpact() != null)
							{
								Object.Instantiate((Object)component5.GetHitImpact(), hitInfo.point, Quaternion.Euler(0f, 0f, 0f));
							}
							if (monProperty.Desc.Xp <= 0)
							{
								return true;
							}
							int num2 = (int)(CalcAtkPow(Vector3.Distance(hitInfo.point, base.transform.position)) * component5.damageFactor);
							num2 += AddUpgradedDamagei(category);
							if (monProperty.Desc.bHalfDamage)
							{
								num2 /= 2;
							}
							monProperty.Desc.rigidity = Rigidity + AddUpgradedShockf(category);
							if (monProperty.Desc.rigidity > 1f)
							{
								monProperty.Desc.rigidity = 1f;
							}
							GlobalVars.Instance.hitParent = component5.transform;
							GlobalVars.Instance.hitBirckman = monProperty.Desc.Seq;
							hitMonSeq = monProperty.Desc.Seq;
							MonManager.Instance.Hit(monProperty.Desc.Seq, num2, monProperty.Desc.rigidity, (int)weaponBy, hitInfo.point, hitInfo.normal, curAmmo);
						}
						result = true;
					}
				}
				else if (gameObject2.layer == LayerMask.NameToLayer("InvincibleArmor") || gameObject2.layer == LayerMask.NameToLayer("Bomb") || gameObject2.layer == LayerMask.NameToLayer("InstalledBomb"))
				{
					GameObject impact2 = VfxOptimizer.Instance.GetImpact(gameObject2.layer);
					if (null != impact2)
					{
						Object.Instantiate((Object)impact2, hitInfo.point, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
						P2PManager.Instance.SendPEER_HIT_IMPACT(gameObject2.layer, hitInfo.point, hitInfo.normal);
					}
					result = true;
				}
			}
		}
		else
		{
			GameObject gameObject5 = GameObject.Find("Me");
			if (gameObject5 != null)
			{
				LocalController component6 = gameObject5.GetComponent<LocalController>();
				if (component6 != null)
				{
					component6.IDidSomething();
				}
			}
			int layerMask3 = (1 << LayerMask.NameToLayer("Chunk")) | (1 << LayerMask.NameToLayer("Brick")) | (1 << LayerMask.NameToLayer("BoxMan")) | (1 << LayerMask.NameToLayer("Mon")) | (1 << LayerMask.NameToLayer("InvincibleArmor")) | (1 << LayerMask.NameToLayer("Bomb")) | (1 << LayerMask.NameToLayer("InstalledBomb"));
			Vector3 direction = p2 - p1;
			direction.Normalize();
			float maxDistance2 = Vector3.Distance(p2, p1);
			RaycastHit hitInfo3 = default(RaycastHit);
			if (collideEnter || Physics.Raycast(p1, direction, out hitInfo3, maxDistance2, layerMask3))
			{
				if (collideEnter)
				{
					hitInfo3.point = hitPoint;
				}
				else
				{
					hitPoint = hitInfo3.point;
				}
				WeaponFunction component7 = GetComponent<WeaponFunction>();
				if (null == component7)
				{
					Debug.LogError("wpnFunc == nulll ");
				}
				TWeapon tWeapon2 = (TWeapon)GetComponent<Weapon>().tItem;
				if (tWeapon2 == null)
				{
					Debug.LogError("wpn == null");
				}
				Item item2 = MyInfoManager.Instance.GetItemBySequence(component7.ItemSeq);
				if (item2 == null)
				{
					item2 = MyInfoManager.Instance.GetUsingEquipByCode(tWeapon2.code);
				}
				int boomDamage = GlobalVars.Instance.applyDurabilityDamage(item2?.Durability ?? (-1), tWeapon2.durabilityMax, (int)CalcAtkPow());
				ExplosionUtil.CheckMyself(hitInfo3.point, boomDamage, Radius1stWpn, (int)weaponBy);
				ExplosionUtil.CheckBoxmen(hitInfo3.point, boomDamage, Radius1stWpn, (int)weaponBy, Rigidity);
				ExplosionUtil.CheckMonster(hitInfo3.point, boomDamage, Radius1stWpn);
				ExplosionUtil.CheckDestructibles(hitInfo3.point, boomDamage, Radius1stWpn);
				result = true;
			}
		}
		return result;
	}

	public bool CanReload()
	{
		return drawn && curAmmo > 0 && magazine.CanReload();
	}

	public void ClipOut()
	{
		cyclic = false;
		reloading = true;
		base.animation.Play("empty");
		GetComponent<Weapon>().GadgetSound(Weapon.GADGET.CLIPOUT);
		Scope component = GetComponent<Scope>();
		if (null != component)
		{
			if (component.IsZooming())
			{
				component.ToggleScoping(forceApply: true);
			}
			component.SetAiming(_aiming: false);
		}
		GetComponent<Aim>().SetAiming(_aiming: false);
	}

	public void ClipIn()
	{
		if (curAmmo > 0)
		{
			curAmmo = magazine.Reload(curAmmo);
			if (!leftHand)
			{
				NoCheat.Instance.Sync(wdAmmo, curAmmo);
				NoCheat.Instance.Sync(wdMagazine, magazine.Cur);
			}
		}
		GetComponent<Weapon>().GadgetSound(Weapon.GADGET.CLIPIN);
	}

	public void BoltUp()
	{
		reloading = false;
		base.animation.Play("idle");
		P2PManager.Instance.SendPEER_GUN_ANIM(MyInfoManager.Instance.Seq, (sbyte)GetComponent<Weapon>().slot, 0);
		GetComponent<Weapon>().GadgetSound(Weapon.GADGET.BOLTUP);
		Scope component = GetComponent<Scope>();
		if (null != component)
		{
			component.SetAiming(_aiming: true);
		}
		GetComponent<Aim>().SetAiming(component == null || !component.Scoping);
	}

	public override void SetDrawn(bool draw)
	{
		cyclic = false;
		drawn = draw;
		if (drawn)
		{
			Restart();
		}
	}

	private void DrawAmmo()
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
				magazineFont.Print(new Vector2((float)(Screen.width - 70), (float)(Screen.height - 12)), NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.MAIN_MAGAZINE, magazine.Cur));
				if (IsFontScaled)
				{
					ammoFont.Scale = curFontScale;
				}
				ammoFont.Print(new Vector2((float)(Screen.width - 10), (float)(Screen.height - 12)), curAmmo);
				if (Launcher != LAUNCHER.NONE)
				{
					num2 = (float)iconLauncher.width * num / (float)iconLauncher.height;
					TextureUtil.DrawTexture(new Rect((float)(Screen.width - 200), (float)Screen.height - num - 12f, num2, num), iconLauncher, ScaleMode.StretchToFill);
					ammoFont.Print(new Vector2((float)(Screen.width - 135), (float)(Screen.height - 12)), NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.MAIN_AMMO2, maxLauncherAmmoGame));
				}
			}
		}
	}

	private void OnGUI()
	{
		if (!leftHand)
		{
			GUISkin skin = GUI.skin;
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			DrawAmmo();
			GUI.enabled = true;
			GUI.skin = skin;
		}
	}

	public int GetNextAtkValue()
	{
		return NextUpgradedDamagei(category);
	}

	public float GetNextShockValue()
	{
		return NextUpgradedShockf(category);
	}

	public int GetNextChargeValue()
	{
		return NextUpgradedChargei(category);
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
		if (DefenseManager.Instance.IsBrickmode)
		{
			num2 -= num2 * DefenseManager.Instance.GetPurcharseItem(3).incAtkSpeed;
		}
		return deltaTime < num2;
	}

	public void ExpandAmmo()
	{
		maxAmmoInst += AddUpgradedChargei(category);
	}

	protected virtual void Fire()
	{
		if (!IsCoolDown())
		{
			if (!magazine.Fire())
			{
				bool flag = true;
				if (CanReload())
				{
					GameObject gameObject = GameObject.Find("Main");
					ShooterTools component = gameObject.GetComponent<ShooterTools>();
					if (BuildOption.Instance.Props.useDefaultAutoReload || (null != component && component.Use("auto_reload")))
					{
						flag = false;
						localCtrl.AutoReload();
					}
				}
				if (flag)
				{
					base.animation.Play("empty");
					EmptySound();
					MyInfoManager.Instance.IsGunEmpty = true;
					P2PManager.Instance.SendPEER_GUN_ANIM(MyInfoManager.Instance.Seq, (sbyte)GetComponent<Weapon>().slot, 1);
				}
				cyclic = false;
			}
			else
			{
				if (MyInfoManager.Instance.IsGunEmpty)
				{
					MyInfoManager.Instance.IsGunEmpty = false;
				}
				NoCheat.Instance.Sync(wdMagazine, magazine.Cur);
				if (localCtrl != null)
				{
					localCtrl.DoFireAnimation(fireAnimation);
				}
				DoFireAnimation("fire");
				FireSound();
				deltaTime = 0f;
				if (IsMuzzle)
				{
					CreateMuzzleFire();
					Shoot();
				}
				else
				{
					CreateBigShoot();
				}
				if (camCtrl != null)
				{
					camCtrl.Pitchup(recoilPitch, recoilYaw);
				}
				if (GetComponent<Aim>() != null)
				{
					GetComponent<Aim>().Inaccurate(localCtrl.CanAimAccuratelyMore());
				}
				Scope component2 = GetComponent<Scope>();
				if (null != component2)
				{
					component2.HandleFireEvent(localCtrl.CanAimAccuratelyMore());
					if (tutoInput != null)
					{
						if (component2.IsZooming())
						{
							tutoInput.setActive(TUTO_INPUT.M_L);
							tutoInput.setClick(TUTO_INPUT.M_L);
						}
						else
						{
							tutoInput.setActive(TUTO_INPUT.M_R);
							tutoInput.setClick(TUTO_INPUT.M_R);
						}
					}
				}
				if (semiAuto)
				{
					curSemiAutoMaxCyclicAmmo--;
					if (curSemiAutoMaxCyclicAmmo == 0)
					{
						cyclic = false;
					}
				}
			}
		}
	}

	private void CreateBigShoot()
	{
		if (!(null == muzzleFire) && !(null == muzzle))
		{
			Vector2 vector = CalcDeflection();
			shootRay = cam.ScreenPointToRay(new Vector3(vector.x, vector.y, 0f));
			Vector3 vector2 = shootRay.origin + shootRay.direction * 1.5f;
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
			misObj = (Object.Instantiate((Object)shootObj) as GameObject);
			misObj.transform.position = vector2;
			misObj.transform.forward = shootRay.direction;
			MissleInfo missleInfo = new MissleInfo();
			missleInfo.uniq = MyInfoManager.Instance.Slot * 400 + misSeq;
			missleInfo.obj = misObj;
			missleInfo.prepos = vector2 - shootRay.direction * backward;
			dicMis.Add(missleInfo.uniq, missleInfo);
			Transform[] componentsInChildren = misObj.GetComponentsInChildren<Transform>();
			int num = 0;
			while (misSmokeTrans == null && num < componentsInChildren.Length)
			{
				if (componentsInChildren[num].name.Contains("Effect_Dummy"))
				{
					misSmokeTrans = componentsInChildren[num];
				}
				num++;
			}
			if (IsCrossbowType() || IsStaffType())
			{
				misSmokeEff = null;
			}
			else
			{
				misSmokeEff = (Object.Instantiate((Object)GlobalVars.Instance.missileSmokeEff) as GameObject);
				misSmokeEff.transform.position = misSmokeTrans.position;
				misSmokeEff.transform.parent = misSmokeTrans;
			}
			isMisFired = true;
			dtCollide = 0f;
			dtMissile = 0f;
			misDir = shootRay.direction;
			Rigidbody component = misObj.GetComponent<Rigidbody>();
			if (null != component)
			{
				component.AddForce(shootRay.direction * misSpeed, ForceMode.Impulse);
			}
			P2PManager.Instance.SendPEER_BIG_FIRE(MyInfoManager.Instance.Seq, (int)GetComponent<Weapon>().slot, missleInfo.uniq, vector2, shootRay.direction);
			misSeq++;
		}
	}

	protected void DoFireAnimation(string fireAnimation)
	{
		if (!base.animation.IsPlaying(fireAnimation))
		{
			base.animation.Play(fireAnimation);
		}
		else
		{
			float length = base.animation[fireAnimation].length;
			base.animation[fireAnimation].time = length / 4f;
		}
	}

	protected void EmptySound()
	{
		GetComponent<Weapon>().GadgetSound(Weapon.GADGET.DRYFIRE);
	}

	protected void FireSound()
	{
		GetComponent<Weapon>().FireSound();
	}

	private void FireSpecialSound()
	{
		AudioSource component = GetComponent<AudioSource>();
		if (null != component && null != fireSoundSpecial)
		{
			component.PlayOneShot(fireSoundSpecial);
		}
	}

	private void FlyingSpecialSound()
	{
		AudioSource component = GetComponent<AudioSource>();
		if (null != component && null != flyingSoundSpecial)
		{
			component.clip = flyingSoundSpecial;
			component.loop = true;
			component.Play();
		}
	}

	private void FlyingSpecialSoundStop()
	{
		AudioSource component = GetComponent<AudioSource>();
		if (null != component && null != flyingSoundSpecial)
		{
			component.clip = flyingSoundSpecial;
			component.Stop();
		}
	}

	private bool CanFire()
	{
		return localCtrl.CanControl() && !reloading && drawn;
	}

	private GameObject GetMuzzleFireEffByLauncher()
	{
		LAUNCHER launcher = Launcher;
		if (launcher == LAUNCHER.GRANADE || launcher == LAUNCHER.ROCKET)
		{
			return GlobalVars.Instance.misslieMuzzleFireEff;
		}
		Debug.LogError("(GetMuzzleFireEffByLauncher)no exist launcher: " + Launcher);
		return null;
	}

	private GameObject GetMissileByLauncher()
	{
		switch (Launcher)
		{
		case LAUNCHER.GRANADE:
			return GlobalVars.Instance.missileObj;
		case LAUNCHER.ROCKET:
			return GlobalVars.Instance.rocketObj;
		default:
			Debug.LogError("(GetMissileByLauncher)no exist launcher: " + Launcher);
			return null;
		}
	}

	private GameObject GetSmokeEffByLauncher()
	{
		LAUNCHER launcher = Launcher;
		if (launcher == LAUNCHER.GRANADE || launcher == LAUNCHER.ROCKET)
		{
			return GlobalVars.Instance.missileSmokeEff;
		}
		Debug.LogError("(GetSmokeEffByLauncher)no exist launcher: " + Launcher);
		return null;
	}

	private GameObject GetExplosionEffByLauncher()
	{
		if (!BuildOption.Instance.IsNetmarble && !BuildOption.Instance.IsDeveloper)
		{
			if (!BuildOption.Instance.Props.useUskMuzzleEff || !applyUsk)
			{
				LAUNCHER launcher = Launcher;
				if (launcher == LAUNCHER.GRANADE || launcher == LAUNCHER.ROCKET)
				{
					return GlobalVars.Instance.missileExplosionEff;
				}
			}
			else if (GlobalVars.Instance.explosionUsk != null)
			{
				return GlobalVars.Instance.explosionUsk;
			}
		}
		else
		{
			if (MyInfoManager.Instance.IsBelow12())
			{
				return GlobalVars.Instance.missileExplosionEff11;
			}
			LAUNCHER launcher = Launcher;
			if (launcher == LAUNCHER.GRANADE || launcher == LAUNCHER.ROCKET)
			{
				return GlobalVars.Instance.missileExplosionEff;
			}
		}
		return null;
	}

	private void CheckFire1()
	{
		if (CanFire())
		{
			if (custom_inputs.Instance.GetButtonDown("K_FIRE1"))
			{
				cyclic = true;
				if (semiAuto)
				{
					curSemiAutoMaxCyclicAmmo = semiAutoMaxCyclicAmmo;
				}
				Fire();
			}
			if (custom_inputs.Instance.GetButtonUp("K_FIRE1"))
			{
				cyclic = false;
			}
			if (canCyclic && cyclic)
			{
				Fire();
			}
		}
	}

	public void scopeOff()
	{
		Scope component = GetComponent<Scope>();
		if (null != component)
		{
			if (component.IsZooming())
			{
				component.ToggleScoping(forceApply: true);
			}
			component.SetAiming(_aiming: false);
			Aim component2 = GetComponent<Aim>();
			if (component2 != null || component2.enabled)
			{
				GetComponent<Aim>().SetAiming(_aiming: true);
			}
		}
	}

	private void CheckFire2()
	{
		if (CanFire() && !IsCoolDown() && custom_inputs.Instance.GetButtonDown("K_FIRE2"))
		{
			Scope component = GetComponent<Scope>();
			if (null != component)
			{
				GetComponent<Aim>().SetAiming(component.ToggleScoping());
				if (tutoInput != null)
				{
					if (component.IsZooming())
					{
						tutoInput.setActive(TUTO_INPUT.M_L);
						tutoInput.setClick(TUTO_INPUT.M_L);
					}
					else
					{
						tutoInput.setActive(TUTO_INPUT.M_R);
						tutoInput.setClick(TUTO_INPUT.M_R);
					}
				}
			}
			else if (Launcher != LAUNCHER.NONE && !isMisFired && maxLauncherAmmoGame > 0)
			{
				Vector2 vector = CalcDeflection();
				shootRay = cam.ScreenPointToRay(new Vector3(vector.x, vector.y, 0f));
				Vector3 vector2 = shootRay.origin + shootRay.direction * 1.5f;
				if (muzzleFxInstance2 == null)
				{
					GameObject gameObject = Object.Instantiate((Object)GetMuzzleFireEffByLauncher()) as GameObject;
					Recursively.SetLayer(gameObject.GetComponent<Transform>(), LayerMask.NameToLayer("FPWeapon"));
					gameObject.transform.position = misMuzzleTrans.position;
					gameObject.transform.parent = misMuzzleTrans;
					gameObject.transform.localRotation = Quaternion.Euler(90f, 90f, 0f);
					muzzleFxInstance2 = gameObject;
				}
				ParticleEmitter particleEmitter = null;
				int childCount = muzzleFxInstance2.transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					Transform child = muzzleFxInstance2.transform.GetChild(i);
					particleEmitter = child.GetComponent<ParticleEmitter>();
					if ((bool)particleEmitter)
					{
						particleEmitter.Emit();
					}
				}
				misObj = (Object.Instantiate((Object)GetMissileByLauncher()) as GameObject);
				misObj.transform.position = vector2;
				misObj.transform.forward = shootRay.direction;
				Transform[] componentsInChildren = misObj.GetComponentsInChildren<Transform>();
				int num = 0;
				while (misSmokeTrans == null && num < componentsInChildren.Length)
				{
					if (componentsInChildren[num].name.Contains("Effect_Dummy"))
					{
						misSmokeTrans = componentsInChildren[num];
					}
					num++;
				}
				misSmokeEff = (Object.Instantiate((Object)GetSmokeEffByLauncher()) as GameObject);
				misSmokeEff.transform.position = vector2;
				misSmokeEff.transform.parent = misSmokeTrans;
				misDir = shootRay.direction;
				GameObject x = GameObject.Find("Me");
				if (x == null)
				{
					Debug.LogError("me error");
				}
				misPrePos = vector2 - shootRay.direction * backward;
				isMisFired = true;
				base.animation.Play("bigfire");
				base.animation["bigfire"].wrapMode = WrapMode.Once;
				Rigidbody component2 = misObj.GetComponent<Rigidbody>();
				if (null != component2)
				{
					component2.AddForce(shootRay.direction * ThrowForce, ForceMode.Impulse);
				}
				P2PManager.Instance.SendPEER_BIG_WPN_FIRE(MyInfoManager.Instance.Seq, (int)Launcher, MyInfoManager.Instance.Slot, misObj.transform.position, shootRay.direction);
				maxLauncherAmmoGame = NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.MAIN_AMMO2, maxLauncherAmmoGame) - 1;
				maxLauncherAmmoGame = NoCheat.Instance.HideVal(NoCheat.WATCH_DOG.MAIN_AMMO2, maxLauncherAmmoGame);
				if (wdAmmo2 == NoCheat.WATCH_DOG.MAIN_AMMO2)
				{
					NoCheat.Instance.Sync(wdAmmo2, maxLauncherAmmoGame);
				}
				FireSpecialSound();
				localCtrl.DoFireAnimation("sniping_h");
				DoFireAnimation("fire");
				deltaTime = 0f;
				camCtrl.Pitchup(recoilPitch2ndWpn, recoilYaw2ndWpn);
			}
		}
	}

	private MissleInfo[] ToMissileArray()
	{
		List<MissleInfo> list = new List<MissleInfo>();
		foreach (KeyValuePair<int, MissleInfo> dicMi in dicMis)
		{
			list.Add(dicMi.Value);
		}
		return list.ToArray();
	}

	public void collisionTest1st()
	{
		if (!IsMuzzle && !IsMuzzle && dicMis != null && dicMis.Count > 0)
		{
			MissleInfo[] array = ToMissileArray();
			for (int i = 0; i < array.Length; i++)
			{
				bool flag = false;
				bool collideEnter = false;
				GlobalVars.Instance.hitBirckman = -1;
				GlobalVars.Instance.hitParent = null;
				SelfCollsion component = array[i].obj.GetComponent<SelfCollsion>();
				if (component != null)
				{
					collideEnter = component.IsCollideEnter();
					hitPoint = component.colPoint;
				}
				missileUniqSeq = array[i].uniq;
				flag = updateCollideLauncher1(collideEnter, array[i].prepos, array[i].obj.transform.position);
				if (flag)
				{
					array[i].obj.transform.position = hitPoint;
				}
				array[i].prepos = array[i].obj.transform.position;
				if (IsCrossbowType())
				{
					array[i].obj.transform.position += misDir * (Time.deltaTime * misSpeed);
				}
				array[i].ElapsedP2P += Time.deltaTime;
				if (array[i].ElapsedP2P > BuildOption.Instance.Props.SendRate)
				{
					array[i].ElapsedP2P = 0f;
					P2PManager.Instance.SendPEER_BIG_WPN_FLY(MyInfoManager.Instance.Seq, array[i].uniq, array[i].obj.transform.position, array[i].obj.transform.forward);
				}
				array[i].Elapsed += Time.deltaTime;
				if (array[i].Elapsed > 4f || flag)
				{
					bool viewColeff = false;
					if (component != null)
					{
						if (IsBrickDead)
						{
							component.NoUse();
						}
						if (hitBrickSeq >= 0)
						{
							component.brickID = hitBrickSeq;
						}
						if (hitMonSeq >= 0)
						{
							component.monID = hitMonSeq;
						}
						if (flag)
						{
							viewColeff = true;
							component.Explosion(hitPoint, (!IsCrossbowType()) ? Quaternion.Euler(0f, 0f, 0f) : array[i].obj.transform.rotation, myself: true);
						}
						else
						{
							viewColeff = true;
							component.Explosion(array[i].obj.transform.position, (!IsCrossbowType()) ? Quaternion.Euler(0f, 0f, 0f) : array[i].obj.transform.rotation, myself: true);
						}
					}
					P2PManager.Instance.SendPEER_BIG_WPN_KABOOM(MyInfoManager.Instance.Seq, array[i].uniq, array[i].obj.transform.position, array[i].obj.transform.rotation.eulerAngles, viewColeff);
					Object.DestroyImmediate(array[i].obj);
					if (misSmokeEff != null)
					{
						Object.DestroyImmediate(misSmokeEff);
					}
					dtCollide = 0f;
					isMisFired = false;
					dicMis.Remove(array[i].uniq);
				}
			}
		}
	}

	public void collsionTest2nd()
	{
		if (Launcher != LAUNCHER.NONE && isMisFired)
		{
			dtCollide += Time.deltaTime;
			bool flag = false;
			flag = updateCollideLauncher2(misPrePos, misObj.transform.position);
			misPrePos = misObj.transform.position;
			if (Launcher != 0)
			{
				misObj.transform.position += misDir * (Time.deltaTime * misSpeed);
			}
			if (dtCollide > BuildOption.Instance.Props.SendRate)
			{
				dtCollide = 0f;
				P2PManager.Instance.SendPEER_BIG_WPN_FLY(MyInfoManager.Instance.Seq, MyInfoManager.Instance.Slot, misObj.transform.position, misObj.transform.forward);
			}
			dtMissile += Time.deltaTime;
			if (dtMissile > 5f || flag)
			{
				P2PManager.Instance.SendPEER_BIG_WPN_KABOOM(MyInfoManager.Instance.Seq, MyInfoManager.Instance.Slot, misObj.transform.position, misObj.transform.rotation.eulerAngles, viewColeff: true);
				Object.Instantiate((Object)GetExplosionEffByLauncher(), misObj.transform.position, Quaternion.Euler(0f, 0f, 0f));
				Object.DestroyImmediate(misObj);
				Object.DestroyImmediate(misSmokeEff);
				dtMissile = 0f;
				dtCollide = 0f;
				isMisFired = false;
			}
		}
	}

	private void UpdateAmmoFontScale()
	{
		if (IsFontScaled)
		{
			deltaTimeFontScale += Time.deltaTime;
			if (IsFontBig)
			{
				curFontScale += deltaTimeFontScale * 0.5f;
			}
			else
			{
				curFontScale -= deltaTimeFontScale * 0.1f;
			}
			if (curFontScale < minFontScale)
			{
				curFontScale = minFontScale;
			}
			if (curFontScale > maxFontScale)
			{
				curFontScale = maxFontScale;
				IsFontBig = false;
			}
			if (deltaTimeFontScale >= 2.5f)
			{
				IsFontScaled = false;
				deltaTimeFontScale = 0f;
			}
		}
	}

	private void Update()
	{
		if (Screen.lockCursor && BrickManager.Instance.IsLoaded)
		{
			if (!leftHand && !GlobalVars.Instance.cheatBlock)
			{
				NoCheat.Instance.KillCheater(wdAmmo, curAmmo);
				NoCheat.Instance.KillCheater(wdMagazine, magazine.Cur);
				if (wdAmmo2 == NoCheat.WATCH_DOG.MAIN_AMMO2)
				{
					NoCheat.Instance.KillCheater(wdAmmo2, maxLauncherAmmoGame);
				}
			}
			deltaTime += Time.deltaTime;
			VerifyCamera();
			VerifyLocalController();
			Aim component = GetComponent<Aim>();
			if (component != null)
			{
				component.Accurate(localCtrl.CanAimAccurately());
			}
			Scope component2 = GetComponent<Scope>();
			if (null != component2)
			{
				component2.Accurate(localCtrl.CanAimAccurately());
			}
			CheckFire1();
			CheckFire2();
			UpdateAmmoFontScale();
			if (!base.animation.IsPlaying("fire") && !base.animation.IsPlaying("empty") && !base.animation.IsPlaying("idle"))
			{
				base.animation.Play("idle");
			}
		}
	}

	private float CalcAtkPow()
	{
		float atkPow = AtkPow;
		return atkPow + WantedManager.Instance.GetWantedAtkPowBoost(MyInfoManager.Instance.Seq, atkPow);
	}

	private int CalcDamage2ndWpn()
	{
		float num = (float)Damage2ndWpn;
		num += WantedManager.Instance.GetWantedAtkPowBoost(MyInfoManager.Instance.Seq, num);
		return (int)num;
	}

	private float CalcAtkPow(float distance)
	{
		float atkPow = AtkPow;
		float range = GetComponent<Weapon>().range;
		float effectiveRange = GetComponent<Weapon>().effectiveRange;
		if (distance <= effectiveRange || range == float.PositiveInfinity || effectiveRange == float.PositiveInfinity)
		{
			return CalcAtkPow();
		}
		if (distance >= range || Mathf.Abs(range - effectiveRange) < 0.0001f)
		{
			return 0f;
		}
		float t = (distance - effectiveRange) / (range - effectiveRange);
		return (float)Mathf.CeilToInt(Mathf.Lerp(CalcAtkPow(), 0f, t));
	}

	private void SetBackword()
	{
		if (BackShotPosition || IsRightWeaponType())
		{
			backward = 1.5f;
		}
	}

	private bool IsCrossbowType()
	{
		return weaponBy == Weapon.BY.CROSSBOW_CONDOR || weaponBy == Weapon.BY.EAGLEEYE || weaponBy == Weapon.BY.CROSSBOW_CONDOR_G || weaponBy == Weapon.BY.EAGLEEYE_W || weaponBy == Weapon.BY.CROSSBOW_CONDOR_G_MAX || weaponBy == Weapon.BY.CROSSBOW_HORNET_S || weaponBy == Weapon.BY.CROSSBOW_HORNET_S_MAX || weaponBy == Weapon.BY.SHAFT || weaponBy == Weapon.BY.XWING || weaponBy == Weapon.BY.NEEDLE || weaponBy == Weapon.BY.MOONLIGHT || weaponBy == Weapon.BY.Q_CONDOR || weaponBy == Weapon.BY.Q_HORNET || weaponBy == Weapon.BY.SHAFT_MAX || weaponBy == Weapon.BY.XWING_MAX || weaponBy == Weapon.BY.MOONLIGHT_MAX || weaponBy == Weapon.BY.CROSSBOW_CONDOR_MAX || weaponBy == Weapon.BY.CROSSBOW_HORNET_MAX || weaponBy == Weapon.BY.EAGLEEYE_OR || weaponBy == Weapon.BY.SHAFT_OR || weaponBy == Weapon.BY.XWING_OR || weaponBy == Weapon.BY.NEEDLE_OR || weaponBy == Weapon.BY.CACTUS_GUN;
	}

	private bool IsStaffType()
	{
		return weaponBy == Weapon.BY.STAFF01 || weaponBy == Weapon.BY.STAFF01_G;
	}

	private bool IsCrossbowLauncherType()
	{
		return weaponBy == Weapon.BY.CROSSBOW_LAUNCHER || weaponBy == Weapon.BY.Q_BOOMSTICK || weaponBy == Weapon.BY.HELLFIRE || weaponBy == Weapon.BY.CROSSBOW_LAUNCHER_MAX || weaponBy == Weapon.BY.HELLFIRE_MAX;
	}

	private bool IsLauncherType()
	{
		return weaponBy == Weapon.BY.XMAS_BAZOOKA;
	}

	private bool IsRightWeaponType()
	{
		return Launcher != LAUNCHER.NONE;
	}
}
