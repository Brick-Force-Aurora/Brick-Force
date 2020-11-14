using System;
using UnityEngine;

public class CaptureTheFlagMatch : MonoBehaviour
{
	public GameObject FlagObj;

	public GameObject[] rbPlanes;

	public GameObject effGetFlag;

	public GameObject effSetFlag;

	public Texture2D position_B;

	public Texture2D position_N;

	public Texture2D position_R;

	public Texture2D flagGauge;

	public Texture2D flagGauge_R;

	public Texture2D flagGauge_B;

	public Texture2D myCatch;

	public AudioClip sndFlagGet;

	public AudioClip sndFlagWarn;

	public AudioClip sndFlagCapture;

	public AudioClip sndFlagFail;

	private GameObject me;

	private string strVoiceName = string.Empty;

	private float deltaTimeDelaySound;

	private float deltaTimeDelaySoundMax = 2f;

	private float deltaTime;

	private bool delayLoad = true;

	private BattleChat battleChat;

	private LocalController localController;

	private float timerWaitNextBattle;

	private float deltaTimeTcp;

	private bool bRounding;

	private int captureTheFlag = -1;

	private bool bLoaded;

	private float dtResetFlagmax = 30f;

	private string statusMessage = string.Empty;

	private float statusDelta;

	private float statusMessageLimit = 5f;

	private bool bPlaySoundWarn;

