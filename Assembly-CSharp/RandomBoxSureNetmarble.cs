using System;
using UnityEngine;

[Serializable]
public class RandomBoxSureNetmarble : Dialog
{
	public UIImageList imgList;

	public UILabelList labelList;

	public UILabel warningText;

	public UIMyButton ruleNotice;

	public UIMyButton ok;

	public UIMyButton cancel;

	private int chest;

	private int index;

	private int token;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.RANDOMBOX_SURENETMARBLE;
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
		warningText.SetText(GetText());
	}

	public override void Update()
	{
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		imgList.Draw();
		labelList.Draw();
		warningText.Draw();
		ruleNotice.Draw();
		ok.Draw();
		if (cancel.Draw() || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
		}
		if (ok.isClick() || GlobalVars.Instance.IsReturnPressed())
		{
			CSNetManager.Instance.Sock.SendCS_TC_OPEN_PRIZE_TAG_REQ(chest, index, token <= 0);
			result = true;
		}
		if (ruleNotice.isClick())
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
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}

	private string GetText()
	{
		if (token <= 0)
		{
			return StringMgr.Instance.Get("SURE_USE_TREASURE_CHEST_BY_COIN");
		}
		return string.Format(StringMgr.Instance.Get("SURE_USE_TREASURE_CHEST_BY_TOKEN"), token, TokenManager.Instance.GetTokenString());
	}
}
