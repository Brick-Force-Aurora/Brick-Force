public struct BF_PEER_HIT_BRICKMAN
{
	public ushort bitvector1;

	public bool lucky
	{
		get
		{
			uint num = (uint)(bitvector1 & 1);
			return (num == 1) ? true : false;
		}
		set
		{
			uint num = value ? (num = 1u) : (num = 0u);
			bitvector1 = (ushort)(num | bitvector1);
		}
	}

	public int part
	{
		get
		{
			return (int)((uint)(bitvector1 & 0x1E) / 2u);
		}
		set
		{
			bitvector1 = (ushort)((value * 2) | bitvector1);
		}
	}

	public int curammo
	{
		get
		{
			return (int)((uint)(bitvector1 & 0xFFE0) / 32u);
		}
		set
		{
			bitvector1 = (ushort)((value * 32) | bitvector1);
		}
	}
}
