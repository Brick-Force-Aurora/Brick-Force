using System;
using UnityEngine;

[Serializable]
public class MapeditAuthorityDialog : Dialog
{
	public class DevInfo
	{
		public int lv;

		public int rank;

		public string nick;
	}

	public Texture2D LockedRoom;

	public Texture2D UnlockedRoom;

	public Texture2D roomMaster;

	public DevInfo devInfo = new DevInfo();

	private Vector2 crdChannelText = new Vector2(10f, 20f);

	private Vector2 crdRoomText = new Vector2(70f, 20f);

	private Vector2 crdCountText = new Vector2(334f, 20f);

	private Rect crdKey = new Rect(348f, 16f, 18f, 9f);

	private Rect crdPswdFld = new Rect(370f, 9f, 55f, 25f);

	private Rect crdButtonPopA = new Rect(15f, 50f, 400f, 39f);

	private Rect crdRoomMaster = new Rect(30f, 60f, 20f, 20f);

	private Rect crdBadgeMark = new Rect(170f, 60f, 34f, 17f);

	private Vector2 crdMasterText = new Vector2(310f, 68f);

	private Rect crdBoxGray = new Rect(15f, 98f, 402f, 28f);

	private Vector2 crdClanText = new Vector2(40f, 110f);

	private Vector2 crdBadgeText = new Vector2(80f, 110f);

	private Vector2 crdNickText = new Vector2(180f, 110f);

	private Vector2 crdAuthoText = new Vector2(290f, 110f);

	private Vector2 crdBanText = new Vector2(373f, 110f);

	private Rect crdBoxPopline = new Rect(15f, 135f, 402f, 271f);

	private Rect crdBadgeMarkList = new Rect(64f, 144f, 34f, 17f);

	private Vector2 crdMasterTextList = new Vector2(180f, 154f);

	private Rect crdToggleList = new Rect(280f, 142f, 21f, 22f);

	private Rect crdBanBtnList = new Rect(330f, 136f, 82f, 34f);

	private int excileID = -1;

	private int maxRoomPswd = 4;

	private string roomPswd = string.Empty;

	private bool returnPressed;

	private bool bShowMessage;

