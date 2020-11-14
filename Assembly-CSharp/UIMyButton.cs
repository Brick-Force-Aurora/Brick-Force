using System;
using UnityEngine;

[Serializable]
public class UIMyButton : UIBase
{
	public Vector2 area;

	public string textKey = string.Empty;

	private string text = string.Empty;

	public string guiStyle;

	public Texture2D contentImage;

	public string toolTipString;

	private bool buttonClick;

	public override bool Draw()
	{
		if (!isDraw)
		{
			return false;
		}
		if (guiStyle.Length == 0)
		{
			return false;
		}
		string text = this.text;
		if (this.text.Length == 0 && textKey.Length != 0)
		{
			text = StringMgr.Instance.Get(textKey);
		}
		if (contentImage != null)
		{
			GUIContent content = new GUIContent(text, contentImage);
			GlobalVars instance = GlobalVars.Instance;
			Vector2 showPosition = base.showPosition;
			float x = showPosition.x;
			Vector2 showPosition2 = base.showPosition;
			buttonClick = instance.MyButton3(new Rect(x, showPosition2.y, area.x, area.y), content, guiStyle);
		}
		else if (toolTipString.Length > 0)
		{
			GlobalVars instance2 = GlobalVars.Instance;
			Vector2 showPosition3 = base.showPosition;
			float x2 = showPosition3.x;
			Vector2 showPosition4 = base.showPosition;
			buttonClick = instance2.MyButton(new Rect(x2, showPosition4.y, area.x, area.y), new GUIContent(text, toolTipString), guiStyle);
		}
		else
		{
			GlobalVars instance3 = GlobalVars.Instance;
			Vector2 showPosition5 = base.showPosition;
			float x3 = showPosition5.x;
			Vector2 showPosition6 = base.showPosition;
			buttonClick = instance3.MyButton(new Rect(x3, showPosition6.y, area.x, area.y), text, guiStyle);
		}
		return buttonClick;
	}

	public override bool SkipDraw()
	{
		return buttonClick = false;
	}

	public void SetText(string _text)
	{
		text = _text;
	}

	public bool isClick()
	{
		return buttonClick;
	}
}
