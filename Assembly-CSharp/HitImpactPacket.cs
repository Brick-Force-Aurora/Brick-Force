using UnityEngine;

public class HitImpactPacket
{
	public FirePacket firePacket;

	public byte layer;

	public Vector3 hitpoint;

	public Vector3 hitnml;

	public HitImpactPacket(FirePacket _firePacket, int _layer, Vector3 _hitpoint, Vector3 _hitnml)
	{
		firePacket = _firePacket;
		layer = (byte)_layer;
		hitpoint = _hitpoint;
		hitnml = _hitnml;
	}
}
