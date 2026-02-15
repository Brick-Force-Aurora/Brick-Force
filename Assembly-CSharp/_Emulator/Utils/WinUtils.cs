using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace _Emulator
{
    public class WinUtils
    {
        public static IntPtr GetModuleContainingAdress(IntPtr address)
        {
            var hSnapshot = Import.CreateToolhelp32Snapshot(SnapshotFlags.Module, Import.GetCurrentProcessId());
            var hModule = IntPtr.Zero;

            if (hSnapshot != (IntPtr)(-1))
            {
                var moduleEntry = new MODULEENTRY32()
                {
                    dwSize = (uint)Marshal.SizeOf(typeof(MODULEENTRY32))
                };

                if (Import.Module32First(hSnapshot, ref moduleEntry))
                {
                    while (Import.Module32Next(hSnapshot, ref moduleEntry))
                    {
                        if (address.ToInt64() >= moduleEntry.modBaseAddr.ToInt64() && address.ToInt64() <= (moduleEntry.modBaseAddr.ToInt64() + moduleEntry.modBaseSize))
                        {
                            hModule = moduleEntry.hModule;
                            break;
                        }
                    }
                }

                Import.CloseHandle(hSnapshot);
            }

            return hModule;
        }

        public static string GetModuleNameFromHandle(IntPtr hModule)
        {
            StringBuilder fileName = new StringBuilder(255);
            Import.GetModuleBaseName(Import.GetCurrentProcess(), hModule, fileName, fileName.Capacity);
            return fileName.ToString();
        }

        public static string GetModuleFileNameFromHandle(IntPtr hModule)
        {
            StringBuilder fileName = new StringBuilder(255);
            Import.GetModuleFileName(hModule, fileName, fileName.Capacity);
            return fileName.ToString();
        }

        public static string GetModuleNameFromAddress(IntPtr address)
        {
            return GetModuleNameFromHandle(GetModuleContainingAdress(address));
        }

        public static string GetModuleFileNameFromAddress(IntPtr address)
        {
            return GetModuleFileNameFromHandle(GetModuleContainingAdress(address));
        }

        public static string GetModuleNameWithOffset(IntPtr address)
        {
            var hModule = GetModuleContainingAdress(address);
            return GetModuleNameFromHandle(hModule) + " + 0x" + (address.ToInt64() - hModule.ToInt64()).ToString("X");
        }

        public static string GetModuleFileNameWithOffset(IntPtr address)
        {
            var hModule = GetModuleContainingAdress(address);
            return GetModuleFileNameFromHandle(hModule) + " + 0x" + (address.ToInt64() - hModule.ToInt64()).ToString("X");
        }
    }
}
