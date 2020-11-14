using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LobbyChat
{
	private Queue<ChatText> chatQ;

	private Vector2 scrollPosition = Vector2.zero;

	private Vector2 viewSizeCopy = Vector2.zero;

	private string message = string.Empty;

	public int maxMessageLength = 100;

	public int maxChatQueueSize = 100;

	private ChatText.CHAT_TYPE chatMode;

	private int selected;

	private Rect crdChatClientHigh = new Rect(249f, 238f, 556f, 524f);

	private Rect crdChatClientMid = new Rect(14f, 488f, 790f, 274f);

	private Rect crdChatClientLow = new Rect(249f, 488f, 556f, 274f);

	private Rect crdChatClientCM = new Rect(15f, 477f, 678f, 274f);

	private float chatReadStartY = 36f;

	private float chatReadHeightSub = 70f;

	private Rect crdInputTxtFldCM = new Rect(115f, 723f, 576f, 26f);

	private Rect crdInputTxtFldGR = new Rect(115f, 736f, 689f, 26f);

	private Rect crdInputTxtFldLobby = new Rect(350f, 736f, 455f, 26f);

	public Rect crdInputTxtFld = new Rect(350f, 736f, 556f, 26f);

	private Rect crdChatRead = new Rect(215f, 460f, 338f, 110f);

	private Rect crdClient = new Rect(249f, 488f, 556f, 230f);

	private bool bBtnActive = true;

	private bool cursorToEnd;

	public string Message
	{
		get
		{
			return message;
		}
		set
		{
			message = value;
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

	public bool CursorToEnd
	{
		set
		{
			cursorToEnd = value;
		}
	}

	public void BtnActive(bool set)
	{
		bBtnActive = set;
	}

	public void Start()
	{
		chatQ = new Queue<ChatText>();
		chatMode = ChatText.CHAT_TYPE.NORMAL;
		bBtnActive = true;
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			ChatSelectBtns component = gameObject.GetComponent<ChatSelectBtns>();
			if (component != null)
			{
				component.chatModeLobby();
			}
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

	public void Enqueue(ChatText chatText)
	{
		if (chatQ != null)
		{
			chatQ.Enqueue(chatText);
			while (chatQ.Count > maxChatQueueSize)
			{
				chatQ.Dequeue();
			}
		}
	}

	public void ApplyFocus()
	{
		string nameOfFocusedControl = GUI.GetNameOfFocusedControl();
		if (!DialogManager.Instance.IsModal && nameOfFocusedControl != "SearchKeyInput" && nameOfFocusedControl != "ChatInput")
		{
			GUI.FocusControl("ChatInput");
		}
	}

	public void SetChatStyle(LOBBYCHAT_STYLE style)
	{
		switch (style)
		{
		case LOBBYCHAT_STYLE.HIGH:
			crdInputTxtFld = crdInputTxtFldLobby;
			crdClient = crdChatClientHigh;
			crdChatRead = crdChatClientHigh;
			break;
		case LOBBYCHAT_STYLE.MIDDLE:
			crdInputTxtFld = crdInputTxtFldGR;
			crdClient = crdChatClientMid;
			crdChatRead = crdChatClientMid;
			break;
		case LOBBYCHAT_STYLE.LOW:
			crdInputTxtFld = crdInputTxtFldLobby;
			crdClient = crdChatClientLow;
			crdChatRead = crdChatClientLow;
			break;
		case LOBBYCHAT_STYLE.CLANMATCH:
			crdInputTxtFld = crdInputTxtFldCM;
			crdClient = crdChatClientCM;
			crdChatRead = crdChatClientCM;
			break;
		}
		crdChatRead.y += chatReadStartY;
		crdChatRead.height -= chatReadHeightSub;
		ChatSelectBtns component = GameObject.Find("Main").GetComponent<ChatSelectBtns>();
		if (component != null)
		{
			component.rcBox(crdInputTxtFld.x, crdInputTxtFld.y);
		}
	}

	public void hideCloseButton(bool close)
	{
	}

	public void OnGUI()
	{
		int num3 = GUI.depth = (GUI.depth = 50);
		Rect position = new Rect(crdClient);
		GUI.Box(position, string.Empty, "BoxChatBase");
		Rect position2 = new Rect(crdChatRead);
		position2.y = crdChatRead.y - 35f;
		position2.width = crdChatRead.width - 120f;
		position2.height = 34f;
		string[] array = new string[3]
		{
			StringMgr.Instance.Get("ALL"),
			StringMgr.Instance.Get("WHISPER"),
			StringMgr.Instance.Get("CLAN")
		};
		if (!DialogManager.Instance.IsModal && !bBtnActive)
		{
			GUI.enabled = false;
		}
		int num4 = selected;
		selected = GUI.SelectionGrid(position2, selected, array, array.Length, "BtnChat");
		if (num4 != selected)
		{
			if (selected == 0)
			{
				chatMode = ChatText.CHAT_TYPE.NORMAL;
			}
			else if (selected == 1)
			{
				chatMode = ChatText.CHAT_TYPE.NORMAL;
			}
			else if (selected == 2 && MyInfoManager.Instance.IsClanMember)
			{
				chatMode = ChatText.CHAT_TYPE.CLAN;
			}
			changeChildButton();
		}
		CheckChatKey();
		ApplyFocus();
		string nextControlName = "ChatInput";
		string text = message;
		switch (chatMode)
		{
		case ChatText.CHAT_TYPE.NORMAL:
			GUI.SetNextControlName(nextControlName);
			message = GUI.TextField(crdInputTxtFld, message);
			break;
		case ChatText.CHAT_TYPE.CLAN:
			GUI.SetNextControlName(nextControlName);
			message = GUI.TextField(crdInputTxtFld, message, "ClanChat");
			break;
		case ChatText.CHAT_TYPE.TEAM:
			GUI.SetNextControlName(nextControlName);
			message = GUI.TextField(crdInputTxtFld, message, "TeamChat");
			break;
		case ChatText.CHAT_TYPE.SYSTEM:
			GUI.SetNextControlName(nextControlName);
			message = GUI.TextField(crdInputTxtFld, message, "GmChat");
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
		float num5 = viewSizeCopy.y - crdChatRead.height;
		if (num5 < 0f)
		{
			num5 = 0f;
		}
		bool flag = num5 <= scrollPosition.y;
		CalcViewSize(selected);
		if (flag)
		{
			scrollPosition.y = viewSizeCopy.y - crdChatRead.height;
			if (scrollPosition.y < 0f)
			{
				scrollPosition.y = 0f;
			}
		}
		scrollPosition = GUI.BeginScrollView(crdChatRead, scrollPosition, new Rect(0f, 0f, viewSizeCopy.x, viewSizeCopy.y));
		float y = scrollPosition.y;
		float num6 = scrollPosition.y + crdChatRead.height;
		Vector2 pos = new Vector2(0f, 0f);
		foreach (ChatText item in chatQ)
		{
			if (item.Filtered(selected))
			{
				Vector2 vector = LabelUtil.CalcSize("MissionLabel", item.FullMessage, viewSizeCopy.x);
				float y2 = pos.y;
				float num7 = pos.y + vector.y;
				if (num7 >= y && y2 <= num6)
				{
					LabelUtil.TextOut(pos, item.FullMessage, "MissionLabel", item.TextColor, item.OutlineColor, TextAnchor.UpperLeft, viewSizeCopy.x);
					if (GlobalVars.Instance.MyButton(new Rect(pos.x, pos.y, vector.x, vector.y), string.Empty, "InvisibleButton") && Event.current.button == 1 && item.Seq >= 0 && item.Seq != MyInfoManager.Instance.Seq)
					{
						UserMenu userMenu = ContextMenuManager.Instance.Popup();
						if (userMenu != null)
						{
							bool flag2 = MyInfoManager.Instance.IsClanee(item.Seq);
							userMenu.InitDialog(MouseUtil.ScreenToPixelPoint(Input.mousePosition), item.Seq, item.Speaker, !flag2, masterAssign: false);
						}
					}
				}
				pos.y += vector.y;
			}
		}
		GUI.EndScrollView();
		if (!DialogManager.Instance.IsModal && !bBtnActive)
		{
			GUI.enabled = true;
		}
	}

	private void CheckChatKey()
	{
		if (Event.current.type == EventType.KeyDown && !DialogManager.Instance.IsModal)
		{
			if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
			{
				message.Trim();
				message = RemoveSystemKey(message);
				if (message.Length > 0)
				{
					CommandInterpreter.Instance.IsWhisper = false;
					if (ChatLogManager.Instance.Log(message) && !CommandInterpreter.Instance.Parse(message))
					{
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
			else if (MyInfoManager.Instance.IsGM && Input.GetKeyDown(KeyCode.F4))
			{
				chatMode = ChatText.CHAT_TYPE.SYSTEM;
			}
			else if (Event.current.keyCode == KeyCode.UpArrow)
			{
				string command = message;
				command = CommandInterpreter.Instance.GetNextCommand(command);
				if (command.Length > 0)
				{
					message = command;
				}
			}
			else if (Event.current.keyCode == KeyCode.DownArrow)
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
		if (custom_inputs.Instance.GetButtonDown("K_NORMAL_CHAT"))
		{
			chatMode = ChatText.CHAT_TYPE.NORMAL;
			changeChildButton();
		}
		else if (custom_inputs.Instance.GetButtonDown("K_CLAN_CHAT"))
		{
			if (MyInfoManager.Instance.IsClanMember)
			{
				chatMode = ChatText.CHAT_TYPE.CLAN;
				changeChildButton();
			}
		}
		else if (custom_inputs.Instance.GetButtonDown("K_TEAM_CHAT"))
		{
			chatMode = ChatText.CHAT_TYPE.TEAM;
			changeChildButton();
		}
	}

	private void CheckEvent()
	{
		for (DurabilityEvent durabilityEvent = MyInfoManager.Instance.DeDurabilityEvent(); durabilityEvent != null; durabilityEvent = MyInfoManager.Instance.DeDurabilityEvent())
		{
			string text = durabilityEvent.ToString();
			if (text.Length > 0)
			{
				Enqueue(new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, string.Empty, text));
			}
		}
		string text2 = MyInfoManager.Instance.DeBattleStartRemain();
		if (text2 != null && text2.Length > 0)
		{
			Enqueue(new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, string.Empty, text2));
		}
	}

	public void Update()
	{
		CheckChatKey2();
		CheckEvent();
	}

	private string RemoveSystemKey(string msg)
	{
		msg = msg.Replace("\n", string.Empty);
		msg = msg.Replace("\t", string.Empty);
		msg = msg.Replace("[", " ");
		msg = msg.Replace("]", " ");
		return msg;
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
}
