public class Sleep : ScriptCmd
{
	private float howlong;

	public float Howlong
	{
		get
		{
			return howlong;
		}
		set
		{
			howlong = value;
		}
	}

	public override string GetDescription()
	{
		string str = "sleep" + ScriptCmd.ArgDelimeters[0];
		return str + howlong.ToString("0.##");
	}

	public override int GetIconIndex()
	{
		return 3;
	}

	public static string GetDefaultDescription()
	{
		return "sleep" + ScriptCmd.ArgDelimeters[0] + "0";
	}

	public override string GetName()
	{
		return "Sleep";
	}
}
