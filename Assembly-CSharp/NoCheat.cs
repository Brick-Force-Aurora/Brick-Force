using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class NoCheat : MonoBehaviour
{
	private class CWatchDog
	{
		public int val_i;

		public float val_f;

		public string val_s;
	}

	public enum WATCH_DOG
	{
		HIT_POINT,
		MAIN_MAGAZINE,
		MAIN_AMMO,
		AUX_MAGAZINE,
		AUX_AMMO,
		SPECIAL_AMMO,
		ARMOR,
		MAIN_AMMO2,
		CH,
		RUNSPEED,
		WALKSPEED,
		GODMODE,
		RESPAWMTIME,
		GHOSTMODE,
		GM,
		NUM
	}

	private float deltaTime;

	private CWatchDog[] watchDogs;

	private bool initOnce;

	private string genCode = string.Empty;

	private string[] localPath;

	private string[] remotePath;

	private int repaired;

	private static NoCheat _instance;

	private byte[] desKey;

	private float unityLapTime4Sys;

	private float unityLapTime4Svr;

	private float sysLapTime;

	private DateTime dtPrevSysTime = DateTime.Now;

	private int speedHackCnt4Sys;

	private int speedHackCnt4Svr;

	private bool checkSpeedHack;

	public string GenCode => genCode;

	public static NoCheat Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (UnityEngine.Object.FindObjectOfType(typeof(NoCheat)) as NoCheat);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the NoCheat Instance");
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
		initOnce = false;
	}

	public void Sync(WATCH_DOG watchDog, int val)
	{
		int num = (int)(CSNetManager.Instance.Sock.RecvKey + watchDog);
		int num2 = num % 15;
		int val_i = val << 1;
		watchDogs[num2].val_i = val_i;
	}

	public void Sync(WATCH_DOG watchDog, float val)
	{
		int num = (int)(CSNetManager.Instance.Sock.RecvKey + watchDog);
		int num2 = num % 15;
		int num3 = num % 15;
		float val_f = val * (float)num3;
		watchDogs[num2].val_f = val_f;
	}

	public void Sync(WATCH_DOG watchDog, string val)
	{
		int num = (int)(CSNetManager.Instance.Sock.RecvKey + watchDog);
		int num2 = num % 15;
		if (desKey == null)
		{
			setKeyDES("6789");
		}
		string val_s = Encrypt(val);
		watchDogs[num2].val_s = val_s;
	}

	public void KillCheater(WATCH_DOG watchDog, int val)
	{
		if (!GlobalVars.Instance.shutdownNow)
		{
			int num = (int)(CSNetManager.Instance.Sock.RecvKey + watchDog);
			int num2 = num % 15;
			int num3 = watchDogs[num2].val_i >> 1;
			if (num3 != val)
			{
				Debug.LogError("watchdog: " + watchDog + ", descypt: " + num3 + ", val: " + val);
				BuildOption.Instance.HardExit();
			}
		}
	}

	public void KillCheater(WATCH_DOG watchDog, float val)
	{
		if (!GlobalVars.Instance.shutdownNow)
		{
			int num = (int)(CSNetManager.Instance.Sock.RecvKey + watchDog);
			int num2 = num % 15;
			int num3 = num % 15;
			float num4 = watchDogs[num2].val_f / (float)num3;
			float num5 = num4 - val;
			if (num5 < -0.001f || num5 > 0.001f)
			{
				Debug.LogError("watchdog: " + watchDog + ", descypt: " + num4 + ", val: " + val);
				BuildOption.Instance.HardExit();
			}
		}
	}

	public void KillCheater(WATCH_DOG watchDog, string val)
	{
		if (!GlobalVars.Instance.shutdownNow)
		{
			int num = (int)(CSNetManager.Instance.Sock.RecvKey + watchDog);
			int num2 = num % 15;
			string text = Decrypt(watchDogs[num2].val_s);
			if (text != val)
			{
				Debug.LogError("watchdog: " + watchDog + ", descypt: " + text + ", val: " + val);
				BuildOption.Instance.HardExit();
			}
		}
	}

	private void Start()
	{
		watchDogs = new CWatchDog[15];
		for (int i = 0; i < 15; i++)
		{
			watchDogs[i] = new CWatchDog();
			watchDogs[i].val_i = 0;
			watchDogs[i].val_f = 0f;
			watchDogs[i].val_s = string.Empty;
		}
	}

	public static string ComputeHash(byte[] buffer, HashAlgorithm Algorithm)
	{
		byte[] value = Algorithm.ComputeHash(buffer);
		return BitConverter.ToString(value).Replace("-", string.Empty);
	}

	public static byte[] Merge(string[] filePath)
	{
		List<byte> list = new List<byte>();
		for (int i = 0; i < filePath.Length; i++)
		{
			FileStream fileStream = File.OpenRead(filePath[i]);
			byte[] array = new byte[fileStream.Length];
			fileStream.Read(array, 0, (int)fileStream.Length);
			for (int j = 0; j < array.Length; j++)
			{
				list.Add(array[j]);
			}
			fileStream.Close();
		}
		return list.ToArray();
	}

	public void AutoRepair()
	{
		repaired = 0;
		for (int i = 0; i < localPath.Length; i++)
		{
			StartCoroutine(Download(localPath[i], remotePath[i]));
		}
	}

	private IEnumerator Download(string local, string remote)
	{
		WWW www = new WWW(remote);
		yield return (object)www;
		FileStream fs = File.Open(local, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
		BinaryWriter w = new BinaryWriter(fs);
		w.Write(www.bytes, 0, www.bytes.Length);
		w.Close();
		fs.Close();
		if (++repaired >= localPath.Length)
		{
			BuildOption.Instance.HardExit();
		}
	}

	public void setKeyDES(string key)
	{
		desKey = Encoding.Unicode.GetBytes(key);
	}

	public string Encrypt(string plainText)
	{
		if (string.IsNullOrEmpty(plainText))
		{
			throw new ArgumentException("The string which needs to be encrypted can not be null.");
		}
		if (desKey.Length != 8)
		{
			throw new Exception("Invalid key. Key length must be 8 byte.");
		}
		DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
		MemoryStream memoryStream = new MemoryStream();
		CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateEncryptor(desKey, desKey), CryptoStreamMode.Write);
		StreamWriter streamWriter = new StreamWriter(cryptoStream);
		streamWriter.Write(plainText);
		streamWriter.Flush();
		cryptoStream.FlushFinalBlock();
		streamWriter.Flush();
		return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
	}

	public string Decrypt(string cypherText)
	{
		if (string.IsNullOrEmpty(cypherText))
		{
			throw new ArgumentNullException("The string which needs to be decrypted can not be null.");
		}
		if (desKey.Length != 8)
		{
			throw new Exception("Invalid key. Key length must be 8 byte.");
		}
		DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
		MemoryStream stream = new MemoryStream(Convert.FromBase64String(cypherText));
		CryptoStream stream2 = new CryptoStream(stream, dESCryptoServiceProvider.CreateDecryptor(desKey, desKey), CryptoStreamMode.Read);
		StreamReader streamReader = new StreamReader(stream2);
		return streamReader.ReadToEnd();
	}

	private void Update()
	{
		TimeSpan timeSpan = DateTime.Now - dtPrevSysTime;
		dtPrevSysTime = DateTime.Now;
		if (!initOnce)
		{
			initOnce = true;
			string str = "http://" + BuildOption.Instance.Props.GetResourceServer;
			string dataPath = Application.dataPath;
			string[] array = new string[9]
			{
				"Managed\\Assembly-CSharp.dll",
				"Resources\\Template\\character.txt.cooked",
				"Resources\\Template\\accessory.txt.cooked",
				"Resources\\Template\\costume.txt.cooked",
				"Resources\\Template\\special.txt.cooked",
				"Resources\\Template\\weapon.txt.cooked",
				"Resources\\Template\\upgradeprops.txt.cooked",
				"Resources\\Template\\upgrade.txt.cooked",
				"Resources\\Template\\buff.txt.cooked"
			};
			string[] array2 = new string[9]
			{
				"/HashProtect/Managed/Assembly-CSharp.dll.cooked",
				"/HashProtect/Resources/Template/character.txt.cooked",
				"/HashProtect/Resources/Template/accessory.txt.cooked",
				"/HashProtect/Resources/Template/costume.txt.cooked",
				"/HashProtect/Resources/Template/special.txt.cooked",
				"/HashProtect/Resources/Template/weapon.txt.cooked",
				"/HashProtect/Resources/Template/upgradeprops.txt.cooked",
				"/HashProtect/Resources/Template/upgrade.txt.cooked",
				"/HashProtect/Resources/Template/buff.txt.cooked"
			};
			localPath = new string[array.Length];
			remotePath = new string[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				localPath[i] = Path.Combine(dataPath, array[i]);
				remotePath[i] = str + array2[i];
			}
			byte[] buffer = Merge(localPath);
			genCode = ComputeHash(buffer, new MD5CryptoServiceProvider());
		}
		deltaTime += Time.deltaTime;
		if (deltaTime > 30f)
		{
			deltaTime = 0f;
			if (localPath != null)
			{
				byte[] buffer2 = Merge(localPath);
				genCode = ComputeHash(buffer2, new MD5CryptoServiceProvider());
			}
		}
		if (checkSpeedHack)
		{
			unityLapTime4Sys += Time.deltaTime;
			unityLapTime4Svr += Time.deltaTime;
			sysLapTime += (float)timeSpan.TotalMilliseconds / 1000f;
		}
		if (unityLapTime4Sys > 1f)
		{
			float num = unityLapTime4Sys * 0.3f;
			float num2 = Mathf.Abs(unityLapTime4Sys - sysLapTime);
			if (num2 > num)
			{
				speedHackCnt4Sys++;
			}
			else
			{
				speedHackCnt4Sys = 0;
			}
			if (speedHackCnt4Sys > 10 && RoomManager.Instance.CurrentRoomType != 0)
			{
				BuildOption.Instance.HardExit();
			}
			unityLapTime4Sys = 0f;
			sysLapTime = 0f;
		}
	}

	public void ResetSpeedHack()
	{
		checkSpeedHack = false;
		speedHackCnt4Sys = 0;
		speedHackCnt4Svr = 0;
		dtPrevSysTime = DateTime.Now;
		unityLapTime4Sys = 0f;
		unityLapTime4Svr = 0f;
		sysLapTime = 0f;
	}

	public void SetSpeedHack()
	{
		ResetSpeedHack();
		checkSpeedHack = true;
	}

	public void CheckServerTime(long st1, long st2)
	{
		if (checkSpeedHack)
		{
			long ticks = st2 - st1;
			float num = (float)new TimeSpan(ticks).TotalMilliseconds / 1000f;
			float num2 = num * 0.3f;
			float num3 = Mathf.Abs(unityLapTime4Svr - num);
			if (num3 > num2)
			{
				speedHackCnt4Svr++;
			}
			else
			{
				speedHackCnt4Svr = 0;
			}
			unityLapTime4Svr = 0f;
			if (speedHackCnt4Svr >= 10 && BuildOption.Instance.Props.kickSvrSpdHck && RoomManager.Instance.CurrentRoomType != 0)
			{
				BuildOption.Instance.HardExit();
			}
		}
	}

	private void OnGUI()
	{
	}

	public int HideVal(WATCH_DOG watchDog, int val)
	{
		int num = (int)(CSNetManager.Instance.Sock.RecvKey + watchDog);
		int num2 = num % 2 + 1;
		val *= CSNetManager.Instance.Sock.RecvKey;
		return val << num2;
	}

	public int UnhideVal(WATCH_DOG watchDog, int val)
	{
		int num = (int)(CSNetManager.Instance.Sock.RecvKey + watchDog);
		int num2 = num % 2 + 1;
		return (val >> num2) / CSNetManager.Instance.Sock.RecvKey;
	}
}
