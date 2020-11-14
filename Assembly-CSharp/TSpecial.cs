using UnityEngine;

public class TSpecial : TItem
{
	public int functionMask;

	public string param;

	public bool IsConsumableBuff => functionMask == 82 || functionMask == 81 || functionMask == 84 || functionMask == 83 || functionMask == 86 || functionMask == 85;

	public TSpecial(string itemCode, string itemName, Texture2D itemIcon, int ct, bool _isAmount, int _functionMask, string itemComment, bool itemDiscomposable, string itemBpBackCode, bool basic, int _season, string _param, int starRate)
		: base(itemCode, TYPE.SPECIAL, itemName, itemIcon, ct, 0, itemTakeoffable: false, SLOT.NONE, itemComment, null, itemDiscomposable, itemBpBackCode, -1, UPGRADE_CATEGORY.NONE, basic, starRate)
	{
		functionMask = _functionMask;
		IsAmount = _isAmount;
		season = _season;
		param = _param;
	}

	public int Param2Index()
	{
		int result = -1;
		if (param.Length > 0)
		{
			if (IsConsumableBuff)
			{
				TBuff tBuff = BuffManager.Instance.Get(param);
				if (tBuff != null)
				{
					result = tBuff.Index;
				}
			}
			else if (functionMask == 80)
			{
				try
				{
					return int.Parse(param);
				}
				catch
				{
					return -1;
				}
			}
		}
		return result;
	}
}
