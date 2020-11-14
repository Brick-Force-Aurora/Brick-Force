using UnityEngine;

public class TItem
{
	public enum TYPE
	{
		WEAPON,
		CLOTH,
		ACCESSORY,
		CHARACTER,
		SPECIAL,
		UPGRADE,
		BUNDLE
	}

	public enum UPGRADE_CATEGORY
	{
		NONE = -1,
		HEAVY,
		ASSAULT,
		SNIPER,
		SUB_MACHINE,
		HAND_GUN,
		MELEE,
		GRENADE,
		FLASH_BANG,
		SMOKE,
		UPPER_LOWER,
		HELMET,
		OTHER,
		SHOTGUN,
		MAX
	}

	public enum CAT_TYPE
	{
		PREMIUM,
		WEAPON,
		CLOTH,
		ACCESSORY,
		CHARACTER,
		ACTION,
		TOOLBOX,
		UPGRADE,
		BUNDLE,
		ETC
	}

	public enum SLOT
	{
		UPPER = 0,
		LOWER = 1,
		MELEE = 2,
		AUX = 3,
		MAIN = 4,
		BOMB = 5,
		HEAD = 6,
		FACE = 7,
		BACK = 8,
		LEG = 9,
		SASH1 = 10,
		SASH2 = 11,
		SASH3 = 12,
		LAUNCHER = 13,
		MAGAZINE_L = 14,
		MAGAZINE_R = 0xF,
		KIT = 0x10,
		CHARACTER = 17,
		NUM = 18,
		NONE = -1
	}

	public enum FUNCTION_MASK
	{
		CLAN_MARK,
		FLY,
		FLY_FAST,
		LINE_TOOL,
		REPLACE_TOOL,
		AUTO_RELOAD,
		HEAL_PACK,
		ASSAULT_AMMO,
		SPEEDUP,
		HEARTBEAT_RADAR,
		GRENADE_AMMO,
		PISTOL_AMMO,
		HEAVY_AMMO,
		SNIPER_AMMO,
		SUBMACHINE_AMMO,
		RESPAWN,
		AUTO_HEAL,
		PREMIUM_ACCOUNT,
		HEAL_PACK50,
		HEAL_PACK30,
		HP_COOLTIME,
		MAIN_AMMO_INC,
		AUX_AMMO_INC,
		SPECIAL_AMMO_INC,
		CHARGE_COIN,
		ADD_MAP_SLOT,
		RESET_MAP_SLOT,
		BRICKSTAR_BUILDER,
		CIRCLE_WINDOW_TCKT,
		COMPUTER_BOX01_TCKT,
		COMPUTER_BOX02_TCKT,
		COMPUTER_BOX03_TCKT,
		CRATER_TCKT,
		FLAMMABLE_DRUM_TCKT,
		FUNCTION_SET_TCKT,
		GRAVITY_BLUEA_TCKT,
		GRAVITY_BLUEB_TCKT,
		GRAVITY_REDA_TCKT,
		GRAVITY_REDB_TCKT,
		HATCH_TCKT,
		LASERWALL_TCKT,
		METALCABINET_TCKT,
		METALLADDER_TCKT,
		SCIFI_SET_TCKT,
		SOLAR_COLLECTOR_TCKT,
		GROUND01_TCKT,
		GROUND02_TCKT,
		METAL1_TCKT,
		METAL2_TCKT,
		METAL3_TCKT,
		METAL4_TCKT,
		TOXIC_DRUM_TCKT,
		TRAMPOLINE_HOR_TCKT,
		TRAMPOLINE_VER_TCKT,
		VALVE_TCKT,
		SPEED_TCKT,
		EXPLOSION_TCKT,
		GRAVITY_TCKT,
		PLANK_TCKT,
		WOODBARREL_TCKT,
		CANDLE_TCKT,
		FENCE_TCKT,
		BENCH_TCKT,
		ARMOR_TCKT,
		TORCH_TCKT,
		FLAGBLUE_TCKT,
		FLAGRED_TCKT,
		STAINED_TCKT,
		WINDOW_TCKT,
		TRAP_TCKT,
		DOOR_TCKT,
		FREE_PROTAL_TCKT,
		WOOD_SET_TCKT,
		FIRE_SET_TCKT,
		FLAG_SET_TCKT,
		WINDOW_SET_TCKT,
		TEAM_PORTAL_TCKT,
		PORTAL_SET_TCKT,
		NORMAL3_SET_TCKT,
		FUNCTION3_SET_TCKT,
		CHARGE_FP,
		CONSUMABLE_XP_BONUS,
		CONSUMABLE_FP_BONUS,
		CONSUMABLE_XP_BONUS2,
		CONSUMABLE_FP_BONUS2,
		CONSUMABLE_XP_BONUS3,
		CONSUMABLE_FP_BONUS3,
		NICK_NAME,
		DASH_TIME_INC,
		RESPWAN_TIME_DEC,
		FALLEN_DAMAGE_REDUCE,
		JUST_RESPAWN,
		RECORD_FULLY_INIT,
		RECORD_TEAM_INIT,
		RECORD_INDIVIDUAL_INIT,
		RECORD_BUNGEE_INIT,
		RECORD_EXPLOSION_INIT,
		RECORD_MISSION_INIT,
		RECORD_BND_INIT,
		RECORD_FLAG_INIT,
		RECORD_WEAPON_INIT,
		MINERAIL01_TCKT,
		MINERAIL02_TCKT,
		DRYGRASS_TCKT,
		WOODBOARD_TCKT,
		WHEEL_TCKT,
		CACTUS_TCKT,
		WEATERNBOARD_TCKT,
		HAYSTACK_TCKT,
		MINERAL_TCKT,
		NORMAL4_SET_TCKT,
		FUNC4_SET_TCKT,
		SEASON4_SET_TCKT,
		SPECIAL_AMMO_ADD,
		GOLD_TCKT,
		TNTBARREL_TCKT,
		WOODBOX_01_TCKT,
		WOODBOX_02_TCKT,
		REED_TCKT,
		BEAR_TRAP_TCKT,
		TRAIN_SET_TCKT
	}

