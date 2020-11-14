using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomListFrame
{
	public Texture2D lockedRoom;

	public Rect crdFrame = new Rect(250f, 42f, 510f, 425f);

	private Rect crdQuickJoin = new Rect(256f, 48f, 165f, 24f);

	private Rect crdCreateRoom = new Rect(430f, 48f, 165f, 24f);

	private Rect crdRoomNo = new Rect(253f, 77f, 33f, 26f);

	private Rect crdRoomTitle = new Rect(414f, 77f, 186f, 26f);

	private Rect crdRoomType = new Rect(600f, 77f, 40f, 26f);

	private Rect crdRoomStatus = new Rect(640f, 77f, 57f, 26f);

	private Rect crdRoomNumPlayers = new Rect(697f, 77f, 47f, 26f);

	private Rect crdRoomMap = new Rect(286f, 77f, 128f, 26f);

	public Rect crdListTop = new Rect(740f, 42f, 50f, 26f);

	private Vector2 crdModeIconSize = new Vector2(27f, 16f);

	public Rect crdRoomList = new Rect(250f, 70f, 510f, 360f);

	public float height = 21f;

	public Vector2 lockSize = new Vector2(11f, 14f);

	public Vector2 crdRoomButton = new Vector2(490f, 26f);

	private Room.COLUMN sortedBy;

	private bool ascending = true;

	private Vector2 scrollPosition = Vector2.zero;

	private bool once;

	private StreamedLevelLoadibilityChecker sll;

	private Texture2D[] modeIcon;

	public void Start()
	{
		modeIcon = new Texture2D[10];
		modeIcon[0] = GlobalVars.Instance.iconMapMode;
		modeIcon[1] = GlobalVars.Instance.iconTeamMode;
		modeIcon[2] = GlobalVars.Instance.iconsurvivalMode;
		modeIcon[3] = GlobalVars.Instance.iconCTFMode;
		modeIcon[4] = GlobalVars.Instance.iconBlastMode;
		modeIcon[5] = GlobalVars.Instance.iconDefenseMode;
		modeIcon[6] = GlobalVars.Instance.iconBndMode;
		modeIcon[7] = GlobalVars.Instance.iconBungeeMode;
		modeIcon[8] = GlobalVars.Instance.iconEscapeMode;
		modeIcon[9] = GlobalVars.Instance.iconZombieMode;
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			sll = gameObject.GetComponent<StreamedLevelLoadibilityChecker>();
		}
	}

	public void Update()
	{
		if (!once)
		{
			once = true;
		}
	}

	public void OnGUI()
	{
		GUI.Box(crdFrame, string.Empty, "BoxWaiting01");
		GUIContent content = new GUIContent(StringMgr.Instance.Get("QUICK_JOIN").ToUpper(), GlobalVars.Instance.iconQuickjoin);
		if (GlobalVars.Instance.MyButton3(crdQuickJoin, content, "BtnAction"))
		{
			if (sll != null && !sll.CanStreamedLevelBeLoaded())
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("STREAMING_WAIT"));
			}
			else if (!ChannelManager.Instance.CurChannel.IsSmartQuickJoin)
			{
				CSNetManager.Instance.Sock.SendCS_QUICK_JOIN_REQ(-1, -1);
			}
			else
			{
				((QuickJoinDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.QUICKJOIN, exclusive: false))?.InitDialog();
			}
		}
		content = new GUIContent(StringMgr.Instance.Get("CREATE_ROOM").ToUpper(), GlobalVars.Instance.iconBlock);
		if (GlobalVars.Instance.MyButton3(crdCreateRoom, content, "ButtonSub"))
		{
			if (sll != null && !sll.CanStreamedLevelBeLoaded())
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("STREAMING_WAIT"));
			}
			else
			{
				((CreateRoomDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.CREATE_ROOM, exclusive: true))?.InitDialog();
			}
		}
		if (GlobalVars.Instance.MyButton(crdRoomNo, StringMgr.Instance.Get("ROOM_NO"), "ButtonColumn"))
		{
			if (sortedBy == Room.COLUMN.NO)
			{
				ascending = !ascending;
			}
			sortedBy = Room.COLUMN.NO;
		}
		if (GlobalVars.Instance.MyButton(crdRoomMap, StringMgr.Instance.Get("ROOM_MAP"), "ButtonColumn"))
		{
			if (sortedBy == Room.COLUMN.MAP_ALIAS)
			{
				ascending = !ascending;
			}
			sortedBy = Room.COLUMN.MAP_ALIAS;
		}
		if (GlobalVars.Instance.MyButton(crdRoomTitle, StringMgr.Instance.Get("ROOM_TITLE"), "ButtonColumn"))
		{
			if (sortedBy == Room.COLUMN.TITLE)
			{
				ascending = !ascending;
			}
			sortedBy = Room.COLUMN.TITLE;
		}
		if (GlobalVars.Instance.MyButton(crdRoomType, StringMgr.Instance.Get("ROOM_TYPE"), "ButtonColumn"))
		{
			if (sortedBy == Room.COLUMN.TYPE)
			{
				ascending = !ascending;
			}
			sortedBy = Room.COLUMN.TYPE;
		}
		if (GlobalVars.Instance.MyButton(crdRoomStatus, StringMgr.Instance.Get("ROOM_STATUS"), "ButtonColumn"))
		{
			if (sortedBy == Room.COLUMN.STATUS)
			{
				ascending = !ascending;
			}
			sortedBy = Room.COLUMN.STATUS;
		}
		if (GlobalVars.Instance.MyButton(crdRoomNumPlayers, StringMgr.Instance.Get("NUM_PLAYERS"), "ButtonColumn"))
		{
			if (sortedBy == Room.COLUMN.NUM_PLAYER)
			{
				ascending = !ascending;
			}
			sortedBy = Room.COLUMN.NUM_PLAYER;
		}
		if (GUI.Button(crdListTop, string.Empty, "ButtonColumn"))
		{
			scrollPosition = Vector2.zero;
		}
		List<KeyValuePair<int, Room>> list = RoomManager.Instance.ToSortedList(sortedBy, ascending, Room.ROOM_TYPE.NONE, Room.ROOM_STATUS.NONE);
		int num = -1;
		Vector2 zero = Vector2.zero;
		float width = crdRoomNo.width;
		float width2 = crdRoomTitle.width;
		float width3 = crdRoomType.width;
		float width4 = crdRoomStatus.width;
		float width5 = crdRoomNumPlayers.width;
		float width6 = crdRoomMap.width;
		scrollPosition = GUI.BeginScrollView(viewRect: new Rect(0f, 0f, width + width2 + width3 + width4 + width5, height * (float)list.Count), position: crdRoomList, scrollPosition: scrollPosition);
		float y = scrollPosition.y;
		float num2 = scrollPosition.y + crdRoomList.height;
		foreach (KeyValuePair<int, Room> item in list)
		{
			zero.x = 0f;
			float y2 = zero.y;
			float num3 = zero.y + height;
			if (num3 >= y && y2 <= num2)
			{
				if (item.Value.Locked)
				{
					TextureUtil.DrawTexture(new Rect(zero.x + 1f, zero.y + (height - lockSize.y) / 2f, lockSize.x, lockSize.y), lockedRoom, ScaleMode.StretchToFill);
				}
				if (GlobalVars.Instance.MyButton(new Rect(zero.x, zero.y, crdRoomButton.x, crdRoomButton.y), string.Empty, "RoomButton"))
				{
					num = item.Value.No;
				}
				LabelUtil.TextOut(new Vector2(zero.x + lockSize.x, zero.y), item.Value.GetString(Room.COLUMN.NO), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
				zero.x += width;
				LabelUtil.TextOut(new Vector2(zero.x + width6 / 2f, zero.y), item.Value.GetString(Room.COLUMN.MAP_ALIAS), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
				zero.x += width6;
				LabelUtil.TextOut(new Vector2(zero.x + width2 / 2f, zero.y), item.Value.GetString(Room.COLUMN.TITLE), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
				zero.x += width2;
				int type = (int)item.Value.Type;
				if (0 <= type && type < modeIcon.Length && modeIcon[type] != null)
				{
					TextureUtil.DrawTexture(new Rect(zero.x + (width3 - crdModeIconSize.x) / 2f, zero.y + 4f, crdModeIconSize.x, crdModeIconSize.y), modeIcon[type], ScaleMode.StretchToFill);
				}
				zero.x += width3;
				LabelUtil.TextOut(new Vector2(zero.x + width4 / 2f, zero.y + 1f), item.Value.GetString(Room.COLUMN.STATUS), "TinyLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
				zero.x += width4;
				LabelUtil.TextOut(new Vector2(zero.x + width5 / 2f, zero.y), item.Value.GetString(Room.COLUMN.NUM_PLAYER), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
			}
			zero.y += height;
		}
		GUI.EndScrollView();
		if (num >= 0)
		{
			if (sll != null && !sll.CanStreamedLevelBeLoaded())
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("STREAMING_WAIT"));
			}
			else
			{
				Room room = RoomManager.Instance.GetRoom(num);
				if (room != null)
				{
					if (room.Locked)
					{
						((RoomPswdDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.ROOM_PSWD, exclusive: true))?.InitDialog(room.No);
					}
					else if (!CSNetManager.Instance.Sock.SendCS_JOIN_REQ(num, string.Empty, invite: false))
					{
					}
				}
			}
		}
	}
}
