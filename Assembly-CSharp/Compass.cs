using UnityEngine;

public class Compass : MonoBehaviour
{
	public enum DESTINATION_LEVEL
	{
		LOBBY,
		BATTLE_TUTOR,
		SQUAD,
		ROOM
	}

	private string srcLevel;

	private DESTINATION_LEVEL dst;

	private int channel;

	private int roomIdx;

	private string roomPswd;

	private LOBBY_TYPE lobbyType;

	private static Compass _instance;

	public string SrcLevel => srcLevel;

	public DESTINATION_LEVEL Dst => dst;

	public int Channel => channel;

	public int RoomIdx => roomIdx;

	public string RoomPswd => roomPswd;

	public LOBBY_TYPE LobbyType
	{
		get
		{
			return lobbyType;
		}
		set
		{
			lobbyType = value;
		}
	}

	public static Compass Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(Compass)) as Compass);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the Compass Instance");
				}
			}
			return _instance;
		}
	}

	public string GetDestinationLevel()
	{
		switch (dst)
		{
		case DESTINATION_LEVEL.LOBBY:
			return "Lobby";
		case DESTINATION_LEVEL.BATTLE_TUTOR:
			return "BattleTutor";
		case DESTINATION_LEVEL.SQUAD:
			return "Squad";
		default:
			return string.Empty;
		}
	}

	public void SetDestination(DESTINATION_LEVEL _dst, int _channel, int _roomIdx, string _roomPswd)
	{
		SetDestination(_dst, _channel);
		roomIdx = _roomIdx;
		roomPswd = _roomPswd;
	}

	public void SetDestination(DESTINATION_LEVEL _dst, int _channel)
	{
		roomIdx = -1;
		roomPswd = string.Empty;
		if (!Application.isLoadingLevel && Application.loadedLevelName != "SceneSwitch")
		{
			Channel channel = ChannelManager.Instance.Get(_channel);
			if (channel.Mode == 1 && !MyInfoManager.Instance.IsNewbie)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NEWBIE_ONLY"));
			}
			else if (channel.UserCount >= channel.MaxUserCount)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CHANNEL_IS_CROWDED"));
			}
			else
			{
				dst = _dst;
				this.channel = _channel;
				lobbyType = GlobalVars.Instance.LobbyType;
				srcLevel = Application.loadedLevelName;
				Application.LoadLevel("SceneSwitch");
			}
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
		roomIdx = -1;
		roomPswd = string.Empty;
	}

	private void Update()
	{
	}
}
