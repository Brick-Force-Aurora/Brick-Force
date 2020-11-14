using System.Collections.Generic;
using UnityEngine;

public class BrickChunk : MonoBehaviour
{
	private int maxChildren = 255;

	public void Init(Material material, int maxNumberOfChildren)
	{
		MeshRenderer component = GetComponent<MeshRenderer>();
		if (component != null)
		{
			component.material = material;
		}
		maxChildren = maxNumberOfChildren;
	}

	public bool AddBrick(GameObject brick, bool merge)
	{
		if (base.transform.childCount >= maxChildren)
		{
			return false;
		}
		brick.transform.parent = base.transform;
		if (merge)
		{
			Merge();
		}
		return true;
	}

	private MeshFilter[] GetMeshFiltersInBricks()
	{
		MeshFilter[] componentsInChildren = GetComponentsInChildren<MeshFilter>(includeInactive: true);
		List<MeshFilter> list = new List<MeshFilter>();
		int num = LayerMask.NameToLayer("BulletMark");
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			int layer = componentsInChildren[i].gameObject.layer;
			if (layer != num)
			{
				list.Add(componentsInChildren[i]);
			}
		}
		return list.ToArray();
	}

	public void Merge()
	{
		MeshFilter component = GetComponent<MeshFilter>();
		component.mesh.Clear();
		MeshFilter[] meshFiltersInBricks = GetMeshFiltersInBricks();
		if (meshFiltersInBricks.Length > 0)
		{
			base.transform.GetComponent<MeshRenderer>().material = ((Component)meshFiltersInBricks[0]).renderer.sharedMaterial;
			CombineInstance[] array = new CombineInstance[meshFiltersInBricks.Length - 1];
			int i = 0;
			int num = 0;
			for (; i < meshFiltersInBricks.Length; i++)
			{
				if (meshFiltersInBricks[i] != component)
				{
					array[num].mesh = meshFiltersInBricks[i].sharedMesh;
					array[num].transform = meshFiltersInBricks[i].transform.localToWorldMatrix;
					num++;
				}
				meshFiltersInBricks[i].gameObject.SetActive(value: false);
			}
			component.mesh.CombineMeshes(array);
			base.transform.gameObject.SetActive(value: true);
			base.transform.gameObject.GetComponent<MeshCollider>().sharedMesh = base.transform.gameObject.GetComponent<MeshFilter>().mesh;
		}
	}
}
