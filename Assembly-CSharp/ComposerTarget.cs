using UnityEngine;

public class ComposerTarget : MonoBehaviour
{
	private MeshRenderer mr;

	private Transform target;

	private void Start()
	{
		mr = GetComponentInChildren<MeshRenderer>();
		target = base.transform.Find("TargetBox");
	}

	private void Update()
	{
	}

	public void ShowTarget(bool show)
	{
		if (!(null == mr))
		{
			mr.enabled = show;
		}
	}

	public void CenterAndSize(Vector3 center, Vector3 size)
	{
		if (null != target)
		{
			size += new Vector3(0.05f, 0.05f, 0.05f);
			target.localPosition = center;
			target.localScale = size;
		}
	}
}
