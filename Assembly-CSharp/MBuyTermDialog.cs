using System;
using UnityEngine;

[Serializable]
public class MBuyTermDialog : Dialog
{
	public Tooltip tooltip;

	public Texture2D itemStarGauge;

	public Texture2D itemStarGaugeBg;

	public Texture2D itemStarGauge2;

	public Texture2D iconPoint;

	public Texture2D iconBrick;

	public Texture2D iconSale;

	public Texture2D iconNew;

	public Texture2D iconHot;

	private Good[] goods;

	private bool[] IsBuy;

	private Vector2 scrollPosition = Vector2.zero;

	private float viewWidth = 680f;

	private float viewHeight = 1000f;

	private string lastTooltip = string.Empty;

	private Vector2 ltTooltip = Vector2.zero;

	private Vector2 crdItem = new Vector2(700f, 140f);

	private Rect crdItemBtn = new Rect(0f, 0f, 154f, 130f);

	private Rect crdItemIcon = new Rect(12f, 6f, 142f, 100f);

	private Rect crdChkFP = new Rect(235f, 7f, 21f, 22f);

	private Rect crdStarGauge = new Rect(90f, 116f, 64f, 12f);

	private Rect crdItemPriceIcon = new Rect(4f, 116f, 12f, 12f);

	private Rect crdItemSpecialIcon = new Rect(4f, 75f, 28f, 28f);

	private Vector2 crdItemPrice = new Vector2(22f, 122f);

	private Vector2 crdOption = new Vector2(409f, 7f);

	private Rect crdBuyButton = new Rect(496f, 668f, 196f, 34f);

	private Rect crdToggle = new Rect(20f, 668f, 265f, 22f);

	private int selFilter;

	private bool[] wasBrickPoint;

	private bool[] isBrickPoint;

	private bool[] wasForcePoint;

	private bool[] isForcePoint;

	private bool[] wasCashPoint;

	private bool[] isCashPoint;

	private int[] percent;

	private Good.BUY_HOW[] buyHow;

	private string[] options;

	private int[] percentDC = new int[5]
	{
		0,
		3,
		5,
		8,
		15
	};

	private bool sendBuyPacket;

	private float focusTime;

	private bool bWeared;

	private string bigTitle = string.Empty;

	private bool prevWearType;

	private bool enoughPoint = true;

	private bool enoughBrick = true;

	private bool enoughCash = true;

	public override void Start()
	{
		tooltip.Start();
		id = DialogManager.DIALOG_INDEX.MBUY_TERM;
	}

