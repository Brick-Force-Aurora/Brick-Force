using System;
using System.IO;
using UnityEngine;

public class RegMap
{
	private const byte TAG_ABUSE = 16;

	private int latestFileVer = 3;

	private int ver = 3;

	private int map = -1;

	private string developer;

	private string alias;

	private DateTime regDate;

	private ushort modeMask;

	private int release;

	private int latestRelease;

	public int Rank;

	public int RankChg;

	public byte tagMask;

	private Texture2D thumbnail;

	private bool clanMatchable;

	private bool officialMap;

	private bool blocked;

	private int likes;

	private int disLikes;

	private int downloadCount;

	private int downloadFee;

	public int Map => map;

	public string Developer
	{
		get
		{
			return developer;
		}
		set
		{
			developer = value;
		}
	}

	public string Alias
	{
		get
		{
			return alias;
		}
		set
		{
			alias = value;
		}
	}

	public string Version => "(" + release.ToString() + "/" + latestRelease.ToString() + ")";

	public DateTime RegisteredDate => regDate;

	public ushort ModeMask
	{
		get
		{
			return modeMask;
		}
		set
		{
			modeMask = value;
		}
	}

	public int Release
	{
		get
		{
			return release;
		}
		set
		{
			release = value;
		}
	}

	public int LatestRelease
	{
		get
		{
			return latestRelease;
		}
		set
		{
			if (value > latestRelease)
			{
				latestRelease = value;
			}
		}
	}

	public Texture2D Thumbnail
	{
		get
		{
			if (null == thumbnail)
			{
				ThumbnailDownloader.Instance.Enqueue(isUserMap: false, map);
			}
			return thumbnail;
		}
		set
		{
			bool flag = false;
			if (thumbnail == null && value != null)
			{
				flag = true;
			}
			thumbnail = value;
			if (flag)
			{
				Save();
			}
		}
	}

	public bool ClanMatchable
	{
		set
		{
			clanMatchable = value;
		}
	}

	public bool OfficialMap
	{
		get
		{
			return officialMap;
		}
		set
		{
			officialMap = value;
		}
	}

	public bool Blocked
	{
		get
		{
			return blocked;
		}
		set
		{
			blocked = value;
		}
	}

	public int Likes
	{
		get
		{
			return likes;
		}
		set
		{
			likes = value;
		}
	}

	public int DisLikes
	{
		get
		{
			return disLikes;
		}
		set
		{
			disLikes = value;
		}
	}

	public int DownloadCount
	{
		get
		{
			return downloadCount;
		}
		set
		{
			downloadCount = value;
		}
	}

	public int DownloadFee
	{
		get
		{
			return downloadFee;
		}
		set
		{
			downloadFee = value;
		}
	}

	public bool IsLatest => release == latestRelease;

	public RegMap()
	{
		map = -1;
		developer = string.Empty;
		alias = string.Empty;
		thumbnail = null;
		clanMatchable = false;
		officialMap = false;
		likes = 0;
		disLikes = 0;
		downloadCount = 0;
		downloadFee = 0;
		release = 0;
		latestRelease = 0;
		tagMask = 0;
	}

	public RegMap(int _map, string _developer, string _alias, DateTime _regDate, ushort _modeMask, bool _clanMatchable, bool _officialMap, int _likes, int _disLikes, int _downloadCount, int _downloadFee, int _release, int _latestRelease, byte _tagmask, bool _blocked)
	{
		map = _map;
		developer = _developer;
		alias = _alias;
		regDate = _regDate;
		modeMask = _modeMask;
		clanMatchable = _clanMatchable;
		officialMap = _officialMap;
		thumbnail = null;
		likes = _likes;
		disLikes = _disLikes;
		downloadCount = _downloadCount;
		downloadFee = _downloadFee;
		release = _release;
		latestRelease = _latestRelease;
		tagMask = _tagmask;
		blocked = _blocked;
	}

	public bool IsPlayableAt(int roomType)
	{
		if (roomType >= Room.modeSelector.Length)
		{
			return false;
		}
		return (modeMask & GlobalVars.Instance.getBattleMode(roomType)) != 0;
	}

