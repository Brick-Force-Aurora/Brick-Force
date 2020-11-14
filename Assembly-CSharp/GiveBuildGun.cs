public class GiveBuildGun : ScriptCmd
{
	private string code;

	public string Code
	{
		get
		{
			return code;
		}
		set
		{
			code = value;
		}
	}

	public override int GetIconIndex()
	{
		return 7;
	}

	public override string GetDescription()
	{
		string str = "givebuildgun" + ScriptCmd.ArgDelimeters[0];
		return str + code.ToString();
	}

	public static string GetDefaultDescription()
	{
		return "givebuildgun" + ScriptCmd.ArgDelimeters[0] + string.Empty;
	}

	public override string GetName()
	{
		return "GiveBuildGun";
	}
}
