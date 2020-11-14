using System;
using UnityEngine;

[Serializable]
public class Accuracy
{
	public float accuracy = 60f;

	public float accurateMin;

	public float accurateMax;

	public float inaccurateMin;

	public float inaccurateMax;

	public float accurateSpread;

	public float accurateCenter;

	public float inaccurateSpread;

	public float inaccurateCenter;

	public float moveInaccuracyFactor = 2f;

	private float inaccurate;

	private float accurate;

	public float _Accuracy
	{
		get
		{
			return accuracy;
		}
		set
		{
			accuracy = value;
		}
	}

	public float AccurateMin
	{
		get
		{
			return accurateMin;
		}
		set
		{
			accurateMin = value;
		}
	}

	public float AccurateMax
	{
		get
		{
			return accurateMax;
		}
		set
		{
			accurateMax = value;
		}
	}

	public float InaccurateMin
	{
		get
		{
			return inaccurateMin;
		}
		set
		{
			inaccurateMin = value;
		}
	}

	public float InaccurateMax
	{
		get
		{
			return inaccurateMax;
		}
		set
		{
			inaccurateMax = value;
		}
	}

	public float AccurateSpread
	{
		get
		{
			return accurateSpread;
		}
		set
		{
			accurateSpread = value;
		}
	}

	public float AccurateCenter
	{
		get
		{
			return accurateCenter;
		}
		set
		{
			accurateCenter = value;
		}
	}

	public float InaccurateSpread
	{
		get
		{
			return inaccurateMax;
		}
		set
		{
			inaccurateSpread = value;
		}
	}

	public float InaccurateCenter
	{
		get
		{
			return inaccurateCenter;
		}
		set
		{
			inaccurateCenter = value;
		}
	}

	public float MoveInaccuracyFactor
	{
		get
		{
			return moveInaccuracyFactor;
		}
		set
		{
			moveInaccuracyFactor = value;
		}
	}

	public float Inaccurate => inaccurate;

	public float Accurate => accurate;

	public void Init()
	{
		accurate = accurateMin;
		inaccurate = inaccurateMin;
	}

	public void MakeInaccurate(bool aimAccurateMore)
	{
		float b = accurateMax;
		float b2 = inaccurateMax;
		accurate += accurateSpread;
		inaccurate += inaccurateSpread;
		if (aimAccurateMore)
		{
			b2 = (inaccurateMax + inaccurateMin) * 0.5f;
			b = (accurateMax + accurateMin) * 0.5f;
		}
		accurate = Mathf.Min(accurate, b);
		inaccurate = Mathf.Min(inaccurate, b2);
	}

	public void MakeAccurate(bool aimAccurate)
	{
		float num = accurateMin;
		float num2 = inaccurateMin;
		if (!aimAccurate)
		{
			num *= moveInaccuracyFactor;
			num2 *= moveInaccuracyFactor;
		}
		accurate -= accurateCenter * Time.deltaTime;
		inaccurate -= inaccurateCenter * Time.deltaTime;
		accurate = Mathf.Max(accurate, num);
		inaccurate = Mathf.Max(inaccurate, num2);
	}

	public Vector2 CalcDeflection()
	{
		float f = UnityEngine.Random.Range(-1f, 1f);
		float f2 = UnityEngine.Random.Range(-1f, 1f);
		float num = UnityEngine.Random.Range(0f, 100f);
		float num2;
		float num3;
		if (num < accuracy)
		{
			num2 = UnityEngine.Random.Range(0f, accurate / 2f);
			num3 = UnityEngine.Random.Range(0f, accurate / 2f);
		}
		else
		{
			if (accurate > inaccurate)
			{
				Debug.LogError("ERROR, inaccurate should be bigger than accurate value");
				inaccurate = accurate;
			}
			num2 = UnityEngine.Random.Range(accurate / 2f, inaccurate / 2f);
			num3 = UnityEngine.Random.Range(accurate / 2f, inaccurate / 2f);
		}
		num2 *= Mathf.Sign(f);
		num3 *= Mathf.Sign(f2);
		float factor = (16f / 9f) / ((float)Screen.width / (float)Screen.height) * 60f / Camera.main.fieldOfView;
		return new Vector2((float)(Screen.width / 2) + (float)Screen.width * num2 * factor, (float)(Screen.height / 2) + (float)Screen.width * num3 * factor);
		//return new Vector2((float)(Screen.width / 2) + (float)Screen.width * num2, (float)(Screen.height / 2) + (float)Screen.width * num3);
	}
}
