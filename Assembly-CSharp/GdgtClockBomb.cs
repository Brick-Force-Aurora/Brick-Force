using UnityEngine;

public class GdgtClockBomb : WeaponGadget
{
	private ExplosionMatch explosionMatch;

	private BrickManDesc desc;

	public override void Install(bool install)
	{
		if (install)
		{
			GetComponent<Weapon>().StartFireSound();
		}
		else
		{
			GetComponent<Weapon>().EndFireSound();
		}
	}

	private void EnsureVisibility()
	{
		if (desc != null && explosionMatch != null)
		{
			if (explosionMatch.BombInstaller == desc.Seq)
			{
				Hide();
			}
			else if (!desc.IsHidePlayer)
			{
				Show();
			}
		}
	}

	private void VerifyBrickManDesc()
	{
		if (desc == null)
		{
			PlayerProperty[] allComponents = Recursively.GetAllComponents<PlayerProperty>(base.transform, includeInactive: true);
			if (allComponents.Length > 0)
			{
				desc = allComponents[0].Desc;
			}
		}
	}

	private void VerifyExplosionMatch()
	{
		if (explosionMatch == null)
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				explosionMatch = gameObject.GetComponent<ExplosionMatch>();
			}
		}
	}

	private void Show()
	{
		MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			meshRenderer.enabled = true;
		}
	}

	private void Hide()
	{
		GetComponent<Weapon>().EndFireSound();
		MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			meshRenderer.enabled = false;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		VerifyExplosionMatch();
		VerifyBrickManDesc();
		EnsureVisibility();
	}
}
