public class Squad
{
	private int index;

	private int memberCount;

	private int maxMember;

	private int winCount;

	private int drawCount;

	private int loseCount;

	private int leader;

	private string teamLeader;

	private int wannaPlayMap;

	private int wannaPlayMode;

	public int Index => index;

	public string Name
	{
		get
		{
			int num = index + 1;
			return MyInfoManager.Instance.ClanName + "-" + num.ToString();
		}
	}

	public string MemberCountString => memberCount.ToString() + "/" + maxMember.ToString();

	public int MemberCount
	{
		get
		{
			return memberCount;
		}
		set
		{
			memberCount = value;
		}
	}

	public int MaxMember
	{
		get
		{
			return maxMember;
		}
		set
		{
			maxMember = value;
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

	public string TeamLeader
	{
		get
		{
			return teamLeader;
		}
		set
		{
			teamLeader = value;
		}
	}

	public int Leader
	{
		get
		{
			return leader;
		}
		set
		{
			leader = value;
		}
	}

	public int WannaPlayMap
	{
		get
		{
			return wannaPlayMap;
		}
		set
		{
			wannaPlayMap = value;
		}
	}

	public int WannaPlayMode
	{
		get
		{
			return wannaPlayMode;
		}
		set
		{
			wannaPlayMode = value;
		}
	}

	public string Record => winCount.ToString() + StringMgr.Instance.Get("WIN") + " " + drawCount.ToString() + StringMgr.Instance.Get("DRAW") + " " + loseCount.ToString() + StringMgr.Instance.Get("LOSE");

	public Squad(int i, int cnt, int max, int win, int draw, int lose, int leaderSeq, string leaderNickname)
	{
		index = i;
		memberCount = cnt;
		maxMember = max;
		winCount = win;
		drawCount = draw;
		loseCount = lose;
		leader = leaderSeq;
		teamLeader = leaderNickname;
	}
}
