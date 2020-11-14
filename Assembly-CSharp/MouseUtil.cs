using UnityEngine;

public class MouseUtil
{
	public static Vector2 ScreenToPixelPoint(Vector2 screenPoint)
	{
		return new Vector2(screenPoint.x, (float)Screen.height - screenPoint.y);
	}

	public static bool MouseOver(Rect rc)
	{
		Vector2 vector = ScreenToPixelPoint(Input.mousePosition);
		if (rc.x > vector.x || vector.x > rc.x + rc.width || rc.y > vector.y || vector.y > rc.y + rc.height)
		{
			return false;
		}
		return true;
	}

	public static bool ClickInside(Rect rc)
	{
		if (!Input.GetMouseButtonDown(0) && Input.GetMouseButtonDown(1) && Input.GetMouseButtonDown(2))
		{
			return false;
		}
		Vector2 vector = GlobalVars.Instance.PixelToGUIPoint(ScreenToPixelPoint(Input.mousePosition));
		if (rc.x > vector.x || vector.x > rc.x + rc.width || rc.y > vector.y || vector.y > rc.y + rc.height)
		{
			return false;
		}
		return true;
	}

	public static bool ClickOutside(Rect rc)
	{
		if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
		{
			Vector2 vector = GlobalVars.Instance.PixelToGUIPoint(ScreenToPixelPoint(Input.mousePosition));
			if (rc.x > vector.x || vector.x > rc.x + rc.width || rc.y > vector.y || vector.y > rc.y + rc.height)
			{
				return true;
			}
		}
		return false;
	}
}