	public bool IsPlayableAt(int roomType, Channel.MODE channelMode)
	{
		if (roomType >= Room.modeSelector.Length)
		{
			return false;
		}
		bool flag = (modeMask & GlobalVars.Instance.getBattleMode(roomType)) != 0;
		if (flag)
		{
			switch (channelMode)
			{
			case Channel.MODE.CLAN:
				flag = clanMatchable;
				break;
			case Channel.MODE.NEWBIE:
				flag = officialMap;
				break;
			case Channel.MODE.MAPEDIT:
				flag = false;
				break;
			}
		}
		return flag;
	}

	public string GetStarAvgString()
	{
		return ((float)likes / 20f).ToString("0.#");
	}

	public void setRank(int rk, int rkchg)
	{
		Rank = rk;
		RankChg = rkchg;
	}

	private bool Save(string fileName)
	{
		try
		{
			FileStream output = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Write);
			BinaryWriter binaryWriter = new BinaryWriter(output);
			binaryWriter.Write(ver);
			binaryWriter.Write(map);
			binaryWriter.Write(alias);
			binaryWriter.Write(developer);
			binaryWriter.Write(regDate.Year);
			binaryWriter.Write((sbyte)regDate.Month);
			binaryWriter.Write((sbyte)regDate.Day);
			binaryWriter.Write((sbyte)regDate.Hour);
			binaryWriter.Write((sbyte)regDate.Minute);
			binaryWriter.Write((sbyte)regDate.Second);
			binaryWriter.Write(modeMask);
			binaryWriter.Write(clanMatchable);
			binaryWriter.Write(officialMap);
			if (null == thumbnail)
			{
				binaryWriter.Write(0);
			}
			else
			{
				byte[] array = thumbnail.EncodeToPNG();
				binaryWriter.Write(array.Length);
				for (int i = 0; i < array.Length; i++)
				{
					binaryWriter.Write(array[i]);
				}
			}
			binaryWriter.Close();
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message.ToString());
			return false;
			IL_0144:;
		}
		return true;
	}

	public bool Save()
	{
		string text = Path.Combine(Application.dataPath, "Resources/Cache");
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for Resources");
			return false;
		}
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for cache");
			return false;
		}
		return Save(Path.Combine(text, "downloaded" + map + ".regmap"));
	}

	public bool Load(int map)
	{
		string text = Path.Combine(Application.dataPath, "Resources/Cache");
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for cache");
			return false;
		}
		string text2 = Path.Combine(text, "downloaded" + map + ".regmap");
		if (!File.Exists(text2))
		{
			return false;
		}
		return Load(text2);
	}

	public bool Load(string fileName)
	{
		try
		{
			FileStream input = File.Open(fileName, FileMode.Open, FileAccess.Read);
			BinaryReader binaryReader = new BinaryReader(input);
			ver = binaryReader.ReadInt32();
			map = binaryReader.ReadInt32();
			alias = binaryReader.ReadString();
			developer = binaryReader.ReadString();
			int year = binaryReader.ReadInt32();
			sbyte b = binaryReader.ReadSByte();
			sbyte b2 = binaryReader.ReadSByte();
			sbyte b3 = binaryReader.ReadSByte();
			sbyte b4 = binaryReader.ReadSByte();
			sbyte b5 = binaryReader.ReadSByte();
			regDate = new DateTime(year, b, b2, b3, b4, b5);
			modeMask = ((ver > 2) ? binaryReader.ReadUInt16() : binaryReader.ReadByte());
			clanMatchable = binaryReader.ReadBoolean();
			officialMap = (ver >= 2 && binaryReader.ReadBoolean());
			int num = binaryReader.ReadInt32();
			if (num <= 0)
			{
				thumbnail = null;
			}
			else
			{
				byte[] array = new byte[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = binaryReader.ReadByte();
				}
				thumbnail = new Texture2D(128, 128, TextureFormat.RGB24, mipmap: false);
				thumbnail.LoadImage(array);
				thumbnail.Apply();
			}
			binaryReader.Close();
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message.ToString());
			return false;
			IL_0172:;
		}
		if (ver < latestFileVer)
		{
			ver = latestFileVer;
			Save();
		}
		return true;
	}

	public bool IsAbuseMap()
	{
		return (tagMask & 0x10) != 0;
	}
}
