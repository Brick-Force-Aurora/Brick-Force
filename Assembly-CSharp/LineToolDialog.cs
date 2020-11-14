using System;
using UnityEngine;

[Serializable]
public class LineToolDialog : Dialog
{
	public Texture2D gaugeFrame;

	public Texture2D gauge;

	private bool progressing;

	private Brick with;

	private LineTool lineTool;

	private int doneCount;

	private Item item;

	private Rect crdBrick = new Rect(122f, 56f, 64f, 64f);

	private Vector2 crdProgress = new Vector2(156f, 140f);

	private Rect crdGaugeFrame = new Rect(20f, 128f, 272f, 26f);

	private Rect crdOK = new Rect(20f, 172f, 128f, 34f);

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.LINE_TOOL;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog(ref LineTool tool, Brick brick)
	{
		long num = MyInfoManager.Instance.HaveFunction("line_tool");
		if (num >= 0)
		{
			item = MyInfoManager.Instance.GetItemBySequence(num);
		}
		lineTool = tool;
		doneCount = 0;
		with = null;
		if (brick != null)
		{
			if (brick.ticket.Length <= 0 || MyInfoManager.Instance.HaveFunction(brick.ticket) >= 0)
			{
				with = brick;
			}
			else
			{
				SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("BUY_CREATIVE_TICKET"));
			}
		}
		progressing = false;
	}

	public override bool DoDialog()
	{
		if (with == null)
		{
			return true;
		}
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		Vector2 pos = new Vector2(size.x / 2f, 10f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("LINE_TOOL"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		GUI.Box(crdBrick, with.Icon, "ButtonBrick");
		int num = lineTool.Count + doneCount;
		if (progressing)
		{
			num++;
		}
		float num2 = (float)doneCount / (float)num;
		TextureUtil.DrawTexture(new Rect(crdGaugeFrame.x + 4f, crdGaugeFrame.y + 4f, (crdGaugeFrame.width - 8f) * num2, crdGaugeFrame.height - 8f), gauge, ScaleMode.StretchToFill);
		TextureUtil.DrawTexture(crdGaugeFrame, gaugeFrame, ScaleMode.StretchToFill);
		LabelUtil.TextOut(crdProgress, doneCount.ToString() + "/" + num.ToString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		bool enabled = GUI.enabled;
		GUI.enabled = (lineTool.Count > 0 && !progressing);
		if (GlobalVars.Instance.MyButton(crdOK, StringMgr.Instance.Get("START"), "BtnAction"))
		{
			MoveFirst();
		}
		GUI.enabled = enabled;
		Rect rc = new Rect(size.x - 44f, 5f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			GlobalVars.Instance.resetMenuEx();
			result = true;
		}
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}

	public void MoveFirst()
	{
		byte x = 0;
		byte y = 0;
		byte z = 0;
		byte rot = 0;
		if (lineTool.MoveFirst(with, ref x, ref y, ref z, ref rot))
		{
			progressing = true;
			CSNetManager.Instance.Sock.SendCS_LINE_BRICK_REQ(item.Seq, item.Template.code, with.index, x, y, z, rot);
		}
	}

	public void MoveNext(bool success)
	{
		if (success)
		{
			doneCount++;
		}
		byte x = 0;
		byte y = 0;
		byte z = 0;
		byte rot = 0;
		if (!lineTool.MoveNext(with, ref x, ref y, ref z, ref rot))
		{
			progressing = false;
		}
		else
		{
			CSNetManager.Instance.Sock.SendCS_LINE_BRICK_REQ(item.Seq, item.Template.code, with.index, x, y, z, rot);
		}
	}
}
