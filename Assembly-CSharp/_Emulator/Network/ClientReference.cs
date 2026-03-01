using System;
using System.Collections.Generic;
using System.Net.Sockets;
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
        public Msg4Recv recvBuf;
        public CSteamID steamID = CSteamID.Nil;
        public bool isSteam = false;
        public volatile bool didHeartBeat = false;
        public float heartBeatToleranceTime;
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
        public bool buildModeRequestedMap = false;
        public bool isBreakingInto;
        public ClientStatus clientStatus;
        public BrickManDesc.STATUS status;
        public SlotData slot;
        public Inventory inventory;
        public DummyData data;
        public MatchData matchData;
        public ChannelReference channel;
        public ChunkedBufferReceiver chunkedBufferReceiver = new ChunkedBufferReceiver();
        public int lastOpenedChestSeq = -1;
        public bool isVersionSetUp = false;

        public readonly ServerEmulator emulator;
        public readonly byte sendKey;
        private readonly object dataLock = new object();

        private readonly Action<MsgReference> sendMessage;

        public ClientReference(ServerEmulator emulator, Socket _socket, string _name = "", int _seq = -1)
        {
            this.emulator = emulator;
            this.sendKey = emulator.sendKey;
            heartBeatToleranceTime = 0f;
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
            if (!emulator.hasHost && ip.Equals("127.0.0.1"))
            {
                emulator.hasHost = true;
                isHost = true;
            }
            isVersionSetUp = false;
            recvBuf = new Msg4Recv(new byte[8192]);
            isSteam = false;
            chunkedBufferReceiver.IsServer = true;
            sendMessage = SendMessageToTCP;
        }

        public ClientReference(ServerEmulator emulator, CSteamID _steamID, string _name = "", int _seq = -1)
        {
            this.emulator = emulator;
            this.sendKey = emulator.sendKey;
            heartBeatToleranceTime = 0f;
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
            recvBuf = new Msg4Recv(new byte[8192]);
            isSteam = true;
            chunkedBufferReceiver.IsServer = true;
            sendMessage = SendMessageToSteam;
        }


        public void Send(MsgReference msgRef)
        {
            sendMessage(msgRef);
        }

        private void SendMessageToSteam(MsgReference msgRef)
        {
            if (!msgRef.doChunked || msgRef.msg._msg.Offset <= ChunkedBufferReceiver.MAX_CHUNK_LENGTH)
            {
                WritePacketSteam(new Msg4Send(msgRef.msg._id, uint.MaxValue, uint.MaxValue, msgRef.msg._msg, sendKey));
                return;
            }
            msgRef.msg._msg.SendChunked(msgRef.msg._id, sendKey, "Server", WritePacketSteam);
        }

        private void SendMessageToTCP(MsgReference msgRef)
        {
            if (!msgRef.doChunked || msgRef.msg._msg.Offset <= ChunkedBufferReceiver.MAX_CHUNK_LENGTH)
            {
                WritePacketTcp(new Msg4Send(msgRef.msg._id, uint.MaxValue, uint.MaxValue, msgRef.msg._msg, sendKey));
                return;
            }
            msgRef.msg._msg.SendChunked(msgRef.msg._id, sendKey, "Server", WritePacketTcp);
        }

        public void Disconnect(string message = null, bool serverPrefix = true)
        {
            MsgBody body = new MsgBody();
            if (message != null)
            {
                if (serverPrefix)
                {
                    message = "Disconnected from server:\n" + message;
                }
                body.Write(message);
            }
            Send(new MsgReference(ExtensionOpcodes.opDisconnectAck, body, this));
        }

        public bool CloseClient()
        {
            string idInfo = isSteam
                ? ("SteamID=" + steamID.m_SteamID)
                : (socket != null ? socket.RemoteEndPoint.ToString() : "Socket=null");

            bool removed = false;
            lock (emulator.dataLock)
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
                    removed = emulator.clientList.Remove(this);
                    Debug.Log("[Disconnect] clientList.Remove(" + idInfo + ") => " + removed);
                }
                catch (Exception ex)
                {
                    Debug.LogError("[Disconnect] clientList.Remove failed for " + idInfo + "\n" + ex);
                }
            }

            if (isLoaded)
            {
                // TODO: THIS IS NOT ENOUGH APPARENTLY
                try
                {
                    emulator.SendLeave(this);
                }
                catch (Exception ex)
                {
                    Debug.LogError("[Disconnect] SendLeave failed for " + idInfo + "\n" + ex);
                }

                try
                {
                    if (isSteam)
                        emulator.SendSlotDataSteam(matchData);
                    else
                        emulator.SendSlotData(matchData);
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
                        if (!socket.Connected)
                        {
                            socket.Shutdown(SocketShutdown.Both);
                        }
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

            return removed;
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

        internal void WritePacketTcp(Msg4Send msg)
        {
            if (!socket.Connected)
            {
                CloseClient();
                return;
            }
            // ExtensionOpcodes.opDisconnectAck = 1007
            if (msg.id == 1007)
            {
                // Send disconnect packet synchronously so we know when it was sent fully
                socket.Send(msg.Buffer, 0, msg.Buffer.Length, SocketFlags.None);
                CloseClient();
                return;
            }
            socket.BeginSend(msg.Buffer, 0, msg.Buffer.Length, SocketFlags.None, SendCallback, null);
        }

        private void SendCallback(IAsyncResult result)
        {
            socket.EndSend(result);
        }

        private void WritePacketSteam(Msg4Send msg)
        {
            SteamNetworkingManager.instance.SendMessageToUser(SteamNetworkingChannel.ToClient, steamID, msg);
            if (msg.id == 1007)
            {
                CloseClient();
            }
        }
    }
}
