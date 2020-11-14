using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RankMapFrame
{
	public int BoardIndex = 1;

	public Texture2D nonAvailable;

	public Texture2D selectedMapFrame;

	public Texture2D mapFrame;

	public Texture2D texStarGrade;

	public Texture2D texStarGradeBg;

	public Texture2D[] texRanks;

	public float doubleClickTimeout = 0.2f;

	private List<RankMap> ranking;

	private int subTab;

	private int page;

	private Vector2 scrollPosition = Vector2.zero;

	private bool waitingList;

	private Rect crdFrame = new Rect(266f, 70f, 735f, 590f);

	private Vector2 crdMapBtn = new Vector2(715f, 140f);

	private float crdMapOffset = 10f;

	private Rect crdThumbnail = new Rect(6f, 6f, 128f, 128f);

	private Rect crdLeftBtn = new Rect(532f, 676f, 22f, 18f);

	private Rect crdRightBtn = new Rect(683f, 676f, 22f, 18f);

	private Rect crdPageBox = new Rect(566f, 676f, 108f, 18f);

	private Vector2 crdDeveloperLabel = new Vector2(145f, 10f);

	private Vector2 crdDeveloperVal = new Vector2(150f, 30f);

	private Vector2 crdMapAliasLabel = new Vector2(145f, 50f);

	private Vector2 crdMapAliasVal = new Vector2(150f, 70f);

	private Vector2 crdLastModifiedLabel = new Vector2(145f, 90f);

	private Vector2 crdLastModifiedVal = new Vector2(150f, 110f);

	private Vector2 crdPlayCountLabel = new Vector2(345f, 10f);

	private Vector2 crdPlayCountVal = new Vector2(350f, 30f);

	private Vector2 crdDownloadCountLabel = new Vector2(345f, 50f);

	private Vector2 crdDownloadCountVal = new Vector2(350f, 70f);

	private Vector2 crdDownloadFeeLabel = new Vector2(345f, 90f);

	private Vector2 crdDownloadFeeVal = new Vector2(350f, 110f);

	private Vector2 crdMapEvalLabel = new Vector2(530f, 10f);

	private Rect crdStar = new Rect(535f, 30f, 74f, 14f);

	private Vector2 crdGradeText = new Vector2(620f, 28f);

	private Vector2 crdSupportModeLabel = new Vector2(530f, 50f);

	private Rect crdModeIcon = new Rect(535f, 70f, 35f, 22f);

	private Rect crdDownloaded = new Rect(99f, 99f, 30f, 30f);

	private Rect crdRank = new Rect(10f, 93f, 36f, 38f);

	private Rect crdMakeRoomBtn = new Rect(631f, 720f, 176f, 34f);

	private Rect crdSaveBtn = new Rect(827f, 720f, 176f, 34f);

	public int selected;

	public bool bGuiEnable;

	public bool bUpdateList;

	private int xCount = 3;

	public void Start()
	{
		ranking = new List<RankMap>();
		waitingList = true;
		page = 1;
		selected = 0;
	}

	public void SelectedTab(int tab)
	{
		int num = subTab;
		subTab = tab;
		if (num != tab)
		{
			bUpdateList = true;
			BeginMapList(0);
			selected = 0;
		}
	}

	public void BeginMapList(int curPage)
	{
		page = curPage;
		ranking.Clear();
		scrollPosition = Vector2.zero;
	}

	public void AddMapItem(int rowNo, int rank, int rankChg, RegMap regMap)
	{
		ranking.Add(new RankMap(rowNo, rank, rankChg, regMap));
	}

	public void EndMapList()
	{
		waitingList = false;
	}

	private int MinIndex()
	{
		int num = 2147483647;
		for (int i = 0; i < ranking.Count; i++)
		{
			if (num > ranking[i].RowNo)
			{
				num = ranking[i].RowNo;
			}
		}
		return num;
	}

	private int MaxIndex()
	{
		int num = -2147483648;
		for (int i = 0; i < ranking.Count; i++)
		{
			if (num < ranking[i].RowNo)
			{
				num = ranking[i].RowNo;
			}
		}
		return num;
	}

	private void DoModeSelector()
	{
		if (bUpdateList)
		{
			bUpdateList = false;
			selected = 0;
			if (waitingList)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("WAIT_A_MOMENT"));
			}
			else
			{
				waitingList = true;
			}
		}
	}

	private void DoPagePanel()
	{
		int num = MinIndex();
		int num2 = MaxIndex();
		if (GlobalVars.Instance.MyButton(crdLeftBtn, string.Empty, "Left"))
		{
			if (waitingList)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("WAIT_A_MOMENT"));
			}
			else if (page <= 2)
			{
				waitingList = true;
			}
			else if (page > 2 && num >= 0)
			{
				selected = 0;
				waitingList = true;
			}
		}
		GUI.Box(crdPageBox, string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(crdPageBox.x + crdPageBox.width / 2f, crdPageBox.y + crdPageBox.height / 2f), page.ToString(), "MiniLabel", GlobalVars.Instance.txtMainColor, new Color(0f, 0f, 0f, 0f), TextAnchor.MiddleCenter);
		if (GlobalVars.Instance.MyButton(crdRightBtn, string.Empty, "Right"))
		{
			if (waitingList)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("WAIT_A_MOMENT"));
			}
			else if (num2 >= 0)
			{
				selected = 0;
				waitingList = true;
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
		LabelUtil.TextOut(crdDeveloperLabel, StringMgr.Instance.Get("DEVELOPER") + " : ", "MiniLabel", txtMainColor, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdDeveloperVal, reg.Developer, "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMapAliasLabel, StringMgr.Instance.Get("MAP_NAME_IS") + " " + StringMgr.Instance.Get("MAP_VERSION") + " : ", "MiniLabel", txtMainColor, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMapAliasVal, reg.Alias + reg.Version, "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdLastModifiedLabel, StringMgr.Instance.Get("LAST_MODIFIED_DATE"), "MiniLabel", txtMainColor, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdLastModifiedVal, DateTimeLocal.ToString(reg.RegisteredDate), "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdPlayCountLabel, StringMgr.Instance.Get("PLAY_COUNT") + " : ", "MiniLabel", txtMainColor, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdPlayCountVal, reg.DisLikes.ToString(), "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdDownloadCountLabel, StringMgr.Instance.Get("DOWNLOAD_COUNT") + " : ", "MiniLabel", txtMainColor, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdDownloadCountVal, reg.DownloadCount.ToString(), "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdDownloadFeeLabel, StringMgr.Instance.Get("DOWNLOAD_FEE") + " : ", "MiniLabel", txtMainColor, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdDownloadFeeVal, reg.DownloadFee.ToString(), "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMapEvalLabel, StringMgr.Instance.Get("MAP_EVAL") + " : ", "MiniLabel", txtMainColor, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		TextureUtil.DrawTexture(crdStar, texStarGradeBg, ScaleMode.StretchToFill);
		LabelUtil.TextOut(crdGradeText, "[ " + reg.GetStarAvgString() + " ]", "MiniLabel", txtMainColor, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		float num = (float)reg.Likes / 100f;
		Rect position = new Rect(crdStar.x, crdStar.y, crdStar.width * num, crdStar.height);
		GUI.BeginGroup(position);
		TextureUtil.DrawTexture(new Rect(0f, 0f, crdStar.width, crdStar.height), texStarGrade);
		GUI.EndGroup();
		LabelUtil.TextOut(crdSupportModeLabel, StringMgr.Instance.Get("SUPPORT_MODE") + " : ", "MiniLabel", txtMainColor, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
	}

	public void OnGUI()
	{
		DoPagePanel();
		DoModeSelector();
		if (ranking.Count != 0)
		{
			Texture2D[] array = new Texture2D[ranking.Count];
			for (int i = 0; i < ranking.Count; i++)
			{
				if (ranking[i].OrgMap.Thumbnail == null)
				{
					array[i] = nonAvailable;
				}
				else
				{
					array[i] = ranking[i].OrgMap.Thumbnail;
				}
			}
			float num = crdMapBtn.y * (float)ranking.Count;
			if (ranking.Count > 0)
			{
				num += (float)(ranking.Count - 1) * crdMapOffset;
			}
			scrollPosition = GUI.BeginScrollView(viewRect: new Rect(0f, 0f, crdFrame.width - 29f, num), position: crdFrame, scrollPosition: scrollPosition, alwaysShowHorizontal: false, alwaysShowVertical: false);
			Vector2 vector = new Vector2(0f, 0f);
			for (int j = 0; j < ranking.Count; j++)
			{
				Rect position = new Rect(vector.x, vector.y, crdMapBtn.x, crdMapBtn.y);
				GUI.BeginGroup(position);
				if (GlobalVars.Instance.MyButton(new Rect(0f, 0f, crdMapBtn.x, crdMapBtn.y), string.Empty, "BtnMapList"))
				{
					selected = j;
				}
				if (selected == j)
				{
					GUI.Box(new Rect(0f, 0f, crdMapBtn.x, crdMapBtn.y), string.Empty, "ViewSelected");
				}
				TextureUtil.DrawTexture(crdThumbnail, array[j], ScaleMode.StretchToFill);
				if (ranking[j].OrgMap.Alias.Length > 0)
				{
					PrintMapInfo(ranking[j].OrgMap);
					DrawMode(ranking[j].OrgMap.ModeMask);
				}
				if (RegMapManager.Instance.IsDownloaded(ranking[j].OrgMap.Map))
				{
					TextureUtil.DrawTexture(crdDownloaded, GlobalVars.Instance.iconDownloaded, ScaleMode.StretchToFill);
				}
				if (page == 1 && j < 3)
				{
					TextureUtil.DrawTexture(crdRank, texRanks[j], ScaleMode.StretchToFill);
				}
				GUI.EndGroup();
				vector.y += crdMapBtn.y + crdMapOffset;
			}
			GUI.EndScrollView();
			GUIContent content = new GUIContent(StringMgr.Instance.Get("CREATE_ROOM").ToUpper(), GlobalVars.Instance.iconJoin);
			if (ChannelManager.Instance.CurChannel.Mode != 3 && RegMapManager.Instance.IsDownloaded(ranking[selected].OrgMap.Map) && GlobalVars.Instance.MyButton3(crdMakeRoomBtn, content, "BtnAction"))
			{
				CreateRoomDialog createRoomDialog = (CreateRoomDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.CREATE_ROOM, exclusive: true);
				if (createRoomDialog != null && !createRoomDialog.InitDialog4TeamMatch(ranking[selected].OrgMap.Map, ranking[selected].OrgMap.ModeMask))
				{
					DialogManager.Instance.Clear();
				}
			}
			if (RegMapManager.Instance.IsDownloaded(ranking[selected].OrgMap.Map))
			{
				content = new GUIContent(StringMgr.Instance.Get("DELETE").ToUpper(), GlobalVars.Instance.iconGarbage);
				if (GlobalVars.Instance.MyButton3(crdSaveBtn, content, "BtnAction"))
				{
					CSNetManager.Instance.Sock.SendCS_DEL_DOWNLOAD_MAP_REQ(ranking[selected].OrgMap.Map);
				}
			}
			else
			{
				bool enabled = GUI.enabled;
				GUI.enabled = ranking[selected].IsLatest;
				content = new GUIContent(StringMgr.Instance.Get("SAVE").ToUpper(), GlobalVars.Instance.iconDisk);
				if (GlobalVars.Instance.MyButton3(crdSaveBtn, content, "BtnAction"))
				{
					((DownloadFeeDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.DOWNLOAD_FEE, exclusive: true))?.InitDialog(ranking[selected].OrgMap);
				}
				GUI.enabled = enabled;
			}
		}
	}
}
