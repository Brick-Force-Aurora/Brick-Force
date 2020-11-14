using System;
using UnityEngine;

[Serializable]
public class ClanConfirmDialog : Dialog
{
	public enum CLAN_CONFIRM_WHAT
	{
		DESTROY_CLAN,
		KICK_CLAN_MEMBER,
		LEAVE_CLAN,
		DELEGATE_MASTER
	}

	public Texture2D icon;

	public Rect crdComment = new Rect(15f, 50f, 310f, 73f);

	public Rect crdCommentLabel = new Rect(15f, 50f, 310f, 73f);

	private Rect crdOk = new Rect(350f, 170f, 90f, 34f);

	private CLAN_CONFIRM_WHAT what;

	private int targetSeq = -1;

	private string targetName = string.Empty;

	private string objectName = string.Empty;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.CLAN_CONFIRM;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog(CLAN_CONFIRM_WHAT _what, int seq, string name, string objName)
	{
		what = _what;
		targetSeq = seq;
		targetName = name;
		objectName = objName;
	}

	private void DoTitle()
	{
		Vector2 pos = new Vector2(size.x / 2f, 10f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("CLAN"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		DoTitle();
		GUI.Box(crdComment, string.Empty, "LineBoxBlue");
		switch (what)
		{
		case CLAN_CONFIRM_WHAT.DESTROY_CLAN:
			GUI.Label(crdCommentLabel, StringMgr.Instance.Get("SURE_DESTROY_CLAN"), "MiddleCenterLabel");
			if (GlobalVars.Instance.MyButton(crdOk, StringMgr.Instance.Get("OK"), "BtnAction"))
			{
				CSNetManager.Instance.Sock.SendCS_DESTROY_CLAN_REQ(MyInfoManager.Instance.ClanSeq);
				result = true;
			}
			break;
		case CLAN_CONFIRM_WHAT.KICK_CLAN_MEMBER:
			GUI.Label(crdCommentLabel, string.Format(StringMgr.Instance.Get("CONFIRM_KICK_CLAN_MEMBER"), targetName), "MiddleCenterLabel");
			if (GlobalVars.Instance.MyButton(crdOk, StringMgr.Instance.Get("OK"), "BtnAction"))
			{
				string contents2 = "CLAN_EXILE_COMMENT" + GlobalVars.DELIMITER + "n" + MyInfoManager.Instance.ClanName;
				CSNetManager.Instance.Sock.SendCS_CLAN_KICK_REQ(MyInfoManager.Instance.ClanSeq, targetSeq, targetName, "CLAN_EXILE", contents2);
				result = true;
			}
			break;
		case CLAN_CONFIRM_WHAT.LEAVE_CLAN:
			GUI.Label(crdCommentLabel, StringMgr.Instance.Get("CONFIRM_CLAN_LEAVE"), "MiddleCenterLabel");
			if (GlobalVars.Instance.MyButton(crdOk, StringMgr.Instance.Get("OK"), "BtnAction"))
			{
				CSNetManager.Instance.Sock.SendCS_LEAVE_CLAN_REQ();
				result = true;
			}
			break;
		case CLAN_CONFIRM_WHAT.DELEGATE_MASTER:
			GUI.Label(crdCommentLabel, string.Format(StringMgr.Instance.Get("SURE_DELEGATE_MASTER"), targetName), "MiddleCenterLabel");
			if (GlobalVars.Instance.MyButton(crdOk, StringMgr.Instance.Get("OK"), "BtnAction"))
			{
				string contents = "CLAN_PROMOTE_COMMENT" + GlobalVars.DELIMITER + "n" + MyInfoManager.Instance.ClanName + GlobalVars.DELIMITER + "n" + objectName;
				CSNetManager.Instance.Sock.SendCS_TRANSFER_MASTER_REQ(MyInfoManager.Instance.ClanSeq, targetSeq, targetName, "CLAN_PROMOTE", contents);
				result = true;
			}
			break;
		}
		Rect rc = new Rect(size.x - 44f, 5f, 34f, 34f);
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
