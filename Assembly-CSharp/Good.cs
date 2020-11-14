using System;
using System.Collections.Generic;
using UnityEngine;

public class Good
{
	public enum BUY_HOW
	{
		GENERAL_POINT,
		BRICK_POINT,
		CASH_POINT
	}

	public string code;

	public TItem tItem;

	public List<PriceTag> prices;

	public bool isNew;

	public bool isHot;

	public bool isVisible = true;

	public bool isSpecialOffer;

	public bool isOfferOnce;

	public bool isGiftable;

	public bool isPromo;

	public sbyte minlvFp;

	public sbyte minlvTk;

	private bool rebuyInvisible;

	private bool check;

	private string buyErr = string.Empty;

	public int priceSel = 1;

	public float starRate => tItem.StarRate;

	public bool Check
	{
		get
		{
			return check;
		}
		set
		{
			check = value;
		}
	}

	public string BuyErr
	{
		get
		{
			return buyErr;
		}
		set
		{
			buyErr = value;
		}
	}

	public bool isSale
	{
		get
		{
			for (int i = 0; i < prices.Count; i++)
			{
				if (prices[i].IsDiscount)
				{
					return true;
				}
			}
			return false;
		}
	}

	public bool IsCashable => CanBuy(BUY_HOW.CASH_POINT);

	public bool IsPointable => CanBuy(BUY_HOW.GENERAL_POINT);

	public bool IsBrickPointable => CanBuy(BUY_HOW.BRICK_POINT);

	public bool IsAmount => tItem.IsAmount;

	public int HotNewValue
	{
		get
		{
			int result = 3;
			if (isHot && isNew)
			{
				result = 0;
			}
			else if (isHot)
			{
				result = 1;
			}
			else if (isNew)
			{
				result = 2;
			}
			return result;
		}
	}

	public Good(string _code, TItem _tItem, bool _isNew, bool _isHot, bool _isVisible, bool _isSpecialOffer, bool _isOfferOnce, bool _isGiftable, bool _isPromo, bool _rebuyInvisible, sbyte _minlvFp, sbyte _minlvTk)
	{
		code = _code;
		tItem = _tItem;
		isNew = _isNew;
		isHot = _isHot;
		isVisible = _isVisible;
		isSpecialOffer = _isSpecialOffer;
		isOfferOnce = _isOfferOnce;
		isGiftable = _isGiftable;
		isPromo = _isPromo;
		rebuyInvisible = _rebuyInvisible;
		minlvFp = _minlvFp;
		minlvTk = _minlvTk;
		prices = new List<PriceTag>();
	}

	public bool CanBuy(BUY_HOW buyHow)
	{
		bool result = false;
		foreach (PriceTag price in prices)
		{
			if (price.CanBuy(buyHow))
			{
				result = true;
			}
		}
		return result;
	}

	public bool CanBuy(BUY_HOW buyHow, bool rebuy)
	{
		bool result = false;
		foreach (PriceTag price in prices)
		{
			if (price.CanBuy(buyHow, rebuy))
			{
				result = true;
			}
		}
		return result;
	}

	public void OnEverySec()
	{
		if (prices != null)
		{
			for (int i = 0; i < prices.Count; i++)
			{
				prices[i].OnEverySec();
			}
		}
	}

	public void AddPrice(int opt, int pp, int bp, int cp, int cb, int dp, int db, int dc, int dcStart, int dcEnd, int vStart, int vEnd)
	{
		bool flag = false;
		int num = 0;
		while (!flag && num < prices.Count)
		{
			if (prices[num].option > opt)
			{
				flag = true;
				prices.Insert(num, new PriceTag(opt, pp, bp, cp, cb, dp, db, dc, dcStart, dcEnd, vStart, vEnd));
			}
			if (prices[num].option == opt)
			{
				Debug.LogError("Same option found for one item " + code + "[" + opt + "]");
			}
			num++;
		}
		if (!flag)
		{
			prices.Add(new PriceTag(opt, pp, bp, cp, cb, dp, db, dc, dcStart, dcEnd, vStart, vEnd));
		}
	}

	public string[] GetOptionStrings(BUY_HOW buyHow, int percent)
	{
		if (tItem == null)
		{
			return null;
		}
		List<string> list = new List<string>();
		for (int i = 0; i < prices.Count; i++)
		{
			string optionString = prices[i].GetOptionString(buyHow, tItem.IsAmount, percent);
			if (optionString.Length > 0)
			{
				list.Add(optionString);
			}
		}
		return list.ToArray();
	}

	public string GetOptionStringByOption(int opt)
	{
		if (tItem == null)
		{
			return string.Empty;
		}
		return tItem.GetOptionStringByOption(opt);
	}

