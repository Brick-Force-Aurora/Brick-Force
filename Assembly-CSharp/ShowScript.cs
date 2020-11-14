public class ShowScript : ScriptCmd
{
	private int id;

	private bool visible;

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

	public bool Visible
	{
		get
		{
			return visible;
		}
		set
		{
			visible = value;
		}
	}

	public override int GetIconIndex()
	{
		return 5;
	}

	public override string GetDescription()
	{
		string str = "showscript" + ScriptCmd.ArgDelimeters[0];
		str = str + id.ToString() + ScriptCmd.ArgDelimeters[0];
		return str + visible.ToString();
	}

	public static string GetDefaultDescription()
	{
		return "showscript" + ScriptCmd.ArgDelimeters[0] + "-1" + ScriptCmd.ArgDelimeters[0] + "true";
	}

	public override string GetName()
	{
		return "ShowScript";
	}
}
