using UnityEngine;

public class ClanMatchRounding : MonoBehaviour
{
	public enum STEP
	{
		WAIT,
		CHANGED
	}

	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.GAME_CONTROL;

	public GameObject me;

	public Texture2D[] countDigit;

	private int count;

	private bool rounding;

	private STEP step;

	private float deltaTime;

	private LocalController localController;

	private EquipCoordinator equipcoord;

	private float deltaTimeRoundingWait = 5f;

	private float preWaitTime;

	private bool showRoundMessage = true;

	private bool clockbombkill;

	public int Count
	{
		get
		{
			return count;
		}
		set
		{
			count = value;
		}
	}

	public bool Rounding => rounding;

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

	private void Start()
	{
		rounding = false;
		showRoundMessage = true;
		VerifyLocalController();
	}

	private void VerifyLocalController()
	{
		me = GameObject.Find("Me");
		if (null != me)
		{
			localController = me.GetComponent<LocalController>();
			equipcoord = me.GetComponent<EquipCoordinator>();
		}
	}

	private void VerifyRoundingWaitTime(float timeMax)
	{
		deltaTimeRoundingWait = timeMax;
	}

	private void OnRoundEnd(int ticket)
	{
		rounding = true;
		step = STEP.WAIT;
		count = 0;
		deltaTime = 0f;
		preWaitTime = 0f;
		localController.CancelTrain();
		localController.CancelCannon();
		GlobalVars.Instance.DropedWeaponAllClear();
		ZombieVsHumanManager.Instance.ResetGameStuff();
		if (Application.loadedLevelName.Contains("Explosion"))
		{
			GameObject gameObject = GameObject.Find("InstalledClockBomb");
			if (null != gameObject)
			{
				InstalledBomb component = gameObject.GetComponent<InstalledBomb>();
				if (null != component)
				{
					component.StopClockBombSound();
				}
			}
		}
		MyInfoManager.Instance.RoundEnd(ticket);
	}

	private void OnClanMatchHalfTime(int ticket)
	{
		rounding = true;
		step = STEP.WAIT;
		count = 0;
		deltaTime = 0f;
		preWaitTime = 0f;
		clockbombkill = true;
		MyInfoManager.Instance.ClanMatchHalfTime(ticket);
	}

	private void OnGetBack2Spawner()
	{
		step = STEP.CHANGED;
		count = 0;
		deltaTime = 0f;
		SpawnerDesc spawner = BrickManager.Instance.GetSpawner(MyInfoManager.Instance.GetRoundingSpawnerType(), MyInfoManager.Instance.Ticket);
		if (spawner != null)
		{
			VerifyLocalController();
			if (clockbombkill)
			{
				if (equipcoord != null)
				{
					equipcoord.DeleteClcokBomb();
				}
				clockbombkill = false;
			}
			GlobalVars.Instance.DropedWeaponAllClear();
			PaletteManager.Instance.Switch(on: false);
			if (localController != null)
			{
				localController.Respawn(spawner.position, Rot.ToQuaternion(spawner.rotation));
			}
			VoiceManager.Instance.Play("Ingame_Ready_combo_1");
		}
	}

	private void OnMatchRestartCount(int cnt)
	{
		count = cnt;
		deltaTime = 0f;
		GlobalVars.Instance.ResetWheelKey();
		if (count == countDigit.Length - 1)
		{
			VoiceManager.Instance.Play("Ingame_Start_combo_1");
		}
	}

	private void OnMatchRestarted()
	{
		rounding = false;
		MyInfoManager.Instance.MatchRestarted();
	}

	private void Update()
	{
		if (rounding)
		{
			preWaitTime += Time.deltaTime;
			if (RoomManager.Instance.Master == MyInfoManager.Instance.Seq)
			{
				deltaTime += Time.deltaTime;
				switch (step)
				{
				case STEP.WAIT:
					if (deltaTime > deltaTimeRoundingWait)
					{
						MatchEnder component = GetComponent<MatchEnder>();
						if (null == component || !component.IsOverAll)
						{
							CSNetManager.Instance.Sock.SendCS_GET_BACK2SPAWNER_REQ();
							base.transform.gameObject.BroadcastMessage("OnGetBack2Spawner");
						}
					}
					break;
				case STEP.CHANGED:
					if (deltaTime > 1f)
					{
						count++;
						deltaTime = 0f;
						if (count == countDigit.Length - 1)
						{
							VoiceManager.Instance.Play("Ingame_Start_combo_1");
						}
						if (count < countDigit.Length)
						{
							CSNetManager.Instance.Sock.SendCS_MATCH_RESTART_COUNT_REQ(count);
						}
						else
						{
							CSNetManager.Instance.Sock.SendCS_MATCH_RESTARTED_REQ();
							base.transform.gameObject.BroadcastMessage("OnMatchRestarted");
						}
					}
					break;
				}
			}
		}
	}

	private void OnGUI()
	{
		if (rounding && MyInfoManager.Instance.isGuiOn)
		{
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			switch (step)
			{
			case STEP.WAIT:
				if (preWaitTime > 0.5f && showRoundMessage)
				{
					string roundingMessage = RoomManager.Instance.RoundingMessage;
					if (roundingMessage.Length > 0)
					{
						LabelUtil.TextOut(new Vector2((float)(Screen.width / 2 + 2), (float)(Screen.height / 2 + 62)), roundingMessage, "BigLabel", Color.black, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
						LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), (float)(Screen.height / 2 + 60)), roundingMessage, "BigLabel", new Color(0.91f, 0.6f, 0f, 1f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
					}
					string roundingHelp = RoomManager.Instance.RoundingHelp;
					if (roundingHelp.Length > 0)
					{
						LabelUtil.TextOut(new Vector2((float)(Screen.width / 2 + 2), (float)(Screen.height / 2 + 122)), roundingHelp, "BigLabel", Color.black, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
						LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), (float)(Screen.height / 2 + 120)), roundingHelp, "BigLabel", new Color(0.91f, 0.6f, 0f, 1f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
					}
				}
				break;
			case STEP.CHANGED:
				if (0 <= count && count < countDigit.Length)
				{
					Texture2D texture2D = countDigit[count];
					if (null != texture2D)
					{
						TextureUtil.DrawTexture(new Rect((float)((Screen.width - texture2D.width) / 2), (float)((Screen.height - texture2D.height) / 2), (float)texture2D.width, (float)texture2D.height), texture2D);
					}
				}
				break;
			}
			GUI.enabled = true;
		}
	}
}
