using System;
using UnityEngine;

namespace _Emulator
{
    class Config
    {
        public static Config instance;
        public CSVLoader csv;

        public Color crosshairColor = Color.green;

        public Config()
        {
            LoadConfigFromDisk();
        }

        public void LoadConfigFromDisk(string path = "Config\\Config.csv")
        {
            csv = new CSVLoader();
            csv.Load(path);
            ClientExtension.instance.hostIP = csv._rows[0][1];
            ServerEmulator.instance.debugHandle = Convert.ToBoolean(csv._rows[1][1]);
            ServerEmulator.instance.debugSend = Convert.ToBoolean(csv._rows[2][1]);
            ServerEmulator.instance.debugPing = Convert.ToBoolean(csv._rows[3][1]);
            crosshairColor.r = Convert.ToSingle(csv._rows[4][1]);
            crosshairColor.g = Convert.ToSingle(csv._rows[5][1]);
            crosshairColor.b = Convert.ToSingle(csv._rows[6][1]);
        }
    }
}
