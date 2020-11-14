using System.Collections.Generic;
using UnityEngine;

public class SquadManager : MonoBehaviour
{
	private Dictionary<int, Squad> dicSquad;

	private int squad;

	private Dictionary<int, NameCard> dicMember;

	private bool isMatching;

	private static SquadManager _instance;

	public Squad CurSquad
	{
		get
		{
			if (dicSquad.ContainsKey(squad))
			{
				return dicSquad[squad];
			}
			return null;
		}
	}

	public int Squad => squad;

	public bool IsMatching
	{
		get
		{
			return isMatching;
		}
		set
		{
			isMatching = value;
		}
	}

	public static SquadManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(SquadManager)) as SquadManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the SquadManager Instance");
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		dicSquad = new Dictionary<int, Squad>();
		dicMember = new Dictionary<int, NameCard>();
		squad = -1;
		Object.DontDestroyOnLoad(this);
	}

	public void Join(int index)
	{
		squad = index;
	}

	public void Clear()
	{
		Leave();
		dicSquad.Clear();
	}

	public void Leave()
	{
		dicMember.Clear();
		squad = -1;
		isMatching = false;
	}

	public Squad GetSquad(int index)
	{
		if (!dicSquad.ContainsKey(index))
		{
			return null;
		}
		return dicSquad[index];
	}

	public void UpdateAlways(int clan, int index, int memberCount, int maxMember, int win, int draw, int lose, int leaderSeq, string leaderNickname)
	{
		if (!dicSquad.ContainsKey(index))
		{
			dicSquad.Add(index, new Squad(index, memberCount, maxMember, win, draw, lose, leaderSeq, leaderNickname));
		}
		else
		{
			dicSquad[index].MemberCount = memberCount;
			dicSquad[index].MaxMember = maxMember;
			dicSquad[index].WinCount = win;
			dicSquad[index].DrawCount = draw;
			dicSquad[index].LoseCount = lose;
			dicSquad[index].Leader = leaderSeq;
			dicSquad[index].TeamLeader = leaderNickname;
		}
	}

	public void Del(int clan, int index)
	{
		if (dicSquad.ContainsKey(index))
		{
			dicSquad.Remove(index);
		}
	}

	public Squad[] GetSquadArray()
	{
		List<Squad> list = new List<Squad>();
		foreach (KeyValuePair<int, Squad> item in dicSquad)
		{
			list.Add(item.Value);
		}
		return list.ToArray();
	}

	public NameCard GetSquadMember(int seq)
	{
		if (dicMember.ContainsKey(seq))
		{
			return dicMember[seq];
		}
		return null;
	}

	public NameCard[] GetSquadMemberArray()
	{
		List<NameCard> list = new List<NameCard>();
		foreach (KeyValuePair<int, NameCard> item in dicMember)
		{
			list.Add(item.Value);
		}
		return list.ToArray();
	}

	public NameCard[] GetSquadMemberArrayInclueMe()
	{
		List<NameCard> list = new List<NameCard>();
		int level = XpManager.Instance.GetLevel(MyInfoManager.Instance.Xp);
		NameCard item = new NameCard(MyInfoManager.Instance.Seq, MyInfoManager.Instance.Nickname, level, -1, MyInfoManager.Instance.Rank);
		list.Add(item);
		foreach (KeyValuePair<int, NameCard> item2 in dicMember)
		{
			list.Add(item2.Value);
		}
		return list.ToArray();
	}

	public void AddMember(int seq, string nickname, int xp, int rank)
	{
		if (!dicMember.ContainsKey(seq))
		{
			dicMember.Add(seq, new NameCard(seq, nickname, XpManager.Instance.GetLevel(xp), -1, rank));
		}
	}

	public void DelMember(int seq)
	{
		if (dicMember.ContainsKey(seq))
		{
			dicMember.Remove(seq);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
