using System;
using UnityEngine;

public class ExplosionMatch : MonoBehaviour
{
	public enum STEP
	{
		NOTHING,
		INSTALL_TRY,
		INSTALLED,
		UNINSTALL_TRY,
		UNINSTALLED,
		BLASTING,
		BLASTED
	}

	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.GAME_CONTROL;

	public Texture2D[] endImage;

	public float endCodeLimit = 5f;

	private bool showRoundMessage = true;

	public GameObject[] coreObjs;

	public GameObject InstalledClockBomb;

	public float statusMessageLimit = 5f;

	public static int playTimePerRound = 180;

	private STEP step;

	private int endCode = -1;

	private float endCodeDelta;

	private float deltaTime;

	private bool delayLoad = true;

	private BattleChat battleChat;

	private GameObject me;

	private LocalController localController;

	private bool bRedTeam;

	private InstalledBomb clockBomb;

	private bool bNextVoice;

	private bool bNextVoiceDelay;

	private string strVoiceName = string.Empty;

	private string strVoiceNameNext = string.Empty;

	private float deltaTimeDelaySound;

	private float deltaTimeDelaySoundMax = 3f;

	private float deltaTimeDelaySoundMaxNext = 3f;

	private float deltaTimeNextDelay;

	private float changedRoundTime;

	private int bombInstaller = -1;

	private int blastTarget = -1;

	private string statusMessage = string.Empty;

	private float statusDelta;

	public bool ShowRoundMessage
	{
		get
		{
			return showRoundMessage;
		}
		set
		{
			showRoundMessage = value;
		}
	}

	public STEP Step
	{
		set
		{
			step = value;
		}
	}

	public int BombInstaller => bombInstaller;

	public int BlastTarget => blastTarget;

	public bool IsInstalled => step != 0 && step != STEP.INSTALL_TRY;

	public bool CanInstall => step == STEP.NOTHING;

	public bool CanUninstall => step == STEP.INSTALLED;

	public bool CanBlast => step == STEP.INSTALLED || step == STEP.UNINSTALL_TRY;

	public bool Blastable => step == STEP.INSTALLED || step == STEP.BLASTING || step == STEP.UNINSTALL_TRY;

	public void Installed(int installer, int target)
	{
		bombInstaller = installer;
		blastTarget = target;
		step = STEP.INSTALLED;
		statusDelta = 0f;
		statusMessage = StringMgr.Instance.Get("CLOCK_BOMB_INSTALLED");
		if (bRedTeam)
		{
			PlaySoundDelay("Boom_EG_Red_4", 0f);
		}
		else
		{
			PlaySoundDelay("Boom_EG_Blue", 0f);
		}
	}

	public void Uninstalled(int Uninstaller)
	{
		step = STEP.UNINSTALLED;
		statusDelta = 0f;
		statusMessage = StringMgr.Instance.Get("CLOCK_BOMB_UNINSTALLED");
		if (bRedTeam)
		{
			PlaySoundDelay("Boom_CG_Red_4", 0f);
		}
		else
		{
			PlaySoundDelay("Boom_CG_Blue14", 0f);
		}
	}

	public void Blasted()
	{
		step = STEP.BLASTED;
	}

	private void Awake()
	{
	}

	private void PlaySoundDelay(string str, float timeMax)
	{
		if (!bNextVoice && strVoiceName.Length > 0)
		{
			bNextVoice = true;
			strVoiceNameNext = str;
			deltaTimeDelaySoundMaxNext = timeMax;
		}
		else if (timeMax < 0.01f)
		{
			VoiceManager.Instance.Play(str);
		}
		else
		{
			deltaTimeDelaySoundMax = timeMax;
			deltaTimeDelaySound = 0f;
			strVoiceName = str;
		}
	}

