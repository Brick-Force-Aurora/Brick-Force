using UnityEngine;

public class GdgtMelee : WeaponGadget
{
	public override void Fire(int projectile, Vector3 origin, Vector3 direction)
	{
		GetComponent<Weapon>().FireSound();
	}

	private void Start()
	{
		if (BuildOption.Instance.Props.useUskWeaponTex && applyUsk)
		{
			SkinnedMeshRenderer[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
			{
				if (skinnedMeshRenderer.material.mainTexture != null && UskManager.Instance.Get(skinnedMeshRenderer.material.mainTexture.name) != null)
				{
					skinnedMeshRenderer.material.mainTexture = UskManager.Instance.Get(skinnedMeshRenderer.material.mainTexture.name);
				}
			}
			MeshRenderer[] componentsInChildren2 = GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren2)
			{
				if (meshRenderer.material.mainTexture != null && UskManager.Instance.Get(meshRenderer.material.mainTexture.name) != null)
				{
					meshRenderer.material.mainTexture = UskManager.Instance.Get(meshRenderer.material.mainTexture.name);
				}
			}
		}
	}
}
