public class EditingMap
{
	private int seq;

	private string mapTitle;

	private char mapSize;

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

	public string MapTitle
	{
		get
		{
			return mapTitle;
		}
		set
		{
			mapTitle = value;
		}
	}

	public char MapSize
	{
		get
		{
			return mapSize;
		}
		set
		{
			mapSize = value;
		}
	}

	public EditingMap(int s, string title, char size)
	{
		seq = s;
		mapTitle = title;
		mapSize = size;
	}
}
