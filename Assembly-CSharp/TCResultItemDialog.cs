using System;
using UnityEngine;

[Serializable]
public class TCResultItemDialog : Dialog
{
	public UIImageRotate successRotate;

	public UISprite successEffect;

	public UIGroup background;

	public UIImageList imgList;

	public UILabelList labelList;

	public UIMyButton exit;

	public UIMyButton ok;

	public UILabel itemName;

	public UIImage itemBackNomal;

	public UIImage itemBackRare;

	public UIImage itemIcon;

	public UILabel itemTime;

	public TooltipProperty property;

	public UILabel itemExplain;

	private int winner;

	private string nickname;

	private string code = "zzzzz";

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.TCRESULT;
		property.IsShop = true;
		property.categoryAnchor = TextAnchor.MiddleLeft;
		property.categoryLabelType = "Label";
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog(long seq, int index, string code, int amount, bool wasKey)
	{
		TItem tItem = TItemManager.Instance.Get<TItem>(code);
		if (tItem != null)
		{
			itemName.textKey = tItem.name;
			itemIcon.texImage = tItem.CurIcon();
			property.sizeX = background.area.x - 30f;
			property.Start();
			property.tItem = tItem;
			TooltipProperty tooltipProperty = property;
			Vector2 showPosition = itemName.showPosition;
			tooltipProperty.categoryPosX = showPosition.x;
			TooltipProperty tooltipProperty2 = property;
			Vector2 showPosition2 = itemName.showPosition;
			tooltipProperty2.categoryPosY = showPosition2.y + 18f;
			itemTime.SetText(property.tItem.GetOptionStringByOption(amount));
			itemExplain.textKey = tItem.comment;
			if (wasKey)
			{
				itemBackNomal.IsDraw = false;
				itemBackRare.IsDraw = true;
			}
			else
			{
				itemBackNomal.IsDraw = true;
				itemBackRare.IsDraw = false;
			}
			RareTextAdd();
		}
	}

	public override void Update()
	{
		successRotate.Update();
		successEffect.Update();
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		successRotate.Draw();
		successEffect.Draw();
		background.BeginGroup();
		imgList.Draw();
		labelList.Draw();
		itemName.Draw();
		itemBackNomal.Draw();
		itemBackRare.Draw();
		itemIcon.Draw();
		itemTime.Draw();
		property.DoPropertyGuage(85f);
		itemExplain.Draw();
		if (exit.Draw() || ok.Draw() || GlobalVars.Instance.IsReturnPressed())
		{
			result = true;
		}
		background.EndGroup();
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}

	public void SetRareText(int _winner, string _nickname, string _code)
	{
		winner = _winner;
		nickname = _nickname;
		code = _code;
	}

	private void RareTextAdd()
	{
		TItem tItem = TItemManager.Instance.Get<TItem>(code);
		if (tItem != null)
		{
			string text = string.Format(StringMgr.Instance.Get("TREASURE_GET_CONGRATULATIONS"), nickname, tItem.Name);
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.TREASURE, winner, nickname, text));
			}
		}
		code = "zzzzz";
	}
}
