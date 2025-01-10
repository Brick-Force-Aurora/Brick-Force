using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace _Emulator
{
    class ConfigGUI : MonoBehaviour
    {
        public static ConfigGUI instance;
        private Rect configGUIRect = new Rect(0f, 0f, 200f, 0f);
        private bool hidden = true;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F7))
                hidden = !hidden;

            Config.instance.ApplyAxisRatio();
            Config.instance.ApplyCrosshairHue();
        }

        private void OnGUI()
        {
            if (!hidden)
            {
                configGUIRect = GUILayout.Window(104, configGUIRect, ConfigGUIWindow, "Config");
            }
        }

        private void ConfigGUIWindow(int winID)
        {
            if (GUILayout.Button("Save"))
                Config.instance.SaveConfigToDisk();

            if (GUILayout.Button("Load"))
                Config.instance.LoadConfigFromDisk();

            GUILayout.Label("Axis Ratio: " + Config.instance.axisRatio.ToString("n2"));
            Config.instance.axisRatio = GUILayout.HorizontalSlider(Config.instance.axisRatio, 1f, 2.25f);

            GUILayout.Label("Crosshair Hue: " + Config.instance.crosshairHue.ToString("n2"));
            Config.instance.crosshairHue = GUILayout.HorizontalSlider(Config.instance.crosshairHue, 0f, 360f);

            Config.instance.autoClearDeadClients = GUILayout.Toggle(Config.instance.autoClearDeadClients, "Auto Clear Dead Clients");
            Config.instance.oneClientPerIP = GUILayout.Toggle(Config.instance.oneClientPerIP, "One Client Per IP");
            Config.instance.blockConnections = GUILayout.Toggle(Config.instance.blockConnections, "Block All Connections");
            GUILayout.Label("Max Connections: " + Config.instance.maxConnections);
            Config.instance.maxConnections = Mathf.FloorToInt(GUILayout.HorizontalSlider(Config.instance.maxConnections, 1, 16));
            ServerEmulator.instance.debugHandle = GUILayout.Toggle(ServerEmulator.instance.debugHandle, "Debug Handle");
            ServerEmulator.instance.debugSend = GUILayout.Toggle(ServerEmulator.instance.debugSend, "Debug Send");
            ServerEmulator.instance.debugPing = GUILayout.Toggle(ServerEmulator.instance.debugPing, "Debug Ping");
            SteamManager.debug = GUILayout.Toggle(SteamManager.debug, "Debug Steam");
        }
    }
}
