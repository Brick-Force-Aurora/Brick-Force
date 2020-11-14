using System;
using UnityEngine;

[Serializable]
public class UIFlickerColor : UIImage
{
	public Color startColor;

	public Color endColor;

	public float changeTime = 1f;

	public float hideTime;

	private float totalTime;

	private float currentTime;

	private float change;

	private bool isReverse;

	public override bool Update()
	{
		if (!isDraw)
		{
			return true;
		}
		if (hideTime > 0f && totalTime > hideTime)
		{
			isDraw = false;
			Reset();
		}
		currentTime += Time.deltaTime;
		totalTime += Time.deltaTime;
		if (currentTime > changeTime)
		{
			isReverse = !isReverse;
			currentTime = 0f;
		}
		change = currentTime / changeTime;
		if (change < 0f)
		{
			change = 0f;
		}
		if (change > 1f)
		{
			change = 1f;
		}
		return true;
	}

	public override bool Draw()
	{
		if (!isDraw)
		{
			return false;
		}
		Color color = GUI.color;
		if (!isReverse)
		{
			GUI.color = Color.Lerp(startColor, endColor, change);
		}
		else
		{
			GUI.color = Color.Lerp(endColor, startColor, change);
		}
		base.Draw();
		GUI.color = color;
		return false;
	}

	public void Reset()
	{
		currentTime = 0f;
		totalTime = 0f;
		change = 0f;
		isReverse = false;
	}
}
