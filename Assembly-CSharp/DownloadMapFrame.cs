using System;
using UnityEngine;

[Serializable]
public class DownloadMapFrame
{
	public Texture2D nonAvailable;

	public float doubleClickTimeout = 0.2f;

	public Texture2D selectedMapFrame;

	public Texture2D mapFrame;

	public Texture2D texStarGrade;

	public Texture2D texStarGradeBg;

	private Vector2 scrollPosition = Vector2.zero;

	private int subTab;

	public int modeTab;

	private Rect crdLeftBtn = new Rect(542f, 724f, 22f, 18f);

	private Rect crdRightBtn = new Rect(691f, 724f, 22f, 18f);

	private Rect crdPageBox = new Rect(572f, 724f, 108f, 18f);

	private Vector2 crdMapSize = new Vector2(150f, 196f);

	private Vector2 crdMapOffset = new Vector2(35f, 21f);

	private Rect crdRegMapRect = new Rect(264f, 142f, 732f, 515f);

	private Rect crdRegMapRectTemp = new Rect(264f, 142f, 732f, 515f);

	private float chatGap = 275f;

	private Vector2 crdMapNameLabel = new Vector2(145f, 10f);

	private Vector2 crdMapNameVal = new Vector2(150f, 30f);

	private Vector2 crdLastModifiedLabel = new Vector2(145f, 55f);

	private Vector2 crdLastModifiedVal = new Vector2(150f, 75f);

	private Vector2 crdDownloadCountLabel = new Vector2(345f, 10f);

	private Vector2 crdDownloadCountVal = new Vector2(350f, 30f);

	private Vector2 crdMapEvalLabel = new Vector2(345f, 55f);

	private Rect crdStar = new Rect(350f, 75f, 74f, 14f);

	private Vector2 crdGradeText = new Vector2(435f, 75f);

	private Vector2 crdModeLabel = new Vector2(512f, 10f);

	private Rect crdModeIcon = new Rect(512f, 30f, 35f, 22f);

	private Vector2 crdAlias = new Vector2(5f, 174f);

	private Rect[] crdBtns;

	private Rect[] crdBtns2;

	private bool waitingList;

	private bool bUpdateList;

	public int selected;

	private float lastClickTime;

	private int page = 1;

	public int firstIndexer = -1;

	public int lastIndexer = -1;

	private int xCount = 3;

	private bool chatView;

	public void Start()
	{
		crdBtns = new Rect[2];
		crdBtns[0] = new Rect(715f, 714f, 139f, 38f);
		crdBtns[1] = new Rect(859f, 714f, 139f, 38f);
		crdBtns2 = new Rect[2];
		crdBtns2[0] = new Rect(715f, 714f - chatGap, 139f, 38f);
		crdBtns2[1] = new Rect(859f, 714f - chatGap, 139f, 38f);
		selected = 0;
		waitingList = true;
		CSNetManager.Instance.Sock.SendCS_MY_DOWNLOAD_MAP_REQ(-1, 1, 0, GlobalVars.Instance.getBattleMode(subTab));
	}

	public void BeginMapList(int curPage)
	{
		page = curPage;
		scrollPosition = Vector2.zero;
	}

	public void EndMapList()
	{
		waitingList = false;
	}

