using System;
using UnityEngine;

[Serializable]
public class MapInfoDlg : Dialog
{
	public Texture2D nonAvailable;

	public Texture2D icon;

	public Rect crdIcon = new Rect(0f, 0f, 12f, 12f);

	public Vector2 crdTitle = new Vector2(0f, 0f);

	public Rect crdThumbnail = new Rect(27f, 63f, 128f, 128f);

	public Vector2 crdAlias = new Vector2(166f, 25f);

	public Vector2 crdDeveloper = new Vector2(166f, 60f);

	public Vector2 crdLastModified = new Vector2(166f, 80f);

	public Vector2 crdPossibleMode = new Vector2(166f, 100f);

	public Vector2 crdModeLT = new Vector2(166f, 120f);

	public Rect crdButtonBase = new Rect(340f, 235f, 90f, 21f);

	public Rect crdOutline = new Rect(0f, 0f, 100f, 100f);

	public Rect crdMapInfoBox = new Rect(0f, 0f, 100f, 100f);

	private RegMap regMap;

	private UserMapInfo userMap;

	public RegMap GetRegMap()
	{
		return regMap;
	}

	public UserMapInfo GetUserMap()
	{
		return userMap;
	}

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.MAP_INFO;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog(RegMap mi)
	{
		userMap = null;
		regMap = mi;
	}

	public void InitDialog(UserMapInfo mi)
	{
		userMap = mi;
		regMap = null;
	}

	private void DrawThumbnail()
	{
		if (userMap != null)
		{
			TextureUtil.DrawTexture(crdThumbnail, (!(userMap.Thumbnail == null)) ? userMap.Thumbnail : nonAvailable);
		}
		else if (regMap != null)
		{
			TextureUtil.DrawTexture(crdThumbnail, (!(regMap.Thumbnail == null)) ? regMap.Thumbnail : nonAvailable);
		}
	}

	private void PrintMapInfo()
	{
		GUI.Box(crdMapInfoBox, string.Empty, "BoxSunken");
		if (userMap != null)
		{
			LabelUtil.TextOut(crdAlias, userMap.Alias, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdDeveloper, StringMgr.Instance.Get("DEVELOPER_IS") + " " + MyInfoManager.Instance.Nickname, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdLastModified, StringMgr.Instance.Get("LAST_MODIFIED_DATE") + " " + DateTimeLocal.ToString(userMap.LastModified), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		else if (regMap != null)
		{
			LabelUtil.TextOut(crdAlias, regMap.Alias, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdDeveloper, StringMgr.Instance.Get("DEVELOPER_IS") + " " + regMap.Developer, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdLastModified, StringMgr.Instance.Get("REGISTERED_DATE") + " " + DateTimeLocal.ToString(regMap.RegisteredDate), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdPossibleMode, StringMgr.Instance.Get("POSSIBLE_MODE"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text = Room.ModeMask2String(regMap.ModeMask);
			LabelUtil.TextOut(crdModeLT, text, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
	}

	private bool CanMakeRoom()
	{
		Room room = RoomManager.Instance.GetRoom(RoomManager.Instance.CurrentRoom);
		if (room != null)
		{
			return false;
		}
		if (ChannelManager.Instance.CurChannel == null)
		{
			return false;
		}
		if (ChannelManager.Instance.CurChannel.Mode == 3)
		{
			return null != userMap;
		}
		if (regMap == null)
		{
			return false;
		}
		return RegMapManager.Instance.IsDownloaded(regMap.Map);
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		TextureUtil.DrawTexture(crdIcon, icon, ScaleMode.StretchToFill);
		LabelUtil.TextOut(crdTitle, StringMgr.Instance.Get("MAP_INFO"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		GUI.Box(crdOutline, string.Empty, "BoxPopLine");
		DrawThumbnail();
		PrintMapInfo();
		Rect rc = crdButtonBase;
		if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("CANCEL"), "BtnAction"))
		{
			result = true;
		}
		rc.x -= 100f;
		if (CanMakeRoom())
		{
			if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("CREATE_ROOM"), "BtnAction"))
			{
				UserMapInfo userMapInfo = GetUserMap();
				if (userMapInfo != null)
				{
					CreateRoomDialog createRoomDialog = (CreateRoomDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.CREATE_ROOM, exclusive: true);
					if (createRoomDialog != null && !createRoomDialog.InitDialog4MapEditorLoad(userMapInfo.Slot))
					{
						DialogManager.Instance.Clear();
					}
				}
				else
				{
					RegMap regMap = GetRegMap();
					if (regMap != null)
					{
						CreateRoomDialog createRoomDialog2 = (CreateRoomDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.CREATE_ROOM, exclusive: true);
						if (!createRoomDialog2.InitDialog4TeamMatch(regMap.Map, regMap.ModeMask))
						{
							DialogManager.Instance.Clear();
						}
					}
				}
			}
			rc.x -= 100f;
		}
		if (userMap != null)
		{
			if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("RENAME_MAP"), "BtnAction"))
			{
				((RenameMapDlg)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.RENAME_MAP, exclusive: true))?.InitDialog(userMap);
			}
			rc.x -= 100f;
		}
		if (this.regMap != null)
		{
			if (RegMapManager.Instance.IsDownloaded(this.regMap.Map))
			{
				if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("DELETE"), "BtnAction"))
				{
					CSNetManager.Instance.Sock.SendCS_DEL_DOWNLOAD_MAP_REQ(this.regMap.Map);
				}
				rc.x -= 100f;
			}
			else
			{
				bool enabled = GUI.enabled;
				GUI.enabled = this.regMap.IsLatest;
				if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("SAVE"), "BtnAction"))
				{
					((DownloadFeeDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.DOWNLOAD_FEE, exclusive: true))?.InitDialog(this.regMap);
				}
				GUI.enabled = enabled;
				rc.x -= 100f;
			}
		}
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}
}
