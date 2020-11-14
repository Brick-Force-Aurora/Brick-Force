using System;
using UnityEngine;

[Serializable]
public class ChangeNoticeDialog : Dialog
{
	public int maxNotice = 100;

	public Rect crdNotice = new Rect(14f, 48f, 336f, 217f);

	private Rect crdOk = new Rect(299f, 252f, 80f, 34f);

	private string notice = string.Empty;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.CHANGE_NOTICE;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog()
	{
		notice = string.Empty;
	}

	private void DoTitle()
	{
		Vector2 pos = new Vector2(size.x / 2f, 15f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("CLAN_NOTICE"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
	}

	private void DoNotice()
	{
		notice = GUI.TextArea(crdNotice, notice, maxNotice);
	}

	private bool CheckBadword()
	{
		string text = WordFilter.Instance.CheckBadword(notice);
		if (text.Length > 0)
		{
			MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("BAD_WORD_DETECT"), text));
			return false;
		}
		return true;
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		DoTitle();
		DoNotice();
		if (GlobalVars.Instance.MyButton(crdOk, StringMgr.Instance.Get("OK"), "BtnAction") && CheckBadword())
		{
			CSNetManager.Instance.Sock.SendCS_CHANGE_CLAN_NOTICE_REQ(MyInfoManager.Instance.ClanSeq, notice);
			result = true;
		}
		Rect rc = new Rect(size.x - 39f, 5f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
		}
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}
}