	private void Awake()
	{
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

	private void OnLoadComplete()
	{
		TrainManager.Instance.Load();
		BrickManManager.Instance.InitFlagVars();
		SpawnerDesc spawner = BrickManager.Instance.GetSpawner(MyInfoManager.Instance.GetTeamSpawnerType(), MyInfoManager.Instance.Ticket);
		if (spawner != null)
		{
			localController.Spawn(spawner.position, Rot.ToQuaternion(spawner.rotation));
		}
		else
		{
			localController.Spawn(BrickManager.Instance.GetRandomSpawnPos(), Rot.ToQuaternion((byte)UnityEngine.Random.Range(0, 4)));
		}
		if (!MyInfoManager.Instance.GetCommonMask(MyInfoManager.COMMON_OPT.DONOT_FLAG_GUIDE))
		{
			FLAGGuideDialog fLAGGuideDialog = (FLAGGuideDialog)DialogManager.Instance.GetDialogAlways(DialogManager.DIALOG_INDEX.FLAG_GUIDE);
			if (fLAGGuideDialog != null && !fLAGGuideDialog.DontShowThisMessageAgain)
			{
				((FLAGGuideDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.FLAG_GUIDE, exclusive: false))?.InitDialog();
			}
		}
		SpawnerDesc spawner2 = BrickManager.Instance.userMap.GetSpawner(Brick.SPAWNER_TYPE.FLAG_SPAWNER, 0);
		FlagObj.transform.position = new Vector3(spawner2.position.x, spawner2.position.y - 0.3f, spawner2.position.z);
		BrickManManager.Instance.vFlag = FlagObj.transform.position;
		rbPlanes[2].transform.position = spawner2.position;
		spawner2 = BrickManager.Instance.userMap.GetSpawner(Brick.SPAWNER_TYPE.RED_FLAG_SPAWNER, 0);
		rbPlanes[0].transform.position = spawner2.position;
		spawner2 = BrickManager.Instance.userMap.GetSpawner(Brick.SPAWNER_TYPE.BLUE_FLAG_SPAWNER, 0);
		rbPlanes[1].transform.position = spawner2.position;
		GameObject brickObjectByPos = BrickManager.Instance.GetBrickObjectByPos(FlagObj.transform.position);
		if (null != brickObjectByPos)
		{
			BrickProperty component = brickObjectByPos.GetComponent<BrickProperty>();
			if (null == component)
			{
				Debug.LogError("<FlagObj> get BrickProperty failed..");
				return;
			}
			captureTheFlag = component.Seq;
		}
		bLoaded = true;
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
		if (MyInfoManager.Instance.isGuiOn && bLoaded)
		{
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			TextureUtil.DrawTexture(new Rect((float)((Screen.width - 360) / 2), 50f, 360f, 23f), flagGauge);
			Vector3 position = rbPlanes[0].transform.position;
			Vector3 position2 = rbPlanes[1].transform.position;
			Vector3 position3 = FlagObj.transform.position;
			if (MyInfoManager.Instance.Seq == BrickManManager.Instance.haveFlagSeq)
			{
				position3 = me.transform.position;
			}
			float num = Vector3.Distance(position, position3);
			float num2 = Vector3.Distance(position2, position3);
			float num3 = num + num2;
			float num4 = num / num3;
			if (bRounding)
			{
				num4 = 0.5f;
			}
			int num5 = (int)(360f * num4);
			int num6 = (Screen.width - 360 - 16) / 2 + num5;
			if (BrickManManager.Instance.haveFlagSeq < 0)
			{
				TextureUtil.DrawTexture(new Rect((float)num6, 72f, 16f, 16f), position_N);
			}
			else if (IsRedTeam(BrickManManager.Instance.haveFlagSeq))
			{
				TextureUtil.DrawTexture(new Rect((float)num6, 72f, 16f, 16f), position_R);
			}
			else
			{
				TextureUtil.DrawTexture(new Rect((float)num6, 72f, 16f, 16f), position_B);
			}
			if (num4 < 0.25f)
			{
				TextureUtil.DrawTexture(new Rect((float)((Screen.width - 360) / 2), 50f, 360f, 23f), flagGauge_R);
				if (!IsOurTeam(BrickManManager.Instance.haveFlagSeq) && !IsRedTeam(BrickManManager.Instance.haveFlagSeq) && !bPlaySoundWarn)
				{
					GetComponent<AudioSource>().clip = sndFlagWarn;
					GetComponent<AudioSource>().loop = true;
					GetComponent<AudioSource>().Play();
					bPlaySoundWarn = true;
				}
			}
			else if (num4 > 0.75f)
			{
				TextureUtil.DrawTexture(new Rect((float)((Screen.width - 360) / 2), 50f, 360f, 23f), flagGauge_B);
				if (!IsOurTeam(BrickManManager.Instance.haveFlagSeq) && IsRedTeam(BrickManManager.Instance.haveFlagSeq) && !bPlaySoundWarn)
				{
					GetComponent<AudioSource>().clip = sndFlagWarn;
					GetComponent<AudioSource>().loop = true;
					GetComponent<AudioSource>().Play();
					bPlaySoundWarn = true;
				}
			}
			else if (bPlaySoundWarn)
			{
				bPlaySoundWarn = false;
				GetComponent<AudioSource>().Stop();
			}
			if (statusMessage.Length > 0)
			{
				float a = 1f;
				float num7 = (statusDelta - (statusMessageLimit - 2f)) / 2f;
				if (num7 > 0f)
				{
					a = Mathf.Lerp(1f, 0f, num7);
				}
				LabelUtil.TextOut(new Vector2((float)(Screen.width / 2 + 2), (float)(Screen.height / 2 + 32)), statusMessage, "BigLabel", new Color(0f, 0f, 0f, a), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), (float)(Screen.height / 2 + 30)), statusMessage, "BigLabel", new Color(0.91f, 0.6f, 0f, a), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
			if (BrickManManager.Instance.haveFlagSeq == MyInfoManager.Instance.Seq)
			{
				TextureUtil.DrawTexture(new Rect(10f, (float)(Screen.height - 340), 83f, 103f), myCatch);
			}
		}
	}

	private void OnGetBack2Spawner()
	{
		BrickManManager.Instance.InitFlagVars();
		if (BrickManager.Instance.userMap != null)
		{
			SpawnerDesc spawner = BrickManager.Instance.userMap.GetSpawner(Brick.SPAWNER_TYPE.FLAG_SPAWNER, 0);
			if (spawner != null && FlagObj != null)
			{
				FlagObj.transform.position = new Vector3(spawner.position.x, spawner.position.y - 0.3f, spawner.position.z);
				BrickManManager.Instance.vFlag = FlagObj.transform.position;
			}
		}
	}

	public void OnMatchRestarted()
	{
		bRounding = false;
		deltaTimeTcp = 0f;
	}

	private bool IsRedTeam(int seq)
	{
		int num = -1;
		if (BrickManManager.Instance.haveFlagSeq == MyInfoManager.Instance.Seq)
		{
			num = MyInfoManager.Instance.Slot;
		}
		else
		{
			BrickManDesc desc = BrickManManager.Instance.GetDesc(BrickManManager.Instance.haveFlagSeq);
			if (desc == null)
			{
				return false;
			}
			num = desc.Slot;
		}
		return num < 8;
	}

	private bool IsOurTeam(int seq)
	{
		if (seq == MyInfoManager.Instance.Seq)
		{
			return true;
		}
		bool flag = MyInfoManager.Instance.Slot < 8;
		BrickManDesc desc = BrickManManager.Instance.GetDesc(seq);
		if (desc == null)
		{
			return false;
		}
		bool flag2 = desc.Slot < 8;
		if (flag == flag2)
		{
			return true;
		}
		return false;
	}

	private void OnDroped()
	{
		if (BrickManManager.Instance.haveFlagSeq == -1)
		{
			SpawnerDesc spawner = BrickManager.Instance.userMap.GetSpawner(Brick.SPAWNER_TYPE.FLAG_SPAWNER, 0);
			if (spawner != null && FlagObj != null)
			{
				FlagObj.transform.position = new Vector3(spawner.position.x, spawner.position.y - 0.3f, spawner.position.z);
				BrickManManager.Instance.vFlag = FlagObj.transform.position;
			}
		}
		else if (BrickManManager.Instance.haveFlagSeq == -2)
		{
			FlagObj.transform.position = BrickManManager.Instance.vFlag;
		}
		if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master)
		{
			localController.checkResetFlag = true;
			localController.dtResetFlag = 0f;
		}
	}

