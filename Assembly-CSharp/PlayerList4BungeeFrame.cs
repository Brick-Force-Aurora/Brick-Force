using System;
using UnityEngine;

[Serializable]
public class PlayerList4BungeeFrame
{
	public Texture2D tinyMasterIcon;

	public Texture2D locked;

	public Vector2 lockedSize = new Vector2(17f, 24f);

	private Vector2 crdRoomTitle = new Vector2(620f, 76f);

	private Vector2 crdPlayerSize = new Vector2(324f, 93f);

	private Vector2 crdPlayerLTBlue = new Vector2(280f, 94f);

	private Vector2 crdPlayerLTRed = new Vector2(660f, 94f);

	private Vector2 offset = new Vector2(3f, 5f);

	private Vector2 crdStatus = new Vector2(175f, 12f);

	private Rect crdX = new Rect(310f, -12f, 14f, 14f);

	private Vector2 crdBadge = new Vector2(284f, 5f);

	private Vector2 crdNickname = new Vector2(175f, 78f);

	private Rect crdPortrait = new Rect(2f, 2f, 88f, 88f);

	private Vector2 crdFace = new Vector2(90f, 90f);

	private Rect crdTinyMasterIcon = new Rect(1f, 1f, 12f, 12f);

	private Rect crdClanMark = new Rect(257f, 2f, 22f, 22f);

	private Vector2 crdClanName = new Vector2(175f, 53f);

	private Color txtMainColor;

	private BrickManDesc myDesc;

	public void Start()
	{
		myDesc = new BrickManDesc(MyInfoManager.Instance.Seq, MyInfoManager.Instance.Nickname, MyInfoManager.Instance.GetUsings(), MyInfoManager.Instance.Status, MyInfoManager.Instance.Xp, MyInfoManager.Instance.ClanSeq, MyInfoManager.Instance.ClanName, MyInfoManager.Instance.ClanMark, MyInfoManager.Instance.Rank, null, null);
		GameObject x = BrickManManager.Instance.Get(MyInfoManager.Instance.Seq);
		if (x == null)
		{
			BrickManManager.Instance.AddBrickMan(myDesc);
		}
		txtMainColor = GlobalVars.Instance.GetByteColor2FloatColor(244, 151, 25);
	}

	public void Close()
	{
		BrickManManager.Instance.Remove(MyInfoManager.Instance.Seq);
	}

	public void ResetMyPlayerStyle()
	{
		BrickManManager.Instance.Remove(MyInfoManager.Instance.Seq);
		myDesc = null;
		myDesc = new BrickManDesc(MyInfoManager.Instance.Seq, MyInfoManager.Instance.Nickname, MyInfoManager.Instance.GetUsings(), MyInfoManager.Instance.Status, MyInfoManager.Instance.Xp, MyInfoManager.Instance.ClanSeq, MyInfoManager.Instance.ClanName, MyInfoManager.Instance.ClanMark, MyInfoManager.Instance.Rank, null, null);
		BrickManManager.Instance.AddBrickMan(myDesc);
	}

