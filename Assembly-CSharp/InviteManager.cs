using System.Collections.Generic;
using UnityEngine;

public class InviteManager : MonoBehaviour
{
	public List<Invite> listInvite = new List<Invite>();

	private static InviteManager _instance;

	private Invite savedForClan;

	public static InviteManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(InviteManager)) as InviteManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the InviteManager Instance");
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	public void AddInvite(Invite invite)
	{
		Remove(invite.invitorSeq);
		listInvite.Add(invite);
		InviteNoticeDialog inviteNoticeDialog = (InviteNoticeDialog)DialogManager.Instance.GetDialogAlways(DialogManager.DIALOG_INDEX.INVITE_NOTICE);
		if (inviteNoticeDialog != null)
		{
			((InviteNoticeDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.INVITE_NOTICE, exclusive: false))?.InitDialog();
		}
	}

	public void Remove(int key)
	{
		int num = 0;
		while (true)
		{
			if (num >= listInvite.Count)
			{
				return;
			}
			if (listInvite[num].invitorSeq == key)
			{
				break;
			}
			num++;
		}
		listInvite.RemoveAt(num);
	}

	public void RemoveAll()
	{
		if (listInvite.Count > 0)
		{
			savedForClan = listInvite[0];
		}
		listInvite.Clear();
	}

	public Invite GetData()
	{
		return savedForClan;
	}
}
