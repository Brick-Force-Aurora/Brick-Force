using System;
using UnityEngine;

[Serializable]
public class VoteBanishDialog : Dialog
{
	public UIImageList imgList;

	public UILabelList labelList;

	public UILabel voteText;

	public UIFlickerColor timeFlicker;

	public UILabel remainTime;

	public UIGauge yesGauge;

	public UILabel yesText;

	public UIGauge noGauge;

	public UILabel noText;

	public UIMyButton ok;

	public UIMyButton cancel;

	public UIMyButton exit;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.VOTE_BANISH;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog()
	{
	}

	public override void Update()
	{
		timeFlicker.Update();
	}

	public override bool DoDialog()
	{
		VoteStatus vote = RoomManager.Instance.vote;
		if (vote == null)
		{
			return true;
		}
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		imgList.Draw();
		labelList.Draw();
		voteText.SetTextFormat(vote.targetNickname, vote.GetVoteReason());
		yesText.SetTextFormat(vote.yes, vote.total);
		noText.SetTextFormat(vote.no, vote.total);
		remainTime.SetTextFormat(vote.GetRemainTime());
		voteText.Draw();
		timeFlicker.Draw();
		remainTime.Draw();
		yesGauge.valueMax = (float)vote.total;
		yesGauge.valueNow = (float)vote.yes;
		noGauge.valueMax = (float)vote.total;
		noGauge.valueNow = (float)vote.no;
		yesGauge.Draw();
		yesText.Draw();
		noGauge.Draw();
		noText.Draw();
		ok.Draw();
		cancel.Draw();
		exit.Draw();
		if (exit.isClick() || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
		}
		if (ok.isClick())
		{
			CSNetManager.Instance.Sock.SendCS_KICKOUT_VOTE_REQ(yes: true);
			result = true;
		}
		if (cancel.isClick())
		{
			CSNetManager.Instance.Sock.SendCS_KICKOUT_VOTE_REQ(yes: false);
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
