using UnityEngine;

public class Weapon : Equip
{
	public enum TYPE
	{
		MELEE,
		AUX,
		MAIN,
		PROJECTILE,
		MODE_SPECIFIC,
		COUNT
	}

	public enum BY
	{
		ZOMBIE = -11,
		SELF,
		RANKINGUP,
		GOAL,
		CACTUS,
		BLACKHOLE,
		TRAP,
		TOXIC_DRUM,
		FLAM_DRUM,
		FIRE,
		COMPOSER,
		FALLOUT,
		BOXSHOT,
		M16,
		BM9,
		UZI,
		SSG69,
		M7,
		KG400,
		AK47,
		M4,
		PPSH41,
		ANACONDA,
		TOY_HAMMER,
		WRENCH,
		PIPE_WRENCH,
		BMK,
		M1918,
		MG42,
		MP40,
		STG44,
		BC1900,
		BWP38,
		BWPP,
		BMN91,
		GRENADE,
		VULCAN,
		AIR_CANNON,
		KG440,
		K1,
		K2,
		AWP,
		BRG42,
		CLOCKBOMB,
		BILLY_CLUB,
		LIGHT_SAVER,
		FIELD_SHOVEL,
		K1_SPECIAL,
		K2_SPECIAL,
		MG42_SPECIAL,
		TRG42_SPECIAL,
		LIGHT_SAVER_R,
		EVENT_HAMMER,
		FLASHBANG,
		BARRETT_M82A1,
		C8A1_CARBINE,
		FAMAS,
		G36K,
		HK416,
		JNG90,
		L85A2,
		WZ96_Beryl,
		DESERTEAGLE,
		GLOCK18,
		LUGER_P08,
		AK47_G,
		CONTEST_HAMMER,
		JNG_90_G,
		DESERTEAGLE_G,
		DEVILWEAPON,
		PUMPKINWEAPON,
		DEVILSTICK,
		PUMPKINSTICK,
		SKULLSTICK,
		DEVILBOMB,
		PUMPKINBOMB,
		DEVILWEAPON_MAX,
		PUMPKINWEAPON_MAX,
		DEVILSTICK_MAX,
		PUMPKINSTICK_MAX,
		SKULLSTICK_MAX,
		DEVILBOMB_MAX,
		PUMPKINBOMB_MAX,
		ECO,
		ERASER,
		PREDATOR,
		RAVAGER,
		FRYPAN,
		DELTA,
		BARD,
		SENSBOMB,
		B11,
		BN2000,
		W1893,
		W1893_MAX,
		ERASER_MAX,
		RAVAGER_MAX,
		SENSBOMB_MAX,
		XMAS_ROLLINGGUN,
		XMAS_BAZOOKA,
		XMAS_PIGGUN,
		XMAS_STICK01,
		XMAS_STICK02,
		XMAS_STICK03,
		XMASBOMB01,
		XMASBOMB02,
		XMASWINEBOTTLE,
		TOYHAMMER_BLUE,
		XMAS_PIGGUN_MAX,
		XMAS_ROLLINGGUN_MAX,
		XMAS_STICK01_MAX,
		XMAS_STICK02_MAX,
		XMAS_STICK03_MAX,
		XMASBOMB01_MAX,
		XMASBOMB02_MAX,
		XMASWINEBOTTLE_MAX,
		AWM_MAX,
		B11_MAX,
		BN2000_MAX,
		FAMAS_MAX,
		KG400_MAX,
		MATIAZO,
		ERASER_SPECIAL,
		PREDATOR_SPECIAL,
		RAVAGER_SPECIAL,
		DELTA_SPECIAL,
		REMOCON_SPECIAL,
		SPACEBOMB_SPECIAL,
		WINTER_STICK01,
		WINTER_STICK02,
		VALENTINESTICK,
		EVENT19_STICK,
		KG400_G,
		EGGBOMB,
		EGGBOMB_MAX,
		EASTER02_STICK,
		WAL01_STICK,
		WAL02_STICK,
		ARCHER_STICK,
		KNIGHT_STICK,
		MAGE_STICK,
		THIEF_STICK,
		S3_BOMB01,
		S3_BOMB02,
		ANCIENT,
		EAGLEEYE,
		FORESTHUNTER,
		HORNET,
		SPARROW,
		CROSSBOW_LAUNCHER,
		CROSSBOW_CONDOR,
		CROSSBOW_HORNET,
		CROSSBOW_HYDRA,
		KING_STICK,
		PRINCESS_STICK,
		STAFF01,
		WATERGUN,
		WATERDUCK,
		WATERSTICK,
		SUM02STICK,
		WATERBOMB,
		W_INDEPENDENCE_STICK,
		W_INDEPENDENCE_GUN,
		SUMMER_WATER_TURTLE_GUN,
		SUMMER_WATER_OCTOPUS_GUN,
		SUMMER_WATER_SPRAY_PUMP,
		SUMMER_WATER_BANANA_GUN,
		SUMMER_WATER_STICK02,
		SUMMER_WATER_BOMB02,
		BRICK_BOOMER,
		PREDATOR_O,
		MATIAZO_S,
		STAFF01_G,
		EAGLEEYE_W,
		CROSSBOW_CONDOR_G,
		CROSSBOW_CONDOR_G_MAX,
		WZ96_W,
		CROSSBOW_HORNET_S,
		AK47_B,
		M4_R,
		BMN91_R,
		BK416_C,
		K1_SPECIAL_B,
		ARCHER_STICK_R,
		TRG42_SPECIAL_B,
		ERASER_W,
		WZ96_W_MAX,
		CROSSBOW_HORNET_S_MAX,
		AK47_B_MAX,
		M4_R_MAX,
		BMN91_R_MAX,
		BK416_C_MAX,
		K1_SPECIAL_B_MAX,
		ARCHER_STICK_R_MAX,
		TRG42_SPECIAL_B_MAX,
		ERASER_W_MAX,
		YD_STICK,
		HELLFIRE,
		SHAFT,
		XWING,
		NEEDLE,
		MOONLIGHT,
		DRAGON_STICK,
		CENTAURUS_STICK,
		DARKKNIGHT_STICK,
		GRIFFON_STICK,
		ORC_STICK,
		WARLOCK_STICK,
		FRENCH_COSTUME_STICK,
		Q_BOOMSTICK,
		Q_CONDOR,
		Q_HORNET,
		Q_HYDRA,
		Q_BOMB01,
		Q_GRANATE,
		Q_ANA,
		Q_B1,
		Q_B1R,
		Q_B1R_1,
		Q_B2,
		Q_B2R,
		Q_BMK,
		Q_G36,
		Q_AWM,
		Q_BC1900,
		Q_BM9,
		Q_BRG42,
		Q_ERASER,
		Q_ERASER_R,
		Q_L85A2,
		Q_PREDATOR,
		Q_PREDATOR_R,
		Q_RAVAGER,
		Q_RAVAGER_R,
		Q_BWP,
		Q_DELTA,
		Q_DELTA_R,
		Q_M4,
		Q_M16,
		Q_WZ96,
		Q_DESERTEAGLE,
		Q_M1918,
		Q_MG42,
		Q_MG42_r,
		Q_GLOCK18,
		Q_FAMAS,
		Q_JNG90,
		Q_M82A1,
		Q_UZI,
		Q_AK47,
		Q_C8A1,
		Q_MP40,
		Q_PPSH41,
		Q_HK416,
		Q_STH44,
		Q_LUGERP08,
		Q_KISSKARD,
		Q_BMN91,
		Q_B_KG400,
		Q_B_KGS440,
		Q_BRG42_O,
		Q_BRG42_R,
		Q_BWPP,
		Q_FN2000,
		Q_K11,
		Q_MATIAZO,
		Q_SSG69,
		Q_W1893,
		Q_AK47_A,
		Q_AK47_G,
		Q_ECO,
		Q_ERASER_O,
		Q_PREDATOR_O,
		Q_M7,
		SUMMER_WATER_PUMP_MAX,
		SUMMER_WATER_DUCK_MAX,
		SUMMER_WATER_TURTLE_GUN_MAX,
		SUMMER_WATER_OCTOPUS_GUN_MAX,
		SUMMER_WATER_SPRAY_PUMP_MAX,
		SUMMER_WATER_BANANA_GUN_MAX,
		HELLFIRE_MAX,
		SHAFT_MAX,
		XWING_MAX,
		NEEDLE_MAX,
		MOONLIGHT_MAX,
		CROSSBOW_LAUNCHER_MAX,
		CROSSBOW_CONDOR_MAX,
		CROSSBOW_HORNET_MAX,
		CROSSBOW_HYDRA_MAX,
		LOLLIPOP_STICK,
		Q_B_KG400_MAX,
		Q_SSG69_MAX,
		Q_AK47_MAX,
		Q_BMK_MAX,
		Q_MG42_MAX,
		Q_BWP38_MAX,
		Q_B1_MAX,
		Q_AWM_MAX,
		Q_WZ96_MAX,
		Q_DESERTEAGLE_MAX,
		Q_ECO_MAX,
		Q_DELTA_MAX,
		Q_KISSKARD_MAX,
		Q_K11_MAX,
		Q_FN2000_MAX,
		Q_W1893_MAX,
		Q_BOMB01_MAX,
		Q_BOOMSTICK_MAX,
		Q_CONDOR_MAX,
		Q_HORNET_MAX,
		Q_HYDRA_MAX,
		Q_M4_02,
		Q_M1918_02,
		Q_ANA_02,
		Q_MATIAZO_02,
		Q_JNG90_02,
		Q_BC1900_02,
		Q_C8A1_02,
		Q_UZI_02,
		Q_GLOCK18_02,
		K1_SPECIAL_G,
		K2_SPECIAL_G,
		TRG42_SPECIAL_G,
		MG42_SPECIAL_G,
		RAVAGER_SPECIAL_G,
		DELTA_SPECIAL_G,
		PREDATOR_SPECIAL_G,
		ERASER_SPECIAL_G,
		DRAKE3000,
		GAMMA,
		TSUNAMIRAMA,
		ITEM_FLASHBANG,
		ITEM_KGS440,
		CARROT_BOMB,
		BAMBUSOIDEAE_STICK,
		BULLOCK,
		SHOOTINGSTAR,
		CAVALRY,
		HALFMOON,
		ROLLER,
		WESTERNSIGN,
		CACTUS_BOMB,
		TNT_BOMB,
		ROSE_STICK,
		SUNDANCE,
		HORSEHAIR,
		GOLDBADGE,
		HORSESHOE,
		HELLGATE,
		STEYRAUG_WHITE,
		OBLIVIONGUN_WHITE,
		STEYRAUG_BLACK,
		OBLIVIONGUN_BLACK,
		STEYRAUG_DESERT,
		OBLIVIONGUN_DESERT,
		MP7_SPECIAL,
		M60_SPECIAL,
		SCAR_SPECIAL,
		FLASH_SPECIAL,
		HK417_SPECIAL,
		TASER_SPECIAL,
		RIG_BOMB_SPECIAL,
		BULLOCK_T,
		BULLOCK02,
		SHOOTINGSTAR_T,
		SHOOTINGSTAR02,
		CAVALRY_T,
		CAVALRY02,
		HALFMOON_T,
		HALFMOON02,
		ROLLER_T,
		ROLLER02,
		SUNDANCE_T,
		SUNDANCE02,
		HORSEHAIR_T,
		HORSEHAIR02,
		GOLDBADGE_T,
		GOLDBADGE02,
		HORSESHOE_T,
		HORSESHOE02,
		HELLGATE_T,
		HELLGATE02,
		B_36K_OR,
		ECO_OR,
		EAGLEEYE_OR,
		SHAFT_OR,
		XWING_OR,
		NEEDLE_OR,
		SUNDANCE_OR,
		HELLGATE_OR,
		ROLLER_OR,
		SHOOTINGSTAR_OR,
		CACTUS_GUN,
		PSYCHOPATH_BOMB,
		PSYCHOPATH_STICK,
		REAPER_STICK,
		PSYCHOPATH_BOMB_MAX,
		PSYCHOPATH_STICK_MAX,
		REAPER_STICK_MAX,
		SKELETON_SNIPER,
		SKELETON_SNIPER_MAX
	}

