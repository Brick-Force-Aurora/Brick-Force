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
            DebugConsole.instance = coreObject.AddComponent<DebugConsole>();
            ServerEmulator.instance = coreObject.AddComponent<ServerEmulator>();
            UnityEngine.Object.DontDestroyOnLoad(coreObject);
            Config.instance = new Config();
        }
    }
}