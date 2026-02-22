using System;
using System.Reflection;
using System.Runtime.InteropServices;
using EasyHook;
using Debug = UnityEngine.Debug;
using System.Runtime.CompilerServices;

namespace _Emulator
{
    class HooksNative
    {
        static MethodInfo oD3D9PresentHookInfo = typeof(HooksNative).GetMethod("hD3D9Present", BindingFlags.Public | BindingFlags.Static);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate int D3D9PresentDelegate(IntPtr _this, IntPtr src, IntPtr dst, int wndOverride, IntPtr dirtyRegion);
        static D3D9PresentDelegate dD3D9Present;
        static LocalHook D3D9PresentHook;
        // First 5 bytes are wildcards to account for existing hooks.
        const string D3D9PresentPattern = "? ? ? ? ? 83 E4 F8 51 51 56 8B 75 ? 8B CE F7 D9 57 1B C9 8D 46 ? 23 C8 6A 00 51 8D 4C 24 ? E8 ? ? ? ? F7 46 ? ? ? ? ? 74 ? BF 6C 08 76 88 EB ? 6A 00";

        static MethodInfo oD3D9ResetHookInfo = typeof(HooksNative).GetMethod("hD3D9Reset", BindingFlags.Public | BindingFlags.Static);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate int D3D9ResetDelegate(IntPtr _this, IntPtr param);
        static D3D9ResetDelegate dD3D9Reset;
        static LocalHook D3D9ResetHook;
        // First 5 bytes are wildcards to account for existing hooks.
        const string D3D9ResetPattern = "? ? ? ? ? 83 E4 F8 81 EC 44 01 00 00 A1 ? ? ? ? 33 C4 89 84 24 ? ? ? ? 53";

        private static bool initialized = false;


        public static int hD3D9Present(IntPtr _this, IntPtr src, IntPtr dst, int wndOverride, IntPtr dirtyRegion)
        {
            ImGuiBackend.instance.OnPresent(_this);
            return (int)dD3D9Present.DynamicInvoke(new object[] { _this, src, dst, wndOverride, dirtyRegion });
        }

        public static int hD3D9Reset(IntPtr _this, IntPtr param)
        {
            ImGuiBackend.instance.OnReset(_this);
            var result = (int)dD3D9Reset.DynamicInvoke(new object[] { _this, param });
            ImGuiBackend.instance.AfterReset(_this);
            return result;
        }

        /// <summary>
        /// Find existing instructions like jmps to detect existing hooks on an instruction.
        /// We can usually assume that the code at the jmp address matches the signature of the hook function.
        /// This allows us to follow the jmp and hook the existing hook instead to deal with unreliable overlays like Afterburner/RivaTuner.
        /// This flow is potentially unreliable due to the assumptions it makes about the instruction layout and should be optional.
        /// </summary>
        public static unsafe IntPtr ResolveExistingHooks(string name, IntPtr addr)
        {
            unsafe
            {
                if (WinUtils.IsMemoryReadable(addr, (IntPtr)6))
                {
                    var pBytes = (byte*)addr.ToPointer();
                    if (*pBytes == 0xE9)
                    {
                        if (Config.instance.followHooks)
                        {
                            Debug.Log(name + ": Already hooked (0xE9), following hook.");
                            return (IntPtr)(((IntPtr)BitConverter.ToUInt32(new byte[] { pBytes[1], pBytes[2], pBytes[3], pBytes[4] }, 0)).ToInt64() + ((IntPtr)(&pBytes[5])).ToInt64());
                        }

                        else
                        {
                            Debug.Log(name + ": Already hooked (0xE9), not following hook.");
                        }
                    }
                }

                else
                {
                    Debug.Log(name + ": Memory wasn't readable when checking for existing hooks.");
                }
            }

            return addr;
        }

