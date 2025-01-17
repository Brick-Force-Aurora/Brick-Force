using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ImGuiNET;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Vector4 = System.Numerics.Vector4;
using Vector2 = System.Numerics.Vector2;
using System.IO;

namespace _Emulator
{
    class ImGuiBackend : MonoBehaviour
    {
        public static ImGuiBackend instance;

        private bool presentInit = false;
        private bool contextInit = false;
        private bool wndProcInit = false;
        private bool blockPresent = false;
        private IntPtr hwnd = IntPtr.Zero;
        private Vector2 lastScreenSize;
        private bool lastFullscreen;
        private Vector4 lastThemeColor;
        private bool shutdownFromResize = false;
        public readonly object imguiLock = new object();
        private Import.WndProcDelegate dWndProc = null;
        private IntPtr oWndProc = IntPtr.Zero;
        private ImFontPtr font = null;
        private float dpiScale = 1f;

        ImGuiBackend()
        {
            dWndProc = new Import.WndProcDelegate(WndProc);
        }

        private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            switch ((ImportTypes.WindowMessage)msg)
            {
                // Screen changes etc.
                case ImportTypes.WindowMessage.WM_SIZE:
                    {
                        shutdownFromResize = true;
                        Shutdown(false, false);
                        break;
                    }

                // Temporarily shutdown ImGui on focus loss to prevent crashing when refocusing with fullscreen active.
                case ImportTypes.WindowMessage.WM_ACTIVATE:
                    {
                        if (lastFullscreen)
                        {
                            if (wParam == (IntPtr)0)
                                Shutdown(false, false);
                            else
                                OnScreenSizeChanged();
                        }
                        break;
                    }

                // Don't freeze when closing the game window.
                case ImportTypes.WindowMessage.WM_QUIT:
                case ImportTypes.WindowMessage.WM_DESTROY:
                case ImportTypes.WindowMessage.WM_CLOSE:
                    {
                        Application.Quit();
                        break;
                    }
            }

            if (ImGui.ImGui_ImplWin32_WndProcHandler(hWnd, msg, wParam, lParam) != 0)
                return (IntPtr)1;

            if (Config.instance.menuBlocksInput && ImGuiMenu.instance.isVisible)
            {
                return (IntPtr)1;
            }

            if (oWndProc != IntPtr.Zero)
                return Import.CallWindowProc(oWndProc, hWnd, msg, wParam, lParam);

            return IntPtr.Zero;
        }

        public void OnPresent(IntPtr pDevice)
        {
            lock (imguiLock)
            {
                UpdateScreenSize();

                if (blockPresent)
                    return;

                if (!presentInit)
                {
                    hwnd = Import.GetDeviceWindow(pDevice);
                    if (hwnd != IntPtr.Zero)
                    {
                        presentInit = true;
                        if (!contextInit)
                        {
                            ImGui.CreateContext();
                            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.NoMouseCursorChange;
                            Import.igStyleColorsCustomDefault();
                            LoadFont();
                            contextInit = true;
                        }
                        ImGui.ImGui_ImplWin32_Init(hwnd);
                        ImGui.ImGui_ImplDX9_Init(pDevice);
                        if (!wndProcInit)
                        {
                            oWndProc = Import.SetWindowLongPtr(hwnd, -4, Marshal.GetFunctionPointerForDelegate(dWndProc));
                            wndProcInit = true;
                        }
                    }

                    else
                        Debug.LogError("OnPresent: Can't find process HWND.");
                }

                if (presentInit && contextInit)
                {
                    UpdateDPIScale();
                    UpdateTheme();
                    ImGui.ImGui_ImplDX9_NewFrame();
                    ImGui.ImGui_ImplWin32_NewFrame();
                    ImGui.NewFrame();
                    ImGui.PushFont(font);
                    ImGuiMenu.instance.Render();
                    ImGui.PopFont();
                    ImGui.Render();
                    ImGui.ImGui_ImplDX9_RenderDrawData(ImGui.GetDrawData());
                    ImGui.EndFrame();
                }
            }
        }

        public void OnReset(IntPtr pDevice)
        {
            lock (imguiLock)
            {
                if (presentInit)
                {
                    presentInit = false;
                    ImGui.ImGui_ImplDX9_InvalidateDeviceObjects();
                }
            }
        }

        public void AfterReset(IntPtr pDevice)
        {
            lock (imguiLock)
            {
                ImGui.ImGui_ImplDX9_CreateDeviceObjects();
            }
        }

        public void OnScreenSizeChanged()
        {
            lock (imguiLock)
            {
                blockPresent = false;
            }
        }

        public void Shutdown(bool destroyContext = true, bool removeWndProc = true)
        {
            lock (imguiLock)
            {
                if (presentInit)
                {
                    presentInit = false;
                    blockPresent = true;
                    if (removeWndProc)
                    {
                        Import.SetWindowLongPtr(hwnd, -4, oWndProc);
                        wndProcInit = false;
                    }
                    ImGui.ImGui_ImplDX9_Shutdown();
                    ImGui.ImGui_ImplWin32_Shutdown();
                    if (destroyContext)
                    {
                        ImGui.DestroyContext();
                        contextInit = false;
                    }
                }
            }
        }

        void Awake()
        {
            lastScreenSize = new Vector2(Screen.width, Screen.height);
            lastFullscreen = Screen.fullScreen;
            lastThemeColor = Config.themeColorDefault;
        }

        void UpdateScreenSize()
        {
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            if (lastScreenSize != screenSize)
            {
                lastScreenSize = screenSize;
                OnScreenSizeChanged();
            }

            bool fullscreen = Screen.fullScreen;
            if (lastFullscreen != fullscreen)
            {
                lastFullscreen = fullscreen;
                OnScreenSizeChanged();
            }

            if (shutdownFromResize)
            {
                shutdownFromResize = false;
                OnScreenSizeChanged();
            }
        }

        void LoadFont()
        {
            if (File.Exists("Font.ttf"))
            {
                ImGui.GetIO().Fonts.Clear();
                //font = ImGui.GetIO().Fonts.AddFontFromFileTTF("Font.ttf", 15f * dpiScale, new ImFontConfigPtr(IntPtr.Zero), Import.igGetGlyphRanges());
                font = ImGui.GetIO().Fonts.AddFontFromFileTTF("Font.ttf", 15f * dpiScale, new ImFontConfigPtr(IntPtr.Zero), ImGui.GetIO().Fonts.GetGlyphRangesJapanese());
                ImGui.ImGui_ImplDX9_InvalidateDeviceObjects();
            }
            else
                Debug.LogError("Font.ttf missing");
        }

        void UpdateTheme()
        {
            if (Config.instance != null && Config.instance.themeColor != null && Config.instance.themeColor != lastThemeColor)
            {
                Import.igStyleColorsCustom(Config.instance.themeColor);
                lastThemeColor = Config.instance.themeColor;
            }
        }

        void UpdateDPIScale()
        {
            if (hwnd != IntPtr.Zero && Config.instance != null)
            {
                var scale = Config.instance.dpiAware ? ImGui.ImGui_ImplWin32_GetDpiScaleForHwnd(hwnd) : 1f;
                if (scale != dpiScale)
                {
                    dpiScale = scale;
                    ImGuiMenu.instance.dpiScale = dpiScale;
                    Import.igScaleAllSizesReset(dpiScale);
                    Import.igStyleColorsCustom(Config.instance.themeColor);
                    LoadFont();
                }
            }
        }
    }
}
