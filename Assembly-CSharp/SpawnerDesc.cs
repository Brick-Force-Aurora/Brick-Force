using UnityEngine;

public class SpawnerDesc
{
	public int sequence;

	public Vector3 position;

	public byte rotation;

	public SpawnerDesc(int seq, Vector3 pos, byte rot)
	{
		sequence = seq;
		position = pos;
		rotation = rot;
	}
}
