public class Clan
{
	public enum CLAN_STATUS
	{
		NO_MEMBER = -1,
		MEMBER,
		STAFF,
		MASTER
	}

	private int seq;

	private int mark;

	private string name;

	private string clanMaster;

	private int winCount;

	private int drawCount;

	private int loseCount;

	private int noMember;

	private int rank;

	private int rankChg;

	private int matchPoint;

	public int gold;

	public int silver;

	public int bronze;

	private int day;

	private int month;

	private int year;

	public int Seq => seq;

	public int Mark
	{
		get
		{
			return mark;
		}
		set
		{
			mark = value;
		}
	}

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
		}
	}

	public string ClanMaster
	{
		get
		{
			return clanMaster;
		}
		set
		{
			clanMaster = value;
		}
	}

	public int WinCount
	{
		get
		{
			return winCount;
		}
		set
		{
			winCount = value;
		}
	}

	public int DrawCount
	{
		get
		{
			return drawCount;
		}
		set
		{
			drawCount = value;
		}
	}

	public int LoseCount
	{
		get
		{
			return loseCount;
		}
		set
		{
			loseCount = value;
		}
	}

	public int NoMember
	{
		get
		{
			return noMember;
		}
		set
		{
			noMember = value;
		}
	}

	public int Rank
	{
		get
		{
			return rank;
		}
		set
		{
			rank = value;
		}
	}

	public int RankChg
	{
		get
		{
			return rankChg;
		}
		set
		{
			rankChg = value;
		}
	}

	public int MatchPoint
	{
		get
		{
			return matchPoint;
		}
		set
		{
			matchPoint = value;
		}
	}

	public Clan(int _seq, int _mark, string _name, int _winCount, int _drawCount, int _loseCount, int _noMember, int _rank, int _rankChg, int _matchPoint, int _year, int _month, int _day, int _gold, int _silver, int _bronze, string _clanMaster)
	{
		seq = _seq;
		mark = _mark;
		name = _name;
		winCount = _winCount;
		drawCount = _drawCount;
		loseCount = _loseCount;
		noMember = _noMember;
		rank = _rank;
		rankChg = _rankChg;
		matchPoint = _matchPoint;
		day = _day;
		month = _month;
		year = _year;
		gold = _gold;
		silver = _silver;
		bronze = _bronze;
		clanMaster = _clanMaster;
	}

	public string RecordString()
	{
		return winCount.ToString() + "-" + drawCount.ToString() + "-" + loseCount.ToString();
	}

	public string MemberCountString()
	{
		return noMember.ToString();
	}

	public string RankString()
	{
		if (rank < 0)
		{
			return "-";
		}
		return rank.ToString();
	}

	public string MatchPointString()
	{
		if (matchPoint < 0)
		{
			return "-";
		}
		return matchPoint.ToString();
	}

	public string CeateDateString()
	{
		if (day < 0)
		{
			return "-";
		}
		return year.ToString() + "." + month.ToString() + "." + day.ToString();
	}

	public string GoldSilverBronzeString()
	{
		return gold.ToString() + "/" + silver.ToString() + "/" + bronze.ToString();
	}

	public int Compare(Clan arg)
	{
		if (rank >= arg.rank)
		{
			return 1;
		}
		return -1;
	}
}
