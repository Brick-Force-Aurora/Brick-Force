using UnityEngine;

public class GadgetCannon : MonoBehaviour
{
	public GameObject muzzleFire;

	public GameObject bulletTrail;

	public GameObject[] muzzleFxInstances;

	private CannonController cannonController;

	private AudioSource audioSource;

	private int fireCount;

	private void Start()
	{
		fireCount = 0;
		cannonController = GetComponent<CannonController>();
		if (null == cannonController)
		{
			Debug.LogError("ERROR, Fail to get cannnon controller");
		}
		audioSource = GetComponent<AudioSource>();
		if (null == audioSource)
		{
			Debug.LogError("ERROR, Fail to get audio source  ");
		}
		if (cannonController.muzzles != null && cannonController.muzzles.Length > 0)
		{
			muzzleFxInstances = new GameObject[cannonController.muzzles.Length];
			for (int i = 0; i < cannonController.muzzles.Length; i++)
			{
				muzzleFxInstances[i] = null;
			}
		}
	}

	private void Update()
	{
	}

	private void DoFireSound()
	{
		if (!(null == cannonController) && null != audioSource)
		{
			if (null != audioSource)
			{
				if (GlobalVars.Instance.mute)
				{
					audioSource.volume = 0f;
				}
				else
				{
					audioSource.volume = cannonController.fireVolume;
				}
			}
			audioSource.PlayOneShot(cannonController.fireSound);
		}
	}

	private void DoMuzzleFire()
	{
		if (!(null == cannonController) && cannonController.muzzles != null && cannonController.muzzles.Length > 0)
		{
			int num = fireCount % cannonController.muzzles.Length;
			if (muzzleFxInstances[num] == null)
			{
				GameObject gameObject = Object.Instantiate((Object)cannonController.muzzleFire) as GameObject;
				gameObject.transform.position = cannonController.muzzles[num].position;
				gameObject.transform.parent = cannonController.muzzles[num];
				gameObject.transform.localRotation = Quaternion.Euler(90f, 90f, 0f);
				muzzleFxInstances[num] = gameObject;
			}
			ParticleEmitter particleEmitter = null;
			int childCount = muzzleFxInstances[num].transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = muzzleFxInstances[num].transform.GetChild(i);
				particleEmitter = child.GetComponent<ParticleEmitter>();
				if ((bool)particleEmitter)
				{
					particleEmitter.Emit();
				}
			}
		}
	}

	public void Fire(int cannon, int shooter, Vector3 origin, Vector3 direction)
	{
		if (!(null == cannonController) && cannonController.BrickSeq == cannon && cannonController.Shooter == shooter)
		{
			fireCount++;
			cannonController.DoFireAnimation();
			DoFireSound();
			DoMuzzleFire();
			Shoot(origin, direction);
		}
	}

	private void Shoot(Vector3 origin, Vector3 direction)
	{
		if (!(null == bulletTrail))
		{
			GameObject gameObject = VfxOptimizer.Instance.CreateFx(bulletTrail, origin, Quaternion.FromToRotation(Vector3.forward, direction), VfxOptimizer.VFX_TYPE.BULLET_TRAIL);
			if (null != gameObject)
			{
				gameObject.GetComponent<Bullet>().Speed = 600f;
			}
		}
	}

	public void Move(int cannon, int shooter, float x, float y)
	{
		if (!(cannonController == null) && cannonController.BrickSeq == cannon && cannonController.Shooter == shooter)
		{
			cannonController.Move(x, y);
		}
	}
}
