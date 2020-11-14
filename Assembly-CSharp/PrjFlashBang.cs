using UnityEngine;

public class PrjFlashBang : Projectile
{
	public Weapon.BY weaponBY = Weapon.BY.COMPOSER;

	private void Update()
	{
		deltaTime += Time.deltaTime;
		base.DetonatorTime += Time.deltaTime;
		if (base.DetonatorTime > explosionTime)
		{
			if (null != explosion)
			{
				Object.Instantiate((Object)explosion, base.transform.position, Quaternion.Euler(0f, 0f, 0f));
			}
			GlobalVars.Instance.SwitchFlashbang(bVis: true, base.transform.position);
			P2PManager.Instance.SendPEER_PROJECTILE_KABOOM(MyInfoManager.Instance.Seq, base.Index);
			if (null != projectileAlert)
			{
				projectileAlert.RemoveMine(base.Index);
			}
			Object.Destroy(base.transform.gameObject);
		}
		else if (deltaTime > BuildOption.Instance.Props.SendRate)
		{
			deltaTime = 0f;
			P2PManager.Instance.SendPEER_PROJECTILE_FLY(MyInfoManager.Instance.Seq, base.Index, base.transform.position, base.transform.rotation.eulerAngles, GlobalVars.Instance.maxDistanceFlashbang);
			if (null != projectileAlert)
			{
				projectileAlert.TrackMine(base.Index, weaponBY, base.transform.position, GlobalVars.Instance.maxDistanceFlashbang);
			}
		}
	}

	private void FixedUpdate()
	{
		Rigidbody component = GetComponent<Rigidbody>();
		if (component != null)
		{
			component.AddTorque(Vector3.right * 1000f, ForceMode.Acceleration);
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
