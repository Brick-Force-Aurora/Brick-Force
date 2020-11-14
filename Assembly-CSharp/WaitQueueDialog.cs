using System;
using UnityEngine;

[Serializable]
public class WaitQueueDialog : Dialog
{
	public Vector2 sizeOk = new Vector2(85f, 26f);

	public float msgY = 50f;

	private int waiting;

	public int Waiting
	{
		get
		{
			return waiting;
		}
		set
		{
			waiting = value;
		}
	}

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.WAIT_QUEUE;
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUI.depth = 0;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		string text = StringMgr.Instance.Get("WAITING_QUEUING") + " " + waiting.ToString();
		LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), msgY), text, "BigLabel", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		if (GlobalVars.Instance.MyButton(new Rect(((float)Screen.width - sizeOk.x) / 2f, size.y - sizeOk.y - 25f, sizeOk.x, sizeOk.y), StringMgr.Instance.Get("CANCEL"), "BtnAction"))
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				gameObject.BroadcastMessage("OnLoginFail");
			}
			CSNetManager.Instance.Clear();
			result = true;
		}
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		return result;
	}

	public override void OnPopup()
	{
		size.x = GlobalVars.Instance.ScreenRect.width;
		rc = new Rect(0f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog()
	{
	}
}
