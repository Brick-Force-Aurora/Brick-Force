using System.Collections.Generic;
using UnityEngine;

public class MonDesc
{
	public bool bP2P;

	public float timerRebirth;

	public int atkedSeq = -1;

	public float moveSpeed;

	public float shootDelay;

	public int typeID = -1;

	public int tblID = -1;

	public Dictionary<int, int> dicDamageLog;

	public Dictionary<int, int> dicInflictedDamage;

	public float rigidity;

	public bool bRedTeam = true;

	public int Dp;

	private int seq;

	public bool bHalfDamage;

	public int max_xp;

	private int xp;

	private int aiAtkWho;

	private float deltaTimeInflictedDamage;

	public bool IsHit;

	public bool colHit;

	public int coreToDmg;

	public int Seq => seq;

	public int Xp
	{
		get
		{
			return xp;
		}
		set
		{
			xp = value;
		}
	}

	public int AiAtkWho
	{
		get
		{
			return aiAtkWho;
		}
		set
		{
			aiAtkWho = value;
		}
	}

	public MonDesc(int _tbl, int _typeID, int _seq, int _xp, bool _bP2P, int _dp)
	{
		tblID = _tbl;
		typeID = _typeID;
		seq = _seq;
		xp = _xp;
		max_xp = _xp;
		bP2P = _bP2P;
		Dp = _dp;
		bRedTeam = ((seq % 2 == 0) ? true : false);
	}

	public bool isSmoke()
	{
		float num = (float)xp / (float)max_xp;
		if (num > 0.3f)
		{
			return false;
		}
		return true;
	}

	public void ResetGameStuff()
	{
	}

	public bool IsHostile()
	{
		return true;
	}

	public void InitLog()
	{
		if (dicDamageLog == null)
		{
			dicDamageLog = new Dictionary<int, int>();
		}
		if (dicInflictedDamage == null)
		{
			dicInflictedDamage = new Dictionary<int, int>();
		}
	}

	public void LogAttacker(int shooter, int damage)
	{
		if (dicDamageLog != null && damage > 0)
		{
			if (dicDamageLog.ContainsKey(shooter))
			{
				Dictionary<int, int> dictionary;
				Dictionary<int, int> dictionary2 = dictionary = dicDamageLog;
				int key;
				int key2 = key = shooter;
				key = dictionary[key];
				dictionary2[key2] = key + damage;
			}
			else
			{
				dicDamageLog.Add(shooter, damage);
			}
			if (dicInflictedDamage != null && damage > 0)
			{
				if (dicInflictedDamage.ContainsKey(shooter))
				{
					Dictionary<int, int> dictionary3;
					Dictionary<int, int> dictionary4 = dictionary3 = dicInflictedDamage;
					int key;
					int key3 = key = shooter;
					key = dictionary3[key];
					dictionary4[key3] = key + damage;
				}
				else
				{
					dicInflictedDamage.Add(shooter, damage);
				}
			}
		}
	}

	public void ReportInflictedDamage()
	{
		deltaTimeInflictedDamage += Time.deltaTime;
		if (deltaTimeInflictedDamage > 5f)
		{
			deltaTimeInflictedDamage = 0f;
			if (dicInflictedDamage != null && dicInflictedDamage.Count > 0)
			{
				CSNetManager.Instance.Sock.SendCS_INFLICTED_DAMAGE_REQ(dicInflictedDamage);
				dicInflictedDamage.Clear();
			}
		}
	}

	public void clearLog()
	{
		dicDamageLog.Clear();
		dicInflictedDamage.Clear();
	}
}
