using UnityEngine;

public class Wanted : MonoBehaviour
{
	private enum ACTION_STEP
	{
		NONE = -1,
		MOVE_IN,
		PAUSE,
		MOVE_OUT
	}

	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	public Texture2D wantedCheck;

	public Texture2D wantedBg;

	public Texture2D wanted;

	public Texture2D iAmWanted;

	private Rect crdTMWantedList = new Rect(2f, 229f, 204f, 111f);

	private Rect crdIMWantedList = new Rect(2f, 229f, 204f, 88f);

	private Rect crdWantedTitle = new Rect(8f, 237f, 191f, 31f);

	private Vector2 crdWantedText = new Vector2(103f, 252f);

	private Vector2 crdWanted = new Vector2(28f, 285f);

	private Vector2 crdIAmWanted = new Vector2(2f, 350f);

	private Vector2 crdIAmWantedSizeMax = new Vector2(200f, 200f);

	private Vector2 crdIAmWantedSizeMin = new Vector2(100f, 100f);

	private float offset = 24f;

	public float wantedDeltaMax = 0.5f;

	public float wantedDelta = 1f;

	private LocalController localController;

	private Vector2 crdWantedXY = Vector2.zero;

	private Rect crdWantedCheck = new Rect(0f, 0f, 18f, 16f);

	private bool isTeamMatch;

	private Color red = Color.red;

	private Color blue = Color.blue;

	private Color white = Color.white;

	private Color orange = Color.yellow;

	private float moveInMax = 0.2f;

	private float pauseMax = 0.5f;

	private float moveOutMax = 0.2f;

	private ACTION_STEP actionStep = ACTION_STEP.NONE;

	private float actionDelta;

	private Vector2 bgLeft;

	private Vector2 bgCenter;

	private Vector2 bgRight;

	private Vector2 wantedRight;

	private Vector2 wantedCenter;

	private Vector2 wantedLeft;

	private Vector2 bgXY;

	private Vector2 wantedXY;

	private float wantedActionHeight = 164f;

	private void Start()
	{
		wantedDelta = 1f;
		actionStep = ACTION_STEP.NONE;
		TeamMatch component = GetComponent<TeamMatch>();
		if (null != component)
		{
			isTeamMatch = true;
		}
		IndividualMatch component2 = GetComponent<IndividualMatch>();
		if (null != component2)
		{
			isTeamMatch = false;
		}
		red = GlobalVars.Instance.GetByteColor2FloatColor(245, 83, 26);
		blue = GlobalVars.Instance.GetByteColor2FloatColor(25, 159, 237);
		orange = GlobalVars.Instance.GetByteColor2FloatColor(250, 190, 27);
	}

	private void Action()
	{
		actionStep = ACTION_STEP.MOVE_IN;
		wantedDelta = 0f;
		actionDelta = 0f;
		bgLeft = new Vector2((float)(-wantedBg.width), wantedActionHeight);
		bgCenter = new Vector2((float)((Screen.width - wantedBg.width) / 2), wantedActionHeight);
		bgRight = new Vector2((float)Screen.width, wantedActionHeight);
		wantedRight = new Vector2((float)Screen.width, wantedActionHeight);
		wantedCenter = new Vector2((float)((Screen.width - wanted.width) / 2), wantedActionHeight);
		wantedLeft = new Vector2((float)(-wanted.width), wantedActionHeight);
		bgXY = bgLeft;
		wantedXY = wantedRight;
	}

	private void Update()
	{
		VerifyLocalController();
		wantedDelta += Time.deltaTime;
		if (actionStep != ACTION_STEP.NONE)
		{
			actionDelta += Time.deltaTime;
			switch (actionStep)
			{
			case ACTION_STEP.MOVE_IN:
				if (actionDelta > moveInMax)
				{
					actionDelta = 0f;
					actionStep = ACTION_STEP.PAUSE;
				}
				else
				{
					bgXY = Vector2.Lerp(bgLeft, bgCenter, actionDelta / moveInMax);
					wantedXY = Vector2.Lerp(wantedRight, wantedCenter, actionDelta / moveInMax);
				}
				break;
			case ACTION_STEP.PAUSE:
				if (actionDelta > pauseMax)
				{
					actionDelta = 0f;
					actionStep = ACTION_STEP.MOVE_OUT;
				}
				else
				{
					bgXY = bgCenter;
					wantedXY = wantedCenter;
				}
				break;
			case ACTION_STEP.MOVE_OUT:
				if (actionDelta > moveOutMax)
				{
					actionDelta = 0f;
					actionStep = ACTION_STEP.NONE;
				}
				else
				{
					bgXY = Vector2.Lerp(bgCenter, bgRight, actionDelta / moveInMax);
					wantedXY = Vector2.Lerp(wantedCenter, wantedLeft, actionDelta / moveInMax);
				}
				break;
			}
		}
	}

