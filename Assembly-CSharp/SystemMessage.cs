using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SystemMessage
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

	public float statusCenterMessageLimit = 10f;

	private Vector2 strLen = Vector2.zero;

	private float crdLeft;

	public string SysMsg => SysMessageCenter;

	public void SetCoord(float width)
	{
		rcSysMessage = new Rect(crdLT.x + width, crdLT.y, crdRB.x - (crdLT.x + width), crdRB.y - crdLT.y);
		rcSysMessageCenter = new Rect(100f, 100f, (float)(Screen.width - 200), (float)(Screen.height - 200));
	}

	public void Start()
	{
		statusMessageQ = new Queue<string>();
		statusMessageCenterQ = new Queue<string>();
	}

	public void OnDisable()
	{
		statusMessageQ.Clear();
		statusMessageCenterQ.Clear();
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
		statusMessageCenterQ.Enqueue(msg);
	}

	public void OnGUI()
	{
		GUI.BeginGroup(rcSysMessage);
		if (SysMessage.Length > 0)
		{
			if (bCalcLength)
			{
				strLen = LabelUtil.CalcLength("SysMsgLabel", SysMessage);
				bCalcLength = false;
			}
			crdLeft = 330f * (statusDelta * 0.2f);
			Rect position = new Rect(rcSysMessage.width - crdLeft, 0f, 1000f, 20f);
			GUI.Label(position, SysMessage, "SysMsgLabel");
		}
		GUI.EndGroup();
		if (SysMessageCenter.Length > 0)
		{
			rcSysMessageCenter = new Rect(100f, 100f, (float)(Screen.width - 200), (float)(Screen.height - 200));
			Rect position2 = new Rect(rcSysMessageCenter);
			position2.x += 2f;
			position2.y -= 198f;
			Rect position3 = new Rect(rcSysMessageCenter);
			position3.y -= 200f;
			float a = 1f;
			float num = (statusDelta - (statusCenterMessageLimit - 2f)) / 2f;
			if (num > 0f)
			{
				a = Mathf.Lerp(1f, 0f, num);
			}
			Color color = GUI.color;
			GUIStyle style = GUI.skin.GetStyle("BigLabel");
			TextAnchor alignment = style.alignment;
			GUI.color = new Color(0f, 0f, 0f, a);
			style.alignment = TextAnchor.MiddleCenter;
			GUI.Label(position2, SysMessageCenter, "BigLabel");
			GUI.color = new Color(0.91f, 0.6f, 0f, a);
			GUI.Label(position3, SysMessageCenter, "BigLabel");
			style.alignment = alignment;
			GUI.color = color;
		}
	}

	public void Update()
	{
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
			}
		}
	}
}
