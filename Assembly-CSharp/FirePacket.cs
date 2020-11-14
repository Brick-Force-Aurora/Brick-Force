using UnityEngine;

public class FirePacket
{
	public int shooter;

	public byte slot;

	public ushort usID;

	public Vector3 shootpos;

	public Vector3 shootdir;

	public FirePacket(int _shooter, int _slot, int _id, Vector3 p, Vector3 d)
	{
		shooter = _shooter;
		slot = (byte)_slot;
		usID = (ushort)_id;
		shootpos = p;
		shootdir = d;
	}
}
