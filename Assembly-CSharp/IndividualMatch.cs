using System;
using UnityEngine;

public class IndividualMatch : MonoBehaviour
{
	private float deltaTime;

	private bool delayLoad = true;

	private BattleChat battleChat;

	private LocalController localController;

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
		GameObject gameObject = GameObject.Find("Me");
		if (null == gameObject)
		{
			Debug.LogError("Fail to find Me");
		}
		else
		{
			EquipCoordinator component = gameObject.GetComponent<EquipCoordinator>();
			if (null == component)
			{
				Debug.LogError("Fail to get EquipCoordinator component for Me");
			}
			else
			{
				component.Initialize(array);
			}
			localController = gameObject.GetComponent<LocalController>();
			if (null == localController)
			{
				Debug.LogError("Fail to get LocalController component for Me");
			}
		}
	}

	private void OnLoadComplete()
	{
		TrainManager.Instance.Load();
		SpawnerDesc spawner = BrickManager.Instance.GetSpawner(Brick.SPAWNER_TYPE.SINGLE_SPAWNER, MyInfoManager.Instance.Ticket);
		if (spawner != null)
		{
			localController.Spawn(spawner.position, Rot.ToQuaternion(spawner.rotation));
		}
		else
		{
			localController.Spawn(BrickManager.Instance.GetRandomSpawnPos(), Rot.ToQuaternion((byte)UnityEngine.Random.Range(0, 4)));
		}
		if (!MyInfoManager.Instance.GetCommonMask(MyInfoManager.COMMON_OPT.DONOT_BATTLE_GUIDE))
		{
			BattleGuideDialog battleGuideDialog = (BattleGuideDialog)DialogManager.Instance.GetDialogAlways(DialogManager.DIALOG_INDEX.BATTLE_GUIDE);
			if (battleGuideDialog != null && !battleGuideDialog.DontShowThisMessageAgain)
			{
				((BattleGuideDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.BATTLE_GUIDE, exclusive: false))?.InitDialog();
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
		WantedManager.Instance.ResetGameStuff();
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
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
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
}
