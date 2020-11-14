using System;
using UnityEngine;

[Serializable]
public class TutorCompleteDialog : Dialog
{
	public float msgY;

	private Vector2 sizeOk = new Vector2(130f, 34f);

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.TUTOR_COMPLETE;
	}

	public override void OnPopup()
	{
		size.x = GlobalVars.Instance.ScreenRect.width;
		rc = new Rect(0f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog()
	{
	}

	public override void OnClose(DialogManager.DIALOG_INDEX popup)
	{
		if (MyInfoManager.Instance.Tutorialed >= 2)
		{
			Application.LoadLevel("BfStart");
		}
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		if (GlobalVars.Instance.isLoadBattleTutor)
		{
			GUI.Label(new Rect(0f, 10f, size.x, size.y - sizeOk.y - 20f), StringMgr.Instance.Get("TUTORIAL_COMPLETE"), "MsgLabel");
		}
		else
		{
			GUI.Label(new Rect(0f, 10f, size.x, size.y - sizeOk.y - 20f), StringMgr.Instance.Get("TUTORIAL_COMPLETE2"), "MsgLabel");
		}
		if (GlobalVars.Instance.MyButton(new Rect((GlobalVars.Instance.ScreenRect.width - sizeOk.x) / 2f, size.y - sizeOk.y - 10f, sizeOk.x, sizeOk.y), StringMgr.Instance.Get("OK"), "BtnAction"))
		{
			if (MyInfoManager.Instance.Tutorialed < 2)
			{
				DialogManager.Instance.Push(DialogManager.DIALOG_INDEX.TUTOR_Q_POPUP, string.Empty);
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
