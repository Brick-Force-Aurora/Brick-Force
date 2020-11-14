using UnityEngine;

public class HitManPacket
{
	public FirePacket firePacket;

	public int hitMan;

	public byte hitPart;

	public Vector3 hitpoint;

	public Vector3 hitnml;

	public ushort damage;

	public float rigidity;

	public ushort weaponBy;

	public bool bLucky;

	public HitManPacket(FirePacket _firePacket, int _hitMan, int _hitPart, Vector3 _hitpoint, Vector3 _hitnml, int _damage, float _rigidity, int _weapnby, bool _lucky)
	{
		firePacket = _firePacket;
		hitMan = _hitMan;
		hitPart = (byte)_hitPart;
		hitpoint = _hitpoint;
		hitnml = _hitnml;
		damage = (ushort)_damage;
		rigidity = _rigidity;
		weaponBy = (ushort)_weapnby;
		bLucky = _lucky;
	}
}
