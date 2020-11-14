using System;
using UnityEngine;

[Serializable]
public class UIImage : UIBase
{
	public Vector2 area;

	public Texture2D texImage;

	public string guiStyle;

	public override bool Draw()
	{
		if (!isDraw)
		{
			return false;
		}
		if (texImage != null)
		{
			if (area == Vector2.zero)
			{
				Vector2 showPosition = base.showPosition;
				float x = showPosition.x;
				Vector2 showPosition2 = base.showPosition;
				TextureUtil.DrawTexture(new Rect(x, showPosition2.y, (float)texImage.width, (float)texImage.height), texImage);
			}
			else
			{
				Vector2 showPosition3 = base.showPosition;
				float x2 = showPosition3.x;
				Vector2 showPosition4 = base.showPosition;
				TextureUtil.DrawTexture(new Rect(x2, showPosition4.y, area.x, area.y), texImage);
			}
		}
		else if (guiStyle != null && guiStyle.Length > 0)
		{
			Vector2 showPosition5 = base.showPosition;
			float x3 = showPosition5.x;
			Vector2 showPosition6 = base.showPosition;
			GUI.Box(new Rect(x3, showPosition6.y, area.x, area.y), string.Empty, guiStyle);
		}
		return false;
	}
}
