using System;
using UnityEngine;

[Serializable]
public class Token
{
	public enum TYPE
	{
		TOKEN,
		NETMARBLE,
		TOONY
	}

	public string name;

	public Texture2D mark;

	public string skin;
}
