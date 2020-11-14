using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TCBoardDialog : Dialog
{
	private enum RESET_STEP
	{
		NONE,
		HOLD,
		FLICKER,
		FADEOUT
	}

	public Texture bg;

	public Texture coin;

	public Texture erasingCard;

	public Texture starStroke;

	public Texture rareItem;

	public AudioClip snd;

	public Tooltip tooltip;

	private float erasingMax = 0.3f;

	private float flyingMax = 0.3f;

	private float strokingMax = 0.5f;

	private Rect crdBack = new Rect(6f, 9f, 64f, 44f);

	private Rect crdX = new Rect(717f, 10f, 34f, 34f);

	private Rect crdBoardOutline = new Rect(14f, 67f, 570f, 464f);

	private Vector2 cardSize = new Vector2(24f, 24f);

	private Vector2 cardOffset = new Vector2(4f, 4f);

	private Vector2 cardLT = new Vector2(6f, 11f);

	private float offsetFive = 10f;

	private Rect crdPrizeList = new Rect(598f, 67f, 148f, 192f);

	private Rect crdPrizeRect = new Rect(602f, 71f, 134f, 186f);

	private Vector2 crdPrize = new Vector2(120f, 60f);

	private float prizeOffset = 2f;

	private Rect crdBoardInfo = new Rect(598f, 266f, 148f, 100f);

	private Rect crdMoneyInfo = new Rect(598f, 374f, 148f, 55f);

	private Rect crdNoConfirm = new Rect(602f, 434f, 160f, 22f);

	private Rect crdFreeBtn = new Rect(592f, 455f, 160f, 44f);

	private Rect crdTokenBtn = new Rect(592f, 492f, 160f, 44f);

	private Vector2 crdFlyDst = new Vector2(662f, 102f);

	private Vector2 crdBigRareFx = new Vector2(30f, 30f);

	private Vector2 crdSmallRareFx = new Vector2(10f, 10f);

	private TcStatus tcStatus;

	private List<byte> tcBoard;

	private List<int> tcWasKey;

	private Dictionary<int, Erasing> erasing;

	private Dictionary<int, Stroking> stroking;

	private Dictionary<int, RareStuff> rareFx;

	private Stack<TcPrize> prizes;

	private Dictionary<int, Flying> flying;

	private RESET_STEP resetStep;

	private float resetTime;

	private bool showFlicker = true;

	private float fadeOutMax = 0.5f;

	private float flickerMax = 1.2f;

	private float flickerTime;

	private float holdMax = 2.5f;

	private int curTicket = -1;

	private Vector2 spPrize = new Vector2(0f, 0f);

	private bool noConfirm;

	private RandomBoxSureDialog areYouSure;

	public override void Start()
	{
		tooltip.Start();
		id = DialogManager.DIALOG_INDEX.TCBOARD;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public override void OnClose(DialogManager.DIALOG_INDEX popup)
	{
		if (popup != DialogManager.DIALOG_INDEX.TCGATE)
		{
			CSNetManager.Instance.Sock.SendCS_TC_CLOSE_REQ();
		}
	}

	public void InitDialog(TcStatus tcs, List<byte> board, List<int> wasKey)
	{
		areYouSure = null;
		curTicket = -1;
		spPrize = new Vector2(0f, 0f);
		tcStatus = tcs;
		tcBoard = new List<byte>();
		tcWasKey = new List<int>();
		erasing = new Dictionary<int, Erasing>();
		stroking = new Dictionary<int, Stroking>();
		rareFx = new Dictionary<int, RareStuff>();
		prizes = new Stack<TcPrize>();
		flying = new Dictionary<int, Flying>();
		for (int i = 0; i < board.Count; i++)
		{
			tcBoard.Add(board[i]);
		}
		for (int j = 0; j < wasKey.Count; j++)
		{
			tcWasKey.Add(wasKey[j]);
		}
		wasKey.Clear();
	}

	public void Reset(int seq)
	{
		if (tcStatus.Seq == seq)
		{
			resetStep = RESET_STEP.HOLD;
			resetTime = 0f;
			showFlicker = true;
			flickerTime = 0f;
		}
	}

	public override void Update()
	{
		foreach (KeyValuePair<int, RareStuff> item in rareFx)
		{
			item.Value.Update();
		}
		foreach (KeyValuePair<int, Stroking> item2 in stroking)
		{
			item2.Value.deltaTime += Time.deltaTime;
		}
		foreach (KeyValuePair<int, Erasing> item3 in erasing)
		{
			item3.Value.deltaTime += Time.deltaTime;
		}
		foreach (KeyValuePair<int, Flying> item4 in flying)
		{
			item4.Value.deltaTime += Time.deltaTime;
		}
		TcPrize[] array = prizes.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Update();
		}
		switch (resetStep)
		{
		case RESET_STEP.HOLD:
			resetTime += Time.deltaTime;
			if (resetTime > holdMax)
			{
				resetTime = 0f;
				resetStep = RESET_STEP.FLICKER;
			}
			break;
		case RESET_STEP.FLICKER:
			resetTime += Time.deltaTime;
			if (resetTime > flickerMax)
			{
				resetTime = 0f;
				resetStep = RESET_STEP.FADEOUT;
			}
			else
			{
				flickerTime += Time.deltaTime;
				if (flickerTime > 0.3f)
				{
					flickerTime = 0f;
					showFlicker = !showFlicker;
				}
			}
			break;
		case RESET_STEP.FADEOUT:
			resetTime += Time.deltaTime;
			if (resetTime > fadeOutMax)
			{
				GlobalVars.Instance.PlayOneShot(snd);
				resetStep = RESET_STEP.NONE;
				SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("TREASURE_CHEST_RESET"));
				resetTime = 0f;
				for (int j = 0; j < tcBoard.Count; j++)
				{
					tcBoard[j] = 1;
				}
				tcWasKey.Clear();
			}
			break;
		}
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		DoTitle();
		DoBoard();
		DoPrizes();
		DoTcStatus();
		DoMoney();
		DoStroking();
		DoTooltip();
		DoFlying();
		DoRareFx();
		noConfirm = GUI.Toggle(crdNoConfirm, noConfirm, StringMgr.Instance.Get("DONT_ASK_GAMBLE"));
		bool enabled = GUI.enabled;
		GUI.enabled = (resetStep == RESET_STEP.NONE && tcStatus != null && 0 < MyInfoManager.Instance.FreeCoin && 0 <= curTicket && curTicket < tcBoard.Count && tcBoard[curTicket] > 0 && tcStatus.CoinPrice != 0);
		if (GlobalVars.Instance.MyButton(crdFreeBtn, StringMgr.Instance.Get("FREE"), "BtnAction"))
		{
			if (noConfirm)
			{
				CSNetManager.Instance.Sock.SendCS_TC_OPEN_PRIZE_TAG_REQ(tcStatus.Seq, curTicket, freeCoin: true);
			}
			else
			{
				areYouSure = (RandomBoxSureDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.RANDOMBOX_SURE, exclusive: false);
				if (areYouSure != null)
				{
					areYouSure.InitDialog(tcStatus.Seq, curTicket, 0);
				}
			}
		}
		GUI.enabled = (resetStep == RESET_STEP.NONE && tcStatus != null && tcStatus.TokenPrice <= MyInfoManager.Instance.Cash && 0 <= curTicket && curTicket < tcBoard.Count && tcBoard[curTicket] > 0 && tcStatus.TokenPrice != 0);
		if (GlobalVars.Instance.MyButton(crdTokenBtn, TokenManager.Instance.GetTokenString(), "BtnAction"))
		{
			if (noConfirm)
			{
				CSNetManager.Instance.Sock.SendCS_TC_OPEN_PRIZE_TAG_REQ(tcStatus.Seq, curTicket, freeCoin: false);
			}
			else
			{
				areYouSure = (RandomBoxSureDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.RANDOMBOX_SURE, exclusive: false);
				if (areYouSure != null)
				{
					areYouSure.InitDialog(tcStatus.Seq, curTicket, tcStatus.TokenPrice);
				}
			}
		}
		GUI.enabled = enabled;
		if (GlobalVars.Instance.MyButton(crdX, string.Empty, "BtnClose"))
		{
			result = true;
		}
		if (GlobalVars.Instance.MyButton(crdBack, string.Empty, "BtnBack") || (!DialogManager.Instance.IsPopup(DialogManager.DIALOG_INDEX.RANDOMBOX_SURE) && GlobalVars.Instance.IsEscapePressed()))
		{
			CSNetManager.Instance.Sock.SendCS_TC_LEAVE_REQ();
			((TCGateDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.TCGATE, exclusive: true))?.InitDialog();
		}
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}

	private void DoTitle()
	{
		Vector2 pos = new Vector2(size.x / 2f, 15f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("TREASURE_CHEST"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
	}

	private void DoRareFx()
	{
		for (int i = 0; i < tcWasKey.Count; i++)
		{
			int key = tcWasKey[i];
			if (rareFx.ContainsKey(key))
			{
				if (!rareFx[key].Alive)
				{
					rareFx.Remove(key);
				}
				else
				{
					RareFx[] array = rareFx[key].ToArray();
					for (int j = 0; j < array.Length; j++)
					{
						switch (array[j].RareFxStep)
						{
						case RareFx.RAREFX_STEP.SIZE_UP:
						{
							Vector2 vector4 = Vector2.Lerp(Vector2.zero, crdBigRareFx, array[j].Delta);
							Vector2 start = array[j].Start;
							float x = start.x - vector4.x / 2f;
							Vector2 start2 = array[j].Start;
							Rect position3 = new Rect(x, start2.y - vector4.y / 2f, vector4.x, vector4.y);
							Vector2 start3 = array[j].Start;
							float angle = Mathf.Lerp(0f, 359f, array[j].Delta);
							Matrix4x4 matrix = GUI.matrix;
							GUIUtility.RotateAroundPivot(angle, start3);
							TextureUtil.DrawTexture(position3, starStroke, ScaleMode.StretchToFill);
							GUI.matrix = matrix;
							break;
						}
						case RareFx.RAREFX_STEP.FLY:
						{
							Vector2 vector2 = Vector2.Lerp(array[j].Start, crdFlyDst, array[j].Delta);
							Vector2 vector3 = Vector2.Lerp(crdBigRareFx, crdSmallRareFx, array[j].Delta);
							Rect position2 = new Rect(vector2.x - vector3.x / 2f, vector2.y - vector3.y / 2f, vector3.x, vector3.y);
							TextureUtil.DrawTexture(position2, starStroke, ScaleMode.StretchToFill);
							break;
						}
						case RareFx.RAREFX_STEP.BOUNCE:
						{
							Vector2 vector = Vector2.Lerp(crdFlyDst, array[j].End, array[j].Delta);
							Rect position = new Rect(vector.x - crdSmallRareFx.x / 2f, vector.y - crdSmallRareFx.y / 2f, crdSmallRareFx.x, crdSmallRareFx.y);
							Color color = GUI.color;
							GUI.color = Color.Lerp(Color.white, new Color(1f, 1f, 1f, 0f), array[j].Delta);
							TextureUtil.DrawTexture(position, starStroke, ScaleMode.StretchToFill);
							GUI.color = color;
							break;
						}
						}
					}
				}
			}
		}
	}

	private void DoFlying()
	{
		for (int i = 0; i < tcBoard.Count; i++)
		{
			int num = i % 20;
			int num2 = i / 20;
			if (flying.ContainsKey(i))
			{
				if (flying[i].deltaTime > flyingMax)
				{
					prizes.Push(new TcPrize(flying[i]));
					flying.Remove(i);
				}
				else
				{
					Rect rect = new Rect((float)num * (cardSize.x + cardOffset.x) + cardLT.x, (float)num2 * (cardSize.y + cardOffset.y) + cardLT.y + (float)(num2 / 5) * offsetFive, cardSize.x, cardSize.y);
					Vector2 a = new Vector2(rect.x + rect.width / 2f, rect.y + rect.height / 2f);
					a += new Vector2(crdBoardOutline.x, crdBoardOutline.y);
					Vector2 vector = Vector2.Lerp(a, new Vector2(crdPrizeRect.x, crdPrizeRect.y), flying[i].deltaTime / flyingMax);
					Vector2 vector2 = Vector2.Lerp(Vector2.zero, crdPrize, flying[i].deltaTime / flyingMax);
					GUI.Box(new Rect(vector.x, vector.y, vector2.x, vector2.y), string.Empty, "BoxBrickSel");
				}
			}
		}
	}

	private void DoStroking()
	{
		for (int i = 0; i < tcBoard.Count; i++)
		{
			if (stroking.ContainsKey(i))
			{
				int num = i % 20;
				int num2 = i / 20;
				if (stroking[i].deltaTime > strokingMax)
				{
					stroking.Remove(i);
				}
				else
				{
					Rect rect = new Rect((float)num * (cardSize.x + cardOffset.x) + cardLT.x, (float)num2 * (cardSize.y + cardOffset.y) + cardLT.y + (float)(num2 / 5) * offsetFive, cardSize.x, cardSize.y);
					Vector2 a = new Vector2(rect.x + rect.width / 2f, rect.y + rect.height / 2f);
					a += new Vector2(crdBoardOutline.x, crdBoardOutline.y);
					float num3 = Mathf.Lerp(0f, 2.5f * (float)starStroke.width, stroking[i].deltaTime / strokingMax);
					float num4 = Mathf.Lerp(0f, 2.5f * (float)starStroke.height, stroking[i].deltaTime / strokingMax);
					Color color = GUI.color;
					GUI.color = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(1f, 1f, 1f, 0.5f), stroking[i].deltaTime / strokingMax);
					TextureUtil.DrawTexture(new Rect(a.x - num3 / 2f, a.y - num4 / 2f, num3, num4), starStroke, ScaleMode.StretchToFill);
					GUI.color = color;
				}
			}
		}
	}

	private void DoBoard()
	{
		TextureUtil.DrawTexture(crdBoardOutline, bg, ScaleMode.StretchToFill);
		GUI.Box(crdBoardOutline, string.Empty, "BoxPopLine");
		GUI.BeginGroup(crdBoardOutline);
		for (int i = 0; i < tcBoard.Count; i++)
		{
			int num = i % 20;
			int num2 = i / 20;
			if (tcBoard[i] != 0)
			{
				Rect rc = new Rect((float)num * (cardSize.x + cardOffset.x) + cardLT.x, (float)num2 * (cardSize.y + cardOffset.y) + cardLT.y + (float)(num2 / 5) * offsetFive, cardSize.x, cardSize.y);
				bool enabled = GUI.enabled;
				GUI.enabled = (resetStep == RESET_STEP.NONE);
				if (GlobalVars.Instance.MyButton(rc, string.Empty, "TreasureChestCard"))
				{
					if (curTicket != i && !stroking.ContainsKey(i))
					{
						stroking.Add(i, new Stroking());
					}
					curTicket = i;
				}
				if (curTicket == i)
				{
					GUI.Box(new Rect(rc.x - 2f, rc.y - 2f, rc.width + 4f, rc.height + 4f), string.Empty, "BoxBrickSel");
				}
				GUI.enabled = enabled;
			}
			else if (erasing.ContainsKey(i))
			{
				if (erasing[i].deltaTime > erasingMax)
				{
					erasing.Remove(i);
				}
				else
				{
					Rect rect = new Rect((float)num * (cardSize.x + cardOffset.x) + cardLT.x, (float)num2 * (cardSize.y + cardOffset.y) + cardLT.y + (float)(num2 / 5) * offsetFive, cardSize.x, cardSize.y);
					Vector2 vector = new Vector2(rect.x + rect.width / 2f, rect.y + rect.height / 2f);
					float num3 = Mathf.Lerp((float)erasingCard.width, 0f, erasing[i].deltaTime / erasingMax);
					float num4 = Mathf.Lerp((float)erasingCard.height, 0f, erasing[i].deltaTime / erasingMax);
					TextureUtil.DrawTexture(new Rect(vector.x - num3 / 2f, vector.y - num4 / 2f, num3, num4), erasingCard, ScaleMode.StretchToFill);
				}
			}
			else
			{
				bool enabled2 = GUI.enabled;
				GUI.enabled = (resetStep == RESET_STEP.NONE);
				Rect position = new Rect((float)num * (cardSize.x + cardOffset.x) + cardLT.x, (float)num2 * (cardSize.y + cardOffset.y) + cardLT.y + (float)(num2 / 5) * offsetFive, cardSize.x, cardSize.y);
				GUI.Box(position, string.Empty, "TreasureChestTookOff");
				for (int j = 0; j < tcWasKey.Count; j++)
				{
					if (tcWasKey[j] == i)
					{
						Vector2 vector2 = new Vector2(position.x + position.width / 2f, position.y + position.height / 2f);
						Rect position2 = new Rect(vector2.x - (float)(starStroke.width / 2), vector2.y - (float)(starStroke.height / 2), (float)starStroke.width, (float)starStroke.height);
						TextureUtil.DrawTexture(position2, starStroke, ScaleMode.StretchToFill);
					}
				}
				GUI.enabled = enabled2;
			}
			switch (resetStep)
			{
			case RESET_STEP.FADEOUT:
			{
				Rect rect3 = new Rect((float)num * (cardSize.x + cardOffset.x) + cardLT.x, (float)num2 * (cardSize.y + cardOffset.y) + cardLT.y + (float)(num2 / 5) * offsetFive, cardSize.x, cardSize.y);
				Vector2 vector3 = new Vector2(rect3.x + rect3.width / 2f, rect3.y + rect3.height / 2f);
				float num5 = Mathf.Lerp((float)erasingCard.width, 0f, resetTime / fadeOutMax);
				float num6 = Mathf.Lerp((float)erasingCard.height, 0f, resetTime / fadeOutMax);
				TextureUtil.DrawTexture(new Rect(vector3.x - num5 / 2f, vector3.y - num6 / 2f, num5, num6), erasingCard, ScaleMode.StretchToFill);
				break;
			}
			case RESET_STEP.FLICKER:
				if (showFlicker)
				{
					Rect rect2 = new Rect((float)num * (cardSize.x + cardOffset.x) + cardLT.x, (float)num2 * (cardSize.y + cardOffset.y) + cardLT.y + (float)(num2 / 5) * offsetFive, cardSize.x, cardSize.y);
					TextureUtil.DrawTexture(new Rect(rect2.x, rect2.y, rect2.width, rect2.height), erasingCard, ScaleMode.StretchToFill);
				}
				break;
			}
		}
		GUI.EndGroup();
	}

	private void DoTcStatus()
	{
		GUI.Box(crdBoardInfo, string.Empty, "BoxFadeBlue");
		string title = tcStatus.GetTitle();
		Vector2 pos = new Vector2(crdBoardInfo.x + 5f, crdBoardInfo.y + 5f);
		LabelUtil.TextOut(pos, title, "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		pos.y += 25f;
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("TAG_REMAIN"), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(new Vector2(crdBoardInfo.x + crdBoardInfo.width - 5f, pos.y), tcStatus.Cur.ToString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		pos.y += 20f;
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("RARE_REMAIN"), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(new Vector2(crdBoardInfo.x + crdBoardInfo.width - 5f, pos.y), tcStatus.Key.ToString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		pos.y += 20f;
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("RARE_CHANCE"), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(new Vector2(crdBoardInfo.x + crdBoardInfo.width - 5f, pos.y), tcStatus.Chance.ToString("0.##") + " %", "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
	}

	private void DoPrizes()
	{
		TcPrize[] array = prizes.ToArray();
		GUI.Box(crdPrizeList, string.Empty, "BoxPopLine");
		float num = CalcFlyingOffset();
		spPrize = GUI.BeginScrollView(viewRect: new Rect(0f, 0f, crdPrize.x, (float)array.Length * (crdPrize.y + prizeOffset) + num), position: crdPrizeRect, scrollPosition: spPrize);
		float y = spPrize.y;
		float num2 = y + crdPrizeRect.height;
		for (int i = 0; i < array.Length; i++)
		{
			Rect position = new Rect(0f, (float)i * (crdPrize.y + prizeOffset) + num, crdPrize.x, crdPrize.y);
			float y2 = position.y;
			float num3 = y2 + position.height;
			if (num3 >= y && y2 <= num2)
			{
				if (GUI.Button(position, new GUIContent(array[i].Icon, i.ToString()), "BtnBlue"))
				{
				}
				Color color = GUI.color;
				GUI.color = GlobalVars.Instance.txtMainColor;
				Rect position2 = new Rect(position.x + 4f, position.y, position.width - 8f, position.height);
				GUI.Label(position2, array[i].Name, "MiniLabel");
				GUI.color = color;
				if (i == 0 && array[i].NeedOutline)
				{
					GUI.Box(position, string.Empty, "BoxBrickSel");
				}
				if (array[i].IsRareItem)
				{
					TextureUtil.DrawTexture(new Rect(position.x + 2f, position.y + position.height - (float)(rareItem.height + 2), (float)rareItem.width, (float)rareItem.height), rareItem, ScaleMode.StretchToFill);
				}
			}
		}
		GUI.EndScrollView();
	}

	private void DoTooltip()
	{
		Dialog top = DialogManager.Instance.GetTop();
		if (GUI.tooltip.Length > 0 && top != null && top.ID == DialogManager.DIALOG_INDEX.TCBOARD)
		{
			tooltip.ItemCode = string.Empty;
			int num = -1;
			try
			{
				num = int.Parse(GUI.tooltip);
			}
			catch
			{
			}
			TcPrize[] array = prizes.ToArray();
			if (0 <= num && num < array.Length)
			{
				tooltip.ItemCode = array[num].Code;
				tooltip.IsShop = true;
				tooltip.Prize = array[num];
			}
			if (tooltip.ItemCode.Length > 0)
			{
				Vector2 coord = new Vector2(crdBoardOutline.x + crdBoardOutline.width - tooltip.size.x, crdBoardOutline.y);
				tooltip.SetCoord(coord);
				GUI.Box(tooltip.ClientRect, string.Empty, "TooltipWindow");
				GUI.BeginGroup(tooltip.ClientRect);
				tooltip.DoDialog();
				GUI.EndGroup();
			}
		}
	}

	private float CalcFlyingOffset()
	{
		float num = 0f;
		foreach (KeyValuePair<int, Flying> item in flying)
		{
			Vector2 vector = Vector2.Lerp(Vector2.zero, crdPrize, item.Value.deltaTime / flyingMax);
			num += vector.y;
		}
		return num;
	}

	private void DoMoney()
	{
		Texture2D mark = TokenManager.Instance.currentToken.mark;
		GUI.Box(crdMoneyInfo, string.Empty, "BoxFadeBlue");
		Vector2 vector = new Vector2(crdMoneyInfo.x + 10f, crdMoneyInfo.y + 7f);
		Vector2 pos = new Vector2(crdMoneyInfo.x + crdMoneyInfo.width - 10f, crdMoneyInfo.y + 5f);
		if (tcStatus.TokenPrice != 0)
		{
			TextureUtil.DrawTexture(new Rect(vector.x, vector.y, (float)mark.width, (float)mark.height), mark);
			string text = tcStatus.TokenPrice.ToString() + " / " + MyInfoManager.Instance.Cash;
			Color clrText = new Color(0.2f, 0.75f, 0.06f);
			if (tcStatus.TokenPrice > MyInfoManager.Instance.Cash)
			{
				clrText = Color.red;
			}
			LabelUtil.TextOut(pos, text, "Label", clrText, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			vector.y += 20f;
			pos.y += 20f;
		}
		if (tcStatus.CoinPrice != 0)
		{
			TextureUtil.DrawTexture(new Rect(vector.x, vector.y, (float)coin.width, (float)coin.height), coin);
			string text = tcStatus.CoinPrice.ToString() + " / " + MyInfoManager.Instance.FreeCoin;
			Color clrText = GlobalVars.Instance.txtMainColor;
			if (tcStatus.CoinPrice > MyInfoManager.Instance.FreeCoin)
			{
				clrText = Color.red;
			}
			LabelUtil.TextOut(pos, text, "Label", clrText, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		}
	}

	public void TookOff(int chest, int index, bool wasKey)
	{
		if (tcStatus.Seq == chest && 0 <= index && index < tcBoard.Count && tcBoard[index] > 0)
		{
			tcBoard[index] = 0;
			if (wasKey)
			{
				tcWasKey.Add(index);
			}
			erasing.Add(index, new Erasing());
		}
	}

	public void ReceivePrize(long seq, int index, string code, int amount, bool wasKey)
	{
		TItem tItem = TItemManager.Instance.Get<TItem>(code);
		if (tItem == null)
		{
			Debug.LogError("Fail to get TItem for " + code);
		}
		else
		{
			spPrize = Vector2.zero;
			GlobalVars.Instance.PlayOneShot(snd);
			flying.Add(index, new Flying(seq, tItem, amount, wasKey));
			if (wasKey)
			{
				int num = index % 20;
				int num2 = index / 20;
				Rect rect = new Rect((float)num * (cardSize.x + cardOffset.x) + cardLT.x, (float)num2 * (cardSize.y + cardOffset.y) + cardLT.y + (float)(num2 / 5) * offsetFive, cardSize.x, cardSize.y);
				Vector2 a = new Vector2(rect.x + rect.width / 2f, rect.y + rect.height / 2f);
				a += new Vector2(crdBoardOutline.x, crdBoardOutline.y);
				rareFx.Add(index, new RareStuff(a, crdFlyDst));
			}
		}
	}
}
