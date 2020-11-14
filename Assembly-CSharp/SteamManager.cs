using Infernum;
using UnityEngine;

public class SteamManager : MonoBehaviour
{
	private bool _loaded;

	private static SteamManager _instance;

	public static SteamManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(SteamManager)) as SteamManager);
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
