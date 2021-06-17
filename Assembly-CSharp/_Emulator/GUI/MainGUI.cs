using UnityEngine;

namespace _Emulator
{
    public class MainGUI : MonoBehaviour
    {
        public static MainGUI instance;
        public bool setupHidden = false;
        public bool hostHidden = true;
        private Rect setupGUIRect = new Rect(0, 0, 200f, 0);
        private Rect hostGUIRect = new Rect(0, 0, 200f, 0);
        private string customMessage = "Custom Message";

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F6))
                setupHidden = !setupHidden;

            if (Input.GetKeyDown(KeyCode.F4))
                hostHidden = !hostHidden;

            ClientExtension.instance.HandleReliableKillLog();
        }

        private void OnGUI()
        {
            if (!setupHidden)
                setupGUIRect = GUILayout.Window(100, setupGUIRect, SetupGUIWindow, "Setup");

            if (!hostHidden && ServerEmulator.instance.serverCreated)
                hostGUIRect = GUILayout.Window(101, hostGUIRect, HostGUIWindow, "Host");
        }

        private void SetupGUIWindow(int winID)
        {
            GUILayout.Label("Host IP:");
            ClientExtension.instance.hostIP = GUILayout.TextField(ClientExtension.instance.hostIP);

            if (GUILayout.Button("Host Match"))
            {
                hostHidden = true;
                ServerEmulator.instance.SetupServer();
                ClientExtension.instance.LoadServer();
            }

            if (GUILayout.Button("Join Match"))
            {
                hostHidden = true;
                ClientExtension.instance.LoadServer();
            }
        }

        private void HostGUIWindow(int winID)
        {
            if (GUILayout.Button("Shutdown"))
            {
                ServerEmulator.instance.ShutdownInit();
            }

            if (GUILayout.Button("Reset"))
            {
                ServerEmulator.instance.Reset();
            }

            if (GUILayout.Button("End Match"))
            {
                ServerEmulator.instance.matchData.EndMatch();
            }

            if (GUILayout.Button("Clear Buffers"))
            {
                ServerEmulator.instance.ClearBuffers();
            }

            customMessage = GUILayout.TextField(customMessage);

            if (GUILayout.Button("Send Custom Message"))
            {
                ServerEmulator.instance.SendCustomMessage(customMessage);
            }

            GUILayout.Label("Clients:");

            for (int i = 0; i < ServerEmulator.instance.clientList.Count; i++)
            {
                if (GUILayout.Button(ServerEmulator.instance.clientList[i].GetIdentifier()) && !ServerEmulator.instance.clientList[i].isHost)
                {
                    ServerEmulator.instance.SendDisconnect(ServerEmulator.instance.clientList[i]);
                }
            }
        }
    }
}