	public override void OnPopup()
	{
		size.x = 744f;
		size.y = 718f;
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog(int _selFilter, bool _bWeared = false)
	{
		goods = ShopManager.Instance.GetCheckedAll();
		selFilter = _selFilter;
		sendBuyPacket = false;
		bWeared = _bWeared;
		prevWearType = bWeared;
		if (bWeared)
		{
			bigTitle = StringMgr.Instance.Get("SHOP_ITEM_PREVIEW_BUY");
		}
		else
		{
			bigTitle = StringMgr.Instance.Get("BTN_MULTIPURCHASE");
		}
		buyHow = new Good.BUY_HOW[goods.Length];
		for (int i = 0; i < goods.Length; i++)
		{
			buyHow[i] = Good.BUY_HOW.GENERAL_POINT;
		}
		wasBrickPoint = new bool[goods.Length];
		for (int j = 0; j < goods.Length; j++)
		{
			wasBrickPoint[j] = false;
		}
		isBrickPoint = new bool[goods.Length];
		for (int k = 0; k < goods.Length; k++)
		{
			isBrickPoint[k] = false;
		}
		wasForcePoint = new bool[goods.Length];
		for (int l = 0; l < goods.Length; l++)
		{
			wasForcePoint[l] = false;
		}
		isForcePoint = new bool[goods.Length];
		for (int m = 0; m < goods.Length; m++)
		{
			isForcePoint[m] = true;
		}
		isCashPoint = new bool[goods.Length];
		for (int n = 0; n < goods.Length; n++)
		{
			isCashPoint[n] = false;
		}
		wasCashPoint = new bool[goods.Length];
		for (int num = 0; num < goods.Length; num++)
		{
			wasCashPoint[num] = false;
		}
		percent = new int[goods.Length];
		for (int num2 = 0; num2 < goods.Length; num2++)
		{
			percent[num2] = 0;
		}
		IsBuy = new bool[goods.Length];
		for (int num3 = 0; num3 < goods.Length; num3++)
		{
			IsBuy[num3] = goods[num3].Check;
		}
		for (int num4 = 0; num4 < goods.Length; num4++)
		{
			if (goods[num4].IsCashable)
			{
				buyHow[num4] = Good.BUY_HOW.CASH_POINT;
			}
		}
	}

	private string ItemSlot2Tooltip(Item item, int i)
	{
		return (item != null) ? ("*" + i.ToString() + item.Seq.ToString()) : string.Empty;
	}

	public override void Update()
	{
		focusTime += Time.deltaTime;
	}

	private void ShowTooltip(int id)
	{
		tooltip.DoDialog();
	}

	private void DoTooltip(Vector2 offset)
	{
		Dialog top = DialogManager.Instance.GetTop();
		if (GUI.tooltip.Length > 0 && top != null && top.ID == DialogManager.DIALOG_INDEX.MBUY_TERM)
		{
			if (lastTooltip != GUI.tooltip)
			{
				focusTime = 0f;
				tooltip.ItemCode = GUI.tooltip;
				tooltip.IsShop = true;
				if (!DialogManager.Instance.IsModal)
				{
					GlobalVars.Instance.PlaySoundMouseOver();
				}
			}
			if (focusTime > 0.3f)
			{
				Vector2 coord = ltTooltip;
				float num = coord.y + tooltip.size.y;
				if (num > size.y)
				{
					coord.y -= num - size.y;
				}
				tooltip.SetCoord(coord);
				GUI.Box(tooltip.ClientRect, string.Empty, "TooltipWindow");
				GUI.BeginGroup(tooltip.ClientRect);
				tooltip.DoDialog();
				GUI.EndGroup();
			}
			lastTooltip = GUI.tooltip;
		}
	}

	private int DoBrickPointReceipt(int id, int percent)
	{
		int price = goods[id].GetPrice(goods[id].priceSel, buyHow[id]);
		return price - Mathf.CeilToInt((float)price * ((float)percent / 100f));
	}

	private int DoGeneralPointReceipt(int id, int percent)
	{
		int price = goods[id].GetPrice(goods[id].priceSel, buyHow[id]);
		return price - Mathf.CeilToInt((float)price * ((float)percent / 100f));
	}

	private bool DoAutochargedGeneralPointReceipt(ref int tk, ref int fp)
	{
		if (BuildOption.Instance.IsNetmarble)
		{
			return false;
		}
		int point = MyInfoManager.Instance.Point;
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < goods.Length; i++)
		{
			if (goods[i].Check && buyHow[i] == Good.BUY_HOW.GENERAL_POINT)
			{
				int price = goods[i].GetPrice(goods[i].priceSel, buyHow[i]);
				num2 += price - Mathf.CeilToInt((float)price * ((float)percent[i] / 100f));
			}
		}
		int num3 = 0;
		for (int j = 0; j < goods.Length; j++)
		{
			if (goods[j].Check && buyHow[j] == Good.BUY_HOW.CASH_POINT)
			{
				int price2 = goods[j].GetPrice(goods[j].priceSel, buyHow[j]);
				num3 += price2 - Mathf.CeilToInt((float)price2 * ((float)percent[j] / 100f));
			}
		}
		num = point - num2;
		int num4 = MyInfoManager.Instance.Cash - num3;
		if (num < 0 && ChannelManager.Instance.Tk2FpMultiple > 0)
		{
			int num5 = (num2 - point) / ChannelManager.Instance.Tk2FpMultiple;
			if ((num2 - point) % ChannelManager.Instance.Tk2FpMultiple > 0)
			{
				num5++;
			}
			if (point >= num5)
			{
				int num6 = num5 * ChannelManager.Instance.Tk2FpMultiple;
				int num7 = point + num6;
				if (num4 < num5)
				{
					return false;
				}
				tk += num5;
				fp += num6;
				num = num7 - num2;
				point = 0;
			}
		}
		return num >= 0;
	}

