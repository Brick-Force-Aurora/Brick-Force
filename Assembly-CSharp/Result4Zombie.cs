using System;
using System.Collections.Generic;
using UnityEngine;

public class Result4Zombie : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	public string[] grade1st;

	public string[] grade2nd;

	public string[] grade3rd;

	public Texture2D levelUpIcon;

	public Texture2D cover;

	public Texture2D winnerBg;

	public float wait = 15f;

	private Rect crdFrame = new Rect(5f, 0f, 846f, 483f);

	private Rect crdBoxGreen = new Rect(13f, 7f, 827f, 22f);

	private float resultY = 19f;

	private float firstRowY = 48f;

	private float markX = 31f;

	private float badgeX = 66f;

	private float nickX = 180f;

	private float zombieWinX = 320f;

	private float brickManWinX = 440f;

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

	private float offset = 28f;

	private Rect crdWinnerBox = new Rect(852f, 0f, 166f, 84f);

	private Rect crdWinnerBg = new Rect(855f, 4f, 150f, 66f);

	private Vector2 crdWinnerKillDeath = new Vector2(1002f, 58f);

	private Vector2 crdWinnerNick = new Vector2(1002f, 30f);

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

	private Dictionary<int, int> dicLeveler;

	private RegMap playMap;

	private string tooltipMessage = string.Empty;

	private bool IsDownloadButtonView;

	private void Start()
	{
		GlobalVars.Instance.ApplyAudioSource();
		playMap = RegMapManager.Instance.Get(RoomManager.Instance.CurMap);
		DialogManager.Instance.CloseAll();
		BrickManager.Instance.MakeSystemMapInstance(BrickManager.SYSTEM_MAP.INDIVIDUAL_MATCH_AWARDS);
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
		DrawClanMark(new Rect(markX - crdClanMarkSize.x / 2f, y - crdClanMarkSize.y / 2f, crdClanMarkSize.x, crdClanMarkSize.y), mark);
		int level = XpManager.Instance.GetLevel(ru.nextXp);
		Texture2D badge = XpManager.Instance.GetBadge(level, rank);
		if (null != badge)
		{
			TextureUtil.DrawTexture(new Rect(badgeX - crdBadgeSize.x / 2f, y - crdBadgeSize.y / 2f - 1f, crdBadgeSize.x, crdBadgeSize.y), badge);
		}
		LabelUtil.TextOut(new Vector2(nickX, y), ru.nickname, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(zombieWinX, y), ru.death.ToString(), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(brickManWinX, y), ru.kill.ToString(), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
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
			TextureUtil.DrawTexture(crdThumbnail, cover, ScaleMode.StretchToFill);
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
			if (GlobalVars.Instance.MyButton(new Rect(crdBtnEval.x, crdBtnEval.y - 30f, crdBtnEval.width, crdBtnEval.height), StringMgr.Instance.Get("REPORT_GM_TITLE_02"), "BtnBlue"))
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

	private void DoWinner()
	{
		int kill = RoomManager.Instance.RU[0].kill;
		int death = RoomManager.Instance.RU[0].death;
		string nickname = RoomManager.Instance.RU[0].nickname;
		GUI.Box(crdWinnerBox, string.Empty, "BoxMapE");
		TextureUtil.DrawTexture(crdWinnerBg, winnerBg, ScaleMode.StretchToFill);
		LabelUtil.TextOut(crdWinnerKillDeath, kill.ToString() + "/" + death.ToString(), "Label", Color.grey, GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
		LabelUtil.TextOut(crdWinnerNick, nickname, "Label", Color.black, GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
	}

	private void OnGUI()
	{
		GlobalVars.Instance.BeginGUI(null);
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		GUI.enabled = !DialogManager.Instance.IsModal;
		GUI.Box(crdFrame, string.Empty, "BoxResult");
		GUI.Box(crdBoxGreen, string.Empty, "BoxGreen02");
		LabelUtil.TextOut(new Vector2(markX, resultY), StringMgr.Instance.Get("MARK"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(badgeX, resultY), StringMgr.Instance.Get("BADGE"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(nickX, resultY), StringMgr.Instance.Get("CHARACTER"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(zombieWinX, resultY), StringMgr.Instance.Get("ZOMBIE_ZOMBIE_ALIVE"), "Label", new Color(1f, 0.66f, 0f, 1f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(brickManWinX, resultY), StringMgr.Instance.Get("ZOMBIE_HUMAN_ALIVE"), "Label", new Color(0f, 0.87f, 1f, 1f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(scoreX, resultY), StringMgr.Instance.Get("SCORE"), "Label", new Color(0.8f, 1f, 0.02f, 1f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(xpX, resultY), StringMgr.Instance.Get("XP"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(pointX, resultY), StringMgr.Instance.Get("POINT"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		float y = firstRowY;
		for (int i = 0; i < RoomManager.Instance.RU.Length; i++)
		{
			y = GridOut(RoomManager.Instance.RU[i], y);
		}
		DoWinner();
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
		LabelUtil.TextOut(new Vector2(10f, 10f), tooltipMessage, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
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

	private void EmotionalAct()
	{
		for (int i = 0; i < RoomManager.Instance.RU.Length; i++)
		{
			GameObject gameObject = BrickManManager.Instance.Get(RoomManager.Instance.RU[i].seq);
			if (null != gameObject)
			{
				TPController component = gameObject.GetComponent<TPController>();
				if (null != component)
				{
					string empty = string.Empty;
					float num = (float)i / (float)RoomManager.Instance.RU.Length;
					empty = ((num < 0.2f) ? grade1st[UnityEngine.Random.Range(0, grade1st.Length)] : ((!(num < 0.6f)) ? grade3rd[UnityEngine.Random.Range(0, grade3rd.Length)] : grade2nd[UnityEngine.Random.Range(0, grade2nd.Length)]));
					component.EmotionalAct(empty);
				}
			}
		}
	}

	private void CreateAwardees()
	{
		BrickManManager.Instance.ClearBrickManEtc();
		Brick.SPAWNER_TYPE sPAWNER_TYPE = Brick.SPAWNER_TYPE.RED_TEAM_SPAWNER;
		Brick.SPAWNER_TYPE sPAWNER_TYPE2 = Brick.SPAWNER_TYPE.BLUE_TEAM_SPAWNER;
		int num = 0;
		for (int i = 0; i < RoomManager.Instance.RU.Length; i++)
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
					SpawnerDesc awardSpawner4TeamMatch = BrickManager.Instance.GetAwardSpawner4TeamMatch((num >= 8) ? sPAWNER_TYPE2 : sPAWNER_TYPE, num++);
					if (awardSpawner4TeamMatch != null)
					{
						gameObject.transform.position = new Vector3(awardSpawner4TeamMatch.position.x, awardSpawner4TeamMatch.position.y - 0.5f, awardSpawner4TeamMatch.position.z);
						gameObject.transform.rotation = Rot.ToQuaternion(awardSpawner4TeamMatch.rotation);
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
