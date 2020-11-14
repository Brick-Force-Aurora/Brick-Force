using System.Collections.Generic;
using UnityEngine;

public class MyInfoManager : MonoBehaviour
{
	public enum RESET_BATTLE_RECORD
	{
		RESET_BATTLE_RECORD_ALL = 0,
		RESET_BATTLE_RECORD_TEAM_MATCH = 1,
		RESET_BATTLE_RECORD_INDIVIDUAL_MATCH = 2,
		RESET_BATTLE_RECORD_CTF = 3,
		RESET_BATTLE_RECORD_BLAST_MODE = 4,
		RESET_BATTLE_RECORD_DEFENSE_MODE = 5,
		RESET_BATTLE_RECORD_BND_MODE = 6,
		RESET_BATTLE_RECORD_BUNGEE = 7,
		RESET_BATTLE_RECORD_WEAPON = 0x10
	}

	public enum COMMON_OPT
	{
		DONOT_NEWBIE_CHANNEL_MSG = 1,
		DONOT_BUNGEE_GUIDE = 2,
		DONOT_MAPEDIT_GUIDE = 4,
		DONOT_BND_GUIDE = 8,
		DONOT_EXPLOSION_ATTACK_GUIDE = 0x10,
		DONOT_EXPLOSION_DEFENCE_GUIDE = 0x20,
		INVITE_MASK1_FRIEND = 0x40,
		INVITE_MASK2_ALL_NO = 0x80,
		WHISPER_MASK1_FRIEND = 0x100,
		WHISPER_MASK2_ALL_NO = 0x200,
		DONOT_BATTLE_GUIDE = 0x400,
		DONOT_ZOMBIE_GUIDE = 0x800,
		DONOT_FLAG_GUIDE = 0x1000,
		DONOT_DEFENSE_GUIDE = 0x2000,
		DONOT_ESCAPE_GUIDE = 0x4000
	}

	public enum CONTROL_MODE
	{
		NONE = -1,
		PLAY_MODE,
		SPECTATOR_MODE,
		FLY_MODE,
		PLAYING_SPECTATOR,
		PLAYING_FLY_MODE
	}

	public enum RESULT_EVENT
	{
		NONE,
		LEVEL_UP,
		HEAVY_LEVEL_UP,
		ASSAULT_LEVEL_UP,
		SNIPER_LEVEL_UP,
		SUBMACHINE_LEVEL_UP,
		HANDGUN_LEVEL_UP,
		MELEE_LEVEL_UP,
		SPECIAL_LEVEL_UP
	}

	public enum AUTOLOGIN
	{
		NONE,
		INFERNUM,
		RUNUP,
		NETMARBLE
	}

	public const byte SITE_CODE_DEFAULT = 0;

	public const byte SITE_CODE_NETMARBLE = 1;

	public const byte SITE_CODE_TOONYLAND = 11;

	private float deltaTime;

	private float checkOneSecond;

	private Dictionary<int, NameCard> friends;

	private Dictionary<int, NameCard> bans;

	private Dictionary<int, NameCard> clanee;

	private Queue<RESULT_EVENT> qResultEvent;

	private Queue<DurabilityEvent> qDurabilityEvent;

	private Queue<string> qBattleStartRemain;

	private int clanApplicant = -1;

	private float pingTime;

	private int avgPingTime = -1;

	private AUTOLOGIN autoLogin;

	private bool longTimePlayReward;

	private bool longTimePlayActive;

	private static MyInfoManager _instance;

	private Queue<float> chatFixedTimeQ = new Queue<float>();

	private bool chatBlocked;

	private float deltaChatBlocked;

	public SpeedHackProtector spdhackProtector;

	public bool IsEditor;

	public bool MsgBoxConfirm;

	private int seq;

	private int playAuthCode;

	private int age = 100;

	private byte siteCode;

	private string nickname;

	private int xp;

	private int rank;

	private sbyte tutorialed;

	private bool onceTutorialAlways;

	private int countryFilter;

	private bool agreeTos;

	private bool needPlayerInfo;

	private int firstLoginFp;

	private int point;

	private int brick;

	private int cash;

	private int freeCoin;

	private int starDust;

	private int clanSeq;

	private string clanName;

	private int clanMark;

	private int clanLv;

	private string clanMaName;

	private int senseBombSeq;

	private int extraSlots;

	private sbyte slot = -1;

	private int status;

	private bool isRounding;

	private int clanMatchRounding;

	private int ticket;

	private bool breakingInto;

	public ExplosionMatchDesc BlastModeDesc;

	public BuildNDestroyModeDesc BndModeDesc;

	private bool isModified;

	public bool bBombInstallFail;

	public bool bBombUnInstallFail;

	private int kill;

	private int death;

	private int assist;

	private int mission;

	private int score;

	private int heavy;

	private int assault;

	private int sniper;

	private int subMachine;

	private int handGun;

	private int melee;

	private int special;

	private int gm;

	public int qjModeMask;

	public int qjOfficialMask;

	public int qjCommonMask;

	private bool switchGOD;

	private bool switchGUI = true;

	private bool IsGuiOn = true;

	private bool IsStraightMovement;

	private bool IsInvisibilityOn;

	private bool IsGhostOn;

	private CONTROL_MODE controlMode;

	public Dictionary<long, Item> inventory;

	private long[] wpnChg = new long[4];

	private long[] drpItm = new long[4];

	private long[] shooterTools;

	private long[] weaponSlots;

	private bool isGunEmpty;

	private bool isYang;

	public string charCode = string.Empty;

	public int bombAmmo = 1;

	public int ClanApplicant
	{
		get
		{
			return clanApplicant;
		}
		set
		{
			clanApplicant = value;
		}
	}

	public float PingTime
	{
		get
		{
			return pingTime;
		}
		set
		{
			pingTime = value;
		}
	}

	public bool IsSpectator => controlMode == CONTROL_MODE.SPECTATOR_MODE || controlMode == CONTROL_MODE.PLAYING_SPECTATOR;

	public int AvgPingTime
	{
		get
		{
			return avgPingTime;
		}
		set
		{
			avgPingTime = value;
		}
	}

	public bool IsAutoLogin => autoLogin != AUTOLOGIN.NONE;

	public AUTOLOGIN AutoLogin
	{
		get
		{
			return autoLogin;
		}
		set
		{
			autoLogin = value;
		}
	}

	public bool UseLongTimePlayReward
	{
		get
		{
			return longTimePlayReward;
		}
		set
		{
			longTimePlayReward = value;
		}
	}

	public bool IsLongTimePlayActive
	{
		get
		{
			return longTimePlayActive;
		}
		set
		{
			longTimePlayActive = value;
		}
	}

