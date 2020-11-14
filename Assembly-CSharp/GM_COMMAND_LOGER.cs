public class GM_COMMAND_LOGER
{
	public enum GM_COMMAND_LOG
	{
		CAMERA_FLY_ON,
		CAMERA_FLY_USE,
		CAMERA_SPECTATOR_ON,
		CAMERA_SPECTATOR_USE,
		CAMERA_OFF,
		GUI_ON,
		GUI_OFF,
		GOD_ON,
		GOD_USE,
		GOD_OFF,
		GHOST_ON,
		GHOST_USE,
		GHOST_OFF,
		SPEED_ON,
		SPEED_USE,
		SPEED_OFF,
		STRAIGHT_MOVEMENT_ON,
		STRAIGHT_MOVEMENT_USE,
		STRAIGHT_MOVEMENT_OFF,
		INVISIBLE_ON,
		INVISIBLE_USE,
		INVISIBLE_OFF,
		MUTE_ON,
		MUTE_OFF,
		NUM
	}

	private static bool[] sendUseCommand;

	public static void SendLog(GM_COMMAND_LOG log)
	{
		if (GM_COMMAND_LOG.NUM > log)
		{
			if (sendUseCommand == null)
			{
				sendUseCommand = new bool[24];
				for (int i = 0; i < 24; i++)
				{
					sendUseCommand[i] = false;
				}
			}
			if (IsResendAble(log))
			{
				CSNetManager.Instance.Sock.SendCS_GM_COMMAND_USAGE_LOG_REQ((int)log);
			}
		}
	}

	private static bool IsResendAble(GM_COMMAND_LOG log)
	{
		switch (log)
		{
		case GM_COMMAND_LOG.CAMERA_FLY_USE:
		case GM_COMMAND_LOG.CAMERA_SPECTATOR_USE:
		case GM_COMMAND_LOG.GOD_USE:
		case GM_COMMAND_LOG.GHOST_USE:
		case GM_COMMAND_LOG.SPEED_USE:
		case GM_COMMAND_LOG.STRAIGHT_MOVEMENT_USE:
		case GM_COMMAND_LOG.INVISIBLE_USE:
			if (!sendUseCommand[(int)log])
			{
				sendUseCommand[(int)log] = true;
				return true;
			}
			return false;
		default:
			return true;
		}
	}
}
