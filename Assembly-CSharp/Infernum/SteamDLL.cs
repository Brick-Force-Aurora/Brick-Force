using System;
using System.Runtime.InteropServices;

namespace Infernum
{
	public class SteamDLL
	{
		[DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
		public static extern void SteamAPI_Shutdown();

		[DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool SteamAPI_IsSteamRunning();

		[DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool SteamAPI_RestartAppIfNecessary(uint unOwnAppID);

		[DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
		private static extern void SteamAPI_WriteMiniDump(uint uStructuredExceptionCode, IntPtr pvExceptionInfo, uint uBuildID);

		[DllImport("steam_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private static extern void SteamAPI_SetMiniDumpComment(string pchMsg);

		[DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr SteamClient();

		[DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool SteamAPI_Init();

		[DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr SteamUser();

		[DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr SteamFriends();

		[DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr SteamUtils();

		[DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr SteamMatchMaking();

		[DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr SteamUserStats();

		[DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr SteamApps();

		[DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr SteamNetworking();

		[DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr SteamMatchmakingServers();

		[DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr SteamRemoteStorage();

		[DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr SteamScreenshots();

		[DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr SteamHTTP();

		[DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr SteamUnifiedMessages();
	}
}
