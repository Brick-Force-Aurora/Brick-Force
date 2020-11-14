using System.Collections.Generic;
using UnityEngine;

public class UskManager : MonoBehaviour
{
	private Dictionary<string, Texture> dic;

	public bool bLoaded;

	private static UskManager _instance;

	public static UskManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(UskManager)) as UskManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the UskManager Instance");
				}
			}
			return _instance;
		}
	}

	public void Add(string key, Texture clip)
	{
		key = key.ToLower();
		if (!dic.ContainsKey(key))
		{
			dic.Add(key, clip);
		}
	}

	public Texture Get(string key)
	{
		key = key.ToLower();
		if (!dic.ContainsKey(key))
		{
			return null;
		}
		return dic[key];
	}

	public void Clear()
	{
		if (dic != null && dic.Count > 0)
		{
			dic.Clear();
		}
	}

	private void Awake()
	{
		dic = new Dictionary<string, Texture>();
		Object.DontDestroyOnLoad(this);
	}
}
