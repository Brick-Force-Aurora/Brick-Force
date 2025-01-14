using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace _Emulator
{
    enum InputModeFlags : int
    {
        ENABLE_PROCESSED_INPUT = 0x1,
        ENABLE_LINE_INPUT = 0x2,
        ENABLE_ECHO_INPUT = 0x4,
        ENABLE_WINDOW_INPUT = 0x8,
        ENABLE_MOUSE_INPUT = 0x10,
        ENABLE_INSERT_MODE = 0x20,
        ENABLE_QUICK_EDIT_MODE = 0x40,
        ENABLE_EXTENDED_FLAGS = 0x80,
        ENABLE_AUTO_POSITION = 0x100,
        ENABLE_VIRTUAL_TERMINAL_INPUT = 0x200
    }
    enum OutputModeFlags : int
    {
        ENABLE_PROCESSED_OUTPUT = 0x1,
        ENABLE_WRAP_AT_EOL_OUTPUT = 0x2,
        ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x4,
        DISABLE_NEWLINE_AUTO_RETURN = 0x8,
        ENABLE_LVB_GRID_WORLDWIDE = 0x10
    }

    enum StdHandle : int
    {
        STD_INPUT_HANDLE = -10,
        STD_OUTPUT_HANDLE = -11,
        STD_ERROR_HANDLE = -12
    }

    class Import
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtect(IntPtr address, uint size, uint newProtect, out uint oldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(StdHandle nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void SetStdHandle(StdHandle nStdHandle, IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int AttachConsole(uint processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint GetCurrentProcessId();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void OutputDebugString(string lpOutputString);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string lpModuleName);

        [DllImport("d3d9helper.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern IntPtr GetD3D9Reset();

        [DllImport("d3d9helper.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern IntPtr GetD3D9Present();

        [DllImport("d3d9helper.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern IntPtr GetD3D9EndScene();

        [DllImport("d3d9helper.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern IntPtr GetProcessWindow();

        [DllImport("d3d9helper.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern IntPtr GetDeviceWindow(IntPtr device);

        public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            else
                return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        public delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(ImportTypes.Keys vKeys);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetExitCodeProcess(IntPtr hProcess, out uint lpExitCode);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetDpiForWindow(IntPtr hwnd);

        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igStyleColorsCustomDefault();

        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igStyleColorsCustom(Vector4 color);

        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igScaleAllSizesReset(float scale);
    }
}
