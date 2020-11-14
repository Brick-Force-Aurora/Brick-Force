using UnityEngine;

public class TUpgrade : TItem
{
	public int tier;

	public string target;

	public int playerLv;

	public int reqLv;

	public int maxLv;

	public int targetType;

	public TUpgrade(string itemCode, string itemName, Texture2D itemIcon, int ct, int _tier, string _target, int _playerLv, int _reqLv, int _maxLv, string itemComment, int starRate)
		: base(itemCode, TYPE.UPGRADE, itemName, itemIcon, ct, 0, itemTakeoffable: false, SLOT.NONE, itemComment, null, itemDiscomposable: false, string.Empty, -1, UPGRADE_CATEGORY.NONE, basic: false, starRate)
	{
		tier = _tier;
		target = _target;
		playerLv = _playerLv;
		reqLv = _reqLv;
		maxLv = _maxLv;
		targetType = TItem.String2UpgradeType(target);
		IsAmount = true;
	}
}