	private void DrawWanted(string nickname, Color color)
	{
		crdWantedCheck.x = crdWantedXY.x - crdWantedCheck.width - 4f;
		crdWantedCheck.y = crdWantedXY.y;
		TextureUtil.DrawTexture(crdWantedCheck, wantedCheck);
		LabelUtil.TextOut(crdWantedXY, nickname, "Label", color, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		crdWantedXY.y += offset;
	}

	private void DrawWantedList()
	{
		int[] array = WantedManager.Instance.ToArray();
		if (array.Length > 0)
		{
			if (isTeamMatch)
			{
				GUI.Box(crdTMWantedList, string.Empty, "BoxMapE");
			}
			else
			{
				GUI.Box(crdIMWantedList, string.Empty, "BoxMapE");
			}
			GUI.Box(crdWantedTitle, string.Empty, "BoxMyInfo");
			LabelUtil.TextOut(crdWantedText, StringMgr.Instance.Get("WANTED_TEXT"), "BigLabel", orange, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			crdWantedXY = crdWanted;
			int num = 0;
			int num2 = (!isTeamMatch) ? 1 : 2;
			int num3 = 0;
			while (num < num2 && num3 < array.Length)
			{
				if (array[num3] == MyInfoManager.Instance.Seq)
				{
					num++;
					DrawWanted(MyInfoManager.Instance.Nickname, (!isTeamMatch) ? white : ((MyInfoManager.Instance.Slot >= 8) ? blue : red));
				}
				else
				{
					BrickManDesc desc = BrickManManager.Instance.GetDesc(array[num3]);
					if (desc != null)
					{
						num++;
						DrawWanted(desc.Nickname, (!isTeamMatch) ? white : ((desc.Slot >= 8) ? blue : red));
					}
				}
				num3++;
			}
		}
	}

	private void DrawIAMWanted()
	{
		if (WantedManager.Instance.IsWanted(MyInfoManager.Instance.Seq))
		{
			Vector2 vector = Vector2.Lerp(crdIAmWantedSizeMax, crdIAmWantedSizeMin, wantedDelta / wantedDeltaMax);
			if (null != iAmWanted)
			{
				TextureUtil.DrawTexture(new Rect(crdIAmWanted.x, crdIAmWanted.y, vector.x, vector.y), iAmWanted, ScaleMode.StretchToFill);
			}
		}
	}

	private void DrawCenterAction()
	{
		if (actionStep != ACTION_STEP.NONE)
		{
			TextureUtil.DrawTexture(new Rect(bgXY.x, bgXY.y, (float)wantedBg.width, (float)wantedBg.height), wantedBg);
			TextureUtil.DrawTexture(new Rect(wantedXY.x, wantedXY.y, (float)wanted.width, (float)wanted.height), wanted);
		}
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn && BuildOption.Instance.Props.UseWanted && RoomManager.Instance.Wanted)
		{
			GUISkin skin = GUI.skin;
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			DrawWantedList();
			DrawIAMWanted();
			DrawCenterAction();
			GUI.enabled = true;
			GUI.skin = skin;
		}
	}

	private void VerifyLocalController()
	{
		if (null == localController)
		{
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
	}

	public void OnSelectWanted(int wanted)
	{
		if (wanted == MyInfoManager.Instance.Seq)
		{
			SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("YOU_ARE_WANTED"));
			Action();
			localController.WantedOn();
		}
		else
		{
			BrickManDesc desc = BrickManManager.Instance.GetDesc(wanted);
			if (desc != null)
			{
				SystemMsgManager.Instance.ShowMessage(string.Format(StringMgr.Instance.Get("WANTED_IS"), desc.Nickname));
			}
		}
	}
}
