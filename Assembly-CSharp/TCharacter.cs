using UnityEngine;

public class TCharacter : TItem
{
	public string prefix;

	public string gender;

	public Material mainMat;

	public TCharacter(string itemCode, string itemName, Texture2D itemIcon, int ct, bool itemTakeoffable, string _gender, string _prefix, string itemComment, TBuff tb, bool itemDiscomposable, string itemBpBackCode, int _season, Material itemMainMat, string _grp1, string _grp2, string _grp3, int starRate)
		: base(itemCode, TYPE.CHARACTER, itemName, itemIcon, ct, 0, itemTakeoffable, SLOT.CHARACTER, itemComment, tb, itemDiscomposable, itemBpBackCode, -1, UPGRADE_CATEGORY.NONE, basic: false, starRate)
	{
		gender = _gender;
		prefix = _prefix;
		season = _season;
		mainMat = itemMainMat;
		grp1 = _grp1;
		grp2 = _grp2;
		grp3 = _grp3;
	}
}
