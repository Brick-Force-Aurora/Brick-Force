using UnityEngine;

public class AutoRotator : MonoBehaviour
{
	public enum ROTATE
	{
		LEFT = -1,
		STOP,
		RIGHT
	}

	public float rotateSpeed = 45f;

	public ROTATE rotate;

	public bool stopOnStart = true;

	private void Start()
	{
		if (stopOnStart)
		{
			rotate = ROTATE.STOP;
		}
	}

	public void Rotate(ROTATE rot)
	{
		rotate = rot;
	}

	private void Update()
	{
		float num = (float)rotate * rotateSpeed;
		base.transform.Rotate(0f, num * Time.deltaTime, 0f);
	}
}