	private void OnReturnBack()
	{
		if (BrickManager.Instance.userMap != null)
		{
			SpawnerDesc spawner = BrickManager.Instance.userMap.GetSpawner(Brick.SPAWNER_TYPE.FLAG_SPAWNER, 0);
			FlagObj.transform.position = new Vector3(spawner.position.x, spawner.position.y - 0.3f, spawner.position.z);
			BrickManManager.Instance.vFlag = FlagObj.transform.position;
			if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master)
			{
				localController.checkResetFlag = false;
			}
		}
	}

	private void OnChangeRoomMaster()
	{
		if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master && BrickManManager.Instance.haveFlagSeq < 0)
		{
			localController.checkResetFlag = true;
			localController.dtResetFlag = 0f;
		}
	}

	private void OnPicked(int seq)
	{
		statusDelta = 0f;
		if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master)
		{
			localController.checkResetFlag = false;
		}
		if (seq == MyInfoManager.Instance.Seq)
		{
			if (localController != null)
			{
				localController.IDidSomething();
			}
			GetComponent<AudioSource>().PlayOneShot(sndFlagGet);
			PlaySoundDelay("Flag_allyget_4", 1.2f);
			statusMessage = string.Format(StringMgr.Instance.Get("PLAYER_GET_FLAG"), MyInfoManager.Instance.Nickname);
		}
		else
		{
			GameObject gameObject = BrickManManager.Instance.Get(seq);
			if (gameObject != null)
			{
				Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i].name.Contains("Bip01 Neck"))
					{
						UnityEngine.Object.Instantiate((UnityEngine.Object)effGetFlag, componentsInChildren[i].position, Quaternion.Euler(0f, 0f, 0f));
					}
				}
				if (IsOurTeam(seq))
				{
					PlaySoundDelay("Flag_allyget_4", 1.2f);
				}
				else
				{
					PlaySoundDelay("Flag_enemyget_4", 1.2f);
				}
				BrickManDesc desc = BrickManManager.Instance.GetDesc(seq);
				statusMessage = string.Format(StringMgr.Instance.Get("PLAYER_GET_FLAG"), desc.Nickname);
			}
		}
	}

	public void OnCaptured()
	{
		if (BrickManager.Instance.userMap != null)
		{
			SpawnerDesc spawnerDesc = (!IsRedTeam(BrickManManager.Instance.haveFlagSeq)) ? BrickManager.Instance.userMap.GetSpawner(Brick.SPAWNER_TYPE.RED_FLAG_SPAWNER, 0) : BrickManager.Instance.userMap.GetSpawner(Brick.SPAWNER_TYPE.BLUE_FLAG_SPAWNER, 0);
			FlagObj.transform.position = new Vector3(spawnerDesc.position.x, spawnerDesc.position.y - 0.3f, spawnerDesc.position.z);
			BrickManManager.Instance.vFlag = FlagObj.transform.position;
			timerWaitNextBattle = 0f;
			UnityEngine.Object.Instantiate((UnityEngine.Object)effSetFlag, FlagObj.transform.position, Quaternion.Euler(0f, 0f, 0f));
			if (IsOurTeam(BrickManManager.Instance.haveFlagSeq))
			{
				GetComponent<AudioSource>().PlayOneShot(sndFlagCapture);
				PlaySoundDelay("Flag_win_3", 2f);
				statusMessage = StringMgr.Instance.Get("OUR_SET_FLAG");
			}
			else
			{
				GetComponent<AudioSource>().PlayOneShot(sndFlagFail);
				PlaySoundDelay("Flag_lose_3", 2f);
				statusMessage = StringMgr.Instance.Get("ENEMY_SET_FLAG");
			}
			localController.checkResetFlag = false;
			statusDelta = 0f;
		}
	}

	private void PlaySoundDelay(string str, float timeMax)
	{
		deltaTimeDelaySoundMax = timeMax;
		deltaTimeDelaySound = 0f;
		strVoiceName = str;
	}

	private void OnLeaved(int seq)
	{
		GameObject gameObject = BrickManManager.Instance.Get(seq);
		if (gameObject != null)
		{
			FlagObj.transform.position = gameObject.transform.position;
			BrickManManager.Instance.vFlag = gameObject.transform.position;
			BrickManManager.Instance.haveFlagSeq = -1;
		}
	}

	private bool UpdateSuccessfulCaptureTheFlag()
	{
		if (!BrickManManager.Instance.bSuccessFlagCapture)
		{
			return false;
		}
		timerWaitNextBattle += Time.deltaTime;
		if (timerWaitNextBattle > 5f)
		{
			timerWaitNextBattle = 0f;
			bRounding = true;
			BrickManManager.Instance.bSuccessFlagCapture = false;
			BrickManManager.Instance.haveFlagSeq = -1;
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				gameObject.BroadcastMessage("OnRoundEnd", MyInfoManager.Instance.Seq);
			}
		}
		return true;
	}

	private void VerifyLocalController()
	{
		me = GameObject.Find("Me");
		if (null != me)
		{
			localController = me.GetComponent<LocalController>();
			if (null == localController)
			{
				Debug.LogError("Fail to get LocalController component for Me");
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
		if (!bRounding && bLoaded)
		{
			VerifyLocalController();
			if (localController.checkResetFlag)
			{
				localController.dtResetFlag += Time.deltaTime;
				if (localController.dtResetFlag > dtResetFlagmax && BrickManager.Instance.userMap != null)
				{
					SpawnerDesc spawner = BrickManager.Instance.userMap.GetSpawner(Brick.SPAWNER_TYPE.FLAG_SPAWNER, 0);
					Vector3 vector = new Vector3(spawner.position.x, spawner.position.y - 0.3f, spawner.position.z);
					CSNetManager.Instance.Sock.SendCS_CTF_FLAG_RETURN_REQ(vector.x, vector.y, vector.z);
					localController.checkResetFlag = false;
				}
			}
			if (strVoiceName.Length > 0)
			{
				deltaTimeDelaySound += Time.deltaTime;
				if (deltaTimeDelaySound > deltaTimeDelaySoundMax)
				{
					VoiceManager.Instance.Play(strVoiceName);
					strVoiceName = string.Empty;
				}
			}
			if (BrickManManager.Instance.bSendTcpCheckOnce)
			{
				deltaTimeTcp += Time.deltaTime;
				if (deltaTimeTcp > 1f)
				{
					BrickManManager.Instance.bSendTcpCheckOnce = false;
				}
			}
			if (!UpdateSuccessfulCaptureTheFlag())
			{
				if (BrickManManager.Instance.haveFlagSeq >= 0)
				{
					if (BrickManManager.Instance.haveFlagSeq == MyInfoManager.Instance.Seq)
					{
						SpawnerDesc spawnerDesc = (MyInfoManager.Instance.Slot >= 8) ? BrickManager.Instance.userMap.GetSpawner(Brick.SPAWNER_TYPE.RED_FLAG_SPAWNER, 0) : BrickManager.Instance.userMap.GetSpawner(Brick.SPAWNER_TYPE.BLUE_FLAG_SPAWNER, 0);
						float num = Vector3.Distance(me.transform.position, spawnerDesc.position);
						int num2 = 0;
						if (IsRedTeam(MyInfoManager.Instance.Seq))
						{
							num2 = 1;
						}
						Vector3 position = rbPlanes[num2].transform.position;
						float y = position.y;
						Vector3 position2 = me.transform.position;
						float num3 = y - position2.y;
						FlagObj.transform.position = new Vector3(0f, -10000f, 0f);
						if (!BrickManManager.Instance.bSendTcpCheckOnce && num3 < 0.7f && num < 3.2f)
						{
							CSNetManager.Instance.Sock.SendCS_CTF_CAPTURE_FLAG_REQ(captureTheFlag, opponent: true);
							BrickManManager.Instance.bSendTcpCheckOnce = true;
							deltaTimeTcp = 0f;
						}
					}
					else
					{
						GameObject gameObject = BrickManManager.Instance.Get(BrickManManager.Instance.haveFlagSeq);
						if ((bool)gameObject)
						{
							Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>();
							int num4 = 0;
							while (true)
							{
								if (num4 >= componentsInChildren.Length)
								{
									return;
								}
								if (componentsInChildren[num4].name.Contains("dummy_water"))
								{
									break;
								}
								num4++;
							}
							FlagObj.transform.position = componentsInChildren[num4].position;
							FlagObj.transform.rotation = gameObject.transform.rotation;
						}
					}
				}
				else if (!localController.IsDead && !BrickManManager.Instance.bSendTcpCheckOnce)
				{
					float num5 = Vector3.Distance(me.transform.position, FlagObj.transform.position);
					Vector3 position3 = FlagObj.transform.position;
					float y2 = position3.y;
					Vector3 position4 = me.transform.position;
					float num6 = y2 - position4.y;
					if (num6 < 0.7f && num5 < 1.8f)
					{
						CSNetManager.Instance.Sock.SendCS_CTF_PICK_FLAG_REQ(captureTheFlag);
						BrickManManager.Instance.bSendTcpCheckOnce = true;
						deltaTimeTcp = 0f;
					}
				}
			}
		}
	}
}
