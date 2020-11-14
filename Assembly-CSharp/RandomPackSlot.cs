using System;
using UnityEngine;

[Serializable]
public class RandomPackSlot
{
	public Texture2D bg;

	public Texture2D[] fx;

	public Texture2D itemBg;

	public Texture2D[] random;

	public Texture2D[] weapon;

	public Texture2D[] cloth;

	public Texture2D GetTexture2D(int tab, int x, int y, int status)
	{
		if (status == 4)
		{
			return itemBg;
		}
		if (x == 0 && y == 0)
		{
			return random[status];
		}
		if (tab == 0)
		{
			return weapon[status];
		}
		return cloth[status];
	}
}