	public string code;

	private Texture2D icon;

	public Texture2D icon11;

	public Texture2D funcIcon;

	public int season;

	public string grp1 = "none";

	public string grp2 = "none";

	public string grp3 = "none";

	private bool isBasic;

	public TYPE type;

	public string name;

	public int catType;

	public int catKind;

	public bool takeoffable;

	public SLOT slot;

	public string comment = string.Empty;

	public string exp1 = string.Empty;

	public string exp2 = string.Empty;

	public string exp3 = string.Empty;

	public TBuff tBuff;

	public bool discomposable;

	public string bpBackCode;

	public int upgradeType;

	public UPGRADE_CATEGORY upgradeCategory = UPGRADE_CATEGORY.NONE;

	private int _starRate = 100;

	public bool IsAmount;

	private static string[] types = new string[10]
	{
		"premium",
		"weapon",
		"cloth",
		"accessory",
		"character",
		"action",
		"toolbox",
		"upgrade",
		"bundle",
		"etc"
	};

	private static string[] weaponKinds = new string[5]
	{
		"all",
		"main",
		"aux",
		"melee",
		"spec"
	};

	private static string[] clothKinds = new string[4]
	{
		"all",
		"helmet",
		"upper",
		"lower"
	};

	private static string[] accessoryKinds = new string[9]
	{
		"all",
		"holster",
		"magazine_l",
		"magazine_r",
		"bag",
		"mask",
		"legcase",
		"kit",
		"bottle"
	};

