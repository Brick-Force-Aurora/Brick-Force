using System;
using UnityEngine;

public class BndMatch : MonoBehaviour
{
	public GameObject bndWallObject;

	private BndWall bndWall;

	private float deltaTime;

	private bool delayLoad = true;

	private BattleChat battleChat;

	private LocalController localController;

	private string[] preUsebles;

	private bool isBuilderMode;

	private Radar radar;

	private BndTimer timer;

	public Texture2D iconBattle;

	public Texture2D iconBuild;

	public Vector2 weaponIcon = new Vector2(-5f, 30f);

	private EquipCoordinator equipCoordinator;

	public bool IsBuildPhase
	{
		get
		{
			if (null == timer)
			{
				return true;
			}
			return timer.IsBuildPhase;
		}
	}

	public bool IsBuilderMode
	{
		get
		{
			return isBuilderMode;
		}
		set
		{
			isBuilderMode = value;
			BrickManager.Instance.ShowTeamSpawners(timer.IsBuildPhase || isBuilderMode);
		}
	}

	public bool AmIUsingBuildGun
	{
		get
		{
			if (null == equipCoordinator)
			{
				return false;
			}
			return equipCoordinator.CurrentWeapon == 4;
		}
	}

	private void InitializeFirstPerson()
	{
		int[] usables = new int[1]
		{
			4
		};
		GameObject gameObject = GameObject.Find("Me");
		if (null == gameObject)
		{
			Debug.LogError("Fail to find Me");
		}
		else
		{
			equipCoordinator = gameObject.GetComponent<EquipCoordinator>();
			if (null == equipCoordinator)
			{
				Debug.LogError("Fail to get EquipCoordinator component for Me");
			}
			else
			{
				equipCoordinator.Initialize(usables);
			}
			localController = gameObject.GetComponent<LocalController>();
			if (null == localController)
			{
				Debug.LogError("Fail to get LocalController component for Me");
			}
		}
	}

