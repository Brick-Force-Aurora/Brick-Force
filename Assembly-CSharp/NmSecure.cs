using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public class NmSecure : MonoBehaviour
{
	public delegate void CallbackFalsificationNotify();

	private static NmSecure _instance;

	public static NmSecure Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = (Object.FindObjectOfType(typeof(NmSecure)) as NmSecure);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get XTrap Instance");
				}
			}
			return _instance;
		}
	}

	[DllImport("nmsv", CallingConvention = CallingConvention.Cdecl)]
	public static extern int setntcb(CallbackFalsificationNotify NotifyCallback);

	[DllImport("nmsv", CallingConvention = CallingConvention.Cdecl)]
	public static extern int ctsvar(int type);

	[DllImport("nmsv", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool setnvl(int hHandle, long value);

	[DllImport("nmsv", CallingConvention = CallingConvention.Cdecl)]
	public static extern long getnvl(int hHandle);

	[DllImport("nmsv", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool nvlad(int hHandle, long value);

	[DllImport("nmsv", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool nvlsu(int hHandle, long value);

	[DllImport("nmsv", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool setfvl(int hHandle, float value);

	[DllImport("nmsv", CallingConvention = CallingConvention.Cdecl)]
	public static extern float getfvl(int hHandle);

	[DllImport("nmsv", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool fvlad(int hHandle, float value);

	[DllImport("nmsv", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool fvlsu(int hHandle, float value);

	[DllImport("nmsv", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool setdvl(int hHandle, double value);

	[DllImport("nmsv", CallingConvention = CallingConvention.Cdecl)]
	public static extern double getdvl(int hHandle);

	[DllImport("nmsv", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool dvlad(int hHandle, double value);

	[DllImport("nmsv", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool dvlsu(int hHandle, double value);

	[DllImport("nmsv", CallingConvention = CallingConvention.Cdecl)]
	public static extern void rlsvar(int hHandle);

	[DllImport("nmsv", CallingConvention = CallingConvention.Cdecl)]
	public static extern void rlsvarall();

	private static void FalsificationNotifyCallback()
	{
		Debug.Log("NmSecure data is falsified");
		BuildOption.Instance.HardExit();
	}

	private void Awake()
	{
		if (BuildOption.IsWindowsPlayerOrEditor())
		{
			setntcb(FalsificationNotifyCallback);
		}
	}

	private static void Main(string[] args)
	{
		setntcb(FalsificationNotifyCallback);
		int hHandle = ctsvar(1);
		int hHandle2 = ctsvar(2);
		int hHandle3 = ctsvar(3);
		int hHandle4 = ctsvar(4);
		int hHandle5 = ctsvar(5);
		int hHandle6 = ctsvar(6);
		int hHandle7 = ctsvar(7);
		int hHandle8 = ctsvar(8);
		int hHandle9 = ctsvar(9);
		int hHandle10 = ctsvar(10);
		int hHandle11 = ctsvar(11);
		int num = 0;
		setnvl(hHandle, num);
		setnvl(hHandle2, num);
		setnvl(hHandle3, num);
		setnvl(hHandle4, num);
		setnvl(hHandle5, num);
		setfvl(hHandle6, 1000.1f);
		setdvl(hHandle7, 1000.1);
		Debug.Log("press enter or 'g'");
		int num2 = 0;
		num2++;
		nvlad(hHandle, 1L);
		nvlsu(hHandle2, 1L);
		nvlsu(hHandle3, 1L);
		nvlsu(hHandle4, 1L);
		nvlsu(hHandle5, 1L);
		fvlsu(hHandle6, 1.1f);
		dvlad(hHandle7, 1.1);
		nvlad(hHandle8, 1L);
		nvlad(hHandle9, 1L);
		nvlad(hHandle10, 1L);
		nvlad(hHandle11, 1L);
		Debug.Log("normal count \t\t= {0:d}" + num2);
		Debug.Log("secure byte count \t= {0:d}" + (int)getnvl(hHandle));
		Debug.Log("secure short count \t= {0:d}" + (int)getnvl(hHandle2));
		Debug.Log("secure short count \t= {0:d}" + (int)getnvl(hHandle3));
		Debug.Log("secure long count \t= {0:d}" + (int)getnvl(hHandle4));
		Debug.Log("secure int64 count \t= {0:d}" + getnvl(hHandle5));
		Debug.Log("secure float count \t= {0:f}" + getfvl(hHandle6));
		Debug.Log("secure double count \t= {0:f}" + getdvl(hHandle7));
		Debug.Log("secure ushort count \t= {0:d}" + (int)getnvl(hHandle8));
		Debug.Log("secure ushort count \t= {0:d}" + (int)getnvl(hHandle9));
		Debug.Log("secure ulong count \t= {0:d}" + (int)getnvl(hHandle10));
		Debug.Log("secure uint64 count \t= {0:d}" + getnvl(hHandle11));
		Debug.Log("\n");
		Thread.Sleep(50);
		rlsvarall();
	}
}
