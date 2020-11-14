using UnityEngine;

public class InvincibleArmor : MonoBehaviour
{
	public GameObject UnbreakableCapsule;

	public float lifeTime = 5f;

	private GameObject armor;

	private float deltaTime;

	private bool waitDestroy;

	private float deltaTimeWaitDestroy;

	private void Start()
	{
		armor = null;
		deltaTime = 0f;
	}

	public void Enable()
	{
		if (armor == null)
		{
			armor = (Object.Instantiate((Object)UnbreakableCapsule, base.transform.position, base.transform.rotation) as GameObject);
		}
		deltaTime = 0f;
	}

	public void Destroy()
	{
		if (null != armor)
		{
			Object.DestroyImmediate(armor);
			waitDestroy = false;
		}
		else
		{
			waitDestroy = true;
		}
	}

	private void Update()
	{
		if (null != armor)
		{
			armor.transform.position = base.transform.position;
			deltaTime += Time.deltaTime;
			if (deltaTime > lifeTime)
			{
				Object.DestroyImmediate(armor);
				armor = null;
			}
		}
		if (waitDestroy)
		{
			deltaTimeWaitDestroy += Time.deltaTime;
			if (deltaTimeWaitDestroy >= 0.1f)
			{
				deltaTimeWaitDestroy = 0f;
				if (armor != null)
				{
					Object.DestroyImmediate(armor);
					armor = null;
					waitDestroy = false;
				}
			}
		}
	}
}
