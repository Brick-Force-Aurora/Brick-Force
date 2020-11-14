using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
	private Dictionary<string, Good> dic;

	private Dictionary<string, Good> cache;

	private List<string> logPromo;

	private List<string> logBought;

	private float deltaTime;

	private bool isLoaded;

	private static ShopManager instance;

	public bool IsLoaded => isLoaded;

	public static ShopManager Instance
	{
		get
		{
			if (null == instance)
			{
				instance = (Object.FindObjectOfType(typeof(ShopManager)) as ShopManager);
				if (null == instance)
				{
					Debug.LogError("ERROR, Fail to get the ShopManager Instance");
				}
			}
			return instance;
		}
	}

	private void OnApplicationQuit()
	{
	}

	private void Awake()
	{
		dic = new Dictionary<string, Good>();
		cache = new Dictionary<string, Good>();
		logPromo = new List<string>();
		logBought = new List<string>();
		Object.DontDestroyOnLoad(this);
	}

	private IEnumerator LoadFromWWW()
	{
		Property prop = BuildOption.Instance.Props;
		string urlShop = "http://" + prop.GetResourceServer + "/BfData/Template/shop.txt.cooked";
		WWW wwwShop = new WWW(urlShop);
		yield return (object)wwwShop;
		string urlShopCat = "http://" + prop.GetResourceServer + "/BfData/Template/shopCategory.txt.cooked";
		WWW wwwShopCat = new WWW(urlShopCat);
		yield return (object)wwwShopCat;
		using (MemoryStream msShop = new MemoryStream(wwwShop.bytes))
		{
			using (BinaryReader rdShop = new BinaryReader(msShop))
			{
				using (MemoryStream msShopCat = new MemoryStream(wwwShopCat.bytes))
				{
					using (BinaryReader rdShopCat = new BinaryReader(msShopCat))
					{
						CSVLoader csvShop = new CSVLoader();
						CSVLoader csvShopCat = new CSVLoader();
						if (csvShop.SecuredLoadFromBinaryReader(rdShop) && csvShopCat.SecuredLoadFromBinaryReader(rdShopCat))
						{
							ParseData(csvShopCat, csvShop);
							isLoaded = true;
						}
					}
				}
			}
		}
		if (!isLoaded)
		{
			Debug.LogError("Fail to download " + urlShop + "," + urlShopCat);
		}
	}

	private bool LoadFromLocalFileSystem()
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for Resources");
			return false;
		}
		string text2 = Path.Combine(text, "Template/shop.txt");
		string text3 = Path.Combine(text, "Template/shopCategory.txt");
		CSVLoader cSVLoader = new CSVLoader();
		CSVLoader cSVLoader2 = new CSVLoader();
		if (Application.platform == RuntimePlatform.WindowsEditor || !cSVLoader.SecuredLoad(text2))
		{
			if (!cSVLoader.Load(text2))
			{
				Debug.LogError("ERROR, Fail to load resource file" + text2);
				return false;
			}
			if (!cSVLoader.SecuredSave(text2))
			{
				Debug.LogError("ERROR, Load success " + text2 + " but save secured failed");
			}
		}
		if (Application.platform == RuntimePlatform.WindowsEditor || !cSVLoader2.SecuredLoad(text3))
		{
			if (!cSVLoader2.Load(text3))
			{
				Debug.LogError("ERROR, Fail to load resource file" + text3);
				return false;
			}
			if (!cSVLoader2.SecuredSave(text3))
			{
				Debug.LogError("ERROR, Load success " + text3 + " but save secured failed");
			}
		}
		ParseData(cSVLoader2, cSVLoader);
		return true;
	}

	private void ParseData(CSVLoader csvShopCat, CSVLoader csvShop)
	{
		for (int i = 0; i < csvShopCat.Rows; i++)
		{
			csvShopCat.ReadValue(0, i, string.Empty, out string Value);
			csvShopCat.ReadValue(1, i, 0, out int Value2);
			csvShopCat.ReadValue(2, i, 0, out int Value3);
			csvShopCat.ReadValue(3, i, 0, out int Value4);
			csvShopCat.ReadValue(4, i, 0, out int Value5);
			csvShopCat.ReadValue(5, i, 0, out int Value6);
			csvShopCat.ReadValue(6, i, 0, out int Value7);
			csvShopCat.ReadValue(7, i, 0, out int Value8);
			csvShopCat.ReadValue(8, i, 0, out int Value9);
			csvShopCat.ReadValue(9, i, 0, out int Value10);
			csvShopCat.ReadValue(10, i, 0, out int Value11);
			csvShopCat.ReadValue(11, i, 0, out int Value12);
			Value = Value.Trim().ToLower();
			TItem tItem = TItemManager.Instance.Get<TItem>(Value);
			if (dic.ContainsKey(Value))
			{
				Debug.LogError("Duplicated good found for " + Value);
			}
			else if (tItem == null)
			{
				Debug.LogError("Fail to find item template : " + Value);
			}
			else
			{
				tItem._StarRate = Value5;
				dic.Add(Value, new Good(Value, tItem, Value2 == 1, Value3 == 1, Value4 == 1, Value6 == 1, Value7 == 1, Value8 == 1, Value9 == 1, Value10 == 1, (sbyte)Value11, (sbyte)Value12));
			}
		}
		for (int j = 0; j < csvShop.Rows; j++)
		{
			csvShop.ReadValue(0, j, string.Empty, out string Value13);
			csvShop.ReadValue(1, j, 0, out int Value14);
			csvShop.ReadValue(2, j, 0, out int Value15);
			csvShop.ReadValue(3, j, 0, out int Value16);
			csvShop.ReadValue(4, j, 0, out int Value17);
			csvShop.ReadValue(5, j, 0, out int Value18);
			Value13 = Value13.Trim().ToLower();
			if (!dic.ContainsKey(Value13))
			{
				Debug.Log("Fail to find good : %d" + Value13);
			}
			else
			{
				dic[Value13].AddPrice(Value14, Value15, Value16, Value17, Value18, 0, 0, 0, 0, 0, 0, 314816281);
			}
		}
	}

	public void Load()
	{
		dic.Clear();
		Property props = BuildOption.Instance.Props;
		if (!props.loadShopTxt && !Application.isEditor)
		{
			isLoaded = true;
		}
		else if (props.isWebPlayer)
		{
			StartCoroutine(LoadFromWWW());
		}
		else
		{
			isLoaded = LoadFromLocalFileSystem();
		}
	}

	public bool mePromo(string code)
	{
		for (int i = 0; i < logPromo.Count; i++)
		{
			if (logPromo[i] == code)
			{
				return true;
			}
		}
		return false;
	}

	public void LogPromo(string promo)
	{
		for (int i = 0; i < logPromo.Count; i++)
		{
			if (logPromo[i] == promo)
			{
				return;
			}
		}
		logPromo.Add(promo);
	}

	public bool meBought(string code)
	{
		for (int i = 0; i < logBought.Count; i++)
		{
			if (logBought[i] == code)
			{
				return true;
			}
		}
		return false;
	}

	public void LogBought(string bought)
	{
		for (int i = 0; i < logBought.Count; i++)
		{
			if (logBought[i] == bought)
			{
				return;
			}
		}
		logBought.Add(bought);
	}

	public void Cache(string code, sbyte isNew, sbyte isHot, sbyte isVisible, byte starRate, sbyte specialOffer, sbyte offerOnce, sbyte giftable, sbyte promo, int opt, int pointPrice, int brickPrice, int cashPrice, int cashBack, int dscntPoint, int dscntBrick, int dscntCash, int dscntStart, int dscntEnd, int vsblStart, int vsblEnd, sbyte rebuyInvisible, sbyte minlvFp, sbyte minlvTk)
	{
		TItem tItem = TItemManager.Instance.Get<TItem>(code);
		if (tItem != null)
		{
			starRate = (byte)Mathf.Abs(starRate);
			tItem._StarRate = starRate;
			if (!cache.ContainsKey(code))
			{
				cache.Add(code, new Good(code, tItem, isNew == 1, isHot == 1, isVisible == 1, specialOffer == 1, offerOnce == 1, giftable == 1, promo == 1, rebuyInvisible == 1, minlvFp, minlvTk));
			}
			cache[code].AddPrice(opt, pointPrice, brickPrice, cashPrice, cashBack, dscntPoint, dscntBrick, dscntCash, dscntStart, dscntEnd, vsblStart, vsblEnd);
		}
	}

	public void CacheDone()
	{
		logPromo.Clear();
		logBought.Clear();
		foreach (KeyValuePair<string, Good> item in cache)
		{
			if (dic.ContainsKey(item.Key))
			{
				dic[item.Key] = item.Value;
			}
			else
			{
				dic.Add(item.Key, item.Value);
			}
		}
		cache.Clear();
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if (deltaTime > 1f)
		{
			deltaTime = 0f;
			foreach (KeyValuePair<string, Good> item in dic)
			{
				item.Value.OnEverySec();
			}
		}
	}

	public Good Get(string code)
	{
		if (!dic.ContainsKey(code))
		{
			return null;
		}
		return dic[code];
	}

	public Good[] GetSpecialOffers(int classify, int myLevel)
	{
		List<Good> list = new List<Good>();
		foreach (KeyValuePair<string, Good> item in dic)
		{
			if (item.Value.isSpecialOffer && item.Value.IsVisible())
			{
				switch (classify)
				{
				case 1:
					if (!item.Value.IsCashable)
					{
						goto default;
					}
					goto case 0;
				default:
					if ((classify != 2 || !item.Value.IsPointable) && (classify != 3 || !item.Value.IsBrickPointable))
					{
						break;
					}
					goto case 0;
				case 0:
					list.Add(item.Value);
					break;
				}
			}
		}
		list.Sort((Good prev, Good next) => prev.Compare(next, myLevel));
		return list.ToArray();
	}

	public void InitAllGoods()
	{
		foreach (KeyValuePair<string, Good> item in dic)
		{
			if (item.Value.IsVisible())
			{
				item.Value.Check = false;
				item.Value.BuyErr = string.Empty;
			}
		}
	}

	public bool IsMultiBuy()
	{
		int num = 0;
		foreach (KeyValuePair<string, Good> item in dic)
		{
			if (item.Value.IsVisible())
			{
				if (item.Value.Check)
				{
					num++;
				}
				if (num > 1)
				{
					return true;
				}
			}
		}
		return false;
	}

	public Good[] GetCheckedAll()
	{
		List<Good> list = new List<Good>();
		foreach (KeyValuePair<string, Good> item in dic)
		{
			if (item.Value.IsVisible() && item.Value.Check)
			{
				list.Add(item.Value);
			}
		}
		return list.ToArray();
	}

	public Good[] GetAll(int classify, int myLevel)
	{
		List<Good> list = new List<Good>();
		foreach (KeyValuePair<string, Good> item in dic)
		{
			if (item.Value.IsVisible())
			{
				switch (classify)
				{
				case 1:
					if (!item.Value.IsCashable)
					{
						goto default;
					}
					goto case 0;
				default:
					if ((classify != 2 || !item.Value.IsPointable) && (classify != 3 || !item.Value.IsBrickPointable))
					{
						break;
					}
					goto case 0;
				case 0:
					list.Add(item.Value);
					break;
				}
			}
		}
		list.Sort((Good prev, Good next) => prev.Compare(next, myLevel));
		return list.ToArray();
	}

	public Good[] GetNew(int classify, int myLevel)
	{
		List<Good> list = new List<Good>();
		foreach (KeyValuePair<string, Good> item in dic)
		{
			if (item.Value.isNew && item.Value.IsVisible())
			{
				switch (classify)
				{
				case 1:
					if (!item.Value.IsCashable)
					{
						goto default;
					}
					goto case 0;
				default:
					if ((classify != 2 || !item.Value.IsPointable) && (classify != 3 || !item.Value.IsBrickPointable))
					{
						break;
					}
					goto case 0;
				case 0:
					list.Add(item.Value);
					break;
				}
			}
		}
		list.Sort((Good prev, Good next) => prev.Compare(next, myLevel));
		return list.ToArray();
	}

	public Good[] GetHot(int classify, int myLevel)
	{
		List<Good> list = new List<Good>();
		foreach (KeyValuePair<string, Good> item in dic)
		{
			if (item.Value.isHot && item.Value.IsVisible())
			{
				switch (classify)
				{
				case 1:
					if (!item.Value.IsCashable)
					{
						goto default;
					}
					goto case 0;
				default:
					if ((classify != 2 || !item.Value.IsPointable) && (classify != 3 || !item.Value.IsBrickPointable))
					{
						break;
					}
					goto case 0;
				case 0:
					list.Add(item.Value);
					break;
				}
			}
		}
		list.Sort((Good prev, Good next) => prev.Compare(next, myLevel));
		return list.ToArray();
	}

	public Good[] GetGoodsByCat(int catType, int catKind, int classify, int category, int myLevel)
	{
		List<Good> list = new List<Good>();
		foreach (KeyValuePair<string, Good> item in dic)
		{
			if (item.Value.tItem.catType == catType && item.Value.IsVisible() && (catKind <= 0 || item.Value.tItem.catKind == catKind))
			{
				switch (classify)
				{
				case 1:
					if (!item.Value.IsCashable)
					{
						goto default;
					}
					goto case 0;
				default:
					if ((classify != 2 || !item.Value.IsPointable) && (classify != 3 || !item.Value.IsBrickPointable))
					{
						break;
					}
					goto case 0;
				case 0:
					if (category >= 0)
					{
						TWeapon tWeapon = (TWeapon)item.Value.tItem;
						if (tWeapon.cat == category)
						{
							list.Add(item.Value);
						}
					}
					else
					{
						list.Add(item.Value);
					}
					break;
				}
			}
		}
		list.Sort((Good prev, Good next) => prev.Compare(next, myLevel));
		return list.ToArray();
	}

	public Good[] GetAvailableItemsCurrentLevel(int level)
	{
		List<Good> list = new List<Good>();
		foreach (KeyValuePair<string, Good> item in dic)
		{
			if (level == item.Value.minlvFp || level == item.Value.minlvTk)
			{
				list.Add(item.Value);
			}
		}
		return list.ToArray();
	}

	public Good[] GetRelateGoods(string groupName)
	{
		List<Good> list = new List<Good>();
		foreach (KeyValuePair<string, Good> item in dic)
		{
			if (item.Value.IsVisible() && (groupName == item.Value.tItem.grp1 || groupName == item.Value.tItem.grp2 || groupName == item.Value.tItem.grp3))
			{
				list.Add(item.Value);
			}
		}
		return list.ToArray();
	}
}
