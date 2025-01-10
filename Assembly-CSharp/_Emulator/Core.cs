using UnityEngine;

namespace _Emulator
{
    class Core
    {
        public static Core instance = new Core();
        private GameObject coreObject;

        public void Initialize()
        {
            Hooks.Initialize();
            coreObject = new GameObject();
            MainGUI.instance = coreObject.AddComponent<MainGUI>();
            InventoryGUI.instance = coreObject.AddComponent<InventoryGUI>();
            ConfigGUI.instance = coreObject.AddComponent<ConfigGUI>();
            DebugConsole.instance = coreObject.AddComponent<DebugConsole>();
            ServerEmulator.instance = coreObject.AddComponent<ServerEmulator>();
            SteamManager.Instance = coreObject.AddComponent<SteamManager>();
            SteamLobbyManager.instance = coreObject.AddComponent<SteamLobbyManager>();
            SteamNetworkingManager.instance = coreObject.AddComponent<SteamNetworkingManager>();
            SteamGUI.instance = coreObject.AddComponent<SteamGUI>();
            UnityEngine.Object.DontDestroyOnLoad(coreObject);
            Config.instance = new Config();
            SetupBuildConfig();
        }

        private void SetupBuildConfig()
        {
            Application.runInBackground = true;
            BuildOption.Instance.Props.UseP2pHolePunching = true;
            BuildOption.Instance.Props.isDuplicateExcuteAble = true;
        }
    }
}