using System;
using UnityEngine;

public class ZombieMatch : MonoBehaviour
{
	public enum STEP
	{
		WAITING,
		SET_POSITION,
		ZOMBIE,
		ZOMBIE_PLAY
	}

	private const float roundingTime = 3f;

	private const int maxCountDown4SetPositionPhase = 10;

	public Texture2D zombieWin;

	public Texture2D brickManWin;

	private float deltaTime;

	private bool delayLoad = true;

	private BattleChat battleChat;

	private GameObject me;

	private LocalController localController;

	public static int playTimePerRound = 180;

	private int countDown4SetPositionPhase;

	private float deltaTime4ZombieMatch;

	private float deltaTime4ZombieStatus;

	private STEP step;

	private Color clrMessage = new Color(0.91f, 0.6f, 0f, 1f);

	private int endCode = -1;

	private float deltaTime4EndCode;

	public Rect crdEndCode = new Rect(0f, 0f, 0f, 0f);

	private bool newZombie;

	private float deltaNewZombie;

	private MatchEnder matchEnder;

	public STEP Step => step;

	public bool IsPlaying => step == STEP.ZOMBIE_PLAY;

	private void VerifyMatchEnder()
	{
		if (matchEnder == null)
		{
			matchEnder = GetComponent<MatchEnder>();
		}
	}

	private void InitializeFirstPerson()
	{
		int[] array = null;
		switch (RoomManager.Instance.WeaponOption)
		{
		case 2:
			array = new int[1];
			break;
		case 1:
			array = new int[2]
			{
				0,
				1
			};
			break;
		default:
			array = new int[4]
			{
				0,
				2,
				1,
				3
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

	private void MakeBrickMan()
	{
		int[] array = null;
		switch (RoomManager.Instance.WeaponOption)
		{
		case 2:
			array = new int[1];
			break;
		case 1:
			array = new int[2]
			{
				0,
				1
			};
			break;
		default:
			array = new int[4]
			{
				0,
				2,
				1,
				3
			};
			break;
		}
		if (null != me)
		{
			EquipCoordinator component = me.GetComponent<EquipCoordinator>();
			if (null != component)
			{
				component.ResetWeaponOnly(array);
			}
			if (null != localController)
			{
				localController.Cure();
			}
		}
	}

	private void MakeZombie()
	{
		if (null != localController)
		{
			localController.CancelCannon();
		}
		if (null != localController)
		{
			localController.CancelTrain();
		}
		int[] usables = new int[1];
		if (null != me)
		{
			EquipCoordinator component = me.GetComponent<EquipCoordinator>();
			if (null != component)
			{
				component.ResetWeaponOnly(usables);
			}
			if (null != localController)
			{
				localController.Infect();
			}
		}
	}

	private void OnLoadComplete()
	{
		TrainManager.Instance.Load();
		if (MyInfoManager.Instance.BreakingInto)
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				string text = StringMgr.Instance.Get("WATCHING_USER_CHANGE");
				gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, string.Empty, text));
			}
			GlobalVars.Instance.senseBombInit();
			MyInfoManager.Instance.ControlMode = MyInfoManager.CONTROL_MODE.PLAYING_SPECTATOR;
			localController.ResetGravity();
			GlobalVars.Instance.battleStarting = false;
			CSNetManager.Instance.Sock.SendCS_ZOMBIE_OBSERVER_REQ();
		}
		else
		{
			SpawnerDesc spawner = BrickManager.Instance.GetSpawner(Brick.SPAWNER_TYPE.SINGLE_SPAWNER, MyInfoManager.Instance.Ticket);
			if (spawner != null)
			{
				localController.Spawn(spawner.position, Rot.ToQuaternion(spawner.rotation));
			}
			else
			{
				localController.Spawn(BrickManager.Instance.GetRandomSpawnPos(), Rot.ToQuaternion((byte)UnityEngine.Random.Range(0, 4)));
			}
		}
		if (!MyInfoManager.Instance.GetCommonMask(MyInfoManager.COMMON_OPT.DONOT_ZOMBIE_GUIDE))
		{
			ZombieGuideDialog zombieGuideDialog = (ZombieGuideDialog)DialogManager.Instance.GetDialogAlways(DialogManager.DIALOG_INDEX.ZOMBIE_GUIDE);
			if (zombieGuideDialog != null && !zombieGuideDialog.DontShowThisMessageAgain)
			{
				((ZombieGuideDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.ZOMBIE_GUIDE, exclusive: false))?.InitDialog();
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
		deltaTime4EndCode = 0f;
		deltaTime4ZombieMatch = 0f;
		deltaTime4ZombieStatus = 0f;
		newZombie = false;
		deltaNewZombie = 0f;
		endCode = -1;
		StartWaitingPhase();
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
		ZombieVsHumanManager.Instance.ResetGameStuff();
		MyInfoManager.Instance.ResetGameStuff();
		TrainManager.Instance.UnLoad();
		BrickManDesc[] array = BrickManManager.Instance.ToDescriptorArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ResetGameStuff();
		}
	}

