using UnityEngine;

public class LabelUtil
{
	private static GUIStyle pushedStyleSize;

	private static int pushedFontSize;

	public static void ToBold(string style)
	{
		GUIStyle style2 = GUI.skin.GetStyle(style);
		style2.fontStyle = FontStyle.Bold;
	}

	public static void ToNormal(string style)
	{
		GUIStyle style2 = GUI.skin.GetStyle(style);
		style2.fontStyle = FontStyle.Normal;
	}

	public static void PushSize(string style, int fontSize)
	{
		pushedStyleSize = GUI.skin.GetStyle(style);
		pushedFontSize = pushedStyleSize.fontSize;
		pushedStyleSize.fontSize = fontSize;
	}

	public static void PopSize()
	{
		pushedStyleSize.fontSize = pushedFontSize;
	}

	public static Vector2 CalcLength(string style, string text)
	{
		GUIStyle style2 = GUI.skin.GetStyle(style);
		return style2.CalcSize(new GUIContent(text));
	}

	public static Vector2 CalcSize(string style, string text, float width)
	{
		GUIStyle style2 = GUI.skin.GetStyle(style);
		Vector2 result = style2.CalcSize(new GUIContent(text));
		if (result.x > width)
		{
			float y = style2.CalcHeight(new GUIContent(text), width);
			result.x = width;
			result.y = y;
		}
		return result;
	}

	public static Vector2 TextOut(Vector3 sp, string text, string style, Color clrText, Color clrOutline, TextAnchor alignment)
	{
		return TextOut(new Vector2(sp.x, (float)Screen.height - sp.y), text, style, clrText, clrOutline, alignment);
	}

	public static Vector2 TextOut(Vector2 pos, string text, string style, Color clrText, Color clrOutline, TextAnchor alignment, float width)
	{
		Color color = GUI.color;
		GUIStyle style2 = GUI.skin.GetStyle(style);
		Vector2 result = style2.CalcSize(new GUIContent(text));
		if (result.x > width)
		{
			float y = style2.CalcHeight(new GUIContent(text), width);
			result.x = width;
			result.y = y;
		}
		switch (alignment)
		{
		case TextAnchor.UpperCenter:
			pos.x -= result.x / 2f;
			break;
		case TextAnchor.UpperRight:
			pos.x -= result.x;
			break;
		case TextAnchor.MiddleLeft:
			pos.y -= result.y / 2f;
			break;
		case TextAnchor.MiddleCenter:
			pos.x -= result.x / 2f;
			pos.y -= result.y / 2f;
			break;
		case TextAnchor.MiddleRight:
			pos.x -= result.x;
			pos.y -= result.y / 2f;
			break;
		case TextAnchor.LowerLeft:
			pos.y -= result.y;
			break;
		case TextAnchor.LowerCenter:
			pos.x -= result.x / 2f;
			pos.y -= result.y;
			break;
		case TextAnchor.LowerRight:
			pos.x -= result.x;
			pos.y -= result.y;
			break;
		}
		if (clrOutline != Color.clear)
		{
			float[] array = new float[9]
			{
				-1f,
				0f,
				1f,
				-1f,
				0f,
				1f,
				-1f,
				0f,
				1f
			};
			float[] array2 = new float[9]
			{
				-1f,
				-1f,
				-1f,
				0f,
				0f,
				0f,
				1f,
				1f,
				1f
			};
			GUI.color = clrOutline;
			for (int i = 0; i < 9; i++)
			{
				if (i != 4)
				{
					GUI.Label(new Rect(pos.x + array[i], pos.y + array2[i], result.x, result.y), text, style);
				}
			}
		}
		GUI.color = clrText;
		GUI.Label(new Rect(pos.x, pos.y, result.x, result.y), text, style);
		GUI.color = color;
		return result;
	}

	public static Vector2 TextOut(Vector2 pos, Vector2 size, string text, string style, Color clrText, Color clrOutline, TextAnchor alignment)
	{
		string text2 = text;
		string str = string.Empty;
		Vector2 vector = CalcLength(style, text2);
		while (size.x < vector.x && text2.Length > 0)
		{
			str = "...";
			text2 = text2.Remove(text2.Length - 1);
			vector = CalcLength(style, text2 + str);
		}
		return TextOut(pos, text2 + str, style, clrText, clrOutline, alignment);
	}

	public static Vector2 TextOut(Vector2 pos, string text, string style, Color clrText, TextAnchor alignment)
	{
		Color color = GUI.color;
		GUIStyle style2 = GUI.skin.GetStyle(style);
		Vector2 result = style2.CalcSize(new GUIContent(text));
		result.x += 5f;
		switch (alignment)
		{
		case TextAnchor.UpperCenter:
			pos.x -= result.x / 2f;
			break;
		case TextAnchor.UpperRight:
			pos.x -= result.x;
			break;
		case TextAnchor.MiddleLeft:
			pos.y -= result.y / 2f;
			break;
		case TextAnchor.MiddleCenter:
			pos.x -= result.x / 2f;
			pos.y -= result.y / 2f;
			break;
		case TextAnchor.MiddleRight:
			pos.x -= result.x;
			pos.y -= result.y / 2f;
			break;
		case TextAnchor.LowerLeft:
			pos.y -= result.y;
			break;
		case TextAnchor.LowerCenter:
			pos.x -= result.x / 2f;
			pos.y -= result.y;
			break;
		case TextAnchor.LowerRight:
			pos.x -= result.x;
			pos.y -= result.y;
			break;
		}
		GUI.color = clrText;
		GUI.Label(new Rect(pos.x, pos.y, result.x, result.y), text, style);
		GUI.color = color;
		return result;
	}

	public static Vector2 TextOut(Vector2 pos, string text, string style, Color clrText, Color clrOutline, TextAnchor alignment)
	{
		Color color = GUI.color;
		GUIStyle style2 = GUI.skin.GetStyle(style);
		Vector2 result = style2.CalcSize(new GUIContent(text));
		result.x += 5f;
		switch (alignment)
		{
		case TextAnchor.UpperCenter:
			pos.x -= result.x / 2f;
			break;
		case TextAnchor.UpperRight:
			pos.x -= result.x;
			break;
		case TextAnchor.MiddleLeft:
			pos.y -= result.y / 2f;
			break;
		case TextAnchor.MiddleCenter:
			pos.x -= result.x / 2f;
			pos.y -= result.y / 2f;
			break;
		case TextAnchor.MiddleRight:
			pos.x -= result.x;
			pos.y -= result.y / 2f;
			break;
		case TextAnchor.LowerLeft:
			pos.y -= result.y;
			break;
		case TextAnchor.LowerCenter:
			pos.x -= result.x / 2f;
			pos.y -= result.y;
			break;
		case TextAnchor.LowerRight:
			pos.x -= result.x;
			pos.y -= result.y;
			break;
		}
		if (clrOutline != Color.clear)
		{
			float[] array = new float[9]
			{
				-1f,
				0f,
				1f,
				-1f,
				0f,
				1f,
				-1f,
				0f,
				1f
			};
			float[] array2 = new float[9]
			{
				-1f,
				-1f,
				-1f,
				0f,
				0f,
				0f,
				1f,
				1f,
				1f
			};
			GUI.color = clrOutline;
			for (int i = 0; i < 9; i++)
			{
				if (i != 4)
				{
					GUI.Label(new Rect(pos.x + array[i], pos.y + array2[i], result.x, result.y), text, style);
				}
			}
		}
		GUI.color = clrText;
		GUI.Label(new Rect(pos.x, pos.y, result.x, result.y), text, style);
		GUI.color = color;
		return result;
	}
}
