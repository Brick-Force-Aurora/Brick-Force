public struct BF_PeeMOVE
{
	public byte bitvector1;

	public byte cc
	{
		get
		{
			return (byte)(bitvector1 & 0x3F);
		}
		set
		{
			bitvector1 = (byte)(value | bitvector1);
		}
	}

	public bool isDead
	{
		get
		{
			byte b = (byte)((uint)(bitvector1 & 0x40) / 64u);
			return (b == 1) ? true : false;
		}
		set
		{
			byte b = value ? (b = 1) : (b = 0);
			bitvector1 = (byte)((b * 64) | bitvector1);
		}
	}

	public bool isRegularSend
	{
		get
		{
			byte b = (byte)((uint)(bitvector1 & 0x80) / 128u);
			return (b == 1) ? true : false;
		}
		set
		{
			byte b = value ? (b = 1) : (b = 0);
			bitvector1 = (byte)((b * 128) | bitvector1);
		}
	}
}
