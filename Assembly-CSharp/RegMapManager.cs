using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RegMapManager : MonoBehaviour
{
	public Dictionary<int, RegMap> dicRegMap;

	private Dictionary<int, RegMap> dicDownloaded;

	private Dictionary<int, RegMap> dicDeleted;

	private static RegMapManager _instance;

	private Dictionary<int, float> dicRegMapReq = new Dictionary<int, float>();

	public static RegMapManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (UnityEngine.Object.FindObjectOfType(typeof(RegMapManager)) as RegMapManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the RegMapManager Instance");
				}
			}
			return _instance;
		}
	}

	public RegMap[] ToArray(int roomType)
	{
		List<RegMap> list = new List<RegMap>();
		foreach (KeyValuePair<int, RegMap> item in dicDownloaded)
		{
			if (item.Value.IsPlayableAt(roomType))
			{
				list.Add(item.Value);
			}
		}
		return list.ToArray();
	}

	public RegMap[] ToArray(int roomType, int page)
	{
		int num = (page - 1) * 12;
		int num2 = num + 12;
		int num3 = 0;
		List<RegMap> list = new List<RegMap>();
		foreach (KeyValuePair<int, RegMap> item in dicDownloaded)
		{
			if (num3 >= num2)
			{
				break;
			}
			if (item.Value.IsPlayableAt(roomType))
			{
				if (num3 >= num && num3 < num2)
				{
					list.Add(item.Value);
				}
				num3++;
			}
		}
		return list.ToArray();
	}

	public RegMap[] ToArray(int roomType, Channel.MODE channelMode)
	{
		List<RegMap> list = new List<RegMap>();
		foreach (KeyValuePair<int, RegMap> item in dicDownloaded)
		{
			if (item.Value.IsPlayableAt(roomType, channelMode))
			{
				list.Add(item.Value);
			}
		}
		return list.ToArray();
	}

	private void Awake()
	{
		dicRegMap = new Dictionary<int, RegMap>();
		dicDownloaded = new Dictionary<int, RegMap>();
		dicDeleted = new Dictionary<int, RegMap>();
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void OnApplicationQuit()
	{
		_instance = null;
	}

	public bool IsDownloaded(int map)
	{
		return dicDownloaded.ContainsKey(map);
	}

	public bool IsDeleted(int map)
	{
		return dicDeleted.ContainsKey(map);
	}

	public void Clear()
	{
		dicRegMap.Clear();
		dicDownloaded.Clear();
		dicDeleted.Clear();
	}

	public void DownloadedClear()
	{
		dicDownloaded.Clear();
		dicDeleted.Clear();
	}

	public void LogDeletedMap(int map)
	{
		if (!dicDeleted.ContainsKey(map))
		{
			RegMap value = null;
			if (dicRegMap.ContainsKey(map))
			{
				value = dicRegMap[map];
			}
			else if (dicDownloaded.ContainsKey(map))
			{
				value = dicDownloaded[map];
			}
			dicDeleted.Add(map, value);
		}
	}

	public void SetDownloadFirst(int map)
	{
		if (dicRegMap.ContainsKey(map) && !dicDownloaded.ContainsKey(map))
		{
			Dictionary<int, RegMap> dictionary = new Dictionary<int, RegMap>(dicDownloaded);
			dicDownloaded.Clear();
			dicDownloaded.Add(map, dicRegMap[map]);
			foreach (KeyValuePair<int, RegMap> item in dictionary)
			{
				dicDownloaded.Add(item.Key, item.Value);
			}
		}
	}

	public void SetDownload(int map, bool download)
	{
		if (download)
		{
			if (dicRegMap.ContainsKey(map) && !dicDownloaded.ContainsKey(map))
			{
				dicDownloaded.Add(map, dicRegMap[map]);
			}
		}
		else if (dicDownloaded.ContainsKey(map))
		{
			dicDownloaded.Remove(map);
		}
	}

	public RegMap GetAlways(int map, string developer, string alias, DateTime regDate, ushort modeMask, bool clanMatchable, bool officialMap, int likes, int dislikes, int downloadCount, int downloadFee, int release, int latestRelease, byte tagMask, bool blocked)
	{
		if (dicRegMapReq.ContainsKey(map))
		{
			dicRegMapReq.Remove(map);
		}
		if (dicRegMap.ContainsKey(map))
		{
			dicRegMap[map].Likes = likes;
			dicRegMap[map].DisLikes = dislikes;
			dicRegMap[map].DownloadCount = downloadCount;
			dicRegMap[map].DownloadFee = downloadFee;
			dicRegMap[map].ClanMatchable = clanMatchable;
			dicRegMap[map].OfficialMap = officialMap;
			dicRegMap[map].Blocked = blocked;
			dicRegMap[map].ModeMask = modeMask;
			dicRegMap[map].Release = release;
			dicRegMap[map].LatestRelease = latestRelease;
			dicRegMap[map].tagMask = tagMask;
			dicRegMap[map].Developer = developer;
			return dicRegMap[map];
		}
		RegMap regMap = new RegMap(map, developer, alias, regDate, modeMask, clanMatchable, officialMap, likes, dislikes, downloadCount, downloadFee, release, latestRelease, tagMask, blocked);
		dicRegMap.Add(map, regMap);
		return regMap;
	}

	public void SetThumbnail(int map, Texture2D thumbnail)
	{
		if (dicRegMap.ContainsKey(map))
		{
			dicRegMap[map].Thumbnail = thumbnail;
		}
	}

	private void RequestRegMap(int map)
	{
		if (!dicRegMapReq.ContainsKey(map))
		{
			CSNetManager.Instance.Sock.SendCS_REG_MAP_INFO_REQ(map);
			dicRegMapReq.Add(map, Time.time);
		}
		else if (Time.time - dicRegMapReq[map] > 10f)
		{
			CSNetManager.Instance.Sock.SendCS_REG_MAP_INFO_REQ(map);
			dicRegMapReq[map] = Time.time;
		}
	}

	public RegMap Get(int map)
	{
		if (!dicRegMap.ContainsKey(map))
		{
			RequestRegMap(map);
			return null;
		}
		return dicRegMap[map];
	}

	public void Add(RegMap regMap)
	{
		if (!dicRegMap.ContainsKey(regMap.Map))
		{
			dicRegMap.Add(regMap.Map, regMap);
		}
	}

    public void UpdateMap(RegMap regMap)
    {
        if (dicRegMap.ContainsKey(regMap.Map))
        {
            dicRegMap[regMap.Map] = regMap;
			Debug.LogError("Map updated");
        }
    }

    public bool Del(int map)
	{
		if (!dicRegMap.ContainsKey(map))
		{
			return false;
		}
		return dicRegMap.Remove(map);
	}

	private void Start()
	{
		//This loads all regmap files
		string path = Path.Combine(Application.dataPath, "Resources/Cache");
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		string[] files = Directory.GetFiles(path, "*.regmap", SearchOption.AllDirectories);
		for (int i = 0; i < files.Length; i++)
		{
			RegMap regMap = new RegMap();
			if (regMap.Load(files[i]))
			{
				Add(regMap);
			}
		}
	}

	private void Update()
	{
	}
}
