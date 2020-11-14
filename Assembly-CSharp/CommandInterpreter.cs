using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CommandInterpreter : MonoBehaviour
{
	private bool isLoaded;

	private List<string> cmdLog;

	private Dictionary<string, string> dic;

	private static CommandInterpreter _instance;

	public bool IsWhisper;

	public bool IsLoaded => isLoaded;

	public static CommandInterpreter Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (UnityEngine.Object.FindObjectOfType(typeof(CommandInterpreter)) as CommandInterpreter);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the CommandInterpreter Instance");
				}
			}
			return _instance;
		}
	}

	private void Start()
	{
	}

	public void Load()
	{
		Property props = BuildOption.Instance.Props;
		if (props.isWebPlayer)
		{
			StartCoroutine(LoadFromWWW());
		}
		else
		{
			isLoaded = LoadFromLocalFileSystem();
		}
	}

	private void Awake()
	{
		cmdLog = new List<string>();
		dic = new Dictionary<string, string>();
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void ParseData(CSVLoader csvLoader)
	{
		for (int i = 0; i < csvLoader.Rows; i++)
		{
			csvLoader.ReadValue(0, i, string.Empty, out string Value);
			Value.Trim();
			csvLoader.ReadValue(1, i, string.Empty, out string Value2);
			Value2.Trim();
			if (Value.Length <= 0 || Value2.Length <= 0)
			{
				Debug.LogError("ERROR, Empty key or value at row: " + i);
			}
			if (dic.ContainsKey(Value))
			{
				Debug.LogError("ERROR, duplicate command key: " + Value + " at row: " + i);
			}
			else
			{
				dic.Add(Value, Value2);
			}
		}
	}

	private bool LoadFromLocalFileSystem()
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for Resources");
			return false;
		}
		string text2 = Path.Combine(text, "Template/command.txt");
		CSVLoader cSVLoader = new CSVLoader();
		if (Application.platform == RuntimePlatform.WindowsEditor || !cSVLoader.SecuredLoad(text2))
		{
			if (!cSVLoader.Load(text2))
			{
				Debug.LogError("ERROR, Fail to load resource file" + text2);
				return false;
			}
			if (!cSVLoader.SecuredSave(text2))
			{
				Debug.LogError("ERROR, Load success " + text2 + " but save secured failed");
			}
		}
		ParseData(cSVLoader);
		return true;
	}

	private IEnumerator LoadFromWWW()
	{
		Property prop = BuildOption.Instance.Props;
		string url = "http://" + prop.GetResourceServer + "/BfData/Template/command.txt.cooked";
		WWW www = new WWW(url);
		yield return (object)www;
		using (MemoryStream stream = new MemoryStream(www.bytes))
		{
			using (BinaryReader reader = new BinaryReader(stream))
			{
				CSVLoader csvLoader = new CSVLoader();
				if (csvLoader.SecuredLoadFromBinaryReader(reader))
				{
					ParseData(csvLoader);
					isLoaded = true;
				}
			}
		}
		if (!isLoaded)
		{
			Debug.LogError("Fail to download " + url);
		}
	}

	public bool IsReturnWhisper(string command)
	{
		command = command.TrimStart();
		if (command.Length <= 0)
		{
			return false;
		}
		if (command[0] != '/')
		{
			return false;
		}
		command = command.TrimStart('/');
		string[] array = command.Split(new char[1]
		{
			' '
		}, 3, StringSplitOptions.RemoveEmptyEntries);
		if (array != null && array.Length > 0)
		{
			string key = array[0].ToLower();
			if (dic.ContainsKey(key))
			{
				string a = dic[key];
				if (a == "return_whisper")
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool Parse(string command)
	{
		command = command.TrimStart();
		if (command.Length <= 0)
		{
			return false;
		}
		if (command[0] != '/')
		{
			return false;
		}
		command = command.TrimStart('/');
		string[] array = command.Split(new char[1]
		{
			' '
		}, 3, StringSplitOptions.RemoveEmptyEntries);
		BfCommand cmd = null;
		if (array != null && array.Length > 0)
		{
			string key = array[0].ToLower();
			if (dic.ContainsKey(key))
			{
				string text = dic[key];
				switch (text)
				{
				case "whisper":
					if (array.Length >= 3)
					{
						cmd = new BfCommand(BfCommand.BF_COMMAND.WHISPER_CMD, array[1], array[2]);
						if (array[1].ToLower() == MyInfoManager.Instance.Nickname.ToLower())
						{
							return true;
						}
						if (MyInfoManager.Instance.IsBan(array[1]))
						{
							GameObject gameObject = GameObject.Find("Main");
							if (null != gameObject)
							{
								string text2 = StringMgr.Instance.Get("WHISPER_LIMIT_MASSAGE01");
								gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, string.Empty, text2));
							}
							return true;
						}
						IsWhisper = true;
						GlobalVars.Instance.whisperNickTo = array[1];
					}
					else if (array.Length == 2)
					{
						return true;
					}
					break;
				case "camera":
					if (array.Length >= 2 && MyInfoManager.Instance.IsGM)
					{
						cmd = new BfCommand(BfCommand.BF_COMMAND.CAMERA_CMD, array[1].ToLower(), string.Empty);
					}
					break;
				case "ghost":
					if (array.Length >= 2 && MyInfoManager.Instance.IsGM)
					{
						cmd = new BfCommand(BfCommand.BF_COMMAND.GHOST_CMD, array[1].ToLower(), string.Empty);
					}
					break;
				case "god":
					if (array.Length >= 2 && MyInfoManager.Instance.IsGM)
					{
						cmd = new BfCommand(BfCommand.BF_COMMAND.GOD_CMD, array[1].ToLower(), string.Empty);
					}
					break;
				case "gui":
					if (array.Length >= 2 && MyInfoManager.Instance.IsGM)
					{
						cmd = new BfCommand(BfCommand.BF_COMMAND.GUI_CMD, array[1].ToLower(), string.Empty);
					}
					break;
				case "go":
					if (array.Length >= 2 && MyInfoManager.Instance.IsGM)
					{
						cmd = new BfCommand(BfCommand.BF_COMMAND.STRAIGHT_MOVEMENT_CMD, array[1].ToLower(), string.Empty);
					}
					break;
				case "invisible":
					if (array.Length >= 2 && MyInfoManager.Instance.IsGM)
					{
						cmd = new BfCommand(BfCommand.BF_COMMAND.INVISIBLE_CMD, array[1].ToLower(), string.Empty);
					}
					break;
				case "mute":
					if (array.Length >= 3 && MyInfoManager.Instance.IsGM)
					{
						cmd = new BfCommand(BfCommand.BF_COMMAND.MUTE_CMD, array[1].ToLower(), array[2].ToLower());
					}
					break;
				case "block":
					if (array.Length >= 2)
					{
						cmd = new BfCommand(BfCommand.BF_COMMAND.BAN_CMD, array[1].ToLower(), string.Empty);
					}
					break;
				}
			}
		}
		if (!Execute(cmd))
		{
			return false;
		}
		LogCommand("/" + command);
		return true;
	}

	private void LogCommand(string command)
	{
		for (int i = 0; i < cmdLog.Count; i++)
		{
			if (cmdLog[i] == command)
			{
				if (i > 0)
				{
					string value = cmdLog[0];
					cmdLog[0] = command;
					cmdLog[i] = value;
				}
				return;
			}
		}
		cmdLog.Insert(0, command);
	}

	public string GetNextCommand(string command)
	{
		if (command.Length > 0 && cmdLog.Count > 0)
		{
			int num = cmdLog.LastIndexOf(command);
			if (0 <= num && num < cmdLog.Count)
			{
				if (num + 1 >= cmdLog.Count)
				{
					return command;
				}
				return cmdLog[num + 1];
			}
		}
		if (cmdLog.Count <= 0)
		{
			return string.Empty;
		}
		return cmdLog[0];
	}

	public string GetPrevCommand(string command)
	{
		if (command.Length > 0 && cmdLog.Count > 0)
		{
			int num = cmdLog.LastIndexOf(command);
			if (0 <= num && num < cmdLog.Count)
			{
				if (num == 0)
				{
					return cmdLog[0];
				}
				return cmdLog[num - 1];
			}
		}
		return string.Empty;
	}

	private bool Execute(BfCommand cmd)
	{
		GameObject gameObject = GameObject.Find("Main");
		if (cmd != null)
		{
			switch (cmd.Cmd)
			{
			case BfCommand.BF_COMMAND.WHISPER_CMD:
				CSNetManager.Instance.Sock.SendCS_WHISPER_REQ(cmd.Arg1, cmd.Arg2);
				return true;
			case BfCommand.BF_COMMAND.CAMERA_CMD:
				switch (cmd.Arg1)
				{
				case "fly":
					MyInfoManager.Instance.ControlMode = MyInfoManager.CONTROL_MODE.FLY_MODE;
					GM_COMMAND_LOGER.SendLog(GM_COMMAND_LOGER.GM_COMMAND_LOG.CAMERA_FLY_ON);
					if (null != gameObject)
					{
						gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, StringMgr.Instance.Get("MODE_CHANGE"), StringMgr.Instance.Get("FLY_MODE")));
					}
					break;
				case "spectator":
					MyInfoManager.Instance.ControlMode = MyInfoManager.CONTROL_MODE.SPECTATOR_MODE;
					GM_COMMAND_LOGER.SendLog(GM_COMMAND_LOGER.GM_COMMAND_LOG.CAMERA_SPECTATOR_ON);
					if (null != gameObject)
					{
						gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, StringMgr.Instance.Get("MODE_CHANGE"), StringMgr.Instance.Get("SPECTATOR_MODE")));
					}
					break;
				case "play":
					MyInfoManager.Instance.ControlMode = MyInfoManager.CONTROL_MODE.PLAY_MODE;
					GM_COMMAND_LOGER.SendLog(GM_COMMAND_LOGER.GM_COMMAND_LOG.CAMERA_OFF);
					if (null != gameObject)
					{
						gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, StringMgr.Instance.Get("MODE_CHANGE"), StringMgr.Instance.Get("PLAY_MODE")));
					}
					break;
				}
				return true;
			case BfCommand.BF_COMMAND.GOD_CMD:
				switch (cmd.Arg1)
				{
				case "on":
				case "true":
					MyInfoManager.Instance.GodMode = true;
					NoCheat.Instance.Sync(NoCheat.WATCH_DOG.GODMODE, MyInfoManager.Instance.GodMode ? 1 : 0);
					GM_COMMAND_LOGER.SendLog(GM_COMMAND_LOGER.GM_COMMAND_LOG.GOD_ON);
					if (null != gameObject)
					{
						gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, StringMgr.Instance.Get("MODE_CHANGE"), StringMgr.Instance.Get("GOD_MODE_ON")));
					}
					break;
				case "off":
				case "false":
					MyInfoManager.Instance.GodMode = false;
					NoCheat.Instance.Sync(NoCheat.WATCH_DOG.GODMODE, MyInfoManager.Instance.GodMode ? 1 : 0);
					GM_COMMAND_LOGER.SendLog(GM_COMMAND_LOGER.GM_COMMAND_LOG.GOD_OFF);
					if (null != gameObject)
					{
						gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, StringMgr.Instance.Get("MODE_CHANGE"), StringMgr.Instance.Get("GOD_MODE_OFF")));
					}
					break;
				}
				return true;
			case BfCommand.BF_COMMAND.GHOST_CMD:
				switch (cmd.Arg1)
				{
				case "on":
				case "true":
					MyInfoManager.Instance.isGhostOn = true;
					NoCheat.Instance.Sync(NoCheat.WATCH_DOG.GHOSTMODE, MyInfoManager.Instance.isGhostOn ? 1 : 0);
					GM_COMMAND_LOGER.SendLog(GM_COMMAND_LOGER.GM_COMMAND_LOG.GHOST_ON);
					if (null != gameObject)
					{
						gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, StringMgr.Instance.Get("MODE_CHANGE"), StringMgr.Instance.Get("GHOST_MODE_ON")));
					}
					MyInfoManager.Instance.ControlMode = MyInfoManager.CONTROL_MODE.FLY_MODE;
					break;
				case "off":
				case "false":
					MyInfoManager.Instance.isGhostOn = false;
					NoCheat.Instance.Sync(NoCheat.WATCH_DOG.GHOSTMODE, MyInfoManager.Instance.isGhostOn ? 1 : 0);
					GM_COMMAND_LOGER.SendLog(GM_COMMAND_LOGER.GM_COMMAND_LOG.GHOST_OFF);
					if (null != gameObject)
					{
						gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, StringMgr.Instance.Get("MODE_CHANGE"), StringMgr.Instance.Get("GHOST_MODE_OFF")));
					}
					MyInfoManager.Instance.ControlMode = MyInfoManager.CONTROL_MODE.PLAY_MODE;
					break;
				}
				return true;
			case BfCommand.BF_COMMAND.GUI_CMD:
				switch (cmd.Arg1)
				{
				case "on":
				{
					MyInfoManager.Instance.isGuiOn = true;
					GM_COMMAND_LOGER.SendLog(GM_COMMAND_LOGER.GM_COMMAND_LOG.GUI_ON);
					GameObject gameObject3 = GameObject.Find("Main Camera");
					if (null == gameObject3)
					{
						Debug.LogError("Fail to find mainCamera for radar");
					}
					else
					{
						CameraController component2 = gameObject3.GetComponent<CameraController>();
						if (null == component2)
						{
							Debug.LogError("Fail to get CameraController for radar");
						}
						component2.EnableFpCam(enable: true);
					}
					break;
				}
				case "off":
				{
					MyInfoManager.Instance.isGuiOn = false;
					GM_COMMAND_LOGER.SendLog(GM_COMMAND_LOGER.GM_COMMAND_LOG.GUI_OFF);
					GameObject gameObject2 = GameObject.Find("Main Camera");
					if (null == gameObject2)
					{
						Debug.LogError("Fail to find mainCamera for radar");
					}
					else
					{
						CameraController component = gameObject2.GetComponent<CameraController>();
						if (null == component)
						{
							Debug.LogError("Fail to get CameraController for radar");
						}
						component.EnableFpCam(enable: false);
					}
					break;
				}
				}
				return true;
			case BfCommand.BF_COMMAND.STRAIGHT_MOVEMENT_CMD:
				switch (cmd.Arg1)
				{
				case "on":
				case "true":
					MyInfoManager.Instance.isStraightMovement = true;
					GM_COMMAND_LOGER.SendLog(GM_COMMAND_LOGER.GM_COMMAND_LOG.STRAIGHT_MOVEMENT_ON);
					if (null != gameObject)
					{
						gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, StringMgr.Instance.Get("MODE_CHANGE"), StringMgr.Instance.Get("STRAIGHT_MOVEMENT_MODE_ON")));
					}
					break;
				case "off":
				case "false":
					MyInfoManager.Instance.isStraightMovement = false;
					GM_COMMAND_LOGER.SendLog(GM_COMMAND_LOGER.GM_COMMAND_LOG.STRAIGHT_MOVEMENT_OFF);
					if (null != gameObject)
					{
						gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, StringMgr.Instance.Get("MODE_CHANGE"), StringMgr.Instance.Get("STRAIGHT_MOVEMENT_OFF")));
					}
					break;
				}
				return true;
			case BfCommand.BF_COMMAND.INVISIBLE_CMD:
				switch (cmd.Arg1)
				{
				case "on":
				case "true":
					MyInfoManager.Instance.isInvisibilityOn = true;
					GM_COMMAND_LOGER.SendLog(GM_COMMAND_LOGER.GM_COMMAND_LOG.INVISIBLE_ON);
					if (null != gameObject)
					{
						gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, StringMgr.Instance.Get("MODE_CHANGE"), StringMgr.Instance.Get("INVISIBLE_MODE_ON")));
					}
					break;
				case "off":
				case "false":
					MyInfoManager.Instance.isInvisibilityOn = false;
					GM_COMMAND_LOGER.SendLog(GM_COMMAND_LOGER.GM_COMMAND_LOG.INVISIBLE_OFF);
					if (null != gameObject)
					{
						gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, StringMgr.Instance.Get("MODE_CHANGE"), StringMgr.Instance.Get("INVISIBLE_MODE_OFF")));
					}
					break;
				}
				return true;
			case BfCommand.BF_COMMAND.MUTE_CMD:
				CSNetManager.Instance.Sock.SendCS_MUTE_REQ(cmd.Arg1, Convert.ToInt32(cmd.Arg2));
				return true;
			case BfCommand.BF_COMMAND.BAN_CMD:
				CSNetManager.Instance.Sock.SendCS_ADD_BAN_BY_NICKNAME_REQ(cmd.Arg1);
				return true;
			}
		}
		return false;
	}

	public static string GetSystemParameters()
	{
		string text = string.Empty;
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		for (int i = 1; i < commandLineArgs.Length; i++)
		{
			text += commandLineArgs[i];
			if (i + 1 < commandLineArgs.Length)
			{
				text += " ";
			}
		}
		return text;
	}

	public static string ExtractValueFromParameter(string name, string def)
	{
		name = name.ToLower();
		string systemParameters = GetSystemParameters();
		string[] array = systemParameters.Split(new char[1]
		{
			' '
		}, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[1]
			{
				'='
			}, 2, StringSplitOptions.RemoveEmptyEntries);
			if (array2 != null && array2.Length == 2)
			{
				string a = array2[0].ToLower();
				if (a == name)
				{
					return array2[1];
				}
			}
		}
		return def;
	}

	public static string ExtractValueFromParameter(string parameters, string name, string def)
	{
		name = name.ToLower();
		string[] array = parameters.Split(new char[1]
		{
			' '
		}, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[1]
			{
				'='
			}, 2, StringSplitOptions.RemoveEmptyEntries);
			if (array2 != null && array2.Length == 2)
			{
				string a = array2[0].ToLower();
				if (a == name)
				{
					return array2[1];
				}
			}
		}
		return def;
	}

	public static string[] ExtractValueFromParameterByRunup()
	{
		string systemParameters = GetSystemParameters();
		string[] array = systemParameters.Split(new char[2]
		{
			' ',
			'/'
		}, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length != 4)
		{
			return null;
		}
		string[] array2 = new string[4];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = array[i].Trim();
		}
		return array2;
	}

	public static string[] ExtractValueFromParameterByRunup(string parameters)
	{
		string[] array = parameters.Split(new char[2]
		{
			' ',
			'/'
		}, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length != 4)
		{
			return null;
		}
		string[] array2 = new string[4];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = array[i].Trim();
		}
		return array2;
	}
}
