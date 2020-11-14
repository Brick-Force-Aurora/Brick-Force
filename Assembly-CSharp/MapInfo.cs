using System;
using UnityEngine;

[Serializable]
public class MapInfo
{
	public string[] cboxMainNames;

	public string[] cboxMyMapNames;

	public string[] cboxMapSubNames;

	public string[] cboxMapSearchNames;

	public string[] cboxMainNamesReal;

	public string[] cboxMyMapNamesReal;

	public string[] cboxMapSubNamesReal;

	public string[] cboxMapSearchNamesReal;

	private string selectedMainName;

	private string selectedMyMapName;

	private string selectedSubName;

	private string selectedSearchName;

	public ComboBox cboxMain;

	private GUIContent[] listMainContent;

	private Rect crdCBoxMain = new Rect(260f, 75f, 160f, 25f);

	private Rect crdTotalMap = new Rect(35f, 131f, 180f, 28f);

	private Rect crdTodayMap = new Rect(35f, 168f, 180f, 28f);

	private Rect crdWeeklyMap = new Rect(35f, 205f, 180f, 28f);

	private Rect crdFameMap = new Rect(35f, 242f, 180f, 28f);

	private Rect crdMyMap = new Rect(35f, 590f, 180f, 28f);

	private Rect crdMyRegMap = new Rect(35f, 627f, 180f, 28f);

	private Rect crdMyDownloadMap = new Rect(35f, 664f, 180f, 28f);

	public ComboBox cboxSub;

	public ComboBox cboxSearch;

	private GUIContent[] listMyMapContent;

	private GUIContent[] listSubContent;

	private GUIContent[] listSearchContent;

	private Rect crdCBoxSub = new Rect(251f, 106f, 160f, 25f);

	private Rect crdCBoxSearch = new Rect(501f, 106f, 160f, 25f);

	private MyMapFrame myMapFrm;

	private SearchMapFrame searchMapFrm;

	private int selMain;

	private int selSub;

	private int selSearch;

	private bool bGuiEnable = true;

	private Rect crdSelectedOption = new Rect(0f, 0f, 0f, 0f);

	public Tooltip tooltip;

	private string lastTooltip = string.Empty;

	private float focusTime;

	private string tooltipMessage = string.Empty;

