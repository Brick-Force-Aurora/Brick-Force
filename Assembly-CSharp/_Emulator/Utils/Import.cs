using System;
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
    }
}
