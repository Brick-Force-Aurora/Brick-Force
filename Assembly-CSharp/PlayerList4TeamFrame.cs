using System;
using UnityEngine;

[Serializable]
public class PlayerList4TeamFrame
{
	public Texture2D tinyMasterIcon;

	public Texture2D weaponAll;

	public Texture2D weaponMelee;

	public Texture2D weaponPistol;

	public Texture2D locked;

	public Vector2 lockedSize = new Vector2(17f, 24f);

	private Vector2 crdRoomTitle = new Vector2(620f, 76f);

	private Vector2 crdPlayerSize = new Vector2(178f, 93f);

	private Vector2 crdPlayerLTBlue = new Vector2(258f, 94f);

	private Vector2 crdPlayerLTRed = new Vector2(650f, 94f);

	private Vector2 offset = new Vector2(3f, 5f);

	private Rect crdVLine = new Rect(633f, 150f, 1f, 344f);

	private Vector2 crdStatus = new Vector2(173f, 35f);

	private Rect crdX = new Rect(112f, -12f, 14f, 14f);

	private Vector2 crdBadge = new Vector2(136f, 5f);

	private Vector2 crdNickname = new Vector2(90f, 78f);

	private Rect crdPortrait = new Rect(1f, 1f, 58f, 58f);

	private Rect crdTinyMasterIcon = new Rect(1f, 1f, 12f, 12f);

	private Rect crdClanMark = new Rect(107f, 2f, 22f, 22f);

