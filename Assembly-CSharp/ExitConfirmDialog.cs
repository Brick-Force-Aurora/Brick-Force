using System;
using UnityEngine;

[Serializable]
public class ExitConfirmDialog : Dialog
{
	private string text;

	public float msgY = 50f;

	private Vector2 sizeOk = new Vector2(100f, 34f);

	private int Line;

	private bool IsLong;

	private bool softExit;

	private bool closeButtonHide;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.EXIT_CONFIRM;
	}

	public override void OnPopup()
	{
		size.x = GlobalVars.Instance.ScreenRect.width;
		rc = new Rect(0f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		if (text.Length <= 0)
		{
			LabelUtil.TextOut(new Vector2(GlobalVars.Instance.ScreenRect.width / 2f, msgY), StringMgr.Instance.Get("ARE_YOU_SURE_EXIT"), "BigLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
		else
		{
			Vector2 vector = LabelUtil.CalcSize("BigLabel", text, size.x);
			Line = (int)(vector.y / 22f);
			if (Line >= 3)
			{
				IsLong = true;
			}
			if (!IsLong)
			{
				LabelUtil.TextOut(new Vector2(GlobalVars.Instance.ScreenRect.width / 2f, msgY - 15f), text, "BigLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(GlobalVars.Instance.ScreenRect.width / 2f, msgY + 10f), StringMgr.Instance.Get("ARE_YOU_SURE_EXIT"), "BigLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
			else
			{
				LabelUtil.TextOut(new Vector2(GlobalVars.Instance.ScreenRect.width / 2f, msgY), text, "BigLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
		}
		if (GlobalVars.Instance.MyButton(new Rect(GlobalVars.Instance.ScreenRect.width / 2f - 50f, size.y - sizeOk.y - 25f, sizeOk.x, sizeOk.y), StringMgr.Instance.Get("OK"), "BtnAction") || GlobalVars.Instance.IsReturnPressed())
		{
			result = true;
			if (softExit)
			{
				BuildOption.Instance.Exit();
			}
			else
			{
				BuildOption.Instance.HardExit();
			}
		}
		Rect rc = new Rect(size.x - 44f, 5f, 34f, 34f);
		if ((!closeButtonHide && GlobalVars.Instance.MyButton(rc, string.Empty, "BtnClose")) || GlobalVars.Instance.IsEscapePressed())
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

	public void InitDialog(string textMore)
	{
		text = textMore;
		softExit = false;
		closeButtonHide = false;
	}

	public void InitDialog()
	{
		text = string.Empty;
		softExit = true;
		closeButtonHide = false;
	}

	public void CloseButtonHide(bool isHide)
	{
		closeButtonHide = isHide;
	}
}
