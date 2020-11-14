using System.Collections.Generic;
using UnityEngine;

public class BrickManDesc
{
	public enum STATUS
	{
		PLAYER_WAITING,
		PLAYER_READY,
		PLAYER_LOADING,
		PLAYER_P2PING,
		PLAYER_PLAYING,
		PLAYER_SHOP,
		PLAYER_INVEN
	}

	public bool IsEditor;

	private int seq;

	private string nick;

	private string[] equipment;

	private int hp;

	private int maxHp;

	private int armor;

	private int maxArmor;

	private int xp;

	private int rank;

	private sbyte slot;

	private int status;

	private float initTime;

	private bool isSpectator;

	private bool isInvisibilityOn;

	private int kill;

	private int death;

	private int assist;

	private int mission;

	private int score;

	private int clan = -1;

	private string clanName = string.Empty;

	private int clanMark = -1;

	private int avgPingTime = -1;

	private string[] wpnChg;

	private string[] drpItm;

	private float lastNotified;

	private bool isGM;

	public int hackShoot;

	public SpeedHackProtector spdhackProtector;

	public bool nextmov;

	public int accumDamaged;

	public bool NeedP2ping => status == 2 || status == 3 || status == 4;

	public int Seq => seq;

	public string Nickname
	{
		get
		{
			return nick;
		}
		set
		{
			nick = value;
		}
	}

	public string[] Equipment => equipment;

	public int Hp
	{
		get
		{
			return hp;
		}
		set
		{
			hp = value;
		}
	}

	public int MaxHp
	{
		get
		{
			return maxHp;
		}
		set
		{
			maxHp = value;
		}
	}

	public float HpRatio => (maxHp > 0) ? ((float)hp / (float)maxHp) : 0f;

	public int Armor
	{
		get
		{
			return armor;
		}
		set
		{
			armor = value;
		}
	}

	public int MaxArmor
	{
		get
		{
			return maxArmor;
		}
		set
		{
			maxArmor = value;
		}
	}

	public float ArmorRatio => (maxArmor > 0) ? ((float)armor / (float)maxArmor) : 0f;

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

	public int Rank
	{
		get
		{
			return rank;
		}
		set
		{
			rank = value;
		}
	}

	public sbyte Slot
	{
		get
		{
			return slot;
		}
		set
		{
			slot = value;
		}
	}

	public int Status
	{
		get
		{
			return status;
		}
		set
		{
			status = value;
			if (status == 2)
			{
				initTime = Time.time;
			}
		}
	}

	public bool IsTooLong4Init => (status == 2 || status == 3) && Time.time - initTime > 20f;

	public bool IsSpectator
	{
		get
		{
			return isSpectator;
		}
		set
		{
			isSpectator = value;
		}
	}

	public bool IsHidePlayer => isSpectator || isInvisibilityOn;

	public bool IsInvisibilityOn
	{
		get
		{
			return isInvisibilityOn;
		}
		set
		{
			isInvisibilityOn = value;
		}
	}

	public int Kill
	{
		get
		{
			return kill;
		}
		set
		{
			kill = value;
		}
	}

	public int Death
	{
		get
		{
			return death;
		}
		set
		{
			death = value;
		}
	}

	public int Assist
	{
		get
		{
			return assist;
		}
		set
		{
			assist = value;
		}
	}

	public int Mission
	{
		get
		{
			return mission;
		}
		set
		{
			mission = value;
		}
	}

	public int Score
	{
		get
		{
			return score;
		}
		set
		{
			score = value;
		}
	}

	public int Clan
	{
		get
		{
			return clan;
		}
		set
		{
			clan = value;
		}
	}

	public string ClanName
	{
		get
		{
			return clanName;
		}
		set
		{
			clanName = ClanName;
		}
	}

	public int ClanMark
	{
		get
		{
			return clanMark;
		}
		set
		{
			clanMark = value;
		}
	}

	public int AvgPingTime
	{
		get
		{
			return avgPingTime;
		}
		set
		{
			avgPingTime = value;
		}
	}

	public string[] WpnChg => wpnChg;

	public string[] DrpItm => drpItm;

	public float LastNotified
	{
		get
		{
			return lastNotified;
		}
		set
		{
			lastNotified = value;
		}
	}

	public bool IsGM
	{
		get
		{
			return isGM;
		}
		set
		{
			isGM = value;
		}
	}

