using UnityEngine;

public class AutoLogout : MonoBehaviour
{
	public Texture2D bg;

	private string token = string.Empty;

	private string[] tokens;

	private void Awake()
	{
	}

	private void Start()
	{
		BuildOption.Instance.ResetSingletons();
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		GUI.enabled = !DialogManager.Instance.IsModal;
		TextureUtil.DrawTexture(new Rect((float)((Screen.width - bg.width) / 2), (float)((Screen.height - bg.height) / 2), (float)bg.width, (float)bg.height), bg);
		LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), (float)(Screen.height - 32)), StringMgr.Instance.Get("AUTOLOGOUT_GUIDE"), "BigLabel", new Color(0.9f, 0.6f, 0f), GlobalVars.txtEmptyColor, TextAnchor.LowerCenter);
		GUI.enabled = true;
	}

	public void Relogin(string parameters)
	{
		if (BuildOption.Instance.IsRunup)
		{
			tokens = CommandInterpreter.ExtractValueFromParameterByRunup(parameters);
			if (tokens != null && tokens.Length > 0)
			{
				MyInfoManager.Instance.AutoLogin = MyInfoManager.AUTOLOGIN.RUNUP;
				CSNetManager.Instance.SwitchAfter = new SockTcp();
				if (!CSNetManager.Instance.SwitchAfter.Open(CSNetManager.Instance.RoundRobinIp, CSNetManager.Instance.RoundRobinPort))
				{
					BuildOption.Instance.Exit();
				}
				else if (CSNetManager.Instance.Sock != null)
				{
					CSNetManager.Instance.Sock.Close();
				}
			}
		}
		else if (BuildOption.Instance.IsNetmarble)
		{
			if (token.Length > 0)
			{
				MyInfoManager.Instance.AutoLogin = MyInfoManager.AUTOLOGIN.NETMARBLE;
				CSNetManager.Instance.SwitchAfter = new SockTcp();
				if (!CSNetManager.Instance.SwitchAfter.Open(CSNetManager.Instance.RoundRobinIp, CSNetManager.Instance.RoundRobinPort))
				{
					BuildOption.Instance.Exit();
				}
				else if (CSNetManager.Instance.Sock != null)
				{
					CSNetManager.Instance.Sock.Close();
				}
			}
		}
		else
		{
			token = CommandInterpreter.ExtractValueFromParameter(parameters, "loginToken", string.Empty);
			if (token.Length > 0)
			{
				MyInfoManager.Instance.AutoLogin = MyInfoManager.AUTOLOGIN.INFERNUM;
				string a = CommandInterpreter.ExtractValueFromParameter(parameters, "sitecode", string.Empty).ToLower();
				if (a == "steam" && !SteamManager_o.Instance.LoadSteamDll())
				{
					Debug.Log("SteamAPI_Init Failed");
				}
				CSNetManager.Instance.SwitchAfter = new SockTcp();
				if (!CSNetManager.Instance.SwitchAfter.Open(CSNetManager.Instance.RoundRobinIp, CSNetManager.Instance.RoundRobinPort))
				{
					BuildOption.Instance.Exit();
				}
				else if (CSNetManager.Instance.Sock != null)
				{
					CSNetManager.Instance.Sock.Close();
				}
			}
		}
	}

	private void OnRoundRobin()
	{
		CSNetManager.Instance.SwitchAfter = new SockTcp();
		if (CSNetManager.Instance.SwitchAfter.Open(CSNetManager.Instance.BfServer, CSNetManager.Instance.BfPort) && CSNetManager.Instance.Sock != null)
		{
			CSNetManager.Instance.Sock.Close();
		}
	}

	private void OnServiceFail(int reason)
	{
	}

	private void OnSeed()
	{
		switch (MyInfoManager.Instance.AutoLogin)
		{
		case MyInfoManager.AUTOLOGIN.INFERNUM:
			CSNetManager.Instance.Sock.SendCS_AUTO_LOGIN_REQ(token, BuildOption.Instance.Major, BuildOption.Instance.Minor);
			break;
		case MyInfoManager.AUTOLOGIN.RUNUP:
			CSNetManager.Instance.Sock.SendCS_AUTO_LOGIN_TO_RUNUP_REQ(tokens[0], tokens[1], int.Parse(tokens[2]), BuildOption.Instance.Major, BuildOption.Instance.Minor);
			break;
		case MyInfoManager.AUTOLOGIN.NETMARBLE:
			CSNetManager.Instance.Sock.SendCS_AUTO_LOGIN_TO_NETMARBLE_REQ(NMCrypt_Manager.Instance.cookie, BuildOption.Instance.Major, BuildOption.Instance.Minor);
			break;
		}
	}

	private void OnLoginFail()
	{
	}
}
