public class BfCommand
{
	public enum BF_COMMAND
	{
		NONE = -1,
		WHISPER_CMD,
		CAMERA_CMD,
		GUI_CMD,
		GOD_CMD,
		GHOST_CMD,
		SPEED_CMD,
		STRAIGHT_MOVEMENT_CMD,
		INVISIBLE_CMD,
		MUTE_CMD,
		BAN_CMD
	}

	private BF_COMMAND cmd;

	private string arg1;

	private string arg2;

	public BF_COMMAND Cmd => cmd;

	public string Arg1 => arg1;

	public string Arg2 => arg2;

	public BfCommand(BF_COMMAND _cmd, string _arg1, string _arg2)
	{
		cmd = _cmd;
		arg1 = _arg1;
		arg2 = _arg2;
	}
}
