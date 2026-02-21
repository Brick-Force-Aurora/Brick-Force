using System;
using System.Collections.Generic;
using System.Net.Sockets;
using _Emulator.Network;
using Steamworks;
using Debug = UnityEngine.Debug;

namespace _Emulator
{
    public class ClientReference
    {
        public enum ClientStatus
        {
            Invalid = -1,
            Lobby = 0,
            Room = 1,
            Match = 2
        }

        public Socket socket;
        public string ip;
        public int port;
        public byte[] buffer;
        public CSteamID steamID = CSteamID.Nil;
        public bool isSteam = false;
        public volatile bool didHeartBeat = false;
        public float lastHeartBeatTime;
        public float loginToleranceTime;
        public string name;
        public int seq;
        public bool isLoaded;
        public bool isHost;
        public int kills = 0;
        public int deaths = 0;
        public int assists = 0;
        public int score = 0;
        public bool isZombie = false;
        public bool isBreakingInto;
        public ClientStatus clientStatus;
        public BrickManDesc.STATUS status;
        public SlotData slot;
        public Inventory inventory;
        public DummyData data;
        public MatchData matchData;
        public ChannelReference channel;
        public ChunkedBufferReceiver chunkedBufferReceiver = new ChunkedBufferReceiver();
        public ChunkedBufferSender chunkedBufferSender = new ChunkedBufferSender();
        public int lastOpenedChestSeq = -1;
        public bool isVersionSetUp = false;

        private readonly object dataLock = new object();

        public ClientReference(Socket _socket, string _name = "", int _seq = -1)
        {
            lastHeartBeatTime = float.MaxValue;
            loginToleranceTime = 0f;
            socket = _socket;
            name = _name;
            seq = _seq;
            clientStatus = ClientStatus.Invalid;
            status = BrickManDesc.STATUS.PLAYER_WAITING;
            data = new DummyData();
            ip = socket.RemoteEndPoint.ToString().Split(':')[0];
            isLoaded = false;
            isHost = false;
            // First person to join with 127.0.0.1 is host
            if (!ServerEmulator.instance.hasHost && ip.Equals("127.0.0.1"))
            {
                ServerEmulator.instance.hasHost = true;
                isHost = true;
            }
            isVersionSetUp = false;
            buffer = new byte[8192];
            isSteam = false;
            SetupChunkedBuffers();
        }

        public ClientReference(CSteamID _steamID, string _name = "", int _seq = -1)
        {
            lastHeartBeatTime = float.MaxValue;
            loginToleranceTime = 0f;
            steamID = _steamID;
            name = _name;
            seq = _seq;
            clientStatus = ClientStatus.Invalid;
            status = BrickManDesc.STATUS.PLAYER_WAITING;
            data = new DummyData();
            isLoaded = false;
            isHost = SteamManager.Initialized && SteamLobbyManager.instance.IsCurrentOwner(_steamID);
            isVersionSetUp = false;
            buffer = new byte[8192];
            isSteam = true;
            SetupChunkedBuffers();
        }

        private void SetupChunkedBuffers()
        {
            chunkedBufferReceiver.IsServer = true;
            chunkedBufferSender.IsServer = true;
        }

        public bool Disconnect(bool send = true)
        {
            string idInfo = isSteam
                ? ("SteamID=" + steamID.m_SteamID)
                : (socket != null ? socket.RemoteEndPoint.ToString() : "Socket=null");

            if (send && isLoaded)
            {
                try
                {
                    ServerEmulator.instance.SendLeave(this);
                }
                catch (Exception ex)
                {
                    Debug.LogError("[Disconnect] SendLeave failed for " + idInfo + "\n" + ex);
                }

                try
                {
                    if (isSteam)
                        ServerEmulator.instance.SendSlotDataSteam(matchData);
                    else
                        ServerEmulator.instance.SendSlotData(matchData);
                }
                catch (Exception ex)
                {
                    Debug.LogError("[Disconnect] SendSlotData failed for " + idInfo + "\n" + ex);
                }
            }

            if (isSteam)
            {
                try
                {
                    if (SteamManager.Initialized &&
                        SteamNetworkingManager.instance != null &&
                        steamID != CSteamID.Nil)
                    {
                        Debug.Log("[Disconnect] Closing Steam session for " + idInfo);
                        SteamNetworkingManager.instance.CloseSessionWithUser(steamID);
                    }
                    else
                    {
                        Debug.LogWarning("[Disconnect] Steam session close skipped (not initialized or invalid ID) for " + idInfo);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("[Disconnect] CloseSessionWithUser failed for " + idInfo + "\n" + ex);
                }
            }
            else
            {
                try
                {
                    if (socket != null)
                    {
                        Debug.Log("[Disconnect] Closing TCP socket for " + idInfo);
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                    }
                    else
                    {
                        Debug.LogWarning("[Disconnect] Socket already null for " + idInfo);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("[Disconnect] Socket close failed for " + idInfo + "\n" + ex);
                }
            }

            lock (dataLock)
            {
                try
                {
                    if (matchData != null)
                        matchData.RemoveClient(this);
                }
                catch (Exception ex)
                {
                    Debug.LogError("[Disconnect] matchData.RemoveClient failed for " + idInfo + "\n" + ex);
                }

                try
                {
                    if (channel != null)
                        channel.RemoveClient(this);
                }
                catch (Exception ex)
                {
                    Debug.LogError("[Disconnect] channel.RemoveClient failed for " + idInfo + "\n" + ex);
                }

                try
                {
                    bool removed = ServerEmulator.instance.clientList.Remove(this);
                    Debug.Log("[Disconnect] clientList.Remove(" + idInfo + ") => " + removed);
                    return removed;
                }
                catch (Exception ex)
                {
                    Debug.LogError("[Disconnect] clientList.Remove failed for " + idInfo + "\n" + ex);
                }
            }

            return false;
        }

        public bool AssignSlot(SlotData _slot)
        {
            if (_slot.isUsed || _slot.isLocked)
                return false;

            DetachSlot();

            slot = _slot;
            slot.client = this;
            slot.isUsed = true;
            return true;
        }

        public void DetachSlot()
        {
            if (slot == null)
                return;

            slot.client = null;
            slot.isUsed = false;
            slot = null;
        }

        public string GetIdentifier()
        {
            if (isSteam)
                return name + "-" + seq + "-" + steamID;
            else
                return name + "-" + seq + "-" + ip;
        }
    }
}
