using UnityEngine;

public class MissleInfo
{
	public GameObject obj;

	public int uniq;

	public Vector3 prepos = Vector3.zero;

	private float elapsed;

	private float elapsedP2P;

	public float Elapsed
	{
		get
		{
			return elapsed;
		}
		set
		{
			elapsed = value;
		}
	}

	public float ElapsedP2P
	{
		get
		{
			return elapsedP2P;
		}
		set
		{
			elapsedP2P = value;
		}
	}
}
