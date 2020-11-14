using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class RoomConfigDialog : Dialog
{
	public int maxRoomTitle = 16;

	public int maxRoomPswd = 4;

	public int maxMapName = 16;

	public float doubleClickTimeout = 0.3f;

	public Texture2D nonAvailable;

	public Texture2D selectedMapFrame;

	public string[] modeOptKeys;

	public string[] weaponOptKeys;

	public int[] killCounts;

	public int[] timeLimits;

	public int[] waveTimeLimits;

	public int[] buildPhaseTimes;

	public int[] battlePhaseTimes;

	public int[] repeats;

	public string[] roundOptions;

	public int[] rounds;

	public string[] nuclearKeys;

	public int[] bungeeTimes;

	public int[] arrivalCounts;

	private Room room;

	private string roomPswd = string.Empty;

	private int option;

	private float lastClickTime;

	private int weaponOpt;

	private int killCount;

	private int timeLimit;

	private int round;

	private int waveTimeLimit;

	private int nuclear;

	private int arrivalCount = 2;

	private bool[] IsSupportClanMode;

	private float crdBoxOffset = 25f;

	private bool breakInto;

	private bool teamBalance;

	private bool itemPickup;

	private bool useBuildGun;

	private bool useWeaponOption;

	private bool wanted;

	private int buildPhaseTime;

	private int battlePhaseTime;

	private int repeat;

	private string[] weaponOptions;

	private string[] killCountOptions;

	private string[] timeLimitOptions;

	private string[] waveTimeLimitOptions;

	private string[] buildPhaseTimeOptions;

	private string[] battlePhaseTimeOptions;

	private string[] repeatOptions;

	private string[] nuclearOptions;

	private string[] bungeeTimeLimitOptions;

	private string[] arrivalCountOptions;

	private RegMap[] reg;

	private Vector2 regMapScrollPosition = Vector2.zero;

	private int regMap;

	public Texture2D lockedIcon;

	private Rect crdOutline = new Rect(13f, 54f, 780f, 667f);

	public Vector2 crdRoomTitleText = new Vector2(45f, 12f);

	public Vector2 crdRoomTitle = new Vector2(100f, 12f);

	private Rect crdLockedIcon = new Rect(202f, 143f, 18f, 9f);

	public Rect crdVLine = new Rect(240f, 34f, 1f, 186f);

	private Vector2 crdPswdLbl = new Vector2(35f, 113f);

	private Rect crdPswdFld = new Rect(28f, 135f, 200f, 25f);

	private Vector2 crdNumPlayerSet = new Vector2(35f, 87f);

	private Vector2 crdNumPlayer = new Vector2(225f, 87f);

	private Rect crdNumPlayerBox = new Rect(28f, 83f, 200f, 27f);

	private string[] BattleModes;

	private Room.ROOM_TYPE[] roomTypes;

	private Vector2 optionScrollPosition = Vector2.zero;

	private Vector2 crdModeTitleL = new Vector2(27f, 180f);

	private Vector2 modeSize = new Vector2(190f, 22f);

	private Rect crdModeView = new Rect(26f, 192f, 210f, 180f);

	private Rect crdOption = new Rect(12f, 370f, 230f, 432f);

	private Vector2 crdOptionBG = new Vector2(178f, 18f);

	private Vector2 crdArrow = new Vector2(22f, 18f);

	private Rect crdMapView = new Rect(269f, 79f, 500f, 622f);

	private Vector2 crdMapSize = new Vector2(150f, 196f);

	private float crdMapOffset = 15f;

	private Vector2 crdDeveloper = new Vector2(5f, 157f);

	private Vector2 crdAlias = new Vector2(5f, 174f);

	private Rect crdBtnOk = new Rect(654f, 726f, 142f, 34f);

	private Color txtMainClr;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.ROOM_CONFIG;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
		txtMainClr = GlobalVars.Instance.txtMainColor;
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
		if (BuildOption.Instance.Props.UseItemDrop)
		{
			itemPickup = true;
		}
		wanted = false;
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
		nuclearOptions = new string[nuclearKeys.Length];
		for (int m = 0; m < nuclearKeys.Length; m++)
		{
			nuclearOptions[m] = StringMgr.Instance.Get(nuclearKeys[m]);
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

	private int RoomType2Option(Room.ROOM_TYPE roomType)
	{
		for (int i = 0; i < roomTypes.Length; i++)
		{
			if (roomType == roomTypes[i])
			{
				return i;
			}
		}
		return roomTypes.Length;
	}

	public void InitDialog(Room _room)
	{
		Init();
		room = _room;
		option = RoomType2Option(room.Type);
		SetupREG();
		round = -1;
		killCount = -1;
		timeLimit = -1;
		weaponOpt = -1;
		buildPhaseTime = -1;
		battlePhaseTime = -1;
		repeat = -1;
		if (room.Type == Room.ROOM_TYPE.EXPLOSION)
		{
			int num = 0;
			while (round < 0 && num < rounds.Length)
			{
				if (rounds[num] == RoomManager.Instance.KillCount)
				{
					round = num;
				}
				num++;
			}
		}
		else if (room.Type == Room.ROOM_TYPE.ESCAPE)
		{
			int num2 = 0;
			while (round < 0 && num2 < rounds.Length)
			{
				if (arrivalCounts[num2] == RoomManager.Instance.KillCount)
				{
					arrivalCount = num2;
				}
				num2++;
			}
		}
		else
		{
			int num3 = 0;
			while (killCount < 0 && num3 < killCounts.Length)
			{
				if (killCounts[num3] == RoomManager.Instance.KillCount)
				{
					killCount = num3;
				}
				num3++;
			}
		}
		if (room.Type == Room.ROOM_TYPE.ZOMBIE)
		{
			int num4 = 0;
			while (round < 0 && num4 < rounds.Length)
			{
				if (rounds[num4] == RoomManager.Instance.KillCount)
				{
					round = num4;
				}
				num4++;
			}
		}
		if (room.Type == Room.ROOM_TYPE.BND)
		{
			int num5 = BndTimer.BuildPhaseTime(RoomManager.Instance.TimeLimit);
			int num6 = BndTimer.BattlePhaseTime(RoomManager.Instance.TimeLimit);
			int num7 = BndTimer.Repeat(RoomManager.Instance.TimeLimit);
			int num8 = 0;
			while (buildPhaseTime < 0 && num8 < buildPhaseTimes.Length)
			{
				if (buildPhaseTimes[num8] == num5)
				{
					buildPhaseTime = num8;
				}
				num8++;
			}
			int num9 = 0;
			while (battlePhaseTime < 0 && num9 < battlePhaseTimes.Length)
			{
				if (battlePhaseTimes[num9] == num6)
				{
					battlePhaseTime = num9;
				}
				num9++;
			}
			int num10 = 0;
			while (repeat < 0 && num10 < repeats.Length)
			{
				if (repeats[num10] == num7)
				{
					repeat = num10;
				}
				num10++;
			}
		}
		else if (room.Type == Room.ROOM_TYPE.BUNGEE)
		{
			int num11 = 0;
			while (timeLimit < 0 && num11 < bungeeTimes.Length)
			{
				if (bungeeTimes[num11] == RoomManager.Instance.TimeLimit)
				{
					timeLimit = num11;
				}
				num11++;
			}
		}
		else
		{
			int num12 = 0;
			while (timeLimit < 0 && num12 < timeLimits.Length)
			{
				if (timeLimits[num12] == RoomManager.Instance.TimeLimit)
				{
					timeLimit = num12;
				}
				num12++;
			}
		}
		if (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.MISSION)
		{
			weaponOpt = RoomManager.Instance.WeaponOption;
		}
		if (killCount < 0 || killCount >= killCounts.Length)
		{
			killCount = 0;
		}
		if (timeLimit < 0 || timeLimit >= timeLimits.Length)
		{
			timeLimit = 0;
		}
		roomPswd = RoomManager.Instance.Pswd;
		breakInto = RoomManager.Instance.BreakingInto;
		teamBalance = RoomManager.Instance.TeamBalance;
		useBuildGun = RoomManager.Instance.UseBuildGun;
		useWeaponOption = (RoomManager.Instance.WeaponOption != 3);
		itemPickup = RoomManager.Instance.DropItem;
		wanted = RoomManager.Instance.Wanted;
		nuclear = 0;
		bool flag = false;
		int num13 = 0;
		while (!flag && num13 < reg.Length)
		{
			if (reg[num13].Map == RoomManager.Instance.CurMap)
			{
				flag = true;
				regMap = num13;
				int num14 = num13 / 3;
				if (num14 % 3 > 0)
				{
					num14++;
				}
				regMapScrollPosition = new Vector2(0f, (num14 <= 0) ? 0f : ((float)(132 * (num14 - 1))));
			}
			num13++;
		}
	}

	private void SetupREG()
	{
		reg = RegMapManager.Instance.ToArray(option, (Channel.MODE)ChannelManager.Instance.CurChannel.Mode);
		reg = reg.OrderBy(x => x.Alias).ToArray();
	}

	private void DoTitleAndPswd()
	{
		LabelUtil.TextOut(crdRoomTitleText, StringMgr.Instance.Get("ROOM_TITLE"), "BigLabel", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		LabelUtil.TextOut(crdRoomTitle, room.Title, "BigLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdPswdLbl, StringMgr.Instance.Get("PASSWORD"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		string text = roomPswd;
		roomPswd = GUI.PasswordField(crdPswdFld, roomPswd, '*');
		if (roomPswd.Length > maxRoomPswd)
		{
			roomPswd = text;
		}
		TextureUtil.DrawTexture(crdLockedIcon, lockedIcon, ScaleMode.StretchToFill);
	}

	private void VerifyDefenseModeOptions()
	{
		if (nuclear < 0)
		{
			nuclear = 0;
		}
		if (nuclear >= nuclearKeys.Length)
		{
			nuclear = nuclearKeys.Length - 1;
		}
		if (waveTimeLimit < 0)
		{
			waveTimeLimit = 0;
		}
		if (waveTimeLimit >= waveTimeLimits.Length)
		{
			waveTimeLimit = waveTimeLimits.Length - 1;
		}
	}

	private void DoDefenseModeOption()
	{
		VerifyDefenseModeOptions();
		GUI.BeginGroup(crdOption);
		float num = 0f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("NUCLEAR_LIFE"), "MiniLabel", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += 22f;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			nuclear--;
			if (nuclear < 0)
			{
				nuclear = 0;
			}
		}
		GUI.Box(new Rect(25f, num + 2f, crdOptionBG.x, crdOptionBG.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), nuclearOptions[nuclear], "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			nuclear++;
			if (nuclear >= nuclearKeys.Length)
			{
				nuclear = nuclearKeys.Length - 1;
			}
		}
		num += 21f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("TIME_LIMIT"), "MiniLabel", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += 22f;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			waveTimeLimit--;
			if (waveTimeLimit < 0)
			{
				waveTimeLimit = 0;
			}
		}
		GUI.Box(new Rect(25f, num, crdOptionBG.x, crdOptionBG.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num - 2f), waveTimeLimitOptions[waveTimeLimit] + StringMgr.Instance.Get("MINUTES"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			waveTimeLimit++;
			if (waveTimeLimit >= waveTimeLimits.Length)
			{
				waveTimeLimit = waveTimeLimits.Length - 1;
			}
		}
		num += crdBoxOffset;
		breakInto = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), breakInto, StringMgr.Instance.Get("BREAK_INTO"));
		if (BuildOption.Instance.Props.UseItemDrop)
		{
			num += crdBoxOffset;
			itemPickup = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), itemPickup, StringMgr.Instance.Get("ROOM_SET_ITEMDROP"));
		}
		GUI.EndGroup();
	}

	private void VerifyIndividualMatchOptions()
	{
		if (timeLimit < 0)
		{
			timeLimit = 0;
		}
		if (timeLimit >= timeLimits.Length)
		{
			timeLimit = timeLimits.Length - 1;
		}
		if (killCount < 0)
		{
			killCount = 0;
		}
		if (killCount >= killCounts.Length)
		{
			killCount = killCounts.Length - 1;
		}
		if (weaponOpt < 0)
		{
			weaponOpt = 0;
		}
		if (weaponOpt >= weaponOptions.Length)
		{
			weaponOpt = weaponOptions.Length - 1;
		}
	}

	private void DoIndividualMatchOption()
	{
		VerifyIndividualMatchOptions();
		GUI.BeginGroup(crdOption);
		float num = 0f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("TIME_LIMIT"), "MiniLabel", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += 22f;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			timeLimit--;
			if (timeLimit < 0)
			{
				timeLimit = 0;
			}
		}
		GUI.Box(new Rect(25f, num + 2f, crdOptionBG.x, crdOptionBG.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), timeLimitOptions[timeLimit] + StringMgr.Instance.Get("MINUTES"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			timeLimit++;
			if (timeLimit >= timeLimits.Length)
			{
				timeLimit = timeLimits.Length - 1;
			}
		}
		num += 21f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("KILL_COUNT"), "MiniLabel", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += 22f;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			killCount--;
			if (killCount < 0)
			{
				killCount = 0;
			}
		}
		GUI.Box(new Rect(25f, num + 2f, crdOptionBG.x, crdOptionBG.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), killCountOptions[killCount] + StringMgr.Instance.Get("KILL"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			killCount++;
			if (killCount >= killCounts.Length)
			{
				killCount = killCounts.Length - 1;
			}
		}
		num += 21f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("WEAPON_OPTION"), "MiniLabel", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += 22f;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			weaponOpt--;
			if (weaponOpt < 0)
			{
				weaponOpt = 0;
			}
		}
		GUI.Box(new Rect(25f, num + 2f, crdOptionBG.x, crdOptionBG.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), weaponOptions[weaponOpt], "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			weaponOpt++;
			if (weaponOpt >= weaponOptions.Length)
			{
				weaponOpt = weaponOptions.Length - 1;
			}
		}
		num += crdBoxOffset;
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

	private void VerifyTeamMatchOptions()
	{
		if (timeLimit < 0)
		{
			timeLimit = 0;
		}
		if (timeLimit >= timeLimits.Length)
		{
			timeLimit = timeLimits.Length - 1;
		}
		if (killCount < 0)
		{
			killCount = 0;
		}
		if (killCount >= killCounts.Length)
		{
			killCount = killCounts.Length - 1;
		}
		if (weaponOpt < 0)
		{
			weaponOpt = 0;
		}
		if (weaponOpt >= weaponOptions.Length)
		{
			weaponOpt = weaponOptions.Length - 1;
		}
	}

	private void DoTeamMatchOption()
	{
		VerifyTeamMatchOptions();
		GUI.BeginGroup(crdOption);
		float num = 0f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("TIME_LIMIT"), "MiniLabel", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += 22f;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			timeLimit--;
			if (timeLimit < 0)
			{
				timeLimit = 0;
			}
		}
		GUI.Box(new Rect(25f, num + 2f, crdOptionBG.x, crdOptionBG.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), timeLimitOptions[timeLimit] + StringMgr.Instance.Get("MINUTES"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			timeLimit++;
			if (timeLimit >= timeLimits.Length)
			{
				timeLimit = timeLimits.Length - 1;
			}
		}
		num += 21f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("KILL_COUNT"), "MiniLabel", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += 22f;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			killCount--;
			if (killCount < 0)
			{
				killCount = 0;
			}
		}
		GUI.Box(new Rect(25f, num + 2f, crdOptionBG.x, crdOptionBG.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), killCountOptions[killCount] + StringMgr.Instance.Get("KILL"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			killCount++;
			if (killCount >= killCounts.Length)
			{
				killCount = killCounts.Length - 1;
			}
		}
		num += 21f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("WEAPON_OPTION"), "MiniLabel", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += 22f;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			weaponOpt--;
			if (weaponOpt < 0)
			{
				weaponOpt = 0;
			}
		}
		GUI.Box(new Rect(25f, num + 2f, crdOptionBG.x, crdOptionBG.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), weaponOptions[weaponOpt], "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			weaponOpt++;
			if (weaponOpt >= weaponOptions.Length)
			{
				weaponOpt = weaponOptions.Length - 1;
			}
		}
		num += crdBoxOffset;
		breakInto = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), breakInto, StringMgr.Instance.Get("BREAK_INTO"));
		num += crdBoxOffset;
		teamBalance = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), teamBalance, StringMgr.Instance.Get("TEAM_BALANCE"));
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

	private void VerifyBndOption()
	{
		if (buildPhaseTime < 0)
		{
			buildPhaseTime = 0;
		}
		if (buildPhaseTime >= buildPhaseTimes.Length)
		{
			buildPhaseTime = buildPhaseTimes.Length - 1;
		}
		if (battlePhaseTime < 0)
		{
			battlePhaseTime = 0;
		}
		if (battlePhaseTime >= battlePhaseTimes.Length)
		{
			battlePhaseTime = battlePhaseTimes.Length - 1;
		}
		if (repeat < 0)
		{
			repeat = 0;
		}
		if (repeat >= repeatOptions.Length)
		{
			repeat = repeatOptions.Length - 1;
		}
		if (killCount < 0)
		{
			killCount = 0;
		}
		if (killCount >= killCounts.Length)
		{
			killCount = killCounts.Length - 1;
		}
		if (weaponOpt < 0)
		{
			weaponOpt = 0;
		}
		if (weaponOpt >= weaponOptions.Length)
		{
			weaponOpt = weaponOptions.Length - 1;
		}
	}

	private void DoBndOption()
	{
		VerifyBndOption();
		GUI.BeginGroup(crdOption);
		float num = 0f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("BUILD_PHASE_TIME"), "MiniLabel", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += 22f;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			buildPhaseTime--;
			if (buildPhaseTime < 0)
			{
				buildPhaseTime = 0;
			}
		}
		GUI.Box(new Rect(25f, num + 2f, crdOptionBG.x, crdOptionBG.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), buildPhaseTimeOptions[buildPhaseTime] + StringMgr.Instance.Get("MINUTES"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			buildPhaseTime++;
			if (buildPhaseTime >= buildPhaseTimes.Length)
			{
				buildPhaseTime = buildPhaseTimes.Length - 1;
			}
		}
		num += 21f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("SHOOT_PHASE_TIME"), "MiniLabel", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += 22f;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			battlePhaseTime--;
			if (battlePhaseTime < 0)
			{
				battlePhaseTime = 0;
			}
		}
		GUI.Box(new Rect(25f, num + 2f, crdOptionBG.x, crdOptionBG.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), battlePhaseTimeOptions[battlePhaseTime] + StringMgr.Instance.Get("MINUTES"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			battlePhaseTime++;
			if (battlePhaseTime >= battlePhaseTimes.Length)
			{
				battlePhaseTime = battlePhaseTimes.Length - 1;
			}
		}
		num += 21f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("BND_REPEAT"), "MiniLabel", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += 22f;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			repeat--;
			if (repeat < 0)
			{
				repeat = 0;
			}
		}
		GUI.Box(new Rect(25f, num + 2f, crdOptionBG.x, crdOptionBG.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), repeatOptions[repeat] + StringMgr.Instance.Get("TIMES_UNIT"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			repeat++;
			if (repeat >= repeatOptions.Length)
			{
				repeat = repeatOptions.Length - 1;
			}
		}
		num += 21f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("KILL_COUNT"), "MiniLabel", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += 22f;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			killCount--;
			if (killCount < 0)
			{
				killCount = 0;
			}
		}
		GUI.Box(new Rect(25f, num + 2f, crdOptionBG.x, crdOptionBG.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), killCountOptions[killCount] + StringMgr.Instance.Get("KILL"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			killCount++;
			if (killCount >= killCounts.Length)
			{
				killCount = killCounts.Length - 1;
			}
		}
		num += 21f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("WEAPON_OPTION"), "MiniLabel", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += 22f;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			weaponOpt--;
			if (weaponOpt < 0)
			{
				weaponOpt = 0;
			}
		}
		GUI.Box(new Rect(25f, num + 2f, crdOptionBG.x, crdOptionBG.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), weaponOptions[weaponOpt], "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			weaponOpt++;
			if (weaponOpt >= weaponOptions.Length)
			{
				weaponOpt = weaponOptions.Length - 1;
			}
		}
		num += crdBoxOffset;
		breakInto = GUI.Toggle(new Rect(5f, num, crdOption.width - 22f, 22f), breakInto, StringMgr.Instance.Get("BREAK_INTO"));
		num += crdBoxOffset;
		teamBalance = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), teamBalance, StringMgr.Instance.Get("TEAM_BALANCE"));
		itemPickup = false;
		if (BuildOption.Instance.AllowBuildGunInDestroyPhase())
		{
			num += 30f;
			useBuildGun = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), useBuildGun, StringMgr.Instance.Get("USE_BUILD_GUN"));
		}
		GUI.EndGroup();
	}

	private bool DoBnd()
	{
		bool result = false;
		DoBndOption();
		if ((DoRegMap() || GlobalVars.Instance.MyButton(crdBtnOk, StringMgr.Instance.Get("OK"), "BtnAction")) && CheckConfig())
		{
			int num = BndTimer.PackTimerOption(buildPhaseTimes[buildPhaseTime], battlePhaseTimes[battlePhaseTime], repeats[repeat]);
			CSNetManager.Instance.Sock.SendCS_ROOM_CONFIG_REQ(killCounts[killCount], num, weaponOpt, reg[regMap].Map, breakInto ? 1 : 0, teamBalance ? 1 : 0, itemPickup ? 1 : 0, useBuildGun ? 1 : 0, reg[regMap].Alias, roomPswd, (int)roomTypes[option]);
			result = true;
		}
		return result;
	}

	private bool DoTeamMatch()
	{
		bool result = false;
		DoTeamMatchOption();
		if (DoRegMap() || GlobalVars.Instance.MyButton(crdBtnOk, StringMgr.Instance.Get("OK"), "BtnAction"))
		{
			if (CheckConfig())
			{
				CSNetManager.Instance.Sock.SendCS_ROOM_CONFIG_REQ(killCounts[killCount], timeLimits[timeLimit], weaponOpt, reg[regMap].Map, breakInto ? 1 : 0, teamBalance ? 1 : 0, itemPickup ? 1 : 0, wanted ? 1 : 0, reg[regMap].Alias, roomPswd, (int)roomTypes[option]);
				result = true;
			}
			if (ChannelManager.Instance.CurChannel.Mode == 4)
			{
				CSNetManager.Instance.Sock.SendCS_CHG_SQUAD_OPTION_REQ(reg[regMap].Map, (int)room.Type, room.MaxPlayer / 2);
			}
		}
		return result;
	}

	private bool DoIndividualMatch()
	{
		bool result = false;
		DoIndividualMatchOption();
		if ((DoRegMap() || GlobalVars.Instance.MyButton(crdBtnOk, StringMgr.Instance.Get("OK"), "BtnAction")) && CheckConfig())
		{
			CSNetManager.Instance.Sock.SendCS_ROOM_CONFIG_REQ(killCounts[killCount], timeLimits[timeLimit], weaponOpt, reg[regMap].Map, breakInto ? 1 : 0, teamBalance ? 1 : 0, itemPickup ? 1 : 0, wanted ? 1 : 0, reg[regMap].Alias, roomPswd, (int)roomTypes[option]);
			result = true;
		}
		return result;
	}

	private bool DoDefense()
	{
		bool result = false;
		DoDefenseModeOption();
		if ((DoRegMap() || GlobalVars.Instance.MyButton(crdBtnOk, StringMgr.Instance.Get("OK"), "BtnAction")) && CheckConfig())
		{
			int num = Convert.ToInt32(nuclearOptions[nuclear]);
			CSNetManager.Instance.Sock.SendCS_ROOM_CONFIG_REQ(num, waveTimeLimits[waveTimeLimit], weaponOpt, reg[regMap].Map, breakInto ? 1 : 0, teamBalance ? 1 : 0, itemPickup ? 1 : 0, 0, reg[regMap].Alias, roomPswd, (int)roomTypes[option]);
			result = true;
		}
		return result;
	}

	private void VerifyCTFModeOptions()
	{
		if (timeLimit < 0)
		{
			timeLimit = 0;
		}
		if (timeLimit >= timeLimits.Length)
		{
			timeLimit = timeLimits.Length - 1;
		}
		if (weaponOpt < 0)
		{
			weaponOpt = 0;
		}
		if (weaponOpt >= weaponOptions.Length)
		{
			weaponOpt = weaponOptions.Length - 1;
		}
	}

	private void DoCTFModeOption()
	{
		VerifyCTFModeOptions();
		GUI.BeginGroup(crdOption);
		float num = 0f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("TIME_LIMIT"), "MiniLabel", new Color(0.9f, 0.6f, 0f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += 22f;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			timeLimit--;
			if (timeLimit < 0)
			{
				timeLimit = 0;
			}
		}
		GUI.Box(new Rect(25f, num + 2f, crdOptionBG.x, crdOptionBG.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), timeLimitOptions[timeLimit], "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			timeLimit++;
			if (timeLimit >= timeLimits.Length)
			{
				timeLimit = timeLimits.Length - 1;
			}
		}
		num += 21f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("WEAPON_OPTION"), "MiniLabel", new Color(0.9f, 0.6f, 0f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += 22f;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			weaponOpt--;
			if (weaponOpt < 0)
			{
				weaponOpt = 0;
			}
		}
		GUI.Box(new Rect(25f, num + 2f, crdOptionBG.x, crdOptionBG.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), weaponOptions[weaponOpt], "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			weaponOpt++;
			if (weaponOpt >= weaponOptions.Length)
			{
				weaponOpt = weaponOptions.Length - 1;
			}
		}
		num += crdBoxOffset;
		breakInto = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), breakInto, StringMgr.Instance.Get("BREAK_INTO"));
		num += crdBoxOffset;
		teamBalance = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), teamBalance, StringMgr.Instance.Get("TEAM_BALANCE"));
		if (BuildOption.Instance.Props.UseItemDrop)
		{
			num += crdBoxOffset;
			itemPickup = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), itemPickup, StringMgr.Instance.Get("ROOM_SET_ITEMDROP"));
		}
		GUI.EndGroup();
	}

	private bool DoCTF()
	{
		bool result = false;
		DoCTFModeOption();
		if (DoRegMap() || GlobalVars.Instance.MyButton(crdBtnOk, StringMgr.Instance.Get("OK"), "BtnAction"))
		{
			if (CheckConfig())
			{
				CSNetManager.Instance.Sock.SendCS_ROOM_CONFIG_REQ(killCounts[killCount], timeLimits[timeLimit], weaponOpt, reg[regMap].Map, breakInto ? 1 : 0, teamBalance ? 1 : 0, itemPickup ? 1 : 0, 0, reg[regMap].Alias, roomPswd, (int)roomTypes[option]);
				result = true;
			}
			if (ChannelManager.Instance.CurChannel.Mode == 4)
			{
				CSNetManager.Instance.Sock.SendCS_CHG_SQUAD_OPTION_REQ(reg[regMap].Map, (int)room.Type, room.MaxPlayer / 2);
			}
		}
		return result;
	}

	private bool DoExplosion()
	{
		bool result = false;
		DoExplosionModeOption();
		if (DoRegMap() || GlobalVars.Instance.MyButton(crdBtnOk, StringMgr.Instance.Get("OK"), "BtnAction"))
		{
			if (CheckConfig())
			{
				CSNetManager.Instance.Sock.SendCS_ROOM_CONFIG_REQ(rounds[round], ExplosionMatch.playTimePerRound, weaponOpt, reg[regMap].Map, breakInto ? 1 : 0, teamBalance ? 1 : 0, itemPickup ? 1 : 0, 0, reg[regMap].Alias, roomPswd, (int)roomTypes[option]);
				result = true;
			}
			if (ChannelManager.Instance.CurChannel.Mode == 4)
			{
				CSNetManager.Instance.Sock.SendCS_CHG_SQUAD_OPTION_REQ(reg[regMap].Map, (int)room.Type, room.MaxPlayer / 2);
			}
		}
		return result;
	}

	private void VerifyExplosionModeOptions()
	{
		if (round < 0)
		{
			round = 0;
		}
		if (round >= roundOptions.Length)
		{
			round = roundOptions.Length - 1;
		}
		if (weaponOpt < 0)
		{
			weaponOpt = 0;
		}
		if (weaponOpt >= weaponOptions.Length)
		{
			weaponOpt = weaponOptions.Length - 1;
		}
	}

	private void DoExplosionModeOption()
	{
		VerifyExplosionModeOptions();
		GUI.BeginGroup(crdOption);
		float num = 0f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("ROUND"), "MiniLabel", new Color(0.9f, 0.6f, 0f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += 22f;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			round--;
			if (round < 0)
			{
				round = 0;
			}
		}
		GUI.Box(new Rect(25f, num + 2f, crdOptionBG.x, crdOptionBG.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), roundOptions[round], "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			round++;
			if (round >= roundOptions.Length)
			{
				round = roundOptions.Length - 1;
			}
		}
		num += 21f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("WEAPON_OPTION"), "MiniLabel", new Color(0.9f, 0.6f, 0f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += 22f;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			weaponOpt--;
			if (weaponOpt < 0)
			{
				weaponOpt = 0;
			}
		}
		GUI.Box(new Rect(25f, num + 2f, crdOptionBG.x, crdOptionBG.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), weaponOptions[weaponOpt], "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			weaponOpt++;
			if (weaponOpt >= weaponOptions.Length)
			{
				weaponOpt = weaponOptions.Length - 1;
			}
		}
		num += crdBoxOffset;
		breakInto = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), breakInto, StringMgr.Instance.Get("BREAK_INTO"));
		num += crdBoxOffset;
		teamBalance = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), teamBalance, StringMgr.Instance.Get("TEAM_BALANCE"));
		if (BuildOption.Instance.Props.UseItemDrop)
		{
			num += crdBoxOffset;
			itemPickup = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), itemPickup, StringMgr.Instance.Get("ROOM_SET_ITEMDROP"));
		}
		GUI.EndGroup();
	}

	private bool CheckConfig()
	{
		if (regMap < 0 || regMap >= reg.Length)
		{
			SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("CANT_PLAY_WITHOUT_MAP"));
			return false;
		}
		return true;
	}

	private bool DoRegMap()
	{
		bool result = false;
		int num = reg.Length / 3;
		if (reg.Length % 3 > 0)
		{
			num++;
		}
		Rect viewRect = new Rect(0f, 0f, crdMapSize.x * 3f + crdMapOffset * 2f, crdMapSize.y * (float)num);
		if (num > 1)
		{
			viewRect.height += crdMapOffset * (float)(num - 1);
		}
		regMapScrollPosition = GUI.BeginScrollView(crdMapView, regMapScrollPosition, viewRect);
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				int num2 = 3 * i + j;
				if (num2 < reg.Length)
				{
					Rect rect = new Rect((float)j * (crdMapSize.x + crdMapOffset), (float)i * (crdMapSize.y + crdMapOffset), crdMapSize.x, crdMapSize.y);
					Rect position = new Rect(rect.x, rect.y, rect.width, rect.width + 4f);
					TextureUtil.DrawTexture(position, (!(reg[num2].Thumbnail == null)) ? reg[num2].Thumbnail : nonAvailable);
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
						TextureUtil.DrawTexture(new Rect(rect.x, rect.y, (float)GlobalVars.Instance.iconNewmap.width * 0.7f, (float)GlobalVars.Instance.iconNewmap.height * 0.7f), GlobalVars.Instance.iconNewmap, ScaleMode.StretchToFill);
					}
					else if ((reg[num2].tagMask & 8) != 0)
					{
						TextureUtil.DrawTexture(new Rect(rect.x, rect.y, (float)GlobalVars.Instance.iconglory.width * 0.7f, (float)GlobalVars.Instance.iconglory.height * 0.7f), GlobalVars.Instance.iconglory, ScaleMode.StretchToFill);
					}
					else if ((reg[num2].tagMask & 4) != 0)
					{
						TextureUtil.DrawTexture(new Rect(rect.x, rect.y, (float)GlobalVars.Instance.iconMedal.width * 0.7f, (float)GlobalVars.Instance.iconMedal.height * 0.7f), GlobalVars.Instance.iconMedal, ScaleMode.StretchToFill);
					}
					else if ((reg[num2].tagMask & 2) != 0)
					{
						TextureUtil.DrawTexture(new Rect(rect.x, rect.y, (float)GlobalVars.Instance.icongoldRibbon.width * 0.7f, (float)GlobalVars.Instance.icongoldRibbon.height * 0.7f), GlobalVars.Instance.icongoldRibbon, ScaleMode.StretchToFill);
					}
					if (reg[num2].IsAbuseMap())
					{
						float x = rect.x + rect.width - (float)GlobalVars.Instance.iconDeclare.width * 0.7f;
						TextureUtil.DrawTexture(new Rect(x, rect.y, (float)GlobalVars.Instance.iconDeclare.width * 0.7f, (float)GlobalVars.Instance.iconDeclare.height * 0.7f), GlobalVars.Instance.iconDeclare, ScaleMode.StretchToFill);
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

	private void DoNumPlayers()
	{
		GUI.Box(crdNumPlayerBox, string.Empty, "BoxFadeBlue");
		LabelUtil.TextOut(crdNumPlayerSet, StringMgr.Instance.Get("NUM_PLAYERS_SET"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdNumPlayer, room.MaxPlayer.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		GUI.Box(crdOutline, string.Empty, "LineBoxBlue");
		DoTitleAndPswd();
		DoMode();
		DoNumPlayers();
		switch (roomTypes[option])
		{
		case Room.ROOM_TYPE.TEAM_MATCH:
			result = DoTeamMatch();
			break;
		case Room.ROOM_TYPE.INDIVIDUAL:
			result = DoIndividualMatch();
			break;
		case Room.ROOM_TYPE.MISSION:
			result = DoDefense();
			break;
		case Room.ROOM_TYPE.CAPTURE_THE_FLAG:
			result = DoCTF();
			break;
		case Room.ROOM_TYPE.EXPLOSION:
			result = DoExplosion();
			break;
		case Room.ROOM_TYPE.BND:
			result = DoBnd();
			break;
		case Room.ROOM_TYPE.BUNGEE:
			result = DoBungee();
			break;
		case Room.ROOM_TYPE.ESCAPE:
			result = DoEscape();
			break;
		case Room.ROOM_TYPE.ZOMBIE:
			result = DoZombie();
			break;
		}
		GUI.Box(crdVLine, string.Empty, "DivideLineV");
		Rect rc = new Rect(size.x - 44f, 5f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
		}
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}

	private bool DoBungee()
	{
		bool result = false;
		DoBungeeModeOption();
		if ((DoRegMap() || GlobalVars.Instance.MyButton(crdBtnOk, StringMgr.Instance.Get("OK"), "BtnAction")) && CheckConfig())
		{
			CSNetManager.Instance.Sock.SendCS_ROOM_CONFIG_REQ(killCounts[killCount], bungeeTimes[timeLimit], 0, reg[regMap].Map, breakInto ? 1 : 0, 0, 0, 0, reg[regMap].Alias, roomPswd, (int)roomTypes[option]);
			result = true;
		}
		return result;
	}

	private bool DoEscape()
	{
		bool result = false;
		DoEscapeModeOption();
		if ((DoRegMap() || GlobalVars.Instance.MyButton(crdBtnOk, StringMgr.Instance.Get("OK"), "BtnAction")) && CheckConfig())
		{
			CSNetManager.Instance.Sock.SendCS_ROOM_CONFIG_REQ(arrivalCounts[arrivalCount], timeLimits[timeLimit], (!useWeaponOption) ? 3 : 0, reg[regMap].Map, breakInto ? 1 : 0, 0, itemPickup ? 1 : 0, 0, reg[regMap].Alias, roomPswd, (int)roomTypes[option]);
			result = true;
		}
		return result;
	}

	private bool DoZombie()
	{
		bool result = false;
		DoZombieModeOption();
		if ((DoRegMap() || GlobalVars.Instance.MyButton(crdBtnOk, StringMgr.Instance.Get("OK"), "BtnAction")) && CheckConfig())
		{
			CSNetManager.Instance.Sock.SendCS_ROOM_CONFIG_REQ(rounds[round], ZombieMatch.playTimePerRound, 0, reg[regMap].Map, breakInto ? 1 : 0, 0, 0, 0, reg[regMap].Alias, roomPswd, (int)roomTypes[option]);
			result = true;
		}
		return result;
	}

	private void VerifyBungeeModeOptions()
	{
		if (timeLimit < 0)
		{
			timeLimit = 0;
		}
		if (timeLimit >= bungeeTimeLimitOptions.Length)
		{
			timeLimit = bungeeTimeLimitOptions.Length - 1;
		}
		if (killCount < 0)
		{
			killCount = 0;
		}
		if (killCount >= killCounts.Length)
		{
			killCount = killCounts.Length - 1;
		}
	}

	private void DoBungeeModeOption()
	{
		VerifyBungeeModeOptions();
		GUI.BeginGroup(crdOption);
		float num = 0f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("TIME_LIMIT"), "MiniLabel", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += 22f;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			timeLimit--;
			if (timeLimit < 0)
			{
				timeLimit = 0;
			}
		}
		GUI.Box(new Rect(25f, num + 2f, crdOptionBG.x, crdOptionBG.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), bungeeTimeLimitOptions[timeLimit] + StringMgr.Instance.Get("MINUTES"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			timeLimit++;
			if (timeLimit >= bungeeTimeLimitOptions.Length)
			{
				timeLimit = bungeeTimeLimitOptions.Length - 1;
			}
		}
		num += 21f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("BUNGEE_COUNT"), "MiniLabel", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += 22f;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			killCount--;
			if (killCount < 0)
			{
				killCount = 0;
			}
		}
		GUI.Box(new Rect(25f, num + 2f, crdOptionBG.x, crdOptionBG.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), killCountOptions[killCount] + StringMgr.Instance.Get("KILL"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			killCount++;
			if (killCount >= killCounts.Length)
			{
				killCount = killCounts.Length - 1;
			}
		}
		num += 21f;
		num += crdBoxOffset;
		breakInto = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), breakInto, StringMgr.Instance.Get("BREAK_INTO"));
		GUI.EndGroup();
	}

	private void VerifyEscapeModeOptions()
	{
		if (timeLimit < 0)
		{
			timeLimit = 0;
		}
		if (timeLimit >= timeLimitOptions.Length)
		{
			timeLimit = timeLimitOptions.Length - 1;
		}
		if (arrivalCount < 0)
		{
			arrivalCount = 0;
		}
		if (arrivalCount >= arrivalCounts.Length)
		{
			arrivalCount = arrivalCounts.Length - 1;
		}
	}

	private void DoEscapeModeOption()
	{
		VerifyEscapeModeOptions();
		GUI.BeginGroup(crdOption);
		float num = 0f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("TIME_LIMIT"), "MiniLabel", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += 22f;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			timeLimit--;
			if (timeLimit < 0)
			{
				timeLimit = 0;
			}
		}
		GUI.Box(new Rect(25f, num + 2f, crdOptionBG.x, crdOptionBG.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), timeLimitOptions[timeLimit] + StringMgr.Instance.Get("MINUTES"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			timeLimit++;
			if (timeLimit >= timeLimits.Length)
			{
				timeLimit = timeLimits.Length - 1;
			}
		}
		num += 21f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("ARRIVAL_COUNT"), "MiniLabel", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += 22f;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			arrivalCount--;
			if (arrivalCount < 0)
			{
				arrivalCount = 0;
			}
		}
		GUI.Box(new Rect(25f, num + 2f, crdOptionBG.x, crdOptionBG.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), arrivalCountOptions[arrivalCount] + StringMgr.Instance.Get("TIMES_UNIT"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			arrivalCount++;
			if (arrivalCount >= arrivalCounts.Length)
			{
				arrivalCount = arrivalCounts.Length - 1;
			}
		}
		num += 21f;
		num += 30f;
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

	private void VerifyZombieModeOptions()
	{
		if (round < 0)
		{
			round = 0;
		}
		if (round >= roundOptions.Length)
		{
			round = roundOptions.Length - 1;
		}
	}

	private void DoZombieModeOption()
	{
		VerifyZombieModeOptions();
		GUI.BeginGroup(crdOption);
		float num = 0f;
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), StringMgr.Instance.Get("ROUND"), "MiniLabel", new Color(0.9f, 0.6f, 0f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		num += 22f;
		if (GlobalVars.Instance.MyButton(new Rect(5f, num, crdArrow.x, crdArrow.y), string.Empty, "Left"))
		{
			round--;
			if (round < 0)
			{
				round = 0;
			}
		}
		GUI.Box(new Rect(25f, num + 2f, crdOptionBG.x, crdOptionBG.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdOption.width / 2f, num), roundOptions[round], "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(crdOption.width - crdArrow.x - 5f, num, crdArrow.x, crdArrow.y), string.Empty, "Right"))
		{
			round++;
			if (round >= roundOptions.Length)
			{
				round = roundOptions.Length - 1;
			}
		}
		num += 21f;
		num += 30f;
		breakInto = GUI.Toggle(new Rect(5f, num, crdOption.width - 21f, 22f), breakInto, StringMgr.Instance.Get("BREAK_INTO"));
		GUI.EndGroup();
	}
}
