using System.Collections.Generic;
using UnityEngine;

public class GdgtGun : WeaponGadget
{
	private Transform muzzle;

	private Transform shell;

	private GameObject muzzleFxInstance;

	private GameObject muzzleFxInstance2;

	public bool isMuzzle = true;

	public GameObject shootObj;

	public GameObject bulletObj;

	public Weapon.BY weaponBy;

	public LAUNCHER launcher = LAUNCHER.NONE;

	public bool carryAnim;

	private bool leftHand;

	private Dictionary<int, MissleInfo> dicMis;

	private GameObject misObj;

	private GameObject misSmokeEff;

	private Dictionary<int, ProjectileWrap> dic;

	public bool IsMuzzle
	{
		get
		{
			return isMuzzle;
		}
		set
		{
			isMuzzle = value;
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

	public LAUNCHER Launcher
	{
		get
		{
			return launcher;
		}
		set
		{
			launcher = value;
		}
	}

	public bool CarryAnim
	{
		get
		{
			return carryAnim;
		}
		set
		{
			carryAnim = value;
		}
	}

	public bool LeftHand
	{
		set
		{
			leftHand = value;
		}
	}

	public override void ClipOut()
	{
		GetComponent<Weapon>().GadgetSound(Weapon.GADGET.CLIPOUT);
	}

	public override void ClipIn()
	{
		GetComponent<Weapon>().GadgetSound(Weapon.GADGET.CLIPIN);
	}

	public override void BoltUp()
	{
		GetComponent<Weapon>().GadgetSound(Weapon.GADGET.BOLTUP);
	}

	private void CreateShell()
	{
		if (!(null == shell) && !(GetComponent<Weapon>().CatridgeOrClip == null))
		{
			VfxOptimizer.VFX_TYPE vfxType = VfxOptimizer.VFX_TYPE.SHELL;
			if (leftHand)
			{
				vfxType = VfxOptimizer.VFX_TYPE.SHELL2;
			}
			GameObject gameObject = VfxOptimizer.Instance.CreateFx(GetComponent<Weapon>().CatridgeOrClip, shell.position, Quaternion.Euler(0f, 0f, 0f), vfxType);
			if (null != gameObject)
			{
				gameObject.transform.parent = shell;
				gameObject.transform.localRotation = (leftHand ? Quaternion.Euler(-90f, 90f, 0f) : Quaternion.Euler(90f, 90f, 0f));
			}
		}
	}

	private void CreateMuzzleFire()
	{
		if (!(null == muzzle))
		{
			if (muzzleFxInstance == null)
			{
				GameObject gameObject = Object.Instantiate((Object)GetComponent<Weapon>().muzzleFire) as GameObject;
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

	public override void GunAnim(int anim)
	{
		switch (anim)
		{
		case 0:
			if (!base.animation.IsPlaying("idle"))
			{
				base.animation.Play("idle");
			}
			break;
		case 1:
			if (!base.animation.IsPlaying("empty"))
			{
				base.animation.Play("empty");
			}
			break;
		}
	}

	public override void FireAction()
	{
		FireSound();
		DoFireAnimation("fire");
	}

	private void FireSound()
	{
		GetComponent<Weapon>().FireSound();
	}

	private void Shoot(Vector3 origin, Vector3 direction)
	{
		GameObject gameObject = VfxOptimizer.Instance.CreateFx(GetComponent<Weapon>().BulletOrBody, origin, Quaternion.FromToRotation(Vector3.forward, direction), VfxOptimizer.VFX_TYPE.BULLET_TRAIL);
		if (null != gameObject)
		{
			gameObject.GetComponent<Bullet>().Speed = 600f;
		}
	}

	public override void Fire(int tile, Vector3 origin, Vector3 direction)
	{
		FireSound();
		if (IsMuzzle)
		{
			CreateMuzzleFire();
			CreateShell();
			Shoot(origin, direction);
		}
		else
		{
			CreateBigShoot(tile, origin, direction);
		}
		DoFireAnimation("fire");
	}

	private void CreateBigShoot(int projectile, Vector3 origin, Vector3 direction)
	{
		if (!(null == muzzle) && !(null == shootObj))
		{
			if (muzzleFxInstance == null)
			{
				GameObject gameObject = Object.Instantiate((Object)GetComponent<Weapon>().muzzleFire) as GameObject;
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
			misObj.transform.position = origin;
			misObj.transform.rotation = Quaternion.FromToRotation(Vector3.forward, direction);
			if (IsCrossbowType() || IsStaffType())
			{
				misSmokeEff = null;
			}
			else
			{
				misSmokeEff = (Object.Instantiate((Object)GlobalVars.Instance.missileSmokeEff) as GameObject);
				misSmokeEff.transform.position = misObj.transform.position;
				misSmokeEff.transform.parent = misObj.transform;
			}
			dic.Add(projectile, new ProjectileWrap(misObj));
		}
	}

	public void destroyBullet(int bulletID)
	{
		if (!isMuzzle && dic.ContainsKey(bulletID))
		{
			ProjectileWrap projectileWrap = dic[bulletID];
			if (projectileWrap != null)
			{
				Object.DestroyImmediate(projectileWrap.projectile);
				dic.Remove(bulletID);
			}
		}
	}

	public void createBullet(Vector3 pnt, Vector3 nml, int brickSeq, int bulletId, int hitState)
	{
		if (!(bulletObj == null))
		{
			if (!isMuzzle)
			{
				if (dic.ContainsKey(bulletId))
				{
					ProjectileWrap projectileWrap = dic[bulletId];
					if (projectileWrap != null)
					{
						Object.DestroyImmediate(projectileWrap.projectile);
						dic.Remove(bulletId);
					}
				}
				if (!(bulletObj == null))
				{
					GameObject gameObject = Object.Instantiate((Object)bulletObj) as GameObject;
					gameObject.transform.position = pnt;
					gameObject.transform.forward = nml;
					if (brickSeq >= 0 && hitState == 1)
					{
						CheckBrickDead component = gameObject.GetComponent<CheckBrickDead>();
						component.brickID = brickSeq;
					}
					if (brickSeq >= 0 && hitState == 2)
					{
						CheckMonDead component2 = gameObject.GetComponent<CheckMonDead>();
						component2.MonID = brickSeq;
					}
					if (hitState == 0 || hitState == 2)
					{
						ParentFollow component3 = gameObject.GetComponent<ParentFollow>();
						if (component3 != null)
						{
							component3.HitParent = GlobalVars.Instance.hitParent;
							component3.ParentSeq = GlobalVars.Instance.hitBirckman;
							GlobalVars.Instance.hitParent = null;
							GlobalVars.Instance.hitBirckman = -1;
						}
					}
				}
			}
			else
			{
				GameObject gameObject2 = Object.Instantiate((Object)bulletObj) as GameObject;
				gameObject2.transform.position = pnt;
				gameObject2.transform.forward = nml;
				if (brickSeq >= 0 && hitState == 1)
				{
					CheckBrickDead component4 = gameObject2.GetComponent<CheckBrickDead>();
					component4.brickID = brickSeq;
				}
				if (brickSeq >= 0 && hitState == 2)
				{
					CheckMonDead component5 = gameObject2.GetComponent<CheckMonDead>();
					component5.MonID = brickSeq;
				}
				if (hitState == 0 || hitState == 2)
				{
					ParentFollow component6 = gameObject2.GetComponent<ParentFollow>();
					if (component6 != null)
					{
						component6.HitParent = GlobalVars.Instance.hitParent;
						component6.ParentSeq = GlobalVars.Instance.hitBirckman;
						GlobalVars.Instance.hitParent = null;
						GlobalVars.Instance.hitBirckman = -1;
					}
				}
			}
		}
	}

	private GameObject GetMuzzleFireEffByLauncher()
	{
		LAUNCHER lAUNCHER = Launcher;
		if (lAUNCHER == LAUNCHER.GRANADE || lAUNCHER == LAUNCHER.ROCKET)
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
			return GlobalVars.Instance.CurMissileObj();
		case LAUNCHER.ROCKET:
			return GlobalVars.Instance.CurRocketObj();
		default:
			Debug.LogError("(GetMissileByLauncher)no exist launcher: " + Launcher);
			return null;
		}
	}

	private GameObject GetSmokeEffByLauncher()
	{
		if (Launcher == LAUNCHER.NONE)
		{
			return null;
		}
		LAUNCHER lAUNCHER = Launcher;
		if (lAUNCHER == LAUNCHER.GRANADE || lAUNCHER == LAUNCHER.ROCKET)
		{
			return GlobalVars.Instance.missileSmokeEff;
		}
		Debug.LogError("(GetSmokeEffByLauncher)no exist launcher: " + Launcher);
		return null;
	}

	private GameObject GetExplosionEffByLauncher()
	{
		if (Launcher == LAUNCHER.NONE)
		{
			return null;
		}
		LAUNCHER lAUNCHER = Launcher;
		if (lAUNCHER == LAUNCHER.GRANADE || lAUNCHER == LAUNCHER.ROCKET)
		{
			return GlobalVars.Instance.CurMissileExplosionEff();
		}
		Debug.LogError("(GetExplosionEffByLauncher)no exist launcher: " + Launcher);
		return null;
	}

	public override void Fire2(int ammoId, int launcher, Vector3 pos, Vector3 rot)
	{
		if (Launcher != LAUNCHER.NONE && dic != null)
		{
			if (dic.ContainsKey(ammoId) && misObj != null)
			{
				Object.Instantiate((Object)GetExplosionEffByLauncher(), misObj.transform.position, Quaternion.Euler(rot));
				if (misSmokeEff != null)
				{
					Object.DestroyImmediate(misSmokeEff);
					misSmokeEff = null;
				}
				Object.DestroyImmediate(misObj);
				misObj = null;
				dic.Remove(ammoId);
			}
			Launcher = (LAUNCHER)launcher;
			Quaternion localRotation = Quaternion.FromToRotation(Vector3.forward, rot);
			if (muzzleFxInstance2 == null)
			{
				GameObject gameObject = muzzleFxInstance2 = (Object.Instantiate((Object)GetMuzzleFireEffByLauncher()) as GameObject);
			}
			muzzleFxInstance2.transform.position = pos;
			muzzleFxInstance2.transform.localRotation = localRotation;
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
			misObj.transform.position = pos;
			misObj.transform.rotation = Quaternion.FromToRotation(Vector3.forward, rot);
			if (IsCrossbowType())
			{
				misSmokeEff = null;
			}
			else
			{
				misSmokeEff = (Object.Instantiate((Object)GlobalVars.Instance.missileSmokeEff) as GameObject);
				misSmokeEff.transform.position = pos;
				misSmokeEff.transform.parent = misObj.transform;
			}
			dic.Add(ammoId, new ProjectileWrap(misObj));
			base.animation.Play("bigfire");
			base.animation["bigfire"].wrapMode = WrapMode.Once;
		}
	}

	public override void Fly(int projectile, Vector3 pos, Vector3 rot)
	{
		if (dic != null && dic.ContainsKey(projectile))
		{
			dic[projectile].targetPos = pos;
			dic[projectile].targetRot = rot;
		}
	}

	public override void KaBoom(int projectile, Vector3 pos, Vector3 rot, bool viewColeff)
	{
		if (dic != null && dic.Count != 0)
		{
			if (Launcher == LAUNCHER.NONE)
			{
				if (!IsMuzzle && viewColeff && dic.ContainsKey(projectile))
				{
					ProjectileWrap projectileWrap = dic[projectile];
					if (!carryAnim && projectileWrap != null)
					{
						SelfCollsion component = projectileWrap.projectile.GetComponent<SelfCollsion>();
						if (component != null)
						{
							component.Explosion(pos, projectileWrap.projectile.transform.rotation, myself: false);
						}
						Object.DestroyImmediate(projectileWrap.projectile);
					}
					if (misSmokeEff != null)
					{
						Object.DestroyImmediate(misSmokeEff);
						misSmokeEff = null;
					}
					dic.Remove(projectile);
				}
			}
			else if (dic.ContainsKey(projectile))
			{
				Object.Instantiate((Object)GetExplosionEffByLauncher(), pos, Quaternion.Euler(rot));
				if (misSmokeEff != null)
				{
					Object.DestroyImmediate(misSmokeEff);
					misSmokeEff = null;
				}
				if (misObj != null)
				{
					Object.DestroyImmediate(misObj);
					misObj = null;
				}
				dic.Remove(projectile);
			}
		}
	}

	private void Start()
	{
		muzzle = null;
		shell = null;
		dic = new Dictionary<int, ProjectileWrap>();
		if (!BuildOption.Instance.IsNetmarble && !BuildOption.Instance.IsDeveloper && BuildOption.Instance.Props.useUskMuzzleEff && applyUsk)
		{
			GetComponent<Weapon>().muzzleFire = GlobalVars.Instance.muzzlefireUsk1;
		}
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
		Transform[] componentsInChildren2 = GetComponentsInChildren<Transform>();
		int num = 0;
		while ((muzzle == null || shell == null) && num < componentsInChildren2.Length)
		{
			if (componentsInChildren2[num].name.Contains("Dummy_fire_effect"))
			{
				muzzle = componentsInChildren2[num];
			}
			if (componentsInChildren2[num].name.Contains("Dummy_shell"))
			{
				shell = componentsInChildren2[num];
			}
			num++;
		}
		InitializeAnimation();
	}

	private void DoFireAnimation(string fireAnimation)
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

	private void InitializeAnimation()
	{
		base.animation.wrapMode = WrapMode.Loop;
		base.animation["empty"].layer = 1;
		base.animation["empty"].wrapMode = WrapMode.ClampForever;
		base.animation["fire"].layer = 1;
		base.animation["fire"].wrapMode = WrapMode.Once;
		base.animation["idle"].layer = 1;
		base.animation["idle"].wrapMode = WrapMode.Loop;
		if (carryAnim)
		{
			base.animation["carry"].layer = 1;
			base.animation["carry"].wrapMode = WrapMode.Once;
		}
		if (Launcher != LAUNCHER.NONE)
		{
			base.animation["bigfire"].layer = 1;
			base.animation["bigfire"].wrapMode = WrapMode.Once;
		}
		if (!Application.loadedLevelName.Contains("Lobby") && !Application.loadedLevelName.Contains("Briefing"))
		{
			base.animation.CrossFade("idle");
		}
	}

	private void OnDisable()
	{
		if (dic != null)
		{
			foreach (KeyValuePair<int, ProjectileWrap> item in dic)
			{
				Object.DestroyImmediate(item.Value.projectile);
			}
			dic.Clear();
		}
		if (misSmokeEff != null)
		{
			Object.DestroyImmediate(misSmokeEff);
			misSmokeEff = null;
		}
		if (misObj != null)
		{
			Object.DestroyImmediate(misObj);
			misObj = null;
		}
	}

	private void Update()
	{
		if (dic != null)
		{
			if (dic.Count > 0)
			{
				foreach (KeyValuePair<int, ProjectileWrap> item in dic)
				{
					if (IsOnlyPosType())
					{
						item.Value.Fly2();
					}
					else
					{
						item.Value.Fly();
					}
				}
			}
			if (!Application.loadedLevelName.Contains("Lobby") && !Application.loadedLevelName.Contains("Briefing") && !base.animation.IsPlaying("fire") && !base.animation.IsPlaying("empty") && !base.animation.IsPlaying("idle"))
			{
				base.animation.Play("idle");
			}
		}
	}

	private bool IsOnlyPosType()
	{
		return weaponBy == Weapon.BY.CROSSBOW_CONDOR || weaponBy == Weapon.BY.EAGLEEYE || weaponBy == Weapon.BY.CROSSBOW_CONDOR_G || weaponBy == Weapon.BY.EAGLEEYE_W || weaponBy == Weapon.BY.XMAS_BAZOOKA || weaponBy == Weapon.BY.STAFF01 || weaponBy == Weapon.BY.STAFF01_G || Launcher != LAUNCHER.NONE || weaponBy == Weapon.BY.CROSSBOW_LAUNCHER || weaponBy == Weapon.BY.HELLFIRE || weaponBy == Weapon.BY.SHAFT || weaponBy == Weapon.BY.XWING || weaponBy == Weapon.BY.NEEDLE || weaponBy == Weapon.BY.MOONLIGHT || weaponBy == Weapon.BY.EAGLEEYE_OR || weaponBy == Weapon.BY.SHAFT_OR || weaponBy == Weapon.BY.XWING_OR || weaponBy == Weapon.BY.NEEDLE_OR || weaponBy == Weapon.BY.CACTUS_GUN;
	}

	private bool IsCrossbowType()
	{
		return weaponBy == Weapon.BY.CROSSBOW_CONDOR || weaponBy == Weapon.BY.EAGLEEYE || weaponBy == Weapon.BY.CROSSBOW_CONDOR_G || weaponBy == Weapon.BY.EAGLEEYE_W || weaponBy == Weapon.BY.CROSSBOW_CONDOR_G_MAX || weaponBy == Weapon.BY.CROSSBOW_HORNET_S || weaponBy == Weapon.BY.CROSSBOW_HORNET_S_MAX || weaponBy == Weapon.BY.SHAFT || weaponBy == Weapon.BY.XWING || weaponBy == Weapon.BY.NEEDLE || weaponBy == Weapon.BY.MOONLIGHT || weaponBy == Weapon.BY.EAGLEEYE_OR || weaponBy == Weapon.BY.SHAFT_OR || weaponBy == Weapon.BY.XWING_OR || weaponBy == Weapon.BY.NEEDLE_OR || weaponBy == Weapon.BY.CACTUS_GUN;
	}

	private bool IsStaffType()
	{
		return weaponBy == Weapon.BY.STAFF01 || weaponBy == Weapon.BY.STAFF01_G;
	}
}
