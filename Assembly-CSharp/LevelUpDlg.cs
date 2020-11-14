using System;
using UnityEngine;

[Serializable]
public class LevelUpDlg : Dialog
{
	public Texture2D iconKing;

	public Texture2D iconPoint;

	private Vector2 scrollPos = Vector2.zero;

	private int myLv = -1;

	private Good[][] canBuyGoods;

	private Good[] canBuyGoodsNext;

	private int prevLv;

	private int nextLv;

	private Rect crdCongBox = new Rect(10f, 59f, 450f, 64f);

	private Rect crdKing = new Rect(18f, 17f, 94f, 105f);

	private Rect crdCongLabel = new Rect(140f, 70f, 320f, 64f);

	private Rect crdExplBox = new Rect(10f, 133f, 450f, 615f);

	private Vector2 crdNewRank = new Vector2(20f, 140f);

	private Rect crdMyBadge = new Rect(30f, 172f, 34f, 17f);

	private Vector2 crdMyRank = new Vector2(80f, 168f);

	private Vector2 crdCompenTitle = new Vector2(20f, 205f);

	private Rect crdItemIcon4LC = new Rect(30f, 235f, 117f, 64f);

	public Rect crdFPIcon4LC = new Rect(77f, 245f, 36f, 36f);

	private Vector2 crdDetail4LC = new Vector2(155f, 262f);

	private Vector2 crdNoLevelupComp = new Vector2(30f, 243f);

	private Vector2 crdCompenTitleNext = new Vector2(20f, 305f);

	private Rect crdItemIcon4LCNext = new Rect(30f, 335f, 117f, 64f);

	public Rect crdFPIcon4LCNext = new Rect(77f, 345f, 36f, 36f);

	private Vector2 crdDetail4LCNext = new Vector2(155f, 362f);

	private Vector2 crdNoLevelupCompNext = new Vector2(30f, 342f);

	public Vector2 crdNoMoreLevelupCompNext = new Vector2(30f, 362f);

	public Rect crdYouCanBuyFrame = new Rect(20f, 415f, 430f, 315f);

	public Rect crdYouCanBuy = new Rect(30f, 435f, 410f, 281f);

