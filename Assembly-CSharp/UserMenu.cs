using System;
using UnityEngine;

[Serializable]
public class UserMenu : Dialog
{
	private int target = -1;

	private string targetNickname = string.Empty;

	private bool isClanInvitable;

	private bool isMasterAssign;

	private Rect crdBtnBase = new Rect(4f, 4f, 100f, 26f);

	public float offset = 4f;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.USER_MENU;
	}

	public override bool DoDialog()
	{
		if (target < 0 || target == MyInfoManager.Instance.Seq)
		{
			return true;
		}
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		Rect rc = crdBtnBase;
		if (RoomManager.Instance.HaveCurrentRoomInfo)
		{
			if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("INVITE_MENU"), "BtnBlue"))
			{
				CSNetManager.Instance.Sock.SendCS_INVITE_REQ(target, targetNickname);
				result = true;
			}
			rc.y += crdBtnBase.height + 4f;
		}
		else
		{
			if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("JOIN_MENU"), "BtnBlue"))
			{
				CSNetManager.Instance.Sock.SendCS_FOLLOWING_REQ(target, targetNickname);
				result = true;
			}
			rc.y += crdBtnBase.height + 4f;
		}
		if (!MyInfoManager.Instance.IsFriend(target) && !MyInfoManager.Instance.IsBan(target))
		{
			if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("ADD_FRIEND"), "BtnBlue"))
			{
				CSNetManager.Instance.Sock.SendCS_ADD_FRIEND_REQ(target);
				result = true;
			}
			rc.y += crdBtnBase.height + 4f;
			if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("ADD_BAN"), "BtnBlue"))
			{
				CSNetManager.Instance.Sock.SendCS_ADD_BAN_REQ(target);
				result = true;
			}
			rc.y += crdBtnBase.height + 4f;
		}
		else
		{
			if (MyInfoManager.Instance.IsFriend(target))
			{
				if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("DEL_FRIEND"), "BtnBlue"))
				{
					CSNetManager.Instance.Sock.SendCS_DEL_FRIEND_REQ(target);
					result = true;
				}
				rc.y += crdBtnBase.height + 4f;
			}
			if (MyInfoManager.Instance.IsBan(target))
			{
				if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("DEL_BAN"), "BtnBlue"))
				{
					CSNetManager.Instance.Sock.SendCS_DEL_BAN_REQ(target);
					result = true;
				}
				rc.y += crdBtnBase.height + 4f;
			}
		}
		if (MyInfoManager.Instance.IsClanStaff && isClanInvitable)
		{
			if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("CLAN_INVITATION"), "BtnBlue"))
			{
				string title = "CLAN_INVITATION";
				string contents = "CLAN_INVITATION_COMMENT" + GlobalVars.DELIMITER + "n" + MyInfoManager.Instance.Nickname + GlobalVars.DELIMITER + "n" + MyInfoManager.Instance.ClanName;
				CSNetManager.Instance.Sock.SendCS_SEND_CLAN_INVITATION_REQ(target, targetNickname, title, contents);
				result = true;
			}
			rc.y += crdBtnBase.height + 4f;
		}
		if (isMasterAssign)
		{
			if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("MASTER_ASSIGN"), "BtnBlue"))
			{
				CSNetManager.Instance.Sock.SendCS_DELEGATE_MASTER_REQ(target);
				result = true;
			}
			rc.y += crdBtnBase.height + 4f;
		}
		if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("WHISPER"), "BtnBlue"))
		{
			DialogManager.Instance.CloseAll();
			GameObject gameObject = GameObject.Find("Main");
			if (gameObject != null)
			{
				Lobby component = gameObject.GetComponent<Lobby>();
				if (component != null)
				{
					component.lobbyChat.Message = "/w " + targetNickname + " ";
					component.lobbyChat.ApplyFocus();
					component.lobbyChat.CursorToEnd = true;
				}
				Briefing4TeamMatch component2 = gameObject.GetComponent<Briefing4TeamMatch>();
				if (component2 != null)
				{
					component2.IsMessenger = false;
					component2.lobbyChat.Message = "/w " + targetNickname + " ";
					component2.lobbyChat.ApplyFocus();
					component2.lobbyChat.CursorToEnd = true;
				}
			}
			result = true;
		}
		rc.y += crdBtnBase.height + 4f;
		if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("SEND_MEMO"), "BtnBlue"))
		{
			result = true;
			DialogManager.Instance.CloseAll();
			DialogManager.Instance.Push(DialogManager.DIALOG_INDEX.MEMO, targetNickname);
		}
		rc.y += crdBtnBase.height + 4f;
		if (BuildOption.Instance.Props.UseAccuse)
		{
			if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("REPORT_GM_TITLE_01"), "BtnBlue"))
			{
				AccusationDialog accusationDialog = (AccusationDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.ACCUSATION, exclusive: true);
				string[] users = new string[1]
				{
					targetNickname
				};
				accusationDialog?.InitDialog(users);
				result = true;
			}
			rc.y += crdBtnBase.height + 4f;
		}
		if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("SHOW_PLAYER_INFO"), "BtnBlue"))
		{
			CSNetManager.Instance.Sock.SendCS_PLAYER_DETAIL_REQ(target);
			result = true;
		}
		rc.y += crdBtnBase.height + 4f;
		WindowUtil.EatEvent();
		GUI.skin = skin;
		return result;
	}

	private void RecalcButtonWidth()
	{
		string[] array = new string[12]
		{
			StringMgr.Instance.Get("INVITE_MENU"),
			StringMgr.Instance.Get("JOIN_MENU"),
			StringMgr.Instance.Get("ADD_FRIEND"),
			StringMgr.Instance.Get("ADD_BAN"),
			StringMgr.Instance.Get("DEL_FRIEND"),
			StringMgr.Instance.Get("DEL_BAN"),
			StringMgr.Instance.Get("CLAN_INVITATION"),
			StringMgr.Instance.Get("SHOW_PLAYER_INFO"),
			StringMgr.Instance.Get("MASTER_ASSIGN"),
			StringMgr.Instance.Get("WHISPER"),
			StringMgr.Instance.Get("SEND_MEMO"),
			StringMgr.Instance.Get("REPORT_GM_TITLE_01")
		};
		GUIStyle style = GUI.skin.GetStyle("BtnAction");
		if (style != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				Vector2 vector = style.CalcSize(new GUIContent(array[i]));
				if (vector.x + 4f > crdBtnBase.width)
				{
					crdBtnBase.width = vector.x + 4f;
				}
			}
		}
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog(Vector3 pos, int seq, string nickname, bool clanInvitable, bool masterAssign)
	{
		pos.x *= GlobalVars.Instance.ScreenRect.width / (float)Screen.width;
		pos.y *= GlobalVars.Instance.ScreenRect.height / (float)Screen.height;
		target = seq;
		targetNickname = nickname;
		isClanInvitable = clanInvitable;
		isMasterAssign = masterAssign;
		RecalcButtonWidth();
		if (!MyInfoManager.Instance.IsFriend(target) && !MyInfoManager.Instance.IsBan(target))
		{
			size = new Vector2(crdBtnBase.width + 8f, crdBtnBase.height * 3f + 4f * offset);
		}
		else
		{
			size = new Vector2(crdBtnBase.width + 8f, crdBtnBase.height * 2f + 3f * offset);
		}
		if (MyInfoManager.Instance.IsClanStaff && isClanInvitable)
		{
			size.y += crdBtnBase.height + offset;
		}
		if (isMasterAssign)
		{
			size.y += crdBtnBase.height + offset;
		}
		if (BuildOption.Instance.Props.UseAccuse)
		{
			size.y += crdBtnBase.height + offset;
		}
		size.y += crdBtnBase.height * 3f + 3f * offset;
		if (pos.x + size.x > GlobalVars.Instance.ScreenRect.width)
		{
			pos.x -= size.x;
		}
		if (pos.y + size.y > GlobalVars.Instance.ScreenRect.height)
		{
			pos.y -= size.y;
		}
		rc = new Rect(pos.x, pos.y, size.x, size.y);
	}
}
