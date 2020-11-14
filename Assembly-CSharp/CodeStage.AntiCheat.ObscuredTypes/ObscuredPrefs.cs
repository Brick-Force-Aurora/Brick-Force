using System;
using System.Text;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public static class ObscuredPrefs
	{
		public enum DeviceLockLevel : byte
		{
			None,
			Soft,
			Strict
		}

		private static string encryptionKey = "e806f6";

		private static bool savesAlterationReported;

		private static bool foreignSavesReported;

		private static string deviceHash;

		public static Action onAlterationDetected;

		public static bool preservePlayerPrefs;

		public static Action onPossibleForeignSavesDetected;

		public static DeviceLockLevel lockToDevice;

		public static bool readForeignSaves;

		public static bool emergencyMode;

		private static string DeviceHash
		{
			get
			{
				if (string.IsNullOrEmpty(deviceHash))
				{
					deviceHash = GetDeviceID();
				}
				return deviceHash;
			}
		}

		public static void ForceLockToDeviceInit()
		{
			if (string.IsNullOrEmpty(deviceHash))
			{
				deviceHash = GetDeviceID();
			}
			else
			{
				Debug.LogWarning("[ACT] ObscuredPrefs.ForceLockToDeviceInit() is called, but LockToDevice feature is already inited!");
			}
		}

		public static void SetNewCryptoKey(string newKey)
		{
			encryptionKey = newKey;
		}

		public static void SetInt(string key, int value)
		{
			SetStringValue(key, value.ToString());
		}

		public static int GetInt(string key)
		{
			return GetInt(key, 0);
		}

		public static int GetInt(string key, int defaultValue)
		{
			string key2 = EncryptKey(key);
			if (!PlayerPrefs.HasKey(key2) && PlayerPrefs.HasKey(key))
			{
				int @int = PlayerPrefs.GetInt(key, defaultValue);
				if (!preservePlayerPrefs)
				{
					SetInt(key, @int);
					PlayerPrefs.DeleteKey(key);
				}
				return @int;
			}
			string data = GetData(key2, defaultValue.ToString());
			int.TryParse(data, out int result);
			return result;
		}

		public static void SetString(string key, string value)
		{
			SetStringValue(key, value);
		}

		public static string GetString(string key)
		{
			return GetString(key, string.Empty);
		}

		public static string GetString(string key, string defaultValue)
		{
			string key2 = EncryptKey(key);
			if (!PlayerPrefs.HasKey(key2) && PlayerPrefs.HasKey(key))
			{
				string @string = PlayerPrefs.GetString(key, defaultValue);
				if (!preservePlayerPrefs)
				{
					SetString(key, @string);
					PlayerPrefs.DeleteKey(key);
				}
				return @string;
			}
			return GetData(key2, defaultValue);
		}

		public static void SetFloat(string key, float value)
		{
			SetStringValue(key, value.ToString());
		}

		public static float GetFloat(string key)
		{
			return GetFloat(key, 0f);
		}

		public static float GetFloat(string key, float defaultValue)
		{
			string key2 = EncryptKey(key);
			if (!PlayerPrefs.HasKey(key2) && PlayerPrefs.HasKey(key))
			{
				float @float = PlayerPrefs.GetFloat(key, defaultValue);
				if (!preservePlayerPrefs)
				{
					SetFloat(key, @float);
					PlayerPrefs.DeleteKey(key);
				}
				return @float;
			}
			string data = GetData(key2, defaultValue.ToString());
			float.TryParse(data, out float result);
			return result;
		}

		public static void SetDouble(string key, double value)
		{
			SetStringValue(key, value.ToString());
		}

		public static double GetDouble(string key)
		{
			return GetDouble(key, 0.0);
		}

		public static double GetDouble(string key, double defaultValue)
		{
			string data = GetData(EncryptKey(key), defaultValue.ToString());
			double.TryParse(data, out double result);
			return result;
		}

		public static void SetLong(string key, long value)
		{
			SetStringValue(key, value.ToString());
		}

		public static long GetLong(string key)
		{
			return GetLong(key, 0L);
		}

		public static long GetLong(string key, long defaultValue)
		{
			string data = GetData(EncryptKey(key), defaultValue.ToString());
			long.TryParse(data, out long result);
			return result;
		}

		public static void SetBool(string key, bool value)
		{
			SetInt(key, value ? 1 : 0);
		}

		public static bool GetBool(string key)
		{
			return GetBool(key, defaultValue: false);
		}

		public static bool GetBool(string key, bool defaultValue)
		{
			string data = GetData(EncryptKey(key), (defaultValue ? 1 : 0).ToString());
			int.TryParse(data, out int result);
			return result == 1;
		}

		public static void SetVector3(string key, Vector3 value)
		{
			string value2 = value.x + "|" + value.y + "|" + value.z;
			SetStringValue(key, value2);
		}

		public static Vector3 GetVector3(string key)
		{
			return GetVector3(key, Vector3.zero);
		}

		public static Vector3 GetVector3(string key, Vector3 defaultValue)
		{
			string data = GetData(EncryptKey(key), "{not_found}");
			if (data == "{not_found}")
			{
				return defaultValue;
			}
			string[] array = data.Split('|');
			float.TryParse(array[0], out float result);
			float.TryParse(array[1], out float result2);
			float.TryParse(array[2], out float result3);
			return new Vector3(result, result2, result3);
		}

		public static void SetByteArray(string key, byte[] value)
		{
			SetStringValue(key, Encoding.UTF8.GetString(value, 0, value.Length));
		}

		public static byte[] GetByteArray(string key)
		{
			return GetByteArray(key, 0, 0);
		}

		public static byte[] GetByteArray(string key, byte defaultValue, int defaultLength)
		{
			string data = GetData(EncryptKey(key), "{not_found}");
			byte[] array;
			if (data == "{not_found}")
			{
				array = new byte[defaultLength];
				for (int i = 0; i < defaultLength; i++)
				{
					array[i] = defaultValue;
				}
			}
			else
			{
				array = Encoding.UTF8.GetBytes(data);
			}
			return array;
		}

		public static bool HasKey(string key)
		{
			if (PlayerPrefs.HasKey(key))
			{
				return true;
			}
			return PlayerPrefs.HasKey(EncryptKey(key));
		}

		public static void DeleteKey(string key)
		{
			PlayerPrefs.DeleteKey(EncryptKey(key));
			PlayerPrefs.DeleteKey(key);
		}

		public static void DeleteAll()
		{
			PlayerPrefs.DeleteAll();
		}

		public static void Save()
		{
			PlayerPrefs.Save();
		}

		private static void SetStringValue(string key, string value)
		{
			PlayerPrefs.SetString(EncryptKey(key), EncryptValue(value));
		}

		private static string GetData(string key, string defaultValueRaw)
		{
			string @string = PlayerPrefs.GetString(key, defaultValueRaw);
			if (@string != defaultValueRaw)
			{
				@string = DecryptValue(@string);
				if (@string == string.Empty)
				{
					@string = defaultValueRaw;
				}
			}
			else
			{
				string text = DecryptKey(key);
				string key2 = EncryptKeyDeprecated(text);
				@string = PlayerPrefs.GetString(key2, defaultValueRaw);
				if (@string != defaultValueRaw)
				{
					@string = DecryptValueDeprecated(@string);
					PlayerPrefs.DeleteKey(key2);
					SetStringValue(text, @string);
				}
				else if (PlayerPrefs.HasKey(text))
				{
					Debug.LogWarning("[ACT] Are you trying to read data saved with regular PlayerPrefs using ObscuredPrefs (key = " + text + ")?");
				}
			}
			return @string;
		}

		private static string EncryptKey(string key)
		{
			key = ObscuredString.EncryptDecrypt(key, encryptionKey);
			key = Convert.ToBase64String(Encoding.UTF8.GetBytes(key));
			return key;
		}

		private static string DecryptKey(string key)
		{
			byte[] array = Convert.FromBase64String(key);
			key = Encoding.UTF8.GetString(array, 0, array.Length);
			key = ObscuredString.EncryptDecrypt(key, encryptionKey);
			return key;
		}

		private static string EncryptValue(string value)
		{
			string s = ObscuredString.EncryptDecrypt(value, encryptionKey);
			s = Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
			if (lockToDevice != 0)
			{
				string text = s;
				return text + ':' + CalculateChecksum(s + DeviceHash) + ":" + DeviceHash;
			}
			return s + ':' + CalculateChecksum(s);
		}

		private static string DecryptValue(string value)
		{
			string[] array = value.Split(':');
			if (array.Length < 2)
			{
				SavesTampered();
				return string.Empty;
			}
			string text = array[0];
			string a = array[1];
			byte[] array2;
			try
			{
				array2 = Convert.FromBase64String(text);
			}
			catch
			{
				SavesTampered();
				return string.Empty;
				IL_004c:;
			}
			string @string = Encoding.UTF8.GetString(array2, 0, array2.Length);
			string result = ObscuredString.EncryptDecrypt(@string, encryptionKey);
			if (array.Length == 3)
			{
				if (a != CalculateChecksum(text + DeviceHash))
				{
					SavesTampered();
				}
			}
			else if (array.Length == 2)
			{
				if (a != CalculateChecksum(text))
				{
					SavesTampered();
				}
			}
			else
			{
				SavesTampered();
			}
			if (lockToDevice != 0 && !emergencyMode)
			{
				if (array.Length >= 3)
				{
					string a2 = array[2];
					if (a2 != DeviceHash)
					{
						if (!readForeignSaves)
						{
							result = string.Empty;
						}
						PossibleForeignSavesDetected();
					}
				}
				else if (lockToDevice == DeviceLockLevel.Strict)
				{
					if (!readForeignSaves)
					{
						result = string.Empty;
					}
					PossibleForeignSavesDetected();
				}
				else if (a != CalculateChecksum(text))
				{
					if (!readForeignSaves)
					{
						result = string.Empty;
					}
					PossibleForeignSavesDetected();
				}
			}
			return result;
		}

		private static string CalculateChecksum(string input)
		{
			int num = 0;
			byte[] bytes = Encoding.UTF8.GetBytes(input + encryptionKey);
			int num2 = bytes.Length;
			int num3 = encryptionKey.Length ^ 0x40;
			for (int i = 0; i < num2; i++)
			{
				byte b = bytes[i];
				num += b + b * (i + num3) % 3;
			}
			return num.ToString("X2");
		}

		private static void SavesTampered()
		{
			if (onAlterationDetected != null && !savesAlterationReported)
			{
				savesAlterationReported = true;
				onAlterationDetected();
			}
		}

		private static void PossibleForeignSavesDetected()
		{
			if (onPossibleForeignSavesDetected != null && !foreignSavesReported)
			{
				foreignSavesReported = true;
				onPossibleForeignSavesDetected();
			}
		}

		private static string GetDeviceID()
		{
			string text = string.Empty;
			if (string.IsNullOrEmpty(text))
			{
				text = SystemInfo.deviceUniqueIdentifier;
			}
			return CalculateChecksum(text);
		}

		private static string EncryptKeyDeprecated(string key)
		{
			key = ObscuredString.EncryptDecrypt(key);
			if (lockToDevice != 0)
			{
				key = ObscuredString.EncryptDecrypt(key, GetDeviceIDDeprecated());
			}
			key = Convert.ToBase64String(Encoding.UTF8.GetBytes(key));
			return key;
		}

		private static string DecryptValueDeprecated(string value)
		{
			byte[] array = Convert.FromBase64String(value);
			value = Encoding.UTF8.GetString(array, 0, array.Length);
			if (lockToDevice != 0)
			{
				value = ObscuredString.EncryptDecrypt(value, GetDeviceIDDeprecated());
			}
			value = ObscuredString.EncryptDecrypt(value, encryptionKey);
			return value;
		}

		private static string GetDeviceIDDeprecated()
		{
			return SystemInfo.deviceUniqueIdentifier;
		}
	}
}
