using System;

[Serializable]
public class UILabelList : UIBase
{
	public UILabel[] uiLabels;

	public override bool Draw()
	{
		if (!isDraw)
		{
			return false;
		}
		if (uiLabels == null)
		{
			return false;
		}
		if (uiLabels.Length == 0)
		{
			return false;
		}
		for (int i = 0; i < uiLabels.Length; i++)
		{
			uiLabels[i].Draw();
		}
		return false;
	}
}
