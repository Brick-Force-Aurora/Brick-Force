using System;
using UnityEngine;

public class Defense : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	public Texture texBeeRed;

	public Texture texBeeBlue;

	public Texture texBee2Red;

	public Texture texBee2Blue;

	public Texture texBomberRed;

	public Texture texBomberBlue;

	public Texture texChampionRed;

	public Texture texChampionBlue;

	public Texture texIntruderRed;

	public Texture texIntruderBlue;

	public Texture2D defenseStateBg;

	public Texture2D blueBg;

	public Texture2D blueGauge;

	public Texture2D redBg;

	public Texture2D redGauge;

	private float redPt = 52f;

	private float bluePt = 57f;

	public ImageFont hpFontPoint;

	public UIFlickerColor flickerRed;

	public UIFlickerColor flickerBlue;

	private float deltaTime;

	private bool delayLoad = true;

	private BattleChat battleChat;

	private LocalController localController;

	private bool bLoaded;

	private float monGenDeltaTime;

	private bool bDelayBattle;

	private float dtBattle;

	private string statusMessage = string.Empty;

	private float statusDelta;

	private float statusMessageLimit = 5f;

	private bool isOutQueue;

	private float dtQueue;

	private string strQueue = string.Empty;

	private Texture2D texQueue;

	private string nameQueue = string.Empty;

	private int dmgQueue;

	private int RedTeamDP;

	private int BlueTeamDP;

	private void ResetDefensePoint()
	{
		RedTeamDP = 0;
		BlueTeamDP = 0;
	}

	public void AddDefensePoint(bool bRed, int point)
	{
		if (bRed)
		{
			RedTeamDP += point;
		}
		else
		{
			BlueTeamDP += point;
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
		SpawnerDesc spawner = BrickManager.Instance.GetSpawner((MyInfoManager.Instance.Slot < 4) ? Brick.SPAWNER_TYPE.RED_TEAM_SPAWNER : Brick.SPAWNER_TYPE.BLUE_TEAM_SPAWNER, MyInfoManager.Instance.Ticket);
		if (spawner != null)
		{
			localController.Spawn(spawner.position, Rot.ToQuaternion(spawner.rotation));
		}
		else
		{
			Debug.LogError("Fail to get spawner ");
			localController.Spawn(BrickManager.Instance.GetRandomSpawnPos(), Rot.ToQuaternion((byte)UnityEngine.Random.Range(0, 4)));
		}
		if (!MyInfoManager.Instance.GetCommonMask(MyInfoManager.COMMON_OPT.DONOT_DEFENSE_GUIDE))
		{
			DefenseGuideDialog defenseGuideDialog = (DefenseGuideDialog)DialogManager.Instance.GetDialogAlways(DialogManager.DIALOG_INDEX.DEFENSE_GUIDE);
			if (defenseGuideDialog != null && !defenseGuideDialog.DontShowThisMessageAgain)
			{
				((DefenseGuideDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.DEFENSE_GUIDE, exclusive: false))?.InitDialog();
			}
		}
		bLoaded = true;
	}

	private void Awake()
	{
	}

	private void Start()
	{
		GlobalVars.Instance.DropedWeaponAllClear();
		GlobalVars.Instance.ApplyAudioSource();
		GlobalVars.Instance.SwitchFlashbang(bVis: false, Vector3.zero);
		InitializeFirstPerson();
		BrickManManager.Instance.OnStart();
		VfxOptimizer.Instance.SetupCamera();
		battleChat = GetComponent<BattleChat>();
		delayLoad = true;
		deltaTime = 0f;
		bDelayBattle = false;
		dtBattle = 0f;
		monGenDeltaTime = 1000f;
		DefenseManager.Instance.init();
		ResetDefensePoint();
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
			MonManager.Instance.Clear();
			DefenseManager.Instance.RedPoint = 0;
			DefenseManager.Instance.BluePoint = 0;
		}
	}

	private float getGaugeValue(int Point)
	{
		if (Point == 0)
		{
			return 0f;
		}
		if (Point > 150)
		{
			Point = 150;
		}
		return (float)Point / 150f;
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn && bLoaded)
		{
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			Rect position = new Rect((float)(Screen.width / 2 - defenseStateBg.width / 2), 0f, (float)defenseStateBg.width, (float)defenseStateBg.height);
			TextureUtil.DrawTexture(position, defenseStateBg, ScaleMode.StretchToFill, alphaBlend: true);
			if (MyInfoManager.Instance.IsRedTeam())
			{
				flickerRed.ResetAddPosition();
				flickerRed.AddPositionX((float)(Screen.width / 2));
				flickerRed.Draw();
			}
			else
			{
				flickerBlue.ResetAddPosition();
				flickerBlue.AddPositionX((float)(Screen.width / 2));
				flickerBlue.Draw();
			}
			Vector2 pos = new Vector2((float)(Screen.width / 2) - redPt, 20f);
			Vector2 pos2 = new Vector2((float)(Screen.width / 2) + bluePt, 20f);
			hpFontPoint.Print(pos, DefenseManager.Instance.CoreLifeRed);
			hpFontPoint.Print(pos2, DefenseManager.Instance.CoreLifeBlue);
			Rect position2 = new Rect((float)(Screen.width / 2 - redBg.width - 10), 40f, (float)redBg.width, (float)redBg.height);
			TextureUtil.DrawTexture(position2, redBg, ScaleMode.StretchToFill, alphaBlend: true);
			Vector2 pos3 = new Vector2((float)(Screen.width / 2 - redBg.width + 6), 53f);
			LabelUtil.TextOut(pos3, DefenseManager.Instance.RedPoint.ToString(), "PointLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			float gaugeValue = getGaugeValue(DefenseManager.Instance.RedPoint);
			if (gaugeValue > 0f)
			{
				float num = 150f - 150f * gaugeValue;
				GUI.BeginGroup(new Rect((float)(Screen.width / 2 - redBg.width + 27) + num, 53f, (float)redGauge.width * gaugeValue, (float)redGauge.height));
				Rect position3 = new Rect(0f, 0f, (float)redGauge.width, (float)redGauge.height);
				TextureUtil.DrawTexture(position3, redGauge, ScaleMode.StretchToFill, alphaBlend: true);
				GUI.EndGroup();
			}
			Rect position4 = new Rect((float)(Screen.width / 2 + 10), 40f, (float)blueBg.width, (float)blueBg.height);
			TextureUtil.DrawTexture(position4, blueBg, ScaleMode.StretchToFill, alphaBlend: true);
			Vector2 pos4 = new Vector2((float)(Screen.width / 2 + blueBg.width - 12), 53f);
			LabelUtil.TextOut(pos4, DefenseManager.Instance.BluePoint.ToString(), "PointLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			float gaugeValue2 = getGaugeValue(DefenseManager.Instance.BluePoint);
			if (gaugeValue2 > 0f)
			{
				GUI.BeginGroup(new Rect((float)(Screen.width / 2 + 11), 53f, (float)blueGauge.width * gaugeValue2, (float)blueGauge.height));
				Rect position5 = new Rect(0f, 0f, (float)blueGauge.width, (float)blueGauge.height);
				TextureUtil.DrawTexture(position5, blueGauge, ScaleMode.StretchToFill, alphaBlend: true);
				GUI.EndGroup();
			}
			if (strQueue.Length > 0)
			{
				Rect position6 = new Rect((float)(Screen.width / 2 - 343), 100f, 686f, 65f);
				GUI.Box(position6, strQueue, "BoxMsgBg");
				GUI.Box(new Rect((float)(Screen.width - 200), 0f, 200f, 104f), string.Empty, "BoxMoninfoBg");
				TextureUtil.DrawTexture(new Rect((float)(Screen.width - 190), 3f, 80f, 98f), texQueue, ScaleMode.StretchToFill, alphaBlend: true);
				Color byteColor2FloatColor = GlobalVars.Instance.GetByteColor2FloatColor(242, 167, 25);
				LabelUtil.TextOut(new Vector2((float)(Screen.width - 110), 20f), StringMgr.Instance.Get("NAME"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
				LabelUtil.TextOut(new Vector2((float)(Screen.width - 100), 40f), nameQueue, "MiniLabel", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
				LabelUtil.TextOut(new Vector2((float)(Screen.width - 110), 60f), StringMgr.Instance.Get("NUCLEAR_DAMAGE"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
				LabelUtil.TextOut(new Vector2((float)(Screen.width - 100), 80f), dmgQueue.ToString(), "MiniLabel", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
			}
			if (MyInfoManager.Instance.CheckControllable() && !bDelayBattle && !MyInfoManager.Instance.BreakingInto)
			{
				float num2 = DefenseManager.Instance.GetWaveTable().interval - dtBattle;
				int num3 = (int)num2;
				Rect position7 = new Rect((float)(Screen.width / 2 - 343), 100f, 686f, 65f);
				GUI.Box(position7, StringMgr.Instance.Get("MON_GEN_WAIT") + " : " + num3.ToString(), "BoxMsgBg");
			}
			if (statusMessage.Length > 0)
			{
				float a = 1f;
				float num4 = (statusDelta - (statusMessageLimit - 2f)) / 2f;
				if (num4 > 0f)
				{
					a = Mathf.Lerp(1f, 0f, num4);
				}
				LabelUtil.TextOut(new Vector2((float)(Screen.width / 2 + 2), (float)(Screen.height / 2 + 32)), statusMessage, "BigLabel", new Color(0f, 0f, 0f, a), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), (float)(Screen.height / 2 + 30)), statusMessage, "BigLabel", new Color(0.91f, 0.6f, 0f, a), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
			GUI.enabled = true;
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
			flickerRed.Update();
			flickerBlue.Update();
			if (MyInfoManager.Instance.CheckControllable() && !bDelayBattle)
			{
				dtBattle += Time.deltaTime;
				if (dtBattle >= DefenseManager.Instance.GetWaveTable().interval)
				{
					bDelayBattle = true;
					DefenseManager.Instance.CurWave++;
				}
			}
			if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master && bDelayBattle)
			{
				monGenDeltaTime += Time.deltaTime;
				if (monGenDeltaTime >= DefenseManager.Instance.GetWaveTable().interval)
				{
					MonManager.Instance.MonGenerateNew();
					monGenDeltaTime = 0f;
				}
			}
			if (MonManager.Instance.BossUiQ.Count > 0 && !isOutQueue)
			{
				BossUiInfo bossUiInfo = MonManager.Instance.BossUiQ.Dequeue();
				strQueue = bossUiInfo.msg;
				texQueue = bossUiInfo.tex2d;
				nameQueue = bossUiInfo.name;
				dmgQueue = bossUiInfo.dmg;
				isOutQueue = true;
				dtQueue = 0f;
			}
			if (isOutQueue)
			{
				dtQueue += Time.deltaTime;
				if (dtQueue > 5f)
				{
					strQueue = string.Empty;
					isOutQueue = false;
				}
			}
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
		}
		GlobalVars.Instance.UpdateFlashbang();
	}
}
