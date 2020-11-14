using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class WindowSystemManager : MonoBehaviour
{
	public struct RECT
	{
		public int left;

		public int top;

		public int right;

		public int bottom;
	}

	private const long WS_DLGFRAME = 4194304L;

	private const long WS_BORDER = 8388608L;

	private const long WS_MINIMIZEBOX = 131072L;

	private const long WS_SYSMENU = 524288L;

	private const uint SWP_NOSIZE = 1u;

	private const uint SWP_NOMOVE = 2u;

	private const uint SWP_FRAMECHANGED = 32u;

	private const uint SWP_NOZORDER = 4u;

	private const int SM_CXVIRTUALSCREEN = 78;

	private const int SM_CYVIRTUALSCREEN = 79;

	private bool isResize;

	private static WindowSystemManager _instance;

	private bool dragonWay = true;

	private Vector2 pos = Vector2.zero;

	private int windowDiffX;

	private int windowDiffY;

	private int windowPosX;

	private int windowPosY;

	public static WindowSystemManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (UnityEngine.Object.FindObjectOfType(typeof(WindowSystemManager)) as WindowSystemManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the VersionTextureManager Instance");
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
	}

	public void HideWindowBorderAndTitle()
	{
		if (!BuildOption.Instance.Props.isDuplicateExcuteAble && DoesWindowHaveBorderAndTitle())
		{
			IntPtr intPtr = FindWindow(null, "BrickForce");
			long windowLong = GetWindowLong(intPtr, -16);
			windowLong &= -4194305;
			windowLong &= -8388609;
			SetWindowLong(intPtr, -16, windowLong);
			RECT lpRect = default(RECT);
			lpRect.left = 0;
			lpRect.top = 0;
			lpRect.right = PlayerPrefs.GetInt("BfScreenWidth", Screen.currentResolution.width);
			lpRect.bottom = PlayerPrefs.GetInt("BfScreenHeight", Screen.currentResolution.height);
			AdjustWindowRect(ref lpRect, windowLong, bMenu: false);
			SetWindowPos(intPtr, 0, 0, 0, lpRect.right - lpRect.left, lpRect.bottom - lpRect.top, 6u);
		}
	}

	public void ShowWindowBorderAndTitle()
	{
		if (!DoesWindowHaveBorderAndTitle())
		{
			IntPtr intPtr = FindWindow(null, "BrickForce");
			long windowLong = GetWindowLong(intPtr, -16);
			windowLong |= 0x400000;
			windowLong |= 0x800000;
			windowLong |= 0x20000;
			windowLong |= 0x80000;
			SetWindowLong(intPtr, -16, windowLong);
			RECT lpRect = default(RECT);
			lpRect.left = 0;
			lpRect.top = 0;
			lpRect.right = PlayerPrefs.GetInt("BfScreenWidth", Screen.currentResolution.width);
			lpRect.bottom = PlayerPrefs.GetInt("BfScreenHeight", Screen.currentResolution.height);
			AdjustWindowRect(ref lpRect, windowLong, bMenu: false);
			SetWindowPos(intPtr, 0, 0, 0, lpRect.right - lpRect.left, lpRect.bottom - lpRect.top, 6u);
		}
	}

	private void Update()
	{
		if (dragonWay)
		{
			if (!Screen.fullScreen && !BuildOption.Instance.Props.isWebPlayer)
			{
				if (Room.IsPlayingScene())
				{
					HideWindowBorderAndTitle();
				}
				else
				{
					ShowWindowBorderAndTitle();
				}
			}
		}
		else if (Room.IsPlayingScene())
		{
			HideWindowTitle();
		}
		else if (isResize)
		{
			ReSize();
		}
		else
		{
			ShowWindowTitle();
		}
	}

	[DllImport("user32.dll")]
	public static extern long GetWindowLong(IntPtr hwnd, int index);

	[DllImport("user32.dll")]
	public static extern long SetWindowLong(IntPtr hwnd, int index, long newLong);

	[DllImport("user32.dll")]
	public static extern IntPtr FindWindow(string className, string windowName);

	[DllImport("user32.dll")]
	public static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

	[DllImport("user32.dll")]
	public static extern int GetClientRect(IntPtr hwnd, ref RECT lpRect);

	[DllImport("user32.dll")]
	public static extern int GetWindowRect(IntPtr hwnd, ref RECT lpRect);

	[DllImport("user32.dll")]
	public static extern int AdjustWindowRect(ref RECT lpRect, long dwStyle, bool bMenu);

	public void HideWindowTitle()
	{
		if (!Screen.fullScreen && !BuildOption.Instance.Props.isDuplicateExcuteAble && !BuildOption.Instance.Props.isWebPlayer && DoesWindowHaveBorderAndTitle())
		{
			IntPtr intPtr = FindWindow(null, "BrickForce");
			long windowLong = GetWindowLong(intPtr, -16);
			windowLong &= -4194305;
			windowLong &= -8388609;
			RECT lpRect = default(RECT);
			RECT lpRect2 = default(RECT);
			GetClientRect(intPtr, ref lpRect);
			GetWindowRect(intPtr, ref lpRect2);
			windowDiffX = lpRect2.right - lpRect2.left - lpRect.right;
			windowDiffY = lpRect2.bottom - lpRect2.top - lpRect.bottom;
			windowPosX = lpRect2.left;
			windowPosY = lpRect2.top;
			SetWindowLong(intPtr, -16, windowLong);
			SetWindowPos(intPtr, 0, lpRect2.left + windowDiffX, lpRect2.top + windowDiffY, lpRect.right, lpRect.bottom, 32u);
		}
	}

	public void ShowWindowTitle()
	{
		if (!Screen.fullScreen && !BuildOption.Instance.Props.isWebPlayer && !DoesWindowHaveBorderAndTitle())
		{
			IntPtr intPtr = FindWindow(null, "BrickForce");
			long windowLong = GetWindowLong(intPtr, -16);
			windowLong |= 0x400000;
			windowLong |= 0x800000;
			windowLong |= 0x20000;
			windowLong |= 0x80000;
			RECT lpRect = default(RECT);
			RECT lpRect2 = default(RECT);
			GetClientRect(intPtr, ref lpRect);
			GetWindowRect(intPtr, ref lpRect2);
			SetWindowLong(intPtr, -16, windowLong);
			if (windowDiffX == 0 && windowDiffY == 0)
			{
				SetWindowPos(intPtr, 0, windowPosX, windowPosY, lpRect.right + windowDiffX, lpRect.bottom + windowDiffY, 34u);
			}
			else
			{
				SetWindowPos(intPtr, 0, windowPosX, windowPosY, lpRect.right + windowDiffX, lpRect.bottom + windowDiffY, 32u);
			}
			if (windowDiffX == 0 && windowDiffY == 0)
			{
				isResize = true;
			}
		}
	}

	private void ReSize()
	{
		IntPtr intPtr = FindWindow(null, "BrickForce");
		RECT lpRect = default(RECT);
		RECT lpRect2 = default(RECT);
		GetClientRect(intPtr, ref lpRect);
		GetWindowRect(intPtr, ref lpRect2);
		windowDiffX = lpRect2.right - lpRect2.left - lpRect.right;
		windowDiffY = lpRect2.bottom - lpRect2.top - lpRect.bottom;
		windowPosX = lpRect2.left;
		windowPosY = lpRect2.top;
		SetWindowPos(intPtr, 0, windowPosX - windowDiffX, windowPosY - windowDiffY, lpRect.right + windowDiffX * 2, lpRect.bottom + windowDiffY * 2, 32u);
		isResize = false;
	}

	private bool DoesWindowHaveBorderAndTitle()
	{
		IntPtr hwnd = FindWindow(null, "BrickForce");
		long windowLong = GetWindowLong(hwnd, -16);
		return 0 != (windowLong & 0x800000);
	}

	[DllImport("user32.dll")]
	public static extern int GetSystemMetrics(int index);
}
