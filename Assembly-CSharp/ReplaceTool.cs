using UnityEngine;

public class ReplaceTool : EditorTool
{
	public ReplaceTool(EditorToolScript ets, Item i, BattleChat _battleChat)
		: base(ets, i, _battleChat)
	{
	}

	public override bool IsEnable()
	{
		return item != null && item.EnoughToConsume;
	}

	public override bool Update()
	{
		if (!battleChat.IsChatting && custom_inputs.Instance.GetButtonDown(editorToolScript.inputKey) && IsEnable())
		{
			active = true;
			return true;
		}
		if (!battleChat.IsChatting && custom_inputs.Instance.GetButtonDown(editorToolScript.inputKey) && !IsEnable())
		{
			GameObject gameObject = GameObject.Find("Me");
			if (null != gameObject)
			{
				LocalController component = gameObject.GetComponent<LocalController>();
				if (null != component)
				{
					component.addStatusMsg(StringMgr.Instance.Get("ITEM_USED_ALL"));
				}
			}
			return false;
		}
		return false;
	}
}
