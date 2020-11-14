using System;
using UnityEngine;

[Serializable]
public class BriefingPanel4Individual
{
	private Rect crdStartBtn = new Rect(820f, 518f, 192f, 86f);

	private float inviteAfter = BriefingPanel4TeamMatch.RANDOM_INVITE_RESEND_TIME;

	public void Start()
	{
	}

	public void OnGUI()
	{
		if (MyInfoManager.Instance.Slot >= 0)
		{
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			bool flag = false;
			if (RoomManager.Instance.Master == MyInfoManager.Instance.Seq)
			{
				flag = true;
			}
			Room room = RoomManager.Instance.GetRoom(RoomManager.Instance.CurrentRoom);
			if (room != null && room.Status == Room.ROOM_STATUS.PLAYING)
			{
				GUIContent content = new GUIContent(StringMgr.Instance.Get("START").ToUpper(), GlobalVars.Instance.iconStart);
				if (GlobalVars.Instance.MyButton3(crdStartBtn, content, "BtnAction"))
				{
					if (!room.isBreakInto)
					{
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CANT_BREAK_INTO"));
					}
					else if (!P2PManager.Instance.RendezvousPointed)
					{
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("RENDEZVOUS_NOT_COMPLETED"));
					}
					else if (MyInfoManager.Instance.HaveWeaponLimitedByStarRate())
					{
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("WEAPON_STAR_LIMIT"));
					}
					else if (!Application.isLoadingLevel)
					{
						MyInfoManager.Instance.BreakingInto = true;
						CSNetManager.Instance.Sock.SendCS_BREAK_INTO_REQ();
					}
				}
			}
			else if (flag)
			{
				bool battleStarting = GlobalVars.Instance.battleStarting;
				string key = (!battleStarting) ? "START" : "CANCEL";
				GUIContent content2 = new GUIContent(StringMgr.Instance.Get(key).ToUpper(), GlobalVars.Instance.iconStart);
				if (GlobalVars.Instance.MyButton3(crdStartBtn, content2, "BtnAction"))
				{
					if (!P2PManager.Instance.RendezvousPointed)
					{
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("RENDEZVOUS_NOT_COMPLETED"));
					}
					else if (MyInfoManager.Instance.HaveWeaponLimitedByStarRate())
					{
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("WEAPON_STAR_LIMIT"));
					}
					else if (!Application.isLoadingLevel)
					{
						if (!battleStarting)
						{
							GlobalVars.Instance.RemainTime = 5f;
							CSNetManager.Instance.Sock.SendCS_START_REQ((int)GlobalVars.Instance.RemainTime);
						}
						else
						{
							GlobalVars.Instance.battleStarting = false;
							CSNetManager.Instance.Sock.SendCS_START_REQ(-1);
						}
					}
				}
				content2 = new GUIContent(StringMgr.Instance.Get("RANDOM_INVITE"));
				if (GlobalVars.Instance.MyButton3(new Rect(crdStartBtn.x, crdStartBtn.y + crdStartBtn.height + 4f, crdStartBtn.width, crdStartBtn.height), content2, "BtnAction"))
				{
					if (BriefingPanel4TeamMatch.RANDOM_INVITE_RESEND_TIME < inviteAfter)
					{
						CSNetManager.Instance.Sock.SendCS_RANDOM_INVITE_REQ();
						inviteAfter = 0f;
					}
					else
					{
						GameObject gameObject = GameObject.Find("Main");
						if (null != gameObject)
						{
							string text = string.Format(StringMgr.Instance.Get("RANDOM_INVITE_WARNING"), ((int)BriefingPanel4TeamMatch.RANDOM_INVITE_RESEND_TIME).ToString());
							gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, string.Empty, text));
						}
					}
				}
			}
			else if (MyInfoManager.Instance.Status != 1)
			{
				GUIContent content3 = new GUIContent(StringMgr.Instance.Get("READY").ToUpper(), GlobalVars.Instance.iconStart);
				if (GlobalVars.Instance.MyButton3(crdStartBtn, content3, "BtnAction"))
				{
					if (!P2PManager.Instance.RendezvousPointed)
					{
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("RENDEZVOUS_NOT_COMPLETED"));
					}
					else if (MyInfoManager.Instance.HaveWeaponLimitedByStarRate())
					{
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("WEAPON_STAR_LIMIT"));
					}
					else if (!Application.isLoadingLevel)
					{
						CSNetManager.Instance.Sock.SendCS_SET_STATUS_REQ(1);
					}
				}
			}
			else
			{
				GUIContent content4 = new GUIContent(StringMgr.Instance.Get("ROOM_STATUS_WAITING").ToUpper(), GlobalVars.Instance.iconStart);
				if (GlobalVars.Instance.MyButton3(crdStartBtn, content4, "BtnAction") && !Application.isLoadingLevel)
				{
					CSNetManager.Instance.Sock.SendCS_SET_STATUS_REQ(0);
				}
			}
		}
	}

	public void Update()
	{
		inviteAfter += Time.deltaTime;
	}
}
