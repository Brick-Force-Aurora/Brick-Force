using System;
using System.IO;
using UnityEngine;

public class UserMapInfo
{
	private byte slot;

	private string alias;

	private int brickCount;

	private DateTime lastModified;

	private sbyte premium;

	private Texture2D thumbnail;

	public Texture2D Thumbnail
	{
		get
		{
			if (null == thumbnail && alias.Length > 0 && lastModified.Year > 1971)
			{
				ThumbnailDownloader.Instance.Enqueue(isUserMap: true, slot);
			}
			return thumbnail;
		}
		set
		{
			thumbnail = value;
		}
	}

	public byte Slot => slot;

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

	public int BrickCount
	{
		get
		{
			return brickCount;
		}
		set
		{
			brickCount = value;
		}
	}

	public DateTime LastModified
	{
		get
		{
			return lastModified;
		}
		set
		{
			lastModified = value;
		}
	}

	public bool IsPremium => premium != 0;

	public sbyte Premium
	{
		get
		{
			return premium;
		}
		set
		{
			premium = value;
		}
	}

	public UserMapInfo(byte _slot, sbyte _premium)
	{
		slot = _slot;
		alias = string.Empty;
		thumbnail = null;
		premium = _premium;
	}

	public UserMapInfo(byte _slot, string _alias, int _brickCount, DateTime _lastModified, sbyte _premium)
	{
		slot = _slot;
		alias = _alias;
		brickCount = _brickCount;
		lastModified = _lastModified;
		thumbnail = null;
		premium = _premium;
	}

	public void VerifySavedData()
	{
		if (alias.Length > 0 && thumbnail == null && lastModified.Year <= 1971)
		{
			alias = string.Empty;
			brickCount = 0;
		}
	}

	public bool LoadCache()
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
		string text2 = Path.Combine(text, "downloaded" + slot + ".umi.cache");
		if (!File.Exists(text2))
		{
			return false;
		}
		return Load(text2);
	}

	public bool SaveCache()
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
		return Save(Path.Combine(text, "downloaded" + slot + ".umi.cache"));
	}

	private bool Save(string fileName)
	{
		try
		{
			FileStream output = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Write);
			BinaryWriter binaryWriter = new BinaryWriter(output);
			binaryWriter.Write(slot);
			binaryWriter.Write(alias);
			binaryWriter.Write(brickCount);
			binaryWriter.Write(lastModified.Year);
			binaryWriter.Write((sbyte)lastModified.Month);
			binaryWriter.Write((sbyte)lastModified.Day);
			binaryWriter.Write((sbyte)lastModified.Hour);
			binaryWriter.Write((sbyte)lastModified.Minute);
			binaryWriter.Write((sbyte)lastModified.Second);
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
			IL_0114:;
		}
		return true;
	}

	private bool Load(string fileName)
	{
		try
		{
			FileStream input = File.Open(fileName, FileMode.Open, FileAccess.Read);
			BinaryReader binaryReader = new BinaryReader(input);
			slot = binaryReader.ReadByte();
			alias = binaryReader.ReadString();
			brickCount = binaryReader.ReadInt32();
			int year = binaryReader.ReadInt32();
			sbyte b = binaryReader.ReadSByte();
			sbyte b2 = binaryReader.ReadSByte();
			sbyte b3 = binaryReader.ReadSByte();
			sbyte b4 = binaryReader.ReadSByte();
			sbyte b5 = binaryReader.ReadSByte();
			lastModified = new DateTime(year, b, b2, b3, b4, b5);
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
			IL_0119:;
		}
		return true;
	}
}
