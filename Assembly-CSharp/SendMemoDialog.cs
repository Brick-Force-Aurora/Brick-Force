using System;
using UnityEngine;

[Serializable]
public class SendMemoDialog : Dialog
{
	public int maxId = 10;

	public int maxTitle = 16;

	public int maxMemoLength = 500;

	private string receiver = string.Empty;

	private string title = string.Empty;

	private string contents = string.Empty;

	private Good good;

	private TItem tItem;

	private Good.BUY_HOW buyHow;

	private int selected;

	private bool doDialogOnce;

	public Texture2D icon;

	public Rect crdIcon = new Rect(0f, 0f, 12f, 12f);

	public Rect crdMemoOutline = new Rect(0f, 0f, 100f, 100f);

	public Rect crdPresentOutline = new Rect(0f, 0f, 100f, 100f);

	public Vector2 crdRecvLabel = new Vector2(114f, 44f);

	public Rect crdRecvTxtFld = new Rect(116f, 44f, 220f, 27f);

	public Vector2 crdMemoTitleLabel = new Vector2(114f, 74f);

	public Rect crdMemoTitleTxtFld = new Rect(116f, 74f, 220f, 27f);

	public Rect crdMemoContents = new Rect(30f, 108f, 280f, 260f);

	private string autoChargeConfirm = string.Empty;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.SEND_MEMO;
		doDialogOnce = false;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	private void ShowPresent()
	{
		GUI.Box(crdPresentOutline, string.Empty, "BoxFadeBlue");
		if (good != null && tItem != null && null != tItem.CurIcon())
		{
			TextureUtil.DrawTexture(new Rect(crdPresentOutline.x + 20f, crdPresentOutline.y + 20f, (float)tItem.CurIcon().width, (float)tItem.CurIcon().height), tItem.CurIcon());
			Vector2 pos = new Vector2(crdPresentOutline.x + crdPresentOutline.width - 20f, crdPresentOutline.y + 20f);
			LabelUtil.TextOut(pos, tItem.Name, "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			pos.y += 20f;
			LabelUtil.TextOut(pos, good.GetRemainString(selected, buyHow), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			pos.y += 20f;
			LabelUtil.TextOut(pos, good.GetPriceString(selected, buyHow), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		}
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		Vector2 pos = new Vector2(size.x / 2f, 10f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("PRESENT"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		LabelUtil.TextOut(crdRecvLabel, StringMgr.Instance.Get("MEMO_RCV"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		string text = receiver;
		GUI.SetNextControlName("MemoReceiverInput");
		receiver = GUI.TextField(crdRecvTxtFld, receiver);
		if (receiver.Length > maxId)
		{
			receiver = text;
		}
		LabelUtil.TextOut(crdMemoTitleLabel, StringMgr.Instance.Get("MEMO_TITLE"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		string text2 = title;
		GUI.SetNextControlName("MemoTitleInput");
		title = GUI.TextField(crdMemoTitleTxtFld, title);
		if (title.Length > maxTitle)
		{
			title = text2;
		}
		GUI.SetNextControlName("MemoContentsInput");
		contents = GUI.TextArea(crdMemoContents, contents, maxMemoLength);
		ShowPresent();
		Rect rc = new Rect(size.x - 115f, size.y - 44f, 100f, 34f);
		if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("PRESENT"), "BtnAction"))
		{
			receiver.Trim();
			title.Trim();
			if (receiver.Length <= 0)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("INPUT_MEMO_RECEIVER"));
			}
			else if (title.Length <= 0)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("INPUT_MEMO_TITLE"));
			}
			else
			{
				string a = MyInfoManager.Instance.Nickname.ToLower();
				string b = receiver.ToLower();
				if (a == b)
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CANNOT_SELF_MEMO"));
				}
				else
				{
					((PresentConfirmDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.PRESENT_CONFIRM, exclusive: true))?.InitDialog(good, buyHow, selected, receiver, title, contents, autoChargeConfirm);
				}
			}
		}
		Rect rc2 = new Rect(size.x - 44f, 5f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc2, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
		}
		if (!doDialogOnce)
		{
			doDialogOnce = true;
			Dialog top = DialogManager.Instance.GetTop();
			if (top != null && top.ID == id)
			{
				if (receiver.Length == 0)
				{
					GUI.FocusControl("MemoReceiverInput");
				}
				else
				{
					GUI.FocusControl("MemoTitleInput");
				}
			}
		}
		GUI.skin = skin;
		return result;
	}

	public void InitDialog(Good g, Good.BUY_HOW how, int sel)
	{
		good = g;
		tItem = good.tItem;
		buyHow = how;
		selected = sel;
		receiver = string.Empty;
		title = tItem.Name;
		if (title.Length > maxTitle)
		{
			title = title.Remove(maxTitle);
		}
		contents = StringMgr.Instance.Get("DEFAULT_PRESENT_CONTENTS");
		doDialogOnce = false;
		autoChargeConfirm = string.Empty;
	}

	public void InitDialog(Good g, Good.BUY_HOW how, int sel, string extraConfirm)
	{
		InitDialog(g, how, sel);
		autoChargeConfirm = extraConfirm;
	}

	public void InitDialog(string rev)
	{
		receiver = rev;
	}
}
