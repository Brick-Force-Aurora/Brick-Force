public class ClanMemberCard : NameCard
{
	private int clanLv;

	private int clanRoyalty;

	private int clanPoint;

	public int ClanLv
	{
		get
		{
			return clanLv;
		}
		set
		{
			clanLv = value;
		}
	}

	public int ClanRoyalty
	{
		get
		{
			return clanRoyalty;
		}
		set
		{
			clanRoyalty = value;
		}
	}

	public int ClanPoint
	{
		get
		{
			return clanPoint;
		}
		set
		{
			clanPoint = value;
		}
	}

	public ClanMemberCard(int _seq, string _nickname, int _lv, int _rank, int _clanLv, int _clanRoyalty, int _clanPoint)
		: base(_seq, _nickname, _lv, -1, _rank)
	{
		clanLv = _clanLv;
		clanRoyalty = _clanRoyalty;
		clanPoint = _clanPoint;
	}

	public int Compare(ClanMemberCard arg, CLANSORT sortBy, bool ascending)
	{
		int num = 0;
		switch (sortBy)
		{
		case CLANSORT.POINT:
			num = clanPoint.CompareTo(arg.clanPoint);
			break;
		case CLANSORT.NAME:
			num = base.Nickname.CompareTo(arg.Nickname);
			break;
		case CLANSORT.LV:
			num = clanLv.CompareTo(arg.clanLv);
			break;
		case CLANSORT.CNNT:
			num = base.IsConnected.CompareTo(arg.IsConnected);
			break;
		}
		if (!ascending)
		{
			num = -num;
		}
		return num;
	}
}
