using System.Collections.Generic;
using UnityEngine;

public class TreasureChestManager : MonoBehaviour
{
	private Dictionary<int, TcStatus> dicTc;

	private static TreasureChestManager _instance;

	public static TreasureChestManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(TreasureChestManager)) as TreasureChestManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the TreasureChestManager Instance");
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		dicTc = new Dictionary<int, TcStatus>();
		Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
	}

	public TcStatus Get(int seq)
	{
		if (!dicTc.ContainsKey(seq))
		{
			return null;
		}
		return dicTc[seq];
	}

	public void Refresh(int seq, int cur, int key, int maxKey)
	{
		if (dicTc.ContainsKey(seq))
		{
			dicTc[seq].Update(cur, key, maxKey);
		}
	}

	public void UpdateAlways(int seq, int index, int max, int cur, int key, int maxKey, int coinPrice, int tokenPrice, string alias)
	{
		if (dicTc.ContainsKey(seq))
		{
			dicTc[seq].Update(max, cur, key, maxKey);
		}
		else
		{
			dicTc.Add(seq, new TcStatus(seq, index, max, cur, key, maxKey, coinPrice, tokenPrice, alias));
		}
	}

	public TcStatus[] ToArray()
	{
		List<TcStatus> list = new List<TcStatus>();
		foreach (KeyValuePair<int, TcStatus> item in dicTc)
		{
			list.Add(item.Value);
		}
		return list.ToArray();
	}

	private void Update()
	{
	}
}
