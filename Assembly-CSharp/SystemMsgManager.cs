using System.Collections.Generic;
using UnityEngine;

public class SystemMsgManager : MonoBehaviour
{
	private Queue<SystemMsg> qMessages;

	public GUIDepth.LAYER guiDepth;

	private static SystemMsgManager instance;

	public static SystemMsgManager Instance
	{
		get
		{
			if (null == instance)
			{
				instance = (Object.FindObjectOfType(typeof(SystemMsgManager)) as SystemMsgManager);
				if (null == instance)
				{
					Debug.LogError("ERROR, Fail to get SystemMsgManager Instance");
				}
			}
			return instance;
		}
	}

	private void Awake()
	{
		qMessages = new Queue<SystemMsg>();
		Object.DontDestroyOnLoad(this);
	}

	private void OnApplicationQuit()
	{
		instance = null;
	}

	public void OnGUI()
	{
		GUISkin gUISkin = GUISkinFinder.Instance.GetGUISkin();
		if (null != gUISkin)
		{
			GUI.skin = gUISkin;
			GUI.depth = (int)guiDepth;
			SystemMsg[] array = qMessages.ToArray();
			if (array.Length > 0)
			{
				float height = 0f;
				if (array[array.Length - 1].CalcRC(ref height))
				{
					foreach (SystemMsg qMessage in qMessages)
					{
						qMessage.Adjust(height);
					}
				}
			}
			foreach (SystemMsg qMessage2 in qMessages)
			{
				qMessage2.Show();
			}
		}
	}

	public void ShowMessage(string msg)
	{
		if (qMessages.Count > 0)
		{
			SystemMsg systemMsg = qMessages.Peek();
			if (systemMsg.Message == msg)
			{
				systemMsg.Reset();
				return;
			}
		}
		qMessages.Enqueue(new SystemMsg(msg, 5f));
	}

	public void ShowMessage(string msg, float time)
	{
		if (qMessages.Count > 0)
		{
			SystemMsg systemMsg = qMessages.Peek();
			if (systemMsg.Message == msg)
			{
				systemMsg.Reset();
				systemMsg.Laptime = time;
				return;
			}
		}
		qMessages.Enqueue(new SystemMsg(msg, time));
	}

	private void Update()
	{
		foreach (SystemMsg qMessage in qMessages)
		{
			qMessage.Update();
		}
		while (qMessages.Count > 0 && !qMessages.Peek().Valid)
		{
			qMessages.Dequeue();
		}
	}
}
