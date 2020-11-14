using UnityEngine;

public class Rot
{
	public static Quaternion ToQuaternion(byte rot)
	{
		switch (rot)
		{
		case 0:
			return Quaternion.Euler(0f, 0f, 0f);
		case 1:
			return Quaternion.Euler(0f, 90f, 0f);
		case 2:
			return Quaternion.Euler(0f, 180f, 0f);
		case 3:
			return Quaternion.Euler(0f, 270f, 0f);
		default:
			return Quaternion.Euler(0f, 0f, 0f);
		}
	}
}
