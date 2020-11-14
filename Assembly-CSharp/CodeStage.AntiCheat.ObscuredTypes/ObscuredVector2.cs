using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public struct ObscuredVector2
	{
		public static Action onCheatingDetected;

		private static int cryptoKey = 120206;

		private int currentCryptoKey;

		private Vector2 hiddenValue;

		private Vector2 fakeValue;

		private bool inited;

		public float x
		{
			get
			{
				float num = InternalDecryptField(hiddenValue.x);
				if (onCheatingDetected != null && fakeValue != new Vector2(0f, 0f) && Math.Abs(num - fakeValue.x) > 0.0005f)
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
				if (onCheatingDetected != null && fakeValue != new Vector2(0f, 0f) && Math.Abs(num - fakeValue.y) > 0.0005f)
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
				default:
					throw new IndexOutOfRangeException("Invalid ObscuredVector2 index!");
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
				default:
					throw new IndexOutOfRangeException("Invalid ObscuredVector2 index!");
				}
			}
		}

		private ObscuredVector2(Vector2 value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = new Vector2(0f, 0f);
			inited = true;
		}

		public static void SetNewCryptoKey(int newKey)
		{
			cryptoKey = newKey;
		}

		public Vector2 GetEncrypted()
		{
			if (currentCryptoKey != cryptoKey)
			{
				Vector2 value = InternalDecrypt();
				hiddenValue = Encrypt(value, cryptoKey);
				currentCryptoKey = cryptoKey;
			}
			return hiddenValue;
		}

		public void SetEncrypted(Vector2 encrypted)
		{
			hiddenValue = encrypted;
			if (onCheatingDetected != null)
			{
				fakeValue = InternalDecrypt();
			}
		}

		public static Vector2 Encrypt(Vector2 value)
		{
			return Encrypt(value, 0);
		}

		public static Vector2 Encrypt(Vector2 value, int key)
		{
			if (key == 0)
			{
				key = cryptoKey;
			}
			value.x = (float)ObscuredDouble.Encrypt((double)value.x, key);
			value.y = (float)ObscuredDouble.Encrypt((double)value.y, key);
			return value;
		}

		public static Vector2 Decrypt(Vector2 value)
		{
			return Decrypt(value, 0);
		}

		public static Vector2 Decrypt(Vector2 value, int key)
		{
			if (key == 0)
			{
				key = cryptoKey;
			}
			value.x = (float)ObscuredDouble.Decrypt((long)value.x, key);
			value.y = (float)ObscuredDouble.Decrypt((long)value.y, key);
			return value;
		}

		private Vector2 InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = Encrypt(new Vector2(0f, 0f));
				fakeValue = new Vector2(0f, 0f);
				inited = true;
			}
			int num = cryptoKey;
			if (currentCryptoKey != cryptoKey)
			{
				num = currentCryptoKey;
			}
			Vector2 result = default(Vector2);
			result.x = (float)ObscuredDouble.Decrypt((long)hiddenValue.x, num);
			result.y = (float)ObscuredDouble.Decrypt((long)hiddenValue.y, num);
			if (onCheatingDetected != null && !fakeValue.Equals(new Vector2(0f, 0f)) && !result.Equals(fakeValue))
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

		public static implicit operator ObscuredVector2(Vector2 value)
		{
			ObscuredVector2 result = new ObscuredVector2(Encrypt(value));
			if (onCheatingDetected != null)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator Vector2(ObscuredVector2 value)
		{
			return value.InternalDecrypt();
		}
	}
}
