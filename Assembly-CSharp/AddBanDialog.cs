using System;
using UnityEngine;

[Serializable]
public class AddBanDialog : Dialog
{
	public int maxId = 5;

	private string banWannabe = string.Empty;

	public Vector2 crdTitle = new Vector2(0f, 0f);

	public Vector2 crdMessage = new Vector2(0f, 0f);

	public Rect crdBanTxtFld = new Rect(173f, 105f, 194f, 27f);

	private Rect crdAddBan = new Rect(250f, 165f, 130f, 34f);

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.ADD_BAN;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		Vector2 pos = new Vector2(size.x / 2f, 15f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("ADD_BAN"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		LabelUtil.TextOut(crdMessage, StringMgr.Instance.Get("INPUT_NICKNAME4BAN"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		string text = banWannabe;
		GUI.SetNextControlName("BanWannabe");
		banWannabe = GUI.TextField(crdBanTxtFld, banWannabe);
		if (banWannabe.Length > maxId)
		{
			banWannabe = text;
		}
		if (GlobalVars.Instance.MyButton(crdAddBan, StringMgr.Instance.Get("ADD_BAN"), "BtnAction"))
		{
			banWannabe.Trim();
			if (banWannabe.Length > 0)
			{
				CSNetManager.Instance.Sock.SendCS_ADD_BAN_BY_NICKNAME_REQ(banWannabe);
				result = true;
			}
			else
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("INPUT_NICKNAME4BAN"));
			}
		}
		Rect rc = new Rect(size.x - 44f, 5f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
		}
		Dialog top = DialogManager.Instance.GetTop();
		if (top != null && top.ID == id)
		{
			GUI.FocusWindow((int)id);
			GUI.FocusControl("BanWannabe");
		}
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}

	public void InitDialog()
	{
		banWannabe = string.Empty;
	}
}
