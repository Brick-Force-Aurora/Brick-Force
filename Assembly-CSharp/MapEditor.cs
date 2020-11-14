using System;
using System.Collections.Generic;
using UnityEngine;

public class MapEditor : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	public Texture2D authMark;

	public Texture2D editMark;

	private float deltaTime;

	private bool delayLoad = true;

	private BattleChat battleChat;

	private LocalController localController;

	private bool bLoaded;

	private List<int> loadPlayherList = new List<int>();

	public float waitBoxWidth = 700f;

	public float waitBoxHeight = 100f;

	private void Start()
	{
		GlobalVars.Instance.DropedWeaponAllClear();
		GlobalVars.Instance.ApplyAudioSource();
		GlobalVars.Instance.SwitchFlashbang(bVis: false, Vector3.zero);
		InitializeFirstPerson();
		battleChat = GetComponent<BattleChat>();
		BrickManManager.Instance.OnStart();
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

	private void Awake()
	{
	}

	private void StartLoad()
	{
		GC.Collect();
		BrickManager.Instance.userMap = new UserMap();
		CSNetManager.Instance.Sock.SendCS_CACHE_BRICK_REQ();
	}

	private void OnGUI()
	{
		if (bLoaded && MyInfoManager.Instance.isGuiOn)
		{
			GUISkin skin = GUI.skin;
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			GUI.DrawTexture(new Rect(10f, 10f, 32f, 32f), authMark);
			GUI.DrawTexture(new Rect(45f, 10f, 32f, 32f), editMark);
			GUI.enabled = true;
			GUI.skin = skin;
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
			EquipCoordinator component = gameObject.GetComponent<EquipCoordinator>();
			if (null == component)
			{
				Debug.LogError("Fail to get EquipCoordinator component for Me");
			}
			else
			{
				component.Initialize(usables);
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
		CSNetManager.Instance.Sock.SendCS_RESUME_ROOM_REQ(2);
		localController.Spawn(BrickManager.Instance.GetRandomSpawnPos(), Rot.ToQuaternion((byte)UnityEngine.Random.Range(0, 4)));
		bLoaded = true;
		if (!MyInfoManager.Instance.GetCommonMask(MyInfoManager.COMMON_OPT.DONOT_MAPEDIT_GUIDE))
		{
			MapEditGuideDialog mapEditGuideDialog = (MapEditGuideDialog)DialogManager.Instance.GetDialogAlways(DialogManager.DIALOG_INDEX.BUILD_GUIDE);
			if (mapEditGuideDialog != null && !mapEditGuideDialog.DontShowThisMessageAgain)
			{
				((MapEditGuideDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.BUILD_GUIDE, exclusive: false))?.InitDialog();
			}
		}
	}

	private void ResetGameStuff()
	{
		MyInfoManager.Instance.ResetGameStuff();
	}

	private void OnDisable()
	{
		if (Application.isLoadingLevel)
		{
			ResetGameStuff();
			Screen.lockCursor = false;
			UserMapInfoManager.Instance.CurSlot = byte.MaxValue;
			BrickManager.Instance.Clear();
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
			if (!battleChat.IsChatting && BrickManager.Instance.IsLoaded && custom_inputs.Instance.GetButtonDown("K_MAIN_MENU") && GlobalVars.Instance.IsMenuExOpenOk())
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
			if (!battleChat.IsChatting && DialogManager.Instance.IsPopup(DialogManager.DIALOG_INDEX.MAPEDIT_AUTHORITY) && Input.GetKeyDown(KeyCode.Escape))
			{
				GlobalVars.Instance.SetForceClosed(set: true);
				DialogManager.Instance.CloseAll();
			}
			if (!battleChat.IsChatting && custom_inputs.Instance.GetButtonDown("K_MAP_AUTH"))
			{
				if (DialogManager.Instance.IsPopup(DialogManager.DIALOG_INDEX.MAPEDIT_AUTHORITY))
				{
					DialogManager.Instance.CloseAll();
				}
				else if (!DialogManager.Instance.IsModal && !PaletteManager.Instance.MenuOn)
				{
					DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.MAPEDIT_AUTHORITY, exclusive: true);
				}
			}
		}
	}

	public void AddLoadPlayer(int seq)
	{
		if (!loadPlayherList.Contains(seq))
		{
			loadPlayherList.Add(seq);
		}
	}

	public void RemoveLoadPlayer(int seq)
	{
		if (loadPlayherList.Contains(seq))
		{
			loadPlayherList.Remove(seq);
		}
	}
}
