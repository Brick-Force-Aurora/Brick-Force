using System;
using UnityEngine;

[Serializable]
public class EditingMapFrame
{
	public Texture2D premiumIcon;

	public Texture2D slotLock;

	public Texture2D slotEmpty;

	public Texture2D emptySlot;

	public Texture2D nonAvailable;

	public Texture2D selectedMapFrame;

	private UserMapInfo[] umi;

	private Texture2D[] umiThumbnail;

	private Vector2 scrollPosition = Vector2.zero;

	private Rect crdLeftBtn = new Rect(540f, 724f, 22f, 18f);

	private Rect crdRightBtn = new Rect(691f, 724f, 22f, 18f);

	private Rect crdPageBox = new Rect(572f, 724f, 108f, 18f);

	private Vector2 crdMapSize = new Vector2(150f, 196f);

	private Vector2 crdMapOffset = new Vector2(35f, 21f);

	private Rect crdThumbnail = new Rect(6f, 6f, 128f, 128f);

	private Vector2 crdMapNameLabel = new Vector2(145f, 10f);

	private Vector2 crdMapNameVal = new Vector2(150f, 30f);

	private Vector2 crdLastModifiedLabel = new Vector2(145f, 55f);

	private Vector2 crdLastModifiedVal = new Vector2(150f, 75f);

	private Rect crdRegMapRect = new Rect(264f, 142f, 732f, 515f);

	private Rect crdRegMapRectTemp = new Rect(264f, 142f, 732f, 515f);

	private Vector2 crdAlias = new Vector2(5f, 174f);

	private float chatGap = 275f;

	private Rect[] crdBtns;

	private Rect[] crdBtns2;

	private int selected;

	private int page = 1;

	private float doubleClickTimeout = 0.3f;

	private float lastClickTime;

	public int addSlot;

	private bool chatView;

	public void Start()
	{
		crdBtns = new Rect[3];
		crdBtns[0] = new Rect(266f, 714f, 139f, 38f);
		crdBtns[1] = new Rect(715f, 714f, 139f, 38f);
		crdBtns[2] = new Rect(859f, 714f, 139f, 38f);
		crdBtns2 = new Rect[3];
		crdBtns2[0] = new Rect(266f, 714f - chatGap, 139f, 38f);
		crdBtns2[1] = new Rect(715f, 714f - chatGap, 139f, 38f);
		crdBtns2[2] = new Rect(859f, 714f - chatGap, 139f, 38f);
		UserMapInfoManager.Instance.Verify();
		CSNetManager.Instance.Sock.SendCS_USER_MAP_REQ(page);
	}

	public void BeginMapList(int curPage)
	{
		page = curPage;
		scrollPosition = Vector2.zero;
	}

