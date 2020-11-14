using System;
using UnityEngine;

[Serializable]
public class Tooltip : Dialog
{
	public Texture2D iconPoint;

	public Texture2D iconBrick;

	public Texture2D gauge;

	public Texture2D gaugeFrame;

	public Texture2D[] gaugeTiers;

	private string itemCode = string.Empty;

	private string itemSeq = string.Empty;

	private bool isShop;

	private Item item;

	private TItem tItem;

	private Good good;

	private TcPrize tcPrize;

	private Vector2 curUIPos = Vector2.zero;

	private bool bCalcHeight;

	private float brunchGap = 5f;

	private float priceOffset = 18f;

	public TooltipProperty property;

	public string ItemCode
	{
		get
		{
			return itemCode;
		}
		set
		{
			itemCode = value;
			tItem = TItemManager.Instance.Get<TItem>(itemCode);
			good = ShopManager.Instance.Get(itemCode);
			property.tItem = tItem;
		}
	}

	public string ItemSeq
	{
		get
		{
			return itemSeq;
		}
		set
		{
			itemSeq = value;
		}
	}

	public bool IsShop
	{
		get
		{
			return isShop;
		}
		set
		{
			property.IsShop = (isShop = value);
		}
	}

	public TcPrize Prize
	{
		set
		{
			tcPrize = value;
		}
	}

	public void SetItem(Item _item)
	{
		item = _item;
		property.SetItem(item);
	}

	public override void Start()
	{
		size.x = 275f;
		size.y = 330f;
		itemCode = string.Empty;
		itemSeq = string.Empty;
		item = null;
		tItem = null;
		good = null;
		isShop = false;
		property.Start();
		property.gauge = gauge;
		property.gaugeFrame = gaugeFrame;
		property.gaugeTiers = gaugeTiers;
	}

	private float totalHeight()
	{
		bCalcHeight = true;
		curUIPos = Vector2.zero;
		if (tItem != null)
		{
			float num = (float)(int)((float)tItem.CurIcon().width * 0.4f);
			float b = (float)(int)((float)tItem.CurIcon().height * 0.4f);
			GUIStyle style = GUI.skin.GetStyle("Label");
			float a = style.CalcHeight(new GUIContent(tItem.Name), size.x - num - 8f);
			curUIPos.y += Mathf.Max(a, b);
			curUIPos.y += 24f;
			curUIPos.y += 4f;
			style = GUI.skin.GetStyle("MiniLabel");
			float num2 = style.CalcHeight(new GUIContent(StringMgr.Instance.Get(tItem.comment)), size.x - 16f);
			curUIPos.y += num2;
			curUIPos.y += brunchGap;
			curUIPos.y = property.GagueHeight(curUIPos.y);
			if (good != null && good.GetCashback() > 0)
			{
				curUIPos.y += brunchGap;
				curUIPos.y += 18f;
			}
			DoPriceTag();
			DoAmount();
			curUIPos.y += 10f;
		}
		bCalcHeight = false;
		return curUIPos.y;
	}

