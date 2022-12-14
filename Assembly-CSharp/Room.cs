using UnityEngine;

public class Room
{
	public enum COLUMN
	{
		NO,
		TYPE,
		TITLE,
		STATUS,
		NUM_PLAYER,
		MAP_ALIAS,
		PING,
		COUNTRY,
		WPN_OPT
	}

	public enum ROOM_TYPE
	{
		NONE = -1,
		MAP_EDITOR,
		TEAM_MATCH,
		INDIVIDUAL,
		CAPTURE_THE_FLAG,
		EXPLOSION,
		MISSION,
		BND,
		BUNGEE,
		ESCAPE,
		ZOMBIE,
		NUM_TYPE
	}

	public enum MODE_MASK
	{
		TEAM_MATCH_MASK = 1,
		INDIVIDUAL_MATCH_MASK = 2,
		CAPTURE_THE_FALG_MATCH = 4,
		EXPLOSION_MATCH = 8,
		MISSION_MASK = 0x10,
		BND_MASK = 0x20,
		BUNGEE_MASK = 0x40,
		ESCAPE_MASK = 0x80,
		ZOMBIE_MASK = 0x100,
		ALL_MASK = 0x7FFF
	}

	public enum ROOM_STATUS
	{
		NONE = -1,
		WAITING,
		PENDING,
		PLAYING,
		MATCHING,
		MATCH_END
	}

	public static string[] sceneByMode = new string[10]
	{
		"MapEditor",
		"TeamMatch",
		"IndividualMatch",
		"CaptureTheFlagMatch",
		"ExplosionMatch",
		"Defense",
		"BndMatch",
		"Bungee",
		"Escape",
		"Zombie"
	};

	public static string[] typeStringKey = new string[10]
	{
		"ROOM_TYPE_MAP_EDITOR",
		"ROOM_TYPE_TEAM_MATCH",
		"ROOM_TYPE_INDIVIDUAL_MATCH",
		"ROOM_TYPE_CAPTURE_THE_FLAG",
		"ROOM_TYPE_EXPLOSION",
		"ROOM_TYPE_MISSION",
		"ROOM_TYPE_BND",
		"ROOM_TYPE_BUNGEE",
		"ROOM_TYPE_ESCAPE",
		"ROOM_TYPE_ZOMBIE"
	};

	public static ushort[] modeMasks = new ushort[10]
	{
		0,
		1,
		2,
		4,
		8,
		16,
		32,
		64,
		128,
		256
	};

	public static ushort[] modeSelector = new ushort[10]
	{
		32767,
		1,
		2,
		4,
		8,
		16,
		32,
		64,
		128,
		256
	};

	public static byte official = 1;

	public static byte clanMatch = 2;

	public static byte excludeRanking = 4;

	public static byte blocked = 8;

	private static string[] statusStringKey = new string[3]
	{
		"ROOM_STATUS_WAITING",
		"ROOM_STATUS_PENDING",
		"ROOM_STATUS_PLAYING"
	};

	private bool locked;

	public int no;

	private int squad;

	private int squadCounter;

	private string title;

	private ROOM_TYPE type;

	private ROOM_STATUS status;

	private int curPlayer;

	private int maxPlayer;

	private string curMapAlias;

	public int map;

	public int goal;

	public int timelimit;

	public int weaponOption;

	public int ping;

	public int score1;

	public int score2;

	public int CountryFilter;

	public bool isBreakInto;

	public bool isDropItem;

	public bool isWanted;

	public bool Locked
	{
		get
		{
			return locked;
		}
		set
		{
			locked = value;
		}
	}

	public int No => no;

	public int Squad
	{
		get
		{
			return squad;
		}
		set
		{
			squad = value;
		}
	}

	public int SquadCounter
	{
		get
		{
			return squadCounter;
		}
		set
		{
			squadCounter = value;
		}
	}

	public string Title
	{
		get
		{
			return title;
		}
		set
		{
			title = value;
		}
	}

	public ROOM_TYPE Type
	{
		get
		{
			return type;
		}
		set
		{
			type = value;
		}
	}

	public ROOM_STATUS Status
	{
		get
		{
			return status;
		}
		set
		{
			status = value;
		}
	}

	public int CurPlayer
	{
		get
		{
			return curPlayer;
		}
		set
		{
			curPlayer = value;
		}
	}

	public int MaxPlayer
	{
		get
		{
			return maxPlayer;
		}
		set
		{
			maxPlayer = value;
		}
	}

	public string CurMapAlias
	{
		get
		{
			return curMapAlias;
		}
		set
		{
			curMapAlias = value;
		}
	}