	public void Start()
	{
		Lobby lobby = null;
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			lobby = gameObject.GetComponent<Lobby>();
			if (null != lobby)
			{
				myMapFrm = lobby.myMapFrm;
				searchMapFrm = lobby.searchMapFrm;
			}
		}
		int num = 0;
		cboxMainNamesReal = new string[cboxMainNames.Length];
		for (num = 0; num < cboxMainNames.Length; num++)
		{
			cboxMainNamesReal[num] = StringMgr.Instance.Get(cboxMainNames[num]);
		}
		cboxMyMapNamesReal = new string[cboxMyMapNames.Length];
		for (num = 0; num < cboxMyMapNames.Length; num++)
		{
			cboxMyMapNamesReal[num] = StringMgr.Instance.Get(cboxMyMapNames[num]);
		}
		cboxMapSubNamesReal = new string[cboxMapSubNames.Length];
		for (num = 0; num < cboxMapSubNames.Length; num++)
		{
			cboxMapSubNamesReal[num] = StringMgr.Instance.Get(cboxMapSubNames[num]);
		}
		cboxMapSearchNamesReal = new string[cboxMapSearchNames.Length];
		for (num = 0; num < cboxMapSearchNames.Length; num++)
		{
			cboxMapSearchNamesReal[num] = StringMgr.Instance.Get(cboxMapSearchNames[num]);
		}
		cboxMain = new ComboBox();
		cboxMain.Initialize(bImage: false, new Vector2(crdCBoxMain.width, crdCBoxMain.height));
		listMainContent = new GUIContent[cboxMainNamesReal.Length];
		for (num = 0; num < cboxMainNamesReal.Length; num++)
		{
			listMainContent[num] = new GUIContent(cboxMainNamesReal[num]);
		}
		cboxSub = new ComboBox();
		cboxSub.Initialize(bImage: false, new Vector2(crdCBoxSub.width, crdCBoxSub.height));
		cboxSub.setStyleNames("BoxFilterBg", "BtnArrowDn", "BtnArrowUp", "BoxFilterCombo");
		cboxSearch = new ComboBox();
		cboxSearch.Initialize(bImage: false, new Vector2(crdCBoxSearch.width, crdCBoxSearch.height));
		cboxSearch.setStyleNames("BoxFilterBg", "BtnArrowDn", "BtnArrowUp", "BoxFilterCombo");
		listMyMapContent = new GUIContent[cboxMyMapNamesReal.Length];
		for (num = 0; num < cboxMyMapNamesReal.Length; num++)
		{
			listMyMapContent[num] = new GUIContent(cboxMyMapNamesReal[num]);
		}
		checkBattleModes();
		listSearchContent = new GUIContent[cboxMapSearchNamesReal.Length];
		for (num = 0; num < cboxMapSearchNamesReal.Length; num++)
		{
			listSearchContent[num] = new GUIContent(cboxMapSearchNamesReal[num]);
		}
	}

	public void checkBattleModes()
	{
		listSubContent = new GUIContent[BuildOption.Instance.Props.SupportModeCount];
		GlobalVars.Instance.allocBattleMode(10);
		int num = 0;
		for (int i = 0; i < cboxMapSubNamesReal.Length; i++)
		{
			if (BuildOption.Instance.Props.IsSupportMode((Room.ROOM_TYPE)i))
			{
				listSubContent[num] = new GUIContent(cboxMapSubNamesReal[i]);
				GlobalVars.Instance.setBattleMode(num, Room.modeSelector[i]);
				num++;
			}
		}
	}

	private void CheckGUIEnable()
	{
		bool flag = cboxMain.IsClickedComboButton();
		bool flag2 = cboxSub.IsClickedComboButton();
		bool flag3 = cboxSearch.IsClickedComboButton();
		if (!flag && !flag2 && !flag3)
		{
			bGuiEnable = true;
		}
		else
		{
			bGuiEnable = false;
		}
	}

	private void OptionSelector()
	{
		if (GlobalVars.Instance.MyButton(crdTotalMap, StringMgr.Instance.Get("TOTAL_MAP"), "BtnMapMgr"))
		{
			selMain = 0;
			searchMapFrm.SelectedMainTab(selMain);
			crdSelectedOption = crdTotalMap;
		}
		if (GlobalVars.Instance.MyButton(crdTodayMap, StringMgr.Instance.Get("TODAY_POP_MAP"), "BtnMapMgr"))
		{
			selMain = 1;
			searchMapFrm.SelectedMainTab(selMain);
			crdSelectedOption = crdTodayMap;
		}
		if (GlobalVars.Instance.MyButton(crdWeeklyMap, StringMgr.Instance.Get("WEEKLY_POP_MAP"), "BtnMapMgr"))
		{
			selMain = 2;
			searchMapFrm.SelectedMainTab(selMain);
			crdSelectedOption = crdWeeklyMap;
		}
		if (GlobalVars.Instance.MyButton(crdFameMap, StringMgr.Instance.Get("HALL_OF_FAME"), "BtnMapMgr"))
		{
			selMain = 3;
			searchMapFrm.SelectedMainTab(selMain);
			crdSelectedOption = crdFameMap;
		}
		if (GlobalVars.Instance.MyButton(crdMyMap, StringMgr.Instance.Get("MY_MAP_SLOT"), "BtnMapMgr"))
		{
			selMain = 4;
			searchMapFrm.SelectedMainTab(selMain);
			crdSelectedOption = crdMyMap;
		}
		if (GlobalVars.Instance.MyButton(crdMyRegMap, StringMgr.Instance.Get("MY_REG_MAP"), "BtnMapMgr"))
		{
			selMain = 5;
			searchMapFrm.SelectedMainTab(selMain);
			crdSelectedOption = crdMyRegMap;
		}
		if (GlobalVars.Instance.MyButton(crdMyDownloadMap, StringMgr.Instance.Get("MY_DOWNLOAD_MAP"), "BtnMapMgr"))
		{
			selMain = 6;
			searchMapFrm.SelectedMainTab(selMain);
			crdSelectedOption = crdMyDownloadMap;
		}
		if (selMain == 0)
		{
			crdSelectedOption = crdTotalMap;
		}
		GUI.Box(crdSelectedOption, string.Empty, "BoxMapMgrF");
	}

	private void SelectorTitle()
	{
		string empty = string.Empty;
		Vector2 zero = Vector2.zero;
		if (selMain == 0)
		{
			empty = StringMgr.Instance.Get("MAP_MANAGEMENT").ToUpper() + " - " + StringMgr.Instance.Get("TOTAL_MAP");
			zero = new Vector2(30f, 80f);
			LabelUtil.TextOut(zero, empty, "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			Vector2 vector = LabelUtil.CalcLength("BigLabel", empty);
			LabelUtil.TextOut(new Vector2(vector.x + 40f, vector.y + 60f), StringMgr.Instance.Get("DETAIL_ALLMAP"), "MidLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		else if (selMain == 1)
		{
			empty = StringMgr.Instance.Get("MAP_MANAGEMENT").ToUpper() + " - " + StringMgr.Instance.Get("TODAY_POP_MAP");
			zero = new Vector2(30f, 80f);
			LabelUtil.TextOut(zero, empty, "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			Vector2 vector2 = LabelUtil.CalcLength("BigLabel", empty);
			LabelUtil.TextOut(new Vector2(vector2.x + 40f, vector2.y + 60f), StringMgr.Instance.Get("DETAIL_TODAYMAP"), "MidLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		else if (selMain == 2)
		{
			empty = StringMgr.Instance.Get("MAP_MANAGEMENT").ToUpper() + " - " + StringMgr.Instance.Get("WEEKLY_POP_MAP");
			zero = new Vector2(30f, 80f);
			LabelUtil.TextOut(zero, empty, "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			Vector2 vector3 = LabelUtil.CalcLength("BigLabel", empty);
			LabelUtil.TextOut(new Vector2(vector3.x + 40f, vector3.y + 60f), StringMgr.Instance.Get("DETAIL_WEEKLYMAP"), "MidLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		else if (selMain == 3)
		{
			empty = StringMgr.Instance.Get("MAP_MANAGEMENT").ToUpper() + " - " + StringMgr.Instance.Get("HALL_OF_FAME");
			zero = new Vector2(30f, 80f);
			LabelUtil.TextOut(zero, empty, "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			Vector2 vector4 = LabelUtil.CalcLength("BigLabel", empty);
			LabelUtil.TextOut(new Vector2(vector4.x + 40f, vector4.y + 60f), StringMgr.Instance.Get("DETAIL_HONORMAP"), "MidLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		else if (selMain == 4)
		{
			empty = StringMgr.Instance.Get("MAP_MANAGEMENT").ToUpper() + " - " + StringMgr.Instance.Get("MY_MAP_SLOT");
			zero = new Vector2(30f, 80f);
			LabelUtil.TextOut(zero, empty, "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			Vector2 vector5 = LabelUtil.CalcLength("BigLabel", empty);
			LabelUtil.TextOut(new Vector2(vector5.x + 40f, vector5.y + 60f), StringMgr.Instance.Get("DETAIL_MYMAP"), "MidLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		else if (selMain == 5)
		{
			empty = StringMgr.Instance.Get("MAP_MANAGEMENT").ToUpper() + " - " + StringMgr.Instance.Get("MY_REG_MAP");
			zero = new Vector2(30f, 80f);
			LabelUtil.TextOut(zero, empty, "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			Vector2 vector6 = LabelUtil.CalcLength("BigLabel", empty);
			LabelUtil.TextOut(new Vector2(vector6.x + 40f, vector6.y + 60f), StringMgr.Instance.Get("DETAIL_MYREGMAP"), "MidLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		else if (selMain == 6)
		{
			empty = StringMgr.Instance.Get("MAP_MANAGEMENT").ToUpper() + " - " + StringMgr.Instance.Get("MY_DOWNLOAD_MAP");
			zero = new Vector2(30f, 80f);
			LabelUtil.TextOut(zero, empty, "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			Vector2 vector7 = LabelUtil.CalcLength("BigLabel", empty);
			LabelUtil.TextOut(new Vector2(vector7.x + 40f, vector7.y + 60f), StringMgr.Instance.Get("DETAIL_MYDNMAP"), "MidLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
	}

	private void ShowTooltip(int id)
	{
		LabelUtil.TextOut(new Vector2(10f, 10f), tooltipMessage, "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
	}

	private void DoTooltip()
	{
		if (!DialogManager.Instance.IsModal && Event.current.type == EventType.Repaint && GUI.enabled)
		{
			if (lastTooltip != GUI.tooltip)
			{
				focusTime = 0f;
			}
			if (focusTime > 0.3f && Event.current.type == EventType.Repaint && GUI.tooltip.Length > 0)
			{
				tooltipMessage = GUI.tooltip;
				Vector2 vector = GlobalVars.Instance.ToGUIPoint(Event.current.mousePosition);
				GUIStyle style = GUI.skin.GetStyle("MiniLabel");
				if (style != null)
				{
					Vector2 vector2 = style.CalcSize(new GUIContent(tooltipMessage));
					Rect rc = new Rect(vector.x, vector.y, vector2.x + 20f, vector2.y + 20f);
					GlobalVars.Instance.FitRightNBottomRectInScreen(ref rc);
					GUI.Window(1101, rc, ShowTooltip, string.Empty, "LineWindow");
				}
			}
			lastTooltip = GUI.tooltip;
		}
		else
		{
			GUI.tooltip = string.Empty;
		}
	}

	public void OnGUI()
	{
		OptionSelector();
		SelectorTitle();
		switch (selMain)
		{
		case 4:
			myMapFrm.bGuiEnable = bGuiEnable;
			myMapFrm.currentTab = 0;
			myMapFrm.OnGUI();
			break;
		case 5:
			myMapFrm.bGuiEnable = bGuiEnable;
			myMapFrm.currentTab = 1;
			myMapFrm.modeTab = selSub;
			myMapFrm.OnGUI();
			selSub = cboxSub.List(crdCBoxSub, selectedSubName, listSubContent);
			selectedSubName = listSubContent[selSub].text;
			break;
		case 6:
			myMapFrm.bGuiEnable = bGuiEnable;
			myMapFrm.currentTab = 2;
			myMapFrm.modeTab = selSub;
			myMapFrm.OnGUI();
			selSub = cboxSub.List(crdCBoxSub, selectedSubName, listSubContent);
			selectedSubName = listSubContent[selSub].text;
			break;
		case 0:
		case 1:
		case 2:
		case 3:
			searchMapFrm.bGuiEnable = bGuiEnable;
			searchMapFrm.SelectedTab(selSub);
			searchMapFrm.OnGUI();
			if (selMain == 0)
			{
				selSub = cboxSub.List(crdCBoxSub, selectedSubName, listSubContent);
				selectedSubName = listSubContent[selSub].text;
				selSearch = cboxSearch.List(crdCBoxSearch, selectedSearchName, listSearchContent);
				selectedSearchName = listSearchContent[selSearch].text;
				searchMapFrm.selSearch = selSearch;
			}
			break;
		}
		DoTooltip();
	}

	public void Update()
	{
		focusTime += Time.deltaTime;
		CheckGUIEnable();
	}

	public void Close()
	{
		cboxMain.ForceUnShow();
		cboxSub.ForceUnShow();
	}
}
