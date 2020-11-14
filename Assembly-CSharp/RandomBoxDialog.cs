using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RandomBoxDialog : Dialog
{
	public enum STEP
	{
		READY,
		SHOW_BACK,
		WAIT_INPUT,
		SPIN,
		RESULT,
		SHOW_FRONT,
		WAIT_RETRY
	}

	private float deltaTime;

	private float spinDelta;

	private float flickerDelta;

	private STEP step;

	private Queue<int> qSlot;

	private Dictionary<int, TItem> dicResult;

	private bool fadeIn;

	private int spinIndex;

	private int spinning;

	private string resultCode = string.Empty;

	private int resultRemain;

	private int resultPhase;

	public AudioClip sndSlotOn;

	public AudioClip sndSlotSpin;

	public AudioClip sndSlotResult;

	public AudioClip sndPopup;

	public RandomPackSlot tSlot;

	public Vector2 crdSlotSize = new Vector2(146f, 45f);

	public Vector2 crdSlotLT = new Vector2(0f, 0f);

	public Vector2 crdSlotOffset = new Vector2(23f, 16f);

	public string[] mainTabKey;

	public Texture2D mainIcon;

	private string[] mainTabstrs;

	private int mainTab;

	private int mainTabPre;

	private Vector2[] scrollPosition;

	public Rect crdMainIcon = new Rect(0f, 0f, 0f, 0f);

	public Rect crdBtnClose = new Rect(605f, 10f, 77f, 26f);

	public Rect crdMainTab = new Rect(10f, 50f, 280f, 28f);

	public Rect crdBrickPoint = new Rect(140f, 80f, 187f, 23f);

	public Rect crdTitle = new Rect(140f, 480f, 187f, 23f);

	public Rect crdItemFloor = new Rect(20f, 90f, 351f, 221f);

	public Rect crdTabOutLine = new Rect(10f, 90f, 350f, 300f);

	public Rect crdVisualBox = new Rect(320f, 90f, 350f, 300f);

	public Rect crdStartBtn = new Rect(400f, 340f, 80f, 30f);

	private Rect crdItemBtn = new Rect(40f, 28f, 54f, 51f);

	public Rect crdUdItemBtn = new Rect(3f, 10f, 130f, 71f);

	private Rect crdItems = new Rect(3f, 3f, 122f, 105f);

	public Rect crdItemList = new Rect(30f, 100f, 351f, 221f);

	public Rect crdChipItemBack = new Rect(30f, 100f, 351f, 221f);

	public Vector2 crdItemTitle = new Vector2(20f, 0f);

	public Vector2 crdUdItemTitle = new Vector2(20f, 0f);

	public Vector2 crdItemBrickPoint = new Vector2(20f, 89f);

	public Vector2 crdBrickPointStr = new Vector2(150f, 80f);

	public Vector2 crdTitleStr = new Vector2(150f, 480f);

	public int ud_startx;

	public int ud_starty = 520;

	public float focusTime = 0.3f;

	public float flickerTime = 0.5f;

	public float spinTime = 0.05f;

	public float resultWait = 1f;

	public Tooltip tooltip;

	private int mouseOverBtn = -1;

	private int selectedBtn;

	private string gachaponCode = string.Empty;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.RANDOMBOX;
		mainTabPre = -1;
		mainTab = 0;
		mainTabstrs = new string[mainTabKey.Length];
		scrollPosition = new Vector2[mainTabKey.Length];
		for (int i = 0; i < mainTabKey.Length; i++)
		{
			scrollPosition[i] = Vector2.zero;
			mainTabstrs[i] = StringMgr.Instance.Get(mainTabKey[i]);
		}
		qSlot = new Queue<int>();
		dicResult = new Dictionary<int, TItem>();
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog()
	{
		mainTabPre = -1;
		mainTab = 0;
		selectedBtn = 0;
		gachaponCode = string.Empty;
		SetStep(STEP.READY);
	}

	private void reset()
	{
		gachaponCode = string.Empty;
	}

	private void DoMainTitle()
	{
		Vector2 pos = new Vector2(size.x / 2f, 15f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("TREASURE_CHEST"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
	}

	private void DoRandomPanel()
	{
		GUI.Box(crdVisualBox, string.Empty, "BoxPopLine");
		int num = -1;
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				Rect position = new Rect(crdSlotLT.x + (float)i * (crdSlotSize.x + crdSlotOffset.x), crdSlotLT.y + (float)j * (crdSlotSize.y + crdSlotOffset.y), crdSlotSize.x, crdSlotSize.y);
				GUI.BeginGroup(position);
				TextureUtil.DrawTexture(new Rect(0f, 0f, crdSlotSize.x, crdSlotSize.y), tSlot.bg);
				Texture2D texture2D = null;
				switch (step)
				{
				case STEP.READY:
					texture2D = tSlot.GetTexture2D(mainTab, i, j, 0);
					if (null != texture2D)
					{
						TextureUtil.DrawTexture(new Rect((crdSlotSize.x - (float)texture2D.width) / 2f, (crdSlotSize.y - (float)texture2D.height) / 2f, (float)texture2D.width, (float)texture2D.height), texture2D);
					}
					break;
				case STEP.SHOW_BACK:
					texture2D = tSlot.GetTexture2D(mainTab, i, j, (!qSlot.Contains(j + i * 3)) ? 2 : 0);
					if (null != texture2D)
					{
						TextureUtil.DrawTexture(new Rect((crdSlotSize.x - (float)texture2D.width) / 2f, (crdSlotSize.y - (float)texture2D.height) / 2f, (float)texture2D.width, (float)texture2D.height), texture2D);
					}
					if (qSlot.Count > 0 && qSlot.Peek() == j + i * 3)
					{
						float num8 = Mathf.Lerp(1.5f, 1f, deltaTime * (1f / focusTime));
						texture2D = tSlot.GetTexture2D(mainTab, i, j, 3);
						if (null != texture2D)
						{
							float num9 = (float)texture2D.width * num8;
							float num10 = (float)texture2D.height * num8;
							TextureUtil.DrawTexture(new Rect((crdSlotSize.x - num9) / 2f, (crdSlotSize.y - num10) / 2f, num9, num10), texture2D);
						}
					}
					break;
				case STEP.WAIT_INPUT:
				{
					texture2D = tSlot.GetTexture2D(mainTab, i, j, 2);
					if (null != texture2D)
					{
						TextureUtil.DrawTexture(new Rect((crdSlotSize.x - (float)texture2D.width) / 2f, (crdSlotSize.y - (float)texture2D.height) / 2f, (float)texture2D.width, (float)texture2D.height), texture2D);
					}
					Rect rc = new Rect(base.ClientRect.x + position.x, base.ClientRect.y + position.y, position.width, position.height);
					if (MouseUtil.MouseOver(rc))
					{
						Color white = Color.white;
						white = ((!fadeIn) ? Color.Lerp(Color.white, new Color(1f, 1f, 1f, 0f), deltaTime * (1f / flickerTime)) : Color.Lerp(new Color(1f, 1f, 1f, 0f), Color.white, deltaTime * (1f / flickerTime)));
						Color color = GUI.color;
						GUI.color = white;
						texture2D = tSlot.GetTexture2D(mainTab, i, j, 1);
						if (null != texture2D)
						{
							TextureUtil.DrawTexture(new Rect((crdSlotSize.x - (float)texture2D.width) / 2f, (crdSlotSize.y - (float)texture2D.height) / 2f, (float)texture2D.width, (float)texture2D.height), texture2D);
						}
						GUI.color = color;
						num = j + i * 3;
						if (mouseOverBtn != num)
						{
							fadeIn = true;
							deltaTime = flickerTime;
						}
						if (Input.GetMouseButtonUp(0))
						{
							CSNetManager.Instance.Sock.SendCS_OPEN_RANDOM_BOX_REQ(Convert.ToInt32(gachaponCode));
							SetStep(STEP.SPIN);
							if (i == 0 && j == 0)
							{
								spinIndex = UnityEngine.Random.Range(1, 6);
							}
							else
							{
								spinIndex = j + i * 3;
							}
						}
					}
					break;
				}
				case STEP.SPIN:
					if (spinIndex != j + i * 3)
					{
						texture2D = tSlot.GetTexture2D(mainTab, i, j, 0);
						if (null != texture2D)
						{
							TextureUtil.DrawTexture(new Rect((crdSlotSize.x - (float)texture2D.width) / 2f, (crdSlotSize.y - (float)texture2D.height) / 2f, (float)texture2D.width, (float)texture2D.height), texture2D);
						}
					}
					else
					{
						texture2D = tSlot.GetTexture2D(mainTab, i, j, 2);
						if (null != texture2D)
						{
							int num11 = spinning % 3;
							float num12 = (float)texture2D.height;
							switch (num11)
							{
							case 1:
								num12 *= 0.7f;
								break;
							case 2:
								num12 *= 0.3f;
								break;
							}
							TextureUtil.DrawTexture(new Rect((crdSlotSize.x - (float)texture2D.width) / 2f, (crdSlotSize.y - num12) / 2f, (float)texture2D.width, num12), texture2D);
						}
					}
					break;
				case STEP.RESULT:
					if (spinIndex != j + i * 3)
					{
						texture2D = tSlot.GetTexture2D(mainTab, i, j, 0);
						if (null != texture2D)
						{
							TextureUtil.DrawTexture(new Rect((crdSlotSize.x - (float)texture2D.width) / 2f, (crdSlotSize.y - (float)texture2D.height) / 2f, (float)texture2D.width, (float)texture2D.height), texture2D);
						}
					}
					else
					{
						texture2D = tSlot.GetTexture2D(mainTab, i, j, 4);
						if (null != texture2D)
						{
							TextureUtil.DrawTexture(new Rect((crdSlotSize.x - (float)texture2D.width) / 2f, (crdSlotSize.y - (float)texture2D.height) / 2f, (float)texture2D.width, (float)texture2D.height), texture2D);
						}
						TItem tItem3 = TItemManager.Instance.Get<TItem>(resultCode);
						if (tItem3 != null)
						{
							float num6 = (float)tItem3.CurIcon().width * 0.6f;
							float num7 = (float)tItem3.CurIcon().height * 0.6f;
							TextureUtil.DrawTexture(new Rect((crdSlotSize.x - num6) / 2f, (crdSlotSize.y - num7) / 2f, num6, num7), tItem3.CurIcon(), ScaleMode.StretchToFill);
						}
						Texture2D texture2D3 = tSlot.fx[resultPhase % 2];
						if (null != texture2D3)
						{
							TextureUtil.DrawTexture(new Rect((crdSlotSize.x - (float)texture2D3.width) / 2f, (crdSlotSize.y - (float)texture2D3.height) / 2f, (float)texture2D3.width, (float)texture2D3.height), texture2D3);
						}
					}
					break;
				case STEP.SHOW_FRONT:
					if (spinIndex != j + i * 3)
					{
						texture2D = tSlot.GetTexture2D(mainTab, i, j, ((i != 0 || j != 0) && !qSlot.Contains(j + i * 3)) ? 4 : 0);
						if (qSlot.Count > 0 && qSlot.Peek() == j + i * 3)
						{
							if (null != texture2D)
							{
								int num13 = spinning % 3;
								float num14 = (float)texture2D.height;
								switch (num13)
								{
								case 1:
									num14 *= 0.7f;
									break;
								case 2:
									num14 *= 0.3f;
									break;
								}
								TextureUtil.DrawTexture(new Rect((crdSlotSize.x - (float)texture2D.width) / 2f, (crdSlotSize.y - num14) / 2f, (float)texture2D.width, num14), texture2D);
							}
							TItem tItem4 = (!dicResult.ContainsKey(j + i * 3)) ? null : dicResult[j + i * 3];
							if (tItem4 != null)
							{
								float num15 = Mathf.Lerp(1.5f, 1f, deltaTime * (1f / focusTime));
								texture2D = tItem4.CurIcon();
								if (null != texture2D)
								{
									float num16 = (float)texture2D.width * num15 * 0.6f;
									float num17 = (float)texture2D.height * num15 * 0.6f;
									TextureUtil.DrawTexture(new Rect((crdSlotSize.x - num16) / 2f, (crdSlotSize.y - num17) / 2f, num16, num17), texture2D);
								}
							}
						}
						else
						{
							if (null != texture2D)
							{
								TextureUtil.DrawTexture(new Rect((crdSlotSize.x - (float)texture2D.width) / 2f, (crdSlotSize.y - (float)texture2D.height) / 2f, (float)texture2D.width, (float)texture2D.height), texture2D);
							}
							TItem tItem5 = (!dicResult.ContainsKey(j + i * 3)) ? null : dicResult[j + i * 3];
							if (tItem5 != null && !qSlot.Contains(j + i * 3))
							{
								float num18 = (float)tItem5.CurIcon().width * 0.6f;
								float num19 = (float)tItem5.CurIcon().height * 0.6f;
								TextureUtil.DrawTexture(new Rect((crdSlotSize.x - num18) / 2f, (crdSlotSize.y - num19) / 2f, num18, num19), tItem5.CurIcon(), ScaleMode.StretchToFill);
							}
						}
					}
					else
					{
						texture2D = tSlot.GetTexture2D(mainTab, i, j, 4);
						if (null != texture2D)
						{
							TextureUtil.DrawTexture(new Rect((crdSlotSize.x - (float)texture2D.width) / 2f, (crdSlotSize.y - (float)texture2D.height) / 2f, (float)texture2D.width, (float)texture2D.height), texture2D);
						}
						TItem tItem6 = TItemManager.Instance.Get<TItem>(resultCode);
						if (tItem6 != null)
						{
							float num20 = (float)tItem6.CurIcon().width * 0.6f;
							float num21 = (float)tItem6.CurIcon().height * 0.6f;
							TextureUtil.DrawTexture(new Rect((crdSlotSize.x - num20) / 2f, (crdSlotSize.y - num21) / 2f, num20, num21), tItem6.CurIcon(), ScaleMode.StretchToFill);
						}
						Texture2D texture2D4 = tSlot.fx[resultPhase % 2];
						if (null != texture2D4)
						{
							TextureUtil.DrawTexture(new Rect((crdSlotSize.x - (float)texture2D4.width) / 2f, (crdSlotSize.y - (float)texture2D4.height) / 2f, (float)texture2D4.width, (float)texture2D4.height), texture2D4);
						}
					}
					break;
				case STEP.WAIT_RETRY:
					if (spinIndex != j + i * 3)
					{
						texture2D = tSlot.GetTexture2D(mainTab, i, j, ((i != 0 || j != 0) && !qSlot.Contains(j + i * 3)) ? 4 : 0);
						if (null != texture2D)
						{
							TextureUtil.DrawTexture(new Rect((crdSlotSize.x - (float)texture2D.width) / 2f, (crdSlotSize.y - (float)texture2D.height) / 2f, (float)texture2D.width, (float)texture2D.height), texture2D);
						}
						TItem tItem = (!dicResult.ContainsKey(j + i * 3)) ? null : dicResult[j + i * 3];
						if (tItem != null && !qSlot.Contains(j + i * 3))
						{
							float num2 = (float)tItem.CurIcon().width * 0.6f;
							float num3 = (float)tItem.CurIcon().height * 0.6f;
							TextureUtil.DrawTexture(new Rect((crdSlotSize.x - num2) / 2f, (crdSlotSize.y - num3) / 2f, num2, num3), tItem.CurIcon(), ScaleMode.StretchToFill);
						}
					}
					else
					{
						texture2D = tSlot.GetTexture2D(mainTab, i, j, 4);
						if (null != texture2D)
						{
							TextureUtil.DrawTexture(new Rect((crdSlotSize.x - (float)texture2D.width) / 2f, (crdSlotSize.y - (float)texture2D.height) / 2f, (float)texture2D.width, (float)texture2D.height), texture2D);
						}
						TItem tItem2 = TItemManager.Instance.Get<TItem>(resultCode);
						if (tItem2 != null)
						{
							float num4 = (float)tItem2.CurIcon().width * 0.6f;
							float num5 = (float)tItem2.CurIcon().height * 0.6f;
							TextureUtil.DrawTexture(new Rect((crdSlotSize.x - num4) / 2f, (crdSlotSize.y - num5) / 2f, num4, num5), tItem2.CurIcon(), ScaleMode.StretchToFill);
						}
						Texture2D texture2D2 = tSlot.fx[resultPhase % 2];
						if (null != texture2D2)
						{
							TextureUtil.DrawTexture(new Rect((crdSlotSize.x - (float)texture2D2.width) / 2f, (crdSlotSize.y - (float)texture2D2.height) / 2f, (float)texture2D2.width, (float)texture2D2.height), texture2D2);
						}
					}
					break;
				}
				GUI.EndGroup();
			}
		}
		mouseOverBtn = num;
	}

	private void SetStep(STEP newStep)
	{
		step = newStep;
		deltaTime = 0f;
		spinDelta = 0f;
		flickerDelta = 0f;
		switch (step)
		{
		case STEP.READY:
			break;
		case STEP.WAIT_RETRY:
			break;
		case STEP.SHOW_BACK:
		{
			qSlot.Clear();
			List<int> list3 = new List<int>();
			for (int l = 0; l < 6; l++)
			{
				list3.Add(l);
			}
			for (int m = 0; m < 6; m++)
			{
				int index2 = UnityEngine.Random.Range(0, list3.Count);
				qSlot.Enqueue(list3[index2]);
				list3.RemoveAt(index2);
			}
			GlobalVars.Instance.PlaySound(sndSlotOn);
			break;
		}
		case STEP.WAIT_INPUT:
			fadeIn = false;
			break;
		case STEP.SPIN:
			spinIndex = -1;
			spinning = 0;
			resultCode = string.Empty;
			resultRemain = 0;
			GlobalVars.Instance.PlaySound(sndSlotSpin);
			break;
		case STEP.RESULT:
			resultPhase = 0;
			GlobalVars.Instance.PlaySound(sndSlotResult);
			break;
		case STEP.SHOW_FRONT:
		{
			qSlot.Clear();
			dicResult.Clear();
			c_Gachapon gachaponByCode = RandomboxItemManager.Instance.GetGachaponByCode(gachaponCode);
			if (gachaponByCode == null)
			{
				Debug.LogError("Fail to get gachapon");
			}
			else
			{
				List<int> list = new List<int>();
				for (int i = 1; i < 6; i++)
				{
					if (i != spinIndex)
					{
						list.Add(i);
					}
				}
				List<TItem> list2 = new List<TItem>();
				for (int j = 0; j < gachaponByCode.items.Length; j++)
				{
					if (resultCode != gachaponByCode.items[j])
					{
						TItem item = TItemManager.Instance.Get<TItem>(gachaponByCode.items[j]);
						list2.Add(item);
					}
				}
				for (int k = 0; k < 4; k++)
				{
					int index = UnityEngine.Random.Range(0, list.Count);
					int num = list[index];
					qSlot.Enqueue(num);
					list.RemoveAt(index);
					index = UnityEngine.Random.Range(0, list2.Count);
					dicResult.Add(num, list2[index]);
					list2.RemoveAt(index);
				}
			}
			break;
		}
		}
	}

	public override void Update()
	{
		deltaTime += Time.deltaTime;
		spinDelta += Time.deltaTime;
		flickerDelta += Time.deltaTime;
		switch (step)
		{
		case STEP.READY:
			break;
		case STEP.SHOW_BACK:
			if (deltaTime > focusTime)
			{
				if (qSlot.Count == 0)
				{
					SetStep(STEP.WAIT_INPUT);
				}
				else
				{
					qSlot.Dequeue();
					deltaTime = 0f;
					if (qSlot.Count > 0)
					{
						GlobalVars.Instance.PlaySound(sndSlotOn);
					}
				}
			}
			break;
		case STEP.WAIT_INPUT:
			if (flickerDelta > flickerTime)
			{
				fadeIn = !fadeIn;
				flickerDelta = 0f;
			}
			break;
		case STEP.SPIN:
			if (spinDelta > spinTime)
			{
				spinning++;
				spinDelta = 0f;
			}
			if (spinning > 20 && resultCode.Length > 0)
			{
				TItem tItem = TItemManager.Instance.Get<TItem>(resultCode);
				if (tItem != null)
				{
					SetStep(STEP.RESULT);
				}
			}
			break;
		case STEP.RESULT:
		{
			RandomBoxConfirmDialog randomBoxConfirmDialog = (RandomBoxConfirmDialog)DialogManager.Instance.GetDialogAlways(DialogManager.DIALOG_INDEX.RANDOMBOX_CONFIRM);
			if (randomBoxConfirmDialog != null && randomBoxConfirmDialog.CloseOnce)
			{
				SetStep(STEP.SHOW_FRONT);
			}
			else if (flickerDelta > flickerTime)
			{
				resultPhase++;
				flickerDelta = 0f;
				if (resultPhase == 4)
				{
					GlobalVars.Instance.PlaySound(sndPopup);
					((RandomBoxConfirmDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.RANDOMBOX_CONFIRM, exclusive: false))?.InitDialog(resultCode, resultRemain);
				}
			}
			break;
		}
		case STEP.SHOW_FRONT:
			if (spinDelta > spinTime)
			{
				spinning++;
				spinDelta = 0f;
			}
			if (flickerDelta > flickerTime)
			{
				resultPhase++;
				flickerDelta = 0f;
			}
			if (deltaTime > focusTime)
			{
				if (qSlot.Count == 0)
				{
					SetStep(STEP.WAIT_RETRY);
				}
				else
				{
					qSlot.Dequeue();
					deltaTime = 0f;
				}
			}
			break;
		case STEP.WAIT_RETRY:
			if (flickerDelta > flickerTime)
			{
				resultPhase++;
				flickerDelta = 0f;
			}
			break;
		}
	}

	private bool ChangePackPossible()
	{
		return step == STEP.READY || step == STEP.WAIT_INPUT || step == STEP.SHOW_BACK || step == STEP.WAIT_RETRY;
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		DoMainTitle();
		GUI.Box(crdTabOutLine, string.Empty, "BoxPopLine");
		mainTab = GUI.SelectionGrid(crdMainTab, mainTab, mainTabstrs, mainTabstrs.Length, "popTab");
		if (mainTab != mainTabPre)
		{
			if (!ChangePackPossible())
			{
				mainTab = mainTabPre;
			}
			else
			{
				mainTabPre = mainTab;
				reset();
				SetStep(STEP.READY);
			}
		}
		DoRandomPanel();
		GUI.Box(crdBrickPoint, string.Empty, "BoxFadeBlue");
		string text = string.Format(StringMgr.Instance.Get("CURRENT_TOKEN"), MyInfoManager.Instance.Cash);
		LabelUtil.TextOut(crdBrickPointStr, text, "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		GUI.Box(crdChipItemBack, string.Empty, "BoxInnerLine");
		c_Gachapon[] gachaponsByCat = RandomboxItemManager.Instance.GetGachaponsByCat(mainTab);
		int num = gachaponsByCat.Length;
		int num2 = 2;
		int num3 = num / num2;
		if (num % num2 > 0)
		{
			num3++;
		}
		Vector2 vector = new Vector2(135f, 108f);
		float num4 = vector.x * (float)num2;
		if (num2 > 1)
		{
			num4 += (float)((num2 - 1) * 2);
		}
		float num5 = vector.y * (float)num3;
		if (num3 > 1)
		{
			num5 += (float)((num3 - 1) * 4);
		}
		Rect viewRect = new Rect(0f, 0f, num4, num5);
		scrollPosition[mainTab] = GUI.BeginScrollView(crdItemList, scrollPosition[mainTab], viewRect, alwaysShowHorizontal: false, alwaysShowVertical: false);
		Rect position = new Rect(0f, 0f, vector.x, vector.y);
		int num6 = 0;
		int num7 = 0;
		while (num6 < num && num7 < num3)
		{
			position.y = (float)num7 * (vector.y + 2f);
			int num8 = 0;
			while (num6 < num && num8 < num2)
			{
				position.x = (float)num8 * (vector.x + 2f) + 20f;
				GUI.BeginGroup(position);
				if (num6 == 0 && gachaponCode == string.Empty)
				{
					gachaponCode = gachaponsByCat[num6].code;
					selectedBtn = 0;
				}
				if (GlobalVars.Instance.MyButton(crdItems, string.Empty, "BtnItem") && ChangePackPossible())
				{
					gachaponCode = gachaponsByCat[num6].code;
					selectedBtn = num6;
					SetStep(STEP.READY);
				}
				if (selectedBtn == num6)
				{
					GUI.Box(crdItems, string.Empty, "ViewSelected");
				}
				crdItemTitle.x = (crdItems.x + crdItems.width) / 2f;
				LabelUtil.TextOut(crdItemTitle, StringMgr.Instance.Get(gachaponsByCat[num6].strtblCode), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.LowerCenter);
				string text2 = string.Format(StringMgr.Instance.Get("NUM_TOKEN"), gachaponsByCat[num6].brickPoint, TokenManager.Instance.GetTokenString());
				crdItemBrickPoint.x = (crdItems.x + crdItems.width) / 2f;
				LabelUtil.TextOut(crdItemBrickPoint, text2, "Label", new Color(1f, 1f, 1f), GlobalVars.txtEmptyColor, TextAnchor.LowerCenter);
				if (gachaponsByCat[num6].icon == null)
				{
					Debug.LogError("Fail to get icon for item " + gachaponsByCat[num6].code);
				}
				else
				{
					TextureUtil.DrawTexture(crdItemBtn, gachaponsByCat[num6].icon, ScaleMode.StretchToFill);
				}
				GUI.EndGroup();
				num6++;
				num8++;
			}
			num7++;
		}
		GUI.EndScrollView();
		bool enabled = GUI.enabled;
		GUI.enabled = (step == STEP.READY || step == STEP.WAIT_RETRY);
		if (GlobalVars.Instance.MyButton(crdStartBtn, StringMgr.Instance.Get("START"), "BtnChat") && MyInfoManager.Instance.BrickPoint < gachaponsByCat[selectedBtn].brickPoint)
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NOT_ENOUGH_BRICKPOINT4RANDOMBOX"));
		}
		GUI.enabled = enabled;
		string text3 = string.Format(StringMgr.Instance.Get("COMPOSE_GOODS"));
		LabelUtil.TextOut(crdTitleStr, text3, "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.LowerCenter);
		Rect position2 = new Rect((float)ud_startx, (float)ud_starty, 143f, 112f);
		for (int i = 0; i < 5; i++)
		{
			if (!(gachaponCode == string.Empty))
			{
				int num9 = gachaponsByCat[selectedBtn].qualities[i];
				string key = gachaponsByCat[selectedBtn].items[i];
				TItem tItem = TItemManager.Instance.Get<TItem>(key);
				position2.x = (float)(i * 145 + 13);
				GUI.Button(position2, new GUIContent(string.Empty, key), "InvisibleButton");
				GUI.BeginGroup(position2);
				Rect position3 = new Rect(0f, 0f, 143f, 112f);
				if (num9 == 1)
				{
					GUI.Box(position3, string.Empty, "BoxRareItemBg");
				}
				else
				{
					GUI.Box(position3, string.Empty, "BoxNormalItemBg");
				}
				if (num9 == 1)
				{
					LabelUtil.TextOut(crdUdItemTitle, tItem.Name, "Label", new Color(1f, 1f, 1f), GlobalVars.txtEmptyColor, TextAnchor.LowerCenter);
				}
				else
				{
					LabelUtil.TextOut(crdUdItemTitle, tItem.Name, "Label", new Color(1f, 1f, 1f), GlobalVars.txtEmptyColor, TextAnchor.LowerCenter);
				}
				if (tItem.CurIcon() == null)
				{
					Debug.LogError("Fail to get icon for item " + tItem.CurIcon());
				}
				else
				{
					TextureUtil.DrawTexture(crdUdItemBtn, tItem.CurIcon(), ScaleMode.StretchToFill);
				}
				GUI.EndGroup();
			}
		}
		Rect rc = new Rect(size.x - 50f, 10f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
		}
		DoTooltip();
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}

	private void DoTooltip()
	{
		Dialog top = DialogManager.Instance.GetTop();
		if (GUI.tooltip.Length > 0 && top != null && top.ID == DialogManager.DIALOG_INDEX.RANDOMBOX)
		{
			c_Gachapon gachaponByCode = RandomboxItemManager.Instance.GetGachaponByCode(gachaponCode);
			TItem tItem = TItemManager.Instance.Get<TItem>(GUI.tooltip);
			if (gachaponByCode != null && tItem != null)
			{
				bool flag = false;
				int num = 0;
				while (!flag && num < 5)
				{
					if (gachaponByCode.items[num] == tItem.code)
					{
						flag = true;
						tooltip.ItemCode = tItem.code;
						Vector2 vector = new Vector2((float)(num * 145 + 13), (float)ud_starty);
						Vector2 vector2 = new Vector2(vector.x + 143f, vector.y);
						float x = tooltip.size.x;
						float num2 = tooltip.size.y;
						if (tItem.type != 0)
						{
							num2 -= 130f;
						}
						if (num < 4)
						{
							tooltip.SetCoord(new Vector2(vector.x, vector.y - num2));
						}
						else
						{
							tooltip.SetCoord(new Vector2(vector2.x - x, vector2.y - num2));
						}
					}
					num++;
				}
				if (flag)
				{
					GUI.Box(tooltip.ClientRect, string.Empty, "TooltipWindow");
					GUI.BeginGroup(tooltip.ClientRect);
					tooltip.DoDialog();
					GUI.EndGroup();
				}
			}
		}
	}

	public void OpenRandomBox(string code, int remain)
	{
		resultCode = code;
		resultRemain = remain;
	}
}
