using System;
using UnityEngine;

[Serializable]
public class BriefingPanel4TeamMatch
{
	private Rect crdStartBtn = new Rect(820f, 518f, 192f, 86f);

	private Rect crdChangeLeftBtn = new Rect(618f, 258f, 30f, 22f);

	public static float RANDOM_INVITE_RESEND_TIME = 3f;

	private float inviteAfter = RANDOM_INVITE_RESEND_TIME;

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
				if (ChannelManager.Instance.CurChannel.Mode != 4 && GlobalVars.Instance.MyButton(crdChangeLeftBtn, string.Empty, "BtnTeamChange") && !Application.isLoadingLevel)
				{
					CSNetManager.Instance.Sock.SendCS_TEAM_CHANGE_REQ(clickSlot: false, -1);
				}
			}
			else if (flag)
			{
				bool battleStarting = GlobalVars.Instance.battleStarting;
				if (ChannelManager.Instance.CurChannel.Mode != 4)
				{
					if (GlobalVars.Instance.MyButton(crdChangeLeftBtn, string.Empty, "BtnTeamChange") && !Application.isLoadingLevel)
					{
						CSNetManager.Instance.Sock.SendCS_TEAM_CHANGE_REQ(clickSlot: false, -1);
					}
					GUIContent content = new GUIContent(StringMgr.Instance.Get("RANDOM_INVITE"));
					if (GlobalVars.Instance.MyButton3(new Rect(crdStartBtn.x, crdStartBtn.y + crdStartBtn.height + 4f, crdStartBtn.width, crdStartBtn.height), content, "BtnAction"))
					{
						if (RANDOM_INVITE_RESEND_TIME < inviteAfter)
						{
							CSNetManager.Instance.Sock.SendCS_RANDOM_INVITE_REQ();
							inviteAfter = 0f;
						}
						else
						{
							GameObject gameObject = GameObject.Find("Main");
							if (null != gameObject)
							{
								string text = string.Format(StringMgr.Instance.Get("RANDOM_INVITE_WARNING"), ((int)RANDOM_INVITE_RESEND_TIME).ToString());
								gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, string.Empty, text));
							}
						}
					}
				}
				if (ChannelManager.Instance.CurChannel.Mode != 4 || GlobalVars.Instance.clanTeamMatchSuccess == 1)
				{
					string key = (!battleStarting) ? "START" : "CANCEL";
					GUIContent content = new GUIContent(StringMgr.Instance.Get(key).ToUpper(), GlobalVars.Instance.iconStart);
					if (GlobalVars.Instance.MyButton3(crdStartBtn, content, "BtnAction"))
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
				}
				else if (ChannelManager.Instance.CurChannel.Mode == 4)
				{
					if (GlobalVars.Instance.clanTeamMatchSuccess == -1)
					{
						string key2 = (!battleStarting) ? "BTN_MATCH" : "CANCEL";
						GUIContent content = new GUIContent(StringMgr.Instance.Get(key2).ToUpper(), GlobalVars.Instance.iconStart);
						if (GlobalVars.Instance.MyButton3(crdStartBtn, content, "BtnAction"))
						{
							Squad curSquad = SquadManager.Instance.CurSquad;
							int maxMember = curSquad.MaxMember;
							if (maxMember > curSquad.MemberCount)
							{
								MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CLAN_MATCH_MORE_PEOPLE_NEED"));
							}
							else
							{
								RegMap regMap = RegMapManager.Instance.Get(room.map);
								if (regMap == null)
								{
									MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CANT_PLAY_WITHOUT_MAP"));
								}
								else
								{
									GlobalVars.Instance.clanMatchMaxPlayer = room.MaxPlayer / 2;
									if (room.Type == Room.ROOM_TYPE.EXPLOSION)
									{
										CSNetManager.Instance.Sock.SendCS_MATCH_TEAM_START_REQ((int)room.Type, GlobalVars.Instance.clanMatchMaxPlayer, room.map, 5, 180, 0, regMap.Alias, wanted: false);
									}
									else
									{
										CSNetManager.Instance.Sock.SendCS_MATCH_TEAM_START_REQ((int)room.Type, GlobalVars.Instance.clanMatchMaxPlayer, room.map, 50, 480, 0, regMap.Alias, (room.Type == Room.ROOM_TYPE.TEAM_MATCH) ? true : false);
									}
								}
							}
						}
					}
					else
					{
						string key3 = (!battleStarting) ? "BTN_MATCH" : "CANCEL";
						GUIContent content = new GUIContent(StringMgr.Instance.Get(key3).ToUpper(), GlobalVars.Instance.iconStart);
						GlobalVars.Instance.MyButton3(crdStartBtn, content, "BtnAction");
					}
				}
			}
			else
			{
				if (ChannelManager.Instance.CurChannel.Mode != 4 && GlobalVars.Instance.MyButton(crdChangeLeftBtn, string.Empty, "BtnTeamChange"))
				{
					if (MyInfoManager.Instance.Seq != RoomManager.Instance.Master && MyInfoManager.Instance.Status == 1)
					{
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CANT_ON_READY"));
					}
					else if (!Application.isLoadingLevel)
					{
						CSNetManager.Instance.Sock.SendCS_TEAM_CHANGE_REQ(clickSlot: false, -1);
					}
				}
				if (MyInfoManager.Instance.Status != 1)
				{
					GUIContent content = new GUIContent(StringMgr.Instance.Get("READY").ToUpper(), GlobalVars.Instance.iconStart);
					if (GlobalVars.Instance.MyButton3(crdStartBtn, content, "BtnAction"))
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
					GUIContent content = new GUIContent(StringMgr.Instance.Get("ROOM_STATUS_WAITING").ToUpper(), GlobalVars.Instance.iconStart);
					if (GlobalVars.Instance.MyButton3(crdStartBtn, content, "BtnAction") && !Application.isLoadingLevel)
					{
						CSNetManager.Instance.Sock.SendCS_SET_STATUS_REQ(0);
					}
				}
			}
		}
	}

	public void Update()
	{
		inviteAfter += Time.deltaTime;
	}
}
