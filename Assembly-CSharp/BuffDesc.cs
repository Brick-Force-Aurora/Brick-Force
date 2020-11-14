using System;
using UnityEngine;

[Serializable]
public class BuffDesc
{
	public enum WHY
	{
		ITEM,
		PREMIUM,
		APS,
		GM,
		CHANNEL,
		PC_BANG
	}

	public Texture2D icon;

	public string tooltip;
}
