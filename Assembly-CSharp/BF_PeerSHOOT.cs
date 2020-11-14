public struct BF_PeerSHOOT
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

	public int hitpart
	{
		get
		{
			return (int)((uint)(bitvector1 & 0xE) / 2u);
		}
		set
		{
			bitvector1 = (ushort)(((ushort)value * 2) | bitvector1);
		}
	}

	public int damage
	{
		get
		{
			return (int)((uint)(bitvector1 & 0xFFF0) / 16u);
		}
		set
		{
			bitvector1 = (ushort)(((ushort)value * 16) | bitvector1);
		}
	}
}