	public string GetOptionString(int sel, BUY_HOW buyHow, int percent)
	{
		if (sel >= prices.Count || tItem == null)
		{
			return string.Empty;
		}
		int num = 0;
		for (int i = 0; i < prices.Count; i++)
		{
			if (prices[i].CanBuy(buyHow))
			{
				if (num == sel)
				{
					return prices[i].GetOptionString(buyHow, tItem.IsAmount, percent);
				}
				num++;
			}
		}
		return string.Empty;
	}

	public string GetRemainString(int sel, BUY_HOW buyHow)
	{
		if (sel >= prices.Count || tItem == null)
		{
			return string.Empty;
		}
		int num = 0;
		for (int i = 0; i < prices.Count; i++)
		{
			if (prices[i].CanBuy(buyHow))
			{
				if (num == sel)
				{
					return prices[i].GetRemainString(tItem.IsAmount);
				}
				num++;
			}
		}
		return string.Empty;
	}

	public int GetPrice(int sel, BUY_HOW buyHow)
	{
		if (sel >= prices.Count || tItem == null)
		{
			return -1;
		}
		int percent = 0;
		if (BuildOption.Instance.Props.usePriceDiscount && tItem.type == TItem.TYPE.WEAPON)
		{
			TWeapon tWeapon = (TWeapon)tItem;
			percent = tWeapon.GetDiscountRatio();
		}
		int num = 0;
		for (int i = 0; i < prices.Count; i++)
		{
			if (prices[i].CanBuy(buyHow))
			{
				if (num == sel)
				{
					return prices[i].GetPrice(buyHow, percent);
				}
				num++;
			}
		}
		return -1;
	}

	public int GetOriginalPrice(int sel, BUY_HOW buyHow)
	{
		if (sel >= prices.Count || tItem == null)
		{
			return -1;
		}
		int percent = 0;
		int num = 0;
		for (int i = 0; i < prices.Count; i++)
		{
			if (prices[i].CanBuy(buyHow))
			{
				if (num == sel)
				{
					return prices[i].GetPrice(buyHow, percent);
				}
				num++;
			}
		}
		return -1;
	}

	public int GetPriceByOpt(int opt, BUY_HOW buyHow)
	{
		for (int i = 0; i < prices.Count; i++)
		{
			if (prices[i].option == opt)
			{
				return prices[i].GetPrice(buyHow);
			}
		}
		return -1;
	}

	public bool IsVisible()
	{
		bool flag = false;
		for (int i = 0; i < prices.Count; i++)
		{
			if (prices[i].CanBuy(BUY_HOW.CASH_POINT) || prices[i].CanBuy(BUY_HOW.BRICK_POINT) || prices[i].CanBuy(BUY_HOW.GENERAL_POINT))
			{
				flag = true;
			}
		}
		return flag && isVisible && (!isPromo || ShopManager.Instance.mePromo(code)) && (!isOfferOnce || !ShopManager.Instance.meBought(code));
	}

	public bool IsRebuyable()
	{
		bool flag = false;
		for (int i = 0; i < prices.Count; i++)
		{
			if (prices[i].CanBuy(BUY_HOW.CASH_POINT) || prices[i].CanBuy(BUY_HOW.BRICK_POINT) || prices[i].CanBuy(BUY_HOW.GENERAL_POINT))
			{
				flag = true;
			}
		}
		return flag && (isVisible || rebuyInvisible) && (!isPromo || ShopManager.Instance.mePromo(code)) && (!isOfferOnce || !ShopManager.Instance.meBought(code));
	}

	public int GetOption(int sel, BUY_HOW buyHow)
	{
		int num = 0;
		for (int i = 0; i < prices.Count; i++)
		{
			if (prices[i].CanBuy(buyHow))
			{
				if (num == sel)
				{
					return prices[i].option;
				}
				num++;
			}
		}
		return -1;
	}

	private PriceTag GetDefaultPriceTag(BUY_HOW buyHow)
	{
		List<PriceTag> list = new List<PriceTag>();
		for (int i = 0; i < prices.Count; i++)
		{
			if (prices[i].CanBuy(buyHow))
			{
				list.Add(prices[i]);
			}
		}
		if (list.Count <= 0)
		{
			return null;
		}
		if (list.Count > 1)
		{
			return list[1];
		}
		return list[0];
	}

	public int GetDefaultPriceSel(BUY_HOW buyHow)
	{
		List<PriceTag> list = new List<PriceTag>();
		for (int i = 0; i < prices.Count; i++)
		{
			if (prices[i].CanBuy(buyHow))
			{
				list.Add(prices[i]);
			}
		}
		if (list.Count <= 0)
		{
			return -1;
		}
		if (list.Count > 1)
		{
			return 1;
		}
		return 0;
	}

