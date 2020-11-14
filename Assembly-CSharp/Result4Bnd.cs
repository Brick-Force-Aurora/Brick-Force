using System;
using System.Collections.Generic;
using UnityEngine;

public class Result4Bnd : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	public string[] winner;

	public string[] win;

	public string[] draw;

	public string[] lose;

	public string[] loser;

	public float wait = 10f;

	public Texture2D levelUpIcon;

	public Texture2D redBg;

	public Texture2D blueBg;

	public Texture2D mapCover;

	public Texture2D winnerImg;

	public Texture2D loseImg;

	private Rect crdFrame = new Rect(5f, 0f, 846f, 483f);

	private Rect crdRedWin = new Rect(10f, 4f, 834f, 237f);

	private Rect crdBlueWin = new Rect(10f, 237f, 834f, 237f);

	private Rect crdRedWinImg = new Rect(706f, 187f, 130f, 50f);

	private Rect crdBlueWinImg = new Rect(706f, 422f, 130f, 50f);

	private Rect crdBoxRed = new Rect(15f, 7f, 825f, 22f);

	private Rect crdBoxBlue = new Rect(15f, 240f, 825f, 22f);

	private float redResultY = 17f;

	private float blueResultY = 251f;

	private float redFirstRowY = 43f;

	private float blueFirstRowY = 279f;

	private float markX = 31f;

	private float badgeX = 66f;

	private float nickX = 180f;

	private float killX = 320f;

	private float missionX = 460f;

	private float scoreX = 540f;

	private float xpX = 620f;

	private float pointX = 700f;

	private float buffX = 780f;

	private Vector2 crdBadgeSize = new Vector2(34f, 17f);

	private Vector2 crdClanMarkSize = new Vector2(16f, 16f);

	private Vector2 crdBuffSize = new Vector2(12f, 12f);

	private Vector2 crdLevelupSize = new Vector2(18f, 22f);

	private float myRowX = 13f;

	private Vector2 myRowSize = new Vector2(825f, 22f);

	private float offset = 26f;

	private Rect crdTotalFrame = new Rect(855f, 0f, 164f, 87f);

	private Rect crdRedTotal = new Rect(880f, 30f, 54f, 49f);

	private Rect crdBlueTotal = new Rect(940f, 30f, 54f, 49f);

	private Vector2 crdRTK = new Vector2(880f, 27f);

	private Vector2 crdRTD = new Vector2(934f, 82f);

	private Vector2 crdBTK = new Vector2(940f, 27f);

	private Vector2 crdBTD = new Vector2(994f, 82f);

	private Vector2 crdTotalStuff = new Vector2(937f, 12f);

	private Rect crdStarFrame = new Rect(855f, 88f, 164f, 394f);

	private Rect crdThumbnail = new Rect(862f, 102f, 150f, 150f);

	private Vector2 crdDeveloper = new Vector2(866f, 208f);

	private Vector2 crdAlias = new Vector2(866f, 228f);

	private Rect crdIconGood = new Rect(875f, 274f, 22f, 22f);

	private Rect crdIconBad = new Rect(875f, 310f, 22f, 22f);

	private Rect crdBtnEval = new Rect(862f, 382f, 150f, 26f);

	private Rect crdDownload = new Rect(862f, 414f, 150f, 26f);

	private Rect crdBtnLobby = new Rect(862f, 446f, 150f, 26f);

	private float deltaTime;

	private int step;

	private int cvtEndCode;

	private bool IsDownloadButtonView;

	private Dictionary<int, int> dicLeveler;

	private RegMap playMap;

	private string tooltipMessage = string.Empty;

	private void Start()
	{
		GlobalVars.Instance.ApplyAudioSource();
		playMap = RegMapManager.Instance.Get(RoomManager.Instance.CurMap);
		DialogManager.Instance.CloseAll();
		BrickManager.Instance.MakeSystemMapInstance(BrickManager.SYSTEM_MAP.TEAM_MATCH_AWARDS);
		SetupEndCode();
		CreateAwardees();
		step = 0;
		IsDownloadButtonView = false;
		ApplyGameResult();
		CheckLevelUpPlayer();
	}

	private void CheckLevelUpPlayer()
	{
		dicLeveler = new Dictionary<int, int>();
		for (int i = 0; i < RoomManager.Instance.RU.Length; i++)
		{
			int level = XpManager.Instance.GetLevel(RoomManager.Instance.RU[i].prevXp);
			int level2 = XpManager.Instance.GetLevel(RoomManager.Instance.RU[i].nextXp);
			if (level2 > level)
			{
				GameObject gameObject = BrickManManager.Instance.Get(RoomManager.Instance.RU[i].seq);
				if (null != gameObject)
				{
					dicLeveler.Add(RoomManager.Instance.RU[i].seq, level2);
					TPController component = gameObject.GetComponent<TPController>();
					if (null != component)
					{
						component.Congratulation();
					}
				}
				if (RoomManager.Instance.RU[i].seq == MyInfoManager.Instance.Seq)
				{
					if (!BuildOption.Instance.IsInfernum && BuildOption.Instance.Props.useLevelupCompensation)
					{
						((LevelUpDlg)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.LEVELUP, exclusive: true))?.InitDialog(level, level2);
					}
					MyInfoManager.Instance.EnResultEvent(MyInfoManager.RESULT_EVENT.LEVEL_UP);
					CSNetManager.Instance.Sock.SendCS_LEVELUP_EVENT_REQ();
				}
			}
		}
	}

	private void SetupEndCode()
	{
		cvtEndCode = 0;
		if (RoomManager.Instance.endCode < 0)
		{
			if (MyInfoManager.Instance.Slot < 8)
			{
				cvtEndCode = 1;
			}
			else
			{
				cvtEndCode = -1;
			}
		}
		else if (RoomManager.Instance.endCode > 0)
		{
			if (MyInfoManager.Instance.Slot < 8)
			{
				cvtEndCode = -1;
			}
			else
			{
				cvtEndCode = 1;
			}
		}
	}

	private void CreateAwardees()
	{
		BrickManManager.Instance.ClearBrickManEtc();
		Brick.SPAWNER_TYPE spawnerType = Brick.SPAWNER_TYPE.BLUE_TEAM_SPAWNER;
		Brick.SPAWNER_TYPE spawnerType2 = Brick.SPAWNER_TYPE.RED_TEAM_SPAWNER;
		if (cvtEndCode == 1)
		{
			spawnerType = Brick.SPAWNER_TYPE.RED_TEAM_SPAWNER;
			spawnerType2 = Brick.SPAWNER_TYPE.BLUE_TEAM_SPAWNER;
		}
		int num = 0;
		for (int i = 0; i < RoomManager.Instance.RU.Length; i++)
		{
			if (!RoomManager.Instance.RU[i].red)
			{
				BrickManDesc brickManDesc = BrickManManager.Instance.GetDesc(RoomManager.Instance.RU[i].seq);
				if (brickManDesc == null && RoomManager.Instance.RU[i].seq == MyInfoManager.Instance.Seq)
				{
					brickManDesc = new BrickManDesc(MyInfoManager.Instance.Seq, MyInfoManager.Instance.Nickname, MyInfoManager.Instance.GetUsings(), 0, MyInfoManager.Instance.Xp, MyInfoManager.Instance.ClanSeq, MyInfoManager.Instance.ClanName, MyInfoManager.Instance.ClanMark, MyInfoManager.Instance.Rank, null, null);
				}
				if (brickManDesc != null)
				{
					GameObject gameObject = BrickManManager.Instance.AddBrickMan(brickManDesc);
					if (null != gameObject)
					{
						SpawnerDesc awardSpawner4TeamMatch = BrickManager.Instance.GetAwardSpawner4TeamMatch(spawnerType, num++);
						if (awardSpawner4TeamMatch != null)
						{
							gameObject.transform.position = new Vector3(awardSpawner4TeamMatch.position.x, awardSpawner4TeamMatch.position.y - 0.5f, awardSpawner4TeamMatch.position.z);
							gameObject.transform.rotation = Rot.ToQuaternion(awardSpawner4TeamMatch.rotation);
						}
					}
				}
			}
		}
		num = 0;
		for (int j = 0; j < RoomManager.Instance.RU.Length; j++)
		{
			if (RoomManager.Instance.RU[j].red)
			{
				BrickManDesc brickManDesc2 = BrickManManager.Instance.GetDesc(RoomManager.Instance.RU[j].seq);
				if (brickManDesc2 == null && RoomManager.Instance.RU[j].seq == MyInfoManager.Instance.Seq)
				{
					brickManDesc2 = new BrickManDesc(MyInfoManager.Instance.Seq, MyInfoManager.Instance.Nickname, MyInfoManager.Instance.GetUsings(), 0, MyInfoManager.Instance.Xp, MyInfoManager.Instance.ClanSeq, MyInfoManager.Instance.ClanName, MyInfoManager.Instance.ClanMark, MyInfoManager.Instance.Rank, null, null);
				}
				if (brickManDesc2 != null)
				{
					GameObject gameObject2 = BrickManManager.Instance.AddBrickMan(brickManDesc2);
					if (null != gameObject2)
					{
						SpawnerDesc awardSpawner4TeamMatch2 = BrickManager.Instance.GetAwardSpawner4TeamMatch(spawnerType2, num++);
						if (awardSpawner4TeamMatch2 != null)
						{
							gameObject2.transform.position = new Vector3(awardSpawner4TeamMatch2.position.x, awardSpawner4TeamMatch2.position.y - 0.5f, awardSpawner4TeamMatch2.position.z);
							gameObject2.transform.rotation = Rot.ToQuaternion(awardSpawner4TeamMatch2.rotation);
						}
					}
				}
			}
		}
	}

	private void DrawClanMark(Rect rc, int mark)
	{
		if (mark >= 0)
		{
			Texture2D bg = ClanMarkManager.Instance.GetBg(mark);
			Color colorValue = ClanMarkManager.Instance.GetColorValue(mark);
			Texture2D amblum = ClanMarkManager.Instance.GetAmblum(mark);
			if (null != bg)
			{
				TextureUtil.DrawTexture(rc, bg);
			}
			Color color = GUI.color;
			GUI.color = colorValue;
			if (null != amblum)
			{
				TextureUtil.DrawTexture(rc, amblum);
			}
			GUI.color = color;
		}
	}

	private float GridOut(ResultUnit ru, float y)
	{
		int rank = -1;
		int mark = -1;
		BrickManDesc desc = BrickManManager.Instance.GetDesc(ru.seq);
		if (desc != null)
		{
			mark = desc.ClanMark;
			rank = desc.Rank;
		}
		else if (ru.seq == MyInfoManager.Instance.Seq)
		{
			mark = MyInfoManager.Instance.ClanMark;
			rank = MyInfoManager.Instance.Rank;
		}
		if (MyInfoManager.Instance.Seq == ru.seq)
		{
			GUI.Box(new Rect(myRowX, y - myRowSize.y / 2f, myRowSize.x, myRowSize.y), string.Empty, "BoxMyInfo");
		}
		DrawClanMark(new Rect(markX - crdClanMarkSize.x / 2f, y - crdClanMarkSize.y / 2f - 1f, crdClanMarkSize.x, crdClanMarkSize.y), mark);
		int level = XpManager.Instance.GetLevel(ru.nextXp);
		Texture2D badge = XpManager.Instance.GetBadge(level, rank);
		if (null != badge)
		{
			TextureUtil.DrawTexture(new Rect(badgeX - crdBadgeSize.x / 2f, y - crdBadgeSize.y / 2f - 1f, crdBadgeSize.x, crdBadgeSize.y), badge);
		}
		LabelUtil.TextOut(new Vector2(nickX, y), ru.nickname, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(killX, y), ru.kill.ToString() + "/" + ru.assist.ToString() + "/" + ru.death.ToString(), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(missionX, y), "0", "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(scoreX, y), ru.score.ToString(), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(xpX, y), ru.xp.ToString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(pointX, y), ru.point.ToString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		Vector2 vector = new Vector2(buffX, y);
		BuffDesc[] array = BuffManager.Instance.ToWhyArray(ru.buff);
		for (int i = 0; i < array.Length; i++)
		{
			GUI.Button(new Rect(vector.x, vector.y - crdBuffSize.y / 2f, crdBuffSize.x, crdBuffSize.y), new GUIContent(array[i].icon, StringMgr.Instance.Get(array[i].tooltip)), "InvisibleButton");
			vector.x += crdBuffSize.x + 2f;
		}
		if (dicLeveler.ContainsKey(ru.seq))
		{
			TextureUtil.DrawTexture(new Rect(vector.x, vector.y - crdLevelupSize.y / 2f, crdLevelupSize.x, crdLevelupSize.y), levelUpIcon, ScaleMode.StretchToFill);
		}
		return y + offset;
	}

	private void DoTotal()
	{
		int redTotalKill = RoomManager.Instance.redTotalKill;
		int redTotalDeath = RoomManager.Instance.redTotalDeath;
		int blueTotalKill = RoomManager.Instance.blueTotalKill;
		int blueTotalDeath = RoomManager.Instance.blueTotalDeath;
		GUI.Box(crdTotalFrame, string.Empty, "BoxMapE");
		TextureUtil.DrawTexture(crdRedTotal, redBg, ScaleMode.StretchToFill);
		TextureUtil.DrawTexture(crdBlueTotal, blueBg, ScaleMode.StretchToFill);
		LabelUtil.TextOut(crdTotalStuff, StringMgr.Instance.Get("ALL") + " " + StringMgr.Instance.Get("KILL") + "/" + StringMgr.Instance.Get("DEATH"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(crdRTK, redTotalKill.ToString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdRTD, redTotalDeath.ToString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.LowerRight);
		LabelUtil.TextOut(crdBTK, blueTotalKill.ToString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdBTD, blueTotalDeath.ToString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.LowerRight);
	}

	private void DoStarDust()
	{
		bool flag = false;
		GUI.Box(crdStarFrame, string.Empty, "BoxMapE");
		if (playMap != null)
		{
			if (playMap.Thumbnail != null)
			{
				TextureUtil.DrawTexture(crdThumbnail, playMap.Thumbnail, ScaleMode.StretchToFill);
			}
			TextureUtil.DrawTexture(crdThumbnail, mapCover, ScaleMode.StretchToFill);
			DateTime registeredDate = playMap.RegisteredDate;
			if (registeredDate.Year == DateTime.Today.Year && registeredDate.Month == DateTime.Today.Month && registeredDate.Day == DateTime.Today.Day)
			{
				TextureUtil.DrawTexture(new Rect(crdThumbnail.x, crdThumbnail.y, (float)GlobalVars.Instance.iconNewmap.width, (float)GlobalVars.Instance.iconNewmap.height), GlobalVars.Instance.iconNewmap, ScaleMode.StretchToFill);
			}
			else if ((playMap.tagMask & 8) != 0)
			{
				TextureUtil.DrawTexture(new Rect(crdThumbnail.x, crdThumbnail.y, (float)GlobalVars.Instance.iconglory.width, (float)GlobalVars.Instance.iconglory.height), GlobalVars.Instance.iconglory, ScaleMode.StretchToFill);
			}
			else if ((playMap.tagMask & 4) != 0)
			{
				TextureUtil.DrawTexture(new Rect(crdThumbnail.x, crdThumbnail.y, (float)GlobalVars.Instance.iconMedal.width, (float)GlobalVars.Instance.iconMedal.height), GlobalVars.Instance.iconMedal, ScaleMode.StretchToFill);
			}
			else if ((playMap.tagMask & 2) != 0)
			{
				TextureUtil.DrawTexture(new Rect(crdThumbnail.x, crdThumbnail.y, (float)GlobalVars.Instance.icongoldRibbon.width, (float)GlobalVars.Instance.icongoldRibbon.height), GlobalVars.Instance.icongoldRibbon, ScaleMode.StretchToFill);
			}
			if (playMap.IsAbuseMap())
			{
				float x = crdThumbnail.x + crdThumbnail.width - (float)GlobalVars.Instance.iconDeclare.width;
				TextureUtil.DrawTexture(new Rect(x, crdThumbnail.y, (float)GlobalVars.Instance.iconDeclare.width, (float)GlobalVars.Instance.iconDeclare.height), GlobalVars.Instance.iconDeclare, ScaleMode.StretchToFill);
			}
			LabelUtil.TextOut(crdDeveloper, StringMgr.Instance.Get("DEVELOPER_IS") + playMap.Developer, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdAlias, playMap.Alias, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			TextureUtil.DrawTexture(crdIconGood, GlobalVars.Instance.iconThumbUp, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(crdIconBad, GlobalVars.Instance.iconThumbDn, ScaleMode.StretchToFill);
			Vector2 pos = new Vector2(crdIconGood.x + 60f, crdIconGood.y + 4f);
			Vector2 pos2 = new Vector2(crdIconBad.x + 60f, crdIconBad.y + 4f);
			LabelUtil.TextOut(pos, playMap.Likes.ToString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(pos2, playMap.DisLikes.ToString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			GUI.enabled = ((RoomManager.Instance.commented != 1) ? true : false);
			if (GlobalVars.Instance.MyButton(crdBtnEval, StringMgr.Instance.Get("DO_EVAL"), "BtnBlue"))
			{
				((MapEvalDlg)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.MAP_EVAL, exclusive: true))?.InitDialog(playMap.Map);
			}
			GUI.enabled = true;
			if (BuildOption.Instance.Props.UseAccuse && GlobalVars.Instance.MyButton(new Rect(crdBtnEval.x, crdBtnEval.y - 30f, crdBtnEval.width, crdBtnEval.height), StringMgr.Instance.Get("REPORT_GM_TITLE_02"), "BtnBlue"))
			{
				((AccusationMapDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.ACCUSATION_MAP, exclusive: true))?.InitDialog(playMap);
			}
			if (!RegMapManager.Instance.IsDownloaded(RoomManager.Instance.CurMap))
			{
				RegMap regMap = RegMapManager.Instance.Get(RoomManager.Instance.CurMap);
				if (regMap != null)
				{
					bool enabled = GUI.enabled;
					GUI.enabled = regMap.IsLatest;
					IsDownloadButtonView = true;
					if (GlobalVars.Instance.MyButton(crdDownload, StringMgr.Instance.Get("MAP_DOWNLOAD"), "BtnBlue"))
					{
						((MapDetailDlg)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.MAP_DETAIL, exclusive: true))?.InitDialog(regMap);
					}
					GUI.enabled = enabled;
				}
			}
			if (GlobalVars.Instance.MyButton(crdBtnLobby, StringMgr.Instance.Get("LOBBY"), "BtnBlue"))
			{
				CSNetManager.Instance.Sock.SendCS_RESULT_DONE_REQ();
				Application.LoadLevel("Briefing4TeamMatch");
			}
			if (flag && !Application.isLoadingLevel)
			{
				CSNetManager.Instance.Sock.SendCS_RESULT_DONE_REQ();
				Application.LoadLevel("Briefing4TeamMatch");
			}
		}
	}

	private void OnGUI()
	{
		GlobalVars.Instance.BeginGUI(null);
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		GUI.enabled = !DialogManager.Instance.IsModal;
		GUI.Box(crdFrame, string.Empty, "BoxResult");
		GUI.Box(crdBoxRed, string.Empty, "BoxRed02");
		LabelUtil.TextOut(new Vector2(markX, redResultY), StringMgr.Instance.Get("MARK"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(badgeX, redResultY), StringMgr.Instance.Get("BADGE"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(nickX, redResultY), StringMgr.Instance.Get("CHARACTER"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(killX, redResultY), StringMgr.Instance.Get("KILL") + "/" + StringMgr.Instance.Get("ASSIST") + "/" + StringMgr.Instance.Get("DEATH"), "Label", new Color(1f, 0.66f, 0f, 1f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(missionX, redResultY), StringMgr.Instance.Get("MISSION"), "Label", new Color(0f, 0.87f, 1f, 1f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(scoreX, redResultY), StringMgr.Instance.Get("SCORE"), "Label", new Color(0.8f, 1f, 0.02f, 1f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(xpX, redResultY), StringMgr.Instance.Get("XP"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(pointX, redResultY), StringMgr.Instance.Get("POINT"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		float y = redFirstRowY;
		for (int i = 0; i < RoomManager.Instance.RU.Length; i++)
		{
			if (RoomManager.Instance.RU[i].red)
			{
				y = GridOut(RoomManager.Instance.RU[i], y);
			}
		}
		GUI.Box(crdBoxBlue, string.Empty, "BoxBlue02");
		LabelUtil.TextOut(new Vector2(markX, blueResultY), StringMgr.Instance.Get("MARK"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(badgeX, blueResultY), StringMgr.Instance.Get("BADGE"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(nickX, blueResultY), StringMgr.Instance.Get("CHARACTER"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(killX, blueResultY), StringMgr.Instance.Get("KILL") + "/" + StringMgr.Instance.Get("ASSIST") + "/" + StringMgr.Instance.Get("DEATH"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(missionX, blueResultY), StringMgr.Instance.Get("MISSION"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(scoreX, blueResultY), StringMgr.Instance.Get("SCORE"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(xpX, blueResultY), StringMgr.Instance.Get("XP"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(pointX, blueResultY), StringMgr.Instance.Get("POINT"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		y = blueFirstRowY;
		for (int j = 0; j < RoomManager.Instance.RU.Length; j++)
		{
			if (!RoomManager.Instance.RU[j].red)
			{
				y = GridOut(RoomManager.Instance.RU[j], y);
			}
		}
		if (MyInfoManager.Instance.IsRedTeam())
		{
			GUI.Box(crdRedWin, string.Empty, "BoxWin");
			switch (cvtEndCode)
			{
			case -1:
				TextureUtil.DrawTexture(crdRedWinImg, winnerImg, ScaleMode.StretchToFill);
				break;
			case 1:
				TextureUtil.DrawTexture(crdRedWinImg, loseImg, ScaleMode.StretchToFill);
				break;
			}
		}
		else
		{
			GUI.Box(crdBlueWin, string.Empty, "BoxWin");
			switch (cvtEndCode)
			{
			case -1:
				TextureUtil.DrawTexture(crdBlueWinImg, loseImg, ScaleMode.StretchToFill);
				break;
			case 1:
				TextureUtil.DrawTexture(crdBlueWinImg, winnerImg, ScaleMode.StretchToFill);
				break;
			}
		}
		DoTotal();
		DoStarDust();
		if (Event.current.type == EventType.Repaint && GUI.tooltip.Length > 0)
		{
			tooltipMessage = GUI.tooltip;
			Vector2 vector = GlobalVars.Instance.ToGUIPoint(Event.current.mousePosition);
			GUIStyle style = GUI.skin.GetStyle("Label");
			if (style != null)
			{
				Vector2 vector2 = style.CalcSize(new GUIContent(tooltipMessage));
				Rect clientRect = new Rect(vector.x, vector.y, vector2.x + 20f, vector2.y + 20f);
				GUI.Window(1103, clientRect, ShowTooltip, string.Empty, "LineWindow");
			}
		}
		GUI.enabled = true;
		GUI.skin = skin;
		GlobalVars.Instance.EndGUI();
	}

	private void ShowTooltip(int id)
	{
		LabelUtil.TextOut(new Vector2(10f, 10f), tooltipMessage, "MiniLabel", Color.yellow, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
	}

	private void EmotionalAct()
	{
		int num = 0;
		TPController tPController = null;
		for (int i = 0; i < RoomManager.Instance.RU.Length; i++)
		{
			if (!RoomManager.Instance.RU[i].red)
			{
				GameObject gameObject = BrickManManager.Instance.Get(RoomManager.Instance.RU[i].seq);
				if (null != gameObject)
				{
					TPController component = gameObject.GetComponent<TPController>();
					if (null != component)
					{
						string ani = win[UnityEngine.Random.Range(0, win.Length)];
						if (cvtEndCode == -1)
						{
							ani = lose[UnityEngine.Random.Range(0, lose.Length)];
							tPController = component;
						}
						else if (num == 0)
						{
							ani = winner[UnityEngine.Random.Range(0, winner.Length)];
						}
						num++;
						component.EmotionalAct(ani);
					}
				}
			}
		}
		num = 0;
		for (int j = 0; j < RoomManager.Instance.RU.Length; j++)
		{
			if (RoomManager.Instance.RU[j].red)
			{
				GameObject gameObject2 = BrickManManager.Instance.Get(RoomManager.Instance.RU[j].seq);
				if (null != gameObject2)
				{
					TPController component2 = gameObject2.GetComponent<TPController>();
					if (null != component2)
					{
						string ani2 = win[UnityEngine.Random.Range(0, win.Length)];
						if (cvtEndCode == 1)
						{
							ani2 = lose[UnityEngine.Random.Range(0, lose.Length)];
							tPController = component2;
						}
						else if (num == 0)
						{
							ani2 = winner[UnityEngine.Random.Range(0, winner.Length)];
						}
						num++;
						component2.EmotionalAct(ani2);
					}
				}
			}
		}
		if (null != tPController)
		{
			tPController.EmotionalAct(loser[UnityEngine.Random.Range(0, loser.Length)]);
		}
	}

	private void Update()
	{
		switch (step)
		{
		case 0:
			deltaTime += Time.deltaTime;
			if (deltaTime > 0.1f)
			{
				deltaTime = 0f;
				step++;
			}
			break;
		case 1:
			EmotionalAct();
			step++;
			break;
		case 2:
			deltaTime += Time.deltaTime;
			if (deltaTime > wait && !Application.isLoadingLevel && RoomManager.Instance.commented == 1 && !IsDownloadButtonView)
			{
				CSNetManager.Instance.Sock.SendCS_RESULT_DONE_REQ();
				Application.LoadLevel("Briefing4TeamMatch");
			}
			break;
		}
	}

	private void ApplyGameResult()
	{
		for (int i = 0; i < RoomManager.Instance.RU.Length; i++)
		{
			if (RoomManager.Instance.RU[i].seq == MyInfoManager.Instance.Seq)
			{
				MyInfoManager.Instance.Xp = RoomManager.Instance.RU[i].nextXp;
			}
			else
			{
				BrickManDesc desc = BrickManManager.Instance.GetDesc(RoomManager.Instance.RU[i].seq);
				if (desc != null)
				{
					desc.Xp = RoomManager.Instance.RU[i].nextXp;
					NameCard friend = MyInfoManager.Instance.GetFriend(desc.Seq);
					if (friend != null)
					{
						friend.Lv = XpManager.Instance.GetLevel(desc.Xp);
					}
					NameCard user = ChannelUserManager.Instance.GetUser(desc.Seq);
					if (user != null)
					{
						user.Lv = XpManager.Instance.GetLevel(desc.Xp);
					}
				}
			}
		}
	}

	private void OnNoticeCenter(string text)
	{
		SystemInform.Instance.AddMessageCenter(text);
	}
}
