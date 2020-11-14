using System;
using UnityEngine;

[Serializable]
public class SpeedHackProtector : MonoBehaviour
{
	private bool breakinto;

	private uint pvMov;

	public bool Breakinto
	{
		get
		{
			return breakinto;
		}
		set
		{
			breakinto = value;
		}
	}

	public uint PvMov
	{
		get
		{
			return pvMov;
		}
		set
		{
			pvMov = value;
		}
	}

	public void InitializePacketVariation()
	{
		pvMov = 0u;
		breakinto = false;
	}

	private void Start()
	{
	}
}
