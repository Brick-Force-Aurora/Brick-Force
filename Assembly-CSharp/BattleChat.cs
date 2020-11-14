using System.Collections.Generic;
using UnityEngine;

public class BattleChat : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	public Vector2 offset = new Vector2(5f, 100f);

	private float chatBtnSize = 100f;

	private float width = 256f;

	public int maxMessageLength = 100;

	public int maxChatQueueSize = 100;

	private Vector2 scrollPosition = Vector2.zero;

	private Vector2 viewSizeCopy = Vector2.zero;

	private Rect crdChatRead = new Rect(215f, 460f, 320f, 220f);

	private int selected;

	private bool chatting;

	private bool cursorToEnd;

	private ChatText.CHAT_TYPE chatMode;

	private string message = string.Empty;

	private Queue<ChatText> chatQ;

	private bool startTransParency;

	private float Elapsed;

	private float ElapsedMax = 15f;

	private float textAlpha = 1f;

	private Color UiColor = new Color(1f, 1f, 1f, 1f);

	public bool IsChatting => chatting;

	public bool CursorToEnd
	{
		set
		{
			cursorToEnd = value;
		}
	}

	public ChatText.CHAT_TYPE ChatMode
	{
		get
		{
			return chatMode;
		}
		set
		{
			chatMode = value;
			message = string.Empty;
			GlobalVars.Instance.whisperNickTo = string.Empty;
		}
	}

	private void Awake()
	{
		chatQ = new Queue<ChatText>();
		chatMode = ChatText.CHAT_TYPE.NORMAL;
	}

	private void Start()
	{
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			ChatSelectBtns component = gameObject.GetComponent<ChatSelectBtns>();
			if (component != null)
			{
				component.chatModeBattle();
			}
		}
	}

	public void Cancel()
	{
		message = string.Empty;
		chatting = false;
	}

	private void ApplyFocus()
	{
		string nameOfFocusedControl = GUI.GetNameOfFocusedControl();
		if (nameOfFocusedControl != "ChatInput")
		{
			GUI.FocusControl("ChatInput");
		}
		ChatSelectBtns component = GameObject.Find("Main").GetComponent<ChatSelectBtns>();
		if (component != null)
		{
			component.rcBox(offset.x + chatBtnSize, (float)Screen.height - offset.y + 2f);
		}
	}

	private void CalcViewSize(int selectedTab)
	{
		viewSizeCopy.x = crdChatRead.width - 13f;
		viewSizeCopy.y = 0f;
		foreach (ChatText item in chatQ)
		{
			if (item.Filtered(selectedTab))
			{
				string fullMessage = item.FullMessage;
				GUIStyle style = GUI.skin.GetStyle("MissionLabel");
				Vector2 vector = style.CalcSize(new GUIContent(fullMessage));
				if (vector.x > viewSizeCopy.x)
				{
					vector.y = style.CalcHeight(new GUIContent(fullMessage), viewSizeCopy.x);
				}
				viewSizeCopy.y += vector.y;
			}
		}
	}

	private void SetTranceparancy(int set)
	{
		switch (set)
		{
		case 1:
			startTransParency = true;
			break;
		case 0:
			startTransParency = false;
			textAlpha = 1f;
			Elapsed = 0f;
			UiColor.a = 1f;
			break;
		case -1:
			startTransParency = true;
			textAlpha = 1f;
			Elapsed = 0f;
			UiColor.a = 1f;
			break;
		}
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn)
		{
			CheckChatKey();
			GUISkin gUISkin = GUISkinFinder.Instance.GetGUISkin();
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			UiColor.a = textAlpha;
			if (chatting)
			{
				ApplyFocus();
				string text = message;
				switch (chatMode)
				{
				case ChatText.CHAT_TYPE.NORMAL:
					GUI.SetNextControlName("ChatInput");
					message = GUI.TextField(new Rect(offset.x + chatBtnSize + 1f, (float)Screen.height - offset.y, width, 29f), message);
					break;
				case ChatText.CHAT_TYPE.CLAN:
					GUI.SetNextControlName("ChatInput");
					message = GUI.TextField(new Rect(offset.x + chatBtnSize + 1f, (float)Screen.height - offset.y, width, 29f), message, "ClanChat");
					break;
				case ChatText.CHAT_TYPE.TEAM:
					GUI.SetNextControlName("ChatInput");
					message = GUI.TextField(new Rect(offset.x + chatBtnSize + 1f, (float)Screen.height - offset.y, width, 29f), message, "TeamChat");
					break;
				case ChatText.CHAT_TYPE.SYSTEM:
					GUI.SetNextControlName("ChatInput");
					message = GUI.TextField(new Rect(offset.x + chatBtnSize + 1f, (float)Screen.height - offset.y, width, 29f), message, "GmChat");
					break;
				}
				if (message.Length > maxMessageLength)
				{
					message = text;
				}
				if (GlobalVars.Instance.whisperNickFrom.Length > 0 && CommandInterpreter.Instance.IsReturnWhisper(message))
				{
					message = "/w " + GlobalVars.Instance.whisperNickFrom + " ";
					cursorToEnd = true;
				}
				if (cursorToEnd)
				{
					cursorToEnd = false;
					TextEditor textEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
					if (textEditor != null)
					{
						textEditor.pos = 1000;
						textEditor.selectPos = 1000;
					}
				}
			}
			float num = viewSizeCopy.y - crdChatRead.height;
			if (num < 0f)
			{
				num = 0f;
			}
			float num2 = crdChatRead.height - viewSizeCopy.y;
			num2 = ((!(num2 < 0f)) ? 0f : (0f - num2));
			bool flag = num <= scrollPosition.y;
			CalcViewSize(selected);
			if (flag)
			{
				scrollPosition.y = viewSizeCopy.y - crdChatRead.height;
				if (scrollPosition.y < 0f)
				{
					scrollPosition.y = 0f;
				}
			}
			crdChatRead.x = offset.x;
			crdChatRead.y = (float)Screen.height - offset.y - 220f;
			bool flag2 = false;
			if (textAlpha > 0f)
			{
				flag2 = true;
				scrollPosition = GUI.BeginScrollView(crdChatRead, scrollPosition, new Rect(0f, 0f, viewSizeCopy.x, viewSizeCopy.y));
			}
			Vector2 pos = new Vector2(0f, 220f + num2);
			ChatText[] array = chatQ.ToArray();
			for (int num3 = array.Length - 1; num3 >= 0; num3--)
			{
				if (array[num3].Filtered(selected))
				{
					Vector2 vector = LabelUtil.CalcSize("MissionLabel", array[num3].FullMessage, viewSizeCopy.x);
					pos.y -= vector.y;
					array[num3].setTextAlpha(textAlpha);
					array[num3].setOutTextAlpha(textAlpha);
					LabelUtil.TextOut(pos, array[num3].FullMessage, "MissionLabel", array[num3].TextColor, array[num3].OutlineColor, TextAnchor.UpperLeft, viewSizeCopy.x);
				}
			}
			if (flag2)
			{
				GUI.EndScrollView();
			}
			GUI.color = new Color(1f, 1f, 1f, 1f);
			GUI.enabled = true;
			GUI.skin = gUISkin;
		}
	}

	private void OnChat(ChatText chatText)
	{
		Enqueue(chatText);
	}

	private void Enqueue(ChatText chatText)
	{
		if (chatQ != null)
		{
			chatQ.Enqueue(chatText);
			SetTranceparancy(-1);
			while (chatQ.Count > maxChatQueueSize)
			{
				chatQ.Dequeue();
			}
		}
	}

	private void CheckChatKey()
	{
		if (Event.current.type == EventType.KeyDown && !DialogManager.Instance.IsModal)
		{
			if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
			{
				chatting = !chatting;
				if (chatting)
				{
					SetTranceparancy(0);
				}
				else
				{
					SetTranceparancy(1);
				}
				if (!chatting && message.Length > 0)
				{
					CommandInterpreter.Instance.IsWhisper = false;
					if (ChatLogManager.Instance.Log(message) && !CommandInterpreter.Instance.Parse(message))
					{
						message = RemoveSystemKey(message);
						switch (chatMode)
						{
						case ChatText.CHAT_TYPE.NORMAL:
							if (MyInfoManager.Instance.CheckChatTime())
							{
								CSNetManager.Instance.Sock.SendCS_CHAT_REQ(message);
							}
							else
							{
								string text = string.Format(StringMgr.Instance.Get("LIMITED_CHAT_TIME"), CustomGameConfig.limitChatTime, CustomGameConfig.limitChatCount, CustomGameConfig.chatBlockTime);
								Enqueue(new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, string.Empty, text));
							}
							break;
						case ChatText.CHAT_TYPE.CLAN:
							CSNetManager.Instance.Sock.SendCS_CLAN_CHAT_REQ(message);
							break;
						case ChatText.CHAT_TYPE.TEAM:
							CSNetManager.Instance.Sock.SendCS_TEAM_CHAT_REQ(message);
							break;
						case ChatText.CHAT_TYPE.SYSTEM:
							CSNetManager.Instance.Sock.SendCS_GM_SAYS_REQ(message);
							break;
						}
					}
					message = string.Empty;
					if (!CommandInterpreter.Instance.IsWhisper)
					{
						GlobalVars.Instance.whisperNickTo = string.Empty;
					}
					if (GlobalVars.Instance.whisperNickTo.Length > 0)
					{
						message = "/w " + GlobalVars.Instance.whisperNickTo + " ";
						cursorToEnd = true;
					}
				}
			}
			else if (chatting && Event.current.keyCode == KeyCode.Escape)
			{
				chatting = !chatting;
				message = string.Empty;
				GlobalVars.Instance.resetMenuEx();
			}
			else if (MyInfoManager.Instance.IsGM && Input.GetKeyDown(KeyCode.F4))
			{
				chatMode = ChatText.CHAT_TYPE.SYSTEM;
				if (!chatting)
				{
					chatting = true;
				}
			}
			else if (Event.current.keyCode == KeyCode.UpArrow)
			{
				if (chatting)
				{
					string command = message;
					command = CommandInterpreter.Instance.GetNextCommand(command);
					if (command.Length > 0)
					{
						message = command;
					}
				}
			}
			else if (Event.current.keyCode == KeyCode.DownArrow && chatting)
			{
				string command2 = message;
				command2 = CommandInterpreter.Instance.GetPrevCommand(command2);
				if (command2.Length > 0)
				{
					message = command2;
				}
			}
		}
	}

	private void CheckChatKey2()
	{
		if (!DialogManager.Instance.IsModal)
		{
			if (custom_inputs.Instance.GetButtonDown("K_NORMAL_CHAT"))
			{
				chatMode = ChatText.CHAT_TYPE.NORMAL;
				changeChildButton();
				if (!chatting)
				{
					chatting = true;
				}
			}
			else if (custom_inputs.Instance.GetButtonDown("K_CLAN_CHAT"))
			{
				if (MyInfoManager.Instance.IsClanMember)
				{
					chatMode = ChatText.CHAT_TYPE.CLAN;
					changeChildButton();
					if (!chatting)
					{
						chatting = true;
					}
				}
			}
			else if (custom_inputs.Instance.GetButtonDown("K_TEAM_CHAT"))
			{
				chatMode = ChatText.CHAT_TYPE.TEAM;
				changeChildButton();
				if (!chatting)
				{
					chatting = true;
				}
			}
		}
	}

	private void changeChildButton()
	{
		ChatSelectBtns component = GameObject.Find("Main").GetComponent<ChatSelectBtns>();
		if (component != null)
		{
			message = string.Empty;
			GlobalVars.Instance.whisperNickTo = string.Empty;
			component.changeChildIdx(chatMode);
		}
	}

	private void Update()
	{
		CheckChatKey2();
		if (startTransParency)
		{
			Elapsed += Time.deltaTime;
			if (Elapsed > ElapsedMax)
			{
				textAlpha -= Time.deltaTime;
				if (textAlpha < 0f)
				{
					textAlpha = 0f;
				}
			}
		}
	}

	private string RemoveSystemKey(string msg)
	{
		msg = msg.Replace("\n", string.Empty);
		msg = msg.Replace("\t", string.Empty);
		msg = msg.Replace("[", " ");
		msg = msg.Replace("]", " ");
		return msg;
	}
}