	private Vector2 crdIconSize = new Vector2(117f, 64f);

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.LEVELUP;
	}

	public override void OnPopup()
	{
		if (!BuildOption.Instance.Props.useLevelupCompensationYouCanBuyItem)
		{
			size.y = 415f;
			crdExplBox.height = 274f;
		}
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog(int _prevLv, int _nextLv)
	{
		prevLv = _prevLv;
		nextLv = _nextLv;
		myLv = nextLv;
		int num = 0;
		int num2 = nextLv - prevLv + 1;
		canBuyGoods = new Good[num2 - 1][];
		for (int i = prevLv + 1; i < prevLv + num2; i++)
		{
			canBuyGoods[num] = ShopManager.Instance.GetAvailableItemsCurrentLevel(i);
			num++;
		}
		canBuyGoodsNext = ShopManager.Instance.GetAvailableItemsCurrentLevel(myLv + 1);
	}

	private void DoLevelupCompensation(string badgeName)
	{
		string text = string.Format(StringMgr.Instance.Get("TITLE_LEVELUP"), badgeName) + " :";
		LabelUtil.TextOut(crdCompenTitle, text, "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LevelUpCompensation curCompensation = LevelUpCompensationManager.Instance.getCurCompensation(myLv);
		if (curCompensation == null)
		{
			LabelUtil.TextOut(crdNoLevelupComp, StringMgr.Instance.Get("NO_LEVELUP_COMPENSATION"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		else
		{
			TItem tItem = null;
			Texture2D texture2D = null;
			string empty = string.Empty;
			switch (curCompensation.amount)
			{
			case 0:
				tItem = TItemManager.Instance.Get<TItem>(curCompensation.code);
				if (tItem != null)
				{
					texture2D = tItem.CurIcon();
					empty = ((curCompensation.opt < 1000000) ? (tItem.Name + " (" + curCompensation.opt.ToString() + " " + StringMgr.Instance.Get("DAYS") + ")") : (tItem.Name + " (" + StringMgr.Instance.Get("INFINITE") + ")"));
					if (texture2D != null)
					{
						TextureUtil.DrawTexture(crdItemIcon4LC, texture2D);
					}
					LabelUtil.TextOut(crdDetail4LC, empty, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
				}
				break;
			case 1:
				tItem = TItemManager.Instance.Get<TItem>(curCompensation.code);
				if (tItem != null)
				{
					texture2D = tItem.CurIcon();
					empty = ((curCompensation.opt < 1000000) ? (tItem.Name + " (" + curCompensation.opt.ToString() + " " + StringMgr.Instance.Get("TIMES_UNIT") + ")") : (tItem.Name + " (" + StringMgr.Instance.Get("INFINITE") + ")"));
					if (texture2D != null)
					{
						TextureUtil.DrawTexture(crdItemIcon4LC, texture2D);
					}
					LabelUtil.TextOut(crdDetail4LC, empty, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
				}
				break;
			case 2:
				texture2D = iconPoint;
				empty = StringMgr.Instance.Get("POINT") + " ( " + curCompensation.opt.ToString() + " ) ";
				if (texture2D != null)
				{
					TextureUtil.DrawTexture(crdFPIcon4LC, texture2D);
				}
				LabelUtil.TextOut(crdDetail4LC, empty, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
				break;
			}
		}
	}

	private void DoNextLevelupCompensation(int lv, string badgeName)
	{
		string text = string.Format(StringMgr.Instance.Get("TITLE_LEVELUP_NEXT"), badgeName) + " :";
		LabelUtil.TextOut(crdCompenTitleNext, text, "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LevelUpCompensation curCompensation = LevelUpCompensationManager.Instance.getCurCompensation(lv);
		if (curCompensation == null)
		{
			LabelUtil.TextOut(crdNoLevelupCompNext, StringMgr.Instance.Get("NO_LEVELUP_COMPENSATION"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		else
		{
			TItem tItem = null;
			Texture2D texture2D = null;
			string empty = string.Empty;
			switch (curCompensation.amount)
			{
			case 0:
				tItem = TItemManager.Instance.Get<TItem>(curCompensation.code);
				if (tItem != null)
				{
					texture2D = tItem.CurIcon();
					empty = tItem.Name + " (" + curCompensation.opt.ToString() + " " + StringMgr.Instance.Get("DAYS") + ")";
					if (texture2D != null)
					{
						TextureUtil.DrawTexture(crdItemIcon4LCNext, texture2D);
					}
					LabelUtil.TextOut(crdDetail4LCNext, empty, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
				}
				break;
			case 1:
				tItem = TItemManager.Instance.Get<TItem>(curCompensation.code);
				if (tItem != null)
				{
					texture2D = tItem.CurIcon();
					empty = tItem.Name + " (" + curCompensation.opt.ToString() + " " + StringMgr.Instance.Get("TIMES_UNIT") + ")";
					if (texture2D != null)
					{
						TextureUtil.DrawTexture(crdItemIcon4LCNext, texture2D);
					}
					LabelUtil.TextOut(crdDetail4LCNext, empty, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
				}
				break;
			case 2:
				texture2D = iconPoint;
				empty = StringMgr.Instance.Get("POINT") + " ( " + curCompensation.opt.ToString() + " ) ";
				if (texture2D != null)
				{
					TextureUtil.DrawTexture(crdFPIcon4LCNext, texture2D);
				}
				LabelUtil.TextOut(crdDetail4LCNext, empty, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
				break;
			}
		}
	}

	private float _ListYouCanBuy(float y, Good[] array)
	{
		Rect position = new Rect(10f, 0f, crdIconSize.x, crdIconSize.y);
		Vector2 pos = new Vector2(20f + crdIconSize.x, 0f);
		foreach (Good good in array)
		{
			string empty = string.Empty;
			empty = empty + "- " + good.tItem.Name;
			if (good.tItem.CurIcon() != null)
			{
				position.y = y;
				TextureUtil.DrawTexture(position, good.tItem.CurIcon());
			}
			if (myLv == good.minlvFp)
			{
				empty = empty + " ( " + StringMgr.Instance.Get("POINT") + " )";
			}
			else if (myLv == good.minlvTk)
			{
				empty = empty + " ( " + TokenManager.Instance.GetTokenString() + " )";
			}
			if (empty.Length > 0)
			{
				pos.y = y + crdIconSize.y / 2f;
				LabelUtil.TextOut(pos, empty, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
				y += crdIconSize.y;
			}
		}
		return y;
	}

	private int getTotGoods()
	{
		int num = 0;
		for (int i = 0; i < canBuyGoods.Length; i++)
		{
			if (canBuyGoods[i] != null)
			{
				num += canBuyGoods[i].Length;
			}
		}
		return num;
	}

	private void DoYouCanBuy()
	{
		GUI.Box(crdYouCanBuyFrame, string.Empty, "LineBoxBlue");
		int totGoods = getTotGoods();
		Rect viewRect = new Rect(0f, 0f, crdYouCanBuy.width - 20f, (float)totGoods * crdIconSize.y + 80f);
		if (viewRect.height < crdYouCanBuy.height)
		{
			viewRect.height = crdYouCanBuy.height;
		}
		scrollPos = GUI.BeginScrollView(crdYouCanBuy, scrollPos, viewRect);
		float num = 0f;
		LabelUtil.TextOut(new Vector2(0f, num), StringMgr.Instance.Get("PROMOTE_MSG04") + " :", "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		if (canBuyGoods[0].Length > 0)
		{
			for (int i = 0; i < canBuyGoods.Length; i++)
			{
				num += 25f;
				num = _ListYouCanBuy(num, canBuyGoods[i]);
			}
		}
		else
		{
			num += 25f;
			LabelUtil.TextOut(new Vector2(10f, num), StringMgr.Instance.Get("NO_LEVELUP_COMPENSATION"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			num += crdIconSize.y;
		}
		num += 30f;
		LabelUtil.TextOut(new Vector2(0f, num), StringMgr.Instance.Get("PROMOTE_MSG05") + " :", "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		num += 25f;
		if (canBuyGoodsNext.Length > 0)
		{
			num = _ListYouCanBuy(num, canBuyGoodsNext);
		}
		else
		{
			LabelUtil.TextOut(new Vector2(10f, num), StringMgr.Instance.Get("NO_LEVELUP_COMPENSATION"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			num += crdIconSize.y;
		}
		GUI.EndScrollView();
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		Vector2 pos = new Vector2(size.x / 2f, 10f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("PROMOTE_MSG01"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		GUI.Box(crdCongBox, string.Empty, "LineBoxBlue");
		TextureUtil.DrawTexture(crdKing, iconKing);
		GUI.Label(crdCongLabel, StringMgr.Instance.Get("PROMOTE_MSG02"), "Label");
		GUI.Box(crdExplBox, string.Empty, "BoxFadeBlue");
		LabelUtil.TextOut(crdNewRank, StringMgr.Instance.Get("PROMOTE_MSG03") + " :", "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		Texture2D badge = XpManager.Instance.GetBadge(myLv, MyInfoManager.Instance.Rank);
		string rank = XpManager.Instance.GetRank(myLv, MyInfoManager.Instance.Rank);
		if (null != badge)
		{
			TextureUtil.DrawTexture(crdMyBadge, badge);
			LabelUtil.TextOut(crdMyRank, rank, "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		DoLevelupCompensation(rank);
		if (myLv < XpManager.Instance.MaxLevelByXp)
		{
			int lv = myLv + 1;
			string rank2 = XpManager.Instance.GetRank(lv, MyInfoManager.Instance.Rank);
			DoNextLevelupCompensation(lv, rank2);
		}
		else
		{
			LabelUtil.TextOut(crdNoMoreLevelupCompNext, StringMgr.Instance.Get("NO_MORE_LEVELUP_COMPENSATION"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		DoYouCanBuy();
		Rect rc = new Rect(size.x - 44f, 5f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
		}
		GUI.skin = skin;
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		return result;
	}
}
