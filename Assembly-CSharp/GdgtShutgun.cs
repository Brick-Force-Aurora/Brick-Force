using System.Collections.Generic;
using UnityEngine;

public class GdgtShutgun : WeaponGadget
{
	private Transform muzzle;

	private Transform shell;

	private GameObject muzzleFxInstance;

	private GameObject muzzleFxInstance2;

	private LAUNCHER Launcher = LAUNCHER.NONE;

	private GameObject misObj;

	private GameObject misSmokeEff;

	private Dictionary<int, ProjectileWrap> dic;

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
		if (!(null == shell))
		{
			GameObject gameObject = VfxOptimizer.Instance.CreateFx(GetComponent<Weapon>().CatridgeOrClip, shell.position, Quaternion.Euler(0f, 0f, 0f), VfxOptimizer.VFX_TYPE.SHELL);
			if (null != gameObject)
			{
				gameObject.transform.parent = shell;
				gameObject.transform.localRotation = Quaternion.Euler(90f, 90f, 0f);
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

	public override void Fire(int projectile, Vector3 origin, Vector3 direction)
	{
		FireSound();
		CreateMuzzleFire();
		CreateShell();
		Shoot(origin, direction);
		DoFireAnimation("fire");
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
		LAUNCHER launcher = Launcher;
		if (launcher == LAUNCHER.GRANADE || launcher == LAUNCHER.ROCKET)
		{
			return GlobalVars.Instance.missileExplosionEff;
		}
		Debug.LogError("(GetExplosionEffByLauncher)no exist launcher: " + Launcher);
		return null;
	}

	public override void Fire2(int ammoId, int launcher, Vector3 pos, Vector3 rot)
	{
		Launcher = (LAUNCHER)launcher;
		Quaternion quaternion = Quaternion.Euler(0f, 0f, 0f);
		if (muzzleFxInstance2 == null)
		{
			GameObject gameObject = muzzleFxInstance2 = (Object.Instantiate((Object)GetMuzzleFireEffByLauncher()) as GameObject);
		}
		muzzleFxInstance2.transform.position = pos;
		muzzleFxInstance2.transform.localRotation = quaternion;
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
		misObj = (Object.Instantiate((Object)GetMissileByLauncher(), pos, quaternion) as GameObject);
		misSmokeEff = (Object.Instantiate((Object)GetSmokeEffByLauncher()) as GameObject);
		misSmokeEff.transform.position = pos;
		misSmokeEff.transform.parent = misObj.transform;
		dic.Add(0, new ProjectileWrap(misObj));
	}

	public override void Fly(int projectile, Vector3 pos, Vector3 rot)
	{
		if (dic != null && dic.ContainsKey(0))
		{
			dic[0].targetPos = pos;
			dic[0].targetRot = rot;
		}
	}

	public override void KaBoom(int projectile, Vector3 pos, Vector3 rot, bool viewColeff)
	{
		Object.Instantiate((Object)GetExplosionEffByLauncher(), pos, Quaternion.Euler(rot));
		Object.DestroyImmediate(misObj);
		Object.DestroyImmediate(misSmokeEff);
		dic.Remove(0);
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
		base.animation.CrossFade("idle");
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

	private void Update()
	{
		if (dic != null)
		{
			foreach (KeyValuePair<int, ProjectileWrap> item in dic)
			{
				item.Value.Fly();
			}
			if (!Application.loadedLevelName.Contains("Lobby") && !Application.loadedLevelName.Contains("Briefing") && !base.animation.IsPlaying("fire") && !base.animation.IsPlaying("empty") && !base.animation.IsPlaying("idle"))
			{
				base.animation.Play("idle");
			}
		}
	}
}
