using UnityEngine;

public class TimeLimitedDestroyer : MonoBehaviour
{
	public float limit = 1f;

	private float deltaTime;

	private void Start()
	{
		deltaTime = 0f;
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if (deltaTime > limit)
		{
			Object.DestroyImmediate(base.gameObject);
		}
	}
}
