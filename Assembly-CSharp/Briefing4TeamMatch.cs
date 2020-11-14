using UnityEngine;

public class Briefing4TeamMatch : MonoBehaviour
{
	public Texture2D texPopupBg;

	public Texture2D unreadBg;

	public LobbyTools lobbyTools;

	public LobbyChat lobbyChat;

	public Mirror mirror;

	public BndConfig bndConfig;

	public CtfModeConfig ctfConfig;

	public DefenseModeConfig dmConfig;

	public BlastModeConfig bmConfig;

	public IndividualMatchConfig imConfig;

	public TeamMatchConfig tmConfig;

	public BungeeModeConfig bgConfig;

	public EscapeModeConfig escConfig;

	public ZombieModeConfig zombieConfig;

	public PlayerList4IndividualFrame playerListFrmIndividual;

	public PlayerList4DefenseFrame playerListFrmDefense;

	public PlayerList4BungeeFrame playerListFrmBungee;

	public PlayerList4TeamFrame playerListFrm;

	public Equipment equipmentFrm;

	public Shop shopFrm;

	public Messenger messenger;

	public BriefingPanel4Individual briefingPanelIndividual;

	public BriefingPanel4Defense briefingPanelDefense;

	public BriefingPanel4TeamMatch briefingPanel;

	public ChannelLabel channelLabel;

	private float deltaTime;

	private Rect crdLine = new Rect(220f, 488f, 827f, 12f);

	private Rect crdVLine = new Rect(247f, 43f, 1f, 685f);

	private Rect crdMessengerBtn = new Rect(840f, 726f, 172f, 34f);

	private Rect crdMessengerBtnStatus = new Rect(820f, 730f, 20f, 22f);

	private Rect crdMainGrid = new Rect(426f, 9f, 420f, 46f);

	private Vector2 crdMatchingProgress = new Vector2(400f, 160f);

	private int guiStep;

	public int clanMatchBackStep;

	private bool isMessenger;

	public bool bChatView = true;

	private int dotCount;

	private Vector2 crdMatchingWarn = new Vector2(200f, 82f);

	private Vector2 crdMatching = new Vector2(200f, 52f);

	private Vector2 crdMatchingTitle = new Vector2(12f, 8f);

	private Rect crdSearchingCancel = new Rect(110f, 120f, 180f, 34f);

	private int selected;

	private float allReadyTimer = 15f;

	private int lastCount = 16;

	public bool IsMessenger
	{
		set
		{
			isMessenger = value;
			messengerReset();
		}
	}

	public bool GotoLobby()
	{
		if (guiStep == 0)
		{
			if (isMessenger)
			{
				isMessenger = false;
				messengerReset();
				return false;
			}
			return true;
		}
		ReleaseGuiStep();
		return false;
	}

	private void Start()
	{
		MyInfoManager.Instance.DeResultEvent();
		deltaTime = 0f;
		guiStep = 0;
		isMessenger = false;
		bChatView = true;
		clanMatchBackStep = 0;
		GlobalVars.Instance.ApplyAudioSource();
		BrickManager.Instance.MakeSystemMapInstance(BrickManager.SYSTEM_MAP.WAITING);
		briefingPanelIndividual.Start();
		briefingPanelDefense.Start();
		briefingPanel.Start();
		lobbyChat.Start();
		lobbyChat.SetChatStyle(LOBBYCHAT_STYLE.LOW);
		mirror.Start();
		equipmentFrm.Start();
		shopFrm.Start();
		messenger.Start();
		channelLabel.Start();
		bndConfig.Start();
		ctfConfig.Start();
		dmConfig.Start();
		bmConfig.Start();
		imConfig.Start();
		tmConfig.Start();
		bgConfig.Start();
		BrickManManager.Instance.OnStart();
		playerListFrmIndividual.Start();
		playerListFrmDefense.Start();
		playerListFrm.Start();
		playerListFrmBungee.Start();
		mirror.MirrorType = MIRROR_TYPE.SIMPLE;
		if (RoomManager.Instance.Master == MyInfoManager.Instance.Seq)
		{
			CSNetManager.Instance.Sock.SendCS_RESUME_ROOM_REQ(0);
		}
		GameObject gameObject = GameObject.Find("TeamFlag");
		if (null != gameObject)
		{
			if (RoomManager.Instance.IsKindOfIndividual)
			{
				SkinnedMeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
				{
					skinnedMeshRenderer.enabled = false;
				}
			}
			else
			{
				SkinnedMeshRenderer[] componentsInChildren2 = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
				foreach (SkinnedMeshRenderer skinnedMeshRenderer2 in componentsInChildren2)
				{
					skinnedMeshRenderer2.enabled = true;
				}
			}
		}
		DialogManager.Instance.CloseAll();
		Object.Instantiate((Object)VersionTextureManager.Instance.seasonTexture.objPreviewBg);
	}

