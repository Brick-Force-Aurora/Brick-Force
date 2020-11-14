using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public struct ObscuredQuaternion
	{
		public static Action onCheatingDetected;

		private static int cryptoKey = 120205;

		private int currentCryptoKey;

		private Quaternion hiddenValue;

		public Quaternion fakeValue;

		private bool inited;

		private ObscuredQuaternion(Quaternion value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = Quaternion.identity;
			inited = true;
		}

		public static void SetNewCryptoKey(int newKey)
		{
			cryptoKey = newKey;
		}

		public Quaternion GetEncrypted()
		{
			if (currentCryptoKey != cryptoKey)
			{
				Quaternion value = InternalDecrypt();
				hiddenValue = Encrypt(value, cryptoKey);
				currentCryptoKey = cryptoKey;
			}
			return hiddenValue;
		}

		public void SetEncrypted(Quaternion encrypted)
		{
			hiddenValue = encrypted;
			if (onCheatingDetected != null)
			{
				fakeValue = InternalDecrypt();
			}
		}

		public static Quaternion Encrypt(Quaternion value)
		{
			return Encrypt(value, 0);
		}

		public static Quaternion Encrypt(Quaternion value, int key)
		{
			if (key == 0)
			{
				key = cryptoKey;
			}
			value.x = (float)ObscuredDouble.Encrypt((double)value.x, key);
			value.y = (float)ObscuredDouble.Encrypt((double)value.y, key);
			value.z = (float)ObscuredDouble.Encrypt((double)value.z, key);
			value.w = (float)ObscuredDouble.Encrypt((double)value.w, key);
			return value;
		}

		public static Quaternion Decrypt(Quaternion value)
		{
			return Decrypt(value, 0);
		}

		public static Quaternion Decrypt(Quaternion value, int key)
		{
			if (key == 0)
			{
				key = cryptoKey;
			}
			value.x = (float)ObscuredDouble.Decrypt((long)value.x, key);
			value.y = (float)ObscuredDouble.Decrypt((long)value.y, key);
			value.z = (float)ObscuredDouble.Decrypt((long)value.z, key);
			value.w = (float)ObscuredDouble.Decrypt((long)value.w, key);
			return value;
		}

		private Quaternion InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = Encrypt(Quaternion.identity);
				fakeValue = Quaternion.identity;
				inited = true;
			}
			int num = cryptoKey;
			if (currentCryptoKey != cryptoKey)
			{
				num = currentCryptoKey;
			}
			Quaternion result = default(Quaternion);
			result.x = (float)ObscuredDouble.Decrypt((long)hiddenValue.x, num);
			result.y = (float)ObscuredDouble.Decrypt((long)hiddenValue.y, num);
			result.z = (float)ObscuredDouble.Decrypt((long)hiddenValue.z, num);
			result.w = (float)ObscuredDouble.Decrypt((long)hiddenValue.w, num);
			if (onCheatingDetected != null && !fakeValue.Equals(Quaternion.identity) && !result.Equals(fakeValue))
			{
				onCheatingDetected();
				onCheatingDetected = null;
			}
			return result;
		}

		public override int GetHashCode()
		{
			return InternalDecrypt().GetHashCode();
		}

		public override string ToString()
		{
			return InternalDecrypt().ToString();
		}

		public string ToString(string format)
		{
			return InternalDecrypt().ToString(format);
		}

		public static implicit operator ObscuredQuaternion(Quaternion value)
		{
			ObscuredQuaternion result = new ObscuredQuaternion(Encrypt(value));
			if (onCheatingDetected != null)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator Quaternion(ObscuredQuaternion value)
		{
			return value.InternalDecrypt();
		}
	}
}
