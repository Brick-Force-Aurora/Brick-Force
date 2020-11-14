using UnityEngine;

public class FadeIn : MonoBehaviour
{
	public Color targetColor;

	private MeshRenderer mr;

	private void Start()
	{
		mr = GetComponent<MeshRenderer>();
		if (null == mr)
		{
			Debug.LogError("Fail to find MeshRenderer for FadeIn");
		}
	}

	private void Update()
	{
		Color color = mr.material.GetColor("_TintColor");
		color = Color.Lerp(color, targetColor, 10f * Time.deltaTime);
		mr.material.SetColor("_TintColor", color);
	}
}
