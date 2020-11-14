using UnityEngine;

public class RadioMenu : MonoBehaviour
{
	public enum RADIO
	{
		NONE = -1,
		ASK,
		CMD,
		REPLY
	}

	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.TOP_MENU;

	public string[] radioAskKeys;

	public string[] radioCmdKeys;

	public string[] radioReplyKeys;

	public string radioCancelKey;

	private string[] sndRadioAsks = new string[6]
	{
		"Request01_4",
		"Request02_3",
		"Request03",
		"Request04_1",
		"Request05",
		"Request06_1"
	};

	private string[] sndRadioCmds = new string[5]
	{
		"Order01_3",
		"Order02_2",
		"Order03",
		"Order04_1",
		"Order05_5"
	};

	private string[] sndRadioReplys = new string[6]
	{
		"Reply01_1",
		"Reply02_2",
		"Reply03_1",
		"Reply04",
		"Reply05_1",
		"Reply06_4"
	};

	private BattleChat battleChat;

	private AudioSource audioSource;

	public float offset = 24f;

	private LocalController localController;

	private RADIO mode = RADIO.NONE;

	public bool On => mode != RADIO.NONE;

	public void Cancel()
	{
		mode = RADIO.NONE;
	}

	private void Start()
	{
		battleChat = GetComponent<BattleChat>();
		audioSource = GetComponent<AudioSource>();
		GameObject gameObject = GameObject.Find("Me");
		if (null != gameObject)
		{
			localController = gameObject.GetComponent<LocalController>();
		}
	}

	private void CheckShortcut()
	{
		if (mode != RADIO.NONE)
		{
			KeyCode[] array = new KeyCode[9]
			{
				KeyCode.Alpha1,
				KeyCode.Alpha2,
				KeyCode.Alpha3,
				KeyCode.Alpha4,
				KeyCode.Alpha5,
				KeyCode.Alpha6,
				KeyCode.Alpha7,
				KeyCode.Alpha8,
				KeyCode.Alpha9
			};
			switch (mode)
			{
			case RADIO.ASK:
				for (int j = 0; j < 9 && j < radioAskKeys.Length; j++)
				{
					if (Input.GetKeyDown(array[j]))
					{
						CSNetManager.Instance.Sock.SendCS_RADIO_MSG_REQ((int)mode, j);
						return;
					}
				}
				break;
			case RADIO.CMD:
				for (int k = 0; k < 9 && k < radioCmdKeys.Length; k++)
				{
					if (Input.GetKeyDown(array[k]))
					{
						CSNetManager.Instance.Sock.SendCS_RADIO_MSG_REQ((int)mode, k);
						return;
					}
				}
				break;
			case RADIO.REPLY:
				for (int i = 0; i < 9 && i < radioReplyKeys.Length; i++)
				{
					if (Input.GetKeyDown(array[i]))
					{
						CSNetManager.Instance.Sock.SendCS_RADIO_MSG_REQ((int)mode, i);
						return;
					}
				}
				break;
			}
			if (Input.GetKeyUp(KeyCode.Alpha0))
			{
				mode = RADIO.NONE;
			}
		}
	}

	public string GetString(RadioSignal signal)
	{
		string result = string.Empty;
		switch (signal.Category)
		{
		case 0:
			result = StringMgr.Instance.Get(radioAskKeys[signal.Message]);
			break;
		case 1:
			result = StringMgr.Instance.Get(radioCmdKeys[signal.Message]);
			break;
		case 2:
			result = StringMgr.Instance.Get(radioReplyKeys[signal.Message]);
			break;
		}
		return result;
	}