	public static MyInfoManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(MyInfoManager)) as MyInfoManager);
			}
			return _instance;
		}
	}

	public bool FlyingFast
	{
		get
		{
			long num = HaveFunction("fly_fast");
			return num >= 0 && controlMode == CONTROL_MODE.PLAYING_FLY_MODE;
		}
	}

	public int Seq
	{
		get
		{
			return seq;
		}
		set
		{
			seq = value;
		}
	}

	public int PlayAuthCode
	{
		get
		{
			return playAuthCode;
		}
		set
		{
			playAuthCode = value;
		}
	}

	public int Age
	{
		get
		{
			return age;
		}
		set
		{
			age = value;
		}
	}

	public byte SiteCode
	{
		get
		{
			return siteCode;
		}
		set
		{
			siteCode = value;
			if (siteCode == 11)
			{
				TokenManager.Instance.SetCurrentToken(Token.TYPE.TOONY);
			}
		}
	}

	public string Nickname
	{
		get
		{
			return nickname;
		}
		set
		{
			nickname = value;
		}
	}

	public int Xp
	{
		get
		{
			return xp;
		}
		set
		{
			xp = value;
		}
	}

	public int Rank
	{
		get
		{
			return rank;
		}
		set
		{
			rank = value;
		}
	}

	public sbyte Tutorialed
	{
		get
		{
			return tutorialed;
		}
		set
		{
			tutorialed = value;
		}
	}

	public bool OnceTutorialAlways
	{
		get
		{
			return onceTutorialAlways;
		}
		set
		{
			onceTutorialAlways = value;
		}
	}

	public bool IsNewbie => XpManager.Instance.GetLevel(xp) <= BuildOption.Instance.Props.maxNewbieLevel;

	public int CountryFilter
	{
		get
		{
			return countryFilter;
		}
		set
		{
			countryFilter = value;
		}
	}

	public bool AgreeTos
	{
		get
		{
			return agreeTos;
		}
		set
		{
			agreeTos = value;
		}
	}

	public bool NeedPlayerInfo
	{
		get
		{
			return needPlayerInfo;
		}
		set
		{
			needPlayerInfo = value;
		}
	}

	public int FirstLoginFp
	{
		get
		{
			return firstLoginFp;
		}
		set
		{
			firstLoginFp = value;
		}
	}

	public bool Tutorialable => tutorialed == 0;

	public bool BattleTutorialable => tutorialed == 0 || tutorialed == 2;

	public int Point
	{
		get
		{
			return point;
		}
		set
		{
			point = value;
		}
	}

	public int BrickPoint
	{
		get
		{
			return brick;
		}
		set
		{
			brick = value;
		}
	}

	public int Cash
	{
		get
		{
			return cash;
		}
		set
		{
			cash = value;
		}
	}

	public int FreeCoin
	{
		get
		{
			return freeCoin;
		}
		set
		{
			freeCoin = value;
		}
	}

	public int StarDust
	{
		get
		{
			return starDust;
		}
		set
		{
			starDust = value;
		}
	}

	public int ClanSeq
	{
		get
		{
			return clanSeq;
		}
		set
		{
			clanSeq = value;
		}
	}

	public string ClanName
	{
		get
		{
			return clanName;
		}
		set
		{
			clanName = value;
		}
	}

	public int ClanMark
	{
		get
		{
			return clanMark;
		}
		set
		{
			clanMark = value;
		}
	}

	public int ClanLv
	{
		get
		{
			return clanLv;
		}
		set
		{
			clanLv = value;
		}
	}

	public string ClanMaName
	{
		get
		{
			return clanMaName;
		}
		set
		{
			clanMaName = value;
		}
	}

	public int SenseBombSeq
	{
		get
		{
			return senseBombSeq;
		}
		set
		{
			senseBombSeq = value;
		}
	}

	public int ExtraSlots
	{
		get
		{
			return extraSlots;
		}
		set
		{
			extraSlots = value;
		}
	}

	public bool IsClanMember => clanLv >= 0;

	public bool IsClanStaff => clanLv >= 1;

	public bool IsClanMaster => clanLv == 2;

	public sbyte Slot
	{
		get
		{
			return slot;
		}
		set
		{
			slot = value;
		}
	}

	public int Status
	{
		get
		{
			return status;
		}
		set
		{
			status = value;
		}
	}

	public bool IsRounding => isRounding;

	public int Ticket
	{
		get
		{
			return ticket;
		}
		set
		{
			ticket = value;
		}
	}

	public bool BreakingInto
	{
		get
		{
			return breakingInto;
		}
		set
		{
			breakingInto = value;
		}
	}

	public bool IsModified
	{
		get
		{
			return isModified;
		}
		set
		{
			isModified = value;
		}
	}

	public int Kill
	{
		get
		{
			return kill;
		}
		set
		{
			kill = value;
		}
	}

	public int Death
	{
		get
		{
			return death;
		}
		set
		{
			death = value;
		}
	}

	public int Assist
	{
		get
		{
			return assist;
		}
		set
		{
			assist = value;
		}
	}

	public int Mission
	{
		get
		{
			return mission;
		}
		set
		{
			mission = value;
		}
	}

	public int Score
	{
		get
		{
			return score;
		}
		set
		{
			score = value;
		}
	}

	public int Heavy
	{
		get
		{
			return heavy;
		}
		set
		{
			heavy = value;
		}
	}

	public int Assault
	{
		get
		{
			return assault;
		}
		set
		{
			assault = value;
		}
	}

	public int Sniper
	{
		get
		{
			return sniper;
		}
		set
		{
			sniper = value;
		}
	}

	public int SubMachine
	{
		get
		{
			return subMachine;
		}
		set
		{
			subMachine = value;
		}
	}

	public int HandGun
	{
		get
		{
			return handGun;
		}
		set
		{
			handGun = value;
		}
	}

	public int Melee
	{
		get
		{
			return melee;
		}
		set
		{
			melee = value;
		}
	}

	public int Special
	{
		get
		{
			return special;
		}
		set
		{
			special = value;
		}
	}

	public int GM
	{
		get
		{
			return gm;
		}
		set
		{
			gm = value;
		}
	}

	public bool IsGM => gm != 0;

	public bool GodMode
	{
		get
		{
			return switchGOD && IsGM;
		}
		set
		{
			switchGOD = value;
		}
	}

	public bool SwitchGUI
	{
		get
		{
			return switchGUI;
		}
		set
		{
			switchGUI = value;
		}
	}

	public bool isGuiOn
	{
		get
		{
			return IsGuiOn;
		}
		set
		{
			IsGuiOn = value;
		}
	}

	public bool isStraightMovement
	{
		get
		{
			return IsStraightMovement;
		}
		set
		{
			IsStraightMovement = value;
		}
	}

	public bool isInvisibilityOn
	{
		get
		{
			return IsInvisibilityOn;
		}
		set
		{
			P2PManager.Instance.SendPEER_INVISIBLILITY(value);
			IsInvisibilityOn = value;
		}
	}

	public bool isGhostOn
	{
		get
		{
			return IsGhostOn;
		}
		set
		{
			IsGhostOn = value;
		}
	}

	public CONTROL_MODE ControlMode
	{
		get
		{
			return controlMode;
		}
		set
		{
			controlMode = value;
		}
	}

	public long[] ShooterTools => shooterTools;

	public long[] WeaponSlots => weaponSlots;

	public bool IsGunEmpty
	{
		get
		{
			return isGunEmpty;
		}
		set
		{
			isGunEmpty = value;
		}
	}

	public bool IsYang
	{
		get
		{
			return isYang;
		}
		set
		{
			isYang = value;
		}
	}

	public bool IsEmptyClan => clanSeq >= 0 && clanee.Count <= 0;

	private void UpdateChatLimit()
	{
		bool flag = chatFixedTimeQ.Count > 0;
		while (flag)
		{
			float num = chatFixedTimeQ.Peek();
			if (Mathf.Abs(Time.fixedTime - num) < (float)CustomGameConfig.limitChatTime)
			{
				flag = false;
			}
			else
			{
				chatFixedTimeQ.Dequeue();
				if (chatFixedTimeQ.Count <= 0)
				{
					flag = false;
				}
			}
		}
		if (chatBlocked)
		{
			deltaChatBlocked += Time.deltaTime;
			if (deltaChatBlocked > (float)CustomGameConfig.chatBlockTime)
			{
				chatBlocked = false;
				deltaChatBlocked = 0f;
			}
		}
	}

	public bool CheckChatTime()
	{
		if (CustomGameConfig.limitChatTime <= 0 || CustomGameConfig.limitChatCount <= 0 || CustomGameConfig.chatBlockTime <= 0)
		{
			return true;
		}
		if (chatBlocked)
		{
			return false;
		}
		chatFixedTimeQ.Enqueue(Time.fixedTime);
		if (chatFixedTimeQ.Count > CustomGameConfig.limitChatCount)
		{
			chatBlocked = true;
			deltaChatBlocked = 0f;
			return false;
		}
		return true;
	}

	public void InitializePacketVariation()
	{
		if (spdhackProtector == null)
		{
			spdhackProtector = new SpeedHackProtector();
		}
		spdhackProtector.InitializePacketVariation();
	}

	private void OnApplicationQuit()
	{
	}

	private void Awake()
	{
		autoLogin = AUTOLOGIN.NONE;
		controlMode = CONTROL_MODE.PLAY_MODE;
		deltaTime = 0f;
		checkOneSecond = 0f;
		inventory = new Dictionary<long, Item>();
		friends = new Dictionary<int, NameCard>();
		bans = new Dictionary<int, NameCard>();
		clanee = new Dictionary<int, NameCard>();
		qResultEvent = new Queue<RESULT_EVENT>();
		qDurabilityEvent = new Queue<DurabilityEvent>();
		qBattleStartRemain = new Queue<string>();
		shooterTools = new long[5];
		ClearShooterTool();
		if (BuildOption.Instance.IsNetmarble)
		{
			weaponSlots = new long[5];
		}
		else
		{
			weaponSlots = new long[10];
		}
		ClearWeaponSlot();
		ResetWpnChg();
		ResetDrpWpn();
		Object.DontDestroyOnLoad(this);
	}

	public void InitInfo(int _xp, sbyte _tutorialed, int _countryFilter, sbyte _tos, int _extraSlots, int _rank, int _firstLoginFp)
	{
		xp = _xp;
		tutorialed = _tutorialed;
		onceTutorialAlways = false;
		countryFilter = _countryFilter;
		agreeTos = (_tos != 0);
		extraSlots = _extraSlots;
		needPlayerInfo = false;
		rank = _rank;
		firstLoginFp = _firstLoginFp;
	}

	public void ClearWeaponSlot()
	{
		for (int i = 0; i < weaponSlots.Length; i++)
		{
			weaponSlots[i] = -1L;
		}
	}

	public void ClearShooterTool()
	{
		for (int i = 0; i < shooterTools.Length; i++)
		{
			shooterTools[i] = -1L;
		}
	}

	public void EnDurabilityEvent(string code, int durability, int diff)
	{
		qDurabilityEvent.Enqueue(new DurabilityEvent(code, durability, diff));
	}

	public DurabilityEvent DeDurabilityEvent()
	{
		if (qDurabilityEvent.Count <= 0)
		{
			return null;
		}
		return qDurabilityEvent.Dequeue();
	}

	public void EnResultEvent(RESULT_EVENT resultEvent)
	{
		qResultEvent.Enqueue(resultEvent);
	}

	public void EnBattleStartRemain(string msg)
	{
		qBattleStartRemain.Enqueue(msg);
	}

	public string DeBattleStartRemain()
	{
		if (qBattleStartRemain.Count <= 0)
		{
			return null;
		}
		return qBattleStartRemain.Dequeue();
	}

	public void DeResultEvent()
	{
		while (qResultEvent.Count > 0)
		{
			string text = string.Empty;
			switch (qResultEvent.Dequeue())
			{
			case RESULT_EVENT.LEVEL_UP:
				text = string.Format(StringMgr.Instance.Get("MY_LEVEL_UP_NOTICE"), XpManager.Instance.GetRank(XpManager.Instance.GetLevel(Xp), Rank));
				break;
			case RESULT_EVENT.HEAVY_LEVEL_UP:
				text = string.Format(StringMgr.Instance.Get("HEAVY_LEVEL_UP_NOTICE"), XpManager.Instance.GetWeaponLevel4Player(TWeapon.CATEGORY.HEAVY, Heavy));
				break;
			case RESULT_EVENT.ASSAULT_LEVEL_UP:
				text = string.Format(StringMgr.Instance.Get("ASSAULT_LEVEL_UP_NOTICE"), XpManager.Instance.GetWeaponLevel4Player(TWeapon.CATEGORY.ASSAULT, Assault));
				break;
			case RESULT_EVENT.SNIPER_LEVEL_UP:
				text = string.Format(StringMgr.Instance.Get("SNIPER_LEVEL_UP_NOTICE"), XpManager.Instance.GetWeaponLevel4Player(TWeapon.CATEGORY.SNIPER, Sniper));
				break;
			case RESULT_EVENT.SUBMACHINE_LEVEL_UP:
				text = string.Format(StringMgr.Instance.Get("SUB_MACHINE_LEVEL_UP_NOTICE"), XpManager.Instance.GetWeaponLevel4Player(TWeapon.CATEGORY.SUB_MACHINE, SubMachine));
				break;
			case RESULT_EVENT.HANDGUN_LEVEL_UP:
				text = string.Format(StringMgr.Instance.Get("HAND_GUN_LEVEL_UP_NOTICE"), XpManager.Instance.GetWeaponLevel4Player(TWeapon.CATEGORY.HAND_GUN, HandGun));
				break;
			case RESULT_EVENT.MELEE_LEVEL_UP:
				text = string.Format(StringMgr.Instance.Get("MELEE_LEVEL_UP_NOTICE"), XpManager.Instance.GetWeaponLevel4Player(TWeapon.CATEGORY.MELEE, Melee));
				break;
			case RESULT_EVENT.SPECIAL_LEVEL_UP:
				text = string.Format(StringMgr.Instance.Get("SPECIAL_LEVEL_UP_NOTICE"), XpManager.Instance.GetWeaponLevel4Player(TWeapon.CATEGORY.SPECIAL, Special));
				break;
			}
			if (text.Length >= 0)
			{
				MessageBoxMgr.Instance.AddMessage(text);
			}
		}
	}

	public void Clear()
	{
		ClearGmFunction();
		inventory.Clear();
		friends.Clear();
		bans.Clear();
		clanee.Clear();
		qResultEvent.Clear();
		qDurabilityEvent.Clear();
		ClearShooterTool();
		ClearWeaponSlot();
		gm = 0;
		controlMode = CONTROL_MODE.PLAY_MODE;
		ResetWpnChg();
		ResetDrpWpn();
		clanSeq = -1;
		clanName = string.Empty;
		clanMark = -1;
		clanLv = -1;
		chatFixedTimeQ.Clear();
		chatBlocked = false;
		deltaChatBlocked = 0f;
	}

	public void ResetGameStuff()
	{
		ClearGmFunction();
		IsEditor = false;
		breakingInto = false;
		BlastModeDesc = null;
		BndModeDesc = null;
		kill = 0;
		death = 0;
		assist = 0;
		mission = 0;
		score = 0;
		isRounding = false;
		clanMatchRounding = 0;
		ResetWpnChg();
		ResetDrpWpn();
		if (controlMode == CONTROL_MODE.PLAYING_SPECTATOR)
		{
			controlMode = CONTROL_MODE.NONE;
		}
	}

	private void ResetWpnChg()
	{
		for (int i = 0; i < wpnChg.Length; i++)
		{
			wpnChg[i] = -1L;
		}
	}

	private void ResetDrpWpn()
	{
		for (int i = 0; i < drpItm.Length; i++)
		{
			drpItm[i] = -1L;
		}
	}

	public void TurnoffFlyMode()
	{
		if (!IsGM)
		{
			controlMode = CONTROL_MODE.NONE;
		}
	}

	public void ToggleFlyModeByJetPack()
	{
		long num = HaveFunction("fly");
		long num2 = HaveFunction("fly_fast");
		if (num >= 0 || num2 >= 0)
		{
			if (controlMode == CONTROL_MODE.PLAYING_FLY_MODE)
			{
				controlMode = CONTROL_MODE.NONE;
			}
			else
			{
				controlMode = CONTROL_MODE.PLAYING_FLY_MODE;
			}
		}
	}

	private void Update()
	{
		UpdateChatLimit();
		deltaTime += Time.deltaTime;
		if (deltaTime > 1f && !Application.isLoadingLevel && (Application.loadedLevelName == "Lobby" || Application.loadedLevelName.Contains("Briefing")))
		{
			deltaTime = 0f;
			foreach (KeyValuePair<long, Item> item in inventory)
			{
				if (item.Value.IsExpiring() && item.Value.Template != null)
				{
					long expiring = item.Value.Seq;
					long alter = -1L;
					if (item.Value.Usage == Item.USAGE.EQUIP && TItem.SLOT.UPPER <= item.Value.Template.slot && item.Value.Template.slot <= TItem.SLOT.BOMB)
					{
						alter = GetAnyNotUsingUnlimitBySlot(item.Value.Template.slot).Seq;
					}
					else if (item.Value.Usage == Item.USAGE.EQUIP && (item.Value.Template.code == "s08" || item.Value.Template.code == "s09" || item.Value.Template.code == "s92"))
					{
						alter = GetAnyNotUsingUnlimitBySlot(TItem.SLOT.NONE).Seq;
					}
					CSNetManager.Instance.Sock.SendCS_CHECK_TERM_ITEM_REQ(expiring, alter);
				}
				if (item.Value.IsPremium && item.Value.Usage == Item.USAGE.EQUIP)
				{
					long num = HaveFunction("premium_account");
					if (num < 0)
					{
						if (TItem.SLOT.UPPER <= item.Value.Template.slot && item.Value.Template.slot <= TItem.SLOT.BOMB)
						{
							CSNetManager.Instance.Sock.SendCS_EQUIP_REQ(GetAnyNotUsingUnlimitBySlot(item.Value.Template.slot).Seq);
						}
						else if (item.Value.Template.code == "s08" || item.Value.Template.code == "s09" || item.Value.Template.code == "s92")
						{
							CSNetManager.Instance.Sock.SendCS_EQUIP_REQ(GetAnyNotUsingUnlimitBySlot(TItem.SLOT.NONE).Seq);
						}
					}
				}
			}
		}
		checkOneSecond += Time.deltaTime;
		if (checkOneSecond > 1f)
		{
			while (checkOneSecond > 1f)
			{
				checkOneSecond -= 1f;
				foreach (KeyValuePair<long, Item> item2 in inventory)
				{
					item2.Value.TickTok();
				}
			}
			if (checkOneSecond < 0f)
			{
				checkOneSecond = 0f;
			}
		}
	}

	public bool IsBelow12()
	{
		if (BuildOption.Instance.IsDeveloper && BuildOption.Instance.Props.MyAge < 12)
		{
			return true;
		}
		if (BuildOption.Instance.IsNetmarble && age < 12)
		{
			return true;
		}
		return false;
	}

	public void TermItemExpired(long seq)
	{
		if (inventory.ContainsKey(seq))
		{
			inventory[seq].Usage = Item.USAGE.DELETED;
			inventory[seq].Remain = 0;
		}
	}

	public void SetDurability(long seq, int durability)
	{
		if (inventory.ContainsKey(seq) && inventory[seq].Durability >= 0)
		{
			inventory[seq].Durability = durability;
		}
	}

	public void SetItem(long seq, string code, Item.USAGE usage, int remain, sbyte premium, int durability)
	{
		if (code == "c17" && usage == Item.USAGE.EQUIP)
		{
			GlobalVars.Instance.SetYangDingVoice(bSet: true);
		}
		if (inventory.ContainsKey(seq))
		{
			inventory[seq].Refresh(usage, remain, premium, durability);
		}
		else
		{
			if (premium != 0 && premium != 1 && premium != 2)
			{
				Debug.LogError("Invalid premium code for new item");
			}
			TItem tItem = TItemManager.Instance.Get<TItem>(code);
			if (tItem == null)
			{
				Debug.LogError("Fail to find item template for " + code);
			}
			else
			{
				inventory.Add(seq, new Item(seq, tItem, code, usage, remain, premium, durability));
			}
		}
	}

	public void Erase(long seq)
	{
		for (int i = 0; i < weaponSlots.Length; i++)
		{
			if (weaponSlots[i] >= 0 && weaponSlots[i] == seq)
			{
				weaponSlots[i] = -1L;
			}
		}
		for (int j = 0; j < shooterTools.Length; j++)
		{
			if (shooterTools[j] >= 0 && shooterTools[j] == seq)
			{
				shooterTools[j] = -1L;
			}
		}
		if (inventory.ContainsKey(seq))
		{
			inventory.Remove(seq);
		}
	}

	public void RebuyItem(long seq, string code, int remain, int durability)
	{
		if (inventory.ContainsKey(seq))
		{
			inventory[seq].Buy(remain, Item.USAGE.UNEQUIP, durability);
		}
	}

	public void OpenRandomBoxItem(long seq, string code, int remain)
	{
		TItem tItem = TItemManager.Instance.Get<TItem>(code);
		if (tItem == null)
		{
			Debug.LogError("Fail to get item template for " + code);
		}
		else if (inventory.ContainsKey(seq))
		{
			inventory.Add(seq, new Item(seq, tItem, code, Item.USAGE.NOT_USING, remain, 0, -1));
		}
	}

	public void ReceivePrize(long seq, string code, Item.USAGE usage, int remain, int durability)
	{
		TItem tItem = TItemManager.Instance.Get<TItem>(code);
		if (tItem == null)
		{
			Debug.LogError("Fail to get item template for " + code);
		}
		else if (inventory.ContainsKey(seq))
		{
			inventory[seq].Buy(remain, usage, -1);
		}
		else
		{
			inventory.Add(seq, new Item(seq, tItem, code, usage, remain, 0, durability));
		}
	}

	public void ReceivePresentItem(long seq, string code, int remain, int durability)
	{
		TItem tItem = TItemManager.Instance.Get<TItem>(code);
		if (tItem == null)
		{
			Debug.LogError("Fail to get item template for " + code);
		}
		else
		{
			Item.USAGE uSAGE = (!tItem.IsAmount && remain >= 0) ? Item.USAGE.NOT_USING : Item.USAGE.UNEQUIP;
			if (inventory.ContainsKey(seq))
			{
				inventory[seq].Buy(remain, uSAGE, durability);
			}
			else
			{
				inventory.Add(seq, new Item(seq, tItem, code, uSAGE, remain, 0, durability));
			}
		}
	}

	public void BuyItem(long seq, string code, int remain, sbyte premium, int durability)
	{
		TItem tItem = TItemManager.Instance.Get<TItem>(code);
		if (tItem == null)
		{
			Debug.LogError("Fail to get item template for " + code);
		}
		else
		{
			Item.USAGE uSAGE = (!tItem.IsAmount && tItem.catType != 0 && remain >= 0) ? Item.USAGE.NOT_USING : Item.USAGE.UNEQUIP;
			if (inventory.ContainsKey(seq))
			{
				inventory[seq].Buy(remain, uSAGE, durability);
			}
			else
			{
				inventory.Add(seq, new Item(seq, tItem, code, uSAGE, remain, premium, durability));
			}
		}
	}

	public void SetItemUsage(long seq, string code, Item.USAGE usage)
	{
		if (inventory.ContainsKey(seq) && inventory[seq].Usage != Item.USAGE.DELETED)
		{
			inventory[seq].Usage = usage;
		}
	}

	public bool HaveWeaponLimitedByStarRate()
	{
		foreach (KeyValuePair<long, Item> item in inventory)
		{
			if (item.Value.Usage == Item.USAGE.EQUIP && item.Value.Template.type == TItem.TYPE.WEAPON && item.Value.IsLimitedByStarRate)
			{
				return true;
			}
		}
		return false;
	}

	public string[] GetUsings()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<long, Item> item in inventory)
		{
			if (item.Value.Usage == Item.USAGE.EQUIP)
			{
				list.Add(item.Value.Code);
			}
		}
		return list.ToArray();
	}

	public Item[] GetDeletedItems()
	{
		List<Item> list = new List<Item>();
		foreach (KeyValuePair<long, Item> item in inventory)
		{
			if (item.Value.Usage == Item.USAGE.DELETED)
			{
				list.Add(item.Value);
			}
		}
		return list.ToArray();
	}

	public Item[] GetUsingItems()
	{
		List<Item> list = new List<Item>();
		foreach (KeyValuePair<long, Item> item in inventory)
		{
			if (item.Value.Usage == Item.USAGE.EQUIP)
			{
				list.Add(item.Value);
			}
		}
		return list.ToArray();
	}

	public void VerifyMustEquipSlots()
	{
		for (TItem.SLOT sLOT = TItem.SLOT.UPPER; sLOT <= TItem.SLOT.BOMB; sLOT++)
		{
			Item usingItemBySlot = GetUsingItemBySlot(sLOT);
			if (usingItemBySlot == null)
			{
				usingItemBySlot = GetAnyNotUsingUnlimitBySlot(sLOT);
				if (usingItemBySlot != null)
				{
					CSNetManager.Instance.Sock.SendCS_EQUIP_REQ(usingItemBySlot.Seq);
				}
			}
		}
		long num = -1L;
		bool flag = false;
		foreach (KeyValuePair<long, Item> item in inventory)
		{
			if (item.Value.Template.code == "s07")
			{
				num = item.Value.Seq;
			}
			if ((item.Value.Template.code == "s07" || item.Value.Template.code == "s08" || item.Value.Template.code == "s09" || item.Value.Template.code == "s92") && item.Value.Usage == Item.USAGE.EQUIP)
			{
				flag = true;
			}
		}
		if (!flag && num >= 0)
		{
			CSNetManager.Instance.Sock.SendCS_EQUIP_REQ(num);
		}
	}

	public void EquipDefaultItems()
	{
		charCode = string.Empty;
		for (TItem.SLOT sLOT = TItem.SLOT.UPPER; sLOT <= TItem.SLOT.BOMB; sLOT++)
		{
			Item anyNotUsingUnlimitBySlot = GetAnyNotUsingUnlimitBySlot(sLOT);
			if (anyNotUsingUnlimitBySlot != null)
			{
				CSNetManager.Instance.Sock.SendCS_EQUIP_REQ(anyNotUsingUnlimitBySlot.Seq);
			}
		}
	}

	private Item GetAnyNotUsingUnlimitBySlot(TItem.SLOT slot)
	{
		foreach (KeyValuePair<long, Item> item in inventory)
		{
			if (item.Value.Template.slot == slot && !item.Value.IsPremium && item.Value.Template.IsBasic && item.Value.Amount < 0 && item.Value.Remain < 0 && item.Value.Usage == Item.USAGE.UNEQUIP)
			{
				return item.Value;
			}
		}
		return null;
	}

	public Item GetUsingItemBySlot(TItem.SLOT slot)
	{
		foreach (KeyValuePair<long, Item> item in inventory)
		{
			TItem tItem = TItemManager.Instance.Get<TItem>(item.Value.Code);
			if (tItem != null && item.Value.Usage == Item.USAGE.EQUIP && tItem.slot == slot)
			{
				return item.Value;
			}
		}
		return null;
	}

	public string GetUsingBySlot(TItem.SLOT slot)
	{
		foreach (KeyValuePair<long, Item> item in inventory)
		{
			TItem tItem = TItemManager.Instance.Get<TItem>(item.Value.Code);
			if (tItem != null && item.Value.Usage == Item.USAGE.EQUIP && tItem.slot == slot)
			{
				return item.Value.Code;
			}
		}
		return string.Empty;
	}

	public bool WeaponChange(long weaponSeq)
	{
		if (!inventory.ContainsKey(weaponSeq))
		{
			return false;
		}
		Item item = inventory[weaponSeq];
		if (item.Template.type != 0)
		{
			return false;
		}
		TWeapon tWeapon = (TWeapon)item.Template;
		int[] array = new int[6]
		{
			-1,
			-1,
			2,
			1,
			0,
			3
		};
		int num = array[(int)tWeapon.slot];
		wpnChg[num] = weaponSeq;
		return true;
	}

	public bool DropWeaponChange(long weaponSeq)
	{
		if (!inventory.ContainsKey(weaponSeq))
		{
			return false;
		}
		Item item = inventory[weaponSeq];
		if (item.Template.type != 0)
		{
			return false;
		}
		TWeapon tWeapon = (TWeapon)item.Template;
		int[] array = new int[6]
		{
			-1,
			-1,
			2,
			1,
			0,
			3
		};
		int num = array[(int)tWeapon.slot];
		drpItm[num] = weaponSeq;
		return true;
	}

	public Item GetCurrentWeaponBySlot(TItem.SLOT slot)
	{
		for (int i = 0; i < wpnChg.Length; i++)
		{
			if (inventory.ContainsKey(wpnChg[i]))
			{
				TItem template = inventory[wpnChg[i]].Template;
				if (template.slot == slot && template.type == TItem.TYPE.WEAPON)
				{
					return inventory[wpnChg[i]];
				}
			}
		}
		foreach (KeyValuePair<long, Item> item in inventory)
		{
			TItem template2 = item.Value.Template;
			if (template2 != null && item.Value.Usage == Item.USAGE.EQUIP && template2.slot == slot && template2.type == TItem.TYPE.WEAPON)
			{
				return item.Value;
			}
		}
		return null;
	}

	public float SumFunctionFactor(string func)
	{
		func = func.ToLower();
		int num = TItem.String2FunctionMask(func);
		if (num < 0)
		{
			return 0f;
		}
		float num2 = 0f;
		foreach (KeyValuePair<long, Item> item in inventory)
		{
			TItem tItem = TItemManager.Instance.Get<TItem>(item.Value.Code);
			if (tItem != null && !item.Value.IsAmount && item.Value.Usage == Item.USAGE.EQUIP)
			{
				if (tItem.type == TItem.TYPE.ACCESSORY)
				{
					TAccessory tAccessory = (TAccessory)tItem;
					if (tAccessory.functionMask == num)
					{
						num2 += tAccessory.functionFactor;
					}
				}
				else if (tItem.type == TItem.TYPE.CLOTH)
				{
					TCostume tCostume = (TCostume)tItem;
					if (tCostume.functionMask == num)
					{
						num2 += tCostume.functionFactor;
					}
				}
			}
		}
		return num2;
	}

	public int SumArmor()
	{
		int num = 0;
		foreach (KeyValuePair<long, Item> item in inventory)
		{
			TItem tItem = TItemManager.Instance.Get<TItem>(item.Value.Code);
			if (tItem != null && !item.Value.IsAmount && item.Value.Usage == Item.USAGE.EQUIP)
			{
				if (tItem.type == TItem.TYPE.ACCESSORY)
				{
					TAccessory tAccessory = (TAccessory)tItem;
					tAccessory.safeArmor();
					num += tAccessory.armor;
				}
				else if (tItem.type == TItem.TYPE.CLOTH)
				{
					TCostume tCostume = (TCostume)tItem;
					tCostume.safeArmor();
					num += tCostume.armor;
				}
			}
		}
		foreach (KeyValuePair<long, Item> item2 in inventory)
		{
			if (item2.Value.Usage == Item.USAGE.EQUIP)
			{
				int num2 = 0;
				int num3 = 0;
				num2 = 11;
				num3 = item2.Value.upgradeProps[num2].grade;
				if (num3 > 0 && item2.Value.Template.upgradeCategory >= TItem.UPGRADE_CATEGORY.HEAVY)
				{
					num += (int)PimpManager.Instance.getValue((int)item2.Value.Template.upgradeCategory, num2, num3 - 1);
				}
			}
		}
		return num;
	}

	public long HaveFunction(string[] func)
	{
		for (int i = 0; i < func.Length; i++)
		{
			long num = HaveFunction(func[i]);
			if (num >= 0)
			{
				return num;
			}
		}
		return -1L;
	}

	public long HaveFunction(string func)
	{
		func = func.ToLower();
		int num = TItem.String2FunctionMask(func);
		if (num < 0)
		{
			return -1L;
		}
		foreach (KeyValuePair<long, Item> item in inventory)
		{
			TItem tItem = TItemManager.Instance.Get<TItem>(item.Value.Code);
			if (tItem != null)
			{
				if (tItem.type == TItem.TYPE.SPECIAL)
				{
					TSpecial tSpecial = (TSpecial)tItem;
					if (tSpecial.functionMask == num)
					{
						if (item.Value.IsAmount)
						{
							if (item.Value.EnoughToConsume)
							{
								return item.Value.Seq;
							}
						}
						else if (item.Value.Usage == Item.USAGE.EQUIP || item.Value.Usage == Item.USAGE.UNEQUIP)
						{
							return item.Value.Seq;
						}
					}
				}
				else if (tItem.type == TItem.TYPE.ACCESSORY)
				{
					TAccessory tAccessory = (TAccessory)tItem;
					if (tAccessory.functionMask == num && item.Value.Usage == Item.USAGE.EQUIP)
					{
						return item.Value.Seq;
					}
				}
				else if (tItem.type == TItem.TYPE.CLOTH)
				{
					TCostume tCostume = (TCostume)tItem;
					if (tCostume.functionMask == num && item.Value.Usage == Item.USAGE.EQUIP)
					{
						return item.Value.Seq;
					}
				}
			}
		}
		return -1L;
	}

	public float HaveFunctionTotalFactor(string func)
	{
		func = func.ToLower();
		int num = TItem.String2FunctionMask(func);
		if (num < 0)
		{
			return -1f;
		}
		float num2 = 0f;
		foreach (KeyValuePair<long, Item> item in inventory)
		{
			if (item.Value.Usage == Item.USAGE.EQUIP)
			{
				TItem tItem = TItemManager.Instance.Get<TItem>(item.Value.Code);
				if (tItem != null)
				{
					if (tItem.type == TItem.TYPE.ACCESSORY)
					{
						TAccessory tAccessory = (TAccessory)tItem;
						if (tAccessory.functionMask == num)
						{
							num2 += tAccessory.functionFactor;
						}
					}
					else if (tItem.type == TItem.TYPE.CLOTH)
					{
						TCostume tCostume = (TCostume)tItem;
						if (tCostume.functionMask == num)
						{
							num2 += tCostume.functionFactor;
						}
					}
				}
			}
		}
		return num2;
	}

	public Texture2D HaveFunctionTex(string func)
	{
		func = func.ToLower();
		int num = TItem.String2FunctionMask(func);
		if (num < 0)
		{
			return null;
		}
		foreach (KeyValuePair<long, Item> item in inventory)
		{
			if (item.Value.Usage == Item.USAGE.EQUIP)
			{
				TItem tItem = TItemManager.Instance.Get<TItem>(item.Value.Code);
				if (tItem != null)
				{
					if (tItem.type == TItem.TYPE.ACCESSORY)
					{
						TAccessory tAccessory = (TAccessory)tItem;
						if (tAccessory.functionMask == num)
						{
							return tAccessory.funcIcon;
						}
					}
					else if (tItem.type == TItem.TYPE.CLOTH)
					{
						TCostume tCostume = (TCostume)tItem;
						if (tCostume.functionMask == num)
						{
							return tCostume.funcIcon;
						}
					}
				}
			}
		}
		return null;
	}

	public Item GetItemBySequence(long seq)
	{
		if (inventory.ContainsKey(seq))
		{
			return inventory[seq];
		}
		return null;
	}

	public Item IsUsebleUpgrader(int grade, int uptype)
	{
		foreach (KeyValuePair<long, Item> item in inventory)
		{
			TItem tItem = TItemManager.Instance.Get<TItem>(item.Value.Code);
			if (tItem != null && tItem.type == TItem.TYPE.UPGRADE)
			{
				TUpgrade tUpgrade = (TUpgrade)tItem;
				if (tUpgrade != null && tUpgrade.targetType == uptype && grade >= tUpgrade.reqLv && grade < tUpgrade.maxLv)
				{
					return item.Value;
				}
			}
		}
		return null;
	}

	public Item IsExistItem(string code)
	{
		foreach (KeyValuePair<long, Item> item in inventory)
		{
			if (item.Value.Code == code)
			{
				return item.Value;
			}
		}
		return null;
	}

	public Item[] GetItemsByCat(int filter, int catType, int catKind, int category = -1)
	{
		List<Item> list = new List<Item>();
		foreach (KeyValuePair<long, Item> item in inventory)
		{
			TItem tItem = TItemManager.Instance.Get<TItem>(item.Value.Code);
			if (tItem != null && item.Value.IsFiltered(filter))
			{
				if (catType == 0)
				{
					if (!item.Value.IsPremiumOrPCBang || item.Value.Usage != Item.USAGE.DELETED)
					{
						list.Add(item.Value);
					}
				}
				else if (tItem.catType == catType && (catKind <= 0 || tItem.catKind == catKind) && (!item.Value.IsPremiumOrPCBang || item.Value.Usage != Item.USAGE.DELETED))
				{
					if (category >= 0)
					{
						TWeapon tWeapon = (TWeapon)tItem;
						if (tWeapon.cat == category)
						{
							list.Add(item.Value);
						}
					}
					else
					{
						list.Add(item.Value);
					}
				}
			}
		}
		list.Sort((Item prev, Item next) => prev.Compare(next));
		return list.ToArray();
	}

	public Item[] GetPCBangPremiumItems(int mainTab)
	{
		switch (mainTab)
		{
		case 10:
			if (BuildOption.Instance.Props.usePremiumItem)
			{
				return GetPremiumItems();
			}
			if (BuildOption.Instance.Props.usePCBangItem)
			{
				return GetPcbangItems();
			}
			break;
		case 11:
			return GetPcbangItems();
		}
		return null;
	}

	public Item[] GetPremiumItems()
	{
		if (HaveFunction("premium_account") >= 0)
		{
			List<Item> list = new List<Item>();
			foreach (KeyValuePair<long, Item> item in inventory)
			{
				if (item.Value.IsPremium)
				{
					list.Add(item.Value);
				}
			}
			return list.ToArray();
		}
		return PremiumItemManager.Instance.GetPremiumItems();
	}

	public Item[] GetPcbangItems()
	{
		if (BuffManager.Instance.IsPCBangBuff())
		{
			List<Item> list = new List<Item>();
			foreach (KeyValuePair<long, Item> item in inventory)
			{
				if (item.Value.IsPCBang)
				{
					list.Add(item.Value);
				}
			}
			return list.ToArray();
		}
		return PremiumItemManager.Instance.GetPcbangItems();
	}

	public NameCard[] GetClanees(bool connectedOnly)
	{
		List<NameCard> list = new List<NameCard>();
		foreach (KeyValuePair<int, NameCard> item in clanee)
		{
			if (!connectedOnly || item.Value.IsConnected)
			{
				list.Add(item.Value);
			}
		}
		return list.ToArray();
	}

	public NameCard[] GetClaneesIncludeMe(bool connectedOnly)
	{
		List<NameCard> list = new List<NameCard>();
		int level = XpManager.Instance.GetLevel(Instance.Xp);
		NameCard item = new NameCard(Instance.Seq, Instance.Nickname, level, -1, Instance.Rank);
		list.Add(item);
		foreach (KeyValuePair<int, NameCard> item2 in clanee)
		{
			if (!connectedOnly || item2.Value.IsConnected)
			{
				list.Add(item2.Value);
			}
		}
		return list.ToArray();
	}

	public NameCard[] GetFriends(bool connectedOnly)
	{
		List<NameCard> list = new List<NameCard>();
		foreach (KeyValuePair<int, NameCard> friend in friends)
		{
			if (!connectedOnly || friend.Value.IsConnected)
			{
				list.Add(friend.Value);
			}
		}
		return list.ToArray();
	}

	public NameCard[] GetBans()
	{
		List<NameCard> list = new List<NameCard>();
		foreach (KeyValuePair<int, NameCard> ban in bans)
		{
			list.Add(ban.Value);
		}
		return list.ToArray();
	}

	public bool IsBan(int seq)
	{
		return bans.ContainsKey(seq);
	}

	public NameCard GetBan(int seq)
	{
		if (bans.ContainsKey(seq))
		{
			return bans[seq];
		}
		return null;
	}

	public bool IsBan(string speaker)
	{
		NameCard[] array = GetBans();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Nickname == speaker)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsFriend(int seq)
	{
		return friends.ContainsKey(seq);
	}

	public bool IsClanee(int seq)
	{
		return clanee.ContainsKey(seq);
	}

	public void AddClanee(int seq, string nickname, int lv, int svrId, int rank)
	{
		if (seq != Instance.Seq && Instance.IsClanMember)
		{
			if (!clanee.ContainsKey(seq))
			{
				clanee.Add(seq, new NameCard(seq, nickname, lv, svrId, rank));
			}
			else
			{
				clanee[seq].Lv = lv;
				clanee[seq].SvrId = svrId;
				clanee[seq].Rank = rank;
			}
		}
	}

	public void DelClanee(int seq)
	{
		if (clanee.ContainsKey(seq))
		{
			clanee.Remove(seq);
		}
	}

	public NameCard GetClanee(int seq)
	{
		if (clanee.ContainsKey(seq))
		{
			return clanee[seq];
		}
		return null;
	}

	public void ClearClanee()
	{
		clanee.Clear();
	}

	public void AddFriend(int seq, string nickname, int lv, int svrId, int rank)
	{
		if (!friends.ContainsKey(seq))
		{
			friends.Add(seq, new NameCard(seq, nickname, lv, svrId, rank));
		}
		else
		{
			friends[seq].Lv = lv;
			friends[seq].SvrId = svrId;
			friends[seq].Rank = rank;
		}
	}

	public void DelFriend(int seq)
	{
		if (friends.ContainsKey(seq))
		{
			friends.Remove(seq);
		}
	}

	public NameCard GetFriend(int seq)
	{
		if (friends.ContainsKey(seq))
		{
			return friends[seq];
		}
		return null;
	}

	public void AddBan(int seq, string nickname)
	{
		if (!bans.ContainsKey(seq))
		{
			bans.Add(seq, new NameCard(seq, nickname, -1, -1, -1));
		}
	}

	public void DelBan(int seq)
	{
		if (bans.ContainsKey(seq))
		{
			bans.Remove(seq);
		}
	}

	public void RoundEnd(int respawnTicket)
	{
		ticket = respawnTicket;
		isRounding = true;
	}

	public void ClanMatchHalfTime(int respawnTicket)
	{
		ticket = respawnTicket;
		isRounding = true;
		clanMatchRounding++;
		ClanMatchExplosionChangeMessage();
	}

	public void MatchRestarted()
	{
		isRounding = false;
	}

	public Brick.SPAWNER_TYPE GetTeamSpawnerType()
	{
		if (clanMatchRounding % 2 == 0)
		{
			return (slot < 8) ? Brick.SPAWNER_TYPE.RED_TEAM_SPAWNER : Brick.SPAWNER_TYPE.BLUE_TEAM_SPAWNER;
		}
		return (slot >= 8) ? Brick.SPAWNER_TYPE.RED_TEAM_SPAWNER : Brick.SPAWNER_TYPE.BLUE_TEAM_SPAWNER;
	}

	public Brick.SPAWNER_TYPE GetRoundingSpawnerType()
	{
		if (RoomManager.Instance.IsKindOfIndividual)
		{
			return Brick.SPAWNER_TYPE.SINGLE_SPAWNER;
		}
		return Instance.GetTeamSpawnerType();
	}

	public bool AmIBlasting()
	{
		bool flag = false;
		if (clanMatchRounding % 2 == 0)
		{
			return slot < 8;
		}
		return slot >= 8;
	}

	public bool CheckControllable()
	{
		if (Application.loadedLevelName.Contains("Tutor"))
		{
			return true;
		}
		Room room = RoomManager.Instance.GetRoom(RoomManager.Instance.CurrentRoom);
		if (EscapeGoalTrigger.IsSendGoal())
		{
			return false;
		}
		return room != null && room.Status == Room.ROOM_STATUS.PLAYING && !isRounding && status == 4 && !IsSpectator;
	}

	public Item GetUsingEquipByCode(string code)
	{
		foreach (KeyValuePair<long, Item> item in inventory)
		{
			if (code == item.Value.Code && item.Value.Usage == Item.USAGE.EQUIP)
			{
				return item.Value;
			}
		}
		return null;
	}

	public Item[] GetItemsCanMerge(Item source)
	{
		List<Item> list = new List<Item>();
		foreach (KeyValuePair<long, Item> item in inventory)
		{
			if (CanCompose(item.Value, source))
			{
				list.Add(item.Value);
			}
		}
		list.Sort((Item prev, Item next) => prev.Compare(next));
		return list.ToArray();
	}

	private static bool CanCompose(Item myItem, Item srcItem)
	{
		if (myItem.Seq == srcItem.Seq)
		{
			return false;
		}
		if (myItem.Template == null || myItem.Template.code != srcItem.Template.code || srcItem.Template.IsAmount || myItem.Usage != Item.USAGE.NOT_USING)
		{
			return false;
		}
		return true;
	}

	public int UseGmFunction()
	{
		if (IsInvisibilityOn)
		{
			return 1;
		}
		if (IsGhostOn)
		{
			return 1;
		}
		if (ControlMode == CONTROL_MODE.FLY_MODE || ControlMode == CONTROL_MODE.SPECTATOR_MODE)
		{
			return 1;
		}
		if (GodMode)
		{
			return 1;
		}
		if (IsStraightMovement)
		{
			return 1;
		}
		return 0;
	}

	public int ClearGmFunction()
	{
		IsInvisibilityOn = false;
		IsGhostOn = false;
		ControlMode = CONTROL_MODE.PLAY_MODE;
		GodMode = false;
		IsGuiOn = true;
		IsStraightMovement = false;
		BrickManManager.Instance.ClearAllInvisibility();
		return 0;
	}

	public bool GetCommonMask(COMMON_OPT mask)
	{
		return (qjCommonMask & (int)mask) != 0;
	}

	public void SaveDonotCommonMask(COMMON_OPT mask)
	{
		SetCommonMask(mask);
		SaveCommonMaskServer();
	}

	public void SetCommonMask(COMMON_OPT mask)
	{
		qjCommonMask |= (int)mask;
	}

	public void SaveCommonMaskServer()
	{
		CSNetManager.Instance.Sock.SendCS_SAVE_PLAYER_COMMON_OPT_REQ(qjCommonMask);
	}

	public void RemoveCommonMask(COMMON_OPT mask)
	{
		qjCommonMask &= (int)(~mask);
	}

	public void BungeeFlyModeByItem(bool flyMode)
	{
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BUNGEE)
		{
			if (flyMode)
			{
				controlMode = CONTROL_MODE.PLAYING_FLY_MODE;
			}
			else
			{
				controlMode = CONTROL_MODE.NONE;
			}
		}
	}

	public void ClanMatchExplosionChangeMessage()
	{
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.EXPLOSION)
		{
			bool flag = false;
			if ((clanMatchRounding % 2 != 0) ? ((slot >= 8) ? true : false) : ((slot < 8) ? true : false))
			{
				SystemMsgManager.Instance.ShowMessage(string.Format(StringMgr.Instance.Get("GUIDE_EXPLOSION_ATTACK01"), custom_inputs.Instance.GetKeyCodeName("K_MODE"), custom_inputs.Instance.GetKeyCodeName("K_ACTION")), 6f);
			}
			else
			{
				SystemMsgManager.Instance.ShowMessage(string.Format(StringMgr.Instance.Get("GUIDE_EXPLOSION_DEFENCE01"), custom_inputs.Instance.GetKeyCodeName("K_ACTION")), 6f);
			}
		}
	}

	public bool IsPremiumAccount()
	{
		return HaveFunction("premium_account") >= 0;
	}

	public bool IsNotPlaying()
	{
		return status != 4;
	}

	public bool IsRedTeam()
	{
		switch (RoomManager.Instance.CurrentRoomType)
		{
		case Room.ROOM_TYPE.MAP_EDITOR:
			return false;
		case Room.ROOM_TYPE.TEAM_MATCH:
			return slot < 8;
		case Room.ROOM_TYPE.INDIVIDUAL:
			return false;
		case Room.ROOM_TYPE.CAPTURE_THE_FLAG:
			return slot < 8;
		case Room.ROOM_TYPE.EXPLOSION:
			return slot < 8;
		case Room.ROOM_TYPE.MISSION:
			return slot < 4;
		case Room.ROOM_TYPE.BND:
			return slot < 8;
		case Room.ROOM_TYPE.BUNGEE:
			return false;
		case Room.ROOM_TYPE.ESCAPE:
			return false;
		case Room.ROOM_TYPE.ZOMBIE:
			return false;
		default:
			return false;
		}
	}
}