	private Vector2 crdClanName = new Vector2(173f, 53f);

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
	}

	public void ResetMyPlayerStyle()
	{
		BrickManManager.Instance.Remove(MyInfoManager.Instance.Seq);
		myDesc = null;
		myDesc = new BrickManDesc(MyInfoManager.Instance.Seq, MyInfoManager.Instance.Nickname, MyInfoManager.Instance.GetUsings(), MyInfoManager.Instance.Status, MyInfoManager.Instance.Xp, MyInfoManager.Instance.ClanSeq, MyInfoManager.Instance.ClanName, MyInfoManager.Instance.ClanMark, MyInfoManager.Instance.Rank, null, null);
		BrickManManager.Instance.AddBrickMan(myDesc);
	}

	public void Close()
	{
		BrickManManager.Instance.Remove(MyInfoManager.Instance.Seq);
	}

	public void OnGUI()
	{
		txtMainColor = GlobalVars.Instance.GetByteColor2FloatColor(244, 151, 25);
		GUI.Box(crdVLine, string.Empty, "DivideLineV");
		Room room = RoomManager.Instance.GetRoom(RoomManager.Instance.CurrentRoom);
		if (room != null)
		{
			LabelUtil.TextOut(crdRoomTitle, room.GetString(), "BigLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
		BrickManDesc[] array = BrickManManager.Instance.ToDescriptorArrayBySlot();
		for (int i = 0; i < 16; i++)
		{
			int num = i;
			bool flag = i < 8;
			bool flag2 = false;
			if (MyInfoManager.Instance.Slot == num)
			{
				flag2 = true;
			}
			if (num >= 0)
			{
				int num2 = 8;
				if (flag)
				{
					num2 = 0;
				}
				int num3 = num % 2;
				int num4 = (num - num2) / 2;
				Rect rc = (!flag) ? new Rect(crdPlayerLTBlue.x + (crdPlayerSize.x + offset.x) * (float)num3, crdPlayerLTBlue.y + (crdPlayerSize.y + offset.y) * (float)num4, crdPlayerSize.x, crdPlayerSize.y) : new Rect(crdPlayerLTRed.x + (crdPlayerSize.x + offset.x) * (float)num3, crdPlayerLTRed.y + (crdPlayerSize.y + offset.y) * (float)num4, crdPlayerSize.x, crdPlayerSize.y);
				if (array[i] != null || flag2)
				{
					aPlayer(flag, rc, (!flag2) ? array[i] : myDesc);
				}
				else if (RoomManager.Instance.SlotStatus[i])
				{
					if (GlobalVars.Instance.MyButton(rc, string.Empty, (!flag) ? "ButtonBlue" : "ButtonRed"))
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
				else if (GlobalVars.Instance.MyButton(rc, string.Empty, "ButtonLock") && RoomManager.Instance.Master == MyInfoManager.Instance.Seq && ChannelManager.Instance.CurChannel.Mode != 4)
				{
					CSNetManager.Instance.Sock.SendCS_SLOT_LOCK_REQ((sbyte)i, 0);
				}
			}
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

	private void aPlayer(bool isRed, Rect rc, BrickManDesc desc)
	{
		bool flag = false;
		if (RoomManager.Instance.Master == MyInfoManager.Instance.Seq)
		{
			flag = true;
		}
		GUI.Box(rc, string.Empty, (!isRed) ? "ButtonBlue" : "ButtonRed");
		Vector2 pos = new Vector2(rc.x + crdStatus.x, rc.y + crdStatus.y);
		bool flag2 = (MyInfoManager.Instance.Seq == desc.Seq) ? true : false;
		if (flag2)
		{
			desc.Status = MyInfoManager.Instance.Status;
			GUI.Box(rc, string.Empty, "BtnSelectF");
		}
		switch (desc.Status)
		{
		case 2:
		case 4:
			LabelUtil.TextOut(pos, StringMgr.Instance.Get("PLAYING"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			break;
		case 5:
			LabelUtil.TextOut(pos, StringMgr.Instance.Get("SHOPPING"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			break;
		case 6:
			LabelUtil.TextOut(pos, StringMgr.Instance.Get("EQUIPPING"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			break;
		case 1:
			if (RoomManager.Instance.Master != desc.Seq)
			{
				LabelUtil.TextOut(pos, StringMgr.Instance.Get("READY"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			}
			break;
		}
		if (flag && !flag2 && GlobalVars.Instance.MyButton(new Rect(rc.x, rc.y + crdX.y, crdX.width, crdX.height), string.Empty, "X"))
		{
			CSNetManager.Instance.Sock.SendCS_KICK_REQ(desc.Seq);
		}
		if (GlobalVars.Instance.MyButton(rc, string.Empty, "InvisibleButton") && Event.current.button == 1 && desc.Seq >= 0 && desc.Seq != MyInfoManager.Instance.Seq)
		{
			UserMenu userMenu = ContextMenuManager.Instance.Popup();
			if (userMenu != null)
			{
				bool flag3 = MyInfoManager.Instance.IsClanee(desc.Seq) || desc.Clan >= 0;
				userMenu.InitDialog(MouseUtil.ScreenToPixelPoint(Input.mousePosition), desc.Seq, desc.Nickname, !flag3, flag);
			}
		}
		DrawClanMark(new Rect(rc.x + crdClanMark.x, rc.y + crdClanMark.y, crdClanMark.width, crdClanMark.height), desc.ClanMark);
		int level = XpManager.Instance.GetLevel(desc.Xp);
		Texture2D badge = XpManager.Instance.GetBadge(level, desc.Rank);
		if (null != badge)
		{
			TextureUtil.DrawTexture(new Rect(rc.x + crdBadge.x, rc.y + crdBadge.y, (float)badge.width, (float)badge.height), badge);
		}
		LabelUtil.TextOut(new Vector2(rc.x + crdNickname.x, rc.y + crdNickname.y), desc.Nickname, "Label", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(rc.x + crdClanName.x, rc.y + crdClanName.y), desc.ClanName, "MiniLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
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
				TextureUtil.DrawTexture(position, camera.targetTexture, ScaleMode.StretchToFill, alphaBlend: true);
			}
		}
		if (RoomManager.Instance.Master == desc.Seq)
		{
			TextureUtil.DrawTexture(new Rect(rc.x + crdTinyMasterIcon.x, rc.y + crdTinyMasterIcon.y, crdTinyMasterIcon.width, crdTinyMasterIcon.height), tinyMasterIcon, ScaleMode.StretchToFill);
		}
	}

	private int ToLocalIndex(int slot)
	{
		if (MyInfoManager.Instance.Slot == slot)
		{
			return -1;
		}
		if (MyInfoManager.Instance.Slot < slot)
		{
			slot--;
		}
		int[] array = new int[15]
		{
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			9,
			10,
			12,
			13,
			14,
			15,
			16,
			17
		};
		int[] array2 = new int[15]
		{
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			7,
			9,
			12,
			13,
			14,
			15,
			16,
			17
		};
		if (MyInfoManager.Instance.Slot < 8)
		{
			return array[slot];
		}
		return array2[slot];
	}
}
