using System;
using UnityEngine;

[Serializable]
public class OfficialMapOnly : Dialog
{
	private bool dontShowThisMessageAgain;

	private Rect crdMsg = new Rect(30f, 30f, 590f, 160f);

	private Rect crdMsgOutline = new Rect(25f, 25f, 650f, 200f);

	private Rect crdOk = new Rect(550f, 245f, 128f, 34f);

	private Rect crdDontShowThisMessageAgaing = new Rect(25f, 245f, 400f, 20f);

	public bool DontShowThisMessageAgain => dontShowThisMessageAgain;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.OFFICIAL_MAP_ONLY;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog()
	{
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		GUI.Box(crdMsgOutline, string.Empty, "LineBoxBlue");
		GUI.Label(crdMsg, StringMgr.Instance.Get("OFFICIAL_MAP_ONLY"), "MiddleCenterLabel");
		dontShowThisMessageAgain = GUI.Toggle(crdDontShowThisMessageAgaing, dontShowThisMessageAgain, StringMgr.Instance.Get("DONT_SHOW_AGAIN"));
		if (GlobalVars.Instance.MyButton(crdOk, StringMgr.Instance.Get("OK"), "BtnAction"))
		{
			if (dontShowThisMessageAgain)
			{
				MyInfoManager.Instance.SaveDonotCommonMask(MyInfoManager.COMMON_OPT.DONOT_NEWBIE_CHANNEL_MSG);
			}
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
