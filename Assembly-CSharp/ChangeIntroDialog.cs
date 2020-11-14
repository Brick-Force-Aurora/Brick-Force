using System;
using UnityEngine;

[Serializable]
public class ChangeIntroDialog : Dialog
{
	public Texture2D icon;

	public int maxIntro = 300;

	public Rect crdIcon = new Rect(2f, 2f, 34f, 34f);

	public Rect crdIntro = new Rect(14f, 48f, 336f, 217f);

	private Rect crdOk = new Rect(292f, 251f, 90f, 34f);

	private string intro = string.Empty;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.CHANGE_INTRO;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog()
	{
		intro = string.Empty;
	}

	private void DoTitle()
	{
		Vector2 pos = new Vector2(size.x / 2f, 10f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("CLAN_INTRO"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
	}

	private void DoIntro()
	{
		intro = GUI.TextArea(crdIntro, intro, maxIntro);
	}

	private bool CheckInput()
	{
		string text = WordFilter.Instance.CheckBadword(intro);
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
		DoIntro();
		if (GlobalVars.Instance.MyButton(crdOk, StringMgr.Instance.Get("OK"), "BtnAction") && CheckInput())
		{
			CSNetManager.Instance.Sock.SendCS_CHANGE_CLAN_INTRO_REQ(MyInfoManager.Instance.Seq, intro);
			result = true;
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
