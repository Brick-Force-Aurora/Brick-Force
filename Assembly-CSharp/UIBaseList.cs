using System;
using System.Collections.Generic;

[Serializable]
public class UIBaseList : UIBase
{
	public List<UIBase> listBases = new List<UIBase>();

	public override bool Draw()
	{
		if (!isDraw)
		{
			return false;
		}
		for (int i = 0; i < listBases.Count; i++)
		{
			listBases[i].Draw();
		}
		return false;
	}

	public override bool SkipDraw()
	{
		for (int i = 0; i < listBases.Count; i++)
		{
			listBases[i].SkipDraw();
		}
		return false;
	}

	public void ListAddPositionX(float x)
	{
		for (int i = 0; i < listBases.Count; i++)
		{
			listBases[i].AddPositionX(x);
		}
	}

	public void ListAddPositionY(float y)
	{
		for (int i = 0; i < listBases.Count; i++)
		{
			listBases[i].AddPositionY(y);
		}
	}

	public void ListResetAddPosition()
	{
		for (int i = 0; i < listBases.Count; i++)
		{
			listBases[i].ResetAddPosition();
		}
	}
}
