using UnityEngine;

public class HitBrickPacket
{
	public FirePacket firePacket;

	public bool isBullet;

	public int brickseq;

	public Vector3 hitpoint;

	public Vector3 hitnml;

	public bool destructable;

	public byte layer;

	public ushort damage;

	public HitBrickPacket(FirePacket _firePacket, bool _isBullet, int _brickseq, Vector3 _hitpoint, Vector3 _hitnml, bool _destructable, int _layer, int _damage)
	{
		firePacket = _firePacket;
		isBullet = _isBullet;
		brickseq = _brickseq;
		hitpoint = _hitpoint;
		hitnml = _hitnml;
		destructable = _destructable;
		layer = (byte)_layer;
		damage = (ushort)_damage;
	}
}
