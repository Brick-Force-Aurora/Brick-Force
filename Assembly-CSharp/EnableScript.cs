public class EnableScript : ScriptCmd
{
	private int id;

	private bool enable;

	public int Id
	{
		get
		{
			return id;
		}
		set
		{
			id = value;
		}
	}

	public bool Enable
	{
		get
		{
			return enable;
		}
		set
		{
			enable = value;
		}
	}

	public override int GetIconIndex()
	{
		return 0;
	}

	public override string GetDescription()
	{
		string str = "enablescript" + ScriptCmd.ArgDelimeters[0];
		str = str + id.ToString() + ScriptCmd.ArgDelimeters[0];
		return str + enable.ToString();
	}

	public static string GetDefaultDescription()
	{
		return "enablescript" + ScriptCmd.ArgDelimeters[0] + "-1" + ScriptCmd.ArgDelimeters[0] + "true";
	}

	public override string GetName()
	{
		return "EnableScript";
	}
}
