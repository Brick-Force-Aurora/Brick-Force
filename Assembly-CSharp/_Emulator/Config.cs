using _Emulator.JSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using UnityEngine;
using Vector4 = System.Numerics.Vector4;

namespace _Emulator
{
    class Config
    {
        public static Config instance;
        public JsonObject configData;

        public Color crosshairColor = Color.green;
        public static readonly Vector4 themeColorDefault = new Vector4(1f, 0f, 0.39f, 1f);
        public Vector4 themeColor = new Vector4(1f, 0f, 0.39f, 1f);
        public bool dpiAware = false;
        public float crosshairHue = 90f;
        private float oldCrosshairHue = -1f;
        public bool uskTextures = false;
        private bool oldUskTextures = false;
        public float axisRatio = 2.25f;
        private float oldAxisRatio = 0f;
        public bool oneClientPerIP = true;
        public bool blockConnections = false;
        public bool autoClearDeadClients = false;
        public int maxConnections = 16;

        public Config()
        {
            LoadConfigFromDisk();
        }
        public void SaveConfigToDisk(string path = "Config\\Config.json")
        {
            using (var writer = new StreamWriter(path))
            {
                var jsonWriter = new JsonWriter(writer); 
                configData = new JsonObject
                {
                    { "host_ip", ClientExtension.instance.hostIP },
                    { "debug_handle", ServerEmulator.instance.debugHandle },
                    { "debug_send", ServerEmulator.instance.debugSend },
                    { "debug_ping", ServerEmulator.instance.debugPing },
                    { "debug_steam", SteamManager.debug },
                    { "crosshair_r", crosshairColor.r },
                    { "crosshair_g", crosshairColor.g },
                    { "crosshair_b", crosshairColor.b },
                    { "dpi_aware", dpiAware },
                    { "theme_r", themeColor.X },
                    { "theme_g", themeColor.Y },
                    { "theme_b", themeColor.Z },
                    { "usk_textures", uskTextures },
                    { "axis_ratio", axisRatio },
                    { "one_client_per_ip", oneClientPerIP },
                    { "block_connections", blockConnections },
                    { "auto_clear_dead_clients", autoClearDeadClients },
                    { "max_connections", maxConnections }
                };
                jsonWriter.WriteObject(configData);
            }
        }

        public void LoadConfigFromDisk(string path = "Config\\Config.json")
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning("Config file not found. Using default values.");
                SaveConfigToDisk();
                return;
            }

            using (var reader = new StreamReader(path))
            {
                var jsonReader = new JsonReader(reader);
                configData = jsonReader.ReadObject<JsonObject>();
            }

            ClientExtension.instance.hostIP = configData.Get<string>("host_ip", "");
            ServerEmulator.instance.debugHandle = configData.Get<bool>("debug_handle", false);
            ServerEmulator.instance.debugSend = configData.Get<bool>("debug_send", false);
            ServerEmulator.instance.debugPing = configData.Get<bool>("debug_ping", false);
            SteamManager.debug = configData.Get<bool>("debug_steam", false);
            crosshairColor.r = configData.Get<float>("crosshair_r", 0.0f);
            crosshairColor.g = configData.Get<float>("crosshair_g", 1.0f);
            crosshairColor.b = configData.Get<float>("crosshair_b", 0.0f);
            Utils.RGBToHSV(crosshairColor, out float H, out float S, out float V);
            crosshairHue = H * 360f;
            dpiAware = configData.Get<bool>("dpi_aware", false);
            themeColor.X = configData.Get<float>("theme_r", themeColorDefault.X);
            themeColor.Y = configData.Get<float>("theme_g", themeColorDefault.Y);
            themeColor.Z = configData.Get<float>("theme_b", themeColorDefault.Z);
            uskTextures = configData.Get<bool>("usk_textures", false);
            oldUskTextures = !uskTextures;
            axisRatio = configData.Get<float>("axis_ratio", 2.25f);
            oneClientPerIP = configData.Get<bool>("one_client_per_ip", true);
            blockConnections = configData.Get<bool>("block_connections", false);
            autoClearDeadClients = configData.Get<bool>("auto_clear_dead_clients", false);
            maxConnections = configData.Get<int>("max_connections", 16);
            ApplyUskTextures();
            ApplyAxisRatio();
            ApplyCrosshairHue();
        }

        public void ApplyAxisRatio()
        {
            if (oldAxisRatio != axisRatio)
            {
                oldAxisRatio = axisRatio;
                CameraController.ySpeed = CameraController.xSpeed * (1f / axisRatio);
            }
        }

        public void ApplyUskTextures()
        {
            if (oldUskTextures != uskTextures)
            {
                oldUskTextures = uskTextures;
                BuildOption.Instance.Props.useUskWeaponTex = uskTextures;
                BuildOption.Instance.Props.useUskWeaponIcon = uskTextures;
                BuildOption.Instance.Props.useUskMuzzleEff = uskTextures;
                BuildOption.Instance.Props.useUskDecal = uskTextures;
            }
        }

        public void ApplyCrosshairHue()
        {
            if (oldCrosshairHue != crosshairHue)
            {
                oldCrosshairHue = crosshairHue;
                crosshairColor = Utils.HSVToRGB(crosshairHue / 360f, 1f, 1f);
            }
        }
    }
}
