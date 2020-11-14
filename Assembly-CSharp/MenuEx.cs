using System;
using UnityEngine;

[Serializable]
public class MenuEx : Dialog
{
	public RenderTexture thumbnail;

	private UserMapInfo umi;

	private bool copyRight;

	public bool useKickOutVote = true;

	public int kickOutVoteQuorum = 4;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.MENU_EX;
	}

	public override void OnPopup()
	{
		RecalcSize();
	}

	private void RecalcSize()
	{
		copyRight = GetCopyRight();
		size = new Vector2(170f, 196f);
		if (copyRight)
		{
			size.y += 56f;
		}
		else if (IsShowBanishMenu())
		{
			size.y += 28f;
		}
		if (IsShowAccusationMenu())
		{
			size.y += 28f;
		}
		if (IsShowAccusationMapMenu())
		{
			size.y += 28f;
		}
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public override bool DoDialog()
	{
		bool result = false;
		RecalcSize();
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		LabelUtil.TextOut(new Vector2(size.x / 2f, 20f), StringMgr.Instance.Get("MAIN_MENU"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		float num = 50f;
		if (GlobalVars.Instance.MyButton(new Rect(6f, num, 158f, 26f), StringMgr.Instance.Get("CHANGE_SETTING"), "BtnBlue"))
		{
			GlobalVars.Instance.SetForceClosed(set: true);
			((SettingDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.SETTING, exclusive: true))?.InitDialog();
		}
		num += 28f;
		if (!copyRight && IsShowBanishMenu())
		{
			if (GlobalVars.Instance.MyButton(new Rect(6f, num, 158f, 26f), StringMgr.Instance.Get("KICK_VOTE_BUTTON01"), "BtnBlue"))
			{
				if (RoomManager.Instance.IsVoteAble())
				{
					GlobalVars.Instance.SetForceClosed(set: true);
					((VoteBanishDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.VOTE_BANISH, exclusive: true))?.InitDialog();
				}
				else if (!RoomManager.Instance.IsVoteAble() && RoomManager.Instance.IsVoteProgress())
				{
					result = true;
					SystemMsgManager.Instance.ShowMessage(string.Format(StringMgr.Instance.Get("KICK_VOTE_MESSAGE07"), MyInfoManager.Instance.Nickname));
				}
				else if (BrickManManager.Instance.GetPlayingPlayerCount() < kickOutVoteQuorum - 1)
				{
					result = true;
					SystemMsgManager.Instance.ShowMessage(string.Format(StringMgr.Instance.Get("KICK_VOTE_MESSAGE01"), kickOutVoteQuorum));
				}
				else
				{
					GlobalVars.Instance.SetForceClosed(set: true);
					((SuggestBanishDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.SUGGEST_BANISH, exclusive: true))?.InitDialog();
				}
			}
			num += 28f;
		}
		if (IsShowAccusationMenu())
		{
			if (GlobalVars.Instance.MyButton(new Rect(6f, num, 158f, 26f), StringMgr.Instance.Get("REPORT_GM_TITLE_01"), "BtnBlue"))
			{
				BrickManDesc[] array = BrickManManager.Instance.ToDescriptorArray();
				AccusationDialog accusationDialog = (AccusationDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.ACCUSATION, exclusive: true);
				string[] array2 = new string[array.Length + 1];
				array2[0] = StringMgr.Instance.Get("REPORT_GM_USER");
				for (int i = 0; i < array.Length; i++)
				{
					array2[i + 1] = array[i].Nickname;
				}
				accusationDialog?.InitDialog(array2);
			}
			num += 28f;
		}
		if (IsShowAccusationMapMenu())
		{
			if (GlobalVars.Instance.MyButton(new Rect(6f, num, 158f, 26f), StringMgr.Instance.Get("REPORT_GM_TITLE_02"), "BtnBlue"))
			{
				RegMap regMap = RegMapManager.Instance.Get(RoomManager.Instance.CurMap);
				if (regMap != null && regMap.Thumbnail != null)
				{
					((AccusationMapDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.ACCUSATION_MAP, exclusive: true))?.InitDialog(regMap);
				}
			}
			num += 28f;
		}
		if (copyRight)
		{
			if (GlobalVars.Instance.MyButton(new Rect(6f, num, 158f, 26f), StringMgr.Instance.Get("SAVE"), "BtnBlue"))
			{
				result = true;
				CSNetManager.Instance.Sock.SendCS_SAVE_REQ(umi.Slot, ThumbnailToPNG());
			}
			num += 28f;
			if (GlobalVars.Instance.MyButton(new Rect(6f, num, 158f, 26f), StringMgr.Instance.Get("REGISTER"), "BtnBlue"))
			{
				RegisterDialog registerDialog = (RegisterDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.REGISTER, exclusive: true);
				if (registerDialog != null)
				{
					GlobalVars.Instance.SetForceClosed(set: true);
					registerDialog.InitDialog(umi);
				}
			}
			num += 28f;
		}
		if (GlobalVars.Instance.MyButton(new Rect(6f, num, 158f, 26f), StringMgr.Instance.Get("BTN_SELF_RESPAWN"), "BtnBlue"))
		{
			if (_SelfRespawnDialog())
			{
				GlobalVars.Instance.SetForceClosed(set: true);
			}
			else
			{
				result = true;
			}
		}
		num += 28f;
		if (GlobalVars.Instance.MyButton(new Rect(6f, num, 158f, 26f), StringMgr.Instance.Get("EXIT_ROOM"), "BtnBlue"))
		{
			GlobalVars.Instance.SetForceClosed(set: true);
			_BackConfirmDialog();
		}
		num += 28f;
		if (GlobalVars.Instance.MyButton(new Rect(6f, num, 158f, 26f), StringMgr.Instance.Get("QUIT_GAME"), "BtnBlue"))
		{
			GlobalVars.Instance.SetForceClosed(set: true);
			_ExitConfirmDialog();
		}
		num += 28f;
		if (GlobalVars.Instance.MyButton(new Rect(6f, num, 158f, 26f), StringMgr.Instance.Get("CANCEL"), "BtnBlue"))
		{
			result = true;
		}
		if (GlobalVars.Instance.IsEscapePressed())
		{
			GlobalVars.Instance.resetMenuEx();
			result = true;
		}
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}

	public void InitDialog()
	{
		GlobalVars.Instance.IsEscapePressed();
	}

	private bool IsTutorial()
	{
		if (Application.loadedLevelName.Contains("Tutor"))
		{
			return true;
		}
		return false;
	}

	private bool GetCopyRight()
	{
		bool result = false;
		umi = null;
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.MAP_EDITOR)
		{
			umi = UserMapInfoManager.Instance.GetCur();
			if (umi != null)
			{
				result = true;
			}
		}
		return result;
	}

	private void _BackConfirmDialog()
	{
		BackConfirmDialog backConfirmDialog = (BackConfirmDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.BACK_CONFIRM, exclusive: true);
		if (backConfirmDialog != null)
		{
			string textMore = string.Empty;
			if (RoomManager.Instance.CurrentRoomType != 0)
			{
				Room room = RoomManager.Instance.GetRoom(RoomManager.Instance.CurrentRoom);
				if (room != null && room.Status == Room.ROOM_STATUS.PLAYING)
				{
					textMore = StringMgr.Instance.Get("CAN_NOT_GET_XPNPOINT");
				}
				backConfirmDialog.InitDialog(textMore, bEditor: false);
			}
			else if (MyInfoManager.Instance.IsModified && UserMapInfoManager.Instance.master == MyInfoManager.Instance.Seq)
			{
				Room room2 = RoomManager.Instance.GetRoom(RoomManager.Instance.CurrentRoom);
				if (room2 != null && room2.Status == Room.ROOM_STATUS.PLAYING)
				{
					textMore = StringMgr.Instance.Get("MAPEDIT_END_POPUP");
				}
				backConfirmDialog.InitDialog(textMore, bEditor: true);
			}
			else
			{
				backConfirmDialog.InitDialog(textMore, bEditor: false);
			}
		}
	}

	private void _ExitConfirmDialog()
	{
		ExitConfirmDialog exitConfirmDialog = (ExitConfirmDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.EXIT_CONFIRM, exclusive: true);
		if (exitConfirmDialog != null)
		{
			string textMore = string.Empty;
			if (RoomManager.Instance.CurrentRoomType != 0)
			{
				Room room = RoomManager.Instance.GetRoom(RoomManager.Instance.CurrentRoom);
				if (room != null && room.Status == Room.ROOM_STATUS.PLAYING)
				{
					textMore = StringMgr.Instance.Get("CAN_NOT_GET_XPNPOINT");
				}
			}
			exitConfirmDialog.InitDialog(textMore);
		}
	}

	private bool _SelfRespawnDialog()
	{
		LocalController localController = null;
		GameObject gameObject = GameObject.Find("Me");
		if (null != gameObject)
		{
			localController = gameObject.GetComponent<LocalController>();
		}
		Room room = RoomManager.Instance.GetRoom(RoomManager.Instance.CurrentRoom);
		if (room != null && room.Status == Room.ROOM_STATUS.PLAYING && localController != null && !localController.Invincible)
		{
			((SelfRespawnDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.SELF_RESPAWN_CONFIRM, exclusive: true))?.InitDialog(StringMgr.Instance.Get("SELF_RESPAWN_WARNING"));
			return true;
		}
		SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("SELF_RESPAWN_WARNING2"));
		return false;
	}

	private byte[] ThumbnailToPNG()
	{
		int width = thumbnail.width;
		int height = thumbnail.height;
		RenderTexture.active = thumbnail;
		Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGB24, mipmap: false);
		texture2D.ReadPixels(new Rect(0f, 0f, (float)width, (float)height), 0, 0);
		texture2D.Apply();
		RenderTexture.active = null;
		return texture2D.EncodeToPNG();
	}

	private bool IsShowBanishMenu()
	{
		if (!useKickOutVote)
		{
			return false;
		}
		if (IsTutorial())
		{
			return false;
		}
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.MAP_EDITOR)
		{
			return false;
		}
		return true;
	}

	private bool IsShowAccusationMenu()
	{
		if (IsTutorial())
		{
			return false;
		}
		if (BrickManManager.Instance.GetDescCount() == 0)
		{
			return false;
		}
		return BuildOption.Instance.Props.UseAccuse;
	}

	private bool IsShowAccusationMapMenu()
	{
		if (IsTutorial())
		{
			return false;
		}
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.MAP_EDITOR)
		{
			return false;
		}
		return BuildOption.Instance.Props.UseAccuse;
	}
}
