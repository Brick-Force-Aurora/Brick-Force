using System;
using UnityEngine;

[Serializable]
public class RoomPswdDialog : Dialog
{
	private int roomNo;

	private string roomPswd = string.Empty;

	public int maxRoomPswd = 4;

	public Rect crdPswd = new Rect(27f, 55f, 115f, 20f);

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.ROOM_PSWD;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog(int no)
	{
		roomNo = no;
		roomPswd = string.Empty;
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		Vector2 pos = new Vector2(size.x / 2f, 10f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("PASSWORD"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		string text = roomPswd;
		GUI.SetNextControlName("PasswordTextField");
		roomPswd = GUI.PasswordField(crdPswd, roomPswd, '*');
		if (roomPswd.Length > maxRoomPswd)
		{
			roomPswd = text;
		}
		if (GlobalVars.Instance.MyButton(new Rect(25f, 87f, 122f, 34f), StringMgr.Instance.Get("OK"), "BtnAction"))
		{
			roomPswd.Trim();
			if (roomPswd.Length > 0)
			{
				if (CSNetManager.Instance.Sock.SendCS_JOIN_REQ(roomNo, roomPswd, invite: false))
				{
				}
				result = true;
			}
			else
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("INPUT_ROOM_PSWD"));
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
			GUI.FocusControl("PasswordTextField");
		}
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}
}
