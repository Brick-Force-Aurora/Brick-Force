using System;
using UnityEngine;

[Serializable]
public class BackConfirmDialog : Dialog
{
	public RenderTexture thumbnail;

	private string text;

	private bool isEditor;

	private UserMapInfo umi;

	public float msgY = 50f;

	private Vector2 sizeOk = new Vector2(100f, 34f);

	public UIMyButton mapAccusation;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.BACK_CONFIRM;
	}

	public override void OnPopup()
	{
		size.x = GlobalVars.Instance.ScreenRect.width;
		rc = new Rect(0f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog(string textMore, bool bEditor)
	{
		text = textMore;
		isEditor = bEditor;
	}

	private void BackToScene()
	{
		GlobalVars.Instance.SetForceClosed(set: false);
		GlobalVars.Instance.tutorFirstScriptOn = true;
		RoomManager.Instance.ClearVote();
		Squad curSquad = SquadManager.Instance.CurSquad;
		if (curSquad != null)
		{
			CSNetManager.Instance.Sock.SendCS_LEAVE_REQ();
			CSNetManager.Instance.Sock.SendCS_LEAVE_SQUAD_REQ();
			SquadManager.Instance.Leave();
		}
		if (!Application.loadedLevelName.Contains("Tutor"))
		{
			P2PManager.Instance.Shutdown();
			CSNetManager.Instance.Sock.SendCS_LEAVE_REQ();
			GlobalVars.Instance.GotoLobbyRoomList = true;
			Application.LoadLevel("Lobby");
		}
		else
		{
			P2PManager.Instance.Shutdown();
			CSNetManager.Instance.Sock.SendCS_LEAVE_REQ();
			Application.LoadLevel("BfStart");
		}
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

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		if (text == null || text.Length <= 0)
		{
			LabelUtil.TextOut(new Vector2(GlobalVars.Instance.ScreenRect.width / 2f, msgY), StringMgr.Instance.Get("ARE_YOU_SURE_BACK"), "BigLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
		else if (!isEditor)
		{
			LabelUtil.TextOut(new Vector2(GlobalVars.Instance.ScreenRect.width / 2f, msgY - 15f), text, "BigLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(GlobalVars.Instance.ScreenRect.width / 2f, msgY + 20f), StringMgr.Instance.Get("ARE_YOU_SURE_BACK"), "BigLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
		else
		{
			LabelUtil.TextOut(new Vector2(GlobalVars.Instance.ScreenRect.width / 2f, msgY), text, "BigLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
		if (isEditor)
		{
			if (GlobalVars.Instance.MyButton(new Rect(GlobalVars.Instance.ScreenRect.width / 2f - 110f, size.y - sizeOk.y - 25f, sizeOk.x, sizeOk.y), StringMgr.Instance.Get("SAVE"), "BtnAction") || GlobalVars.Instance.IsReturnPressed())
			{
				result = true;

                GetCopyRight();
                ThumbnailToPNG();
                //umi is missing or wrong on create should generate this and add to new slot
                CSNetManager.Instance.Sock.SendCS_SAVE_REQ(0, ThumbnailToPNG());
                //CSNetManager.Instance.Sock.SendCS_SAVE_REQ(umi.Slot, ThumbnailToPNG());
				BackToScene();
			}
			if (GlobalVars.Instance.MyButton(new Rect(GlobalVars.Instance.ScreenRect.width / 2f + 10f, size.y - sizeOk.y - 25f, sizeOk.x, sizeOk.y), StringMgr.Instance.Get("CANCEL"), "BtnAction") || GlobalVars.Instance.IsReturnPressed())
			{
				MyInfoManager.Instance.IsModified = false;
				result = true;
				BackToScene();
			}
		}
		else if (GlobalVars.Instance.MyButton(new Rect(GlobalVars.Instance.ScreenRect.width / 2f - 50f, size.y - sizeOk.y - 25f, sizeOk.x, sizeOk.y), StringMgr.Instance.Get("OK"), "BtnAction") || GlobalVars.Instance.IsReturnPressed())
		{
			result = true;
			BackToScene();
		}
		Rect rc = new Rect(size.x - 44f, 5f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
		}
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}
}