	public void OnGUI()
	{
		Room room = RoomManager.Instance.GetRoom(RoomManager.Instance.CurrentRoom);
		if (room != null)
		{
			LabelUtil.TextOut(crdRoomTitle, room.GetString(), "BigLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
		BrickManDesc[] array = BrickManManager.Instance.ToDescriptorArrayBySlot();
		for (int i = 0; i < 8; i++)
		{
			int num = i;
			bool flag = i < 4;
			bool flag2 = false;
			if (MyInfoManager.Instance.Slot == num)
			{
				flag2 = true;
			}
			if (num >= 0)
			{
				int num2 = num % 4;
				Rect rc = (!flag) ? new Rect(crdPlayerLTBlue.x, crdPlayerLTBlue.y + (crdPlayerSize.y + offset.y) * (float)num2, crdPlayerSize.x, crdPlayerSize.y) : new Rect(crdPlayerLTRed.x, crdPlayerLTRed.y + (crdPlayerSize.y + offset.y) * (float)num2, crdPlayerSize.x, crdPlayerSize.y);
				if (array[i] != null || flag2)
				{
					aPlayer(rc, (!flag2) ? array[i] : myDesc, flag);
				}
				else if (RoomManager.Instance.SlotStatus[i])
				{
					if (GlobalVars.Instance.MyButton(rc, string.Empty, "BtnGreen"))
					{
						if (RoomManager.Instance.Master == MyInfoManager.Instance.Seq && Event.current.button == 1)
						{
							CSNetManager.Instance.Sock.SendCS_SLOT_LOCK_REQ((sbyte)i, 1);
						}
						else if (MyInfoManager.Instance.Seq != RoomManager.Instance.Master && MyInfoManager.Instance.Status == 1)
						{
							MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CANT_ON_READY"));
						}
						else
						{
							CSNetManager.Instance.Sock.SendCS_TEAM_CHANGE_REQ(clickSlot: true, i);
						}
					}
				}
				else
				{
					if (GlobalVars.Instance.MyButton(rc, string.Empty, "ButtonLockIcon") && RoomManager.Instance.Master == MyInfoManager.Instance.Seq)
					{
						CSNetManager.Instance.Sock.SendCS_SLOT_LOCK_REQ((sbyte)i, 0);
					}
					float num3 = 46.2f;
					Rect position = new Rect(rc.x + rc.width / 2f - num3 * 0.5f, rc.y + rc.height / 2f - num3 * 0.5f, num3, num3);
					GUI.Box(position, string.Empty, "LockIcon");
				}
			}
		}
	}

	private void aPlayer(Rect rc, BrickManDesc desc, bool isRed)
	{
		bool flag = false;
		if (RoomManager.Instance.Master == MyInfoManager.Instance.Seq)
		{
			flag = true;
		}
		bool flag2 = (MyInfoManager.Instance.Seq == desc.Seq) ? true : false;
		if (flag2)
		{
			desc.Status = MyInfoManager.Instance.Status;
		}
		if (flag && !flag2 && GlobalVars.Instance.MyButton(new Rect(rc.x + crdX.x, rc.y + crdX.y, crdX.width, crdX.height), string.Empty, "X"))
		{
			CSNetManager.Instance.Sock.SendCS_KICK_REQ(desc.Seq);
		}
		if (GlobalVars.Instance.MyButton(rc, string.Empty, "BtnGreen") && Event.current.button == 1 && desc.Seq >= 0 && desc.Seq != MyInfoManager.Instance.Seq)
		{
			UserMenu userMenu = ContextMenuManager.Instance.Popup();
			if (userMenu != null)
			{
				bool flag3 = MyInfoManager.Instance.IsClanee(desc.Seq) || desc.Clan >= 0;
				userMenu.InitDialog(MouseUtil.ScreenToPixelPoint(Input.mousePosition), desc.Seq, desc.Nickname, !flag3, flag);
			}
		}
		if (flag2)
		{
			GUI.Box(rc, string.Empty, "BtnSelectF");
		}
		Vector2 pos = new Vector2(rc.x + crdStatus.x, rc.y + crdStatus.y);
		switch (desc.Status)
		{
		case 2:
		case 4:
			LabelUtil.TextOut(pos, StringMgr.Instance.Get("PLAYING"), "Label", Color.grey, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			break;
		case 5:
			LabelUtil.TextOut(pos, StringMgr.Instance.Get("SHOPPING"), "Label", Color.grey, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			break;
		case 6:
			LabelUtil.TextOut(pos, StringMgr.Instance.Get("EQUIPPING"), "Label", Color.grey, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			break;
		case 1:
			if (RoomManager.Instance.Master != desc.Seq)
			{
				LabelUtil.TextOut(pos, StringMgr.Instance.Get("READY"), "Label", Color.grey, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
			break;
		}
		DrawClanMark(new Rect(rc.x + crdClanMark.x, rc.y + crdClanMark.y, crdClanMark.width, crdClanMark.height), desc.ClanMark);
		int level = XpManager.Instance.GetLevel(desc.Xp);
		Texture2D badge = XpManager.Instance.GetBadge(level, desc.Rank);
		if (null != badge)
		{
			TextureUtil.DrawTexture(new Rect(rc.x + crdBadge.x, rc.y + crdBadge.y, (float)badge.width, (float)badge.height), badge);
		}
		LabelUtil.TextOut(new Vector2(rc.x + crdNickname.x, rc.y + crdNickname.y), desc.Nickname, "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(rc.x + crdClanName.x, rc.y + crdClanName.y), desc.ClanName, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		GameObject gameObject = BrickManManager.Instance.Get(desc.Seq);
		if (null != gameObject)
		{
			Rect position = new Rect(rc.x + crdPortrait.x, rc.y + crdPortrait.y, crdPortrait.width, crdPortrait.height);
			Camera camera = null;
			Camera[] componentsInChildren = gameObject.GetComponentsInChildren<Camera>();
			int num = 0;
			while (camera == null && num < componentsInChildren.Length)
			{
				if (componentsInChildren[num].enabled)
				{
					camera = componentsInChildren[num];
				}
				num++;
			}
			if (null != camera)
			{
				GUI.Box(new Rect(rc.x + crdPortrait.x, rc.y + crdPortrait.x, crdFace.x, crdFace.y), string.Empty, "BoxPlayerGreen");
				TextureUtil.DrawTexture(position, camera.targetTexture, ScaleMode.StretchToFill, alphaBlend: true);
			}
		}
		if (RoomManager.Instance.Master == desc.Seq)
		{
			TextureUtil.DrawTexture(new Rect(rc.x + crdTinyMasterIcon.x, rc.y + crdTinyMasterIcon.y, crdTinyMasterIcon.width, crdTinyMasterIcon.height), tinyMasterIcon, ScaleMode.StretchToFill);
		}
	}

	private void DrawClanMark(Rect rc, int mark)
	{
		if (mark >= 0)
		{
			Texture2D bg = ClanMarkManager.Instance.GetBg(mark);
			Color colorValue = ClanMarkManager.Instance.GetColorValue(mark);
			Texture2D amblum = ClanMarkManager.Instance.GetAmblum(mark);
			if (null != bg)
			{
				TextureUtil.DrawTexture(rc, bg);
			}
			Color color = GUI.color;
			GUI.color = colorValue;
			if (null != amblum)
			{
				TextureUtil.DrawTexture(rc, amblum);
			}
			GUI.color = color;
		}
	}
}
