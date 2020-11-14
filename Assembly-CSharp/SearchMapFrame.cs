using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SearchMapFrame
{
	public int BoardIndex;

	public Texture2D nonAvailable;

	public Texture2D selectedMapFrame;

	public Texture2D mapFrame;

	public Texture2D texDownloaded;

	public float doubleClickTimeout = 0.2f;

	private int selMain;

	private int subTab;

	private int page;

	private List<RegMap> reg;

	private Vector2 scrollPosition = Vector2.zero;

	private sbyte lastFlag;

	private string lastFilter = string.Empty;

	private string searchKey = string.Empty;

	private bool isMapAlias = true;

	private bool isDeveloper;

	private bool waitingList;

	public Texture2D searchIcon;

	public Texture2D resetIcon;

	private Vector2 crdToday = new Vector2(600f, 115f);

	private Rect crdSearchBtn = new Rect(900f, 102f, 34f, 34f);

	private Rect crdRefreshBtn = new Rect(940f, 105f, 28f, 28f);

	private Rect crdKeyTxtFld = new Rect(680f, 106f, 210f, 26f);

	private Rect crdFrame = new Rect(255f, 142f, 762f, 515f);

	private Rect crdFrameTemp = new Rect(255f, 142f, 762f, 515f);

	private Vector2 crdMapBtn = new Vector2(742f, 142f);

	private float crdMapOffset = 10f;

	private Rect crdThumbnail = new Rect(6f, 6f, 128f, 128f);

	private float chatGap = 275f;

	private Rect crdLeftBtn = new Rect(540f, 724f, 22f, 18f);

	private Rect crdRightBtn = new Rect(691f, 724f, 22f, 18f);

	private Rect crdPageBox = new Rect(572f, 724f, 108f, 18f);

	private Vector2 crdDeveloperVal = new Vector2(680f, 10f);

	private Vector2 crdMapAliasLabel = new Vector2(145f, 50f);

	private Vector2 crdMapAliasVal = new Vector2(145f, 10f);

	private Vector2 crdLastModifiedLabel = new Vector2(145f, 70f);

	private Vector2 crdSupportModeLabel = new Vector2(145f, 90f);

	private Rect crdModeIcon = new Rect(145f, 110f, 35f, 22f);

	private Rect crdThumbUp = new Rect(430f, 50f, 22f, 22f);

	private Rect crdThumbDn = new Rect(530f, 50f, 22f, 22f);

	private Rect crdSave = new Rect(630f, 50f, 22f, 22f);

	private Rect crdDownloaded = new Rect(99f, 99f, 30f, 30f);

	private Rect crdDetailBtn = new Rect(715f, 714f, 139f, 38f);

	private Rect crdSaveBtn = new Rect(859f, 714f, 139f, 38f);

	public int selected;

	public int firstIndexer = -1;

	public int lastIndexer = -1;

	public bool bGuiEnable;

	public bool bUpdateList;

	public int selSearch;

	public int year4Week;

	public int month4Week;

	public int day4Week;

	public int chartSeq;

	public int firstHonor = -1;

	public int lastHonor = -1;

	private float lastClickTime;

	private int xCount = 30;

	private bool chatView;

	public void SelectedMainTab(int tab)
	{
		int num = selMain;
		selMain = tab;
		if (num != selMain)
		{
			bUpdateList = true;
			BeginMapList(0);
			selected = 0;
		}
	}

	public void SelectedTab(int tab)
	{
		int num = subTab;
		subTab = tab;
		if (num != subTab)
		{
			bUpdateList = true;
			BeginMapList(0);
			selected = 0;
		}
	}

	public void Start()
	{
		selMain = 0;
		reg = new List<RegMap>();
		page = 1;
		waitingList = true;
		selected = 0;
		CSNetManager.Instance.Sock.SendCS_ALL_MAP_REQ(-1, 1, 0, GlobalVars.Instance.getBattleMode(subTab), 0, string.Empty);
	}

	public void BeginMapList(int curPage)
	{
		page = curPage;
		reg.Clear();
		scrollPosition = Vector2.zero;
	}

	public void AddMapItem(RegMap regMap)
	{
		reg.Add(regMap);
	}

	public void EndMapList()
	{
		waitingList = false;
	}

	private string GetLastFilter()
	{
		if (lastFilter.Length <= 0)
		{
			return string.Empty;
		}
		return "%" + lastFilter + "%";
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
		if (bUpdateList)
		{
			bUpdateList = false;
			if (waitingList)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("WAIT_A_MOMENT"));
			}
			else
			{
				waitingList = true;
				ResetSearch();
				if (selMain == 0)
				{
					CSNetManager.Instance.Sock.SendCS_ALL_MAP_REQ(-1, 1, 0, GlobalVars.Instance.getBattleMode(subTab), lastFlag, GetLastFilter());
				}
				else if (selMain == 1)
				{
					CSNetManager.Instance.Sock.SendCS_MAP_DAY_REQ(-1, 1, 0);
				}
				else if (selMain == 2)
				{
					CSNetManager.Instance.Sock.SendCS_WEEKLY_CHART_REQ(-1, 1, 0);
				}
				else if (selMain == 3)
				{
					CSNetManager.Instance.Sock.SendCS_MAP_HONOR_REQ(-1, 1, 0);
				}
			}
		}
	}

	private void ResetSearch()
	{
		if (lastFlag == 1)
		{
			isMapAlias = true;
			isDeveloper = false;
		}
		else if (lastFlag == 2)
		{
			isMapAlias = false;
			isDeveloper = true;
		}
		searchKey = lastFilter;
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
				selected = 0;
				waitingList = true;
				if (selMain == 0)
				{
					CSNetManager.Instance.Sock.SendCS_ALL_MAP_REQ(-1, 1, firstIndexer, GlobalVars.Instance.getBattleMode(subTab), lastFlag, GetLastFilter());
				}
				else if (selMain == 1)
				{
					CSNetManager.Instance.Sock.SendCS_MAP_DAY_REQ(-1, 1, firstHonor);
				}
				else if (selMain == 2)
				{
					CSNetManager.Instance.Sock.SendCS_WEEKLY_CHART_REQ(-1, 1, chartSeq);
				}
				else if (selMain == 3)
				{
					CSNetManager.Instance.Sock.SendCS_MAP_HONOR_REQ(-1, 1, firstHonor);
				}
			}
			else if (page > 2 && num >= 0)
			{
				selected = 0;
				waitingList = true;
				if (selMain == 0)
				{
					CSNetManager.Instance.Sock.SendCS_ALL_MAP_REQ(page, page - 1, firstIndexer, GlobalVars.Instance.getBattleMode(subTab), lastFlag, GetLastFilter());
				}
				else if (selMain == 1)
				{
					CSNetManager.Instance.Sock.SendCS_MAP_DAY_REQ(page, page - 1, firstHonor);
				}
				else if (selMain == 2)
				{
					CSNetManager.Instance.Sock.SendCS_WEEKLY_CHART_REQ(page, page - 1, chartSeq);
				}
				else if (selMain == 3)
				{
					CSNetManager.Instance.Sock.SendCS_MAP_HONOR_REQ(page, page - 1, firstHonor);
				}
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
				selected = 0;
				waitingList = true;
				if (selMain == 0)
				{
					CSNetManager.Instance.Sock.SendCS_ALL_MAP_REQ(page, page + 1, lastIndexer, GlobalVars.Instance.getBattleMode(subTab), lastFlag, GetLastFilter());
				}
				else if (selMain == 1)
				{
					CSNetManager.Instance.Sock.SendCS_MAP_DAY_REQ(page, page + 1, lastHonor);
				}
				else if (selMain == 2)
				{
					CSNetManager.Instance.Sock.SendCS_WEEKLY_CHART_REQ(page, page + 1, chartSeq);
				}
				else if (selMain == 3)
				{
					CSNetManager.Instance.Sock.SendCS_MAP_HONOR_REQ(page, page + 1, lastHonor);
				}
			}
		}
	}

	private void DoSearch()
	{
		if (selMain <= 0)
		{
			bool flag = isMapAlias;
			isMapAlias = ((selSearch == 0) ? true : false);
			if (flag != isMapAlias)
			{
				if (flag)
				{
					isMapAlias = flag;
				}
				else
				{
					isDeveloper = false;
				}
			}
			flag = isDeveloper;
			isDeveloper = ((selSearch == 1) ? true : false);
			if (flag != isDeveloper)
			{
				if (flag)
				{
					isDeveloper = flag;
				}
				else
				{
					isMapAlias = false;
				}
			}
			if (GlobalVars.Instance.MyButton(crdSearchBtn, string.Empty, "BtnDetail"))
			{
				searchKey.Trim();
				if (waitingList)
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("WAIT_A_MOMENT"));
				}
				else if (searchKey.Length < 2)
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CHARACTERS2MORE"));
				}
				else
				{
					lastFlag = (sbyte)(isMapAlias ? 1 : 2);
					lastFilter = searchKey;
					waitingList = true;
					CSNetManager.Instance.Sock.SendCS_ALL_MAP_REQ(-1, 1, 0, GlobalVars.Instance.getBattleMode(subTab), lastFlag, GetLastFilter());
				}
			}
			if (GlobalVars.Instance.MyButton(crdRefreshBtn, string.Empty, "BtnRefresh"))
			{
				searchKey.Trim();
				if (waitingList)
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("WAIT_A_MOMENT"));
				}
				else
				{
					lastFlag = 0;
					lastFilter = string.Empty;
					searchKey = string.Empty;
					isMapAlias = true;
					isDeveloper = false;
					waitingList = true;
					subTab = 0;
					selected = 0;
					CSNetManager.Instance.Sock.SendCS_ALL_MAP_REQ(-1, 1, 0, GlobalVars.Instance.getBattleMode(subTab), lastFlag, GetLastFilter());
				}
			}
			GUI.SetNextControlName("SearchKeyInput");
			string text = searchKey;
			searchKey = GUI.TextField(crdKeyTxtFld, searchKey);
			if (searchKey.Length > 10)
			{
				searchKey = text;
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
		LabelUtil.TextOut(crdMapAliasVal, reg.Alias, "SubTitleLabel", txtMainColor, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdDeveloperVal, StringMgr.Instance.Get("DEVELOPER") + " : " + reg.Developer, "SubTitleLabel", txtMainColor, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperRight);
		LabelUtil.TextOut(crdMapAliasLabel, StringMgr.Instance.Get("VERSIONINFO") + string.Empty + StringMgr.Instance.Get("MAP_VERSION") + " : " + reg.Version, "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdLastModifiedLabel, StringMgr.Instance.Get("LAST_MODIFIED_DATE") + " : " + DateTimeLocal.ToString(reg.RegisteredDate), "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdSupportModeLabel, StringMgr.Instance.Get("SUPPORT_MODE"), "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		TextureUtil.DrawTexture(crdThumbUp, GlobalVars.Instance.iconThumbUp, ScaleMode.StretchToFill);
		LabelUtil.TextOut(new Vector2(crdThumbUp.x + 30f, crdThumbUp.y), reg.Likes.ToString(), "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		TextureUtil.DrawTexture(crdThumbDn, GlobalVars.Instance.iconThumbDn, ScaleMode.StretchToFill);
		LabelUtil.TextOut(new Vector2(crdThumbDn.x + 30f, crdThumbDn.y), reg.DisLikes.ToString(), "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		TextureUtil.DrawTexture(crdSave, GlobalVars.Instance.iconSave, ScaleMode.StretchToFill);
		LabelUtil.TextOut(new Vector2(crdSave.x + 30f, crdSave.y), reg.DownloadCount.ToString(), "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
	}

	private string CalcStartDate(int year, int mon, int day)
	{
		string empty = string.Empty;
		bool flag = true;
		int[] array = new int[12]
		{
			31,
			28,
			31,
			30,
			31,
			30,
			31,
			31,
			30,
			31,
			30,
			31
		};
		if ((year % 4 == 0 && year % 10 != 0) || year % 400 == 0)
		{
			array[1] = 29;
		}
		if (day - 7 < 1)
		{
			flag = false;
			mon--;
			if (mon < 1)
			{
				year--;
				mon = 12;
			}
		}
		int num = (!flag) ? (array[mon - 1] + day - 7) : (day - 7);
		return year.ToString() + "." + mon.ToString("0#") + "." + num.ToString("0#");
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
		if (!bGuiEnable)
		{
			GUI.enabled = false;
		}
		if (selMain == 1)
		{
			LabelUtil.TextOut(crdToday, "[ " + DateTime.Now.ToString("yyyy-MM-dd") + " ]", "SubTitleLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperCenter);
		}
		else if (selMain == 2)
		{
			string text = CalcStartDate(year4Week, month4Week, day4Week);
			string text2 = year4Week.ToString() + "." + month4Week.ToString("0#") + "." + day4Week.ToString("0#");
			LabelUtil.TextOut(crdToday, "[ " + text + " ~ " + text2 + " ]", "SubTitleLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperCenter);
		}
		DoPagePanel();
		DoModeSelector();
		DoSearch();
		if (reg.Count == 0)
		{
			if (!bGuiEnable)
			{
				GUI.enabled = true;
			}
		}
		else
		{
			Texture2D[] array = new Texture2D[reg.Count];
			for (int i = 0; i < reg.Count; i++)
			{
				if (reg[i].Thumbnail == null)
				{
					array[i] = nonAvailable;
				}
				else
				{
					array[i] = reg[i].Thumbnail;
				}
			}
			float num = crdMapBtn.y * (float)reg.Count;
			if (reg.Count > 0)
			{
				num += (float)(reg.Count - 1) * crdMapOffset;
			}
			VerifyChatView();
			if (chatView)
			{
				crdFrame.height = 300f;
			}
			else
			{
				crdFrame.height = crdFrameTemp.height;
			}
			scrollPosition = GUI.BeginScrollView(viewRect: new Rect(0f, 0f, crdFrame.width - 20f, num), position: crdFrame, scrollPosition: scrollPosition, alwaysShowHorizontal: false, alwaysShowVertical: false);
			Vector2 vector = new Vector2(0f, 0f);
			for (int j = 0; j < reg.Count; j++)
			{
				Rect position = new Rect(vector.x, vector.y, crdMapBtn.x, crdMapBtn.y);
				GUI.BeginGroup(position);
				if (GlobalVars.Instance.MyButton(new Rect(0f, 0f, crdMapBtn.x, crdMapBtn.y), string.Empty, "BtnMapList"))
				{
					selected = j;
					if (Time.time - lastClickTime > doubleClickTimeout)
					{
						lastClickTime = Time.time;
					}
					else
					{
						((MapDetailDlg)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.MAP_DETAIL, exclusive: true))?.InitDialog(reg[selected]);
					}
				}
				if (selected == j)
				{
					GUI.Box(new Rect(0f, 0f, crdMapBtn.x, crdMapBtn.y), string.Empty, "ViewSelected");
				}
				if (!reg[j].Blocked)
				{
					TextureUtil.DrawTexture(crdThumbnail, array[j], ScaleMode.StretchToFill);
				}
				else
				{
					TextureUtil.DrawTexture(crdThumbnail, GlobalVars.Instance.iconBoxGray, ScaleMode.StretchToFill);
					float num2 = (float)GlobalVars.Instance.iconLockSlot.width;
					float num3 = (float)GlobalVars.Instance.iconLockSlot.height;
					float x = crdThumbnail.x + crdThumbnail.width / 2f - num2 / 2f;
					float y = crdThumbnail.y + crdThumbnail.height / 2f - num3 / 2f;
					TextureUtil.DrawTexture(new Rect(x, y, num2, num3), GlobalVars.Instance.iconLockSlot, ScaleMode.StretchToFill);
				}
				if (reg[j].Alias.Length > 0)
				{
					PrintMapInfo(reg[j]);
					DrawMode(reg[j].ModeMask);
				}
				if (RegMapManager.Instance.IsDownloaded(reg[j].Map))
				{
					TextureUtil.DrawTexture(crdDownloaded, texDownloaded, ScaleMode.StretchToFill);
				}
				DateTime registeredDate = reg[j].RegisteredDate;
				if (registeredDate.Year == DateTime.Today.Year && registeredDate.Month == DateTime.Today.Month && registeredDate.Day == DateTime.Today.Day)
				{
					TextureUtil.DrawTexture(new Rect(0f, 0f, (float)GlobalVars.Instance.iconNewmap.width, (float)GlobalVars.Instance.iconNewmap.height), GlobalVars.Instance.iconNewmap, ScaleMode.StretchToFill);
				}
				else if ((reg[j].tagMask & 8) != 0)
				{
					TextureUtil.DrawTexture(new Rect(0f, 0f, (float)GlobalVars.Instance.iconglory.width, (float)GlobalVars.Instance.iconglory.height), GlobalVars.Instance.iconglory, ScaleMode.StretchToFill);
				}
				else if ((reg[j].tagMask & 4) != 0)
				{
					TextureUtil.DrawTexture(new Rect(0f, 0f, (float)GlobalVars.Instance.iconMedal.width, (float)GlobalVars.Instance.iconMedal.height), GlobalVars.Instance.iconMedal, ScaleMode.StretchToFill);
				}
				else if ((reg[j].tagMask & 2) != 0)
				{
					TextureUtil.DrawTexture(new Rect(0f, 0f, (float)GlobalVars.Instance.icongoldRibbon.width, (float)GlobalVars.Instance.icongoldRibbon.height), GlobalVars.Instance.icongoldRibbon, ScaleMode.StretchToFill);
				}
				if (reg[j].IsAbuseMap())
				{
					float x2 = crdDownloaded.x + crdDownloaded.width - (float)GlobalVars.Instance.iconDeclare.width;
					TextureUtil.DrawTexture(new Rect(x2, 0f, (float)GlobalVars.Instance.iconDeclare.width, (float)GlobalVars.Instance.iconDeclare.height), GlobalVars.Instance.iconDeclare, ScaleMode.StretchToFill);
				}
				if (reg[j].IsLatest && !reg[j].Blocked)
				{
					GUI.Box(new Rect(500f, 80f, 158f, 50f), string.Empty, "BoxFadeBlue");
					LabelUtil.TextOut(new Vector2(579f, 100f), StringMgr.Instance.Get("DOWNLOAD_FEE"), "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.MiddleCenter);
					if (BuildOption.Instance.Props.useBrickPoint)
					{
						LabelUtil.TextOut(new Vector2(579f, 120f), "( " + reg[j].DownloadFee.ToString() + " BP )", "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.MiddleCenter);
					}
					else
					{
						LabelUtil.TextOut(new Vector2(579f, 120f), "( " + reg[j].DownloadFee.ToString() + " FP )", "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.MiddleCenter);
					}
				}
				else if (reg[j].Blocked)
				{
					LabelUtil.TextOut(new Vector2(579f, 110f), StringMgr.Instance.Get("NOTICE_BLOCK_MAP"), "MissionLabel", Color.red, new Color(0f, 0f, 0f, 0f), TextAnchor.MiddleCenter);
				}
				else if (!reg[j].IsLatest)
				{
					LabelUtil.TextOut(new Vector2(579f, 110f), StringMgr.Instance.Get("NOTICE_PREV_MAP"), "MissionLabel", Color.red, new Color(0f, 0f, 0f, 0f), TextAnchor.MiddleCenter);
				}
				GUI.EndGroup();
				vector.y += crdMapBtn.y + crdMapOffset;
			}
			GUI.EndScrollView();
			if (selected >= 0 && selected < reg.Count)
			{
				Rect rc = new Rect(crdDetailBtn);
				if (chatView)
				{
					rc.y -= chatGap;
				}
				Rect rc2 = new Rect(crdSaveBtn);
				if (chatView)
				{
					rc2.y -= chatGap;
				}
				GUIContent content = new GUIContent(StringMgr.Instance.Get("DETAIL_VIEW").ToUpper(), GlobalVars.Instance.iconDetail);
				if (GlobalVars.Instance.MyButton3(rc, content, "BtnAction"))
				{
					((MapDetailDlg)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.MAP_DETAIL, exclusive: true))?.InitDialog(reg[selected]);
				}
				if (RegMapManager.Instance.IsDownloaded(reg[selected].Map))
				{
					content = new GUIContent(StringMgr.Instance.Get("DELETE").ToUpper(), GlobalVars.Instance.iconGarbage);
					if (GlobalVars.Instance.MyButton3(rc2, content, "BtnAction"))
					{
						CSNetManager.Instance.Sock.SendCS_DEL_DOWNLOAD_MAP_REQ(reg[selected].Map);
					}
				}
				else
				{
					bool enabled = GUI.enabled;
					if (enabled)
					{
						GUI.enabled = reg[selected].IsLatest;
					}
					if (GUI.enabled)
					{
						GUI.enabled = !reg[selected].Blocked;
					}
					content = new GUIContent(StringMgr.Instance.Get("DOWNLOAD").ToUpper(), GlobalVars.Instance.iconDisk);
					if (GlobalVars.Instance.MyButton3(rc2, content, "BtnAction"))
					{
						((MapDetailDlg)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.MAP_DETAIL, exclusive: true))?.InitDialog(reg[selected]);
					}
					if (enabled)
					{
						GUI.enabled = enabled;
					}
				}
			}
			if (!bGuiEnable)
			{
				GUI.enabled = true;
			}
		}
	}
}
