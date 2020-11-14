using System;
using UnityEngine;

[Serializable]
public class Property
{
	public enum XTRAP_TARGET
	{
		NOT_USE,
		NETMARBLE,
		INFERNUM,
		WAVE,
		AXESO5,
		TEST_SERVER
	}

	public string Name;

	public string Alias;

	public string GetTokensURL;

	public string GetTokensURL2;

	public Token.TYPE TokenType;

	public string PswdRequestURL;

	public string RegisterURL;

	public string ExceptionURL;

	public bool ReadServerFromPreference;

	public string RoundRobinServer;

	public string ResourceServer;

	public XTRAP_TARGET UseXTrapTarget;

	public LangOptManager.LANG_OPT DefaultLanguage;

	public LangOptManager.LANG_OPT[] supportLanguages;

	public bool ShowGrb;

	public bool UseP2pHolePunching = true;

	public bool loadShopTxt;

	public bool kickSvrSpdHck;

	public bool useUskWeaponTex;

	public bool useUskWeaponIcon;

	public bool useUskDecal;

	public bool useUskMuzzleEff;

	public BuildOption.RANDOM_BOX_TYPE randomBox;

	public bool allowBuildGunInDestroyPhase;

	public bool ShowAgb;

	public bool refreshRoomsManually = true;

	public bool tutorialAlways = true;

	public int maxNewbieLevel = 2;

	public bool fullScreenMode;

	public BuildOption.SEASON season = BuildOption.SEASON.BRICK_STAR;

	public Texture2D logo;

	public Texture2D publisher;

	public string movieLogo;

	public string DefaultVoice;

	public int DefaultVoiceVer = 1;

	public string DefaultUskFile = "usk.unity3d";

	public string[] copyRights;

	public BuildOption.COUNTRY_FILTER[] filters;

	public string auxRoundRobinServer;

	public BuildOption.COUNTRY_FILTER[] auxFilters;

	public float SendRate = 0.06f;

	public string[] LangVoices = new string[7]
	{
		"voice_kr_2",
		"voice_en_1",
		"voice_ge_1",
		"voice_fr_1",
		"voice_pl_1",
		"voice_sp_1",
		"voice_tk_1"
	};

	public string[] LangVoiceNames = new string[7]
	{
		"KOREAN",
		"ENGLISH",
		"GERMAN",
		"FRENCH",
		"POLISH",
		"SPANISH",
		"TURKISH"
	};

	public int[] LangVoiceVers = new int[7]
	{
		1,
		1,
		1,
		1,
		1,
		1,
		1
	};

	private bool[] supportMode;

	private int supportModeCount;

	public bool brickSoundChange;

	public bool ApplyItemUpgrade = true;

	public bool useArmor = true;

	public bool useDefaultDash;

	public bool useDefaultAutoReload;

	public bool useDurability = true;

	public float defaultCameraSpeedFactor = 1f;

	public bool useBrickPoint = true;

	public bool isluncherExecuteOnly;

	public bool isDuplicateExcuteAble = true;

	public bool teamMatchMode = true;

	public bool individualMatchMode = true;

	public bool ctfMatchMode = true;

	public bool explosionMatchMode = true;

	public bool defenseMatchMode = true;

	public bool bndMatchMode = true;

	public bool bungeeMode = true;

	public bool escapeMode;

	public bool zombieMode;

	public bool clanMode = true;

	public bool itemBuyLimit;

	public bool canForcePointBuy;

	public bool useLevelupCompensation;

	public bool useLevelupCompensationYouCanBuyItem = true;

	public bool useCountryFilterInSettingDlg = true;

	public bool usePriceDiscount = true;

	public bool packetSaving;

	public int MyAge = 100;

	public bool usePremiumItem = true;

	public bool usePCBangItem;

	public bool usePointGiftAble;

	public bool UseSteam;

	public bool UseItemDrop = true;

	public bool UseEscapeAttack;

	public bool UseAccuse = true;

	public bool UseAccuseToolButton = true;

	public bool UseWanted;

	public WantedOpt wantedOpt;

	public float goalRatioForChannel = 0.25f;

	public BuildOption.XP_MODE xpMode;

	public string GetRoundRobinServer
	{
		get
		{
			if (ReadServerFromPreference)
			{
				RoundRobinServer = PlayerPrefs.GetString("TargetServer", RoundRobinServer);
			}
			return RoundRobinServer;
		}
	}

	public string GetResourceServer
	{
		get
		{
			if (ReadServerFromPreference)
			{
				ResourceServer = PlayerPrefs.GetString("ResourceServer", ResourceServer);
			}
			return ResourceServer;
		}
	}

	public bool UseXTrapNetmarble => UseXTrapTarget == XTRAP_TARGET.NETMARBLE;

	public bool UseXTrapInfernum => UseXTrapTarget == XTRAP_TARGET.INFERNUM;

	public bool UseXTrapWave => UseXTrapTarget == XTRAP_TARGET.WAVE;

	public bool UseXTrapAxeso5 => UseXTrapTarget == XTRAP_TARGET.AXESO5;

	public bool UseXTrapTest => UseXTrapTarget == XTRAP_TARGET.TEST_SERVER;

	public bool UseXTrap => UseXTrapTarget != XTRAP_TARGET.NOT_USE;

	public bool LanguageSelectable => supportLanguages.Length > 0;

	public bool isWebPlayer => false;

	public BuildOption.COUNTRY_FILTER[] Filters
	{
		get
		{
			if (auxRoundRobinServer.Length <= 0 || RoundRobinServer != auxRoundRobinServer)
			{
				return filters;
			}
			return auxFilters;
		}
	}

	public int SupportModeCount => supportModeCount;

	public string GetLangVoc(int id)
	{
		if (id >= LangVoices.Length)
		{
			PlayerPrefs.SetInt("BfVoice", 0);
			return LangVoices[0];
		}
		return LangVoices[id];
	}

	public string GetLangVoiceName(int id)
	{
		if (id >= LangVoiceNames.Length)
		{
			PlayerPrefs.SetInt("BfVoice", 0);
			return LangVoiceNames[0];
		}
		return LangVoiceNames[id];
	}

	public int GetLangVoiceVer(int id)
	{
		if (id >= LangVoiceVers.Length)
		{
			PlayerPrefs.SetInt("BfVoice", 0);
			return LangVoiceVers[0];
		}
		return LangVoiceVers[id];
	}

	public int GetVocID(string name)
	{
		for (int i = 0; i < LangVoiceNames.Length; i++)
		{
			if (LangVoiceNames[i] == name)
			{
				return i;
			}
		}
		return -1;
	}

	public void Awake()
	{
		supportMode = new bool[10]
		{
			true,
			teamMatchMode,
			individualMatchMode,
			ctfMatchMode,
			explosionMatchMode,
			defenseMatchMode,
			bndMatchMode,
			bungeeMode,
			escapeMode,
			zombieMode
		};
		supportModeCount = 0;
		for (int i = 0; i < supportMode.Length; i++)
		{
			if (supportMode[i])
			{
				supportModeCount++;
			}
		}
	}

	public bool IsSupportMode(Room.ROOM_TYPE t)
	{
		if (t == Room.ROOM_TYPE.NONE)
		{
			return false;
		}
		if (t < Room.ROOM_TYPE.MAP_EDITOR || (int)t >= supportMode.Length)
		{
			Debug.LogError("ROOM_TYPE is out of range for supportMode" + t.ToString());
			return false;
		}
		return supportMode[(int)t];
	}

	public int GetSeasonCount()
	{
		return (int)season;
	}
}
