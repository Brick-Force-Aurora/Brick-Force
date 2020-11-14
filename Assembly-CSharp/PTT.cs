using UnityEngine;

internal class PTT
{
	public Weapon.BY weapon;

	public Vector3 pos;

	public float range;

	public PTT(Weapon.BY w, Vector3 p, float r)
	{
		weapon = w;
		pos = p;
		range = r;
	}
}
