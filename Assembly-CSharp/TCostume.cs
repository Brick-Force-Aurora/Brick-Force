using UnityEngine;

public class TCostume : TItem
{
	public string main;

	public string aux;

	public string mark;

	public int functionMask;

	public float functionFactor;

	public Material mainMat;

	public Material auxMat;

	public Material markMat;

	public int armor;

	private int[] ah_armor;

	private int ah_key;

	private int ah_index;

	public TCostume(string itemCode, string itemName, string itemMain, string itemAux, Material itemMainMat, Material itemAuxMat, Texture2D itemIcon, int ct, int ck, bool itemTakeoffable, SLOT itemSlot, string itemComment, TBuff tb, bool itemDiscomposable, string itemBpBackCode, int itemArmor, int upCat, bool basic, string itemMark, Material itemMarkMat, int _season, string _grp1, string _grp2, string _grp3, int _functionMask, float _functionFactor, Texture2D _funcIcon, int starRate)
		: base(itemCode, TYPE.CLOTH, itemName, itemIcon, ct, ck, itemTakeoffable, itemSlot, itemComment, tb, itemDiscomposable, itemBpBackCode, 1, (UPGRADE_CATEGORY)upCat, basic, starRate)
	{
		main = itemMain;
		aux = itemAux;
		mainMat = itemMainMat;
		auxMat = itemAuxMat;
		armor = itemArmor;
		mark = itemMark;
		markMat = itemMarkMat;
		ah_armor = new int[5];
		ah_key = itemName.Length;
		ah_index = ah_key % 5;
		ah_armor[ah_index] = armor << 1;
		season = _season;
		grp1 = _grp1;
		grp2 = _grp2;
		grp3 = _grp3;
		functionMask = _functionMask;
		functionFactor = _functionFactor;
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
