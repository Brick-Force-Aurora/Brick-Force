using UnityEngine;

public class LoadOthersMain : MonoBehaviour
{
	public enum STEP
	{
		PUBLISHER,
		DEVELOPER,
		AUTO_LOGIN,
		AUTO_LOGIN_TO_RUNUP,
		AUTO_LOGIN_TO_NETMARBLE,
		MOVIE_PUBLISHER
	}

	public Texture exeLogo;

	private Texture publisherLogo;

	public Texture loadingImage;

	public float fadeInTime = 3f;

	public float fadeOutTime = 1f;

	public AudioClip bang;

	public Vector2 logoSize = new Vector2(343f, 120f);

	private bool once;

	private bool onceItem;

	private bool onceusk;

	private bool onceShop;

	private float deltaTime;

	private float alpha;

	private float alphaFrom;

	private float alphaTo = 1f;

	private bool isFadeIn = true;

	public bool isDownLoading;

	public bool isProgressStop;

	public STEP step;

	public string token = string.Empty;

	public string[] tokens;

	private void Awake()
	{
	}

	private void Start()
	{
		once = false;
		onceItem = false;
		onceShop = false;
		publisherLogo = BuildOption.Instance.Props.publisher;
		if (VersionTextureManager.Instance.moviePublisher != null)
		{
			step = STEP.MOVIE_PUBLISHER;
		}
		else if (null != publisherLogo)
		{
			step = STEP.PUBLISHER;
		}
		else
		{
			step = STEP.DEVELOPER;
		}
		FadeIn();
	}

