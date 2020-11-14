public struct BF_PeerHP
{
	public uint bitvector1;

	public uint hp
	{
		get
		{
			return bitvector1 & 0x3FF;
		}
		set
		{
			bitvector1 = (value | bitvector1);
		}
	}

	public uint armor
	{
		get
		{
			return (bitvector1 & 0xFFC00) / 1024u;
		}
		set
		{
			bitvector1 = ((value * 1024) | bitvector1);
		}
	}

	public uint maxArmor
	{
		get
		{
			return (uint)((int)bitvector1 & -1048576) / 1048576u;
		}
		set
		{
			bitvector1 = ((value * 1048576) | bitvector1);
		}
	}
}
