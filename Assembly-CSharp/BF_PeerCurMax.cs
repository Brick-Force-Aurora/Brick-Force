public struct BF_PeerCurMax
{
	public uint bitvector1;

	public uint cur
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

	public uint max
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
}
