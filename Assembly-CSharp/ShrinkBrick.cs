using UnityEngine;

public class ShrinkBrick : MonoBehaviour
{
	private Vector3 center = Vector3.zero;

	private Vector3 size = Vector3.zero;

	public void CenterAndSize(Vector3 _center, Vector3 _size)
	{
		center = _center;
		size = _size;
	}

	private void Start()
	{
		Transform transform = base.transform.Find("ShrinkBox");
		if (null != transform)
		{
			transform.localPosition = center;
			transform.localScale = size;
		}
	}

	private void Update()
	{
	}
}
