using UnityEngine;

public class MsgBox
{
	public enum TYPE
	{
		ERROR,
		WARNING,
		SELECT,
		FORCE_POINT_CHARGE,
		QUIT
	}

	private string msg;

	private TYPE type;

	public string Message => msg;

	public TYPE Type => type;

	public Color MsgColor
	{
		get
		{
			if (type == TYPE.WARNING)
			{
				return Color.white;
			}
			return new Color(0.91f, 0.3f, 0f);
		}
	}

	public MsgBox(TYPE _type, string _msg)
	{
		msg = _msg;
		type = _type;
	}
}