	private static string[] slotString = new string[18]
	{
		"upper",
		"lower",
		"melee",
		"aux",
		"main",
		"bomb",
		"head",
		"face",
		"back",
		"leg",
		"sash1",
		"sash2",
		"sash3",
		"launcher",
		"magazine_l",
		"magazine_r",
		"kit",
		"character"
	};

	protected static string[] functionString = new string[121]
	{
		"clan_mark",
		"fly",
		"fly_fast",
		"line_tool",
		"replace_tool",
		"auto_reload",
		"heal",
		"assault_ammo",
		"speedup",
		"heartbeat_radar",
		"grenade_ammo",
		"pistol_ammo",
		"heavy_ammo",
		"sniper_ammo",
		"submachine_ammo",
		"respawn",
		"auto_heal",
		"premium_account",
		"heal50",
		"heal30",
		"hp_cooltime",
		"main_ammo_inc",
		"aux_ammo_inc",
		"special_ammo_inc",
		"charge_coin",
		"add_map_slot",
		"reset_map_slot",
		"brickstar_builder",
		"circle_window_tckt",
		"computer_box01_tckt",
		"computer_box02_tckt",
		"computer_box03_tckt",
		"crater_tckt",
		"flammable_drum_tckt",
		"function_set_tckt",
		"gravity_bluea_tckt",
		"gravity_blueb_tckt",
		"gravity_reda_tckt",
		"gravity_redb_tckt",
		"hatch_tckt",
		"laserwall_tckt",
		"metalcabinet_tckt",
		"metalladder_tckt",
		"scifi_set_tckt",
		"solar_collector_tckt",
		"ground01_tckt",
		"ground02_tckt",
		"metal1_tckt",
		"metal2_tckt",
		"metal3_tckt",
		"metal4_tckt",
		"toxic_drum_tckt",
		"trampoline_hor_tckt",
		"trampoline_ver_tckt",
		"valve_tckt",
		"speed_tckt",
		"explosion_tckt",
		"gravity_tckt",
		"plank_tckt",
		"woodbarrel_tckt",
		"candle_tckt",
		"fence_tckt",
		"bench_tckt",
		"armor_tckt",
		"torch_tckt",
		"flagblue_tckt",
		"flagred_tckt",
		"stained_tckt",
		"window_tckt",
		"trap_tckt",
		"door_tckt",
		"free_protal_tckt",
		"wood_set_tckt",
		"fire_set_tckt",
		"flag_set_tckt",
		"window_set_tckt",
		"team_portal_tckt",
		"portal_set_tckt",
		"normal3_set_tckt",
		"function3_set_tckt",
		"charge_force_point",
		"consumable_xp_bonus",
		"consumable_fp_bonus",
		"consumable_xp_bonus2",
		"consumable_fp_bonus2",
		"consumable_xp_bonus3",
		"consumable_fp_bonus3",
		"nick_name",
		"dash_time_inc",
		"respwan_time_dec",
		"fallen_damage_reduce",
		"just_respawn",
		"record_fully_init",
		"record_team_init",
		"record_individual_init",
		"record_bungee_init",
		"record_explosion_init",
		"record_mission_init",
		"record_bnd_init",
		"record_flag_init",
		"record_weapon_init",
		"minerail01_tckt",
		"minerail02_tckt",
		"drygrass_tckt",
		"woodboard_tckt",
		"wheel_tckt",
		"cactus_tckt",
		"westerndoor_tckt",
		"haystack_tckt",
		"mineral_tckt",
		"normal4_set_tckt",
		"function4_set_tckt",
		"season4_set_tckt",
		"special_ammo_add",
		"gold_tckt",
		"tntbarrel_tckt",
		"woodbox_01_tckt",
		"woodbox_02_tckt",
		"reed_tckt",
		"bear_trap_tckt",
		"train_set_tckt"
	};

	private static string[] upgradeTypes = new string[2]
	{
		"weapon",
		"cloth"
	};

