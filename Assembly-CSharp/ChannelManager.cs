using System.Collections.Generic;
using UnityEngine;

public class ChannelManager : MonoBehaviour
{
	public string PrevScene = string.Empty;

	private int curChannelId = -1;

	private int loginChannelId = -1;

	private float deltaTime;

	private float refreshTimeOut = 3f;

	private string lastError = string.Empty;

	private int tk2fpMultiple;

	private Dictionary<int, Channel> channelDictionary;

	private static ChannelManager _instance;

	public int CurChannelId
	{
		get
		{
			return curChannelId;
		}
		set
		{
			curChannelId = value;
		}
	}

	public int LoginChannelId
	{
		get
		{
			return loginChannelId;
		}
		set
		{
			loginChannelId = value;
		}
	}

	public Channel CurChannel
	{
		get
		{
			if (channelDictionary.ContainsKey(curChannelId))
			{
				return channelDictionary[curChannelId];
			}
			return null;
		}
	}

	public int Tk2FpMultiple
	{
		get
		{
			return tk2fpMultiple;
		}
		set
		{
			tk2fpMultiple = value;
		}
	}

	public static ChannelManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(ChannelManager)) as ChannelManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the ChannelManager Instance");
				}
			}
			return _instance;
		}
	}

	public Channel GetTutorialableChannel()
	{
		bool isNewbie = MyInfoManager.Instance.IsNewbie;
		List<Channel> list = new List<Channel>();
		int levelMixLank = XpManager.Instance.GetLevelMixLank(MyInfoManager.Instance.Xp, MyInfoManager.Instance.Rank);
		foreach (KeyValuePair<int, Channel> item in channelDictionary)
		{
			if (item.Value.UserCount < item.Value.MaxUserCount && (item.Value.Mode != 1 || isNewbie) && item.Value.IsUseAbleLevel(levelMixLank))
			{
				list.Add(item.Value);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].Id == curChannelId)
			{
				return list[i];
			}
		}
		if (list.Count <= 0)
		{
			Debug.LogError("Impossable GetTutorialableChannel() check Channel");
			return null;
		}
		return list[Random.Range(0, list.Count)];
	}

	public Channel GetBestPlayChannel()
	{
		bool isNewbie = MyInfoManager.Instance.IsNewbie;
		Channel channel = null;
		if (isNewbie)
		{
			channel = GetBestChannel(1);
		}
		if (!isNewbie || channel == null)
		{
			channel = GetBestChannel(2);
		}
		return channel;
	}

	public Channel GetBestBuildChannel()
	{
		return GetBestChannel(3);
	}

	public Channel GetBestClanChannel()
	{
		return GetBestChannel(4);
	}

	private Channel GetBestChannel(int mode)
	{
		float num = Mathf.Max(0.25f, Mathf.Min(1f, BuildOption.Instance.Props.goalRatioForChannel));
		int levelMixLank = XpManager.Instance.GetLevelMixLank(MyInfoManager.Instance.Xp, MyInfoManager.Instance.Rank);
		Channel channel = null;
		foreach (KeyValuePair<int, Channel> item in channelDictionary)
		{
			int num2 = (int)((float)item.Value.MaxUserCount * num);
			if (item.Value.UserCount < num2 && item.Value.Mode == mode && (channel == null || channel.UserCount < item.Value.UserCount) && item.Value.IsUseAbleLevel(levelMixLank))
			{
				channel = item.Value;
			}
		}
		if (channel == null)
		{
			foreach (KeyValuePair<int, Channel> item2 in channelDictionary)
			{
				if (item2.Value.Mode == mode)
				{
					bool flag = item2.Value.IsUseAbleLevel(levelMixLank);
					if ((channel == null || channel.UserCount > item2.Value.UserCount) && flag)
					{
						channel = item2.Value;
					}
					if (!flag)
					{
						string rank = XpManager.Instance.GetRank(item2.Value.MinLvRank);
						string rank2 = XpManager.Instance.GetRank(item2.Value.MaxLvRank);
						lastError = string.Format(StringMgr.Instance.Get("CHANNEL_LIMIT_MSG01"), rank, rank2);
					}
				}
			}
			if (channel != null)
			{
				lastError = string.Empty;
			}
		}
		return channel;
	}

	public bool IsLastError()
	{
		return lastError.Length > 0;
	}

	public string GetBestChannelLastError()
	{
		return lastError;
	}

	private void Update()
	{
		if (CSNetManager.Instance.Sock != null && CSNetManager.Instance.Sock.IsConnected() && Application.loadedLevelName.Contains("ChangeChannel"))
		{
			deltaTime += Time.deltaTime;
			if (deltaTime > refreshTimeOut)
			{
				deltaTime = 0f;
				CSNetManager.Instance.Sock.SendCS_CHANNEL_LIST_REQ();
			}
		}
	}

	private void Awake()
	{
		channelDictionary = new Dictionary<int, Channel>();
		Object.DontDestroyOnLoad(this);
	}

	public void Clear()
	{
		curChannelId = -1;
		loginChannelId = -1;
		channelDictionary.Clear();
	}

	public Channel Get(int id)
	{
		if (channelDictionary.ContainsKey(id))
		{
			return channelDictionary[id];
		}
		return null;
	}

	public void UpdateAlways(int id, int mode, string name, string ip, int port, int userCount, int maxUserCount, int country, byte minLvRank, byte maxLvRank, ushort xpBonus, ushort fpBonus, int limitStarRate)
	{
		if (userCount < 0)
		{
			if (channelDictionary.ContainsKey(id))
			{
				channelDictionary.Remove(id);
			}
		}
		else if (!channelDictionary.ContainsKey(id))
		{
			channelDictionary.Add(id, new Channel(id, mode, name, ip, port, userCount, maxUserCount, country, minLvRank, maxLvRank, xpBonus, fpBonus, limitStarRate));
		}
		else
		{
			channelDictionary[id].UserCount = userCount;
			channelDictionary[id].MaxUserCount = maxUserCount;
			channelDictionary[id].LimitStarRate = limitStarRate;
		}
	}

	public Channel[] ToArraySortedByMode()
	{
		List<KeyValuePair<int, Channel>> list = ToSortedList();
		List<Channel> list2 = new List<Channel>();
		for (int i = 1; i <= 4; i++)
		{
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j].Value.Mode == i)
				{
					list2.Add(list[j].Value);
				}
			}
		}
		return list2.ToArray();
	}

	public Channel[] ToArray(bool isBuild)
	{
		List<KeyValuePair<int, Channel>> list = ToSortedList();
		List<Channel> list2 = new List<Channel>();
		for (int i = 0; i < list.Count; i++)
		{
			if (isBuild)
			{
				if (list[i].Value.Mode == 3)
				{
					list2.Add(list[i].Value);
				}
			}
			else if (list[i].Value.Mode != 3)
			{
				list2.Add(list[i].Value);
			}
		}
		return list2.ToArray();
	}

	public Channel[] ToArray(int mode)
	{
		List<KeyValuePair<int, Channel>> list = ToSortedList();
		List<Channel> list2 = new List<Channel>();
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].Value.Mode == mode)
			{
				list2.Add(list[i].Value);
			}
		}
		return list2.ToArray();
	}

	public Channel[] ToArray(int mode, int country)
	{
		List<KeyValuePair<int, Channel>> list = ToSortedList();
		List<Channel> list2 = new List<Channel>();
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].Value.Mode == mode && (country < 0 || list[i].Value.Country == country))
			{
				list2.Add(list[i].Value);
			}
		}
		return list2.ToArray();
	}

	private List<KeyValuePair<int, Channel>> ToSortedList()
	{
		List<KeyValuePair<int, Channel>> list = new List<KeyValuePair<int, Channel>>(channelDictionary);
		list.Sort((KeyValuePair<int, Channel> firstPair, KeyValuePair<int, Channel> nextPair) => firstPair.Value.Compare(nextPair.Value));
		return list;
	}

	public bool IsSupportMode(Room.ROOM_TYPE t)
	{
		if (CurChannel != null)
		{
			switch (CurChannel.Mode)
			{
			case 3:
				return t == Room.ROOM_TYPE.MAP_EDITOR;
			case 4:
				return t == Room.ROOM_TYPE.TEAM_MATCH || t == Room.ROOM_TYPE.CAPTURE_THE_FLAG || t == Room.ROOM_TYPE.EXPLOSION;
			case 1:
				return t != Room.ROOM_TYPE.MAP_EDITOR;
			case 2:
				return t != Room.ROOM_TYPE.MAP_EDITOR;
			}
		}
		return false;
	}

	public int GetAllUserCount()
	{
		int num = 0;
		foreach (KeyValuePair<int, Channel> item in channelDictionary)
		{
			num += item.Value.UserCount;
		}
		return num;
	}
}
