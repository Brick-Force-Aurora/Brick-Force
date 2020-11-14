using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public struct ObscuredVector3
	{
		public static Action onCheatingDetected;

		private static int cryptoKey = 120207;

		private int currentCryptoKey;

		private Vector3 hiddenValue;

		private Vector3 fakeValue;

		private bool inited;

		public float x
		{
			get
			{
				float num = InternalDecryptField(hiddenValue.x);
				if (onCheatingDetected != null && fakeValue != new Vector3(0f, 0f) && Math.Abs(num - fakeValue.x) > 0.0005f)
				{
					onCheatingDetected();
					onCheatingDetected = null;
				}
				return num;
			}
			set
			{
				hiddenValue.x = InternalEncryptField(value);
				if (onCheatingDetected != null)
				{
					fakeValue.x = value;
				}
			}
		}

		public float y
		{
			get
			{
				float num = InternalDecryptField(hiddenValue.y);
				if (onCheatingDetected != null && fakeValue != new Vector3(0f, 0f) && Math.Abs(num - fakeValue.y) > 0.0005f)
				{
					onCheatingDetected();
					onCheatingDetected = null;
				}
				return num;
			}
			set
			{
				hiddenValue.y = InternalEncryptField(value);
				if (onCheatingDetected != null)
				{
					fakeValue.y = value;
				}
			}
		}

		public float z
		{
			get
			{
				float num = InternalDecryptField(hiddenValue.z);
				if (onCheatingDetected != null && fakeValue != new Vector3(0f, 0f) && Math.Abs(num - fakeValue.z) > 0.0005f)
				{
					onCheatingDetected();
					onCheatingDetected = null;
				}
				return num;
			}
			set
			{
				hiddenValue.z = InternalEncryptField(value);
				if (onCheatingDetected != null)
				{
					fakeValue.z = value;
				}
			}
		}

		public float this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return x;
				case 1:
					return y;
				case 2:
					return z;
				default:
					throw new IndexOutOfRangeException("Invalid ObscuredVector3 index!");
				}
			}
			set
			{
				switch (index)
				{
				case 0:
					x = value;
					break;
				case 1:
					y = value;
					break;
				case 2:
					z = value;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid ObscuredVector3 index!");
				}
			}
		}

		private ObscuredVector3(Vector3 value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = new Vector3(0f, 0f);
			inited = true;
		}

		public static void SetNewCryptoKey(int newKey)
		{
			cryptoKey = newKey;
		}

		public Vector3 GetEncrypted()
		{
			if (currentCryptoKey != cryptoKey)
			{
				Vector3 value = InternalDecrypt();
				hiddenValue = Encrypt(value, cryptoKey);
				currentCryptoKey = cryptoKey;
			}
			return hiddenValue;
		}

		public void SetEncrypted(Vector3 encrypted)
		{
			hiddenValue = encrypted;
			if (onCheatingDetected != null)
			{
				fakeValue = InternalDecrypt();
			}
		}

		public static Vector3 Encrypt(Vector3 value)
		{
			return Encrypt(value, 0);
		}

		public static Vector3 Encrypt(Vector3 value, int key)
		{
			if (key == 0)
			{
				key = cryptoKey;
			}
			value.x = (float)ObscuredDouble.Encrypt((double)value.x, key);
			value.y = (float)ObscuredDouble.Encrypt((double)value.y, key);
			value.z = (float)ObscuredDouble.Encrypt((double)value.z, key);
			return value;
		}

		public static Vector3 Decrypt(Vector3 value)
		{
			return Decrypt(value, 0);
		}

		public static Vector3 Decrypt(Vector3 value, int key)
		{
			if (key == 0)
			{
				key = cryptoKey;
			}
			value.x = (float)ObscuredDouble.Decrypt((long)value.x, key);
			value.y = (float)ObscuredDouble.Decrypt((long)value.y, key);
			value.z = (float)ObscuredDouble.Decrypt((long)value.z, key);
			return value;
		}

		private Vector3 InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = Encrypt(new Vector3(0f, 0f));
				fakeValue = new Vector3(0f, 0f);
				inited = true;
			}
			int num = cryptoKey;
			if (currentCryptoKey != cryptoKey)
			{
				num = currentCryptoKey;
			}
			Vector3 result = default(Vector3);
			result.x = (float)ObscuredDouble.Decrypt((long)hiddenValue.x, num);
			result.y = (float)ObscuredDouble.Decrypt((long)hiddenValue.y, num);
			result.z = (float)ObscuredDouble.Decrypt((long)hiddenValue.z, num);
			if (onCheatingDetected != null && !fakeValue.Equals(new Vector3(0f, 0f)) && !result.Equals(fakeValue))
			{
				onCheatingDetected();
				onCheatingDetected = null;
			}
			return result;
		}

		private float InternalDecryptField(float encrypted)
		{
			int num = cryptoKey;
			if (currentCryptoKey != cryptoKey)
			{
				num = currentCryptoKey;
			}
			return (float)ObscuredDouble.Decrypt((long)encrypted, num);
		}

		private float InternalEncryptField(float encrypted)
		{
			return (float)ObscuredDouble.Encrypt((double)encrypted, cryptoKey);
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

		public static implicit operator ObscuredVector3(Vector3 value)
		{
			ObscuredVector3 result = new ObscuredVector3(Encrypt(value));
			if (onCheatingDetected != null)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator Vector3(ObscuredVector3 value)
		{
			return value.InternalDecrypt();
		}
	}
}