	private void DrawCurrentChannel()
	{
		Channel curChannel = ChannelManager.Instance.CurChannel;
		if (curChannel != null)
		{
			LabelUtil.TextOut(new Vector2(400f, 17f), curChannel.Name, "MiniLabel", new Color(0.91f, 0.6f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
	}

	private void messengerReset()
	{
		messenger.IsBriefing = isMessenger;
		lobbyChat.BtnActive(!isMessenger);
		messenger.ToggleLeftTop();
	}

	private void MatchingProgress(int id)
	{
		string text = StringMgr.Instance.Get("CLAN_MATCH_SEARCING");
		for (int i = 0; i < dotCount; i++)
		{
			text += ".";
		}
		LabelUtil.TextOut(crdMatchingTitle, StringMgr.Instance.Get("CLAN_MATCH_SEARCH"), "Label", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMatchingWarn, StringMgr.Instance.Get("WARNING_MSG_CLAN_MATCH"), "Label", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.MiddleCenter, 360f);
		LabelUtil.TextOut(crdMatching, text, "Label", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.MiddleCenter);
		Squad curSquad = SquadManager.Instance.CurSquad;
		if (curSquad != null && SquadManager.Instance.CurSquad.Leader == MyInfoManager.Instance.Seq && GlobalVars.Instance.MyButton(crdSearchingCancel, StringMgr.Instance.Get("CLAN_MATCH_CANCEL"), "BtnAction"))
		{
			CSNetManager.Instance.Sock.SendCS_MATCH_TEAM_CANCEL_REQ();
		}
	}

	private void OnGUI()
	{
		GlobalVars.Instance.BeginGUI(VersionTextureManager.Instance.seasonTexture.texScreenBg);
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		GUI.enabled = !DialogManager.Instance.IsModal;
		GUI.depth = 50;
		TextureUtil.DrawTexture(GlobalVars.Instance.UIScreenRect, texPopupBg, ScaleMode.StretchToFill);
		lobbyTools.OnGUI();
		string[] array = new string[3]
		{
			StringMgr.Instance.Get("GAME_ROOMS_SUBTAB"),
			StringMgr.Instance.Get("ITEM_SHOPPING_MAINTAB"),
			StringMgr.Instance.Get("MY_ITEMS_MAINTAB")
		};
		crdMainGrid.width = (float)(array.Length * 105);
		int num = selected;
		selected = GUI.SelectionGrid(crdMainGrid, selected, array, array.Length, "BtnMain");
		if (num != selected && selected == 0)
		{
			ReleaseGuiStep();
		}
		else if (num == 1 && selected == 2)
		{
			shopFrm.RollbackPreview();
		}
		if (num != selected && selected == 2)
		{
			if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master || MyInfoManager.Instance.Status != 1)
			{
				guiStep = 1;
				equipmentFrm.Default();
				bChatView = false;
				lobbyChat.hideCloseButton(close: false);
				lobbyChat.SetChatStyle(LOBBYCHAT_STYLE.LOW);
				CSNetManager.Instance.Sock.SendCS_SET_STATUS_REQ(6);
			}
			else
			{
				selected = num;
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CANT_ON_READY"));
			}
		}
		if (num != selected && selected == 1)
		{
			if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master || MyInfoManager.Instance.Status != 1)
			{
				guiStep = 2;
				bChatView = false;
				lobbyChat.hideCloseButton(close: false);
				lobbyChat.SetChatStyle(LOBBYCHAT_STYLE.LOW);
				CSNetManager.Instance.Sock.SendCS_SET_STATUS_REQ(5);
				shopFrm.InitPreview();
			}
			else
			{
				selected = num;
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CANT_ON_READY"));
			}
		}
		if (SquadManager.Instance.IsMatching)
		{
			Rect clientRect = new Rect(((float)Screen.width - crdMatchingProgress.x) / 2f, ((float)Screen.height - crdMatchingProgress.y) / 2f, crdMatchingProgress.x, crdMatchingProgress.y);
			GUI.Window(1024, clientRect, MatchingProgress, string.Empty);
		}
		if (guiStep == 0)
		{
			GUI.Box(crdVLine, string.Empty, "DivideLineV");
			channelLabel.OnGUI();
			mirror.OnGUI();
			switch (RoomManager.Instance.CurrentRoomType)
			{
			case Room.ROOM_TYPE.TEAM_MATCH:
				tmConfig.OnGUI();
				break;
			case Room.ROOM_TYPE.INDIVIDUAL:
				imConfig.OnGUI();
				break;
			case Room.ROOM_TYPE.MISSION:
				dmConfig.OnGUI();
				break;
			case Room.ROOM_TYPE.CAPTURE_THE_FLAG:
				ctfConfig.OnGUI();
				break;
			case Room.ROOM_TYPE.EXPLOSION:
				bmConfig.OnGUI();
				break;
			case Room.ROOM_TYPE.BND:
				bndConfig.OnGUI();
				break;
			case Room.ROOM_TYPE.BUNGEE:
				bgConfig.OnGUI();
				break;
			case Room.ROOM_TYPE.ESCAPE:
				escConfig.OnGUI();
				break;
			case Room.ROOM_TYPE.ZOMBIE:
				zombieConfig.OnGUI();
				break;
			default:
				Debug.LogError("not support mode: " + RoomManager.Instance.CurrentRoomType);
				break;
			}
			lobbyChat.OnGUI();
			if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.INDIVIDUAL || RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.ESCAPE || RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.ZOMBIE)
			{
				playerListFrmIndividual.OnGUI();
			}
			else if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.MISSION)
			{
				playerListFrmDefense.OnGUI();
			}
			else if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BUNGEE)
			{
				playerListFrmBungee.OnGUI();
			}
			else
			{
				playerListFrm.OnGUI();
			}
			if (RoomManager.Instance.IsKindOfIndividual)
			{
				briefingPanelIndividual.OnGUI();
			}
			else if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.MISSION)
			{
				briefingPanelDefense.OnGUI();
			}
			else
			{
				briefingPanel.OnGUI();
			}
			GUI.Box(crdLine, string.Empty, "DivideLine");
			GUI.Box(crdMessengerBtnStatus, string.Empty, (!isMessenger) ? "IconArrowL" : "IconArrowR");
			if (GlobalVars.Instance.MyButtonBold(crdMessengerBtn, StringMgr.Instance.Get("MESSENGER").ToUpper(), "BtnAction"))
			{
				isMessenger = !isMessenger;
				messengerReset();
			}
			if (isMessenger)
			{
				messenger.OnGUI();
			}
		}
		else if (guiStep == 1)
		{
			bool enabled = GUI.enabled;
			if (GUI.enabled)
			{
				GUI.enabled = !equipmentFrm.CheckFilterCombo();
			}
			GUI.Box(crdVLine, string.Empty, "DivideLineV");
			mirror.OnGUI();
			equipmentFrm.OnGUI();
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
				}
			}
			else
			{
				Rect rc2 = new Rect(559f, 472f, 150f, 16f);
				if (GlobalVars.Instance.MyButton(rc2, string.Empty, "BtnDropDown"))
				{
					bChatView = false;
				}
			}
			GUI.enabled = enabled;
			equipmentFrm.DoFilterCombo();
		}
		else if (guiStep == 2)
		{
			bool enabled2 = GUI.enabled;
			if (GUI.enabled)
			{
				GUI.enabled = !shopFrm.CheckFilterCombo();
			}
			GUI.Box(crdVLine, string.Empty, "DivideLineV");
			mirror.OnGUI();
			shopFrm.OnGUI();
			if (bChatView)
			{
				lobbyChat.OnGUI();
				messenger.OnGUI();
			}
			if (!bChatView)
			{
				Rect rc3 = new Rect(559f, 750f, 150f, 16f);
				if (GlobalVars.Instance.MyButton(rc3, string.Empty, "BtnDropUp"))
				{
					bChatView = true;
				}
			}
			else
			{
				Rect rc4 = new Rect(559f, 472f, 150f, 16f);
				if (GlobalVars.Instance.MyButton(rc4, string.Empty, "BtnDropDown"))
				{
					bChatView = false;
				}
			}
			GUI.enabled = enabled2;
			shopFrm.DoFilterCombo();
		}
		GUI.enabled = true;
		GlobalVars.Instance.EndGUI();
	}

	private void Update()
	{
		lobbyChat.Update();
		messenger.Update();
		mirror.Update();
		lobbyTools.Update();
		channelLabel.Update();
		equipmentFrm.Update();
		shopFrm.Update();
		briefingPanel.Update();
		briefingPanelIndividual.Update();
		briefingPanelDefense.Update();
		deltaTime += Time.deltaTime;
		if (deltaTime > 0.5f)
		{
			deltaTime = 0f;
			dotCount++;
			if (dotCount > 3)
			{
				dotCount = 0;
			}
		}
		DoMasterKick();
		FindMatchTeamMember();
	}

	private void OnClanExiled()
	{
		Squad curSquad = SquadManager.Instance.CurSquad;
		if (curSquad == null)
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("JUST_EXILED_FROM_CLAN"));
		}
		else if (!Application.isLoadingLevel)
		{
			DialogManager.Instance.CloseAll();
			ContextMenuManager.Instance.CloseAll();
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("JUST_EXILED_FROM_CLAN"));
			P2PManager.Instance.Shutdown();
			CSNetManager.Instance.Sock.SendCS_LEAVE_REQ();
			CSNetManager.Instance.Sock.SendCS_LEAVE_SQUAD_REQ();
			SquadManager.Instance.Leave();
			CSNetManager.Instance.Sock.SendCS_LEAVE_SQUADING_REQ();
			SquadManager.Instance.Clear();
			Application.LoadLevel("Lobby");
		}
	}

	private void End()
	{
	}

	private void OnDisable()
	{
	}

	private void OnKillLog(KillInfo log)
	{
	}

	private void OnChat(ChatText chatText)
	{
		lobbyChat.Enqueue(chatText);
	}

	private void OnNoticeCenter(string text)
	{
		SystemInform.Instance.AddMessageCenter(text);
	}

	private void DoMasterKick()
	{
		if (IsAllReady())
		{
			int num = Mathf.FloorToInt(allReadyTimer);
			if (num >= 0)
			{
				if (num != lastCount && num <= 10)
				{
					CSNetManager.Instance.Sock.SendCS_MASTER_KICKING_REQ(num);
					lastCount = num;
				}
				allReadyTimer -= Time.deltaTime;
				if (lastCount <= 0 && RoomManager.Instance.Master == MyInfoManager.Instance.Seq)
				{
					P2PManager.Instance.Shutdown();
					CSNetManager.Instance.Sock.SendCS_LEAVE_REQ();
					GlobalVars.Instance.GotoLobbyRoomList = true;
					Application.LoadLevel("Lobby");
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("ROOM_MASTER_KICKED_MESSAGE"));
				}
			}
		}
		else
		{
			allReadyTimer = 15f;
			lastCount = 16;
		}
	}

	private bool IsAllReady()
	{
		if (RoomManager.Instance.CurrentRoomStatus != 0)
		{
			return false;
		}
		if (MyInfoManager.Instance.Seq != RoomManager.Instance.Master)
		{
			return false;
		}
		Squad curSquad = SquadManager.Instance.CurSquad;
		if (curSquad != null)
		{
			return false;
		}
		int num = 0;
		BrickManDesc[] array = BrickManManager.Instance.ToDescriptorArrayBySlot();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				num++;
				if (array[i].Status != 1 && array[i].Seq != RoomManager.Instance.Master)
				{
					return false;
				}
			}
			else if (MyInfoManager.Instance.Slot == i)
			{
				num++;
			}
		}
		if (num < 6)
		{
			return false;
		}
		return true;
	}

	public void ReleaseGuiStep()
	{
		if (guiStep == 1)
		{
			bChatView = true;
			guiStep = 0;
			lobbyChat.hideCloseButton(close: true);
			playerListFrm.ResetMyPlayerStyle();
			lobbyChat.SetChatStyle(LOBBYCHAT_STYLE.LOW);
			CSNetManager.Instance.Sock.SendCS_SET_STATUS_REQ(0);
			selected = 0;
		}
		else if (guiStep == 2 && !shopFrm.GetBuyConfirm())
		{
			bChatView = true;
			guiStep = 0;
			lobbyChat.hideCloseButton(close: true);
			playerListFrm.ResetMyPlayerStyle();
			shopFrm.RollbackPreview();
			lobbyChat.SetChatStyle(LOBBYCHAT_STYLE.LOW);
			CSNetManager.Instance.Sock.SendCS_SET_STATUS_REQ(0);
			selected = 0;
		}
	}

	private bool FindMatchTeamMember()
	{
		if (MyInfoManager.Instance.Seq != RoomManager.Instance.Master)
		{
			return false;
		}
		if (ChannelManager.Instance.CurChannel.Mode != 4)
		{
			return false;
		}
		BrickManDesc[] array = BrickManManager.Instance.ToDescriptorArray();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsHostile())
			{
				return true;
			}
		}
		if (GlobalVars.Instance.clanTeamMatchSuccess == 1)
		{
			GlobalVars.Instance.clanTeamMatchSuccess = -1;
		}
		return false;
	}
}
