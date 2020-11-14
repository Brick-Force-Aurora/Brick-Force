using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InviteNoticeDialog : Dialog
{
	public UIImageList imgList;

	public UILabelList labelList;

	public UIMyButton inviteSetUp;

	public UIMyButton exit;

	public UIMyButton allRejection;

	public UIMyButton btnClose;

	public UIScrollView scrollView;

	public UIImage inviteTextBack;

	public UILabel inviteText;

	public UIMyButton go;

	public UIMyButton no;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.INVITE_NOTICE;
		scrollView.listBases.Add(inviteTextBack);
		scrollView.listBases.Add(inviteText);
		scrollView.listBases.Add(go);
		scrollView.listBases.Add(no);
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog()
	{
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		imgList.Draw();
		labelList.Draw();
		if (inviteSetUp.Draw())
		{
			InviteManager.Instance.RemoveAll();
			SettingDialog settingDialog = (SettingDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.SETTING, exclusive: true);
			if (settingDialog != null)
			{
				settingDialog.InitDialog();
				settingDialog.SetTab(5);
			}
		}
		if (allRejection.Draw())
		{
			InviteManager.Instance.RemoveAll();
		}
		if (exit.Draw() || btnClose.Draw())
		{
			InviteManager.Instance.RemoveAll();
			result = true;
		}
		List<Invite> listInvite = InviteManager.Instance.listInvite;
		scrollView.SetListCount(listInvite.Count);
		scrollView.BeginScroll();
		for (int i = 0; i < listInvite.Count; i++)
		{
			Channel channel = ChannelManager.Instance.Get(listInvite[i].channelIndex);
			if (channel != null)
			{
				string text = string.Format(StringMgr.Instance.Get("INVITE_MESSAGE"), listInvite[i].invitorNickname, channel.Name, Room.Type2String(listInvite[i].mode));
				inviteText.SetText(text);
			}
			scrollView.SetListPostion(i);
			scrollView.Draw();
			if (go.isClick())
			{
				Compass.Instance.SetDestination(Compass.DESTINATION_LEVEL.ROOM, listInvite[i].channelIndex, listInvite[i].roomNo, listInvite[i].pswd);
				InviteManager.Instance.RemoveAll();
				result = true;
			}
			if (no.isClick())
			{
				InviteManager.Instance.Remove(listInvite[i].invitorSeq);
				GlobalVars.Instance.clanSendJoinREQ = -1;
			}
		}
		scrollView.EndScroll();
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}
}
