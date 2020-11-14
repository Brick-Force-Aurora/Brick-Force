using UnityEngine;

public struct SecureFloat
{
	private int key;

	private float webValue;

	public float Get()
	{
		if (BuildOption.IsWindowsPlayerOrEditor())
		{
			return NmSecure.getfvl(key);
		}
		return webValue / (float)(key % 15 + 2);
	}

	public void Set(float value)
	{
		if (BuildOption.IsWindowsPlayerOrEditor())
		{
			NmSecure.setfvl(key, value);
		}
		else
		{
			webValue = value * (float)(key % 15 + 2);
		}
	}

	public void Add(float value)
	{
		if (BuildOption.IsWindowsPlayerOrEditor())
		{
			NmSecure.fvlad(key, value);
		}
		else
		{
			Set(Get() + value);
		}
	}

	public void Minus(float value)
	{
		if (BuildOption.IsWindowsPlayerOrEditor())
		{
			NmSecure.fvlsu(key, value);
		}
		else
		{
			Set(Get() - value);
		}
	}

	public void Init(float value)
	{
		if (BuildOption.IsWindowsPlayerOrEditor())
		{
			key = NmSecure.ctsvar(6);
		}
		else
		{
			key = Time.frameCount + Random.Range(1, 99999);
		}
		Set(value);
	}

	public void Release()
	{
		if (BuildOption.IsWindowsPlayerOrEditor())
		{
			NmSecure.rlsvar(key);
		}
	}

	public void Reset()
	{
		float value = Get();
		Release();
		Init(value);
	}

	public void Reset(float value)
	{
		Release();
		Init(value);
	}
}