	public void DoAmount()
	{
		if (isShop && tcPrize != null)
		{
			curUIPos.y += brunchGap;
			float y = curUIPos.y;
			if (bCalcHeight)
			{
				curUIPos.y += priceOffset;
			}
			else
			{
				LabelUtil.TextOut(new Vector2(10f, y), tcPrize.AmountString, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
				y += priceOffset;
			}
		}
	}

	public void DoPriceTag()
	{
		if (isShop && good != null && tcPrize == null)
		{
			curUIPos.y += brunchGap;
			float num = curUIPos.y;
			if (good.IsPointable)
			{
				if (bCalcHeight)
				{
					curUIPos.y += priceOffset;
				}
				else
				{
					TextureUtil.DrawTexture(new Rect(10f, num + 4f, (float)iconPoint.width, (float)iconPoint.height), iconPoint, ScaleMode.StretchToFill);
					LabelUtil.TextOut(new Vector2(30f, num), good.GetDefaultPrice() + "/" + good.GetDefaultOption(Good.BUY_HOW.GENERAL_POINT), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
				}
				if (BuildOption.Instance.Props.itemBuyLimit)
				{
					Texture2D badge = XpManager.Instance.GetBadge(good.minlvFp);
					string rank = XpManager.Instance.GetRank(good.minlvFp);
					if (null != badge)
					{
						Vector2 vector = LabelUtil.CalcLength("MiniLabel", good.GetDefaultPrice() + "/" + good.GetDefaultOption(Good.BUY_HOW.GENERAL_POINT));
						TextureUtil.DrawTexture(new Rect(40f + vector.x, num, (float)badge.width, (float)badge.height), badge);
						string text = string.Format(StringMgr.Instance.Get("ITEM_PURCHASE_LIMIT_MSG05"), rank);
						LabelUtil.TextOut(new Vector2(40f + vector.x + (float)badge.width + 4f, num), text, "MiniLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
					}
				}
				num += priceOffset;
			}
			if (good.IsBrickPointable && BuildOption.Instance.Props.useBrickPoint)
			{
				if (bCalcHeight)
				{
					curUIPos.y += priceOffset;
				}
				else
				{
					TextureUtil.DrawTexture(new Rect(10f, num + 4f, (float)iconPoint.width, (float)iconPoint.height), iconBrick, ScaleMode.StretchToFill);
					LabelUtil.TextOut(new Vector2(30f, num), good.GetDefaultBrickPrice() + "/" + good.GetDefaultOption(Good.BUY_HOW.BRICK_POINT), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
					num += priceOffset;
				}
			}
			if (good.IsCashable)
			{
				if (bCalcHeight)
				{
					curUIPos.y += priceOffset;
				}
				else
				{
					Texture2D mark = TokenManager.Instance.currentToken.mark;
					TextureUtil.DrawTexture(new Rect(10f, num + 4f, (float)iconPoint.width, (float)iconPoint.height), mark, ScaleMode.StretchToFill);
					LabelUtil.TextOut(new Vector2(30f, num), good.GetDefaultTokenPrice() + "/" + good.GetDefaultOption(Good.BUY_HOW.CASH_POINT), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
				}
				if (BuildOption.Instance.Props.itemBuyLimit)
				{
					Texture2D badge2 = XpManager.Instance.GetBadge(good.minlvTk);
					string rank2 = XpManager.Instance.GetRank(good.minlvTk);
					if (null != badge2)
					{
						Vector2 vector2 = LabelUtil.CalcLength("MiniLabel", good.GetDefaultTokenPrice() + "/" + good.GetDefaultOption(Good.BUY_HOW.CASH_POINT));
						TextureUtil.DrawTexture(new Rect(40f + vector2.x, num, (float)badge2.width, (float)badge2.height), badge2);
						string text2 = string.Format(StringMgr.Instance.Get("ITEM_PURCHASE_LIMIT_MSG05"), rank2);
						LabelUtil.TextOut(new Vector2(40f + vector2.x + (float)badge2.width + 4f, num), text2, "MiniLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
					}
				}
				num += priceOffset;
			}
		}
	}

	private void DoCashBack()
	{
		if (isShop && good != null)
		{
			int cashback = good.GetCashback();
			if (cashback > 0)
			{
				curUIPos.y += brunchGap;
				LabelUtil.TextOut(new Vector2(247f, curUIPos.y), "+" + cashback.ToString("n0"), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				TextureUtil.DrawTexture(new Rect(247f, curUIPos.y + 4f, (float)iconPoint.width, (float)iconPoint.height), iconPoint, ScaleMode.StretchToFill);
				curUIPos.y += priceOffset;
			}
		}
	}

	public override bool DoDialog()
	{
		if (tItem != null)
		{
			size.y = totalHeight();
			curUIPos.y = 0f;
			GUISkin skin = GUI.skin;
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			Color byteColor2FloatColor = GlobalVars.Instance.GetByteColor2FloatColor(244, 151, 25);
			float num = (float)(int)((float)tItem.CurIcon().width * 0.4f);
			float num2 = (float)(int)((float)tItem.CurIcon().height * 0.4f);
			Rect position = new Rect(size.x - num - 4f, 4f, num, num2);
			TextureUtil.DrawTexture(position, tItem.CurIcon(), ScaleMode.StretchToFill);
			Color color = GUI.color;
			GUI.color = byteColor2FloatColor;
			float width = size.x - num - 8f;
			GUIStyle style = GUI.skin.GetStyle("Label");
			float num3 = style.CalcHeight(new GUIContent(tItem.Name), width);
			GUI.Label(new Rect(4f, 4f, width, num3), tItem.Name, "Label");
			curUIPos.y += Mathf.Max(num3, num2);
			GUI.color = color;
			if (!BuildOption.Instance.IsNetmarble && !BuildOption.Instance.IsDeveloper && isShop && tItem.upgradeCategory != TItem.UPGRADE_CATEGORY.NONE)
			{
				Rect position2 = new Rect(size.x - 20f, 5f, 16f, 16f);
				TextureUtil.DrawTexture(position2, GlobalVars.Instance.iconUpgrade, ScaleMode.StretchToFill);
			}
			property.categoryPosY = curUIPos.y;
			curUIPos.y += 24f;
			Rect position3 = new Rect(6f, curUIPos.y, size.x - 6f, 1f);
			TextureUtil.DrawTexture(position3, GlobalVars.Instance.headLine, ScaleMode.StretchToFill);
			curUIPos.y += 4f;
			style = GUI.skin.GetStyle("MiniLabel");
			num3 = style.CalcHeight(new GUIContent(StringMgr.Instance.Get(tItem.comment)), size.x - 16f);
			GUI.Label(new Rect(8f, curUIPos.y, size.x - 16f, num3), StringMgr.Instance.Get(tItem.comment), "MiniLabel");
			curUIPos.y += num3;
			curUIPos.y += brunchGap;
			curUIPos.y = property.DoPropertyGuage(curUIPos.y);
			DoCashBack();
			DoPriceTag();
			DoAmount();
			GUI.skin = skin;
		}
		return false;
	}

	public void SetCoord(Vector2 pos)
	{
		rc = new Rect(pos.x, pos.y, size.x, size.y);
	}
}
