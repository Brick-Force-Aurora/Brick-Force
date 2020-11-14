using System;
using UnityEngine;

[Serializable]
public class LongTimePlayDialog : Dialog
{
	public UIImageList imgList;

	public UILabelList labelList;

	public UIMyButton exit;

	public UIMyButton ok;

	public UIMyButton gameExit;

	public UILabel mainLabel;

	public UILabel isExitLabel;

	public UILabel reciveCount;

	public UILabel initializationTime;

	public UILabel currentMyCoin;

	public UIImage pcbangPlusImage;

	public UIGauge timeGauge;

	public UILabel remainTime;

	public UIImage doneStamp;

	public UILabel explain;

	private int playTime;

	private int rewardCount;

	private int resetAfter;

	private int cycle;

	private int max;

	private float serverDataSinceTime;

	public int RemainMinuteUntilReward
	{
		get
		{
			if (max == rewardCount)
			{
				return -1;
			}
			int num = (int)(Time.realtimeSinceStartup - serverDataSinceTime);
			int num2 = (cycle - playTime - num) / 60 + 1;
			if (num2 <= 0)
			{
				num2 = 1;
			}
			return num2;
		}
	}

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.LONG_TIME_PLAY;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog(bool isGameExit)
	{
		ResetPCBangData();
		if (isGameExit)
		{
			gameExit.IsDraw = true;
			isExitLabel.IsDraw = true;
		}
		else
		{
			gameExit.IsDraw = false;
			isExitLabel.IsDraw = false;
		}
	}

	public void SetData(int _playTime, int _rewardCount, int _resetAfter, int _cycle, int _max)
	{
		playTime = _playTime;
		rewardCount = _rewardCount;
		resetAfter = _resetAfter;
		cycle = _cycle;
		max = _max;
		timeGauge.valueMax = (float)cycle;
		serverDataSinceTime = Time.realtimeSinceStartup;
		doneStamp.IsDraw = (rewardCount == max);
		ResetPCBangData();
	}

	public void ResetPCBangData()
	{
		if (BuffManager.Instance.IsPCBangBuff())
		{
			mainLabel.textKey = "PLAY_REWARD_PCBANG01";
			pcbangPlusImage.IsDraw = true;
			explain.textKey = "PLAY_REWARD_PCBANG02";
		}
		else
		{
			mainLabel.textKey = "PLAY_REWARD_NORMAL01";
			pcbangPlusImage.IsDraw = false;
			explain.textKey = "PLAY_REWARD_CMTCOIN";
		}
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
		mainLabel.Draw();
		isExitLabel.Draw();
		reciveCount.SetTextFormat(rewardCount, max);
		reciveCount.Draw();
		int num = (int)(Time.realtimeSinceStartup - serverDataSinceTime);
		int num2 = (resetAfter - num) / 60;
		int num3 = num2 / 60;
		num2 %= 60;
		initializationTime.SetTextFormat(num3, num2);
		initializationTime.Draw();
		currentMyCoin.SetTextFormat(MyInfoManager.Instance.FreeCoin);
		currentMyCoin.Draw();
		pcbangPlusImage.Draw();
		timeGauge.valueNow = (float)(num + playTime);
		if (rewardCount == max)
		{
			timeGauge.valueNow = timeGauge.valueMax;
		}
		timeGauge.Draw();
		num2 = (cycle - playTime - num) / 60;
		num3 = num2 / 60;
		num2 = num2 % 60 + 1;
		if (num2 <= 0)
		{
			num2 = 1;
		}
		if (max == rewardCount)
		{
			num2 = 0;
		}
		remainTime.SetTextFormat(num2.ToString());
		remainTime.Draw();
		doneStamp.Draw();
		explain.SetTextFormat(max);
		explain.Draw();
		if (exit.Draw() || ok.Draw() || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
		}
		if (gameExit.Draw())
		{
			BuildOption.Instance.Exit();
		}
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}
}
