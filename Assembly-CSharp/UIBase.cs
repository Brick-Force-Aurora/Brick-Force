using UnityEngine;

public class UIBase
{
	protected bool isDraw = true;

	public Vector2 position;

	private Vector2 addPosition = Vector2.zero;

	public bool IsDraw
	{
		get
		{
			return isDraw;
		}
		set
		{
			isDraw = value;
		}
	}

	public Vector2 showPosition => position + addPosition;

	public virtual bool Draw()
	{
		return false;
	}

	public virtual bool SkipDraw()
	{
		return false;
	}

	public virtual bool Update()
	{
		return true;
	}

	public void AddPositionX(float x)
	{
		addPosition.x += x;
	}

	public void AddPositionY(float y)
	{
		addPosition.y += y;
	}

	public void ResetAddPosition()
	{
		addPosition = Vector2.zero;
	}
}
