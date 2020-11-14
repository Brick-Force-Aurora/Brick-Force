using System;
using UnityEngine;

[Serializable]
public class TCNetmarbleDialog : Dialog
{
	public UIImageList imgList;

	public UILabelList labelList;

	public UIMyButton exit;

	public UIMyButton goBack;

	public UIMyButton buyToken;

	public UILabel title;

	public UILabel explain;

	public UIImage myToken;

	public UILabel myTokenHave;

	public UILabel myCoinHave;

	public UIImage token;

	public UILabel tokenCount;

	public UIImage coin;

	public UILabel coinCount;

	public UIMyButton tokenButton;

	public UIFlickerColor tokenHighlight;

	public UIMyButton coinButton;

	public UIFlickerColor coinHighlight;

	private TcStatus tcStatus;

	private RandomBoxSureDialog areYouSure;

	public UISpriteMoveEmitter bubbleEffect;

	public RouletteEffect rouletteEffect;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.TCNETMARBLE;
		rouletteEffect.start();
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public override void OnClose(DialogManager.DIALOG_INDEX popup)
	{
		if (popup != DialogManager.DIALOG_INDEX.TCGATE)
		{
			CSNetManager.Instance.Sock.SendCS_TC_CLOSE_REQ();
		}
	}

	public void InitDialog(TcStatus curStatus)
	{
		tcStatus = curStatus;
		myToken.texImage = TokenManager.Instance.currentToken.mark;
		token.texImage = TokenManager.Instance.currentToken.mark;
		title.SetText(tcStatus.GetTitle());
		if (tcStatus.CoinPrice == 0)
		{
			explain.textKey = "GACHAPON_CMT02";
			coin.IsDraw = false;
			coinCount.IsDraw = false;
			coinButton.IsDraw = false;
			coinHighlight.IsDraw = false;
		}
		else
		{
			explain.textKey = "GACHAPON_CMT01";
			coin.IsDraw = true;
			coinCount.IsDraw = true;
			coinButton.IsDraw = true;
			coinHighlight.IsDraw = true;
			coinCount.SetText(tcStatus.CoinPrice.ToString());
		}
		if (tcStatus.TokenPrice == 0)
		{
			token.IsDraw = false;
			tokenCount.IsDraw = false;
			tokenButton.IsDraw = false;
			tokenHighlight.IsDraw = false;
		}
		else
		{
			token.IsDraw = true;
			tokenCount.IsDraw = true;
			tokenButton.IsDraw = true;
			tokenHighlight.IsDraw = true;
			tokenCount.SetText(tcStatus.TokenPrice.ToString());
		}
		rouletteEffect.IsDraw = false;
	}

	public override void Update()
	{
		tokenHighlight.Update();
		coinHighlight.Update();
		bubbleEffect.Update();
		rouletteEffect.Update();
	}

	public override bool DoDialog()
	{
		bool flag = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		bool enabled = GUI.enabled;
		GUI.enabled = (!rouletteEffect.IsDraw && enabled);
		flag = DoMain();
		GUI.enabled = enabled;
		rouletteEffect.Draw();
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return flag;
	}

	private bool DoMain()
	{
		bool result = false;
		imgList.Draw();
		labelList.Draw();
		bubbleEffect.Draw();
		title.Draw();
		explain.Draw();
		token.Draw();
		tokenCount.Draw();
		coin.Draw();
		coinCount.Draw();
		tokenButton.Draw();
		tokenHighlight.Draw();
		coinButton.Draw();
		coinHighlight.Draw();
		myTokenHave.SetText(MyInfoManager.Instance.Cash.ToString("n0"));
		myCoinHave.SetText(MyInfoManager.Instance.FreeCoin.ToString("n0"));
		myToken.Draw();
		myTokenHave.Draw();
		myCoinHave.Draw();
		if (BuildOption.Instance.Props.GetTokensURL.Length > 0)
		{
			string text = (!BuildOption.Instance.IsInfernum) ? string.Format(StringMgr.Instance.Get("BUY_TOKEN"), TokenManager.Instance.GetTokenString()) : StringMgr.Instance.Get("BUY_TOKEN2");
			buyToken.SetText(text.ToUpper());
			if (buyToken.Draw())
			{
				string url = BuildOption.Instance.Props.GetTokensURL + BuildOption.Instance.TokensParameter();
				if (MyInfoManager.Instance.SiteCode == 11)
				{
					url = BuildOption.Instance.Props.GetTokensURL2 + BuildOption.Instance.TokensParameter();
				}
				BuildOption.OpenURL(url);
			}
		}
		if (tokenButton.isClick())
		{
			if (MyInfoManager.Instance.Cash < tcStatus.TokenPrice)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NOT_ENOUGH_MONEY"));
			}
			else
			{
				((RandomBoxSureNetmarble)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.RANDOMBOX_SURENETMARBLE, exclusive: false))?.InitDialog(tcStatus.Seq, 1, tcStatus.TokenPrice);
			}
		}
		if (coinButton.isClick())
		{
			areYouSure = (RandomBoxSureDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.RANDOMBOX_SURE, exclusive: false);
			if (areYouSure != null)
			{
				areYouSure.InitDialog(tcStatus.Seq, 1, 0);
			}
		}
		if (exit.Draw())
		{
			result = true;
		}
		if (goBack.Draw() || (GlobalVars.Instance.IsEscapePressed() && !DialogManager.Instance.IsPopup(DialogManager.DIALOG_INDEX.RANDOMBOX_SURE) && !rouletteEffect.IsDraw))
		{
			CSNetManager.Instance.Sock.SendCS_TC_LEAVE_REQ();
			((TCGateDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.TCGATE, exclusive: true))?.InitDialog();
		}
		return result;
	}

	public void ReceivePrize(long seq, int index, string code, int amount, bool wasKey)
	{
		TItem tItem = TItemManager.Instance.Get<TItem>(code);
		if (tItem == null)
		{
			Debug.LogError("Fail to get TItem for " + code);
		}
		else
		{
			rouletteEffect.InitDialog(seq, index, code, amount, wasKey, tcStatus);
		}
	}
}
