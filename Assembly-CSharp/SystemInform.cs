using System.Collections.Generic;
using UnityEngine;

public class SystemInform : MonoBehaviour
{
	private Vector2 crdLT = new Vector2(0f, 20f);

	private Vector2 crdRB = new Vector2(860f, 55f);

	private Rect rcSysMessage;

	private Rect rcSysMessageCenter;

	private Queue<string> statusMessageQ;

	private string SysMessage = string.Empty;

	private bool bReceived;

	private float statusDelta;

	private bool bCalcLength;

	private Queue<string> statusMessageCenterQ;

	private string SysMessageCenter = string.Empty;

	private bool bReceivedCenter;

	public float statusDeltaCenter;

	private float statusCenterMessageLimit = 30f;

	private Vector2 strLen = Vector2.zero;

	private float crdLeft;

	private static SystemInform _instance;

	private float twidth;

	private Lobby lobby;

	public string SysMsg => SysMessageCenter;

	public static SystemInform Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(SystemInform)) as SystemInform);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the SystemInform Instance");
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	public void SetToolbarSize(float _twidth)
	{
		twidth = _twidth;
	}

	public void SetCoord(float width)
	{
		rcSysMessage = new Rect(crdLT.x + width, crdLT.y, crdRB.x - (crdLT.x + width), crdRB.y - crdLT.y);
		rcSysMessageCenter = new Rect(100f, 100f, (float)(Screen.width - 200), (float)(Screen.height - 200));
	}

	public void AddMessage(string msg)
	{
		if (SysMessage.Length == 0)
		{
			bReceived = true;
		}
		statusMessageQ.Enqueue(msg);
	}

	public void AddMessageCenter(string msg)
	{
		if (SysMessageCenter.Length == 0)
		{
			bReceivedCenter = true;
		}
		else
		{
			statusDeltaCenter = statusCenterMessageLimit - 2f;
		}
		statusMessageCenterQ.Enqueue(msg);
	}

	private void Start()
	{
		statusMessageQ = new Queue<string>();
		statusMessageCenterQ = new Queue<string>();
	}

	private void VerifyLobby()
	{
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			lobby = gameObject.GetComponent<Lobby>();
		}
	}

	private void Update()
	{
		VerifyLobby();
		if (bReceived)
		{
			SysMessage = statusMessageQ.Dequeue();
			statusDelta = 0f;
			bCalcLength = true;
			bReceived = false;
		}
		if (bReceivedCenter)
		{
			SysMessageCenter = statusMessageCenterQ.Dequeue();
			statusDeltaCenter = 0f;
			bReceivedCenter = false;
		}
		if (SysMessage.Length > 0 && !bCalcLength)
		{
			statusDelta += Time.deltaTime;
			float num = rcSysMessage.width - crdLeft;
			if (num < 0f && 0f - num > strLen.x)
			{
				statusDelta = 0f;
				if (statusMessageQ.Count > 0)
				{
					SysMessage = statusMessageQ.Dequeue();
					bCalcLength = true;
				}
				else
				{
					SysMessage = string.Empty;
				}
			}
		}
		if (SysMessageCenter.Length > 0)
		{
			statusDeltaCenter += Time.deltaTime;
			if (statusDeltaCenter > statusCenterMessageLimit)
			{
				statusDeltaCenter = 0f;
				SysMessageCenter = string.Empty;
				if (statusMessageCenterQ.Count > 0)
				{
					SysMessageCenter = statusMessageCenterQ.Dequeue();
				}
				else
				{
					SysMessageCenter = string.Empty;
				}
			}
		}
	}

	private void OnGUI()
	{
		GlobalVars.Instance.BeginGUI(null);
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		GUI.enabled = true;
		GUI.depth = 35;
		GUI.depth = 0;
		if (SysMessageCenter.Length > 0)
		{
			rcSysMessageCenter = new Rect(100f, 80f, 824f, 568f);
			Rect position = new Rect(rcSysMessageCenter);
			position.x += 2f;
			position.y -= 198f;
			Rect position2 = new Rect(rcSysMessageCenter);
			position2.y -= 200f;
			float a = 1f;
			float num = (statusDeltaCenter - (statusCenterMessageLimit - 2f)) / 2f;
			if (num > 0f)
			{
				a = Mathf.Lerp(1f, 0f, num);
			}
			Color color = GUI.color;
			GUIStyle style = GUI.skin.GetStyle("BigLabel");
			TextAnchor alignment = style.alignment;
			GUI.color = new Color(0f, 0f, 0f, a);
			style.alignment = TextAnchor.MiddleCenter;
			GUI.Label(position, SysMessageCenter, "BigLabel");
			GUI.color = new Color(0.91f, 0.6f, 0f, a);
			GUI.Label(position2, SysMessageCenter, "BigLabel");
			style.alignment = alignment;
			GUI.color = color;
		}
		GUI.enabled = true;
		GlobalVars.Instance.EndGUI();
	}

	public void DoScrollMessage()
	{
		rcSysMessage = new Rect(crdLT.x + twidth, crdLT.y, crdRB.x - (crdLT.x + twidth), crdRB.y - crdLT.y);
		GUI.BeginGroup(rcSysMessage);
		if (SysMessage.Length > 0 && lobby != null && lobby.CurLobbyType == LOBBY_TYPE.BASE)
		{
			if (bCalcLength)
			{
				strLen = LabelUtil.CalcLength("SysMsgLabel", SysMessage);
				bCalcLength = false;
			}
			crdLeft = 330f * (statusDelta * 0.2f);
			LabelUtil.TextOut(new Vector2(rcSysMessage.width - crdLeft, rcSysMessage.height / 2f), SysMessage, "SysMsgLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		}
		GUI.EndGroup();
	}
}