	public enum GADGET
	{
		DRYFIRE,
		CLIPOUT,
		CLIPIN,
		BOLTUP
	}

	public TYPE slot;

	public AudioClip fireSound;

	public AudioClip dryFireSound;

	public AudioClip clipOutSound;

	public AudioClip clipInSound;

	public AudioClip boltUpSound;

	public GameObject muzzleFire;

	public float reloadSpeed = 1f;

	public float drawSpeed = 1f;

	public float range = float.PositiveInfinity;

	public float effectiveRange = float.PositiveInfinity;

	public GameObject BulletOrBody;

	public GameObject CatridgeOrClip;

	public float brokenRatio = 0.4f;

	public Vector3 carryRotation = new Vector3(0f, 90f, 90f);

	public static bool isInitialize;

	private float heldTime;

	public TYPE _slot
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

	public AudioClip _fireSound
	{
		get
		{
			return fireSound;
		}
		set
		{
			fireSound = value;
		}
	}

	public AudioClip DryFireSound
	{
		get
		{
			return dryFireSound;
		}
		set
		{
			dryFireSound = value;
		}
	}

	public AudioClip ClipOutSound
	{
		get
		{
			return clipOutSound;
		}
		set
		{
			clipOutSound = value;
		}
	}

	public AudioClip ClipInSound
	{
		get
		{
			return clipInSound;
		}
		set
		{
			clipInSound = value;
		}
	}

