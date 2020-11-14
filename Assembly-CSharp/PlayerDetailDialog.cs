using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerDetailDialog : Dialog
{
	public Texture2D gauge;

	public Rect crdBtnClose = new Rect(640f, 455f, 90f, 26f);

	public RenderTexture othersView;

	public Vector2 crdTitle = new Vector2(19f, 10f);

	public Rect crdPortraitFrame = new Rect(10f, 52f, 135f, 157f);

	public Rect crdPortrait = new Rect(10f, 60f, 135f, 135f);

	public Rect crdIdentityFrame = new Rect(150f, 52f, 228f, 157f);

	public Rect crdIdentityLabelFrame = new Rect(150f, 52f, 57f, 155f);

	public Rect crdIdentityLineFrame = new Rect(150f, 52f, 228f, 157f);

	public Vector2 crdClanLabel = new Vector2(180f, 70f);

	public Vector2 crdBadgeLabel = new Vector2(180f, 100f);

	public Vector2 crdNicknameLabel = new Vector2(180f, 130f);

	public Vector2 crdRankLabel = new Vector2(180f, 160f);

	public Vector2 crdXpLabel = new Vector2(180f, 190f);

	public Rect crdClanMark = new Rect(214f, 55f, 32f, 32f);

	public Vector2 crdClanName = new Vector2(250f, 58f);

	public Vector2 crdNoClanName = new Vector2(214f, 58f);

	public Rect crdBadge = new Rect(214f, 93f, 34f, 17f);

	public Rect crdBadgeName = new Rect(250f, 80f, 95f, 40f);

	public Vector2 crdNickname = new Vector2(214f, 118f);

	public Vector2 crdRank = new Vector2(214f, 148f);

	public Vector2 crdXp = new Vector2(305f, 183f);

	public Rect crdXpGaugeFrame = new Rect(214f, 190f, 86f, 10f);

	public Rect crdXpGauge = new Rect(216f, 192f, 82f, 6f);

	public Vector2 crdTotalBattleLogLabel = new Vector2(17f, 221f);

	public Rect crdTotalBattleLogFrame = new Rect(14f, 243f, 362f, 98f);

	public Rect crdTotalBattleLabelFrame = new Rect(16f, 245f, 130f, 94f);

	public Vector2 crdKillDeathLabel = new Vector2(84f, 263f);

	public Vector2 crdAssistLabel = new Vector2(84f, 290f);

	public Vector2 crdGiveupLabel = new Vector2(84f, 321f);

	public Vector2 crdKillDeathValue = new Vector2(156f, 252f);

	public Vector2 crdAssistValue = new Vector2(156f, 282f);

	public Vector2 crdGiveupValue = new Vector2(156f, 309f);

	public Vector2 crdAssetLabel = new Vector2(17f, 353f);

	public Rect crdAssetFrame = new Rect(14f, 375f, 363f, 64f);

	private Rect crdAssetLabelFrame = new Rect(16f, 377f, 130f, 86f);

	private Vector2 crdGeneralLabel = new Vector2(84f, 392f);

	private Vector2 crdPointValue = new Vector2(156f, 380f);

	public Vector2 crdDetailBattleLogLabel = new Vector2(408f, 58f);

	public Rect crdDetailBattleLogFrame = new Rect(395f, 53f, 338f, 390f);

	public Rect crdDetailPosition = new Rect(410f, 89f, 310f, 356f);

	public Rect crdDetailView = new Rect(0f, 0f, 290f, 925f);

	public Rect crdTmFrame = new Rect(0f, 0f, 0f, 0f);

	public Vector2 crdTmLabel = new Vector2(0f, 0f);

	public Vector2 crdTmRecordValue = new Vector2(0f, 0f);

	public Vector2 crdTmKillDeathValue = new Vector2(0f, 0f);

	public Vector2 crdTmAssistValue = new Vector2(0f, 0f);

	public Vector2 crdTmGiveupValue = new Vector2(0f, 0f);

	public Vector2 crdTmScoreRecordValue = new Vector2(0f, 0f);

	public Rect crdImFrame = new Rect(0f, 0f, 0f, 0f);

	public Vector2 crdImLabel = new Vector2(0f, 0f);

	public Vector2 crdImKillDeathValue = new Vector2(0f, 0f);

	public Vector2 crdImGiveupValue = new Vector2(0f, 0f);

	public Vector2 crdImScoreRecordValue = new Vector2(0f, 0f);

	public Rect crdDmFrame = new Rect(0f, 0f, 0f, 0f);

	public Vector2 crdDmLabel = new Vector2(0f, 0f);

	public Vector2 crdDmRecordValue = new Vector2(0f, 0f);

	public Vector2 crdDmKillDeathValue = new Vector2(0f, 0f);

	public Vector2 crdDmAssistValue = new Vector2(0f, 0f);

	public Vector2 crdDmGiveupValue = new Vector2(0f, 0f);

	public Vector2 crdDmScoreRecordValue = new Vector2(0f, 0f);

	public Rect crdBmFrame = new Rect(0f, 0f, 0f, 0f);

	public Vector2 crdBmLabel = new Vector2(0f, 0f);

	public Vector2 crdBmRecordValue = new Vector2(0f, 0f);

	public Vector2 crdBmKillDeathValue = new Vector2(0f, 0f);

	public Vector2 crdBmAssistValue = new Vector2(0f, 0f);

	public Vector2 crdBmGiveupValue = new Vector2(0f, 0f);

	public Vector2 crdBmScoreRecordValue = new Vector2(0f, 0f);

	public Rect crdCtfFrame = new Rect(0f, 0f, 0f, 0f);

	public Vector2 crdCtfLabel = new Vector2(0f, 0f);

	public Vector2 crdCtfRecordValue = new Vector2(0f, 0f);

	public Vector2 crdCtfKillDeathValue = new Vector2(0f, 0f);

	public Vector2 crdCtfAssistValue = new Vector2(0f, 0f);

	public Vector2 crdCtfGiveupValue = new Vector2(0f, 0f);

	public Vector2 crdCtfScoreRecordValue = new Vector2(0f, 0f);

	public Rect crdBndFrame = new Rect(0f, 0f, 0f, 0f);

	public Vector2 crdBndLabel = new Vector2(0f, 0f);

	public Vector2 crdBndRecordValue = new Vector2(0f, 0f);

	public Vector2 crdBndKillDeathValue = new Vector2(0f, 0f);

	public Vector2 crdBndAssistValue = new Vector2(0f, 0f);

	public Vector2 crdBndGiveupValue = new Vector2(0f, 0f);

	public Vector2 crdBndScoreRecordValue = new Vector2(0f, 0f);

	public Rect crdWeaponFrame = new Rect(0f, 0f, 0f, 0f);

	public Vector2 crdWeaponLabel = new Vector2(0f, 0f);

	public Vector2 crdMainLabel = new Vector2(0f, 0f);

	public Vector2 crdAuxLabel = new Vector2(0f, 0f);

	public Vector2 crdMeleeLabel = new Vector2(0f, 0f);

	public Vector2 crdSpecLabel = new Vector2(0f, 0f);

	public Rect crdMainGaugeFrame = new Rect(0f, 0f, 0f, 0f);

	public Rect crdMainGauge = new Rect(0f, 0f, 0f, 0f);

	public Rect crdAuxGaugeFrame = new Rect(0f, 0f, 0f, 0f);

	public Rect crdAuxGauge = new Rect(0f, 0f, 0f, 0f);

	public Rect crdMeleeGaugeFrame = new Rect(0f, 0f, 0f, 0f);

	public Rect crdMeleeGauge = new Rect(0f, 0f, 0f, 0f);

	public Rect crdSpecGaugeFrame = new Rect(0f, 0f, 0f, 0f);

	public Rect crdSpecGauge = new Rect(0f, 0f, 0f, 0f);

	public Vector2 crdMainPercent = new Vector2(0f, 0f);

	public Vector2 crdAuxPercent = new Vector2(0f, 0f);

	public Vector2 crdMeleePercent = new Vector2(0f, 0f);

	public Vector2 crdSpecPercent = new Vector2(0f, 0f);

	public Rect crdWeaponLvFrame = new Rect(0f, 755f, 290f, 25f);

	public Vector2 crdWeaponLvLabel = new Vector2(10f, 755f);

	public Vector2 crdHeavyWpnLabel = new Vector2(10f, 780f);

	public Vector2 crdAssaultWpnLabel = new Vector2(10f, 800f);

	public Vector2 crdSniperWpnLabel = new Vector2(10f, 820f);

	public Vector2 crdSubMachineWpnLabel = new Vector2(10f, 840f);

	public Vector2 crdHandGunWpnLabel = new Vector2(10f, 860f);

	public Vector2 crdMeleeWpnLabel = new Vector2(10f, 880f);

	public Vector2 crdSpecialWpnLabel = new Vector2(10f, 900f);

	public Rect crdHeavyWpnGaugeFrame = new Rect(108f, 790f, 100f, 10f);

	public Rect crdHeavyWpnGauge = new Rect(110f, 792f, 96f, 6f);

	public Vector2 crdHeavyWpnLevel = new Vector2(210f, 783f);

	public Rect crdAssaultWpnGaugeFrame = new Rect(108f, 810f, 100f, 10f);

	public Rect crdAssaultWpnGauge = new Rect(110f, 812f, 96f, 6f);

	public Vector2 crdAssaultWpnLevel = new Vector2(210f, 803f);

	public Rect crdSniperWpnGaugeFrame = new Rect(108f, 830f, 100f, 10f);

	public Rect crdSniperWpnGauge = new Rect(110f, 832f, 96f, 6f);

	public Vector2 crdSniperWpnLevel = new Vector2(210f, 823f);

	public Rect crdSubMachineWpnGaugeFrame = new Rect(108f, 850f, 100f, 10f);

	public Rect crdSubMachineWpnGauge = new Rect(110f, 852f, 96f, 6f);

	public Vector2 crdSubMachineWpnWpnLevel = new Vector2(210f, 843f);

	public Rect crdHandGunWpnGaugeFrame = new Rect(108f, 870f, 100f, 10f);

	public Rect crdHandGunWpnGauge = new Rect(110f, 872f, 96f, 6f);

	public Vector2 crdHandGunWpnLevel = new Vector2(210f, 863f);

	public Rect crdMeleeWpnGaugeFrame = new Rect(108f, 890f, 100f, 10f);

	public Rect crdMeleeWpnGauge = new Rect(110f, 892f, 96f, 6f);

	public Vector2 crdMeleeWpnLevel = new Vector2(210f, 883f);

	public Rect crdSpecialWpnGaugeFrame = new Rect(108f, 910f, 100f, 10f);

	public Rect crdSpecialWpnGauge = new Rect(110f, 912f, 96f, 6f);

	public Vector2 crdSpecialWpnLevel = new Vector2(210f, 903f);

	public Rect crdHeavyTooltip = new Rect(10f, 788f, 280f, 14f);

	public Rect crdAssaultTooltip = new Rect(10f, 808f, 280f, 14f);

	public Rect crdSniperTooltip = new Rect(10f, 828f, 280f, 14f);

	public Rect crdSubmachineTooltip = new Rect(10f, 848f, 280f, 14f);

	public Rect crdHandgunTooltip = new Rect(10f, 868f, 280f, 14f);

	public Rect crdMeleeTooltip = new Rect(10f, 888f, 280f, 14f);

	public Rect crdSpecialTooltip = new Rect(10f, 908f, 280f, 14f);

	private int Target;

	private string Nickname;

	private int Xp;

	private int Point;

	private int Brick;

	private int Cash;

	private int TmKill;

	private int TmDeath;

	private int TmAssist;

	private int TmWin;

	private int TmDraw;

	private int TmLose;

	private int TmGiveup;

	private int TmScoreRecord;

	private int ImKill;

	private int ImDeath;

	private int ImGiveup;

	private int ImScoreRecord;

	private int DmKill;

	private int DmDeath;

	private int DmAssist;

	private int DmSuccess;

	private int DmFail;

	private int DmGiveup;

	private int DmScoreRecord;

	private int BmKill;

	private int BmDeath;

	private int BmAssist;

	private int BmWin;

	private int BmDraw;

	private int BmLose;

	private int BmGiveup;

	private int BmScoreRecord;

	private int CtfKill;

	private int CtfDeath;

	private int CtfAssist;

	private int CtfWin;

	private int CtfDraw;

	private int CtfLose;

	private int CtfGiveup;

	private int CtfScoreRecord;

	private int BndKill;

	private int BndDeath;

	private int BndAssist;

	private int BndWin;

	private int BndDraw;

	private int BndLose;

	private int BndGiveup;

	private int BndScoreRecord;

	private int BungeeCount;

	private int BungeeDeath;

	private int BungeeGiveup;

	private int BungeeScoreRecord;

	private int EscapeCount;

	private int EscapeGoalCount;

	private int EscapeGiveup;

	private int EscapeScoreRecord;

	private int ZombieHumanAlive;

	private int ZombieZombieAlive;

	private int ZombieGiveup;

	private int ZombieScoreRecord;

	private int ZombieCount;

	private int MainKill;

	private int AuxKill;

	private int MeleeKill;

	private int SpecKill;

	private List<string> EquipList;

	private int Clan;

	private string ClanName;

	private int ClanMark;

	private int Rank;

	private int Heavy;

	private int Assault;

	private int Sniper;

	private int SubMachine;

	private int HandGun;

	private int Melee;

	private int Special;

	private Texture2D badge;

	private string badgeName = string.Empty;

	private Vector2 spDetail = Vector2.zero;

	private Color txtMainColor = new Color(1f, 1f, 1f, 1f);

	private float DefenseY;

	private float contorlY;

	private string heavyTooltip = string.Empty;

	private string assaultTooltip = string.Empty;

	private string sniperTooltip = string.Empty;

	private string subMachineTooltip = string.Empty;

	private string handgunTooltip = string.Empty;

	private string meleeTooltip = string.Empty;

	private string specialTooltip = string.Empty;

	private Rect crdCurFrame;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.PLAYER_DETAIL;
		applyNewCoordY();
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog(int target, string nickname, int xp, int point, int brick, int cash, int token, int tmKill, int tmDeath, int tmAssist, int tmWin, int tmDraw, int tmLose, int tmGiveup, int tmScoreRecord, int imKill, int imDeath, int imGiveup, int imScoreRecord, int dmKill, int dmDeath, int dmAssist, int dmSuccess, int dmFail, int dmGiveup, int dmScoreRecord, int bmKill, int bmDeath, int bmAssist, int bmWin, int bmDraw, int bmLose, int bmGiveup, int bmScoreRecord, int ctfKill, int ctfDeath, int ctfAssist, int ctfWin, int ctfDraw, int ctfLose, int ctfGiveup, int ctfScoreRecord, int bndKill, int bndDeath, int bndAssist, int bndWin, int bndDraw, int bndLose, int bndGiveup, int bndScoreRecord, int bungeeCount, int bungeeDeath, int bungeeGiveup, int bungeeScoreRecord, int escapeCount, int escapeGoalCount, int escapeGiveup, int escapeScoreRecord, int zombieHumanAlive, int zombieZombieAlive, int zombieGiveup, int zombieScoreRecord, int zombieCount, int mainKill, int auxKill, int meleeKill, int specKill, List<string> equipList, int clan, string clanName, int clanMark, int rank, int heavy, int assault, int sniper, int subMachine, int handGun, int melee, int special)
	{
		Target = target;
		Nickname = nickname;
		Xp = xp;
		Point = point;
		Brick = brick;
		Cash = cash;
		TmKill = tmKill;
		TmDeath = tmDeath;
		TmAssist = tmAssist;
		TmWin = tmWin;
		TmDraw = tmDraw;
		TmLose = tmLose;
		TmGiveup = tmGiveup;
		TmScoreRecord = tmScoreRecord;
		ImKill = imKill;
		ImDeath = imDeath;
		ImGiveup = imGiveup;
		ImScoreRecord = imScoreRecord;
		DmKill = dmKill;
		DmDeath = dmDeath;
		DmAssist = dmAssist;
		DmSuccess = dmSuccess;
		DmFail = dmFail;
		DmGiveup = dmGiveup;
		DmScoreRecord = dmScoreRecord;
		BmKill = bmKill;
		BmDeath = bmDeath;
		BmAssist = bmAssist;
		BmWin = bmWin;
		BmDraw = bmDraw;
		BmLose = bmLose;
		BmGiveup = bmGiveup;
		BmScoreRecord = bmScoreRecord;
		CtfKill = ctfKill;
		CtfDeath = ctfDeath;
		CtfAssist = ctfAssist;
		CtfWin = ctfWin;
		CtfDraw = ctfDraw;
		CtfLose = ctfLose;
		CtfGiveup = ctfGiveup;
		CtfScoreRecord = ctfScoreRecord;
		BndKill = bndKill;
		BndDeath = bndDeath;
		BndAssist = bndAssist;
		BndWin = bndWin;
		BndDraw = bndDraw;
		BndLose = bndLose;
		BndGiveup = bndGiveup;
		BndScoreRecord = bndScoreRecord;
		BungeeCount = bungeeCount;
		BungeeDeath = bungeeDeath;
		BungeeGiveup = bungeeGiveup;
		BungeeScoreRecord = bungeeScoreRecord;
		EscapeCount = escapeCount;
		EscapeGoalCount = escapeGoalCount;
		EscapeGiveup = escapeGiveup;
		EscapeScoreRecord = escapeScoreRecord;
		ZombieHumanAlive = zombieHumanAlive;
		ZombieZombieAlive = zombieZombieAlive;
		ZombieGiveup = zombieGiveup;
		ZombieScoreRecord = zombieScoreRecord;
		ZombieCount = zombieCount;
		MainKill = mainKill;
		AuxKill = auxKill;
		MeleeKill = meleeKill;
		SpecKill = specKill;
		Clan = clan;
		ClanName = clanName;
		ClanMark = clanMark;
		Rank = rank;
		Heavy = heavy;
		Assault = assault;
		Sniper = sniper;
		SubMachine = subMachine;
		HandGun = handGun;
		Melee = melee;
		Special = special;
		int level = XpManager.Instance.GetLevel(Xp);
		badge = XpManager.Instance.GetBadge(level, Rank);
		badgeName = XpManager.Instance.GetRank(level, Rank);
		if (EquipList == null)
		{
			EquipList = new List<string>();
		}
		EquipList.Clear();
		GameObject gameObject = GameObject.Find("Other");
		if (null != gameObject)
		{
			LookCoordinator component = gameObject.GetComponent<LookCoordinator>();
			if (null != component)
			{
				component.Reset();
				foreach (string equip in equipList)
				{
					component.Equip(equip);
					EquipList.Add(equip);
				}
				component.ChangeWeapon(Weapon.TYPE.MAIN);
			}
			AutoRotator component2 = gameObject.GetComponent<AutoRotator>();
			if (null != component2)
			{
				component2.stopOnStart = false;
				component2.Rotate(AutoRotator.ROTATE.LEFT);
			}
		}
		txtMainColor = GlobalVars.Instance.txtMainColor;
	}

	private void applyNewCoordY()
	{
		contorlY = 0f;
		if (!BuildOption.Instance.Props.teamMatchMode)
		{
			contorlY = 135f;
			crdDetailView.height -= 135f;
		}
		if (!BuildOption.Instance.Props.individualMatchMode)
		{
			contorlY += 95f;
			crdDetailView.height -= 95f;
		}
		if (!BuildOption.Instance.Props.defenseMatchMode)
		{
			DefenseY = crdBmFrame.y - crdDmFrame.y;
			crdDetailView.height -= DefenseY;
		}
		if (!BuildOption.Instance.Props.explosionMatchMode)
		{
			contorlY += 135f;
			crdDetailView.height -= 135f;
		}
		if (!BuildOption.Instance.Props.ctfMatchMode)
		{
			contorlY += 135f;
			crdDetailView.height -= 135f;
		}
		if (!BuildOption.Instance.Props.bndMatchMode)
		{
			contorlY += 135f;
			crdDetailView.height -= 135f;
		}
		if (BuildOption.Instance.Props.bungeeMode)
		{
			contorlY -= 135f;
			crdDetailView.height += 135f;
		}
		if (BuildOption.Instance.Props.escapeMode)
		{
			contorlY -= 135f;
			crdDetailView.height += 135f;
		}
		if (BuildOption.Instance.Props.zombieMode)
		{
			contorlY -= 135f;
			crdDetailView.height += 135f;
		}
		if (BuildOption.Instance.IsNetmarble || BuildOption.Instance.IsDeveloper)
		{
			crdDetailView.height -= 180f;
		}
	}

	private void DoTitle()
	{
		LabelUtil.TextOut(new Vector2(size.x / 2f, 10f), StringMgr.Instance.Get("USER_INFORMATION"), "BigLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
	}

	private void DoPortrait()
	{
		GUI.Box(crdPortraitFrame, string.Empty, "LineBoxYellow");
		TextureUtil.DrawTexture(crdPortrait, othersView, ScaleMode.StretchToFill);
	}

	private void DrawClanMark(int clanMark)
	{
		if (clanMark >= 0)
		{
			Texture2D bg = ClanMarkManager.Instance.GetBg(clanMark);
			Color colorValue = ClanMarkManager.Instance.GetColorValue(clanMark);
			Texture2D amblum = ClanMarkManager.Instance.GetAmblum(clanMark);
			if (null != bg)
			{
				TextureUtil.DrawTexture(crdClanMark, bg);
			}
			Color color = GUI.color;
			GUI.color = colorValue;
			if (null != amblum)
			{
				TextureUtil.DrawTexture(crdClanMark, amblum);
			}
			GUI.color = color;
		}
	}

	private void DoIdentity()
	{
		GUI.Box(crdIdentityLineFrame, string.Empty, "LineBoxBlue");
		LabelUtil.TextOut(crdClanLabel, StringMgr.Instance.Get("CLAN"), "Label", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(crdBadgeLabel, StringMgr.Instance.Get("BADGE"), "Label", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(crdNicknameLabel, StringMgr.Instance.Get("NICKNAME"), "Label", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(crdRankLabel, StringMgr.Instance.Get("RANK"), "Label", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(crdXpLabel, StringMgr.Instance.Get("XP"), "Label", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		if (Clan < 0)
		{
			LabelUtil.TextOut(crdNoClanName, StringMgr.Instance.Get("NO_CLAN"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		else
		{
			if (!BuildOption.Instance.IsNetmarbleOrDev)
			{
				DrawClanMark(ClanMark);
			}
			LabelUtil.TextOut(crdClanName, ClanName, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		if (null != badge)
		{
			TextureUtil.DrawTexture(crdBadge, badge, ScaleMode.StretchToFill);
		}
		GUI.Label(crdBadgeName, badgeName, "MiddleLeftLabel");
		LabelUtil.TextOut(crdNickname, Nickname, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdRank, (Rank <= 0) ? StringMgr.Instance.Get("NO_RANKING") : Rank.ToString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		GUI.Box(crdXpGaugeFrame, string.Empty, "BoxDark");
		float ratio = XpManager.Instance.GetRatio(Xp);
		TextureUtil.DrawTexture(new Rect(crdXpGauge.x, crdXpGauge.y, crdXpGauge.width * ratio, crdXpGauge.height), gauge, ScaleMode.StretchToFill);
		float num = Mathf.Floor(ratio * 10000f) / 100f;
		LabelUtil.TextOut(crdXp, num.ToString("0.##") + "%", "MiniLabel", new Color(0.7f, 0.7f, 0.7f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
	}

	private void DoTotalBattleLog()
	{
		LabelUtil.TextOut(crdTotalBattleLogLabel, StringMgr.Instance.Get("TOTAL_BATTLE_LOG"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		GUI.Box(crdTotalBattleLogFrame, string.Empty, "BoxPopLine");
		GUI.Box(crdTotalBattleLabelFrame, string.Empty, "BoxBlue");
		LabelUtil.TextOut(crdKillDeathLabel, StringMgr.Instance.Get("KILL") + "/" + StringMgr.Instance.Get("DEATH"), "Label", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(crdAssistLabel, StringMgr.Instance.Get("ASSIST"), "Label", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(crdGiveupLabel, StringMgr.Instance.Get("GIVEUP"), "Label", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		int num = TmKill + ImKill + BmKill + CtfKill;
		int num2 = TmDeath + ImDeath + BmDeath + CtfDeath;
		int num3 = TmAssist + DmAssist + BmAssist + CtfAssist;
		int num4 = TmGiveup + ImGiveup + DmGiveup + BmGiveup + CtfGiveup;
		float num5 = 0f;
		if (num2 <= 0)
		{
			if (num > 0)
			{
				num5 = 100f;
			}
		}
		else
		{
			num5 = (float)num / (float)(num + num2) * 100f;
		}
		string text = num.ToString() + "/" + num2.ToString() + " (" + num5.ToString("0.##") + "%)";
		string text2 = num3.ToString();
		string text3 = num4.ToString();
		LabelUtil.TextOut(crdKillDeathValue, text, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdAssistValue, text2, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdGiveupValue, text3, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
	}

	private void DoAsset()
	{
		if (MyInfoManager.Instance.Seq == Target)
		{
			LabelUtil.TextOut(crdAssetLabel, StringMgr.Instance.Get("POINT"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			GUI.Box(crdAssetFrame, string.Empty, "BoxPopLine");
			GUI.Box(crdAssetLabelFrame, string.Empty, "BoxBlue");
			Vector2 pos = crdGeneralLabel;
			LabelUtil.TextOut(pos, StringMgr.Instance.Get("GENERAL_POINT"), "Label", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			if (BuildOption.Instance.Props.useBrickPoint)
			{
				pos.y += 26f;
				LabelUtil.TextOut(pos, StringMgr.Instance.Get("BRICK_POINT"), "Label", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
			pos.y += 26f;
			LabelUtil.TextOut(pos, TokenManager.Instance.GetTokenString(), "Label", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			string text = Cash.ToString("n0");
			string text2 = Point.ToString("n0");
			string text3 = Brick.ToString("n0");
			pos = crdPointValue;
			LabelUtil.TextOut(pos, text2, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			if (BuildOption.Instance.Props.useBrickPoint)
			{
				pos.y += 26f;
				LabelUtil.TextOut(pos, text3, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			pos.y += 26f;
			LabelUtil.TextOut(pos, text, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
	}

	private void DrawTM()
	{
		if (BuildOption.Instance.Props.teamMatchMode)
		{
			Vector2 pos = new Vector2(crdCurFrame.x + 10f, crdCurFrame.y);
			Vector2 pos2 = new Vector2(pos.x, pos.y + 25f);
			Vector2 pos3 = new Vector2(pos.x, pos.y + 45f);
			Vector2 pos4 = new Vector2(pos.x, pos.y + 65f);
			Vector2 pos5 = new Vector2(pos.x, pos.y + 85f);
			Vector2 pos6 = new Vector2(pos.x, pos.y + 105f);
			GUI.Box(crdCurFrame, string.Empty, "BoxBlue");
			LabelUtil.TextOut(pos, StringMgr.Instance.Get("ROOM_TYPE_TEAM_MATCH"), "Label", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text = StringMgr.Instance.Get("RECORD") + ": " + TmWin.ToString() + "-" + TmDraw.ToString() + "-" + TmLose.ToString() + " (W-D-L)";
			LabelUtil.TextOut(pos2, text, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			float num = 0f;
			if (TmDeath + TmKill > 0)
			{
				num = (float)TmKill / (float)(TmKill + TmDeath) * 100f;
			}
			string text2 = StringMgr.Instance.Get("KILL") + "/" + StringMgr.Instance.Get("DEATH") + ": " + TmKill.ToString() + "/" + TmDeath.ToString() + " (" + num.ToString("0.##") + "%)";
			LabelUtil.TextOut(pos3, text2, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text3 = StringMgr.Instance.Get("ASSIST") + ": " + TmAssist.ToString();
			LabelUtil.TextOut(pos4, text3, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text4 = StringMgr.Instance.Get("GIVEUP") + ": " + TmGiveup.ToString();
			LabelUtil.TextOut(pos5, text4, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text5 = StringMgr.Instance.Get("BEST_SCORE") + ": " + TmScoreRecord.ToString();
			LabelUtil.TextOut(pos6, text5, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			crdCurFrame.y += 135f;
		}
	}

	private void DrawIM()
	{
		if (BuildOption.Instance.Props.individualMatchMode)
		{
			Vector2 pos = new Vector2(crdCurFrame.x + 10f, crdCurFrame.y);
			Vector2 pos2 = new Vector2(pos.x, pos.y + 25f);
			Vector2 pos3 = new Vector2(pos.x, pos.y + 45f);
			Vector2 pos4 = new Vector2(pos.x, pos.y + 65f);
			GUI.Box(crdCurFrame, string.Empty, "BoxBlue");
			LabelUtil.TextOut(pos, StringMgr.Instance.Get("ROOM_TYPE_INDIVIDUAL_MATCH"), "Label", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			float num = 0f;
			if (ImKill + ImDeath > 0)
			{
				num = (float)ImKill / (float)(ImKill + ImDeath) * 100f;
			}
			string text = StringMgr.Instance.Get("KILL") + "/" + StringMgr.Instance.Get("DEATH") + ": " + ImKill.ToString() + "/" + ImDeath.ToString() + " (" + num.ToString("0.##") + "%)";
			LabelUtil.TextOut(pos2, text, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text2 = StringMgr.Instance.Get("GIVEUP") + ": " + ImGiveup.ToString();
			LabelUtil.TextOut(pos3, text2, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text3 = StringMgr.Instance.Get("BEST_SCORE") + ": " + ImScoreRecord.ToString();
			LabelUtil.TextOut(pos4, text3, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			crdCurFrame.y += 95f;
		}
	}

	private void DrawDM()
	{
		if (BuildOption.Instance.Props.defenseMatchMode)
		{
			Vector2 pos = new Vector2(crdCurFrame.x + 10f, crdCurFrame.y);
			Vector2 pos2 = new Vector2(pos.x, pos.y + 25f);
			Vector2 pos3 = new Vector2(pos.x, pos.y + 45f);
			Vector2 pos4 = new Vector2(pos.x, pos.y + 65f);
			Vector2 pos5 = new Vector2(pos.x, pos.y + 85f);
			Vector2 pos6 = new Vector2(pos.x, pos.y + 105f);
			GUI.Box(crdCurFrame, string.Empty, "BoxBlue");
			LabelUtil.TextOut(pos, StringMgr.Instance.Get("ROOM_TYPE_MISSION"), "Label", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text = StringMgr.Instance.Get("RECORD") + ": " + DmSuccess.ToString() + "-" + DmFail.ToString() + " (Success-Fail)";
			LabelUtil.TextOut(pos2, text, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			float num = 0f;
			if (DmKill + DmDeath > 0)
			{
				num = (float)DmKill / (float)(DmKill + DmDeath) * 100f;
			}
			string text2 = StringMgr.Instance.Get("KILL") + "/" + StringMgr.Instance.Get("DEATH") + ": " + DmKill.ToString() + "/" + DmDeath.ToString() + " (" + num.ToString("0.##") + "%)";
			LabelUtil.TextOut(pos3, text2, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text3 = StringMgr.Instance.Get("ASSIST") + ": " + DmAssist.ToString();
			LabelUtil.TextOut(pos4, text3, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text4 = StringMgr.Instance.Get("GIVEUP") + ": " + DmGiveup.ToString();
			LabelUtil.TextOut(pos5, text4, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text5 = StringMgr.Instance.Get("BEST_SCORE") + ": " + DmScoreRecord.ToString();
			LabelUtil.TextOut(pos6, text5, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			crdCurFrame.y += 135f;
		}
	}

	private void DrawBM()
	{
		if (BuildOption.Instance.Props.explosionMatchMode)
		{
			Vector2 pos = new Vector2(crdCurFrame.x + 10f, crdCurFrame.y);
			Vector2 pos2 = new Vector2(pos.x, pos.y + 25f);
			Vector2 pos3 = new Vector2(pos.x, pos.y + 45f);
			Vector2 pos4 = new Vector2(pos.x, pos.y + 65f);
			Vector2 pos5 = new Vector2(pos.x, pos.y + 85f);
			Vector2 pos6 = new Vector2(pos.x, pos.y + 105f);
			GUI.Box(crdCurFrame, string.Empty, "BoxBlue");
			LabelUtil.TextOut(pos, StringMgr.Instance.Get("ROOM_TYPE_EXPLOSION"), "Label", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text = StringMgr.Instance.Get("RECORD") + ": " + BmWin.ToString() + "-" + BmDraw.ToString() + "-" + BmLose.ToString() + " (W-D-L)";
			LabelUtil.TextOut(pos2, text, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			float num = 0f;
			if (BmDeath + BmKill > 0)
			{
				num = (float)BmKill / (float)(BmKill + BmDeath) * 100f;
			}
			string text2 = StringMgr.Instance.Get("KILL") + "/" + StringMgr.Instance.Get("DEATH") + ": " + BmKill.ToString() + "/" + BmDeath.ToString() + " (" + num.ToString("0.##") + "%)";
			LabelUtil.TextOut(pos3, text2, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text3 = StringMgr.Instance.Get("ASSIST") + ": " + BmAssist.ToString();
			LabelUtil.TextOut(pos4, text3, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text4 = StringMgr.Instance.Get("GIVEUP") + ": " + BmGiveup.ToString();
			LabelUtil.TextOut(pos5, text4, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text5 = StringMgr.Instance.Get("BEST_SCORE") + ": " + BmScoreRecord.ToString();
			LabelUtil.TextOut(pos6, text5, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			crdCurFrame.y += 135f;
		}
	}

	private void DrawBND()
	{
		if (BuildOption.Instance.Props.bndMatchMode)
		{
			Vector2 pos = new Vector2(crdCurFrame.x + 10f, crdCurFrame.y);
			Vector2 pos2 = new Vector2(pos.x, pos.y + 25f);
			Vector2 pos3 = new Vector2(pos.x, pos.y + 45f);
			Vector2 pos4 = new Vector2(pos.x, pos.y + 65f);
			Vector2 pos5 = new Vector2(pos.x, pos.y + 85f);
			Vector2 pos6 = new Vector2(pos.x, pos.y + 105f);
			GUI.Box(crdCurFrame, string.Empty, "BoxBlue");
			LabelUtil.TextOut(pos, StringMgr.Instance.Get("ROOM_TYPE_BND"), "Label", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text = StringMgr.Instance.Get("RECORD") + ": " + BndWin.ToString() + "-" + BndDraw.ToString() + "-" + BndLose.ToString() + " (W-D-L)";
			LabelUtil.TextOut(pos2, text, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			float num = 0f;
			if (BndDeath + BndKill > 0)
			{
				num = (float)BndKill / (float)(BndKill + BndDeath) * 100f;
			}
			string text2 = StringMgr.Instance.Get("KILL") + "/" + StringMgr.Instance.Get("DEATH") + ": " + BndKill.ToString() + "/" + BndDeath.ToString() + " (" + num.ToString("0.##") + "%)";
			LabelUtil.TextOut(pos3, text2, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text3 = StringMgr.Instance.Get("ASSIST") + ": " + BndAssist.ToString();
			LabelUtil.TextOut(pos4, text3, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text4 = StringMgr.Instance.Get("GIVEUP") + ": " + BndGiveup.ToString();
			LabelUtil.TextOut(pos5, text4, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text5 = StringMgr.Instance.Get("BEST_SCORE") + ": " + BndScoreRecord.ToString();
			LabelUtil.TextOut(pos6, text5, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			crdCurFrame.y += 135f;
		}
	}

	private void DrawBungee()
	{
		if (BuildOption.Instance.Props.bungeeMode)
		{
			Vector2 pos = new Vector2(crdCurFrame.x + 10f, crdCurFrame.y);
			Vector2 pos2 = new Vector2(pos.x, pos.y + 25f);
			Vector2 pos3 = new Vector2(pos.x, pos.y + 45f);
			Vector2 pos4 = new Vector2(pos.x, pos.y + 65f);
			Vector2 pos5 = new Vector2(pos.x, pos.y + 85f);
			GUI.Box(crdCurFrame, string.Empty, "BoxBlue");
			LabelUtil.TextOut(pos, StringMgr.Instance.Get("ROOM_TYPE_BUNGEE"), "Label", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text = StringMgr.Instance.Get("PARTICIPATION_COUNT") + ": " + BungeeCount.ToString();
			LabelUtil.TextOut(pos2, text, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text2 = StringMgr.Instance.Get("DEATH") + ": " + BungeeDeath.ToString();
			LabelUtil.TextOut(pos3, text2, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text3 = StringMgr.Instance.Get("GIVEUP") + ": " + BungeeGiveup.ToString();
			LabelUtil.TextOut(pos4, text3, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text4 = StringMgr.Instance.Get("BEST_SCORE") + ": " + BungeeScoreRecord.ToString();
			LabelUtil.TextOut(pos5, text4, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			crdCurFrame.y += 115f;
		}
	}

	private void DrawEscape()
	{
		if (BuildOption.Instance.Props.escapeMode)
		{
			Vector2 pos = new Vector2(crdCurFrame.x + 10f, crdCurFrame.y);
			Vector2 pos2 = new Vector2(pos.x, pos.y + 25f);
			Vector2 pos3 = new Vector2(pos.x, pos.y + 45f);
			Vector2 pos4 = new Vector2(pos.x, pos.y + 65f);
			Vector2 pos5 = new Vector2(pos.x, pos.y + 85f);
			Vector2 pos6 = new Vector2(pos.x, pos.y + 105f);
			GUI.Box(crdCurFrame, string.Empty, "BoxBlue");
			LabelUtil.TextOut(pos, StringMgr.Instance.Get("ROOM_TYPE_ESCAPE"), "Label", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text = StringMgr.Instance.Get("PARTICIPATION_COUNT") + ": " + EscapeCount.ToString();
			LabelUtil.TextOut(pos2, text, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text2 = StringMgr.Instance.Get("ARRIVAL_COUNT") + ": " + EscapeGoalCount.ToString();
			LabelUtil.TextOut(pos3, text2, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			float num = 0f;
			if (EscapeCount > 0)
			{
				num = (float)EscapeGoalCount / (float)EscapeCount;
			}
			string text3 = StringMgr.Instance.Get("ARRIVAL_COUNT_AVERAGE") + ": " + num.ToString();
			LabelUtil.TextOut(pos4, text3, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text4 = StringMgr.Instance.Get("GIVEUP") + ": " + EscapeGiveup.ToString();
			LabelUtil.TextOut(pos5, text4, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text5 = StringMgr.Instance.Get("BEST_SCORE") + ": " + EscapeScoreRecord.ToString();
			LabelUtil.TextOut(pos6, text5, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			crdCurFrame.y += 135f;
		}
	}

	private void DrawZombie()
	{
		if (BuildOption.Instance.Props.zombieMode)
		{
			Vector2 pos = new Vector2(crdCurFrame.x + 10f, crdCurFrame.y);
			Vector2 pos2 = new Vector2(pos.x, pos.y + 25f);
			Vector2 pos3 = new Vector2(pos.x, pos.y + 45f);
			Vector2 pos4 = new Vector2(pos.x, pos.y + 65f);
			Vector2 pos5 = new Vector2(pos.x, pos.y + 85f);
			Vector2 pos6 = new Vector2(pos.x, pos.y + 105f);
			GUI.Box(crdCurFrame, string.Empty, "BoxBlue");
			LabelUtil.TextOut(pos, StringMgr.Instance.Get("ROOM_TYPE_ZOMBIE"), "Label", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text = StringMgr.Instance.Get("PARTICIPATION_COUNT") + ": " + ZombieCount.ToString();
			LabelUtil.TextOut(pos2, text, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text2 = StringMgr.Instance.Get("ZOMBIE_HUMAN_ALIVE") + ": " + ZombieHumanAlive.ToString();
			LabelUtil.TextOut(pos3, text2, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text3 = StringMgr.Instance.Get("ZOMBIE_ZOMBIE_ALIVE") + ": " + ZombieZombieAlive.ToString();
			LabelUtil.TextOut(pos4, text3, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text4 = StringMgr.Instance.Get("GIVEUP") + ": " + ZombieGiveup.ToString();
			LabelUtil.TextOut(pos5, text4, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text5 = StringMgr.Instance.Get("BEST_SCORE") + ": " + ZombieScoreRecord.ToString();
			LabelUtil.TextOut(pos6, text5, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			crdCurFrame.y += 135f;
		}
	}

	private void DrawCTF()
	{
		if (BuildOption.Instance.Props.ctfMatchMode)
		{
			Vector2 pos = new Vector2(crdCurFrame.x + 10f, crdCurFrame.y);
			Vector2 pos2 = new Vector2(pos.x, pos.y + 25f);
			Vector2 pos3 = new Vector2(pos.x, pos.y + 45f);
			Vector2 pos4 = new Vector2(pos.x, pos.y + 65f);
			Vector2 pos5 = new Vector2(pos.x, pos.y + 85f);
			Vector2 pos6 = new Vector2(pos.x, pos.y + 105f);
			GUI.Box(crdCurFrame, string.Empty, "BoxBlue");
			LabelUtil.TextOut(pos, StringMgr.Instance.Get("ROOM_TYPE_CAPTURE_THE_FLAG"), "Label", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text = StringMgr.Instance.Get("RECORD") + ": " + CtfWin.ToString() + "-" + CtfDraw.ToString() + "-" + CtfLose.ToString() + " (W-D-L)";
			LabelUtil.TextOut(pos2, text, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			float num = 0f;
			if (CtfDeath + CtfKill > 0)
			{
				num = (float)CtfKill / (float)(CtfKill + CtfDeath) * 100f;
			}
			string text2 = StringMgr.Instance.Get("KILL") + "/" + StringMgr.Instance.Get("DEATH") + ": " + CtfKill.ToString() + "/" + CtfDeath.ToString() + " (" + num.ToString("0.##") + "%)";
			LabelUtil.TextOut(pos3, text2, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text3 = StringMgr.Instance.Get("ASSIST") + ": " + CtfAssist.ToString();
			LabelUtil.TextOut(pos4, text3, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text4 = StringMgr.Instance.Get("GIVEUP") + ": " + CtfGiveup.ToString();
			LabelUtil.TextOut(pos5, text4, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			string text5 = StringMgr.Instance.Get("BEST_SCORE") + ": " + CtfScoreRecord.ToString();
			LabelUtil.TextOut(pos6, text5, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			crdCurFrame.y += 135f;
		}
	}

	private void DoDetailBattleLog()
	{
		LabelUtil.TextOut(crdDetailBattleLogLabel, StringMgr.Instance.Get("DETAIL_BATTLE_LOG"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		GUI.Box(crdDetailBattleLogFrame, string.Empty, "BoxPopLine");
		spDetail = GUI.BeginScrollView(crdDetailPosition, spDetail, crdDetailView);
		crdCurFrame = crdTmFrame;
		DrawTM();
		DrawIM();
		DrawDM();
		DrawBM();
		DrawCTF();
		DrawBND();
		DrawBungee();
		DrawEscape();
		DrawZombie();
		DrawWeaponRecord();
		if (!BuildOption.Instance.IsNetmarble && !BuildOption.Instance.IsDeveloper)
		{
			DrawWeaponLevel();
		}
		GUI.EndScrollView();
	}

	private void DrawWeaponLevel()
	{
		GUI.Box(new Rect(crdWeaponLvFrame.x, crdWeaponLvFrame.y - DefenseY - contorlY, crdWeaponLvFrame.width, crdWeaponLvFrame.height), string.Empty, "BoxBlue");
		LabelUtil.TextOut(new Vector2(crdWeaponLvLabel.x, crdWeaponLvLabel.y - DefenseY - contorlY), StringMgr.Instance.Get("WEAPON_LEVEL"), "Label", new Color(0.87f, 0.63f, 0.32f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(new Vector2(crdHeavyWpnLabel.x, crdHeavyWpnLabel.y - DefenseY - contorlY), StringMgr.Instance.Get("HEAVY_WPN"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(new Vector2(crdAssaultWpnLabel.x, crdAssaultWpnLabel.y - DefenseY - contorlY), StringMgr.Instance.Get("ASSAULT_WPN"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(new Vector2(crdSniperWpnLabel.x, crdSniperWpnLabel.y - DefenseY - contorlY), StringMgr.Instance.Get("SNIPER_WPN"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(new Vector2(crdSubMachineWpnLabel.x, crdSubMachineWpnLabel.y - DefenseY - contorlY), StringMgr.Instance.Get("SUB_MACHINE_WPN"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(new Vector2(crdHandGunWpnLabel.x, crdHandGunWpnLabel.y - DefenseY - contorlY), StringMgr.Instance.Get("HAND_GUN_WPN"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(new Vector2(crdMeleeWpnLabel.x, crdMeleeWpnLabel.y - DefenseY - contorlY), StringMgr.Instance.Get("MELEE_WPN"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(new Vector2(crdSpecialWpnLabel.x, crdSpecialWpnLabel.y - DefenseY - contorlY), StringMgr.Instance.Get("SPECIAL_WPN"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		int num = XpManager.Instance.GetWeaponLevel(TWeapon.CATEGORY.HEAVY, Heavy) + 1;
		float weaponLevelRatio = XpManager.Instance.GetWeaponLevelRatio(TWeapon.CATEGORY.HEAVY, Heavy);
		float num2 = weaponLevelRatio * 100f;
		string text = num.ToString() + "/" + XpManager.Instance.MaxWeaponLevel.ToString() + "Lv (" + num2.ToString("0.##") + "%)";
		heavyTooltip = StringMgr.Instance.Get("CUR_WEAPON_LV_EFFECT");
		int discountRatio = TWeapon.GetDiscountRatio(num);
		if (discountRatio <= 0)
		{
			heavyTooltip += StringMgr.Instance.Get("NO_WEAPON_LV_EFFECT");
		}
		else
		{
			heavyTooltip += string.Format(StringMgr.Instance.Get("HEAVY_LEVEL_UP_TOOLTIP"), discountRatio);
		}
		if (num < XpManager.Instance.MaxWeaponLevel)
		{
			heavyTooltip = heavyTooltip + "\n" + StringMgr.Instance.Get("NEXT_WEAPON_LV_EFFECT");
			discountRatio = TWeapon.GetDiscountRatio(num + 1);
			if (discountRatio <= 0)
			{
				heavyTooltip += StringMgr.Instance.Get("NO_WEAPON_LV_EFFECT");
			}
			else
			{
				heavyTooltip += string.Format(StringMgr.Instance.Get("HEAVY_LEVEL_UP_TOOLTIP"), discountRatio);
			}
		}
		int num3 = XpManager.Instance.GetWeaponLevel(TWeapon.CATEGORY.ASSAULT, Assault) + 1;
		float weaponLevelRatio2 = XpManager.Instance.GetWeaponLevelRatio(TWeapon.CATEGORY.ASSAULT, Assault);
		float num4 = weaponLevelRatio2 * 100f;
		string text2 = num3.ToString() + "/" + XpManager.Instance.MaxWeaponLevel.ToString() + "Lv (" + num4.ToString("0.##") + "%)";
		assaultTooltip = StringMgr.Instance.Get("CUR_WEAPON_LV_EFFECT");
		discountRatio = TWeapon.GetDiscountRatio(num3);
		if (discountRatio <= 0)
		{
			assaultTooltip += StringMgr.Instance.Get("NO_WEAPON_LV_EFFECT");
		}
		else
		{
			assaultTooltip += string.Format(StringMgr.Instance.Get("ASSAULT_LEVEL_UP_TOOLTIP"), discountRatio);
		}
		if (num3 < XpManager.Instance.MaxWeaponLevel)
		{
			assaultTooltip = assaultTooltip + "\n" + StringMgr.Instance.Get("NEXT_WEAPON_LV_EFFECT");
			discountRatio = TWeapon.GetDiscountRatio(num3 + 1);
			if (discountRatio <= 0)
			{
				assaultTooltip += StringMgr.Instance.Get("NO_WEAPON_LV_EFFECT");
			}
			else
			{
				assaultTooltip += string.Format(StringMgr.Instance.Get("ASSAULT_LEVEL_UP_TOOLTIP"), discountRatio);
			}
		}
		int num5 = XpManager.Instance.GetWeaponLevel(TWeapon.CATEGORY.SNIPER, Sniper) + 1;
		float weaponLevelRatio3 = XpManager.Instance.GetWeaponLevelRatio(TWeapon.CATEGORY.SNIPER, Sniper);
		float num6 = weaponLevelRatio3 * 100f;
		string text3 = num5.ToString() + "/" + XpManager.Instance.MaxWeaponLevel.ToString() + "Lv (" + num6.ToString("0.##") + "%)";
		sniperTooltip = StringMgr.Instance.Get("CUR_WEAPON_LV_EFFECT");
		discountRatio = TWeapon.GetDiscountRatio(num5);
		if (discountRatio <= 0)
		{
			sniperTooltip += StringMgr.Instance.Get("NO_WEAPON_LV_EFFECT");
		}
		else
		{
			sniperTooltip += string.Format(StringMgr.Instance.Get("SNIPER_LEVEL_UP_TOOLTIP"), discountRatio);
		}
		if (num5 < XpManager.Instance.MaxWeaponLevel)
		{
			sniperTooltip = sniperTooltip + "\n" + StringMgr.Instance.Get("NEXT_WEAPON_LV_EFFECT");
			discountRatio = TWeapon.GetDiscountRatio(num5 + 1);
			if (discountRatio <= 0)
			{
				sniperTooltip += StringMgr.Instance.Get("NO_WEAPON_LV_EFFECT");
			}
			else
			{
				sniperTooltip += string.Format(StringMgr.Instance.Get("SNIPER_LEVEL_UP_TOOLTIP"), discountRatio);
			}
		}
		int num7 = XpManager.Instance.GetWeaponLevel(TWeapon.CATEGORY.SUB_MACHINE, SubMachine) + 1;
		float weaponLevelRatio4 = XpManager.Instance.GetWeaponLevelRatio(TWeapon.CATEGORY.SUB_MACHINE, SubMachine);
		float num8 = weaponLevelRatio4 * 100f;
		string text4 = num7.ToString() + "/" + XpManager.Instance.MaxWeaponLevel.ToString() + "Lv (" + num8.ToString("0.##") + "%)";
		subMachineTooltip = StringMgr.Instance.Get("CUR_WEAPON_LV_EFFECT");
		discountRatio = TWeapon.GetDiscountRatio(num7);
		if (discountRatio <= 0)
		{
			subMachineTooltip += StringMgr.Instance.Get("NO_WEAPON_LV_EFFECT");
		}
		else
		{
			subMachineTooltip += string.Format(StringMgr.Instance.Get("SUB_MACHINE_LEVEL_UP_TOOLTIP"), discountRatio);
		}
		if (num7 < XpManager.Instance.MaxWeaponLevel)
		{
			subMachineTooltip = subMachineTooltip + "\n" + StringMgr.Instance.Get("NEXT_WEAPON_LV_EFFECT");
			discountRatio = TWeapon.GetDiscountRatio(num7 + 1);
			if (discountRatio <= 0)
			{
				subMachineTooltip += StringMgr.Instance.Get("NO_WEAPON_LV_EFFECT");
			}
			else
			{
				subMachineTooltip += string.Format(StringMgr.Instance.Get("SUB_MACHINE_LEVEL_UP_TOOLTIP"), discountRatio);
			}
		}
		int num9 = XpManager.Instance.GetWeaponLevel(TWeapon.CATEGORY.HAND_GUN, HandGun) + 1;
		float weaponLevelRatio5 = XpManager.Instance.GetWeaponLevelRatio(TWeapon.CATEGORY.HAND_GUN, HandGun);
		float num10 = weaponLevelRatio5 * 100f;
		string text5 = num9.ToString() + "/" + XpManager.Instance.MaxWeaponLevel.ToString() + "Lv (" + num10.ToString("0.##") + "%)";
		handgunTooltip = StringMgr.Instance.Get("CUR_WEAPON_LV_EFFECT");
		discountRatio = TWeapon.GetDiscountRatio(num9);
		if (discountRatio <= 0)
		{
			handgunTooltip += StringMgr.Instance.Get("NO_WEAPON_LV_EFFECT");
		}
		else
		{
			handgunTooltip += string.Format(StringMgr.Instance.Get("HAND_GUN_LEVEL_UP_TOOLTIP"), discountRatio);
		}
		if (num9 < XpManager.Instance.MaxWeaponLevel)
		{
			handgunTooltip = handgunTooltip + "\n" + StringMgr.Instance.Get("NEXT_WEAPON_LV_EFFECT");
			discountRatio = TWeapon.GetDiscountRatio(num9 + 1);
			if (discountRatio <= 0)
			{
				handgunTooltip += StringMgr.Instance.Get("NO_WEAPON_LV_EFFECT");
			}
			else
			{
				handgunTooltip += string.Format(StringMgr.Instance.Get("HAND_GUN_LEVEL_UP_TOOLTIP"), discountRatio);
			}
		}
		int num11 = XpManager.Instance.GetWeaponLevel(TWeapon.CATEGORY.MELEE, Melee) + 1;
		float weaponLevelRatio6 = XpManager.Instance.GetWeaponLevelRatio(TWeapon.CATEGORY.MELEE, Melee);
		float num12 = weaponLevelRatio6 * 100f;
		string text6 = num11.ToString() + "/" + XpManager.Instance.MaxWeaponLevel.ToString() + "Lv (" + num12.ToString("0.##") + "%)";
		meleeTooltip = StringMgr.Instance.Get("CUR_WEAPON_LV_EFFECT");
		discountRatio = TWeapon.GetDiscountRatio(num11);
		if (discountRatio <= 0)
		{
			meleeTooltip += StringMgr.Instance.Get("NO_WEAPON_LV_EFFECT");
		}
		else
		{
			meleeTooltip += string.Format(StringMgr.Instance.Get("MELEE_LEVEL_UP_TOOLTIP"), discountRatio);
		}
		if (num11 < XpManager.Instance.MaxWeaponLevel)
		{
			meleeTooltip = meleeTooltip + "\n" + StringMgr.Instance.Get("NEXT_WEAPON_LV_EFFECT");
			discountRatio = TWeapon.GetDiscountRatio(num11 + 1);
			if (discountRatio <= 0)
			{
				meleeTooltip += StringMgr.Instance.Get("NO_WEAPON_LV_EFFECT");
			}
			else
			{
				meleeTooltip += string.Format(StringMgr.Instance.Get("MELEE_LEVEL_UP_TOOLTIP"), discountRatio);
			}
		}
		int num13 = XpManager.Instance.GetWeaponLevel(TWeapon.CATEGORY.SPECIAL, Special) + 1;
		float weaponLevelRatio7 = XpManager.Instance.GetWeaponLevelRatio(TWeapon.CATEGORY.SPECIAL, Special);
		float num14 = weaponLevelRatio7 * 100f;
		string text7 = num13.ToString() + "/" + XpManager.Instance.MaxWeaponLevel.ToString() + "Lv (" + num14.ToString("0.##") + "%)";
		specialTooltip = StringMgr.Instance.Get("CUR_WEAPON_LV_EFFECT");
		discountRatio = TWeapon.GetDiscountRatio(num13);
		if (discountRatio <= 0)
		{
			specialTooltip += StringMgr.Instance.Get("NO_WEAPON_LV_EFFECT");
		}
		else
		{
			specialTooltip += string.Format(StringMgr.Instance.Get("SPECIAL_LEVEL_UP_TOOLTIP"), discountRatio);
		}
		if (num13 < XpManager.Instance.MaxWeaponLevel)
		{
			specialTooltip = specialTooltip + "\n" + StringMgr.Instance.Get("NEXT_WEAPON_LV_EFFECT");
			discountRatio = TWeapon.GetDiscountRatio(num13 + 1);
			if (discountRatio <= 0)
			{
				specialTooltip += StringMgr.Instance.Get("NO_WEAPON_LV_EFFECT");
			}
			else
			{
				specialTooltip += string.Format(StringMgr.Instance.Get("SPECIAL_LEVEL_UP_TOOLTIP"), discountRatio);
			}
		}
		GUI.Box(new Rect(crdHeavyWpnGaugeFrame.x, crdHeavyWpnGaugeFrame.y - DefenseY - contorlY, crdHeavyWpnGaugeFrame.width, crdHeavyWpnGaugeFrame.height), string.Empty, "BoxDark");
		TextureUtil.DrawTexture(new Rect(crdHeavyWpnGauge.x, crdHeavyWpnGauge.y - DefenseY - contorlY, weaponLevelRatio * crdHeavyWpnGauge.width, crdHeavyWpnGauge.height), gauge);
		LabelUtil.TextOut(new Vector2(crdHeavyWpnLevel.x, crdHeavyWpnLevel.y - DefenseY - contorlY), text, "MiniLabel", new Color(0.7f, 0.7f, 0.7f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		GUI.Box(new Rect(crdAssaultWpnGaugeFrame.x, crdAssaultWpnGaugeFrame.y - DefenseY - contorlY, crdAssaultWpnGaugeFrame.width, crdAssaultWpnGaugeFrame.height), string.Empty, "BoxDark");
		TextureUtil.DrawTexture(new Rect(crdAssaultWpnGauge.x, crdAssaultWpnGauge.y - DefenseY - contorlY, weaponLevelRatio2 * crdAssaultWpnGauge.width, crdAssaultWpnGauge.height), gauge);
		LabelUtil.TextOut(new Vector2(crdAssaultWpnLevel.x, crdAssaultWpnLevel.y - DefenseY - contorlY), text2, "MiniLabel", new Color(0.7f, 0.7f, 0.7f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		GUI.Box(new Rect(crdSniperWpnGaugeFrame.x, crdSniperWpnGaugeFrame.y - DefenseY - contorlY, crdSniperWpnGaugeFrame.width, crdSniperWpnGaugeFrame.height), string.Empty, "BoxDark");
		TextureUtil.DrawTexture(new Rect(crdSniperWpnGauge.x, crdSniperWpnGauge.y - DefenseY - contorlY, weaponLevelRatio3 * crdSniperWpnGauge.width, crdSniperWpnGauge.height), gauge);
		LabelUtil.TextOut(new Vector2(crdSniperWpnLevel.x, crdSniperWpnLevel.y - DefenseY - contorlY), text3, "MiniLabel", new Color(0.7f, 0.7f, 0.7f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		GUI.Box(new Rect(crdSubMachineWpnGaugeFrame.x, crdSubMachineWpnGaugeFrame.y - DefenseY - contorlY, crdSubMachineWpnGaugeFrame.width, crdSubMachineWpnGaugeFrame.height), string.Empty, "BoxDark");
		TextureUtil.DrawTexture(new Rect(crdSubMachineWpnGauge.x, crdSubMachineWpnGauge.y - DefenseY - contorlY, weaponLevelRatio4 * crdSubMachineWpnGauge.width, crdSubMachineWpnGauge.height), gauge);
		LabelUtil.TextOut(new Vector2(crdSubMachineWpnWpnLevel.x, crdSubMachineWpnWpnLevel.y - DefenseY - contorlY), text4, "MiniLabel", new Color(0.7f, 0.7f, 0.7f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		GUI.Box(new Rect(crdHandGunWpnGaugeFrame.x, crdHandGunWpnGaugeFrame.y - DefenseY - contorlY, crdHandGunWpnGaugeFrame.width, crdHandGunWpnGaugeFrame.height), string.Empty, "BoxDark");
		TextureUtil.DrawTexture(new Rect(crdHandGunWpnGauge.x, crdHandGunWpnGauge.y - DefenseY - contorlY, weaponLevelRatio5 * crdHandGunWpnGauge.width, crdHandGunWpnGauge.height), gauge);
		LabelUtil.TextOut(new Vector2(crdHandGunWpnLevel.x, crdHandGunWpnLevel.y - DefenseY - contorlY), text5, "MiniLabel", new Color(0.7f, 0.7f, 0.7f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		GUI.Box(new Rect(crdMeleeWpnGaugeFrame.x, crdMeleeWpnGaugeFrame.y - DefenseY - contorlY, crdMeleeWpnGaugeFrame.width, crdMeleeWpnGaugeFrame.height), string.Empty, "BoxDark");
		TextureUtil.DrawTexture(new Rect(crdMeleeWpnGauge.x, crdMeleeWpnGauge.y - DefenseY - contorlY, weaponLevelRatio6 * crdMeleeWpnGauge.width, crdMeleeWpnGauge.height), gauge);
		LabelUtil.TextOut(new Vector2(crdMeleeWpnLevel.x, crdMeleeWpnLevel.y - DefenseY - contorlY), text6, "MiniLabel", new Color(0.7f, 0.7f, 0.7f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		GUI.Box(new Rect(crdSpecialWpnGaugeFrame.x, crdSpecialWpnGaugeFrame.y - DefenseY - contorlY, crdSpecialWpnGaugeFrame.width, crdSpecialWpnGaugeFrame.height), string.Empty, "BoxDark");
		TextureUtil.DrawTexture(new Rect(crdSpecialWpnGauge.x, crdSpecialWpnGauge.y - DefenseY - contorlY, weaponLevelRatio7 * crdSpecialWpnGauge.width, crdSpecialWpnGauge.height), gauge);
		LabelUtil.TextOut(new Vector2(crdSpecialWpnLevel.x, crdSpecialWpnLevel.y - DefenseY - contorlY), text7, "MiniLabel", new Color(0.7f, 0.7f, 0.7f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		GUI.Button(new Rect(crdHeavyTooltip.x, crdHeavyTooltip.y - DefenseY - contorlY, crdHeavyTooltip.width, crdHeavyTooltip.height), new GUIContent(string.Empty, heavyTooltip), "InvisibleButton");
		GUI.Button(new Rect(crdAssaultTooltip.x, crdAssaultTooltip.y - DefenseY - contorlY, crdAssaultTooltip.width, crdAssaultTooltip.height), new GUIContent(string.Empty, assaultTooltip), "InvisibleButton");
		GUI.Button(new Rect(crdSniperTooltip.x, crdSniperTooltip.y - DefenseY - contorlY, crdSniperTooltip.width, crdSniperTooltip.height), new GUIContent(string.Empty, sniperTooltip), "InvisibleButton");
		GUI.Button(new Rect(crdSubmachineTooltip.x, crdSubmachineTooltip.y - DefenseY - contorlY, crdSubmachineTooltip.width, crdSubmachineTooltip.height), new GUIContent(string.Empty, subMachineTooltip), "InvisibleButton");
		GUI.Button(new Rect(crdHandgunTooltip.x, crdHandgunTooltip.y - DefenseY - contorlY, crdHandgunTooltip.width, crdHandgunTooltip.height), new GUIContent(string.Empty, handgunTooltip), "InvisibleButton");
		GUI.Button(new Rect(crdMeleeTooltip.x, crdMeleeTooltip.y - DefenseY - contorlY, crdMeleeTooltip.width, crdMeleeTooltip.height), new GUIContent(string.Empty, meleeTooltip), "InvisibleButton");
		GUI.Button(new Rect(crdSpecialTooltip.x, crdSpecialTooltip.y - DefenseY - contorlY, crdSpecialTooltip.width, crdSpecialTooltip.height), new GUIContent(string.Empty, specialTooltip), "InvisibleButton");
	}

	private void DrawWeaponRecord()
	{
		GUI.Box(new Rect(crdWeaponFrame.x, crdWeaponFrame.y - DefenseY - contorlY, crdWeaponFrame.width, crdWeaponFrame.height), string.Empty, "BoxBlue");
		LabelUtil.TextOut(new Vector2(crdWeaponLabel.x, crdWeaponLabel.y - DefenseY - contorlY), StringMgr.Instance.Get("WEAPON_RECORD"), "Label", new Color(0.87f, 0.63f, 0.32f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(new Vector2(crdMainLabel.x, crdMainLabel.y - DefenseY - contorlY), StringMgr.Instance.Get("MAIN_WEAPON"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(new Vector2(crdAuxLabel.x, crdAuxLabel.y - DefenseY - contorlY), StringMgr.Instance.Get("AUX_WEAPON"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(new Vector2(crdMeleeLabel.x, crdMeleeLabel.y - DefenseY - contorlY), StringMgr.Instance.Get("MELEE_WEAPON"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(new Vector2(crdSpecLabel.x, crdSpecLabel.y - DefenseY - contorlY), StringMgr.Instance.Get("SPEC_WEAPON"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		int num = MainKill + AuxKill + MeleeKill + SpecKill;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		if (num > 0)
		{
			num2 = (float)MainKill / (float)num;
			num3 = (float)AuxKill / (float)num;
			num4 = (float)MeleeKill / (float)num;
			num5 = (float)SpecKill / (float)num;
		}
		float num6 = num2 * 100f;
		float num7 = num3 * 100f;
		float num8 = num4 * 100f;
		float num9 = num5 * 100f;
		GUI.Box(new Rect(crdMainGaugeFrame.x, crdMainGaugeFrame.y - DefenseY - contorlY, crdMainGaugeFrame.width, crdMainGaugeFrame.height), string.Empty, "BoxDark");
		TextureUtil.DrawTexture(new Rect(crdMainGauge.x, crdMainGauge.y - DefenseY - contorlY, num2 * crdMainGauge.width, crdMainGauge.height), gauge);
		LabelUtil.TextOut(new Vector2(crdMainPercent.x, crdMainPercent.y - DefenseY - contorlY), num6.ToString("0.##") + "%", "MiniLabel", new Color(0.7f, 0.7f, 0.7f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		GUI.Box(new Rect(crdAuxGaugeFrame.x, crdAuxGaugeFrame.y - DefenseY - contorlY, crdAuxGaugeFrame.width, crdAuxGaugeFrame.height), string.Empty, "BoxDark");
		TextureUtil.DrawTexture(new Rect(crdAuxGauge.x, crdAuxGauge.y - DefenseY - contorlY, num3 * crdAuxGauge.width, crdAuxGauge.height), gauge);
		LabelUtil.TextOut(new Vector2(crdAuxPercent.x, crdAuxPercent.y - DefenseY - contorlY), num7.ToString("0.##") + "%", "MiniLabel", new Color(0.7f, 0.7f, 0.7f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		GUI.Box(new Rect(crdMeleeGaugeFrame.x, crdMeleeGaugeFrame.y - DefenseY - contorlY, crdMeleeGaugeFrame.width, crdMeleeGaugeFrame.height), string.Empty, "BoxDark");
		TextureUtil.DrawTexture(new Rect(crdMeleeGauge.x, crdMeleeGauge.y - DefenseY - contorlY, num4 * crdMeleeGauge.width, crdMeleeGauge.height), gauge);
		LabelUtil.TextOut(new Vector2(crdMeleePercent.x, crdMeleePercent.y - DefenseY - contorlY), num8.ToString("0.##") + "%", "MiniLabel", new Color(0.7f, 0.7f, 0.7f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		GUI.Box(new Rect(crdSpecGaugeFrame.x, crdSpecGaugeFrame.y - DefenseY - contorlY, crdSpecGaugeFrame.width, crdSpecGaugeFrame.height), string.Empty, "BoxDark");
		TextureUtil.DrawTexture(new Rect(crdSpecGauge.x, crdSpecGauge.y - DefenseY - contorlY, num5 * crdSpecGauge.width, crdSpecGauge.height), gauge);
		LabelUtil.TextOut(new Vector2(crdSpecPercent.x, crdSpecPercent.y - DefenseY - contorlY), num9.ToString("0.##") + "%", "MiniLabel", new Color(0.7f, 0.7f, 0.7f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		DoTitle();
		DoPortrait();
		DoIdentity();
		DoTotalBattleLog();
		DoAsset();
		DoDetailBattleLog();
		Rect rc = new Rect(size.x - 50f, 10f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
		}
		if (GUI.tooltip.Length > 0 && (heavyTooltip == GUI.tooltip || assaultTooltip == GUI.tooltip || sniperTooltip == GUI.tooltip || subMachineTooltip == GUI.tooltip || handgunTooltip == GUI.tooltip || meleeTooltip == GUI.tooltip || specialTooltip == GUI.tooltip))
		{
			Vector2 mousePosition = Event.current.mousePosition;
			GUIStyle style = GUI.skin.GetStyle("MiniLabel");
			if (style != null)
			{
				Vector2 vector = style.CalcSize(new GUIContent(GUI.tooltip));
				Rect position = new Rect(mousePosition.x, mousePosition.y, vector.x + 24f, vector.y + 24f);
				float num = position.x + position.width;
				float width = base.rc.width;
				if (num > width)
				{
					position.x -= num - width;
				}
				GUI.Box(position, string.Empty, "LineWindow");
				GUI.Label(new Rect(position.x + 12f, position.y + 12f, position.width, position.height), GUI.tooltip, "MiniLabel");
			}
		}
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}
}
