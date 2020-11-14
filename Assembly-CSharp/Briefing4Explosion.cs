using UnityEngine;

public class Briefing4Explosion : MonoBehaviour
{
	public Texture2D texPopupBg;

	public LobbyTools lobbyTools;

	public LobbyChat lobbyChat;

	public Mirror mirror;

	public BlastModeConfig bmConfig;

	public PlayerList4TeamFrame playerListFrm;

	public Equipment equipmentFrm;

	public Shop shopFrm;

	public Messenger messenger;

	public BriefingPanel4TeamMatch briefingPanel;

	public ChannelLabel channelLabel;

	private float deltaTime;

	private Rect crdCloseBtn = new Rect(977f, 5f, 34f, 34f);

	private Rect crdLine = new Rect(220f, 488f, 827f, 12f);

	private Vector2 crdBigTitle = new Vector2(260f, 15f);

	private Rect crdVLine = new Rect(247f, 43f, 1f, 685f);

	private Rect crdMyEquipBtn = new Rect(820f, 606f, 192f, 56f);

	private Rect crdShopBtn = new Rect(820f, 665f, 192f, 56f);

	private Rect crdMessengerBtn = new Rect(840f, 726f, 172f, 34f);

	private Rect crdMessengerBtnStatus = new Rect(820f, 730f, 20f, 22f);

	private int guiStep;

	private bool isMessenger;

	private bool bChatView = true;

	private void Start()
	{
		MyInfoManager.Instance.DeResultEvent();
		deltaTime = 0f;
		guiStep = 0;
		isMessenger = false;
		bChatView = false;
		GlobalVars.Instance.ApplyAudioSource();
		BrickManager.Instance.MakeSystemMapInstance(BrickManager.SYSTEM_MAP.WAITING);
		lobbyTools.Start();
		briefingPanel.Start();
		lobbyChat.Start();
		lobbyChat.SetChatStyle(LOBBYCHAT_STYLE.MIDDLE);
		mirror.Start();
		equipmentFrm.Start();
		shopFrm.Start();
		bmConfig.Start();
		messenger.Start();
		BrickManManager.Instance.OnStart();
		playerListFrm.Start();
		mirror.MirrorType = MIRROR_TYPE.SIMPLE;
		channelLabel.Start();
		if (RoomManager.Instance.Master == MyInfoManager.Instance.Seq)
		{
			CSNetManager.Instance.Sock.SendCS_RESUME_ROOM_REQ(0);
		}
	}