	private void Awake()
	{
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
			string text = string.Empty;
			switch (step)
			{
			case STEP.WAITING:
				text = string.Empty;
				break;
			case STEP.SET_POSITION:
				text = string.Format(StringMgr.Instance.Get("ZOMBIES_WILL_BE_SELECTED_AFTER"), countDown4SetPositionPhase);
				break;
			case STEP.ZOMBIE:
				text = string.Empty;
				break;
			case STEP.ZOMBIE_PLAY:
				text = string.Empty;
				if (newZombie)
				{
					if (deltaNewZombie < 3f)
					{
						text = StringMgr.Instance.Get("YOU_ARE_ZOMBIE");
					}
				}
				else if (deltaTime4ZombieMatch < 3f)
				{
					if (ZombieVsHumanManager.Instance.IsZombie(MyInfoManager.Instance.Seq))
					{
						text = StringMgr.Instance.Get("YOU_ARE_ZOMBIE");
					}
					else if (ZombieVsHumanManager.Instance.IsHuman(MyInfoManager.Instance.Seq))
					{
						text = StringMgr.Instance.Get("YOU_ARE_BRICKMAN");
					}
				}
				break;
			}
			if (null != matchEnder && !matchEnder.IsOverAll)
			{
				if (endCode >= 0)
				{
					Texture2D texture2D = (endCode != 0) ? brickManWin : zombieWin;
					if (null != texture2D)
					{
						crdEndCode.width = (float)texture2D.width;
						crdEndCode.height = (float)texture2D.height;
						crdEndCode.x = (float)((Screen.width - texture2D.width) / 2);
						crdEndCode.y = (float)((Screen.height - texture2D.height) / 2);
						TextureUtil.DrawTexture(crdEndCode, texture2D);
					}
				}
				if (text.Length > 0)
				{
					LabelUtil.TextOut(new Vector2((float)(Screen.width / 2 + 2), (float)(Screen.height / 2 + 32)), text, "BigLabel", clrMessage, Color.black, TextAnchor.MiddleCenter);
				}
			}
		}
	}

	private void Update()
	{
		VerifyMatchEnder();
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
			UpdateEndCode();
			UpdateZombieStatusNoti();
			UpdateZombieMatch();
			UpdateNewZombieMsg();
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
	}

	public void SetRoundResult(sbyte endCode4BrickMan, sbyte endCode4Zombie, sbyte roundCode)
	{
		if (endCode4Zombie > 0 && endCode4BrickMan < 0)
		{
			endCode = 0;
		}
		else if (endCode4BrickMan > 0 && endCode4Zombie < 0)
		{
			endCode = 1;
		}
		else
		{
			endCode = -1;
		}
		deltaTime4EndCode = 0f;
	}

	private void OnRoundEnd(int ticket)
	{
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("VerifyRoundingWaitTime", 3f);
		}
		StartWaitingPhase();
	}

	private void OnGetBack2Spawner()
	{
		ZombieVsHumanManager.Instance.ResetGameStuff();
	}

	private void UpdateEndCode()
	{
		if (endCode >= 0)
		{
			deltaTime4EndCode += Time.deltaTime;
			if (deltaTime4EndCode > 3f)
			{
				deltaTime4EndCode = 0f;
				endCode = -1;
			}
		}
	}

	private void UpdateZombieStatusNoti()
	{
		deltaTime4ZombieStatus += Time.deltaTime;
		if (deltaTime4ZombieStatus > 1f && MyInfoManager.Instance.Seq == RoomManager.Instance.Master)
		{
            deltaTime4ZombieStatus = 0f;
			CSNetManager.Instance.Sock.SendCS_ZOMBIE_STATUS_REQ((int)step, (int)deltaTime4ZombieMatch, countDown4SetPositionPhase);
		}
	}

	private void UpdateZombieMatch()
	{
		if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master)
		{
			switch (step)
			{
			case STEP.WAITING:
				break;
			case STEP.ZOMBIE:
				break;
			case STEP.SET_POSITION:
				deltaTime4ZombieMatch += Time.deltaTime;
				if (deltaTime4ZombieMatch > 1f)
				{
					deltaTime4ZombieMatch = 0f;
					countDown4SetPositionPhase--;
					if (countDown4SetPositionPhase <= 0)
					{
						countDown4SetPositionPhase = 0;
						StartZombiePhase();
					}
				}
				break;
			case STEP.ZOMBIE_PLAY:
				deltaTime4ZombieMatch += Time.deltaTime;
				break;
			}
		}
	}

	private void UpdateNewZombieMsg()
	{
		if (step == STEP.ZOMBIE_PLAY && newZombie)
		{
			deltaNewZombie += Time.deltaTime;
			if (deltaNewZombie > 3f)
			{
				newZombie = false;
				deltaNewZombie = 0f;
			}
		}
	}

	private void ClearDeltaTimeAndCounter()
	{
		deltaTime4ZombieMatch = 0f;
		countDown4SetPositionPhase = 10;
	}

	private void StartWaitingPhase()
	{
		ClearDeltaTimeAndCounter();
		step = STEP.WAITING;
	}

	private void StartZombiePhase()
	{
		ClearDeltaTimeAndCounter();
		step = STEP.ZOMBIE;
		if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master)
		{
			CSNetManager.Instance.Sock.SendCS_ZOMBIE_INFECTION_REQ();
		}
	}

	private void StartSetPositionPhase()
	{
		ClearDeltaTimeAndCounter();
		step = STEP.SET_POSITION;
		ZombieVsHumanManager.Instance.ResetGameStuff();
		MakeBrickMan();
	}

	private void StartZombiePlayPhase()
	{
		ClearDeltaTimeAndCounter();
		step = STEP.ZOMBIE_PLAY;
		newZombie = false;
		if (ZombieVsHumanManager.Instance.IsZombie(MyInfoManager.Instance.Seq))
		{
			MakeZombie();
		}
	}

	private void OnInfection(Infection infection)
	{
		if (infection.Host == MyInfoManager.Instance.Seq)
		{
			localController.Heal(0.2f);
		}
		if (infection.NewZombie == MyInfoManager.Instance.Seq && ZombieVsHumanManager.Instance.IsZombie(MyInfoManager.Instance.Seq))
		{
			MakeZombie();
			if (ZombieVsHumanManager.Instance.GetHumanCount() > 0)
			{
				newZombie = true;
				deltaNewZombie = 0f;
			}
		}
	}

	private void OnSelectZombies()
	{
		StartZombiePlayPhase();
	}

	private void OnResume()
	{
		StartSetPositionPhase();
	}

	private void OnMatchRestarted()
	{
		StartSetPositionPhase();
	}

	private void OnZombieStatus(ZombieStatus zs)
	{
		countDown4SetPositionPhase = zs._cntDn;
        deltaTime4ZombieMatch = (float)zs._time;
		step = (STEP)zs._status;
	}
}
