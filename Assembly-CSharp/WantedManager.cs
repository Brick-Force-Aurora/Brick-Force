using System.Collections.Generic;
using UnityEngine;

public class WantedManager : MonoBehaviour
{
	private static WantedManager _instance;

	private List<int> list = new List<int>();

	public static WantedManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(WantedManager)) as WantedManager);
			}
			return _instance;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	public int[] ToArray()
	{
		return list.ToArray();
	}

	public void ResetGameStuff()
	{
		list.Clear();
	}

	public bool IsWanted(int seq)
	{
		int num = list.IndexOf(seq);
		return num >= 0;
	}

	public void AddWanted(int wanted)
	{
		if ((RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.TEAM_MATCH || RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.INDIVIDUAL) && BuildOption.Instance.Props.UseWanted && RoomManager.Instance.Wanted)
		{
			int num = list.IndexOf(wanted);
			if (num < 0)
			{
				BrickManDesc desc = BrickManManager.Instance.GetDesc(wanted);
				if (wanted == MyInfoManager.Instance.Seq || desc != null)
				{
					list.Add(wanted);
				}
			}
		}
	}

	public void DelWanted(int wanted)
	{
		int num = list.IndexOf(wanted);
		if (0 <= num)
		{
			list.RemoveAt(num);
		}
	}

	public int GetWantedHpMaxBoost(int seq, int hpDefault)
	{
		int result = 0;
		if (IsWanted(seq))
		{
			result = (int)(BuildOption.Instance.Props.wantedOpt.hpMaxUp * (float)hpDefault);
		}
		return result;
	}

	public float GetWantedAtkPowBoost(int seq, float atkPow)
	{
		float result = 0f;
		if (IsWanted(seq))
		{
			result = BuildOption.Instance.Props.wantedOpt.atkPowUp * atkPow;
		}
		return result;
	}
}
