using System;
using UnityEngine;

[Serializable]
public class TCGateDialog : Dialog
{
	public UIImageList imgList;

	public UILabelList labelList;

	public UIMyButton exit;

	public UIMyButton enter;

	public UIImage myToken;

	public UILabel myTokenHave;

	public UILabel myCoinHave;

	public UIScrollView scrollBoard;

	public UIMyButton boardBack;

	public UIImage nomal;

	public UIImage premium;

	public UIImage select;

	public UILabel boardName;

	public UIImageCounter maxRare;

	public UIImageCounter currentRare;

	public UILabel count;

	public UIImage token;

	public UILabel tokenCount;

	public UIImage coin;

	public UILabel coinCount;

	public UIImage itemIconTable;

	public UILabel itemTimeTable;

	public UIScrollView scrollRare;

	public UILabel itemName;

	public UIImage itemBackNomal;

	public UIImage itemBackRare;

	public UIImage itemIcon;

	public UILabel itemTime;

	public TooltipProperty property;

	public UIImage itemLine;

	public UILabel itemExplain;

	public UIGroup tooltip;

	public UILabelList tooltipLabels;

	public UILabel tooltipName;

	public UILabel tooltipCount;

	public UILabel tooltipRare;

	public UILabel tooltipProbability;

	private string lastTooltip = string.Empty;

	private int curBoard;

	private int clickBoard = -1;

	private bool doubleClicked;

	private float lastClickTime;

