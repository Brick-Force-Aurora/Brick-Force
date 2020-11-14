using UnityEngine;

public class PrjSmoke : Projectile
{
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

	private void Update()
	{
		Rigidbody component = GetComponent<Rigidbody>();
		if (!(null != component) || !component.isKinematic)
		{
			deltaTime += Time.deltaTime;
			base.DetonatorTime += Time.deltaTime;
			if (base.DetonatorTime > explosionTime)
			{
				if (null != explosion)
				{
					GameObject gameObject = Object.Instantiate((Object)explosion, base.transform.position, Quaternion.Euler(0f, 0f, 0f)) as GameObject;
					ParticleEmitter[] componentsInChildren = gameObject.GetComponentsInChildren<ParticleEmitter>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].minEnergy += base.PersistTime;
						componentsInChildren[i].maxEnergy += base.PersistTime;
					}
				}
				P2PManager.Instance.SendPEER_PROJECTILE_KABOOM(MyInfoManager.Instance.Seq, base.Index);
				Object.Destroy(base.transform.gameObject);
			}
			else if (deltaTime > BuildOption.Instance.Props.SendRate)
			{
				deltaTime = 0f;
				P2PManager.Instance.SendPEER_PROJECTILE_FLY(MyInfoManager.Instance.Seq, base.Index, base.transform.position, base.transform.rotation.eulerAngles, 0f);
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
}
