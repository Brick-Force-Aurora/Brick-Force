using System;
using UnityEngine;

[Serializable]
public class UIImageRotate : UIBase
{
	public Vector2 area;

	public Texture2D texImage;

	public float rotateSpeed = 30f;

	private float rotateAngle;

	public override bool Update()
	{
		rotateAngle += Time.deltaTime * rotateSpeed;
		return true;
	}

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
				area.x = (float)texImage.width;
				area.y = (float)texImage.height;
			}
			Matrix4x4 matrix = GUI.matrix;
			float angle = rotateAngle;
			GlobalVars instance = GlobalVars.Instance;
			Vector2 showPosition = base.showPosition;
			float x = showPosition.x + area.x * 0.5f;
			Vector2 showPosition2 = base.showPosition;
			GUIUtility.RotateAroundPivot(angle, instance.PixelToGUIScalePoint(new Vector2(x, showPosition2.y + area.y * 0.5f)));
			Vector2 showPosition3 = base.showPosition;
			float x2 = showPosition3.x;
			Vector2 showPosition4 = base.showPosition;
			TextureUtil.DrawTexture(new Rect(x2, showPosition4.y, area.x, area.y), texImage);
			GUI.matrix = matrix;
		}
		return false;
	}
}