	private int DoCashPointReceipt(int id, int percent)
	{
		int price = goods[id].GetPrice(goods[id].priceSel, buyHow[id]);
		return price - Mathf.CeilToInt((float)price * ((float)percent / 100f));
	}

	private int GetWeaponLevel(int cat)
	{
		int result = -1;
		switch (cat)
		{
		case 0:
			result = XpManager.Instance.GetWeaponLevel((TWeapon.CATEGORY)cat, MyInfoManager.Instance.Heavy);
			break;
		case 1:
			result = XpManager.Instance.GetWeaponLevel((TWeapon.CATEGORY)cat, MyInfoManager.Instance.Assault);
			break;
		case 2:
			result = XpManager.Instance.GetWeaponLevel((TWeapon.CATEGORY)cat, MyInfoManager.Instance.Sniper);
			break;
		case 3:
			result = XpManager.Instance.GetWeaponLevel((TWeapon.CATEGORY)cat, MyInfoManager.Instance.SubMachine);
			break;
		case 4:
			result = XpManager.Instance.GetWeaponLevel((TWeapon.CATEGORY)cat, MyInfoManager.Instance.HandGun);
			break;
		case 5:
			result = XpManager.Instance.GetWeaponLevel((TWeapon.CATEGORY)cat, MyInfoManager.Instance.Melee);
			break;
		case 6:
			result = XpManager.Instance.GetWeaponLevel((TWeapon.CATEGORY)cat, MyInfoManager.Instance.Special);
			break;
		}
		return result;
	}

	private void DoWeaponDiscount(int id)
	{
		for (int i = 0; i < 5; i++)
		{
			int price = goods[i].GetPrice(goods[i].priceSel, buyHow[i]);
			if (percentDC[i] > 0)
			{
				price -= Mathf.CeilToInt((float)price * ((float)percentDC[i] / 100f));
			}
		}
	}

