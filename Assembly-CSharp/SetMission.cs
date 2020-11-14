public class SetMission : ScriptCmd
{
	private string progress;

	private string title;

	private string subTitle;

	private string tag = string.Empty;

	public string Progress
	{
		get
		{
			return progress;
		}
		set
		{
			progress = value;
		}
	}

	public string Title
	{
		get
		{
			return title;
		}
		set
		{
			title = value;
		}
	}

	public string SubTitle
	{
		get
		{
			return subTitle;
		}
		set
		{
			subTitle = value;
		}
	}

	public string Tag
	{
		get
		{
			return tag;
		}
		set
		{
			tag = value;
		}
	}

	public override string GetDescription()
	{
		string str = "setmission" + ScriptCmd.ArgDelimeters[0];
		str = str + progress + ScriptCmd.ArgDelimeters[0];
		str = str + title + ScriptCmd.ArgDelimeters[0];
		str = str + subTitle + ScriptCmd.ArgDelimeters[0];
		return str + tag;
	}

	public override int GetIconIndex()
	{
		return 8;
	}

	public static string GetDefaultDescription()
	{
		return "setmission" + ScriptCmd.ArgDelimeters[0] + string.Empty;
	}

	public override string GetName()
	{
		return "SetMission";
	}
}