	private static string[] upgradeCategories = new string[13]
	{
		"pimp_cat_heavy",
		"pimp_cat_assault",
		"pimp_cat_sniper",
		"pimp_cat_sub_machine",
		"pimp_cat_hand_gun",
		"pimp_cat_melee",
		"pimp_cat_grenade",
		"pimp_cat_flash_bang",
		"pimp_cat_smoke",
		"pimp_cat_upper_lower",
		"pimp_cat_helmet",
		"pimp_cat_other",
		"pimp_cat_shotgun"
	};

	public bool IsBasic => isBasic;

	public string Name => StringMgr.Instance.Get(name);

	public int _StarRate
	{
		get
		{
			return _starRate;
		}
		set
		{
			_starRate = value;
		}
	}

	public float StarRate => (float)_starRate / 100f;

	public bool IsEquipable => (SLOT.UPPER <= slot && slot < SLOT.NUM) || code == "s07" || code == "s08" || code == "s09" || code == "s92";

	public TItem(string itemCode, TYPE itemType, string itemName, Texture2D itemIcon, int ct, int ck, bool itemTakeoffable, SLOT itemSlot, string itemComment, TBuff tb, bool itemDiscomposable, string itemBpBackCode, int upType, UPGRADE_CATEGORY upCat, bool basic, int starRate)
	{
		code = itemCode;
		type = itemType;
		name = itemName;
		icon = itemIcon;
		catType = ct;
		catKind = ck;
		takeoffable = itemTakeoffable;
		slot = itemSlot;
		comment = itemComment;
		IsAmount = false;
		tBuff = tb;
		discomposable = itemDiscomposable;
		bpBackCode = itemBpBackCode;
		upgradeType = upType;
		upgradeCategory = upCat;
		isBasic = basic;
		_starRate = starRate;
	}

	public void SetIcon(Texture2D _icon)
	{
		icon = _icon;
	}

	public Texture2D CurIcon()
	{
		if (icon11 == null)
		{
			return icon;
		}
		if (BuildOption.Instance.IsDeveloper && BuildOption.Instance.Props.MyAge < 12 && icon11 != null)
		{
			return icon11;
		}
		if (BuildOption.Instance.IsNetmarble && MyInfoManager.Instance.Age < 12 && icon11 != null)
		{
			return icon11;
		}
		return icon;
	}

	public string GetOptionStringByOption(int opt)
	{
		if (opt >= 1000000)
		{
			return StringMgr.Instance.Get("INFINITE");
		}
		if (IsAmount)
		{
			return opt.ToString() + " " + StringMgr.Instance.Get("TIMES_UNIT");
		}
		return opt.ToString() + " " + StringMgr.Instance.Get("DAYS");
	}

	public static int String2Type(string typeString)
	{
		for (int i = 0; i < types.Length; i++)
		{
			if (types[i] == typeString)
			{
				return i;
			}
		}
		return types.Length;
	}

	public static int String2Kind(int type, string kindString)
	{
		switch (type)
		{
		case 1:
			for (int k = 0; k < weaponKinds.Length; k++)
			{
				if (weaponKinds[k] == kindString)
				{
					return k;
				}
			}
			return weaponKinds.Length;
		case 2:
			for (int j = 0; j < clothKinds.Length; j++)
			{
				if (clothKinds[j] == kindString)
				{
					return j;
				}
			}
			return clothKinds.Length;
		case 3:
			for (int i = 0; i < accessoryKinds.Length; i++)
			{
				if (accessoryKinds[i] == kindString)
				{
					return i;
				}
			}
			return accessoryKinds.Length;
		default:
			return -1;
		}
	}

