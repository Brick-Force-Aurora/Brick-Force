using System.Collections.Generic;
using UnityEngine;

public class ZombieVsHumanManager : MonoBehaviour
{
	private static ZombieVsHumanManager _instance;

	private List<int> zombies = new List<int>();

	private List<int> humans = new List<int>();

	private List<int> dead = new List<int>();

	private bool respawnable;

	public static ZombieVsHumanManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(ZombieVsHumanManager)) as ZombieVsHumanManager);
			}
			return _instance;
		}
	}

	public bool AmIRespawnable
	{
		get
		{
			return respawnable;
		}
		set
		{
			respawnable = value;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	public void SetupLocalDeath(bool headshot)
	{
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.ZOMBIE && BuildOption.Instance.Props.zombieMode)
		{
			if (IsZombie(MyInfoManager.Instance.Seq) && !headshot)
			{
				respawnable = true;
			}
			else
			{
				respawnable = false;
			}
		}
	}

	public void ResetGameStuff()
	{
		zombies.Clear();
		humans.Clear();
		dead.Clear();
		respawnable = false;
	}

	public void AddZombie(int zombie)
	{
		DelHuman(zombie);
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.ZOMBIE && BuildOption.Instance.Props.zombieMode)
		{
			int num = zombies.IndexOf(zombie);
			if (num < 0)
			{
				BrickManDesc desc = BrickManManager.Instance.GetDesc(zombie);
				if (zombie == MyInfoManager.Instance.Seq || desc != null)
				{
					zombies.Add(zombie);
				}
			}
		}
	}

	private void DelZombie(int zombie)
	{
		int num = zombies.IndexOf(zombie);
		if (0 <= num)
		{
			zombies.RemoveAt(num);
		}
	}

	public void Die(int seq)
	{
		DelHuman(seq);
		DelZombie(seq);
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.ZOMBIE && BuildOption.Instance.Props.zombieMode)
		{
			int num = dead.IndexOf(seq);
			if (num < 0)
			{
				BrickManDesc desc = BrickManManager.Instance.GetDesc(seq);
				if (seq == MyInfoManager.Instance.Seq || desc != null)
				{
					dead.Add(seq);
				}
			}
		}
	}

	public void AddHuman(int human)
	{
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.ZOMBIE && BuildOption.Instance.Props.zombieMode)
		{
			int num = humans.IndexOf(human);
			if (num < 0)
			{
				BrickManDesc desc = BrickManManager.Instance.GetDesc(human);
				if (human == MyInfoManager.Instance.Seq || desc != null)
				{
					humans.Add(human);
				}
			}
		}
	}

	private void DelHuman(int human)
	{
		int num = humans.IndexOf(human);
		if (0 <= num)
		{
			humans.RemoveAt(num);
		}
	}

	public bool IsZombie(int seq)
	{
		if (zombies.Count == 0)
		{
			return false;
		}
		int num = zombies.IndexOf(seq);
		return num >= 0;
	}

	public bool IsHuman(int seq)
	{
		int num = humans.IndexOf(seq);
		return num >= 0;
	}

	public float GetZombieRatio(int max)
	{
		float num = 0f;
		if (max > 0)
		{
			num = (float)zombies.Count / (float)max;
			if (num > 1f)
			{
				num = 1f;
			}
		}
		return num;
	}

	public float GetHumanRatio(int max)
	{
		float num = 0f;
		if (max > 0)
		{
			num = (float)humans.Count / (float)max;
			if (num > 1f)
			{
				num = 1f;
			}
		}
		return num;
	}

	public int GetZombieCount()
	{
		return zombies.Count;
	}

	public int GetHumanCount()
	{
		return humans.Count;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
