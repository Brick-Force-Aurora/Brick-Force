using UnityEngine;

public class BndWall : MonoBehaviour
{
	private float deltaTime;

	public float hideTime = 3f;

	public float showTime = 3f;

	private bool hiding;

	private bool showing;

	private Vector3 scaleShow = Vector3.zero;

	private Vector3 scaleHide = Vector3.zero;

	public GameObject[] probeTop;

	public GameObject[] probeBottom;

	private void Start()
	{
		hiding = false;
		showing = false;
		scaleShow = base.transform.localScale;
		scaleHide = new Vector3(scaleShow.x, 0f, scaleShow.z);
		for (int i = 0; i < probeTop.Length; i++)
		{
			Vector3 position = probeTop[i].transform.position;
			probeTop[i].transform.position = new Vector3(position.x, 110f, position.z);
		}
		for (int j = 0; j < probeBottom.Length; j++)
		{
			Vector3 position2 = probeBottom[j].transform.position;
			probeBottom[j].transform.position = new Vector3(position2.x, -110f, position2.z);
		}
	}

	private void Hiding()
	{
		if (hiding)
		{
			deltaTime += Time.deltaTime;
			if (deltaTime <= hideTime)
			{
				base.transform.localScale = Vector3.Lerp(scaleShow, scaleHide, deltaTime / hideTime);
			}
			else
			{
				hiding = false;
				base.transform.localScale = scaleHide;
				MeshCollider[] componentsInChildren = GetComponentsInChildren<MeshCollider>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].enabled = false;
				}
			}
			for (int j = 0; j < probeTop.Length; j++)
			{
				Vector3 position = probeTop[j].transform.position;
				Transform transform = probeTop[j].transform;
				float x = position.x;
				Vector3 localScale = base.transform.localScale;
				transform.position = new Vector3(x, localScale.y * 10f, position.z);
			}
			for (int k = 0; k < probeBottom.Length; k++)
			{
				Vector3 position2 = probeBottom[k].transform.position;
				Transform transform2 = probeBottom[k].transform;
				float x2 = position2.x;
				Vector3 localScale2 = base.transform.localScale;
				transform2.position = new Vector3(x2, (0f - localScale2.y) * 10f, position2.z);
			}
		}
	}

	private void Showing()
	{
		if (showing)
		{
			deltaTime += Time.deltaTime;
			if (deltaTime <= showTime)
			{
				base.transform.localScale = Vector3.Lerp(scaleHide, scaleShow, deltaTime / hideTime);
			}
			else
			{
				showing = false;
				base.transform.localScale = scaleShow;
				MeshCollider[] componentsInChildren = GetComponentsInChildren<MeshCollider>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].enabled = true;
				}
			}
			for (int j = 0; j < probeTop.Length; j++)
			{
				Vector3 position = probeTop[j].transform.position;
				Transform transform = probeTop[j].transform;
				float x = position.x;
				Vector3 localScale = base.transform.localScale;
				transform.position = new Vector3(x, localScale.y * 10f, position.z);
			}
			for (int k = 0; k < probeBottom.Length; k++)
			{
				Vector3 position2 = probeBottom[k].transform.position;
				Transform transform2 = probeBottom[k].transform;
				float x2 = position2.x;
				Vector3 localScale2 = base.transform.localScale;
				transform2.position = new Vector3(x2, (0f - localScale2.y) * 10f, position2.z);
			}
		}
	}

	private void Update()
	{
		Hiding();
		Showing();
	}

	public void Hide(bool rightNow)
	{
		if (rightNow)
		{
			deltaTime = hideTime;
		}
		else
		{
			deltaTime = 0f;
		}
		hiding = true;
	}

	public void Show(bool rightNow)
	{
		if (rightNow)
		{
			deltaTime = showTime;
		}
		else
		{
			deltaTime = 0f;
		}
		showing = true;
	}
}
