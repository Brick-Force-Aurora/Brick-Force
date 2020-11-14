using System;
using UnityEngine;

[Serializable]
public class LobbyTools
{
	public enum TAB
	{
		ROOM_LIST,
		INVENTORY,
		SHOP,
		MY_MAPS,
		SEARCH_MAPS,
		RANK_MAPS,
		NUM
	}

	public Color clrFadeIn = Color.white;

	public Color clrFadeOut = new Color(1f, 1f, 1f, 0f);

	private Color clrFade = Color.white;

	private Color clrFadeReverse = new Color(1f, 1f, 1f, 0f);

	public float fadeInTime = 3f;

	public float fadeOutTime = 1f;

	private bool fadeIn;

	private float deltaTime;

	public Texture2D memo;

	public Texture2D treasureChest;

	public Texture2D dailyMission;

	public Texture2D longTimePlay;

	public Texture2D unreadBg;

	private float deltaChange;

	private float deltaChangeMax = 3f;

	private bool viewUnread = true;

	private Rect crdFrame = new Rect(0f, 5f, 1024f, 53f);

	private Rect crdBackBtn = new Rect(6f, 9f, 64f, 44f);

	private Vector2 crdLT = new Vector2(84f, 8f);

	private Vector2 crdBtnSize = new Vector2(36f, 36f);

	private float crdBtnOffset = 12f;

	private Vector2 overallSize = Vector2.zero;

	private TAB currentTab;

	public float WidthToolbar => overallSize.x;

	public TAB CurrentTab => currentTab;

	public void Start()
	{
	}

