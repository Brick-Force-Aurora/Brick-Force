using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
	private float deltaTime;

	private int wait = -1;

	private Dictionary<int, Mission> dicMission;

	private static MissionManager _instance;

	public bool HaveMission => dicMission.Count > 0;

	public bool CanReceiveMission => wait < 0 && dicMission.Count <= 0;

	public bool CanCompleteMission
	{
		get
		{
			if (dicMission.Count <= 0)
			{
				return false;
			}
			foreach (KeyValuePair<int, Mission> item in dicMission)
			{
				if (item.Value.CanComplete)
				{
					return true;
				}
			}
			return false;
		}
	}

	public static MissionManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(MissionManager)) as MissionManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the MissionManager Instance");
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		dicMission = new Dictionary<int, Mission>();
		Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
		wait = -1;
		deltaTime = 0f;
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if (deltaTime > 1f)
		{
			deltaTime = 0f;
			if (wait >= 0)
			{
				wait--;
			}
		}
	}

	public void Progress(int mission, int progress)
	{
		if (dicMission.ContainsKey(mission))
		{
			dicMission[mission].SetProgress(progress);
		}
	}

	public void UpdateAlways(int _index, string _description, int _goal, int _progress, bool _complete, int atleast)
	{
		if (dicMission.ContainsKey(_index))
		{
			dicMission[_index].SetMission(_progress, _complete);
		}
		else
		{
			dicMission.Add(_index, new Mission(_index, _description, _goal, _progress, _complete, atleast));
		}
	}

	public void SetMissionWait(int _wait)
	{
		wait = _wait;
	}

	public void Del(int _index)
	{
		dicMission.Remove(_index);
	}

	public void Clear()
	{
		dicMission.Clear();
	}

	public int CompletedCount()
	{
		int num = 0;
		foreach (KeyValuePair<int, Mission> item in dicMission)
		{
			if (item.Value.Completed)
			{
				num++;
			}
		}
		return num;
	}

	public Mission[] ToArray()
	{
		SortedList<int, Mission> sortedList = new SortedList<int, Mission>();
		foreach (KeyValuePair<int, Mission> item in dicMission)
		{
			sortedList.Add(item.Value.Index, item.Value);
		}
		List<Mission> list = new List<Mission>();
		foreach (KeyValuePair<int, Mission> item2 in sortedList)
		{
			list.Add(item2.Value);
		}
		return list.ToArray();
	}

	public void Complete(int count)
	{
		if (count >= 3)
		{
			Clear();
		}
		else
		{
			foreach (KeyValuePair<int, Mission> item in dicMission)
			{
				if (item.Value.CanComplete)
				{
					item.Value.Complete();
				}
			}
		}
	}
}
