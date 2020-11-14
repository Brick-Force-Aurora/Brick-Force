using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ReplaceToolDialog : Dialog
{
	public Texture2D gaugeFrame;

	public Texture2D gauge;

	private bool progressing;

	private Brick prev;

	private Brick next;

	private Queue<BrickInst> todo;

	private int doneCount;

	private Item item;

	private Rect crdPrev = new Rect(64f, 56f, 64f, 64f);

	private Rect crdNext = new Rect(184f, 56f, 64f, 64f);

	private Rect crdReplaceDst = new Rect(35f, 128f, 240f, 34f);

	private Vector2 crdProgress = new Vector2(156f, 184f);

	private Rect crdGaugeFrame = new Rect(20f, 172f, 272f, 26f);

	private Rect crdOK = new Rect(20f, 220f, 128f, 34f);

	public Brick Prev
	{
		get
		{
			return prev;
		}
		set
		{
			prev = value;
		}
	}

	public Brick Next
	{
		get
		{
			return next;
		}
		set
		{
			next = value;
		}
	}

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.REPLACE_TOOL;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog(Brick src, Brick dst)
	{
		long num = MyInfoManager.Instance.HaveFunction("replace_tool");
		if (num >= 0)
		{
			item = MyInfoManager.Instance.GetItemBySequence(num);
		}
		prev = src;
		next = null;
		if (dst != null && dst.CheckReplace() == Brick.REPLACE_CHECK.OK && (dst.ticket.Length <= 0 || MyInfoManager.Instance.HaveFunction(dst.ticket) >= 0))
		{
			next = dst;
		}
		doneCount = 0;
		BrickInst[] array = BrickManager.Instance.ToBrickInstArray(src);
		todo = new Queue<BrickInst>();
		for (int i = 0; i < array.Length; i++)
		{
			todo.Enqueue(array[i]);
		}
		progressing = false;
	}

	public override bool DoDialog()
	{
		bool result = false;
		if (item == null)
		{
			return true;
		}
		if (prev == null)
		{
			return true;
		}
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		Vector2 pos = new Vector2(size.x / 2f, 10f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("REPLACE_TOOL"), "BigLabel", GlobalVars.Instance.txtMainColor, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperCenter);
		GUI.Box(crdPrev, prev.Icon, "ButtonBrick");
		if (next == null)
		{
			GUI.Box(crdNext, string.Empty, "ButtonBrick");
		}
		else
		{
			GUI.Box(crdNext, next.Icon, "ButtonBrick");
		}
		int num = todo.Count + doneCount;
		if (progressing)
		{
			num++;
		}
		float num2 = (float)doneCount / (float)num;
		TextureUtil.DrawTexture(new Rect(crdGaugeFrame.x + 4f, crdGaugeFrame.y + 4f, (crdGaugeFrame.width - 8f) * num2, crdGaugeFrame.height - 8f), gauge, ScaleMode.StretchToFill);
		TextureUtil.DrawTexture(crdGaugeFrame, gaugeFrame, ScaleMode.StretchToFill);
		LabelUtil.TextOut(crdProgress, doneCount.ToString() + "/" + num.ToString(), "Label", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.MiddleCenter);
		bool enabled = GUI.enabled;
		GUI.enabled = (todo.Count > 0 && !progressing);
		if (GlobalVars.Instance.MyButton(crdReplaceDst, StringMgr.Instance.Get("CHG_REPLACE_DST"), "BtnAction"))
		{
			((ReplaceDstDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.REPLACE_DST, exclusive: false))?.InitDialog(this);
		}
		GUI.enabled = (next != null && todo.Count > 0 && !progressing);
		if (GlobalVars.Instance.MyButton(crdOK, StringMgr.Instance.Get("START"), "BtnAction"))
		{
			MoveFirst();
		}
		GUI.enabled = enabled;
		Rect rc = new Rect(size.x - 50f, 10f, 34f, 34f);
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

	private bool CheckCount(Brick with)
	{
		if (with.maxInstancePerMap < 0)
		{
			return true;
		}
		if (BrickManager.Instance.CountLimitedBrick(with.GetIndex()) < with.maxInstancePerMap)
		{
			return true;
		}
		SystemMsgManager.Instance.ShowMessage("OVER_NUM");
		return false;
	}

	private void MoveFirst()
	{
		BrickInst brickInst = null;
		if (CheckCount(next) && UserMapInfoManager.Instance.CheckAuth(showMessage: true) && BrickManager.Instance.checkAddMinMaxGravity(next.seq))
		{
			while (brickInst == null && todo.Count > 0)
			{
				brickInst = todo.Peek();
				BrickInst brickInst2 = BrickManager.Instance.GetBrickInst(brickInst.Seq);
				if (brickInst2 == null)
				{
					brickInst = null;
				}
				todo.Dequeue();
			}
		}
		if (brickInst != null)
		{
			progressing = true;
			CSNetManager.Instance.Sock.SendCS_REPLACE_BRICK_REQ(item.Seq, item.Template.code, brickInst.Seq, next.index, brickInst.PosX, brickInst.PosY, brickInst.PosZ, brickInst.Rot);
		}
	}

	public void MoveNext(bool success)
	{
		if (success)
		{
			doneCount++;
		}
		BrickInst brickInst = null;
		if (CheckCount(next) && UserMapInfoManager.Instance.CheckAuth(showMessage: true))
		{
			while (brickInst == null && todo.Count > 0)
			{
				brickInst = todo.Peek();
				BrickInst brickInst2 = BrickManager.Instance.GetBrickInst(brickInst.Seq);
				if (brickInst2 == null)
				{
					brickInst = null;
				}
				todo.Dequeue();
			}
		}
		if (brickInst == null)
		{
			progressing = false;
		}
		else
		{
			CSNetManager.Instance.Sock.SendCS_REPLACE_BRICK_REQ(item.Seq, item.Template.code, brickInst.Seq, next.index, brickInst.PosX, brickInst.PosY, brickInst.PosZ, brickInst.Rot);
		}
	}
}
