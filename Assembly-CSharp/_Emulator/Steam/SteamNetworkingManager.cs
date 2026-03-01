using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using _Emulator.Steam;
using Steamworks;
using UnityEngine;
using static Mono.Xml.MiniParser;

namespace _Emulator
{
    [DisallowMultipleComponent]
    public class SteamNetworkingManager : MonoBehaviour
    {
        public static SteamNetworkingManager instance;

        private Callback<SteamNetworkingMessagesSessionRequest_t> m_CallbackSessionRequest;

        private bool shouldReceive = false;

        private SteamChannelHandler[] handlers;
        private readonly SteamNetworkingChannel[] channels = (SteamNetworkingChannel[]) Enum.GetValues(typeof(SteamNetworkingChannel));

        public SteamChannelHandler ServerChannel
        {
            get => handlers[(int)SteamNetworkingChannel.ToHost];
        }

        public void StartReceive()
        {
            shouldReceive = true;
        }

        public void EndReceive()
        {
            shouldReceive = false;
        }

        void Awake()
        {
            handlers = new SteamChannelHandler[4];
            for (int i = 1; i < handlers.Length; i++) {
                handlers[i] = new SteamChannelHandler((SteamNetworkingChannel)i);
            }
        }

        void OnEnable()
        {
            if (SteamManager.Initialized)
            {
                m_CallbackSessionRequest = Callback<SteamNetworkingMessagesSessionRequest_t>.Create(OnSessionRequest);
            }
        }

        void Update()
        {
            if (shouldReceive && SteamManager.Initialized)
            {
                HandleReceiveNetwork();
                HandleClientMessages();
            }
        }

        private void HandleReceiveNetwork()
        {
            foreach (SteamNetworkingChannel channel in channels)
            {
                SteamChannelHandler handler = handlers[(int)channel];
                IntPtr[] receiveBuffers = new IntPtr[16];
                int nMessages = SteamNetworkingMessages.ReceiveMessagesOnChannel((int)channel, receiveBuffers, receiveBuffers.Length);
                if (handler == null)
                {
                    // We ignore this channel :)
                    continue;
                }
                for (int i = 0; i < nMessages; i++)
                {
                    try
                    {
                        SteamNetworkingMessage_t steamMsg = (SteamNetworkingMessage_t)Marshal.PtrToStructure(receiveBuffers[i], typeof(SteamNetworkingMessage_t));
                        byte[] msg = new byte[steamMsg.m_cbSize];
                        Marshal.Copy(steamMsg.m_pData, msg, 0, msg.Length);
                        if (SteamManager.debug)
                            Debug.Log("Steam - Received " + msg.Length + " bytes from " + steamMsg.m_identityPeer.GetSteamID() + " via " + channel);
                        handler.Enqueue(steamMsg.m_identityPeer.GetSteamID(), msg);
                    }
                    finally
                    {
                        Marshal.DestroyStructure(receiveBuffers[i], typeof(SteamNetworkingMessage_t));
                    }
                }
            }
        }

        private void HandleClientMessages()
        {
            handlers[(int)SteamNetworkingChannel.ToClient].Dequeue(ClientExtension.instance.ReceiveSteam);
            handlers[(int)SteamNetworkingChannel.ToP2P].Dequeue(P2PExtension.instance.ReceiveSteam);
        }

        public void SendInitMessageToHost()
        {
            var buffer = new byte[1];
            buffer[0] = 0;
            SendMessageToUser(SteamNetworkingChannel.Generic, SteamLobbyManager.instance.GetCurrentOwner(), buffer);
        }

        public void SendMessageToHost(Msg4Send msg)
        {
            SendMessageToUser(SteamNetworkingChannel.ToHost, SteamLobbyManager.instance.GetCurrentOwner(), msg);
        }

        public void SendMessageToPeer(CSteamID steamID, P2PMsg4Send msg)
        {
            SendMessageToUser(SteamNetworkingChannel.ToP2P, steamID, msg.Buffer);
        }

        public void SendMessageToUser(SteamNetworkingChannel channel, CSteamID steamID, Msg4Send msg)
        {
            SendMessageToUser(channel, steamID, msg.Buffer);
        }

        public void SendMessageToUser(SteamNetworkingChannel channel, CSteamID steamID, byte[] msg)
        {
            if (SteamManager.Initialized)
            {
                if (steamID == SteamUser.GetSteamID()) // Queue locally instead of sending
                {
                    SteamChannelHandler handler = handlers[(int)channel];
                    if (handler != null)
                    {
                        handler.Enqueue(steamID, msg);
                    }
                }
                else
                {
                    int sendFlags = channel == SteamNetworkingChannel.ToP2P ? 0 : 8;
                    SteamNetworkingIdentity identityRemote = new SteamNetworkingIdentity();
                    identityRemote.SetSteamID(steamID);

                    IntPtr ptr = Marshal.AllocHGlobal(msg.Length);

                    try
                    {
                        Marshal.Copy(msg, 0, ptr, msg.Length);
                        var result = SteamNetworkingMessages.SendMessageToUser(ref identityRemote, ptr, (uint)msg.Length, sendFlags, (int)channel);
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(ptr);
                    }
                }
            }
        }

        public void CloseSessionWithUser(CSteamID steamID)
        {
            if (SteamManager.debug)
                Debug.Log("Steam - CloseSessionWithUser: " + steamID.ToString());
            SteamNetworkingIdentity identity = new SteamNetworkingIdentity();
            identity.SetSteamID(steamID);
            SteamNetworkingMessages.CloseSessionWithUser(ref identity);
        }

        void OnSessionRequest(SteamNetworkingMessagesSessionRequest_t sessionRequest)
        {
            if (SteamManager.Initialized)
            {
                lock (SteamLobbyManager.instance.currentLobbyLock)
                {
                    if (SteamLobbyManager.instance.currentLobby != null)
                    {
                        var steamID = sessionRequest.m_identityRemote.GetSteamID();
                        bool accepted = false;

                        // Only accept connections from lobby members.
                        if (SteamLobbyManager.instance.currentLobby.IsLobbyMember(steamID))
                        {
                            SteamNetworkingMessages.AcceptSessionWithUser(ref sessionRequest.m_identityRemote);
                            accepted = true;
                        }

                        if (SteamManager.debug)
                            Debug.Log("Steam - OnSessionRequest from " + steamID + " | " + (accepted ? "Accepted" : "Rejected"));
                    }
                }
            }
        }
    }
}
