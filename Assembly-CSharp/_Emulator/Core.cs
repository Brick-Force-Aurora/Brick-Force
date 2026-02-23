using System.IO;
using UnityEngine;

namespace _Emulator
{
    public class Core
    {
        private static Version _version;
        private static string _versionString;
        public static Version Version {
            get
            {
                if (_version == null)
                {
                    _version = GetGithubVersionOrUnknown();
                    Debug.Log($"Aurora Version: {_version}");
                }
                return _version;
            }
        }
        public static string VersionStr
        {
            get
            {
                if (_versionString == null)
                {
                    _versionString = Version.ToString();
                }
                return _versionString;
            }
        }

        public static Core instance = new Core();
        private GameObject coreObject;

        public void Initialize()
        {
            HooksManaged.Initialize();
            coreObject = new GameObject();
            ClientExtension.instance = coreObject.AddComponent<ClientExtension>();
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
            SetupOperationProcessor();
            RegisterCommands();
        }

        private void SetupOperationProcessor()
        {
            if (GameObject.Find("BrickEdit") != null)
            {
                return;
            }
            GameObject brickEdit = new GameObject("BrickEdit");
            OperationProcessor.Instance = brickEdit.AddComponent<OperationProcessor>();
            BulkChangeProcessor.Instance = brickEdit.AddComponent<BulkChangeProcessor>();
        }

        private void RegisterCommands()
        {
            CommandHandler handler = CommandHandler.Instance;
            handler.Register("w", "whisper", new WhisperCommand());
            handler.Register("r", "reply", new WhisperReplyCommand());

            // BrickEdit commands
            handler.Register("/set hollow", new BrickEditSetHollowCommand());
            handler.Register("/set", new BrickEditSetCommand());
            handler.Register("/remove near", "/del near", "/delete near", new BrickEditRemoveNearCommand());
            handler.Register("/remove", "/del", "/delete", new BrickEditRemoveCommand());
            handler.Register("/replace near", new BrickEditReplaceNearCommand());
            handler.Register("/replace", new BrickEditReplaceCommand());
            handler.Register("/sel", "/selection", new BrickEditSelectionCommand());
            handler.Register("/pos1", new BrickEditPos1Command());
            handler.Register("/pos2", new BrickEditPos2Command());
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

        private static Version GetGithubVersionOrUnknown()
        {
            try
            {
                string path = Path.GetFullPath(
                    Path.Combine(Application.dataPath, "../launcher_data.dat")
                );

                if (!File.Exists(path))
                {
                    Debug.LogWarning("launcher_data.dat not found at: " + path);
                    return Version.UNKNOWN;
                }

                using (var fs = File.OpenRead(path))
                using (var br = new BinaryReader(fs))
                {
                    // Uses your NBT reader extension method
                    CompoundTag root = br.ReadAsNbt().Value;

                    if (root == null || !root.Has("version", TagType.INT_ARRAY))
                    {
                        Debug.LogWarning("NBT key 'version' not found or not INT_ARRAY in launcher_data.dat");
                        return Version.UNKNOWN;
                    }

                    // Directly access the stored IntArrayTag
                    int[] components = root.GetIntArray("version");
                    if (components.Length != 4)
                    {
                        Debug.LogWarning("NBT 'version' array missing/invalid");
                        return Version.UNKNOWN;
                    }
                    return Version.From(components);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Failed to read launcher version (launcher_data.dat): " + ex.Message);
                return Version.UNKNOWN;
            }
        }
    }
}