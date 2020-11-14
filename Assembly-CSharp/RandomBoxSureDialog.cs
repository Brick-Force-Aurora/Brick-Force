using System;
using UnityEngine;

[Serializable]
public class RandomBoxSureDialog : Dialog
{
	public Texture2D icon;

	public Rect crdIcon = new Rect(10f, 10f, 34f, 34f);

	public Rect crdComment = new Rect(15f, 50f, 310f, 73f);

	public Rect crdCommentLabel = new Rect(15f, 50f, 310f, 73f);

	public Rect crdOk = new Rect(91f, 171f, 77f, 34f);

	public Rect crdCancel = new Rect(174f, 171f, 77f, 34f);

	private int chest;

	private int index;

	private int token;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.RANDOMBOX_SURE;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog(int c, int i, int t)
	{
		chest = c;
		index = i;
		token = t;
	}

	private void DoTitle()
	{
		Vector2 pos = new Vector2(size.x / 2f, 10f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("TREASURE_CHEST"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		DoTitle();
		GUI.Box(crdComment, string.Empty, "BoxPopLine");
		if (token <= 0)
		{
			GUI.Label(crdCommentLabel, StringMgr.Instance.Get("SURE_USE_TREASURE_CHEST_BY_COIN"), "MiddleCenterLabel");
		}
		else
		{
			GUI.Label(crdCommentLabel, string.Format(StringMgr.Instance.Get("SURE_USE_TREASURE_CHEST_BY_TOKEN"), token, TokenManager.Instance.GetTokenString()), "MiddleCenterLabel");
		}
		if (GlobalVars.Instance.MyButton(crdOk, StringMgr.Instance.Get("OK"), "BtnAction"))
		{
			CSNetManager.Instance.Sock.SendCS_TC_OPEN_PRIZE_TAG_REQ(chest, index, token <= 0);
			result = true;
		}
		if (GlobalVars.Instance.MyButton(crdCancel, StringMgr.Instance.Get("CANCEL"), "BtnAction"))
		{
			result = true;
		}
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}
}
