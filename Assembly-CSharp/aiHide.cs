using UnityEngine;

public class aiHide : MonAI
{
	public GameObject effHide;

	public float hideTime = 3f;

	private float deltaTimeHide;

	private bool canApply = true;

	private bool isHide;

	public bool CanApply => canApply;

	public bool IsHide => isHide;

	public void setHide(bool bSet)
	{
		if (canApply)
		{
			if (bSet)
			{
				Object.Instantiate((Object)effHide, base.transform.position, Quaternion.Euler(0f, 0f, 0f));
				SkinnedMeshRenderer[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>();
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
				{
					Material material = skinnedMeshRenderer.material;
					Color color = skinnedMeshRenderer.material.color;
					float r = color.r;
					Color color2 = skinnedMeshRenderer.material.color;
					float g = color2.g;
					Color color3 = skinnedMeshRenderer.material.color;
					material.SetColor("_Color", new Color(r, g, color3.b, 0.5f));
				}
				deltaTimeHide = 0f;
				isHide = true;
			}
			else
			{
				SkinnedMeshRenderer[] componentsInChildren2 = GetComponentsInChildren<SkinnedMeshRenderer>();
				foreach (SkinnedMeshRenderer skinnedMeshRenderer2 in componentsInChildren2)
				{
					Material material2 = skinnedMeshRenderer2.material;
					Color color4 = skinnedMeshRenderer2.material.color;
					float r2 = color4.r;
					Color color5 = skinnedMeshRenderer2.material.color;
					float g2 = color5.g;
					Color color6 = skinnedMeshRenderer2.material.color;
					material2.SetColor("_Color", new Color(r2, g2, color6.b, 1f));
				}
				isHide = false;
				canApply = false;
			}
		}
	}

	public override void updateHide()
	{
		if (canApply && isHide)
		{
			deltaTimeHide += Time.deltaTime;
			if (deltaTimeHide > hideTime)
			{
				setHide(bSet: false);
			}
		}
	}
}
