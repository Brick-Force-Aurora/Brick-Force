using System;
using UnityEngine;

namespace _Emulator
{
    class Config
    {
        public static Config instance;
        public CSVLoader csv;

        public Color crosshairColor = Color.green;
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
        public void SaveConfigToDisk(string path = "Config\\Config.csv")
        {
            csv.SetValue("host_ip", ClientExtension.instance.hostIP);
            csv.SetValue("debug_handle", ServerEmulator.instance.debugHandle);
            csv.SetValue("debug_send", ServerEmulator.instance.debugSend);
            csv.SetValue("debug_ping", ServerEmulator.instance.debugPing);
            csv.SetValue("crosshair_r", crosshairColor.r);
            csv.SetValue("crosshair_g", crosshairColor.g);
            csv.SetValue("crosshair_b", crosshairColor.b);
            csv.SetValue("usk_textures", uskTextures);
            csv.SetValue("axis_ratio", axisRatio);
            csv.SetValue("one_client_per_ip", oneClientPerIP);
            csv.SetValue("block_connections", blockConnections);
            csv.SetValue("auto_clear_dead_clients", autoClearDeadClients);
            csv.SetValue("max_connections", maxConnections);
            csv.Save(path, "Config\tValue");
        }

        public void LoadConfigFromDisk(string path = "Config\\Config.csv")
        {
            csv = new CSVLoader();
            csv.Load(path);
            ClientExtension.instance.hostIP = csv.GetValue<string>("host_ip");
            ServerEmulator.instance.debugHandle = csv.GetValue<bool>("debug_handle");
            ServerEmulator.instance.debugSend = csv.GetValue<bool>("debug_send");
            ServerEmulator.instance.debugPing = csv.GetValue<bool>("debug_ping");
            crosshairColor.r = csv.GetValue<float>("crosshair_r");
            crosshairColor.g = csv.GetValue<float>("crosshair_g");
            crosshairColor.b = csv.GetValue<float>("crosshair_b");
            Utils.RGBToHSV(crosshairColor, out float H, out float S, out float V);
            crosshairHue = H * 360f;
            uskTextures = csv.GetValue<bool>("usk_textures");
            oldUskTextures = !uskTextures;
            axisRatio = csv.GetValue<float>("axis_ratio");
            oneClientPerIP = csv.GetValue<bool>("one_client_per_ip");
            blockConnections = csv.GetValue<bool>("block_connections");
            autoClearDeadClients = csv.GetValue<bool>("auto_clear_dead_clients");
            maxConnections = csv.GetValue<int>("max_connections");
            ApplyUskTextures();
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