	private void OnRadioMsg(RadioSignal signal)
	{
		bool flag = false;
		if (signal.Sender == MyInfoManager.Instance.Seq)
		{
			mode = RADIO.NONE;
		}
		else
		{
			GameObject gameObject = BrickManManager.Instance.Get(signal.Sender);
			if (gameObject != null)
			{
				LookCoordinator component = gameObject.GetComponent<LookCoordinator>();
				if (component != null && component.IsYang)
				{
					flag = true;
				}
			}
		}
		string text = string.Empty;
		switch (signal.Category)
		{
		case 0:
			if (0 <= signal.Message && signal.Message < sndRadioAsks.Length)
			{
				text = sndRadioAsks[signal.Message];
			}
			break;
		case 1:
			if (0 <= signal.Message && signal.Message < sndRadioCmds.Length)
			{
				text = sndRadioCmds[signal.Message];
			}
			break;
		case 2:
			if (0 <= signal.Message && signal.Message < sndRadioReplys.Length)
			{
				text = sndRadioReplys[signal.Message];
			}
			break;
		}
		int @int = PlayerPrefs.GetInt("RadioSndMute", 0);
		if (text.Length > 0 && audioSource != null && @int == 0)
		{
			if (!flag)
			{
				VoiceManager.Instance.Play(text);
			}
			else
			{
				VoiceManager.Instance.Play2(text);
			}
		}
	}

	private float GetHeight()
	{
		switch (mode)
		{
		case RADIO.ASK:
			return (float)(radioAskKeys.Length + 1) * offset;
		case RADIO.CMD:
			return (float)(radioCmdKeys.Length + 1) * offset;
		case RADIO.REPLY:
			return (float)(radioReplyKeys.Length + 1) * offset;
		default:
			return 0f;
		}
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn)
		{
			GUISkin skin = GUI.skin;
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.enabled = !DialogManager.Instance.IsModal;
			if (mode != RADIO.NONE)
			{
				float height = GetHeight();
				GUI.Box(new Rect(0f, 175f, 196f, height), string.Empty);
				float num = 175f;
				switch (mode)
				{
				case RADIO.ASK:
					for (int j = 0; j < radioAskKeys.Length; j++)
					{
						string str2 = StringMgr.Instance.Get(radioAskKeys[j]);
						LabelUtil.TextOut(new Vector2(5f, num), (j + 1).ToString() + "." + str2, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
						num += offset;
					}
					break;
				case RADIO.CMD:
					for (int k = 0; k < radioCmdKeys.Length; k++)
					{
						string str3 = StringMgr.Instance.Get(radioCmdKeys[k]);
						LabelUtil.TextOut(new Vector2(5f, num), (k + 1).ToString() + "." + str3, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
						num += offset;
					}
					break;
				case RADIO.REPLY:
					for (int i = 0; i < radioReplyKeys.Length; i++)
					{
						string str = StringMgr.Instance.Get(radioReplyKeys[i]);
						LabelUtil.TextOut(new Vector2(5f, num), (i + 1).ToString() + "." + str, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
						num += offset;
					}
					break;
				}
				LabelUtil.TextOut(new Vector2(5f, num), "0." + StringMgr.Instance.Get(radioCancelKey), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			GUI.enabled = true;
			GUI.skin = skin;
		}
	}

	private void CheckOnOff()
	{
		if (custom_inputs.Instance.GetButtonDown("K_TOGGLE_RADIO"))
		{
			if (PlayerPrefs.GetInt("RadioSndMute", 0) == 0 || 1 == 0)
			{
				PlayerPrefs.SetInt("RadioSndMute", 1);
			}
			else
			{
				PlayerPrefs.SetInt("RadioSndMute", 0);
			}
		}
	}

	private void Update()
	{
		if (!battleChat.IsChatting && !localController.IsDead && !DialogManager.Instance.IsModal)
		{
			if (custom_inputs.Instance.GetButtonDown("K_RADIO_ASK"))
			{
				mode = RADIO.ASK;
			}
			if (custom_inputs.Instance.GetButtonDown("K_RADIO_CMD"))
			{
				mode = RADIO.CMD;
			}
			if (custom_inputs.Instance.GetButtonDown("K_RADIO_REPLY"))
			{
				mode = RADIO.REPLY;
			}
			CheckShortcut();
		}
		CheckOnOff();
	}
}