	private float deltaTimeMsg;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.MAPEDIT_AUTHORITY;
	}

	public override void OnPopup()
	{
		size.x = 432f;
		size.y = 419f;
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
		if (RoomManager.Instance.GetCurrentRoomInfo().Locked)
		{
			roomPswd = "****";
		}
		else
		{
			roomPswd = string.Empty;
		}
	}

	public void InitDialog()
	{
	}

	private bool IsDeveloper()
	{
		if (UserMapInfoManager.Instance.master == MyInfoManager.Instance.Seq)
		{
			devInfo.lv = XpManager.Instance.GetLevel(MyInfoManager.Instance.Xp);
			devInfo.rank = MyInfoManager.Instance.Rank;
			devInfo.nick = MyInfoManager.Instance.Nickname;
			return true;
		}
		BrickManDesc[] array = BrickManManager.Instance.ToDescriptorArray();
		foreach (BrickManDesc brickManDesc in array)
		{
			if (UserMapInfoManager.Instance.master == brickManDesc.Seq)
			{
				devInfo.lv = XpManager.Instance.GetLevel(brickManDesc.Xp);
				devInfo.rank = brickManDesc.Rank;
				devInfo.nick = brickManDesc.Nickname;
				return true;
			}
		}
		return false;
	}

	private bool CheckAuth()
	{
		if (UserMapInfoManager.Instance.master == MyInfoManager.Instance.Seq)
		{
			return true;
		}
		MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NO_MAPEDIT_AUTH"));
		return false;
	}

	public override bool DoDialog()
	{
		if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter))
		{
			returnPressed = true;
		}
		bool result = false;
		deltaTimeMsg += Time.deltaTime;
		if (bShowMessage && deltaTimeMsg > 1.2f)
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CHANGE_PASSWORD"));
			bShowMessage = false;
		}
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		BrickManDesc[] array = BrickManManager.Instance.ToDescriptorArray();
		LabelUtil.TextOut(crdChannelText, ChannelManager.Instance.CurChannel.Name, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		Room room = RoomManager.Instance.GetRoom(RoomManager.Instance.CurrentRoom);
		if (room != null)
		{
			LabelUtil.TextOut(crdRoomText, room.GetString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		}
		int num = array.Length + 1;
		int maxPlayer = RoomManager.Instance.GetCurrentRoomInfo().MaxPlayer;
		LabelUtil.TextOut(crdCountText, string.Empty + num + "/" + maxPlayer, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
		TextureUtil.DrawTexture(crdKey, LockedRoom);
		string text = roomPswd;
		if (UserMapInfoManager.Instance.master != MyInfoManager.Instance.Seq)
		{
			GUI.enabled = false;
		}
		roomPswd = GUI.PasswordField(crdPswdFld, roomPswd, "*"[0]);
		if (roomPswd.Length > maxRoomPswd)
		{
			roomPswd = text;
		}
		if (UserMapInfoManager.Instance.master != MyInfoManager.Instance.Seq)
		{
			GUI.enabled = true;
		}
		if (returnPressed && UserMapInfoManager.Instance.master == MyInfoManager.Instance.Seq)
		{
			returnPressed = false;
			bShowMessage = true;
			deltaTimeMsg = 0f;
			CSNetManager.Instance.Sock.SendCS_ROOM_CONFIG_REQ(0, 0, 0, 0, 0, 0, 0, 0, UserMapInfoManager.Instance.GetCur().Alias, roomPswd, 0);
		}
		GUI.Box(crdButtonPopA, string.Empty, "BoxPopline");
		if (IsDeveloper())
		{
			TextureUtil.DrawTexture(crdRoomMaster, roomMaster);
			TextureUtil.DrawTexture(crdBadgeMark, XpManager.Instance.GetBadge(devInfo.lv, devInfo.rank));
			LabelUtil.TextOut(crdMasterText, devInfo.nick, "MiniLabel", new Color(0.87f, 0.63f, 0.32f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
		GUI.Box(crdBoxGray, string.Empty, "BoxFadeBlue");
		LabelUtil.TextOut(crdClanText, StringMgr.Instance.Get("CLAN"), "MiniLabel", new Color(0.87f, 0.63f, 0.32f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(crdBadgeText, StringMgr.Instance.Get("BADGE"), "MiniLabel", new Color(0.87f, 0.63f, 0.32f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(crdNickText, StringMgr.Instance.Get("NICKNAME"), "MiniLabel", new Color(0.87f, 0.63f, 0.32f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(crdAuthoText, StringMgr.Instance.Get("AUTH"), "MiniLabel", new Color(0.87f, 0.63f, 0.32f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(crdBanText, StringMgr.Instance.Get("EXILE"), "MiniLabel", new Color(0.87f, 0.63f, 0.32f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		GUI.Box(crdBoxPopline, string.Empty, "BoxPopline");
		int num2 = 33;
		int num3 = 0;
		if (UserMapInfoManager.Instance.master != MyInfoManager.Instance.Seq)
		{
			Rect position = new Rect(crdBadgeMarkList);
			int level = XpManager.Instance.GetLevel(MyInfoManager.Instance.Xp);
			TextureUtil.DrawTexture(position, XpManager.Instance.GetBadge(level, MyInfoManager.Instance.Rank));
			Vector2 pos = new Vector2(crdMasterTextList.x, crdMasterTextList.y);
			LabelUtil.TextOut(pos, MyInfoManager.Instance.Nickname, "MiniLabel", new Color(0.87f, 0.63f, 0.32f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			GUI.enabled = false;
			Rect position2 = new Rect(crdToggleList);
			bool isEditor = MyInfoManager.Instance.IsEditor;
			GUI.Toggle(position2, isEditor, string.Empty);
			Rect rc = new Rect(crdBanBtnList);
			GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("EXILE"), "BtnAction");
			GUI.enabled = true;
			num3 = 33;
		}
		int num4 = 0;
		foreach (BrickManDesc brickManDesc in array)
		{
			if (brickManDesc != null && UserMapInfoManager.Instance.master != brickManDesc.Seq)
			{
				int num5 = num2 * num4 + num3;
				Rect position3 = new Rect(crdBadgeMarkList);
				position3.y += (float)num5;
				int level2 = XpManager.Instance.GetLevel(brickManDesc.Xp);
				TextureUtil.DrawTexture(position3, XpManager.Instance.GetBadge(level2, brickManDesc.Rank));
				Vector2 pos2 = new Vector2(crdMasterTextList.x, crdMasterTextList.y);
				pos2.y += (float)num5;
				LabelUtil.TextOut(pos2, brickManDesc.Nickname, "MiniLabel", new Color(0.87f, 0.63f, 0.32f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				if (UserMapInfoManager.Instance.master != MyInfoManager.Instance.Seq)
				{
					GUI.enabled = false;
				}
				Rect position4 = new Rect(crdToggleList);
				position4.y += (float)num5;
				bool isEditor2 = brickManDesc.IsEditor;
				bool flag = GUI.Toggle(position4, isEditor2, string.Empty);
				if (isEditor2 != flag && CheckAuth())
				{
					CSNetManager.Instance.Sock.SendCS_ME_CHG_EDITOR_REQ(brickManDesc.Seq, flag);
				}
				Rect rc2 = new Rect(crdBanBtnList);
				rc2.y += (float)num5;
				if (GlobalVars.Instance.MyButton(rc2, StringMgr.Instance.Get("EXILE"), "BtnAction") && CheckAuth())
				{
					excileID = brickManDesc.Seq;
					string msg = string.Format(StringMgr.Instance.Get("PLAYER_EXILE"), brickManDesc.Nickname);
					MessageBoxMgr.Instance.AddSelectMessage(msg);
				}
				if (UserMapInfoManager.Instance.master != MyInfoManager.Instance.Seq)
				{
					GUI.enabled = true;
				}
				num4++;
			}
		}
		if (MyInfoManager.Instance.MsgBoxConfirm)
		{
			CSNetManager.Instance.Sock.SendCS_KICK_REQ(excileID);
			MyInfoManager.Instance.MsgBoxConfirm = false;
			excileID = -1;
		}
		GUI.skin = skin;
		return result;
	}
}
