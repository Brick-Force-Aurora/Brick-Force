using System;
using UnityEngine;

[Serializable]
public class MapIntroDlg : Dialog
{
	private Rect crdIntroArea = new Rect(18f, 70f, 358f, 232f);

	private string mapIntroduce = string.Empty;

	private int maxIntroduceLength = 250;

	private int mapSeq = -1;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.MAPINTRO;
	}

	public override void OnPopup()
	{
		size.x = 396f;
		size.y = 364f;
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog(int seq)
	{
		mapSeq = seq;
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		Vector2 pos = new Vector2(size.x / 2f, 15f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("MAP_INTRO"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		GUI.SetNextControlName("MapIntroduceInput");
		mapIntroduce = GUI.TextArea(crdIntroArea, mapIntroduce, maxIntroduceLength);
		Vector2 pos2 = new Vector2(crdIntroArea.x, crdIntroArea.y + crdIntroArea.height + 5f);
		LabelUtil.TextOut(pos2, StringMgr.Instance.Get("CAN_250_BYTE"), "MiniLabel", Color.red, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		Vector2 pos3 = new Vector2(crdIntroArea.x + crdIntroArea.width, crdIntroArea.y + crdIntroArea.height + 5f);
		string text = mapIntroduce.Length.ToString() + "/" + maxIntroduceLength.ToString() + StringMgr.Instance.Get("BYTE");
		LabelUtil.TextOut(pos3, text, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		Rect rc = new Rect(size.x - 187f, size.y - 44f, 176f, 34f);
		if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("OK"), "BtnAction"))
		{
			GlobalVars.Instance.introTemp = mapIntroduce;
			CSNetManager.Instance.Sock.SendCS_CHG_MAP_INTRO_REQ(mapSeq, mapIntroduce);
			result = true;
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
}
