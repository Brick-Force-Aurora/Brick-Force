using UnityEngine;

public class FadeOutDestroyer : MonoBehaviour
{
	private SkinnedMeshRenderer[] smrArray;

	private MeshRenderer[] mrArray;

	public float fadeOutSpeed = 10f;

	private void Start()
	{
		smrArray = GetComponentsInChildren<SkinnedMeshRenderer>();
		mrArray = GetComponentsInChildren<MeshRenderer>();
		ChangeSkinnedMeshRenderersShaderToFadeoutable();
		ChangeMeshRenderersShaderToFadeoutable();
	}

	private void ChangeSkinnedMeshRenderersShaderToFadeoutable()
	{
		for (int i = 0; i < smrArray.Length; i++)
		{
			smrArray[i].material.shader = Shader.Find("Transparent/Diffuse");
			Color color = smrArray[i].material.GetColor("_Color");
			smrArray[i].material.SetColor("_Color", color);
		}
	}

	private void ChangeMeshRenderersShaderToFadeoutable()
	{
		for (int i = 0; i < mrArray.Length; i++)
		{
			mrArray[i].material.shader = Shader.Find("Transparent/Diffuse");
			Color color = mrArray[i].material.GetColor("_Color");
			mrArray[i].material.SetColor("_Color", color);
		}
	}

	private bool FadeOutSkinnedMeshRenderer()
	{
		bool result = true;
		for (int i = 0; i < smrArray.Length; i++)
		{
			Color color = smrArray[i].material.GetColor("_Color");
			if (color.a >= 0.0001f)
			{
				result = false;
				color.a = Mathf.Lerp(color.a, 0f, fadeOutSpeed * Time.deltaTime);
				smrArray[i].material.SetColor("_Color", color);
			}
		}
		return result;
	}

	private bool FadeOutMeshRenderer()
	{
		bool result = true;
		for (int i = 0; i < mrArray.Length; i++)
		{
			Color color = mrArray[i].material.GetColor("_Color");
			if (color.a >= 0.0001f)
			{
				result = false;
				color.a = Mathf.Lerp(color.a, 0f, fadeOutSpeed * Time.deltaTime);
				mrArray[i].material.SetColor("_Color", color);
			}
		}
		return result;
	}

	private void Update()
	{
		bool flag = FadeOutSkinnedMeshRenderer();
		if (FadeOutMeshRenderer() && flag)
		{
			Object.Destroy(base.transform.gameObject);
		}
	}
}
