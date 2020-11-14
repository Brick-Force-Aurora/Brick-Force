using UnityEngine;

public class UniformedScaler : MonoBehaviour
{
	public float speed = 1f;

	public Vector3 targetScale;

	private void Start()
	{
	}

	private void Update()
	{
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, targetScale, speed * Time.deltaTime);
	}
}
