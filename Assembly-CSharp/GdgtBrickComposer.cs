using UnityEngine;

public class GdgtBrickComposer : WeaponGadget
{
	private Transform muzzle;

	private GameObject muzzleFxInstance;

	public Transform transformFever;

	private GameObject objFever;

	public override void Compose(bool isDel)
	{
		CreateMuzzleFire();
		if (isDel)
		{
			DelFireSound();
		}
		else
		{
			FireSound();
		}
		DoFireAnimation("fire");
	}

	private void FireSound()
	{
		if (!BuildOption.Instance.Props.brickSoundChange)
		{
			AudioSource component = GetComponent<AudioSource>();
			AudioClip fireSound = GetComponent<Weapon>().fireSound;
			if (null != component && null != fireSound)
			{
				component.PlayOneShot(fireSound);
			}
		}
	}

	private void DelFireSound()
	{
		if (!BuildOption.Instance.Props.brickSoundChange)
		{
			AudioSource component = GetComponent<AudioSource>();
			AudioClip clipOutSound = GetComponent<Weapon>().clipOutSound;
			if (null != component && null != clipOutSound)
			{
				component.PlayOneShot(clipOutSound);
			}
		}
	}

	private void CreateMuzzleFire()
	{
		if (!(null == muzzle) && !(null == GetComponent<Weapon>().muzzleFire))
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

	private void Start()
	{
		muzzle = null;
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
		int num = 0;
		while (muzzle == null && num < componentsInChildren.Length)
		{
			if (componentsInChildren[num].name.Contains("Dummy_fire_effect"))
			{
				muzzle = componentsInChildren[num];
			}
			num++;
		}
		InitializeAnimation();
	}

	private void updateFever()
	{
		if (transformFever != null && objFever != null)
		{
			objFever.transform.position = transformFever.position;
			objFever.transform.rotation = transformFever.rotation;
		}
	}

	private void Update()
	{
		updateFever();
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
		base.animation["fire"].layer = 1;
		base.animation["fire"].wrapMode = WrapMode.Once;
		base.animation["idle"].layer = 1;
		base.animation["idle"].wrapMode = WrapMode.Loop;
		base.animation.CrossFade("idle");
	}

	public override void setFever(bool isOn)
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
		}
	}
}
