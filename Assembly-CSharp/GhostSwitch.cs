using System;
using System.Collections.Generic;
using UnityEngine;

public class GhostSwitch : MonoBehaviour
{
	private bool isGhost;

	private BrickManDesc desc;

	private SkinnedMeshRenderer[] smrDisabled;

	private MeshRenderer[] mrDisabled;

	private Collider[] colDisabled;

	private ParticleRenderer[] prDisabled;

	private void Start()
	{
	}

	private void VerifyDesc()
	{
		if (desc == null)
		{
			PlayerProperty component = GetComponent<PlayerProperty>();
			if (null != component)
			{
				desc = component.Desc;
			}
		}
	}

	private void Update()
	{
		VerifyDesc();
		if (desc != null)
		{
			if (desc.IsHidePlayer && !isGhost)
			{
				EnableGhost();
			}
			if (!desc.IsHidePlayer && isGhost)
			{
				DisableGhost();
			}
		}
	}

	private void EnableGhost()
	{
		if (!isGhost)
		{
			isGhost = true;
			ParticleRenderer[] componentsInChildren = GetComponentsInChildren<ParticleRenderer>(includeInactive: false);
			List<ParticleRenderer> list = new List<ParticleRenderer>();
			ParticleRenderer[] array = componentsInChildren;
			foreach (ParticleRenderer particleRenderer in array)
			{
				if ((bool)particleRenderer && particleRenderer.enabled)
				{
					particleRenderer.enabled = false;
					list.Add(particleRenderer);
				}
			}
			prDisabled = list.ToArray();
			Collider[] componentsInChildren2 = GetComponentsInChildren<Collider>(includeInactive: false);
			List<Collider> list2 = new List<Collider>();
			Collider[] array2 = componentsInChildren2;
			foreach (Collider collider in array2)
			{
				if ((bool)collider && collider.enabled)
				{
					collider.enabled = false;
					list2.Add(collider);
				}
			}
			colDisabled = list2.ToArray();
			MeshRenderer[] componentsInChildren3 = GetComponentsInChildren<MeshRenderer>(includeInactive: false);
			List<MeshRenderer> list3 = new List<MeshRenderer>();
			MeshRenderer[] array3 = componentsInChildren3;
			foreach (MeshRenderer meshRenderer in array3)
			{
				if ((bool)meshRenderer && meshRenderer.enabled)
				{
					meshRenderer.enabled = false;
					list3.Add(meshRenderer);
				}
			}
			mrDisabled = list3.ToArray();
			SkinnedMeshRenderer[] componentsInChildren4 = GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: false);
			List<SkinnedMeshRenderer> list4 = new List<SkinnedMeshRenderer>();
			SkinnedMeshRenderer[] array4 = componentsInChildren4;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in array4)
			{
				if ((bool)skinnedMeshRenderer && skinnedMeshRenderer.enabled)
				{
					skinnedMeshRenderer.enabled = false;
					list4.Add(skinnedMeshRenderer);
				}
			}
			smrDisabled = list4.ToArray();
		}
	}

	public void DisableGhost()
	{
		if (isGhost)
		{
			isGhost = false;
			ParticleRenderer[] array = prDisabled;
			foreach (ParticleRenderer particleRenderer in array)
			{
				try
				{
					if ((bool)particleRenderer)
					{
						particleRenderer.enabled = true;
					}
				}
				catch (Exception ex)
				{
					Debug.LogError(ex.Message.ToString());
				}
			}
			MeshRenderer[] array2 = mrDisabled;
			foreach (MeshRenderer meshRenderer in array2)
			{
				if ((bool)meshRenderer)
				{
					meshRenderer.enabled = true;
				}
			}
			mrDisabled = null;
			SkinnedMeshRenderer[] array3 = smrDisabled;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in array3)
			{
				if ((bool)skinnedMeshRenderer)
				{
					skinnedMeshRenderer.enabled = true;
				}
			}
			smrDisabled = null;
			Collider[] array4 = colDisabled;
			foreach (Collider collider in array4)
			{
				if ((bool)collider)
				{
					collider.enabled = true;
				}
			}
			colDisabled = null;
		}
	}
}
