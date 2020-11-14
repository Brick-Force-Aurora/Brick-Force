using System.Collections.Generic;
using UnityEngine;

public class MemoManager : MonoBehaviour
{
	private Dictionary<long, Memo> dic;

	private static MemoManager _instance;

	public static MemoManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(MemoManager)) as MemoManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the MemoManager Instance");
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		dic = new Dictionary<long, Memo>();
		Object.DontDestroyOnLoad(this);
	}

	public Memo[] ToArray()
	{
		List<Memo> list = new List<Memo>();
		foreach (KeyValuePair<long, Memo> item in dic)
		{
			list.Add(item.Value);
		}
		List<Memo> list2 = new List<Memo>();
		for (int num = list.Count - 1; num >= 0; num--)
		{
			list2.Add(list[num]);
		}
		return list2.ToArray();
	}

	public int GetOption(long seq)
	{
		if (dic.ContainsKey(seq))
		{
			return dic[seq].option;
		}
		return -1;
	}

	public void ClearPresent(long seq)
	{
		if (dic.ContainsKey(seq))
		{
			dic[seq].attached = string.Empty;
			dic[seq].option = 0;
			dic[seq].check = false;
		}
	}

	public void Clear()
	{
		dic.Clear();
	}

	public void Add(long seq, Memo memo)
	{
		if (!dic.ContainsKey(seq))
		{
			dic.Add(seq, memo);
		}
	}

	public void Del(long seq)
	{
		if (dic.ContainsKey(seq))
		{
			dic.Remove(seq);
		}
	}

	public bool HaveUnreadMemo()
	{
		foreach (KeyValuePair<long, Memo> item in dic)
		{
			if (!item.Value.IsRead)
			{
				return true;
			}
		}
		return false;
	}

	public int GetUnreadMemoCount()
	{
		int num = 0;
		foreach (KeyValuePair<long, Memo> item in dic)
		{
			if (!item.Value.IsRead)
			{
				num++;
			}
		}
		return num;
	}

	public int GetMemoCountPercent()
	{
		if (dic == null)
		{
			return 0;
		}
		return dic.Count;
	}
}
