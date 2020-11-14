using _Emulator;
using System;
using System.Net;
using UnityEngine;

public class CSNetManager : MonoBehaviour
{
	private string roundRobinIp = string.Empty;

	private int roundRobinPort = 18890;

	private string bfServer = string.Empty;

	private int bfPort;

	private static CSNetManager _instance;

	private SockTcp sock;

	private SockTcp switchAfter;

	public bool needDump;

	public bool needFullDump;

	public string RoundRobinIp => roundRobinIp;

	public int RoundRobinPort => roundRobinPort;

	public string BfServer
	{
		get
		{
			return bfServer;
		}
		set
		{
			bfServer = value;
		}
	}

	public int BfPort
	{
		get
		{
			return bfPort;
		}
		set
		{
			bfPort = value;
		}
	}

	public static CSNetManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (UnityEngine.Object.FindObjectOfType(typeof(CSNetManager)) as CSNetManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the CSNetManager Instance");
				}
			}
			return _instance;
		}
	}

	public SockTcp Sock
	{
		get
		{
			return sock;
		}
		set
		{
			sock = value;
		}
	}

	public SockTcp SwitchAfter
	{
		get
		{
			return switchAfter;
		}
		set
		{
			switchAfter = value;
		}
	}

	public void Clear()
	{
		if (sock != null)
		{
			sock.Close();
			sock = null;
		}
		if (switchAfter != null)
		{
			switchAfter.Close();
			switchAfter = null;
		}
	}

	private void OnApplicationQuit()
	{
		Clear();
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
		Core.instance.Initialize();
		sock = null;
		switchAfter = null;
		Property props = BuildOption.Instance.Props;
		if (props.isWebPlayer)
		{
			Security.PrefetchSocketPolicy(props.GetRoundRobinServer, 843);
		}
		try
		{
            IPAddress[] hostAddresses = Dns.GetHostAddresses(props.GetRoundRobinServer);
			if (hostAddresses != null && hostAddresses.Length > 0)
			{
				roundRobinIp = hostAddresses[0].ToString();
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message.ToString());
			Debug.LogError("Fail to dns look up, it will treat the address as an ip address");
			roundRobinIp = props.GetRoundRobinServer;
		}
	}

	private void Update()
	{
		if (switchAfter != null)
		{
			sock = switchAfter;
			switchAfter = null;
		}
		if (sock != null)
		{
			sock.Update();
			if (!Application.isLoadingLevel && Application.loadedLevelName != "Bootstrap" && Application.loadedLevelName != "Login" && !sock.IsConnected() && (switchAfter == null || !switchAfter.IsConnected()) && !GlobalVars.Instance.shutdownNow)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NETWORK_BROKEN"));
				BuildOption.Instance.Exit();
			}
		}
	}
}
