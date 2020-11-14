using System.Collections.Generic;
using UnityEngine;

public class ChatLogManager : MonoBehaviour
{
	public int maxCopynpaste = 5;

	public float penaltyTime = 10f;

	public float coolTime = 3f;

	private Queue<string> latestChat;

	private bool penalty;

	private float deltaTime;

	private float loggingTime;

	private static ChatLogManager _instance;

	public static ChatLogManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(ChatLogManager)) as ChatLogManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the ChatLogManager Instance");
				}
			}
			return _instance;
		}
	}

	private void Start()
	{
		latestChat = new Queue<string>();
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	public bool Log(string text)
	{
		if (penalty)
		{
			return false;
		}
		int num = 0;
		foreach (string item in latestChat)
		{
			if (item == text && ++num >= maxCopynpaste)
			{
				deltaTime = 0f;
				penalty = true;
				latestChat.Clear();
				SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("COPYNPASTE_WARNING"));
				return false;
			}
		}
		loggingTime = 0f;
		latestChat.Enqueue(text);
		while (latestChat.Count > maxCopynpaste)
		{
			latestChat.Dequeue();
		}
		return true;
	}

	private void Update()
	{
		if (penalty)
		{
			deltaTime += Time.deltaTime;
			if (deltaTime > penaltyTime)
			{
				penalty = false;
			}
		}
		loggingTime += Time.deltaTime;
		if (loggingTime > coolTime)
		{
			latestChat.Clear();
		}
	}
}