	public string GetItemTypeString()
	{
		switch (catType)
		{
		case 0:
			return StringMgr.Instance.Get("ETC");
		case 1:
			return StringMgr.Instance.Get("WEAPON");
		case 2:
			return StringMgr.Instance.Get("CLOTH");
		case 3:
			return StringMgr.Instance.Get("ACCESSORY");
		case 4:
			return StringMgr.Instance.Get("CHARACTER");
		case 5:
			return StringMgr.Instance.Get("ACTIONPANEL");
		case 6:
			return StringMgr.Instance.Get("TOOLBOX");
		case 7:
			return StringMgr.Instance.Get("UPGRADE");
		case 8:
			return StringMgr.Instance.Get("BUNDLE");
		case 9:
			return StringMgr.Instance.Get("ETC");
		default:
			return string.Empty;
		}
	}

	public string GetItemTypeString(LangOptManager.LANG_OPT language)
	{
		switch (catType)
		{
		case 0:
			return StringMgr.Instance.Get("ETC", language);
		case 1:
			return StringMgr.Instance.Get("WEAPON", language);
		case 2:
			return StringMgr.Instance.Get("CLOTH", language);
		case 3:
			return StringMgr.Instance.Get("ACCESSORY", language);
		case 4:
			return StringMgr.Instance.Get("CHARACTER", language);
		case 5:
			return StringMgr.Instance.Get("ACTIONPANEL", language);
		case 6:
			return StringMgr.Instance.Get("TOOLBOX", language);
		case 7:
			return StringMgr.Instance.Get("UPGRADE", language);
		case 8:
			return StringMgr.Instance.Get("BUNDLE", language);
		case 9:
			return StringMgr.Instance.Get("ETC", language);
		default:
			return string.Empty;
		}
	}

	public string GetKindString()
	{
		switch (catType)
		{
		case 1:
			return string.Empty;
		case 2:
			switch (catKind)
			{
			case 0:
				return StringMgr.Instance.Get("CLOTH");
			case 1:
				return StringMgr.Instance.Get("HELMET");
			case 2:
				return StringMgr.Instance.Get("UPPER_CLOTH");
			case 3:
				return StringMgr.Instance.Get("LOWER_CLOTH");
			default:
				return StringMgr.Instance.Get("CLOTH");
			}
		case 3:
			switch (catKind)
			{
			case 0:
				return StringMgr.Instance.Get("ETC");
			case 1:
				return StringMgr.Instance.Get("HOLSTER");
			case 2:
				return StringMgr.Instance.Get("SIDEAMMO");
			case 3:
				return StringMgr.Instance.Get("WAISTBAG");
			case 4:
				return StringMgr.Instance.Get("BAG");
			case 5:
				return StringMgr.Instance.Get("MASK");
			case 6:
				return StringMgr.Instance.Get("LEGCASE");
			case 7:
				return StringMgr.Instance.Get("KIT");
			case 8:
				return StringMgr.Instance.Get("BOTTLE");
			default:
				return StringMgr.Instance.Get("ETC");
			}
		default:
			return string.Empty;
		}
	}

	public static SLOT String2Slot(string text)
	{
		for (int i = 0; i < slotString.Length; i++)
		{
			if (text == slotString[i])
			{
				return (SLOT)i;
			}
		}
		return SLOT.NUM;
	}

	public static int String2UpgradeType(string upType)
	{
		for (int i = 0; i < upgradeTypes.Length; i++)
		{
			if (upgradeTypes[i] == upType)
			{
				return i;
			}
		}
		return -1;
	}

	public static int String2UpgradeCategory(string cat)
	{
		for (int i = 0; i < upgradeCategories.Length; i++)
		{
			if (upgradeCategories[i] == cat)
			{
				return i;
			}
		}
		return -1;
	}

	public static string FunctionMaskToString(int func)
	{
		if (func < 0 || func >= functionString.Length)
		{
			return string.Empty;
		}
		return functionString[func];
	}

	public static int String2FunctionMask(string func)
	{
		func = func.ToLower();
		for (int i = 0; i < functionString.Length; i++)
		{
			if (functionString[i] == func)
			{
				return i;
			}
		}
		return -1;
	}
}
