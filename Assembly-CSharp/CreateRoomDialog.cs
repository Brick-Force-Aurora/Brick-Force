using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CreateRoomDialog : Dialog
{
	private const int create = 0;

	private const int load = 1;

	public const int USE_ESCAPE_DONOT_ATTACK = 3;

	public int maxRoomTitle = 16;

	public int maxRoomPswd = 4;

	public int maxMapName = 10;

	public Texture2D[] landscapes;

	public Texture2D[] skyboxes;

	public int[] tex2LandscapeIndex = new int[12]
	{
		9,
		10,
		11,
		0,
		1,
		2,
		3,
		4,
		5,
		6,
		7,
		8
	};

	public int[] tex2SkyboxIndex = new int[10]
	{
		7,
		8,
		9,
		0,
		1,
		2,
		3,
		4,
		5,
		6
	};

	public Texture2D premiumIcon;

	public Texture2D slotLock;

	public Texture2D slotEmpty;

	public Texture2D nonAvailable;

	public Texture2D emptySlot;

	public Texture2D selectedMapFrame;

	public Texture2D homeIcon;

	public Texture2D lockedIcon;

	public Texture2D pointIcon;

	public Texture2D mapIcon;

	public Texture2D iconKey;

	public string[] modeOptKeys;

	public string[] mapEditModeKeys;

	public string[] weaponOptKeys;

	public int[] killCounts;

	public int[] timeLimits;

	public int[] buildPhaseTimes;

	public int[] battlePhaseTimes;

	public int[] repeats;

	public string[] pointOptions;

	public int[] points;

	public string[] roundOptions;

	public int[] rounds;

	public float doubleClickTimeout = 0.3f;

	public int[] minMaxPlayers;

	public int[] maxMaxPlayers;

	private int clanMinPlayers = 8;

	private int clanMaxPlayers = 16;

	public string[] waveKeys;

	public int[] waveTimeLimits;

	private string[] waveOptions;

	private int wave;

	private int waveTimeLimit;

	public int[] bungeeTimes;

	public int[] arrivalCounts;

	private string roomTitle = string.Empty;

	private string roomPswd = string.Empty;

	private Vector2 optionScrollPosition = Vector2.zero;

	private int option;

	private int maxPlayers = 16;

	private string[] BattleModes;

	private Room.ROOM_TYPE[] roomTypes;

	private bool updateOnce;

	private UserMapInfo[] umi;

	private string newMapName = string.Empty;

	private Vector2 skyboxScrollPosition = Vector2.zero;

	private int skybox;

	private Vector2 landscapeScrollPosition = Vector2.zero;

	private int landscape;

	private Vector2 umiScrollPosition = Vector2.zero;

	private float lastClickTime;

	private int killCount;

	private int timeLimit;

	private int weaponOpt;

	private int point;

	private int round;

	private int arrivalCount = 2;

	private bool useWeaponOption = true;

	private string[] weaponOptions;

	private string[] killCountOptions;

	private string[] timeLimitOptions;

	private string[] buildPhaseTimeOptions;

	private string[] battlePhaseTimeOptions;

	private string[] repeatOptions;

	private string[] waveTimeLimitOptions;

	private string[] bungeeTimeLimitOptions;

	private string[] arrivalCountOptions;

	private bool breakInto = true;

	private bool teamBalance;

	private bool itemPickup;

	private bool wanted;

	private RegMap[] reg;

	private Vector2 regMapScrollPosition = Vector2.zero;

	private int regMap;

	private Vector2 crdBigTitle = new Vector2(403f, 35f);

	private Vector2 crdRoomTitleL = new Vector2(15f, 30f);

	private Rect crdRoomTitle = new Rect(15f, 40f, 210f, 26f);

	private Vector2 crdPwTitleL = new Vector2(15f, 82f);

	private Rect crdPswd = new Rect(15f, 93f, 210f, 26f);

	private Rect crdIconKey = new Rect(198f, 102f, 18f, 9f);

	private Rect crdHLine = new Rect(10f, 141f, 220f, 1f);

	private Rect crdVLine = new Rect(240f, 4f, 1f, 760f);

	private Rect crdHLineMapDiv = new Rect(240f, 306f, 784f, 1f);

	private Rect crdButtonOk = new Rect(570f, 720f, 178f, 34f);

	private Rect crdUserMapRect = new Rect(264f, 72f, 500f, 202f);

	private Rect crdRegMapRect = new Rect(264f, 72f, 500f, 634f);

	private Vector2 crdMapSize = new Vector2(150f, 196f);

	private Vector2 crdMapOffset = new Vector2(15f, 15f);

	private Vector2 crdDeveloper = new Vector2(5f, 157f);

	private Vector2 crdAlias = new Vector2(5f, 174f);

	private Rect crdNewMapNamePoint = new Rect(265f, 316f, 20f, 22f);

	private Vector2 crdNewMapName = new Vector2(344f, 316f);

	private Rect crdNewMapNameTxtFld = new Rect(265f, 347f, 230f, 26f);

	private Rect crdSkyboxPoint = new Rect(260f, 396f, 20f, 22f);

	private Vector2 crdSkyboxLabel = new Vector2(287f, 420f);

	private Rect crdSkyboxView = new Rect(260f, 425f, 230f, 290f);

	private Vector2 crdSkyboxSize = new Vector2(70f, 70f);

	private Rect crdLandscapePoint = new Rect(510f, 396f, 20f, 22f);

	private Vector2 crdLandscapeLabel = new Vector2(541f, 420f);

	private Rect crdLandscapeView = new Rect(511f, 425f, 230f, 290f);

	private Vector2 crdLandscapeSize = new Vector2(70f, 70f);

	private int xCount = 3;

	private Vector2 crdModeTitleL = new Vector2(15f, 160f);

	private Vector2 modeSize = new Vector2(190f, 22f);

	private Rect crdModeView = new Rect(15f, 173f, 210f, 180f);

	private Rect crdOptionBound = new Rect(5f, 351f, 230f, 432f);

	private Rect crdOptionBound4Build = new Rect(5f, 156f, 230f, 432f);

	private Rect crdOption = new Rect(5f, 268f, 230f, 320f);

	private Vector2 BoxTextBgSize = new Vector2(180f, 24f);

	private Vector2 crdArrow = new Vector2(22f, 18f);

	private float crdLabelOffset = 25f;

	private float crdBoxOffset = 25f;

	private float crdValueOffset = 3f;

	private ushort curModeMask = 32767;

	private Color txtMainClr = Color.white;

	private int umiSlot;

	private bool[] IsSupportClanMode;

	private int buildPhaseTime;

	private int battlePhaseTime;

	private int repeat;

	private bool useBuildGun;

	private int bungeeTime;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.CREATE_ROOM;
		txtMainClr = GlobalVars.Instance.GetByteColor2FloatColor(244, 151, 25);
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
		updateOnce = false;
	}

	private void SetRandomRoomTitle()
	{
		string[] array = new string[5];
		for (int i = 0; i < 5; i++)
		{
			string arg = "RANDOM_ROOM_TITLE";
			arg += i;
			array[i] = StringMgr.Instance.Get(arg);
		}
		if (ChannelManager.Instance.CurChannel.Mode == 4)
		{
			roomTitle = MyInfoManager.Instance.ClanName;
		}
		else
		{
			roomTitle = array[UnityEngine.Random.Range(0, 5)];
		}
		roomPswd = string.Empty;
	}

	public void SetupUMI()
	{
		UserMapInfoManager.Instance.Verify();
		umi = UserMapInfoManager.Instance.ToArray();
	}

	private void SetupREG()
	{
		reg = RegMapManager.Instance.ToArray(option, (Channel.MODE)ChannelManager.Instance.CurChannel.Mode);
		reg = reg.OrderBy(x => x.Alias).ToArray();
	}

	private bool IsClanSupportMode(int id)
	{
		if (!IsSupportClanMode[id])
		{
			return false;
		}
		return true;
	}

	private void Init()
	{
		GlobalVars.Instance.clanSendSqudREQ = -1;
		if (BuildOption.Instance.Props.UseItemDrop)
		{
			itemPickup = true;
		}
		if (BuildOption.Instance.Props.UseWanted)
		{
			wanted = false;
		}
		IsSupportClanMode = new bool[10]
		{
			false,
			true,
			false,
			true,
			true,
			false,
			false,
			false,
			false,
			false
		};
		List<string> list = new List<string>();
		List<Room.ROOM_TYPE> list2 = new List<Room.ROOM_TYPE>();
		if (ChannelManager.Instance.CurChannel.Mode != 3)
		{
			GlobalVars.Instance.allocBattleMode(10);
			int num = 0;
			for (Room.ROOM_TYPE rOOM_TYPE = Room.ROOM_TYPE.MAP_EDITOR; rOOM_TYPE < Room.ROOM_TYPE.NUM_TYPE; rOOM_TYPE++)
			{
				if (ChannelManager.Instance.CurChannel.Mode != 4)
				{
					if (rOOM_TYPE != 0 && BuildOption.Instance.Props.IsSupportMode(rOOM_TYPE))
					{
						list.Add(StringMgr.Instance.Get(modeOptKeys[(int)rOOM_TYPE]));
						list2.Add(rOOM_TYPE);
						GlobalVars.Instance.setBattleMode(num++, Room.modeSelector[(int)rOOM_TYPE]);
					}
				}
				else if (rOOM_TYPE != 0 && BuildOption.Instance.Props.IsSupportMode(rOOM_TYPE) && IsClanSupportMode((int)rOOM_TYPE))
				{
					list.Add(StringMgr.Instance.Get(modeOptKeys[(int)rOOM_TYPE]));
					list2.Add(rOOM_TYPE);
					GlobalVars.Instance.setBattleMode(num++, Room.modeSelector[(int)rOOM_TYPE]);
				}
			}
		}
		else
		{
			for (Room.ROOM_TYPE rOOM_TYPE2 = Room.ROOM_TYPE.MAP_EDITOR; rOOM_TYPE2 < Room.ROOM_TYPE.NUM_TYPE; rOOM_TYPE2++)
			{
				if (rOOM_TYPE2 == Room.ROOM_TYPE.MAP_EDITOR)
				{
					list.Add(StringMgr.Instance.Get(modeOptKeys[(int)rOOM_TYPE2]));
					list2.Add(rOOM_TYPE2);
				}
			}
		}
		BattleModes = list.ToArray();
		roomTypes = list2.ToArray();
		weaponOptions = new string[weaponOptKeys.Length];
		for (int i = 0; i < weaponOptions.Length; i++)
		{
			weaponOptions[i] = StringMgr.Instance.Get(weaponOptKeys[i]);
		}
		killCountOptions = new string[killCounts.Length];
		for (int j = 0; j < killCounts.Length; j++)
		{
			killCountOptions[j] = killCounts[j].ToString();
		}
		timeLimitOptions = new string[timeLimits.Length];
		for (int k = 0; k < timeLimits.Length; k++)
		{
			int num4 = timeLimits[k] / 60;
			timeLimitOptions[k] = num4.ToString();
		}
		waveTimeLimitOptions = new string[waveTimeLimits.Length];
		for (int l = 0; l < waveTimeLimits.Length; l++)
		{
			int num5 = waveTimeLimits[l] / 60;
			waveTimeLimitOptions[l] = num5.ToString();
		}
		waveOptions = new string[waveKeys.Length];
		for (int m = 0; m < waveKeys.Length; m++)
		{
			waveOptions[m] = StringMgr.Instance.Get(waveKeys[m]);
		}
		buildPhaseTimeOptions = new string[buildPhaseTimes.Length];
		for (int n = 0; n < buildPhaseTimes.Length; n++)
		{
			int num6 = buildPhaseTimes[n] / 60;
			buildPhaseTimeOptions[n] = num6.ToString();
		}
		battlePhaseTimeOptions = new string[battlePhaseTimes.Length];
		for (int num7 = 0; num7 < battlePhaseTimes.Length; num7++)
		{
			int num8 = battlePhaseTimes[num7] / 60;
			battlePhaseTimeOptions[num7] = num8.ToString();
		}
		repeatOptions = new string[repeats.Length];
		for (int num9 = 0; num9 < repeats.Length; num9++)
		{
			repeatOptions[num9] = repeats[num9].ToString();
		}
		bungeeTimeLimitOptions = new string[bungeeTimes.Length];
		for (int num10 = 0; num10 < bungeeTimes.Length; num10++)
		{
			int num11 = bungeeTimes[num10] / 60;
			bungeeTimeLimitOptions[num10] = num11.ToString();
		}
		arrivalCountOptions = new string[arrivalCounts.Length];
		for (int num12 = 0; num12 < arrivalCounts.Length; num12++)
		{
			arrivalCountOptions[num12] = arrivalCounts[num12].ToString();
		}
	}

	private int GetDefaultOptionOnCurChannel()
	{
		if (ChannelManager.Instance.CurChannel == null)
		{
			return 0;
		}
		if (ChannelManager.Instance.CurChannel.Mode == 3)
		{
			return 0;
		}
		if (ChannelManager.Instance.CurChannel.Mode == 4)
		{
			return GetRoomTypeIndex(Room.ROOM_TYPE.TEAM_MATCH);
		}
		if ((curModeMask & 1) != 0)
		{
			return GetRoomTypeIndex(Room.ROOM_TYPE.TEAM_MATCH);
		}
		if ((curModeMask & 2) != 0)
		{
			return GetRoomTypeIndex(Room.ROOM_TYPE.INDIVIDUAL);
		}
		if ((curModeMask & 4) != 0)
		{
			return GetRoomTypeIndex(Room.ROOM_TYPE.CAPTURE_THE_FLAG);
		}
		if ((curModeMask & 8) != 0)
		{
			return GetRoomTypeIndex(Room.ROOM_TYPE.EXPLOSION);
		}
		if ((curModeMask & 0x10) != 0)
		{
			return GetRoomTypeIndex(Room.ROOM_TYPE.MISSION);
		}
		if ((curModeMask & 0x80) != 0)
		{
			return GetRoomTypeIndex(Room.ROOM_TYPE.ESCAPE);
		}
		if ((curModeMask & 0x100) != 0)
		{
			return GetRoomTypeIndex(Room.ROOM_TYPE.ZOMBIE);
		}
		return 0;
	}

	private bool IsProperOptionOnCurChannel(int opt)
	{
		if (ChannelManager.Instance.CurChannel == null)
		{
			Debug.LogError("ChannelManager.Instance.CurChannel == null");
			return false;
		}
		if (ChannelManager.Instance.CurChannel.Mode == 3)
		{
			return roomTypes[opt] == Room.ROOM_TYPE.MAP_EDITOR;
		}
		if (ChannelManager.Instance.CurChannel.Mode == 4)
		{
			return roomTypes[opt] == Room.ROOM_TYPE.TEAM_MATCH || roomTypes[opt] == Room.ROOM_TYPE.CAPTURE_THE_FLAG || roomTypes[opt] == Room.ROOM_TYPE.EXPLOSION;
		}
		return roomTypes[opt] != Room.ROOM_TYPE.MAP_EDITOR;
	}

	public void InitDialog()
	{
		Init();
		option = GetDefaultOptionOnCurChannel();
		SetRandomRoomTitle();
		SetupUMI();
		SetupREG();
	}

	public bool InitDialog4TeamMatch(int map, ushort modeMask)
	{
		curModeMask = modeMask;
		InitDialog();
		if (!IsProperOptionOnCurChannel(0))
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("OPTION_IS_BANNED_CUR_CHANNEL"));
			return false;
		}
		bool flag = false;
		int num = 0;
		while (!flag && num < reg.Length)
		{
			if (reg[num].Map == map)
			{
				flag = true;
				regMap = num;
				int num2 = num / 3;
				if (num2 % 3 > 0)
				{
					num2++;
				}
				regMapScrollPosition = new Vector2(0f, (num2 <= 0) ? 0f : ((float)(132 * (num2 - 1))));
			}
			num++;
		}
		return true;
	}

	public bool InitDialog4MapEditorNew(int slot)
	{
		curModeMask = 32767;
		InitDialog();
		umiSlot = slot;
		if (!IsProperOptionOnCurChannel(0))
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("OPTION_IS_BANNED_CUR_CHANNEL"));
			return false;
		}
		if (UserMapInfoManager.Instance.HaveEmptyUserMap())
		{
			option = 0;
		}
		return true;
	}

	public bool InitDialog4MapEditorLoad(int slot)
	{
		curModeMask = 32767;
		InitDialog();
		umiSlot = slot;
		if (!IsProperOptionOnCurChannel(0))
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("OPTION_IS_BANNED_CUR_CHANNEL"));
			return false;
		}
		option = 0;
		return true;
	}

	private void DoTitleAndPswd()
	{
		string text = roomTitle;
		LabelUtil.TextOut(crdRoomTitleL, StringMgr.Instance.Get("ROOM_TITLE").ToUpper(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		roomTitle = GUI.TextField(crdRoomTitle, roomTitle);
		if (roomTitle.Length > maxRoomTitle)
		{
			roomTitle = text;
		}
		string text2 = roomPswd;
		LabelUtil.TextOut(crdPwTitleL, StringMgr.Instance.Get("PASSWORD").ToUpper(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		roomPswd = GUI.PasswordField(crdPswd, roomPswd, '*');
		if (roomPswd.Length > maxRoomPswd)
		{
			roomPswd = text2;
		}
		TextureUtil.DrawTexture(crdIconKey, iconKey, ScaleMode.StretchToFill);
	}

	private void DoMode()
	{
		LabelUtil.TextOut(crdModeTitleL, StringMgr.Instance.Get("ROOM_TYPE").ToUpper(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		Rect rect = new Rect(0f, 0f, modeSize.x, (float)BattleModes.Length * modeSize.y);
		optionScrollPosition = GUI.BeginScrollView(crdModeView, optionScrollPosition, rect);
		int num = option;
		option = GUI.SelectionGrid(rect, option, BattleModes, 1, "BoxGridStyle");
		if (num != option)
		{
			GlobalVars.Instance.PlaySoundButtonClick();
			if (IsProperOptionOnCurChannel(option))
			{
				SetupREG();
			}
			else
			{
				option = num;
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("OPTION_IS_BANNED_CUR_CHANNEL"));
			}
		}
		GUI.EndScrollView();
	}

	private float DoNumPlayers(Vector2 pos, bool bDivline)
	{
		float y = pos.y;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, y), StringMgr.Instance.Get("NUM_PLAYERS_SET"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		y += crdLabelOffset;
		if (GlobalVars.Instance.MyButton(new Rect(5f, y, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			if (ChannelManager.Instance.CurChannel.Mode != 4)
			{
				maxPlayers--;
				if (maxPlayers <= 1)
				{
					maxPlayers = 2;
				}
			}
			else
			{
				maxPlayers -= 2;
				if (maxPlayers <= 6)
				{
					maxPlayers = 8;
				}
			}
		}
		GUI.Box(new Rect(25f, y, BoxTextBgSize.x, BoxTextBgSize.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, y + crdValueOffset), maxPlayers.ToString(), "Label", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, y, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			if (ChannelManager.Instance.CurChannel.Mode != 4)
			{
				maxPlayers++;
			}
			else
			{
				maxPlayers += 2;
			}
		}
		if (ChannelManager.Instance.CurChannel.Mode != 4)
		{
			if (maxPlayers > maxMaxPlayers[(int)roomTypes[option]])
			{
				maxPlayers = maxMaxPlayers[(int)roomTypes[option]];
			}
			if (maxPlayers < minMaxPlayers[(int)roomTypes[option]])
			{
				maxPlayers = minMaxPlayers[(int)roomTypes[option]];
			}
		}
		else
		{
			if (maxPlayers > clanMaxPlayers)
			{
				maxPlayers = clanMaxPlayers;
			}
			if (maxPlayers < clanMinPlayers)
			{
				maxPlayers = clanMinPlayers;
			}
		}
		return y + crdBoxOffset;
	}

	public override bool DoDialog()
	{
		bool flag = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		GUI.Box(new Rect(0f, 0f, size.x, size.y), string.Empty, "BoxBackground");
		if (!updateOnce)
		{
			updateOnce = true;
			if (ChannelManager.Instance.CurChannel.Mode == 1 && !MyInfoManager.Instance.GetCommonMask(MyInfoManager.COMMON_OPT.DONOT_NEWBIE_CHANNEL_MSG))
			{
				OfficialMapOnly officialMapOnly = (OfficialMapOnly)DialogManager.Instance.GetDialogAlways(DialogManager.DIALOG_INDEX.OFFICIAL_MAP_ONLY);
				if (officialMapOnly != null && !officialMapOnly.DontShowThisMessageAgain)
				{
					((OfficialMapOnly)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.OFFICIAL_MAP_ONLY, exclusive: false))?.InitDialog();
				}
			}
		}
		string text = (ChannelManager.Instance.CurChannel.Mode != 3) ? StringMgr.Instance.Get("CREATE_ROOM") : StringMgr.Instance.Get("BUILD");
		LabelUtil.TextOut(crdBigTitle, text.ToUpper(), "BigBtnLabel", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		DoTitleAndPswd();
		GUI.Box(crdHLine, string.Empty, "DivideLine");
		if (ChannelManager.Instance.CurChannel.Mode != 3)
		{
			DoMode();
		}
		else
		{
			GUI.BeginGroup(crdOptionBound4Build);
			DoNumPlayers(Vector2.zero, bDivline: false);
			GUI.EndGroup();
		}
		switch (roomTypes[option])
		{
		case Room.ROOM_TYPE.MAP_EDITOR:
			flag = DoCreator();
			break;
		case Room.ROOM_TYPE.TEAM_MATCH:
			flag = DoTeamMatch();
			break;
		case Room.ROOM_TYPE.INDIVIDUAL:
			flag = DoIndividualMatch();
			break;
		case Room.ROOM_TYPE.CAPTURE_THE_FLAG:
			flag = DoCTF();
			break;
		case Room.ROOM_TYPE.EXPLOSION:
			flag = DoExplosion();
			break;
		case Room.ROOM_TYPE.MISSION:
			flag = DoDefense();
			break;
		case Room.ROOM_TYPE.BND:
			flag = DoBnd();
			break;
		case Room.ROOM_TYPE.BUNGEE:
			flag = DoBungee();
			break;
		case Room.ROOM_TYPE.ESCAPE:
			flag = DoEscape();
			break;
		case Room.ROOM_TYPE.ZOMBIE:
			flag = DoZombie();
			break;
		}
		GUI.Box(crdVLine, string.Empty, "DivideLineV");
		Rect rc = new Rect(size.x - 44f, 5f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			flag = true;
		}
		if (flag)
		{
			DialogManager.Instance.CloseAll();
		}
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return flag;
	}

	private bool CheckInput()
	{
		roomTitle.Trim();
		roomPswd.Trim();
		if (roomTitle.Length <= 0)
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("INPUT_ROOM_TITLE"));
			return false;
		}
		string text = WordFilter.Instance.CheckBadword(roomTitle);
		if (text.Length > 0)
		{
			MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("BAD_WORD_DETECT"), text));
			return false;
		}
		if (roomTitle.Length > maxRoomTitle)
		{
			roomTitle = roomTitle.Remove(maxRoomTitle);
		}
		return true;
	}

	private bool DoDefense()
	{
		bool result = false;
		DoDefenseModeOption();
		GUIContent content = new GUIContent(StringMgr.Instance.Get("CREATE_ROOM").ToUpper(), GlobalVars.Instance.iconBlock);
		if (DoRegMap() || GlobalVars.Instance.MyButton3(crdButtonOk, content, "BtnAction") || GlobalVars.Instance.IsReturnPressed())
		{
			result = CreateDefense();
		}
		return result;
	}

	private bool DoCTF()
	{
		bool result = false;
		DoCTFModeOption();
		GUIContent content = new GUIContent(StringMgr.Instance.Get("CREATE_ROOM").ToUpper(), GlobalVars.Instance.iconBlock);
		if (DoRegMap() || GlobalVars.Instance.MyButton3(crdButtonOk, content, "BtnAction") || GlobalVars.Instance.IsReturnPressed())
		{
			if (ChannelManager.Instance.CurChannel.Mode != 4)
			{
				result = CreateCTF();
			}
			else if (MyInfoManager.Instance.ClanName.Length > 0)
			{
				result = CreateCTF();
			}
			else
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("WARNING_MSG_CREATE_ROOM"));
			}
		}
		if (UpdateCreateRoom())
		{
			int[] param = new int[8]
			{
				points[point],
				timeLimits[timeLimit],
				weaponOpt,
				reg[regMap].Map,
				breakInto ? 1 : 0,
				teamBalance ? 1 : 0,
				0,
				itemPickup ? 1 : 0
			};
			if (CSNetManager.Instance.Sock.SendCS_CREATE_ROOM_REQ(3, roomTitle, roomPswd.Length > 0, roomPswd, maxPlayers, param, reg[regMap].Alias))
			{
				result = true;
			}
		}
		return result;
	}

	private bool DoBnd()
	{
		bool result = false;
		DoBndOption();
		GUIContent content = new GUIContent(StringMgr.Instance.Get("CREATE_ROOM").ToUpper(), GlobalVars.Instance.iconBlock);
		if (DoRegMap() || GlobalVars.Instance.MyButton3(crdButtonOk, content, "BtnAction") || GlobalVars.Instance.IsReturnPressed())
		{
			result = CreateBnd();
		}
		return result;
	}

	private void DoBndOption()
	{
		GUI.BeginGroup(crdOptionBound);
		float num = 0f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("BUILD_PHASE_TIME"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += crdLabelOffset;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			buildPhaseTime--;
			if (buildPhaseTime < 0)
			{
				buildPhaseTime = 0;
			}
		}
		GUI.Box(new Rect(25f, num, BoxTextBgSize.x, BoxTextBgSize.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num + crdValueOffset), buildPhaseTimeOptions[buildPhaseTime] + StringMgr.Instance.Get("MINUTES"), "Label", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			buildPhaseTime++;
			if (buildPhaseTime >= buildPhaseTimes.Length)
			{
				buildPhaseTime = buildPhaseTimes.Length - 1;
			}
		}
		num += crdBoxOffset;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("SHOOT_PHASE_TIME"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += crdLabelOffset;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			battlePhaseTime--;
			if (battlePhaseTime < 0)
			{
				battlePhaseTime = 0;
			}
		}
		GUI.Box(new Rect(25f, num, BoxTextBgSize.x, BoxTextBgSize.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num + crdValueOffset), battlePhaseTimeOptions[battlePhaseTime] + StringMgr.Instance.Get("MINUTES"), "Label", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			battlePhaseTime++;
			if (battlePhaseTime >= battlePhaseTimes.Length)
			{
				battlePhaseTime = battlePhaseTimes.Length - 1;
			}
		}
		num += crdBoxOffset;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("BND_REPEAT"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += crdLabelOffset;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			repeat--;
			if (repeat < 0)
			{
				repeat = 0;
			}
		}
		GUI.Box(new Rect(25f, num, BoxTextBgSize.x, BoxTextBgSize.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num + crdValueOffset), repeatOptions[repeat] + StringMgr.Instance.Get("TIMES_UNIT"), "Label", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			repeat++;
			if (repeat >= repeats.Length)
			{
				repeat = repeats.Length - 1;
			}
		}
		num += crdBoxOffset;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("KILL_COUNT"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += crdLabelOffset;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			killCount--;
			if (killCount < 0)
			{
				killCount = 0;
			}
		}
		GUI.Box(new Rect(25f, num, BoxTextBgSize.x, BoxTextBgSize.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num + crdValueOffset), killCountOptions[killCount] + StringMgr.Instance.Get("KILL"), "Label", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			killCount++;
			if (killCount >= killCounts.Length)
			{
				killCount = killCounts.Length - 1;
			}
		}
		num += crdBoxOffset;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("WEAPON_OPTION"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += crdLabelOffset;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			weaponOpt--;
			if (weaponOpt < 0)
			{
				weaponOpt = 0;
			}
		}
		GUI.Box(new Rect(25f, num, BoxTextBgSize.x, BoxTextBgSize.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num + crdValueOffset), weaponOptions[weaponOpt], "Label", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			weaponOpt++;
			if (weaponOpt >= weaponOptions.Length)
			{
				weaponOpt = weaponOptions.Length - 1;
			}
		}
		num += crdBoxOffset;
		num = DoNumPlayers(new Vector2(crdOption.x, num), bDivline: true);
		breakInto = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), breakInto, StringMgr.Instance.Get("BREAK_INTO"));
		num += crdBoxOffset;
		teamBalance = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), teamBalance, StringMgr.Instance.Get("TEAM_BALANCE"));
		itemPickup = false;
		if (BuildOption.Instance.AllowBuildGunInDestroyPhase())
		{
			num += crdBoxOffset;
			useBuildGun = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), useBuildGun, StringMgr.Instance.Get("USE_BUILD_GUN"));
		}
		GUI.EndGroup();
	}

	private void DoBungeeModeOption()
	{
		GUI.BeginGroup(crdOptionBound);
		float num = 0f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("TIME_LIMIT"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += crdLabelOffset;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			bungeeTime--;
			if (bungeeTime < 0)
			{
				bungeeTime = 0;
			}
		}
		GUI.Box(new Rect(25f, num, BoxTextBgSize.x, BoxTextBgSize.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num + crdValueOffset), bungeeTimeLimitOptions[bungeeTime] + StringMgr.Instance.Get("MINUTES"), "Label", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			bungeeTime++;
			if (bungeeTime >= bungeeTimeLimitOptions.Length)
			{
				bungeeTime = bungeeTimeLimitOptions.Length - 1;
			}
		}
		num += crdBoxOffset;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("BUNGEE_COUNT"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += crdLabelOffset;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			killCount--;
			if (killCount < 0)
			{
				killCount = 0;
			}
		}
		GUI.Box(new Rect(25f, num, BoxTextBgSize.x, BoxTextBgSize.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num + crdValueOffset), killCountOptions[killCount] + StringMgr.Instance.Get("KILL"), "Label", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			killCount++;
			if (killCount >= killCounts.Length)
			{
				killCount = killCounts.Length - 1;
			}
		}
		num += crdBoxOffset;
		num = DoNumPlayers(new Vector2(crdOption.x, num), bDivline: true);
		breakInto = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), breakInto, StringMgr.Instance.Get("BREAK_INTO"));
		GUI.EndGroup();
	}

	private void DoIndividualMatchOption()
	{
		GUI.BeginGroup(crdOptionBound);
		float num = 0f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("TIME_LIMIT"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += crdLabelOffset;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			timeLimit--;
			if (timeLimit < 0)
			{
				timeLimit = 0;
			}
		}
		GUI.Box(new Rect(25f, num, BoxTextBgSize.x, BoxTextBgSize.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num + crdValueOffset), timeLimitOptions[timeLimit] + StringMgr.Instance.Get("MINUTES"), "Label", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			timeLimit++;
			if (timeLimit >= timeLimits.Length)
			{
				timeLimit = timeLimits.Length - 1;
			}
		}
		num += crdBoxOffset;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("KILL_COUNT"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += crdLabelOffset;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			killCount--;
			if (killCount < 0)
			{
				killCount = 0;
			}
		}
		GUI.Box(new Rect(25f, num, BoxTextBgSize.x, BoxTextBgSize.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num + crdValueOffset), killCountOptions[killCount] + StringMgr.Instance.Get("KILL"), "Label", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			killCount++;
			if (killCount >= killCounts.Length)
			{
				killCount = killCounts.Length - 1;
			}
		}
		num += crdBoxOffset;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("WEAPON_OPTION"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += crdLabelOffset;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			weaponOpt--;
			if (weaponOpt < 0)
			{
				weaponOpt = 0;
			}
		}
		GUI.Box(new Rect(25f, num, BoxTextBgSize.x, BoxTextBgSize.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num + crdValueOffset), weaponOptions[weaponOpt], "Label", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			weaponOpt++;
			if (weaponOpt >= weaponOptions.Length)
			{
				weaponOpt = weaponOptions.Length - 1;
			}
		}
		num += crdBoxOffset;
		num = DoNumPlayers(new Vector2(crdOption.x, num), bDivline: true);
		breakInto = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), breakInto, StringMgr.Instance.Get("BREAK_INTO"));
		if (BuildOption.Instance.Props.UseItemDrop)
		{
			num += crdBoxOffset;
			itemPickup = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), itemPickup, StringMgr.Instance.Get("ROOM_SET_ITEMDROP"));
		}
		if (BuildOption.Instance.Props.UseWanted)
		{
			num += crdBoxOffset;
			wanted = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), wanted, StringMgr.Instance.Get("ROOM_SET_WANTED"));
		}
		GUI.EndGroup();
	}

	private void DoExplosionModeOption()
	{
		GUI.BeginGroup(crdOptionBound);
		float num = 0f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("ROUND"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += crdLabelOffset;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			round--;
			if (round < 0)
			{
				round = 0;
			}
		}
		GUI.Box(new Rect(25f, num, BoxTextBgSize.x, BoxTextBgSize.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num + crdValueOffset), roundOptions[round], "Label", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			round++;
			if (round >= roundOptions.Length)
			{
				round = roundOptions.Length - 1;
			}
		}
		num += crdBoxOffset;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("WEAPON_OPTION"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += crdLabelOffset;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			weaponOpt--;
			if (weaponOpt < 0)
			{
				weaponOpt = 0;
			}
		}
		GUI.Box(new Rect(25f, num, BoxTextBgSize.x, BoxTextBgSize.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num + crdValueOffset), weaponOptions[weaponOpt], "Label", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			weaponOpt++;
			if (weaponOpt >= weaponOptions.Length)
			{
				weaponOpt = weaponOptions.Length - 1;
			}
		}
		num += crdBoxOffset;
		num = DoNumPlayers(new Vector2(crdOption.x, num), bDivline: true);
		breakInto = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), breakInto, StringMgr.Instance.Get("BREAK_INTO"));
		if (ChannelManager.Instance.CurChannel.Mode != 4)
		{
			num += crdBoxOffset;
			teamBalance = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), teamBalance, StringMgr.Instance.Get("TEAM_BALANCE"));
		}
		if (BuildOption.Instance.Props.UseItemDrop)
		{
			num += crdBoxOffset;
			itemPickup = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), itemPickup, StringMgr.Instance.Get("ROOM_SET_ITEMDROP"));
		}
		GUI.EndGroup();
	}

	private void DoCTFModeOption()
	{
		GUI.BeginGroup(crdOptionBound);
		float num = 0f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("TIME_LIMIT"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += crdLabelOffset;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			timeLimit--;
			if (timeLimit < 0)
			{
				timeLimit = 0;
			}
		}
		GUI.Box(new Rect(25f, num, BoxTextBgSize.x, BoxTextBgSize.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num + crdValueOffset), timeLimitOptions[timeLimit], "Label", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			timeLimit++;
			if (timeLimit >= timeLimits.Length)
			{
				timeLimit = timeLimits.Length - 1;
			}
		}
		num += crdBoxOffset;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("WEAPON_OPTION"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += crdLabelOffset;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			weaponOpt--;
			if (weaponOpt < 0)
			{
				weaponOpt = 0;
			}
		}
		GUI.Box(new Rect(25f, num, BoxTextBgSize.x, BoxTextBgSize.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num + crdValueOffset), weaponOptions[weaponOpt], "Label", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			weaponOpt++;
			if (weaponOpt >= weaponOptions.Length)
			{
				weaponOpt = weaponOptions.Length - 1;
			}
		}
		num += crdBoxOffset;
		num = DoNumPlayers(new Vector2(crdOption.x, num), bDivline: true);
		breakInto = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), breakInto, StringMgr.Instance.Get("BREAK_INTO"));
		if (ChannelManager.Instance.CurChannel.Mode != 4)
		{
			num += crdBoxOffset;
			teamBalance = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), teamBalance, StringMgr.Instance.Get("TEAM_BALANCE"));
		}
		if (BuildOption.Instance.Props.UseItemDrop)
		{
			num += crdBoxOffset;
			itemPickup = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), itemPickup, StringMgr.Instance.Get("ROOM_SET_ITEMDROP"));
		}
		GUI.EndGroup();
	}

	private void DoDefenseModeOption()
	{
		GUI.BeginGroup(crdOptionBound);
		float num = 0f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("NUCLEAR_LIFE"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += crdLabelOffset;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			wave--;
			if (wave < 0)
			{
				wave = 0;
			}
		}
		GUI.Box(new Rect(25f, num, BoxTextBgSize.x, BoxTextBgSize.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num + crdValueOffset), waveOptions[wave], "Label", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			wave++;
			if (wave >= waveOptions.Length)
			{
				wave = waveOptions.Length - 1;
			}
		}
		num += crdBoxOffset;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("TIME_LIMIT"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += crdLabelOffset;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			waveTimeLimit--;
			if (waveTimeLimit < 0)
			{
				waveTimeLimit = 0;
			}
		}
		GUI.Box(new Rect(25f, num, BoxTextBgSize.x, BoxTextBgSize.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num + crdValueOffset), waveTimeLimitOptions[waveTimeLimit] + StringMgr.Instance.Get("MINUTES"), "Label", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			waveTimeLimit++;
			if (waveTimeLimit >= waveTimeLimits.Length)
			{
				waveTimeLimit = waveTimeLimits.Length - 1;
			}
		}
		num += crdBoxOffset;
		num = DoNumPlayers(new Vector2(crdOption.x, num), bDivline: true);
		breakInto = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), breakInto, StringMgr.Instance.Get("BREAK_INTO"));
		if (BuildOption.Instance.Props.UseItemDrop)
		{
			num += crdBoxOffset;
			itemPickup = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), itemPickup, StringMgr.Instance.Get("ROOM_SET_ITEMDROP"));
		}
		GUI.EndGroup();
	}

	private void DoTeamMatchOption()
	{
		GUI.BeginGroup(crdOptionBound);
		float num = 0f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("TIME_LIMIT"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += crdLabelOffset;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			timeLimit--;
			if (timeLimit < 0)
			{
				timeLimit = 0;
			}
		}
		GUI.Box(new Rect(25f, num, BoxTextBgSize.x, BoxTextBgSize.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num + crdValueOffset), timeLimitOptions[timeLimit] + StringMgr.Instance.Get("MINUTES"), "Label", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			timeLimit++;
			if (timeLimit >= timeLimits.Length)
			{
				timeLimit = timeLimits.Length - 1;
			}
		}
		num += crdBoxOffset;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("KILL_COUNT"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += crdLabelOffset;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			killCount--;
			if (killCount < 0)
			{
				killCount = 0;
			}
		}
		GUI.Box(new Rect(25f, num, BoxTextBgSize.x, BoxTextBgSize.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num + crdValueOffset), killCountOptions[killCount] + StringMgr.Instance.Get("KILL"), "Label", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			killCount++;
			if (killCount >= killCounts.Length)
			{
				killCount = killCounts.Length - 1;
			}
		}
		num += crdBoxOffset;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("WEAPON_OPTION"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += crdLabelOffset;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			weaponOpt--;
			if (weaponOpt < 0)
			{
				weaponOpt = 0;
			}
		}
		GUI.Box(new Rect(25f, num, BoxTextBgSize.x, BoxTextBgSize.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num + crdValueOffset), weaponOptions[weaponOpt], "Label", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			weaponOpt++;
			if (weaponOpt >= weaponOptions.Length)
			{
				weaponOpt = weaponOptions.Length - 1;
			}
		}
		num += crdBoxOffset;
		num = DoNumPlayers(new Vector2(crdOption.x, num), bDivline: true);
		breakInto = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), breakInto, StringMgr.Instance.Get("BREAK_INTO"));
		if (ChannelManager.Instance.CurChannel.Mode != 4)
		{
			num += crdBoxOffset;
			teamBalance = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), teamBalance, StringMgr.Instance.Get("TEAM_BALANCE"));
		}
		if (BuildOption.Instance.Props.UseItemDrop)
		{
			num += crdBoxOffset;
			itemPickup = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), itemPickup, StringMgr.Instance.Get("ROOM_SET_ITEMDROP"));
		}
		if (BuildOption.Instance.Props.UseWanted)
		{
			num += crdBoxOffset;
			wanted = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), wanted, StringMgr.Instance.Get("ROOM_SET_WANTED"));
		}
		GUI.EndGroup();
	}

	private void DoEscapeModeOption()
	{
		GUI.BeginGroup(crdOptionBound);
		float num = 0f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("TIME_LIMIT"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += crdLabelOffset;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			timeLimit--;
			if (timeLimit < 0)
			{
				timeLimit = 0;
			}
		}
		GUI.Box(new Rect(25f, num, BoxTextBgSize.x, BoxTextBgSize.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num + crdValueOffset), timeLimitOptions[timeLimit] + StringMgr.Instance.Get("MINUTES"), "Label", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			timeLimit++;
			if (timeLimit >= timeLimits.Length)
			{
				timeLimit = timeLimits.Length - 1;
			}
		}
		num += crdBoxOffset;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("ARRIVAL_COUNT"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += crdLabelOffset;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			arrivalCount--;
			if (arrivalCount < 0)
			{
				arrivalCount = 0;
			}
		}
		GUI.Box(new Rect(25f, num, BoxTextBgSize.x, BoxTextBgSize.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num + crdValueOffset), arrivalCountOptions[arrivalCount] + StringMgr.Instance.Get("TIMES_UNIT"), "Label", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			arrivalCount++;
			if (arrivalCount >= arrivalCounts.Length)
			{
				arrivalCount = arrivalCounts.Length - 1;
			}
		}
		num += crdBoxOffset;
		num = DoNumPlayers(new Vector2(crdOption.x, num), bDivline: true);
		breakInto = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), breakInto, StringMgr.Instance.Get("BREAK_INTO"));
		num += crdBoxOffset;
		if (BuildOption.Instance.Props.UseEscapeAttack)
		{
			useWeaponOption = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), useWeaponOption, StringMgr.Instance.Get("ESCAPE_MODE_OPTION_01"));
			num += crdBoxOffset;
			if (BuildOption.Instance.Props.UseItemDrop && useWeaponOption)
			{
				itemPickup = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), itemPickup, StringMgr.Instance.Get("ROOM_SET_ITEMDROP"));
				num += crdBoxOffset;
			}
		}
		else
		{
			useWeaponOption = false;
		}
		GUI.EndGroup();
	}

	private void DoZombieModeOption()
	{
		GUI.BeginGroup(crdOptionBound);
		float num = 0f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("ROUND"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += crdLabelOffset;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			round--;
			if (round < 0)
			{
				round = 0;
			}
		}
		GUI.Box(new Rect(25f, num, BoxTextBgSize.x, BoxTextBgSize.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num + crdValueOffset), roundOptions[round], "Label", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			round++;
			if (round >= roundOptions.Length)
			{
				round = roundOptions.Length - 1;
			}
		}
		num += crdBoxOffset;
		num = DoNumPlayers(new Vector2(crdOption.x, num), bDivline: true);
		breakInto = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), breakInto, StringMgr.Instance.Get("BREAK_INTO"));
		GUI.EndGroup();
	}

	private bool DoTeamMatch()
	{
		bool result = false;
		DoTeamMatchOption();
		GUIContent content = new GUIContent(StringMgr.Instance.Get("CREATE_ROOM").ToUpper(), GlobalVars.Instance.iconBlock);
		if (DoRegMap() || GlobalVars.Instance.MyButton3(crdButtonOk, content, "BtnAction") || GlobalVars.Instance.IsReturnPressed())
		{
			if (ChannelManager.Instance.CurChannel.Mode != 4)
			{
				result = CreateTeamMatch();
			}
			else if (MyInfoManager.Instance.ClanName.Length > 0)
			{
				result = CreateTeamMatch();
			}
			else
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("WARNING_MSG_CREATE_ROOM"));
			}
		}
		if (UpdateCreateRoom())
		{
			int[] param = new int[8]
			{
				killCounts[killCount],
				timeLimits[timeLimit],
				weaponOpt,
				reg[regMap].Map,
				breakInto ? 1 : 0,
				teamBalance ? 1 : 0,
				wanted ? 1 : 0,
				itemPickup ? 1 : 0
			};
			if (CSNetManager.Instance.Sock.SendCS_CREATE_ROOM_REQ(1, roomTitle, roomPswd.Length > 0, roomPswd, maxPlayers, param, reg[regMap].Alias))
			{
				result = true;
			}
		}
		return result;
	}

	private bool DoRegMap()
	{
		bool result = false;
		int num = reg.Length / 3;
		if (reg.Length % 3 > 0)
		{
			num++;
		}
		Rect viewRect = new Rect(0f, 0f, crdMapSize.x * 3f + crdMapOffset.x * 2f, crdMapSize.y * (float)num);
		if (num > 1)
		{
			viewRect.height += crdMapOffset.y * (float)(num - 1);
		}
		regMapScrollPosition = GUI.BeginScrollView(crdRegMapRect, regMapScrollPosition, viewRect);
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				int num2 = 3 * i + j;
				if (num2 < reg.Length)
				{
					Rect rect = new Rect((float)j * (crdMapSize.x + crdMapOffset.x), (float)i * (crdMapSize.y + crdMapOffset.y), crdMapSize.x, crdMapSize.y);
					Rect position = new Rect(rect.x, rect.y, rect.width, rect.width + 4f);
					TextureUtil.DrawTexture(position, (!(reg[num2].Thumbnail == null)) ? reg[num2].Thumbnail : nonAvailable, ScaleMode.StretchToFill);
					if (GlobalVars.Instance.MyButton(rect, string.Empty, "BoxMapSelectBorder"))
					{
						regMap = num2;
						if (Time.time - lastClickTime > doubleClickTimeout)
						{
							lastClickTime = Time.time;
						}
						else
						{
							result = true;
						}
					}
					DateTime registeredDate = reg[num2].RegisteredDate;
					if (registeredDate.Year == DateTime.Today.Year && registeredDate.Month == DateTime.Today.Month && registeredDate.Day == DateTime.Today.Day)
					{
						TextureUtil.DrawTexture(new Rect(rect.x, rect.y, (float)GlobalVars.Instance.iconNewmap.width, (float)GlobalVars.Instance.iconNewmap.height), GlobalVars.Instance.iconNewmap, ScaleMode.StretchToFill);
					}
					else if ((reg[num2].tagMask & 8) != 0)
					{
						TextureUtil.DrawTexture(new Rect(rect.x, rect.y, (float)GlobalVars.Instance.iconglory.width, (float)GlobalVars.Instance.iconglory.height), GlobalVars.Instance.iconglory, ScaleMode.StretchToFill);
					}
					else if ((reg[num2].tagMask & 4) != 0)
					{
						TextureUtil.DrawTexture(new Rect(rect.x, rect.y, (float)GlobalVars.Instance.iconMedal.width, (float)GlobalVars.Instance.iconMedal.height), GlobalVars.Instance.iconMedal, ScaleMode.StretchToFill);
					}
					else if ((reg[num2].tagMask & 2) != 0)
					{
						TextureUtil.DrawTexture(new Rect(rect.x, rect.y, (float)GlobalVars.Instance.icongoldRibbon.width, (float)GlobalVars.Instance.icongoldRibbon.height), GlobalVars.Instance.icongoldRibbon, ScaleMode.StretchToFill);
					}
					if (reg[num2].IsAbuseMap())
					{
						float x = rect.x + rect.width - (float)GlobalVars.Instance.iconDeclare.width;
						TextureUtil.DrawTexture(new Rect(x, rect.y, (float)GlobalVars.Instance.iconDeclare.width, (float)GlobalVars.Instance.iconDeclare.height), GlobalVars.Instance.iconDeclare, ScaleMode.StretchToFill);
					}
					LabelUtil.TextOut(new Vector2(rect.x + crdDeveloper.x, rect.y + crdDeveloper.y), reg[num2].Developer, "MiniLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
					LabelUtil.TextOut(new Vector2(rect.x + crdAlias.x, rect.y + crdAlias.y), reg[num2].Alias, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
					if (regMap == num2)
					{
						TextureUtil.DrawTexture(rect, selectedMapFrame, ScaleMode.StretchToFill);
					}
				}
			}
		}
		GUI.EndScrollView();
		return result;
	}

	private bool DoIndividualMatch()
	{
		bool result = false;
		DoIndividualMatchOption();
		GUIContent content = new GUIContent(StringMgr.Instance.Get("CREATE_ROOM").ToUpper(), GlobalVars.Instance.iconBlock);
		if (DoRegMap() || GlobalVars.Instance.MyButton3(crdButtonOk, content, "BtnAction") || GlobalVars.Instance.IsReturnPressed())
		{
			result = CreateIndividualMatch();
		}
		return result;
	}

	private bool CreateIndividualMatch()
	{
		if (CheckInput())
		{
			if (regMap < 0 || regMap >= reg.Length)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CANT_PLAY_WITHOUT_MAP"));
			}
			else
			{
				int[] param = new int[8]
				{
					killCounts[killCount],
					timeLimits[timeLimit],
					weaponOpt,
					reg[regMap].Map,
					breakInto ? 1 : 0,
					teamBalance ? 1 : 0,
					wanted ? 1 : 0,
					itemPickup ? 1 : 0
				};
				if (CSNetManager.Instance.Sock.SendCS_CREATE_ROOM_REQ(2, roomTitle, roomPswd.Length > 0, roomPswd, maxPlayers, param, reg[regMap].Alias))
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool CreateDefense()
	{
		if (CheckInput())
		{
			if (regMap < 0 || regMap >= reg.Length)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CANT_PLAY_WITHOUT_MAP"));
			}
			else
			{
				int num = Convert.ToInt32(waveOptions[wave]);
				DefenseManager.Instance.CoreLifeRed = num;
				DefenseManager.Instance.CoreLifeBlue = num;
				int[] param = new int[8]
				{
					num,
					waveTimeLimits[waveTimeLimit],
					0,
					reg[regMap].Map,
					breakInto ? 1 : 0,
					0,
					0,
					itemPickup ? 1 : 0
				};
				if (CSNetManager.Instance.Sock.SendCS_CREATE_ROOM_REQ(5, roomTitle, roomPswd.Length > 0, roomPswd, maxPlayers, param, reg[regMap].Alias))
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool CreateTeamMatch()
	{
		if (CheckInput())
		{
			if (regMap < 0 || regMap >= reg.Length)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CANT_PLAY_WITHOUT_MAP"));
			}
			else
			{
				int[] param = new int[8]
				{
					killCounts[killCount],
					timeLimits[timeLimit],
					weaponOpt,
					reg[regMap].Map,
					breakInto ? 1 : 0,
					teamBalance ? 1 : 0,
					wanted ? 1 : 0,
					itemPickup ? 1 : 0
				};
				if (ChannelManager.Instance.CurChannel.Mode != 4)
				{
					if (CSNetManager.Instance.Sock.SendCS_CREATE_ROOM_REQ(1, roomTitle, roomPswd.Length > 0, roomPswd, maxPlayers, param, reg[regMap].Alias))
					{
						return true;
					}
				}
				else
				{
					GlobalVars.Instance.wannaPlayMap = reg[regMap].Map;
					GlobalVars.Instance.wannaPlayMode = (int)roomTypes[option];
					SendCS_CREATE_ROOM_REQ();
				}
			}
		}
		return false;
	}

	private bool CreateMapEditorNew(UserMapInfo curUMI)
	{
		if (CheckInput())
		{
			newMapName.Trim();
			if (newMapName.Length <= 0)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("INPUT_MAP_NAME"));
			}
			else
			{
				if (newMapName.Length >= 2)
				{
					int[] param = new int[8]
					{
						0,
						curUMI.Slot,
						tex2LandscapeIndex[landscape],
						tex2SkyboxIndex[skybox],
						curUMI.Premium,
						0,
						0,
						0
					};
					if (CSNetManager.Instance.Sock.SendCS_CREATE_ROOM_REQ(0, roomTitle, roomPswd.Length > 0, roomPswd, Mathf.RoundToInt((float)maxPlayers), param, newMapName))
					{
						UserMapInfoManager.Instance.CreateBuildMode(curUMI.Slot, newMapName);
					}
					return true;
				}
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("TOO_SHORT_MAP_NAME"));
			}
		}
		return false;
	}

	private bool CreateMapEditorLoad(UserMapInfo curUMI)
	{
		if (CheckInput())
		{
			int[] param = new int[8]
			{
				1,
				curUMI.Slot,
				curUMI.BrickCount,
				0,
				curUMI.Premium,
				0,
				0,
				0
			};
			if (CSNetManager.Instance.Sock.SendCS_CREATE_ROOM_REQ(0, roomTitle, roomPswd.Length > 0, roomPswd, maxPlayers, param, string.Empty))
			{
				UserMapInfoManager.Instance.CreateBuildMode(umiSlot, curUMI.Alias);
			}
			return true;
		}
		return false;
	}

	private bool CreateBnd()
	{
		if (CheckInput())
		{
			if (regMap < 0 || regMap >= reg.Length)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CANT_PLAY_WITHOUT_MAP"));
			}
			else
			{
				int num = BndTimer.PackTimerOption(buildPhaseTimes[buildPhaseTime], battlePhaseTimes[battlePhaseTime], repeats[repeat]);
				int[] param = new int[8]
				{
					killCounts[killCount],
					num,
					weaponOpt,
					reg[regMap].Map,
					breakInto ? 1 : 0,
					teamBalance ? 1 : 0,
					useBuildGun ? 1 : 0,
					itemPickup ? 1 : 0
				};
				if (CSNetManager.Instance.Sock.SendCS_CREATE_ROOM_REQ(6, roomTitle, roomPswd.Length > 0, roomPswd, maxPlayers, param, reg[regMap].Alias))
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool CreateCTF()
	{
		if (CheckInput())
		{
			if (regMap < 0 || regMap >= reg.Length)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CANT_PLAY_WITHOUT_MAP"));
			}
			else
			{
				int[] param = new int[8]
				{
					points[point],
					timeLimits[timeLimit],
					weaponOpt,
					reg[regMap].Map,
					breakInto ? 1 : 0,
					teamBalance ? 1 : 0,
					0,
					itemPickup ? 1 : 0
				};
				if (ChannelManager.Instance.CurChannel.Mode != 4)
				{
					if (CSNetManager.Instance.Sock.SendCS_CREATE_ROOM_REQ(3, roomTitle, roomPswd.Length > 0, roomPswd, maxPlayers, param, reg[regMap].Alias))
					{
						return true;
					}
				}
				else
				{
					GlobalVars.Instance.wannaPlayMap = reg[regMap].Map;
					GlobalVars.Instance.wannaPlayMode = (int)roomTypes[option];
					SendCS_CREATE_ROOM_REQ();
				}
			}
		}
		return false;
	}

	private bool CreateExplosion()
	{
		if (CheckInput())
		{
			if (regMap < 0 || regMap >= reg.Length)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CANT_PLAY_WITHOUT_MAP"));
			}
			else
			{
				int[] param = new int[8]
				{
					rounds[round],
					ExplosionMatch.playTimePerRound,
					weaponOpt,
					reg[regMap].Map,
					breakInto ? 1 : 0,
					teamBalance ? 1 : 0,
					0,
					itemPickup ? 1 : 0
				};
				if (ChannelManager.Instance.CurChannel.Mode != 4)
				{
					if (CSNetManager.Instance.Sock.SendCS_CREATE_ROOM_REQ(4, roomTitle, roomPswd.Length > 0, roomPswd, maxPlayers, param, reg[regMap].Alias))
					{
						return true;
					}
				}
				else
				{
					GlobalVars.Instance.wannaPlayMap = reg[regMap].Map;
					GlobalVars.Instance.wannaPlayMode = (int)roomTypes[option];
					SendCS_CREATE_ROOM_REQ();
				}
			}
		}
		return false;
	}

	private bool CreateBungee()
	{
		if (CheckInput())
		{
			if (regMap < 0 || regMap >= reg.Length)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CANT_PLAY_WITHOUT_MAP"));
			}
			else
			{
				int[] param = new int[8]
				{
					killCounts[killCount],
					bungeeTimes[bungeeTime],
					0,
					reg[regMap].Map,
					breakInto ? 1 : 0,
					0,
					0,
					0
				};
				if (CSNetManager.Instance.Sock.SendCS_CREATE_ROOM_REQ(7, roomTitle, roomPswd.Length > 0, roomPswd, maxPlayers, param, reg[regMap].Alias))
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool CreateEscape()
	{
		if (CheckInput())
		{
			if (regMap < 0 || regMap >= reg.Length)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CANT_PLAY_WITHOUT_MAP"));
			}
			else
			{
				int[] param = new int[8]
				{
					arrivalCounts[arrivalCount],
					timeLimits[timeLimit],
					(!useWeaponOption) ? 3 : 0,
					reg[regMap].Map,
					breakInto ? 1 : 0,
					0,
					0,
					itemPickup ? 1 : 0
				};
				if (CSNetManager.Instance.Sock.SendCS_CREATE_ROOM_REQ(8, roomTitle, roomPswd.Length > 0, roomPswd, maxPlayers, param, reg[regMap].Alias))
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool CreateZombie()
	{
		if (CheckInput())
		{
			if (regMap < 0 || regMap >= reg.Length)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CANT_PLAY_WITHOUT_MAP"));
			}
			else
			{
				int[] param = new int[8]
				{
					rounds[round],
					ZombieMatch.playTimePerRound,
					0,
					reg[regMap].Map,
					breakInto ? 1 : 0,
					0,
					0,
					0
				};
				if (CSNetManager.Instance.Sock.SendCS_CREATE_ROOM_REQ(9, roomTitle, roomPswd.Length > 0, roomPswd, maxPlayers, param, reg[regMap].Alias))
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool DoExplosion()
	{
		bool result = false;
		DoExplosionModeOption();
		GUIContent content = new GUIContent(StringMgr.Instance.Get("CREATE_ROOM").ToUpper(), GlobalVars.Instance.iconBlock);
		if (DoRegMap() || GlobalVars.Instance.MyButton3(crdButtonOk, content, "BtnAction") || GlobalVars.Instance.IsReturnPressed())
		{
			if (ChannelManager.Instance.CurChannel.Mode != 4)
			{
				result = CreateExplosion();
			}
			else if (MyInfoManager.Instance.ClanName.Length > 0)
			{
				result = CreateExplosion();
			}
			else
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("WARNING_MSG_CREATE_ROOM"));
			}
		}
		if (UpdateCreateRoom())
		{
			int[] param = new int[8]
			{
				rounds[round],
				ExplosionMatch.playTimePerRound,
				weaponOpt,
				reg[regMap].Map,
				breakInto ? 1 : 0,
				teamBalance ? 1 : 0,
				0,
				itemPickup ? 1 : 0
			};
			if (CSNetManager.Instance.Sock.SendCS_CREATE_ROOM_REQ(4, roomTitle, roomPswd.Length > 0, roomPswd, maxPlayers, param, reg[regMap].Alias))
			{
				result = true;
			}
		}
		return result;
	}

	private bool DoBungee()
	{
		bool result = false;
		DoBungeeModeOption();
		if (DoRegMap() || GlobalVars.Instance.MyButton(crdButtonOk, StringMgr.Instance.Get("OK"), "BtnAction") || GlobalVars.Instance.IsReturnPressed())
		{
			result = CreateBungee();
		}
		return result;
	}

	private bool DoEscape()
	{
		bool result = false;
		DoEscapeModeOption();
		if (DoRegMap() || GlobalVars.Instance.MyButton(crdButtonOk, StringMgr.Instance.Get("OK"), "BtnAction") || GlobalVars.Instance.IsReturnPressed())
		{
			result = CreateEscape();
		}
		return result;
	}

	private bool DoZombie()
	{
		bool result = false;
		DoZombieModeOption();
		if (DoRegMap() || GlobalVars.Instance.MyButton(crdButtonOk, StringMgr.Instance.Get("OK"), "BtnAction") || GlobalVars.Instance.IsReturnPressed())
		{
			result = CreateZombie();
		}
		return result;
	}

	private bool DoCreator()
	{
		bool result = false;
		bool flag = MyInfoManager.Instance.HaveFunction("premium_account") >= 0;
		int num = umi.Length / 3;
		if (umi.Length % 3 > 0)
		{
			num++;
		}
		bool flag2 = false;
		Rect viewRect = new Rect(0f, 0f, crdMapSize.x * 3f + crdMapOffset.x * 2f, crdMapSize.y * (float)num);
		if (num > 1)
		{
			viewRect.height += crdMapOffset.y * (float)(num - 1);
		}
		umiScrollPosition = GUI.BeginScrollView(crdUserMapRect, umiScrollPosition, viewRect);
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				int num2 = 3 * i + j;
				if (num2 < umi.Length)
				{
					Texture2D texture2D = null;
					texture2D = ((umi[num2].Alias.Length <= 0) ? emptySlot : ((!(umi[num2].Thumbnail == null)) ? umi[num2].Thumbnail : nonAvailable));
					Rect rect = new Rect((float)j * (crdMapSize.x + crdMapOffset.x), (float)i * (crdMapSize.y + crdMapOffset.y), crdMapSize.x, crdMapSize.y);
					Rect position = new Rect(rect.x, rect.y, rect.width, rect.width);
					TextureUtil.DrawTexture(position, texture2D, ScaleMode.StretchToFill);
					if (GlobalVars.Instance.MyButton(rect, string.Empty, (!umi[num2].IsPremium) ? "BoxMapSelectBorder" : "BoxMapSelectBorderPremium") && (!umi[num2].IsPremium || flag))
					{
						umiSlot = umi[num2].Slot;
						if (Time.time - lastClickTime > doubleClickTimeout)
						{
							lastClickTime = Time.time;
						}
						else
						{
							flag2 = true;
						}
					}
					if (umi[num2].Alias.Length > 0)
					{
						LabelUtil.TextOut(new Vector2(rect.x + crdAlias.x, rect.y + crdAlias.y), umi[num2].Alias, "MiniLabel", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
					}
					bool flag3 = false;
					if (umi[num2].IsPremium)
					{
						if (!flag)
						{
							flag3 = true;
							TextureUtil.DrawTexture(new Rect(rect.x + (rect.width - (float)slotLock.width) / 2f, rect.y + (rect.height - (float)slotLock.height) / 2f - 13f, (float)slotLock.width, (float)slotLock.height), slotLock, ScaleMode.StretchToFill);
						}
						TextureUtil.DrawTexture(new Rect(rect.x + 2f, rect.y + 2f, (float)premiumIcon.width, (float)premiumIcon.height), premiumIcon);
					}
					if (!flag3 && umi[num2].Alias.Length <= 0)
					{
						TextureUtil.DrawTexture(new Rect(rect.x + (rect.width - (float)slotEmpty.width) / 2f, rect.y + (rect.height - (float)slotEmpty.height) / 2f - 13f, (float)slotEmpty.width, (float)slotEmpty.height), slotEmpty, ScaleMode.StretchToFill);
					}
					if (umiSlot == umi[num2].Slot)
					{
						TextureUtil.DrawTexture(rect, selectedMapFrame, ScaleMode.StretchToFill);
					}
				}
			}
		}
		GUI.EndScrollView();
		GUI.Box(crdHLineMapDiv, string.Empty, "DivideLine");
		UserMapInfo userMapInfo = UserMapInfoManager.Instance.Get(umiSlot);
		if (flag2 && userMapInfo != null)
		{
			result = ((userMapInfo.Alias.Length <= 0) ? CreateMapEditorNew(userMapInfo) : CreateMapEditorLoad(userMapInfo));
		}
		GUIContent content = new GUIContent(StringMgr.Instance.Get("CREATE_ROOM").ToUpper(), GlobalVars.Instance.iconBlock);
		if ((GlobalVars.Instance.MyButton3(crdButtonOk, content, "BtnAction") || GlobalVars.Instance.IsReturnPressed()) && userMapInfo != null)
		{
			result = ((userMapInfo.Alias.Length <= 0) ? CreateMapEditorNew(userMapInfo) : CreateMapEditorLoad(userMapInfo));
		}
		bool enabled = GUI.enabled;
		GUI.enabled = (userMapInfo != null && userMapInfo.Alias.Length <= 0);
		string text = newMapName;
		TextureUtil.DrawTexture(crdNewMapNamePoint, mapIcon, ScaleMode.StretchToFill);
		LabelUtil.TextOut(crdNewMapName, StringMgr.Instance.Get("MAP_NAME"), "Label", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		newMapName = GUI.TextField(crdNewMapNameTxtFld, newMapName);
		if (newMapName.Length > maxMapName)
		{
			newMapName = text;
		}
		int num3 = skyboxes.Length / xCount;
		if (skyboxes.Length % xCount > 0)
		{
			num3++;
		}
		TextureUtil.DrawTexture(crdSkyboxPoint, mapIcon, ScaleMode.StretchToFill);
		LabelUtil.TextOut(crdSkyboxLabel, StringMgr.Instance.Get("SKYBOX"), "Label", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.LowerLeft);
		Rect rect2 = new Rect(0f, 0f, (float)xCount * crdSkyboxSize.x, (float)num3 * crdSkyboxSize.y);
		skyboxScrollPosition = GUI.BeginScrollView(crdSkyboxView, skyboxScrollPosition, rect2);
		skybox = GUI.SelectionGrid(rect2, skybox, skyboxes, xCount, "SelRect");
		GUI.EndScrollView();
		num3 = landscapes.Length / xCount;
		if (landscapes.Length % xCount > 0)
		{
			num3++;
		}
		TextureUtil.DrawTexture(crdLandscapePoint, mapIcon, ScaleMode.StretchToFill);
		LabelUtil.TextOut(crdLandscapeLabel, StringMgr.Instance.Get("LANDSCAPE"), "Label", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.LowerLeft);
		Rect rect3 = new Rect(0f, 0f, (float)xCount * crdLandscapeSize.x, (float)num3 * crdLandscapeSize.y);
		landscapeScrollPosition = GUI.BeginScrollView(crdLandscapeView, landscapeScrollPosition, rect3);
		landscape = GUI.SelectionGrid(rect3, landscape, landscapes, xCount, "SelRect");
		GUI.EndScrollView();
		GUI.enabled = enabled;
		return result;
	}

	private int GetRoomTypeIndex(Room.ROOM_TYPE roomType)
	{
		for (int i = 0; i < roomTypes.Length; i++)
		{
			if (roomTypes[i] == roomType)
			{
				return i;
			}
		}
		return 0;
	}

	private bool IsClanChannelAndClanMember()
	{
		if (ChannelManager.Instance.CurChannel.Mode != 4)
		{
			return false;
		}
		if (MyInfoManager.Instance.ClanName.Length == 0)
		{
			return false;
		}
		return true;
	}

	private void SendCS_CREATE_ROOM_REQ()
	{
		if (GlobalVars.Instance.clanSendSqudREQ == -1)
		{
			GlobalVars.Instance.clanSendSqudREQ = 0;
			GlobalVars.Instance.clanMatchMaxPlayer = maxPlayers / 2;
			GlobalVars.Instance.ENTER_SQUADING_ACK();
		}
	}

	private bool UpdateCreateRoom()
	{
		if (GlobalVars.Instance.clanSendSqudREQ == 2)
		{
			GlobalVars.Instance.clanSendSqudREQ = -1;
			return true;
		}
		return false;
	}
}
