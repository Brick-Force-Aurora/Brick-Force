using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ClanDialog : Dialog
{
	public Texture2D icon;

	public Texture2D lose;

	public Texture2D draw;

	public Texture2D win;

	public string[] tab4StaffKey;

	public string[] tab4MemberKey;

	public string[] tab4VirtualMemberKey;

	public string[] tab4SomebodyKey;

	public string[] tab4MngKey;

	public string[] clanLevelKey;

	public Texture2D colorFoot;

	public Texture2D grayFoot;

	public float refreshLimit = 1f;

	public Rect crdBtnClose = new Rect(647f, 10f, 77f, 26f);

	private Rect crdBtnRefresh = new Rect(912f, 5f, 34f, 34f);

	public Rect crdIcon = new Rect(10f, 10f, 34f, 34f);

	private Rect crdTab = new Rect(18f, 70f, 402f, 36f);

	private Rect crdTab4NoMember = new Rect(18f, 70f, 200f, 36f);

	private Rect crdMainOutline = new Rect(14f, 105f, 521f, 491f);

	private Rect crdExplOutline = new Rect(545f, 105f, 440f, 491f);

	private Rect crdSearchKey = new Rect(34f, 126f, 358f, 25f);

	private Rect crdSearchBtn = new Rect(413f, 121f, 115f, 34f);

	private Rect crdCreateBtn = new Rect(852f, 58f, 132f, 44f);

	private Rect crdSearchKey4Member = new Rect(34f, 126f, 358f, 25f);

	private Rect crdSearchBtn4Member = new Rect(413f, 121f, 115f, 34f);

	private Rect crClanListTitleTop = new Rect(27f, 166f, 495f, 32f);

	private Vector2 crdSLRank = new Vector2(51f, 181f);

	private Vector2 crdSLName = new Vector2(188f, 181f);

	private Vector2 crdSLRecord = new Vector2(296f, 181f);

	private Vector2 crdSLNoMember = new Vector2(355f, 181f);

	private Vector2 crdSLScore = new Vector2(417f, 181f);

	private Vector2 crdSLDay = new Vector2(494f, 181f);

	private Vector2 crdSLLeftTop = new Vector2(26f, 198f);

	private float crdSLFirstY = 210f;

	private float crdLSItemHeight = 28f;

	private float crdLSItemWidth = 495f;

	private Rect crdSLPrevPage = new Rect(192f, 557f, 13f, 17f);

	private Rect crdSLPageBox = new Rect(220f, 557f, 105f, 18f);

	private Vector2 crdSLPage = new Vector2(277f, 566f);

	private Rect crdSLNextPage = new Rect(334f, 557f, 13f, 17f);

	public Rect crdClanInfo = new Rect(426f, 52f, 316f, 422f);

	private Rect crdClanInfoMark = new Rect(580f, 160f, 64f, 64f);

	private Rect crdClanInfoGeneral = new Rect(558f, 126f, 414f, 169f);

	private Vector2 crdClanInfoName = new Vector2(766f, 150f);

	private Vector2 crdClanInfoMaster = new Vector2(766f, 165f);

	private Vector2 crdClanInfoRank = new Vector2(766f, 180f);

	private Vector2 crdClanInfoRecord = new Vector2(766f, 195f);

	private Vector2 crdClanInfoMembers = new Vector2(766f, 210f);

	private Vector2 crdClanInfoNameLabel = new Vector2(676f, 150f);

	private Vector2 crdClanInfoMasterLabel = new Vector2(676f, 165f);

	private Vector2 crdClanInfoRankLabel = new Vector2(676f, 180f);

	private Vector2 crdClanInfoRecordLabel = new Vector2(676f, 195f);

	private Vector2 crdClanInfoMembersLabel = new Vector2(676f, 210f);

	private Vector2 crdTrophyLabel = new Vector2(580f, 250f);

	private Vector2 crdTrophyResult = new Vector2(800f, 250f);

	private Rect crdClanInfoIntro = new Rect(558f, 310f, 414f, 229f);

	private Rect crdClanInfoIntro4Member = new Rect(558f, 310f, 414f, 229f);

	private Rect crdApply = new Rect(852f, 553f, 126f, 34f);

	private Rect crdCheckRank = new Rect(32f, 557f, 21f, 22f);

	private Rect crdCheckCreateDay = new Rect(375f, 557f, 21f, 22f);

	private bool IsRankSet;

	private bool IsCreateDaySet;

	private Rect crdMngInfoMark = new Rect(49f, 160f, 64f, 64f);

	private Rect crdMngInfoGeneral = new Rect(27f, 126f, 495f, 107f);

	private Rect crdMngInfoNotice = new Rect(27f, 274f, 495f, 96f);

	private Rect crdMngInfoIntro = new Rect(27f, 406f, 495f, 133f);

	public Rect crdMngMember = new Rect(426f, 52f, 316f, 422f);

	private Rect crdCloseClan = new Rect(149f, 553f, 128f, 34f);

	private Rect crdChangeMark = new Rect(21f, 553f, 128f, 34f);

	private Rect crdChangeIntro = new Rect(405f, 553f, 128f, 34f);

	private Rect crdChangeNotice = new Rect(277f, 553f, 128f, 34f);

	private Rect crdMngTab = new Rect(549f, 70f, 424f, 36f);

	private float crdMngMemberItemHeight = 28f;

	private float crdMngMemberItemWidth = 414f;

	private Rect crdMngMemberListPosition = new Rect(560f, 180f, 414f, 360f);

	private Vector2 crdMngMemberRoyalty = new Vector2(20f, 0f);

	private Vector2 crdMngMemberBadge = new Vector2(102f, 0f);

	private Vector2 crdMngMemberNickname = new Vector2(152f, 0f);

	private Vector2 crdMngMemberClanJob = new Vector2(300f, 0f);

	private Vector2 crdMngMemberFoot = new Vector2(364f, 0f);

	private Rect crdExile = new Rect(551f, 553f, 100f, 34f);

	private Rect crdDelegate = new Rect(659f, 553f, 100f, 34f);

	private Rect crdPromote = new Rect(767f, 553f, 100f, 34f);

	private Rect crdDemote = new Rect(875f, 553f, 100f, 34f);

	private Rect crdApprove = new Rect(767f, 553f, 100f, 34f);

	private Rect crdRefuse = new Rect(875f, 553f, 100f, 34f);

	private Vector2 crdMngInfoName = new Vector2(245f, 140f);

	private Vector2 crdMngInfoMaster = new Vector2(245f, 155f);

	private Vector2 crdMngInfoRank = new Vector2(245f, 170f);

	private Vector2 crdMngInfoRecord = new Vector2(245f, 185f);

	private Vector2 crdMngInfoMembers = new Vector2(245f, 200f);

	private Vector2 crdMngInfoNameLabel = new Vector2(145f, 140f);

	private Vector2 crdMngInfoMasterLabel = new Vector2(145f, 155f);

	private Vector2 crdMngInfoRankLabel = new Vector2(145f, 170f);

	private Vector2 crdMngInfoRecordLabel = new Vector2(145f, 185f);

	private Vector2 crdMngInfoMembersLabel = new Vector2(145f, 200f);

	private Vector2 crdMngInform = new Vector2(43f, 248f);

	private Vector2 crdMngPromote = new Vector2(43f, 382f);

	private Vector2 crdMngRankingTitle = new Vector2(599f, 137f);

	private Vector2 crdMngNameTitle = new Vector2(739f, 137f);

	private Vector2 crdMngClanPositionTitle = new Vector2(871f, 137f);

	private Vector2 crdMngClanCnntTitle = new Vector2(936f, 137f);

	private Rect crdMngRankingDropDn = new Rect(588f, 149f, 25f, 23f);

	private Rect crdMngNameDropDn = new Rect(728f, 149f, 25f, 23f);

	private Rect crdMngClanPositionDropDn = new Rect(861f, 149f, 25f, 23f);

	private Rect crdMngClanCnntDropDn = new Rect(927f, 149f, 25f, 23f);

	private Rect crdMyClanInfoMark = new Rect(49f, 160f, 64f, 64f);

	private Rect crdMyClanInfoGeneral = new Rect(27f, 126f, 495f, 148f);

	private Rect crdMyClanInfoNotice = new Rect(27f, 310f, 495f, 96f);

	private Rect crdMyClanInfoIntro = new Rect(27f, 442f, 495f, 133f);

	public Rect crdMyClanMember = new Rect(426f, 52f, 316f, 422f);

	public Rect crdMyClanMemberBox = new Rect(426f, 52f, 316f, 26f);

	public Vector2 crdMyClanMemberLabel = new Vector2(530f, 60f);

	public float crdMyClanMemberItemHeight = 28f;

	private float crdMyClanMemberItemWidth = 414f;

	private Rect crdMyClanMemberListPosition = new Rect(560f, 180f, 414f, 360f);

	private Vector2 crdMyClanMemberRoyalty = new Vector2(20f, 0f);

	private Vector2 crdMyClanMemberBadge = new Vector2(102f, 0f);

	private Vector2 crdMyClanMemberNickname = new Vector2(152f, 0f);

	private Vector2 crdMyClanMemberClanJob = new Vector2(300f, 0f);

	private Vector2 crdMyClanMemberFoot = new Vector2(364f, 0f);

	private Rect crdMyClanLeave = new Rect(852f, 553f, 126f, 34f);

	private Vector2 crdMyClanInfoName = new Vector2(245f, 150f);

	private Vector2 crdMyClanInfoMaster = new Vector2(245f, 165f);

	private Vector2 crdMyClanInfoRank = new Vector2(245f, 180f);

	private Vector2 crdMyClanInfoRecord = new Vector2(245f, 195f);

	private Vector2 crdMyClanInfoMembers = new Vector2(245f, 210f);

	private Vector2 crdMyClanInfoDay = new Vector2(245f, 225f);

	private Vector2 crdMyClanInfoNameLabel = new Vector2(145f, 150f);

	private Vector2 crdMyClanInfoMasterLabel = new Vector2(145f, 165f);

	private Vector2 crdMyClanInfoRankLabel = new Vector2(145f, 180f);

	private Vector2 crdMyClanInfoRecordLabel = new Vector2(145f, 195f);

	private Vector2 crdMyClanInfoMembersLabel = new Vector2(145f, 210f);

	private Vector2 crdMyClanInfoDayLabel = new Vector2(145f, 225f);

	private Vector2 crdMyInform = new Vector2(43f, 288f);

	private Vector2 crdMyPromote = new Vector2(43f, 422f);

	private Vector2 crdMyRankingTitle = new Vector2(599f, 137f);

	private Vector2 crdMyNameTitle = new Vector2(739f, 137f);

	private Vector2 crdMyClanPositionTitle = new Vector2(871f, 137f);

	private Vector2 crdMyClanCnntTitle = new Vector2(936f, 137f);

	private Rect crdMyRankingDropDn = new Rect(588f, 149f, 25f, 23f);

	private Rect crdMyNameDropDn = new Rect(728f, 149f, 25f, 23f);

	private Rect crdMyClanPositionDropDn = new Rect(861f, 149f, 25f, 23f);

	private Rect crdMyClanCnntDropDn = new Rect(927f, 149f, 25f, 23f);

	private Rect crdCRMark = new Rect(49f, 160f, 64f, 64f);

	private Rect crdCRGeneral = new Rect(27f, 126f, 495f, 148f);

	private Rect crdCRGsb = new Rect(27f, 311f, 495f, 32f);

	private Rect crdCRRecord = new Rect(27f, 411f, 495f, 32f);

	public Rect crdCROutline = new Rect(16f, 146f, 357f, 250f);

	private Vector2 crdCRClanName = new Vector2(245f, 150f);

	private Vector2 crdCRClanMaster = new Vector2(245f, 165f);

	private Vector2 crdCRClanInfoRank = new Vector2(245f, 180f);

	private Vector2 crdCRClanInfoRecord = new Vector2(245f, 195f);

	private Vector2 crdCRClanInfoMembers = new Vector2(245f, 210f);

	private Vector2 crdCRClanInfoDay = new Vector2(245f, 225f);

	private Vector2 crdCRClanInfoNameLabel = new Vector2(145f, 150f);

	private Vector2 crdCRClanInfoMasterLabel = new Vector2(145f, 165f);

	private Vector2 crdCRClanInfoRankLabel = new Vector2(145f, 180f);

	private Vector2 crdCRClanInfoRecordLabel = new Vector2(145f, 195f);

	private Vector2 crdCRClanInfoMembersLabel = new Vector2(145f, 210f);

	private Vector2 crdCRClanInfoDayLabel = new Vector2(145f, 225f);

	private Vector2 crdCRClan_Info_Trophy = new Vector2(43f, 286f);

	private Vector2 crdCRClan_Info_Record = new Vector2(43f, 386f);

	private Vector2 crdCRClan_Gold_Title = new Vector2(133f, 315f);

	private Vector2 crdCRClan_Silver_Title = new Vector2(283f, 315f);

	private Vector2 crdCRClan_Copper_Title = new Vector2(433f, 315f);

	private Vector2 crdCRClan_OppClan_Title = new Vector2(121f, 415f);

	private Vector2 crdCRClan_Record_Title = new Vector2(263f, 415f);

	private Vector2 crdCRClan_WinLose_Title = new Vector2(360f, 415f);

	private Vector2 crdCRClan_Day_Title = new Vector2(458f, 415f);

	public Rect crdCRPrevPage = new Rect(163f, 444f, 13f, 17f);

	public Rect crdCRPageBox = new Rect(183f, 444f, 66f, 17f);

	public Vector2 crdCRPage = new Vector2(214f, 451f);

	public Rect crdCRNextPage = new Rect(258f, 444f, 13f, 17f);

	public Rect crdCRMatchOutline = new Rect(0f, 0f, 0f, 0f);

	public Rect crdCRMap = new Rect(0f, 0f, 0f, 0f);

	private Vector2 crdCRListLT = new Vector2(27f, 453f);

	private Vector2 crdCRListSize = new Vector2(483f, 33f);

	private Vector2 crdCRMarkLT = new Vector2(3f, 3f);

	private Vector2 crdCRMarkSize = new Vector2(22f, 22f);

	private Vector2 crdCREnemy = new Vector2(94f, 13f);

	private Vector2 crdCRRecord2 = new Vector2(236f, 13f);

	private Vector2 crdCRWinLose = new Vector2(301f, 5f);

	private Vector2 crdCRDate2 = new Vector2(431f, 13f);

	private Vector2 crdCRResultSize = new Vector2(55f, 23f);

	private float crdCRListOffset = 33f;

	private Rect crdCRMapThumbnail = new Rect(564f, 132f, 64f, 64f);

	private Vector2 crdCRMapNameLabel = new Vector2(700f, 150f);

	private Vector2 crdCRKindLabel = new Vector2(700f, 170f);

	private Vector2 crdCRResultLabel = new Vector2(700f, 190f);

	private Vector2 crdCRDateLabel = new Vector2(700f, 210f);

	private Rect crdCRPlayerList = new Rect(558f, 252f, 425f, 323f);

	private Vector2 crdCROurPanelSize = new Vector2(414f, 64f);

	private Vector2 crdCREnemyPanelSize = new Vector2(414f, 64f);

	private float crdCRPlayerOffset = 22f;

	private float crdCRPlayerBadgeX = 2f;

	private float crdCRPlayerNicknameX = 136f;

	private float crdCRPlayerRecordX = 283f;

	private float crdCRPlayerScoreX = 376f;

	private float crdCRPlayerColumnY = 50f;

	private float crdCRPlayerBadgeColymnX = 22f;

	public Vector2 crdCRPlayerBadgeSize = new Vector2(34f, 17f);

	private int curTab;

	private float refreshDelta;

	private string searchKey = string.Empty;

	private string lastSearchKey = string.Empty;

	private int curClan;

	private string[] tab4Staff;

	private string[] tab4Member;

	private string[] tab4VirtualMember;

	private string[] tab4Somebody;

	private string[] tab4Mng;

	private int clPage = 1;

	private bool bAsceding;

	private Vector2 spClanIntro = Vector2.zero;

	private Vector2 spMyClanMember = Vector2.zero;

	private Vector2 spMyClanInfoIntro = Vector2.zero;

	private Vector2 spMyClanInfoNotice = Vector2.zero;

	private int curMyClanMember;

	private int crPage = 1;

	private int curClanMatch;

	private List<CMR> listCmr;

	private Vector2 spCMPlayer = Vector2.zero;

	private Vector2 spMngMember = Vector2.zero;

	private Vector2 spMngApplicant = Vector2.zero;

	private Vector2 spMngInfoIntro = Vector2.zero;

	private Vector2 spMngInfoNotice = Vector2.zero;

	private int curMngApplicant;

	private int curMngClanMember;

	private int curMngTab;

	private string[] clanLevelName;

	private Dictionary<int, Clan> dicClan;

	private Dictionary<string, ClanReq> dicClanReq;

	private Dictionary<int, ClanMemberCard> dicClanMember;

	private Dictionary<string, ClanApplicant> dicApplicant;

	private Clan selClan;

	private ClanReq selClanReq;

	private string curClanIntro = string.Empty;

	private ClanMemberCard selMngClanMember;

	private ClanApplicant selClanReqApplicant;

	private string myClanIntro = string.Empty;

	private string myClanNotice = string.Empty;

	private int myClanWinCount;

	private int myClanDrawCount;

	private int myClanLoseCount;

	private int myClanRank;

	private int myClanMemberCount;

	private int myClanGlod;

	private int myClanSilver;

	private int myClanBronze;

	private string myClanMaster = string.Empty;

	private CLANSORT sortBy;

	private float deltaTime;

	public string MyClanIntro
	{
		set
		{
			myClanIntro = value;
		}
	}

	public string MyClanNotice
	{
		set
		{
			myClanNotice = value;
		}
	}

	private string MyClanRecordString()
	{
		return myClanWinCount.ToString() + "-" + myClanDrawCount.ToString() + "-" + myClanLoseCount.ToString();
	}

	private string MyClanMemberCountString()
	{
		return myClanMemberCount.ToString();
	}

	private string MyClanRankString()
	{
		if (myClanRank < 0)
		{
			return "-";
		}
		return myClanRank.ToString();
	}

	public override void Start()
	{
		dicClan = new Dictionary<int, Clan>();
		dicClanReq = new Dictionary<string, ClanReq>();
		dicClanMember = new Dictionary<int, ClanMemberCard>();
		dicApplicant = new Dictionary<string, ClanApplicant>();
		listCmr = new List<CMR>();
		id = DialogManager.DIALOG_INDEX.CLAN;
		bAsceding = true;
		clanLevelName = new string[clanLevelKey.Length];
		for (int i = 0; i < clanLevelKey.Length; i++)
		{
			clanLevelName[i] = StringMgr.Instance.Get(clanLevelKey[i]);
		}
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog()
	{
		IsRankSet = false;
		IsCreateDaySet = false;
		tab4Staff = new string[tab4StaffKey.Length];
		for (int i = 0; i < tab4StaffKey.Length; i++)
		{
			tab4Staff[i] = StringMgr.Instance.Get(tab4StaffKey[i]);
		}
		tab4Member = new string[tab4MemberKey.Length];
		for (int j = 0; j < tab4MemberKey.Length; j++)
		{
			tab4Member[j] = StringMgr.Instance.Get(tab4MemberKey[j]);
		}
		tab4VirtualMember = new string[tab4VirtualMemberKey.Length];
		for (int k = 0; k < tab4VirtualMemberKey.Length; k++)
		{
			tab4VirtualMember[k] = StringMgr.Instance.Get(tab4VirtualMemberKey[k]);
		}
		tab4Somebody = new string[tab4SomebodyKey.Length];
		for (int l = 0; l < tab4SomebodyKey.Length; l++)
		{
			tab4Somebody[l] = StringMgr.Instance.Get(tab4SomebodyKey[l]);
		}
		tab4Mng = new string[tab4MngKey.Length];
		for (int m = 0; m < tab4MngKey.Length; m++)
		{
			tab4Mng[m] = StringMgr.Instance.Get(tab4MngKey[m]);
		}
		deltaTime = 0f;
		curTab = 0;
		clPage = 1;
		dicClan.Clear();
		dicClanReq.Clear();
		CSNetManager.Instance.Sock.SendCS_CLAN_LIST_REQ(-1, 1, -1, string.Empty);
		if (MyInfoManager.Instance.ClanLv != 0)
		{
			CSNetManager.Instance.Sock.SendCS_CLAN_APPLY_LIST_REQ();
		}
		crPage = 1;
		curClanMatch = 0;
		spCMPlayer = Vector2.zero;
		RefreshClanMatchList();
		RefreshMyClan();
	}

	public void OnEnterClan()
	{
		crPage = 1;
		curClanMatch = 0;
		spCMPlayer = Vector2.zero;
		RefreshClanMatchList();
		RefreshMyClan();
	}

	public void ThisClanHasNoClanMatchRecord()
	{
		listCmr.Clear();
	}

	public void AddClanMatchStart(int page)
	{
		crPage = page;
		listCmr.Clear();
	}

	public void AddClanMatch(long seq, int map, int kind, int playerCount, int enemyMark, string enemy, int killCount, int deathCount, int result, int score, int goal, int yy, int mm, int dd)
	{
		listCmr.Add(new CMR(seq, MyInfoManager.Instance.ClanSeq, map, kind, playerCount, enemyMark, enemy, killCount, deathCount, result, score, goal, yy, mm, dd));
	}

	public void AddClanMatchEnd()
	{
		curClanMatch = 0;
		if (0 <= curClanMatch && curClanMatch < listCmr.Count)
		{
			CMR cMR = listCmr[curClanMatch];
			if (cMR != null && !cMR.PlayerList)
			{
				CSNetManager.Instance.Sock.SendCS_CLAN_MATCH_PLAYER_LIST_REQ(cMR.ClanMatch);
			}
		}
	}

	public bool AddClanMatchPlayer(long clanMatch, int clan, int xp, int rank, string nickname, int killCount, int deathCount, int assistCount, int score)
	{
		for (int i = 0; i < listCmr.Count; i++)
		{
			if (listCmr[i].ClanMatch == clanMatch && !listCmr[i].PlayerList)
			{
				listCmr[i].AddPlayer(clan, xp, rank, nickname, killCount, assistCount, deathCount, score);
				return true;
			}
		}
		return false;
	}

	public void AddClanMatchPlayerEnd(long clanMatch)
	{
		int num = 0;
		while (true)
		{
			if (num >= listCmr.Count)
			{
				return;
			}
			if (listCmr[num].ClanMatch == clanMatch)
			{
				break;
			}
			num++;
		}
		listCmr[num].PlayerList = true;
	}

	private void RefreshClanMatchList()
	{
		int clanSeq = MyInfoManager.Instance.ClanSeq;
		if (clanSeq >= 0)
		{
			CSNetManager.Instance.Sock.SendCS_CLAN_MATCH_RECORD_LIST_REQ(-1, 1, 0L, clanSeq);
		}
	}

	private void RefreshMyClan()
	{
		int clanSeq = MyInfoManager.Instance.ClanSeq;
		if (clanSeq >= 0)
		{
			dicClanMember.Clear();
			CSNetManager.Instance.Sock.SendCS_SELECT_CLAN_MEMBER_REQ(clanSeq);
			dicApplicant.Clear();
			CSNetManager.Instance.Sock.SendCS_SELECT_CLAN_APPLICANT_REQ(clanSeq);
			CSNetManager.Instance.Sock.SendCS_CLAN_DETAIL_REQ(clanSeq);
		}
	}

	private void DoTitle()
	{
		Vector2 pos = new Vector2(size.x / 2f, 15f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("CLAN").ToUpper(), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
	}

	private void _DoSearchPanel()
	{
		Rect position;
		Rect rc;
		if (MyInfoManager.Instance.IsClanMember)
		{
			position = crdSearchKey4Member;
			rc = crdSearchBtn4Member;
		}
		else
		{
			position = crdSearchKey;
			rc = crdSearchBtn;
			if (GlobalVars.Instance.MyButton(crdCreateBtn, StringMgr.Instance.Get("CLAN_CREATE"), "BtnAction"))
			{
				CSNetManager.Instance.Sock.SendCS_CLAN_NEED_CREATE_POINT_REQ();
				((CreateClanDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.CREATE_CLAN, exclusive: false))?.InitDialog();
			}
		}
		string text = searchKey;
		searchKey = GUI.TextField(position, searchKey);
		if (searchKey.Length > 10)
		{
			searchKey = text;
		}
		if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("DO_SEARCH"), "BtnAction") && CheckSearchKey())
		{
			lastSearchKey = searchKey;
			CSNetManager.Instance.Sock.SendCS_CLAN_LIST_REQ(-1, 1, -1, GetLastSearchKey());
		}
	}

	private string GetLastSearchKey()
	{
		if (lastSearchKey.Length <= 0)
		{
			return string.Empty;
		}
		return "%" + lastSearchKey + "%";
	}

	private bool CheckSearchKey()
	{
		searchKey.Trim();
		if (0 < searchKey.Length && searchKey.Length < 2)
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CHARACTERS2MORE"));
			return false;
		}
		return true;
	}

	private void _DoClanListPage()
	{
		int minClan = GetMinClan();
		int maxClan = GetMaxClan();
		if (GlobalVars.Instance.MyButton(crdSLPrevPage, string.Empty, "Left") && clPage > 1 && maxClan >= 0)
		{
			if (clPage <= 2)
			{
				CSNetManager.Instance.Sock.SendCS_CLAN_LIST_REQ(-1, 1, -1, GetLastSearchKey());
			}
			else
			{
				CSNetManager.Instance.Sock.SendCS_CLAN_LIST_REQ(clPage, clPage - 1, maxClan, GetLastSearchKey());
			}
		}
		GUI.Box(crdSLPageBox, string.Empty, "BoxTextBg");
		LabelUtil.TextOut(crdSLPage, clPage.ToString(), "MiniLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		if (GlobalVars.Instance.MyButton(crdSLNextPage, string.Empty, "Right") && minClan >= 0)
		{
			CSNetManager.Instance.Sock.SendCS_CLAN_LIST_REQ(clPage, clPage + 1, minClan, GetLastSearchKey());
		}
	}

	private void _DoClanInfo()
	{
		GUI.Box(crdClanInfoGeneral, string.Empty, "BoxFadeBlue");
		string text = StringMgr.Instance.Get("APPLY");
		Rect rect = crdClanInfoIntro4Member;
		if (!MyInfoManager.Instance.IsClanMember)
		{
			rect = crdClanInfoIntro;
			if (GlobalVars.Instance.MyButton(crdApply, text, "BtnAction"))
			{
				if (selClan == null)
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("SELECT_CLAN"));
				}
				else
				{
					CSNetManager.Instance.Sock.SendCS_APPLY_CLAN_REQ(selClan.Seq, selClan.Name);
					if (MyInfoManager.Instance.ClanLv != 0)
					{
						CSNetManager.Instance.Sock.SendCS_CLAN_APPLY_LIST_REQ();
					}
				}
			}
		}
		GUI.Box(rect, string.Empty, "BoxFadeBlue");
		if (selClan != null)
		{
			DrawClanMark(crdClanInfoMark, selClan.Mark);
			string name = selClan.Name;
			LabelUtil.TextOut(crdClanInfoName, name, "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdClanInfoMaster, selClan.ClanMaster, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdClanInfoRank, selClan.RankString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdClanInfoRecord, selClan.RecordString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdClanInfoMembers, selClan.MemberCountString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdClanInfoNameLabel, StringMgr.Instance.Get("CLAN_NAME") + ":", "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdClanInfoMasterLabel, StringMgr.Instance.Get("CALN_MASTER") + ":", "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdClanInfoRankLabel, StringMgr.Instance.Get("RANK") + ":", "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdClanInfoRecordLabel, StringMgr.Instance.Get("RECORD") + ":", "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdClanInfoMembersLabel, StringMgr.Instance.Get("CLAN_NUM_MEMBERS") + ":", "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string str = StringMgr.Instance.Get("CALN_INFO_TROPHY") + "(" + StringMgr.Instance.Get("CLAN_INFO_RECORD_LIST_GOLD");
			str = str + "/" + StringMgr.Instance.Get("CLAN_INFO_RECORD_LIST_SILVER");
			str = str + "/" + StringMgr.Instance.Get("CLAN_INFO_RECORD_LIST_COPPER") + ")";
			LabelUtil.TextOut(crdTrophyLabel, str, "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdTrophyResult, selClan.GoldSilverBronzeString(), "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			GUILayout.BeginArea(rect);
			spClanIntro = GUILayout.BeginScrollView(spClanIntro, GUILayout.Width(rect.width), GUILayout.Height(rect.height));
			GUILayout.Label(curClanIntro);
			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}
	}

	private void _DoClanReqInfo()
	{
		GUI.Box(crdClanInfoGeneral, string.Empty, "BoxFadeBlue");
		string text = StringMgr.Instance.Get("CLAN_SIGNUP_CANCLE");
		Rect rect = crdClanInfoIntro4Member;
		if (!MyInfoManager.Instance.IsClanMember)
		{
			rect = crdClanInfoIntro;
			if (GlobalVars.Instance.MyButton(crdApply, text, "BtnAction"))
			{
				if (selClan == null)
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("SELECT_CLAN"));
				}
				else
				{
					CSNetManager.Instance.Sock.SendCS_CLAN_CANCEL_APPLICATION_REQ(selClanReq.Name);
				}
			}
		}
		GUI.Box(rect, string.Empty, "BoxFadeBlue");
		if (selClanReq != null)
		{
			DrawClanMark(crdClanInfoMark, selClanReq.Mark);
			string name = selClanReq.Name;
			LabelUtil.TextOut(crdClanInfoName, name, "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdClanInfoMaster, selClanReq.ClanMaster, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdClanInfoRank, selClanReq.RankString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdClanInfoRecord, selClanReq.RecordString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdClanInfoMembers, selClanReq.MemberCountString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdClanInfoNameLabel, StringMgr.Instance.Get("CLAN_NAME") + ":", "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdClanInfoMasterLabel, StringMgr.Instance.Get("CALN_MASTER") + ":", "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdClanInfoRankLabel, StringMgr.Instance.Get("RANK") + ":", "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdClanInfoRecordLabel, StringMgr.Instance.Get("RECORD") + ":", "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdClanInfoMembersLabel, StringMgr.Instance.Get("CLAN_NUM_MEMBERS") + ":", "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string str = StringMgr.Instance.Get("CALN_INFO_TROPHY") + "(" + StringMgr.Instance.Get("CLAN_INFO_RECORD_LIST_GOLD");
			str = str + "/" + StringMgr.Instance.Get("CLAN_INFO_RECORD_LIST_SILVER");
			str = str + "/" + StringMgr.Instance.Get("CLAN_INFO_RECORD_LIST_COPPER") + ")";
			LabelUtil.TextOut(crdTrophyLabel, str, "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdTrophyResult, selClanReq.GoldSilverBronzeString(), "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			GUILayout.BeginArea(rect);
			spClanIntro = GUILayout.BeginScrollView(spClanIntro, GUILayout.Width(rect.width), GUILayout.Height(rect.height));
			GUILayout.Label(curClanIntro);
			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}
	}

	private void DoClanList()
	{
		_DoSearchPanel();
		GUI.Box(crClanListTitleTop, string.Empty, "BoxFadeBlue");
		LabelUtil.TextOut(crdSLRank, StringMgr.Instance.Get("RANK"), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(crdSLName, StringMgr.Instance.Get("CLAN_NAME"), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(crdSLRecord, StringMgr.Instance.Get("CLAN_RECORD"), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(crdSLNoMember, StringMgr.Instance.Get("CLAN_NUM_MEMBERS"), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(crdSLScore, StringMgr.Instance.Get("CLAN_LIST_SCORE"), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(crdSLDay, StringMgr.Instance.Get("CLAN_LIST_DAY"), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		Rect position = new Rect(crdSLLeftTop.x, crdSLLeftTop.y, crdLSItemWidth, crdLSItemHeight * (float)dicClan.Count);
		List<Clan> list = new List<Clan>();
		int num = 0;
		string[] array = new string[dicClan.Count];
		foreach (KeyValuePair<int, Clan> item in dicClan)
		{
			array[num++] = string.Empty;
			list.Add(item.Value);
		}
		list.Sort((Clan first, Clan next) => first.Compare(next));
		int num3 = curClan;
		curClan = GUI.SelectionGrid(position, curClan, array, 1, "BoxGridStyle");
		selClan = ((0 > curClan || curClan >= list.Count) ? null : list[curClan]);
		if (curClan != num3 && selClan != null)
		{
			curClanIntro = string.Empty;
			CSNetManager.Instance.Sock.SendCS_SELECT_CLAN_INTRO_REQ(selClan.Seq);
		}
		float num4 = crdSLFirstY;
		for (int i = 0; i < list.Count; i++)
		{
			string text = "-";
			if (list[i].Rank > 0)
			{
				text = list[i].Rank.ToString();
			}
			LabelUtil.TextOut(new Vector2(crdSLRank.x, num4), text, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(crdSLName.x, num4), list[i].Name, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(crdSLRecord.x, num4), list[i].RecordString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(crdSLNoMember.x, num4), list[i].MemberCountString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(crdSLScore.x, num4), list[i].MatchPointString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(crdSLDay.x, num4), list[i].CeateDateString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			num4 += crdLSItemHeight;
		}
		_DoClanListPage();
		_DoClanInfo();
		IsRankSet = GUI.Toggle(crdCheckRank, IsRankSet, StringMgr.Instance.Get("CLAN_LIST_ALIGN_RANK"));
		IsCreateDaySet = GUI.Toggle(crdCheckCreateDay, IsCreateDaySet, StringMgr.Instance.Get("CLAN_LIST_ALIGN_DAY"));
		if (GlobalVars.Instance.MyButton(crdBtnRefresh, string.Empty, "BtnRefresh") && refreshDelta > refreshLimit)
		{
			refreshDelta = 0f;
			lastSearchKey = searchKey;
			CSNetManager.Instance.Sock.SendCS_CLAN_LIST_REQ(-1, 1, -1, GetLastSearchKey());
		}
	}

	private void DoVirtualClanList()
	{
		_DoSearchPanel();
		GUI.Box(crClanListTitleTop, string.Empty, "BoxFadeBlue");
		Vector2 pos = crdSLRank;
		pos.x += 70f;
		Vector2 pos2 = crdSLRank;
		pos2.x += 197f;
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("CLAN_NAME"), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(pos2, StringMgr.Instance.Get("CLAN_SIGNUP_DAY"), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		Rect position = new Rect(crdSLLeftTop.x, crdSLLeftTop.y, crdLSItemWidth, crdLSItemHeight * (float)dicClanReq.Count);
		List<ClanReq> list = new List<ClanReq>();
		int num = 0;
		string[] array = new string[dicClanReq.Count];
		foreach (KeyValuePair<string, ClanReq> item in dicClanReq)
		{
			array[num++] = string.Empty;
			list.Add(item.Value);
		}
		int num3 = curClan;
		curClan = GUI.SelectionGrid(position, curClan, array, 1, "BoxGridStyle");
		selClanReq = ((0 > curClan || curClan >= list.Count) ? null : list[curClan]);
		if (curClan != num3 && selClanReq != null)
		{
			curClanIntro = string.Empty;
			CSNetManager.Instance.Sock.SendCS_SELECT_CLAN_INTRO_REQ(selClanReq.Seq);
		}
		float num4 = crdSLFirstY;
		for (int i = 0; i < list.Count; i++)
		{
			LabelUtil.TextOut(new Vector2(pos.x, num4), list[i].Name, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(pos2.x, num4), list[i].GetDateToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			num4 += crdLSItemHeight;
		}
		_DoClanReqInfo();
		if (GlobalVars.Instance.MyButton(crdBtnRefresh, string.Empty, "BtnRefresh") && refreshDelta > refreshLimit)
		{
			refreshDelta = 0f;
			lastSearchKey = searchKey;
			CSNetManager.Instance.Sock.SendCS_CLAN_LIST_REQ(-1, 1, -1, GetLastSearchKey());
		}
	}

	public void ResetCurClan()
	{
		List<Clan> list = new List<Clan>();
		foreach (KeyValuePair<int, Clan> item in dicClan)
		{
			list.Add(item.Value);
		}
		curClan = 0;
		selClan = ((0 > curClan || curClan >= list.Count) ? null : list[curClan]);
		curClanIntro = string.Empty;
		if (selClan != null)
		{
			CSNetManager.Instance.Sock.SendCS_SELECT_CLAN_INTRO_REQ(selClan.Seq);
		}
	}

	public void UpdateCurClanIntro(int clan, string intro)
	{
		if (selClan != null && selClan.Seq == clan)
		{
			curClanIntro = intro;
		}
	}

	public void ResetClanListPage(int _page)
	{
		curClan = 0;
		selClan = null;
		curClanIntro = string.Empty;
		clPage = _page;
		dicClan.Clear();
	}

	public void AddClan(int clan, int mark, string clanName, int winCount, int drawCount, int loseCount, int memberCount, int rank, int rankChg, int matchPoint, int year, int month, int day, int gold, int silver, int bronze, string clanMaster)
	{
		if (!dicClan.ContainsKey(clan))
		{
			dicClan.Add(clan, new Clan(clan, mark, clanName, winCount, drawCount, loseCount, memberCount, rank, rankChg, matchPoint, year, month, day, gold, silver, bronze, clanMaster));
		}
		else
		{
			dicClan[clan].Mark = mark;
			dicClan[clan].Name = clanName;
			dicClan[clan].WinCount = winCount;
			dicClan[clan].DrawCount = drawCount;
			dicClan[clan].LoseCount = loseCount;
			dicClan[clan].NoMember = memberCount;
			dicClan[clan].Rank = rank;
			dicClan[clan].RankChg = rankChg;
			dicClan[clan].MatchPoint = matchPoint;
			dicClan[clan].ClanMaster = clanMaster;
		}
	}

	public void AddClanReq(int clan, string clanName, int year, int month, int day)
	{
		if (!dicClanReq.ContainsKey(clanName))
		{
			Clan clan2 = dicClan[clan];
			dicClanReq.Add(clanName, new ClanReq(clan, clan2.Mark, clanName, clan2.WinCount, clan2.DrawCount, clan2.LoseCount, clan2.NoMember, clan2.Rank, year, month, day, clan2.gold, clan2.silver, clan2.bronze, clan2.ClanMaster));
		}
		else
		{
			dicClanReq[clanName].Name = clanName;
		}
	}

	public void RemoveClanReq(string clanName)
	{
		if (dicClanReq.ContainsKey(clanName))
		{
			dicClanReq.Remove(clanName);
		}
	}

	public void AddClanApplicant(int clan, string nickName, int year, int month, int day)
	{
		if (!dicApplicant.ContainsKey(nickName))
		{
			dicApplicant.Add(nickName, new ClanApplicant(clan, nickName, year, month, day));
		}
		else
		{
			dicApplicant[nickName].Name = nickName;
		}
	}

	public void AddClanMember(int player, string nickname, int xp, int rank, int clanLv, int clanRoyalty, int clanPoint)
	{
		if (player != MyInfoManager.Instance.Seq)
		{
			int level = XpManager.Instance.GetLevel(xp);
			if (!dicClanMember.ContainsKey(player))
			{
				dicClanMember.Add(player, new ClanMemberCard(player, nickname, level, rank, clanLv, clanRoyalty, clanPoint));
			}
			else
			{
				dicClanMember[player].Nickname = nickname;
				dicClanMember[player].Lv = level;
				dicClanMember[player].Rank = rank;
				dicClanMember[player].ClanLv = clanLv;
				dicClanMember[player].ClanRoyalty = clanRoyalty;
				dicClanMember[player].ClanPoint = clanPoint;
			}
		}
	}

	public void ClearClanReq()
	{
		if (dicClanReq != null)
		{
			dicClanReq.Clear();
		}
	}

	public void DelClanMember(int player)
	{
		if (dicClanMember.ContainsKey(player))
		{
			dicClanMember.Remove(player);
		}
	}

	public void DelClanApplicant(string nickName)
	{
		if (dicApplicant.ContainsKey(nickName))
		{
			dicApplicant.Remove(nickName);
		}
	}

	public void DelClanReq()
	{
		if (dicClanReq.ContainsKey(selClanReq.Name))
		{
			dicClanReq.Remove(selClanReq.Name);
		}
	}

	public void UpdateClanMemberLevel(int player, int clanLv)
	{
		if (dicClanMember.ContainsKey(player))
		{
			dicClanMember[player].ClanLv = clanLv;
		}
	}

	private void _DoMyClanMember()
	{
		LabelUtil.TextOut(crdMyRankingTitle, StringMgr.Instance.Get("CLAN_LIST_SCORE"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(crdMyNameTitle, StringMgr.Instance.Get("NAME"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(crdMyClanPositionTitle, StringMgr.Instance.Get("CLAN_POSITION"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(crdMyClanCnntTitle, StringMgr.Instance.Get("CLAN_ACCESS_CHECK"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		List<ClanMemberCard> list = new List<ClanMemberCard>();
		int num = 0;
		string[] array = new string[dicClanMember.Count];
		foreach (KeyValuePair<int, ClanMemberCard> item in dicClanMember)
		{
			array[num++] = string.Empty;
			list.Add(item.Value);
		}
		if (GlobalVars.Instance.MyButton(crdMyRankingDropDn, string.Empty, "BtnArrowDn"))
		{
			bAsceding = !bAsceding;
			sortBy = CLANSORT.POINT;
		}
		if (GlobalVars.Instance.MyButton(crdMyNameDropDn, string.Empty, "BtnArrowDn"))
		{
			bAsceding = !bAsceding;
			sortBy = CLANSORT.NAME;
		}
		if (GlobalVars.Instance.MyButton(crdMyClanPositionDropDn, string.Empty, "BtnArrowDn"))
		{
			bAsceding = !bAsceding;
			sortBy = CLANSORT.LV;
		}
		if (GlobalVars.Instance.MyButton(crdMyClanCnntDropDn, string.Empty, "BtnArrowDn"))
		{
			bAsceding = !bAsceding;
			sortBy = CLANSORT.CNNT;
		}
		list.Sort((ClanMemberCard first, ClanMemberCard next) => first.Compare(next, sortBy, bAsceding));
		Rect rect = new Rect(0f, -4f, crdMyClanMemberItemWidth, crdMyClanMemberItemHeight * (float)list.Count);
		spMyClanMember = GUI.BeginScrollView(crdMyClanMemberListPosition, spMyClanMember, rect);
		curMyClanMember = GUI.SelectionGrid(rect, curMyClanMember, array, 1, "BoxGridStyle");
		float y = spMyClanMember.y;
		float num3 = y + crdMyClanMemberListPosition.height;
		Vector2 zero = Vector2.zero;
		for (int i = 0; i < list.Count; i++)
		{
			if (zero.y >= num3 || zero.y + crdMyClanMemberItemHeight <= y)
			{
				zero.y += crdMyClanMemberItemHeight;
			}
			else
			{
				LabelUtil.TextOut(zero + crdMyClanMemberRoyalty, list[i].ClanPoint.ToString(), "MiniLabel", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
				Texture2D badge = XpManager.Instance.GetBadge(list[i].Lv, list[i].Rank);
				if (null != badge)
				{
					TextureUtil.DrawTexture(new Rect(zero.x + crdMyClanMemberBadge.x, zero.y + crdMyClanMemberBadge.y, (float)badge.width, (float)badge.height), badge);
				}
				LabelUtil.TextOut(zero + crdMyClanMemberNickname, list[i].Nickname, "MiniLabel", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
				LabelUtil.TextOut(zero + crdMyClanMemberClanJob, clanLevelName[list[i].ClanLv], "MiniLabel", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
				NameCard clanee = MyInfoManager.Instance.GetClanee(list[i].Seq);
				Texture2D image = grayFoot;
				if (clanee != null && clanee.IsConnected)
				{
					image = colorFoot;
				}
				TextureUtil.DrawTexture(new Rect(zero.x + crdMyClanMemberFoot.x, zero.y + crdMyClanMemberFoot.y, (float)colorFoot.width, (float)colorFoot.height), image);
				zero.y += crdMyClanMemberItemHeight;
			}
		}
		GUI.EndScrollView();
		if (GlobalVars.Instance.MyButton(crdMyClanLeave, StringMgr.Instance.Get("CLAN_LEAVE"), "BtnAction"))
		{
			if (MyInfoManager.Instance.ClanLv == 2)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CLAN_MASTER_CANNOT_LEAVE"));
			}
			else
			{
				((ClanConfirmDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.CLAN_CONFIRM, exclusive: false))?.InitDialog(ClanConfirmDialog.CLAN_CONFIRM_WHAT.LEAVE_CLAN, -1, string.Empty, string.Empty);
			}
		}
		if (GlobalVars.Instance.MyButton(crdBtnRefresh, string.Empty, "BtnRefresh") && refreshDelta > refreshLimit)
		{
			refreshDelta = 0f;
			RefreshMyClan();
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
				TextureUtil.DrawTexture(rc, bg, ScaleMode.StretchToFill);
			}
			Color color = GUI.color;
			GUI.color = colorValue;
			if (null != amblum)
			{
				TextureUtil.DrawTexture(rc, amblum, ScaleMode.StretchToFill);
			}
			GUI.color = color;
		}
	}

	private void DoMyClan()
	{
		GUI.Box(crdMyClanInfoGeneral, string.Empty, "BoxFadeBlue");
		GUI.Box(crdMyClanInfoIntro, string.Empty, "BoxInnerLine");
		GUI.Box(crdMyClanInfoNotice, string.Empty, "BoxInnerLine");
		DrawClanMark(crdMyClanInfoMark, MyInfoManager.Instance.ClanMark);
		LabelUtil.TextOut(crdMyClanInfoName, MyInfoManager.Instance.ClanName, "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMyClanInfoMaster, myClanMaster, "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMyClanInfoRank, MyClanRankString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMyClanInfoRecord, MyClanRecordString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMyClanInfoMembers, MyClanMemberCountString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMyClanInfoDay, MyClanMemberCountString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMyClanInfoNameLabel, StringMgr.Instance.Get("CLAN_NAME") + ":", "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMyClanInfoMasterLabel, StringMgr.Instance.Get("CALN_MASTER") + ":", "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMyClanInfoRankLabel, StringMgr.Instance.Get("RANK") + ":", "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMyClanInfoRecordLabel, StringMgr.Instance.Get("RECORD") + ":", "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMyClanInfoMembersLabel, StringMgr.Instance.Get("CLAN_NUM_MEMBERS") + ":", "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMyClanInfoDayLabel, StringMgr.Instance.Get("CLAN_LIST_DAY") + ":", "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMyInform, StringMgr.Instance.Get("CLAN_NOTICE"), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMyPromote, StringMgr.Instance.Get("CALN_INFO_PROMOTION"), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		Rect screenRect = crdMyClanInfoIntro;
		GUILayout.BeginArea(screenRect);
		spMyClanInfoIntro = GUILayout.BeginScrollView(spMyClanInfoIntro, GUILayout.Width(screenRect.width), GUILayout.Height(screenRect.height));
		GUILayout.Label(myClanIntro);
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		Rect screenRect2 = crdMyClanInfoNotice;
		GUILayout.BeginArea(screenRect2);
		spMyClanInfoNotice = GUILayout.BeginScrollView(spMyClanInfoNotice, GUILayout.Width(screenRect2.width), GUILayout.Height(screenRect2.height));
		GUILayout.Label(myClanNotice);
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		_DoMyClanMember();
	}

	private void _DoClanMatchList()
	{
		Rect position = new Rect(crdCRListLT.x, crdCRListLT.y, crdCRListSize.x, crdCRListSize.y * (float)listCmr.Count);
		string[] array = new string[listCmr.Count];
		for (int i = 0; i < listCmr.Count; i++)
		{
			array[i] = string.Empty;
		}
		int num = curClanMatch;
		curClanMatch = GUI.SelectionGrid(position, curClanMatch, array, 1, "BtnAction");
		for (int j = 0; j < listCmr.Count; j++)
		{
			DrawClanMark(new Rect(crdCRListLT.x + crdCRMarkLT.x, crdCRListLT.y + crdCRMarkLT.y + (float)j * crdCRListOffset, crdCRMarkSize.x, crdCRMarkSize.y), listCmr[j].EnemyMark);
			LabelUtil.TextOut(new Vector2(crdCRListLT.x + crdCREnemy.x, crdCRListLT.y + crdCREnemy.y + (float)j * crdCRListOffset + 3f), listCmr[j].Enemy, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(crdCRListLT.x + crdCRRecord2.x, crdCRListLT.y + crdCRRecord2.y + (float)j * crdCRListOffset + 3f), listCmr[j].GetMiniResultString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			int result = listCmr[j].Result;
			Rect position2 = new Rect(crdCRListLT.x + crdCRWinLose.x, crdCRListLT.y + crdCRWinLose.y + (float)j * crdCRListOffset, crdCRResultSize.x, crdCRResultSize.y);
			if (result < 0)
			{
				TextureUtil.DrawTexture(position2, lose);
			}
			else if (result > 0)
			{
				TextureUtil.DrawTexture(position2, win);
			}
			else
			{
				TextureUtil.DrawTexture(position2, draw);
			}
			LabelUtil.TextOut(new Vector2(crdCRListLT.x + crdCRDate2.x, crdCRListLT.y + crdCRDate2.y + (float)j * crdCRListOffset + 3f), listCmr[j].GetDateString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
		if (num != curClanMatch && 0 <= curClanMatch && curClanMatch < listCmr.Count)
		{
			CMR cMR = listCmr[curClanMatch];
			if (cMR != null && !cMR.PlayerList)
			{
				CSNetManager.Instance.Sock.SendCS_CLAN_MATCH_PLAYER_LIST_REQ(cMR.ClanMatch);
			}
		}
	}

	private void _DoClanMatchInfo()
	{
		if (0 <= curClanMatch && curClanMatch < listCmr.Count)
		{
			RegMap regMap = RegMapManager.Instance.Get(listCmr[curClanMatch].Map);
			if (regMap != null)
			{
				if (null != regMap.Thumbnail)
				{
					TextureUtil.DrawTexture(crdCRMapThumbnail, regMap.Thumbnail, ScaleMode.StretchToFill);
				}
				LabelUtil.TextOut(crdCRMapNameLabel, regMap.Alias, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
			}
			LabelUtil.TextOut(crdCRKindLabel, listCmr[curClanMatch].GetKindString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
			LabelUtil.TextOut(crdCRResultLabel, listCmr[curClanMatch].GetResultString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
			LabelUtil.TextOut(crdCRDateLabel, listCmr[curClanMatch].GetDateString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		}
	}

	private void _DoClanMatchPlayer()
	{
		if (listCmr != null && 0 <= curClanMatch && curClanMatch < listCmr.Count)
		{
			CMR cMR = listCmr[curClanMatch];
			if (cMR != null)
			{
				CMPlayer[] ourPlayersArray = cMR.GetOurPlayersArray();
				CMPlayer[] enemyPlayersArray = cMR.GetEnemyPlayersArray();
				spCMPlayer = GUI.BeginScrollView(viewRect: new Rect(0f, 0f, crdCROurPanelSize.x, crdCROurPanelSize.y + crdCREnemyPanelSize.y + (float)(ourPlayersArray.Length + enemyPlayersArray.Length) * crdCRPlayerOffset), position: crdCRPlayerList, scrollPosition: spCMPlayer);
				Vector2 zero = Vector2.zero;
				GUI.Box(new Rect(zero.x, zero.y, crdCROurPanelSize.x, crdCROurPanelSize.y), string.Empty, "BoxInnerLine");
				LabelUtil.TextOut(new Vector2(zero.x + 60f, zero.y + 5f), "[" + MyInfoManager.Instance.ClanName + "]", "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
				string text = "Draw";
				if (cMR.Result < 0)
				{
					text = "Lose";
				}
				else if (cMR.Result > 0)
				{
					text = "Win";
				}
				LabelUtil.TextOut(new Vector2(zero.x + crdCROurPanelSize.x - 10f, zero.y + 5f), text, "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				LabelUtil.TextOut(new Vector2(zero.x + crdCRPlayerBadgeColymnX, zero.y + crdCRPlayerColumnY), StringMgr.Instance.Get("BADGE"), "MiniLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(zero.x + crdCRPlayerNicknameX, zero.y + crdCRPlayerColumnY), StringMgr.Instance.Get("CHARACTER"), "MiniLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(zero.x + crdCRPlayerRecordX, zero.y + crdCRPlayerColumnY), StringMgr.Instance.Get("KILL") + "/" + StringMgr.Instance.Get("ASSIST") + "/" + StringMgr.Instance.Get("DEATH"), "MiniLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(zero.x + crdCRPlayerScoreX, zero.y + crdCRPlayerColumnY), StringMgr.Instance.Get("SCORE"), "MiniLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				zero.y += crdCROurPanelSize.y;
				for (int i = 0; i < ourPlayersArray.Length; i++)
				{
					Texture2D badge = XpManager.Instance.GetBadge(XpManager.Instance.GetLevel(ourPlayersArray[i].Xp), ourPlayersArray[i].Rank);
					if (null != badge)
					{
						TextureUtil.DrawTexture(new Rect(zero.x + crdCRPlayerBadgeX, zero.y + 3f, crdCRPlayerBadgeSize.x, crdCRPlayerBadgeSize.y), badge, ScaleMode.StretchToFill);
					}
					LabelUtil.TextOut(new Vector2(zero.x + crdCRPlayerNicknameX, zero.y), ourPlayersArray[i].Nickname, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
					LabelUtil.TextOut(new Vector2(zero.x + crdCRPlayerRecordX, zero.y), ourPlayersArray[i].Record, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
					LabelUtil.TextOut(new Vector2(zero.x + crdCRPlayerScoreX, zero.y), ourPlayersArray[i].Score, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
					zero.y += crdCRPlayerOffset;
				}
				GUI.Box(new Rect(zero.x, zero.y, crdCREnemyPanelSize.x, crdCREnemyPanelSize.y), string.Empty, "BoxInnerLine");
				LabelUtil.TextOut(new Vector2(zero.x + 60f, zero.y + 5f), "[" + cMR.Enemy + "]", "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
				string text2 = "Draw";
				if (cMR.Result < 0)
				{
					text2 = "Win";
				}
				else if (cMR.Result > 0)
				{
					text2 = "Lose";
				}
				LabelUtil.TextOut(new Vector2(zero.x + crdCREnemyPanelSize.x - 10f, zero.y + 5f), text2, "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				LabelUtil.TextOut(new Vector2(zero.x + crdCRPlayerBadgeColymnX, zero.y + crdCRPlayerColumnY), StringMgr.Instance.Get("BADGE"), "MiniLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(zero.x + crdCRPlayerNicknameX, zero.y + crdCRPlayerColumnY), StringMgr.Instance.Get("CHARACTER"), "MiniLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(zero.x + crdCRPlayerRecordX, zero.y + crdCRPlayerColumnY), StringMgr.Instance.Get("KILL") + "/" + StringMgr.Instance.Get("ASSIST") + "/" + StringMgr.Instance.Get("DEATH"), "MiniLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(zero.x + crdCRPlayerScoreX, zero.y + crdCRPlayerColumnY), StringMgr.Instance.Get("SCORE"), "MiniLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				zero.y += crdCREnemyPanelSize.y;
				for (int j = 0; j < ourPlayersArray.Length; j++)
				{
					Texture2D badge2 = XpManager.Instance.GetBadge(XpManager.Instance.GetLevel(enemyPlayersArray[j].Xp), enemyPlayersArray[j].Rank);
					if (null != badge2)
					{
						TextureUtil.DrawTexture(new Rect(zero.x + crdCRPlayerBadgeX, zero.y, crdCRPlayerBadgeSize.x, crdCRPlayerBadgeSize.y), badge2, ScaleMode.StretchToFill);
					}
					LabelUtil.TextOut(new Vector2(zero.x + crdCRPlayerNicknameX, zero.y), enemyPlayersArray[j].Nickname, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
					LabelUtil.TextOut(new Vector2(zero.x + crdCRPlayerRecordX, zero.y), enemyPlayersArray[j].Record, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
					LabelUtil.TextOut(new Vector2(zero.x + crdCRPlayerScoreX, zero.y), enemyPlayersArray[j].Score, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
					zero.y += crdCRPlayerOffset;
				}
				GUI.EndScrollView();
			}
		}
	}

	private void _DoCRPage()
	{
		if (GlobalVars.Instance.MyButton(crdCRPrevPage, string.Empty, "Left"))
		{
			long maxClanMatch = GetMaxClanMatch();
			int clanSeq = MyInfoManager.Instance.ClanSeq;
			if (clanSeq >= 0)
			{
				if (crPage <= 2)
				{
					CSNetManager.Instance.Sock.SendCS_CLAN_MATCH_RECORD_LIST_REQ(-1, 1, 0L, clanSeq);
				}
				else if (crPage > 1 && maxClanMatch >= 0)
				{
					CSNetManager.Instance.Sock.SendCS_CLAN_MATCH_RECORD_LIST_REQ(crPage, crPage - 1, maxClanMatch, clanSeq);
				}
			}
		}
		GUI.Box(crdCRPageBox, string.Empty, "BoxTextBg");
		LabelUtil.TextOut(crdCRPage, crPage.ToString(), "MiniLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		if (GlobalVars.Instance.MyButton(crdCRNextPage, string.Empty, "Right"))
		{
			long minClanMatch = GetMinClanMatch();
			int clanSeq2 = MyInfoManager.Instance.ClanSeq;
			if (clanSeq2 >= 0 && minClanMatch >= 0)
			{
				CSNetManager.Instance.Sock.SendCS_CLAN_MATCH_RECORD_LIST_REQ(crPage, crPage + 1, minClanMatch, clanSeq2);
			}
		}
	}

	private int GetMinClan()
	{
		int num = -1;
		foreach (KeyValuePair<int, Clan> item in dicClan)
		{
			num = ((num >= 0) ? Math.Min(num, item.Key) : item.Key);
		}
		return num;
	}

	private int GetMaxClan()
	{
		int num = -1;
		foreach (KeyValuePair<int, Clan> item in dicClan)
		{
			num = ((num >= 0) ? Math.Max(num, item.Key) : item.Key);
		}
		return num;
	}

	private long GetMinClanMatch()
	{
		long num = -1L;
		for (int i = 0; i < listCmr.Count; i++)
		{
			num = ((num >= 0) ? Math.Min(num, listCmr[i].ClanMatch) : listCmr[i].ClanMatch);
		}
		return num;
	}

	private long GetMaxClanMatch()
	{
		long num = -1L;
		for (int i = 0; i < listCmr.Count; i++)
		{
			num = ((num >= 0) ? Math.Max(num, listCmr[i].ClanMatch) : listCmr[i].ClanMatch);
		}
		return num;
	}

	private void DoClanRecord()
	{
		GUI.Box(crdCRGeneral, string.Empty, "BoxFadeBlue");
		DrawClanMark(crdCRMark, MyInfoManager.Instance.ClanMark);
		LabelUtil.TextOut(crdCRClanName, MyInfoManager.Instance.ClanName, "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdCRClanMaster, myClanMaster, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdCRClanInfoRank, MyClanRankString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdCRClanInfoRecord, MyClanRecordString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdCRClanInfoMembers, MyClanMemberCountString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdCRClanInfoDay, MyClanMemberCountString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdCRClanInfoNameLabel, StringMgr.Instance.Get("CLAN_NAME") + ":", "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdCRClanInfoMasterLabel, StringMgr.Instance.Get("CALN_MASTER") + ":", "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdCRClanInfoRankLabel, StringMgr.Instance.Get("RANK") + ":", "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdCRClanInfoRecordLabel, StringMgr.Instance.Get("RECORD") + ":", "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdCRClanInfoMembersLabel, StringMgr.Instance.Get("CLAN_NUM_MEMBERS") + ":", "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdCRClanInfoDayLabel, StringMgr.Instance.Get("CLAN_LIST_DAY") + ":", "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		GUI.Box(crdCRGsb, string.Empty, "BoxFadeBlue");
		LabelUtil.TextOut(crdCRClan_Info_Trophy, StringMgr.Instance.Get("CALN_INFO_TROPHY"), "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdCRClan_Gold_Title, StringMgr.Instance.Get("CLAN_INFO_RECORD_LIST_GOLD"), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		LabelUtil.TextOut(crdCRClan_Silver_Title, StringMgr.Instance.Get("CLAN_INFO_RECORD_LIST_SILVER"), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		LabelUtil.TextOut(crdCRClan_Copper_Title, StringMgr.Instance.Get("CLAN_INFO_RECORD_LIST_COPPER"), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		Vector2 pos = new Vector2(crdCRClan_Gold_Title.x, crdCRClan_Gold_Title.y + 30f);
		Vector2 pos2 = new Vector2(crdCRClan_Silver_Title.x, crdCRClan_Silver_Title.y + 30f);
		Vector2 pos3 = new Vector2(crdCRClan_Copper_Title.x, crdCRClan_Copper_Title.y + 30f);
		LabelUtil.TextOut(pos, myClanGlod.ToString(), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		LabelUtil.TextOut(pos2, myClanSilver.ToString(), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		LabelUtil.TextOut(pos3, myClanBronze.ToString(), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		GUI.Box(crdCRRecord, string.Empty, "BoxFadeBlue");
		LabelUtil.TextOut(crdCRClan_Info_Record, StringMgr.Instance.Get("CLAN_INFO_RECORD_LIST"), "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdCRClan_OppClan_Title, StringMgr.Instance.Get("CLAN_INFO_RECORD_LIST_OPPONENT"), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		LabelUtil.TextOut(crdCRClan_Record_Title, StringMgr.Instance.Get("RECORD"), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		LabelUtil.TextOut(crdCRClan_WinLose_Title, StringMgr.Instance.Get("CLAN_INFO_CLAN_RECORD_LIST_WINLOSE"), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		LabelUtil.TextOut(crdCRClan_Day_Title, StringMgr.Instance.Get("CLAN_INFO_CLAN_RECORD_LIST_DAY"), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		_DoClanMatchList();
		_DoClanMatchInfo();
		_DoClanMatchPlayer();
	}

	private void _DoMngApplicant()
	{
		if (dicApplicant.Count != 0)
		{
			List<ClanApplicant> list = new List<ClanApplicant>();
			int num = 0;
			string[] array = new string[dicApplicant.Count];
			foreach (KeyValuePair<string, ClanApplicant> item in dicApplicant)
			{
				array[num++] = string.Empty;
				list.Add(item.Value);
			}
			Rect rect = new Rect(0f, 0f, crdMngMemberItemWidth, crdMngMemberItemHeight * (float)list.Count);
			Rect position = new Rect(crdMngMemberListPosition);
			position.y -= 60f;
			position.height += 60f;
			spMngApplicant = GUI.BeginScrollView(position, spMngApplicant, rect);
			curMngApplicant = GUI.SelectionGrid(rect, curMngApplicant, array, 1, "BoxGridStyle");
			selClanReqApplicant = ((0 > curMngApplicant || curMngApplicant >= list.Count) ? null : list[curMngApplicant]);
			Vector2 zero = Vector2.zero;
			Vector2 b = new Vector2(50f, 5f);
			Vector2 b2 = new Vector2(200f, 5f);
			for (int i = 0; i < list.Count; i++)
			{
				LabelUtil.TextOut(zero + b, list[i].Name, "MiniLabel", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
				LabelUtil.TextOut(zero + b2, list[i].GetDateToString(), "MiniLabel", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
				zero.y += crdMngMemberItemHeight;
			}
			GUI.EndScrollView();
			if (MyInfoManager.Instance.IsClanStaff)
			{
				if (GlobalVars.Instance.MyButton(crdApprove, StringMgr.Instance.Get("APPROVE"), "BtnAction"))
				{
					if (selClanReqApplicant == null)
					{
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("SELECT_CLAN_MEMBER"));
					}
					else
					{
						string contents = "CLAN_ACCEPT_COMMENT" + GlobalVars.DELIMITER + "n" + MyInfoManager.Instance.ClanName;
						CSNetManager.Instance.Sock.SendCS_ACCEPT_APPLICANT_REQ(MyInfoManager.Instance.ClanSeq, selClanReqApplicant.Seq, selClanReqApplicant.Name, accept: true, "CLAN_ACCEPT", contents);
					}
				}
				if (GlobalVars.Instance.MyButton(crdRefuse, StringMgr.Instance.Get("REFUSE"), "BtnAction"))
				{
					if (selClanReqApplicant == null)
					{
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("SELECT_CLAN_MEMBER"));
					}
					else
					{
						string contents2 = "CLAN_REFUSED_COMMENT" + GlobalVars.DELIMITER + "n" + MyInfoManager.Instance.ClanName;
						CSNetManager.Instance.Sock.SendCS_ACCEPT_APPLICANT_REQ(MyInfoManager.Instance.ClanSeq, selClanReqApplicant.Seq, selClanReqApplicant.Name, accept: false, "CLAN_ACCEPT", contents2);
					}
				}
			}
		}
	}

	private void _DoMngClanMember()
	{
		LabelUtil.TextOut(crdMngRankingTitle, StringMgr.Instance.Get("CLAN_LIST_SCORE"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(crdMngNameTitle, StringMgr.Instance.Get("NAME"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(crdMngClanPositionTitle, StringMgr.Instance.Get("CLAN_POSITION"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(crdMngClanCnntTitle, StringMgr.Instance.Get("CLAN_ACCESS_CHECK"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		List<ClanMemberCard> list = new List<ClanMemberCard>();
		int num = 0;
		string[] array = new string[dicClanMember.Count];
		foreach (KeyValuePair<int, ClanMemberCard> item in dicClanMember)
		{
			array[num++] = string.Empty;
			list.Add(item.Value);
		}
		if (GlobalVars.Instance.MyButton(crdMngRankingDropDn, string.Empty, "BtnArrowDn"))
		{
			bAsceding = !bAsceding;
			sortBy = CLANSORT.POINT;
		}
		if (GlobalVars.Instance.MyButton(crdMngNameDropDn, string.Empty, "BtnArrowDn"))
		{
			bAsceding = !bAsceding;
			sortBy = CLANSORT.NAME;
		}
		if (GlobalVars.Instance.MyButton(crdMngClanPositionDropDn, string.Empty, "BtnArrowDn"))
		{
			bAsceding = !bAsceding;
			sortBy = CLANSORT.LV;
		}
		if (GlobalVars.Instance.MyButton(crdMngClanCnntDropDn, string.Empty, "BtnArrowDn"))
		{
			bAsceding = !bAsceding;
			sortBy = CLANSORT.CNNT;
		}
		list.Sort((ClanMemberCard first, ClanMemberCard next) => first.Compare(next, sortBy, bAsceding));
		Rect rect = new Rect(0f, 0f, crdMngMemberItemWidth, crdMngMemberItemHeight * (float)list.Count);
		spMngMember = GUI.BeginScrollView(crdMngMemberListPosition, spMngMember, rect);
		curMngClanMember = GUI.SelectionGrid(rect, curMngClanMember, array, 1, "BoxGridStyle");
		selMngClanMember = ((0 > curMngClanMember || curMngClanMember >= list.Count) ? null : list[curMngClanMember]);
		Vector2 zero = Vector2.zero;
		for (int i = 0; i < list.Count; i++)
		{
			LabelUtil.TextOut(zero + crdMngMemberRoyalty, list[i].ClanPoint.ToString(), "MiniLabel", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			Texture2D badge = XpManager.Instance.GetBadge(list[i].Lv, list[i].Rank);
			if (null != badge)
			{
				TextureUtil.DrawTexture(new Rect(zero.x + crdMngMemberBadge.x, zero.y + crdMngMemberBadge.y, (float)badge.width, (float)badge.height), badge);
			}
			LabelUtil.TextOut(zero + crdMngMemberNickname, list[i].Nickname, "MiniLabel", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(zero + crdMngMemberClanJob, clanLevelName[list[i].ClanLv], "MiniLabel", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			NameCard clanee = MyInfoManager.Instance.GetClanee(list[i].Seq);
			Texture2D image = grayFoot;
			if (clanee != null && clanee.IsConnected)
			{
				image = colorFoot;
			}
			TextureUtil.DrawTexture(new Rect(zero.x + crdMngMemberFoot.x, zero.y + crdMngMemberFoot.y, (float)colorFoot.width, (float)colorFoot.height), image);
			zero.y += crdMngMemberItemHeight;
		}
		GUI.EndScrollView();
		if (MyInfoManager.Instance.IsClanMaster)
		{
			if (GlobalVars.Instance.MyButton(crdExile, StringMgr.Instance.Get("EXILE"), "BtnAction"))
			{
				if (selMngClanMember == null)
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("SELECT_CLAN_MEMBER"));
				}
				else if (selMngClanMember.ClanLv > MyInfoManager.Instance.ClanLv)
				{
					MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("FAIL_TO_EXILE_NO_AUTH"), selMngClanMember.Nickname));
				}
				else
				{
					((ClanConfirmDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.CLAN_CONFIRM, exclusive: false))?.InitDialog(ClanConfirmDialog.CLAN_CONFIRM_WHAT.KICK_CLAN_MEMBER, selMngClanMember.Seq, selMngClanMember.Nickname, string.Empty);
				}
			}
			if (GlobalVars.Instance.MyButton(crdDelegate, StringMgr.Instance.Get("DELEGATE"), "BtnAction"))
			{
				if (selMngClanMember == null)
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("SELECT_CLAN_MEMBER"));
				}
				else
				{
					((ClanConfirmDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.CLAN_CONFIRM, exclusive: false))?.InitDialog(ClanConfirmDialog.CLAN_CONFIRM_WHAT.DELEGATE_MASTER, selMngClanMember.Seq, selMngClanMember.Nickname, clanLevelName[2]);
				}
			}
			if (GlobalVars.Instance.MyButton(crdPromote, StringMgr.Instance.Get("PROMOTE"), "BtnAction"))
			{
				if (selMngClanMember == null)
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("SELECT_CLAN_MEMBER"));
				}
				else if (selMngClanMember.ClanLv >= 1)
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NO_MORE_PROMOTE"));
				}
				else
				{
					int num3 = selMngClanMember.ClanLv + 1;
					string contents = "CLAN_PROMOTE_COMMENT" + GlobalVars.DELIMITER + "n" + MyInfoManager.Instance.ClanName + GlobalVars.DELIMITER + "n" + clanLevelName[num3];
					CSNetManager.Instance.Sock.SendCS_UP_CLAN_MEMBER_REQ(MyInfoManager.Instance.ClanSeq, selMngClanMember.Seq, selMngClanMember.Nickname, "CLAN_PROMOTE", contents);
				}
			}
			if (GlobalVars.Instance.MyButton(crdDemote, StringMgr.Instance.Get("DEMOTE"), "BtnAction"))
			{
				if (selMngClanMember == null)
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("SELECT_CLAN_MEMBER"));
				}
				else if (selMngClanMember.ClanLv <= 0)
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NO_MORE_DEMOTE"));
				}
				else
				{
					int num4 = selMngClanMember.ClanLv - 1;
					string contents2 = "CLAN_DEMOTE_COMMENT" + GlobalVars.DELIMITER + "n" + MyInfoManager.Instance.ClanName + GlobalVars.DELIMITER + "n" + clanLevelName[num4];
					CSNetManager.Instance.Sock.SendCS_DOWN_CLAN_MEMBER_REQ(MyInfoManager.Instance.ClanSeq, selMngClanMember.Seq, selMngClanMember.Nickname, "CLAN_DEMOTE", contents2);
				}
			}
		}
	}

	private void DoManage()
	{
		GUI.Box(crdMngInfoGeneral, string.Empty, "BoxFadeBlue");
		GUI.Box(crdMngInfoIntro, string.Empty, "BoxInnerLine");
		GUI.Box(crdMngInfoNotice, string.Empty, "BoxInnerLine");
		DrawClanMark(crdMngInfoMark, MyInfoManager.Instance.ClanMark);
		LabelUtil.TextOut(crdMngInfoName, MyInfoManager.Instance.ClanName, "Label", new Color(0.87f, 0.63f, 0.32f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMngInfoMaster, myClanMaster, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMngInfoRank, MyClanRankString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMngInfoRecord, MyClanRecordString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMngInfoMembers, MyClanMemberCountString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMngInfoNameLabel, StringMgr.Instance.Get("CLAN_NAME") + ":", "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMngInfoMasterLabel, StringMgr.Instance.Get("CLAN_MASTER") + ":", "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMngInfoRankLabel, StringMgr.Instance.Get("RANK") + ":", "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMngInfoRecordLabel, StringMgr.Instance.Get("RECORD") + ":", "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMngInfoMembersLabel, StringMgr.Instance.Get("CLAN_NUM_MEMBERS") + ":", "Label", Color.gray, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMngInform, StringMgr.Instance.Get("CLAN_NOTICE"), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMngPromote, StringMgr.Instance.Get("CALN_INFO_PROMOTION"), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		Rect screenRect = crdMngInfoIntro;
		GUILayout.BeginArea(screenRect);
		spMngInfoIntro = GUILayout.BeginScrollView(spMngInfoIntro, GUILayout.Width(screenRect.width), GUILayout.Height(screenRect.height));
		GUILayout.Label(myClanIntro);
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		Rect screenRect2 = crdMngInfoNotice;
		GUILayout.BeginArea(screenRect2);
		spMngInfoNotice = GUILayout.BeginScrollView(spMngInfoNotice, GUILayout.Width(screenRect2.width), GUILayout.Height(screenRect2.height));
		GUILayout.Label(myClanNotice);
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		if (MyInfoManager.Instance.IsClanMaster)
		{
			if (GlobalVars.Instance.MyButton(crdCloseClan, StringMgr.Instance.Get("CLAN_CLOSE"), "BtnAction"))
			{
				if (!MyInfoManager.Instance.IsEmptyClan)
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("MEMBER_REMAIN4CLOSE_CLAN"));
				}
				else
				{
					((ClanConfirmDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.CLAN_CONFIRM, exclusive: false))?.InitDialog(ClanConfirmDialog.CLAN_CONFIRM_WHAT.DESTROY_CLAN, -1, string.Empty, string.Empty);
				}
			}
			if (GlobalVars.Instance.MyButton(crdChangeMark, StringMgr.Instance.Get("CHANGE_MARK"), "BtnAction"))
			{
				((MarkDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.MARK, exclusive: false))?.InitDialog(MyInfoManager.Instance.ClanMark);
			}
		}
		if (MyInfoManager.Instance.IsClanStaff)
		{
			if (GlobalVars.Instance.MyButton(crdChangeIntro, StringMgr.Instance.Get("CHANGE_INTRO"), "BtnAction"))
			{
				((ChangeIntroDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.CHANGE_INTRO, exclusive: false))?.InitDialog();
			}
			if (GlobalVars.Instance.MyButton(crdChangeNotice, StringMgr.Instance.Get("CHANGE_NOTICE"), "BtnAction"))
			{
				((ChangeNoticeDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.CHANGE_NOTICE, exclusive: false))?.InitDialog();
			}
		}
		curMngTab = GUI.SelectionGrid(crdMngTab, curMngTab, tab4Mng, tab4Mng.Length, "PopTab");
		if (curMngTab == 0)
		{
			_DoMngClanMember();
		}
		else
		{
			_DoMngApplicant();
		}
		if (GlobalVars.Instance.MyButton(crdBtnRefresh, string.Empty, "BtnRefresh") && refreshDelta > refreshLimit)
		{
			refreshDelta = 0f;
			RefreshMyClan();
		}
	}

	public void SetClanDetail(int clan, string intro, string notice, int winCount, int drawCount, int loseCount, int rank, int rankChg, int memberCount, int gold, int silver, int bronze, string clanMaster)
	{
		if (MyInfoManager.Instance.ClanSeq == clan)
		{
			myClanIntro = intro;
			myClanNotice = notice;
			myClanWinCount = winCount;
			myClanDrawCount = drawCount;
			myClanLoseCount = loseCount;
			myClanRank = rank;
			myClanMemberCount = memberCount;
			myClanMaster = clanMaster;
			myClanGlod = gold;
			myClanSilver = silver;
			myClanBronze = bronze;
		}
	}

	public override void Update()
	{
		refreshDelta += Time.deltaTime;
		deltaTime += Time.deltaTime;
		if (deltaTime > 1f)
		{
			deltaTime = 0f;
			if (curTab == 1 || curTab == 3)
			{
				CSNetManager.Instance.Sock.SendCS_WHATSUP_FELLA_REQ();
			}
		}
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		DoTitle();
		string[] array = tab4Somebody;
		if (!MyInfoManager.Instance.IsClanStaff && MyInfoManager.Instance.ClanLv != 0 && dicClanReq.Count > 0)
		{
			array = tab4VirtualMember;
		}
		else if (MyInfoManager.Instance.ClanLv == 0)
		{
			array = tab4Member;
		}
		else if (MyInfoManager.Instance.IsClanStaff)
		{
			array = tab4Staff;
		}
		Rect position = crdTab4NoMember;
		if (MyInfoManager.Instance.IsClanMember)
		{
			position = crdTab;
		}
		if (curTab >= array.Length)
		{
			curTab = array.Length - 1;
		}
		if (curTab < 0)
		{
			curTab = 0;
		}
		GUI.Box(crdMainOutline, string.Empty, "BoxPopLine");
		GUI.Box(crdExplOutline, string.Empty, "BoxPopLine");
		int num = curTab;
		curTab = GUI.SelectionGrid(position, curTab, array, array.Length, "PopTab");
		if (num != curTab)
		{
			curClan = 0;
		}
		switch (curTab)
		{
		case 0:
			DoClanList();
			break;
		case 1:
			if (MyInfoManager.Instance.ClanLv != 0 && dicClanReq.Count > 0)
			{
				DoVirtualClanList();
			}
			else
			{
				DoMyClan();
			}
			break;
		case 2:
			DoClanRecord();
			break;
		case 3:
			DoManage();
			break;
		}
		Rect rc = new Rect(size.x - 44f, 5f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc, string.Empty, "BtnClose") || (DialogManager.Instance.IsSelfModal(DialogManager.DIALOG_INDEX.CLAN) && GlobalVars.Instance.IsEscapePressed()))
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
