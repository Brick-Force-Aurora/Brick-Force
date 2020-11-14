using UnityEngine;

public class ActRotatorExample : MonoBehaviour
{
	[Range(1f, 100f)]
	public float speed = 5f;

	private void Update()
	{
		base.transform.Rotate(speed * Time.deltaTime, speed * Time.deltaTime, speed * Time.deltaTime);
	}
}
