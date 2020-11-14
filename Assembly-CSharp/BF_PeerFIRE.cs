public struct BF_PeerFIRE
{
	public ushort bitvector1;

	public ushort slot
	{
		get
		{
			return (ushort)(bitvector1 & 0xF);
		}
		set
		{
			bitvector1 = (ushort)(value | bitvector1);
		}
	}

	public ushort ammoId
	{
		get
		{
			return (ushort)((uint)(bitvector1 & 0xFFF0) / 16u);
		}
		set
		{
			bitvector1 = (ushort)((value * 16) | bitvector1);
		}
	}
}
