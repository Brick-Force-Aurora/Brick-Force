using System;
using UnityEngine;

[Serializable]
public class SquadingTool
{
	private Rect crdJoin = new Rect(708f, 43f, 150f, 34f);

	private Rect crdCreate = new Rect(863f, 43f, 150f, 34f);

	private Rect crdBack = new Rect(981f, 7f, 34f, 34f);

	private SquadListFrame squadList;

	public void Start(SquadListFrame squadListFrame)
	{
		squadList = squadListFrame;
	}

	public void Update()
	{
	}

	public void OnGUI()
	{
		if (GlobalVars.Instance.MyButton(crdJoin, StringMgr.Instance.Get("JOIN_SQUAD"), "BtnAction"))
		{
			if (squadList.SelectedSquad == null)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("SELECT_SQUAD_TO_JOIN"));
			}
			else
			{
				CSNetManager.Instance.Sock.SendCS_JOIN_SQUAD_REQ(MyInfoManager.Instance.ClanSeq, squadList.SelectedSquad.Index, -1);
			}
		}
		if (GlobalVars.Instance.MyButton(crdCreate, StringMgr.Instance.Get("CREATE_SQUAD"), "BtnAction"))
		{
			((CreateMatchTeamDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.CREATE_MATCH_TEAM, exclusive: true))?.InitDialog();
		}
		if (GlobalVars.Instance.MyButton(crdBack, string.Empty, "BtnClose") || (!GlobalVars.Instance.IsModalAll() && GlobalVars.Instance.IsEscapePressed()))
		{
			CSNetManager.Instance.Sock.SendCS_LEAVE_SQUADING_REQ();
			SquadManager.Instance.Clear();
			Application.LoadLevel("Lobby");
		}
	}
}
