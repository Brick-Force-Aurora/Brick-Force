using System;
using UnityEngine;

[Serializable]
public class KeyDef
{
	public enum CATEGORY
	{
		COMMON,
		SHOOTER_MODE,
		BUILD_MODE,
		WEAPON_CHANGE,
		BUNGEE_MODE
	}

	public string name;

	public CATEGORY category;

	public KeyCode defaultInputKey;

	public KeyCode altDefaultInputKey;
}
