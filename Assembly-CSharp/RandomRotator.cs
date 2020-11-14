using UnityEngine;

public class RandomRotator : MonoBehaviour
{
	public float speedMax = 512f;

	public float speedMin = 128f;

	private float xSpeed;

	private float ySpeed;

	private float zSpeed;

	private void Start()
	{
		xSpeed = Random.Range(speedMin, speedMax);
		ySpeed = Random.Range(speedMin, speedMax);
		zSpeed = Random.Range(speedMin, speedMax);
	}

	private void Update()
	{
		base.transform.Rotate(xSpeed * Time.deltaTime, ySpeed * Time.deltaTime, zSpeed * Time.deltaTime);
	}
}
