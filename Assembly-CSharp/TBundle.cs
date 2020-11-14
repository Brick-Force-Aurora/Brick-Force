using UnityEngine;

public class TBundle : TItem
{
	public TBundle(string itemCode, string itemName, Texture2D itemIcon, int ct, bool _isAmount, string itemComment, int _season, int starRate)
		: base(itemCode, TYPE.BUNDLE, itemName, itemIcon, ct, 0, itemTakeoffable: false, SLOT.NONE, itemComment, null, itemDiscomposable: false, string.Empty, -1, UPGRADE_CATEGORY.NONE, basic: false, starRate)
	{
		IsAmount = _isAmount;
		season = _season;
	}
}
