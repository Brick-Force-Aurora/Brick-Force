using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using EasyHook;
using Debug = UnityEngine.Debug;
using System.Runtime.CompilerServices;
using ImGuiNET;

namespace _Emulator
{
    class HooksNative
    {
        static MethodInfo oD3D9PresentHookInfo = typeof(HooksNative).GetMethod("hD3D9Present", BindingFlags.Public | BindingFlags.Static);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate int D3D9PresentDelegate(IntPtr _this, IntPtr src, IntPtr dst, int wndOverride, IntPtr dirtyRegion);
        static D3D9PresentDelegate dD3D9Present;
        static LocalHook D3D9PresentHook;

        static MethodInfo oD3D9ResetHookInfo = typeof(HooksNative).GetMethod("hD3D9Reset", BindingFlags.Public | BindingFlags.Static);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate int D3D9ResetDelegate(IntPtr _this, IntPtr param);
        static D3D9ResetDelegate dD3D9Reset;
        static LocalHook D3D9ResetHook;

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
                var pD3D9Present = Import.GetD3D9Present();
                if (pD3D9Present != IntPtr.Zero)
                {
                    Debug.Log("D3D9 Present: 0x" + (pD3D9Present.ToInt64() - hD3D9.ToInt64()).ToString("X"));

                    dD3D9Present = (D3D9PresentDelegate)Marshal.GetDelegateForFunctionPointer(pD3D9Present, typeof(D3D9PresentDelegate));
                    RuntimeHelpers.PrepareMethod(oD3D9PresentHookInfo.MethodHandle);
                    D3D9PresentHook = LocalHook.Create(pD3D9Present, new D3D9PresentDelegate(hD3D9Present), null);
                    D3D9PresentHook.ThreadACL.SetExclusiveACL(new Int32[1]);
                }
                else
                    Debug.LogError("D3D9 Present not found.");

                var pD3D9Reset = Import.GetD3D9Reset();
                if (pD3D9Reset != IntPtr.Zero)
                {
                    Debug.Log("D3D9 Reset: 0x" + (pD3D9Reset.ToInt64() - hD3D9.ToInt64()).ToString("X"));

                    dD3D9Reset = (D3D9ResetDelegate)Marshal.GetDelegateForFunctionPointer(pD3D9Reset, typeof(D3D9ResetDelegate));
                    RuntimeHelpers.PrepareMethod(oD3D9ResetHookInfo.MethodHandle);
                    D3D9ResetHook = LocalHook.Create(pD3D9Reset, new D3D9ResetDelegate(hD3D9Reset), null);
                    D3D9ResetHook.ThreadACL.SetExclusiveACL(new Int32[1]);
                }
                else
                    Debug.LogError("D3D9 Reset not found.");
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
