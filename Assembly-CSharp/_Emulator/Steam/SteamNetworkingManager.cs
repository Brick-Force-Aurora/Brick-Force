using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Text;
using Steamworks;
using UnityEngine;

namespace _Emulator
{
    [DisallowMultipleComponent]
    public class SteamNetworkingManager : MonoBehaviour
    {
        internal class LoopbackPacket
        {
            public LoopbackPacket(SteamNetworkingChannel _channel, CSteamID _steamID, byte[] _msg)
            {
                channel = _channel;
                steamID = _steamID;
                msg = new byte[_msg.Length];
                Array.Copy(_msg, msg, msg.Length);
            }

            public SteamNetworkingChannel channel;
            public CSteamID steamID = CSteamID.Nil;
            public byte[] msg;
        }

        public static SteamNetworkingManager instance;

        private Callback<SteamNetworkingMessagesSessionRequest_t> m_CallbackSessionRequest;

        private bool shouldReceive = false;

        private Queue<LoopbackPacket> loopbackQueue = new Queue<LoopbackPacket>();
        private readonly object loopbackLock = new object();

        public void StartReceive()
        {
            shouldReceive = true;
        }

        public void EndReceive()
        {
            shouldReceive = false;
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
                HandleReceiveLoopback();
            }
        }

        private void HandleReceiveNetwork()
        {
            foreach (SteamNetworkingChannel channel in Enum.GetValues(typeof(SteamNetworkingChannel)))
            {
                IntPtr[] receiveBuffers = new IntPtr[16];
                int nMessages = SteamNetworkingMessages.ReceiveMessagesOnChannel((int)channel, receiveBuffers, receiveBuffers.Length);
                for (int i = 0; i < nMessages; i++)
                {
                    try
                    {
                        SteamNetworkingMessage_t steamMsg = (SteamNetworkingMessage_t)Marshal.PtrToStructure(receiveBuffers[i], typeof(SteamNetworkingMessage_t));
                        byte[] msg = new byte[steamMsg.m_cbSize];
                        Marshal.Copy(steamMsg.m_pData, msg, 0, msg.Length);

                        if (SteamManager.debug)
                            Debug.Log("Steam - Received " + msg.Length + " bytes from " + steamMsg.m_identityPeer.GetSteamID() + " via " + channel);

                        HandleMessage(channel, steamMsg.m_identityPeer.GetSteamID(), msg);
                    }
                    finally
                    {
                        Marshal.DestroyStructure(receiveBuffers[i], typeof(SteamNetworkingMessage_t));
                    }
                }
            }
        }

        private void HandleReceiveLoopback()
        {
            lock (loopbackLock)
            {
                if (loopbackQueue.Count < 1)
                    return;

                var packet = loopbackQueue.Peek();
                if (SteamManager.debug)
                    Debug.Log("Steam - Received " + packet.msg.Length + " bytes from " + packet.steamID + " (Loopback) via " + packet.channel);

                try
                {
                    HandleMessage(packet.channel, packet.steamID, packet.msg);
                }

                finally
                {
                    loopbackQueue.Dequeue();
                }
            }
        }

        private void HandleMessage(SteamNetworkingChannel channel, CSteamID steamID, byte[] msg)
        {
            switch (channel)
            {
                case SteamNetworkingChannel.ToHost:
                    {
                        ServerEmulator.instance.ReceiveSteam(steamID, msg);
                        break;
                    }

                case SteamNetworkingChannel.ToClient:
                    {
                        ClientExtension.instance.ReceiveSteam(steamID, msg);
                        break;
                    }

                case SteamNetworkingChannel.ToP2P:
                    {
                        P2PExtension.instance.ReceiveSteam(steamID, msg);
                        break;
                    }
            }
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
                    lock (loopbackLock)
                    {
                        loopbackQueue.Enqueue(new LoopbackPacket(channel, steamID, msg));
                        if (SteamManager.debug)
                            Debug.Log("Steam - Sent " + msg.Length + " bytes to " + steamID + " (Loopback) via " + channel);
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
                        if (SteamManager.debug)
                            Debug.Log("Steam - Sent " + msg.Length + " bytes to " + steamID + " via " + channel + " | " + result);
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(ptr);
                    }
                }
            }
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
