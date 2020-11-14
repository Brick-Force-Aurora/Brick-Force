using UnityEngine;

public class ChatText
{
	public enum CHAT_TYPE
	{
		NONE = -1,
		NORMAL,
		WHISPER,
		SYSTEM,
		CLAN,
		TEAM,
		TREASURE
	}

	private const float lifeTime = 15f;

	private Color textColor;

	private Color outlineColor;

	private int seq;

	private string speaker;

	private string sChatType;

	private string message;

	private CHAT_TYPE chatType;

	private float lapTime;

	private bool isGm;

	private string getsrvnick = string.Empty;

	public Color TextColor => textColor;

	public Color OutlineColor => outlineColor;

	public int Seq => seq;

	public string Speaker => speaker;

	public string FullMessage
	{
		get
		{
			if (chatType == CHAT_TYPE.WHISPER)
			{
				if (speaker == getsrvnick)
				{
					return "[" + sChatType + "]<-[" + speaker + "] " + message;
				}
				return "[" + sChatType + "]->[" + speaker + "] " + message;
			}
			return "[" + sChatType + "][" + speaker + "] " + message;
		}
	}

	public bool IsAlive => lapTime < 15f;

	public ChatText(CHAT_TYPE ct, int sn, string nick, string text, bool gm = false)
	{
		isGm = gm;
		if (ct == CHAT_TYPE.CLAN && MyInfoManager.Instance.Nickname != nick)
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				Lobby component = gameObject.GetComponent<Lobby>();
				if (component != null)
				{
					component.HaveClanOrWhisper = true;
				}
			}
		}
		if (ct == CHAT_TYPE.WHISPER)
		{
			GameObject gameObject2 = GameObject.Find("Main");
			if (null != gameObject2)
			{
				Lobby component2 = gameObject2.GetComponent<Lobby>();
				if (component2 != null)
				{
					component2.HaveClanOrWhisper = true;
				}
			}
		}
		getsrvnick = nick;
		chatType = ct;
		switch (chatType)
		{
		case CHAT_TYPE.NORMAL:
			textColor = ((!isGm) ? Color.white : new Color(1f, 0.6f, 0.4f));
			outlineColor = Color.black;
			sChatType = StringMgr.Instance.Get("CHAT_GENERAL");
			break;
		case CHAT_TYPE.WHISPER:
			textColor = ((!isGm) ? new Color(0.97f, 0.21f, 0.73f, 1f) : new Color(1f, 0.6f, 0.4f));
			outlineColor = Color.black;
			sChatType = StringMgr.Instance.Get("WHISPER");
			if (nick == MyInfoManager.Instance.Nickname)
			{
				nick = GlobalVars.Instance.whisperNickTo;
			}
			else
			{
				GlobalVars.Instance.whisperNickFrom = nick;
			}
			break;
		case CHAT_TYPE.SYSTEM:
			textColor = new Color(1f, 0.8f, 0f, 1f);
			outlineColor = Color.black;
			sChatType = StringMgr.Instance.Get("CHAT_NOTICE");
			if (nick.Length <= 0)
			{
				nick = StringMgr.Instance.Get("GAME_TITLE");
			}
			break;
		case CHAT_TYPE.CLAN:
			textColor = ((!isGm) ? new Color(0.1f, 0.86f, 0.9f) : new Color(1f, 0.6f, 0.4f));
			outlineColor = Color.black;
			sChatType = StringMgr.Instance.Get("CHAT_CLAN");
			break;
		case CHAT_TYPE.TEAM:
			textColor = ((!isGm) ? new Color(0.44f, 1f, 0f, 1f) : new Color(1f, 0.6f, 0.4f));
			outlineColor = Color.black;
			sChatType = StringMgr.Instance.Get("CHAT_TEAM");
			break;
		case CHAT_TYPE.TREASURE:
			textColor = GlobalVars.Instance.txtMainColor;
			outlineColor = Color.black;
			sChatType = StringMgr.Instance.Get("CHAT_NOTICE");
			nick = StringMgr.Instance.Get("TREASURE_CHEST");
			break;
		}
		seq = sn;
		speaker = nick;
		if (chatType != CHAT_TYPE.SYSTEM)
		{
			message = WordFilter.Instance.Filter(text);
		}
		else
		{
			message = text;
		}
		if (text != message)
		{
			textColor.a = 0.5f;
		}
		lapTime = 0f;
	}

	public void setTextAlpha(float alpha)
	{
		textColor.a = alpha;
	}

	public void setOutTextAlpha(float alpha)
	{
		outlineColor.a = alpha;
	}

	public bool Filtered(int selectedTab)
	{
		switch (selectedTab)
		{
		case 1:
			if (chatType == CHAT_TYPE.NORMAL || chatType == CHAT_TYPE.CLAN || chatType == CHAT_TYPE.TEAM)
			{
				return false;
			}
			break;
		case 2:
			if (chatType == CHAT_TYPE.NORMAL || chatType == CHAT_TYPE.TEAM)
			{
				return false;
			}
			break;
		}
		return true;
	}

	public void Update()
	{
		lapTime += Time.deltaTime;
		if (lapTime >= 14f)
		{
			textColor.a -= Time.deltaTime;
			if (textColor.a < 0f)
			{
				textColor.a = 0f;
			}
			outlineColor.a -= 2f * Time.deltaTime;
			if (outlineColor.a < 0f)
			{
				outlineColor.a = 0f;
			}
		}
	}
}
