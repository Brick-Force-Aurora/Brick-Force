using System;
using UnityEngine;

[Serializable]
public class UIScrollView : UIBaseList
{
	public Vector2 area;

	public float offSetY;

	public int offSetXCount = 1;

	public float offSetX;

	public int listCount;

	public Vector2 scrollPoint = Vector2.zero;

	public override bool Draw()
	{
		if (IsSkipAble())
		{
			return SkipDraw();
		}
		return base.Draw();
	}

	public void BeginScroll()
	{
		Rect viewRect = new Rect(0f, 0f, area.x - 25f, (float)((listCount - 1) / offSetXCount + 1) * offSetY);
		Vector2 showPosition = base.showPosition;
		float x = showPosition.x;
		Vector2 showPosition2 = base.showPosition;
		scrollPoint = GUI.BeginScrollView(new Rect(x, showPosition2.y, area.x, area.y), scrollPoint, viewRect);
	}

	public void EndScroll()
	{
		GUI.EndScrollView();
	}

	public void SetListCount(int count)
	{
		listCount = count;
	}

	public int GetListCount()
	{
		return listCount;
	}

	public void SetListPostion(int i)
	{
		ListResetAddPosition();
		if (offSetXCount >= 1)
		{
			int num = i % offSetXCount;
			int num2 = i / offSetXCount;
			ListAddPositionX(offSetX * (float)num);
			ListAddPositionY(offSetY * (float)num2);
		}
	}

	public bool IsSkipAble()
	{
		float y = scrollPoint.y;
		float num = y + area.y;
		float x = scrollPoint.x;
		float num2 = x + area.x;
		float num3 = 1E+08f;
		float num4 = 1E+08f;
		for (int i = 0; i < listBases.Count; i++)
		{
			Vector2 showPosition = listBases[i].showPosition;
			if (showPosition.y < num3)
			{
				Vector2 showPosition2 = listBases[i].showPosition;
				num3 = showPosition2.y;
			}
			Vector2 showPosition3 = listBases[i].showPosition;
			if (showPosition3.x < num4)
			{
				Vector2 showPosition4 = listBases[i].showPosition;
				num4 = showPosition4.x;
			}
		}
		if (num3 + offSetY < y)
		{
			return true;
		}
		if (num3 > num)
		{
			return true;
		}
		if (num4 + offSetX < x)
		{
			return true;
		}
		if (num4 > num2)
		{
			return true;
		}
		return false;
	}
}