	public Room(Room src)
	{
		locked = src.Locked;
		no = src.No;
		title = src.Title;
		type = src.Type;
		status = src.Status;
		curPlayer = src.CurPlayer;
		maxPlayer = src.MaxPlayer;
		map = src.map;
		curMapAlias = src.curMapAlias;
		goal = src.goal;
		timelimit = src.timelimit;
		weaponOption = src.weaponOption;
		ping = src.ping;
		score1 = src.score1;
		score2 = src.score2;
		CountryFilter = src.CountryFilter;
		isBreakInto = src.isBreakInto;
		isDropItem = src.isDropItem;
		isWanted = src.isWanted;
		squad = src.squad;
		squadCounter = src.squadCounter;
	}

	public Room(bool isLocked, int roomNo, string roomTitle, ROOM_TYPE roomType, ROOM_STATUS roomStatus, int cur, int max, int mapId, string alias, int roomGoal, int roomTimelimit, int RoomWeaponOption, int RoomPing, int RoomScore1, int RoomScore2, int countryFilter, bool breakInto, bool wanted, bool dropItem, int _squad, int _squadCounter)
	{
		locked = isLocked;
		no = roomNo;
		title = roomTitle;
		type = roomType;
		status = roomStatus;
		curPlayer = cur;
		maxPlayer = max;
		map = mapId;
		curMapAlias = alias;
		goal = roomGoal;
		timelimit = roomTimelimit;
		weaponOption = RoomWeaponOption;
		ping = RoomPing;
		score1 = RoomScore1;
		score2 = RoomScore2;
		CountryFilter = countryFilter;
		isBreakInto = breakInto;
		isWanted = wanted;
		isDropItem = dropItem;
		squad = _squad;
		squadCounter = _squadCounter;
	}

	public static bool IsPlayingScene()
	{
		for (int i = 0; i < sceneByMode.Length; i++)
		{
			if (Application.loadedLevelName == sceneByMode[i])
			{
				return true;
			}
		}
		return false;
	}

	public static string ModeMask2String(ushort modeMask)
	{
		string text = string.Empty;
		for (int i = 0; i < modeMasks.Length; i++)
		{
			if ((modeMask & modeMasks[i]) != 0)
			{
				if (text.Length > 0)
				{
					text += ", ";
				}
				text += StringMgr.Instance.Get(typeStringKey[i]);
			}
		}
		return text;
	}

	public int Compare(Room arg, COLUMN sortedBy, bool ascending)
	{
		int num = 0;
		switch (sortedBy)
		{
		case COLUMN.NO:
			num = no.CompareTo(arg.No);
			break;
		case COLUMN.TYPE:
		{
			int num3 = (int)type;
			num = num3.CompareTo((int)arg.Type);
			break;
		}
		case COLUMN.TITLE:
			num = title.CompareTo(arg.Title);
			break;
		case COLUMN.STATUS:
		{
			int num2 = (int)status;
			num = num2.CompareTo((int)arg.Status);
			break;
		}
		case COLUMN.NUM_PLAYER:
			num = curPlayer.CompareTo(arg.CurPlayer);
			break;
		case COLUMN.MAP_ALIAS:
			num = curMapAlias.CompareTo(arg.curMapAlias);
			break;
		case COLUMN.PING:
			num = ping.CompareTo(arg.ping);
			break;
		case COLUMN.COUNTRY:
			num = CountryFilter.CompareTo(arg.CountryFilter);
			break;
		case COLUMN.WPN_OPT:
			num = weaponOption.CompareTo(arg.weaponOption);
			break;
		}
		if (!ascending)
		{
			num = -num;
		}
		return num;
	}

	public string GetString(COLUMN col)
	{
		switch (col)
		{
		case COLUMN.NO:
			return (no + 1).ToString();
		case COLUMN.TYPE:
			return StringMgr.Instance.Get(typeStringKey[(int)type]);
		case COLUMN.TITLE:
			return title;
		case COLUMN.STATUS:
			return StringMgr.Instance.Get(statusStringKey[(int)status]);
		case COLUMN.NUM_PLAYER:
			return curPlayer.ToString() + "/" + maxPlayer.ToString();
		case COLUMN.MAP_ALIAS:
			return curMapAlias;
		case COLUMN.PING:
			return ping.ToString();
		case COLUMN.COUNTRY:
			return CountryFilter.ToString();
		case COLUMN.WPN_OPT:
			return weaponOption.ToString();
		default:
			return string.Empty;
		}
	}

	public string GetString()
	{
		return "[" + GetString(COLUMN.NO) + "] " + GetString(COLUMN.TITLE);
	}

	public static string Status2String(int status)
	{
		if (0 > status || status >= statusStringKey.Length)
		{
			return string.Empty;
		}
		return StringMgr.Instance.Get(statusStringKey[status]);
	}

	public static string Type2String(int type)
	{
		if (0 > type || type >= typeStringKey.Length)
		{
			return string.Empty;
		}
		return StringMgr.Instance.Get(typeStringKey[type]);
	}

	public static string Type2String(ROOM_TYPE type)
	{
		if (ROOM_TYPE.MAP_EDITOR > type || (int)type >= typeStringKey.Length)
		{
			return string.Empty;
		}
		return StringMgr.Instance.Get(typeStringKey[(int)type]);
	}
}
