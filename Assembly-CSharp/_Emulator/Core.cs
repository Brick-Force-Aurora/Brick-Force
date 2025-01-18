using UnityEngine;

namespace _Emulator
{
    class Core
    {
        public static Core instance = new Core();
        private GameObject coreObject;

        public void Initialize()
        {
            HooksManaged.Initialize();
            coreObject = new GameObject();
            MainGUI.instance = coreObject.AddComponent<MainGUI>();
            InventoryGUI.instance = coreObject.AddComponent<InventoryGUI>();
            ConfigGUI.instance = coreObject.AddComponent<ConfigGUI>();
            DebugConsole.instance = coreObject.AddComponent<DebugConsole>();
            ServerEmulator.instance = coreObject.AddComponent<ServerEmulator>();
            SteamManager.Instance = coreObject.AddComponent<SteamManager>();
            SteamLobbyManager.instance = coreObject.AddComponent<SteamLobbyManager>();
            SteamNetworkingManager.instance = coreObject.AddComponent<SteamNetworkingManager>();
            SteamFriendsManager.instance = coreObject.AddComponent<SteamFriendsManager>();
            SteamGUI.instance = coreObject.AddComponent<SteamGUI>();
            ImGuiBackend.instance = coreObject.AddComponent<ImGuiBackend>();
            Object.DontDestroyOnLoad(coreObject);
            Config.instance = new Config();
            SetupBuildConfig();
        }

        private void SetupBuildConfig()
        {
            Application.runInBackground = true;
            BuildOption.Instance.Props.UseP2pHolePunching = true;
            BuildOption.Instance.Props.isDuplicateExcuteAble = true;
        }

        public static void SetBalancedItemProperties()
        {
            foreach (var entry in TItemManager.Instance.dic)
            {
                var item = entry.Value;
                var armor = item.type != TItem.TYPE.ACCESSORY || item.slot == TItem.SLOT.HEAD ? 20 : 10;
                var functionMask = "";
                var functionFactor = 0.2f;
                if (item is TCharacter tCharacter)
                {
                    tCharacter.tBuff = BuffManager.Instance.Get("ARMOR");
                }

                else if (item is TCostume tCostume)
                {
                    tCostume.resetArmor(armor);
                }

                else if (item is TAccessory tAccessory)
                {
                    tAccessory.resetArmor(armor);
                    tAccessory.tBuff = BuffManager.Instance.Get("ARMOR");
                    if (functionMask.Length > 1)
                    {
                        tAccessory.functionMask = TItem.String2FunctionMask(functionMask);
                    }
                    tAccessory.functionFactor = functionFactor;
                }
            }
        }
    }
}