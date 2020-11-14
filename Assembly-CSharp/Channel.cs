public class Channel
{
	public enum MODE
	{
		NEWBIE = 1,
		BATTLE,
		MAPEDIT,
		CLAN
	}

	private int id;

	private int mode;

	private string name;

	private string ip;

	private int port;

	private int userCount;

	private int maxUserCount;

	private int country;

	private int minLvRank;

	private int maxLvRank;

	private int xpBonus;

	private int fpBonus;

	private int limitStarRate;

	public int Id => id;

	public int Mode => mode;

	public bool IsSmartQuickJoin => mode == 1 || mode == 2;

	public string Name => name;

	public string Ip => ip;

	public int Port => port;

	public int UserCount
	{
		get
		{
			return userCount;
		}
		set
		{
			userCount = value;
		}
	}

	public int MaxUserCount
	{
		get
		{
			return maxUserCount;
		}
		set
		{
			maxUserCount = value;
		}
	}

	public int Country
	{
		get
		{
			return country;
		}
		set
		{
			country = value;
		}
	}

	public int MinLvRank
	{
		get
		{
			return minLvRank;
		}
		set
		{
			minLvRank = value;
		}
	}

	public int MaxLvRank
	{
		get
		{
			return maxLvRank;
		}
		set
		{
			maxLvRank = value;
		}
	}

	public int XpBonus
	{
		get
		{
			return xpBonus;
		}
		set
		{
			xpBonus = value;
		}
	}

	public int FpBonus
	{
		get
		{
			return fpBonus;
		}
		set
		{
			fpBonus = value;
		}
	}

	public int LimitStarRate
	{
		get
		{
			return limitStarRate;
		}
		set
		{
			limitStarRate = value;
		}
	}

	public bool IsLimitStarRate
	{
		get
		{
			if (mode == 3)
			{
				return false;
			}
			return 0 < limitStarRate && limitStarRate < 10000;
		}
	}

	public Channel(int _id, int _mode, string _name, string _ip, int _port, int _userCount, int _maxUserCount, int _country, byte _minLvRank, byte _maxLvRank, ushort _xpBonus, ushort _fpBonus, int _limitStarRate)
	{
		id = _id;
		mode = _mode;
		name = _name;
		ip = _ip;
		port = _port;
		userCount = _userCount;
		maxUserCount = _maxUserCount;
		country = _country;
		minLvRank = _minLvRank;
		maxLvRank = _maxLvRank;
		xpBonus = _xpBonus;
		fpBonus = _fpBonus;
		limitStarRate = _limitStarRate;
	}

	public string GetMapHint()
	{
		string text = string.Empty;
		switch (mode)
		{
		case 1:
			text = "NEWBIE_CHANNEL_HINT";
			break;
		case 2:
			text = "PLAY_CHANNEL_HINT";
			break;
		case 3:
			text = "BUILD_CHANNEL_HINT";
			break;
		case 4:
			text = "CLAN_CHANNEL_HINT";
			break;
		}
		if (text.Length <= 0)
		{
			return text;
		}
		return StringMgr.Instance.Get(text);
	}

	public int Compare(Channel arg)
	{
		int num = 0;
		num = mode.CompareTo(arg.Mode);
		if (num == 0)
		{
			num = id.CompareTo(arg.Id);
		}
		return num;
	}

	public bool IsUseAbleLevel(int lvRank)
	{
		return lvRank >= MinLvRank && lvRank <= MaxLvRank;
	}
}
