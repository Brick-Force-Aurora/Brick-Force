using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuyTermDialog : Dialog
{
	private Good good;

	private Good.BUY_HOW buyHow;

	private string[] options;

	private int selected;

	public Texture2D gauge;

	public Texture2D gaugeFrame;

	public float maxAtkPow = 100f;

	public float maxRpm = 1000f;

	public float maxRecoilPitch = 5f;

	public float maxMobility = 2f;

	private float maxSlashSpeed = 3f;

	private float maxRange = 25f;

	private float maxRangeBang = 30f;

	private float maxEffTime = 8f;

	public Texture2D icon;

	public Texture2D iconPoint;

	private Vector2 crdTitle = new Vector2(337f, 15f);

	private Vector2 crdItemName = new Vector2(26f, 37f);

	private Vector2 crdCategory = new Vector2(26f, 77f);

	private Rect crdIcon = new Rect(50f, 113f, 161f, 91f);

	private Rect crdComment = new Rect(15f, 235f, 644f, 100f);

	private Rect crdLine = new Rect(0f, 350f, 674f, 1f);

	private Rect crdCloseBtn = new Rect(628f, 10f, 34f, 34f);

	private Vector2 crdWpnLabel1 = new Vector2(523f, 90f);

	private Vector2 crdWpnLabel2 = new Vector2(523f, 110f);

	private Vector2 crdWpnLabel3 = new Vector2(523f, 130f);

	private Vector2 crdWpnLabel4 = new Vector2(523f, 150f);

	private Vector2 crdWpnLabel5 = new Vector2(523f, 170f);

	private Vector2 crdWpnLabel6 = new Vector2(523f, 190f);

	private Vector2 crdWpnLabel7 = new Vector2(523f, 210f);

	private Vector2 crdWpnLabel8 = new Vector2(523f, 230f);

	private Rect crdWpnGaugeOutline1 = new Rect(538f, 83f, 110f, 16f);

	private Rect crdWpnGaugeOutline2 = new Rect(538f, 103f, 110f, 16f);

	private Rect crdWpnGaugeOutline3 = new Rect(538f, 123f, 110f, 16f);

	private Rect crdWpnGaugeOutline4 = new Rect(538f, 143f, 110f, 16f);

	private Rect crdWpnGaugeOutline5 = new Rect(538f, 163f, 110f, 16f);

	private Rect crdWpnGaugeOutline6 = new Rect(538f, 183f, 110f, 16f);

	private Rect crdWpnGaugeOutline7 = new Rect(538f, 203f, 110f, 16f);

	private Rect crdWpnGaugeOutline8 = new Rect(538f, 223f, 110f, 16f);

	private Rect crdWpnGauge1 = new Rect(541f, 85f, 104f, 12f);

	private Rect crdWpnGauge2 = new Rect(541f, 105f, 104f, 12f);

	private Rect crdWpnGauge3 = new Rect(541f, 125f, 104f, 12f);

	private Rect crdWpnGauge4 = new Rect(541f, 145f, 104f, 12f);

	private Rect crdWpnGauge5 = new Rect(541f, 165f, 104f, 12f);

	private Rect crdWpnGauge6 = new Rect(541f, 185f, 104f, 12f);

	private Rect crdWpnGauge7 = new Rect(541f, 205f, 104f, 12f);

	private Vector2 crdWpnValue1 = new Vector2(595f, 90f);

	private Vector2 crdWpnValue2 = new Vector2(595f, 110f);

	private Vector2 crdWpnValue3 = new Vector2(595f, 130f);

	private Vector2 crdWpnValue4 = new Vector2(595f, 150f);

	private Vector2 crdWpnValue5 = new Vector2(595f, 170f);

	private Vector2 crdWpnValue6 = new Vector2(595f, 190f);

	private Vector2 crdWpnValue7 = new Vector2(595f, 210f);

	private Vector2 crdWpnValue8 = new Vector2(595f, 230f);

	private Vector2 crdBuyHowLabel = new Vector2(26f, 360f);

	private Vector2 crdOptionSelectLabel = new Vector2(200f, 360f);

	private Vector2 crdAuthLabel = new Vector2(480f, 360f);

	private Vector2 crdLVTitle = new Vector2(495f, 400f);

	private Vector2 crdDCTitle = new Vector2(555f, 330f);

	private Vector2 crdSumTitle = new Vector2(615f, 330f);

	private Rect crdGeneralPoint = new Rect(26f, 400f, 21f, 22f);

	private Vector2 crdOption = new Vector2(200f, 400f);

	private Rect crdSelectBox = new Rect(413f, 347f, 160f, 20f);

	private Rect crdBuyButton = new Rect(426f, 558f, 196f, 34f);

	private Rect crdPresentButton = new Rect(40f, 558f, 196f, 34f);

	private long rebuy = -1L;

	private int[] percentDC = new int[5]
	{
		0,
		3,
		5,
		8,
		15
	};

	private Color txtMainClr;

	private bool[] canBuyItemFromLv = new bool[3]
	{
		true,
		true,
		true
	};

	private Vector2 crdExtra = new Vector2(650f, 62f);

	private float crdExtraOffset = 20f;

	private float currenty;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.BUY_TERM;
		if (!BuildOption.Instance.Props.useDurability)
		{
			crdWpnLabel8.y = crdWpnLabel7.y;
			crdWpnGaugeOutline8.y = crdWpnGaugeOutline7.y;
			crdWpnValue8.y = crdWpnValue7.y;
		}
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
		txtMainClr = GlobalVars.Instance.txtMainColor;
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

	private void DoWeaponDiscount()
	{
		if (BuildOption.Instance.Props.usePriceDiscount)
		{
			LabelUtil.TextOut(crdAuthLabel, StringMgr.Instance.Get("AUTH_PRICE"), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			TWeapon tWeapon = (TWeapon)good.tItem;
			int weaponLevel = GetWeaponLevel(tWeapon.cat);
			Color byteColor2FloatColor = GlobalVars.Instance.GetByteColor2FloatColor(244, 151, 25);
			LabelUtil.TextOut(crdLVTitle, "Lv.", "Label", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(crdLVTitle.x + 60f, crdLVTitle.y), "D.C", "Label", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(crdLVTitle.x + 120f, crdLVTitle.y), "Sum", "Label", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			float num = crdLVTitle.y + 8f;
			for (int i = 0; i < 5; i++)
			{
				Vector2 pos = new Vector2(crdLVTitle.x, num + (float)((i + 1) * 18));
				Vector2 pos2 = new Vector2(crdDCTitle.x, num + (float)((i + 1) * 18));
				Vector2 pos3 = new Vector2(crdSumTitle.x, num + (float)((i + 1) * 18));
				if (weaponLevel == i)
				{
					crdSelectBox.x = pos.x - 17f;
					crdSelectBox.y = pos.y - 9f;
					GUI.Box(crdSelectBox, string.Empty, "BtnSelectF");
				}
				LabelUtil.TextOut(pos, (i + 1).ToString() + "Lv", "Label", new Color(0.64f, 0.64f, 0.64f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(pos2, percentDC[i].ToString() + "%", "Label", new Color(0.64f, 0.64f, 0.64f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				int num2 = good.GetOriginalPrice(selected, buyHow);
				if (percentDC[i] > 0)
				{
					num2 -= Mathf.CeilToInt((float)num2 * ((float)percentDC[i] / 100f));
				}
				LabelUtil.TextOut(pos3, num2.ToString("n0"), "Label", new Color(0.64f, 0.64f, 0.64f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
		}
	}

	private bool DoBrickPointReceipt(int percent)
	{
		int price = good.GetPrice(selected, buyHow);
		int num = price - Mathf.CeilToInt((float)price * ((float)percent / 100f));
		int brickPoint = MyInfoManager.Instance.BrickPoint;
		int num2 = brickPoint - num;
		return num2 >= 0;
	}

	private bool DoGeneralPointReceipt(int percent)
	{
		int price = good.GetPrice(selected, buyHow);
		int num = price - Mathf.CeilToInt((float)price * ((float)percent / 100f));
		int point = MyInfoManager.Instance.Point;
		int num2 = point - num;
		return num2 >= 0;
	}

	private bool DoAutochargedGeneralPointReceipt(int percent, ref int tk, ref int fp)
	{
		int price = good.GetPrice(selected, buyHow);
		int num = price - Mathf.CeilToInt((float)price * ((float)percent / 100f));
		int point = MyInfoManager.Instance.Point;
		int num2 = point - num;
		if (num2 < 0 && ChannelManager.Instance.Tk2FpMultiple > 0)
		{
			int num3 = (num - point) / ChannelManager.Instance.Tk2FpMultiple;
			if ((num - point) % ChannelManager.Instance.Tk2FpMultiple > 0)
			{
				num3++;
			}
			if (MyInfoManager.Instance.Cash >= num3)
			{
				int num4 = num3 * ChannelManager.Instance.Tk2FpMultiple;
				int num5 = point + num4;
				tk = num3;
				fp = num4;
				num2 = num5 - num;
			}
		}
		return num2 >= 0;
	}

	private bool DoCashPointReceipt(int percent)
	{
		int price = good.GetPrice(selected, buyHow);
		int num = price - Mathf.CeilToInt((float)price * ((float)percent / 100f));
		int cash = MyInfoManager.Instance.Cash;
		int num2 = cash - num;
		return num2 >= 0;
	}

	private bool DoReceipt(int percent)
	{
		bool result = false;
		switch (buyHow)
		{
		case Good.BUY_HOW.GENERAL_POINT:
			result = DoGeneralPointReceipt(percent);
			break;
		case Good.BUY_HOW.BRICK_POINT:
			result = DoBrickPointReceipt(percent);
			break;
		case Good.BUY_HOW.CASH_POINT:
			result = DoCashPointReceipt(percent);
			break;
		}
		if (good.tItem.type == TItem.TYPE.WEAPON)
		{
			DoWeaponDiscount();
		}
		return result;
	}

	private void VerifyBuyHow(int percent)
	{
		if (!BuildOption.Instance.Props.usePriceDiscount)
		{
			percent = 0;
		}
		options = good.GetOptionStrings(buyHow, percent);
		if (buyHow == Good.BUY_HOW.GENERAL_POINT && !good.CanBuy(Good.BUY_HOW.GENERAL_POINT, rebuy >= 0))
		{
			buyHow = Good.BUY_HOW.BRICK_POINT;
			ResetSelectedOption();
		}
		if (buyHow == Good.BUY_HOW.BRICK_POINT && !good.CanBuy(Good.BUY_HOW.BRICK_POINT, rebuy >= 0))
		{
			buyHow = Good.BUY_HOW.CASH_POINT;
			ResetSelectedOption();
		}
		if (buyHow == Good.BUY_HOW.CASH_POINT && !good.CanBuy(Good.BUY_HOW.CASH_POINT, rebuy >= 0))
		{
			buyHow = Good.BUY_HOW.GENERAL_POINT;
			ResetSelectedOption();
		}
		if (selected >= options.Length)
		{
			selected = options.Length - 1;
		}
		if (selected < 0)
		{
			selected = 0;
		}
	}

	private void ResetSelectedOption()
	{
		selected = good.GetDefaultPriceSel(buyHow);
	}

	private void DoBuyHow(int percent)
	{
		LabelUtil.TextOut(crdBuyHowLabel, StringMgr.Instance.Get("BUY_HOW"), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdOptionSelectLabel, StringMgr.Instance.Get("OPTION_SELECT"), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		VerifyBuyHow(percent);
		switch (buyHow)
		{
		case Good.BUY_HOW.GENERAL_POINT:
			flag = true;
			break;
		case Good.BUY_HOW.BRICK_POINT:
			flag2 = true;
			break;
		case Good.BUY_HOW.CASH_POINT:
			flag3 = true;
			break;
		}
		Rect rect = crdGeneralPoint;
		GUI.enabled = good.CanBuy(Good.BUY_HOW.GENERAL_POINT, rebuy >= 0);
		int levelMixLank = XpManager.Instance.GetLevelMixLank(MyInfoManager.Instance.Xp, MyInfoManager.Instance.Rank);
		bool flag4 = false;
		if (BuildOption.Instance.Props.itemBuyLimit && levelMixLank < good.minlvFp)
		{
			flag4 = true;
		}
		if (flag4)
		{
			GUI.enabled = false;
		}
		bool flag5 = GUI.Toggle(rect, flag, StringMgr.Instance.Get("GENERAL_POINT"));
		if (flag4)
		{
			GUI.enabled = true;
			Texture2D badge = XpManager.Instance.GetBadge(good.minlvFp);
			string rank = XpManager.Instance.GetRank(good.minlvFp);
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
		bool flag6 = false;
		if (BuildOption.Instance.Props.useBrickPoint)
		{
			GUI.enabled = good.CanBuy(Good.BUY_HOW.BRICK_POINT, rebuy >= 0);
			flag6 = GUI.Toggle(rect, flag2, StringMgr.Instance.Get("BRICK_POINT"));
			rect.y += 50f;
		}
		GUI.enabled = good.CanBuy(Good.BUY_HOW.CASH_POINT, rebuy >= 0);
		flag4 = false;
		if (BuildOption.Instance.Props.itemBuyLimit && levelMixLank < good.minlvTk)
		{
			flag4 = true;
		}
		if (flag4)
		{
			GUI.enabled = false;
		}
		bool flag7 = GUI.Toggle(rect, flag3, TokenManager.Instance.GetTokenString());
		if (flag4)
		{
			GUI.enabled = true;
			Texture2D badge2 = XpManager.Instance.GetBadge(good.minlvTk);
			string rank2 = XpManager.Instance.GetRank(good.minlvTk);
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
		if (options == null || (!flag && flag5))
		{
			buyHow = Good.BUY_HOW.GENERAL_POINT;
			ResetSelectedOption();
		}
		if (options == null || (!flag2 && flag6))
		{
			buyHow = Good.BUY_HOW.BRICK_POINT;
			ResetSelectedOption();
		}
		if (options == null || (!flag3 && flag7))
		{
			buyHow = Good.BUY_HOW.CASH_POINT;
			ResetSelectedOption();
		}
	}

	private void DoOptions()
	{
		Vector2 vector = crdOption;
		for (int i = 0; i < options.Length; i++)
		{
			bool flag = selected == i;
			bool flag2 = flag;
			flag = GUI.Toggle(new Rect(vector.x, vector.y, 21f, 22f), flag, options[i]);
			if (!flag2 && flag)
			{
				selected = i;
			}
			vector.y += 30f;
		}
	}

	public override bool DoDialog()
	{
		bool result = false;
		int percent = 0;
		Color txtMainColor = GlobalVars.Instance.txtMainColor;
		TItem tItem = good.tItem;
		LabelUtil.TextOut(crdTitle, StringMgr.Instance.Get("BUY"), "BigLabel", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		LabelUtil.TextOut(crdItemName, tItem.Name, "BigLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		TextureUtil.DrawTexture(crdIcon, tItem.CurIcon(), ScaleMode.ScaleToFit);
		GUI.Label(crdComment, StringMgr.Instance.Get(tItem.comment), "MissionLabel");
		switch (tItem.type)
		{
		case TItem.TYPE.WEAPON:
			percent = DoWeapon((TWeapon)tItem);
			break;
		case TItem.TYPE.CLOTH:
			percent = DoCostume((TCostume)tItem);
			break;
		case TItem.TYPE.ACCESSORY:
			percent = DoAccessory((TAccessory)tItem);
			break;
		case TItem.TYPE.CHARACTER:
			percent = DoCharacter((TCharacter)tItem);
			break;
		case TItem.TYPE.BUNDLE:
			percent = DoBundle((TBundle)tItem);
			break;
		case TItem.TYPE.SPECIAL:
			DoSpecial((TSpecial)tItem);
			break;
		}
		if (good.GetCashback() > 0)
		{
			currenty += 5f;
			LabelUtil.TextOut(new Vector2(crdExtra.x - 18f, currenty), "+" + good.GetCashback().ToString("n0"), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			TextureUtil.DrawTexture(new Rect(crdExtra.x - 18f, currenty + 4f, (float)iconPoint.width, (float)iconPoint.height), iconPoint, ScaleMode.StretchToFill);
		}
		GUI.Box(crdLine, string.Empty, "DivideLine");
		DoBuyHow(percent);
		DoOptions();
		bool flag = DoReceipt(percent);
		if (rebuy >= 0)
		{
			if (GlobalVars.Instance.MyButton(crdBuyButton, StringMgr.Instance.Get("REBUY"), "BtnAction"))
			{
				if (!flag)
				{
					int tk = 0;
					int fp = 0;
					if (buyHow == Good.BUY_HOW.GENERAL_POINT && DoAutochargedGeneralPointReceipt(0, ref tk, ref fp))
					{
						((BuyConfirmDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.BUY_CONFIRM, exclusive: true))?.InitDialog(rebuy, good, buyHow, selected, string.Format(StringMgr.Instance.Get("AUTO_CHARGING_PURCHASE"), tk, fp, TokenManager.Instance.GetTokenString()));
					}
					else
					{
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NOT_ENOUGH_MONEY"));
					}
				}
				else
				{
					((BuyConfirmDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.BUY_CONFIRM, exclusive: true))?.InitDialog(rebuy, good, buyHow, selected);
				}
			}
		}
		else
		{
			GUIContent content = new GUIContent(StringMgr.Instance.Get("PRESENT").ToUpper(), GlobalVars.Instance.iconGift);
			bool enabled = GUI.enabled;
			GUI.enabled = ((buyHow == Good.BUY_HOW.CASH_POINT || BuildOption.Instance.Props.usePointGiftAble) && good.isGiftable);
			if (GlobalVars.Instance.MyButton3(crdPresentButton, content, "BtnAction"))
			{
				if (!flag)
				{
					int tk2 = 0;
					int fp2 = 0;
					if (buyHow == Good.BUY_HOW.GENERAL_POINT && DoAutochargedGeneralPointReceipt(0, ref tk2, ref fp2))
					{
						((SendMemoDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.SEND_MEMO, exclusive: true))?.InitDialog(good, buyHow, selected, string.Format(StringMgr.Instance.Get("AUTO_CHARGING_PURCHASE"), tk2, fp2, TokenManager.Instance.GetTokenString()));
					}
					else
					{
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NOT_ENOUGH_MONEY"));
					}
				}
				else
				{
					((SendMemoDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.SEND_MEMO, exclusive: true))?.InitDialog(good, buyHow, selected);
				}
			}
			GUI.enabled = enabled;
			content = new GUIContent(StringMgr.Instance.Get("BUY").ToUpper(), GlobalVars.Instance.iconCart);
			if (GlobalVars.Instance.MyButton3(crdBuyButton, content, "BtnAction"))
			{
				if (!flag)
				{
					int tk3 = 0;
					int fp3 = 0;
					if (buyHow == Good.BUY_HOW.GENERAL_POINT && DoAutochargedGeneralPointReceipt(0, ref tk3, ref fp3))
					{
						((BuyConfirmDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.BUY_CONFIRM, exclusive: true))?.InitDialog(good, buyHow, selected, string.Format(StringMgr.Instance.Get("AUTO_CHARGING_PURCHASE"), tk3, fp3, TokenManager.Instance.GetTokenString()));
					}
					else
					{
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NOT_ENOUGH_MONEY"));
					}
				}
				else
				{
					((BuyConfirmDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.BUY_CONFIRM, exclusive: true))?.InitDialog(good, buyHow, selected);
				}
			}
		}
		if (BuildOption.Instance.IsNetmarble || BuildOption.Instance.IsDeveloper)
		{
			Vector2 pos = new Vector2(size.x / 2f, 310f);
			LabelUtil.TextOut(pos, StringMgr.Instance.Get("PURCHASE_POLICY"), "MissionLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter, 360f);
			Rect rc = new Rect(size.x - 150f, 310f, 120f, 34f);
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
		if (GlobalVars.Instance.MyButton(crdCloseBtn, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
		}
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		return result;
	}

	public void InitDialog(long _rebuy, Good _good)
	{
		rebuy = _rebuy;
		good = _good;
		Init();
	}

	public void InitDialog(Good _good)
	{
		rebuy = -1L;
		good = _good;
		Init();
	}

	private void Init()
	{
		bool flag = true;
		int level = XpManager.Instance.GetLevel(MyInfoManager.Instance.Xp);
		int[] array = new int[3]
		{
			good.minlvFp,
			0,
			good.minlvTk
		};
		for (int i = 0; i < 3; i++)
		{
			if (!BuildOption.Instance.Props.useBrickPoint && i == 1)
			{
				canBuyItemFromLv[i] = false;
			}
			else if (good.CanBuy((Good.BUY_HOW)i, rebuy >= 0))
			{
				if (level >= array[i])
				{
					canBuyItemFromLv[i] = true;
					if (flag)
					{
						buyHow = (Good.BUY_HOW)i;
						flag = false;
					}
				}
				else
				{
					canBuyItemFromLv[i] = false;
				}
			}
		}
		options = null;
		selected = -1;
		if (good.IsCashable)
		{
			buyHow = Good.BUY_HOW.CASH_POINT;
		}
	}

	private void DoGunProperty(TWeapon tWeapon, Gun gun, Scope scope, Aim aim)
	{
		Weapon component = tWeapon.CurPrefab().GetComponent<Weapon>();
		if (null == gun || null == component)
		{
			Debug.LogError("Error, Fail to get gun component from prefab ");
		}
		else
		{
			WpnMod wpnMod = WeaponModifier.Instance.Get((int)gun.weaponBy);
			float num = (wpnMod?.fAtkPow ?? gun.AtkPow) / maxAtkPow;
			float num2 = (wpnMod?.fRateOfFire ?? gun.rateOfFire) / maxRpm;
			float num3 = (wpnMod?.fRecoilPitch ?? gun.recoilPitch) / maxRecoilPitch;
			float num4 = (wpnMod?.fSpeedFactor ?? gun.speedFactor) / maxMobility;
			float num5 = 1f;
			float num6 = (!(null == scope)) ? (scope.accuracy.accuracy / 100f) : (aim.accuracy.accuracy / 100f);
			if (wpnMod != null)
			{
				num6 = ((!(null == scope)) ? (wpnMod.fZAccuracy / 100f) : (wpnMod.fAccuracy / 100f));
			}
			if (num5 < 0f)
			{
				num5 = 0f;
			}
			float num7 = num * 100f;
			float num8 = num2 * 100f;
			float num9 = num3 * 100f;
			float num10 = num4 * 100f;
			float num11 = num5 * 100f;
			float num12 = (float)(wpnMod?.maxAmmo ?? gun.maxAmmo);
			float num13 = (!(null == scope)) ? scope.accuracy.accuracy : aim.accuracy.accuracy;
			if (wpnMod != null)
			{
				num13 = ((!(null == scope)) ? wpnMod.fZAccuracy : wpnMod.fAccuracy);
			}
			float num14 = wpnMod?.fRange ?? component.range;
			float num15 = wpnMod?.effectiveRange ?? component.effectiveRange;
			if (num > 1f)
			{
				num = 1f;
			}
			if (num2 > 1f)
			{
				num2 = 1f;
			}
			if (num3 > 1f)
			{
				num3 = 1f;
			}
			if (num4 > 1f)
			{
				num4 = 1f;
			}
			if (num5 > 1f)
			{
				num5 = 1f;
			}
			if (num6 > 1f)
			{
				num6 = 1f;
			}
			LabelUtil.TextOut(crdWpnLabel1, StringMgr.Instance.Get("ATTACK_POWER"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(crdWpnLabel2, StringMgr.Instance.Get("ACCURACY"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(crdWpnLabel3, StringMgr.Instance.Get("RECOILPITCH"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(crdWpnLabel4, StringMgr.Instance.Get("RPM"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(crdWpnLabel5, StringMgr.Instance.Get("BULLET_COUNT"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(crdWpnLabel6, StringMgr.Instance.Get("MOBILITY"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			if (BuildOption.Instance.Props.useDurability)
			{
				LabelUtil.TextOut(crdWpnLabel7, StringMgr.Instance.Get("DURABILITY"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			}
			LabelUtil.TextOut(crdWpnLabel8, StringMgr.Instance.Get("EFFECTIVE_RANGE"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			Color color = GUI.color;
			GUI.color = Color.yellow;
			TextureUtil.DrawTexture(crdWpnGaugeOutline1, gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(crdWpnGaugeOutline2, gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(crdWpnGaugeOutline3, gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(crdWpnGaugeOutline4, gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(crdWpnGaugeOutline5, gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(crdWpnGaugeOutline6, gaugeFrame, ScaleMode.StretchToFill);
			if (BuildOption.Instance.Props.useDurability)
			{
				TextureUtil.DrawTexture(crdWpnGaugeOutline7, gaugeFrame, ScaleMode.StretchToFill);
			}
			TextureUtil.DrawTexture(crdWpnGaugeOutline8, gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(crdWpnGauge1.x, crdWpnGauge1.y, crdWpnGauge1.width * num, crdWpnGauge1.height), gauge, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(crdWpnGauge2.x, crdWpnGauge2.y, crdWpnGauge2.width * num6, crdWpnGauge2.height), gauge, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(crdWpnGauge3.x, crdWpnGauge3.y, crdWpnGauge3.width * num3, crdWpnGauge3.height), gauge, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(crdWpnGauge4.x, crdWpnGauge4.y, crdWpnGauge4.width * num2, crdWpnGauge4.height), gauge, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(crdWpnGauge6.x, crdWpnGauge6.y, crdWpnGauge6.width * num4, crdWpnGauge6.height), gauge, ScaleMode.StretchToFill);
			if (BuildOption.Instance.Props.useDurability)
			{
				TextureUtil.DrawTexture(new Rect(crdWpnGauge7.x, crdWpnGauge7.y, crdWpnGauge7.width * num5, crdWpnGauge7.height), gauge, ScaleMode.StretchToFill);
			}
			GUI.color = color;
			LabelUtil.TextOut(crdWpnValue1, num7.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(crdWpnValue2, num13.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(crdWpnValue3, num9.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(crdWpnValue4, num8.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(crdWpnValue5, num12.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(crdWpnValue6, num10.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			if (BuildOption.Instance.Props.useDurability)
			{
				LabelUtil.TextOut(crdWpnValue7, num11.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
			LabelUtil.TextOut(crdWpnValue8, num15.ToString() + "/" + num14.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
	}

	private void DoMeleeProperty(TWeapon tWeapon, MeleeWeapon melee)
	{
		if (null == melee)
		{
			Debug.LogError("Error, Fail to get MeleeWeapon component from prefab ");
		}
		else
		{
			WpnMod wpnMod = WeaponModifier.Instance.Get((int)melee.weaponBy);
			float num = (wpnMod?.fAtkPow ?? melee.AtkPow) / maxAtkPow;
			float num2 = (wpnMod?.fSlashSpeed ?? melee.slashSpeed) / maxSlashSpeed;
			float num3 = (wpnMod?.fSpeedFactor ?? melee.speedFactor) / maxMobility;
			float num4 = 1f;
			float num5 = 1f;
			float num6 = num * 100f;
			float num7 = num2 * 100f;
			float num8 = num5 * 100f;
			float num9 = num3 * 100f;
			float num10 = num4 * 100f;
			if (num > 1f)
			{
				num = 1f;
			}
			if (num3 > 1f)
			{
				num3 = 1f;
			}
			if (num4 > 1f)
			{
				num4 = 1f;
			}
			if (num2 > 1f)
			{
				num2 = 1f;
			}
			LabelUtil.TextOut(crdWpnLabel1, StringMgr.Instance.Get("ATTACK_POWER"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(crdWpnLabel2, StringMgr.Instance.Get("ACCURACY"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(crdWpnLabel3, StringMgr.Instance.Get("ATK_SPEED"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(crdWpnLabel4, StringMgr.Instance.Get("BULLET_COUNT"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(crdWpnLabel5, StringMgr.Instance.Get("MOBILITY"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			if (BuildOption.Instance.Props.useDurability)
			{
				LabelUtil.TextOut(crdWpnLabel6, StringMgr.Instance.Get("DURABILITY"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			}
			Color color = GUI.color;
			GUI.color = Color.yellow;
			TextureUtil.DrawTexture(crdWpnGaugeOutline1, gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(crdWpnGaugeOutline2, gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(crdWpnGaugeOutline3, gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(crdWpnGaugeOutline4, gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(crdWpnGaugeOutline5, gaugeFrame, ScaleMode.StretchToFill);
			if (BuildOption.Instance.Props.useDurability)
			{
				TextureUtil.DrawTexture(crdWpnGaugeOutline6, gaugeFrame, ScaleMode.StretchToFill);
			}
			TextureUtil.DrawTexture(new Rect(crdWpnGauge1.x, crdWpnGauge1.y, crdWpnGauge1.width * num, crdWpnGauge1.height), gauge, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(crdWpnGauge2.x, crdWpnGauge2.y, crdWpnGauge2.width * num5, crdWpnGauge2.height), gauge, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(crdWpnGauge3.x, crdWpnGauge3.y, crdWpnGauge3.width * num2, crdWpnGauge3.height), gauge, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(crdWpnGauge5.x, crdWpnGauge5.y, crdWpnGauge5.width * num3, crdWpnGauge5.height), gauge, ScaleMode.StretchToFill);
			if (BuildOption.Instance.Props.useDurability)
			{
				TextureUtil.DrawTexture(new Rect(crdWpnGauge6.x, crdWpnGauge6.y, crdWpnGauge6.width * num4, crdWpnGauge6.height), gauge, ScaleMode.StretchToFill);
			}
			GUI.color = color;
			LabelUtil.TextOut(crdWpnValue1, num6.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(crdWpnValue2, num8.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(crdWpnValue3, num7.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(crdWpnValue4, StringMgr.Instance.Get("INFINITE"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(crdWpnValue5, num9.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			if (BuildOption.Instance.Props.useDurability)
			{
				LabelUtil.TextOut(crdWpnValue6, num10.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
		}
	}

	private void DoGrenade(TWeapon tWeapon, Grenade grenade)
	{
		if (null != grenade)
		{
			WpnMod wpnMod = WeaponModifier.Instance.Get((int)grenade.weaponBy);
			int maxAmmo = (wpnMod != null) ? (maxAmmo = wpnMod.maxAmmo) : (maxAmmo = grenade.maxAmmo);
			float num = (wpnMod?.fAtkPow ?? grenade.AtkPow) / maxAtkPow;
			float num2 = (wpnMod?.fSpeedFactor ?? grenade.speedFactor) / maxMobility;
			float num3 = grenade.Radius / maxRange;
			float num4 = 1f;
			if (num > 1f)
			{
				num = 1f;
			}
			if (num3 > 1f)
			{
				num3 = 1f;
			}
			if (num2 > 1f)
			{
				num2 = 1f;
			}
			if (num4 > 1f)
			{
				num4 = 1f;
			}
			float num5 = num * 100f;
			float num6 = num3 * 100f;
			float num7 = num2 * 100f;
			float num8 = num4 * 100f;
			LabelUtil.TextOut(crdWpnLabel1, StringMgr.Instance.Get("ATTACK_POWER"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(crdWpnLabel2, StringMgr.Instance.Get("EFFECT_RADIUS"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(crdWpnLabel3, StringMgr.Instance.Get("BULLET_COUNT"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(crdWpnLabel4, StringMgr.Instance.Get("MOBILITY"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			if (BuildOption.Instance.Props.useDurability)
			{
				LabelUtil.TextOut(crdWpnLabel5, StringMgr.Instance.Get("DURABILITY"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			}
			Color color = GUI.color;
			GUI.color = Color.yellow;
			TextureUtil.DrawTexture(crdWpnGaugeOutline1, gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(crdWpnGaugeOutline2, gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(crdWpnGaugeOutline3, gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(crdWpnGaugeOutline4, gaugeFrame, ScaleMode.StretchToFill);
			if (BuildOption.Instance.Props.useDurability)
			{
				TextureUtil.DrawTexture(crdWpnGaugeOutline5, gaugeFrame, ScaleMode.StretchToFill);
			}
			TextureUtil.DrawTexture(new Rect(crdWpnGauge1.x, crdWpnGauge1.y, crdWpnGauge1.width * num, crdWpnGauge1.height), gauge, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(crdWpnGauge2.x, crdWpnGauge2.y, crdWpnGauge2.width * num3, crdWpnGauge2.height), gauge, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(crdWpnGauge4.x, crdWpnGauge4.y, crdWpnGauge4.width * num2, crdWpnGauge4.height), gauge, ScaleMode.StretchToFill);
			if (BuildOption.Instance.Props.useDurability)
			{
				TextureUtil.DrawTexture(new Rect(crdWpnGauge5.x, crdWpnGauge5.y, crdWpnGauge5.width * num4, crdWpnGauge5.height), gauge, ScaleMode.StretchToFill);
			}
			GUI.color = color;
			LabelUtil.TextOut(crdWpnValue1, num5.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(crdWpnValue2, num6.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(crdWpnValue3, maxAmmo.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(crdWpnValue4, num7.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			if (BuildOption.Instance.Props.useDurability)
			{
				LabelUtil.TextOut(crdWpnValue5, num8.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
		}
	}

	private void DoFlashbang(TWeapon tWeapon, FlashBang grenade)
	{
		if (null != grenade)
		{
			WpnMod wpnMod = WeaponModifier.Instance.Get((int)grenade.weaponBy);
			int maxAmmo = (wpnMod != null) ? (maxAmmo = wpnMod.maxAmmo) : (maxAmmo = grenade.maxAmmo);
			float num = GlobalVars.Instance.maxDistanceFlashbang / maxRangeBang;
			float num2 = (wpnMod?.explosionTime ?? grenade.explosionTime) / maxEffTime;
			float num3 = (wpnMod?.fSpeedFactor ?? grenade.speedFactor) / maxMobility;
			float num4 = 1f;
			if (num2 > 1f)
			{
				num2 = 1f;
			}
			if (num > 1f)
			{
				num = 1f;
			}
			if (num3 > 1f)
			{
				num3 = 1f;
			}
			float num5 = num2 * 100f;
			float num6 = num * 100f;
			float num7 = num3 * 100f;
			float num8 = num4 * 100f;
			LabelUtil.TextOut(crdWpnLabel1, StringMgr.Instance.Get("EFFECT_RADIUS"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(crdWpnLabel2, StringMgr.Instance.Get("EFFECT_TIME"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(crdWpnLabel3, StringMgr.Instance.Get("BULLET_COUNT"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(crdWpnLabel4, StringMgr.Instance.Get("MOBILITY"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			if (BuildOption.Instance.Props.useDurability)
			{
				LabelUtil.TextOut(crdWpnLabel5, StringMgr.Instance.Get("DURABILITY"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			}
			Color color = GUI.color;
			GUI.color = Color.yellow;
			TextureUtil.DrawTexture(crdWpnGaugeOutline1, gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(crdWpnGaugeOutline2, gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(crdWpnGaugeOutline3, gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(crdWpnGaugeOutline4, gaugeFrame, ScaleMode.StretchToFill);
			if (BuildOption.Instance.Props.useDurability)
			{
				TextureUtil.DrawTexture(crdWpnGaugeOutline5, gaugeFrame, ScaleMode.StretchToFill);
			}
			TextureUtil.DrawTexture(new Rect(crdWpnGauge1.x, crdWpnGauge1.y, crdWpnGauge1.width * num, crdWpnGauge1.height), gauge, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(crdWpnGauge2.x, crdWpnGauge2.y, crdWpnGauge2.width * num2, crdWpnGauge2.height), gauge, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(crdWpnGauge4.x, crdWpnGauge4.y, crdWpnGauge4.width * num3, crdWpnGauge4.height), gauge, ScaleMode.StretchToFill);
			if (BuildOption.Instance.Props.useDurability)
			{
				TextureUtil.DrawTexture(new Rect(crdWpnGauge5.x, crdWpnGauge5.y, crdWpnGauge5.width * num4, crdWpnGauge5.height), gauge, ScaleMode.StretchToFill);
			}
			GUI.color = color;
			LabelUtil.TextOut(crdWpnValue1, num6.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(crdWpnValue2, num5.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(crdWpnValue3, maxAmmo.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(crdWpnValue4, num7.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			if (BuildOption.Instance.Props.useDurability)
			{
				LabelUtil.TextOut(crdWpnValue5, num8.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
		}
	}

	private void DoSmokeGrenade(TWeapon tWeapon, SmokeGrenade grenade)
	{
		if (null != grenade)
		{
			WpnMod wpnMod = WeaponModifier.Instance.Get((int)grenade.weaponBy);
			int maxAmmo = (wpnMod != null) ? (maxAmmo = wpnMod.maxAmmo) : (maxAmmo = grenade.maxAmmo);
			float num = grenade.persistTime / maxEffTime;
			float num2 = (wpnMod?.fSpeedFactor ?? grenade.speedFactor) / maxMobility;
			float num3 = 1f;
			if (num > 1f)
			{
				num = 1f;
			}
			if (num2 > 1f)
			{
				num2 = 1f;
			}
			if (num3 > 1f)
			{
				num3 = 1f;
			}
			float num4 = num * 100f;
			float num5 = num2 * 100f;
			float num6 = num3 * 100f;
			LabelUtil.TextOut(crdWpnLabel1, StringMgr.Instance.Get("EFFECT_TIME"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(crdWpnLabel2, StringMgr.Instance.Get("BULLET_COUNT"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(crdWpnLabel3, StringMgr.Instance.Get("MOBILITY"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			if (BuildOption.Instance.Props.useDurability)
			{
				LabelUtil.TextOut(crdWpnLabel4, StringMgr.Instance.Get("DURABILITY"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			}
			Color color = GUI.color;
			GUI.color = Color.yellow;
			TextureUtil.DrawTexture(crdWpnGaugeOutline1, gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(crdWpnGaugeOutline2, gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(crdWpnGaugeOutline3, gaugeFrame, ScaleMode.StretchToFill);
			if (BuildOption.Instance.Props.useDurability)
			{
				TextureUtil.DrawTexture(crdWpnGaugeOutline4, gaugeFrame, ScaleMode.StretchToFill);
			}
			TextureUtil.DrawTexture(new Rect(crdWpnGauge1.x, crdWpnGauge1.y, crdWpnGauge1.width * num, crdWpnGauge1.height), gauge, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(crdWpnGauge3.x, crdWpnGauge3.y, crdWpnGauge3.width * num2, crdWpnGauge3.height), gauge, ScaleMode.StretchToFill);
			if (BuildOption.Instance.Props.useDurability)
			{
				TextureUtil.DrawTexture(new Rect(crdWpnGauge4.x, crdWpnGauge4.y, crdWpnGauge4.width * num3, crdWpnGauge4.height), gauge, ScaleMode.StretchToFill);
			}
			GUI.color = color;
			LabelUtil.TextOut(crdWpnValue1, num4.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(crdWpnValue2, maxAmmo.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(crdWpnValue3, num5.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			if (BuildOption.Instance.Props.useDurability)
			{
				LabelUtil.TextOut(crdWpnValue4, num6.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
		}
	}

	private void DoSenseBomb(TWeapon tWeapon, SenseBomb grenade)
	{
		if (null != grenade)
		{
			WpnMod wpnMod = WeaponModifier.Instance.Get((int)grenade.weaponBy);
			int maxAmmo = (wpnMod != null) ? (maxAmmo = wpnMod.maxAmmo) : (maxAmmo = grenade.maxAmmo);
			float num = (wpnMod?.fAtkPow ?? grenade.AtkPow) / maxAtkPow;
			float num2 = (wpnMod?.fSpeedFactor ?? grenade.speedFactor) / maxMobility;
			float num3 = grenade.Radius / maxRange;
			float num4 = 1f;
			if (num > 1f)
			{
				num = 1f;
			}
			if (num3 > 1f)
			{
				num3 = 1f;
			}
			if (num2 > 1f)
			{
				num2 = 1f;
			}
			if (num4 > 1f)
			{
				num4 = 1f;
			}
			float num5 = num * 100f;
			float num6 = num3 * 100f;
			float num7 = num2 * 100f;
			float num8 = num4 * 100f;
			LabelUtil.TextOut(crdWpnLabel1, StringMgr.Instance.Get("ATTACK_POWER"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(crdWpnLabel2, StringMgr.Instance.Get("EFFECT_RADIUS"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(crdWpnLabel3, StringMgr.Instance.Get("BULLET_COUNT"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(crdWpnLabel4, StringMgr.Instance.Get("MOBILITY"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			if (BuildOption.Instance.Props.useDurability)
			{
				LabelUtil.TextOut(crdWpnLabel5, StringMgr.Instance.Get("DURABILITY"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			}
			Color color = GUI.color;
			GUI.color = Color.yellow;
			TextureUtil.DrawTexture(crdWpnGaugeOutline1, gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(crdWpnGaugeOutline2, gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(crdWpnGaugeOutline3, gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(crdWpnGaugeOutline4, gaugeFrame, ScaleMode.StretchToFill);
			if (BuildOption.Instance.Props.useDurability)
			{
				TextureUtil.DrawTexture(crdWpnGaugeOutline5, gaugeFrame, ScaleMode.StretchToFill);
			}
			TextureUtil.DrawTexture(new Rect(crdWpnGauge1.x, crdWpnGauge1.y, crdWpnGauge1.width * num, crdWpnGauge1.height), gauge, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(crdWpnGauge2.x, crdWpnGauge2.y, crdWpnGauge2.width * num3, crdWpnGauge2.height), gauge, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(crdWpnGauge4.x, crdWpnGauge4.y, crdWpnGauge4.width * num2, crdWpnGauge4.height), gauge, ScaleMode.StretchToFill);
			if (BuildOption.Instance.Props.useDurability)
			{
				TextureUtil.DrawTexture(new Rect(crdWpnGauge5.x, crdWpnGauge5.y, crdWpnGauge5.width * num4, crdWpnGauge5.height), gauge, ScaleMode.StretchToFill);
			}
			GUI.color = color;
			LabelUtil.TextOut(crdWpnValue1, num5.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(crdWpnValue2, num6.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(crdWpnValue3, maxAmmo.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(crdWpnValue4, num7.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			if (BuildOption.Instance.Props.useDurability)
			{
				LabelUtil.TextOut(crdWpnValue5, num8.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
		}
	}

	private void DoPoisonBomb(TWeapon tWeapon, PoisonBomb grenade)
	{
		if (null != grenade)
		{
			WpnMod wpnMod = WeaponModifier.Instance.Get((int)grenade.weaponBy);
			int maxAmmo = (wpnMod != null) ? (maxAmmo = wpnMod.maxAmmo) : (maxAmmo = grenade.maxAmmo);
			float num = (wpnMod?.fAtkPow ?? grenade.AtkPow) / maxAtkPow;
			float num2 = (wpnMod?.fSpeedFactor ?? grenade.speedFactor) / maxMobility;
			float num3 = grenade.Radius / maxRange;
			float num4 = 1f;
			if (num > 1f)
			{
				num = 1f;
			}
			if (num3 > 1f)
			{
				num3 = 1f;
			}
			if (num2 > 1f)
			{
				num2 = 1f;
			}
			if (num4 > 1f)
			{
				num4 = 1f;
			}
			float num5 = num * 100f;
			float num6 = num3 * 100f;
			float num7 = num2 * 100f;
			float num8 = num4 * 100f;
			LabelUtil.TextOut(crdWpnLabel1, StringMgr.Instance.Get("ATTACK_POWER"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(crdWpnLabel2, StringMgr.Instance.Get("EFFECT_RADIUS"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(crdWpnLabel3, StringMgr.Instance.Get("BULLET_COUNT"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(crdWpnLabel4, StringMgr.Instance.Get("MOBILITY"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			if (BuildOption.Instance.Props.useDurability)
			{
				LabelUtil.TextOut(crdWpnLabel5, StringMgr.Instance.Get("DURABILITY"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			}
			Color color = GUI.color;
			GUI.color = Color.yellow;
			TextureUtil.DrawTexture(crdWpnGaugeOutline1, gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(crdWpnGaugeOutline2, gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(crdWpnGaugeOutline3, gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(crdWpnGaugeOutline4, gaugeFrame, ScaleMode.StretchToFill);
			if (BuildOption.Instance.Props.useDurability)
			{
				TextureUtil.DrawTexture(crdWpnGaugeOutline5, gaugeFrame, ScaleMode.StretchToFill);
			}
			TextureUtil.DrawTexture(new Rect(crdWpnGauge1.x, crdWpnGauge1.y, crdWpnGauge1.width * num, crdWpnGauge1.height), gauge, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(crdWpnGauge2.x, crdWpnGauge2.y, crdWpnGauge2.width * num3, crdWpnGauge2.height), gauge, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(crdWpnGauge4.x, crdWpnGauge4.y, crdWpnGauge4.width * num2, crdWpnGauge4.height), gauge, ScaleMode.StretchToFill);
			if (BuildOption.Instance.Props.useDurability)
			{
				TextureUtil.DrawTexture(new Rect(crdWpnGauge5.x, crdWpnGauge5.y, crdWpnGauge5.width * num4, crdWpnGauge5.height), gauge, ScaleMode.StretchToFill);
			}
			GUI.color = color;
			LabelUtil.TextOut(crdWpnValue1, num5.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(crdWpnValue2, num6.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(crdWpnValue3, maxAmmo.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(crdWpnValue4, num7.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			if (BuildOption.Instance.Props.useDurability)
			{
				LabelUtil.TextOut(crdWpnValue5, num8.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
		}
	}

	private void DoXmasBomb(TWeapon tWeapon, XmasBomb grenade)
	{
		if (null != grenade)
		{
			WpnMod wpnMod = WeaponModifier.Instance.Get((int)grenade.weaponBy);
			int maxAmmo = (wpnMod != null) ? (maxAmmo = wpnMod.maxAmmo) : (maxAmmo = grenade.maxAmmo);
			float num = grenade.persistTime / maxEffTime;
			float num2 = (wpnMod?.fAtkPow ?? grenade.AtkPow) / maxAtkPow;
			float num3 = (wpnMod?.fSpeedFactor ?? grenade.speedFactor) / maxMobility;
			float num4 = grenade.Radius / maxRange;
			float num5 = 1f;
			if (num > 1f)
			{
				num = 1f;
			}
			if (num2 > 1f)
			{
				num2 = 1f;
			}
			if (num4 > 1f)
			{
				num4 = 1f;
			}
			if (num3 > 1f)
			{
				num3 = 1f;
			}
			if (num5 > 1f)
			{
				num5 = 1f;
			}
			float num6 = num * 100f;
			float num7 = num2 * 100f;
			float num8 = num4 * 100f;
			float num9 = num3 * 100f;
			float num10 = num5 * 100f;
			LabelUtil.TextOut(crdWpnLabel1, StringMgr.Instance.Get("EFFECT_TIME"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(crdWpnLabel2, StringMgr.Instance.Get("ATTACK_POWER"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(crdWpnLabel3, StringMgr.Instance.Get("EFFECT_RADIUS"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(crdWpnLabel4, StringMgr.Instance.Get("BULLET_COUNT"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			LabelUtil.TextOut(crdWpnLabel5, StringMgr.Instance.Get("MOBILITY"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			if (BuildOption.Instance.Props.useDurability)
			{
				LabelUtil.TextOut(crdWpnLabel6, StringMgr.Instance.Get("DURABILITY"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			}
			Color color = GUI.color;
			GUI.color = Color.yellow;
			TextureUtil.DrawTexture(crdWpnGaugeOutline1, gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(crdWpnGaugeOutline2, gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(crdWpnGaugeOutline3, gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(crdWpnGaugeOutline4, gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(crdWpnGaugeOutline5, gaugeFrame, ScaleMode.StretchToFill);
			if (BuildOption.Instance.Props.useDurability)
			{
				TextureUtil.DrawTexture(crdWpnGaugeOutline6, gaugeFrame, ScaleMode.StretchToFill);
			}
			TextureUtil.DrawTexture(new Rect(crdWpnGauge1.x, crdWpnGauge1.y, crdWpnGauge1.width * num, crdWpnGauge1.height), gauge, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(crdWpnGauge2.x, crdWpnGauge2.y, crdWpnGauge2.width * num2, crdWpnGauge2.height), gauge, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(crdWpnGauge3.x, crdWpnGauge3.y, crdWpnGauge3.width * num4, crdWpnGauge3.height), gauge, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(crdWpnGauge5.x, crdWpnGauge5.y, crdWpnGauge5.width * num3, crdWpnGauge5.height), gauge, ScaleMode.StretchToFill);
			if (BuildOption.Instance.Props.useDurability)
			{
				TextureUtil.DrawTexture(new Rect(crdWpnGauge6.x, crdWpnGauge6.y, crdWpnGauge6.width * num5, crdWpnGauge6.height), gauge, ScaleMode.StretchToFill);
			}
			GUI.color = color;
			LabelUtil.TextOut(crdWpnValue1, num6.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(crdWpnValue2, num7.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(crdWpnValue3, num8.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(crdWpnValue4, maxAmmo.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(crdWpnValue5, num9.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			if (BuildOption.Instance.Props.useDurability)
			{
				LabelUtil.TextOut(crdWpnValue6, num10.ToString("0.##") + "%", "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
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
				string str = string.Empty;
				switch (tWeapon.cat)
				{
				case 0:
					str = StringMgr.Instance.Get("HEAVY_WPN");
					break;
				case 1:
					str = StringMgr.Instance.Get("ASSAULT_WPN");
					break;
				case 2:
					str = StringMgr.Instance.Get("SNIPER_WPN");
					break;
				case 3:
					str = StringMgr.Instance.Get("SUB_MACHINE_WPN");
					break;
				case 4:
					str = StringMgr.Instance.Get("HAND_GUN_WPN");
					break;
				case 5:
					str = StringMgr.Instance.Get("MELEE_WPN");
					break;
				case 6:
					str = StringMgr.Instance.Get("SPECIAL_WPN");
					break;
				}
				switch (component.slot)
				{
				case Weapon.TYPE.MAIN:
					LabelUtil.TextOut(crdCategory, StringMgr.Instance.Get("MAIN_WEAPON") + "/" + str, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
					DoGunProperty(tWeapon, tWeapon.CurPrefab().GetComponent<Gun>(), tWeapon.CurPrefab().GetComponent<Scope>(), tWeapon.CurPrefab().GetComponent<Aim>());
					break;
				case Weapon.TYPE.AUX:
					LabelUtil.TextOut(crdCategory, StringMgr.Instance.Get("AUX_WEAPON") + "/" + str, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
					DoGunProperty(tWeapon, tWeapon.CurPrefab().GetComponent<Gun>(), tWeapon.CurPrefab().GetComponent<Scope>(), tWeapon.CurPrefab().GetComponent<Aim>());
					break;
				case Weapon.TYPE.MELEE:
					LabelUtil.TextOut(crdCategory, StringMgr.Instance.Get("MELEE_WEAPON") + "/" + str, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
					DoMeleeProperty(tWeapon, tWeapon.CurPrefab().GetComponent<MeleeWeapon>());
					break;
				case Weapon.TYPE.PROJECTILE:
				{
					LabelUtil.TextOut(crdCategory, StringMgr.Instance.Get("SPEC_WEAPON") + "/" + str, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
					Grenade component2 = tWeapon.CurPrefab().GetComponent<Grenade>();
					if (component2 != null)
					{
						DoGrenade(tWeapon, component2);
					}
					FlashBang component3 = tWeapon.CurPrefab().GetComponent<FlashBang>();
					if (component3 != null)
					{
						DoFlashbang(tWeapon, component3);
					}
					SmokeGrenade component4 = tWeapon.CurPrefab().GetComponent<SmokeGrenade>();
					if (component4 != null)
					{
						DoSmokeGrenade(tWeapon, component4);
					}
					SenseBomb component5 = tWeapon.CurPrefab().GetComponent<SenseBomb>();
					if (component5 != null)
					{
						DoSenseBomb(tWeapon, component5);
					}
					XmasBomb component6 = tWeapon.CurPrefab().GetComponent<XmasBomb>();
					if (component6 != null)
					{
						DoXmasBomb(tWeapon, component6);
					}
					PoisonBomb component7 = tWeapon.CurPrefab().GetComponent<PoisonBomb>();
					if (component7 != null)
					{
						DoPoisonBomb(tWeapon, component7);
					}
					break;
				}
				}
			}
		}
		return result;
	}

	private int DoCostume(TCostume tCostume)
	{
		switch (tCostume.slot)
		{
		case TItem.SLOT.UPPER:
			LabelUtil.TextOut(crdCategory, StringMgr.Instance.Get("UPPER_CLOTH"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			break;
		case TItem.SLOT.LOWER:
			LabelUtil.TextOut(crdCategory, StringMgr.Instance.Get("LOWER_CLOTH"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			break;
		}
		Vector2 pos = new Vector2(crdExtra.x, crdExtra.y);
		if (tCostume.armor > 0 && BuildOption.Instance.Props.useArmor)
		{
			LabelUtil.TextOut(crdExtra, string.Format(StringMgr.Instance.Get("ARMOR_UP"), tCostume.armor), "MiniLabel", Color.yellow, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			pos.y += crdExtraOffset;
		}
		if (tCostume.functionMask == 20 && !BuildOption.Instance.IsNetmarbleOrDev)
		{
			LabelUtil.TextOut(pos, string.Format(StringMgr.Instance.Get("HP_COOLTIME_DOWN"), tCostume.functionFactor * 100f), "MiniLabel", Color.yellow, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			pos.y += crdExtraOffset;
		}
		if (tCostume.functionMask == 21)
		{
			LabelUtil.TextOut(pos, string.Format(StringMgr.Instance.Get("MAIN_AMMO_MAX_UP"), tCostume.functionFactor * 100f), "MiniLabel", Color.yellow, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			pos.y += crdExtraOffset;
		}
		if (tCostume.functionMask == 22)
		{
			LabelUtil.TextOut(pos, string.Format(StringMgr.Instance.Get("AUX_AMMO_MAX_UP"), tCostume.functionFactor * 100f), "MiniLabel", Color.yellow, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			pos.y += crdExtraOffset;
		}
		if (tCostume.functionMask == 23)
		{
			LabelUtil.TextOut(pos, string.Format(StringMgr.Instance.Get("GRENADE_AMMO_MAX_UP"), tCostume.functionFactor * 100f), "MiniLabel", Color.yellow, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			pos.y += crdExtraOffset;
		}
		if (tCostume.functionMask == 88)
		{
			LabelUtil.TextOut(pos, string.Format(StringMgr.Instance.Get("DASHTIME"), tCostume.functionFactor * 100f), "MiniLabel", Color.yellow, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			pos.y += crdExtraOffset;
		}
		if (tCostume.functionMask == 89)
		{
			LabelUtil.TextOut(pos, string.Format(StringMgr.Instance.Get("RESPAWNTIME"), tCostume.functionFactor * 100f), "MiniLabel", Color.yellow, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			pos.y += crdExtraOffset;
		}
		if (tCostume.functionMask == 90)
		{
			LabelUtil.TextOut(pos, string.Format(StringMgr.Instance.Get("FALLDAMAGEDEC"), tCostume.functionFactor * 100f), "MiniLabel", Color.yellow, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			pos.y += crdExtraOffset;
		}
		DoBuff(pos, tCostume);
		ConsumableDesc consumableDesc = ConsumableManager.Instance.Get(TItem.FunctionMaskToString(tCostume.functionMask));
		if (consumableDesc != null && consumableDesc.disableByRoomType != null && consumableDesc.disableByRoomType.Length > 0)
		{
			for (int i = 0; i < consumableDesc.disableByRoomType.Length; i++)
			{
				LabelUtil.TextOut(pos, string.Format(StringMgr.Instance.Get("MODE_USE_IMPOSSIBLE"), Room.Type2String((int)consumableDesc.disableByRoomType[i])), "MiniLabel", Color.red, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				pos.y += crdExtraOffset;
			}
		}
		return 0;
	}

	private int DoAccessory(TAccessory tAccessory)
	{
		Accessory component = tAccessory.prefab.GetComponent<Accessory>();
		if (null == component)
		{
			Debug.LogError("Error, Fail to get equip component from accessory prefab ");
		}
		else
		{
			LabelUtil.TextOut(crdCategory, tAccessory.GetKindString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		Vector2 pos = new Vector2(crdExtra.x, crdExtra.y);
		if (tAccessory.armor > 0 && BuildOption.Instance.Props.useArmor)
		{
			LabelUtil.TextOut(pos, string.Format(StringMgr.Instance.Get("ARMOR_UP"), tAccessory.armor), "MiniLabel", Color.yellow, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			pos.y += crdExtraOffset;
		}
		if (tAccessory.functionMask == 20 && !BuildOption.Instance.IsNetmarbleOrDev)
		{
			LabelUtil.TextOut(pos, string.Format(StringMgr.Instance.Get("HP_COOLTIME_DOWN"), tAccessory.functionFactor * 100f), "MiniLabel", Color.yellow, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			pos.y += crdExtraOffset;
		}
		if (tAccessory.functionMask == 21)
		{
			LabelUtil.TextOut(pos, string.Format(StringMgr.Instance.Get("MAIN_AMMO_MAX_UP"), tAccessory.functionFactor * 100f), "MiniLabel", Color.yellow, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			pos.y += crdExtraOffset;
		}
		if (tAccessory.functionMask == 22)
		{
			LabelUtil.TextOut(pos, string.Format(StringMgr.Instance.Get("AUX_AMMO_MAX_UP"), tAccessory.functionFactor * 100f), "MiniLabel", Color.yellow, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			pos.y += crdExtraOffset;
		}
		if (tAccessory.functionMask == 23)
		{
			LabelUtil.TextOut(pos, string.Format(StringMgr.Instance.Get("GRENADE_AMMO_MAX_UP"), tAccessory.functionFactor * 100f), "MiniLabel", Color.yellow, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			pos.y += crdExtraOffset;
		}
		if (tAccessory.functionMask == 113)
		{
			LabelUtil.TextOut(pos, string.Format(StringMgr.Instance.Get("GRENADE_AMMO_MAX_UP02"), tAccessory.functionFactor), "MiniLabel", Color.yellow, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			pos.y += crdExtraOffset;
		}
		if (tAccessory.functionMask == 88)
		{
			LabelUtil.TextOut(pos, string.Format(StringMgr.Instance.Get("DASHTIME"), tAccessory.functionFactor * 100f), "MiniLabel", Color.yellow, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			pos.y += crdExtraOffset;
		}
		if (tAccessory.functionMask == 89)
		{
			LabelUtil.TextOut(pos, string.Format(StringMgr.Instance.Get("RESPAWNTIME"), tAccessory.functionFactor * 100f), "MiniLabel", Color.yellow, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			pos.y += crdExtraOffset;
		}
		if (tAccessory.functionMask == 90)
		{
			LabelUtil.TextOut(pos, string.Format(StringMgr.Instance.Get("FALLDAMAGEDEC"), tAccessory.functionFactor * 100f), "MiniLabel", Color.yellow, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			pos.y += crdExtraOffset;
		}
		DoBuff(pos, tAccessory);
		ConsumableDesc consumableDesc = ConsumableManager.Instance.Get(TItem.FunctionMaskToString(tAccessory.functionMask));
		if (consumableDesc != null && consumableDesc.disableByRoomType != null && consumableDesc.disableByRoomType.Length > 0)
		{
			for (int i = 0; i < consumableDesc.disableByRoomType.Length; i++)
			{
				LabelUtil.TextOut(pos, string.Format(StringMgr.Instance.Get("MODE_USE_IMPOSSIBLE"), Room.Type2String((int)consumableDesc.disableByRoomType[i])), "MiniLabel", Color.red, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				pos.y += crdExtraOffset;
			}
		}
		return 0;
	}

	private int DoCharacter(TCharacter tCharacter)
	{
		Vector2 pos = new Vector2(crdExtra.x, crdExtra.y);
		DoBuff(pos, tCharacter);
		return 0;
	}

	private int DoSpecial(TSpecial tSpecial)
	{
		Vector2 pos = new Vector2(crdExtra.x, crdExtra.y);
		ConsumableDesc consumableDesc = ConsumableManager.Instance.Get(TItem.FunctionMaskToString(tSpecial.functionMask));
		if (consumableDesc != null && consumableDesc.disableByRoomType != null && consumableDesc.disableByRoomType.Length > 0)
		{
			for (int i = 0; i < consumableDesc.disableByRoomType.Length; i++)
			{
				LabelUtil.TextOut(pos, string.Format(StringMgr.Instance.Get("MODE_USE_IMPOSSIBLE"), Room.Type2String((int)consumableDesc.disableByRoomType[i])), "MiniLabel", Color.red, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				pos.y += crdExtraOffset;
			}
		}
		return 0;
	}

	private BundleUnit[] sortBundlePacks(BundleUnit[] packs)
	{
		List<BundleUnit> list = new List<BundleUnit>();
		for (int i = 0; i < packs.Length; i++)
		{
			if (packs[i].tItem.catType == 0)
			{
				list.Add(packs[i]);
			}
		}
		for (int j = 0; j < packs.Length; j++)
		{
			if (packs[j].tItem.catType == 1)
			{
				list.Add(packs[j]);
			}
		}
		for (int k = 0; k < packs.Length; k++)
		{
			if (packs[k].tItem.catType == 2)
			{
				list.Add(packs[k]);
			}
		}
		for (int l = 0; l < packs.Length; l++)
		{
			if (packs[l].tItem.catType == 3 && packs[l].tItem.catKind == 5)
			{
				list.Add(packs[l]);
			}
		}
		for (int m = 0; m < packs.Length; m++)
		{
			if (packs[m].tItem.catType == 3 && packs[m].tItem.catKind == 4)
			{
				list.Add(packs[m]);
			}
		}
		for (int n = 0; n < packs.Length; n++)
		{
			if (packs[n].tItem.catType == 3 && packs[n].tItem.catKind == 7)
			{
				list.Add(packs[n]);
			}
		}
		for (int num = 0; num < packs.Length; num++)
		{
			if (packs[num].tItem.catType == 3 && packs[num].tItem.catKind == 1)
			{
				list.Add(packs[num]);
			}
		}
		for (int num2 = 0; num2 < packs.Length; num2++)
		{
			if (packs[num2].tItem.catType == 3 && packs[num2].tItem.catKind == 2)
			{
				list.Add(packs[num2]);
			}
		}
		for (int num3 = 0; num3 < packs.Length; num3++)
		{
			if (packs[num3].tItem.catType == 3 && packs[num3].tItem.catKind == 3)
			{
				list.Add(packs[num3]);
			}
		}
		for (int num4 = 0; num4 < packs.Length; num4++)
		{
			if (packs[num4].tItem.catType == 3 && packs[num4].tItem.catKind == 6)
			{
				list.Add(packs[num4]);
			}
		}
		for (int num5 = 0; num5 < packs.Length; num5++)
		{
			if (packs[num5].tItem.catType == 3 && packs[num5].tItem.catKind == 8)
			{
				list.Add(packs[num5]);
			}
		}
		for (int num6 = 0; num6 < packs.Length; num6++)
		{
			if (packs[num6].tItem.catType == 4)
			{
				list.Add(packs[num6]);
			}
		}
		for (int num7 = 0; num7 < packs.Length; num7++)
		{
			if (packs[num7].tItem.catType == 5)
			{
				list.Add(packs[num7]);
			}
		}
		for (int num8 = 0; num8 < packs.Length; num8++)
		{
			if (packs[num8].tItem.catType == 6)
			{
				list.Add(packs[num8]);
			}
		}
		for (int num9 = 0; num9 < packs.Length; num9++)
		{
			if (packs[num9].tItem.catType == 7)
			{
				list.Add(packs[num9]);
			}
		}
		for (int num10 = 0; num10 < packs.Length; num10++)
		{
			if (packs[num10].tItem.catType == 8)
			{
				list.Add(packs[num10]);
			}
		}
		for (int num11 = 0; num11 < packs.Length; num11++)
		{
			if (packs[num11].tItem.catType == 9)
			{
				list.Add(packs[num11]);
			}
		}
		return list.ToArray();
	}

	private int DoBundle(TBundle tBundle)
	{
		if (tBundle == null)
		{
			return 0;
		}
		BundleUnit[] array = sortBundlePacks(BundleManager.Instance.Unpack(tBundle.code));
		if (array != null)
		{
			Vector2 pos = crdExtra;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].tItem != null)
				{
					string empty = string.Empty;
					if (array[i].opt >= 1000000)
					{
						empty = StringMgr.Instance.Get("INFINITE");
					}
					else
					{
						empty = array[i].opt.ToString();
						empty = ((!array[i].tItem.IsAmount) ? (empty + StringMgr.Instance.Get("DAYS")) : (empty + StringMgr.Instance.Get("TIMES_UNIT")));
					}
					LabelUtil.TextOut(pos, array[i].tItem.Name + "/" + empty, "MiniLabel", Color.yellow, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
					pos.y += crdExtraOffset;
				}
			}
			currenty = pos.y;
		}
		return 0;
	}

	private void DoBuff(Vector2 pos, TItem tItem)
	{
		if (tItem != null && tItem.tBuff != null)
		{
			if (tItem.tBuff.PointRatio > 0)
			{
				LabelUtil.TextOut(pos, string.Format(StringMgr.Instance.Get("POINT_UP"), tItem.tBuff.PointRatio), "MiniLabel", Color.yellow, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				pos.y += crdExtraOffset;
			}
			if (tItem.tBuff.XpRatio > 0)
			{
				LabelUtil.TextOut(pos, string.Format(StringMgr.Instance.Get("XP_UP"), tItem.tBuff.XpRatio), "MiniLabel", Color.yellow, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				pos.y += crdExtraOffset;
			}
			if (tItem.tBuff.Luck > 0)
			{
				LabelUtil.TextOut(pos, string.Format(StringMgr.Instance.Get("LUCK_UP"), tItem.tBuff.Luck), "MiniLabel", Color.yellow, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				pos.y += crdExtraOffset;
			}
		}
	}
}
