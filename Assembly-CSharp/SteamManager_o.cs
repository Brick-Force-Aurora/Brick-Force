using Infernum;
using UnityEngine;

public class SteamManager_o : MonoBehaviour
{
	private bool _loaded;

	private static SteamManager_o _instance;

	public static SteamManager_o Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(SteamManager_o)) as SteamManager_o);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the SteamManager Instance");
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnDestroy()
	{
		if (_loaded)
		{
			SteamDLL.SteamAPI_Shutdown();
			_loaded = false;
		}
	}

	public bool LoadSteamDll()
	{
		if (!BuildOption.Instance.Props.UseSteam)
		{
			return true;
		}
		if (!_loaded)
		{
			_loaded = SteamDLL.SteamAPI_Init();
		}
		return _loaded;
	}
}
