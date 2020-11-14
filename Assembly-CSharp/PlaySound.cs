public class PlaySound : ScriptCmd
{
	private int index;

	public int Index
	{
		get
		{
			return index;
		}
		set
		{
			index = value;
		}
	}

	public override string GetDescription()
	{
		string str = "playsound" + ScriptCmd.ArgDelimeters[0];
		return str + index.ToString();
	}

	public override int GetIconIndex()
	{
		return 2;
	}

	public static string GetDefaultDescription()
	{
		return "playsound" + ScriptCmd.ArgDelimeters[0] + "-1";
	}

	public override string GetName()
	{
		return "PlaySound";
	}
}