	public AudioClip BoltUpSound
	{
		get
		{
			return boltUpSound;
		}
		set
		{
			boltUpSound = value;
		}
	}

	public GameObject MuzzleFire
	{
		get
		{
			return muzzleFire;
		}
		set
		{
			muzzleFire = value;
		}
	}

	public float ReloadSpeed
	{
		get
		{
			return reloadSpeed;
		}
		set
		{
			reloadSpeed = value;
		}
	}

	public float DrawSpeed
	{
		get
		{
			return drawSpeed;
		}
		set
		{
			drawSpeed = value;
		}
	}

	public float Range
	{
		get
		{
			return range;
		}
		set
		{
			range = value;
		}
	}

	public float EffectiveRange
	{
		get
		{
			return effectiveRange;
		}
		set
		{
			effectiveRange = value;
		}
	}

	public GameObject _BulletOrBody
	{
		get
		{
			return BulletOrBody;
		}
		set
		{
			BulletOrBody = value;
		}
	}

	public GameObject _CatridgeOrClip
	{
		get
		{
			return CatridgeOrClip;
		}
		set
		{
			CatridgeOrClip = value;
		}
	}

	public float BrokenRatio
	{
		get
		{
			return brokenRatio;
		}
		set
		{
			brokenRatio = value;
		}
	}

