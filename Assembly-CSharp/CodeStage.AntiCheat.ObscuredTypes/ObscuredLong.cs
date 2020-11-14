using System;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public struct ObscuredLong : IEquatable<ObscuredLong>
	{
		public static Action onCheatingDetected;

		private static long cryptoKey = 444442L;

		private long currentCryptoKey;

		private long hiddenValue;

		private long fakeValue;

		private bool inited;

		private ObscuredLong(long value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = 0L;
			inited = true;
		}

		public static void SetNewCryptoKey(long newKey)
		{
			cryptoKey = newKey;
		}

		public long GetEncrypted()
		{
			if (currentCryptoKey != cryptoKey)
			{
				hiddenValue = InternalDecrypt();
				hiddenValue = Encrypt(hiddenValue, cryptoKey);
				currentCryptoKey = cryptoKey;
			}
			return hiddenValue;
		}

		public void SetEncrypted(long encrypted)
		{
			hiddenValue = encrypted;
			if (onCheatingDetected != null)
			{
				fakeValue = InternalDecrypt();
			}
		}

		public static long Encrypt(long value)
		{
			return Encrypt(value, 0L);
		}

		public static long Decrypt(long value)
		{
			return Decrypt(value, 0L);
		}

		public static long Encrypt(long value, long key)
		{
			if (key == 0L)
			{
				return value ^ cryptoKey;
			}
			return value ^ key;
		}

		public static long Decrypt(long value, long key)
		{
			if (key == 0L)
			{
				return value ^ cryptoKey;
			}
			return value ^ key;
		}

		private long InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = Encrypt(0L);
				fakeValue = 0L;
				inited = true;
			}
			long key = cryptoKey;
			if (currentCryptoKey != cryptoKey)
			{
				key = currentCryptoKey;
			}
			long num = Decrypt(hiddenValue, key);
			if (onCheatingDetected != null && fakeValue != 0L && num != fakeValue)
			{
				onCheatingDetected();
				onCheatingDetected = null;
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredLong))
			{
				return false;
			}
			ObscuredLong obscuredLong = (ObscuredLong)obj;
			return hiddenValue == obscuredLong.hiddenValue;
		}

		public bool Equals(ObscuredLong obj)
		{
			return hiddenValue == obj.hiddenValue;
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

		public string ToString(IFormatProvider provider)
		{
			return InternalDecrypt().ToString(provider);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return InternalDecrypt().ToString(format, provider);
		}

		public static implicit operator ObscuredLong(long value)
		{
			ObscuredLong result = new ObscuredLong(Encrypt(value));
			if (onCheatingDetected != null)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator long(ObscuredLong value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredLong operator ++(ObscuredLong input)
		{
			long value = input.InternalDecrypt() + 1;
			input.hiddenValue = Encrypt(value, input.currentCryptoKey);
			if (onCheatingDetected != null)
			{
				input.fakeValue = value;
			}
			return input;
		}

		public static ObscuredLong operator --(ObscuredLong input)
		{
			long value = input.InternalDecrypt() - 1;
			input.hiddenValue = Encrypt(value, input.currentCryptoKey);
			if (onCheatingDetected != null)
			{
				input.fakeValue = value;
			}
			return input;
		}
	}
}
