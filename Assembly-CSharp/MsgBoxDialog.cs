using System;
using UnityEngine;

[Serializable]
public class MsgBoxDialog : Dialog
{
	public float msgY = 50f;

	private Vector2 sizeOk = new Vector2(130f, 34f);

	private Vector2 sizeArrow = new Vector2(42f, 62f);

	private bool returnOrEscapePressed;

	private MsgBox msgBox;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.MSG_BOX;
	}

	private void RecalcSize()
	{
		size.x = GlobalVars.Instance.ScreenRect.width;
		rc = new Rect(0f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUI.depth = 0;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		Color color = GUI.color;
		RecalcSize();
		GUI.color = msgBox.MsgColor;
		GUI.color = color;
		GUI.Label(new Rect(0f, 10f, size.x, size.y - sizeOk.y - 20f), msgBox.Message, "MsgLabel");
		if (MessageBoxMgr.Instance.HasPrevItem(msgBox) && GlobalVars.Instance.MyButton(new Rect(0f, 44f, sizeArrow.x, sizeArrow.y), string.Empty, "LeftArrowBig"))
		{
			MsgBox prevItem = MessageBoxMgr.Instance.GetPrevItem(msgBox);
			if (prevItem != null)
			{
				msgBox = prevItem;
			}
		}
		if (MessageBoxMgr.Instance.HasNextItem(msgBox) && GlobalVars.Instance.MyButton(new Rect(GlobalVars.Instance.ScreenRect.width - sizeArrow.x, 44f, sizeArrow.x, sizeArrow.y), string.Empty, "RightArrowBig"))
		{
			MsgBox nextItem = MessageBoxMgr.Instance.GetNextItem(msgBox);
			if (nextItem != null)
			{
				msgBox = nextItem;
			}
		}
		if (msgBox.Type == MsgBox.TYPE.WARNING)
		{
			if (GlobalVars.Instance.MyButton(new Rect((GlobalVars.Instance.ScreenRect.width - sizeOk.x) / 2f, size.y - sizeOk.y - 10f, sizeOk.x, sizeOk.y), StringMgr.Instance.Get("OK"), "BtnAction") || GlobalVars.Instance.IsReturnPressed())
			{
				result = true;
			}
		}
		else if (msgBox.Type == MsgBox.TYPE.SELECT)
		{
			if (GlobalVars.Instance.MyButton(new Rect((GlobalVars.Instance.ScreenRect.width - sizeOk.x - 130f) / 2f, size.y - sizeOk.y - 10f, sizeOk.x, sizeOk.y), StringMgr.Instance.Get("OK"), "BtnAction") || GlobalVars.Instance.IsReturnPressed())
			{
				MyInfoManager.Instance.MsgBoxConfirm = true;
				result = true;
			}
			if (GlobalVars.Instance.MyButton(new Rect((GlobalVars.Instance.ScreenRect.width - sizeOk.x + 130f) / 2f, size.y - sizeOk.y - 10f, sizeOk.x, sizeOk.y), StringMgr.Instance.Get("CANCEL"), "BtnAction") || GlobalVars.Instance.IsEscapePressed())
			{
				MyInfoManager.Instance.MsgBoxConfirm = false;
				result = true;
			}
		}
		else if (msgBox.Type == MsgBox.TYPE.FORCE_POINT_CHARGE)
		{
			if (GlobalVars.Instance.MyButton(new Rect((GlobalVars.Instance.ScreenRect.width - sizeOk.x - 130f) / 2f, size.y - sizeOk.y - 10f, sizeOk.x, sizeOk.y), StringMgr.Instance.Get("OK"), "BtnAction") || GlobalVars.Instance.IsReturnPressed())
			{
				result = true;
			}
			if (BuildOption.Instance.Props.canForcePointBuy && (GlobalVars.Instance.MyButton(new Rect((GlobalVars.Instance.ScreenRect.width - sizeOk.x + 130f) / 2f, size.y - sizeOk.y - 10f, sizeOk.x, sizeOk.y), StringMgr.Instance.Get("FORCE_POINT_CHARGE_BUTTON"), "BtnAction") || GlobalVars.Instance.IsEscapePressed()))
			{
				DialogManager.Instance.CloseAll();
				MessageBoxMgr.Instance.openForcePointChargeDlg = true;
				result = true;
			}
		}
		else if (msgBox.Type == MsgBox.TYPE.QUIT && (GlobalVars.Instance.MyButton(new Rect((GlobalVars.Instance.ScreenRect.width - sizeOk.x) / 2f, size.y - sizeOk.y - 10f, sizeOk.x, sizeOk.y), StringMgr.Instance.Get("OK"), "BtnAction") || GlobalVars.Instance.IsReturnPressed()))
		{
			BuildOption.Instance.Exit();
			result = true;
		}
		if (returnOrEscapePressed)
		{
			returnOrEscapePressed = false;
			result = true;
		}
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		return result;
	}

	public override void OnPopup()
	{
		returnOrEscapePressed = false;
	}

	public override void OnClose(DialogManager.DIALOG_INDEX popup)
	{
		MessageBoxMgr.Instance.Clear();
	}

	public void InitDialog(MsgBox mb)
	{
		msgBox = mb;
	}

	public override void Update()
	{
		if (Input.GetKeyDown(KeyCode.Return) || GlobalVars.Instance.IsEscapePressed())
		{
			returnOrEscapePressed = true;
		}
	}
}
