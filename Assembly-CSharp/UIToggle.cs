using System;
using UnityEngine;

[Serializable]
public class UIToggle : UIBase
{
	public Vector2 area;

	public string textKey = string.Empty;

	private string text = string.Empty;

	public bool toggle;

	private bool toggleOld;

	public override bool Draw()
	{
		if (!isDraw)
		{
			return false;
		}
		if (text.Length == 0 && textKey.Length != 0)
		{
			Vector2 showPosition = base.showPosition;
			float x = showPosition.x;
			Vector2 showPosition2 = base.showPosition;
			toggle = GUI.Toggle(new Rect(x, showPosition2.y, area.x, area.y), toggle, StringMgr.Instance.Get(textKey));
		}
		else
		{
			Vector2 showPosition3 = base.showPosition;
			float x2 = showPosition3.x;
			Vector2 showPosition4 = base.showPosition;
			toggle = GUI.Toggle(new Rect(x2, showPosition4.y, area.x, area.y), toggle, text);
		}
		toggleOld = toggle;
		return toggle;
	}

	public void SetText(string _text)
	{
		text = _text;
	}

	public bool isChangeToggle()
	{
		return toggleOld != toggle;
	}
}
