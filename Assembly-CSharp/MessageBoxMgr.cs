using System.Collections.Generic;
using UnityEngine;

public class MessageBoxMgr : MonoBehaviour
{
	private List<MsgBox> msgBoxes;

	private static MessageBoxMgr _instance;

	public bool openForcePointChargeDlg;

	public static MessageBoxMgr Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(MessageBoxMgr)) as MessageBoxMgr);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the MessageBoxMgr Instance");
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		msgBoxes = new List<MsgBox>();
		Object.DontDestroyOnLoad(this);
	}

	private void OnApplicationQuit()
	{
		_instance = null;
	}

	private void Update()
	{
		if (openForcePointChargeDlg)
		{
			string text = "s80";
			Good good = ShopManager.Instance.Get(text);
			if (good == null)
			{
				Debug.LogError("not found code: " + text);
			}
			else
			{
				((BuyTermDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.BUY_TERM, exclusive: true))?.InitDialog(good);
			}
			openForcePointChargeDlg = false;
		}
	}

	public MsgBox GetNextItem(MsgBox item)
	{
		int num = msgBoxes.IndexOf(item);
		if (num >= msgBoxes.Count - 1)
		{
			return null;
		}
		return msgBoxes[num + 1];
	}

	public bool HasNextItem(MsgBox item)
	{
		int num = msgBoxes.IndexOf(item);
		return num < msgBoxes.Count - 1;
	}

	public MsgBox GetPrevItem(MsgBox item)
	{
		int num = msgBoxes.IndexOf(item);
		if (num <= 0)
		{
			return null;
		}
		return msgBoxes[num - 1];
	}

	public bool HasPrevItem(MsgBox item)
	{
		int num = msgBoxes.IndexOf(item);
		return num > 0;
	}

	public bool DelMessage(MsgBox mb)
	{
		return msgBoxes.Remove(mb);
	}

	public bool HasMsg()
	{
		return (msgBoxes.Count > 0) ? true : false;
	}

	public void Clear()
	{
		msgBoxes.Clear();
	}

	public void AddMessage(string _msg)
	{
		MsgBox msgBox = new MsgBox(MsgBox.TYPE.WARNING, _msg);
		msgBoxes.Add(msgBox);
		((MsgBoxDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.MSG_BOX, exclusive: false))?.InitDialog(msgBox);
	}

	public void AddSelectMessage(string _msg)
	{
		MsgBox msgBox = new MsgBox(MsgBox.TYPE.SELECT, _msg);
		msgBoxes.Add(msgBox);
		((MsgBoxDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.MSG_BOX, exclusive: false))?.InitDialog(msgBox);
	}

	public void AddForcePointChargeMessage(string _msg)
	{
		MsgBox msgBox = new MsgBox(MsgBox.TYPE.FORCE_POINT_CHARGE, _msg);
		msgBoxes.Add(msgBox);
		((MsgBoxDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.MSG_BOX, exclusive: false))?.InitDialog(msgBox);
	}

	public void AddQuitMesssge(string _msg)
	{
		MsgBox msgBox = new MsgBox(MsgBox.TYPE.QUIT, _msg);
		DialogManager.Instance.CloseAll();
		msgBoxes.Clear();
		msgBoxes.Add(msgBox);
		((MsgBoxDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.MSG_BOX, exclusive: false))?.InitDialog(msgBox);
	}
}
