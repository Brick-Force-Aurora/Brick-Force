using UnityEngine;

public class BattleTutor : MonoBehaviour
{
	public Texture2D iconHelp;

	private LocalController localController;

	private bool once;

	private float delayTime;

	private TutoInput tutoInput;

	private void Start()
	{
		GlobalVars.Instance.ApplyAudioSource();
		GlobalVars.Instance.SwitchFlashbang(bVis: false, Vector3.zero);
		GlobalVars.Instance.showBrickId = -1;
		GlobalVars.Instance.sys10First = true;
		GlobalVars.Instance.eventBridge = true;
		GlobalVars.Instance.eventGravity = false;
		BrickManager.Instance.LoadTutorMap(GlobalVars.Instance.isLoadBattleTutor);
		VfxOptimizer.Instance.SetupCamera();
		PaletteManager.Instance.PaletteSet(0, 54, 1);
		tutoInput = GameObject.Find("Main").GetComponent<TutoInput>();
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
			else
			{
				SpawnerDesc spawner = BrickManager.Instance.GetSpawner(Brick.SPAWNER_TYPE.SINGLE_SPAWNER, 0);
				if (spawner != null)
				{
					localController.Spawn(spawner.position, Rot.ToQuaternion(spawner.rotation));
				}
				else
				{
					localController.Spawn(BrickManager.Instance.GetRandomSpawnPos(), Rot.ToQuaternion((byte)Random.Range(0, 4)));
				}
			}
		}
	}

	private void OnDisable()
	{
		if (Application.isLoadingLevel)
		{
			Screen.lockCursor = false;
			BrickManager.Instance.Clear();
			PaletteManager.Instance.Switch(on: false);
			GlobalVars.Instance.preWeaponCode = "aaa";
			GlobalVars.Instance.blockDelBrick = false;
		}
	}

	private void OnGUI()
	{
		if (BrickManager.Instance.IsLoaded)
		{
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.enabled = !DialogManager.Instance.IsModal;
			int num = custom_inputs.Instance.KeyIndex("K_HELP");
			string text = string.Format(StringMgr.Instance.Get("HELP_STRING001"), custom_inputs.Instance.InputKey[num].ToString());
			Vector2 vector = LabelUtil.CalcLength("BoxResult", text);
			vector.x += 60f;
			GUI.Box(new Rect((float)Screen.width - vector.x - 10f, 100f, vector.x, 30f), text, "BoxResult");
			TextureUtil.DrawTexture(new Rect((float)Screen.width - vector.x - 10f - (float)(iconHelp.width / 2), (float)(100 - iconHelp.height / 2), (float)iconHelp.width, (float)iconHelp.height), iconHelp);
			if (tutoInput != null)
			{
				tutoInput.drawInputs();
			}
			GUI.enabled = true;
		}
	}

	private void Update()
	{
		Screen.lockCursor = (!Application.isLoadingLevel && !DialogManager.Instance.IsModal && !PaletteManager.Instance.MenuOn);
		delayTime += Time.deltaTime;
		if (!once && delayTime > 3f && BrickManager.Instance.tutorMap != null)
		{
			once = true;
			BrickManager.Instance.MakeSystemMapInstance(BrickManager.SYSTEM_MAP.BATTLE_TUTOR);
			InitializeFirstPerson();
		}
		if (!Application.isLoadingLevel)
		{
			if (custom_inputs.Instance.GetButtonDown("K_MAIN_MENU") && !DialogManager.Instance.IsPopup(DialogManager.DIALOG_INDEX.MENU_EX) && GlobalVars.Instance.IsMenuExOpenOk())
			{
				((MenuEx)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.MENU_EX, exclusive: true))?.InitDialog();
			}
			if (custom_inputs.Instance.GetButtonDown("K_HELP") && !DialogManager.Instance.IsModal && GlobalVars.Instance.IsMenuExOpenOk())
			{
				DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.HELPWINDOW, exclusive: true);
			}
			GlobalVars.Instance.UpdateFlashbang();
		}
	}

	private void OnNoticeCenter(string text)
	{
		SystemInform.Instance.AddMessageCenter(text);
	}
}
