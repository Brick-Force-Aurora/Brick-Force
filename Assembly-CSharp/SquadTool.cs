using System;
using UnityEngine;

[Serializable]
public class SquadTool
{
	private Rect crdCreate = new Rect(760f, 43f, 210f, 34f);

	private Rect crdBack = new Rect(981f, 7f, 34f, 34f);

	private SquadMemberListFrame squadMemberList;

	public void Start(SquadMemberListFrame squadMemberListFrame)
	{
		squadMemberList = squadMemberListFrame;
	}

	public void Update()
	{
	}

	public void OnGUI()
	{
		Squad curSquad = SquadManager.Instance.CurSquad;
		if (curSquad != null && curSquad.Leader == MyInfoManager.Instance.Seq)
		{
			NameCard selectedMember = squadMemberList.SelectedMember;
			if (selectedMember != null && selectedMember.Nickname != MyInfoManager.Instance.Nickname && GlobalVars.Instance.MyButton(crdCreate, StringMgr.Instance.Get("EXILE"), "BtnAction"))
			{
				CSNetManager.Instance.Sock.SendCS_KICK_SQUAD_REQ(selectedMember.Seq);
			}
		}
		if (GlobalVars.Instance.MyButton(crdBack, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			CSNetManager.Instance.Sock.SendCS_LEAVE_SQUAD_REQ();
			SquadManager.Instance.Leave();
			Application.LoadLevel("Squading");
		}
	}
}
