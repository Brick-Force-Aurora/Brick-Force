using System;
using System.Collections.Generic;
using UnityEngine;

public class Lobby : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.BOTTOM;

	private LOBBY_TYPE curLobbyType = LOBBY_TYPE.ROOMS;

	public LobbyTools lobbyTools;

	public RoomListFrame roomListFrm;

	public LobbyChat lobbyChat;

	public Mirror mirror;

	public Equipment equipmentFrm;

	public Shop shopFrm;

	public MapInfo mapInfo;

	public MyMapFrame myMapFrm;

	public SearchMapFrame searchMapFrm;

	public RankMapFrame rankMapFrm;

	public Messenger messenger;

	public BannerViewer banner;

	public SystemMessage systemMsg;

	private BndConfig bndConfig = new BndConfig();

	private CtfModeConfig ctfConfig = new CtfModeConfig();

	private DefenseModeConfig dmConfig = new DefenseModeConfig();

	private BlastModeConfig bmConfig = new BlastModeConfig();

	private IndividualMatchConfig imConfig = new IndividualMatchConfig();

	private TeamMatchConfig tmConfig = new TeamMatchConfig();

	private BungeeModeConfig bgConfig = new BungeeModeConfig();

	private EscapeModeConfig escConfig = new EscapeModeConfig();

	private ZombieModeConfig zombieConfig = new ZombieModeConfig();

	public Texture2D texPopupBg;

	public Texture2D unreadBg;

	public Texture2D[] crowdedMarks;

	private Color clrFadeIn = Color.white;

	private Color clrFadeOut = new Color(1f, 1f, 1f, 0f);

	private Color clrFade = Color.white;

	public float fadeInTime = 3f;

	public float fadeOutTime = 1f;

	private bool fadeIn;

	private float deltaTime;

	private Rect crdChannelList = new Rect(954f, 9f, 64f, 44f);

	private Rect crdMainGrid = new Rect(426f, 9f, 420f, 46f);

	private Rect crdRightBg = new Rect(248f, 60f, 774f, 707f);

	private Rect crdQuick = new Rect(268f, 80f, 146f, 392f);

	private Rect crdRooms = new Rect(414f, 80f, 146f, 392f);

	private Rect crdShop = new Rect(560f, 80f, 146f, 392f);

	private Rect crdMapManage = new Rect(706f, 80f, 146f, 392f);

	private Rect crdMyItems = new Rect(852f, 80f, 146f, 392f);

	public Rect crdQuickBox = new Rect(268f, 90f, 146f, 80f);

	public Rect crdRoomsBox = new Rect(414f, 90f, 146f, 80f);

	public Rect crdShopBox = new Rect(560f, 90f, 146f, 80f);

	public Rect crdMapBox = new Rect(706f, 90f, 146f, 80f);

	public Rect crdMyItemsBox = new Rect(852f, 90f, 146f, 80f);

	private Rect crdExpand = new Rect(559f, 472f, 150f, 16f);

	private Rect crdLine = new Rect(220f, 488f, 827f, 12f);

	private Rect crdSubBg = new Rect(0f, 0f, 1024f, 768f);

	private Rect crdVLine = new Rect(247f, 43f, 1f, 685f);

	public Texture2D[] texs;

	public Texture2D iconkey;

	private Texture2D[] modeIcon;

	private Texture2D[] pingIcon;

	public string[] strs;

	public string[] strGameModes;

	public Texture2D texStarGrade;

	public Texture2D texStarGradeBg;

	public UIMyButton randomBoxBtn;

	public UILabel randomBoxText;

	public UIImage randomBoxIcon;

	private Rect crdShooterRoomList = new Rect(245f, 126f, 760f, 580f);

	private Rect crdShooterRoomListTemp = new Rect(245f, 126f, 760f, 580f);

	private Vector2 crdRoomBtn = new Vector2(970f, 30f);

	private Vector2 crdNo = new Vector2(280f, 115f);

	private Vector2 crdMap = new Vector2(350f, 115f);

	private Vector2 crdTitle = new Vector2(460f, 115f);

	private Vector2 crdMode = new Vector2(779f, 115f);

	private Vector2 crdWeapon = new Vector2(870f, 115f);

	private Vector2 crdPlayers = new Vector2(950f, 115f);

	private Rect crdCreate = new Rect(255f, 64f, 196f, 34f);

	private Rect crdJoin = new Rect(457f, 64f, 196f, 34f);

	private Rect crdRefreshBtn = new Rect(664f, 64f, 32f, 32f);

	private Rect crdComboMode = new Rect(818f, 70f, 180f, 25f);

	private Rect crdComboStatus = new Rect(760f, 70f, 180f, 25f);

	private Rect crdBlueBox = new Rect(2f, 61f, 241f, 533f);

	private Vector2 crdRoomName = new Vector2(120f, 80f);

	private Rect crdMapThumbnail = new Rect(58f, 100f, 128f, 128f);

	private Vector2 crdMapName = new Vector2(120f, 243f);

	private Rect crdThumbUp = new Rect(14f, 258f, 22f, 22f);

	private Rect crdThumbDn = new Rect(130f, 258f, 22f, 22f);

	private Vector2 crdAbuse = new Vector2(14f, 540f);

	private float optionLX = 72f;

	private float optionRX = 190f;

	private float optionY = 300f;

	private float diff_y = 22f;

	private Vector2 crdBox = new Vector2(100f, 18f);

	private TextAnchor textAnchor = TextAnchor.MiddleCenter;

	private string[] weaponOptions = new string[3]
	{
		"USE_ALL_WEAPON",
		"USE_AUX_WEAPON",
		"USE_MELEE_WEAPON"
	};

	private Room.COLUMN sortedBy;

	private bool ascending = true;

	private int roomNo = -1;

	private int overRoomNo = -1;

	private Vector2 scrollPosition;

	private float doubleClickTimeout = 0.2f;

	private float lastClickTime;

	private bool bGuiEnable = true;

	public float BtnWidth = 30f;

	private float ElaspedTimeRoomInfo;

	private bool bChatExpand;

	private float fExpand;

	private StreamedLevelLoadibilityChecker sll;

	public bool bChatView = true;

	private int startFrame;

	private bool haveClanOrWhisper;

	private ComboBox cboxMode;

	private int selectedMode = -1;

	private List<Room.ROOM_TYPE> modefilters;

	private Room.ROOM_TYPE[] filter = new Room.ROOM_TYPE[10]
	{
		Room.ROOM_TYPE.NONE,
		Room.ROOM_TYPE.TEAM_MATCH,
		Room.ROOM_TYPE.INDIVIDUAL,
		Room.ROOM_TYPE.CAPTURE_THE_FLAG,
		Room.ROOM_TYPE.EXPLOSION,
		Room.ROOM_TYPE.MISSION,
		Room.ROOM_TYPE.BND,
		Room.ROOM_TYPE.BUNGEE,
		Room.ROOM_TYPE.ESCAPE,
		Room.ROOM_TYPE.ZOMBIE
	};

	private ComboBox cboxStatus;

	private int selectedStatus = -1;

	private Room.ROOM_STATUS[] status = new Room.ROOM_STATUS[3]
	{
		Room.ROOM_STATUS.NONE,
		Room.ROOM_STATUS.WAITING,
		Room.ROOM_STATUS.PLAYING
	};

	public Tooltip tooltip;

	private string lastTooltip = string.Empty;

	private float focusTime;

	private string tooltipMessage = string.Empty;

	private int selected;

	public LOBBY_TYPE CurLobbyType
	{
		get
		{
			return curLobbyType;
		}
		set
		{
			curLobbyType = value;
		}
	}

	public bool HaveClanOrWhisper
	{
		get
		{
			return haveClanOrWhisper;
		}
		set
		{
			haveClanOrWhisper = value;
		}
	}

	private void Start()
	{
		GlobalVars.Instance.ApplyAudioSource();
		GlobalVars.Instance.clanSendJoinREQ = -1;
		UserMapInfoManager.Instance.VerifySavedData();
		RoomManager.Instance.ResetCurrentRoomRelatedInfo();
		BrickManager.Instance.MakeSystemMapInstance(BrickManager.SYSTEM_MAP.LOBBY);
		P2PManager.Instance.RemoveAll();
		BrickManManager.Instance.Clear();
		ThumbnailDownloader.Instance.Clear();
		lobbyTools.Start();
		roomListFrm.Start();
		lobbyChat.Start();
		lobbyChat.SetChatStyle(LOBBYCHAT_STYLE.LOW);
		mirror.Start();
		equipmentFrm.Start();
		shopFrm.Start();
		mapInfo.Start();
		myMapFrm.Start();
		searchMapFrm.Start();
		rankMapFrm.Start();
		messenger.Start();
		systemMsg.Start();
		banner.Start();
		banner.SetupMain(this);
		roomNo = -1;
		MyInfoManager.Instance.Slot = -1;
		MyInfoManager.Instance.Status = 0;
		GlobalVars.Instance.battleStarting = false;
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			sll = gameObject.GetComponent<StreamedLevelLoadibilityChecker>();
		}
		SetPage(LOBBY_TYPE.ROOMS);
		CSNetManager.Instance.Sock.SendCS_ROOM_LIST_REQ();
		if (Compass.Instance.LobbyType == LOBBY_TYPE.SHOP)
		{
			selected = 2;
			SetPage(LOBBY_TYPE.SHOP);
			GlobalVars.Instance.LobbyType = LOBBY_TYPE.SHOP;
		}
		if (GlobalVars.Instance.GotoLobbyRoomList)
		{
			GlobalVars.Instance.GotoLobbyRoomList = false;
			SetPage(LOBBY_TYPE.ROOMS);
		}
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
		pingIcon = new Texture2D[4];
		pingIcon[0] = GlobalVars.Instance.texPingGray;
		pingIcon[1] = GlobalVars.Instance.texPingGreen;
		pingIcon[3] = GlobalVars.Instance.texPingRed;
		pingIcon[2] = GlobalVars.Instance.texPingYellow;
		startFrame = Time.frameCount;
		if (MyInfoManager.Instance.FirstLoginFp > 0)
		{
			string msg = string.Format(StringMgr.Instance.Get("FIRST_LOGIN_FP"), MyInfoManager.Instance.FirstLoginFp);
			MessageBoxMgr.Instance.AddMessage(msg);
			MyInfoManager.Instance.FirstLoginFp = 0;
		}
		UnityEngine.Object.Instantiate((UnityEngine.Object)VersionTextureManager.Instance.seasonTexture.objPreviewBg);
	}

	private int convMode2Combo(int GameMode)
	{
		int[] array = new int[4]
		{
			3,
			1,
			0,
			2
		};
		return array[GameMode - 1];
	}

	private int convCombo2Mode(int combo)
	{
		int[] array = new int[4]
		{
			3,
			2,
			4,
			1
		};
		return array[combo];
	}

	public void SetPage(LOBBY_TYPE type)
	{
		if (IsStartAfterPlay() && curLobbyType != type)
		{
			if (type == LOBBY_TYPE.EQUIP)
			{
				equipmentFrm.Default();
			}
			if (type == LOBBY_TYPE.SHOP)
			{
				shopFrm.InitPreview();
			}
			if (type == LOBBY_TYPE.MAP)
			{
				mapInfo.checkBattleModes();
			}
			if (type != LOBBY_TYPE.ROOMS)
			{
				GlobalVars.Instance.clanSendJoinREQ = -1;
				bChatView = false;
				lobbyChat.hideCloseButton(close: false);
			}
			else
			{
				bChatView = true;
				lobbyChat.hideCloseButton(close: true);
			}
			lobbyChat.SetChatStyle(LOBBYCHAT_STYLE.LOW);
			curLobbyType = type;
			GlobalVars.Instance.LobbyType = type;
			if (BuildOption.Instance.Props.refreshRoomsManually && curLobbyType == LOBBY_TYPE.ROOMS)
			{
				CSNetManager.Instance.Sock.SendCS_ROOM_LIST_REQ();
			}
			if (type == LOBBY_TYPE.SHOP || type == LOBBY_TYPE.EQUIP)
			{
				mirror.MirrorType = MIRROR_TYPE.SIMPLE;
			}
			else
			{
				mirror.MirrorType = MIRROR_TYPE.BASE;
			}
		}
	}

	private void UpdateFade()
	{
		deltaTime += Time.deltaTime;
		if (fadeIn)
		{
			if (deltaTime <= fadeInTime)
			{
				clrFade = Color.Lerp(clrFadeOut, clrFadeIn, deltaTime / fadeInTime);
			}
			else
			{
				deltaTime = 0f;
				fadeIn = false;
			}
		}
		else if (deltaTime <= fadeOutTime)
		{
			clrFade = Color.Lerp(clrFadeIn, clrFadeOut, deltaTime / fadeOutTime);
		}
		else
		{
			deltaTime = 0f;
			fadeIn = true;
		}
	}

	private void Update()
	{
		focusTime += Time.deltaTime;
		lobbyChat.Update();
		roomListFrm.Update();
		messenger.Update();
		mirror.Update();
		lobbyTools.Update();
		equipmentFrm.Update();
		shopFrm.Update();
		mapInfo.Update();
		banner.Update();
		UpdateFade();
		if (curLobbyType == LOBBY_TYPE.ROOMS && roomNo >= 0)
		{
			ElaspedTimeRoomInfo += Time.deltaTime;
			if (ElaspedTimeRoomInfo > 3f)
			{
				CSNetManager.Instance.Sock.SendCS_ROOM_REQ(roomNo);
				ElaspedTimeRoomInfo = 0f;
			}
		}
		deltaTime += Time.deltaTime;
		if (fadeIn)
		{
			if (deltaTime <= fadeInTime)
			{
				clrFade = Color.Lerp(clrFadeOut, clrFadeIn, deltaTime / fadeInTime);
			}
			else
			{
				deltaTime = 0f;
				fadeIn = false;
			}
		}
		else if (deltaTime <= fadeOutTime)
		{
			clrFade = Color.Lerp(clrFadeIn, clrFadeOut, deltaTime / fadeOutTime);
		}
		else
		{
			deltaTime = 0f;
			fadeIn = true;
		}
		if (UpdateJoinRoom() && !CSNetManager.Instance.Sock.SendCS_JOIN_REQ(roomNo, string.Empty, invite: false))
		{
		}
	}

	private void OnDisable()
	{
	}

	private void OnClanExiled()
	{
		MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("JUST_EXILED_FROM_CLAN"));
	}

	private void OnNotice(string text)
	{
		if (curLobbyType == LOBBY_TYPE.ROOMS)
		{
			SystemInform.Instance.AddMessage(text);
		}
	}

	private void OnNoticeCenter(string text)
	{
		SystemInform.Instance.AddMessageCenter(text);
	}

	private void OnButtonList()
	{
	}

	private void OnGUI_Lobby()
	{
		Color byteColor2FloatColor = GlobalVars.Instance.GetByteColor2FloatColor(244, 151, 25);
		lobbyTools.OnGUI();
		mirror.OnGUI();
		banner.OnGUI();
		GUI.Box(crdRightBg, string.Empty, "BoxBase");
		Rect position = new Rect(crdLine.x, crdLine.y - fExpand, crdLine.width, crdLine.height);
		GUI.Box(position, string.Empty, "DivideLine");
		if (!DialogManager.Instance.IsModal)
		{
			GUI.enabled = bGuiEnable;
		}
		if (bChatExpand)
		{
			if (ChannelManager.Instance.CurChannel.Mode == 3)
			{
				if (GlobalVars.Instance.MyButton(crdQuickBox, string.Empty, "BtnMenu"))
				{
					if (sll != null && !sll.CanStreamedLevelBeLoaded())
					{
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("STREAMING_WAIT"));
					}
					else
					{
						((CreateRoomDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.CREATE_ROOM, exclusive: false))?.InitDialog();
					}
				}
			}
			else if (GlobalVars.Instance.MyButton(crdQuickBox, string.Empty, "BtnMenu") && IsStartAfterPlay())
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
			if (GlobalVars.Instance.MyButton(crdRoomsBox, string.Empty, "BtnMenu"))
			{
				haveClanOrWhisper = false;
				SetPage(LOBBY_TYPE.ROOMS);
			}
			if (GlobalVars.Instance.MyButton(crdShopBox, string.Empty, "BtnMenu"))
			{
				haveClanOrWhisper = false;
				GoToShop();
			}
			if (GlobalVars.Instance.MyButton(crdMapBox, string.Empty, "BtnMenu"))
			{
				haveClanOrWhisper = false;
				SetPage(LOBBY_TYPE.MAP);
			}
			if (GlobalVars.Instance.MyButton(crdMyItemsBox, string.Empty, "BtnMenu"))
			{
				haveClanOrWhisper = false;
				SetPage(LOBBY_TYPE.EQUIP);
			}
			if (GlobalVars.Instance.MyButton(new Rect(crdExpand.x, crdExpand.y - fExpand, crdExpand.width, crdExpand.height), string.Empty, "BtnDropDown"))
			{
				bChatExpand = false;
				fExpand = 0f;
				lobbyChat.SetChatStyle(LOBBYCHAT_STYLE.LOW);
				messenger.ChangeHeight(fExpand);
			}
		}
		else
		{
			if (ChannelManager.Instance.CurChannel.Mode == 3)
			{
				if (GlobalVars.Instance.MyButton(crdQuick, string.Empty, "BtnQuickJoinB" + BuildOption.Instance.Props.GetSeasonCount()) && IsStartAfterPlay())
				{
					if (sll != null && !sll.CanStreamedLevelBeLoaded())
					{
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("STREAMING_WAIT"));
					}
					else
					{
						((CreateRoomDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.CREATE_ROOM, exclusive: false))?.InitDialog();
					}
				}
			}
			else if (GlobalVars.Instance.MyButton(crdQuick, string.Empty, "BtnQuickJoin" + BuildOption.Instance.Props.GetSeasonCount()) && IsStartAfterPlay())
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
			if (ChannelManager.Instance.CurChannel.Mode == 3)
			{
				if (GlobalVars.Instance.MyButton(crdRooms, string.Empty, "BtnGameRoomsB" + BuildOption.Instance.Props.GetSeasonCount()))
				{
					haveClanOrWhisper = false;
					SetPage(LOBBY_TYPE.ROOMS);
				}
			}
			else if (GlobalVars.Instance.MyButton(crdRooms, string.Empty, "BtnGameRooms" + BuildOption.Instance.Props.GetSeasonCount()))
			{
				haveClanOrWhisper = false;
				SetPage(LOBBY_TYPE.ROOMS);
			}
			if (GlobalVars.Instance.MyButton(crdShop, string.Empty, "BtnItemShop" + BuildOption.Instance.Props.GetSeasonCount()))
			{
				haveClanOrWhisper = false;
				SetPage(LOBBY_TYPE.SHOP);
			}
			if (GlobalVars.Instance.MyButton(crdMapManage, string.Empty, "BtnMapManager" + BuildOption.Instance.Props.GetSeasonCount()))
			{
				haveClanOrWhisper = false;
				SetPage(LOBBY_TYPE.MAP);
			}
			if (GlobalVars.Instance.MyButton(crdMyItems, string.Empty, "BtnMyItems" + BuildOption.Instance.Props.GetSeasonCount()))
			{
				haveClanOrWhisper = false;
				SetPage(LOBBY_TYPE.EQUIP);
			}
			if (GlobalVars.Instance.MyButton(crdExpand, string.Empty, "BtnDropUp"))
			{
				bChatExpand = true;
				fExpand = 250f;
				lobbyChat.SetChatStyle(LOBBYCHAT_STYLE.HIGH);
				messenger.ChangeHeight(fExpand);
			}
			GUI.Box(crdQuickBox, string.Empty, "BoxMain");
			GUI.Box(crdRoomsBox, string.Empty, "BoxMain");
			GUI.Box(crdShopBox, string.Empty, "BoxMain");
			GUI.Box(crdMapBox, string.Empty, "BoxMain");
			GUI.Box(crdMyItemsBox, string.Empty, "BoxMain");
		}
		string[] array = new string[2];
		char c = '\n';
		int num = 0;
		string text;
		if (ChannelManager.Instance.CurChannel.Mode == 3)
		{
			num = 0;
			text = StringMgr.Instance.Get("CREATE_MAP_LF");
			string[] array2 = text.Split(c);
			foreach (string text2 in array2)
			{
				array[num++] = text2;
			}
			if (num == 2)
			{
				LabelUtil.TextOut(new Vector2(crdQuickBox.x + crdQuickBox.width / 2f, crdQuickBox.y + 30f), array[0], "Label", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(crdQuickBox.x + crdQuickBox.width / 2f, crdQuickBox.y + 50f), array[1], "Label", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
			else
			{
				LabelUtil.TextOut(new Vector2(crdQuickBox.x + crdQuickBox.width / 2f, crdQuickBox.y + 40f), array[0], "Label", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
			num = 0;
			text = StringMgr.Instance.Get("BUILD_ROOMS_LF");
			string[] array3 = text.Split(c);
			foreach (string text3 in array3)
			{
				array[num++] = text3;
			}
			if (num == 2)
			{
				LabelUtil.TextOut(new Vector2(crdRoomsBox.x + crdRoomsBox.width / 2f, crdRoomsBox.y + 30f), array[0], "Label", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(crdRoomsBox.x + crdRoomsBox.width / 2f, crdRoomsBox.y + 50f), array[1], "Label", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
			else
			{
				LabelUtil.TextOut(new Vector2(crdRoomsBox.x + crdRoomsBox.width / 2f, crdRoomsBox.y + 40f), array[0], "Label", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
		}
		else
		{
			num = 0;
			text = StringMgr.Instance.Get("QUICK_JOIN_LF");
			string[] array4 = text.Split(c);
			foreach (string text4 in array4)
			{
				array[num++] = text4;
			}
			if (num == 2)
			{
				LabelUtil.TextOut(new Vector2(crdQuickBox.x + crdQuickBox.width / 2f, crdQuickBox.y + 30f), array[0], "Label", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(crdQuickBox.x + crdQuickBox.width / 2f, crdQuickBox.y + 50f), array[1], "Label", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
			else
			{
				LabelUtil.TextOut(new Vector2(crdQuickBox.x + crdQuickBox.width / 2f, crdQuickBox.y + 40f), array[0], "Label", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
			num = 0;
			text = StringMgr.Instance.Get("GAME_ROOMS_LF");
			string[] array5 = text.Split(c);
			foreach (string text5 in array5)
			{
				array[num++] = text5;
			}
			if (num == 2)
			{
				LabelUtil.TextOut(new Vector2(crdRoomsBox.x + crdRoomsBox.width / 2f, crdRoomsBox.y + 30f), array[0], "Label", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(crdRoomsBox.x + crdRoomsBox.width / 2f, crdRoomsBox.y + 50f), array[1], "Label", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
			else
			{
				LabelUtil.TextOut(new Vector2(crdRoomsBox.x + crdRoomsBox.width / 2f, crdRoomsBox.y + 40f), array[0], "Label", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
		}
		num = 0;
		text = StringMgr.Instance.Get("ITEM_SHOPPING_LF");
		string[] array6 = text.Split(c);
		foreach (string text6 in array6)
		{
			array[num++] = text6;
		}
		if (num == 2)
		{
			LabelUtil.TextOut(new Vector2(crdShopBox.x + crdShopBox.width / 2f, crdShopBox.y + 30f), array[0], "Label", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(crdShopBox.x + crdShopBox.width / 2f, crdShopBox.y + 50f), array[1], "Label", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
		else
		{
			LabelUtil.TextOut(new Vector2(crdShopBox.x + crdShopBox.width / 2f, crdShopBox.y + 40f), array[0], "Label", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
		num = 0;
		text = StringMgr.Instance.Get("MAP_MANAGER_LF");
		string[] array7 = text.Split(c);
		foreach (string text7 in array7)
		{
			array[num++] = text7;
		}
		if (num == 2)
		{
			LabelUtil.TextOut(new Vector2(crdMapBox.x + crdMapBox.width / 2f, crdMapBox.y + 30f), array[0], "Label", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(crdMapBox.x + crdMapBox.width / 2f, crdMapBox.y + 50f), array[1], "Label", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
		else
		{
			LabelUtil.TextOut(new Vector2(crdMapBox.x + crdMapBox.width / 2f, crdMapBox.y + 40f), array[0], "Label", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
		num = 0;
		text = StringMgr.Instance.Get("MY_ITEMS_LF");
		string[] array8 = text.Split(c);
		foreach (string text8 in array8)
		{
			array[num++] = text8;
		}
		if (num == 2)
		{
			LabelUtil.TextOut(new Vector2(crdMyItemsBox.x + crdMyItemsBox.width / 2f, crdMyItemsBox.y + 30f), array[0], "Label", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(crdMyItemsBox.x + crdMyItemsBox.width / 2f, crdMyItemsBox.y + 50f), array[1], "Label", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
		else
		{
			LabelUtil.TextOut(new Vector2(crdMyItemsBox.x + crdMyItemsBox.width / 2f, crdMyItemsBox.y + 40f), array[0], "Label", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
		lobbyChat.OnGUI();
		messenger.OnGUI();
		if (GlobalVars.Instance.MyButton3(crdChannelList, new GUIContent(ChannelManager.Instance.CurChannel.Name, GlobalVars.Instance.iconDetail), "BtnAction"))
		{
			ChannelManager.Instance.PrevScene = Application.loadedLevelName;
			Application.LoadLevel("ChangeChannel");
		}
		SystemInform.Instance.DoScrollMessage();
		if (!bGuiEnable)
		{
			GUI.enabled = true;
		}
		GUI.enabled = !DialogManager.Instance.IsModal;
	}

	private void OnGUI_ItemShop()
	{
		bool enabled = GUI.enabled;
		if (enabled)
		{
			GUI.enabled = !shopFrm.CheckFilterCombo();
		}
		Color byteColor2FloatColor = GlobalVars.Instance.GetByteColor2FloatColor(244, 151, 25);
		TextureUtil.DrawTexture(crdSubBg, texPopupBg, ScaleMode.StretchToFill);
		GUI.Box(crdVLine, string.Empty, "DivideLineV");
		lobbyTools.OnGUI();
		string[] array = new string[4]
		{
			StringMgr.Instance.Get("GAME_ROOMS_MAINTAB"),
			StringMgr.Instance.Get("MAP_MANAGER_MAINTAB"),
			StringMgr.Instance.Get("ITEM_SHOPPING_MAINTAB"),
			StringMgr.Instance.Get("MY_ITEMS_MAINTAB")
		};
		crdMainGrid.width = (float)(105 * array.Length);
		int num = selected;
		selected = GUI.SelectionGrid(crdMainGrid, selected, array, array.Length, "BtnMain");
		if (!shopFrm.GetBuyConfirm())
		{
			Vector2 pos = new Vector2(260f, 65f);
			if (shopFrm.ActiveRelateItem)
			{
				LabelUtil.TextOut(pos, StringMgr.Instance.Get("RELATED_ITEMS").ToUpper(), "BigBtnLabel", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			else if (BuildOption.Instance.Props.randomBox == BuildOption.RANDOM_BOX_TYPE.NETMARBLE)
			{
				if (randomBoxBtn.Draw())
				{
					CSNetManager.Instance.Sock.SendCS_TC_OPEN_REQ();
				}
				randomBoxText.Draw();
				randomBoxIcon.Draw();
			}
			if (ChannelManager.Instance.CurChannel.Mode == 3)
			{
				if (num != selected && selected == 0)
				{
					shopFrm.RollbackPreview();
					shopFrm.CurItem = 0;
					SetPage(LOBBY_TYPE.ROOMS);
				}
			}
			else if (num != selected && selected == 0)
			{
				shopFrm.RollbackPreview();
				shopFrm.CurItem = 0;
				SetPage(LOBBY_TYPE.ROOMS);
			}
			if (num != selected && selected == 1)
			{
				shopFrm.RollbackPreview();
				shopFrm.CurItem = 0;
				SetPage(LOBBY_TYPE.MAP);
			}
			if (num != selected && selected == 2)
			{
				SetPage(LOBBY_TYPE.SHOP);
			}
			if (num != selected && selected == 3)
			{
				shopFrm.RollbackPreview();
				shopFrm.CurItem = 0;
				SetPage(LOBBY_TYPE.EQUIP);
			}
			DoChannelList();
		}
		mirror.OnGUI();
		shopFrm.OnGUI();
		if (bChatView)
		{
			lobbyChat.OnGUI();
			messenger.OnGUI();
		}
		if (enabled)
		{
			GUI.enabled = enabled;
		}
		if (!shopFrm.ActiveRelateItem)
		{
			shopFrm.DoFilterCombo();
		}
		if (!bChatView)
		{
			Rect rc = new Rect(559f, 750f, 150f, 16f);
			if (GlobalVars.Instance.MyButton(rc, string.Empty, "BtnDropUp"))
			{
				bChatView = true;
				haveClanOrWhisper = false;
			}
		}
		else
		{
			Rect rc2 = new Rect(559f, 472f, 150f, 16f);
			if (GlobalVars.Instance.MyButton(rc2, string.Empty, "BtnDropDown"))
			{
				bChatView = false;
				haveClanOrWhisper = false;
			}
		}
		if (!bChatView && haveClanOrWhisper)
		{
			Color color = GUI.color;
			GUI.color = clrFade;
			Rect position = new Rect(559f, 750f, 150f, 16f);
			TextureUtil.DrawTexture(position, unreadBg, ScaleMode.StretchToFill);
			GUI.color = color;
		}
		GUI.enabled = enabled;
	}

	private void DoChannelList()
	{
		if (GlobalVars.Instance.MyButton(crdChannelList, string.Empty, "BtnChannelList"))
		{
			ChannelManager.Instance.PrevScene = Application.loadedLevelName;
			Application.LoadLevel("ChangeChannel");
		}
		Vector2 pos = new Vector2(crdChannelList.x + crdChannelList.width / 2f, crdChannelList.y);
		LabelUtil.TextOut(pos, ChannelManager.Instance.CurChannel.Name, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
	}

	private void OnGUI_MyEquip()
	{
		bool enabled = GUI.enabled;
		if (enabled)
		{
			GUI.enabled = !equipmentFrm.CheckFilterCombo();
		}
		TextureUtil.DrawTexture(crdSubBg, texPopupBg, ScaleMode.StretchToFill);
		GUI.Box(crdVLine, string.Empty, "DivideLineV");
		lobbyTools.OnGUI();
		string[] array = new string[4]
		{
			StringMgr.Instance.Get("GAME_ROOMS_MAINTAB"),
			StringMgr.Instance.Get("MAP_MANAGER_MAINTAB"),
			StringMgr.Instance.Get("ITEM_SHOPPING_MAINTAB"),
			StringMgr.Instance.Get("MY_ITEMS_MAINTAB")
		};
		crdMainGrid.width = (float)(105 * array.Length);
		int num = selected;
		selected = GUI.SelectionGrid(crdMainGrid, selected, array, array.Length, "BtnMain");
		if (ChannelManager.Instance.CurChannel.Mode == 3)
		{
			if (num != selected && selected == 0)
			{
				equipmentFrm.CurItem = 0;
				SetPage(LOBBY_TYPE.ROOMS);
			}
		}
		else if (num != selected && selected == 0)
		{
			equipmentFrm.CurItem = 0;
			SetPage(LOBBY_TYPE.ROOMS);
		}
		if (num != selected && selected == 1)
		{
			equipmentFrm.CurItem = 0;
			SetPage(LOBBY_TYPE.MAP);
		}
		if (num != selected && selected == 2)
		{
			equipmentFrm.CurItem = 0;
			SetPage(LOBBY_TYPE.SHOP);
		}
		if (num != selected && selected == 3)
		{
			SetPage(LOBBY_TYPE.EQUIP);
		}
		DoChannelList();
		mirror.OnGUI();
		equipmentFrm.OnGUI();
		if (bChatView)
		{
			lobbyChat.OnGUI();
			messenger.OnGUI();
		}
		if (enabled)
		{
			GUI.enabled = enabled;
		}
		if (!bChatView)
		{
			Rect rc = new Rect(559f, 750f, 150f, 16f);
			if (GlobalVars.Instance.MyButton(rc, string.Empty, "BtnDropUp"))
			{
				bChatView = true;
				haveClanOrWhisper = false;
			}
		}
		else
		{
			Rect rc2 = new Rect(559f, 472f, 150f, 16f);
			if (GlobalVars.Instance.MyButton(rc2, string.Empty, "BtnDropDown"))
			{
				bChatView = false;
				haveClanOrWhisper = false;
			}
		}
		if (!bChatView && haveClanOrWhisper)
		{
			Color color = GUI.color;
			GUI.color = clrFade;
			Rect position = new Rect(559f, 750f, 150f, 16f);
			TextureUtil.DrawTexture(position, unreadBg, ScaleMode.StretchToFill);
			GUI.color = color;
		}
		equipmentFrm.DoFilterCombo();
		GUI.enabled = enabled;
	}

	private void OnGUI_MapManager()
	{
		TextureUtil.DrawTexture(crdSubBg, texPopupBg, ScaleMode.StretchToFill);
		GUI.Box(crdVLine, string.Empty, "DivideLineV");
		lobbyTools.OnGUI();
		string[] array = new string[4]
		{
			StringMgr.Instance.Get("GAME_ROOMS_MAINTAB"),
			StringMgr.Instance.Get("MAP_MANAGER_MAINTAB"),
			StringMgr.Instance.Get("ITEM_SHOPPING_MAINTAB"),
			StringMgr.Instance.Get("MY_ITEMS_MAINTAB")
		};
		crdMainGrid.width = (float)(105 * array.Length);
		int num = selected;
		selected = GUI.SelectionGrid(crdMainGrid, selected, array, array.Length, "BtnMain");
		if (ChannelManager.Instance.CurChannel.Mode == 3)
		{
			if (num != selected && selected == 0)
			{
				SetPage(LOBBY_TYPE.ROOMS);
			}
		}
		else if (num != selected && selected == 0)
		{
			SetPage(LOBBY_TYPE.ROOMS);
		}
		if (num != selected && selected == 1)
		{
			SetPage(LOBBY_TYPE.MAP);
		}
		if (num != selected && selected == 2)
		{
			SetPage(LOBBY_TYPE.SHOP);
		}
		if (num != selected && selected == 3)
		{
			SetPage(LOBBY_TYPE.EQUIP);
		}
		DoChannelList();
		mapInfo.OnGUI();
		if (bChatView)
		{
			lobbyChat.OnGUI();
			messenger.OnGUI();
		}
		if (!bChatView)
		{
			Rect rc = new Rect(559f, 750f, 150f, 16f);
			if (GlobalVars.Instance.MyButton(rc, string.Empty, "BtnDropUp"))
			{
				bChatView = true;
				haveClanOrWhisper = false;
			}
		}
		else
		{
			Rect rc2 = new Rect(559f, 472f, 150f, 16f);
			if (GlobalVars.Instance.MyButton(rc2, string.Empty, "BtnDropDown"))
			{
				bChatView = false;
				haveClanOrWhisper = false;
			}
		}
		if (!bChatView && haveClanOrWhisper)
		{
			Color color = GUI.color;
			GUI.color = clrFade;
			Rect position = new Rect(559f, 750f, 150f, 16f);
			TextureUtil.DrawTexture(position, unreadBg, ScaleMode.StretchToFill);
			GUI.color = color;
		}
	}

	private bool CheckModeCombo()
	{
		if (cboxMode == null)
		{
			return false;
		}
		return cboxMode.IsClickedComboButton();
	}

	private bool CheckStatusCombo()
	{
		if (cboxStatus == null)
		{
			return false;
		}
		return cboxStatus.IsClickedComboButton();
	}

	private void VerifyComboBox()
	{
		if (cboxMode == null)
		{
			cboxMode = new ComboBox();
			cboxMode.Initialize(bImage: false, new Vector2(crdComboMode.width, crdComboMode.height));
			cboxMode.setStyleNames("BoxFilterBg", "BtnArrowDn", "BtnArrowUp", "BoxFilterCombo");
			cboxMode.setTextColor(Color.white, GlobalVars.Instance.GetByteColor2FloatColor(205, 100, 36));
			cboxMode.setBackground(Color.white, GlobalVars.Instance.GetByteColor2FloatColor(0, 53, 92));
		}
		if (cboxStatus == null)
		{
			cboxStatus = new ComboBox();
			cboxStatus.Initialize(bImage: false, new Vector2(crdComboMode.width, crdComboMode.height));
			cboxStatus.setStyleNames("BoxFilterBg", "BtnArrowDn", "BtnArrowUp", "BoxFilterCombo");
			cboxStatus.setTextColor(Color.white, GlobalVars.Instance.GetByteColor2FloatColor(205, 100, 36));
			cboxStatus.setBackground(Color.white, GlobalVars.Instance.GetByteColor2FloatColor(0, 53, 92));
		}
	}

	private void DoStatusComboBox()
	{
		if (ChannelManager.Instance.CurChannel.Mode != 3)
		{
			int num = selectedStatus;
			if (selectedStatus < 0 || selectedStatus >= status.Length)
			{
				selectedStatus = 0;
			}
			string buttonText = string.Empty;
			GUIContent[] array = new GUIContent[status.Length];
			for (int i = 0; i < array.Length; i++)
			{
				string text = Room.Status2String((int)status[i]);
				if (text.Length <= 0)
				{
					text = StringMgr.Instance.Get("ALL");
				}
				array[i] = new GUIContent(text);
				if (selectedStatus == i)
				{
					buttonText = text;
				}
			}
			selectedStatus = cboxStatus.List(crdComboStatus, buttonText, array);
			if (num != selectedStatus)
			{
				RoomManager.Instance.RefreshRoomList();
			}
		}
	}

	private void Awake()
	{
		modefilters = new List<Room.ROOM_TYPE>();
		for (int i = 0; i < filter.Length; i++)
		{
			if (filter[i] == Room.ROOM_TYPE.NONE || (BuildOption.Instance.Props.IsSupportMode(filter[i]) && ChannelManager.Instance.IsSupportMode(filter[i])))
			{
				modefilters.Add(filter[i]);
			}
		}
	}

	private void DoModeComboBox()
	{
		if (ChannelManager.Instance.CurChannel.Mode != 3)
		{
			int num = selectedMode;
			if (selectedMode < 0 || selectedMode >= filter.Length)
			{
				selectedMode = 0;
			}
			string buttonText = string.Empty;
			GUIContent[] array = new GUIContent[modefilters.Count];
			for (int i = 0; i < array.Length; i++)
			{
				string text = Room.Type2String(modefilters[i]);
				if (text.Length <= 0)
				{
					text = StringMgr.Instance.Get("ALL");
				}
				array[i] = new GUIContent(text);
				if (selectedMode == i)
				{
					buttonText = text;
				}
			}
			selectedMode = cboxMode.List(crdComboMode, buttonText, array);
			if (num != selectedMode)
			{
				RoomManager.Instance.RefreshRoomList();
			}
		}
	}

	private void OnGUI_GameRooms()
	{
		VerifyComboBox();
		bool enabled = GUI.enabled;
		if (CheckModeCombo() || CheckStatusCombo())
		{
			GUI.enabled = false;
		}
		Color byteColor2FloatColor = GlobalVars.Instance.GetByteColor2FloatColor(244, 151, 25);
		TextureUtil.DrawTexture(crdSubBg, texPopupBg, ScaleMode.StretchToFill);
		lobbyTools.OnGUI();
		string[] array = new string[4]
		{
			StringMgr.Instance.Get("GAME_ROOMS_MAINTAB"),
			StringMgr.Instance.Get("MAP_MANAGER_MAINTAB"),
			StringMgr.Instance.Get("ITEM_SHOPPING_MAINTAB"),
			StringMgr.Instance.Get("MY_ITEMS_MAINTAB")
		};
		crdMainGrid.width = (float)(105 * array.Length);
		int num = selected;
		selected = GUI.SelectionGrid(crdMainGrid, selected, array, array.Length, "BtnMain");
		if (ChannelManager.Instance.CurChannel.Mode == 3)
		{
			if (num != selected && selected == 0)
			{
				SetPage(LOBBY_TYPE.ROOMS);
			}
		}
		else if (num != selected && selected == 0)
		{
			SetPage(LOBBY_TYPE.ROOMS);
		}
		if (num != selected && selected == 1)
		{
			SetPage(LOBBY_TYPE.MAP);
		}
		if (num != selected && selected == 2)
		{
			SetPage(LOBBY_TYPE.SHOP);
		}
		if (num != selected && selected == 3)
		{
			SetPage(LOBBY_TYPE.EQUIP);
		}
		DoChannelList();
		if (BuildOption.Instance.Props.refreshRoomsManually && GlobalVars.Instance.MyButton(crdRefreshBtn, string.Empty, "BtnRefresh"))
		{
			CSNetManager.Instance.Sock.SendCS_ROOM_LIST_REQ();
		}
		LabelUtil.TextOut(crdNo, "#", "MidLabel", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(crdMap, StringMgr.Instance.Get("MAP").ToUpper(), "MidLabel", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(crdTitle, StringMgr.Instance.Get("ROOM_TITLE").ToUpper(), "MidLabel", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(crdMode, StringMgr.Instance.Get("ROOM_TYPE").ToUpper(), "MidLabel", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(crdWeapon, StringMgr.Instance.Get("WEAPON_USAGE").ToUpper(), "MidLabel", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(crdPlayers, StringMgr.Instance.Get("NUM_PLAYERS").ToUpper(), "MidLabel", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		Vector2 vector = LabelUtil.CalcLength("MidLabel", "#");
		Rect rc = new Rect(crdNo.x + vector.x - 4f, crdNo.y - 12f, 25f, 23f);
		if (GlobalVars.Instance.MyButton(rc, string.Empty, (sortedBy != 0 || ascending) ? "BtnArrowDn" : "BtnArrowUp"))
		{
			if (sortedBy == Room.COLUMN.NO)
			{
				ascending = !ascending;
			}
			sortedBy = Room.COLUMN.NO;
		}
		Vector2 vector2 = LabelUtil.CalcLength("MidLabel", StringMgr.Instance.Get("MAP").ToUpper());
		Rect rc2 = new Rect(crdMap.x + vector2.x - 4f, crdMap.y - 12f, 25f, 23f);
		if (GlobalVars.Instance.MyButton(rc2, string.Empty, (sortedBy != Room.COLUMN.MAP_ALIAS || ascending) ? "BtnArrowDn" : "BtnArrowUp"))
		{
			if (sortedBy == Room.COLUMN.MAP_ALIAS)
			{
				ascending = !ascending;
			}
			sortedBy = Room.COLUMN.MAP_ALIAS;
		}
		Vector2 vector3 = LabelUtil.CalcLength("MidLabel", StringMgr.Instance.Get("ROOM_TITLE").ToUpper());
		Rect rc3 = new Rect(crdTitle.x + vector3.x - 4f, crdTitle.y - 12f, 25f, 23f);
		if (GlobalVars.Instance.MyButton(rc3, string.Empty, (sortedBy != Room.COLUMN.TITLE || ascending) ? "BtnArrowDn" : "BtnArrowUp"))
		{
			if (sortedBy == Room.COLUMN.TITLE)
			{
				ascending = !ascending;
			}
			sortedBy = Room.COLUMN.TITLE;
		}
		Vector2 vector4 = LabelUtil.CalcLength("MidLabel", StringMgr.Instance.Get("ROOM_TYPE").ToUpper());
		Rect rc4 = new Rect(crdMode.x + vector4.x - 4f, crdMode.y - 12f, 25f, 23f);
		if (GlobalVars.Instance.MyButton(rc4, string.Empty, (sortedBy != Room.COLUMN.TYPE || ascending) ? "BtnArrowDn" : "BtnArrowUp"))
		{
			if (sortedBy == Room.COLUMN.TYPE)
			{
				ascending = !ascending;
			}
			sortedBy = Room.COLUMN.TYPE;
		}
		Vector2 vector5 = LabelUtil.CalcLength("MidLabel", StringMgr.Instance.Get("WEAPON_USAGE").ToUpper());
		Rect rc5 = new Rect(crdWeapon.x + vector5.x - 4f, crdWeapon.y - 12f, 25f, 23f);
		if (GlobalVars.Instance.MyButton(rc5, string.Empty, (sortedBy != Room.COLUMN.WPN_OPT || ascending) ? "BtnArrowDn" : "BtnArrowUp"))
		{
			if (sortedBy == Room.COLUMN.WPN_OPT)
			{
				ascending = !ascending;
			}
			sortedBy = Room.COLUMN.WPN_OPT;
		}
		Vector2 vector6 = LabelUtil.CalcLength("MidLabel", StringMgr.Instance.Get("NUM_PLAYERS").ToUpper());
		Rect rc6 = new Rect(crdPlayers.x + vector6.x - 4f, crdPlayers.y - 12f, 25f, 23f);
		if (GlobalVars.Instance.MyButton(rc6, string.Empty, (sortedBy != Room.COLUMN.NUM_PLAYER || ascending) ? "BtnArrowDn" : "BtnArrowUp"))
		{
			if (sortedBy == Room.COLUMN.NUM_PLAYER)
			{
				ascending = !ascending;
			}
			sortedBy = Room.COLUMN.NUM_PLAYER;
		}
		Room.ROOM_TYPE type = (0 > selectedMode || selectedMode >= modefilters.Count) ? Room.ROOM_TYPE.NONE : modefilters[selectedMode];
		Room.ROOM_STATUS rOOM_STATUS = (0 > selectedStatus || selectedStatus >= status.Length) ? Room.ROOM_STATUS.NONE : status[selectedStatus];
		List<KeyValuePair<int, Room>> list = RoomManager.Instance.ToSortedList(sortedBy, ascending, type, rOOM_STATUS);
		bool flag = false;
		foreach (KeyValuePair<int, Room> item in list)
		{
			if (item.Value.No == roomNo)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			roomNo = -1;
		}
		float height = (float)(30 * list.Count);
		Rect position = new Rect(crdShooterRoomList);
		Rect viewRect = new Rect(0f, 0f, crdShooterRoomList.width - 20f, height);
		if (bChatView)
		{
			position.height = 300f;
		}
		else
		{
			position.height = crdShooterRoomListTemp.height;
		}
		scrollPosition = GUI.BeginScrollView(position, scrollPosition, viewRect);
		float y = scrollPosition.y;
		float num2 = scrollPosition.y + position.height;
		float num3 = 0f;
		foreach (KeyValuePair<int, Room> item2 in list)
		{
			Rect rect = new Rect(0f, num3, crdRoomBtn.x, crdRoomBtn.y);
			float num4 = num3;
			float num5 = num3 + rect.height;
			if (num5 >= y && num4 <= num2)
			{
				if (GlobalVars.Instance.MyButton(rect, new GUIContent(string.Empty, item2.Value.No.ToString()), "RoomButton"))
				{
					roomNo = item2.Value.No;
					if (Time.time - lastClickTime > doubleClickTimeout)
					{
						lastClickTime = Time.time;
					}
					else if (sll != null && !sll.CanStreamedLevelBeLoaded())
					{
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("STREAMING_WAIT"));
					}
					else
					{
						Room room = RoomManager.Instance.GetRoom(roomNo);
						if (room == null)
						{
							MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("FAIL_TO_JOIN_ROOM"));
						}
						else if (room.Locked)
						{
							((RoomPswdDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.ROOM_PSWD, exclusive: true))?.InitDialog(room.No);
						}
						else if (ChannelManager.Instance.CurChannel.Mode != 4)
						{
							if (!CSNetManager.Instance.Sock.SendCS_JOIN_REQ(roomNo, string.Empty, invite: false))
							{
							}
						}
						else
						{
							GlobalVars.Instance.roomNo = roomNo;
							if (room.Status != Room.ROOM_STATUS.PLAYING)
							{
								SendCS_JOIN_REQ();
							}
							else
							{
								MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CANT_BREAK_INTO"));
							}
						}
					}
				}
				bool enabled2 = GUI.enabled;
				bool flag2 = CanEnter(item2.Value);
				if (!flag2)
				{
					GUI.enabled = false;
				}
				if (roomNo == item2.Value.No)
				{
					GUI.Box(rect, string.Empty, "ViewSelected");
				}
				if (item2.Value.Locked)
				{
					TextureUtil.DrawTexture(new Rect(crdMap.x - position.x - 15f, num3 + 5f, 12f, 20f), iconkey, ScaleMode.StretchToFill);
				}
				LabelUtil.TextOut(new Vector2(crdNo.x - position.x, num3 + 15f), item2.Value.GetString(Room.COLUMN.NO), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
				LabelUtil.TextOut(new Vector2(crdMap.x - position.x, num3 + 15f), item2.Value.GetString(Room.COLUMN.MAP_ALIAS), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
				LabelUtil.TextOut(new Vector2(crdTitle.x - position.x, num3 + 15f), item2.Value.GetString(Room.COLUMN.TITLE), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
				int type2 = (int)item2.Value.Type;
				if (0 <= type2 && type2 < modeIcon.Length && modeIcon[type2] != null)
				{
					TextureUtil.DrawTexture(new Rect(crdMode.x - position.x, num3 + 4f, 35f, 22f), modeIcon[type2], ScaleMode.StretchToFill);
				}
				int weaponOption = item2.Value.weaponOption;
				if (0 <= weaponOption && weaponOption < GlobalVars.Instance.iconWeaponOpt.Length && GlobalVars.Instance.iconWeaponOpt[weaponOption] != null)
				{
					TextureUtil.DrawTexture(new Rect(crdWeapon.x - position.x, num3 + 4f, 35f, 22f), GlobalVars.Instance.iconWeaponOpt[weaponOption], ScaleMode.StretchToFill);
				}
				Vector2 vector7 = new Vector2(crdNo.x - position.x - 32f, num3 + 6f);
				if (item2.Value.Status == Room.ROOM_STATUS.PLAYING)
				{
					TextureUtil.DrawTexture(new Rect(vector7.x, vector7.y, (float)crowdedMarks[0].width, (float)crowdedMarks[0].height), crowdedMarks[0], ScaleMode.StretchToFill);
				}
				else if (item2.Value.Status == Room.ROOM_STATUS.PENDING)
				{
					TextureUtil.DrawTexture(new Rect(vector7.x, vector7.y, (float)crowdedMarks[2].width, (float)crowdedMarks[2].height), crowdedMarks[2], ScaleMode.StretchToFill);
				}
				else
				{
					TextureUtil.DrawTexture(new Rect(vector7.x, vector7.y, (float)crowdedMarks[1].width, (float)crowdedMarks[1].height), crowdedMarks[1], ScaleMode.StretchToFill);
				}
				DrawMedal(item2.Value.No, crdMap.x - position.x - 35f, num3 + 5f);
				LabelUtil.TextOut(new Vector2(crdPlayers.x - position.x, num3 + 15f), item2.Value.GetString(Room.COLUMN.NUM_PLAYER), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
				if (!flag2)
				{
					GUI.enabled = enabled2;
				}
			}
			num3 += 30f;
		}
		GUI.EndScrollView();
		if (roomNo >= 0)
		{
			RoomManager.Instance.RefreshRoom(roomNo);
		}
		GUIStyle style = GUI.skin.GetStyle("BtnAction");
		style.fontStyle = FontStyle.Bold;
		GUIContent content = new GUIContent(StringMgr.Instance.Get("CREATE_ROOM").ToUpper(), GlobalVars.Instance.iconBlock);
		if (GlobalVars.Instance.MyButton3(crdCreate, content, "BtnAction"))
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
		content = new GUIContent(StringMgr.Instance.Get("JOIN").ToUpper(), GlobalVars.Instance.iconJoin);
		if (roomNo >= 0)
		{
			if (GlobalVars.Instance.MyButton3(crdJoin, content, "BtnAction"))
			{
				if (sll != null && !sll.CanStreamedLevelBeLoaded())
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("STREAMING_WAIT"));
				}
				else
				{
					Room room2 = RoomManager.Instance.GetRoom(roomNo);
					if (room2 == null)
					{
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("FAIL_TO_JOIN_ROOM"));
					}
					else if (room2.Locked)
					{
						((RoomPswdDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.ROOM_PSWD, exclusive: true))?.InitDialog(room2.No);
					}
					else if (ChannelManager.Instance.CurChannel.Mode != 4)
					{
						if (!CSNetManager.Instance.Sock.SendCS_JOIN_REQ(roomNo, string.Empty, invite: false))
						{
						}
					}
					else
					{
						GlobalVars.Instance.roomNo = roomNo;
						SendCS_JOIN_REQ();
					}
				}
			}
		}
		else
		{
			bool enabled3 = GUI.enabled;
			GUI.enabled = false;
			GlobalVars.Instance.MyButton3(crdJoin, content, "BtnAction");
			GUI.enabled = enabled3;
		}
		mirror.OnGUI();
		banner.OnGUI();
		if (bChatView)
		{
			lobbyChat.OnGUI();
			messenger.OnGUI();
		}
		if (enabled)
		{
			GUI.enabled = enabled;
		}
		DoModeComboBox();
		if (!bChatView)
		{
			Rect rc7 = new Rect(559f, 750f, 150f, 16f);
			if (GlobalVars.Instance.MyButton(rc7, string.Empty, "BtnDropUp"))
			{
				bChatView = true;
				haveClanOrWhisper = false;
			}
		}
		else
		{
			Rect rc8 = new Rect(559f, 472f, 150f, 16f);
			if (GlobalVars.Instance.MyButton(rc8, string.Empty, "BtnDropDown"))
			{
				bChatView = false;
				haveClanOrWhisper = false;
			}
		}
		if (!bChatView && haveClanOrWhisper)
		{
			Color color = GUI.color;
			GUI.color = clrFade;
			Rect position2 = new Rect(437f, 750f, 150f, 16f);
			TextureUtil.DrawTexture(position2, unreadBg, ScaleMode.StretchToFill);
			GUI.color = color;
		}
		GUI.enabled = enabled;
		style.fontStyle = FontStyle.Normal;
		DoTooltip();
	}

	private void ShowTooltip(int id)
	{
		LabelUtil.TextOut(new Vector2(10f, 10f), tooltipMessage, "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
	}

	private void DoTooltip()
	{
		if (!DialogManager.Instance.IsModal && Event.current.type == EventType.Repaint && GUI.enabled)
		{
			if (lastTooltip != GUI.tooltip)
			{
				focusTime = 0f;
			}
			if (focusTime > 0.3f)
			{
				int num = -1;
				try
				{
					num = int.Parse(GUI.tooltip);
				}
				catch
				{
				}
				if (num >= 0)
				{
					overRoomNo = num;
				}
				else
				{
					overRoomNo = -1;
				}
				if (overRoomNo >= 0)
				{
					DoRoomDetail();
				}
				else if (Event.current.type == EventType.Repaint && GUI.tooltip.Length > 0)
				{
					tooltipMessage = GUI.tooltip;
					Vector2 vector = GlobalVars.Instance.ToGUIPoint(Event.current.mousePosition);
					GUIStyle style = GUI.skin.GetStyle("MiniLabel");
					if (style != null)
					{
						Vector2 vector2 = style.CalcSize(new GUIContent(tooltipMessage));
						Rect rc = new Rect(vector.x, vector.y, vector2.x + 20f, vector2.y + 20f);
						GlobalVars.Instance.FitRightNBottomRectInScreen(ref rc);
						GUI.Window(1101, rc, ShowTooltip, string.Empty, "LineWindow");
					}
				}
			}
			lastTooltip = GUI.tooltip;
		}
		else
		{
			GUI.tooltip = string.Empty;
		}
	}

	private bool CanEnter(Room room)
	{
		if (room.Status == Room.ROOM_STATUS.PENDING)
		{
			return false;
		}
		if (room.Status == Room.ROOM_STATUS.WAITING)
		{
			if (room.MaxPlayer == room.CurPlayer)
			{
				return false;
			}
		}
		else if (room.Status == Room.ROOM_STATUS.PLAYING)
		{
			if (!room.isBreakInto)
			{
				return false;
			}
			if (room.MaxPlayer == room.CurPlayer)
			{
				return false;
			}
		}
		return true;
	}

	private void DoCtf(Room room, float changed_y)
	{
		LabelUtil.TextOut(new Vector2(optionLX, changed_y), StringMgr.Instance.Get("ROOM_STATUS"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		GUI.Box(new Rect(optionRX - crdBox.x * 0.5f, changed_y - crdBox.y * 0.5f, crdBox.x, crdBox.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(optionRX, changed_y), room.score1.ToString() + "/" + room.score2.ToString(), "MiniLabel", new Color(0.91f, 0.6f, 0f, 1f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		changed_y += diff_y;
		ctfConfig.crdOptionLT.y = changed_y;
		ctfConfig.isRoom = false;
		ctfConfig.DoOption(room);
	}

	private void DoTeamMatch(Room room, float changed_y)
	{
		LabelUtil.TextOut(new Vector2(optionLX, changed_y), StringMgr.Instance.Get("ROOM_STATUS"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		GUI.Box(new Rect(optionRX - crdBox.x * 0.5f, changed_y - crdBox.y * 0.5f, crdBox.x, crdBox.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(optionRX, changed_y), room.score1.ToString() + "/" + room.score2.ToString() + "(" + StringMgr.Instance.Get("KILL") + ")", "MiniLabel", new Color(0.91f, 0.6f, 0f, 1f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		changed_y += diff_y;
		tmConfig.crdOptionLT.y = changed_y;
		tmConfig.isRoom = false;
		tmConfig.DoOption(room);
	}

	private void DoDeathMatch(Room room, float changed_y)
	{
		LabelUtil.TextOut(new Vector2(optionLX, changed_y), StringMgr.Instance.Get("ROOM_STATUS"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		GUI.Box(new Rect(optionRX - crdBox.x * 0.5f, changed_y - crdBox.y * 0.5f, crdBox.x, crdBox.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(optionRX, changed_y), room.score1.ToString() + "(" + StringMgr.Instance.Get("KILL") + ")", "MiniLabel", new Color(0.91f, 0.6f, 0f, 1f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		changed_y += diff_y;
		imConfig.crdOptionLT.y = changed_y;
		imConfig.isRoom = false;
		imConfig.DoOption(room);
	}

	private void DoExplosion(Room room, float changed_y)
	{
		LabelUtil.TextOut(new Vector2(optionLX, changed_y), StringMgr.Instance.Get("ROOM_STATUS"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		GUI.Box(new Rect(optionRX - crdBox.x * 0.5f, changed_y - crdBox.y * 0.5f, crdBox.x, crdBox.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(optionRX, changed_y), room.score1.ToString() + "/" + room.score2.ToString(), "MiniLabel", new Color(0.91f, 0.6f, 0f, 1f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		changed_y += diff_y;
		bmConfig.crdOptionLT.y = changed_y;
		bmConfig.isRoom = false;
		bmConfig.DoOption(room);
	}

	private void DoMission(Room room, float changed_y)
	{
		dmConfig.crdOptionLT.y = changed_y;
		dmConfig.isRoom = false;
		dmConfig.DoOption(room);
	}

	private void DoBnd(Room room, float changed_y)
	{
		LabelUtil.TextOut(new Vector2(optionLX, changed_y), StringMgr.Instance.Get("ROOM_STATUS"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		GUI.Box(new Rect(optionRX - crdBox.x * 0.5f, changed_y - crdBox.y * 0.5f, crdBox.x, crdBox.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(optionRX, changed_y), room.score1.ToString() + "/" + room.score2.ToString() + "(" + StringMgr.Instance.Get("KILL") + ")", "MiniLabel", new Color(0.91f, 0.6f, 0f, 1f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		changed_y += diff_y;
		bndConfig.crdOptionLT.y = changed_y;
		bndConfig.isRoom = false;
		bndConfig.DoOption(room);
	}

	private void DoBungee(Room room, float changed_y)
	{
		LabelUtil.TextOut(new Vector2(optionLX, changed_y), StringMgr.Instance.Get("ROOM_STATUS"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		GUI.Box(new Rect(optionRX - crdBox.x * 0.5f, changed_y - crdBox.y * 0.5f, crdBox.x, crdBox.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(optionRX, changed_y), room.score1.ToString() + "(" + StringMgr.Instance.Get("KILL") + ")", "MiniLabel", new Color(0.91f, 0.6f, 0f, 1f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		changed_y += diff_y;
		bgConfig.crdOptionLT.y = changed_y;
		bgConfig.isRoom = false;
		bgConfig.DoOption(room);
	}

	private void DoEscape(Room room, float changed_y)
	{
		LabelUtil.TextOut(new Vector2(optionLX, changed_y), StringMgr.Instance.Get("ROOM_STATUS"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		GUI.Box(new Rect(optionRX - crdBox.x * 0.5f, changed_y - crdBox.y * 0.5f, crdBox.x, crdBox.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(optionRX, changed_y), room.score1.ToString() + "(" + StringMgr.Instance.Get("TIMES_UNIT") + ")", "MiniLabel", new Color(0.91f, 0.6f, 0f, 1f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		changed_y += diff_y;
		escConfig.crdOptionLT.y = changed_y;
		escConfig.isRoom = false;
		escConfig.DoOption(room);
	}

	private void DoZombie(Room room, float changed_y)
	{
		LabelUtil.TextOut(new Vector2(optionLX, changed_y), StringMgr.Instance.Get("ROOM_STATUS"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		GUI.Box(new Rect(optionRX - crdBox.x * 0.5f, changed_y - crdBox.y * 0.5f, crdBox.x, crdBox.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(optionRX, changed_y), room.score1.ToString() + "/" + room.score2.ToString(), "MiniLabel", new Color(0.91f, 0.6f, 0f, 1f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		changed_y += diff_y;
		zombieConfig.crdOptionLT.y = changed_y;
		zombieConfig.isRoom = false;
		zombieConfig.DoOption(room);
	}

	private void OnGUI()
	{
		GlobalVars.Instance.BeginGUI(VersionTextureManager.Instance.seasonTexture.texScreenBg);
		GUI.depth = (int)guiDepth;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		GUI.enabled = !DialogManager.Instance.IsModal;
		switch (curLobbyType)
		{
		case LOBBY_TYPE.BASE:
			OnGUI_Lobby();
			break;
		case LOBBY_TYPE.SHOP:
			OnGUI_ItemShop();
			break;
		case LOBBY_TYPE.EQUIP:
			OnGUI_MyEquip();
			break;
		case LOBBY_TYPE.MAP:
			OnGUI_MapManager();
			break;
		case LOBBY_TYPE.ROOMS:
			OnGUI_GameRooms();
			break;
		}
		GUI.enabled = true;
		GlobalVars.Instance.EndGUI();
	}

	private void OnChat(ChatText chatText)
	{
		lobbyChat.Enqueue(chatText);
	}

	private void OnKillLog(KillInfo log)
	{
	}

	public void GoToShop()
	{
		SetPage(LOBBY_TYPE.SHOP);
	}

	public void OpenShopTree(string treeName)
	{
		SetPage(LOBBY_TYPE.SHOP);
		if (!shopFrm.findTree(treeName))
		{
			Debug.LogError("not found. name is " + treeName);
		}
	}

	public void DirectBuyItem(string code)
	{
		Good good = ShopManager.Instance.Get(code);
		if (good == null)
		{
			Debug.LogError("not found code: " + code);
		}
		else
		{
			((BuyTermDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.BUY_TERM, exclusive: true))?.InitDialog(good);
		}
	}

	private bool IsStartAfterPlay()
	{
		return startFrame + 2 < Time.frameCount;
	}

	private void DrawMedal(int roomId, float ux, float uy)
	{
		Room room = RoomManager.Instance.GetRoom(roomId);
		if (room != null)
		{
			RegMap regMap = RegMapManager.Instance.Get(room.map);
			if (regMap != null && regMap.Thumbnail != null)
			{
				if ((regMap.tagMask & 8) != 0)
				{
					TextureUtil.DrawTexture(new Rect(ux, uy, (float)(GlobalVars.Instance.iconglory.width / 2), (float)(GlobalVars.Instance.iconglory.height / 2)), GlobalVars.Instance.iconglory, ScaleMode.StretchToFill);
				}
				else if ((regMap.tagMask & 4) != 0)
				{
					TextureUtil.DrawTexture(new Rect(ux, uy, (float)(GlobalVars.Instance.iconMedal.width / 2), (float)(GlobalVars.Instance.iconMedal.height / 2)), GlobalVars.Instance.iconMedal, ScaleMode.StretchToFill);
				}
				else if ((regMap.tagMask & 2) != 0)
				{
					TextureUtil.DrawTexture(new Rect(ux, uy, (float)(GlobalVars.Instance.icongoldRibbon.width / 2), (float)(GlobalVars.Instance.icongoldRibbon.height / 2)), GlobalVars.Instance.icongoldRibbon, ScaleMode.StretchToFill);
				}
			}
		}
	}

	private bool DoRoomDetail()
	{
		if (ChannelManager.Instance.CurChannel.Mode != 3 && overRoomNo >= 0)
		{
			GUI.Box(crdBlueBox, string.Empty, "BoxBase");
			GUI.Box(crdBlueBox, string.Empty, "BoxBase");
			Color clrText = new Color(0.91f, 0.6f, 0f, 1f);
			Room room = RoomManager.Instance.GetRoom(overRoomNo);
			if (room != null)
			{
				RegMap regMap = RegMapManager.Instance.Get(room.map);
				if (regMap != null && regMap.Thumbnail != null)
				{
					TextureUtil.DrawTexture(crdMapThumbnail, regMap.Thumbnail, ScaleMode.StretchToFill);
					DateTime registeredDate = regMap.RegisteredDate;
					if (registeredDate.Year == DateTime.Today.Year && registeredDate.Month == DateTime.Today.Month && registeredDate.Day == DateTime.Today.Day)
					{
						TextureUtil.DrawTexture(new Rect(crdMapThumbnail.x, crdMapThumbnail.y, (float)GlobalVars.Instance.iconNewmap.width, (float)GlobalVars.Instance.iconNewmap.height), GlobalVars.Instance.iconNewmap, ScaleMode.StretchToFill);
					}
					else if ((regMap.tagMask & 8) != 0)
					{
						TextureUtil.DrawTexture(new Rect(crdMapThumbnail.x, crdMapThumbnail.y, (float)GlobalVars.Instance.iconglory.width, (float)GlobalVars.Instance.iconglory.height), GlobalVars.Instance.iconglory, ScaleMode.StretchToFill);
					}
					else if ((regMap.tagMask & 4) != 0)
					{
						TextureUtil.DrawTexture(new Rect(crdMapThumbnail.x, crdMapThumbnail.y, (float)GlobalVars.Instance.iconMedal.width, (float)GlobalVars.Instance.iconMedal.height), GlobalVars.Instance.iconMedal, ScaleMode.StretchToFill);
					}
					else if ((regMap.tagMask & 2) != 0)
					{
						TextureUtil.DrawTexture(new Rect(crdMapThumbnail.x, crdMapThumbnail.y, (float)GlobalVars.Instance.icongoldRibbon.width, (float)GlobalVars.Instance.icongoldRibbon.height), GlobalVars.Instance.icongoldRibbon, ScaleMode.StretchToFill);
					}
					if (regMap.IsAbuseMap())
					{
						float x = crdMapThumbnail.x + crdMapThumbnail.width - (float)GlobalVars.Instance.iconDeclare.width;
						TextureUtil.DrawTexture(new Rect(x, crdMapThumbnail.y, (float)GlobalVars.Instance.iconDeclare.width, (float)GlobalVars.Instance.iconDeclare.height), GlobalVars.Instance.iconDeclare, ScaleMode.StretchToFill);
					}
					LabelUtil.TextOut(crdRoomName, (room.No + 1).ToString() + ". " + room.Title, "MidLabel", Color.white, GlobalVars.txtEmptyColor, textAnchor, crdBlueBox.width - 50f);
					LabelUtil.TextOut(crdMapName, room.CurMapAlias, "MidLabel", clrText, GlobalVars.txtEmptyColor, textAnchor, crdBlueBox.width - 50f);
					TextureUtil.DrawTexture(crdThumbUp, GlobalVars.Instance.iconThumbUp, ScaleMode.StretchToFill);
					LabelUtil.TextOut(new Vector2(crdThumbUp.x + 30f, crdThumbUp.y), regMap.Likes.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
					TextureUtil.DrawTexture(crdThumbDn, GlobalVars.Instance.iconThumbDn, ScaleMode.StretchToFill);
					LabelUtil.TextOut(new Vector2(crdThumbDn.x + 30f, crdThumbDn.y), regMap.DisLikes.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
					if (regMap.IsAbuseMap())
					{
						LabelUtil.TextOut(new Vector2(crdAbuse.x, crdAbuse.y), StringMgr.Instance.Get("REPORT_GM_MAP_REASON_PRINT_01"), "MiniLabel", Color.red, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft, 250f);
					}
					float num = optionY;
					LabelUtil.TextOut(new Vector2(optionLX, num), StringMgr.Instance.Get("ROOM_TYPE"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
					GUI.Box(new Rect(optionRX - crdBox.x * 0.5f, num - crdBox.y * 0.5f, crdBox.x, crdBox.y), string.Empty, "BoxTextBg");
					LabelUtil.TextOut(new Vector2(optionRX, num), room.GetString(Room.COLUMN.TYPE), "MiniLabel", clrText, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
					num += diff_y;
					if (room.Type != Room.ROOM_TYPE.ZOMBIE)
					{
						LabelUtil.TextOut(new Vector2(optionLX, num), StringMgr.Instance.Get("WEAPON_OPTION"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
						GUI.Box(new Rect(optionRX - crdBox.x * 0.5f, num - crdBox.y * 0.5f, crdBox.x, crdBox.y), string.Empty, "BoxTextBg");
						string text = (room.weaponOption >= 0 && room.weaponOption < weaponOptions.Length) ? StringMgr.Instance.Get(weaponOptions[room.weaponOption]) : string.Empty;
						LabelUtil.TextOut(new Vector2(optionRX, num), text, "MiniLabel", clrText, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
						num += diff_y;
					}
					LabelUtil.TextOut(new Vector2(optionLX, num), StringMgr.Instance.Get("NUM_PLAYERS"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
					GUI.Box(new Rect(optionRX - crdBox.x * 0.5f, num - crdBox.y * 0.5f, crdBox.x, crdBox.y), string.Empty, "BoxTextBg");
					LabelUtil.TextOut(new Vector2(optionRX, num), room.GetString(Room.COLUMN.NUM_PLAYER), "MiniLabel", clrText, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
					num += diff_y;
					switch (room.Type)
					{
					case Room.ROOM_TYPE.TEAM_MATCH:
						DoTeamMatch(room, num);
						break;
					case Room.ROOM_TYPE.INDIVIDUAL:
						DoDeathMatch(room, num);
						break;
					case Room.ROOM_TYPE.CAPTURE_THE_FLAG:
						DoCtf(room, num);
						break;
					case Room.ROOM_TYPE.EXPLOSION:
						DoExplosion(room, num);
						break;
					case Room.ROOM_TYPE.BND:
						DoBnd(room, num);
						break;
					case Room.ROOM_TYPE.MISSION:
						DoMission(room, num);
						break;
					case Room.ROOM_TYPE.BUNGEE:
						DoBungee(room, num);
						break;
					case Room.ROOM_TYPE.ESCAPE:
						DoEscape(room, num);
						break;
					case Room.ROOM_TYPE.ZOMBIE:
						DoZombie(room, num);
						break;
					}
				}
			}
		}
		return false;
	}

	private void RelaseCurStep()
	{
		if (curLobbyType == LOBBY_TYPE.SHOP)
		{
			if (!DialogManager.Instance.IsPopup(DialogManager.DIALOG_INDEX.BUY_TERM) && !DialogManager.Instance.IsPopup(DialogManager.DIALOG_INDEX.SEND_MEMO) && !DialogManager.Instance.IsPopup(DialogManager.DIALOG_INDEX.BUY_CONFIRM) && !DialogManager.Instance.IsPopup(DialogManager.DIALOG_INDEX.PRESENT_CONFIRM) && !DialogManager.Instance.IsPopup(DialogManager.DIALOG_INDEX.MBUY_TERM))
			{
				if (shopFrm.ActiveRelateItem)
				{
					ShopManager.Instance.InitAllGoods();
					shopFrm.ActiveRelateItem = false;
				}
				else
				{
					shopFrm.RollbackPreview();
					shopFrm.CurItem = 0;
				}
			}
		}
		else if (curLobbyType == LOBBY_TYPE.EQUIP && !MessageBoxMgr.Instance.HasMsg())
		{
			equipmentFrm.CurItem = 0;
		}
	}

	public bool GotoStartScene()
	{
		RelaseCurStep();
		return true;
	}

	private void SendCS_JOIN_REQ()
	{
		if (GlobalVars.Instance.clanSendJoinREQ == -1)
		{
			GlobalVars.Instance.clanSendJoinREQ = 0;
			GlobalVars.Instance.ENTER_SQUADING_ACK();
		}
	}

	private bool UpdateJoinRoom()
	{
		if (GlobalVars.Instance.clanSendJoinREQ == 2)
		{
			GlobalVars.Instance.clanSendJoinREQ = -1;
			return true;
		}
		return false;
	}
}
