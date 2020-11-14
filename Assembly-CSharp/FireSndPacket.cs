using System;

[Serializable]
public class FireSndPacket
{
	public int shooter;

	public byte slot;

	public FireSndPacket(int _shooter, int _slot)
	{
		shooter = _shooter;
		slot = (byte)_slot;
	}
}
