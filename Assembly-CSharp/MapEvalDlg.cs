using System;
using UnityEngine;

[Serializable]
public class MapEvalDlg : Dialog
{
	private Rect crdOutline = new Rect(18f, 70f, 436f, 260f);

	private Rect crdGood = new Rect(78f, 100f, 21f, 22f);

	private Rect crdBad = new Rect(258f, 100f, 21f, 22f);

	private Vector2 crdLineEval = new Vector2(35f, 188f);

	private Rect crdEvalFld = new Rect(38f, 210f, 396f, 27f);

	private Vector2 crdWarn1 = new Vector2(35f, 260f);

	private Vector2 crdWarn2 = new Vector2(35f, 280f);

	private string strEval = string.Empty;

	private int maxEvalLength = 160;

	private bool isGood;

	private bool isBad;

	private int playmap = -1;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.MAP_EVAL;
	}

	public override void OnPopup()
	{
		size.x = 472f;
		size.y = 388f;
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog(int map)
	{
		strEval = string.Empty;
		isGood = false;
		isBad = false;
		playmap = map;
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		Vector2 pos = new Vector2(size.x / 2f, 15f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("MAP_EVAL"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		GUI.Box(crdOutline, string.Empty, "LineBoxBlue");
		DoGoodBad();
		LabelUtil.TextOut(crdLineEval, StringMgr.Instance.Get("A_LINE_EVAL"), "MidLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		string text = strEval;
		GUI.SetNextControlName("Evalu");
		strEval = GUI.TextField(crdEvalFld, strEval);
		if (strEval.Length > maxEvalLength)
		{
			strEval = text;
		}
		LabelUtil.TextOut(crdWarn1, StringMgr.Instance.Get("MAP_EVAL_WARN_1"), "MidLabel", Color.red, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdWarn2, StringMgr.Instance.Get("MAP_EVAL_WARN_2"), "MidLabel", Color.red, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		Rect rc = new Rect(size.x - 187f, size.y - 44f, 176f, 34f);
		if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("DO_EVAL"), "BtnAction") || GlobalVars.Instance.IsReturnPressed())
		{
			if (!isGood && !isBad)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("LIKE_OR_DISLIKE"));
			}
			else
			{
				CSNetManager.Instance.Sock.SendCS_MAP_EVAL_REQ(playmap, (byte)(isGood ? 1 : 0), strEval);
				result = true;
			}
		}
		Rect rc2 = new Rect(size.x - 50f, 10f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc2, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
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

	private void DoGoodBad()
	{
		bool flag = isGood;
		bool flag2 = isBad;
		isGood = GUI.Toggle(crdGood, flag, string.Empty);
		isBad = GUI.Toggle(crdBad, flag2, string.Empty);
		Rect position = new Rect(crdGood.x + 28f, crdGood.y + 2f, 22f, 22f);
		Rect position2 = new Rect(crdBad.x + 28f, crdBad.y + 2f, 22f, 22f);
		TextureUtil.DrawTexture(position, GlobalVars.Instance.iconThumbUp, ScaleMode.StretchToFill);
		TextureUtil.DrawTexture(position2, GlobalVars.Instance.iconThumbDn, ScaleMode.StretchToFill);
		Vector2 pos = new Vector2(crdGood.x + 55f, crdGood.y + 6f);
		Vector2 pos2 = new Vector2(crdBad.x + 55f, crdBad.y + 6f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("GOOD"), "MidLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(pos2, StringMgr.Instance.Get("BAD"), "MidLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		if (!flag && isGood)
		{
			isBad = false;
		}
		if (!flag2 && isBad)
		{
			isGood = false;
		}
	}
}
