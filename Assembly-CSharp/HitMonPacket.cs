using UnityEngine;

public class HitMonPacket
{
	public FirePacket firePacket;

	public ushort hitMon;

	public ushort damage;

	public float rigidity;

	public Vector3 hitpoint;

	public HitMonPacket(FirePacket _firePacket, int _hitMon, int _damage, float _rigidity, Vector3 _hitpoint)
	{
		firePacket = _firePacket;
		hitMon = (ushort)_hitMon;
		damage = (ushort)_damage;
		rigidity = _rigidity;
		hitpoint = _hitpoint;
	}
}
