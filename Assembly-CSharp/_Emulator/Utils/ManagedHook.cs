using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace _Emulator
{
    class ManagedHook
    {
        private const uint HOOK_SIZE_X64 = 12;
        private const uint HOOK_SIZE_X86 = 7;
        private byte[] original;

        public MethodInfo OriginalMethod { get; private set; }
        public MethodInfo HookMethod { get; private set; }


        public ManagedHook()
        {
            original = null;
            OriginalMethod = HookMethod = null;
        }

        public ManagedHook(MethodInfo orig, MethodInfo hook)
        {
            original = null;
            Init(orig, hook);
        }

        public void Init(MethodInfo orig, MethodInfo hook)
        {
            if (orig == null || hook == null)
                throw new ArgumentException("Both original and hook need to be valid methods");

            RuntimeHelpers.PrepareMethod(orig.MethodHandle);
            RuntimeHelpers.PrepareMethod(hook.MethodHandle);

            OriginalMethod = orig;
            HookMethod = hook;
        }

        public void ApplyHook()
        {
            if (OriginalMethod == null || HookMethod == null)
                throw new ArgumentException("Hook has to be properly Init'd before use");

            if (original != null)
                return;

            IntPtr funcFrom = OriginalMethod.MethodHandle.GetFunctionPointer();
            IntPtr funcTo = HookMethod.MethodHandle.GetFunctionPointer();
            uint oldProt;
            if (IntPtr.Size == 8)
            {
                original = new byte[HOOK_SIZE_X64];

                Import.VirtualProtect(funcFrom, HOOK_SIZE_X64, 0x40, out oldProt);
                unsafe
                {
                    byte* ptr = (byte*)funcFrom;

                    for (int i = 0; i < HOOK_SIZE_X64; ++i)
                    {
                        original[i] = ptr[i];
                    }

                    *(ptr) = 0x48;
                    *(ptr + 0x1) = 0xB8;
                    *(IntPtr*)(ptr + 0x2) = funcTo;
                    *(ptr + 0xA) = 0xFF;
                    *(ptr + 0xB) = 0xE0;
                }
                Import.VirtualProtect(funcFrom, HOOK_SIZE_X64, oldProt, out oldProt);
            }
            else
            {
                original = new byte[HOOK_SIZE_X86];

                Import.VirtualProtect(funcFrom, HOOK_SIZE_X86, 0x40, out oldProt);
                unsafe
                {
                    byte* ptr = (byte*)funcFrom;

                    for (int i = 0; i < HOOK_SIZE_X86; ++i)
                    {
                        original[i] = ptr[i];
                    }

                    *(ptr) = 0xB8;
                    *(IntPtr*)(ptr + 0x1) = funcTo;
                    *(ptr + 0x5) = 0xFF;
                    *(ptr + 0x6) = 0xE0;
                }
                Import.VirtualProtect(funcFrom, HOOK_SIZE_X86, oldProt, out oldProt);

            }
        }

        public void Unhook()
        {
            if (original == null)
                return;

            uint oldProt;
            uint codeSize = (uint)original.Length;
            IntPtr origAddr = OriginalMethod.MethodHandle.GetFunctionPointer();
            Import.VirtualProtect(origAddr, codeSize, 0x40, out oldProt);
            unsafe
            {
                byte* ptr = (byte*)origAddr;
                for (var i = 0; i < codeSize; ++i)
                {
                    ptr[i] = original[i];
                }
            }
            Import.VirtualProtect(origAddr, codeSize, 0x40, out oldProt);

            original = null;
        }

        public void CallOriginal(object _this, object[] _params)
        {
            Unhook();
            OriginalMethod.Invoke(_this, _params);
            ApplyHook();
        }
    }
}
