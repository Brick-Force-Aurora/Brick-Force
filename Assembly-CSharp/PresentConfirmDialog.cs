using System;
using UnityEngine;

[Serializable]
public class PresentConfirmDialog : Dialog
{
	private Good good;

	private TItem tItem;

	private Good.BUY_HOW buyHow;

	private int selected;

	private string receiver = string.Empty;

	private string title = string.Empty;

	private string contents = string.Empty;

	public Texture2D fpIcon;

	public Texture2D bpIcon;

	private Vector2 crdTitle = new Vector2(285f, 10f);

	private Rect crdCloseBtn = new Rect(526f, 5f, 34f, 34f);

	private Rect crdOutline = new Rect(20f, 50f, 530f, 185f);

	private Rect crdMoneyIcon = new Rect(510f, 138f, 16f, 16f);

	private Rect crdPresent = new Rect(430f, 304f, 128f, 34f);

	private string autoChargeConfirm = string.Empty;

	private Rect crdSure = new Rect(20f, 250f, 530f, 64f);

	private void ShowPresent()
	{
		GUI.Box(crdOutline, string.Empty, "BoxInnerLine");
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
			TextureUtil.DrawTexture(new Rect(crdOutline.x + 20f, crdOutline.y + (crdOutline.height - (float)tItem.CurIcon().height) / 2f, (float)tItem.CurIcon().width, (float)tItem.CurIcon().height), tItem.CurIcon());
			Vector2 pos = new Vector2(crdOutline.x + crdOutline.width - 20f, crdOutline.y + 20f);
			LabelUtil.TextOut(pos, tItem.Name, "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
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
		id = DialogManager.DIALOG_INDEX.PRESENT_CONFIRM;
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
		ShowPresent();
		string str = string.Empty;
		if (autoChargeConfirm.Length > 0)
		{
			str = autoChargeConfirm + " ";
		}
		str += string.Format(StringMgr.Instance.Get("ARE_YOU_SURE_PRESENT"), receiver);
		GUI.Label(crdSure, str, "Label");
		if (GlobalVars.Instance.MyButton(crdPresent, StringMgr.Instance.Get("PRESENT"), "BtnAction") || GlobalVars.Instance.IsReturnPressed())
		{
			CSNetManager.Instance.Sock.SendCS_PRESENT_ITEM_REQ(tItem.code, (int)buyHow, good.GetOption(selected, buyHow), receiver, title, contents);
			result = true;
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

	public void InitDialog(Good g, Good.BUY_HOW how, int sel, string rcv, string ttl, string cntnts)
	{
		good = g;
		tItem = good.tItem;
		buyHow = how;
		selected = sel;
		receiver = rcv;
		title = ttl;
		contents = cntnts;
		autoChargeConfirm = string.Empty;
	}

	public void InitDialog(Good g, Good.BUY_HOW how, int sel, string rcv, string ttl, string cntnts, string extraConfirm)
	{
		InitDialog(g, how, sel, rcv, ttl, cntnts);
		autoChargeConfirm = extraConfirm;
	}
}
