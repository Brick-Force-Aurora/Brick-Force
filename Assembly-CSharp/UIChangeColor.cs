using System;
using UnityEngine;

[Serializable]
public class UIChangeColor : UIImage
{
	public Color startColor;

	public Color endColor;

	public float changeTime = 1f;

	private float currentTime;

	private float change;

	public bool changeEndHide;

	public override bool Update()
	{
		if (!isDraw)
		{
			return true;
		}
		if (currentTime < changeTime)
		{
			currentTime += Time.deltaTime;
			change = currentTime / changeTime;
			if (change >= 1f)
			{
				if (changeEndHide)
				{
					isDraw = false;
				}
				change = 1f;
			}
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
		GUI.color = Color.Lerp(startColor, endColor, change);
		Rect position = (area.x == 0f && area.y == 0f) ? new Rect(base.position.x, base.position.y, (float)texImage.width, (float)texImage.height) : new Rect(base.position.x, base.position.y, area.x, area.y);
		TextureUtil.DrawTexture(position, texImage, ScaleMode.StretchToFill, alphaBlend: true);
		GUI.color = color;
		return false;
	}

	public void Reset()
	{
		currentTime = 0f;
		isDraw = true;
	}
}
