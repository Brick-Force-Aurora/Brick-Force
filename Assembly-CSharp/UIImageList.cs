using System;

[Serializable]
public class UIImageList : UIBase
{
	public UIImage[] uiImages;

	public override bool Draw()
	{
		if (!isDraw)
		{
			return false;
		}
		if (uiImages == null)
		{
			return false;
		}
		if (uiImages.Length == 0)
		{
			return false;
		}
		for (int i = 0; i < uiImages.Length; i++)
		{
			uiImages[i].Draw();
		}
		return false;
	}
}
