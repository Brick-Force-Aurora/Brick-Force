public struct SlotAmmoDamage
{
	public uint bitvector1;

	public uint slot
	{
		get
		{
			return bitvector1 & 0xF;
		}
		set
		{
			bitvector1 = (value | bitvector1);
		}
	}

	public uint ammoId
	{
		get
		{
			return (bitvector1 & 0xFFF0) / 16u;
		}
		set
		{
			bitvector1 = ((value * 16) | bitvector1);
		}
	}

	public uint damage
	{
		get
		{
			return (uint)((int)bitvector1 & -65536) / 65536u;
		}
		set
		{
			bitvector1 = ((value * 65536) | bitvector1);
		}
	}
}
