using System.Collections.Generic;

public class CMR
{
	private long clanMatch;

	private int map;

	private int kind;

	private int playerCount;

	private int enemyMark;

	private string enemy;

	private int killCount;

	private int deathCount;

	private int result;

	private int score;

	private int goal;

	private int year;

	private int month;

	private int date;

	private List<CMPlayer> ourPlayers;

	private List<CMPlayer> enemyPlayers;

	private bool playerList;

	public bool PlayerList
	{
		get
		{
			return playerList;
		}
		set
		{
			playerList = value;
		}
	}

	public long ClanMatch => clanMatch;

	public int Map => map;

	public int Kind => kind;

	public int PlayerCount => playerCount;

	public int EnemyMark => enemyMark;

	public string Enemy => enemy;

	public int KillCount => killCount;

	public int DeathCount => deathCount;

	public int Result => result;

	public int Score => score;

	public int Goal => goal;

	public CMR(long _clanMatch, int _clan, int _map, int _kind, int _playerCount, int _enemyMark, string _enemy, int _killCount, int _deathCount, int _result, int _score, int _goal, int _year, int _month, int _date)
	{
		playerList = false;
		clanMatch = _clanMatch;
		map = _map;
		kind = _kind;
		playerCount = _playerCount;
		enemyMark = _enemyMark;
		enemy = _enemy;
		killCount = _killCount;
		deathCount = _deathCount;
		result = _result;
		score = _score;
		goal = _goal;
		year = _year;
		month = _month;
		date = _date;
		ourPlayers = new List<CMPlayer>();
		enemyPlayers = new List<CMPlayer>();
	}

	public string GetKindString()
	{
		return Room.Type2String(kind) + " (" + playerCount.ToString() + " vs " + playerCount.ToString() + ")";
	}

	public string GetResultString()
	{
		return goal.ToString() + " " + StringMgr.Instance.Get("KILL") + " (" + killCount.ToString() + "/" + deathCount.ToString() + ") ";
	}

	public string GetMiniResultString()
	{
		return " (" + killCount.ToString() + "/" + deathCount.ToString() + ") ";
	}

	public string GetDateString()
	{
		return year.ToString() + "." + month.ToString() + "." + date.ToString();
	}

	public void AddPlayer(int clan, int xp, int rank, string nickname, int kill, int assist, int death, int score)
	{
		if (clan == MyInfoManager.Instance.ClanSeq)
		{
			ourPlayers.Add(new CMPlayer(xp, rank, nickname, kill, assist, death, score));
		}
		else
		{
			enemyPlayers.Add(new CMPlayer(xp, rank, nickname, kill, assist, death, score));
		}
	}

	public CMPlayer[] GetOurPlayersArray()
	{
		return ourPlayers.ToArray();
	}

	public CMPlayer[] GetEnemyPlayersArray()
	{
		return enemyPlayers.ToArray();
	}
}
