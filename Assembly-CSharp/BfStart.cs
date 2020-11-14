using UnityEngine;

public class BfStart : MonoBehaviour
{
	private string tooltipMessage = string.Empty;

	public bool flyingToTutorial;

	private Rect crdLogo = new Rect(854f, 15f, 170f, 60f);

	private Vector2 crdVer = new Vector2(1024f, 0f);

	private Rect crdBackBtn = new Rect(6f, 9f, 64f, 44f);

	private Rect crdSettingBtn = new Rect(78f, 9f, 36f, 36f);

	private Rect crdPlay = new Rect(77f, 88f, 422f, 314f);

	private Rect crdPlayBox = new Rect(137f, 287f, 300f, 90f);

	private Vector2 crdPlayLabel = new Vector2(291f, 330f);

	private Rect crdBuild = new Rect(524f, 88f, 422f, 314f);

	private Rect crdBuildBox = new Rect(590f, 287f, 300f, 90f);

	private Vector2 crdBuildLabel = new Vector2(741f, 330f);

	private Rect crdBattleTutor = new Rect(54f, 428f, 284f, 98f);

	private Rect crdBattleTutorBox = new Rect(84f, 430f, 220f, 90f);

	private Vector2 crdBattleTutorLabel = new Vector2(194f, 477f);

	private Rect crdBuildTutor = new Rect(54f, 531f, 284f, 98f);

	private Rect crdBuildTutorBox = new Rect(84f, 533f, 220f, 90f);

	private Vector2 crdBuildTutorLabel = new Vector2(194f, 580f);

	private Rect crdBeginner = new Rect(54f, 635f, 284f, 98f);

	private Rect crdBeginnerBox = new Rect(84f, 637f, 220f, 90f);

	private Vector2 crdBeginnerLabel = new Vector2(194f, 684f);

	private Rect crdShop = new Rect(370f, 428f, 284f, 306f);

	private Rect crdShopBox = new Rect(400f, 435f, 220f, 90f);

	private Vector2 crdShopLabel = new Vector2(511f, 479f);

	private Rect crdClan = new Rect(684f, 428f, 284f, 306f);

	private Rect crdClanBox = new Rect(720f, 435f, 220f, 90f);

	private Vector2 crdClanLabel = new Vector2(830f, 479f);

	private void Awake()
	{
	}

	private void Start()
	{
		GlobalVars.Instance.ApplyAudioSource();
		flyingToTutorial = false;
		if (BuildOption.Instance.Props.tutorialAlways && MyInfoManager.Instance.BattleTutorialable && !MyInfoManager.Instance.OnceTutorialAlways)
		{
			Channel tutorialableChannel = ChannelManager.Instance.GetTutorialableChannel();
			if (tutorialableChannel != null)
			{
				MyInfoManager.Instance.OnceTutorialAlways = true;
				flyingToTutorial = true;
				Compass.Instance.SetDestination(Compass.DESTINATION_LEVEL.BATTLE_TUTOR, tutorialableChannel.Id);
			}
		}
	}

