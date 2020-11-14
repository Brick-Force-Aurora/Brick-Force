using UnityEngine;

public class ZombieTimer : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	public Vector2 offset;

	public Texture2D bkgnd;

	private UIImageSizeChange backChange = new UIImageSizeChange();

	private int play;

	private float dummyDelta;

	private float playDelta;

	private float deltaTime;

	private int remain;

	private LocalController localController;

	private ZombieMatch zombieMatch;

	private float expandArea = 100f;

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

	private void OnGetBack2Spawner()
	{
		remain = RoomManager.Instance.TimeLimit;
	}

	private void Start()
	{
		play = 0;
		playDelta = 0f;
		dummyDelta = 0f;
		remain = RoomManager.Instance.TimeLimit;
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
		zombieMatch = GetComponent<ZombieMatch>();
		backChange.position.x = (float)(bkgnd.width / 2) + expandArea;
		backChange.position.y = (float)(bkgnd.height / 2) + expandArea;
		backChange.texImage = bkgnd;
		backChange.startSize = 1f;
		backChange.AddStep(1.5f, 0.28f);
		backChange.AddStep(1f, 0.28f);
		backChange.repeat = true;
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn)
		{
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			int num = remain / 60;
			int num2 = remain % 60;
			GUI.BeginGroup(new Rect(offset.x - expandArea, offset.y - expandArea, (float)bkgnd.width + expandArea * 2f, (float)bkgnd.height + expandArea * 2f));
			if (remain < 11)
			{
				backChange.Draw();
				LabelUtil.TextOut(new Vector2((float)(bkgnd.width - 10) + expandArea, (float)(bkgnd.height / 2) + expandArea), num.ToString("00") + ":" + num2.ToString("00"), "Label", Color.red, Color.black, TextAnchor.MiddleRight);
			}
			else
			{
				TextureUtil.DrawTexture(new Rect(expandArea, expandArea, (float)bkgnd.width, (float)bkgnd.height), bkgnd);
				LabelUtil.TextOut(new Vector2((float)(bkgnd.width - 10) + expandArea, (float)(bkgnd.height / 2) + expandArea), num.ToString("00") + ":" + num2.ToString("00"), "Label", Color.yellow, Color.black, TextAnchor.MiddleRight);
			}
			GUI.EndGroup();
			GUI.enabled = true;
		}
	}

	private void Update()
	{
		if (RoomManager.Instance.Master == MyInfoManager.Instance.Seq && null != zombieMatch && localController != null && BrickManager.Instance.IsLoaded)
		{
			if (!MyInfoManager.Instance.IsSpectator)
			{
				if (localController.Controllable)
				{
					if (zombieMatch.Step != ZombieMatch.STEP.ZOMBIE_PLAY)
					{
						dummyDelta += Time.deltaTime;
						if (dummyDelta > 1f)
						{
							dummyDelta = 0f;
							CSNetManager.Instance.Sock.SendCS_TIMER_REQ(remain, play);
						}
					}
					else
					{
						playDelta += Time.deltaTime;
						if (playDelta > 1f)
						{
							playDelta = 0f;
							play++;
							if (play % 60 == 0)
							{
								CSNetManager.Instance.Sock.SendCS_STACK_POINT_REQ();
							}
							CSNetManager.Instance.Sock.SendCS_TIMER_REQ(remain, play);
						}
						deltaTime += Time.deltaTime;
						if (deltaTime > 1f)
						{
							deltaTime = 0f;
							remain--;
							if (remain < 0)
							{
								remain = 0;
							}
							else
							{
								CSNetManager.Instance.Sock.SendCS_TIMER_REQ(remain, play);
							}
						}
					}
				}
			}
			else
			{
				playDelta += Time.deltaTime;
				if (playDelta > 1f)
				{
					playDelta = 0f;
					play++;
					if (play % 60 == 0)
					{
						CSNetManager.Instance.Sock.SendCS_STACK_POINT_REQ();
					}
					CSNetManager.Instance.Sock.SendCS_TIMER_REQ(remain, play);
				}
				deltaTime += Time.deltaTime;
				if (deltaTime > 1f)
				{
					deltaTime = 0f;
					remain--;
					if (remain < 0)
					{
						remain = 0;
					}
					else
					{
						CSNetManager.Instance.Sock.SendCS_TIMER_REQ(remain, play);
					}
				}
			}
		}
		backChange.Update();
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
