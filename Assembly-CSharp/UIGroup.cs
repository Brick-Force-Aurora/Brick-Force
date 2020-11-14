using System;
using UnityEngine;

[Serializable]
public class UIGroup : UIBaseList
{
	public Vector2 area;

	public string style;

	public void BeginGroup()
	{
		if (style.Length > 0)
		{
			Vector2 showPosition = base.showPosition;
			float x = showPosition.x;
			Vector2 showPosition2 = base.showPosition;
			GUI.BeginGroup(new Rect(x, showPosition2.y, area.x, area.y), GUI.skin.GetStyle(style));
		}
		else
		{
			Vector2 showPosition3 = base.showPosition;
			float x2 = showPosition3.x;
			Vector2 showPosition4 = base.showPosition;
			GUI.BeginGroup(new Rect(x2, showPosition4.y, area.x, area.y));
		}
	}

	public void EndGroup()
	{
		GUI.EndGroup();
	}
}
