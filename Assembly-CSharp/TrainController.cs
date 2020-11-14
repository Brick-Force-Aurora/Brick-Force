using System;
using UnityEngine;

[Serializable]
public class TrainController
{
	public int shooter = -1;

	public int seq = -1;

	private Vector3 start;

	private Quaternion rot;

	public GameObject train;

	public void setInit(Vector3 p, Quaternion r)
	{
		start = p;
		rot = r;
	}

	public void regen()
	{
		train.transform.position = start;
		train.transform.rotation = rot;
	}
}
