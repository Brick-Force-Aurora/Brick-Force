using System;
using System.Collections.Generic;
using UnityEngine;

public class UserMapInfoManager : MonoBehaviour
{
	public int master = -1;

	private SortedList<int, UserMapInfo> listUMI;

	private Dictionary<int, int> dicRegMap;

	private List<int> cacheRegMap;

	private string curMapName = string.Empty;

	private int curSlot = int.MaxValue;

	private static UserMapInfoManager _instance;

	private float deleteRatio = 1f;

	private float generalRatio = 1f;

	private float specialRatio = 10f;

	public string CurMapName
	{
		get
		{
			return curMapName;
		}
		set
		{
			curMapName = value;
		}
	}

	public int CurSlot
	{
		get
		{
			return curSlot;
		}
		set
		{
			curSlot = value;
		}
	}

	public static UserMapInfoManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (UnityEngine.Object.FindObjectOfType(typeof(UserMapInfoManager)) as UserMapInfoManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the BrickManager Instance");
				}
			}
			return _instance;
		}
	}

	public void CalcCount(ref int generalCount, ref int specialCount, ref int deleteCount)
	{
		generalCount = 0;
		specialCount = 0;
		deleteCount = 0;
		BrickInst[] array = BrickManager.Instance.ToBrickInstArray();
		for (int i = 0; i < array.Length; i++)
		{
			Brick brick = BrickManager.Instance.GetBrick(array[i].Template);
			if (brick != null && !dicRegMap.ContainsKey(array[i].Seq))
			{
				if (brick.category == Brick.CATEGORY.ACCESSORY || brick.category == Brick.CATEGORY.FUNCTIONAL)
				{
					specialCount++;
				}
				else
				{
					generalCount++;
				}
			}
		}
		foreach (KeyValuePair<int, int> item in dicRegMap)
		{
			BrickInst brickInst = BrickManager.Instance.GetBrickInst(item.Key);
			if (brickInst == null)
			{
				deleteCount++;
			}
		}
	}

	public bool CalcFee(int generalCount, int specialCount, int deleteCount, Good.BUY_HOW regHow, ref int totalFee, ref float generalFee, ref float specialFee, ref float deleteFee, ref string pointString, ref string minMaxMessage, ref int diff)
	{
		bool flag = false;
		switch (regHow)
		{
		case Good.BUY_HOW.GENERAL_POINT:
			pointString = StringMgr.Instance.Get("GENERAL_POINT");
			generalFee = generalRatio * (float)generalCount * 1.5f;
			specialFee = specialRatio * (float)specialCount * 1.5f;
			deleteFee = deleteRatio * (float)deleteCount * 1.5f;
			totalFee = Mathf.FloorToInt(generalFee) + Mathf.FloorToInt(specialFee) + Mathf.FloorToInt(deleteFee);
			if (totalFee < 3000)
			{
				totalFee = 3000;
				minMaxMessage = string.Format(StringMgr.Instance.Get("MIN_REG_FEE_MESSAGE"), totalFee, pointString);
			}
			if (totalFee > 6000)
			{
				totalFee = 6000;
				minMaxMessage = string.Format(StringMgr.Instance.Get("MAX_REG_FEE_MESSAGE"), totalFee, pointString);
			}
			flag = (MyInfoManager.Instance.Point >= totalFee);
			if (!flag)
			{
				diff = totalFee - MyInfoManager.Instance.Point;
			}
			break;
		case Good.BUY_HOW.BRICK_POINT:
			pointString = StringMgr.Instance.Get("BRICK_POINT");
			generalFee = generalRatio * (float)generalCount;
			specialFee = specialRatio * (float)specialCount;
			deleteFee = deleteRatio * (float)deleteCount;
			totalFee = Mathf.FloorToInt(generalFee) + Mathf.FloorToInt(specialFee) + Mathf.FloorToInt(deleteFee);
			if (totalFee < 2000)
			{
				totalFee = 2000;
				minMaxMessage = string.Format(StringMgr.Instance.Get("MIN_REG_FEE_MESSAGE"), totalFee, pointString);
			}
			if (totalFee > 4000)
			{
				totalFee = 4000;
				minMaxMessage = string.Format(StringMgr.Instance.Get("MAX_REG_FEE_MESSAGE"), totalFee, pointString);
			}
			flag = (MyInfoManager.Instance.BrickPoint >= totalFee);
			if (!flag)
			{
				diff = totalFee - MyInfoManager.Instance.BrickPoint;
			}
			break;
		case Good.BUY_HOW.CASH_POINT:
			pointString = TokenManager.Instance.GetTokenString();
			generalFee = generalRatio * (float)generalCount * 0.2f;
			specialFee = specialRatio * (float)specialCount * 0.2f;
			deleteFee = deleteRatio * (float)deleteCount * 0.2f;
			totalFee = Mathf.FloorToInt(generalFee) + Mathf.FloorToInt(specialFee) + Mathf.FloorToInt(deleteFee);
			if (totalFee < 400)
			{
				totalFee = 400;
				minMaxMessage = string.Format(StringMgr.Instance.Get("MIN_REG_FEE_MESSAGE"), totalFee, pointString);
			}
			if (totalFee > 800)
			{
				totalFee = 800;
				minMaxMessage = string.Format(StringMgr.Instance.Get("MAX_REG_FEE_MESSAGE"), totalFee, pointString);
			}
			flag = (MyInfoManager.Instance.Cash >= totalFee);
			if (!flag)
			{
				diff = totalFee - MyInfoManager.Instance.Cash;
			}
			break;
		}
		return flag;
	}

	public bool CheckAuth(bool showMessage)
	{
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BUNGEE || RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BND || Instance.master == MyInfoManager.Instance.Seq || MyInfoManager.Instance.IsEditor)
		{
			return true;
		}
		if (showMessage)
		{
			SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("NO_MAPEDIT_AUTH"));
		}
		return false;
	}

	private void OnApplicationQuit()
	{
	}

	private void Awake()
	{
		listUMI = new SortedList<int, UserMapInfo>();
		dicRegMap = new Dictionary<int, int>();
		cacheRegMap = new List<int>();
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	public void VerifyCurMapName(int slot)
	{
		curSlot = slot;
		foreach (KeyValuePair<int, UserMapInfo> item in listUMI)
		{
			if (item.Key == slot)
			{
				curMapName = item.Value.Alias;
			}
		}
	}

	public void CreateBuildMode(int slot, string alias)
	{
		curSlot = slot;
		curMapName = alias;
		cacheRegMap.Clear();
		dicRegMap.Clear();
	}

	public void CacheRegMapBrick(int seq)
	{
		cacheRegMap.Add(seq);
	}

	public void CacheRegMapBrickDone(bool regMapExisting)
	{
		dicRegMap.Clear();
		if (regMapExisting)
		{
			for (int i = 0; i < cacheRegMap.Count; i++)
			{
				if (!dicRegMap.ContainsKey(cacheRegMap[i]))
				{
					dicRegMap.Add(cacheRegMap[i], cacheRegMap[i]);
				}
			}
			cacheRegMap.Clear();
		}
	}

	public void Verify()
	{
		foreach (KeyValuePair<int, UserMapInfo> item in listUMI)
		{
			if (item.Value.BrickCount == 0)
			{
				item.Value.Alias = string.Empty;
			}
		}
	}

	private void Update()
	{
	}

	public void VerifySavedData()
	{
		foreach (KeyValuePair<int, UserMapInfo> item in listUMI)
		{
			item.Value.VerifySavedData();
		}
	}

	public void Clear()
	{
		listUMI.Clear();
		curSlot = int.MaxValue;
	}

	public void SetThumbnail(int slot, Texture2D thumbnail)
	{
		foreach (KeyValuePair<int, UserMapInfo> item in listUMI)
		{
			if (item.Value.Slot == slot)
			{
				Debug.LogError("SetThumbnail");
				item.Value.Thumbnail = thumbnail;
				item.Value.SaveCache();
				break;
			}
		}
	}

	public void Remove(int key)
	{
		if (listUMI.ContainsKey(key))
		{
			listUMI.Remove(key);
		}
	}

	public void ValidateEmpty()
	{
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, UserMapInfo> item in listUMI)
		{
			if (item.Value.Alias.Length <= 0)
			{
				list.Add(item.Key);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			listUMI.Remove(list[i]);
		}
		int num = 2;
		int num2 = 3;
		if (BuildOption.Instance.IsNetmarble)
		{
			num = 0;
		}
		num2 += MyInfoManager.Instance.ExtraSlots;
		foreach (KeyValuePair<int, UserMapInfo> item2 in listUMI)
		{
			if (item2.Value.IsPremium)
			{
				num--;
			}
			else
			{
				num2--;
			}
		}
		if (num < 0)
		{
			num = 0;
		}
		if (num2 < 0)
		{
			num2 = 0;
		}
		int b = 1;
		while (num2 > 0 && b < 200)
		{
			UserMapInfo userMapInfo = Get(b);
			if (userMapInfo == null)
			{
				num2--;
				listUMI.Add(b, new UserMapInfo(b, 0));
			}
			b = (int)(b + 1);
		}
		int b2 = 1;
		while (num > 0 && b2 < 200)
		{
			UserMapInfo userMapInfo2 = Get(b2);
			if (userMapInfo2 == null)
			{
				num--;
				listUMI.Add(b2, new UserMapInfo(b2, 1));
			}
			b2 = (int)(b2 + 1);
		}
	}

	public void AddOrUpdate(int slot, string alias, int brickCount, DateTime lastModified, sbyte premium)
	{
		foreach (KeyValuePair<int, UserMapInfo> item in listUMI)
		{
			if (item.Value.Slot == slot)
			{
				item.Value.Alias = alias;
				item.Value.BrickCount = brickCount;
				item.Value.LastModified = lastModified;
				item.Value.Thumbnail = null;
				return;
			}
		}
		UserMapInfo userMapInfo = new UserMapInfo(slot, premium);
		if (userMapInfo != null)
		{
			if (!userMapInfo.LoadCache() || userMapInfo.Alias != alias || userMapInfo.BrickCount != brickCount)
			{
				userMapInfo.Alias = alias;
				userMapInfo.BrickCount = brickCount;
				userMapInfo.LastModified = lastModified;
				userMapInfo.Thumbnail = null;
			}
			Debug.LogError("Add to UMI");
			listUMI.Add(slot, userMapInfo);
		}
	}

	public UserMapInfo Get(int slot)
	{
		foreach (KeyValuePair<int, UserMapInfo> item in listUMI)
		{
			if (item.Value.Slot == slot)
			{
				return item.Value;
			}
		}
		return null;
	}

	public bool HaveUserMap()
	{
		foreach (KeyValuePair<int, UserMapInfo> item in listUMI)
		{
			if (item.Value.Alias.Length > 0)
			{
				return true;
			}
		}
		return false;
	}

	public bool HaveEmptyUserMap()
	{
		foreach (KeyValuePair<int, UserMapInfo> item in listUMI)
		{
			if (item.Value.Alias.Length <= 0)
			{
				return true;
			}
		}
		return false;
	}

	public UserMapInfo GetCur()
	{
		foreach (KeyValuePair<int, UserMapInfo> item in listUMI)
		{
			if (item.Value.Slot == curSlot)
			{
				return item.Value;
			}
		}
		return null;
	}

	public UserMapInfo[] ToArray()
	{
		UserMapInfo[] array = new UserMapInfo[listUMI.Count];
		int num = 0;
		foreach (KeyValuePair<int, UserMapInfo> item in listUMI)
		{
			array[num++] = item.Value;
		}
		return array;
	}

	public UserMapInfo[] ToArray(int page)
	{
		int num = 0;
		int num2 = (page - 1) * 12;
		int num3 = 0;
		List<UserMapInfo> list = new List<UserMapInfo>();
		foreach (KeyValuePair<int, UserMapInfo> item in listUMI)
		{
			if (num3 >= 12)
			{
				break;
			}
			if (num == num2)
			{
				list.Add(item.Value);
				num2++;
				num3++;
			}
			num++;
		}
		return list.ToArray();
	}
}
