using System;
using System.Text;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public sealed class ObscuredString
	{
		public static Action onCheatingDetected;

		private static string cryptoKey = "4441";

		private string currentCryptoKey;

		private string hiddenValue;

		private string fakeValue;

		private bool inited;

		private ObscuredString(string value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = null;
			inited = true;
		}

		public static void SetNewCryptoKey(string newKey)
		{
			cryptoKey = newKey;
		}

		public string GetEncrypted()
		{
			return hiddenValue;
		}

		public void SetEncrypted(string encrypted)
		{
			hiddenValue = encrypted;
			if (onCheatingDetected != null)
			{
				fakeValue = InternalDecrypt();
			}
		}

		public static string EncryptDecrypt(string value)
		{
			return EncryptDecrypt(value, string.Empty);
		}

		public static string EncryptDecrypt(string value, string key)
		{
			if (string.IsNullOrEmpty(value))
			{
				return string.Empty;
			}
			if (string.IsNullOrEmpty(key))
			{
				key = cryptoKey;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int length = key.Length;
			int length2 = value.Length;
			for (int i = 0; i < length2; i++)
			{
				stringBuilder.Append((char)(value[i] ^ key[i % length]));
			}
			return stringBuilder.ToString();
		}

		private string InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = EncryptDecrypt(string.Empty);
				fakeValue = string.Empty;
				inited = true;
			}
			string key = cryptoKey;
			if (currentCryptoKey != cryptoKey)
			{
				key = currentCryptoKey;
			}
			string text = EncryptDecrypt(hiddenValue, key);
			if (onCheatingDetected != null && fakeValue != null && text != fakeValue)
			{
				onCheatingDetected();
				onCheatingDetected = null;
			}
			return text;
		}

		public override string ToString()
		{
			return InternalDecrypt();
		}

		public override bool Equals(object obj)
		{
			ObscuredString obscuredString = obj as ObscuredString;
			string b = null;
			if (obscuredString != null)
			{
				b = obscuredString.hiddenValue;
			}
			return string.Equals(hiddenValue, b);
		}

		public bool Equals(ObscuredString value)
		{
			string b = null;
			if (value != null)
			{
				b = value.hiddenValue;
			}
			return string.Equals(hiddenValue, b);
		}

		public bool Equals(ObscuredString value, StringComparison comparisonType)
		{
			string b = null;
			if (value != null)
			{
				b = value.InternalDecrypt();
			}
			return string.Equals(InternalDecrypt(), b, comparisonType);
		}

		public override int GetHashCode()
		{
			return InternalDecrypt().GetHashCode();
		}

		public static implicit operator ObscuredString(string value)
		{
			if (value == null)
			{
				return null;
			}
			ObscuredString obscuredString = new ObscuredString(EncryptDecrypt(value));
			if (onCheatingDetected != null)
			{
				obscuredString.fakeValue = value;
			}
			return obscuredString;
		}

		public static implicit operator string(ObscuredString value)
		{
			if (value == null)
			{
				return null;
			}
			return value.InternalDecrypt();
		}

		public static bool operator ==(ObscuredString a, ObscuredString b)
		{
			if (object.ReferenceEquals(a, b))
			{
				return true;
			}
			if ((object)a == null || (object)b == null)
			{
				return false;
			}
			string a2 = a.hiddenValue;
			string b2 = b.hiddenValue;
			return string.Equals(a2, b2);
		}

		public static bool operator !=(ObscuredString a, ObscuredString b)
		{
			return !(a == b);
		}
	}
}
