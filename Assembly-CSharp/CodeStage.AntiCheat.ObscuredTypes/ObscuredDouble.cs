using System;
using System.Runtime.InteropServices;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public struct ObscuredDouble : IEquatable<ObscuredDouble>
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct DoubleLongBytesUnion
		{
			[FieldOffset(0)]
			public double d;

			[FieldOffset(0)]
			public long l;

			[FieldOffset(0)]
			public byte b1;

			[FieldOffset(1)]
			public byte b2;

			[FieldOffset(2)]
			public byte b3;

			[FieldOffset(3)]
			public byte b4;

			[FieldOffset(4)]
			public byte b5;

			[FieldOffset(5)]
			public byte b6;

			[FieldOffset(6)]
			public byte b7;

			[FieldOffset(7)]
			public byte b8;
		}

		public static Action onCheatingDetected;

		private static long cryptoKey = 210987L;

		private long currentCryptoKey;

		private byte[] hiddenValue;

		private double fakeValue;

		private bool inited;

		private ObscuredDouble(byte[] value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = 0.0;
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
				double value = InternalDecrypt();
				hiddenValue = InternalEncrypt(value);
				currentCryptoKey = cryptoKey;
			}
			DoubleLongBytesUnion doubleLongBytesUnion = default(DoubleLongBytesUnion);
			doubleLongBytesUnion.b1 = hiddenValue[0];
			doubleLongBytesUnion.b2 = hiddenValue[1];
			doubleLongBytesUnion.b3 = hiddenValue[2];
			doubleLongBytesUnion.b4 = hiddenValue[3];
			doubleLongBytesUnion.b5 = hiddenValue[4];
			doubleLongBytesUnion.b6 = hiddenValue[5];
			doubleLongBytesUnion.b7 = hiddenValue[6];
			doubleLongBytesUnion.b8 = hiddenValue[7];
			return doubleLongBytesUnion.l;
		}

		public void SetEncrypted(long encrypted)
		{
			DoubleLongBytesUnion doubleLongBytesUnion = default(DoubleLongBytesUnion);
			doubleLongBytesUnion.l = encrypted;
			hiddenValue = new byte[8]
			{
				doubleLongBytesUnion.b1,
				doubleLongBytesUnion.b2,
				doubleLongBytesUnion.b3,
				doubleLongBytesUnion.b4,
				doubleLongBytesUnion.b5,
				doubleLongBytesUnion.b6,
				doubleLongBytesUnion.b7,
				doubleLongBytesUnion.b8
			};
			if (onCheatingDetected != null)
			{
				fakeValue = InternalDecrypt();
			}
		}

		public static long Encrypt(double value)
		{
			return Encrypt(value, cryptoKey);
		}

		public static long Encrypt(double value, long key)
		{
			DoubleLongBytesUnion doubleLongBytesUnion = default(DoubleLongBytesUnion);
			doubleLongBytesUnion.d = value;
			doubleLongBytesUnion.l ^= key;
			return doubleLongBytesUnion.l;
		}

		private static byte[] InternalEncrypt(double value)
		{
			return InternalEncrypt(value, 0L);
		}

		private static byte[] InternalEncrypt(double value, long key)
		{
			long num = key;
			if (num == 0L)
			{
				num = cryptoKey;
			}
			DoubleLongBytesUnion doubleLongBytesUnion = default(DoubleLongBytesUnion);
			doubleLongBytesUnion.d = value;
			doubleLongBytesUnion.l ^= num;
			return new byte[8]
			{
				doubleLongBytesUnion.b1,
				doubleLongBytesUnion.b2,
				doubleLongBytesUnion.b3,
				doubleLongBytesUnion.b4,
				doubleLongBytesUnion.b5,
				doubleLongBytesUnion.b6,
				doubleLongBytesUnion.b7,
				doubleLongBytesUnion.b8
			};
		}

		public static double Decrypt(long value)
		{
			return Decrypt(value, cryptoKey);
		}

		public static double Decrypt(long value, long key)
		{
			DoubleLongBytesUnion doubleLongBytesUnion = default(DoubleLongBytesUnion);
			doubleLongBytesUnion.l = (value ^ key);
			return doubleLongBytesUnion.d;
		}

		private double InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = InternalEncrypt(0.0);
				fakeValue = 0.0;
				inited = true;
			}
			long num = cryptoKey;
			if (currentCryptoKey != cryptoKey)
			{
				num = currentCryptoKey;
			}
			DoubleLongBytesUnion doubleLongBytesUnion = default(DoubleLongBytesUnion);
			doubleLongBytesUnion.b1 = hiddenValue[0];
			doubleLongBytesUnion.b2 = hiddenValue[1];
			doubleLongBytesUnion.b3 = hiddenValue[2];
			doubleLongBytesUnion.b4 = hiddenValue[3];
			doubleLongBytesUnion.b5 = hiddenValue[4];
			doubleLongBytesUnion.b6 = hiddenValue[5];
			doubleLongBytesUnion.b7 = hiddenValue[6];
			doubleLongBytesUnion.b8 = hiddenValue[7];
			doubleLongBytesUnion.l ^= num;
			double d = doubleLongBytesUnion.d;
			if (onCheatingDetected != null && fakeValue != 0.0 && Math.Abs(d - fakeValue) > 1E-06)
			{
				onCheatingDetected();
				onCheatingDetected = null;
			}
			return d;
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

		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredDouble))
			{
				return false;
			}
			double num = ((ObscuredDouble)obj).InternalDecrypt();
			double num2 = InternalDecrypt();
			if (num == num2)
			{
				return true;
			}
			return double.IsNaN(num) && double.IsNaN(num2);
		}

		public bool Equals(ObscuredDouble obj)
		{
			double num = obj.InternalDecrypt();
			double num2 = InternalDecrypt();
			if (num == num2)
			{
				return true;
			}
			return double.IsNaN(num) && double.IsNaN(num2);
		}

		public override int GetHashCode()
		{
			return InternalDecrypt().GetHashCode();
		}

		public static implicit operator ObscuredDouble(double value)
		{
			ObscuredDouble result = new ObscuredDouble(InternalEncrypt(value));
			if (onCheatingDetected != null)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator double(ObscuredDouble value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredDouble operator ++(ObscuredDouble input)
		{
			double value = input.InternalDecrypt() + 1.0;
			input.hiddenValue = InternalEncrypt(value, input.currentCryptoKey);
			if (onCheatingDetected != null)
			{
				input.fakeValue = value;
			}
			return input;
		}

		public static ObscuredDouble operator --(ObscuredDouble input)
		{
			double value = input.InternalDecrypt() - 1.0;
			input.hiddenValue = InternalEncrypt(value, input.currentCryptoKey);
			if (onCheatingDetected != null)
			{
				input.fakeValue = value;
			}
			return input;
		}
	}
}
