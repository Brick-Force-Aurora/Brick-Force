public class Memo
{
	public long seq;

	public string sender;

	public string title;

	public string contents;

	public string attached;

	public int option;

	public bool check;

	private int year;

	private sbyte month;

	private sbyte day;

	private sbyte isRead;

	private sbyte sysFlag;

	public int Year => year;

	public sbyte Month => month;

	public sbyte Day => day;

	public bool IsRead
	{
		get
		{
			return isRead == 1;
		}
		set
		{
			if (value)
			{
				isRead = 1;
			}
			else
			{
				isRead = 0;
			}
		}
	}

	public sbyte SysFlag
	{
		get
		{
			return sysFlag;
		}
		set
		{
			sysFlag = value;
		}
	}

	public string SendDate => year.ToString() + "." + month.ToString() + "." + day.ToString();

	public Memo(long s, string snd, string ttl, string cntnts, string ttchd, int opt, int yy, sbyte mm, sbyte dd, sbyte rd, sbyte sf)
	{
		seq = s;
		sender = snd;
		title = ttl;
		contents = cntnts;
		attached = ttchd;
		option = opt;
		year = yy;
		month = mm;
		day = dd;
		isRead = rd;
		sysFlag = sf;
	}

	public bool needReply()
	{
		return (sysFlag & 2) == 0;
	}

	public bool needStringKey()
	{
		return (sysFlag & 1) != 0;
	}
}
