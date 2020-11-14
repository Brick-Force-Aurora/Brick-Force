using UnityEngine;

public class PriceTag
{
	public int option;

	private int point;

	private int cash;

	private int cashBack;

	private int brick;

	private int pointDiscount;

	private int brickDiscount;

	private int cashDiscount;

	private int dscntStart;

	private int dscntEnd;

	private int vsblStart;

	private int vsblEnd;

	public bool IsVisible => vsblStart <= 0 && vsblEnd > 0;

	public bool IsDiscount => dscntStart <= 0 && dscntEnd > 0;

	public int Point => (!IsDiscount || pointDiscount <= 0) ? point : pointDiscount;

	public int Cash => (!IsDiscount || cashDiscount <= 0) ? cash : cashDiscount;

	public int Brick => (!IsDiscount || brickDiscount <= 0) ? brick : brickDiscount;

	public int PointOrg => point;

	public int BrickOrg => brick;

	public int CashOrg => cash;

	public int CashBack => cashBack;

	public PriceTag(int _option, int _point, int _brick, int _cash, int _cashBack, int _pointDiscount, int _brickDiscount, int _cashDiscount, int _dscntStart, int _dscntEnd, int _vsblStart, int _vsblEnd)
	{
		option = _option;
		point = _point;
		brick = _brick;
		cash = _cash;
		cashBack = _cashBack;
		pointDiscount = _pointDiscount;
		brickDiscount = _brickDiscount;
		cashDiscount = _cashDiscount;
		dscntStart = _dscntStart;
		dscntEnd = _dscntEnd;
		vsblStart = _vsblStart;
		vsblEnd = _vsblEnd;
	}

	public bool CanBuy(Good.BUY_HOW buyHow)
	{
		switch (buyHow)
		{
		case Good.BUY_HOW.GENERAL_POINT:
			return IsVisible && Point > 0;
		case Good.BUY_HOW.BRICK_POINT:
			return IsVisible && Brick > 0;
		case Good.BUY_HOW.CASH_POINT:
			return IsVisible && Cash > 0;
		default:
			return false;
		}
	}

	public bool CanBuy(Good.BUY_HOW buyHow, bool rebuy)
	{
		if (rebuy)
		{
			switch (buyHow)
			{
			case Good.BUY_HOW.GENERAL_POINT:
				return IsVisible && Point > 0 && option < 1000000;
			case Good.BUY_HOW.BRICK_POINT:
				return IsVisible && Brick > 0 && option < 1000000;
			case Good.BUY_HOW.CASH_POINT:
				return IsVisible && Cash > 0 && option < 1000000;
			default:
				return false;
			}
		}
		return CanBuy(buyHow);
	}

	public void OnEverySec()
	{
		if (dscntStart > 0)
		{
			dscntStart--;
		}
		if (dscntEnd > 0)
		{
			dscntEnd--;
		}
		if (vsblStart > 0)
		{
			vsblStart--;
		}
		if (vsblEnd > 0)
		{
			vsblEnd--;
		}
	}

	public string GetOptionString(Good.BUY_HOW buyHow, bool isAmount, int percent)
	{
		string remainString = GetRemainString(isAmount);
		int num = 0;
		switch (buyHow)
		{
		case Good.BUY_HOW.GENERAL_POINT:
			if (!CanBuy(buyHow))
			{
				return string.Empty;
			}
			num = Point - Mathf.CeilToInt((float)Point * ((float)percent / 100f));
			return remainString + "/" + num.ToString("n0") + " " + StringMgr.Instance.Get("POINT");
		case Good.BUY_HOW.BRICK_POINT:
			if (!CanBuy(buyHow))
			{
				return string.Empty;
			}
			num = Brick - Mathf.CeilToInt((float)Brick * ((float)percent / 100f));
			return remainString + "/" + num.ToString("n0") + " " + StringMgr.Instance.Get("BRICK_POINT");
		case Good.BUY_HOW.CASH_POINT:
			if (!CanBuy(buyHow))
			{
				return string.Empty;
			}
			num = Cash - Mathf.CeilToInt((float)Cash * ((float)percent / 100f));
			return remainString + "/" + num.ToString("n0") + " " + TokenManager.Instance.GetTokenString();
		default:
			return string.Empty;
		}
	}

	public string GetRemainString(bool isAmount)
	{
		if (option >= 1000000)
		{
			return StringMgr.Instance.Get("INFINITE");
		}
		if (isAmount)
		{
			return option.ToString() + " " + StringMgr.Instance.Get("TIMES_UNIT");
		}
		return option.ToString() + " " + StringMgr.Instance.Get("DAYS");
	}

	public string GetPriceString(Good.BUY_HOW buyHow, int percent)
	{
		int num = 0;
		switch (buyHow)
		{
		case Good.BUY_HOW.GENERAL_POINT:
			return (Point - Mathf.CeilToInt((float)Point * ((float)percent / 100f))).ToString("n0");
		case Good.BUY_HOW.BRICK_POINT:
			return (Brick - Mathf.CeilToInt((float)Brick * ((float)percent / 100f))).ToString("n0");
		case Good.BUY_HOW.CASH_POINT:
			return (Cash - Mathf.CeilToInt((float)Cash * ((float)percent / 100f))).ToString("n0");
		default:
			return string.Empty;
		}
	}

	public int GetPrice(Good.BUY_HOW buyHow, int percent)
	{
		if (percent == 0)
		{
			return GetPrice(buyHow);
		}
		int result = 0;
		switch (buyHow)
		{
		case Good.BUY_HOW.GENERAL_POINT:
			result = Point - Mathf.CeilToInt((float)Point * ((float)percent / 100f));
			break;
		case Good.BUY_HOW.BRICK_POINT:
			result = Brick - Mathf.CeilToInt((float)Brick * ((float)percent / 100f));
			break;
		case Good.BUY_HOW.CASH_POINT:
			result = Cash - Mathf.CeilToInt((float)Cash * ((float)percent / 100f));
			break;
		}
		return result;
	}

	public int GetPrice(Good.BUY_HOW buyHow)
	{
		switch (buyHow)
		{
		case Good.BUY_HOW.GENERAL_POINT:
			return Point;
		case Good.BUY_HOW.BRICK_POINT:
			return Brick;
		case Good.BUY_HOW.CASH_POINT:
			return Cash;
		default:
			return -1;
		}
	}
}
