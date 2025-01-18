using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using UnityEngine;
using Vector4 = System.Numerics.Vector4;

namespace _Emulator
{
    class Config
    {
        public static Config instance;
        //public JsonObject configData;

        public Color crosshairColor = Color.green;
        public static readonly Vector4 themeColorDefault = new Vector4(1f, 0f, 0.39f, 1f);
        public Vector4 themeColor = new Vector4(1f, 0f, 0.39f, 1f);
        public bool dpiAware = false;
        public bool menuBlocksInput = false;
        public float crosshairHue = 90f;
        private float oldCrosshairHue = -1f;
        public bool uskTextures = false;
        private bool oldUskTextures = false;
        public float axisRatio = 2.25f;
        private float oldAxisRatio = 0f;
        public bool oneClientPerIP = true;
        public bool blockConnections = false;
        public bool autoClearDeadClients = true;
        public int maxConnections = 16;
        public int maxNumRooms = 1;
        public bool onlyHostRooms = true;
        public bool announceLobbyToFriends = true;

        public Config()
        {
            LoadConfigFromDisk();
        }
        public void SaveConfigToDisk(string path = "Config\\Config.json")
        {
            try
            {
                var data = new JsonData();
                data["host_ip"] = ClientExtension.instance.hostIP;
                data["debug_handle"] = ServerEmulator.instance.debugHandle;
                data["debug_send"] = ServerEmulator.instance.debugSend;
                data["debug_ping"] = ServerEmulator.instance.debugPing;
                data["debug_steam"] = SteamManager.debug;
                data["crosshair_r"] = crosshairColor.r;
                data["crosshair_g"] = crosshairColor.g;
                data["crosshair_b"] = crosshairColor.b;
                data["usk_textures"] = uskTextures;
                data["axis_ratio"] = axisRatio;
                data["dpi_aware"] = dpiAware;
                data["menu_blocks_input"] = menuBlocksInput;
                data["theme_r"] = themeColor.X;
                data["theme_g"] = themeColor.Y;
                data["theme_b"] = themeColor.Z;
                data["one_client_per_ip"] = oneClientPerIP;
                data["block_connections"] = blockConnections;
                data["auto_clear_dead_clients"] = autoClearDeadClients;
                data["max_connections"] = maxConnections;
                data["max_num_rooms"] = maxNumRooms;
                data["only_host_rooms"] = onlyHostRooms;
                data["announce_lobby_to_friends"] = announceLobbyToFriends;

                StringBuilder stringBuilder = new StringBuilder();
                JsonWriter writer = new JsonWriter(stringBuilder)
                {
                    PrettyPrint = true,
                    IndentValue = 2
                };

                JsonMapper.ToJson(data, writer);
                File.WriteAllText(path, stringBuilder.ToString());
            }

            catch (Exception ex)
            {
                Debug.LogError("SaveConfigToDisk: " + ex.Message);
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

            try
            {
                var json = File.ReadAllText(path);
                var data = JsonMapper.ToObject(json);

                try { ClientExtension.instance.hostIP = (string)data["host_ip"]; } catch (Exception ex) { Debug.LogError(ex.Message); }
                try { ServerEmulator.instance.debugHandle = (bool)data["debug_handle"]; } catch (Exception ex) { Debug.LogError(ex.Message); }
                try { ServerEmulator.instance.debugSend = (bool)data["debug_send"]; } catch (Exception ex) { Debug.LogError(ex.Message); }
                try { ServerEmulator.instance.debugPing = (bool)data["debug_ping"]; } catch (Exception ex) { Debug.LogError(ex.Message); }
                try { SteamManager.debug = (bool)data["debug_steam"]; } catch (Exception ex) { Debug.LogError(ex.Message); }
                try { crosshairColor.r = (float)(double)data["crosshair_r"]; } catch (Exception ex) { Debug.LogError(ex.Message); }
                try { crosshairColor.g = (float)(double)data["crosshair_g"]; } catch (Exception ex) { Debug.LogError(ex.Message); }
                try { crosshairColor.b = (float)(double)data["crosshair_b"]; } catch (Exception ex) { Debug.LogError(ex.Message); }
                try { dpiAware = (bool)data["dpi_aware"]; } catch (Exception ex) { Debug.LogError(ex.Message); }
                try { menuBlocksInput = (bool)data["menu_blocks_input"]; } catch (Exception ex) { Debug.LogError(ex.Message); }
                try { themeColor.X = (float)(double)data["theme_r"]; } catch (Exception ex) { Debug.LogError(ex.Message); }
                try { themeColor.Y = (float)(double)data["theme_g"]; } catch (Exception ex) { Debug.LogError(ex.Message); }
                try { themeColor.Z = (float)(double)data["theme_b"]; } catch (Exception ex) { Debug.LogError(ex.Message); }
                try { uskTextures = (bool)data["usk_textures"]; } catch (Exception ex) { Debug.LogError(ex.Message); }
                try { axisRatio = (float)(double)data["axis_ratio"]; } catch (Exception ex) { Debug.LogError(ex.Message); }
                try { oneClientPerIP = (bool)data["one_client_per_ip"]; } catch (Exception ex) { Debug.LogError(ex.Message); }
                try { blockConnections = (bool)data["block_connections"]; } catch (Exception ex) { Debug.LogError(ex.Message); }
                try { autoClearDeadClients = (bool)data["auto_clear_dead_clients"]; } catch (Exception ex) { Debug.LogError(ex.Message); }
                try { maxConnections = (int)data["max_connections"]; } catch (Exception ex) { Debug.LogError(ex.Message); }
                try { maxNumRooms = (int)data["max_num_rooms"]; } catch (Exception ex) { Debug.LogError(ex.Message); }
                try { onlyHostRooms = (bool)data["only_host_rooms"]; } catch (Exception ex) { Debug.LogError(ex.Message); }
                try { announceLobbyToFriends = (bool)data["announce_lobby_to_friends"]; } catch (Exception ex) { Debug.LogError(ex.Message); }
            }

            catch (Exception ex)
            {
                Debug.LogError("LoadConfigFromDisk: " + ex.Message);
            }

            Utils.RGBToHSV(crosshairColor, out float H, out float S, out float V);
            crosshairHue = H * 360f;
            oldUskTextures = !uskTextures;

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
