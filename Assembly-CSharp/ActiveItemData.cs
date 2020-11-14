using System;
using UnityEngine;

[Serializable]
public class ActiveItemData
{
	public const float NOT_USE_COOLTIME = -1f;

	private int itemType;

	public Texture2D icon;

	public int chance = 10;

	public GameObject itemPrefap;

	public string itemText;

	public float cooltime = -1f;

	public int GetItemType()
	{
		return itemType;
	}

	public void SetItemType(int type)
	{
		itemType = type;
	}
}
