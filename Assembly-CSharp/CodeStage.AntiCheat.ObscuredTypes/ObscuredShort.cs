using System;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public struct ObscuredShort : IEquatable<ObscuredShort>
	{
		public static Action onCheatingDetected;

		private static short cryptoKey = 214;

		private short currentCryptoKey;

		private short hiddenValue;

		private short fakeValue;

		private bool inited;

		private ObscuredShort(short value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = 0;
			inited = true;
		}

		public static void SetNewCryptoKey(short newKey)
		{
			cryptoKey = newKey;
		}

		public short GetEncrypted()
		{
			if (currentCryptoKey != cryptoKey)
			{
				hiddenValue = InternalDecrypt();
				hiddenValue = EncryptDecrypt(hiddenValue, cryptoKey);
				currentCryptoKey = cryptoKey;
			}
			return hiddenValue;
		}

		public void SetEncrypted(short encrypted)
		{
			hiddenValue = encrypted;
			if (onCheatingDetected != null)
			{
				fakeValue = InternalDecrypt();
			}
		}

		public static short EncryptDecrypt(short value)
		{
			return EncryptDecrypt(value, 0);
		}

		public static short EncryptDecrypt(short value, short key)
		{
			if (key == 0)
			{
				return (short)(value ^ cryptoKey);
			}
			return (short)(value ^ key);
		}

		private short InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = EncryptDecrypt(0);
				fakeValue = 0;
				inited = true;
			}
			short key = cryptoKey;
			if (currentCryptoKey != cryptoKey)
			{
				key = currentCryptoKey;
			}
			short num = EncryptDecrypt(hiddenValue, key);
			if (onCheatingDetected != null && fakeValue != 0 && num != fakeValue)
			{
				onCheatingDetected();
				onCheatingDetected = null;
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredShort))
			{
				return false;
			}
			ObscuredShort obscuredShort = (ObscuredShort)obj;
			return hiddenValue == obscuredShort.hiddenValue;
		}

		public bool Equals(ObscuredShort obj)
		{
			return hiddenValue == obj.hiddenValue;
		}

		public override string ToString()
		{
			return InternalDecrypt().ToString();
		}

		public string ToString(string format)
		{
			return InternalDecrypt().ToString(format);
		}

		public override int GetHashCode()
		{
			return InternalDecrypt().GetHashCode();
		}

		public string ToString(IFormatProvider provider)
		{
			return InternalDecrypt().ToString(provider);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return InternalDecrypt().ToString(format, provider);
		}

		public static implicit operator ObscuredShort(short value)
		{
			ObscuredShort result = new ObscuredShort(EncryptDecrypt(value));
			if (onCheatingDetected != null)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator short(ObscuredShort value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredShort operator ++(ObscuredShort input)
		{
			short value = (short)(input.InternalDecrypt() + 1);
			input.hiddenValue = EncryptDecrypt(value);
			if (onCheatingDetected != null)
			{
				input.fakeValue = value;
			}
			return input;
		}

		public static ObscuredShort operator --(ObscuredShort input)
		{
			short value = (short)(input.InternalDecrypt() - 1);
			input.hiddenValue = EncryptDecrypt(value);
			if (onCheatingDetected != null)
			{
				input.fakeValue = value;
			}
			return input;
		}
	}
}