	private void ResetBndStatus(bool wallRightNow)
	{
		radar.Show(!timer.IsBuildPhase);
		isBuilderMode = false;
		BrickManager.Instance.ShowTeamSpawners(timer.IsBuildPhase);
		if (timer.IsBuildPhase)
		{
			if (null != bndWall)
			{
				bndWall.Show(wallRightNow);
			}
			int[] usables = new int[1]
			{
				4
			};
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
					component.Reinit(usables);
				}
			}
		}
		else
		{
			if (null != bndWall)
			{
				bndWall.Hide(wallRightNow);
			}
			int[] array = null;
			switch (RoomManager.Instance.WeaponOption)
			{
			case 2:
				array = ((BuildOption.Instance.AllowBuildGunInDestroyPhase() || !RoomManager.Instance.UseBuildGun) ? new int[2]
				{
					0,
					4
				} : new int[1]);
				break;
			case 1:
				array = ((BuildOption.Instance.AllowBuildGunInDestroyPhase() || !RoomManager.Instance.UseBuildGun) ? new int[3]
				{
					0,
					1,
					4
				} : new int[2]
				{
					0,
					1
				});
				break;
			default:
				array = ((BuildOption.Instance.AllowBuildGunInDestroyPhase() || !RoomManager.Instance.UseBuildGun) ? new int[5]
				{
					0,
					2,
					1,
					3,
					4
				} : new int[4]
				{
					0,
					2,
					1,
					3
				});
				break;
			}
			GameObject gameObject2 = GameObject.Find("Me");
			if (null == gameObject2)
			{
				Debug.LogError("Fail to find Me");
			}
			else
			{
				EquipCoordinator component2 = gameObject2.GetComponent<EquipCoordinator>();
				if (null == component2)
				{
					Debug.LogError("Fail to get EquipCoordinator component for Me");
				}
				else
				{
					component2.ResetWeaponOnly(array);
				}
			}
		}
	}

	private void OnGetBack2Spawner()
	{
		timer.ResetTimer();
		ResetBndStatus(wallRightNow: false);
		BrickManManager.Instance.Reinit();
	}

	private void OnLoadComplete()
	{
		TrainManager.Instance.Load();
		if (MyInfoManager.Instance.BreakingInto && MyInfoManager.Instance.BndModeDesc != null)
		{
			timer.IsBuildPhase = MyInfoManager.Instance.BndModeDesc.buildPhase;
			MyInfoManager.Instance.BndModeDesc = null;
		}
		ResetBndStatus(wallRightNow: true);
		SpawnerDesc spawner = BrickManager.Instance.GetSpawner(MyInfoManager.Instance.GetTeamSpawnerType(), MyInfoManager.Instance.Ticket);
		if (spawner != null)
		{
			localController.Spawn(spawner.position, Rot.ToQuaternion(spawner.rotation));
		}
		else
		{
			localController.Spawn(BrickManager.Instance.GetRandomSpawnPos(), Rot.ToQuaternion((byte)UnityEngine.Random.Range(0, 4)));
		}
		if (!MyInfoManager.Instance.GetCommonMask(MyInfoManager.COMMON_OPT.DONOT_BND_GUIDE))
		{
			BNDGuideDialog bNDGuideDialog = (BNDGuideDialog)DialogManager.Instance.GetDialogAlways(DialogManager.DIALOG_INDEX.BND_GUIDE);
			if (bNDGuideDialog != null && !bNDGuideDialog.DontShowThisMessageAgain)
			{
				((BNDGuideDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.BND_GUIDE, exclusive: false))?.InitDialog();
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
		radar = GetComponent<Radar>();
		timer = GetComponent<BndTimer>();
		BrickManManager.Instance.OnStart();
		VfxOptimizer.Instance.SetupCamera();
		delayLoad = true;
		deltaTime = 0f;
		bndWall = bndWallObject.GetComponent<BndWall>();
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
		BrickManager.Instance.userMap = new UserMap();
		CSNetManager.Instance.Sock.SendCS_CACHE_BRICK_REQ();
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
		if (null != timer && !timer.IsBuildPhase && BuildOption.Instance.AllowBuildGunInDestroyPhase() && RoomManager.Instance.UseBuildGun)
		{
			if (AmIUsingBuildGun)
			{
				Rect position = new Rect(GlobalVars.Instance.ScreenRect.width - (float)iconBattle.width + weaponIcon.x, (GlobalVars.Instance.ScreenRect.height - (float)iconBattle.height) / 2f + weaponIcon.y, (float)iconBattle.width, (float)iconBattle.height);
				TextureUtil.DrawTexture(position, iconBattle, ScaleMode.StretchToFill);
			}
			else
			{
				Rect position2 = new Rect(GlobalVars.Instance.ScreenRect.width - (float)iconBuild.width + weaponIcon.x, (GlobalVars.Instance.ScreenRect.height - (float)iconBuild.height) / 2f + weaponIcon.y, (float)iconBuild.width, (float)iconBuild.height);
				TextureUtil.DrawTexture(position2, iconBuild, ScaleMode.StretchToFill);
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
		Screen.lockCursor = (!Application.isLoadingLevel && !PaletteManager.Instance.MenuOn && !battleChat.IsChatting && !DialogManager.Instance.IsModal && !flag);
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
				if (DialogManager.Instance.IsModal || PaletteManager.Instance.MenuOn)
				{
					DialogManager.Instance.CloseAll();
					PaletteManager.Instance.Switch(on: false);
				}
				else
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
			}
			if (custom_inputs.Instance.GetButtonDown("K_HELP") && !DialogManager.Instance.IsModal && GlobalVars.Instance.IsMenuExOpenOk() && !battleChat.IsChatting)
			{
				DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.HELPWINDOW, exclusive: true);
			}
			GlobalVars.Instance.UpdateFlashbang();
		}
	}
}
