using System.Collections.Generic;

public class TcStatus
{
	private int seq;

	private int index;

	private int max;

	private int cur;

	private int key;

	private int maxKey;

	private int coinPrice;

	private int tokenPrice;

	private string alias;

	public List<TcTItem> listTItems = new List<TcTItem>();

	public int Seq => seq;

	public int Index => index;

	public int Max => max;

	public int Cur => cur;

	public int Key => key;

	public int MaxKey => maxKey;

	public int CoinPrice => coinPrice;

	public int TokenPrice => tokenPrice;

	public string Alias => alias;

	public float Chance => (float)key / (float)cur * 100f;

	public TcStatus(int _seq, int _index, int _max, int _cur, int _key, int _maxKey, int _coinPrice, int _tokenPrice, string _alias)
	{
		seq = _seq;
		index = _index;
		max = _max;
		cur = _cur;
		key = _key;
		maxKey = _maxKey;
		coinPrice = _coinPrice;
		tokenPrice = _tokenPrice;
		alias = _alias;
	}

	public void Update(int _cur, int _key, int _maxKey)
	{
		cur = _cur;
		key = _key;
		maxKey = _maxKey;
	}

	public void Update(int _max, int _cur, int _key, int _maxKey)
	{
		max = _max;
		cur = _cur;
		key = _key;
		maxKey = _maxKey;
	}

	public string GetTitle()
	{
		return alias;
	}

	public string GetDescription()
	{
		return cur.ToString() + "/" + max.ToString();
	}

	public string GetKeyDescription()
	{
		return key.ToString() + "/" + maxKey.ToString();
	}

	public void ClearExpectations()
	{
		listTItems.Clear();
	}

	public void AddExpectations(TcTItem item)
	{
		listTItems.Add(item);
	}

	public TcTItem[] GetRareArray()
	{
		List<TcTItem> list = new List<TcTItem>();
		foreach (TcTItem listTItem in listTItems)
		{
			TcTItem current = listTItem;
			if (current.isKey)
			{
				list.Add(current);
			}
		}
		return list.ToArray();
	}

	public TcTItem[] GetNormalArray()
	{
		List<TcTItem> list = new List<TcTItem>();
		foreach (TcTItem listTItem in listTItems)
		{
			TcTItem current = listTItem;
			if (!current.isKey)
			{
				list.Add(current);
			}
		}
		return list.ToArray();
	}

	public TcTItem[] GetArraySorted()
	{
		List<TcTItem> list = new List<TcTItem>();
		foreach (TcTItem listTItem in listTItems)
		{
			TcTItem current = listTItem;
			if (current.isKey)
			{
				list.Add(current);
			}
		}
		foreach (TcTItem listTItem2 in listTItems)
		{
			TcTItem current2 = listTItem2;
			if (!current2.isKey)
			{
				list.Add(current2);
			}
		}
		return list.ToArray();
	}

	public TcTItem[] TcTItemToArray()
	{
		return listTItems.ToArray();
	}

	public TcTItem GetFirstRare()
	{
		foreach (TcTItem listTItem in listTItems)
		{
			TcTItem current = listTItem;
			if (current.isKey)
			{
				return current;
			}
		}
		return default(TcTItem);
	}
}
