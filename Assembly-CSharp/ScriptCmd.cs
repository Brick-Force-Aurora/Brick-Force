public class ScriptCmd
{
	public static string[] CmdDelimeters = new string[2]
	{
		")(*&",
		"\0"
	};

	public static string[] ArgDelimeters = new string[2]
	{
		"!@#$",
		"\0"
	};

	public virtual string GetDescription()
	{
		return "null";
	}

	public virtual int GetIconIndex()
	{
		return -1;
	}

	public virtual string GetName()
	{
		return "null";
	}
}
