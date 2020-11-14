using System;
using UnityEngine;

[Serializable]
public class UILabel : UIBase
{
	public enum LABEL_STYLE
	{
		BIGLABEL,
		LABEL,
		MINILABEL
	}

	public enum LABEL_COLOR
	{
		CLEAR,
		WHITE,
		BLACK,
		BLUE,
		GRAY,
		GREEN,
		GREY,
		MAGENTA,
		RED,
		YELLOW,
		MAIN_TEXT,
		C_212_125_74,
		C_50_191_17,
		C_98_60_10
	}

	private static string[] labelStyle = new string[3]
	{
		"BigLabel",
		"Label",
		"MiniLabel"
	};

	private static Color[] colorList = new Color[14]
	{
		Color.clear,
		Color.white,
		Color.black,
		Color.blue,
		Color.gray,
		Color.green,
		Color.grey,
		Color.magenta,
		Color.red,
		Color.yellow,
		GetByteColor2FloatColor(244, 151, 25),
		GetByteColor2FloatColor(212, 125, 74),
		GetByteColor2FloatColor(50, 191, 17),
		GetByteColor2FloatColor(98, 60, 10)
	};

	public string textKey = string.Empty;

	private string text = string.Empty;

	public LABEL_STYLE style;

	public LABEL_COLOR textColor = LABEL_COLOR.WHITE;

	public LABEL_COLOR outLineColor;

	public TextAnchor alignment;

	public float width;

	public override bool Draw()
	{
		if (!isDraw)
		{
			return false;
		}
		string text = this.text;
		if (this.text.Length == 0 && textKey.Length != 0)
		{
			text = StringMgr.Instance.Get(textKey);
		}
		if (width == 0f)
		{
			LabelUtil.TextOut(base.showPosition, text, labelStyle[(int)style], colorList[(int)textColor], colorList[(int)outLineColor], alignment);
		}
		else
		{
			LabelUtil.TextOut(base.showPosition, text, labelStyle[(int)style], colorList[(int)textColor], colorList[(int)outLineColor], alignment, width);
		}
		return false;
	}

	public void SetText(string _text)
	{
		text = _text;
	}

	public void SetTextFormat(object arg0)
	{
		text = string.Format(StringMgr.Instance.Get(textKey), arg0);
	}

	public void SetTextFormat(object arg0, object arg1)
	{
		text = string.Format(StringMgr.Instance.Get(textKey), arg0, arg1);
	}

	public void SetTextFormat(object arg0, object arg1, object arg2)
	{
		text = string.Format(StringMgr.Instance.Get(textKey), arg0, arg1, arg2);
	}

	public static Color GetByteColor2FloatColor(byte bR, byte bG, byte bB)
	{
		return new Color((float)(int)bR / 255f, (float)(int)bG / 255f, (float)(int)bB / 255f);
	}

	public Vector2 CalcLength()
	{
		return LabelUtil.CalcLength(labelStyle[(int)style], text);
	}

	public static Color GetLabelColor(LABEL_COLOR color)
	{
		return colorList[(int)color];
	}
}
