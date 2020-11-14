using UnityEngine;

public class TBuff
{
	private int index;

	private bool isPoint;

	private bool isXp;

	private bool isLuck;

	private float factor;

	public int Index => index;

	public bool IsPoint => isPoint;

	public bool IsXp => isXp;

	public bool IsLuck => isLuck;

	public float Factor => factor;

	public int PointRatio
	{
		get
		{
			if (!isPoint)
			{
				return 0;
			}
			return Mathf.FloorToInt(factor * 100f);
		}
	}

	public int XpRatio
	{
		get
		{
			if (!isXp)
			{
				return 0;
			}
			return Mathf.FloorToInt(factor * 100f);
		}
	}

	public int Luck
	{
		get
		{
			if (!isLuck)
			{
				return 0;
			}
			return Mathf.FloorToInt(factor * 100f);
		}
	}

	public TBuff(int _index, bool _isPoint, bool _isXp, bool _isLuck, float _factor)
	{
		index = _index;
		isPoint = _isPoint;
		isXp = _isXp;
		isLuck = _isLuck;
		factor = _factor;
	}
}
