public class ClanApplicant
{
	private string name;

	private int seq;

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

	public ClanApplicant(int _seq, string _name, int _year, int _month, int _day)
	{
		seq = _seq;
		name = _name;
		day = _day;
		month = _month;
		year = _year;
	}

	public string GetDateToString()
	{
		return year.ToString() + "." + month.ToString() + "." + day.ToString();
	}
}
