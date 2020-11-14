using System;
using UnityEngine;

[Serializable]
public class UIHorizontalSlider : UIBase
{
	public Vector2 area;

	public float minValue;

	public float maxValue = 1f;

	public float value;

	public override bool Draw()
	{
		if (!isDraw)
		{
			return false;
		}
		Vector2 showPosition = base.showPosition;
		float x = showPosition.x;
		Vector2 showPosition2 = base.showPosition;
		value = GUI.HorizontalSlider(new Rect(x, showPosition2.y, area.x, area.y), value, minValue, maxValue);
		return false;
	}
}
