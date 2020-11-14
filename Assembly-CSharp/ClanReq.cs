public class ClanReq
{
	private string name;

	private int seq;

	private int mark;

	private string clanMaster;

	private int rank;

	private int winCount;

	private int drawCount;

	private int loseCount;

	private int noMember;

	private int gold;

	private int silver;

	private int bronze;

	private int day;

	private int month;

	private int year;

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

	public int Seq
	{
		get
		{
			return seq;
		}
		set
		{
			seq = value;
		}
	}

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

	public ClanReq(int _seq, int _mark, string _name, int _winCount, int _drawCount, int _loseCount, int _noMember, int _rank, int _year, int _month, int _day, int _gold, int _silver, int _bronze, string _clanMaster)
	{
		seq = _seq;
		mark = _mark;
		name = _name;
		winCount = _winCount;
		drawCount = _drawCount;
		loseCount = _loseCount;
		noMember = _noMember;
		rank = _rank;
		day = _day;
		month = _month;
		year = _year;
		gold = _gold;
		silver = _silver;
		bronze = _bronze;
		clanMaster = _clanMaster;
	}

	public string GetDateToString()
	{
		return year.ToString() + "." + month.ToString() + "." + day.ToString();
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

	public string GoldSilverBronzeString()
	{
		return gold.ToString() + "/" + silver.ToString() + "/" + bronze.ToString();
	}
}