	public Vector3 CarryRotation
	{
		get
		{
			return carryRotation;
		}
		set
		{
			carryRotation = value;
		}
	}

	public float FlushHeldTime()
	{
		float result = heldTime;
		heldTime = 0f;
		return result;
	}

	public void GadgetSound(GADGET gadget)
	{
		AudioClip audioClip = null;
		switch (gadget)
		{
		case GADGET.DRYFIRE:
			audioClip = dryFireSound;
			break;
		case GADGET.CLIPOUT:
			audioClip = clipOutSound;
			break;
		case GADGET.CLIPIN:
			audioClip = clipInSound;
			break;
		case GADGET.BOLTUP:
			audioClip = boltUpSound;
			break;
		}
		AudioSource audioSource = null;
		AudioSource audioSource2 = null;
		AudioSource[] componentsInChildren = GetComponentsInChildren<AudioSource>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].gameObject == base.gameObject)
			{
				audioSource = componentsInChildren[i];
			}
			else
			{
				audioSource2 = componentsInChildren[i];
			}
		}
		if (null == audioSource2)
		{
			audioSource2 = audioSource;
		}
		if (null != audioSource2 && null != audioClip && !isInitialize)
		{
			audioSource2.PlayOneShot(audioClip);
		}
	}

	public void FireSound()
	{
		AudioSource component = GetComponent<AudioSource>();
		if (null != component && null != fireSound)
		{
			component.PlayOneShot(fireSound);
		}
	}

	public void StartFireSound()
	{
		AudioSource component = GetComponent<AudioSource>();
		if (null != component && null != fireSound)
		{
			component.Stop();
			component.clip = fireSound;
			component.loop = true;
			component.Play();
		}
	}

	public void EndFireSound()
	{
		AudioSource component = GetComponent<AudioSource>();
		if (null != component)
		{
			component.Stop();
		}
	}

	private void Start()
	{
		heldTime = 0f;
		Modify();
	}

	private void Update()
	{
		heldTime += Time.deltaTime;
	}

	private void Modify()
	{
		WeaponFunction component = GetComponent<WeaponFunction>();
		if (null != component)
		{
			WpnMod wpnMod = WeaponModifier.Instance.Get((int)component.weaponBy);
			if (wpnMod != null)
			{
				reloadSpeed = wpnMod.fReloadSpeed;
				drawSpeed = wpnMod.fDrawSpeed;
				range = wpnMod.fRange;
				effectiveRange = wpnMod.effectiveRange;
				brokenRatio = wpnMod.brokenRatio;
			}
		}
	}
}
