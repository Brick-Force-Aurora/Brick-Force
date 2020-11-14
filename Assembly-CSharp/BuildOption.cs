using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class BuildOption : MonoBehaviour
{
	public enum XP_MODE
	{
		HARD,
		EASY
	}

	public enum VOICE_LANG
	{
		KOREAN,
		ENGLISH,
		GERMAN,
		FRENCH,
		POLISH,
		SPANISH,
		TURKISH
	}

	public enum COUNTRY_FILTER
	{
		NONE = -1,
		EU,
		GERMANY,
		FRANCE,
		POLAND,
		SPAIN,
		UNITED_KINGDOM,
		TURKEY,
		CANADA_F,
		CANADA_E,
		USA_EAST,
		USA_WEST,
		MEXICO,
		HONGKONG,
		TAIWAN,
		MALAYSIA,
		INDONESIA,
		USA,
		CANADA,
		K_NEWBIE,
		K_BATTLE,
		K_BUILD,
		K_CLAN,
		I_BEGINNER,
		I_BUILD,
		I_CLAN,
		I_MEDIUM,
		I_VETERAN,
		I_FUN
	}

	public enum TARGET
	{
		DEVELOPER,
		TEST_SERVER,
		KOREA_SERVER,
		CHINESE_SERVER,
		TEST_SERVER_WEB_PLAY,
		DEVELOPER_WEB_PLAY,
		KOREA_SERVER_WEB_PLAY,
		INFERNUM_SERVER,
		INFERNUM_SERVER_WEB_PLAY_EU,
		INFERNUM_SERVER_WEB_PLAY_NA,
		INFERNUM_LOCAL,
		INFERNUM_LOCAL_WEB_PLAY,
		RUNUP_LOCAL,
		RUNUP_LOCAL_WEB_PLAY,
		WAVE_LOCAL,
		WAVE_LOCAL_WEB_PLAYER,
		RUNUP_LIVE,
		RUNUP_LIVE_WEB_PLAYER,
		WAVE_LIVE,
		WAVE_LIVE_WEB_PLAYER,
		WAVE_REAL,
		WAVE_REAL_WEB_PLAYER,
		NETMARBLE_LOCAL,
		NETMARBLE_LIVE,
		AXESO5_LOCAL,
		AXESO5_LIVE
	}

	public enum SEASON
	{
		BRICK_FORCE = 1,
		BRICK_STAR,
		BRICK_SAGA,
		BRICK_WESTERN,
		BRICK_SEASON5
	}

	public enum RANDOM_BOX_TYPE
	{
		INFERNUM,
		NETMARBLE,
		NOT_USE
	}

	public TARGET target;

	public Property[] properties;

	public int Major;

	public int Minor = 218;

	public int Build;

	public bool DontCheckXTrap = true;

	public string[] CountryNames;

	public Texture2D[] CountryIcons;

	public Texture2D defaultCountryFilter;

	private static BuildOption _instance;

	public bool exportItem;

	public bool exportBrick;

	public bool exportTestshop;

	public bool exportBuff;

	private Mutex mMutex;

	private bool createNew;

	public bool IsRunup => target == TARGET.RUNUP_LIVE || target == TARGET.RUNUP_LOCAL || target == TARGET.RUNUP_LIVE_WEB_PLAYER || target == TARGET.RUNUP_LOCAL_WEB_PLAY;

	public bool MustAutoLogin => target == TARGET.RUNUP_LIVE || target == TARGET.RUNUP_LIVE_WEB_PLAYER;

	public bool RunFunPortal => target == TARGET.RUNUP_LIVE || target == TARGET.RUNUP_LOCAL;

	public bool IsInfernum => target == TARGET.INFERNUM_SERVER || target == TARGET.INFERNUM_SERVER_WEB_PLAY_EU || target == TARGET.INFERNUM_SERVER_WEB_PLAY_NA || target == TARGET.INFERNUM_LOCAL || target == TARGET.INFERNUM_LOCAL_WEB_PLAY;

	public bool IsNetmarble => target == TARGET.NETMARBLE_LOCAL || target == TARGET.NETMARBLE_LIVE;

	public bool IsNetmarbleOrDev => target == TARGET.NETMARBLE_LOCAL || target == TARGET.NETMARBLE_LIVE || target == TARGET.DEVELOPER;

	public bool IsDeveloper => target == TARGET.DEVELOPER;

	public bool IsTester => target == TARGET.TEST_SERVER || target == TARGET.TEST_SERVER;

	public bool IsAxeso5 => target == TARGET.AXESO5_LOCAL || target == TARGET.AXESO5_LIVE;

	public bool IsWave => target == TARGET.WAVE_LOCAL || target == TARGET.WAVE_LOCAL_WEB_PLAYER || target == TARGET.WAVE_REAL || target == TARGET.WAVE_REAL_WEB_PLAYER || target == TARGET.DEVELOPER;

	public Property Props => properties[(int)target];

	public static BuildOption Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(BuildOption)) as BuildOption);
				if (null == _instance)
				{
					UnityEngine.Debug.LogError("ERROR, Fail to get the BuildOption Instance");
				}
			}
			return _instance;
		}
	}

	public bool AllowBuildGunInDestroyPhase()
	{
		return properties[(int)target].allowBuildGunInDestroyPhase;
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
		Props.Awake();
	}

	private void Update()
	{
	}

	public void Exit()
	{
		if (MyInfoManager.Instance.IsAutoLogin)
		{
			Application.Quit();
		}
		else
		{
			Application.LoadLevel("Login");
		}
		GlobalVars.Instance.shutdownNow = false;
	}

	public void HardExit()
	{
		Application.Quit();
	}

	public void ResetSingletons()
	{
		ChannelManager.Instance.Clear();
		DialogManager.Instance.Clear();
		ContextMenuManager.Instance.Clear();
		ChannelUserManager.Instance.Clear();
		RoomManager.Instance.CurrentRoom = -1;
		SquadManager.Instance.Clear();
		RoomManager.Instance.Clear();
		UserMapInfoManager.Instance.Clear();
		MyInfoManager.Instance.Clear();
		MemoManager.Instance.Clear();
		RegMapManager.Instance.DownloadedClear();
		BannerManager.Instance.Clear();
		if (!Props.loadShopTxt)
		{
			BundleManager.Instance.Clear();
		}
		MissionManager.Instance.Clear();
		CSNetManager.Instance.Clear();
		P2PManager.Instance.Shutdown();
	}

	private void OnApplicationQuit()
	{
		if (RunFunPortal)
		{
			Process process = new Process();
			process.StartInfo.FileName = "FunPortal.exe";
			process.StartInfo.Arguments = "182";
			process.Start();
		}
	}

	public bool IsDuplicateExcute()
	{
		mMutex = new Mutex(initiallyOwned: true, "Global\\Exe_BrickForce_Mutex", out createNew);
		mMutex.GetHashCode();
		return createNew;
	}

	public static bool IsWindowsPlayerOrEditor()
	{
		/*if (Application.platform == RuntimePlatform.WindowsPlayer)
		{
			return true;
		}
		if (Application.platform == RuntimePlatform.WindowsEditor)
		{
			return true;
		}*/
		return false;
	}

	public static void OpenURL(string url)
	{
		Application.OpenURL(url);
	}

	public string TokensParameter()
	{
		string result = string.Empty;
		if (IsInfernum)
		{
			result = "?userid=" + MyInfoManager.Instance.Seq + "?nick=" + MyInfoManager.Instance.Nickname;
		}
		return result;
	}
}
