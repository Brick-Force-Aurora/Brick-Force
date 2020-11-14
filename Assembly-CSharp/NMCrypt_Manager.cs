using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class NMCrypt_Manager : MonoBehaviour
{
	private static NMCrypt_Manager _instance;

	public byte[] cookie = new byte[2048];

	public static NMCrypt_Manager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = (UnityEngine.Object.FindObjectOfType(typeof(NMCrypt_Manager)) as NMCrypt_Manager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get NMCrypt_Manager Instance");
				}
			}
			return _instance;
		}
	}

	[DllImport("NMCrypt")]
	public static extern int Getint5();

	[DllImport("NMCrypt")]
	public static extern void GetCookie(byte[] cookie_out);

	[DllImport("NMCrypt")]
	public static extern void GetCookieKey(byte[] cookie_out, byte[] key_in);

	[DllImport("NMCrypt")]
	public static extern void GetClipBoard(byte[] key_in);

	[DllImport("NMCrypt")]
	public static extern void SetClipBoard(byte[] key_in);

	private void Awake()
	{
		if (BuildOption.Instance.IsNetmarble)
		{
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			if (commandLineArgs.Length >= 2)
			{
				Encoding aSCII = Encoding.ASCII;
				byte[] bytes = aSCII.GetBytes(commandLineArgs[1]);
				GetCookieKey(cookie, bytes);
			}
		}
		UnityEngine.Object.DontDestroyOnLoad(this);
	}
}
