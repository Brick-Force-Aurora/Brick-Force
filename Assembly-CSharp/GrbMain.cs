using UnityEngine;

public class GrbMain : MonoBehaviour
{
	private enum STEP
	{
		FADE_IN,
		WAIT,
		FADE_OUT,
		AUTO_LOGIN,
		AUTO_LOGIN_TO_RUNUP,
		AUTO_LOGIN_TO_NETMARBLE
	}

	public Texture grbLogo;

	public float fadeInTime = 1f;

	public float waitTime = 3f;

	public float fadeOutTime = 1f;

	private float deltaTime;

	private string token = string.Empty;

	private string[] tokens;

	private STEP step;

	private void Awake()
	{
	}

	private void Start()
	{
		FadeIn();
	}

	private void OnGUI()
	{
		if (null != grbLogo)
		{
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			switch (step)
			{
			case STEP.FADE_IN:
				GUI.color = Color.Lerp(new Color(1f, 1f, 1f, 0f), new Color(1f, 1f, 1f, 1f), deltaTime / fadeInTime);
				break;
			case STEP.WAIT:
				GUI.color = new Color(1f, 1f, 1f, 1f);
				break;
			case STEP.FADE_OUT:
				GUI.color = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(1f, 1f, 1f, 0f), deltaTime / fadeOutTime);
				break;
			}
			TextureUtil.DrawTexture(new Rect((float)((Screen.width - grbLogo.width) / 2), (float)((Screen.height - grbLogo.height) / 2), (float)grbLogo.width, (float)grbLogo.height), grbLogo);
		}
	}

	private void FadeIn()
	{
		step = STEP.FADE_IN;
		deltaTime = 0f;
	}

	private void FadeOut()
	{
		step = STEP.FADE_OUT;
		deltaTime = 0f;
	}

	private void Wait()
	{
		step = STEP.WAIT;
		deltaTime = 0f;
	}

	private void MoveNext()
	{
		if (Application.CanStreamedLevelBeLoaded("Login") && Application.CanStreamedLevelBeLoaded("PlayerInfo") && Application.CanStreamedLevelBeLoaded("BfStart") && Application.CanStreamedLevelBeLoaded("Tos"))
		{
			if (BuildOption.Instance.IsRunup)
			{
				tokens = CommandInterpreter.ExtractValueFromParameterByRunup(WebParam.Instance.Parameters);
				if (tokens == null || tokens.Length <= 0)
				{
					tokens = CommandInterpreter.ExtractValueFromParameterByRunup();
				}
				if (tokens == null || tokens.Length <= 0)
				{
					if (BuildOption.Instance.MustAutoLogin)
					{
						BuildOption.Instance.HardExit();
					}
					else
					{
						Application.LoadLevel("Login");
					}
				}
				else
				{
					MyInfoManager.Instance.AutoLogin = MyInfoManager.AUTOLOGIN.RUNUP;
					CSNetManager.Instance.SwitchAfter = new SockTcp();
					if (!CSNetManager.Instance.SwitchAfter.Open(CSNetManager.Instance.RoundRobinIp, CSNetManager.Instance.RoundRobinPort))
					{
						BuildOption.Instance.Exit();
					}
					else
					{
						if (CSNetManager.Instance.Sock != null)
						{
							CSNetManager.Instance.Sock.Close();
						}
						step = STEP.AUTO_LOGIN_TO_RUNUP;
					}
				}
			}
			else
			{
				token = CommandInterpreter.ExtractValueFromParameter(WebParam.Instance.Parameters, "loginToken", string.Empty);
				if (token.Length <= 0)
				{
					token = CommandInterpreter.ExtractValueFromParameter("loginToken", string.Empty);
				}
				if (BuildOption.Instance.IsNetmarble)
				{
					MyInfoManager.Instance.AutoLogin = MyInfoManager.AUTOLOGIN.NETMARBLE;
					CSNetManager.Instance.SwitchAfter = new SockTcp();
					if (!CSNetManager.Instance.SwitchAfter.Open(CSNetManager.Instance.RoundRobinIp, CSNetManager.Instance.RoundRobinPort))
					{
						BuildOption.Instance.Exit();
					}
					else
					{
						if (CSNetManager.Instance.Sock != null)
						{
							CSNetManager.Instance.Sock.Close();
						}
						step = STEP.AUTO_LOGIN_TO_NETMARBLE;
					}
				}
				else if (token.Length <= 0)
				{
					Application.LoadLevel("Login");
				}
				else
				{
					MyInfoManager.Instance.AutoLogin = MyInfoManager.AUTOLOGIN.INFERNUM;
					string a = CommandInterpreter.ExtractValueFromParameter("sitecode", string.Empty).ToLower();
					if (a == "steam" && !SteamManager.Instance.LoadSteamDll())
					{
						Debug.Log("SteamAPI_Init Failed");
					}
					CSNetManager.Instance.SwitchAfter = new SockTcp();
					if (!CSNetManager.Instance.SwitchAfter.Open(CSNetManager.Instance.RoundRobinIp, CSNetManager.Instance.RoundRobinPort))
					{
						BuildOption.Instance.Exit();
					}
					else
					{
						if (CSNetManager.Instance.Sock != null)
						{
							CSNetManager.Instance.Sock.Close();
						}
						step = STEP.AUTO_LOGIN;
					}
				}
			}
		}
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		switch (step)
		{
		case STEP.FADE_IN:
			if (deltaTime > fadeInTime)
			{
				Wait();
			}
			break;
		case STEP.WAIT:
			if (deltaTime > waitTime)
			{
				FadeOut();
			}
			break;
		case STEP.FADE_OUT:
			if (deltaTime > fadeOutTime)
			{
				MoveNext();
			}
			break;
		}
	}

	private void OnRoundRobin()
	{
		CSNetManager.Instance.SwitchAfter = new SockTcp();
		if (CSNetManager.Instance.SwitchAfter.Open(CSNetManager.Instance.BfServer, CSNetManager.Instance.BfPort))
		{
			if (CSNetManager.Instance.Sock != null)
			{
				CSNetManager.Instance.Sock.Close();
			}
		}
		else
		{
			BuildOption.Instance.Exit();
		}
	}

	private void OnServiceFail(int reason)
	{
		BuildOption.Instance.Exit();
	}

	private void OnSeed()
	{
		switch (step)
		{
		case STEP.AUTO_LOGIN:
			CSNetManager.Instance.Sock.SendCS_AUTO_LOGIN_REQ(token, BuildOption.Instance.Major, BuildOption.Instance.Minor);
			break;
		case STEP.AUTO_LOGIN_TO_RUNUP:
			CSNetManager.Instance.Sock.SendCS_AUTO_LOGIN_TO_RUNUP_REQ(tokens[0], tokens[1], int.Parse(tokens[2]), BuildOption.Instance.Major, BuildOption.Instance.Minor);
			break;
		case STEP.AUTO_LOGIN_TO_NETMARBLE:
			CSNetManager.Instance.Sock.SendCS_AUTO_LOGIN_TO_NETMARBLE_REQ(NMCrypt_Manager.Instance.cookie, BuildOption.Instance.Major, BuildOption.Instance.Minor);
			break;
		}
	}

	private void OnLoginFail()
	{
		BuildOption.Instance.Exit();
	}

	private void OnLoginFailMessage(string message)
	{
		ExitConfirmDialog exitConfirmDialog = (ExitConfirmDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.EXIT_CONFIRM, exclusive: true);
		if (exitConfirmDialog != null)
		{
			exitConfirmDialog.InitDialog(message);
			exitConfirmDialog.CloseButtonHide(isHide: true);
		}
	}
}
