using System.Collections.Generic;
using UnityEngine;

public class ChannelUserManager : MonoBehaviour
{
	private Dictionary<int, NameCard> dic;

	private float deltaTime;

	private static ChannelUserManager _instance;

	public static ChannelUserManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(ChannelUserManager)) as ChannelUserManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the ChannelUserManager Instance");
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		dic = new Dictionary<int, NameCard>();
		Object.DontDestroyOnLoad(this);
	}

	public void Refresh()
	{
		deltaTime += Time.deltaTime;
		if (deltaTime > 3f)
		{
			deltaTime = 0f;
			CSNetManager.Instance.Sock.SendCS_CHANNEL_PLAYER_LIST_REQ();
		}
	}

	public void AddUser(int seq, string nickname, int lv, int rank)
	{
		if (!dic.ContainsKey(seq))
		{
			dic.Add(seq, new NameCard(seq, nickname, lv, ChannelManager.Instance.CurChannelId, rank));
		}
		else
		{
			dic[seq].Lv = lv;
			dic[seq].Rank = rank;
			dic[seq].SvrId = ChannelManager.Instance.CurChannelId;
		}
	}

	public void Clear()
	{
		deltaTime = 0f;
		dic.Clear();
	}

	public void DelUser(int seq)
	{
		if (dic.ContainsKey(seq))
		{
			dic.Remove(seq);
		}
	}

	public NameCard GetUser(int seq)
	{
		if (dic.ContainsKey(seq))
		{
			return dic[seq];
		}
		return null;
	}

	public bool ContainsUser(int seq)
	{
		return dic.ContainsKey(seq);
	}

	public NameCard[] ToArray()
	{
		List<NameCard> list = new List<NameCard>();
		foreach (KeyValuePair<int, NameCard> item in dic)
		{
			list.Add(item.Value);
		}
		return list.ToArray();
	}
}
