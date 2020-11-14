using System;

[Serializable]
public class Magazine
{
	public int max = 30;

	private SecureInt curSecure;

	private int tmp;

	private int curr;

	private int cur
	{
		get
		{
			return curSecure.Get();
		}
		set
		{
			curSecure.Set(value);
		}
	}

	public int Max
	{
		get
		{
			return max;
		}
		set
		{
			max = value;
		}
	}

	public int Cur
	{
		get
		{
			curr = NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.MAIN_MAGAZINE, cur);
			if (curr > max || cur != tmp)
			{
				BuildOption.Instance.HardExit();
			}
			return cur;
		}
		set
		{
			cur = value;
		}
	}

	public bool Empty => cur <= 0;

	public float Ratio
	{
		get
		{
			if ((float)max <= 0f)
			{
				return 0f;
			}
			return (float)NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.MAIN_MAGAZINE, cur) / (float)max;
		}
	}

	public Magazine()
	{
		curSecure.Init(0);
	}

	~Magazine()
	{
		curSecure.Release();
	}

	public bool CanFire()
	{
		return cur > 0;
	}

	public bool CanReload()
	{
		return NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.MAIN_MAGAZINE, cur) < max;
	}

	public void Reset()
	{
		curSecure.Reset();
		cur = 0;
		tmp = cur;
	}

	public bool addAmmo(int ammo)
	{
		int num = NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.MAIN_MAGAZINE, cur);
		if (num == max)
		{
			return false;
		}
		num += ammo;
		if (num > max)
		{
			num = max;
		}
		cur = NoCheat.Instance.HideVal(NoCheat.WATCH_DOG.MAIN_MAGAZINE, num);
		tmp = cur;
		return true;
	}

	public int Reload(int ammo)
	{
		int num = NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.MAIN_MAGAZINE, cur);
		int num2 = max - num;
		if (ammo < num2)
		{
			num += ammo;
			ammo = 0;
		}
		else
		{
			num += num2;
			ammo -= num2;
		}
		cur = NoCheat.Instance.HideVal(NoCheat.WATCH_DOG.MAIN_MAGAZINE, num);
		tmp = cur;
		return ammo;
	}

	public bool Fire()
	{
		if (MyInfoManager.Instance.IsGM && MyInfoManager.Instance.GodMode)
		{
			return true;
		}
		if (cur <= 0)
		{
			return false;
		}
		int val = NoCheat.Instance.UnhideVal(NoCheat.WATCH_DOG.MAIN_MAGAZINE, cur) - 1;
		cur = NoCheat.Instance.HideVal(NoCheat.WATCH_DOG.MAIN_MAGAZINE, val);
		tmp = NoCheat.Instance.HideVal(NoCheat.WATCH_DOG.MAIN_MAGAZINE, val);
		return true;
	}
}
