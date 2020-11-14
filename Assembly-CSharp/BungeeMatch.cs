using System;
using System.Collections.Generic;
using UnityEngine;

public class BungeeMatch : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	private float deltaTime;

	private bool delayLoad = true;

	private BattleChat battleChat;

	private LocalController localController;

	private bool bLoaded;

	private List<EffectivePoint> listEffectivePoint = new List<EffectivePoint>();

	private Camera maincam;

	private void Start()
	{
		GlobalVars.Instance.DropedWeaponAllClear();
		GlobalVars.Instance.ApplyAudioSource();
		GlobalVars.Instance.SwitchFlashbang(bVis: false, Vector3.zero);
		GlobalVars.Instance.resetFever(timeover: true);
		InitializeFirstPerson();
		battleChat = GetComponent<BattleChat>();
		BrickManManager.Instance.OnStart();
		delayLoad = true;
		deltaTime = 0f;
		BlackHole component = GameObject.Find("Main").GetComponent<BlackHole>();
		if (component != null)
		{
			component.placeTo(new Vector3(37f, -15f, 37f));
		}
		GameObject gameObject = GameObject.Find("Main");
		if (gameObject != null)
		{
			ShooterTools component2 = gameObject.GetComponent<ShooterTools>();
			if (component2 != null)
			{
				component2.DoBuff();
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
			if (listEffectivePoint.Count > 0)
			{
				GameObject gameObject = GameObject.Find("Main Camera");
				if (null != gameObject)
				{
					maincam = gameObject.GetComponent<Camera>();
				}
				foreach (EffectivePoint item in listEffectivePoint)
				{
					if (maincam != null)
					{
						Vector3 vector = maincam.WorldToViewportPoint(item.position);
						if (vector.z > 0f && 0f < vector.x && vector.x < 1f && 0f < vector.y && vector.y < 1f)
						{
							Vector3 sp = maincam.WorldToScreenPoint(item.position);
							LabelUtil.TextOut(sp, "+p", "BigLabel", item.color, Color.black, TextAnchor.MiddleCenter);
						}
					}
				}
			}
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
		TrainManager.Instance.Load();
		localController.Spawn(BrickManager.Instance.GetRandomSpawnPos(), Rot.ToQuaternion((byte)UnityEngine.Random.Range(0, 4)));
		bLoaded = true;
		if (!MyInfoManager.Instance.GetCommonMask(MyInfoManager.COMMON_OPT.DONOT_BUNGEE_GUIDE))
		{
			BungeeGuideDialog bungeeGuideDialog = (BungeeGuideDialog)DialogManager.Instance.GetDialogAlways(DialogManager.DIALOG_INDEX.BUNGEE_GUIDE);
			if (bungeeGuideDialog != null && !bungeeGuideDialog.DontShowThisMessageAgain)
			{
				((BungeeGuideDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.BUNGEE_GUIDE, exclusive: false))?.InitDialog();
			}
		}
	}

	private void ResetGameStuff()
	{
		MyInfoManager.Instance.ResetGameStuff();
		TrainManager.Instance.UnLoad();
	}

	private void OnDisable()
	{
		if (Application.isLoadingLevel)
		{
			ResetGameStuff();
			Screen.lockCursor = false;
			UserMapInfoManager.Instance.CurSlot = byte.MaxValue;
			BrickManager.Instance.Clear();
			GlobalVars.Instance.resetFever(timeover: true);
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
		}
		if (listEffectivePoint.Count > 0)
		{
			int num = 0;
			while (num < listEffectivePoint.Count)
			{
				listEffectivePoint[num].time += Time.deltaTime;
				if (listEffectivePoint[num].time > 1f)
				{
					listEffectivePoint.RemoveAt(num);
				}
				else
				{
					num++;
				}
			}
		}
	}

	public void OnEffectivePoint(Vector3 pos, float distance)
	{
		EffectivePoint effectivePoint = new EffectivePoint();
		pos.y += 1f;
		effectivePoint.position = pos;
		if (distance < 1f)
		{
			effectivePoint.color = new Color(1f, 0.5f, 0f, 1f);
		}
		else if (distance < 2f)
		{
			effectivePoint.color = Color.yellow;
		}
		else
		{
			if (!(distance < 3f))
			{
				return;
			}
			effectivePoint.color = Color.gray;
		}
		listEffectivePoint.Add(effectivePoint);
	}
}
