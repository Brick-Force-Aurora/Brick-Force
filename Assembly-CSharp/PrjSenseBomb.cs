using UnityEngine;

public class PrjSenseBomb : Projectile
{
	private void Update()
	{
		deltaTime += Time.deltaTime;
		base.DetonatorTime += Time.deltaTime;
		if (base.DetonatorTime > explosionTime)
		{
			if (!BuildOption.Instance.Props.useUskMuzzleEff || !base.ApplyUsk)
			{
				if (explosion != null)
				{
					Object.Instantiate((Object)explosion, base.transform.position, Quaternion.Euler(0f, 0f, 0f));
				}
			}
			else if (GlobalVars.Instance.explosionUsk != null)
			{
				Object.Instantiate((Object)GlobalVars.Instance.explosionUsk, base.transform.position, Quaternion.Euler(0f, 0f, 0f));
			}
			P2PManager.Instance.SendPEER_PROJECTILE_KABOOM(MyInfoManager.Instance.Seq, base.Index);
			Object.Destroy(base.transform.gameObject);
		}
	}

	private void Start()
	{
		if (BuildOption.Instance.Props.useUskWeaponTex)
		{
			MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				if (meshRenderer.material.mainTexture != null && UskManager.Instance.Get(meshRenderer.material.mainTexture.name) != null)
				{
					meshRenderer.material.mainTexture = UskManager.Instance.Get(meshRenderer.material.mainTexture.name);
				}
			}
		}
	}
}
