using UnityEngine;

public class Kaboom : MonoBehaviour
{
	public GameObject explosion;

	public bool noapplyUsk;

	public float minKaboomTime = 0.4f;

	public float maxKaboomTime = 1.2f;

	private float kaboomDelta;

	private float nextKaboom;

	private void Start()
	{
		kaboomDelta = 0f;
		nextKaboom = 0f;
	}

	private void Update()
	{
		kaboomDelta += Time.deltaTime;
		if (kaboomDelta > nextKaboom)
		{
			kaboomDelta = 0f;
			nextKaboom = Random.Range(minKaboomTime, maxKaboomTime);
			if (!noapplyUsk)
			{
				if (!BuildOption.Instance.Props.useUskMuzzleEff)
				{
					if (explosion != null)
					{
						Object.Instantiate((Object)explosion, base.transform.position, Quaternion.Euler(0f, 0f, 0f));
					}
				}
				else if (GlobalVars.Instance.explosionUsk != null)
				{
					Object.Instantiate((Object)GlobalVars.Instance.explosionUsk, base.transform.position, Quaternion.Euler(0f, 0f, 0f));
				}
			}
			else if (explosion != null)
			{
				Object.Instantiate((Object)explosion, base.transform.position, Quaternion.Euler(0f, 0f, 0f));
			}
		}
	}
}