	public void Update()
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
		clrFadeReverse.a = 1f - clrFade.a;
	}

	private bool VerifyBriefingRoom()
	{
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			Briefing4TeamMatch component = gameObject.GetComponent<Briefing4TeamMatch>();
			if (null != component)
			{
				return component.GotoLobby();
			}
		}
		return false;
	}

	private bool VerifyLobby()
	{
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			Lobby component = gameObject.GetComponent<Lobby>();
			if (null != component)
			{
				return component.GotoStartScene();
			}
		}
		return false;
	}

	private void DrawToolButtonHelpText(Rect rc, string helpText)
	{
		if (BuildOption.Instance.IsNetmarble || BuildOption.Instance.IsDeveloper || BuildOption.Instance.IsInfernum || BuildOption.Instance.IsTester)
		{
			LabelUtil.TextOut(new Vector2(rc.x + rc.width / 2f, crdFrame.y + crdFrame.height), helpText, "MiniLabel", GlobalVars.Instance.GetByteColor2FloatColor(50, 191, 17), GlobalVars.txtEmptyColor, TextAnchor.LowerCenter);
		}
	}

	public void OnGUI()
	{
		bool flag = false;
		if (Application.loadedLevelName.Contains("Briefing"))
		{
			flag = true;
		}
		GUI.Box(crdFrame, string.Empty, "BoxBase");
		if (GlobalVars.Instance.MyButton(crdBackBtn, new GUIContent(string.Empty, StringMgr.Instance.Get("BACK")), "BtnBack") || (!GlobalVars.Instance.IsModalAll() && GlobalVars.Instance.IsEscapePressed()) || GlobalVars.Instance.callRoomList)
		{
			GlobalVars.Instance.callRoomList = false;
			if (flag)
			{
				if (!VerifyBriefingRoom())
				{
					return;
				}
				GlobalVars.Instance.clanTeamMatchSuccess = -1;
				Squad curSquad = SquadManager.Instance.CurSquad;
				if (curSquad != null)
				{
					if (SquadManager.Instance.CurSquad.Leader == MyInfoManager.Instance.Seq)
					{
						CSNetManager.Instance.Sock.SendCS_CLAN_MATCH_TEAM_GETBACK_REQ(MyInfoManager.Instance.ClanSeq, curSquad.Index);
					}
					P2PManager.Instance.Shutdown();
					CSNetManager.Instance.Sock.SendCS_LEAVE_REQ();
					CSNetManager.Instance.Sock.SendCS_LEAVE_SQUAD_REQ();
					SquadManager.Instance.Leave();
					CSNetManager.Instance.Sock.SendCS_LEAVE_SQUADING_REQ();
					SquadManager.Instance.Clear();
					GlobalVars.Instance.LobbyType = LOBBY_TYPE.ROOMS;
					GlobalVars.Instance.GotoLobbyRoomList = true;
					Application.LoadLevel("Lobby");
				}
				else
				{
					P2PManager.Instance.Shutdown();
					CSNetManager.Instance.Sock.SendCS_LEAVE_REQ();
					GlobalVars.Instance.LobbyType = LOBBY_TYPE.ROOMS;
					GlobalVars.Instance.GotoLobbyRoomList = true;
					Application.LoadLevel("Lobby");
				}
			}
			else
			{
				if (!VerifyLobby())
				{
					return;
				}
				ContextMenuManager.Instance.CloseAll();
				Application.LoadLevel("BfStart");
			}
		}
		Vector2 pos = new Vector2(crdBackBtn.x + crdBackBtn.width / 2f, crdBackBtn.y);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("BACK"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		bool flag2 = Application.loadedLevelName.Contains("Lobby");
		Rect rect = new Rect(crdLT.x, crdLT.y, crdBtnSize.x, crdBtnSize.y);
		if (GlobalVars.Instance.MyButton(rect, new GUIContent(string.Empty, StringMgr.Instance.Get("FRIEND")), "BtnFriends"))
		{
			((FriendsDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.FRIENDS, exclusive: true))?.InitDialog();
		}
		DrawToolButtonHelpText(rect, StringMgr.Instance.Get("UPPER_TOOLBAR_ICON_01"));
		rect.x += crdBtnSize.x + crdBtnOffset;
		if (GlobalVars.Instance.MyButton(rect, new GUIContent(string.Empty, StringMgr.Instance.Get("CLAN")), "BtnClan"))
		{
			GlobalVars.Instance.PlaySoundButtonClick();
			((ClanDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.CLAN, exclusive: true))?.InitDialog();
		}
		if (MyInfoManager.Instance.ClanApplicant > 0)
		{
			Rect position = new Rect(rect.x + 18f, rect.y + 19f, (float)unreadBg.width, (float)unreadBg.height);
			TextureUtil.DrawTexture(position, unreadBg, ScaleMode.StretchToFill);
			LabelUtil.TextOut(new Vector2(position.x + 12f, position.y + 11f), MyInfoManager.Instance.ClanApplicant.ToString(), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
		DrawToolButtonHelpText(rect, StringMgr.Instance.Get("UPPER_TOOLBAR_ICON_02"));
		rect.x += crdBtnSize.x + crdBtnOffset;
		if (GlobalVars.Instance.MyButton(rect, new GUIContent(string.Empty, StringMgr.Instance.Get("MEMO")), "BtnMemo"))
		{
			((MemoDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.MEMO, exclusive: true))?.InitDialog();
		}
		if (MemoManager.Instance.HaveUnreadMemo() && MemoManager.Instance.GetMemoCountPercent() >= 80)
		{
			Color color = GUI.color;
			GUI.color = clrFade;
			TextureUtil.DrawTexture(rect, memo, ScaleMode.StretchToFill);
			GUI.color = color;
			Rect position2 = new Rect(rect.x + 18f, rect.y + 19f, (float)unreadBg.width, (float)unreadBg.height);
			TextureUtil.DrawTexture(position2, unreadBg, ScaleMode.StretchToFill);
			deltaChange += Time.deltaTime;
			if (deltaChange > deltaChangeMax)
			{
				deltaChange = 0f;
				viewUnread = !viewUnread;
			}
			if (!viewUnread)
			{
				LabelUtil.TextOut(new Vector2(position2.x + 12f, position2.y + 11f), MemoManager.Instance.GetMemoCountPercent().ToString() + "%", "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
			else
			{
				LabelUtil.TextOut(new Vector2(position2.x + 12f, position2.y + 11f), MemoManager.Instance.GetUnreadMemoCount().ToString(), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
		}
		else if (MemoManager.Instance.HaveUnreadMemo())
		{
			Color color2 = GUI.color;
			GUI.color = clrFade;
			TextureUtil.DrawTexture(rect, memo, ScaleMode.StretchToFill);
			GUI.color = color2;
			Rect position3 = new Rect(rect.x + 18f, rect.y + 19f, (float)unreadBg.width, (float)unreadBg.height);
			TextureUtil.DrawTexture(position3, unreadBg, ScaleMode.StretchToFill);
			LabelUtil.TextOut(new Vector2(position3.x + 12f, position3.y + 11f), MemoManager.Instance.GetUnreadMemoCount().ToString(), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
		else if (MemoManager.Instance.GetMemoCountPercent() >= 80)
		{
			Color color3 = GUI.color;
			GUI.color = clrFade;
			TextureUtil.DrawTexture(rect, memo, ScaleMode.StretchToFill);
			GUI.color = color3;
			Rect position4 = new Rect(rect.x + 18f, rect.y + 19f, (float)unreadBg.width, (float)unreadBg.height);
			TextureUtil.DrawTexture(position4, unreadBg, ScaleMode.StretchToFill);
			LabelUtil.TextOut(new Vector2(position4.x + 12f, position4.y + 11f), MemoManager.Instance.GetMemoCountPercent().ToString() + "%", "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
		DrawToolButtonHelpText(rect, StringMgr.Instance.Get("UPPER_TOOLBAR_ICON_03"));
		rect.x += crdBtnSize.x + crdBtnOffset;
		if (flag2 && BuildOption.Instance.Props.randomBox != BuildOption.RANDOM_BOX_TYPE.NOT_USE)
		{
			if (GlobalVars.Instance.MyButton(rect, new GUIContent(string.Empty, StringMgr.Instance.Get("TREASURE_CHEST")), "BtnRandom"))
			{
				CSNetManager.Instance.Sock.SendCS_TC_OPEN_REQ();
			}
			DrawToolButtonHelpText(rect, StringMgr.Instance.Get("UPPER_TOOLBAR_ICON_05"));
			rect.x += crdBtnSize.x + crdBtnOffset;
		}
		if (GlobalVars.Instance.MyButton(rect, new GUIContent(string.Empty, StringMgr.Instance.Get("DAILY_MISSION")), "BtnMission"))
		{
			((MissionDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.MISSION, exclusive: true))?.InitDialog();
		}
		if (MissionManager.Instance.CanReceiveMission)
		{
			Color color4 = GUI.color;
			GUI.color = clrFade;
			TextureUtil.DrawTexture(rect, dailyMission, ScaleMode.StretchToFill);
			GUI.color = color4;
		}
		DrawToolButtonHelpText(rect, StringMgr.Instance.Get("UPPER_TOOLBAR_ICON_06"));
		rect.x += crdBtnSize.x + crdBtnOffset;
		if (ChannelManager.Instance.CurChannel.Mode != 4 && flag2 && MyInfoManager.Instance.UseLongTimePlayReward)
		{
			if (GlobalVars.Instance.MyButton(rect, new GUIContent(string.Empty, StringMgr.Instance.Get("PLAY_REWARD_NORMAL01")), "BtnBenefit"))
			{
				((LongTimePlayDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.LONG_TIME_PLAY, exclusive: true))?.InitDialog(isGameExit: false);
			}
			if (MyInfoManager.Instance.IsLongTimePlayActive)
			{
				Color color5 = GUI.color;
				GUI.color = clrFadeReverse;
				TextureUtil.DrawTexture(rect, longTimePlay, ScaleMode.StretchToFill);
				GUI.color = color5;
			}
			LongTimePlayDialog longTimePlayDialog = (LongTimePlayDialog)DialogManager.Instance.GetDialogAlways(DialogManager.DIALOG_INDEX.LONG_TIME_PLAY);
			if (longTimePlayDialog != null)
			{
				int remainMinuteUntilReward = longTimePlayDialog.RemainMinuteUntilReward;
				if (remainMinuteUntilReward > 0)
				{
					string text = remainMinuteUntilReward.ToString();
					Rect position5 = new Rect(rect.x + 18f, rect.y + 19f, (float)unreadBg.width, (float)unreadBg.height);
					TextureUtil.DrawTexture(position5, unreadBg, ScaleMode.StretchToFill);
					LabelUtil.TextOut(new Vector2(position5.x + 12f, position5.y + 11f), text, "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				}
			}
			DrawToolButtonHelpText(rect, StringMgr.Instance.Get("UPPER_TOOLBAR_ICON_07"));
			rect.x += crdBtnSize.x + crdBtnOffset;
		}
		if (!flag)
		{
			if (GlobalVars.Instance.MyButton(rect, new GUIContent(string.Empty, StringMgr.Instance.Get("QUICK_JOIN_BTN")), "BtnQuickJoin"))
			{
				((QuickJoinDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.QUICKJOIN, exclusive: false))?.InitDialog();
			}
			DrawToolButtonHelpText(rect, StringMgr.Instance.Get("QUICK_JOIN_BTN"));
		}
		rect.x = (float)((!flag) ? 954 : 1018);
		rect.x -= 2f * (crdBtnSize.x + crdBtnOffset);
		if (BuildOption.Instance.Props.UseAccuse && BuildOption.Instance.Props.UseAccuseToolButton)
		{
			if (GlobalVars.Instance.MyButton(rect, new GUIContent(string.Empty, StringMgr.Instance.Get("DO_ACCUSE_BTN")), "BtnDeclare"))
			{
				((AccusationDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.ACCUSATION, exclusive: true))?.InitDialog();
			}
			DrawToolButtonHelpText(rect, StringMgr.Instance.Get("DO_ACCUSE_BTN"));
		}
		rect.x += crdBtnSize.x + crdBtnOffset;
		if (GlobalVars.Instance.MyButton(rect, new GUIContent(string.Empty, StringMgr.Instance.Get("CHANGE_SETTING")), "BtnSetting"))
		{
			((SettingDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.SETTING, exclusive: true))?.InitDialog();
		}
		DrawToolButtonHelpText(rect, StringMgr.Instance.Get("UPPER_TOOLBAR_ICON_04"));
		rect.x += crdBtnSize.x + crdBtnOffset;
		overallSize = new Vector2(rect.x, rect.height);
		SystemInform.Instance.SetToolbarSize(overallSize.x);
	}
}
