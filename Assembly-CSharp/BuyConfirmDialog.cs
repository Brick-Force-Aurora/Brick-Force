using System;
using UnityEngine;

[Serializable]
public class BuyConfirmDialog : Dialog
{
	private long rebuy = -1L;

	private Good good;

	private TItem tItem;

	private Good.BUY_HOW buyHow;

	private int selected;

	private bool wasEquip;

	public Texture2D fpIcon;

	public Texture2D bpIcon;

	private Vector2 crdTitle = new Vector2(285f, 10f);

	private Rect crdCloseBtn = new Rect(526f, 5f, 34f, 34f);

	private Rect crdOutline = new Rect(20f, 50f, 530f, 185f);

	private Rect crdMoneyIcon = new Rect(510f, 138f, 16f, 16f);

	private Rect crdToggle = new Rect(20f, 304f, 265f, 22f);

	private Rect crdBuy = new Rect(430f, 304f, 128f, 34f);

	private string autoChargeConfirm = string.Empty;

	private Rect crdSure = new Rect(20f, 250f, 530f, 64f);

	private bool cantBuy;

	private void ShowGood()
	{
		GUI.Box(crdOutline, string.Empty, "BoxFadeBlue");
		if (good != null && tItem != null && null != tItem.CurIcon())
		{
			Texture2D texture2D = null;
			switch (buyHow)
			{
			case Good.BUY_HOW.GENERAL_POINT:
				texture2D = fpIcon;
				break;
			case Good.BUY_HOW.BRICK_POINT:
				texture2D = bpIcon;
				break;
			case Good.BUY_HOW.CASH_POINT:
				texture2D = TokenManager.Instance.currentToken.mark;
				break;
			}
			TextureUtil.DrawTexture(new Rect(crdOutline.x + 32f, crdOutline.y + (crdOutline.height - (float)tItem.CurIcon().height) / 2f, (float)tItem.CurIcon().width, (float)tItem.CurIcon().height), tItem.CurIcon());
			Vector2 pos = new Vector2(crdOutline.x + crdOutline.width - 20f, crdOutline.y + 20f);
			LabelUtil.TextOut(pos, tItem.Name, "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			pos.y += 32f;
			LabelUtil.TextOut(pos, good.GetRemainString(selected, buyHow), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			pos.y += 32f;
			if (texture2D != null)
			{
				TextureUtil.DrawTexture(crdMoneyIcon, texture2D);
				pos.x -= crdMoneyIcon.width + 10f;
			}
			LabelUtil.TextOut(pos, good.GetPriceString(selected, buyHow), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		}
	}

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.BUY_CONFIRM;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		LabelUtil.TextOut(crdTitle, StringMgr.Instance.Get("CONFIRM_BUY"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		ShowGood();
		string str = string.Empty;
		if (autoChargeConfirm.Length > 0 && !BuildOption.Instance.IsNetmarble && !BuildOption.Instance.IsDeveloper)
		{
			str = autoChargeConfirm + " ";
		}
		str += StringMgr.Instance.Get("ARE_YOU_SURE_BUY");
		GUI.Label(crdSure, str, "Label");
		if (tItem.IsEquipable)
		{
			wasEquip = GUI.Toggle(crdToggle, wasEquip, StringMgr.Instance.Get("CHECK_DIRECT_EQUIP"));
		}
		if (GlobalVars.Instance.MyButton(crdBuy, StringMgr.Instance.Get("OK"), "BtnAction") || GlobalVars.Instance.IsReturnPressed())
		{
			if (cantBuy)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NOT_ENOUGH_MONEY"));
			}
			else
			{
				if (rebuy < 0)
				{
					CSNetManager.Instance.Sock.SendCS_BUY_ITEM_REQ(good.tItem.code, (int)buyHow, good.GetOption(selected, buyHow), good.IsAmount, wasEquip);
				}
				else
				{
					CSNetManager.Instance.Sock.SendCS_REBUY_ITEM_REQ(rebuy, good.tItem.code, (int)buyHow, good.GetOption(selected, buyHow), good.IsAmount, wasEquip);
				}
				result = true;
			}
		}
		if (GlobalVars.Instance.MyButton(crdCloseBtn, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
		}
		GUI.skin = skin;
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		return result;
	}

	public void InitDialog(Good g, Good.BUY_HOW how, int sel)
	{
		wasEquip = false;
		rebuy = -1L;
		good = g;
		tItem = good.tItem;
		buyHow = how;
		selected = sel;
		autoChargeConfirm = string.Empty;
		cantBuy = false;
		if (BuildOption.Instance.IsNetmarble || BuildOption.Instance.IsDeveloper)
		{
			int price = good.GetPrice(selected, buyHow);
			if (buyHow == Good.BUY_HOW.BRICK_POINT)
			{
				if (MyInfoManager.Instance.BrickPoint < price)
				{
					cantBuy = true;
				}
			}
			else if (buyHow == Good.BUY_HOW.GENERAL_POINT)
			{
				if (MyInfoManager.Instance.Point < price)
				{
					cantBuy = true;
				}
			}
			else if (buyHow == Good.BUY_HOW.CASH_POINT && MyInfoManager.Instance.Cash < price)
			{
				cantBuy = true;
			}
		}
	}

	public void InitDialog(long r, Good g, Good.BUY_HOW how, int sel)
	{
		wasEquip = false;
		rebuy = r;
		good = g;
		tItem = good.tItem;
		buyHow = how;
		selected = sel;
		autoChargeConfirm = string.Empty;
		cantBuy = false;
		if (BuildOption.Instance.IsNetmarble || BuildOption.Instance.IsDeveloper)
		{
			int price = good.GetPrice(selected, buyHow);
			if (buyHow == Good.BUY_HOW.BRICK_POINT)
			{
				if (MyInfoManager.Instance.BrickPoint < price)
				{
					cantBuy = true;
				}
			}
			else if (buyHow == Good.BUY_HOW.GENERAL_POINT)
			{
				if (MyInfoManager.Instance.Point < price)
				{
					cantBuy = true;
				}
			}
			else if (buyHow == Good.BUY_HOW.CASH_POINT && MyInfoManager.Instance.Cash < price)
			{
				cantBuy = true;
			}
		}
	}

	public void InitDialog(Good g, Good.BUY_HOW how, int sel, string extraConfirm)
	{
		InitDialog(g, how, sel);
		autoChargeConfirm = extraConfirm;
		cantBuy = false;
		if (BuildOption.Instance.IsNetmarble || BuildOption.Instance.IsDeveloper)
		{
			int price = good.GetPrice(selected, buyHow);
			if (buyHow == Good.BUY_HOW.BRICK_POINT)
			{
				if (MyInfoManager.Instance.BrickPoint < price)
				{
					cantBuy = true;
				}
			}
			else if (buyHow == Good.BUY_HOW.GENERAL_POINT)
			{
				if (MyInfoManager.Instance.Point < price)
				{
					cantBuy = true;
				}
			}
			else if (buyHow == Good.BUY_HOW.CASH_POINT && MyInfoManager.Instance.Cash < price)
			{
				cantBuy = true;
			}
		}
	}

	public void InitDialog(long r, Good g, Good.BUY_HOW how, int sel, string extraConfirm)
	{
		InitDialog(r, g, how, sel);
		autoChargeConfirm = extraConfirm;
		cantBuy = false;
		if (BuildOption.Instance.IsNetmarble || BuildOption.Instance.IsDeveloper)
		{
			int price = good.GetPrice(selected, buyHow);
			if (buyHow == Good.BUY_HOW.BRICK_POINT)
			{
				if (MyInfoManager.Instance.BrickPoint < price)
				{
					cantBuy = true;
				}
			}
			else if (buyHow == Good.BUY_HOW.GENERAL_POINT)
			{
				if (MyInfoManager.Instance.Point < price)
				{
					cantBuy = true;
				}
			}
			else if (buyHow == Good.BUY_HOW.CASH_POINT && MyInfoManager.Instance.Cash < price)
			{
				cantBuy = true;
			}
		}
	}
}
