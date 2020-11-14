public class NameCard
{
	private int seq;

	private string nickname;

	private int lv;

	private int rank;

	private int svrId;

	public int Seq => seq;

	public string Nickname
	{
		get
		{
			return nickname;
		}
		set
		{
			nickname = value;
		}
	}

	public int Lv
	{
		get
		{
			return lv;
		}
		set
		{
			lv = value;
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

	public int SvrId
	{
		get
		{
			return svrId;
		}
		set
		{
			svrId = value;
		}
	}

	public bool IsConnected => svrId > 0;

	public NameCard(int _seq, string _nickname, int _lv, int _svrId, int _rank)
	{
		seq = _seq;
		nickname = _nickname;
		lv = _lv;
		svrId = _svrId;
		rank = _rank;
	}
}