	public string GetPriceString(int sel, BUY_HOW buyHow)
	{
		if (sel >= prices.Count)
		{
			return string.Empty;
		}
		int percent = 0;
		if (BuildOption.Instance.Props.usePriceDiscount && tItem.type == TItem.TYPE.WEAPON)
		{
			TWeapon tWeapon = (TWeapon)tItem;
			percent = tWeapon.GetDiscountRatio();
		}
		int num = 0;
		for (int i = 0; i < prices.Count; i++)
		{
			if (prices[i].CanBuy(buyHow))
			{
				if (num == sel)
				{
					return prices[i].GetPriceString(buyHow, percent);
				}
				num++;
			}
		}
		return string.Empty;
	}

	public string GetDefaultPrice()
	{
		BUY_HOW buyHow = BUY_HOW.GENERAL_POINT;
		PriceTag defaultPriceTag = GetDefaultPriceTag(buyHow);
		if (defaultPriceTag == null)
		{
			return string.Empty;
		}
		int percent = 0;
		if (BuildOption.Instance.Props.usePriceDiscount && tItem.type == TItem.TYPE.WEAPON)
		{
			TWeapon tWeapon = (TWeapon)tItem;
			percent = tWeapon.GetDiscountRatio();
		}
		return defaultPriceTag.GetPriceString(buyHow, percent);
	}

	public string GetDefaultBrickPrice()
	{
		BUY_HOW buyHow = BUY_HOW.BRICK_POINT;
		PriceTag defaultPriceTag = GetDefaultPriceTag(buyHow);
		if (defaultPriceTag == null)
		{
			return string.Empty;
		}
		int percent = 0;
		if (tItem.type == TItem.TYPE.WEAPON)
		{
			TWeapon tWeapon = (TWeapon)tItem;
			percent = tWeapon.GetDiscountRatio();
		}
		return defaultPriceTag.GetPriceString(buyHow, percent);
	}

	public string GetDefaultTokenPrice()
	{
		BUY_HOW buyHow = BUY_HOW.CASH_POINT;
		PriceTag defaultPriceTag = GetDefaultPriceTag(buyHow);
		if (defaultPriceTag == null)
		{
			return string.Empty;
		}
		int percent = 0;
		if (BuildOption.Instance.Props.usePriceDiscount && tItem.type == TItem.TYPE.WEAPON)
		{
			TWeapon tWeapon = (TWeapon)tItem;
			percent = tWeapon.GetDiscountRatio();
		}
		return defaultPriceTag.GetPriceString(buyHow, percent);
	}

	public string GetDefaultOption(BUY_HOW buyHow)
	{
		PriceTag defaultPriceTag = GetDefaultPriceTag(buyHow);
		if (defaultPriceTag == null || tItem == null)
		{
			return string.Empty;
		}
		return defaultPriceTag.GetRemainString(tItem.IsAmount);
	}

	public int GetLevelComparitor()
	{
		int result = 0;
		if (IsPointable && IsCashable)
		{
			result = Math.Min(minlvFp, minlvTk);
		}
		else if (!IsPointable)
		{
			result = minlvTk;
		}
		else if (!IsCashable)
		{
			result = minlvFp;
		}
		return result;
	}

	public int CompareBySeasonAndName(Good a)
	{
		if (HotNewValue != a.HotNewValue)
		{
			return HotNewValue.CompareTo(a.HotNewValue);
		}
		int num = tItem.season;
		int num2 = a.tItem.season;
		if (num <= 0)
		{
			num = 1;
		}
		if (num2 <= 0)
		{
			num2 = 1;
		}
		if (num != num2)
		{
			return -tItem.season.CompareTo(a.tItem.season);
		}
		return tItem.Name.CompareTo(a.tItem.Name);
	}

	public int Compare(Good a, int myLevel)
	{
		if (!BuildOption.Instance.Props.itemBuyLimit)
		{
			return CompareBySeasonAndName(a);
		}
		int levelComparitor = GetLevelComparitor();
		int levelComparitor2 = a.GetLevelComparitor();
		if (levelComparitor <= myLevel && levelComparitor2 > myLevel)
		{
			return -1;
		}
		if (levelComparitor > myLevel && levelComparitor2 <= myLevel)
		{
			return 1;
		}
		int num = 1;
		if (levelComparitor <= myLevel && levelComparitor2 <= myLevel)
		{
			num = -1;
		}
		if (levelComparitor == levelComparitor2)
		{
			return CompareBySeasonAndName(a);
		}
		return levelComparitor.CompareTo(levelComparitor2) * num;
	}

	public int GetCashback()
	{
		BUY_HOW buyHow = BUY_HOW.CASH_POINT;
		PriceTag defaultPriceTag = GetDefaultPriceTag(buyHow);
		if (defaultPriceTag == null)
		{
			return 0;
		}
		if (defaultPriceTag.CashBack > 0)
		{
			return defaultPriceTag.CashBack;
		}
		return 0;
	}
}
