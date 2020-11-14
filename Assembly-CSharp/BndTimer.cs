using UnityEngine;

public class BndTimer : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	private Rect rcBndTimer = new Rect(0f, 0f, 172f, 49f);

	private Rect rcBndTimerWithRadar = new Rect(0f, 164f, 172f, 49f);

	private Rect rcBndTimerBg = new Rect(0f, 0f, 172f, 49f);

	private Vector2 crdBndTime = new Vector2(104f, 24f);

	public Texture2D bkgnd;

	private Radar radar;

	private int play;

	private float deltaTime;

	private int remain;

	private LocalController localController;

	private bool isBuildPhase = true;

	private int repeat;

	public bool IsBuildPhase
	{
		get
		{
			return isBuildPhase;
		}
		set
		{
			isBuildPhase = value;
		}
	}

	public int RemainRepeat => repeat;

	public static int PackTimerOption(int build, int battle, int rpt)
	{
		build /= 60;
		battle /= 60;
		return (build << 16) | (battle << 8) | rpt;
	}

	public static int BuildPhaseTime(int timerOpt)
	{
		return (timerOpt >> 16) * 60;
	}

	public static int BattlePhaseTime(int timerOpt)
	{
		int num = 255;
		return ((timerOpt >> 8) & num) * 60;
	}

	public static int Repeat(int timerOpt)
	{
		int num = 255;
		return timerOpt & num;
	}

	public void ResetTimer()
	{
		remain = ((!isBuildPhase) ? BattlePhaseTime(RoomManager.Instance.TimeLimit) : BuildPhaseTime(RoomManager.Instance.TimeLimit));
	}

	public void ShiftPhase(bool buildPhase)
	{
		if (buildPhase)
		{
			repeat--;
		}
		isBuildPhase = buildPhase;
		remain = ((!isBuildPhase) ? BattlePhaseTime(RoomManager.Instance.TimeLimit) : BuildPhaseTime(RoomManager.Instance.TimeLimit));
	}

	private void Start()
	{
		radar = null;
		play = 0;
		isBuildPhase = true;
		remain = BuildPhaseTime(RoomManager.Instance.TimeLimit);
		repeat = Repeat(RoomManager.Instance.TimeLimit);
		GameObject gameObject = GameObject.Find("Me");
		if (null == gameObject)
		{
			Debug.LogError("Fail to find Me");
		}
		else
		{
			localController = gameObject.GetComponent<LocalController>();
			if (null == localController)
			{
				Debug.LogError("Fail to get LocalController component for Me");
			}
		}
	}

	private void VerifyRadar()
	{
		if (radar == null)
		{
			radar = GetComponent<Radar>();
		}
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn)
		{
			VerifyRadar();
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			GUI.BeginGroup((!(radar != null) || !radar.IsVisible) ? rcBndTimer : rcBndTimerWithRadar);
			TextureUtil.DrawTexture(rcBndTimerBg, bkgnd);
			string text = StringMgr.Instance.Get((!isBuildPhase) ? "BATTLE_PHASE" : "BUILD_PHASE");
			int num = remain / 60;
			int num2 = remain % 60;
			LabelUtil.TextOut(crdBndTime, text + " " + num.ToString("00") + ":" + num2.ToString("00"), "Label", Color.yellow, Color.black, TextAnchor.MiddleCenter);
			GUI.EndGroup();
			GUI.enabled = true;
		}
	}

	private void Update()
	{
		if (RoomManager.Instance.Master == MyInfoManager.Instance.Seq && localController != null && localController.Controllable && BrickManager.Instance.IsLoaded)
		{
			deltaTime += Time.deltaTime;
			if (deltaTime > 1f)
			{
				deltaTime = 0f;
				remain--;
				play++;
				if (play % 60 == 0)
				{
					CSNetManager.Instance.Sock.SendCS_STACK_POINT_REQ();
				}
				if (remain < 0)
				{
					remain = 0;
					ShiftPhase(!isBuildPhase);
					CSNetManager.Instance.Sock.SendCS_BND_SHIFT_PHASE_REQ(repeat, isBuildPhase);
				}
				else
				{
					CSNetManager.Instance.Sock.SendCS_TIMER_REQ(remain, play);
				}
			}
		}
	}

	private void OnPlayTime(int playTime)
	{
		if (RoomManager.Instance.Master != MyInfoManager.Instance.Seq && playTime > play)
		{
			play = playTime;
		}
	}

	private void OnTimer(int remainTime)
	{
		if (RoomManager.Instance.Master != MyInfoManager.Instance.Seq && remainTime < remain)
		{
			remain = remainTime;
		}
	}
}
