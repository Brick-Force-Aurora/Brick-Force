using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MyRegMapFrame
{
	public int BoardIndex = 2;

	public Texture2D nonAvailable;

	public Texture2D selectedMapFrame;

	public Texture2D mapFrame;

	public Texture2D texStarGrade;

	public Texture2D texStarGradeBg;

	public float doubleClickTimeout = 0.2f;

	private List<RegMap> reg;

	private int subTab;

	private int page;

	private bool waitingList;

	public int firstIndexer = -1;

	public int lastIndexer = -1;

	public Rect crdFrame = new Rect(255f, 100f, 500f, 320f);

	public Rect crdTab = new Rect(250f, 70f, 340f, 26f);

	private Rect crdLeftBtn = new Rect(542f, 724f, 22f, 18f);

	private Rect crdRightBtn = new Rect(691f, 724f, 22f, 18f);

	private Rect crdPageBox = new Rect(572f, 724f, 108f, 18f);

	private Vector2 scrollPosition = Vector2.zero;

	private Vector2 crdMapSize = new Vector2(150f, 196f);

	private Vector2 crdMapOffset = new Vector2(35f, 21f);

	private Rect crdRegMapRect = new Rect(264f, 142f, 732f, 515f);

	private Rect crdRegMapRectTemp = new Rect(264f, 142f, 732f, 515f);

	private float chatGap = 275f;

	private Vector2 crdMapNameText = new Vector2(495f, 270f);

	private Vector2 crdLastDateText = new Vector2(495f, 285f);

	private Vector2 crdDnCountText = new Vector2(495f, 300f);

	private Vector2 crdMapEvalText = new Vector2(495f, 315f);

	private Vector2 crdSppModeText = new Vector2(495f, 355f);

	private Rect crdStarGrade = new Rect(495f, 338f, 74f, 14f);

	private Rect[] crdBtns;

	private Rect[] crdBtns2;

	private Rect crdIconMode = new Rect(556f, 357f, 35f, 22f);

	private int selected = -1;

	public Vector2 mapSize = new Vector2(128f, 128f);

	public Vector2 mapOffset = new Vector2(6f, 6f);

	public Vector2 crdDeveloper = new Vector2(0f, 92f);

	private Vector2 crdAlias = new Vector2(5f, 174f);

	public int modeTab;

	private bool bUpdateList;

	private float lastClickTime;

	private Vector2 crdModeTextSize = new Vector2(0f, 0f);

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
		reg = new List<RegMap>();
		page = 1;
		waitingList = true;
		CSNetManager.Instance.Sock.SendCS_MY_REGISTER_MAP_REQ(-1, 1, 0, GlobalVars.Instance.getBattleMode(subTab));
	}

	public void SelectedTab(int tab)
	{
		int num = subTab;
		subTab = tab;
		if (num != subTab)
		{
			bUpdateList = true;
			BeginMapList(0);
			selected = -1;
		}
	}

	public void BeginMapList(int curPage)
	{
		page = curPage;
		reg.Clear();
	}

	public void AddMapItem(RegMap regMap)
	{
		reg.Add(regMap);
	}

	public void EndMapList()
	{
		waitingList = false;
	}

	private int MinIndex()
	{
		int num = 2147483647;
		for (int i = 0; i < reg.Count; i++)
		{
			if (num > reg[i].Map)
			{
				num = reg[i].Map;
			}
		}
		return num;
	}

	private int MaxIndex()
	{
		int num = -2147483648;
		for (int i = 0; i < reg.Count; i++)
		{
			if (num < reg[i].Map)
			{
				num = reg[i].Map;
			}
		}
		return num;
	}

	private void DoModeSelector()
	{
		int num = subTab;
		if (bUpdateList)
		{
			bUpdateList = false;
			if (waitingList)
			{
				subTab = num;
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("WAIT_A_MOMENT"));
			}
			else
			{
				waitingList = true;
				CSNetManager.Instance.Sock.SendCS_MY_REGISTER_MAP_REQ(-1, 1, 0, GlobalVars.Instance.getBattleMode(subTab));
			}
		}
	}

	private void DoPagePanel()
	{
		int num = MaxIndex();
		int num2 = MinIndex();
		Rect rc = new Rect(crdLeftBtn);
		if (chatView)
		{
			rc.y -= chatGap;
		}
		if (GlobalVars.Instance.MyButton(rc, string.Empty, "Left"))
		{
			if (waitingList)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("WAIT_A_MOMENT"));
			}
			else if (page <= 2)
			{
				selected = -1;
				waitingList = true;
				CSNetManager.Instance.Sock.SendCS_MY_REGISTER_MAP_REQ(-1, 1, 0, GlobalVars.Instance.getBattleMode(subTab));
			}
			else if (page > 2 && num >= 0)
			{
				selected = -1;
				waitingList = true;
				CSNetManager.Instance.Sock.SendCS_MY_REGISTER_MAP_REQ(page, page - 1, firstIndexer, GlobalVars.Instance.getBattleMode(subTab));
			}
		}
		Rect position = new Rect(crdPageBox);
		if (chatView)
		{
			position.y -= chatGap;
		}
		GUI.Box(position, string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(position.x + position.width / 2f, position.y + position.height / 2f), page.ToString(), "MiniLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		Rect rc2 = new Rect(crdRightBtn);
		if (chatView)
		{
			rc2.y -= chatGap;
		}
		if (GlobalVars.Instance.MyButton(rc2, string.Empty, "Right"))
		{
			if (waitingList)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("WAIT_A_MOMENT"));
			}
			else if (num2 >= 0)
			{
				selected = -1;
				waitingList = true;
				CSNetManager.Instance.Sock.SendCS_MY_REGISTER_MAP_REQ(page, page + 1, lastIndexer, GlobalVars.Instance.getBattleMode(subTab));
			}
		}
	}

	private void DrawMode(int modeMask)
	{
		Rect position = new Rect(crdSppModeText.x + crdModeTextSize.x + 6f, crdIconMode.y, crdIconMode.width, crdIconMode.height);
		if ((modeMask & 1) != 0)
		{
			TextureUtil.DrawTexture(position, GlobalVars.Instance.iconTeamMode, ScaleMode.StretchToFill);
			position.x += 38f;
		}
		if ((modeMask & 2) != 0)
		{
			TextureUtil.DrawTexture(position, GlobalVars.Instance.iconsurvivalMode, ScaleMode.StretchToFill);
			position.x += 38f;
		}
		if ((modeMask & 4) != 0)
		{
			TextureUtil.DrawTexture(position, GlobalVars.Instance.iconCTFMode, ScaleMode.StretchToFill);
			position.x += 38f;
		}
		if ((modeMask & 8) != 0)
		{
			TextureUtil.DrawTexture(position, GlobalVars.Instance.iconBlastMode, ScaleMode.StretchToFill);
			position.x += 38f;
		}
		if ((modeMask & 0x10) != 0)
		{
			TextureUtil.DrawTexture(position, GlobalVars.Instance.iconDefenseMode, ScaleMode.StretchToFill);
			position.x += 38f;
		}
		if ((modeMask & 0x20) != 0)
		{
			TextureUtil.DrawTexture(position, GlobalVars.Instance.iconBndMode, ScaleMode.StretchToFill);
			position.x += 38f;
		}
		if ((modeMask & 0x40) != 0)
		{
			TextureUtil.DrawTexture(position, GlobalVars.Instance.iconBungeeMode, ScaleMode.StretchToFill);
			position.x += 38f;
		}
		if ((modeMask & 0x80) != 0)
		{
			TextureUtil.DrawTexture(position, GlobalVars.Instance.iconEscapeMode, ScaleMode.StretchToFill);
			position.x += 38f;
		}
		if ((modeMask & 0x100) != 0)
		{
			TextureUtil.DrawTexture(position, GlobalVars.Instance.iconZombieMode, ScaleMode.StretchToFill);
			position.x += 38f;
		}
	}

	private void PrintMapInfo()
	{
		string text = StringMgr.Instance.Get("MAP_NAME_IS") + " " + StringMgr.Instance.Get("MAP_VERSION") + " " + reg[selected].Alias + reg[selected].Version;
		LabelUtil.TextOut(crdMapNameText, text, "MiniLabel", new Color(0.79f, 0.76f, 0.67f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		text = StringMgr.Instance.Get("LAST_MODIFIED_DATE") + " " + DateTimeLocal.ToString(reg[selected].RegisteredDate);
		LabelUtil.TextOut(crdLastDateText, text, "MiniLabel", new Color(0.79f, 0.76f, 0.67f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		text = StringMgr.Instance.Get("DOWNLOAD_COUNT") + ": " + reg[selected].DownloadCount;
		LabelUtil.TextOut(crdDnCountText, text, "MiniLabel", new Color(0.79f, 0.76f, 0.67f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		text = StringMgr.Instance.Get("MAP_EVAL") + ": ";
		LabelUtil.TextOut(crdMapEvalText, text, "MiniLabel", new Color(0.79f, 0.76f, 0.67f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		TextureUtil.DrawTexture(crdStarGrade, texStarGradeBg, ScaleMode.StretchToFill);
		Vector2 pos = new Vector2(crdStarGrade.x + 80f, crdStarGrade.y - 4f);
		text = "[ " + reg[selected].GetStarAvgString() + " ]";
		LabelUtil.TextOut(pos, text, "MiniLabel", new Color(0.79f, 0.76f, 0.67f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		float num = (float)reg[selected].Likes / 100f;
		TextureUtil.DrawTexture(new Rect(crdStarGrade.x, crdStarGrade.y, crdStarGrade.width * num, crdStarGrade.height), srcRect: new Rect(0f, 0f, num, 1f), image: texStarGrade);
		text = StringMgr.Instance.Get("SUPPORT_MODE") + ": ";
		crdModeTextSize = LabelUtil.TextOut(crdSppModeText, text, "MiniLabel", new Color(0.79f, 0.76f, 0.67f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
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
		DoModeSelector();
		DoPagePanel();
		int num = reg.Count / 4;
		if (reg.Count % 4 > 0)
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
				if (num2 < reg.Count)
				{
					Rect rect = new Rect((float)j * (crdMapSize.x + crdMapOffset.x), (float)i * (crdMapSize.y + crdMapOffset.y), crdMapSize.x, crdMapSize.y);
					Rect position = new Rect(rect.x, rect.y, rect.width, rect.width + 4f);
					if (reg[num2].Thumbnail != null)
					{
						if (!reg[num2].Blocked)
						{
							TextureUtil.DrawTexture(position, reg[num2].Thumbnail, ScaleMode.StretchToFill);
						}
						else
						{
							TextureUtil.DrawTexture(position, GlobalVars.Instance.iconBoxGray, ScaleMode.StretchToFill);
							float num3 = (float)GlobalVars.Instance.iconLockSlot.width;
							float num4 = (float)GlobalVars.Instance.iconLockSlot.height;
							float x = position.x + position.width / 2f - num3 / 2f;
							float y = position.y + position.height / 2f - num4 / 2f;
							TextureUtil.DrawTexture(new Rect(x, y, num3, num4), GlobalVars.Instance.iconLockSlot, ScaleMode.StretchToFill);
						}
					}
					if (GlobalVars.Instance.MyButton(rect, string.Empty, "BoxMapSelectBorder"))
					{
						selected = num2;
						if (reg[selected].Blocked)
						{
							MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NOTICE_BLOCK_MAP"));
						}
						if (Time.time - lastClickTime > doubleClickTimeout)
						{
							lastClickTime = Time.time;
						}
						else
						{
							((MapSettingChangeDlg)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.MAP_SETTING_CHANGE, exclusive: true))?.InitDialog(reg[selected]);
						}
					}
					DateTime registeredDate = reg[num2].RegisteredDate;
					if (registeredDate.Year == DateTime.Today.Year && registeredDate.Month == DateTime.Today.Month && registeredDate.Day == DateTime.Today.Day)
					{
						TextureUtil.DrawTexture(new Rect(rect.x, rect.y, (float)GlobalVars.Instance.iconNewmap.width, (float)GlobalVars.Instance.iconNewmap.height), GlobalVars.Instance.iconNewmap, ScaleMode.StretchToFill);
					}
					else if ((reg[num2].tagMask & 8) != 0)
					{
						TextureUtil.DrawTexture(new Rect(rect.x, rect.y, (float)GlobalVars.Instance.iconglory.width, (float)GlobalVars.Instance.iconglory.height), GlobalVars.Instance.iconglory, ScaleMode.StretchToFill);
					}
					else if ((reg[num2].tagMask & 4) != 0)
					{
						TextureUtil.DrawTexture(new Rect(rect.x, rect.y, (float)GlobalVars.Instance.iconMedal.width, (float)GlobalVars.Instance.iconMedal.height), GlobalVars.Instance.iconMedal, ScaleMode.StretchToFill);
					}
					else if ((reg[num2].tagMask & 2) != 0)
					{
						TextureUtil.DrawTexture(new Rect(rect.x, rect.y, (float)GlobalVars.Instance.icongoldRibbon.width, (float)GlobalVars.Instance.icongoldRibbon.height), GlobalVars.Instance.icongoldRibbon, ScaleMode.StretchToFill);
					}
					if (reg[num2].IsAbuseMap())
					{
						float x2 = rect.x + rect.width - (float)GlobalVars.Instance.iconDeclare.width;
						TextureUtil.DrawTexture(new Rect(x2, rect.y, (float)GlobalVars.Instance.iconDeclare.width, (float)GlobalVars.Instance.iconDeclare.height), GlobalVars.Instance.iconDeclare, ScaleMode.StretchToFill);
					}
					LabelUtil.TextOut(new Vector2(rect.x + crdAlias.x, rect.y + crdAlias.y), reg[num2].Alias, "MiniLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
					if (selected == num2)
					{
						TextureUtil.DrawTexture(rect, selectedMapFrame, ScaleMode.StretchToFill);
					}
				}
			}
		}
		GUI.EndScrollView();
		if (selected >= 0)
		{
			int num5 = 2;
			Rect rc = new Rect(crdBtns[num5]);
			if (chatView)
			{
				rc = crdBtns2[num5];
			}
			if (ChannelManager.Instance.CurChannel.Mode != 3 && RegMapManager.Instance.IsDownloaded(reg[selected].Map))
			{
				GUI.enabled = !reg[selected].Blocked;
				if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("CREATE_ROOM"), "BtnAction"))
				{
					CreateRoomDialog createRoomDialog = (CreateRoomDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.CREATE_ROOM, exclusive: true);
					if (createRoomDialog != null && !createRoomDialog.InitDialog4TeamMatch(reg[selected].Map, reg[selected].ModeMask))
					{
						DialogManager.Instance.Clear();
					}
				}
				GUI.enabled = true;
				num5--;
			}
			rc = crdBtns[num5];
			if (chatView)
			{
				rc = crdBtns2[num5];
			}
			if (RegMapManager.Instance.IsDownloaded(reg[selected].Map))
			{
				if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("DELETE"), "BtnAction"))
				{
					CSNetManager.Instance.Sock.SendCS_DEL_DOWNLOAD_MAP_REQ(reg[selected].Map);
					selected = -1;
				}
				num5--;
			}
			else
			{
				bool enabled = GUI.enabled;
				GUI.enabled = reg[selected].IsLatest;
				if (GUI.enabled)
				{
					GUI.enabled = !reg[selected].Blocked;
				}
				if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("SAVE"), "BtnAction"))
				{
					((DownloadFeeDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.DOWNLOAD_FEE, exclusive: true))?.InitDialog(reg[selected]);
				}
				num5--;
				GUI.enabled = enabled;
			}
			rc = crdBtns[num5];
			if (chatView)
			{
				rc = crdBtns2[num5];
			}
			GUI.enabled = !reg[selected].Blocked;
			if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("MAP_SETTING_CHG"), "BtnAction"))
			{
				((MapSettingChangeDlg)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.MAP_SETTING_CHANGE, exclusive: true))?.InitDialog(reg[selected]);
				selected = -1;
			}
			GUI.enabled = true;
		}
	}
}
