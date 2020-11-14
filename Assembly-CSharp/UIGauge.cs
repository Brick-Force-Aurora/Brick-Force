using System;
using UnityEngine;

[Serializable]
public class UIGauge : UIImage
{
	private const float INVALID = -1000000f;

	public float valueMax = 100f;

	public float valueNow = 100f;

	public bool isLandscape = true;

	public bool isReverse;

	public bool isDrawCut = true;

	private Vector2 imageMax = Vector2.zero;

	private Vector2 imageStart;

	private Vector2 imageNow;

	private Rect cutOff = new Rect(0f, 0f, 100f, 100f);

	public override bool Draw()
	{
		if (!isDraw)
		{
			return false;
		}
		if (isDrawCut)
		{
			return DrawCut();
		}
		if (!Calculate())
		{
			return false;
		}
		return base.Draw();
	}

	private bool DrawCut()
	{
		if (!Calculate())
		{
			return false;
		}
		ref Rect reference = ref cutOff;
		Vector2 showPosition = base.showPosition;
		float x = showPosition.x;
		Vector2 showPosition2 = base.showPosition;
		reference.Set(x, showPosition2.y, area.x, area.y);
		GUI.BeginGroup(cutOff);
		position.x = 0f;
		position.y = 0f;
		if (isLandscape)
		{
			area.x = imageMax.x;
			if (isReverse)
			{
				position.x = 0f - imageMax.x + imageNow.x;
			}
		}
		else
		{
			area.y = imageMax.y;
			if (isReverse)
			{
				position.y = 0f - imageMax.y + imageNow.y;
			}
		}
		base.Draw();
		position = imageStart;
		GUI.EndGroup();
		return true;
	}

	private bool Calculate()
	{
		if (imageMax == Vector2.zero)
		{
			imageStart = position;
			if (area.Equals(Vector2.zero) && texImage != null)
			{
				area.x = (float)texImage.width;
				area.y = (float)texImage.height;
			}
			imageMax = area;
		}
		if (valueMax <= 0f)
		{
			return false;
		}
		float num = valueNow / valueMax;
		if (num < 0f)
		{
			num = 0f;
		}
		if (num > 1f)
		{
			num = 1f;
		}
		imageNow = imageMax * num;
		if (isLandscape)
		{
			area.x = imageNow.x;
			if (isReverse)
			{
				position.x = imageStart.x + imageMax.x - imageNow.x;
			}
		}
		else
		{
			area.y = imageNow.y;
			if (isReverse)
			{
				position.y = imageStart.y + imageMax.y - imageNow.y;
			}
		}
		return true;
	}

	public void SetRatio(float ratio)
	{
		valueNow = ratio * valueMax;
	}
}
