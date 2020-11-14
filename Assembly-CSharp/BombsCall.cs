using System.Collections.Generic;
using UnityEngine;

public class BombsCall : ActiveItemBase
{
	public float bombsCallTime;

	public float explosionTime;

	public float explosionRadius;

	public float bombCreateYPosition;

	public float bombExplosionExceptHigher;

	public float bombExplosionExceptLower;

	public GameObject explosionEffect;

	public GameObject explosionEffect11;

	public GameObject targetEffect;

	public GameObject bombEffect;

	public AudioClip sndBombsCall;

	public AudioClip sndBombsFalling;

	public AudioClip sndBombsExplosion;

	private List<Vector3> explosionPosition = new List<Vector3>();

	private float currentTime;

	private bool createBomb;

	private bool explosion;

	private void Awake()
	{
		currentTime = 0f;
	}

	private void Update()
	{
		currentTime += Time.deltaTime;
		if (!createBomb && bombsCallTime < currentTime)
		{
			createBomb = true;
			BombsCreate();
		}
		else if (!explosion && explosionTime < currentTime)
		{
			explosion = true;
			foreach (Vector3 item in explosionPosition)
			{
				CreateExplosion(item);
			}
			GameObject gameObject = GameObject.Find("Me");
			if (null != gameObject)
			{
				AudioSource component = gameObject.GetComponent<AudioSource>();
				if (component != null)
				{
					component.PlayOneShot(sndBombsExplosion);
				}
			}
			explosionPosition.Clear();
		}
	}

	public override void StartItem()
	{
		GameObject gameObject = GameObject.Find("Me");
		if (null != gameObject)
		{
			AudioSource component = gameObject.GetComponent<AudioSource>();
			if (component != null)
			{
				component.PlayOneShot(sndBombsCall);
			}
		}
	}

	private void BombsCreate()
	{
		GameObject gameObject = GameObject.Find("Me");
		if (null != gameObject)
		{
			AudioSource component = gameObject.GetComponent<AudioSource>();
			if (component != null)
			{
				component.PlayOneShot(sndBombsFalling);
			}
			if (useUserSeq != MyInfoManager.Instance.Seq)
			{
				CreateBomb(gameObject.transform.position);
			}
		}
		Dictionary<int, GameObject> dicBrickMan = BrickManManager.Instance.GetDicBrickMan();
		foreach (KeyValuePair<int, GameObject> item in dicBrickMan)
		{
			if (item.Key != useUserSeq)
			{
				CreateBomb(item.Value.transform.position);
			}
		}
	}

	public void CreateBomb(Vector3 pos)
	{
		Object.Instantiate((Object)targetEffect, pos, Quaternion.Euler(0f, 0f, 0f));
		explosionPosition.Add(pos);
		pos.y += bombCreateYPosition;
		Object.Instantiate((Object)bombEffect, pos, Quaternion.Euler(0f, 0f, 0f));
	}

	public bool CreateExplosion(Vector3 pos)
	{
		if (MyInfoManager.Instance.IsBelow12())
		{
			Object.Instantiate((Object)explosionEffect11, pos, Quaternion.Euler(0f, 0f, 0f));
		}
		else
		{
			Object.Instantiate((Object)explosionEffect, pos, Quaternion.Euler(0f, 0f, 0f));
		}
		GameObject gameObject = GameObject.Find("Me");
		if (null != gameObject && Vector3.Distance(pos, gameObject.transform.position) < explosionRadius)
		{
			float num = pos.y + bombExplosionExceptHigher;
			Vector3 position = gameObject.transform.position;
			if (num > position.y)
			{
				float num2 = pos.y - bombExplosionExceptLower;
				Vector3 position2 = gameObject.transform.position;
				if (num2 < position2.y)
				{
					LocalController component = gameObject.GetComponent<LocalController>();
					if (component != null && !component.IsDead)
					{
						component.GetHitBungeeBomb(useUserSeq, 32);
					}
				}
			}
		}
		return false;
	}
}