	public BrickManDesc(int _seq, string _nick, string[] _equipment, int _status, int _xp, int _clan, string _clanName, int _clanMark, int _rank, string[] _wpnChg, string[] _drpItm)
	{
		seq = _seq;
		nick = _nick;
		equipment = _equipment;
		status = _status;
		xp = _xp;
		slot = -1;
		clan = _clan;
		clanName = _clanName;
		clanMark = _clanMark;
		rank = _rank;
		avgPingTime = -1;
		accumDamaged = 0;
		lastNotified = 0f;
		nextmov = false;
		wpnChg = new string[4]
		{
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty
		};
		int num = 0;
		while (_wpnChg != null && num < _wpnChg.Length)
		{
			ChangeWeapon(_wpnChg[num]);
			num++;
		}
		drpItm = new string[4]
		{
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty
		};
		int num2 = 0;
		while (_drpItm != null && num2 < _drpItm.Length)
		{
			ChangeDropWeapon(_drpItm[num2]);
			num2++;
		}
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.EXPLOSION && RoomManager.Instance.CurrentRoomStatus == Room.ROOM_STATUS.PLAYING && MyInfoManager.Instance.Seq != seq)
		{
			isSpectator = true;
		}
	}

	public bool ChangeWeapon(string nextCode)
	{
		TWeapon tWeapon = TItemManager.Instance.Get<TWeapon>(nextCode);
		if (tWeapon == null)
		{
			return false;
		}
		int[] array = new int[6]
		{
			-1,
			-1,
			2,
			1,
			0,
			3
		};
		int num = (int)tWeapon.slot;
		if (num < 0 || num >= array.Length)
		{
			return false;
		}
		int num2 = array[num];
		if (num2 < 0 || num2 >= wpnChg.Length)
		{
			return false;
		}
		wpnChg[num2] = nextCode;
		return true;
	}

	public bool ChangeDropWeapon(string nextCode)
	{
		TWeapon tWeapon = TItemManager.Instance.Get<TWeapon>(nextCode);
		if (tWeapon == null)
		{
			return false;
		}
		int[] array = new int[6]
		{
			-1,
			-1,
			2,
			1,
			0,
			3
		};
		int num = (int)tWeapon.slot;
		if (num < 0 || num >= array.Length)
		{
			return false;
		}
		int num2 = array[num];
		if (num2 < 0 || num2 >= drpItm.Length)
		{
			return false;
		}
		drpItm[num2] = nextCode;
		return true;
	}

	public void ResetGameStuff()
	{
		kill = 0;
		death = 0;
		assist = 0;
		mission = 0;
		score = 0;
		isSpectator = false;
		avgPingTime = -1;
		lastNotified = 0f;
		int num = 0;
		while (wpnChg != null && num < wpnChg.Length)
		{
			wpnChg[num] = string.Empty;
			num++;
		}
		int num2 = 0;
		while (drpItm != null && num2 < drpItm.Length)
		{
			drpItm[num2] = string.Empty;
			num2++;
		}
	}

	public void Equip(string code)
	{
		List<string> list = new List<string>();
		list.Add(code);
		for (int i = 0; i < equipment.Length; i++)
		{
			list.Add(equipment[i]);
		}
		equipment = list.ToArray();
	}

	public void Unequip(string code)
	{
		List<string> list = new List<string>();
		for (int i = 0; i < equipment.Length; i++)
		{
			if (!(equipment[i] == code))
			{
				list.Add(equipment[i]);
			}
		}
		equipment = list.ToArray();
	}

	public bool IsHostile()
	{
		switch (RoomManager.Instance.CurrentRoomType)
		{
		case Room.ROOM_TYPE.MAP_EDITOR:
			return false;
		case Room.ROOM_TYPE.INDIVIDUAL:
		case Room.ROOM_TYPE.BUNGEE:
		case Room.ROOM_TYPE.ESCAPE:
			return true;
		case Room.ROOM_TYPE.TEAM_MATCH:
		case Room.ROOM_TYPE.CAPTURE_THE_FLAG:
		case Room.ROOM_TYPE.EXPLOSION:
		case Room.ROOM_TYPE.BND:
			return (MyInfoManager.Instance.Slot < 8 && slot >= 8) || (MyInfoManager.Instance.Slot >= 8 && slot < 8);
		case Room.ROOM_TYPE.MISSION:
			return (MyInfoManager.Instance.Slot < 4 && slot >= 4) || (MyInfoManager.Instance.Slot >= 4 && slot < 4);
		case Room.ROOM_TYPE.ZOMBIE:
			return (ZombieVsHumanManager.Instance.IsZombie(MyInfoManager.Instance.Seq) && ZombieVsHumanManager.Instance.IsHuman(seq)) || (ZombieVsHumanManager.Instance.IsHuman(MyInfoManager.Instance.Seq) && ZombieVsHumanManager.Instance.IsZombie(seq));
		default:
			return false;
		}
	}

	public int Compare(BrickManDesc desc)
	{
		if (score == desc.score)
		{
			return kill.CompareTo(desc.kill);
		}
		return -score.CompareTo(desc.score);
	}

	public int EscapeCompare(BrickManDesc desc)
	{
		if (kill == desc.kill)
		{
			if (score == desc.score)
			{
				return -Seq.CompareTo(desc.Seq);
			}
			return -score.CompareTo(desc.score);
		}
		return -kill.CompareTo(desc.kill);
	}

	public bool IsLucky()
	{
		return Random.Range(0, 100) < Luck();
	}

	private int Luck()
	{
		int num = 0;
		Item[] usingItems = MyInfoManager.Instance.GetUsingItems();
		for (int i = 0; i < usingItems.Length; i++)
		{
			TItem template = usingItems[i].Template;
			if (template != null && template.tBuff != null)
			{
				int num2 = 0;
				int num3 = 12;
				int grade = usingItems[i].upgradeProps[num3].grade;
				if (grade > 0)
				{
					num2 = (int)PimpManager.Instance.getValue((int)template.upgradeCategory, num3, grade - 1);
				}
				num += template.tBuff.Luck + num2;
			}
		}
		return num;
	}
}
