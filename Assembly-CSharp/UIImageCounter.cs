using System;
using UnityEngine;

[Serializable]
public class UIImageCounter : UIImage
{
	public float offSetY;

	public int offSetXCount = 1;

	public float offSetX;

	private int listCount;

	private Vector2 curPosition = default(Vector2);

	public override bool Draw()
	{
		if (!isDraw)
		{
			return false;
		}
		for (int i = 0; i < listCount; i++)
		{
			ref Vector2 reference = ref curPosition;
			Vector2 showPosition = base.showPosition;
			reference.x = showPosition.x + offSetX * (float)(i % offSetXCount);
			ref Vector2 reference2 = ref curPosition;
			Vector2 showPosition2 = base.showPosition;
			reference2.y = showPosition2.y + offSetY * (float)(i / offSetXCount);
			if (texImage != null)
			{
				if (area == Vector2.zero)
				{
					TextureUtil.DrawTexture(new Rect(curPosition.x, curPosition.y, (float)texImage.width, (float)texImage.height), texImage);
				}
				else
				{
					TextureUtil.DrawTexture(new Rect(curPosition.x, curPosition.y, area.x, area.y), texImage);
				}
			}
			else if (guiStyle != null && guiStyle.Length > 0)
			{
				GUI.Box(new Rect(curPosition.x, curPosition.y, area.x, area.y), string.Empty, guiStyle);
			}
		}
		return false;
	}

	public void SetListCount(int count)
	{
		listCount = count;
	}
}