	public void SelectedTab(int tab)
	{
		int num = subTab;
		subTab = tab;
		if (num != subTab)
		{
			bUpdateList = true;
			page = 1;
			selected = 0;
		}
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
				CSNetManager.Instance.Sock.SendCS_MY_DOWNLOAD_MAP_REQ(-1, 1, 0, GlobalVars.Instance.getBattleMode(subTab));
			}
		}
	}

	private void DrawMode(int modeMask)
	{
		int num = 0;
		Rect position = new Rect(crdModeIcon);
		if ((modeMask & 1) != 0)
		{
			TextureUtil.DrawTexture(position, GlobalVars.Instance.iconTeamMode, ScaleMode.StretchToFill);
			num++;
		}
		if ((modeMask & 2) != 0)
		{
			int num2 = num % xCount;
			int num3 = num / xCount;
			position.x = crdModeIcon.x + (crdModeIcon.width + 5f) * (float)num2;
			position.y = crdModeIcon.y + (crdModeIcon.height + 5f) * (float)num3;
			TextureUtil.DrawTexture(position, GlobalVars.Instance.iconsurvivalMode, ScaleMode.StretchToFill);
			num++;
		}
		if ((modeMask & 4) != 0)
		{
			int num4 = num % xCount;
			int num5 = num / xCount;
			position.x = crdModeIcon.x + (crdModeIcon.width + 5f) * (float)num4;
			position.y = crdModeIcon.y + (crdModeIcon.height + 5f) * (float)num5;
			TextureUtil.DrawTexture(position, GlobalVars.Instance.iconCTFMode, ScaleMode.StretchToFill);
			num++;
		}
		if ((modeMask & 8) != 0)
		{
			int num6 = num % xCount;
			int num7 = num / xCount;
			position.x = crdModeIcon.x + (crdModeIcon.width + 5f) * (float)num6;
			position.y = crdModeIcon.y + (crdModeIcon.height + 5f) * (float)num7;
			TextureUtil.DrawTexture(position, GlobalVars.Instance.iconBlastMode, ScaleMode.StretchToFill);
			num++;
		}
		if ((modeMask & 0x10) != 0)
		{
			int num8 = num % xCount;
			int num9 = num / xCount;
			position.x = crdModeIcon.x + (crdModeIcon.width + 5f) * (float)num8;
			position.y = crdModeIcon.y + (crdModeIcon.height + 5f) * (float)num9;
			TextureUtil.DrawTexture(position, GlobalVars.Instance.iconDefenseMode, ScaleMode.StretchToFill);
			num++;
		}
		if ((modeMask & 0x20) != 0)
		{
			int num10 = num % xCount;
			int num11 = num / xCount;
			position.x = crdModeIcon.x + (crdModeIcon.width + 5f) * (float)num10;
			position.y = crdModeIcon.y + (crdModeIcon.height + 5f) * (float)num11;
			TextureUtil.DrawTexture(position, GlobalVars.Instance.iconBndMode, ScaleMode.StretchToFill);
			num++;
		}
		if ((modeMask & 0x40) != 0)
		{
			int num12 = num % xCount;
			int num13 = num / xCount;
			position.x = crdModeIcon.x + (crdModeIcon.width + 5f) * (float)num12;
			position.y = crdModeIcon.y + (crdModeIcon.height + 5f) * (float)num13;
			TextureUtil.DrawTexture(position, GlobalVars.Instance.iconBungeeMode, ScaleMode.StretchToFill);
			num++;
		}
		if ((modeMask & 0x80) != 0)
		{
			int num14 = num % xCount;
			int num15 = num / xCount;
			position.x = crdModeIcon.x + (crdModeIcon.width + 5f) * (float)num14;
			position.y = crdModeIcon.y + (crdModeIcon.height + 5f) * (float)num15;
			TextureUtil.DrawTexture(position, GlobalVars.Instance.iconEscapeMode, ScaleMode.StretchToFill);
			num++;
		}
		if ((modeMask & 0x100) != 0)
		{
			int num16 = num % xCount;
			int num17 = num / xCount;
			position.x = crdModeIcon.x + (crdModeIcon.width + 5f) * (float)num16;
			position.y = crdModeIcon.y + (crdModeIcon.height + 5f) * (float)num17;
			TextureUtil.DrawTexture(position, GlobalVars.Instance.iconZombieMode, ScaleMode.StretchToFill);
			num++;
		}
	}

	private void PrintMapInfo(RegMap reg)
	{
		Color txtMainColor = GlobalVars.Instance.txtMainColor;
		LabelUtil.TextOut(crdMapNameLabel, StringMgr.Instance.Get("MAP_NAME_IS") + " " + StringMgr.Instance.Get("MAP_VERSION"), "MiniLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMapNameVal, reg.Alias + reg.Version, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdLastModifiedLabel, StringMgr.Instance.Get("LAST_MODIFIED_DATE"), "MiniLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdLastModifiedVal, DateTimeLocal.ToString(reg.RegisteredDate), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdDownloadCountLabel, StringMgr.Instance.Get("DOWNLOAD_COUNT"), "MiniLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdDownloadCountVal, reg.DownloadCount.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMapEvalLabel, StringMgr.Instance.Get("MAP_EVAL"), "MiniLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		TextureUtil.DrawTexture(crdStar, texStarGradeBg, ScaleMode.StretchToFill);
		string text = "[ " + reg.GetStarAvgString() + " ]";
		LabelUtil.TextOut(crdGradeText, text, "MiniLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		float num = (float)reg.Likes / 100f;
		Rect position = new Rect(crdStar.x, crdStar.y, crdStar.width * num, crdStar.height);
		GUI.BeginGroup(position);
		TextureUtil.DrawTexture(new Rect(0f, 0f, crdStar.width, crdStar.height), texStarGrade);
		GUI.EndGroup();
		LabelUtil.TextOut(crdModeLabel, StringMgr.Instance.Get("SUPPORT_MODE"), "MiniLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
	}

	private void DoPagePanel(int length)
	{
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
			else if (page == 1)
			{
				selected = 0;
				waitingList = true;
				CSNetManager.Instance.Sock.SendCS_MY_DOWNLOAD_MAP_REQ(-1, 1, firstIndexer, GlobalVars.Instance.getBattleMode(subTab));
			}
			else
			{
				selected = 0;
				waitingList = true;
				CSNetManager.Instance.Sock.SendCS_MY_DOWNLOAD_MAP_REQ(page, page - 1, firstIndexer, GlobalVars.Instance.getBattleMode(subTab));
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
			else
			{
				selected = 0;
				waitingList = true;
				CSNetManager.Instance.Sock.SendCS_MY_DOWNLOAD_MAP_REQ(page, page + 1, lastIndexer, GlobalVars.Instance.getBattleMode(subTab));
			}
		}
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
		RegMap[] array = RegMapManager.Instance.ToArray(subTab, page);
		if (array.Length != 0)
		{
			DoPagePanel(array.Length);
			Texture2D[] array2 = new Texture2D[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Thumbnail == null)
				{
					array2[i] = nonAvailable;
				}
				else
				{
					array2[i] = array[i].Thumbnail;
				}
			}
			int num = array.Length / 4;
			if (array.Length % 4 > 0)
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
			for (int j = 0; j < num; j++)
			{
				for (int k = 0; k < 4; k++)
				{
					int num2 = 4 * j + k;
					if (num2 < array.Length)
					{
						Rect rect = new Rect((float)k * (crdMapSize.x + crdMapOffset.x), (float)j * (crdMapSize.y + crdMapOffset.y), crdMapSize.x, crdMapSize.y);
						Rect position = new Rect(rect.x, rect.y, rect.width, rect.width + 4f);
						if (!array[num2].Blocked)
						{
							TextureUtil.DrawTexture(position, array2[num2], ScaleMode.StretchToFill);
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
						string str = "BoxMapSelectBorder";
						if (GlobalVars.Instance.MyButton(rect, string.Empty, str))
						{
							selected = num2;
							if (array[selected].Blocked)
							{
								MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NOTICE_BLOCK_MAP"));
							}
							if (Time.time - lastClickTime > doubleClickTimeout)
							{
								lastClickTime = Time.time;
							}
							else
							{
								((MapDetailDlg)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.MAP_DETAIL, exclusive: true))?.InitDialog(array[selected]);
							}
						}
						DateTime registeredDate = array[num2].RegisteredDate;
						if (registeredDate.Year == DateTime.Today.Year && registeredDate.Month == DateTime.Today.Month && registeredDate.Day == DateTime.Today.Day)
						{
							TextureUtil.DrawTexture(new Rect(rect.x, rect.y, (float)GlobalVars.Instance.iconNewmap.width, (float)GlobalVars.Instance.iconNewmap.height), GlobalVars.Instance.iconNewmap, ScaleMode.StretchToFill);
						}
						else if ((array[num2].tagMask & 8) != 0)
						{
							TextureUtil.DrawTexture(new Rect(rect.x, rect.y, (float)GlobalVars.Instance.iconglory.width, (float)GlobalVars.Instance.iconglory.height), GlobalVars.Instance.iconglory, ScaleMode.StretchToFill);
						}
						else if ((array[num2].tagMask & 4) != 0)
						{
							TextureUtil.DrawTexture(new Rect(rect.x, rect.y, (float)GlobalVars.Instance.iconMedal.width, (float)GlobalVars.Instance.iconMedal.height), GlobalVars.Instance.iconMedal, ScaleMode.StretchToFill);
						}
						else if ((array[num2].tagMask & 2) != 0)
						{
							TextureUtil.DrawTexture(new Rect(rect.x, rect.y, (float)GlobalVars.Instance.icongoldRibbon.width, (float)GlobalVars.Instance.icongoldRibbon.height), GlobalVars.Instance.icongoldRibbon, ScaleMode.StretchToFill);
						}
						if (array[num2].IsAbuseMap())
						{
							float x2 = rect.x + rect.width - (float)GlobalVars.Instance.iconDeclare.width;
							TextureUtil.DrawTexture(new Rect(x2, rect.y, (float)GlobalVars.Instance.iconDeclare.width, (float)GlobalVars.Instance.iconDeclare.height), GlobalVars.Instance.iconDeclare, ScaleMode.StretchToFill);
						}
						LabelUtil.TextOut(new Vector2(rect.x + crdAlias.x, rect.y + crdAlias.y), array[num2].Alias, "MiniLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
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
				int num5 = 1;
				Rect rc = new Rect(crdBtns[num5]);
				if (chatView)
				{
					rc = crdBtns2[num5];
				}
				GUI.enabled = !array[selected].Blocked;
				GUIContent content = new GUIContent(StringMgr.Instance.Get("CREATE_ROOM").ToUpper(), GlobalVars.Instance.iconJoin);
				if (ChannelManager.Instance.CurChannel.Mode != 3 && GlobalVars.Instance.MyButton3(rc, content, "BtnAction"))
				{
					CreateRoomDialog createRoomDialog = (CreateRoomDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.CREATE_ROOM, exclusive: true);
					if (createRoomDialog != null && !createRoomDialog.InitDialog4TeamMatch(array[selected].Map, array[selected].ModeMask))
					{
						DialogManager.Instance.Clear();
					}
				}
				num5--;
				GUI.enabled = true;
				rc = crdBtns[num5];
				if (chatView)
				{
					rc = crdBtns2[num5];
				}
				content = new GUIContent(StringMgr.Instance.Get("DELETE").ToUpper(), GlobalVars.Instance.iconGarbage);
				if (GlobalVars.Instance.MyButton3(rc, content, "BtnAction"))
				{
					CSNetManager.Instance.Sock.SendCS_DEL_DOWNLOAD_MAP_REQ(array[selected].Map);
					selected = 0;
				}
			}
		}
	}
}
