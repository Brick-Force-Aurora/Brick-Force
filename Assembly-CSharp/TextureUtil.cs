using UnityEngine;

public class TextureUtil
{
	public static void DrawTexture(Rect position, Texture image)
	{
		Color color = GUI.color;
		if (!GUI.enabled)
		{
			GUI.color = new Color(color.r, color.g, color.b, color.a * 0.5f);
		}
		GUI.DrawTexture(position, image);
		GUI.color = color;
	}

	public static void DrawTexture(Rect position, Texture image, ScaleMode scaleMode)
	{
		Color color = GUI.color;
		if (!GUI.enabled)
		{
			GUI.color = new Color(color.r, color.g, color.b, color.a * 0.5f);
		}
		GUI.DrawTexture(position, image, scaleMode);
		GUI.color = color;
	}

	public static void DrawTexture(Rect position, Texture image, ScaleMode scaleMode, bool alphaBlend)
	{
		Color color = GUI.color;
		if (!GUI.enabled)
		{
			GUI.color = new Color(color.r, color.g, color.b, color.a * 0.5f);
		}
		GUI.DrawTexture(position, image, scaleMode, alphaBlend);
		GUI.color = color;
	}

	public static void DrawTexture(Rect position, Texture image, Rect srcRect)
	{
		if (Event.current.type.Equals(EventType.Repaint))
		{
			if (GUI.enabled)
			{
				Graphics.DrawTexture(position, image, srcRect, 0, 0, 0, 0);
			}
			else
			{
				Color color = GUI.color;
				float r = color.r;
				Color color2 = GUI.color;
				float g = color2.g;
				Color color3 = GUI.color;
				float b = color3.b;
				Color color4 = GUI.color;
				Graphics.DrawTexture(position, image, srcRect, 0, 0, 0, 0, new Color(r, g, b, color4.a * 0.5f));
			}
		}
	}
}
