using System.Collections.Generic;
using UnityEngine;

public class RareStuff
{
	private RareFx[] stars;

	private int starRandomMin = 6;

	private int starRandomMax = 9;

	public bool Alive
	{
		get
		{
			for (int i = 0; i < stars.Length; i++)
			{
				if (stars[i].RareFxStep != RareFx.RAREFX_STEP.DONE)
				{
					return true;
				}
			}
			return false;
		}
	}

	public RareStuff(Vector2 src, Vector2 dst)
	{
		int num = Random.Range(starRandomMin, starRandomMax);
		stars = new RareFx[num];
		for (int i = 0; i < num; i++)
		{
			stars[i] = new RareFx(src, dst);
		}
	}

	public void Update()
	{
		for (int i = 0; i < stars.Length; i++)
		{
			stars[i].Update();
		}
	}

	public RareFx[] ToArray()
	{
		List<RareFx> list = new List<RareFx>();
		for (int i = 0; i < stars.Length; i++)
		{
			list.Add(stars[i]);
		}
		return list.ToArray();
	}
}