	public void SetRoundResult(sbyte endCode4Red, sbyte endCode4Blue, sbyte roundCode)
	{
		if (endCode4Red > 0 && endCode4Blue < 0)
		{
			endCode = 0;
		}
		else if (endCode4Red < 0 && endCode4Blue > 0)
		{
			endCode = 1;
		}
		else
		{
			endCode = -1;
		}
		showRoundMessage = true;
		endCodeDelta = 0f;
		if (roundCode == 0)
		{
			if (bRedTeam)
			{
				PlaySoundDelay("allyannihilation_4", 4f);
			}
			else
			{
				PlaySoundDelay("enemyannihilation_4", 4f);
			}
			PlaySoundDelay("BlueWin_2", 3.5f);
			changedRoundTime = 12.5f;
		}
		else if (roundCode == 1)
		{
			step = STEP.NOTHING;
			if (bRedTeam)
			{
				PlaySoundDelay("enemyannihilation_4", 4f);
			}
			else
			{
				PlaySoundDelay("allyannihilation_4", 4f);
			}
			PlaySoundDelay("RedWin_4", 3.5f);
			changedRoundTime = 12.5f;
		}
		else if (roundCode == 2)
		{
			PlaySoundDelay("BlueWin_2", 3.5f);
			changedRoundTime = 8.5f;
		}
		else if (roundCode == 3)
		{
			PlaySoundDelay("RedWin_4", 3.5f);
			changedRoundTime = 8.5f;
		}
		else if (roundCode == 4)
		{
			PlaySoundDelay("BlueWin_2", 4.5f);
			changedRoundTime = 8.5f;
		}
		if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master)
		{
			OnRoundEnd(MyInfoManager.Instance.Seq);
		}
	}

	private void OnRoundEnd(int ticket)
	{
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("VerifyRoundingWaitTime", changedRoundTime);
		}
	}

	private void OnClanMatchHalfTime(int ticket)
	{
	}

	private void OnGetBack2Spawner()
	{
		clockBomb.HideAway();
		step = STEP.NOTHING;
		bombInstaller = -1;
		blastTarget = -1;
		localController.ResetClockBomb();
		BombFuction componentInChildren = localController.GetComponentInChildren<BombFuction>();
		if (null != componentInChildren)
		{
			componentInChildren.Clear();
		}
	}

	private void OnMatchRestarted()
	{
	}

	private void InitializeFirstPerson()
	{
		int[] array = null;
		switch (RoomManager.Instance.WeaponOption)
		{
		case 2:
			array = new int[2]
			{
				0,
				4
			};
			break;
		case 1:
			array = new int[3]
			{
				0,
				1,
				4
			};
			break;
		default:
			array = new int[5]
			{
				0,
				2,
				1,
				3,
				4
			};
			break;
		}
		me = GameObject.Find("Me");
		if (null == me)
		{
			Debug.LogError("Fail to find Me");
		}
		else
		{
			EquipCoordinator component = me.GetComponent<EquipCoordinator>();
			if (null == component)
			{
				Debug.LogError("Fail to get EquipCoordinator component for Me");
			}
			else
			{
				component.Initialize(array);
			}
			localController = me.GetComponent<LocalController>();
			if (null == localController)
			{
				Debug.LogError("Fail to get LocalController component for Me");
			}
		}
	}

	private void OnLoadComplete()
	{
		TrainManager.Instance.Load();
		for (int i = 0; i < 2; i++)
		{
			SpawnerDesc spawner = BrickManager.Instance.userMap.GetSpawner(Brick.SPAWNER_TYPE.BOMB_SPAWNER, i);
			if (spawner == null)
			{
				Debug.LogError("Fail to find BOMB SPAWNER from the geometry ");
			}
			else
			{
				GameObject brickObjectByPos = BrickManager.Instance.GetBrickObjectByPos(spawner.position);
				if (null != brickObjectByPos)
				{
					BrickProperty component = brickObjectByPos.GetComponent<BrickProperty>();
					if (null == component)
					{
						Debug.LogError("<BombObj> get BrickProperty failed..");
					}
					else
					{
						coreObjs[i].transform.position = spawner.position;
						coreObjs[i].transform.rotation = brickObjectByPos.transform.rotation;
						coreObjs[i].GetComponent<BlastTarget>().Spot = component.Seq;
					}
				}
			}
		}
		bool flag = false;
		if (MyInfoManager.Instance.BreakingInto && MyInfoManager.Instance.BlastModeDesc != null)
		{
			if (MyInfoManager.Instance.BlastModeDesc.rounding)
			{
				step = STEP.NOTHING;
				bombInstaller = -1;
				blastTarget = -1;
				flag = false;
			}
			else if (MyInfoManager.Instance.BlastModeDesc.bombInstaller < 0 || MyInfoManager.Instance.BlastModeDesc.blastTarget < 0)
			{
				step = STEP.NOTHING;
				bombInstaller = -1;
				blastTarget = -1;
				flag = true;
			}
			else
			{
				step = STEP.INSTALLED;
				bombInstaller = MyInfoManager.Instance.BlastModeDesc.bombInstaller;
				blastTarget = MyInfoManager.Instance.BlastModeDesc.blastTarget;
				clockBomb.Install(MyInfoManager.Instance.BlastModeDesc.point, MyInfoManager.Instance.BlastModeDesc.normal);
				flag = true;
			}
			MyInfoManager.Instance.BlastModeDesc = null;
		}
		if (flag)
		{
			GlobalVars.Instance.battleStarting = false;
			GlobalVars.Instance.senseBombInit();
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				string text = StringMgr.Instance.Get("WATCHING_USER_CHANGE");
				gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, string.Empty, text));
			}
			MyInfoManager.Instance.ControlMode = MyInfoManager.CONTROL_MODE.PLAYING_SPECTATOR;
			localController.ResetGravity();
		}
		else
		{
			SpawnerDesc spawner2 = BrickManager.Instance.GetSpawner(MyInfoManager.Instance.GetTeamSpawnerType(), MyInfoManager.Instance.Ticket);
			if (spawner2 != null)
			{
				localController.Spawn(spawner2.position, Rot.ToQuaternion(spawner2.rotation));
			}
			else
			{
				localController.Spawn(BrickManager.Instance.GetRandomSpawnPos(), Rot.ToQuaternion((byte)UnityEngine.Random.Range(0, 4)));
			}
		}
		if (bRedTeam)
		{
			if (!MyInfoManager.Instance.GetCommonMask(MyInfoManager.COMMON_OPT.DONOT_EXPLOSION_ATTACK_GUIDE))
			{
				ExplosionAttackGuideDialog explosionAttackGuideDialog = (ExplosionAttackGuideDialog)DialogManager.Instance.GetDialogAlways(DialogManager.DIALOG_INDEX.EXPLOSION_ATTACK_GUIDE);
				if (explosionAttackGuideDialog != null && !explosionAttackGuideDialog.DontShowThisMessageAgain)
				{
					((ExplosionAttackGuideDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.EXPLOSION_ATTACK_GUIDE, exclusive: false))?.InitDialog();
				}
			}
		}
		else if (!MyInfoManager.Instance.GetCommonMask(MyInfoManager.COMMON_OPT.DONOT_EXPLOSION_DEFENCE_GUIDE))
		{
			ExplosionDefenceGuideDialog explosionDefenceGuideDialog = (ExplosionDefenceGuideDialog)DialogManager.Instance.GetDialogAlways(DialogManager.DIALOG_INDEX.EXPLOSION_DEFENCE_GUIDE);
			if (explosionDefenceGuideDialog != null && !explosionDefenceGuideDialog.DontShowThisMessageAgain)
			{
				((ExplosionDefenceGuideDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.EXPLOSION_DEFENCE_GUIDE, exclusive: false))?.InitDialog();
			}
		}
	}

	private void Start()
	{
		GlobalVars.Instance.DropedWeaponAllClear();
		GlobalVars.Instance.ApplyAudioSource();
		GlobalVars.Instance.SwitchFlashbang(bVis: false, Vector3.zero);
		InitializeFirstPerson();
		battleChat = GetComponent<BattleChat>();
		BrickManManager.Instance.OnStart();
		VfxOptimizer.Instance.SetupCamera();
		delayLoad = true;
		deltaTime = 0f;
		clockBomb = InstalledClockBomb.GetComponent<InstalledBomb>();
		endCode = -1;
		endCodeDelta = 0f;
		showRoundMessage = true;
		if (MyInfoManager.Instance.Slot < 8)
		{
			bRedTeam = true;
		}
		else
		{
			bRedTeam = false;
		}
		GameObject gameObject = GameObject.Find("Main");
		if (gameObject != null)
		{
			ShooterTools component = gameObject.GetComponent<ShooterTools>();
			if (component != null)
			{
				component.DoBuff();
			}
		}
	}

	private void StartLoad()
	{
		GC.Collect();
		UserMap userMap = new UserMap();
		if (userMap.Load(RoomManager.Instance.CurMap))
		{
			BrickManager.Instance.userMap = userMap;
		}
		else
		{
			BrickManager.Instance.userMap = new UserMap();
			CSNetManager.Instance.Sock.SendCS_CACHE_BRICK_REQ();
		}
	}

	private void ResetGameStuff()
	{
		MyInfoManager.Instance.ResetGameStuff();
		TrainManager.Instance.UnLoad();
		BrickManDesc[] array = BrickManManager.Instance.ToDescriptorArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ResetGameStuff();
		}
	}

	private void OnDisable()
	{
		if (Application.isLoadingLevel)
		{
			ResetGameStuff();
			Screen.lockCursor = false;
			BrickManager.Instance.Clear();
		}
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn)
		{
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			if (0 <= endCode && endCode < endImage.Length && showRoundMessage)
			{
				endCodeDelta += Time.deltaTime;
				if (endCodeDelta > 0.5f)
				{
					Texture2D texture2D = endImage[endCode];
					GUI.DrawTexture(new Rect((float)((Screen.width - texture2D.width) / 2), (float)((Screen.height - texture2D.height) / 2 - 30), (float)texture2D.width, (float)texture2D.height), texture2D, ScaleMode.StretchToFill);
					if (endCodeDelta > endCodeLimit)
					{
						endCodeDelta = 0f;
						endCode = -1;
					}
				}
			}
			if (statusMessage.Length > 0)
			{
				float a = 1f;
				float num = (statusDelta - (statusMessageLimit - 2f)) / 2f;
				if (num > 0f)
				{
					a = Mathf.Lerp(1f, 0f, num);
				}
				LabelUtil.TextOut(new Vector2((float)(Screen.width / 2 + 2), (float)(Screen.height / 2 + 32)), statusMessage, "BigLabel", new Color(0f, 0f, 0f, a), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), (float)(Screen.height / 2 + 30)), statusMessage, "BigLabel", new Color(0.91f, 0.6f, 0f, a), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
			GUI.enabled = true;
		}
	}

	private void UpdateSound()
	{
		if (strVoiceName.Length > 0)
		{
			deltaTimeDelaySound += Time.deltaTime;
			if (deltaTimeDelaySound > deltaTimeDelaySoundMax)
			{
				VoiceManager.Instance.Play(strVoiceName);
				strVoiceName = string.Empty;
				if (bNextVoice)
				{
					bNextVoiceDelay = true;
					deltaTimeNextDelay = 0f;
				}
			}
		}
		if (bNextVoiceDelay)
		{
			deltaTimeNextDelay += Time.deltaTime;
			if (deltaTimeNextDelay > 0.2f)
			{
				bNextVoiceDelay = false;
				deltaTimeDelaySound = 0f;
				bNextVoice = false;
				strVoiceName = strVoiceNameNext;
				deltaTimeDelaySoundMax = deltaTimeDelaySoundMaxNext;
				strVoiceNameNext = string.Empty;
				deltaTimeDelaySoundMaxNext = 0f;
			}
		}
	}

	private void Update()
	{
		bool flag = false;
		Connecting component = GetComponent<Connecting>();
		if (null != component)
		{
			flag = component.Show;
		}
		Screen.lockCursor = (!Application.isLoadingLevel && !battleChat.IsChatting && !DialogManager.Instance.IsModal && !flag);
		if (delayLoad)
		{
			deltaTime += Time.deltaTime;
			if (deltaTime > 1f)
			{
				delayLoad = false;
				StartLoad();
			}
		}
		else if (!Application.isLoadingLevel)
		{
			if (statusMessage.Length > 0)
			{
				statusDelta += Time.deltaTime;
				if (statusDelta > statusMessageLimit)
				{
					statusDelta = 0f;
					statusMessage = string.Empty;
				}
			}
			if (!battleChat.IsChatting && BrickManager.Instance.IsLoaded && custom_inputs.Instance.GetButtonDown("K_MAIN_MENU") && !DialogManager.Instance.IsPopup(DialogManager.DIALOG_INDEX.MENU_EX) && GlobalVars.Instance.IsMenuExOpenOk())
			{
				((MenuEx)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.MENU_EX, exclusive: true))?.InitDialog();
				if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master)
				{
					BrickManDesc[] array = BrickManManager.Instance.ToDescriptorArrayWhoTookTooLongToWait();
					if (array != null && array.Length > 0)
					{
						DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.KICK, exclusive: false);
					}
				}
			}
			if (custom_inputs.Instance.GetButtonDown("K_HELP") && !DialogManager.Instance.IsModal && GlobalVars.Instance.IsMenuExOpenOk() && !battleChat.IsChatting)
			{
				DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.HELPWINDOW, exclusive: true);
			}
			GlobalVars.Instance.UpdateFlashbang();
		}
		UpdateSound();
	}
}