	private void OnGUI()
	{
		switch (step)
		{
		case STEP.DEVELOPER:
			if (null != exeLogo)
			{
				GUI.skin = GUISkinFinder.Instance.GetGUISkin();
				GUI.color = new Color(1f, 1f, 1f, alpha);
				TextureUtil.DrawTexture(new Rect((float)((Screen.width - exeLogo.width) / 2), (float)((Screen.height - exeLogo.height) / 2), (float)exeLogo.width, (float)exeLogo.height), exeLogo);
			}
			break;
		case STEP.PUBLISHER:
			if (null != publisherLogo)
			{
				GUI.skin = GUISkinFinder.Instance.GetGUISkin();
				GUI.color = new Color(1f, 1f, 1f, alpha);
				TextureUtil.DrawTexture(new Rect((float)((Screen.width - publisherLogo.width) / 2), (float)((Screen.height - publisherLogo.height) / 2), (float)publisherLogo.width, (float)publisherLogo.height), publisherLogo);
			}
			break;
		case STEP.MOVIE_PUBLISHER:
			if (null != VersionTextureManager.Instance.moviePublisher)
			{
				GUI.skin = GUISkinFinder.Instance.GetGUISkin();
				GUI.color = new Color(1f, 1f, 1f, 1f);
				TextureUtil.DrawTexture(new Rect((float)((Screen.width - VersionTextureManager.Instance.moviePublisher.width) / 2), (float)((Screen.height - VersionTextureManager.Instance.moviePublisher.height) / 2), (float)VersionTextureManager.Instance.moviePublisher.width, (float)VersionTextureManager.Instance.moviePublisher.height), VersionTextureManager.Instance.moviePublisher);
			}
			break;
		}
		if (isDownLoading)
		{
			GUI.color = new Color(1f, 1f, 1f, 1f);
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			Vector2 vector = new Vector2((float)((Screen.width - loadingImage.width) / 2), (float)((Screen.height - loadingImage.height) / 2));
			TextureUtil.DrawTexture(new Rect(vector.x, vector.y, (float)loadingImage.width, (float)loadingImage.height), loadingImage);
			Texture2D logo = BuildOption.Instance.Props.logo;
			if (null != logo)
			{
				Vector2 vector2 = new Vector2((float)((Screen.width - logo.width) / 2), vector.y + (float)loadingImage.height - logoSize.y);
				TextureUtil.DrawTexture(new Rect(vector2.x, vector2.y, (float)logo.width, (float)logo.height), logo, ScaleMode.StretchToFill);
			}
			LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), (float)(Screen.height - 32)), "Downloading Once...", "BigLabel", new Color(0.9f, 0.6f, 0f), GlobalVars.txtEmptyColor, TextAnchor.LowerCenter);
		}
	}

	private void FadeIn()
	{
		isFadeIn = true;
		deltaTime = 0f;
		alphaFrom = 0f;
		alphaTo = 1f;
	}

	private void FadeOut()
	{
		isFadeIn = false;
		deltaTime = 0f;
		alphaFrom = alpha;
		alphaTo = 0f;
	}

	private void Bang()
	{
		if (bang != null)
		{
			AudioSource component = GetComponent<AudioSource>();
			if (null != component)
			{
				component.PlayOneShot(bang);
			}
		}
	}

	private void Update()
	{
		if (!once && SceneLoadManager.Instance.IsLoadedDone())
		{
			once = true;
			CommandInterpreter.Instance.Load();
			BuffManager.Instance.Load();
			DefenseManager.Instance.LoadAll();
			UpgradePropManager.Instance.LoadAll();
			if (BuildOption.Instance.Props.isluncherExecuteOnly && (BuildOption.Instance.IsNetmarble || BuildOption.Instance.target == BuildOption.TARGET.WAVE_REAL || BuildOption.Instance.IsAxeso5))
			{
				string a = CommandInterpreter.ExtractValueFromParameter("luncher", string.Empty);
				if (a != "use")
				{
					ExitConfirmDialog exitConfirmDialog = (ExitConfirmDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.EXIT_CONFIRM, exclusive: true);
					if (exitConfirmDialog != null)
					{
						exitConfirmDialog.InitDialog(StringMgr.Instance.Get("LUNCHER_MUST_EXCUTE"));
						exitConfirmDialog.CloseButtonHide(isHide: true);
					}
					isProgressStop = true;
				}
			}
			if (!BuildOption.Instance.Props.isDuplicateExcuteAble && !isProgressStop && !BuildOption.Instance.IsDuplicateExcute())
			{
				ExitConfirmDialog exitConfirmDialog2 = (ExitConfirmDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.EXIT_CONFIRM, exclusive: true);
				if (exitConfirmDialog2 != null)
				{
					exitConfirmDialog2.InitDialog(StringMgr.Instance.Get("DUPLICATE_EXCUTE"));
					exitConfirmDialog2.CloseButtonHide(isHide: true);
				}
				isProgressStop = true;
			}
			if (VersionTextureManager.Instance.moviePublisher != null)
			{
				VersionTextureManager.Instance.moviePublisher.Play();
				if (VersionTextureManager.Instance.moviePublisher.audioClip != null)
				{
					AudioSource component = GetComponent<AudioSource>();
					if (null != component)
					{
						component.mute = false;
						component.clip = VersionTextureManager.Instance.moviePublisher.audioClip;
						component.Stop();
						component.Play();
					}
				}
			}
		}
		if (!onceusk && BuffManager.Instance.IsLoaded)
		{
			onceusk = true;
			DownLoadAssetBundleUsk();
		}
		if (!onceItem && UskManager.Instance.bLoaded)
		{
			onceItem = true;
			DownLoadAssetBundleSounds();
			PimpManager.Instance.Load();
			TItemManager.Instance.LoadAll();
			BrickManager.Instance.ChangeUskDecals();
			LevelUpCompensationManager.Instance.Load();
			MissionLoadManager.Instance.Load();
		}
		if (!onceShop && TItemManager.Instance.IsLoaded)
		{
			onceShop = true;
			ShopManager.Instance.Load();
			BundleManager.Instance.Load();
		}
		deltaTime += Time.deltaTime;
		switch (step)
		{
		case STEP.AUTO_LOGIN:
		case STEP.AUTO_LOGIN_TO_RUNUP:
		case STEP.AUTO_LOGIN_TO_NETMARBLE:
			break;
		case STEP.PUBLISHER:
			if (isFadeIn)
			{
				if (deltaTime > fadeInTime)
				{
					FadeOut();
				}
				else if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
				{
					Bang();
					step = STEP.DEVELOPER;
					FadeIn();
				}
				else
				{
					alpha = Mathf.Lerp(alphaFrom, alphaTo, deltaTime / fadeInTime);
				}
			}
			else if (deltaTime > fadeOutTime)
			{
				step = STEP.DEVELOPER;
				FadeIn();
			}
			else
			{
				alpha = Mathf.Lerp(alphaFrom, alphaTo, deltaTime / fadeOutTime);
			}
			break;
		case STEP.DEVELOPER:
			if (isFadeIn)
			{
				if (deltaTime > fadeInTime)
				{
					FadeOut();
				}
				else if (custom_inputs.Instance.GetButtonDown("K_FIRE1") || custom_inputs.Instance.GetButtonDown("K_MAIN_MENU"))
				{
					Bang();
					FadeOut();
				}
				else
				{
					alpha = Mathf.Lerp(alphaFrom, alphaTo, deltaTime / fadeInTime);
				}
			}
			else if (deltaTime > fadeOutTime)
			{
				MoveNext();
			}
			else
			{
				alpha = Mathf.Lerp(alphaFrom, alphaTo, deltaTime / fadeOutTime);
			}
			break;
		case STEP.MOVIE_PUBLISHER:
			if (VersionTextureManager.Instance.moviePublisher != null && VersionTextureManager.Instance.moviePublisher.audioClip != null)
			{
				if (!VersionTextureManager.Instance.moviePublisher.isPlaying)
				{
					step = STEP.DEVELOPER;
					AudioSource component2 = GetComponent<AudioSource>();
					if (null != component2)
					{
						component2.Stop();
					}
					FadeIn();
				}
				else if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
				{
					step = STEP.DEVELOPER;
					AudioSource component3 = GetComponent<AudioSource>();
					if (null != component3)
					{
						component3.Stop();
					}
					FadeIn();
				}
			}
			else
			{
				step = STEP.DEVELOPER;
				AudioSource component4 = GetComponent<AudioSource>();
				if (null != component4)
				{
					component4.Stop();
				}
				FadeIn();
			}
			break;
		}
	}

	private void MoveNext()
	{
		if (!isProgressStop)
		{
			if (StringMgr.Instance.IsLoaded && WordFilter.Instance.IsLoaded && CommandInterpreter.Instance.IsLoaded && TItemManager.Instance.IsLoaded && VoiceManager.Instance.bLoaded && VoiceManager.Instance.bLoaded2 && ShopManager.Instance.IsLoaded && BundleManager.Instance.IsLoaded)
			{
				if (BuildOption.Instance.Props.ShowGrb)
				{
					if (Application.CanStreamedLevelBeLoaded("Grb"))
					{
						Application.LoadLevel("Grb");
					}
				}
				else if (Application.CanStreamedLevelBeLoaded("Login") && Application.CanStreamedLevelBeLoaded("PlayerInfo") && Application.CanStreamedLevelBeLoaded("BfStart") && Application.CanStreamedLevelBeLoaded("Tos"))
				{
					if (BuildOption.Instance.IsRunup)
					{
						tokens = CommandInterpreter.ExtractValueFromParameterByRunup(WebParam.Instance.Parameters);
						if (tokens == null || tokens.Length <= 0)
						{
							tokens = CommandInterpreter.ExtractValueFromParameterByRunup();
						}
						if (tokens == null)
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
							if (a == "steam" && !SteamManager_o.Instance.LoadSteamDll())
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
			else
			{
				isDownLoading = true;
			}
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

	private void DownLoadAssetBundleSounds()
	{
		int @int = PlayerPrefs.GetInt("BfVoice", -1);
		if (!VoiceManager.Instance.bLoaded)
		{
			string empty = string.Empty;
			int num = 1;
			if (@int == -1)
			{
				empty = BuildOption.Instance.Props.DefaultVoice;
				GlobalVars.Instance.DnVoiceFile = BuildOption.Instance.Props.DefaultVoice;
				num = BuildOption.Instance.Props.DefaultVoiceVer;
			}
			else
			{
				GlobalVars.Instance.DnVoiceFile = BuildOption.Instance.Props.GetLangVoc(@int);
				empty = BuildOption.Instance.Props.GetLangVoc(@int);
				empty += ".unity3d";
				num = BuildOption.Instance.Props.GetLangVoiceVer(@int);
			}
			AssetBundleLoadManager.Instance.load(AssetBundleLoadManager.ASS_BUNDLE_TYPE.VOICE, empty, num);
			if (BuildOption.Instance.IsNetmarble || BuildOption.Instance.IsDeveloper)
			{
				AssetBundleLoadManager.Instance.load(AssetBundleLoadManager.ASS_BUNDLE_TYPE.VOICE2, "voice_kr_yd_3.unity3d");
			}
			else
			{
				VoiceManager.Instance.bLoaded2 = true;
			}
		}
	}

	private void DownLoadAssetBundleUsk()
	{
		if (!BuildOption.Instance.Props.useUskWeaponTex && !BuildOption.Instance.Props.useUskWeaponIcon && !BuildOption.Instance.Props.useUskDecal && !BuildOption.Instance.Props.useUskMuzzleEff)
		{
			UskManager.Instance.bLoaded = true;
		}
		else
		{
			AssetBundleLoadManager.Instance.load(AssetBundleLoadManager.ASS_BUNDLE_TYPE.USK, BuildOption.Instance.Props.DefaultUskFile);
		}
	}
}
