using UnityEngine;

public class TimerDefense : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	public Vector2 offset;

	public Texture2D bkgnd;

	private int play;

	private float deltaTime;

	private int remain;

	private bool bBigFont;

	private int remain2;

	private int BigNumber = -1;

	private float deltaTimerBigFont;

	private LocalController localController;

	public int TimeLimit
	{
		get
		{
			return remain;
		}
		set
		{
			remain = value;
		}
	}

	public int TimeLimit2
	{
		get
		{
			return remain2;
		}
		set
		{
			remain2 = value;
			bBigFont = true;
			BigNumber = value;
		}
	}

	private void Start()
	{
		play = 0;
		remain = DefenseManager.Instance.totalWaveTime;
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

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn)
		{
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			int num = remain2 / 60;
			int num2 = remain2 % 60;
			if (num == 0 && num2 <= 10)
			{
				if (num2 != BigNumber && !bBigFont)
				{
					BigNumber = num2;
					bBigFont = true;
				}
				if (bBigFont)
				{
					LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), 50f), num.ToString("00") + ":" + num2.ToString("00"), "BigLabel", Color.red, Color.black, TextAnchor.MiddleCenter);
				}
				else
				{
					LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), 50f), num.ToString("00") + ":" + num2.ToString("00"), "Label", Color.red, Color.black, TextAnchor.MiddleCenter);
				}
			}
			else
			{
				if (remain2 == BigNumber)
				{
					bBigFont = true;
				}
				if (bBigFont)
				{
					LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), 50f), num.ToString("00") + ":" + num2.ToString("00"), "BigLabel", Color.white, Color.black, TextAnchor.MiddleCenter);
				}
				else
				{
					LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), 50f), num.ToString("00") + ":" + num2.ToString("00"), "Label", Color.white, Color.black, TextAnchor.MiddleCenter);
				}
			}
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
				remain2--;
				play++;
				if (remain2 < 0)
				{
					remain2 = 0;
				}
				if (play % 60 == 0)
				{
					CSNetManager.Instance.Sock.SendCS_STACK_POINT_REQ();
				}
				if (remain < 0)
				{
					remain = 0;
				}
				else
				{
					CSNetManager.Instance.Sock.SendCS_TIMER_REQ(remain, play);
				}
			}
			if (bBigFont)
			{
				deltaTimerBigFont += Time.deltaTime;
				if (deltaTimerBigFont >= 0.5f)
				{
					bBigFont = false;
					deltaTimerBigFont = 0f;
				}
			}
		}
		else if (bBigFont)
		{
			deltaTimerBigFont += Time.deltaTime;
			if (deltaTimerBigFont >= 0.5f)
			{
				bBigFont = false;
				deltaTimerBigFont = 0f;
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
			remain2--;
		}
	}
}