	private bool DoReceipt()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < goods.Length; i++)
		{
			if (goods[i].Check)
			{
				switch (buyHow[i])
				{
				case Good.BUY_HOW.GENERAL_POINT:
					num += DoGeneralPointReceipt(i, percent[i]);
					break;
				case Good.BUY_HOW.BRICK_POINT:
					num2 += DoBrickPointReceipt(i, percent[i]);
					break;
				case Good.BUY_HOW.CASH_POINT:
					num3 += DoCashPointReceipt(i, percent[i]);
					break;
				}
			}
		}
		enoughPoint = true;
		enoughBrick = true;
		enoughCash = true;
		bool result = true;
		if (MyInfoManager.Instance.Point < num)
		{
			enoughPoint = false;
			result = false;
		}
		if (MyInfoManager.Instance.BrickPoint < num2)
		{
			enoughBrick = false;
			result = false;
		}
		if (MyInfoManager.Instance.Cash < num3)
		{
			enoughCash = false;
			result = false;
		}
		return result;
	}

	private bool IsBuyWorkEnd()
	{
		if (!sendBuyPacket)
		{
			return false;
		}
		for (int i = 0; i < goods.Length; i++)
		{
			if (goods[i].Check)
			{
				return false;
			}
		}
		return true;
	}

	public override bool DoDialog()
	{
		bool result = false;
		Vector2 pos = new Vector2(size.x / 2f, 15f);
		LabelUtil.TextOut(pos, bigTitle, "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		Rect viewRect = new Rect(0f, 0f, viewWidth, viewHeight);
		Rect position = new Rect(22f, 74f, crdItem.x, 420f);
		scrollPosition = GUI.BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal: false, alwaysShowVertical: false);
		Rect position2 = new Rect(0f, 0f, crdItem.x, crdItem.y);
		float num = 0f;
		for (int i = 0; i < goods.Length; i++)
		{
			TItem tItem = goods[i].tItem;
			if (tItem != null)
			{
				position2.y = (float)i * crdItem.y;
				GUI.BeginGroup(position2);
				GUI.Box(crdItemBtn, new GUIContent(string.Empty, tItem.code), "BtnItem");
				if (tooltip.ItemCode == tItem.code)
				{
					tooltip.ItemCode = tItem.code;
					ltTooltip = new Vector2(position.x + position2.x + crdItemBtn.width, position.y + position2.y - scrollPosition.y);
				}
				if (tItem.CurIcon() == null)
				{
					Debug.LogError("Fail to get icon for item " + tItem.code);
				}
				else
				{
					TextureUtil.DrawTexture(crdItemIcon, tItem.CurIcon(), ScaleMode.ScaleToFit);
				}
				Color color = GUI.color;
				GUI.color = GlobalVars.Instance.txtMainColor;
				GUI.Label(crdItemBtn, tItem.Name, "MiniLabel");
				GUI.color = color;
				DrawDefaultPrice(goods[i]);
				if (itemStarGauge != null && itemStarGaugeBg != null)
				{
					TextureUtil.DrawTexture(crdStarGauge, itemStarGaugeBg, ScaleMode.StretchToFill);
					Rect position3 = new Rect(crdStarGauge.x, crdStarGauge.y, crdStarGauge.width * goods[i].starRate, crdStarGauge.height);
					GUI.BeginGroup(position3);
					TextureUtil.DrawTexture(new Rect(0f, 0f, crdStarGauge.width, crdStarGauge.height), itemStarGauge, ScaleMode.StretchToFill);
					GUI.EndGroup();
					if (goods[i].starRate > 1f)
					{
						float num2 = goods[i].starRate - 1f;
						position3 = new Rect(crdStarGauge.x, crdStarGauge.y, crdStarGauge.width * num2, crdStarGauge.height);
						GUI.BeginGroup(position3);
						TextureUtil.DrawTexture(new Rect(0f, 0f, crdStarGauge.width, crdStarGauge.height), itemStarGauge2, ScaleMode.StretchToFill);
						GUI.EndGroup();
					}
				}
				switch (tItem.type)
				{
				case TItem.TYPE.WEAPON:
					percent[i] = DoWeapon((TWeapon)tItem);
					break;
				case TItem.TYPE.CLOTH:
					percent[i] = DoCostume((TCostume)tItem);
					break;
				case TItem.TYPE.ACCESSORY:
					percent[i] = DoAccessory((TAccessory)tItem);
					break;
				case TItem.TYPE.CHARACTER:
					percent[i] = DoCharacter((TCharacter)tItem);
					break;
				case TItem.TYPE.BUNDLE:
					percent[i] = DoBundle((TBundle)tItem);
					break;
				}
				if (sendBuyPacket)
				{
					if (goods[i].BuyErr.Length > 0)
					{
						LabelUtil.TextOut(new Vector2(crdChkFP.x, crdChkFP.y), goods[i].BuyErr, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft, 300f);
					}
					else if (IsBuy[i])
					{
						LabelUtil.TextOut(new Vector2(crdChkFP.x, crdChkFP.y), StringMgr.Instance.Get("PURCHASE_DONE"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
					}
					else
					{
						LabelUtil.TextOut(new Vector2(crdChkFP.x, crdChkFP.y), StringMgr.Instance.Get("PURCHASE_DON’T"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
					}
				}
				else
				{
					Rect position4 = new Rect(159f, 5f, 21f, 22f);
					goods[i].Check = GUI.Toggle(position4, goods[i].Check, string.Empty);
					DoBuyHow(i, percent[i]);
					DoOptions(i);
				}
				GUI.EndGroup();
				num += crdItem.y;
			}
		}
		GUI.EndScrollView();
		viewHeight = num;
		bool flag = BuildOption.Instance.IsNetmarble || BuildOption.Instance.IsDeveloper;
		if (flag)
		{
			Vector2 pos2 = new Vector2(size.x / 2f, 510f);
			LabelUtil.TextOut(pos2, StringMgr.Instance.Get("PURCHASE_POLICY"), "MissionLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter, 400f);
			Rect rc = new Rect(size.x - 150f, 510f, 120f, 34f);
			if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("PURCHASE_POLICY_BUTTON"), "BtnAction"))
			{
				if (MyInfoManager.Instance.SiteCode == 1)
				{
					Application.OpenURL("http://helpdesk.netmarble.net/HelpStipulation.asp#ach13");
				}
				else if (MyInfoManager.Instance.SiteCode == 11)
				{
					Application.OpenURL("http://www.tooniland.com/common/html/serviceRules.jsp#");
				}
			}
		}
		if (prevWearType)
		{
			bWeared = GUI.Toggle(crdToggle, bWeared, StringMgr.Instance.Get("CHECK_DIRECT_EQUIP"));
		}
		bool flag2 = DoReceipt();
		bool flag3 = false;
		if (!flag2)
		{
			int tk = 0;
			int fp = 0;
			if (!enoughPoint && !flag && DoAutochargedGeneralPointReceipt(ref tk, ref fp))
			{
				string str = string.Format(StringMgr.Instance.Get("AUTO_CHARGING_PURCHASE"), tk, fp, TokenManager.Instance.GetTokenString());
				str += " ";
				str += StringMgr.Instance.Get("ARE_YOU_SURE_BUY");
				Rect position5 = new Rect(30f, 620f, 680f, 40f);
				GUI.Label(position5, str, "Label");
			}
			else
			{
				flag3 = true;
				string text = StringMgr.Instance.Get("NOT_ENOUGH_MONEY");
				Rect position6 = new Rect(30f, 620f, 680f, 40f);
				GUI.Label(position6, text, "Label");
			}
		}
		if (!IsBuyWorkEnd())
		{
			GUIContent content = new GUIContent(StringMgr.Instance.Get("BUY").ToUpper(), GlobalVars.Instance.iconCart);
			if (flag3)
			{
				GUI.enabled = false;
			}
			if (GlobalVars.Instance.MyButton3(crdBuyButton, content, "BtnAction"))
			{
				for (int j = 0; j < goods.Length; j++)
				{
					IsBuy[j] = goods[j].Check;
					if (goods[j].Check)
					{
						CSNetManager.Instance.Sock.SendCS_BUY_ITEM_REQ(goods[j].tItem.code, (int)buyHow[j], goods[j].GetOption(goods[j].priceSel, buyHow[j]), goods[j].IsAmount, bWeared);
					}
				}
				sendBuyPacket = true;
			}
			GUI.enabled = true;
		}
		else if (GlobalVars.Instance.MyButton(crdBuyButton, StringMgr.Instance.Get("CLOSE"), "BtnAction"))
		{
			ShopManager.Instance.InitAllGoods();
			result = true;
		}
		Vector2 pos3 = new Vector2(430f, 560f);
		LabelUtil.TextOut(pos3, StringMgr.Instance.Get("PURCHASE_TOTAL") + " : ", "MissionLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperRight, 360f);
		Vector2 pos4 = new Vector2(440f, 560f);
		LabelUtil.TextOut(pos4, StringMgr.Instance.Get("GENERAL_POINT"), "MissionLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		Vector2 pos5 = new Vector2(700f, 560f);
		LabelUtil.TextOut(pos5, GetTotalPoint(Good.BUY_HOW.GENERAL_POINT), "MissionLabel", (!enoughPoint) ? Color.red : GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		TextureUtil.DrawTexture(new Rect(700f, 565f, (float)iconPoint.width, (float)iconPoint.height), iconPoint, ScaleMode.StretchToFill);
		if (BuildOption.Instance.Props.useBrickPoint)
		{
			pos4.y += 22f;
			LabelUtil.TextOut(pos4, StringMgr.Instance.Get("BRICK_POINT"), "MissionLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			pos5.y += 22f;
			LabelUtil.TextOut(pos5, GetTotalPoint(Good.BUY_HOW.BRICK_POINT), "MissionLabel", (!enoughBrick) ? Color.red : GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			TextureUtil.DrawTexture(new Rect(pos5.x, pos5.y + 5f, (float)iconBrick.width, (float)iconBrick.height), iconBrick, ScaleMode.StretchToFill);
		}
		Texture2D mark = TokenManager.Instance.currentToken.mark;
		pos4.y += 22f;
		LabelUtil.TextOut(pos4, TokenManager.Instance.GetTokenString(), "MissionLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		pos5.y += 22f;
		LabelUtil.TextOut(pos5, GetTotalPoint(Good.BUY_HOW.CASH_POINT), "MissionLabel", (!enoughCash) ? Color.red : GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		TextureUtil.DrawTexture(new Rect(pos5.x, pos5.y + 5f, 12f, 12f), mark, ScaleMode.StretchToFill);
		Rect rc2 = new Rect(size.x - 44f, 5f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc2, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			ShopManager.Instance.InitAllGoods();
			result = true;
		}
		DoTooltip(new Vector2(base.rc.x, base.rc.y));
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		return result;
	}

	private string GetTotalPoint(Good.BUY_HOW wantbuyHow)
	{
		int num = 0;
		for (int i = 0; i < goods.Length; i++)
		{
			if (goods[i].Check && buyHow[i] == wantbuyHow)
			{
				num += goods[i].GetPrice(goods[i].priceSel, wantbuyHow);
			}
		}
		string text = " ";
		switch (wantbuyHow)
		{
		case Good.BUY_HOW.GENERAL_POINT:
			text = text + " / " + MyInfoManager.Instance.Point.ToString();
			break;
		case Good.BUY_HOW.BRICK_POINT:
			text = text + " / " + MyInfoManager.Instance.BrickPoint.ToString();
			break;
		case Good.BUY_HOW.CASH_POINT:
			text = text + " / " + MyInfoManager.Instance.Cash.ToString();
			break;
		}
		return num.ToString() + text;
	}

	private void ResetSelectedOption(int i)
	{
		goods[i].priceSel = goods[i].GetDefaultPriceSel(buyHow[i]);
	}

	private void VerifyBuyHow(int i, int percent)
	{
		if (!BuildOption.Instance.Props.usePriceDiscount)
		{
			percent = 0;
		}
		options = goods[i].GetOptionStrings(buyHow[i], percent);
		if (buyHow[i] == Good.BUY_HOW.GENERAL_POINT && !goods[i].CanBuy(Good.BUY_HOW.GENERAL_POINT, rebuy: false))
		{
			buyHow[i] = Good.BUY_HOW.BRICK_POINT;
			ResetSelectedOption(i);
		}
		if (buyHow[i] == Good.BUY_HOW.BRICK_POINT && !goods[i].CanBuy(Good.BUY_HOW.BRICK_POINT, rebuy: false))
		{
			buyHow[i] = Good.BUY_HOW.CASH_POINT;
			ResetSelectedOption(i);
		}
		if (buyHow[i] == Good.BUY_HOW.CASH_POINT && !goods[i].CanBuy(Good.BUY_HOW.CASH_POINT, rebuy: false))
		{
			buyHow[i] = Good.BUY_HOW.GENERAL_POINT;
			ResetSelectedOption(i);
		}
		if (goods[i].priceSel >= options.Length)
		{
			goods[i].priceSel = options.Length - 1;
		}
		if (goods[i].priceSel < 0)
		{
			goods[i].priceSel = 0;
		}
	}

	private void DoBuyHow(int i, int percent)
	{
		wasForcePoint[i] = false;
		wasBrickPoint[i] = false;
		wasCashPoint[i] = false;
		VerifyBuyHow(i, percent);
		switch (buyHow[i])
		{
		case Good.BUY_HOW.GENERAL_POINT:
			wasForcePoint[i] = true;
			break;
		case Good.BUY_HOW.BRICK_POINT:
			wasBrickPoint[i] = true;
			break;
		case Good.BUY_HOW.CASH_POINT:
			wasCashPoint[i] = true;
			break;
		}
		Rect rect = crdChkFP;
		GUI.enabled = goods[i].CanBuy(Good.BUY_HOW.GENERAL_POINT, rebuy: false);
		int levelMixLank = XpManager.Instance.GetLevelMixLank(MyInfoManager.Instance.Xp, MyInfoManager.Instance.Rank);
		bool flag = false;
		if (BuildOption.Instance.Props.itemBuyLimit && levelMixLank < goods[i].minlvFp)
		{
			flag = true;
		}
		if (flag)
		{
			GUI.enabled = false;
		}
		isForcePoint[i] = GUI.Toggle(rect, wasForcePoint[i], StringMgr.Instance.Get("GENERAL_POINT"));
		if (flag)
		{
			GUI.enabled = true;
			Texture2D badge = XpManager.Instance.GetBadge(goods[i].minlvFp);
			string rank = XpManager.Instance.GetRank(goods[i].minlvFp);
			if (null != badge)
			{
				Rect rect2 = new Rect(rect);
				rect2.x += 23f;
				rect2.y += 23f;
				string text = string.Format(StringMgr.Instance.Get("ITEM_PURCHASE_LIMIT_MSG05"), rank);
				TextureUtil.DrawTexture(new Rect(rect2.x, rect2.y, (float)badge.width, (float)badge.height), badge);
				LabelUtil.TextOut(new Vector2(rect2.x + (float)badge.width + 4f, rect2.y), text, "MiniLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
		}
		rect.y += 50f;
		isBrickPoint[i] = false;
		if (BuildOption.Instance.Props.useBrickPoint)
		{
			GUI.enabled = goods[i].CanBuy(Good.BUY_HOW.BRICK_POINT, rebuy: false);
			isBrickPoint[i] = GUI.Toggle(rect, wasBrickPoint[i], StringMgr.Instance.Get("BRICK_POINT"));
			rect.y += 50f;
		}
		GUI.enabled = goods[i].CanBuy(Good.BUY_HOW.CASH_POINT, rebuy: false);
		flag = false;
		if (BuildOption.Instance.Props.itemBuyLimit && levelMixLank < goods[i].minlvTk)
		{
			flag = true;
		}
		if (flag)
		{
			GUI.enabled = false;
		}
		isCashPoint[i] = GUI.Toggle(rect, wasCashPoint[i], TokenManager.Instance.GetTokenString());
		if (flag)
		{
			GUI.enabled = true;
			Texture2D badge2 = XpManager.Instance.GetBadge(goods[i].minlvTk);
			string rank2 = XpManager.Instance.GetRank(goods[i].minlvTk);
			if (null != badge2)
			{
				Rect rect3 = new Rect(rect);
				rect3.x += 23f;
				rect3.y += 23f;
				string text2 = string.Format(StringMgr.Instance.Get("ITEM_PURCHASE_LIMIT_MSG05"), rank2);
				TextureUtil.DrawTexture(new Rect(rect3.x, rect3.y, (float)badge2.width, (float)badge2.height), badge2);
				LabelUtil.TextOut(new Vector2(rect3.x + (float)badge2.width + 4f, rect3.y), text2, "MiniLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
		}
		GUI.enabled = true;
		if (options == null || (!wasForcePoint[i] && isForcePoint[i]))
		{
			buyHow[i] = Good.BUY_HOW.GENERAL_POINT;
			ResetSelectedOption(i);
		}
		if (options == null || (!wasBrickPoint[i] && isBrickPoint[i]))
		{
			buyHow[i] = Good.BUY_HOW.BRICK_POINT;
			ResetSelectedOption(i);
		}
		if (options == null || (!wasCashPoint[i] && isCashPoint[i]))
		{
			buyHow[i] = Good.BUY_HOW.CASH_POINT;
			ResetSelectedOption(i);
		}
	}

	private void DoOptions(int id)
	{
		Vector2 vector = crdOption;
		for (int i = 0; i < options.Length; i++)
		{
			bool flag = goods[id].priceSel == i;
			bool flag2 = flag;
			flag = GUI.Toggle(new Rect(vector.x, vector.y, 21f, 22f), flag, options[i]);
			if (!flag2 && flag)
			{
				goods[id].priceSel = i;
			}
			vector.y += 30f;
		}
	}

	private int DoWeapon(TWeapon tWeapon)
	{
		int result = 0;
		if (tWeapon.CurPrefab() != null)
		{
			Weapon component = tWeapon.CurPrefab().GetComponent<Weapon>();
			if (null == component)
			{
				Debug.LogError("Error, Fail to get weapon component from prefab ");
			}
			else
			{
				result = tWeapon.GetDiscountRatio();
			}
		}
		return result;
	}

	private int DoCostume(TCostume tCostume)
	{
		return 0;
	}

	private int DoAccessory(TAccessory tAccessory)
	{
		return 0;
	}

	private int DoCharacter(TCharacter tCharacter)
	{
		return 0;
	}

	private int DoBundle(TBundle tBundle)
	{
		return 0;
	}

	private void DrawDefaultPrice(Good good)
	{
		Rect position = new Rect(crdItemSpecialIcon);
		if (good.isSale)
		{
			TextureUtil.DrawTexture(position, iconSale, ScaleMode.StretchToFill);
			position.x += 20f;
		}
		if (good.isNew)
		{
			TextureUtil.DrawTexture(position, iconNew, ScaleMode.StretchToFill);
			position.x += 20f;
		}
		if (good.isHot)
		{
			TextureUtil.DrawTexture(position, iconHot, ScaleMode.StretchToFill);
			position.x += 20f;
		}
		Texture2D mark = TokenManager.Instance.currentToken.mark;
		if (selFilter == 0)
		{
			Rect position2 = new Rect(crdItemPriceIcon);
			if (good.IsPointable)
			{
				TextureUtil.DrawTexture(position2, iconPoint, ScaleMode.StretchToFill);
				position2.x += (float)(mark.width + 2);
			}
			if (good.IsBrickPointable && BuildOption.Instance.Props.useBrickPoint)
			{
				TextureUtil.DrawTexture(position2, iconBrick, ScaleMode.StretchToFill);
				position2.x += (float)(mark.width + 2);
			}
			if (good.IsCashable)
			{
				TextureUtil.DrawTexture(position2, mark, ScaleMode.StretchToFill);
			}
		}
		else
		{
			switch (selFilter)
			{
			case 1:
				if (good.IsCashable)
				{
					TextureUtil.DrawTexture(crdItemPriceIcon, mark, ScaleMode.StretchToFill);
					LabelUtil.TextOut(crdItemPrice, good.GetDefaultTokenPrice(), "MiniLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
				}
				break;
			case 2:
				if (good.IsPointable)
				{
					TextureUtil.DrawTexture(crdItemPriceIcon, iconPoint, ScaleMode.StretchToFill);
					LabelUtil.TextOut(crdItemPrice, good.GetDefaultPrice(), "MiniLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
				}
				break;
			case 3:
				if (good.IsBrickPointable)
				{
					TextureUtil.DrawTexture(crdItemPriceIcon, iconBrick, ScaleMode.StretchToFill);
					LabelUtil.TextOut(crdItemPrice, good.GetDefaultBrickPrice(), "MiniLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
				}
				break;
			}
		}
	}
}
