public struct BF_PeerINITIATE
{
	public ushort bitvector1;

	public int cc
	{
		get
		{
			return bitvector1 & 0x7F;
		}
		set
		{
			bitvector1 = (ushort)(value | bitvector1);
		}
	}

	public int curWeaponType
	{
		get
		{
			return (int)((uint)(bitvector1 & 0x1F80) / 128u);
		}
		set
		{
			bitvector1 = (ushort)((value * 128) | bitvector1);
		}
	}

	public bool empty
	{
		get
		{
			uint num = (uint)(bitvector1 & 0x2000) / 8192u;
			return (num == 1) ? true : false;
		}
		set
		{
			uint num = value ? (num = 1u) : (num = 0u);
			bitvector1 = (ushort)((num * 8192) | bitvector1);
		}
	}

	public bool dead
	{
		get
		{
			uint num = (uint)(bitvector1 & 0x4000) / 16384u;
			return (num == 1) ? true : false;
		}
		set
		{
			uint num = value ? (num = 1u) : (num = 0u);
			bitvector1 = (ushort)((num * 16384) | bitvector1);
		}
	}

	public bool invisibility
	{
		get
		{
			uint num = (uint)(bitvector1 & 0x8000) / 32768u;
			return (num == 1) ? true : false;
		}
		set
		{
			uint num = value ? (num = 1u) : (num = 0u);
			bitvector1 = (ushort)((num * 32768) | bitvector1);
		}
	}
}
