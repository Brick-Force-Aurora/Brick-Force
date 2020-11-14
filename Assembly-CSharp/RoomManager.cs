using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
	private Dictionary<int, Room> dicDumbRoom;

	private Dictionary<int, Room> dicRoom;

	private Dictionary<string, Room.ROOM_TYPE> battleScene;

	private Dictionary<string, Room.ROOM_TYPE> buildScene;

	private Dictionary<string, Room.ROOM_TYPE> gameScene;

	private Dictionary<string, Room.ROOM_TYPE> vsTeamScene;

	private int currentRoom = -1;

	private int prevRoom = -1;

	private int killCount;

	private int timeLimit;

	private int curMap;

	private string curAlias;

	private int master = -1;

	private string pswd = string.Empty;

	private int weaponOption;

	private bool breakingInto;

	private bool useBuildGun;

	private bool teamBalance;

	private bool dropItem;

	private bool wanted;

	private long premiumBricks;

	private int waveOption;

	public sbyte endCode4Red;

	public sbyte endCode4Blue;

	public sbyte endCode;

	public int redTotalKill;

	public int redTotalDeath;

	public int redScore;

	public int redMission;

	public int blueTotalKill;

	public int blueTotalDeath;

	public int blueScore;

	public int blueMission;

	public byte commented;

	public ResultUnit[] RU;

	public VoteStatus vote;

	private bool[] slotStatus;

	private static RoomManager _instance;

	public int Count => dicRoom.Count;

	public int CurrentRoom
	{
		get
		{
			return currentRoom;
		}
		set
		{
			if (currentRoom >= 0)
			{
				prevRoom = currentRoom;
			}
			currentRoom = value;
		}
	}

	public bool HaveCurrentRoomInfo => dicRoom.ContainsKey(currentRoom);

	public bool IsKindOfIndividual
	{
		get
		{
			if (!dicRoom.ContainsKey(currentRoom))
			{
				return false;
			}
			return dicRoom[currentRoom].Type == Room.ROOM_TYPE.INDIVIDUAL || dicRoom[currentRoom].Type == Room.ROOM_TYPE.BUNGEE || dicRoom[currentRoom].Type == Room.ROOM_TYPE.ESCAPE || dicRoom[currentRoom].Type == Room.ROOM_TYPE.ZOMBIE;
		}
	}

	public Room.ROOM_TYPE CurrentRoomType
	{
		get
		{
			if (!dicRoom.ContainsKey(currentRoom))
			{
				return Room.ROOM_TYPE.NONE;
			}
			return dicRoom[currentRoom].Type;
		}
	}

	public Room.ROOM_STATUS CurrentRoomStatus
	{
		get
		{
			if (!dicRoom.ContainsKey(currentRoom))
			{
				return Room.ROOM_STATUS.NONE;
			}
			return dicRoom[currentRoom].Status;
		}
	}

	public int KillCount
	{
		get
		{
			return killCount;
		}
		set
		{
			killCount = value;
		}
	}

	public int TimeLimit
	{
		get
		{
			return timeLimit;
		}
		set
		{
			timeLimit = value;
		}
	}

	public int CurMap
	{
		get
		{
			return curMap;
		}
		set
		{
			curMap = value;
		}
	}

	public string CurAlias
	{
		get
		{
			return curAlias;
		}
		set
		{
			curAlias = value;
		}
	}

	public int Master
	{
		get
		{
			return master;
		}
		set
		{
			master = value;
		}
	}

	public string Pswd
	{
		get
		{
			return pswd;
		}
		set
		{
			pswd = value;
		}
	}

	public int WeaponOption
	{
		get
		{
			return weaponOption;
		}
		set
		{
			weaponOption = value;
		}
	}

	public bool BreakingInto
	{
		get
		{
			return breakingInto;
		}
		set
		{
			breakingInto = value;
		}
	}

	public bool UseBuildGun
	{
		get
		{
			return useBuildGun;
		}
		set
		{
			useBuildGun = value;
		}
	}

	public bool TeamBalance
	{
		get
		{
			return teamBalance;
		}
		set
		{
			teamBalance = value;
		}
	}

	public bool DropItem
	{
		get
		{
			return dropItem;
		}
		set
		{
			dropItem = value;
		}
	}

	public bool Wanted
	{
		get
		{
			return wanted;
		}
		set
		{
			wanted = value;
		}
	}

	public long PremiumBricks
	{
		get
		{
			return premiumBricks;
		}
		set
		{
			premiumBricks = value;
		}
	}

	public int WaveOption
	{
		get
		{
			return waveOption;
		}
		set
		{
			waveOption = value;
		}
	}

	public bool[] SlotStatus => slotStatus;

	public bool NeedRespawnTicket
	{
		get
		{
			if (!dicRoom.ContainsKey(currentRoom))
			{
				return false;
			}
			return dicRoom[currentRoom].Type != Room.ROOM_TYPE.MAP_EDITOR;
		}
	}

	public Weapon.TYPE DefaultWeaponType
	{
		get
		{
			if (!dicRoom.ContainsKey(currentRoom))
			{
				return Weapon.TYPE.MAIN;
			}
			Room room = dicRoom[currentRoom];
			if (room.Type == Room.ROOM_TYPE.MAP_EDITOR)
			{
				return Weapon.TYPE.MODE_SPECIFIC;
			}
			if (room.Type == Room.ROOM_TYPE.BND)
			{
				GameObject gameObject = GameObject.Find("Main");
				if (null != gameObject)
				{
					BndMatch component = gameObject.GetComponent<BndMatch>();
					if (null != component && component.IsBuildPhase)
					{
						return Weapon.TYPE.MODE_SPECIFIC;
					}
				}
			}
			Weapon.TYPE result = Weapon.TYPE.MAIN;
			switch (weaponOption)
			{
			case 1:
				result = Weapon.TYPE.AUX;
				break;
			case 2:
				result = Weapon.TYPE.MELEE;
				break;
			}
			return result;
		}
	}

	public bool IsGameScene => gameScene.ContainsKey(Application.loadedLevelName);

	public bool IsBuildScene => buildScene.ContainsKey(Application.loadedLevelName);

	public bool IsBattleScene => battleScene.ContainsKey(Application.loadedLevelName);

	public bool IsVsTeamScene => vsTeamScene.ContainsKey(Application.loadedLevelName);

	public string ModeSpecificWeaponCode
	{
		get
		{
			string result = string.Empty;
			if (CurrentRoomType == Room.ROOM_TYPE.EXPLOSION)
			{
				result = "ClockBomb";
			}
			else if (IsBuildScene)
			{
				result = "Composer";
				Item usingItemBySlot = MyInfoManager.Instance.GetUsingItemBySlot(TItem.SLOT.NONE);
				if (usingItemBySlot != null)
				{
					switch (usingItemBySlot.Template.code)
					{
					case "s08":
						result = "Composer2";
						break;
					case "s09":
						result = "Composer3";
						break;
					case "s92":
						result = "Composer4";
						break;
					}
				}
			}
			else if (CurrentRoomType == Room.ROOM_TYPE.BUNGEE)
			{
				result = "BrickGun01";
			}
			return result;
		}
	}

	public string RoundingMessage
	{
		get
		{
			string result = string.Empty;
			switch (Instance.CurrentRoomType)
			{
			case Room.ROOM_TYPE.CAPTURE_THE_FLAG:
			case Room.ROOM_TYPE.EXPLOSION:
			case Room.ROOM_TYPE.ZOMBIE:
				result = StringMgr.Instance.Get("WAIT_NEXT_ROUND");
				break;
			case Room.ROOM_TYPE.BND:
			{
				GameObject gameObject = GameObject.Find("Main");
				if (null != gameObject)
				{
					BndTimer component = gameObject.GetComponent<BndTimer>();
					if (null != component && component.RemainRepeat > 0)
					{
						result = StringMgr.Instance.Get((!component.IsBuildPhase) ? "WAIT_BATTLE_PHASE" : "WAIT_BUILD_PHASE");
					}
				}
				break;
			}
			default:
				result = StringMgr.Instance.Get("CLAN_MATCH_HALF_TIME");
				break;
			}
			return result;
		}
	}

	public string RoundingHelp
	{
		get
		{
			string result = string.Empty;
			if (Instance.CurrentRoomType == Room.ROOM_TYPE.BND)
			{
				GameObject gameObject = GameObject.Find("Main");
				if (null != gameObject)
				{
					BndTimer component = gameObject.GetComponent<BndTimer>();
					if (null != component && component.RemainRepeat > 0 && !component.IsBuildPhase && BuildOption.Instance.AllowBuildGunInDestroyPhase() && Instance.UseBuildGun)
					{
						result = string.Format(StringMgr.Instance.Get("CHANGEABLE_MODE"), custom_inputs.Instance.GetKeyCodeName("K_MODE_TOGGLE_IN_BND"));
					}
				}
			}
			return result;
		}
	}

	public static RoomManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(RoomManager)) as RoomManager);
			}
			return _instance;
		}
	}

	public Room GetCurrentRoomInfo()
	{
		if (dicRoom.ContainsKey(currentRoom))
		{
			return dicRoom[currentRoom];
		}
		return null;
	}

	public void SetConfig(string _pswd, int _map, string _alias, int _weaponOption, int _timeLimit, int _killCount, bool _breakingInto, bool _teamBalance, bool _useBuildGun, byte _commented, bool _dropItem, bool _wanted)
	{
		pswd = _pswd;
		curMap = _map;
		curAlias = _alias;
		weaponOption = _weaponOption;
		timeLimit = _timeLimit;
		killCount = _killCount;
		breakingInto = _breakingInto;
		teamBalance = _teamBalance;
		commented = _commented;
		useBuildGun = _useBuildGun;
		dropItem = _dropItem;
		wanted = _wanted;
	}

	private void OnApplicationQuit()
	{
		_instance = null;
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
		dicRoom = new Dictionary<int, Room>();
		dicDumbRoom = new Dictionary<int, Room>();
		gameScene = new Dictionary<string, Room.ROOM_TYPE>();
		battleScene = new Dictionary<string, Room.ROOM_TYPE>();
		buildScene = new Dictionary<string, Room.ROOM_TYPE>();
		vsTeamScene = new Dictionary<string, Room.ROOM_TYPE>();
		for (int i = 0; i < Room.sceneByMode.Length; i++)
		{
			gameScene.Add(Room.sceneByMode[i], (Room.ROOM_TYPE)i);
			if (i != 0)
			{
				battleScene.Add(Room.sceneByMode[i], (Room.ROOM_TYPE)i);
				if (i != 2 && i != 5 && i != 8 && i != 9)
				{
					vsTeamScene.Add(Room.sceneByMode[i], (Room.ROOM_TYPE)i);
				}
			}
		}
		buildScene.Add(Room.sceneByMode[0], Room.ROOM_TYPE.MAP_EDITOR);
		buildScene.Add(Room.sceneByMode[6], Room.ROOM_TYPE.BND);
		slotStatus = new bool[16];
	}

	public void RefreshRoomList()
	{
		dicDumbRoom.Clear();
		foreach (KeyValuePair<int, Room> item in dicRoom)
		{
			dicDumbRoom.Add(item.Key, new Room(item.Value));
		}
	}

	public void RefreshRoom(int roomNo)
	{
		if (!dicRoom.ContainsKey(roomNo))
		{
			if (dicDumbRoom.ContainsKey(roomNo))
			{
				dicDumbRoom.Remove(roomNo);
			}
		}
		else
		{
			if (dicDumbRoom.ContainsKey(roomNo))
			{
				dicDumbRoom.Remove(roomNo);
			}
			dicDumbRoom.Add(roomNo, new Room(dicRoom[roomNo]));
		}
	}

	public void ResetCurrentRoomRelatedInfo()
	{
		CurrentRoom = -1;
		curMap = -1;
		killCount = 0;
		timeLimit = 0;
		master = -1;
		for (int i = 0; i < slotStatus.Length; i++)
		{
			slotStatus[i] = false;
		}
		premiumBricks = 0L;
	}

	public void Clear()
	{
		ResetCurrentRoomRelatedInfo();
		dicRoom.Clear();
	}

	public void ClearRooms()
	{
		dicRoom.Clear();
	}

	public void DelRoom(int no)
	{
		if (!dicRoom.ContainsKey(no))
		{
			Debug.LogError("Fail to find the room ");
		}
		else
		{
			dicRoom.Remove(no);
			if (prevRoom == no)
			{
				prevRoom = -1;
				RefreshRoomList();
			}
		}
	}

	public void AddOrUpdateRoom(int no, Room.ROOM_STATUS status, int cur, int max, bool isLocked, int map, string curMapAlias, int roomGoal, int roomTimelimit, int RoomWeaponOption, int RoomPing, int RoomScore1, int RoomScore2, int countryFilter, bool breakInto, int type, string title, bool dropItem, bool wanted, int squad, int squadCounter)
	{
		if (!dicRoom.ContainsKey(no))
		{
			dicRoom.Add(no, new Room(isLocked, no, title, (Room.ROOM_TYPE)type, status, cur, max, map, curMapAlias, roomGoal, roomTimelimit, RoomWeaponOption, RoomPing, RoomScore1, RoomScore2, countryFilter, breakInto, wanted, dropItem, squad, squadCounter));
		}
		else
		{
			dicRoom[no].Status = status;
			dicRoom[no].CurPlayer = cur;
			dicRoom[no].MaxPlayer = max;
			dicRoom[no].Locked = isLocked;
			dicRoom[no].map = map;
			dicRoom[no].CurMapAlias = curMapAlias;
			dicRoom[no].goal = roomGoal;
			dicRoom[no].timelimit = roomTimelimit;
			dicRoom[no].weaponOption = RoomWeaponOption;
			dicRoom[no].ping = RoomPing;
			dicRoom[no].score1 = RoomScore1;
			dicRoom[no].score2 = RoomScore2;
			dicRoom[no].CountryFilter = countryFilter;
			dicRoom[no].isBreakInto = breakInto;
			dicRoom[no].Type = (Room.ROOM_TYPE)type;
			dicRoom[no].isWanted = wanted;
			dicRoom[no].isDropItem = dropItem;
			dicRoom[no].Squad = squad;
			dicRoom[no].Title = title;
			dicRoom[no].SquadCounter = squadCounter;
		}
	}

	public Room GetRoom(int no)
	{
		if (!dicRoom.ContainsKey(no))
		{
			return null;
		}
		return dicRoom[no];
	}

	public List<KeyValuePair<int, Room>> ToSortedList(Room.COLUMN sortedBy, bool ascending, Room.ROOM_TYPE type, Room.ROOM_STATUS status)
	{
		List<KeyValuePair<int, Room>> list = new List<KeyValuePair<int, Room>>();
		if (BuildOption.Instance.Props.refreshRoomsManually)
		{
			foreach (KeyValuePair<int, Room> item in dicDumbRoom)
			{
				if ((type == Room.ROOM_TYPE.NONE || type == item.Value.Type) && (status == Room.ROOM_STATUS.NONE || status == item.Value.Status))
				{
					list.Add(item);
				}
			}
		}
		else
		{
			foreach (KeyValuePair<int, Room> item2 in dicRoom)
			{
				if ((type == Room.ROOM_TYPE.NONE || type == item2.Value.Type) && (status == Room.ROOM_STATUS.NONE || status == item2.Value.Status))
				{
					list.Add(item2);
				}
			}
		}
		list.Sort((KeyValuePair<int, Room> firstPair, KeyValuePair<int, Room> nextPair) => firstPair.Value.Compare(nextPair.Value, sortedBy, ascending));
		return list;
	}

	public bool IsVoteProgress()
	{
		return vote != null;
	}

	public bool IsVoteAble()
	{
		if (vote == null)
		{
			return false;
		}
		if (vote.isVoted)
		{
			return false;
		}
		return vote.isVoteAble;
	}

	public void ClearVote()
	{
		vote = null;
	}

	public void StartVoteStatus(VoteStatus voteStatus)
	{
		vote = voteStatus;
	}

	public bool IsEscapeNotAttack()
	{
		return CurrentRoomType == Room.ROOM_TYPE.ESCAPE && Instance.WeaponOption == 3;
	}
}
