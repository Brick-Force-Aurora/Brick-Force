using System;
using System.IO;
using System.Text;
using UnityEngine;

public class XpManager : MonoBehaviour
{
	public bool ExportAfterLoad;

	public Texture2D[] badges;

	public Texture2D[] stars;

	public string[] ranks;

	public string[] starRanks;

	public int[] starLevel;

	private int[] xpTable;

	public XpTableByBuild[] xpTables;

	private int maxLevelByXp;

	private static XpManager _instance;

	public int[] heavyTable;

	public int[] assaultTable;

	public int[] sniperTable;

	public int[] subMachineTable;

	public int[] handGunTable;

	public int[] meleeTable;

	public int[] specialTable;

	public int maxWeaponLevel = 3;

	public int MaxLevelByXp => maxLevelByXp;

	public static XpManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (UnityEngine.Object.FindObjectOfType(typeof(XpManager)) as XpManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the XpManager Instance");
				}
			}
			return _instance;
		}
	}

	public int MaxWeaponLevel => maxWeaponLevel + 2;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
		xpTable = xpTables[(int)BuildOption.Instance.Props.xpMode].Table;
		if (xpTable.Length != badges.Length)
		{
			Debug.LogError("XpManager has different number of rank and badges");
		}
		int num = 0;
		for (int i = 0; i < xpTable.Length; i++)
		{
			if (num > xpTable[i])
			{
				Debug.LogError("Xp is lesser than previous xp " + i);
			}
			num = xpTable[i];
		}
		maxLevelByXp = xpTable.Length - 1;
	}

	private void ExportLevelName(LangOptManager.LANG_OPT language)
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for Resources");
		}
		else
		{
			string path = Path.Combine(text, "Template/levelName_insert_" + language.ToString() + ".sql");
			try
			{
				FileStream fileStream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None);
				StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
				streamWriter.WriteLine("USE [BrickForce_LOG]");
				streamWriter.WriteLine("GO");
				streamWriter.WriteLine("DELETE tblLevelName");
				int num = 1;
				for (int i = 0; i < ranks.Length; i++)
				{
					string str = "INSERT INTO tblLevelName( lv, lvName, lvGroupName ) values(";
					str += num.ToString();
					str += ", '";
					str += StringMgr.Instance.Get(ranks[i], language).Replace("'", "''");
					str += "','')";
					streamWriter.WriteLine(str);
					num++;
				}
				for (int j = 0; j < starRanks.Length; j++)
				{
					string str2 = "INSERT INTO tblLevelName( lv, lvName, lvGroupName ) values(";
					str2 += num.ToString();
					str2 += ", '";
					str2 += StringMgr.Instance.Get(starRanks[j], language).Replace("'", "''");
					str2 += "','')";
					streamWriter.WriteLine(str2);
					num++;
				}
				streamWriter.Close();
				fileStream.Close();
			}
			catch (Exception ex)
			{
				Debug.LogError("Export Error: " + ex.Message.ToString());
			}
		}
	}

	private void Export(string postFix, int[] xps)
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for Resources");
		}
		else
		{
			string path = Path.Combine(text, "Template/xp_insert_" + postFix + ".sql");
			try
			{
				FileStream fileStream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None);
				StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.ASCII);
				streamWriter.WriteLine("USE [BrickForce]");
				streamWriter.WriteLine("GO");
				streamWriter.WriteLine("DELETE tblXp");
				for (int i = 0; i < xps.Length; i++)
				{
					string str = "INSERT INTO tblXp( lv, xp ) values(";
					str += i.ToString();
					str += ", ";
					str += xps[i].ToString();
					str += ")";
					streamWriter.WriteLine(str);
				}
				streamWriter.Close();
				fileStream.Close();
			}
			catch (Exception ex)
			{
				Debug.LogError("Export Error: " + ex.Message.ToString());
			}
		}
	}

	public int GetLevel(int xp)
	{
		for (int i = 0; i < xpTable.Length; i++)
		{
			if (xp < xpTable[i])
			{
				return i;
			}
		}
		return maxLevelByXp + 1;
	}

	public int GetLevelMixLank(int xp, int rank)
	{
		int level = GetLevel(xp);
		if (level <= maxLevelByXp)
		{
			return level;
		}
		int rankLevel = GetRankLevel(rank);
		if (rankLevel < 0)
		{
			return maxLevelByXp;
		}
		return level + rankLevel;
	}

	public float GetRatio(int xp)
	{
		int level = GetLevel(xp);
		if (level > maxLevelByXp)
		{
			return 1f;
		}
		int num = xpTable[level];
		int num2 = 0;
		if (level > 0)
		{
			num2 = xpTable[level - 1];
		}
		int num3 = num - num2;
		xp -= num2;
		return (float)xp / (float)num3;
	}

	public string GetXpCountString(int xp)
	{
		int level = GetLevel(xp);
		if (level > maxLevelByXp)
		{
			return (xp - xpTable[maxLevelByXp]).ToString();
		}
		int num = xpTable[level];
		int num2 = 0;
		if (level > 0)
		{
			num2 = xpTable[level - 1];
		}
		int num3 = num - num2;
		xp -= num2;
		return xp.ToString() + " / " + num3;
	}

	private int GetRankLevel(int rank)
	{
		if (rank > 0)
		{
			for (int num = starLevel.Length - 1; num >= 0; num--)
			{
				if (rank <= starLevel[num])
				{
					return num;
				}
			}
		}
		return -1;
	}

	public Texture2D GetBadge(int lv, int rank)
	{
		if (lv <= maxLevelByXp)
		{
			return badges[lv];
		}
		int rankLevel = GetRankLevel(rank);
		if (rankLevel < 0)
		{
			return badges[maxLevelByXp];
		}
		return stars[rankLevel];
	}

	public Texture2D GetBadge(int lvRank)
	{
		if (lvRank <= maxLevelByXp)
		{
			return badges[lvRank];
		}
		int num = lvRank - maxLevelByXp - 1;
		if (num < 0 || num >= starRanks.Length)
		{
			Debug.LogError("invalid rankLevel lvRank = " + lvRank);
			return null;
		}
		return stars[num];
	}

	public void SetStarLevel(int idx, int cnt)
	{
		if (0 <= idx && idx < starLevel.Length)
		{
			starLevel[idx] = cnt;
		}
	}

	public string GetRank(int lv, int rank)
	{
		if (lv <= maxLevelByXp)
		{
			return StringMgr.Instance.Get(ranks[lv]);
		}
		int rankLevel = GetRankLevel(rank);
		if (rankLevel < 0)
		{
			return StringMgr.Instance.Get(ranks[maxLevelByXp]);
		}
		return StringMgr.Instance.Get(starRanks[rankLevel]);
	}

	public string GetRank(int lvRank)
	{
		if (lvRank <= maxLevelByXp)
		{
			return StringMgr.Instance.Get(ranks[lvRank]);
		}
		int num = lvRank - maxLevelByXp - 1;
		if (num < 0 || num >= starRanks.Length)
		{
			Debug.LogError("invalid rankLevel lvRank = " + lvRank);
			return string.Empty;
		}
		return StringMgr.Instance.Get(starRanks[num]);
	}

	public int GetDiscountRatio(TWeapon.CATEGORY category, int xp)
	{
		int weaponLevel = GetWeaponLevel(category, xp);
		if (weaponLevel > maxWeaponLevel)
		{
			return 15;
		}
		if (weaponLevel > 2)
		{
			return 8;
		}
		if (weaponLevel > 1)
		{
			return 5;
		}
		if (weaponLevel > 0)
		{
			return 3;
		}
		return 0;
	}

	public float GetWeaponLevelRatio(TWeapon.CATEGORY category, int xp)
	{
		int weaponLevel = GetWeaponLevel(category, xp);
		if (weaponLevel > maxWeaponLevel)
		{
			return 1f;
		}
		int num = 0;
		int num2 = 0;
		switch (category)
		{
		case TWeapon.CATEGORY.HEAVY:
			num = heavyTable[weaponLevel];
			if (weaponLevel > 0)
			{
				num2 = heavyTable[weaponLevel - 1];
			}
			break;
		case TWeapon.CATEGORY.ASSAULT:
			num = assaultTable[weaponLevel];
			if (weaponLevel > 0)
			{
				num2 = assaultTable[weaponLevel - 1];
			}
			break;
		case TWeapon.CATEGORY.SNIPER:
			num = sniperTable[weaponLevel];
			if (weaponLevel > 0)
			{
				num2 = sniperTable[weaponLevel - 1];
			}
			break;
		case TWeapon.CATEGORY.SUB_MACHINE:
			num = subMachineTable[weaponLevel];
			if (weaponLevel > 0)
			{
				num2 = subMachineTable[weaponLevel - 1];
			}
			break;
		case TWeapon.CATEGORY.HAND_GUN:
			num = handGunTable[weaponLevel];
			if (weaponLevel > 0)
			{
				num2 = handGunTable[weaponLevel - 1];
			}
			break;
		case TWeapon.CATEGORY.MELEE:
			num = meleeTable[weaponLevel];
			if (weaponLevel > 0)
			{
				num2 = meleeTable[weaponLevel - 1];
			}
			break;
		case TWeapon.CATEGORY.SPECIAL:
			num = specialTable[weaponLevel];
			if (weaponLevel > 0)
			{
				num2 = specialTable[weaponLevel - 1];
			}
			break;
		}
		int num3 = num - num2;
		xp -= num2;
		if (num3 == 0)
		{
			return 0f;
		}
		return (float)xp / (float)num3;
	}

	public int GetWeaponLevel4Player(TWeapon.CATEGORY category, int xp)
	{
		return GetWeaponLevel(category, xp) + 1;
	}

	public int GetWeaponLevel(TWeapon.CATEGORY category, int xp)
	{
		switch (category)
		{
		case TWeapon.CATEGORY.HEAVY:
			for (int n = 0; n < heavyTable.Length; n++)
			{
				if (xp < heavyTable[n])
				{
					return n;
				}
			}
			break;
		case TWeapon.CATEGORY.ASSAULT:
			for (int j = 0; j < assaultTable.Length; j++)
			{
				if (xp < assaultTable[j])
				{
					return j;
				}
			}
			break;
		case TWeapon.CATEGORY.SNIPER:
			for (int l = 0; l < sniperTable.Length; l++)
			{
				if (xp < sniperTable[l])
				{
					return l;
				}
			}
			break;
		case TWeapon.CATEGORY.SUB_MACHINE:
			for (int num = 0; num < subMachineTable.Length; num++)
			{
				if (xp < subMachineTable[num])
				{
					return num;
				}
			}
			break;
		case TWeapon.CATEGORY.HAND_GUN:
			for (int m = 0; m < handGunTable.Length; m++)
			{
				if (xp < handGunTable[m])
				{
					return m;
				}
			}
			break;
		case TWeapon.CATEGORY.MELEE:
			for (int k = 0; k < meleeTable.Length; k++)
			{
				if (xp < meleeTable[k])
				{
					return k;
				}
			}
			break;
		case TWeapon.CATEGORY.SPECIAL:
			for (int i = 0; i < specialTable.Length; i++)
			{
				if (xp < specialTable[i])
				{
					return i;
				}
			}
			break;
		}
		return maxWeaponLevel + 1;
	}

	public void SetXpTable(int[] table)
	{
		xpTable = table;
	}
}
