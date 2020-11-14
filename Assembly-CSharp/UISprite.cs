using System;
using UnityEngine;

[Serializable]
public class UISprite : UIBase
{
	public Vector2 area;

	public Texture2D[] texImage;

	public float changeTime = 0.3f;

	public float currentTime;

	public bool playOnce;

	public override bool Update()
	{
		currentTime += Time.deltaTime;
		return true;
	}

	public override bool Draw()
	{
		if (!isDraw)
		{
			return false;
		}
		if (texImage.Length != 0 && changeTime != 0f)
		{
			int num = (int)(currentTime / changeTime);
			if (playOnce && num >= texImage.Length)
			{
				return false;
			}
			int num2 = num % texImage.Length;
			if (area == Vector2.zero)
			{
				Vector2 showPosition = base.showPosition;
				float x = showPosition.x;
				Vector2 showPosition2 = base.showPosition;
				TextureUtil.DrawTexture(new Rect(x, showPosition2.y, (float)texImage[num2].width, (float)texImage[num2].height), texImage[num2], ScaleMode.StretchToFill);
			}
			else
			{
				Vector2 showPosition3 = base.showPosition;
				float x2 = showPosition3.x;
				Vector2 showPosition4 = base.showPosition;
				TextureUtil.DrawTexture(new Rect(x2, showPosition4.y, area.x, area.y), texImage[num2], ScaleMode.StretchToFill);
			}
		}
		return false;
	}

	public void ResetTime()
	{
		currentTime = 0f;
	}

	public void SetEndTime()
	{
		currentTime = changeTime;
	}
}
