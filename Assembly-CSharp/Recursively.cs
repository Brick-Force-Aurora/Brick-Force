using UnityEngine;

public class Recursively
{
	public static void SetLayer(Transform parent, int layer)
	{
		parent.gameObject.layer = layer;
		foreach (Transform item in parent)
		{
			SetLayer(item, layer);
		}
	}

	public static void ChangeLayer(Transform parent, int fromLayer, int toLayer)
	{
		if (parent.gameObject.layer == fromLayer)
		{
			parent.gameObject.layer = toLayer;
		}
		foreach (Transform item in parent)
		{
			ChangeLayer(item, fromLayer, toLayer);
		}
	}

	public static T[] GetAllComponents<T>(Transform t, bool includeInactive) where T : Component
	{
		if (null == t.parent)
		{
			return t.GetComponentsInChildren<T>(includeInactive);
		}
		return GetAllComponents<T>(t.parent, includeInactive);
	}
}
