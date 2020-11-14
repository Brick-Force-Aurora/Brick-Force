public class ShowDialog : ScriptCmd
{
	private int speaker;

	private string dialog;

	public int Speaker
	{
		get
		{
			return speaker;
		}
		set
		{
			speaker = value;
		}
	}

	public string Dialog
	{
		get
		{
			return dialog;
		}
		set
		{
			dialog = value;
		}
	}

	public override string GetDescription()
	{
		string str = "showdialog" + ScriptCmd.ArgDelimeters[0];
		str = str + speaker.ToString() + ScriptCmd.ArgDelimeters[0];
		return str + dialog;
	}

	public override int GetIconIndex()
	{
		return 1;
	}

	public static string GetDefaultDescription()
	{
		return "showdialog" + ScriptCmd.ArgDelimeters[0] + "-1" + ScriptCmd.ArgDelimeters[0] + "...";
	}

	public override string GetName()
	{
		return "ShowDialog";
	}
}
