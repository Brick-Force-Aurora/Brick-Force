using System;
using UnityEngine;

public class ReplaceTool : EditorTool
{

    // AURORA - Start: Change Replace tool to Selection tool
    private byte x1, y1, z1;
	private byte x2, y2, z2;
	private bool hasPos1 = false, hasPos2 = false;
	private byte rotation;
    // AURORA - End

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

	// AURORA - Start: Change Replace tool to Selection tool
	public void SetPos1(byte x, byte y, byte z, byte rot)
    {
		x1 = x;
		y1 = y;
		z1 = z;
		rotation = rot;
		hasPos1 = true;
        GameObject main = GameObject.Find("Main");
        if (main != null)
        {
            main.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, "", $"[BrickEdit] Position 1 set to {x} {y} {z} ({GetBlockCount()} brick(s))"));
        }
    }

    public void SetPos2(byte x, byte y, byte z)
    {
		x2 = x;
		y2 = y;
		z2 = z;
        hasPos2 = true;
        GameObject main = GameObject.Find("Main");
        if (main != null)
        {
            main.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, "", $"[BrickEdit] Position 2 set to {x} {y} {z} ({GetBlockCount()} brick(s))"));
        }
    }

	public void GetRotation(out byte rotation)
	{
		rotation = this.rotation;
	}

    public void GetPos1(out byte x, out byte y, out byte z)
	{
		x = x1;
		y = y1;
		z = z1;
    }

    public void GetPos2(out byte x, out byte y, out byte z)
    {
        x = x2;
        y = y2;
        z = z2;
    }

    public int GetBlockCount()
	{
		if (!hasPos1 || !hasPos2)
		{
			return 0;
		}
		int width = Math.Abs(x2 - x1) + 1;
		int height = Math.Abs(y2 - y1) + 1;
		int depth = Math.Abs(z2 - z1) + 1;
		return width * height * depth;
    }
    // AURORA - End
}
