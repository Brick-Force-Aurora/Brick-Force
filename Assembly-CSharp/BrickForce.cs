using System;
using System.Diagnostics;
using UnityEngine;

public static class BrickForce
{
	public static void Restart()
	{
		try
		{
			ProcessStartInfo processStartInfo = new ProcessStartInfo();
			if (Environment.OSVersion.Version.Major >= 6)
			{
				processStartInfo.Verb = "runas";
			}
			processStartInfo.CreateNoWindow = false;
			processStartInfo.FileName = "BrickForce.exe";
			processStartInfo.UseShellExecute = false;
			processStartInfo.RedirectStandardError = false;
			processStartInfo.RedirectStandardInput = false;
			processStartInfo.RedirectStandardOutput = false;
			Process.Start(processStartInfo);
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogError(ex.Message.ToString());
		}
	}
}