	private void DrawCurrentChannel()
	{
		Channel curChannel = ChannelManager.Instance.CurChannel;
		if (curChannel != null)
		{
			LabelUtil.TextOut(new Vector2(400f, 17f), curChannel.Name, "MiniLabel", new Color(0.91f, 0.6f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
	}

	private void OnGUI()
	{
		GlobalVars.Instance.BeginGUI(VersionTextureManager.Instance.seasonTexture.texScreenBg);
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		GUI.enabled = !DialogManager.Instance.IsModal;
		TextureUtil.DrawTexture(GlobalVars.Instance.UIScreenRect, texPopupBg, ScaleMode.StretchToFill);
		if (guiStep == 0)
		{
			GUI.Box(crdVLine, string.Empty, "DivideLineV");
			if (GlobalVars.Instance.MyButton(crdCloseBtn, string.Empty, "BtnClose") || (!GlobalVars.Instance.IsModalAll() && GlobalVars.Instance.IsEscapePressed()))
			{
				Squad curSquad = SquadManager.Instance.CurSquad;
				if (curSquad != null)
				{
					if (SquadManager.Instance.CurSquad.Leader == MyInfoManager.Instance.Seq)
					{
						CSNetManager.Instance.Sock.SendCS_CLAN_MATCH_TEAM_GETBACK_REQ(MyInfoManager.Instance.ClanSeq, curSquad.Index);
					}
					else
					{
						P2PManager.Instance.Shutdown();
						CSNetManager.Instance.Sock.SendCS_LEAVE_REQ();
						CSNetManager.Instance.Sock.SendCS_LEAVE_SQUAD_REQ();
						SquadManager.Instance.Leave();
						CSNetManager.Instance.Sock.SendCS_LEAVE_SQUADING_REQ();
						SquadManager.Instance.Clear();
						GlobalVars.Instance.GotoLobbyRoomList = true;
						Application.LoadLevel("Lobby");
					}
				}
				else
				{
					P2PManager.Instance.Shutdown();
					CSNetManager.Instance.Sock.SendCS_LEAVE_REQ();
					GlobalVars.Instance.GotoLobbyRoomList = true;
					Application.LoadLevel("Lobby");
				}
			}
			channelLabel.OnGUI();
			mirror.OnGUI();
			bmConfig.OnGUI();
			lobbyChat.OnGUI();
			playerListFrm.OnGUI();
			briefingPanel.OnGUI();
			GUI.Box(crdLine, string.Empty, "DivideLine");
			GUIContent content = new GUIContent(StringMgr.Instance.Get("MY_EQUIP").ToUpper(), GlobalVars.Instance.iconMyItem);
			if (GlobalVars.Instance.MyButton3(crdMyEquipBtn, content, "BtnAction"))
			{
				if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master || MyInfoManager.Instance.Status != 1)
				{
					guiStep = 1;
					equipmentFrm.Default();
					CSNetManager.Instance.Sock.SendCS_SET_STATUS_REQ(6);
				}
				else
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CANT_ON_READY"));
				}
			}
			content = new GUIContent(StringMgr.Instance.Get("SHOPPING").ToUpper(), GlobalVars.Instance.iconCart);
			if (GlobalVars.Instance.MyButton3(crdShopBtn, content, "BtnAction"))
			{
				if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master || MyInfoManager.Instance.Status != 1)
				{
					guiStep = 2;
					CSNetManager.Instance.Sock.SendCS_SET_STATUS_REQ(5);
					shopFrm.InitPreview();
				}
				else
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CANT_ON_READY"));
				}
			}
			GUI.Box(crdMessengerBtnStatus, string.Empty, (!isMessenger) ? "IconArrowL" : "IconArrowR");
			if (GlobalVars.Instance.MyButtonBold(crdMessengerBtn, StringMgr.Instance.Get("MESSENGER").ToUpper(), "BtnAction"))
			{
				isMessenger = !isMessenger;
				messenger.IsBriefing = isMessenger;
				lobbyChat.BtnActive(!isMessenger);
				messenger.ToggleLeftTop();
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
			if (GUI.enabled)
			{
				GUI.enabled = !bChatView;
			}
			Color byteColor2FloatColor = GlobalVars.Instance.GetByteColor2FloatColor(244, 151, 25);
			TextureUtil.DrawTexture(GlobalVars.Instance.UIScreenRect, texPopupBg, ScaleMode.StretchToFill);
			GUI.Box(crdVLine, string.Empty, "DivideLineV");
			LabelUtil.TextOut(crdBigTitle, StringMgr.Instance.Get("MY_EQUIP").ToUpper(), "BigBtnLabel", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			if (GlobalVars.Instance.MyButton(crdCloseBtn, string.Empty, "BtnClose") || (!GlobalVars.Instance.IsModalAll() && GlobalVars.Instance.IsEscapePressed()))
			{
				guiStep = 0;
				playerListFrm.ResetMyPlayerStyle();
				CSNetManager.Instance.Sock.SendCS_SET_STATUS_REQ(0);
			}
			mirror.OnGUI();
			equipmentFrm.OnGUI();
			if (bChatView)
			{
				bool enabled2 = GUI.enabled;
				GUI.enabled = true;
				lobbyChat.OnGUI();
				messenger.OnGUI();
				GUI.enabled = enabled2;
			}
			GUI.enabled = enabled;
			equipmentFrm.DoFilterCombo();
		}
		else if (guiStep == 2)
		{
			bool enabled3 = GUI.enabled;
			if (GUI.enabled)
			{
				GUI.enabled = !shopFrm.CheckFilterCombo();
			}
			if (GUI.enabled)
			{
				GUI.enabled = !bChatView;
			}
			Color byteColor2FloatColor2 = GlobalVars.Instance.GetByteColor2FloatColor(244, 151, 25);
			TextureUtil.DrawTexture(GlobalVars.Instance.UIScreenRect, texPopupBg, ScaleMode.StretchToFill);
			GUI.Box(crdVLine, string.Empty, "DivideLineV");
			LabelUtil.TextOut(crdBigTitle, StringMgr.Instance.Get("SHOPPING").ToUpper(), "BigBtnLabel", byteColor2FloatColor2, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			if (!shopFrm.GetBuyConfirm() && (GlobalVars.Instance.MyButton(crdCloseBtn, string.Empty, "BtnClose") || (!GlobalVars.Instance.IsModalAll() && GlobalVars.Instance.IsEscapePressed())))
			{
				guiStep = 0;
				playerListFrm.ResetMyPlayerStyle();
				shopFrm.RollbackPreview();
				CSNetManager.Instance.Sock.SendCS_SET_STATUS_REQ(0);
			}
			mirror.OnGUI();
			shopFrm.OnGUI();
			if (bChatView)
			{
				bool enabled4 = GUI.enabled;
				GUI.enabled = true;
				lobbyChat.OnGUI();
				messenger.OnGUI();
				GUI.enabled = enabled4;
			}
			GUI.enabled = enabled3;
			shopFrm.DoFilterCombo();
		}
		if (guiStep > 0 && GlobalVars.Instance.MyButton(new Rect(921f, 7f, 44f, 44f), new GUIContent(string.Empty, StringMgr.Instance.Get("CHATTING")), "BtnChatOnOff"))
		{
			bChatView = !bChatView;
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
		deltaTime += Time.deltaTime;
		if (deltaTime > 0.5f)
		{
			deltaTime = 0f;
			Squad curSquad = SquadManager.Instance.CurSquad;
			if (curSquad != null && MyInfoManager.Instance.ClanSeq < 0)
			{
				OnClanExiled();
			}
		}
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
}
