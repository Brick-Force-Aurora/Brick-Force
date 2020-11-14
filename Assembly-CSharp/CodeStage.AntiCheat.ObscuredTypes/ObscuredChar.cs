using System;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public struct ObscuredChar : IEquatable<ObscuredChar>
	{
		public static Action onCheatingDetected;

		private static char cryptoKey = '—';

		private char currentCryptoKey;

		private char hiddenValue;

		private char fakeValue;

		private bool inited;

		private ObscuredChar(char value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = '\0';
			inited = true;
		}

		public static void SetNewCryptoKey(char newKey)
		{
			cryptoKey = newKey;
		}

		public char GetEncrypted()
		{
			if (currentCryptoKey != cryptoKey)
			{
				hiddenValue = InternalDecrypt();
				hiddenValue = EncryptDecrypt(hiddenValue, cryptoKey);
				currentCryptoKey = cryptoKey;
			}
			return hiddenValue;
		}

		public void SetEncrypted(char encrypted)
		{
			hiddenValue = encrypted;
			if (onCheatingDetected != null)
			{
				fakeValue = InternalDecrypt();
			}
		}

		public static char EncryptDecrypt(char value)
		{
			return EncryptDecrypt(value, '\0');
		}

		public static char EncryptDecrypt(char value, char key)
		{
			if (key == '\0')
			{
				return (char)(value ^ cryptoKey);
			}
			return (char)(value ^ key);
		}

		private char InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = EncryptDecrypt('\0');
				fakeValue = '\0';
				inited = true;
			}
			char key = cryptoKey;
			if (currentCryptoKey != cryptoKey)
			{
				key = currentCryptoKey;
			}
			char c = EncryptDecrypt(hiddenValue, key);
			if (onCheatingDetected != null && fakeValue != 0 && c != fakeValue)
			{
				onCheatingDetected();
				onCheatingDetected = null;
			}
			return c;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredChar))
			{
				return false;
			}
			ObscuredChar obscuredChar = (ObscuredChar)obj;
			return hiddenValue == obscuredChar.hiddenValue;
		}

		public bool Equals(ObscuredChar obj)
		{
			return hiddenValue == obj.hiddenValue;
		}

		public override string ToString()
		{
			return InternalDecrypt().ToString();
		}

		public override int GetHashCode()
		{
			return InternalDecrypt().GetHashCode();
		}

		public static implicit operator ObscuredChar(char value)
		{
			ObscuredChar result = new ObscuredChar(EncryptDecrypt(value));
			if (onCheatingDetected != null)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator char(ObscuredChar value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredChar operator ++(ObscuredChar input)
		{
			char value = (char)(input.InternalDecrypt() + 1);
			input.hiddenValue = EncryptDecrypt(value, input.currentCryptoKey);
			if (onCheatingDetected != null)
			{
				input.fakeValue = value;
			}
			return input;
		}

		public static ObscuredChar operator --(ObscuredChar input)
		{
			char value = (char)(input.InternalDecrypt() - 1);
			input.hiddenValue = EncryptDecrypt(value, input.currentCryptoKey);
			if (onCheatingDetected != null)
			{
				input.fakeValue = value;
			}
			return input;
		}
	}
}