	private void ShowTooltip(int id)
	{
		LabelUtil.TextOut(new Vector2(10f, 10f), tooltipMessage, "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
	}

	private void _ExitConfirmDialog()
	{
		if (MyInfoManager.Instance.IsLongTimePlayActive)
		{
			((LongTimePlayDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.LONG_TIME_PLAY, exclusive: true))?.InitDialog(isGameExit: true);
		}
		else
		{
			((ExitConfirmDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.EXIT_CONFIRM, exclusive: true))?.InitDialog();
		}
	}

	private void DrawBackBtnText(string btnTxt)
	{
		Vector2 pos = new Vector2(crdBackBtn.x + crdBackBtn.width / 2f, crdBackBtn.y);
		LabelUtil.TextOut(pos, btnTxt, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
	}

	private void DrawToolButtonHelpText(Rect rc, string helpText)
	{
		if (BuildOption.Instance.IsNetmarble || BuildOption.Instance.IsDeveloper || BuildOption.Instance.IsInfernum || BuildOption.Instance.IsTester)
		{
			LabelUtil.TextOut(new Vector2(rc.x + rc.width / 2f, 58f), helpText, "MiniLabel", GlobalVars.Instance.GetByteColor2FloatColor(50, 191, 17), GlobalVars.txtEmptyColor, TextAnchor.LowerCenter);
		}
	}

	private void OnGUI()
	{
		if (!flyingToTutorial)
		{
			if (BuffManager.Instance.IsPCBangBuff() && !BuffManager.Instance.isPcBangShowDialog)
			{
				BuffManager.Instance.isPcBangShowDialog = true;
				((PCBangDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.PC_BANG_NOTICE, exclusive: false))?.InitDialog();
			}
			GlobalVars.Instance.BeginGUI(VersionTextureManager.Instance.seasonTexture.texScreenBg);
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.enabled = !DialogManager.Instance.IsModal;
			Color byteColor2FloatColor = GlobalVars.Instance.GetByteColor2FloatColor(244, 151, 25);
			string text = "BrickForce ver " + BuildOption.Instance.Major.ToString() + "." + BuildOption.Instance.Minor.ToString() + " " + BuildOption.Instance.Props.Alias + " Release: " + BuildOption.Instance.Build.ToString();
			LabelUtil.TextOut(crdVer, text, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			Texture2D logo = BuildOption.Instance.Props.logo;
			if (null != logo)
			{
				TextureUtil.DrawTexture(crdLogo, logo, ScaleMode.StretchToFill);
			}
			if (MyInfoManager.Instance.IsAutoLogin)
			{
				if (GlobalVars.Instance.MyButton(crdBackBtn, new GUIContent(string.Empty, StringMgr.Instance.Get("QUIT_GAME")), "BtnBack2"))
				{
					_ExitConfirmDialog();
				}
				DrawBackBtnText(StringMgr.Instance.Get("QUIT_GAME"));
			}
			else
			{
				if (GlobalVars.Instance.MyButton(crdBackBtn, new GUIContent(string.Empty, StringMgr.Instance.Get("BACK")), "BtnBack") || (!GlobalVars.Instance.IsModalAll() && GlobalVars.Instance.IsEscapePressed()))
				{
					_ExitConfirmDialog();
				}
				DrawBackBtnText(StringMgr.Instance.Get("BACK"));
			}
			if (GlobalVars.Instance.MyButton(crdSettingBtn, new GUIContent(string.Empty, StringMgr.Instance.Get("CHANGE_SETTING")), "BtnSetting"))
			{
				((SettingDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.SETTING, exclusive: true))?.InitDialog();
			}
			DrawToolButtonHelpText(crdSettingBtn, StringMgr.Instance.Get("UPPER_TOOLBAR_ICON_04"));
			if (GlobalVars.Instance.MyButton(crdPlay, string.Empty, "StartPlay" + BuildOption.Instance.Props.GetSeasonCount()))
			{
				if (MyInfoManager.Instance.Tutorialed < 1)
				{
					DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.TUTOR_Q_POPUP2, exclusive: true);
				}
				else if (!Application.CanStreamedLevelBeLoaded("Lobby"))
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("STREAMING_WAIT"));
				}
				else
				{
					Channel bestPlayChannel = ChannelManager.Instance.GetBestPlayChannel();
					if (bestPlayChannel == null)
					{
						if (ChannelManager.Instance.IsLastError())
						{
							MessageBoxMgr.Instance.AddMessage(ChannelManager.Instance.GetBestChannelLastError());
						}
						else
						{
							MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("SERVICE_CROWDED"));
						}
					}
					else
					{
						Compass.Instance.SetDestination(Compass.DESTINATION_LEVEL.LOBBY, bestPlayChannel.Id);
					}
				}
			}
			GUI.Box(crdPlayBox, string.Empty, "BoxMain");
			LabelUtil.TextOut(crdPlayLabel, StringMgr.Instance.Get("PLAY"), "BigBtnLabel", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			if (GlobalVars.Instance.MyButton(crdBuild, string.Empty, "StartBuild" + BuildOption.Instance.Props.GetSeasonCount()))
			{
				if (MyInfoManager.Instance.Tutorialed < 2)
				{
					DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.TUTOR_Q_POPUP, exclusive: true);
				}
				else if (!Application.CanStreamedLevelBeLoaded("Lobby"))
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("STREAMING_WAIT"));
				}
				else
				{
					Channel bestBuildChannel = ChannelManager.Instance.GetBestBuildChannel();
					if (bestBuildChannel == null)
					{
						if (ChannelManager.Instance.IsLastError())
						{
							MessageBoxMgr.Instance.AddMessage(ChannelManager.Instance.GetBestChannelLastError());
						}
						else
						{
							MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("SERVICE_CROWDED"));
						}
					}
					else
					{
						Compass.Instance.SetDestination(Compass.DESTINATION_LEVEL.LOBBY, bestBuildChannel.Id);
					}
				}
			}
			GUI.Box(crdBuildBox, string.Empty, "BoxMain");
			LabelUtil.TextOut(crdBuildLabel, StringMgr.Instance.Get("BUILD"), "BigBtnLabel", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			if (MyInfoManager.Instance.Tutorialed == 1 || MyInfoManager.Instance.Tutorialed == 3)
			{
				TextureUtil.DrawTexture(crdBattleTutor, VersionTextureManager.Instance.seasonTexture.texDoneTutorial, ScaleMode.StretchToFill);
			}
			else if (GlobalVars.Instance.MyButton(crdBattleTutor, string.Empty, "StartTutor" + BuildOption.Instance.Props.GetSeasonCount()))
			{
				GlobalVars.Instance.isLoadBattleTutor = true;
				GlobalVars.Instance.tutorFirstScriptOn = true;
				if (!Application.CanStreamedLevelBeLoaded("Lobby") || !Application.CanStreamedLevelBeLoaded("BattleTutor"))
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("STREAMING_WAIT"));
				}
				else
				{
					Channel tutorialableChannel = ChannelManager.Instance.GetTutorialableChannel();
					if (tutorialableChannel == null)
					{
						if (ChannelManager.Instance.IsLastError())
						{
							MessageBoxMgr.Instance.AddMessage(ChannelManager.Instance.GetBestChannelLastError());
						}
						else
						{
							MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("SERVICE_CROWDED"));
						}
					}
					else
					{
						Compass.Instance.SetDestination(Compass.DESTINATION_LEVEL.BATTLE_TUTOR, tutorialableChannel.Id);
					}
				}
			}
			if (MyInfoManager.Instance.Tutorialed == 2 || MyInfoManager.Instance.Tutorialed == 3)
			{
				TextureUtil.DrawTexture(crdBuildTutor, VersionTextureManager.Instance.seasonTexture.texDoneTutorial, ScaleMode.StretchToFill);
			}
			else if (GlobalVars.Instance.MyButton(crdBuildTutor, string.Empty, "StartTutor" + BuildOption.Instance.Props.GetSeasonCount()))
			{
				GlobalVars.Instance.tutorFirstScriptOn = true;
				GlobalVars.Instance.isLoadBattleTutor = false;
				GlobalVars.Instance.blockDelBrick = true;
				if (!Application.CanStreamedLevelBeLoaded("Lobby") || !Application.CanStreamedLevelBeLoaded("BattleTutor"))
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("STREAMING_WAIT"));
				}
				else
				{
					Channel tutorialableChannel2 = ChannelManager.Instance.GetTutorialableChannel();
					if (tutorialableChannel2 == null)
					{
						if (ChannelManager.Instance.IsLastError())
						{
							MessageBoxMgr.Instance.AddMessage(ChannelManager.Instance.GetBestChannelLastError());
						}
						else
						{
							MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("SERVICE_CROWDED"));
						}
					}
					else
					{
						Compass.Instance.SetDestination(Compass.DESTINATION_LEVEL.BATTLE_TUTOR, tutorialableChannel2.Id);
					}
				}
			}
			GUIStyle style = GUI.skin.GetStyle("MiniBtnLabel");
			int fontSize = style.fontSize;
			style.fontSize = 20;
			GUI.Box(crdBattleTutorBox, string.Empty, "BoxMain");
			LabelUtil.TextOut(crdBattleTutorLabel, StringMgr.Instance.Get("BTN_BATTLTUTO"), "MiniBtnLabel", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			GUI.Box(crdBuildTutorBox, string.Empty, "BoxMain");
			LabelUtil.TextOut(crdBuildTutorLabel, StringMgr.Instance.Get("BTN_MAPTUTO"), "MiniBtnLabel", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			if (GlobalVars.Instance.MyButton(crdBeginner, string.Empty, "StartBeginner" + BuildOption.Instance.Props.GetSeasonCount()))
			{
				ChannelManager.Instance.PrevScene = Application.loadedLevelName;
				Application.LoadLevel("ChangeChannel");
			}
			GUI.Box(crdBeginnerBox, string.Empty, "BoxMain");
			LabelUtil.TextOut(crdBeginnerLabel, StringMgr.Instance.Get("SERVER_LIST"), "MiniBtnLabel", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			style.fontSize = fontSize;
			if (GlobalVars.Instance.MyButton(crdShop, string.Empty, "StartShop" + BuildOption.Instance.Props.GetSeasonCount()))
			{
				if (!Application.CanStreamedLevelBeLoaded("Lobby"))
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("STREAMING_WAIT"));
				}
				else
				{
					Channel tutorialableChannel3 = ChannelManager.Instance.GetTutorialableChannel();
					if (tutorialableChannel3 == null)
					{
						if (ChannelManager.Instance.IsLastError())
						{
							MessageBoxMgr.Instance.AddMessage(ChannelManager.Instance.GetBestChannelLastError());
						}
						else
						{
							MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("SERVICE_CROWDED"));
						}
					}
					else
					{
						GlobalVars.Instance.LobbyType = LOBBY_TYPE.SHOP;
						Compass.Instance.SetDestination(Compass.DESTINATION_LEVEL.LOBBY, tutorialableChannel3.Id);
					}
				}
			}
			GUI.Box(crdShopBox, string.Empty, "BoxMain");
			LabelUtil.TextOut(crdShopLabel, StringMgr.Instance.Get("SHOP"), "MiniBtnLabel", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			if (GlobalVars.Instance.MyButton(crdClan, string.Empty, "StartClan" + BuildOption.Instance.Props.GetSeasonCount()))
			{
				if (!Application.CanStreamedLevelBeLoaded("Lobby"))
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("STREAMING_WAIT"));
				}
				else
				{
					Channel bestClanChannel = ChannelManager.Instance.GetBestClanChannel();
					if (bestClanChannel == null)
					{
						if (ChannelManager.Instance.IsLastError())
						{
							MessageBoxMgr.Instance.AddMessage(ChannelManager.Instance.GetBestChannelLastError());
						}
						else
						{
							MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("SERVICE_CROWDED"));
						}
					}
					else
					{
						Compass.Instance.SetDestination(Compass.DESTINATION_LEVEL.LOBBY, bestClanChannel.Id);
					}
				}
			}
			GUI.Box(crdClanBox, string.Empty, "BoxMain");
			LabelUtil.TextOut(crdClanLabel, StringMgr.Instance.Get("CLAN"), "MiniBtnLabel", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			if (Event.current.type == EventType.Repaint && GUI.tooltip.Length > 0)
			{
				tooltipMessage = GUI.tooltip;
				Vector2 vector = GlobalVars.Instance.ToGUIPoint(Event.current.mousePosition);
				GUIStyle style2 = GUI.skin.GetStyle("MiniLabel");
				if (style2 != null)
				{
					Vector2 vector2 = style2.CalcSize(new GUIContent(tooltipMessage));
					Rect rc = new Rect(vector.x, vector.y, vector2.x + 20f, vector2.y + 20f);
					GlobalVars.Instance.FitRightNBottomRectInScreen(ref rc);
					GUI.Window(1102, rc, ShowTooltip, string.Empty, "LineWindow");
				}
			}
			GUI.enabled = true;
			GlobalVars.Instance.EndGUI();
		}
	}

	private void Update()
	{
	}

	private void OnNoticeCenter(string text)
	{
		SystemInform.Instance.AddMessageCenter(text);
	}
}
