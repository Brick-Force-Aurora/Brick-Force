using UnityEngine;

public class ChatSelectBtns : MonoBehaviour
{
	public Rect crdCombo = new Rect(249f, 736f, 100f, 26f);

	private string[] chatModes;

	private ComboBox cbox;

	private bool IsBattle;

	private int selected;

	private bool chatView;

	private ChatText.CHAT_TYPE chatMode;

	public bool Chatview
	{
		set
		{
			Chatview = value;
		}
	}

	private void Start()
	{
		cbox = new ComboBox();
		cbox.Initialize(bImage: false, new Vector2(100f, 26f), showUpDn: false, _scrollDown: false);
		cbox.setStyleNames("BoxChatBase", "BtnArrowDn", "BtnArrowUp", "BoxFilterCombo");
		cbox.setBackground(Color.white, GlobalVars.Instance.txtMainColor);
		cbox.setBattleUI(IsBattle);
	}

	public void chatModeLobby()
	{
		IsBattle = false;
		int num = custom_inputs.Instance.KeyIndex("K_NORMAL_CHAT");
		int num2 = custom_inputs.Instance.KeyIndex("K_CLAN_CHAT");
		int num3 = custom_inputs.Instance.KeyIndex("K_TEAM_CHAT");
		string str = custom_inputs.Instance.InputKey[num].ToString();
		string str2 = custom_inputs.Instance.InputKey[num2].ToString();
		string str3 = custom_inputs.Instance.InputKey[num3].ToString();
		chatModes = new string[3];
		chatModes[0] = StringMgr.Instance.Get("CHAT_GENERAL") + "(" + str + ")";
		chatModes[1] = StringMgr.Instance.Get("CHAT_CLAN") + "(" + str2 + ")";
		chatModes[2] = StringMgr.Instance.Get("CHAT_TEAM") + "(" + str3 + ")";
	}

	public void chatModeBattle()
	{
		IsBattle = true;
		int num = custom_inputs.Instance.KeyIndex("K_NORMAL_CHAT");
		int num2 = custom_inputs.Instance.KeyIndex("K_CLAN_CHAT");
		int num3 = custom_inputs.Instance.KeyIndex("K_TEAM_CHAT");
		string str = custom_inputs.Instance.InputKey[num].ToString();
		string str2 = custom_inputs.Instance.InputKey[num2].ToString();
		string str3 = custom_inputs.Instance.InputKey[num3].ToString();
		chatModes = new string[3];
		chatModes[0] = StringMgr.Instance.Get("CHAT_GENERAL") + "(" + str + ")";
		chatModes[1] = StringMgr.Instance.Get("CHAT_CLAN") + "(" + str2 + ")";
		chatModes[2] = StringMgr.Instance.Get("CHAT_TEAM") + "(" + str3 + ")";
	}

	public void OnGUI()
	{
		if (chatView)
		{
			if (!BrickManager.Instance.IsLoaded)
			{
				GlobalVars.Instance.BeginGUI(null);
			}
			GUISkin skin = GUI.skin;
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = 25;
			GUI.enabled = !DialogManager.Instance.IsModal;
			string buttonText = string.Empty;
			GUIContent[] array = new GUIContent[chatModes.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new GUIContent(chatModes[i]);
				if (selected == i)
				{
					buttonText = chatModes[i];
				}
			}
			int num = selected;
			selected = cbox.List(crdCombo, buttonText, array);
			if (num != selected)
			{
				if (selected == 0)
				{
					chatMode = ChatText.CHAT_TYPE.NORMAL;
				}
				else if (selected == 1)
				{
					chatMode = ChatText.CHAT_TYPE.CLAN;
				}
				else if (selected == 2)
				{
					chatMode = ChatText.CHAT_TYPE.TEAM;
				}
				changeParentChatMode();
			}
			GUI.skin = skin;
			GUI.enabled = true;
			if (!BrickManager.Instance.IsLoaded)
			{
				GlobalVars.Instance.EndGUI();
			}
		}
	}

	public void changeChildIdx(ChatText.CHAT_TYPE chatType)
	{
		switch (chatType)
		{
		case ChatText.CHAT_TYPE.NORMAL:
			cbox.SetSelectedItemIndex(0);
			break;
		case ChatText.CHAT_TYPE.CLAN:
			cbox.SetSelectedItemIndex(1);
			break;
		case ChatText.CHAT_TYPE.TEAM:
			cbox.SetSelectedItemIndex(2);
			break;
		case ChatText.CHAT_TYPE.WHISPER:
			cbox.SetSelectedItemIndex(3);
			break;
		}
	}

	public void rcBox(float x)
	{
		crdCombo.x = x - 100f;
	}

	public void rcBox(float x, float y)
	{
		crdCombo.x = x - 100f;
		crdCombo.y = y;
	}

	private void VerifyChatView()
	{
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			Lobby component = gameObject.GetComponent<Lobby>();
			if (null != component)
			{
				chatView = component.bChatView;
			}
			Briefing4TeamMatch component2 = gameObject.GetComponent<Briefing4TeamMatch>();
			if (null != component2)
			{
				chatView = component2.bChatView;
			}
			BattleChat component3 = gameObject.GetComponent<BattleChat>();
			if (null != component3)
			{
				chatView = component3.IsChatting;
			}
			SquadingMain component4 = gameObject.GetComponent<SquadingMain>();
			if (null != component4)
			{
				chatView = true;
			}
			SquadMain component5 = gameObject.GetComponent<SquadMain>();
			if (null != component5)
			{
				chatView = true;
			}
		}
	}

	private void changeParentChatMode()
	{
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			Lobby component = gameObject.GetComponent<Lobby>();
			if (null != component)
			{
				component.lobbyChat.ChatMode = chatMode;
			}
			Briefing4TeamMatch component2 = gameObject.GetComponent<Briefing4TeamMatch>();
			if (null != component2)
			{
				component2.lobbyChat.ChatMode = chatMode;
			}
			BattleChat component3 = gameObject.GetComponent<BattleChat>();
			if (null != component3)
			{
				component3.ChatMode = chatMode;
			}
			SquadingMain component4 = gameObject.GetComponent<SquadingMain>();
			if (null != component4)
			{
				component4.lobbyChat.ChatMode = chatMode;
			}
			SquadMain component5 = gameObject.GetComponent<SquadMain>();
			if (null != component5)
			{
				component5.lobbyChat.ChatMode = chatMode;
			}
		}
	}

	private void Update()
	{
		VerifyChatView();
	}
}
