using UnityEngine;

public class GameCautionManager : MonoBehaviour
{
	private static GameCautionManager _instance;

	public float maxTime = 3600f;

	private float delta;

	private float nextdelta;

	private bool isnext;

	private bool outmsg1;

	private bool outmsg2;

	private int hour;

	public static GameCautionManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(GameCautionManager)) as GameCautionManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the GameCautionManager Instance");
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (BuildOption.Instance.IsNetmarble || BuildOption.Instance.IsDeveloper)
		{
			delta += Time.deltaTime;
			if (delta >= maxTime)
			{
				delta = 0f;
				isnext = true;
				outmsg1 = true;
				hour++;
			}
			if (isnext)
			{
				nextdelta += Time.deltaTime;
				if (nextdelta >= 5f)
				{
					nextdelta = 0f;
					isnext = false;
					outmsg2 = true;
				}
			}
			if (outmsg1)
			{
				GameObject gameObject = GameObject.Find("Main");
				if (null != gameObject)
				{
					Lobby component = gameObject.GetComponent<Lobby>();
					if (null != component && component.bChatView)
					{
						gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, string.Empty, string.Format(StringMgr.Instance.Get("PLAY_TIME_WARNING_01"), hour)));
						outmsg1 = false;
					}
					BattleChat component2 = gameObject.GetComponent<BattleChat>();
					if (null != component2 && component2.IsChatting)
					{
						gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, string.Empty, string.Format(StringMgr.Instance.Get("PLAY_TIME_WARNING_01"), hour)));
						outmsg1 = false;
					}
				}
			}
			if (outmsg2)
			{
				GameObject gameObject2 = GameObject.Find("Main");
				if (null != gameObject2)
				{
					Lobby component3 = gameObject2.GetComponent<Lobby>();
					if (null != component3 && component3.bChatView)
					{
						gameObject2.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, string.Empty, StringMgr.Instance.Get("PLAY_TIME_WARNING_02")));
						outmsg2 = false;
					}
					BattleChat component4 = gameObject2.GetComponent<BattleChat>();
					if (null != component4 && component4.IsChatting)
					{
						gameObject2.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, string.Empty, StringMgr.Instance.Get("PLAY_TIME_WARNING_02")));
						outmsg2 = false;
					}
				}
			}
		}
	}
}
