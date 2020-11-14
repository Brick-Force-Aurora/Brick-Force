using System;
using UnityEngine;

[Serializable]
public class SuggestBanishDialog : Dialog
{
	public UIImageList imgList;

	public UILabelList labelList;

	public UIScrollView scrollNameList;

	public UIMyButton backButton;

	public UIImage rankMark;

	public UILabel name;

	public UIMyButton selected;

	public UIMyButton ok;

	public UIMyButton exit;

	public UIToggle curse;

	public UIToggle hackTool;

	public UIToggle noManner;

	public UIToggle etc;

	private int currentSelect;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.SUGGEST_BANISH;
		scrollNameList.listBases.Add(backButton);
		scrollNameList.listBases.Add(rankMark);
		scrollNameList.listBases.Add(name);
		scrollNameList.listBases.Add(selected);
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog()
	{
		curse.toggle = false;
		hackTool.toggle = false;
		noManner.toggle = false;
		etc.toggle = false;
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
		ok.Draw();
		exit.Draw();
		BrickManDesc[] array = BrickManManager.Instance.ToDescriptorArray();
		scrollNameList.SetListCount(array.Length);
		scrollNameList.BeginScroll();
		for (int i = 0; i < array.Length; i++)
		{
			int level = XpManager.Instance.GetLevel(array[i].Xp);
			rankMark.texImage = XpManager.Instance.GetBadge(level, array[i].Rank);
			name.SetText(array[i].Nickname);
			if (currentSelect == i)
			{
				selected.IsDraw = true;
			}
			else
			{
				selected.IsDraw = false;
			}
			scrollNameList.SetListPostion(i);
			scrollNameList.Draw();
			if (backButton.isClick())
			{
				currentSelect = i;
			}
		}
		scrollNameList.EndScroll();
		curse.Draw();
		hackTool.Draw();
		noManner.Draw();
		etc.Draw();
		if (exit.isClick() || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
		}
		if (ok.isClick())
		{
			int reason = GetReason();
			if (reason == 0)
			{
				SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("KICK_VOTE_MESSAGE17"));
			}
			else
			{
				CSNetManager.Instance.Sock.SendCS_START_KICKOUT_VOTE_REQ(array[currentSelect].Seq, reason);
				result = true;
			}
		}
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}

	public int GetReason()
	{
		int num = 0;
		if (curse.toggle)
		{
			num |= 1;
		}
		if (noManner.toggle)
		{
			num |= 2;
		}
		if (hackTool.toggle)
		{
			num |= 4;
		}
		if (etc.toggle)
		{
			num |= 8;
		}
		return num;
	}
}
