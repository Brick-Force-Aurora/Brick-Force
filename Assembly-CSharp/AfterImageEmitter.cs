using UnityEngine;

public class AfterImageEmitter : MonoBehaviour
{
	public string[] componentsToDel;

	public float minDistance;

	private Vector3 lastPosition = Vector3.zero;

	private bool afterImage;

	public void ShowAfterImage(bool show)
	{
		afterImage = show;
		if (afterImage)
		{
			Shoot();
		}
	}

	private void Shoot()
	{
		GameObject gameObject = Object.Instantiate((Object)base.transform.gameObject, base.transform.position, base.transform.rotation) as GameObject;
		if (null != gameObject)
		{
			for (int i = 0; i < componentsToDel.Length; i++)
			{
				Object.DestroyImmediate(gameObject.GetComponent(componentsToDel[i]));
			}
			gameObject.AddComponent<FadeOutDestroyer>();
			lastPosition = base.transform.position;
		}
	}

	private void Start()
	{
		lastPosition = base.transform.position;
	}

	private void Update()
	{
		if (afterImage && Vector3.Distance(lastPosition, base.transform.position) > minDistance)
		{
			Shoot();
		}
	}
}
