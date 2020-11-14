using System;

[Serializable]
internal class BitField
{
	public static void AddToBitfield(ref int bitfield, int bitCount, int value)
	{
		bitfield <<= bitCount;
		bitfield |= value;
	}

	public static int ReadFromBitfield(ref int bitfield, int bitCount)
	{
		int result = bitfield & ((1 << bitCount) - 1);
		bitfield >>= bitCount;
		return result;
	}
}
