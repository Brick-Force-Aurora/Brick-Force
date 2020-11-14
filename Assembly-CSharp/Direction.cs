using UnityEngine;

public class Direction
{
	public static float DotNormal(Vector3 a, Vector3 b)
	{
		a = a.normalized;
		b = b.normalized;
		return Vector3.Dot(a, b);
	}

	public static bool IsNotSideOnXZPlane(Vector3 pos, Transform reference)
	{
		Vector3 a = pos - reference.position;
		a.y = 0f;
		Vector3 b = reference.TransformDirection(Vector3.forward);
		b.y = 0f;
		float num = DotNormal(a, b);
		return (-1f <= num && num <= -0.85f) || (0.2f <= num && num <= 1f);
	}

	public static bool IsForward(Vector3 pos, Transform reference)
	{
		Vector3 a = pos - reference.position;
		Vector3 b = reference.TransformDirection(Vector3.forward);
		return DotNormal(a, b) > 0f;
	}
}
