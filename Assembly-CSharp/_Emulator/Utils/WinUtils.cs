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

        /// <summary>
        /// Checks if a memory range between address and address + size is readable.
        /// </summary>
        public static bool IsMemoryReadable(IntPtr address, IntPtr size)
        {
            MEMORY_BASIC_INFORMATION mbi;
            int result = Import.VirtualQuery(address, out mbi, (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION)));

            if (result == 0) return false;

            // The memory must be committed (allocated) to be readable
            if (mbi.State != Import.MEM_COMMIT) return false;

            // Out of bounds
            if (address.ToInt64() + size.ToInt64() > mbi.BaseAddress.ToInt64() + mbi.RegionSize.ToInt64()) return false;

            // Mask for all flags that allow reading
            const uint mask = Import.PAGE_READONLY | Import.PAGE_READWRITE | Import.PAGE_EXECUTE_READ | Import.PAGE_EXECUTE_READWRITE;

            return (mbi.Protect & mask) != 0;
        }

        public static unsafe IntPtr ReadPointer(IntPtr address, int offset)
        {
            if (address == IntPtr.Zero)
                return IntPtr.Zero;

            // Check if we are in a 64-bit process (8 bytes)
            if (IntPtr.Size == 8)
            {
                int displacement = *(int*)(address.ToInt64() + offset);
                return (IntPtr)(address.ToInt64() + offset + sizeof(int) + displacement);
            }

            return *(IntPtr*)(address.ToInt64() + offset);
        }

        public static List<int> PatternToBytes(string pattern)
        {
            var bytes = new List<int>();
            string[] parts = pattern.Split(' ');

            foreach (var part in parts)
            {
                if (part == "?" || part == "??")
                {
                    bytes.Add(-1); // Wildcard
                }
                else
                {
                    bytes.Add(Convert.ToInt32(part, 16));
                }
            }
            return bytes;
        }

        /// <summary>
        /// PE module byte matching pattern scan.
        /// </summary>
        public static unsafe IntPtr PatternScan(IntPtr hModule, string signature, int offset = 0, bool dereference = false)
        {
            if (hModule == IntPtr.Zero) return IntPtr.Zero;

            // Navigate PE Headers to find SizeOfImage
            byte* basePtr = (byte*)hModule;
            int e_lfanew = *(int*)(basePtr + 0x3C);
            // ntHeaders + 0x18 (OptionalHeader) + 0x38 (SizeOfImage)
            int sizeOfImage = *(int*)(basePtr + e_lfanew + 0x18 + 0x38);

            var patternBytes = PatternToBytes(signature);
            int patternLength = patternBytes.Count;

            for (int i = 0; i < sizeOfImage - patternLength; ++i)
            {
                bool found = true;
                for (int j = 0; j < patternLength; ++j)
                {
                    if (patternBytes[j] != -1 && basePtr[i + j] != (byte)patternBytes[j])
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    IntPtr foundAddress = (IntPtr)(basePtr + i);
                    return dereference ? ReadPointer(foundAddress, offset) : foundAddress;
                }
            }

            return IntPtr.Zero;
        }
    }
}
