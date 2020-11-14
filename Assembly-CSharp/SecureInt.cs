using UnityEngine;

public struct SecureInt
{
	private int key;

	private int webValue;

	public int Get()
	{
		if (BuildOption.IsWindowsPlayerOrEditor())
		{
			return (int)NmSecure.getnvl(key);
		}
		int num = key % 2 + 1;
		return (webValue >> num) / key;
	}

	public void Set(int value)
	{
		if (BuildOption.IsWindowsPlayerOrEditor())
		{
			NmSecure.setnvl(key, value);
		}
		else
		{
			int num = key % 2 + 1;
			value *= key;
			webValue = value << num;
		}
	}

	public void Add(int value)
	{
		if (BuildOption.IsWindowsPlayerOrEditor())
		{
			NmSecure.nvlad(key, value);
		}
		else
		{
			Set(Get() + value);
		}
	}

	public void Minus(int value)
	{
		if (BuildOption.IsWindowsPlayerOrEditor())
		{
			NmSecure.nvlsu(key, value);
		}
		else
		{
			Set(Get() - value);
		}
	}

	public void Init(int value)
	{
		if (BuildOption.IsWindowsPlayerOrEditor())
		{
			key = NmSecure.ctsvar(3);
		}
		else
		{
			key = Time.frameCount % 100 + Random.Range(1, 99);
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
		int value = Get();
		Release();
		Init(value);
	}

	public void Reset(int value)
	{
		Release();
		Init(value);
	}
}
