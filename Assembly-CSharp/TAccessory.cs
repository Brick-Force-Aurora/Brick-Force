using UnityEngine;

public class TAccessory : TItem
{
	public GameObject prefab;

	public string bone;

	public int functionMask;

	public float functionFactor;

	public int armor;

	private int[] ah_armor;

	private int ah_key;

	private int ah_index;

	public TAccessory(string itemCode, string itemName, string itemBone, GameObject itemPrefab, Texture2D itemIcon, int ct, int ck, bool itemTakeoffable, SLOT itemSlot, string itemComment, TBuff tb, bool itemDiscomposable, string itemBpBackCode, int _functionMask, int _armor, float _functionFactor, int upCat, int _season, string _grp1, string _grp2, string _grp3, Texture2D _funcIcon, int starRate)
		: base(itemCode, TYPE.ACCESSORY, itemName, itemIcon, ct, ck, itemTakeoffable, itemSlot, itemComment, tb, itemDiscomposable, itemBpBackCode, 1, (UPGRADE_CATEGORY)upCat, basic: false, starRate)
	{
		armor = _armor;
		functionMask = _functionMask;
		prefab = itemPrefab;
		bone = itemBone;
		season = _season;
		ah_armor = new int[5];
		ah_key = itemName.Length;
		ah_index = ah_key % 5;
		ah_armor[ah_index] = armor << 1;
		functionFactor = _functionFactor;
		grp1 = _grp1;
		grp2 = _grp2;
		grp3 = _grp3;
		funcIcon = _funcIcon;
	}

	public void resetArmor(int val)
	{
		armor = val;
		ah_armor = new int[5];
		ah_key = base.Name.Length;
		ah_index = ah_key % 5;
		ah_armor[ah_index] = armor << 1;
	}

	public void safeArmor()
	{
		int num = ah_armor[ah_index] >> 1;
		if (num != armor)
		{
			Application.Quit();
		}
	}
}
