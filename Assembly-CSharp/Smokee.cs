using UnityEngine;

public class Smokee : MonoBehaviour
{
	public float smokeeTime = 10f;

	public float lifeTime = 50f;

	private float deltaTime;

	public bool ApplyDotDamage;

	public Weapon.BY weaponBy;

	private float deltaTimeDot;

	private bool bOwn;

	public int dotDamage = 10;

	private void Start()
	{
	}

	public void Mine()
	{
		bOwn = true;
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if (deltaTime > lifeTime)
		{
			Object.Destroy(base.transform.gameObject);
		}
		else if (deltaTime > smokeeTime)
		{
			ParticleEmitter[] componentsInChildren = GetComponentsInChildren<ParticleEmitter>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].maxEmission = 0f;
				componentsInChildren[i].minEmission = 0f;
			}
		}
		if (bOwn && ApplyDotDamage)
		{
			deltaTimeDot += Time.deltaTime;
			if (deltaTimeDot >= 1f)
			{
				deltaTimeDot = 0f;
				float num = -9999f;
				ParticleEmitter[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<ParticleEmitter>();
				for (int j = 0; j < componentsInChildren2.Length; j++)
				{
					if (num < componentsInChildren2[j].maxSize)
					{
						num = componentsInChildren2[j].maxSize;
					}
				}
				float num2 = 9999f;
				componentsInChildren2 = base.gameObject.GetComponentsInChildren<ParticleEmitter>();
				for (int k = 0; k < componentsInChildren2.Length; k++)
				{
					if (num2 > componentsInChildren2[k].minSize)
					{
						num2 = componentsInChildren2[k].minSize;
					}
				}
				if (num > 0f)
				{
					float num3 = (num + num2) * 0.25f;
					GameObject gameObject = GameObject.Find("Me");
					if (null != gameObject)
					{
						float num4 = Vector3.Distance(base.transform.position, gameObject.transform.position);
						if (num4 < num3)
						{
							LocalController component = gameObject.GetComponent<LocalController>();
							if (component != null)
							{
								component.GetHit(MyInfoManager.Instance.Seq, dotDamage, 0f, (int)weaponBy, -1, autoHealPossible: true, checkZombie: false);
							}
						}
					}
					GameObject[] array = BrickManManager.Instance.ToGameObjectArray();
					for (int l = 0; l < array.Length; l++)
					{
						PlayerProperty component2 = array[l].GetComponent<PlayerProperty>();
						if (component2 != null && component2.IsHostile())
						{
							float num5 = Vector3.Distance(base.transform.position, array[l].transform.position);
							if (num5 < num3)
							{
								P2PManager.Instance.SendPEER_BOMBED(MyInfoManager.Instance.Seq, component2.Desc.Seq, dotDamage, 0f, (int)weaponBy);
							}
						}
					}
				}
			}
		}
	}
}
