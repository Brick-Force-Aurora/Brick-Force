using System;
using System.Text.RegularExpressions;
using UnityEngine;

[Serializable]
public class MapPriceChangeDlg : Dialog
{
	private Rect crdOutline = new Rect(18f, 70f, 390f, 134f);

	private Vector2 crdCurPrice = new Vector2(140f, 100f);

	private Vector2 crdCurPrice2 = new Vector2(150f, 100f);

	private Vector2 crdChangePrice = new Vector2(140f, 140f);

	private Rect crdChangePriceFld = new Rect(160f, 136f, 220f, 27f);

	private Vector2 crdPriceExp = new Vector2(30f, 180f);

	private string strPrice = string.Empty;

	private int maxPriceLength = 5;

	private int mapSeq = -1;

	private int mapPrice;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.MAP_PRICE_CHANGE;
	}

	public override void OnPopup()
	{
		size.x = 426f;
		size.y = 263f;
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog(int seq, int price)
	{
		mapSeq = seq;
		mapPrice = price;
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		Vector2 pos = new Vector2(size.x / 2f, 15f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("MAP_PRICE_CHANGE"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		GUI.Box(crdOutline, string.Empty, "LineBoxBlue");
		LabelUtil.TextOut(crdCurPrice, StringMgr.Instance.Get("CUR_PRICE") + " : ", "MidLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		LabelUtil.TextOut(crdCurPrice2, mapPrice.ToString(), "MidLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdChangePrice, StringMgr.Instance.Get("CHANGE_PRICE") + " :", "MidLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		string arg = "100BP";
		if (!BuildOption.Instance.Props.useBrickPoint)
		{
			arg = "1000FP";
		}
		string str = string.Format(StringMgr.Instance.Get("PRICE_CHANGE_EXP"), arg);
		LabelUtil.TextOut(crdPriceExp, "* " + str, "MidLabel", Color.red, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		string text = strPrice;
		GUI.SetNextControlName("MapPrice");
		strPrice = GUI.TextField(crdChangePriceFld, strPrice, 9);
		string pattern = "[^0-9]";
		Regex regex = new Regex(pattern);
		strPrice = regex.Replace(strPrice, string.Empty);
		if (strPrice.Length > 1)
		{
			strPrice = strPrice.TrimStart('0');
		}
		if (strPrice.Length > maxPriceLength)
		{
			strPrice = text;
		}
		int num = 0;
		try
		{
			num = int.Parse(strPrice);
			if (num < 0)
			{
				num = 0;
			}
			if (num > 1000)
			{
				num = 1000;
			}
			strPrice = num.ToString();
		}
		catch
		{
		}
		Rect rc = new Rect(size.x - 187f, size.y - 44f, 176f, 34f);
		if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("OK"), "BtnAction") || GlobalVars.Instance.IsReturnPressed())
		{
			GlobalVars.Instance.downloadPriceTemp = int.Parse(strPrice);
			CSNetManager.Instance.Sock.SendCS_CHG_MAP_DOWNLOAD_FREE_REQ(mapSeq, int.Parse(strPrice));
			result = true;
		}
		Rect rc2 = new Rect(size.x - 50f, 10f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc2, string.Empty, "BtnClose") || (!GlobalVars.Instance.IsModalAll() && GlobalVars.Instance.IsEscapePressed()))
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
}
