public struct BF_PeerHITBRICKMAN_NEW
{
	public uint bitvector1;

	public uint slot
	{
		get
		{
			return bitvector1 & 7;
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
			return (bitvector1 & 0xFF8) / 8u;
		}
		set
		{
			bitvector1 = ((value * 8) | bitvector1);
		}
	}

	public uint damage
	{
		get
		{
			return (bitvector1 & 0xFFFF000) / 4096u;
		}
		set
		{
			bitvector1 = ((value * 4096) | bitvector1);
		}
	}

	public uint hitpart
	{
		get
		{
			return (bitvector1 & 0x70000000) / 268435456u;
		}
		set
		{
			bitvector1 = ((value * 268435456) | bitvector1);
		}
	}

	public uint lucky
	{
		get
		{
			return (uint)((int)bitvector1 & -2147483648) / 2147483648u;
		}
		set
		{
			bitvector1 = (uint)(((int)value * -2147483648) | (int)bitvector1);
		}
	}
}
