using UnityEngine;

public class MonProperty : MonoBehaviour
{
	public MonDesc Desc;

	private Vector3 invisiblePosition;

	public Vector3 InvisiblePosition
	{
		get
		{
			return invisiblePosition;
		}
		set
		{
			invisiblePosition = value;
		}
	}

	public bool IsHostile()
	{
		if (Desc == null)
		{
			return false;
		}
		return Desc.IsHostile();
	}
}