	public int GetEmptyPremiumSlot(int page, int count)
	{
		if (count == 12)
		{
			return 0;
		}
		UserMapInfo[] array = UserMapInfoManager.Instance.ToArray();
		int num = array.Length / 12 + 1;
		if (page > num)
		{
			return 0;
		}
		int num2 = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Alias.Length <= 0)
			{
				num2++;
			}
			else if (array[i].IsPremium)
			{
				num2++;
			}
		}
		return num2;
	}

	private void UpdateThumbnail()
	{
		umi = UserMapInfoManager.Instance.ToArray(page);
		umiThumbnail = new Texture2D[umi.Length];
		addSlot = 0;
		for (int i = 0; i < umi.Length; i++)
		{
			if (umi[i].Alias.Length <= 0)
			{
				umiThumbnail[i] = emptySlot;
				addSlot++;
			}
			else if (umi[i].Thumbnail == null)
			{
				umiThumbnail[i] = nonAvailable;
			}
			else
			{
				umiThumbnail[i] = umi[i].Thumbnail;
			}
			if (umi[i].IsPremium)
			{
				addSlot++;
			}
		}
	}

	private void DoPagePanel()
	{
		Rect rc = new Rect(crdLeftBtn);
		if (chatView)
		{
			rc.y -= chatGap;
		}
		if (GlobalVars.Instance.MyButton(rc, string.Empty, "Left"))
		{
			int num = page - 1;
			if (num < 1)
			{
				num = 1;
			}
			selected = 0;
			CSNetManager.Instance.Sock.SendCS_USER_MAP_REQ(num);
		}
		Rect rc2 = new Rect(crdRightBtn);
		if (chatView)
		{
			rc2.y -= chatGap;
		}
		if (GlobalVars.Instance.MyButton(rc2, string.Empty, "Right"))
		{
			selected = 0;
			CSNetManager.Instance.Sock.SendCS_USER_MAP_REQ(page + 1);
		}
		Rect position = new Rect(crdPageBox);
		if (chatView)
		{
			position.y -= chatGap;
		}
		GUI.Box(position, string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(position.x + position.width / 2f, position.y + position.height / 2f), page.ToString(), "MiniLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
	}

	private void PrintMapInfo(int i)
	{
		Color txtMainColor = GlobalVars.Instance.txtMainColor;
		LabelUtil.TextOut(crdMapNameLabel, StringMgr.Instance.Get("MAP_NAME_IS"), "MiniLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMapNameVal, umi[i].Alias, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdLastModifiedLabel, StringMgr.Instance.Get("LAST_MODIFIED_DATE"), "MiniLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdLastModifiedVal, DateTimeLocal.ToString(umi[i].LastModified), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
	}

	private void VerifyChatView()
	{
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			Lobby component = gameObject.GetComponent<Lobby>();
			if (null != component)
			{
				chatView = component.bChatView;
			}
			Briefing4TeamMatch component2 = gameObject.GetComponent<Briefing4TeamMatch>();
			if (null != component2)
			{
				chatView = component2.bChatView;
			}
		}
	}

	public void OnGUI()
	{
		bool flag = MyInfoManager.Instance.HaveFunction("premium_account") >= 0;
		UpdateThumbnail();
		int num = umi.Length / 4;
		if (umi.Length % 4 > 0)
		{
			num++;
		}
		Rect viewRect = new Rect(0f, 0f, crdMapSize.x * 4f + crdMapOffset.x * 3f, crdMapSize.y * (float)num);
		if (num > 1)
		{
			viewRect.height += crdMapOffset.y * (float)(num - 1);
		}
		VerifyChatView();
		if (chatView)
		{
			crdRegMapRect.height = 300f;
		}
		else
		{
			crdRegMapRect.height = crdRegMapRectTemp.height;
		}
		scrollPosition = GUI.BeginScrollView(crdRegMapRect, scrollPosition, viewRect);
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				int num2 = 4 * i + j;
				if (num2 < umi.Length)
				{
					Rect rect = new Rect((float)j * (crdMapSize.x + crdMapOffset.x), (float)i * (crdMapSize.y + crdMapOffset.y), crdMapSize.x, crdMapSize.y);
					Texture2D texture2D = null;
					Rect position = new Rect(rect.x, rect.y, rect.width, rect.width + 4f);
					if (umi[num2].Alias.Length <= 0)
					{
						texture2D = emptySlot;
					}
					else if (umi[num2].Thumbnail != null)
					{
						texture2D = umi[num2].Thumbnail;
					}
					if (texture2D != null)
					{
						TextureUtil.DrawTexture(position, texture2D, ScaleMode.StretchToFill);
					}
					string str = "BoxMapSelectBorder";
					if (umi[num2].IsPremium)
					{
						str = "BoxMapSelectBorderPremium";
					}
					if (GlobalVars.Instance.MyButton(rect, string.Empty, str))
					{
						if (!umi[num2].IsPremium || flag)
						{
							selected = num2;
						}
						if (Time.time - lastClickTime > doubleClickTimeout)
						{
							lastClickTime = Time.time;
						}
						else if (umi[selected].Alias.Length > 0)
						{
							if (ChannelManager.Instance.CurChannel.Mode == 3)
							{
								CreateRoomDialog createRoomDialog = (CreateRoomDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.CREATE_ROOM, exclusive: true);
								if (createRoomDialog != null && !createRoomDialog.InitDialog4MapEditorLoad(umi[selected].Slot))
								{
									DialogManager.Instance.Clear();
								}
							}
						}
						else if (ChannelManager.Instance.CurChannel.Mode == 3)
						{
							CreateRoomDialog createRoomDialog2 = (CreateRoomDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.CREATE_ROOM, exclusive: true);
							if (createRoomDialog2 != null && !createRoomDialog2.InitDialog4MapEditorNew(umi[selected].Slot))
							{
								DialogManager.Instance.Clear();
							}
						}
					}
					LabelUtil.TextOut(new Vector2(rect.x + crdAlias.x, rect.y + crdAlias.y), umi[num2].Alias, "MiniLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
					if (selected == num2)
					{
						TextureUtil.DrawTexture(rect, selectedMapFrame, ScaleMode.StretchToFill);
					}
					bool flag2 = false;
					if (umi[num2].IsPremium)
					{
						TextureUtil.DrawTexture(new Rect(rect.x + 5f, rect.y + 5f, (float)premiumIcon.width, (float)premiumIcon.height), premiumIcon);
						if (!flag)
						{
							flag2 = true;
							TextureUtil.DrawTexture(new Rect(rect.x + 5f + crdThumbnail.x + (crdThumbnail.width - (float)slotLock.width) / 2f, rect.y + crdThumbnail.y + (crdThumbnail.height - (float)slotLock.height) / 2f, (float)slotLock.width, (float)slotLock.height), slotLock, ScaleMode.StretchToFill);
						}
					}
					if (umi[num2].Alias.Length == 0 && !flag2)
					{
						TextureUtil.DrawTexture(new Rect(rect.x + 5f + crdThumbnail.x + (crdThumbnail.width - (float)slotEmpty.width) / 2f, rect.y + crdThumbnail.y + (crdThumbnail.height - (float)slotEmpty.height) / 2f, (float)slotEmpty.width, (float)slotEmpty.height), slotEmpty, ScaleMode.StretchToFill);
					}
				}
			}
		}
		GUI.EndScrollView();
		if (umi[selected].Alias.Length > 0)
		{
			int num3 = 2;
			Rect rc = new Rect(crdBtns[num3]);
			if (chatView)
			{
				rc = crdBtns2[num3];
			}
			GUIContent content;
			if (ChannelManager.Instance.CurChannel.Mode == 3)
			{
				content = new GUIContent(StringMgr.Instance.Get("CREATE_ROOM").ToUpper(), GlobalVars.Instance.iconJoin);
				if (GlobalVars.Instance.MyButton3(rc, content, "BtnAction"))
				{
					CreateRoomDialog createRoomDialog3 = (CreateRoomDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.CREATE_ROOM, exclusive: true);
					if (createRoomDialog3 != null && !createRoomDialog3.InitDialog4MapEditorLoad(umi[selected].Slot))
					{
						DialogManager.Instance.Clear();
					}
				}
				num3--;
			}
			rc = crdBtns[num3];
			if (chatView)
			{
				rc = crdBtns2[num3];
			}
			content = new GUIContent(StringMgr.Instance.Get("RENAME_MAP").ToUpper(), GlobalVars.Instance.iconRewrite);
			if (GlobalVars.Instance.MyButton3(rc, content, "BtnAction"))
			{
				((RenameMapDlg)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.RENAME_MAP, exclusive: true))?.InitDialog(umi[selected]);
			}
			num3--;
			rc = crdBtns[num3];
			if (chatView)
			{
				rc = crdBtns2[num3];
			}
			Item item = MyInfoManager.Instance.IsExistItem("s27");
			if (item != null)
			{
				content = new GUIContent(StringMgr.Instance.Get("SLOT_INIT").ToUpper(), GlobalVars.Instance.iconReformat);
				if (GlobalVars.Instance.MyButton3(rc, content, "BtnAction"))
				{
					if (umi[selected].IsPremium && !flag)
					{
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("LOCKED_SLOT_PREMIUM"));
					}
					else
					{
						((AreYouSure)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.ARE_YOU_SURE, exclusive: true))?.InitDialog(item, AreYouSure.SURE.RESET_MAP_SLOT, umi[selected]);
					}
				}
			}
		}
		else if (ChannelManager.Instance.CurChannel.Mode == 3)
		{
			Rect rc2 = new Rect(crdBtns[2]);
			if (chatView)
			{
				rc2 = crdBtns2[2];
			}
			GUIContent content2 = new GUIContent(StringMgr.Instance.Get("CREATE_ROOM").ToUpper(), GlobalVars.Instance.iconJoin);
			if (GlobalVars.Instance.MyButton3(rc2, content2, "BtnAction"))
			{
				CreateRoomDialog createRoomDialog4 = (CreateRoomDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.CREATE_ROOM, exclusive: true);
				if (createRoomDialog4 != null && !createRoomDialog4.InitDialog4MapEditorNew(umi[selected].Slot))
				{
					DialogManager.Instance.Clear();
				}
			}
		}
		DoPagePanel();
	}
}