        public static void Initialize()
        {
            var d3d9helper = Import.LoadLibrary("d3d9helper.dll");
            if (d3d9helper == IntPtr.Zero)
            {
                Debug.LogError("d3d9helper Error: " + Marshal.GetLastWin32Error().ToString("X"));
            }

            else
            {
                var hD3D9 = Import.GetModuleHandle("d3d9.dll");

                var pD3D9Present = IntPtr.Zero;

                if (Config.instance.useD3DPatternScan)
                {
                    // Attempt to hook CBaseDevice::Present by byte signature.
                    // This method may not work if the function is already hooked or the pattern isn't present in this version of d3d9.dll.
                    Debug.Log("D3D9 Present: Attemping pattern scan.");
                    pD3D9Present = WinUtils.PatternScan(hD3D9, D3D9PresentPattern);
                    if (pD3D9Present == IntPtr.Zero)
                        Debug.Log("D3D9 Present: Pattern not found.");
                }

                if (pD3D9Present == IntPtr.Zero)
                {
                    // Attempt to hook CBaseDevice::Present by dummy device vtable.
                    // This method may not work if the vtable pointer has been swapped.
                    Debug.Log("D3D9 Present: Attempting Vtable lookup.");
                    pD3D9Present = Import.GetD3D9Present();
                }
                if (pD3D9Present != IntPtr.Zero)
                {
                    pD3D9Present = ResolveExistingHooks("D3D9 Present", pD3D9Present);
                    Debug.Log("D3D9 Present: " + WinUtils.GetModuleFileNameWithOffset(pD3D9Present));

                    dD3D9Present = (D3D9PresentDelegate)Marshal.GetDelegateForFunctionPointer(pD3D9Present, typeof(D3D9PresentDelegate));
                    RuntimeHelpers.PrepareMethod(oD3D9PresentHookInfo.MethodHandle);
                    D3D9PresentHook = LocalHook.Create(pD3D9Present, new D3D9PresentDelegate(hD3D9Present), null);
                    D3D9PresentHook.ThreadACL.SetExclusiveACL(new Int32[1]);
                    Debug.Log("D3D9 Present: Hooked.");
                }
                else
                    Debug.LogError("D3D9 Present: Not found.");

                var pD3D9Reset = IntPtr.Zero;

                if (Config.instance.useD3DPatternScan)
                {
                    // Attempt to hook CBaseDevice::Reset by byte signature.
                    // This method may not work if the function is already hooked or the pattern isn't present in this version of d3d9.dll.
                    Debug.Log("D3D9 Reset: Attemping pattern scan.");
                    pD3D9Reset = WinUtils.PatternScan(hD3D9, D3D9ResetPattern);
                    if (pD3D9Reset == IntPtr.Zero)
                        Debug.Log("D3D9 Reset: Pattern not found.");
                }

                if (pD3D9Reset == IntPtr.Zero)
                {
                    // Attempt to hook CBaseDevice::Reset by dummy device vtable.
                    // This method may not work if the vtable pointer has been swapped.
                    Debug.Log("D3D9 Reset: Attempting Vtable lookup.");
                    pD3D9Reset = Import.GetD3D9Reset();
                }

                if (pD3D9Reset != IntPtr.Zero)
                {
                    pD3D9Reset = ResolveExistingHooks("D3D9 Reset", pD3D9Reset);
                    Debug.Log("D3D9 Reset: " + WinUtils.GetModuleFileNameWithOffset(pD3D9Reset));

                    dD3D9Reset = (D3D9ResetDelegate)Marshal.GetDelegateForFunctionPointer(pD3D9Reset, typeof(D3D9ResetDelegate));
                    RuntimeHelpers.PrepareMethod(oD3D9ResetHookInfo.MethodHandle);
                    D3D9ResetHook = LocalHook.Create(pD3D9Reset, new D3D9ResetDelegate(hD3D9Reset), null);
                    D3D9ResetHook.ThreadACL.SetExclusiveACL(new Int32[1]);
                    Debug.Log("D3D9 Reset: Hooked.");
                }
                else
                    Debug.LogError("D3D9 Reset: Not found.");
            }

            initialized = true;
        }

        public static void Shutdown()
        {
            if (D3D9PresentHook != null)
                D3D9PresentHook.Dispose();

            if (D3D9ResetHook != null)
                D3D9ResetHook.Dispose();

            ImGuiBackend.instance.Shutdown();

            initialized = false;
        }
    }
}