	private float doubleClickTimeout = 0.3f;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.TCGATE;
		scrollBoard.listBases.Add(boardBack);
		scrollBoard.listBases.Add(nomal);
		scrollBoard.listBases.Add(premium);
		scrollBoard.listBases.Add(boardName);
		scrollBoard.listBases.Add(maxRare);
		scrollBoard.listBases.Add(currentRare);
		scrollBoard.listBases.Add(count);
		scrollBoard.listBases.Add(token);
		scrollBoard.listBases.Add(tokenCount);
		scrollBoard.listBases.Add(coin);
		scrollBoard.listBases.Add(coinCount);
		scrollBoard.listBases.Add(select);
		scrollBoard.listBases.Add(itemIconTable);
		scrollBoard.listBases.Add(itemTimeTable);
		scrollRare.listBases.Add(itemName);
		scrollRare.listBases.Add(itemBackNomal);
		scrollRare.listBases.Add(itemBackRare);
		scrollRare.listBases.Add(itemIcon);
		scrollRare.listBases.Add(itemTime);
		scrollRare.listBases.Add(itemExplain);
		property.IsShop = true;
		property.categoryAnchor = TextAnchor.UpperLeft;
		property.categoryLabelType = "Label";
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
		myToken.texImage = TokenManager.Instance.currentToken.mark;
		token.texImage = TokenManager.Instance.currentToken.mark;
		doubleClicked = false;
	}

	public override void OnClose(DialogManager.DIALOG_INDEX popup)
	{
		if (popup != DialogManager.DIALOG_INDEX.TCBOARD && popup != DialogManager.DIALOG_INDEX.TCNETMARBLE)
		{
			CSNetManager.Instance.Sock.SendCS_TC_CLOSE_REQ();
		}
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
		TcStatus[] array = TreasureChestManager.Instance.ToArray();
		scrollBoard.ListResetAddPosition();
		scrollBoard.SetListCount(array.Length);
		scrollBoard.BeginScroll();
		for (int i = 0; i < scrollBoard.GetListCount(); i++)
		{
			TcStatus tcStatus = array[i];
			select.IsDraw = (curBoard == i);
			boardBack.toolTipString = tcStatus.Seq.ToString();
			boardName.SetText(tcStatus.GetTitle());
			if (tcStatus.TokenPrice == 0)
			{
				token.IsDraw = false;
				tokenCount.IsDraw = false;
			}
			else
			{
				token.IsDraw = true;
				tokenCount.IsDraw = true;
				tokenCount.SetText(tcStatus.TokenPrice.ToString());
			}
			if (tcStatus.CoinPrice == 0)
			{
				coin.IsDraw = false;
				coinCount.IsDraw = false;
				premium.IsDraw = true;
			}
			else
			{
				coin.IsDraw = true;
				coinCount.IsDraw = true;
				premium.IsDraw = false;
				coinCount.SetText(tcStatus.CoinPrice.ToString());
			}
			if (BuildOption.Instance.Props.randomBox == BuildOption.RANDOM_BOX_TYPE.INFERNUM)
			{
				maxRare.SetListCount(tcStatus.MaxKey);
				currentRare.SetListCount(tcStatus.Key);
				count.SetText(tcStatus.GetDescription());
				itemIconTable.IsDraw = false;
				itemTimeTable.IsDraw = false;
			}
			else
			{
				TcTItem firstRare = tcStatus.GetFirstRare();
				if (!firstRare.IsNull())
				{
					TItem tItem = TItemManager.Instance.Get<TItem>(firstRare.code);
					if (tItem != null)
					{
						itemIconTable.IsDraw = true;
						itemTimeTable.IsDraw = true;
						premium.IsDraw = false;
						itemIconTable.texImage = tItem.CurIcon();
						if (tItem.IsAmount)
						{
							itemTimeTable.SetText(firstRare.opt.ToString() + " " + StringMgr.Instance.Get("TIMES_UNIT"));
						}
						else if (firstRare.opt >= 1000000)
						{
							itemTimeTable.SetText(StringMgr.Instance.Get("INFINITE"));
						}
						else
						{
							itemTimeTable.SetText(firstRare.opt.ToString() + " " + StringMgr.Instance.Get("DAYS"));
						}
					}
					else
					{
						itemIconTable.IsDraw = false;
						itemTimeTable.IsDraw = false;
					}
				}
			}
			scrollBoard.SetListPostion(i);
			scrollBoard.Draw();
			if (boardBack.isClick())
			{
				curBoard = i;
				if (Time.time - lastClickTime < doubleClickTimeout && clickBoard == curBoard)
				{
					doubleClicked = true;
				}
				else
				{
					lastClickTime = Time.time;
					clickBoard = curBoard;
				}
			}
		}
		scrollBoard.EndScroll();
		if (curBoard >= 0 && curBoard < array.Length)
		{
			TcTItem[] arraySorted = array[curBoard].GetArraySorted();
			scrollRare.ListResetAddPosition();
			itemLine.ResetAddPosition();
			scrollRare.SetListCount(arraySorted.Length);
			scrollRare.BeginScroll();
			for (int j = 0; j < arraySorted.Length; j++)
			{
				TItem tItem2 = TItemManager.Instance.Get<TItem>(arraySorted[j].code);
				if (tItem2 != null)
				{
					itemName.textKey = tItem2.name;
					itemIcon.texImage = tItem2.CurIcon();
					itemExplain.textKey = tItem2.comment;
					if (tItem2.IsAmount)
					{
						itemTime.SetText(arraySorted[j].opt.ToString() + " " + StringMgr.Instance.Get("TIMES_UNIT"));
					}
					else if (arraySorted[j].opt >= 1000000)
					{
						itemTime.SetText(StringMgr.Instance.Get("INFINITE"));
					}
					else
					{
						itemTime.SetText(arraySorted[j].opt.ToString() + " " + StringMgr.Instance.Get("DAYS"));
					}
					if (arraySorted[j].isKey)
					{
						itemBackNomal.IsDraw = false;
						itemBackRare.IsDraw = true;
					}
					else
					{
						itemBackNomal.IsDraw = true;
						itemBackRare.IsDraw = false;
					}
					scrollRare.SetListPostion(j);
					bool flag = scrollRare.IsSkipAble();
					if (!flag)
					{
						scrollRare.Draw();
						property.sizeX = size.x - 100f;
						property.Start();
						property.tItem = tItem2;
						TooltipProperty tooltipProperty = property;
						Vector2 showPosition = itemName.showPosition;
						tooltipProperty.categoryPosX = showPosition.x;
						TooltipProperty tooltipProperty2 = property;
						Vector2 showPosition2 = itemName.showPosition;
						tooltipProperty2.categoryPosY = showPosition2.y + 22f;
						TooltipProperty tooltipProperty3 = property;
						Vector2 showPosition3 = itemName.showPosition;
						tooltipProperty3.DoPropertyGuage(showPosition3.y + 2f);
					}
					if (j != arraySorted.Length - 1)
					{
						if (!flag)
						{
							itemLine.Draw();
						}
						itemLine.AddPositionY(scrollRare.offSetY);
					}
				}
			}
			scrollRare.EndScroll();
		}
		if (exit.Draw() || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
		}
		if ((enter.Draw() || GlobalVars.Instance.IsReturnPressed() || doubleClicked) && curBoard >= 0 && curBoard < array.Length)
		{
			CSNetManager.Instance.Sock.SendCS_TC_ENTER_REQ(array[curBoard].Seq);
		}
		myTokenHave.SetText(MyInfoManager.Instance.Cash.ToString("n0"));
		myCoinHave.SetText(MyInfoManager.Instance.FreeCoin.ToString("n0"));
		myToken.Draw();
		myTokenHave.Draw();
		myCoinHave.Draw();
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}

	private void DoTooltip()
	{
		Dialog top = DialogManager.Instance.GetTop();
		if (GUI.tooltip.Length > 0 && top != null && top.ID == DialogManager.DIALOG_INDEX.TCGATE)
		{
			if (lastTooltip != GUI.tooltip && !DialogManager.Instance.IsModal)
			{
				GlobalVars.Instance.PlaySoundMouseOver();
			}
			Vector2 zero = Vector2.zero;
			zero.Set(300f, 150f);
			Vector2 position = GlobalVars.Instance.ToGUIPoint(Event.current.mousePosition);
			if (position.x > size.x * 0.5f)
			{
				position.x -= zero.x;
			}
			if (position.y > size.y * 0.5f)
			{
				position.y = position.x - zero.y;
			}
			tooltip.position = position;
			tooltip.area = zero;
			tooltip.style = "LineWindow";
			tooltip.BeginGroup();
			TcStatus tcStatus = TreasureChestManager.Instance.Get(Convert.ToInt32(GUI.tooltip));
			tooltipName.SetText(tcStatus.GetTitle());
			tooltipCount.SetText(tcStatus.Cur.ToString());
			tooltipRare.SetText(tcStatus.GetKeyDescription());
			tooltipProbability.SetText(tcStatus.Chance.ToString("0.##") + " %");
			tooltipLabels.Draw();
			tooltipName.Draw();
			tooltipCount.Draw();
			tooltipRare.Draw();
			tooltipProbability.Draw();
			tooltip.EndGroup();
			lastTooltip = GUI.tooltip;
		}
	}
}
