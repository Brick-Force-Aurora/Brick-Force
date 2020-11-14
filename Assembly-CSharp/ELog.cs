using System;
using System.Diagnostics;
using UnityEngine;

public class ELog
{
	private static Stopwatch stopwatch = new Stopwatch();

	public static void Log()
	{
		UnityEngine.Debug.Log(string.Empty + StackTraceUtility.ExtractStackTrace() + " " + Time.frameCount);
	}

	public static void Log(object arg0)
	{
		UnityEngine.Debug.Log(string.Empty + arg0 + " " + Time.frameCount);
	}

	public static void Log(object arg0, object arg1)
	{
		UnityEngine.Debug.Log(string.Empty + arg0 + "  " + arg1 + "  " + Time.frameCount);
	}

	public static void Log(object arg0, object arg1, object arg2)
	{
		UnityEngine.Debug.Log(string.Empty + arg0 + "  " + arg1 + "  " + arg2 + "  " + Time.frameCount);
	}

	public static void Log(object arg0, object arg1, object arg2, object arg3)
	{
		UnityEngine.Debug.Log(string.Empty + arg0 + "  " + arg1 + "  " + arg2 + "  " + arg3 + "  " + Time.frameCount);
	}

	public static void Log(object arg0, object arg1, object arg2, object arg3, object arg4)
	{
		UnityEngine.Debug.Log(string.Empty + arg0 + "  " + arg1 + "  " + arg2 + "  " + arg3 + "  " + arg4 + "  " + Time.frameCount);
	}

	public static void Log(object arg0, object arg1, object arg2, object arg3, object arg4, object arg5)
	{
		UnityEngine.Debug.Log(string.Empty + arg0 + "  " + arg1 + "  " + arg2 + "  " + arg3 + "  " + arg4 + "  " + arg5 + "  " + Time.frameCount);
	}

	public static void StackTrace()
	{
		UnityEngine.Debug.Log(StackTraceUtility.ExtractStackTrace() + Time.frameCount);
	}

	public static void TimerStart()
	{
		stopwatch.Reset();
		stopwatch.Start();
	}

	public static void TimerCheck()
	{
		Log("Timer Check", stopwatch.ElapsedTicks);
	}

	public static void TimerCheck(string name)
	{
		Log(name, "Timer Check", stopwatch.ElapsedTicks);
	}

	public static void GC()
	{
		Resources.UnloadUnusedAssets();
		System.GC.Collect();
		System.GC.WaitForPendingFinalizers();
	}
}
