using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace wlic
{
	public class WXCS_IF
	{
		public static int CS2_POPT_PE = 1;

		public static int CS2_POPT_TEXT = 16;

		public static int CS2_POPT_RDATA = 256;

		public static int CS2_POPT_EDATA = 4096;

		public static int CS2_POPT_RSRC = 65536;

		public static int CS2_POPT_RELOC = 1048576;

		public static int CS2_POPT_E_V = 2;

		[DllImport("keel_xt.dll")]
		public static extern int L0(string pArgv, string pGamePath, int dwTimeout);

		[DllImport("keel_xt.dll")]
		public static extern int C0(string pArgv, string pXTrapPath);

		[DllImport("keel_xt.dll")]
		public static extern int C1();

		[DllImport("keel_xt.dll")]
		public static extern int C2(int lPeriod);

		[DllImport("keel_xt.dll")]
		public static extern int C4(string pUserID, string pServerName, string pCharacterName, string pCharacterClass, int lReserve, Socket Sock);

		[DllImport("keel_xt.dll")]
		public static extern int S0(byte[] packetdata_in, byte[] packetdata_out, int dwMethod);

		public static void XTrap_L_Patch(string pArgv, string pGamePath, int dwTimeout)
		{
			//L0(pArgv, pGamePath, dwTimeout);
		}

		public static void XTrap_C_Start(string lpArgv, string lpXTrapPath)
		{
			//C0(lpArgv, lpXTrapPath);
		}

		public static void XTrap_C_KeepAlive()
		{
			//C1();
		}

		public static void XTrap_C_CallbackAlive(int Period)
		{
			//C2(Period);
		}

		public static void XTrap_C_SetUserInfoEx(string lpUserID, string lpServerName, string lpCharacterName, string lpCharacterClass, int dwReserve, Socket Sock)
		{
			//C4(lpUserID, lpServerName, lpCharacterName, lpCharacterClass, dwReserve, Sock);
		}

		public static int XTrap_CS_Step2(byte[] lpBufPackData_IN, byte[] lpBufPackData_OUT, int dwMethod)
		{
            //return S0(lpBufPackData_IN, lpBufPackData_OUT, dwMethod);
            return 0;
		}
	}
}
