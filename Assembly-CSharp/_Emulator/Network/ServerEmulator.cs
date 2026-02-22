using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Policy;
using System.Text.RegularExpressions;
using _Emulator.Network;
using _Emulator.Network.Gamemodes;
using Steamworks;
using UnityEngine;
using static MyInfoManager;
using static Room;
using static TItem;
using static WindowSystemManager;
using Debug = UnityEngine.Debug;

namespace _Emulator
{
    public class ServerEmulator : MonoBehaviour
    {
        public static ServerEmulator instance;
        private readonly object dataLock = new object();
        public List<ClientReference> clientList = new List<ClientReference>();
        private Socket serverSocket;
        private byte recvKey = byte.MaxValue;
        private byte sendKey = byte.MaxValue;
        internal Queue<MsgReference> readQueue = new Queue<MsgReference>();
        private Queue<MsgReference> writeQueue = new Queue<MsgReference>();
        private int curSeq = 0;
        public bool debugHandle = false;
        public bool debugSend = false;
        public bool debugPing = false;
        public bool serverCreated = false;
        public bool isSteam = false;
        public bool hasHost = false;
        public EmulatorChannelManager channelManager = new EmulatorChannelManager();
        private float killLogTimer = 0f;
        private float lastUpdateTime = 0f;
        public List<KeyValuePair<int, RegMap>> regMaps = new List<KeyValuePair<int, RegMap>>();
        private bool waitForShutDown = false;
        public readonly Version hostVersion;

        public readonly IGameMode[] gameModes;
        public readonly BuildMapEdit buildMapEdit;
        public readonly PlayBuildAndDestroy playBuildAndDestroy;
        public readonly PlayCaptureTheFlag playCaptureTheFlag;
        public readonly PlayDeathMatch playDeathMatch;
        public readonly PlayDefense playDefense;
        public readonly PlayEscape playEscape;
        public readonly PlayExplosion playExplosion;
        public readonly PlayFreefall playFreefall;
        public readonly PlayTeamDeathMatch playTeamDeathMatch;
        public readonly PlayZombie playZombie;

        private readonly Action<ClientReference, MsgReference> sendToSteam;
        private readonly Action<ClientReference, MsgReference> sendToTCP;

        private Action<ClientReference, MsgReference> sendMessage;

        private readonly Dictionary<ushort, Action<MsgReference>> _handlers = new Dictionary<ushort, Action<MsgReference>>();

        public ServerEmulator()
        {
            hostVersion = ClientExtension.GetGithubVersionOrUnknown();
            gameModes = new IGameMode[10];
            gameModes[(int)ROOM_TYPE.MAP_EDITOR] = buildMapEdit = new BuildMapEdit(this);
            gameModes[(int)ROOM_TYPE.BND] = playBuildAndDestroy = new PlayBuildAndDestroy(this);
            gameModes[(int)ROOM_TYPE.CAPTURE_THE_FLAG] = playCaptureTheFlag = new PlayCaptureTheFlag(this);
            gameModes[(int)ROOM_TYPE.INDIVIDUAL] = playDeathMatch = new PlayDeathMatch(this);
            gameModes[(int)ROOM_TYPE.MISSION] = playDefense = new PlayDefense(this);
            gameModes[(int)ROOM_TYPE.ESCAPE] = playEscape = new PlayEscape(this);
            gameModes[(int)ROOM_TYPE.EXPLOSION] = playExplosion = new PlayExplosion(this);
            gameModes[(int)ROOM_TYPE.BUNGEE] = playFreefall = new PlayFreefall(this);
            gameModes[(int)ROOM_TYPE.TEAM_MATCH] = playTeamDeathMatch = new PlayTeamDeathMatch(this);
            gameModes[(int)ROOM_TYPE.ZOMBIE] = playZombie = new PlayZombie(this);

            sendToSteam = SendMessageToSteam;
            sendToTCP = SendMessageToTCP;
        }
        private void RegisterHandlers()
        {
            Action<MessageId, Action<MsgReference>> register = (messageId, action) => _handlers[(ushort)messageId] = action;
            Action<ExtensionOpcodes, Action<MsgReference>> registerCustom = (messageId, action) => _handlers[(ushort)messageId] = action;
            
            foreach (IGameMode gameMode in gameModes)
            {
                gameMode.RegisterNetworkHandlers(register, registerCustom);
            }

            register(MessageId.CS_LOGIN_REQ, HandleLoginRequest);
            register(MessageId.CS_HEARTBEAT_REQ, HandleHeartbeat);
            register(MessageId.CS_ROOM_LIST_REQ, HandleRoomListRequest);
            register(MessageId.CS_CREATE_ROOM_REQ, HandleCreateRoomRequest);
            register(MessageId.CS_ADD_BRICK_REQ, HandleAddBrickRequest);
            register(MessageId.CS_DEL_BRICK_REQ, HandleDelBrickRequest);
            register(MessageId.CS_CACHE_BRICK_REQ, HandleCacheBrickRequest);
            register(MessageId.CS_CACHE_BRICK_ACK, HandleCacheBrickAck);
            register(MessageId.CS_CACHE_BRICK_DONE_ACK, HandleCacheBrickDoneAck);
            register(MessageId.CS_LEAVE_REQ, HandleLeave);
            register(MessageId.CS_CHAT_REQ, HandleChatRequest);
            register(MessageId.CS_JOIN_REQ, HandleJoinRequest);
            register(MessageId.CS_RESUME_ROOM_REQ, HandleResumeRoomRequest);
            register(MessageId.CS_MORPH_BRICK_REQ, HandleMorphBrickRequest);
            register(MessageId.CS_EQUIP_REQ, HandleEquipRequest);
            register(MessageId.CS_UNEQUIP_REQ, HandleUnequipRequest);
            register(MessageId.CS_SAVE_REQ, HandleSaveMap);
            register(MessageId.CS_LOAD_COMPLETE_REQ, HandleLoadComplete);
            register(MessageId.CS_KILL_LOG_REQ, HandleKillLogRequest);
            register(MessageId.CS_SET_STATUS_REQ, HandleSetStatusRequest);
            register(MessageId.CS_START_REQ, HandleStartRequest);
            register(MessageId.CS_REGISTER_REQ, HandleRegisterMapRequest);
            register(MessageId.CS_CHANGE_USERMAP_ALIAS_REQ, HandleChangeUserMapAliasRequest);
            register(MessageId.CS_RESPAWN_TICKET_REQ, HandleRespawnTicketRequest);
            register(MessageId.CS_TIMER_REQ, HandleTimer);
            register(MessageId.CS_MATCH_COUNTDOWN_REQ, HandleMatchCountdown);
            register(MessageId.CS_BREAK_INTO_REQ, HandleBreakIntoRequest);
            register(MessageId.CS_TEAM_SCORE_REQ, HandleTeamScoreRequest);
            register(MessageId.CS_DESTROY_BRICK_REQ, HandleDestroyBrickRequest);
            register(MessageId.CS_TEAM_CHANGE_REQ, HandleTeamChangeRequest);
            register(MessageId.CS_SLOT_LOCK_REQ, HandleSlotLockRequest);
            register(MessageId.CS_KICK_REQ, HandleKickRequest);
            register(MessageId.CS_ROOM_CONFIG_REQ, HandleRoomConfig);
            register(MessageId.CS_TEAM_CHAT_REQ, HandleTeamChatRequest);
            register(MessageId.CS_RADIO_MSG_REQ, HandleRadioMsgRequest);
            register(MessageId.CS_BUY_ITEM_REQ, HandleBuyRequest);
            register(MessageId.CS_P2P_COMPLETE_REQ, HandleP2PComplete);
            register(MessageId.CS_RESULT_DONE_REQ, HandleResultDoneRequest);
            register(MessageId.CS_ROAMOUT_REQ, HandleRoamout);
            register(MessageId.CS_ROAMIN_REQ, HandleRoamin);
            register(MessageId.CS_GET_CANNON_REQ, HandleGetCannonRequest);
            register(MessageId.CS_EMPTY_CANNON_REQ, HandleEmptyCannonRequest);
            register(MessageId.CS_GET_BACK2SPAWNER_REQ, HandleGetBack2SpawnerRequest);
            register(MessageId.CS_MATCH_RESTART_COUNT_REQ, HandleMatchRestartCountRequest);
            register(MessageId.CS_MATCH_RESTARTED_REQ, HandleMatchRestartRequest);
            register(MessageId.CS_ME_CHG_EDITOR_REQ, HandleChangeEditorPermissionRequest);
            register(MessageId.CS_INIT_TERM_ITEM_REQ, HandleInitItemTermRequest);
            register(MessageId.CS_LINE_BRICK_REQ, HandleLineBrickRequest);
            register(MessageId.CS_REPLACE_BRICK_REQ, HandleReplaceBrickRequest);
            register(MessageId.CS_SET_SHOOTER_TOOL_REQ, HandleSetShooterToolRequest);
            register(MessageId.CS_CLEAR_SHOOTER_TOOLS_REQ, HandleClearShooterTools);
            register(MessageId.CS_REG_MAP_INFO_REQ, HandleRegMapInfoRequest);
            register(MessageId.CS_CORE_HP_REQ, HandleCoreHPReq);
            register(MessageId.CS_WEAPON_HELD_RATIO_REQ, HandleWeaponHeldRatioRequest);
            register(MessageId.CS_TC_OPEN_REQ, HandleTCOpenRequest);
            register(MessageId.CS_TC_ENTER_REQ, HandleCS_TC_ENTER_REQ);
            register(MessageId.CS_TC_OPEN_PRIZE_TAG_REQ, HandleCS_TC_OPEN_PRIZE_TAG_REQ);
            register(MessageId.CS_TC_RECEIVE_PRIZE_REQ, HandleCS_TC_RECEIVE_PRIZE_REQ);
            register(MessageId.CS_ACCEPT_DAILY_MISSION_REQ, Handle_CS_ACCEPT_DAILY_MISSION_REQ);
            register(MessageId.CS_DELEGATE_MASTER_REQ, HandleDelegateMasterRequest);
            register(MessageId.CS_TC_LEAVE_REQ, HandleCS_TC_LEAVE_REQ);
            register(MessageId.CS_INFLICTED_DAMAGE_REQ, HandleInflictedDamage);
            //register(MessageId.CS_RESET_USER_MAP_SLOTS_REQ, HandleResetUserMapSlot);
            register(MessageId.CS_WEAPON_CHANGE_REQ, HandleWeaponChangeRequest);
            register(MessageId.CS_SET_WEAPON_SLOT_REQ, HandleSetWeaponSlotRequest);
            register(MessageId.CS_CLEAR_WEAPON_SLOTS_REQ, HandleClearWeaponSlots);
            register(MessageId.CS_MY_DOWNLOAD_MAP_REQ, HandleRequestDownloadedMaps);
            register(MessageId.CS_MY_REGISTER_MAP_REQ, HandleRequestRegisteredMaps);
            //register(MessageId.CS_USER_MAP_REQ, HandleRequestUserMaps);
            register(MessageId.CS_ALL_MAP_REQ, HandleRequestAllMaps);
            register(MessageId.CS_OPEN_DOOR_REQ, HandleOpenDoorRequest);
            register(MessageId.CS_CLOSE_DOOR_REQ, HandleCloseDoorRequest);
            register(MessageId.CS_SAVE_PLAYER_COMMON_OPT_REQ, HandleCommonOpt);
            register(MessageId.CS_ROOM_REQ, HandleRoomRequest);
            register(MessageId.CS_CHARGE_FORCE_POINT_REQ, HandleChargeForcePoint);
            register(MessageId.CS_CHANNEL_PLAYER_LIST_REQ, HandleRequestUserList);
            register(MessageId.CS_BATCH_DEL_BRICK_REQ, HandleBrickBatchDeleteRequest);
            register(MessageId.CS_MISSION_POINT_REQ, HandleMissionPointRequest);
            register(MessageId.CS_GET_TRAIN_REQ, HandleGetTrainRequest);
            register(MessageId.CS_EMPTY_TRAIN_REQ, HandleEmptyTrainRequest);

            registerCustom(ExtensionOpcodes.opInventoryAck, HandleInventoryData);
            registerCustom(ExtensionOpcodes.opDisconnectReq, HandleDisconnect);
            registerCustom(ExtensionOpcodes.opBeginChunkedBufferReq, HandleBeginChunkedBufferReceive);
            registerCustom(ExtensionOpcodes.opChunkedBufferReq, HandleChunkedBufferReceive);
            registerCustom(ExtensionOpcodes.opEndChunkedBufferReq, HandleEndChunkedBufferReceive);
            registerCustom(ExtensionOpcodes.opVersionCheckReq, HandleVersionCheck);
            registerCustom(ExtensionOpcodes.opBulkBrickReq, HandleBulkBrickRequest);
            registerCustom(ExtensionOpcodes.opAmIConnectedReq, HandleAmIConnected);

            Type messageIdType = typeof(MessageId);
            foreach (MessageId id in Enum.GetValues(messageIdType))
            {
                ushort mapId = (ushort)id;
                if (_handlers.ContainsKey(mapId))
                {
                    continue;
                }
                string name = Enum.GetName(messageIdType, id);
                _handlers[mapId] = msgRef => Debug.LogWarning($"Client {msgRef.client.GetIdentifier()} sent unhandled packet: {name} ({mapId})");
            }
            messageIdType = typeof(ExtensionOpcodes);
            foreach (ExtensionOpcodes id in Enum.GetValues(messageIdType))
            {
                ushort mapId = (ushort)id;
                if (_handlers.ContainsKey(mapId))
                {
                    continue;
                }
                string name = Enum.GetName(messageIdType, id);
                _handlers[mapId] = msgRef => Debug.LogWarning($"Client {msgRef.client.GetIdentifier()} sent unhandled packet: {name} ({mapId})");
            }
        }

        public void SetupServer()
        {
            RegisterHandlers();
            isSteam = false;
            hasHost = false;
            try
            {
                sendMessage = sendToTCP;
                if (serverSocket == null)
                {
                    serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    serverSocket.Bind(new IPEndPoint(IPAddress.Any, 5000));
                }
                serverSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                serverSocket.Listen(16);
                serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
                lastUpdateTime = Time.time;
                serverCreated = true;
                Debug.Log("Server created");
            }

            catch (Exception ex)
            {
                Debug.LogError("SetupServer: " + ex.Message);
            }

            //Pulls all loaded RegMaps into the emulator
            regMaps = RegMapManager.Instance.dicRegMap.ToList();
        }

        public void SetupServerSteam()
        {
            RegisterHandlers();
            isSteam = true;
            hasHost = false;
            lastUpdateTime = Time.time;
            serverCreated = true;
            sendMessage = sendToSteam;
            Debug.Log("Server set to Steam");

            //Pulls all loaded RegMaps into the emulator
            regMaps = RegMapManager.Instance.dicRegMap.ToList();
        }

        private void AcceptCallback(IAsyncResult result)
        {
            try
            {
                Socket clientSocket = serverSocket.EndAccept(result);

                ClientReference client = new ClientReference(this, clientSocket);
                if (!Config.instance.blockConnections)
                {
                    if (HandleClientAccepted(client))
                        clientSocket.BeginReceive(client.recvBuf.Buffer, client.recvBuf.Io, client.recvBuf.Buffer.Length - client.recvBuf.Io, SocketFlags.None, new AsyncCallback(ReceiveCallback), client);
                    else
                        SendDisconnect(client);
                }

                else
                    SendDisconnect(client);
            }

            catch (Exception ex)
            {
                Debug.LogError("AcceptCallback: " + ex.Message);
            }

            finally
            {
                serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                ClientReference client = (ClientReference)result.AsyncState;
                int bytes = client.socket.EndReceive(result);
                if (bytes > 0)
                {
                    client.recvBuf.Io += bytes;
                    for (Msg4Recv.MsgStatus status = client.recvBuf.GetStatus(recvKey); status == Msg4Recv.MsgStatus.COMPLETE; status = client.recvBuf.GetStatus(recvKey))
                    {
                        MsgBody msgBody = client.recvBuf.Flush();
                        msgBody.Decrypt(recvKey);
                        lock (dataLock)
                        {
                            readQueue.Enqueue(new MsgReference(new Msg2Handle(client.recvBuf.GetId(), msgBody), client, _channelRef: client.channel, _matchData: client.matchData));
                        }
                    }
                    client.socket.BeginReceive(client.recvBuf.Buffer, client.recvBuf.Io, client.recvBuf.Buffer.Length - client.recvBuf.Io, SocketFlags.None, new AsyncCallback(ReceiveCallback), client);
                }
            }

            catch (Exception ex)
            {
                Debug.LogError(ex);
                Debug.LogError("ReceiveCallback: " + ex.Message);
            }
        }

        internal void SendCallback(IAsyncResult result)
        {
            Socket clientSocket = (Socket)result.AsyncState;
            clientSocket.EndSend(result);
        }

        public void AcceptSteam(CSteamID steamID)
        {
            if (!serverCreated || !isSteam || !SteamManager.Initialized)
                return;

            lock (SteamLobbyManager.instance.currentLobbyLock)
            {
                if (SteamLobbyManager.instance.currentLobby != null)
                {
                    string name = SteamLobbyManager.instance.currentLobby.GetMemberName(steamID);
                    ClientReference client = new ClientReference(this, steamID, name);

                    if (!Config.instance.blockConnections)
                    {
                        if (!HandleClientAccepted(client))
                            SendDisconnect(client);
                    }

                    else
                        SendDisconnect(client);
                }
            }
        }

        public void ReceiveSteam(CSteamID steamID, byte[] msg)
        {
            if (!isSteam)
                return;

            if (msg == null)
            {
                Debug.LogError("ReceiveSteam: msg was null");
                return;
            }

            if (msg.Length < 15)
            {
                Debug.LogError("ReceiveSteam: msg length was " + msg.Length);
                return;
            }

            try
            {
                ClientReference client = FindClientBySteamID(steamID);
                if (client != null)
                {
                    Msg4Recv recv = new Msg4Recv(msg);
                    recv._hdr.FromArray(recv.Buffer);
                    MsgBody msgBody = recv.Flush();
                    msgBody.Decrypt(recvKey);

                    lock (dataLock)
                    {
                        readQueue.Enqueue(new MsgReference(new Msg2Handle(recv.GetId(), msgBody), client, _channelRef: client.channel, _matchData: client.matchData));
                    }
                } else
                {
                    Debug.LogWarning("ReceiveSteam: no ClientReference for " + steamID + " (closing session)");
                    if (SteamManager.Initialized && SteamNetworkingManager.instance != null)
                        SteamNetworkingManager.instance.CloseSessionWithUser(steamID);
                    return;
                }
            }

            catch (Exception ex)
            {
                Debug.LogError("ReceiveSteam: " + ex.Message);
            }
        }

        private void SendMessageToSteam(ClientReference clientRef, MsgReference msgRef)
        {
            if (!msgRef.doChunked || msgRef.msg._msg.Offset <= ChunkedBufferReceiver.MAX_CHUNK_LENGTH)
            {
                clientRef.WritePacketSteam(new Msg4Send(msgRef.msg._id, uint.MaxValue, uint.MaxValue, msgRef.msg._msg, sendKey));
                return;
            }
            msgRef.msg._msg.SendChunked(msgRef.msg._id, sendKey, "Server", clientRef.WritePacketSteam);
        }

        private void SendMessageToTCP(ClientReference clientRef, MsgReference msgRef)
        {
            if (!msgRef.doChunked || msgRef.msg._msg.Offset <= ChunkedBufferReceiver.MAX_CHUNK_LENGTH)
            {
                clientRef.WritePacketTcp(new Msg4Send(msgRef.msg._id, uint.MaxValue, uint.MaxValue, msgRef.msg._msg, sendKey));
                return;
            }
            msgRef.msg._msg.SendChunked(msgRef.msg._id, sendKey, "Server", clientRef.WritePacketTcp);
        }

        private void UnicastMessage(MsgReference msgRef)
        {
            sendMessage(msgRef.client, msgRef);
        }

        private void BroadcastToClientList(List<ClientReference> list, MsgReference msgRef)
        {
            for (int i = 0; i < list.Count; i++)
            {
                sendMessage(list[i], msgRef);
            }
        }

        private void BroadcastToClientList(List<ClientReference> list, MsgReference msgRef, Predicate<ClientReference> filter)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (filter.Invoke(list[i]))
                    continue;
                sendMessage(list[i], msgRef);
            }
        }

        private void BroadcastMessage(MsgReference msgRef)
        {
            BroadcastToClientList(clientList, msgRef);
        }

        private void BroadcastChannelMessage(MsgReference msgRef)
        {
            BroadcastToClientList(msgRef.channelRef.clientList, msgRef);
        }

        private void BroadcastRoomMessage(MsgReference msgRef)
        {
            BroadcastToClientList(msgRef.matchData.clientList, msgRef, client => client.clientStatus < ClientReference.ClientStatus.Room);
        }

        private void BroadcastRedTeamMessage(MsgReference msgRef)
        {
            BroadcastToClientList(msgRef.matchData.clientList, msgRef, client => client.clientStatus < ClientReference.ClientStatus.Room || !client.slot.isRed);
        }

        private void BroadcastBlueTeamMessage(MsgReference msgRef)
        {
            BroadcastToClientList(msgRef.matchData.clientList, msgRef, client => client.clientStatus < ClientReference.ClientStatus.Room || client.slot.isRed);
        }

        private void BroadcastRoomMessageExclusive(MsgReference msgRef)
        {
            BroadcastToClientList(msgRef.matchData.clientList, msgRef, client => client.seq == msgRef.client.seq || client.clientStatus < ClientReference.ClientStatus.Room);
        }

        private ClientReference FindClientBySocket(Socket clientSocket)
        {
            ClientReference client = clientList.Find(x => x.socket == clientSocket);
            if (client == null)
                Debug.LogError("FindClientBySocket: Could not find ClientReference for client: " + clientSocket.RemoteEndPoint.ToString());
            return client;
        }

        private ClientReference FindClientBySteamID(CSteamID steamID)
        {
            ClientReference client = clientList.Find(x => x.steamID == steamID);
            if (client == null)
                Debug.LogError("FindClientBySteamID: Could not find ClientReference for client: " + steamID);
            return client;
        }

        public bool ClientExistsSteam(CSteamID steamID)
        {
            if (!serverCreated || !isSteam)
                return false;

            return clientList.Exists(x => x.steamID == steamID);
        }

        public void ShutdownInit()
        {
            channelManager.Shutdown();
            channelManager = new EmulatorChannelManager();
            //matchData = new MatchData();
            curSeq = 0;
            SendDisconnect(null, SendType.Broadcast);
            waitForShutDown = true;
        }

        public void ShutdownFinally()
        {
            lock (dataLock)
            {
                waitForShutDown = false;
                if (!isSteam && serverSocket != null)
                {
                    try
                    {
                        // Only attempt shutdown if the socket still claims to be connected
                        if (serverSocket.Connected)
                        {
                            serverSocket.Shutdown(SocketShutdown.Both);
                        }
                    }
                    catch (SocketException sockEx)
                    {
                        Debug.LogError("Encountered SocketException during Shutdown: " + sockEx.Message);
                    }
                    catch (ObjectDisposedException objDisEx)
                    {
                        Debug.LogError("Encountered ObjectDisposedException during Shutdown: " + objDisEx.Message);
                    }
                    try
                    {
                        serverSocket.Close();
                    }
                    catch(Exception ex) {
                        Debug.LogError("Encountered Excpetion during Shutdown: " + ex.Message);
                    }
                    serverSocket = null;
                }
                ClearBuffers();
                clientList.Clear();
                serverCreated = false;
            }
        }

        public void ClearBuffers()
        {
            lock (dataLock)
            {
                writeQueue.Clear();
                readQueue.Clear();
            }
        }

        public void Say(MsgReference msg)
        {
            lock (dataLock)
            {
                writeQueue.Enqueue(msg);
            }
        }

        public void SayInstant(MsgReference msg)
        {
            lock (dataLock)
            {
                writeQueue.Enqueue(msg);
                SendMessages();
            }
        }

        private void Update()
        {
            if (!serverCreated)
                return;

            lock (dataLock)
            {
                if (waitForShutDown && (clientList.Count == 0 || isSteam))
                    ShutdownFinally();

                killLogTimer += Time.deltaTime;
                HandleMessages();
                SendMessages();
            }
        }

        private void FixedUpdate()
        {
            if (!serverCreated)
                return;
            HandleClientUpdates();
        }

        public void Reset()
        {
            try
            {
                ClearBuffers();
                if (channelManager != null && channelManager.channels != null)
                {
                    foreach (var channel in channelManager.channels)
                    {
                        if (channel != null && channel.matches != null)
                        {
                            foreach (var match in channel.matches)
                            {
                                if (match != null)
                                {
                                    SendDeleteRoom(match, match.channel);
                                    channel.RemoveMatch(match);
                                }
                            }
                        }
                    }
                }

                channelManager.Shutdown();
                channelManager = new EmulatorChannelManager();
            }
            catch { }

            //matchData.Reset();
            //matchData = new MatchData();
            foreach (ClientReference client in clientList)
            {
                SendChannels(client);
                SendKick(client);
                SendRoomList(client);
            }
            SendCustomMessage("Reset By Host");
        }

        private void HandleClientUpdates()
        {
            float time = Time.time, delta = time - lastUpdateTime;
            ClientReference clientRef;
            for (int i = clientList.Count - 1; i >= 0; i--)
            {
                clientRef = clientList[i];
                if (clientRef.isHost) { continue; }
                // Handle dead clients
                if (clientRef.seq == -1)
                {
                    if (clientRef.loginToleranceTime < 4f)
                    {
                        clientRef.loginToleranceTime += delta;
                        continue;
                    }
                    SendDisconnect(clientRef, message: "Login timedout");
                    clientRef.Disconnect(false);
                    if (ServerEmulator.instance.debugHandle)
                        Debug.Log("[Disconnect] Client login timed out: " + clientRef.GetIdentifier());
                    continue;
                }
                // Handle client heartbeat
                if (clientRef.didHeartBeat)
                {
                    clientRef.lastHeartBeatTime = time;
                    clientRef.didHeartBeat = false;
                } else if (time - clientRef.lastHeartBeatTime > 7.5f)
                {
                    if (ServerEmulator.instance.debugHandle)
                        Debug.Log("[Disconnect] Client timed out: " + clientRef.GetIdentifier());
                    clientRef.Disconnect(true);
                    continue;
                }
            }
            lastUpdateTime = time;
        }

        public bool GetGamestateStrings(out string roomType, out string roomStatus, out string mapAlias)
        {
            if (channelManager != null && channelManager.channels != null)
            {
                foreach (var channel in channelManager.channels)
                {
                    if (channel != null && channel.matches != null)
                    {
                        foreach (var match in channel.matches)
                        {
                            if (match != null && match.room != null)
                            {
                                roomType = BfUtils.RoomTypeToString(match.room.Type);
                                roomStatus = BfUtils.RoomStatusToString(match.room.Status);
                                if (match.room.Type == ROOM_TYPE.MAP_EDITOR)
                                    mapAlias = UserMapInfoManager.Instance.CurMapName;
                                else
                                    mapAlias = match.room.CurMapAlias;
                                return true;
                            }
                        }
                    }
                }
            }

            roomType = "None";
            roomStatus = "None";
            mapAlias = "None";
            return false;
        }

        public void EndAllMatches()
        {
            if (channelManager != null && channelManager.channels != null)
            {
                foreach (var channel in channelManager.channels)
                {
                    if (channel != null && channel.matches != null)
                    {
                        foreach (var match in channel.matches)
                        {
                            if (match != null)
                            {
                                match.EndMatch();
                            }
                        }
                    }
                }
            }
        }

        public int GetMatchCount()
        {
            var result = 0;
            if (channelManager != null && channelManager.channels != null)
            {
                foreach (var channel in channelManager.channels)
                {
                    if (channel != null && channel.matches != null)
                    {
                        foreach (var match in channel.matches)
                        {
                            if (match != null)
                            {
                                result++;
                            }
                        }
                    }
                }
            }

            return result;
        }

        private void HandleMessages()
        {
            if (readQueue.Count < 1)
                return;

            MsgReference msgRef = readQueue.Peek();

            try
            {   
                if (debugSend)
                    Debug.Log($"[Verbose] Processing message ID: {msgRef.msg._id} from client: {msgRef.client.GetIdentifier()}");
                Action<MsgReference> handler;
                if (_handlers.TryGetValue(msgRef.msg._id, out handler))
                {
                    handler(msgRef);
                }
                else
                {
                    Debug.LogWarning("No handler for message ID: " + msgRef.msg._id);
                }
            }

            catch (Exception ex)
            {
                Debug.LogError("HandleMessages: " + ex.Message);
                Debug.LogError("HandleMessages StackTrace: " + ex.StackTrace);
            }

            finally
            {
                readQueue.Dequeue();
            }
        }

        private void SendMessages()
        {
            if (writeQueue.Count < 1)
                return;

            MsgReference msgRef = writeQueue.Peek();

            try
            {
                switch (msgRef.sendType)
                {
                    case SendType.Unicast:
                        UnicastMessage(msgRef);
                        break;

                    case SendType.Broadcast:
                        BroadcastMessage(msgRef);
                        break;

                    case SendType.BroadcastChannel:
                        BroadcastChannelMessage(msgRef);
                        break;

                    case SendType.BroadcastRoom:
                        BroadcastRoomMessage(msgRef);
                        break;

                    case SendType.BroadcastRoomExclusive:
                        BroadcastRoomMessageExclusive(msgRef);
                        break;

                    case SendType.BroadcastRedTeam:
                        BroadcastRedTeamMessage(msgRef);
                        break;

                    case SendType.BroadcastBlueTeam:
                        BroadcastBlueTeamMessage(msgRef);
                        break;
                }
            }

            catch (Exception ex)
            {
                Debug.LogError("SendMessages: " + ex.Message);
                if (debugHandle)
                    Debug.LogError("SendMessages StackTrace: " + ex.StackTrace);
            }

            finally
            {
                writeQueue.Dequeue();
            }
        }

        private bool HandleClientAccepted(ClientReference client)
        {
            lock (dataLock)
            {
                bool nonExisting = false;
                if (isSteam)
                {
                    var existingClient = clientList.Find(x => x.steamID == client.steamID);
                    nonExisting = existingClient == null;
                    if (!nonExisting)
                        nonExisting = existingClient.Disconnect(true);

                    //nonExisting = !clientList.Exists(x => x.steamID == client.steamID);
                }
                else
                    nonExisting = !clientList.Exists(x => x.socket == client.socket) && (!Config.instance.oneClientPerIP || !clientList.Exists(x => x.ip == client.ip));

                if (!Config.instance.blockConnections && clientList.Count < Config.instance.maxConnections && nonExisting)
                {
                    clientList.Add(client);
                    SendConnected(client);
                    return true;
                }

                else
                {
                    Debug.Log("HandleClientAccepted: Blocked Client " + client.GetIdentifier() + " from Connecting");
                    return false;
                }
            }
        }

        private void HandleAmIConnected(MsgReference msgRef)
        {
            msgRef.client.didHeartBeat = true;
            SayInstant(new MsgReference(ExtensionOpcodes.opAmIConnectedAck, null, msgRef.client));
        }

        private void HandleHeartbeat(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out int gmFunction);
            msgRef.client.didHeartBeat = true;
        }

        private void HandleLoginRequest(MsgReference msgRef)
        {
            if (!msgRef.client.isVersionSetUp)
            {
                Debug.LogWarning($"Disconnecting client: Version check failed, client didn't send any version");
                SendDisconnect(msgRef.client, message: $"Version mismatch detected, the host is using a newer version ({hostVersion})");
                msgRef.client.Disconnect(false);
                return;
            }

            msgRef.msg._msg.Read(out string id);
            msgRef.msg._msg.Read(out string pswd);
            msgRef.msg._msg.Read(out int major);
            msgRef.msg._msg.Read(out int minor);
            msgRef.msg._msg.Read(out string privateIpAddress);
            msgRef.msg._msg.Read(out string macAddress);

            msgRef.client.name = id;
            msgRef.client.seq = curSeq++;
            msgRef.client.port = 6000 + msgRef.client.seq;

            ChannelReference channel = channelManager.GetDefaultChannel();
            channel.AddClient(msgRef.client);

            SendPlayerInitInfo(msgRef.client);
            SendChannels(msgRef.client);
            SendCurChannel(msgRef.client, channel.channel.Id);
            SendInventoryRequest(msgRef.client);
            SendLogin(msgRef.client, channel.channel.Id);
            SendPlayerInfo(msgRef.client);
            //SendAllDownloadedMaps(msgRef.client);
            //SendUserMapSlots(msgRef.client);
            //SendAllUserMaps(msgRef.client);
        }

        private void HandleLoadComplete(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out int crc);

            msgRef.client.isLoaded = true;

            if (debugHandle)
                Debug.Log("HandleLoadComplete from: " + msgRef.client.GetIdentifier());
        }

        private void HandleTimer(MsgReference msgRef)
        {
            MatchData matchData = msgRef.matchData;

            msgRef.msg._msg.Read(out int remainTime);
            msgRef.msg._msg.Read(out int playTime);

            if (msgRef.client.seq == matchData.masterSeq)
            {
                matchData.remainTime = remainTime;
                matchData.playTime = playTime;
            }

            if (debugPing)
                Debug.Log("HandleTimer from: " + msgRef.client.GetIdentifier());
            if (matchData.room.type == Room.ROOM_TYPE.BND)
            {
                if (matchData.repeat <= 0)
                {
                    matchData.EndMatch();
                }
            }
            else if (matchData.room.type == Room.ROOM_TYPE.EXPLOSION)
            {
                if (matchData.remainTime <= 0)
                {
                    if (matchData.redScore >= matchData.room.goal || matchData.blueScore >= matchData.room.goal)
                    {
                        matchData.EndMatch();
                    }
                    else
                    {
                        //round code blue team wins when time runs out
                        //this could be wrong and needs to be checked and referenced in exlplosionMatch.cs
                        matchData.blueScore++;
                        playExplosion.HandleRoundEnd(msgRef, 1);
                    }
                }
            }
            else if (matchData.room.type == Room.ROOM_TYPE.ZOMBIE)
            {
                if (matchData.remainTime <= 0 && matchData.zombieRoundsLeft >= 0)
                {
                    playZombie.SendZombieRoundEnd(msgRef, matchData);
                }
            }
            else
            {
                if (matchData.remainTime <= 0)
                    matchData.EndMatch();
            }

            SendTimer(msgRef.client);
        }

        private void HandleMatchCountdown(MsgReference msgRef)
        {
            MatchData matchData = msgRef.matchData;

            msgRef.msg._msg.Read(out int countdownTime);

            if (debugHandle)
                Debug.Log("HandleMatchCountdown from: " + msgRef.client.GetIdentifier());

            if (msgRef.client.seq == matchData.masterSeq)
            {
                matchData.countdownTime = countdownTime;
                if (matchData.countdownTime <= 0)
                {
                    matchData.room.Status = Room.ROOM_STATUS.PLAYING;
                    SendUpdateRoom(matchData);
                }

                SendMatchCountdown(matchData);
            }
        }

        private void HandleRoamout(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out int dest);

            if (debugHandle)
                Debug.Log("HandleRoamout from: " + msgRef.client.GetIdentifier());

            ChannelReference channelRef = channelManager.GetChannelByID(dest);
            if (channelRef != null)
            {
                SendCurChannel(msgRef.client, channelRef.channel.Id);
                channelRef.AddClient(msgRef.client);
                SendUserList(msgRef.client);
                SendRoamin(msgRef.client, channelRef.channel.Id);
            }

            msgRef.client.clientStatus = ClientReference.ClientStatus.Lobby;
        }

        private void HandleRoamin(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out int seq);
            msgRef.msg._msg.Read(out int userType);
            msgRef.msg._msg.Read(out bool isWebPlayer);
            msgRef.msg._msg.Read(out int language);
            msgRef.msg._msg.Read(out string hashCode);

            if (debugHandle)
                Debug.Log("HandleRoamin from: " + msgRef.client.GetIdentifier());

            SendUserList(msgRef.client);
            SendRoamin(msgRef.client, msgRef.client.channel.channel.Id);

            msgRef.client.clientStatus = ClientReference.ClientStatus.Lobby;
        }

        private void HandleRequestDownloadedMaps(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out int prevPage);
            msgRef.msg._msg.Read(out int nextPage);
            msgRef.msg._msg.Read(out int indexer);
            msgRef.msg._msg.Read(out ushort modeMask);

            if (debugHandle)
                Debug.Log("HandleRequestDownloadedMaps from: " + msgRef.client.GetIdentifier());

            SendDownloadedMaps(msgRef.client, nextPage);
        }

        private void HandleRequestRegisteredMaps(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out int prevPage);
            msgRef.msg._msg.Read(out int nextPage);
            msgRef.msg._msg.Read(out int indexer);
            msgRef.msg._msg.Read(out ushort modeMask);

            if (debugHandle)
                Debug.Log("HandleRequestRegisteredMaps from: " + msgRef.client.GetIdentifier());

            //SendRegisteredMaps(msgRef.client, nextPage);
        }

        private void HandleRequestAllMaps(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out int prevPage);
            msgRef.msg._msg.Read(out int nextPage);
            msgRef.msg._msg.Read(out int indexer);
            msgRef.msg._msg.Read(out ushort modeMask);
            msgRef.msg._msg.Read(out int flag);
            msgRef.msg._msg.Read(out string filter);

            if (debugHandle)
                Debug.Log("HandleRequestRegisteredMaps from: " + msgRef.client.GetIdentifier());

            SendAllMaps(msgRef.client, nextPage);
        }

        public void SendAllMaps(ClientReference client, int page)
        {
            MsgBody body = new MsgBody();

            const int mapsPerPage = 12;
            int offset = page * mapsPerPage;
            int remaining = regMaps.Count - offset;
            int count = remaining < mapsPerPage ? remaining : mapsPerPage;

            body.Write(page); //page
            body.Write(count); //count
            for (int i = offset; i < offset + count; i++)
            {
                KeyValuePair<int, RegMap> entry = regMaps[i];
                body.Write(entry.Value.Map);
                body.Write(entry.Value.Developer);
                body.Write(entry.Value.Alias);
                body.Write(entry.Value.ModeMask);
                body.Write((byte)(Room.clanMatch | Room.official));
                body.Write(entry.Value.tagMask);
                body.Write(entry.Value.RegisteredDate.Year);
                body.Write((sbyte)entry.Value.RegisteredDate.Month);
                body.Write((sbyte)entry.Value.RegisteredDate.Day);
                body.Write((sbyte)entry.Value.RegisteredDate.Hour);
                body.Write((sbyte)entry.Value.RegisteredDate.Minute);
                body.Write((sbyte)entry.Value.RegisteredDate.Second);
                body.Write(entry.Value.DownloadFee);
                body.Write(entry.Value.Release);
                body.Write(entry.Value.LatestRelease);
                body.Write(entry.Value.Likes);
                body.Write(entry.Value.DisLikes);
                body.Write(entry.Value.DownloadCount);
            }
            Say(new MsgReference(432, body, client));

            if (debugSend)
                Debug.Log("SendRegisteredMaps to: " + client.GetIdentifier());
        }

        private void HandleRequestUserMaps(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out int page);

            if (debugHandle)
                Debug.Log("HandleRequestUserMaps from: " + msgRef.client.GetIdentifier());

            //SendUserMapSlots(msgRef.client);
            //SendUserMaps(msgRef.client, page);
        }

        private void HandleRequestUserList(MsgReference msgRef)
        {
            if (debugPing)
                Debug.Log("HandleRequestUserList from: " + msgRef.client.GetIdentifier());

            SendUserList(msgRef.client);
        }

        private void HandleJoinRequest(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out int roomNumber);
            msgRef.msg._msg.Read(out string pswd);
            msgRef.msg._msg.Read(out bool invite);

            if (debugHandle)
                Debug.Log("HandleJoin from: " + msgRef.client.GetIdentifier());

            MatchData matchData = msgRef.client.channel.GetMatchByRoomNumber(roomNumber);
            if (roomNumber == matchData.room.No)
            {
                matchData.AddClient(msgRef.client);

                SendJoin(msgRef.client);

                if (isSteam)
                    SendRendezvousInfoSteam(msgRef.client);
                else
                    SendRendezvousInfo(msgRef.client);
                SendMaster(msgRef.client, matchData);
                SendSlotLocks(msgRef.client);
                SendRoomConfig(msgRef.client);
                SendUpdateRoom(matchData, msgRef.client);

                if (isSteam)
                    SendEnterSteam(msgRef.client);
                else
                    SendEnter(msgRef.client);

                if (isSteam)
                    SendSlotDataSteam(matchData);
                else
                    SendSlotData(matchData);

                if (matchData.room.Type == Room.ROOM_TYPE.MAP_EDITOR)
                    SendCopyright(msgRef.client);
            }
        }

        private void HandleBreakIntoRequest(MsgReference msgRef)
        {
            if (debugHandle)
                Debug.Log("HandleBreakInto from: " + msgRef.client.GetIdentifier());

            MatchData matchData = msgRef.matchData;

            int reply = 0;
            bool sendMap = false;

            if (matchData.room.Type == Room.ROOM_TYPE.MAP_EDITOR)
            {
                if (!matchData.cachedMap.isLoaded)
                {
                    SendBreakInto(msgRef.client, -1);
                    return;
                }
                sendMap = true;
            }

            if (!matchData.room.isBreakInto)
                reply = -1;

            else if (matchData.room.Status != Room.ROOM_STATUS.PLAYING)
                reply = -2;

            else
            {
                msgRef.client.status = BrickManDesc.STATUS.PLAYER_LOADING;
                msgRef.client.clientStatus = ClientReference.ClientStatus.Match;
                SendSetStatus(msgRef.client);
                SendTeamScore(matchData);
                for (int i = 0; i < matchData.clientList.Count; i++)
                {
                    SendKillCount(matchData.clientList[i]);
                    SendDeathCount(matchData.clientList[i]);
                }
                if (matchData.room.Type == ROOM_TYPE.BND)
                {
                    playBuildAndDestroy.SendBnDStatus(msgRef.client);
                    SendUpdateRoom(matchData, msgRef.client);
                }
                SendTimer(msgRef.client);
                msgRef.client.isBreakingInto = true;
            }

            SendBreakInto(msgRef.client, reply);

            if (sendMap)
            {
                SendCacheBrick(msgRef.client);
                SendCacheBrickDone(msgRef.client);
            }
        }

        private void HandleLeave(MsgReference msgRef)
        {
            MatchData matchData = msgRef.matchData;

            if (debugHandle)
                Debug.Log("HandleLeave from: " + msgRef.client.GetIdentifier());

            SendLeave(msgRef.client);
            SendSetStatus(msgRef.client);

            matchData.RemoveClient(msgRef.client);

            if (matchData.room.Type == ROOM_TYPE.MAP_EDITOR)
            {
                if (msgRef.client.seq == matchData.masterSeq)
                {
                    if (matchData.room.CurPlayer > 0)
                    {
                        ClientReference[] clients = matchData.clientList.ToArray();
                        foreach (ClientReference client in clients)
                        {
                            SendKick(client);
                            SendRoomList(client);
                        }
                    }
                    SendDeleteRoom(matchData, matchData.channel);
                    msgRef.client.channel.RemoveMatch(matchData);
                }
                return;
            }

            if (matchData.room.CurPlayer <= 0)
            {
                SendDeleteRoom(matchData, matchData.channel);
                msgRef.client.channel.RemoveMatch(matchData);
                return;
            }

            if (msgRef.client.seq == matchData.masterSeq)
            {
                matchData.masterSeq = matchData.clientList[0].seq;
                SendMaster(null, matchData);
            }
        }

        private void HandleCreateRoomRequest(MsgReference msgRef)
        {
            if (Config.instance.onlyHostRooms && !msgRef.client.isHost)
            {
                SendCustomMessage("Only host can create rooms.", msgRef.client, SendType.Unicast);
                return;
            }

            if (Config.instance.maxNumRooms >= 0 && GetMatchCount() >= Config.instance.maxNumRooms)
            {
                SendCustomMessage("Max num rooms reached (" + Config.instance.maxNumRooms + ").", msgRef.client, SendType.Unicast);
                return;
            }

            MatchData matchData = msgRef.client.channel.AddNewMatch();

            msgRef.msg._msg.Read(out int type);
            msgRef.msg._msg.Read(out string title);
            msgRef.msg._msg.Read(out bool isLocked);
            msgRef.msg._msg.Read(out string pswd);
            msgRef.msg._msg.Read(out int maxPlayer);
            int[] parameters = new int[8];
            msgRef.msg._msg.Read(out parameters[0]);
            msgRef.msg._msg.Read(out parameters[1]);
            msgRef.msg._msg.Read(out parameters[2]);
            msgRef.msg._msg.Read(out parameters[3]);
            msgRef.msg._msg.Read(out parameters[4]);
            msgRef.msg._msg.Read(out parameters[5]);
            msgRef.msg._msg.Read(out parameters[6]);
            msgRef.msg._msg.Read(out parameters[7]);
            msgRef.msg._msg.Read(out string alias);
            msgRef.msg._msg.Read(out int master);

            Room.ROOM_TYPE roomType = (Room.ROOM_TYPE)type;

            matchData.room.Type = roomType;
            matchData.room.Title = title;
            matchData.room.Locked = isLocked;
            matchData.room.MaxPlayer = maxPlayer;
            matchData.room.CurMapAlias = alias;
            matchData.masterSeq = msgRef.client.seq;
            matchData.LockSlotsByMaxPlayers(matchData.room.MaxPlayer, roomType);
            matchData.roomCreated = true;

            if (roomType != ROOM_TYPE.NONE && roomType != ROOM_TYPE.NUM_TYPE)
            {
                gameModes[type].HandleRoomCreation(msgRef.client, matchData, matchData.room, parameters);
            }

            if (debugHandle)
                Debug.Log("HandleCreateRoom from: " + msgRef.client.GetIdentifier());

            matchData.AddClient(msgRef.client);

            if (isSteam)
                SendRendezvousInfoSteam(msgRef.client);
            else
                SendRendezvousInfo(msgRef.client);

            SendMaster(msgRef.client, matchData);
            SendSlotLocks(msgRef.client);
            SendRoomConfig(msgRef.client);
            SendAddRoom(msgRef.client, matchData);
            SendCreateRoom(msgRef.client);

            if (isSteam)
                SendEnterSteam(msgRef.client);
            else
                SendEnter(msgRef.client);

            if (isSteam)
                SendSlotDataSteam(matchData);
            else
                SendSlotData(matchData);

            if ((Room.ROOM_TYPE)type == Room.ROOM_TYPE.MAP_EDITOR)
                SendCopyright(msgRef.client);
        }

        private void HandleRoomConfig(MsgReference msgRef)
        {
            MatchData matchData = msgRef.matchData;

            msgRef.msg._msg.Read(out int killCount);
            msgRef.msg._msg.Read(out int timeLimit);
            msgRef.msg._msg.Read(out int weaponOption);
            msgRef.msg._msg.Read(out int nWhere);
            msgRef.msg._msg.Read(out int breakInto);
            msgRef.msg._msg.Read(out int teamBalance);
            msgRef.msg._msg.Read(out int useBuildGun);
            msgRef.msg._msg.Read(out int itemPickup);
            msgRef.msg._msg.Read(out string whereAlias);
            msgRef.msg._msg.Read(out string pswd);
            msgRef.msg._msg.Read(out int type);

            matchData.room.goal = killCount;
            matchData.room.timelimit = timeLimit;
            matchData.room.weaponOption = weaponOption;
            matchData.room.map = nWhere;
            matchData.room.isBreakInto = Convert.ToBoolean(breakInto);
            matchData.isBalance = Convert.ToBoolean(teamBalance);
            matchData.room.isDropItem = Convert.ToBoolean(itemPickup);
            matchData.room.CurMapAlias = whereAlias;
            matchData.room.Type = (Room.ROOM_TYPE)type;

            if ((Room.ROOM_TYPE)type == Room.ROOM_TYPE.BUNGEE)
            {
                matchData.CacheMap(regMaps.Find(x => x.Value.Map == nWhere).Value, new UserMapInfo(0, 0));
            }

            if ((Room.ROOM_TYPE)type == Room.ROOM_TYPE.BND)
            {
                // Unpack the timer configuration for Build and Destroy phases
                int buildTime, destroyTime, repeat;
                PlayBuildAndDestroy.UnpackTimerOption(timeLimit, out buildTime, out destroyTime, out repeat);

                matchData.buildPhaseTime = buildTime;
                matchData.battlePhaseTime = destroyTime;
                matchData.repeat = repeat;
                matchData.useBuildGun = Convert.ToBoolean(useBuildGun);
                matchData.CacheMap(regMaps.Find(x => x.Value.Map == nWhere).Value, new UserMapInfo(0, 0));

                // Initialize BND-specific fields
                /*matchData.currentPhase = MatchData.BnDPhase.Build;
                matchData.currentRound = 1;
                matchData.remainTime = buildTime; // Start with Build phase time*/
            }

            if (debugHandle)
                Debug.Log("HandleRoomConfig from: " + msgRef.client.GetIdentifier());

            SendRoomConfig(msgRef.client);
            SendUpdateRoom(matchData);
        }

        private void HandleRoomRequest(MsgReference msgRef)
        {

            msgRef.msg._msg.Read(out int roomNumber);

            if (debugHandle)
                Debug.Log("HandleRoomRequest from: " + msgRef.client.GetIdentifier());

            MatchData matchData = msgRef.client.channel.GetMatchByRoomNumber(roomNumber);
            SendRoom(msgRef.client);
        }

        private void HandleRoomListRequest(MsgReference msgRef)
        {
            if (debugHandle)
                Debug.Log("HandleRoomListRequest from: " + msgRef.client.GetIdentifier());

            SendRoomList(msgRef.client);
        }

        private void HandleResumeRoomRequest(MsgReference msgRef)
        {
            MatchData matchData = msgRef.matchData;

            msgRef.msg._msg.Read(out int nextStatus);

            if (msgRef.client.seq == matchData.masterSeq)
            {
                matchData.room.Status = (Room.ROOM_STATUS)nextStatus;
            }

            if (debugHandle)
                Debug.Log("HandleResumeRoomRequest from: " + msgRef.client.GetIdentifier());

            SendUpdateRoom(matchData);
        }

        private void HandleTeamChangeRequest(MsgReference msgRef)
        {
            MatchData matchData = msgRef.matchData;

            msgRef.msg._msg.Read(out bool clickSlot);
            msgRef.msg._msg.Read(out int slotNum);

            if (debugHandle)
                Debug.Log("HandleTeamChangeRequest from: " + msgRef.client.GetIdentifier());

            if (slotNum < -1 && slotNum > 15)
                Debug.LogWarning("HandleTeamChangeRequest: Bad slot num " + slotNum + " from client: " + msgRef.client.GetIdentifier());

            else if (slotNum == -1)
            {
                msgRef.client.AssignSlot(matchData.GetNextFreeSlotOnOtherTeam(msgRef.client.slot));
            }

            else
                msgRef.client.AssignSlot(matchData.slots[slotNum]);

            SendTeamChange(msgRef.client);
        }

        private void HandleSlotLockRequest(MsgReference msgRef)
        {
            MatchData matchData = msgRef.matchData;

            msgRef.msg._msg.Read(out sbyte slotNum);
            msgRef.msg._msg.Read(out sbyte lck);

            if (debugHandle)
                Debug.Log("HandleSlotLockRequest from: " + msgRef.client.GetIdentifier());

            if (slotNum < 0 && slotNum > 15)
                Debug.LogWarning("HandleSlotLockRequest: Bad slot num " + slotNum + " from client: " + msgRef.client.GetIdentifier());

            else if (msgRef.client.seq == matchData.masterSeq)
            {
                matchData.slots[slotNum].ToggleLock(Convert.ToBoolean(lck));
                matchData.room.MaxPlayer = matchData.slots.FindAll(x => !x.isLocked).Count;

                SendSlotLock(msgRef.client, matchData, slotNum, SendType.BroadcastRoom);
            }

        }

        private void HandleSetStatusRequest(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out int status);

            msgRef.client.status = (BrickManDesc.STATUS)status;

            if (debugHandle)
                Debug.Log("HandleSetStatusRequest from: " + msgRef.client.GetIdentifier());

            SendSetStatus(msgRef.client);
        }

        private void HandleStartRequest(MsgReference msgRef)
        {
            MatchData matchData = msgRef.matchData;

            msgRef.msg._msg.Read(out int remainingCountdown);

            matchData.lobbyCountdownTime = 0;

            if (debugHandle)
                Debug.Log("HandleStartRequest from: " + msgRef.client.GetIdentifier());

            if (matchData.clientList.Find(x => x.status == BrickManDesc.STATUS.PLAYER_WAITING && x.seq != matchData.masterSeq) != null)
            {
                Debug.LogWarning("HandleStartRequest: Not All Ready");
                return;
            }

            matchData.room.Status = Room.ROOM_STATUS.PENDING;
            SendUpdateRoom(matchData);

            for (int i = 0; i < matchData.clientList.Count; i++)
            {
                matchData.clientList[i].status = BrickManDesc.STATUS.PLAYER_LOADING;
                matchData.clientList[i].clientStatus = ClientReference.ClientStatus.Match;
                SendSetStatus(matchData.clientList[i]);
                SendRespawnTicket(matchData.clientList[i]);
            }

            SendStart(matchData);
        }

        private void HandleWeaponHeldRatioRequest(MsgReference msgRef)
        {
            MatchData matchData = msgRef.matchData;

            msgRef.msg._msg.Read(out int count);
            for (int i = 0; i < count; i++)
            {
                msgRef.msg._msg.Read(out long key);
                msgRef.msg._msg.Read(out float value);
            }

            if (debugHandle)
                Debug.Log("HandleWeaponHeldRatioRequest from: " + msgRef.client.GetIdentifier());

            if (msgRef.client.status <= BrickManDesc.STATUS.PLAYER_LOADING)
            {
                msgRef.client.status = BrickManDesc.STATUS.PLAYER_PLAYING;
                SendSetStatus(msgRef.client);
                SendPostLoadInit(msgRef.client);
            }

            if (msgRef.client.isBreakingInto)
            {
                msgRef.client.isBreakingInto = false;

                for (int i = 0; i < matchData.destroyedBricks.Count; i++)
                    SendDestroyedBrick(msgRef.client, matchData.destroyedBricks[i], matchData);

                SendCannons(msgRef.client);
                SendTrains(msgRef.client);
            }
        }

        private void HandleP2PComplete(MsgReference msgRef)
        {
            if (debugHandle)
                Debug.Log("HandleP2PComplete from: " + msgRef.client.GetIdentifier());

            if (msgRef.client.status == BrickManDesc.STATUS.PLAYER_P2PING)
            {
                msgRef.client.status = BrickManDesc.STATUS.PLAYER_PLAYING;
                SendSetStatus(msgRef.client);
            }
        }

        private void HandleInitItemTermRequest(MsgReference msgRef)
        {

            MsgBody msgBody = msgRef.msg._msg;
            msgBody.Read(out long item);
            msgBody.Read(out int code);

            //TODO: activate Item

            MsgBody body = new MsgBody();
            body.Write(0); // 0 = sucess !0 == fail
            body.Write(item);

            Say(new MsgReference(308, body, msgRef.client, SendType.Unicast));
        }

        private void HandleBuyRequest(MsgReference msgRef)
        {
            //BuyHow See Good.BUY_HOW
            //Option = duration (days)
            //needEquip = Direct Equip
            //val = False afaik
            MsgBody msgBody = msgRef.msg._msg;
            msgBody.Read(out string code);
            msgBody.Read(out int buyHow);
            msgBody.Read(out int option);
            msgBody.Read(out byte val);
            //TODO needEquip
            msgBody.Read(out bool needEqup);
            //seq is error code or unique id
            //Read(out long val); seq
            //msgRef.Read(out string val2); code
            //msgRef.Read(out int val3); remain
            int remain = option * 86400;
            //negative = permanent
            if (option > 30) remain = -1;
            sbyte premium = 0; // isPremium 0 || 1
            int durability = 100; //Durability int.MaxValue = Permanent

            TItem template = TItemManager.Instance.dic.FirstOrDefault(x => x.Value.code == code).Value;
            if (template == null)
            {
                SendCustomMessage("Item doesn't exist.", msgRef.client, SendType.Unicast);
                return;
            }

            var item = msgRef.client.inventory.CreateItem(template);
            if (item != null)
            {
                bool canBuy = false;
                Good good = ShopManager.Instance.dic.FirstOrDefault(x => x.Value.code == code).Value;
                int price = good.GetPriceByOpt(option, (Good.BUY_HOW)buyHow);
                switch ((Good.BUY_HOW)buyHow)
                {
                    case Good.BUY_HOW.BRICK_POINT:
                        break;
                    case Good.BUY_HOW.CASH_POINT:
                        if (msgRef.client.data.tokens >= price)
                        {
                            int tokens = msgRef.client.data.tokens = msgRef.client.data.tokens - price;
                            MsgBody bodyUpdate = new MsgBody();
                            bodyUpdate.Write(msgRef.client.data.forcePoints);
                            bodyUpdate.Write(msgRef.client.data.brickPoints);
                            bodyUpdate.Write(tokens);
                            bodyUpdate.Write(msgRef.client.data.coins);
                            bodyUpdate.Write(msgRef.client.data.starDust);
                            Say(new MsgReference(102, bodyUpdate, msgRef.client, SendType.Unicast));
                            canBuy = true;
                        }
                        break;
                    case Good.BUY_HOW.GENERAL_POINT:
                        if (msgRef.client.data.forcePoints >= price)
                        {
                            int point = msgRef.client.data.forcePoints = msgRef.client.data.forcePoints - price;
                            MsgBody bodyUpdate = new MsgBody();
                            bodyUpdate.Write(point);
                            bodyUpdate.Write(msgRef.client.data.brickPoints);
                            bodyUpdate.Write(msgRef.client.data.tokens);
                            bodyUpdate.Write(msgRef.client.data.coins);
                            bodyUpdate.Write(msgRef.client.data.starDust);
                            Say(new MsgReference(102, bodyUpdate, msgRef.client, SendType.Unicast));
                            canBuy = true;
                        }
                        break;
                    default:
                        break;
                }

                MsgBody body = new MsgBody();
                body.Write(canBuy ? item.Seq : -3);
                body.Write(item.Code);
                body.Write(item.Remain);
                body.Write(Convert.ToSByte(item.IsPremium));
                body.Write(item.Durability);
                Say(new MsgReference(122, body, msgRef.client, SendType.Unicast));

                /*MsgBody body = new MsgBody();
                body.Write(canBuy ? item.Seq : -3);
                body.Write(code);
                body.Write(remain);
                body.Write(premium);
                body.Write(durability);
                Say(new MsgReference(122, body, msgRef.client, SendType.Unicast));*/

            }
            else
                SendCustomMessage("Couldn't create item " + template.name, msgRef.client, SendType.Unicast);
        }

        private void HandleKillLogRequest(MsgReference msgRef)
        {
            if (killLogTimer < 0.2f)
                return;

            MatchData matchData = msgRef.matchData;

            msgRef.msg._msg.Read(out int id);

            if (matchData.killLog.Find(x => x.id == id) != null)
                return;

            if (id != matchData.lastKillLogId)
                matchData.lastKillLogId = id;
            else
                return;

            killLogTimer = 0f;

            msgRef.msg._msg.Read(out sbyte killerType);
            msgRef.msg._msg.Read(out int killer);
            msgRef.msg._msg.Read(out sbyte victimType);
            msgRef.msg._msg.Read(out int victim);
            msgRef.msg._msg.Read(out int weaponBy);
            msgRef.msg._msg.Read(out int slot);
            msgRef.msg._msg.Read(out int category);
            msgRef.msg._msg.Read(out int hitpart);
            msgRef.msg._msg.Read(out int damageLogCount);

            Dictionary<int, int> damageLog = new Dictionary<int, int>();
            for (int i = 0; i < damageLogCount; i++)
            {
                msgRef.msg._msg.Read(out int key);
                msgRef.msg._msg.Read(out int value);

                if (key == victim)
                    continue;
                if (!damageLog.ContainsKey(key))
                    damageLog.Add(key, value);
                else
                    damageLog[key] += value;
            }

            //Debug.Log("VictimType: " + victimType + " weaponBy: " + weaponBy);
            if (victimType == 1 || weaponBy == -1)
            {
                // We do NOT process mob kills here.
                // No kill feed, no score change, nothing.
                //update deathcunt for victim on mob kill
                return;
            }

            if (debugHandle)
                Debug.Log("HandleKillLogRequest from: " + msgRef.client.GetIdentifier());

            ClientReference victimClient = matchData.clientList.Find(x => x.seq == victim);
            victimClient.deaths++;
            if (victimClient.slot.slotIndex < 8) // Blue team
            {
                if (!matchData.deadBluePlayers.Contains(victimClient.seq))
                    matchData.deadBluePlayers.Add(victimClient.seq);
            }
            else // Red team
            {
                if (!matchData.deadRedPlayers.Contains(victimClient.seq))
                    matchData.deadRedPlayers.Add(victimClient.seq);
            }
            SendDeathCount(victimClient);

            if (killer == victim)
                killer = damageLog.OrderByDescending(x => x.Value).FirstOrDefault().Key;

            ClientReference killerClient = matchData.clientList.Find(x => x.seq == killer);
            if (killer != victim)
            {
                killerClient.kills++;
                SendKillCount(killerClient);
            }

            foreach (KeyValuePair<int, int> entry in damageLog)
            {
                if (entry.Key != victim)
                {
                    if (entry.Key != killer)
                    {
                        ClientReference assistClient = matchData.clientList.Find(x => x.seq == entry.Key);
                        assistClient.assists++;
                        assistClient.score += entry.Value;
                        SendAssistCount(assistClient);
                    }

                    else
                    {
                        killerClient.score += entry.Value;
                        SendRoundScore(killerClient);
                    }

                }
            }
            //Fix for hosting killing fall damage?
            // does not work
            if (weaponBy == 0)
            {
                killerType = 0;
                killer = 0;
            }
            KillLogEntry killLogEntry = new KillLogEntry(id, killerType, killer, victimType, victim, (Weapon.BY)weaponBy, slot, category, hitpart, damageLog);
            matchData.killLog.Add(killLogEntry);
            SendKillLogEntry(killLogEntry, matchData);

            if (killer != victim)
            {
                switch (matchData.room.Type)
                {
                    case Room.ROOM_TYPE.TEAM_MATCH:
                        if (victimClient.slot.slotIndex > 7)
                            matchData.redScore++;
                        else
                            matchData.blueScore++;
                        SendTeamScore(matchData);
                        if (matchData.blueScore >= matchData.room.goal || matchData.redScore >= matchData.room.goal)
                        {
                            playTeamDeathMatch.HandleMatchEnd(matchData);
                        }
                        break;

                    case Room.ROOM_TYPE.INDIVIDUAL:
                        matchData.redScore++;
                        SendIndividualScore(matchData);

                        if (matchData.redScore >= matchData.room.goal)
                        {
                            playDeathMatch.HandleMatchEnd(matchData);
                        }
                        break;

                    case Room.ROOM_TYPE.BND:
                        Debug.LogWarning(matchData.isBuildPhase);
                        //the emulator match data is currently in the wrong phase?
                        if (!matchData.isBuildPhase)
                        {
                            // Score during the Destroy phase
                            if (victimClient.slot.slotIndex > 7)
                                matchData.redScore++;
                            else
                                matchData.blueScore++;

                            playBuildAndDestroy.SendBnDScore(matchData);

                            if (matchData.blueScore >= matchData.room.goal || matchData.redScore >= matchData.room.goal)
                            {
                                playBuildAndDestroy.HandleMatchEnd(matchData);
                            }
                        }
                        break;

                    case Room.ROOM_TYPE.CAPTURE_THE_FLAG:
                        if (victimClient.slot.slotIndex > 7)
                            matchData.redKillCount++;
                        else
                            matchData.blueKillCount++;
                        SendTeamScore(matchData);
                        if (matchData.blueScore >= matchData.room.goal || matchData.redScore >= matchData.room.goal)
                        {
                            playCaptureTheFlag.HandleMatchEnd(matchData);
                        }
                        break;

                    case Room.ROOM_TYPE.EXPLOSION:
                        //this needs fixing
                        //we need to check that score does not get added on explosion through bomb
                        //maybe if type == bomb?
                        if((Weapon.BY)weaponBy == Weapon.BY.CLOCKBOMB)
                            break;

                        int totalRed = matchData.redSlots.Count(x => x.isUsed);
                        int totalBlue = matchData.blueSlots.Count(x => x.isUsed);

                        int deadRed = matchData.deadRedPlayers.Count;
                        int deadBlue = matchData.deadBluePlayers.Count;

                        //SendScore!!

                        // DEBUG:
                        Debug.Log($"Explosion check wipe - Red: {deadRed}/{totalRed}, Blue: {deadBlue}/{totalBlue}");

                        if (deadBlue >= totalBlue)
                        {
                            matchData.blueScore++;
                            playExplosion.HandleRoundEnd(msgRef, 1);
                        }
                        else if (deadRed >= totalRed)
                        {
                            matchData.redScore++;
                            playExplosion.HandleRoundEnd(msgRef, 0); 
                        }
                        break;

                    case Room.ROOM_TYPE.ZOMBIE:
                        if ((HitPart.TYPE)hitpart == HitPart.TYPE.BRAIN && matchData.zombiePlayers.Contains(victim))
                        {
                            matchData.zombiePlayers.Remove(victim);
                            matchData.killedPlayers.Add(victim);
                            if (matchData.zombiePlayers.Count <= 0)
                            {
                                playZombie.SendZombieRoundEnd(msgRef, matchData);
                            }
                        }
                        break;

                    case Room.ROOM_TYPE.BUNGEE:
                        matchData.redScore++;
                        playFreefall.SendFreefallScore(matchData);

                        if (matchData.redScore >= matchData.room.goal)
                        {
                            playFreefall.HandleMatchEnd(matchData);
                        }
                        break;
                }
            }
        }

        private void HandleTeamScoreRequest(MsgReference msgRef)
        {
            if (debugHandle)
                Debug.Log("HandleTeamScoreRequest from: " + msgRef.client.GetIdentifier());

            SendTeamScore(msgRef.matchData);
        }

        private void HandleDestroyBrickRequest(MsgReference msgRef)
        {
            MatchData matchData = msgRef.matchData;

            msgRef.msg._msg.Read(out int brick);

            if (debugHandle)
                Debug.Log("HandleDestroyBrickRequest from: " + msgRef.client.GetIdentifier());

            if (!(matchData.destroyedBricks.Exists(x => x == brick)))
            {
                matchData.destroyedBricks.Add(brick);
                SendDestroyBrick(brick, matchData);
            }
        }

        private void HandleRegMapInfoRequest(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out int mapId);

            if (debugHandle)
                Debug.Log("HandleRegMapInfoRequest from: " + msgRef.client.GetIdentifier());
        }

        private void HandleInventoryData(MsgReference msgRef)
        {
            // List to hold new equipment
            List<Item> newEquipment = new List<Item>();

            // Read the total count of items
            msgRef.msg._msg.Read(out int itemCount);
            msgRef.client.inventory = new Inventory(msgRef.client.seq);
            msgRef.client.inventory.equipment.Clear();

            // Read each item's slot and code
            for (int i = 0; i < itemCount; i++)
            {
                msgRef.msg._msg.Read(out string code);
                msgRef.msg._msg.Read(out int usage);
                msgRef.msg._msg.Read(out sbyte toolSlot);

                // Fetch the item template
                TItem template = TItemManager.Instance.Get<TItem>(code);
                if (template != null)
                {
                    try
                    {
                        var item = msgRef.client.inventory.AddItem(template, false, -1, (Item.USAGE)usage);
                        //var item = msgRef.client.inventory.AddItem(template, false, -1, (Item.USAGE)Enum.Parse(typeof(Item.USAGE), usage, true));
                        item.toolSlot = toolSlot;
                    }

                    catch (Exception ex)
                    {
                        Debug.LogWarning("HandleInventoryData: Couldn't add item " + template.name + " (" + template.code + ") | " + ex.Message);
                    }
                }
                else
                {
                    Debug.LogWarning($"Template not found for code: {code}");
                }
            }

            if (debugHandle)
                Debug.Log($"HandleInventoryData from: {msgRef.client.GetIdentifier()}");

            // Notify the client about the updated inventory
            SendInventory(msgRef.client);
        }

        private void HandleEquipRequest(MsgReference msgRef)
        {
            MatchData matchData = msgRef.matchData;

            msgRef.msg._msg.Read(out long itemSeq);

            if (debugHandle)
                Debug.Log("HandleEquipRequest from: " + msgRef.client.GetIdentifier());

            Item item = msgRef.client.inventory.equipment.Find(x => x.Seq == itemSeq);
            if (item != null)
            {
                if (!item.IsEquipable)
                    return;

                int index = Inventory.SlotToIndex(item.Template.slot);
                if (index != -1 && index < msgRef.client.inventory.activeSlots.Length)
                {
                    Item oldItem = msgRef.client.inventory.activeSlots[index];
                    if (oldItem != null)
                    {
                        oldItem.Usage = Item.USAGE.UNEQUIP;
                        msgRef.client.inventory.activeSlots[index] = null;
                        SendUnequip(msgRef.client, oldItem.Seq, oldItem.Code);
                    }
                }

                if (item.Code == "s92" || item.Code == "s09" || item.Code == "s08" || item.Code == "s07")
                {
                    string[] targetCodes = { "s92", "s09", "s08", "s07" };

                    // Find and unequip any items with matching codes in equipment
                    foreach (string code in targetCodes)
                    {
                        Item equippedItem = msgRef.client.inventory.equipment.Find(x => x.Code == code);
                        if (equippedItem != null)
                        {
                            equippedItem.Usage = Item.USAGE.UNEQUIP;
                            SendUnequip(msgRef.client, equippedItem.Seq, equippedItem.Code);
                        }
                    }
                }

                item.Usage = Item.USAGE.EQUIP;
                msgRef.client.inventory.GenerateActiveSlots();

                SendEquip(msgRef.client, item.Seq, item.Code);
            }
        }

        private void HandleUnequipRequest(MsgReference msgRef)
        {
            MatchData matchData = msgRef.matchData;

            // Read the item sequence number from the message.
            msgRef.msg._msg.Read(out long itemSeq);

            if (debugHandle)
                Debug.Log("HandleUnequipRequest from: " + msgRef.client.GetIdentifier());

            // Find the item in the inventory using the sequence number.
            Item item = msgRef.client.inventory.equipment.Find(x => x.Seq == itemSeq);
            if (item != null)
            {
                // Check if the item is currently equipped.
                if (item.Usage != Item.USAGE.EQUIP)
                    return;

                // Find the index of the slot that the item is equipped to.
                int index = Inventory.SlotToIndex(item.Template.slot);
                if (index != -1 && index < msgRef.client.inventory.activeSlots.Length)
                {
                    // Ensure that the item is the one currently equipped in the slot.
                    Item currentItem = msgRef.client.inventory.activeSlots[index];
                    if (currentItem != null && currentItem.Seq == itemSeq)
                    {
                        // Set the item's usage to unequip and update the inventory slot.
                        currentItem.Usage = Item.USAGE.UNEQUIP;
                        msgRef.client.inventory.activeSlots[index] = null;

                        // Send a message to the client indicating the item has been unequipped.
                        SendUnequip(msgRef.client, currentItem.Seq, currentItem.Code);
                    }
                }

                // Regenerate the active slots to reflect the change in the inventory.
                msgRef.client.inventory.GenerateActiveSlots();
            }
        }


        private void HandleClearShooterTools(MsgReference msgRef)
        {
            if (debugHandle)
                Debug.Log("HandleClearShooterTools from: " + msgRef.client.GetIdentifier());

            for (int i = 0; i < msgRef.client.inventory.shooterTools.Length; i++)
            {
                if (msgRef.client.inventory.shooterTools[i] == null)
                    continue;

                msgRef.client.inventory.shooterTools[i].toolSlot = -1;
                msgRef.client.inventory.shooterTools[i] = null;
            }

            msgRef.client.inventory.GenerateActiveTools();
            SendShooterToolList(msgRef.client);
        }

        private void HandleClearWeaponSlots(MsgReference msgRef)
        {
            if (debugHandle)
                Debug.Log("HandleClearWeaponSlots from: " + msgRef.client.GetIdentifier());

            for (int i = 0; i < msgRef.client.inventory.weaponChg.Length; i++)
            {
                if (msgRef.client.inventory.weaponChg[i] == null)
                    continue;

                msgRef.client.inventory.weaponChg[i].toolSlot = -1;
                msgRef.client.inventory.weaponChg[i] = null;
            }

            msgRef.client.inventory.GenerateActiveChange();
            SendWeaponSlotList(msgRef.client);
        }

        private void HandleSetShooterToolRequest(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out sbyte slot);
            msgRef.msg._msg.Read(out long itemSeq);

            if (debugHandle)
                Debug.Log("HandleSetShooterToolRequest from: " + msgRef.client.GetIdentifier());


            if (itemSeq < 0)
            {
                msgRef.client.inventory.shooterTools[slot].toolSlot = -1;
                msgRef.client.inventory.shooterTools[slot] = null;
            }

            else
            {
                Item item = msgRef.client.inventory.equipment.Find(x => x.Seq == itemSeq);

                if (item != null)
                {
                    if (item.toolSlot >= 0)
                    {
                        Item dupeItem = msgRef.client.inventory.shooterTools[item.toolSlot];
                        if (dupeItem != null)
                            msgRef.client.inventory.shooterTools[dupeItem.toolSlot] = null;
                    }

                    Item oldItem = msgRef.client.inventory.shooterTools[slot];
                    if (oldItem != null)
                    {
                        oldItem.toolSlot = -1;
                        msgRef.client.inventory.shooterTools[slot] = null;
                    }

                    item.toolSlot = slot;
                    msgRef.client.inventory.shooterTools[slot] = item;
                }
            }

            msgRef.client.inventory.UpdateActiveEquipment();
            SendShooterToolList(msgRef.client);
            //SendSetShooterTool(msgRef.client, slot, item.Seq);
        }

        private void HandleSetWeaponSlotRequest(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out int slot);
            msgRef.msg._msg.Read(out long itemSeq);

            if (debugHandle)
                Debug.Log("HandleSetWeaponSlotRequest from: " + msgRef.client.GetIdentifier());

            if (itemSeq < 0)
            {
                msgRef.client.inventory.weaponChg[slot].toolSlot = -1;
                msgRef.client.inventory.weaponChg[slot] = null;
            }

            else
            {
                Item item = msgRef.client.inventory.equipment.Find(x => x.Seq == itemSeq);
                if (item != null)
                {
                    if (item.toolSlot >= 0)
                    {
                        Item dupeItem = msgRef.client.inventory.weaponChg[item.toolSlot];
                        if (dupeItem != null)
                            msgRef.client.inventory.weaponChg[dupeItem.toolSlot] = null;
                    }

                    Item oldItem = msgRef.client.inventory.weaponChg[slot];
                    if (oldItem != null)
                    {
                        oldItem.toolSlot = -1;
                        msgRef.client.inventory.shooterTools[slot] = null;
                    }

                    item.toolSlot = (sbyte)slot;
                    msgRef.client.inventory.shooterTools[slot] = item;
                }
            }

            msgRef.client.inventory.GenerateActiveChange();
            SendWeaponSlotList(msgRef.client);
            //SendSetWeaponSlot(msgRef.client, slot, item.Seq);
        }

        private void HandleRadioMsgRequest(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out int seq);
            msgRef.msg._msg.Read(out int category);
            msgRef.msg._msg.Read(out int message);

            if (debugHandle)
                Debug.Log("HandleRadioMsgRequest from: " + msgRef.client.GetIdentifier());

            SendRadioMsg(seq, category, message, msgRef.matchData);
        }

        private void HandleChargeForcePoint(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out long seq);
            msgRef.msg._msg.Read(out string code);

            Item item = MyInfoManager.Instance.inventory[seq];

            Debug.LogWarning("Found Item: " + item.Code + item.IsAmount + " amount: " + item.Amount);
            item.Amount = item.Amount - 1;
            Debug.LogWarning("New Amount " + item.Amount);
            Debug.LogWarning("EnopughToConsume " + item.EnoughToConsume);
            if (!item.EnoughToConsume)
            {
                Debug.LogWarning("Remove Item");
                MyInfoManager.Instance.inventory.Remove(item.Seq);
            }
            TSpecial special = (TSpecial)item.Template;
            int forcePoints = msgRef.client.data.forcePoints = msgRef.client.data.forcePoints + int.Parse(special.param);
            SendForcePointAssetUpdate(msgRef.client, forcePoints);
            SendChargeForcePoint(msgRef.client, seq, code, int.Parse(special.param));
        }

        public void SendForcePointAssetUpdate(ClientReference client, int forcePoints)
        {
            MsgBody bodyUpdate = new MsgBody();
            bodyUpdate.Write(forcePoints);
            bodyUpdate.Write(client.data.brickPoints);
            bodyUpdate.Write(client.data.tokens);
            bodyUpdate.Write(client.data.coins);
            bodyUpdate.Write(client.data.starDust);
            Say(new MsgReference(102, bodyUpdate, client, SendType.Unicast));
        }

        public void SendChargeForcePoint(ClientReference client, long seq, string code, int amount)
        {
            MsgBody body = new MsgBody();



            body.Write(1); //flag for success?
            body.Write(seq); //unused
            body.Write(code);

            body.Write(amount); //charge amount

            Say(new MsgReference(472, body, client, SendType.Unicast));
        }

        private void HandleChatRequest(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out string text);

            if (debugHandle)
                Debug.Log("HandleChatRequest from: " + msgRef.client.GetIdentifier());

            SendChat(msgRef.client, ChatText.CHAT_TYPE.NORMAL, text);
        }

        private void HandleTeamChatRequest(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out string text);

            if (debugHandle)
                Debug.Log("HandleTeamChatRequest from: " + msgRef.client.GetIdentifier());

            SendChat(msgRef.client, ChatText.CHAT_TYPE.TEAM, text);
        }

        private void HandleResultDoneRequest(MsgReference msgRef)
        {
            msgRef.client.status = BrickManDesc.STATUS.PLAYER_WAITING;
            msgRef.client.clientStatus = ClientReference.ClientStatus.Room;

            if (debugHandle)
                Debug.Log("HandleResultDoneRequest from: " + msgRef.client.GetIdentifier());

            SendSetStatus(msgRef.client);
        }

        public void HandleRespawnTicketRequest(MsgReference msgRef)
        {
            if (debugHandle)
                Debug.Log("HandleRespawnTicketRequest from: " + msgRef.client.GetIdentifier());

            SendRespawnTicket(msgRef.client);
        }

        private void HandleWeaponChangeRequest(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out int slot);
            msgRef.msg._msg.Read(out long seq);
            msgRef.msg._msg.Read(out string next);
            msgRef.msg._msg.Read(out string prev);

            if (debugHandle)
                Debug.Log("HandleWeaponChangeRequest from: " + msgRef.client.GetIdentifier());

            SendWeaponChange(msgRef.client, seq);
            SendPlayerWeaponChange(msgRef.client, prev, next);
        }

        private void HandleOpenDoorRequest(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out int seq);

            if (debugHandle)
                Debug.Log("HandleOpenDoorRequest from: " + msgRef.client.GetIdentifier());

            SendOpenDoor(seq, msgRef.matchData);
        }

        private void HandleCloseDoorRequest(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out int seq);

            if (debugHandle)
                Debug.Log("HandleCloseDoorRequest from: " + msgRef.client.GetIdentifier());
        }

        private void HandleDisconnect(MsgReference msgRef)
        {
            msgRef.client.Disconnect();
        }

        private void HandleDelegateMasterRequest(MsgReference msgRef)
        {
            MatchData matchData = msgRef.matchData;

            if (msgRef.client.seq == matchData.masterSeq)
            {
                msgRef.msg._msg.Read(out int newMaster);

                if (debugHandle)
                    Debug.Log("HandleDelegateMasterRequest from: " + msgRef.client.GetIdentifier());

                matchData.masterSeq = newMaster;
                SendMaster(null, matchData);
            }
        }

        private void HandleGetCannonRequest(MsgReference msgRef)
        {
            MatchData matchData = msgRef.matchData;

            msgRef.msg._msg.Read(out int brickSeq);

            if (debugHandle)
                Debug.Log("HandleGetCannonRequest from: " + msgRef.client.GetIdentifier());

            if (!matchData.usedCannons.ContainsKey(brickSeq))
            {
                matchData.usedCannons.Add(brickSeq, msgRef.client.seq);
                SendGetCannon(msgRef.client.seq, brickSeq, matchData);
                Debug.Log(matchData.usedCannons.Count);
            }
        }

        private void HandleEmptyCannonRequest(MsgReference msgRef)
        {
            MatchData matchData = msgRef.matchData;

            msgRef.msg._msg.Read(out int brickSeq);

            if (debugHandle)
                Debug.Log("HandleGetCannonRequest from: " + msgRef.client.GetIdentifier());

            if (matchData.usedCannons.ContainsKey(brickSeq))
            {
                matchData.usedCannons.Remove(brickSeq);
                SendEmptyCannon(brickSeq, matchData);
            }
        }

        private void HandleGetTrainRequest(MsgReference msgRef)
        {
            MatchData matchData = msgRef.matchData;

            msgRef.msg._msg.Read(out int brickSeq);
            msgRef.msg._msg.Read(out int trainId);

            if (debugHandle)
                Debug.Log("HandleGetTrainRequest from: " + msgRef.client.GetIdentifier());

            if (!matchData.usedTrains.ContainsKey(trainId))
            {
                matchData.usedTrains.Add(trainId, msgRef.client.seq);
                SendGetTrain(msgRef.client.seq, trainId, matchData);
            }
        }

        private void HandleEmptyTrainRequest(MsgReference msgRef)
        {
            MatchData matchData = msgRef.matchData;

            msgRef.msg._msg.Read(out int trainId);

            if (debugHandle)
                Debug.Log("HandleEmptyTrainRequest from: " + msgRef.client.GetIdentifier());

            if (matchData.usedTrains.ContainsKey(trainId))
            {
                matchData.usedTrains.Remove(trainId);
                SendEmptyTrain(trainId, matchData);
            }
        }

        private void HandleCacheBrickRequest(MsgReference msgRef)
        {
            if (debugHandle)
                Debug.Log("HandleCacheBrickRequest from: " + msgRef.client.GetIdentifier());

            if (msgRef.matchData.room.Type == ROOM_TYPE.MAP_EDITOR && !msgRef.matchData.cachedMap.isLoaded)
            {
                msgRef.client.buildModeRequestedMap = true;
                return;
            }

            SendCacheBrick(msgRef.client);
            SendCacheBrickDone(msgRef.client);
        }

        private void HandleCacheBrickAck(MsgReference msgRef)
        {
            UserMap userMap = msgRef.matchData.cachedMap;

            MsgBody msg = msgRef.msg._msg;
            msg.Read(out int val);
            for (int i = 0; i < val; i++)
            {
                msg.Read(out int seq);
                msg.Read(out byte template);
                msg.Read(out byte posX);
                msg.Read(out byte posY);
                msg.Read(out byte posZ);
                msg.Read(out ushort code);
                msg.Read(out byte rotation);
                msg.Read(out byte scriptCount);
                userMap.CacheBrick(seq, template, posX, posY, posZ, code, rotation);
                if (scriptCount > 0)
                {
                    msg.Read(out string alias);
                    msg.Read(out bool enableOnAwake);
                    msg.Read(out bool visibleOnAwake);
                    msg.Read(out string commandString);
                    userMap.UpdateScript(seq, alias, enableOnAwake, visibleOnAwake, commandString);
                }
            }
        }

        private void HandleCacheBrickDoneAck(MsgReference msgRef)
        {
            MsgBody msg = msgRef.msg._msg;
            msg.Read(out int mapIndex);
            msg.Read(out int skyboxIndex);
            MatchData match = msgRef.matchData;
            UserMap userMap = match.cachedMap;
            userMap.CacheDone(mapIndex, skyboxIndex);
            match.SetMapDone();
        }

        private void HandleAddBrickRequest(MsgReference msgRef)
        {
            MatchData matchData = msgRef.matchData;

            if (debugHandle)
                Debug.Log("HandleAddBrickRequest from: " + msgRef.client.GetIdentifier());

            msgRef.msg._msg.Read(out byte brickIndex);
            msgRef.msg._msg.Read(out byte x);
            msgRef.msg._msg.Read(out byte y);
            msgRef.msg._msg.Read(out byte z);
            msgRef.msg._msg.Read(out byte rot);

            Brick brick = BrickManager.Instance.GetBrick(brickIndex);
            if (brick == null || (brick.maxInstancePerMap > 0 && matchData.cachedMap.CountLimitedBrick(brickIndex) >= brick.maxInstancePerMap))
            {
                return;
            }

            int seq = matchData.GetNextBrickSeq();
            List<int> morphes = new List<int>();
            BrickInst brickInst = matchData.cachedMap.AddBrickInst(seq, brickIndex, x, y, z, 0, rot);
            if (brickInst != null)
            {
                SendAddBrick(msgRef.client, brickInst);
            }
        }

        private void HandleDelBrickRequest(MsgReference msgRef)
        {
            MatchData matchData = msgRef.matchData;

            if (debugHandle)
                Debug.Log("HandleDelBrickRequest from: " + msgRef.client.GetIdentifier());

            msgRef.msg._msg.Read(out int seq);

            List<int> morphes = new List<int>();
            if (matchData.cachedMap.DelBrickInst(seq, ref morphes))
                SendDelBrick(msgRef.client, seq);
        }

        private void HandleRegisterMapRequest(MsgReference msgRef)
        {
            MatchData matchData = msgRef.matchData;

            if (debugHandle)
                Debug.Log("HandleRegisterMap from: " + msgRef.client.GetIdentifier());

            msgRef.msg._msg.Read(out int slot);
            msgRef.msg._msg.Read(out ushort modeMask);
            msgRef.msg._msg.Read(out int regHow);
            msgRef.msg._msg.Read(out int point);
            msgRef.msg._msg.Read(out int downloadFee);
            msgRef.msg._msg.Read(out string msgEval);
            msgRef.msg._msg.Read(out byte[] textureBuffer);

            if (slot != matchData.cachedMap.map)
            {
                Debug.LogWarning($"HandleRegisterMapRequest: map mismatch. req={slot} cached={matchData.cachedMap.map}");
                return;
            }

            // Thumbnail for the registered map
            Texture2D thumbnail = new Texture2D(128, 128, TextureFormat.RGB24, mipmap: false);
            thumbnail.LoadImage(textureBuffer);
            thumbnail.Apply();

            DateTime time = DateTime.Now;
            int hashId = MapGenerator.instance.GetHashIdForTime(time);

            // Create & register RegMap ONLY here
            RegMap regMap = new RegMap(
                hashId,
                msgRef.client.name + "@Aurora",
                matchData.cachedUMI.Alias,
                time,
                modeMask,
                true, false,
                0, 0, 0, 0, 0, 0, 0,
                false
            );

            regMap.Thumbnail = thumbnail;

            RegMapManager.Instance.Add(regMap);
            RegMapManager.Instance.SetThumbnail(regMap.map, thumbnail);

            // Save registered files under the RegMap ID (separate from user slot file)
            regMap.Save();
            matchData.cachedMap.Save(hashId, matchData.cachedMap.skybox);

            // Keep the user slot as-is; optionally link regMap for current session info
            matchData.cachedUMI.regMap = regMap;   // ok to remember it's registered now
                                                   // DO NOT do: cachedUMI.slot = hashId
                                                   // DO NOT do: cachedMap.map = hashId

            // Pull current map list into the emulator (registered list changed)
            regMaps = RegMapManager.Instance.dicRegMap.ToList();

            MsgBody body = new MsgBody();

            body.Write(matchData.cachedUMI.slot);
            body.Write((int)regMap.ModeMask);

            Say(new MsgReference(52, body, msgRef.client, SendType.BroadcastRoom, matchData.channel, matchData));
        }

        public void SendDelBrick(ClientReference client, int brickSeq)
        {
            MatchData matchData = client.matchData;

            MsgBody body = new MsgBody();

            body.Write(client.seq);
            body.Write(brickSeq);

            Say(new MsgReference(16, body, client, SendType.BroadcastRoom, matchData.channel, matchData));

            if (debugSend)
                Debug.Log("SendDelBrick for room no " + matchData.room.No + " " + client.GetIdentifier());
        }

        public void SendAddBrick(ClientReference client, BrickInst brick)
        {
            MatchData matchData = client.matchData;

            MsgBody body = new MsgBody();

            body.Write(client.seq);
            body.Write(brick.Seq);
            body.Write(brick.Template);
            body.Write(brick.PosX);
            body.Write(brick.PosY);
            body.Write(brick.PosZ);
            body.Write(brick.Rot);

            Say(new MsgReference(14, body, client, SendType.BroadcastRoom, matchData.channel, matchData));

            if (debugSend)
                Debug.Log("SendAddBrick for room no " + matchData.room.No + " " + client.GetIdentifier());
        }

        public void SendCacheBrick(ClientReference client, MatchData matchData = null, SendType sendType = SendType.Unicast)
        {
            if (matchData == null)
            {
                matchData = client.matchData;
            }

            List<KeyValuePair<int, BrickInst>> brickList;
            UserMap userMap = matchData.cachedMap;
            brickList = userMap.dic.ToList();

            int chunkSize = 100;
            int chunkCount = Mathf.CeilToInt((float)brickList.Count / (float)chunkSize);
            int processedCount = 0;

            for (int chunk = 0; chunk < chunkCount; chunk++)
            {
                int remaining = brickList.Count - processedCount;
                if (remaining < chunkSize)
                    chunkSize = remaining;

                MsgBody body = new MsgBody();

                body.Write(chunkSize);

                for (int i = 0; i < chunkSize; i++, processedCount++)
                {
                    BrickInst brickInst = brickList[processedCount].Value;
                    body.Write(brickInst.Seq);
                    body.Write(brickInst.Template);
                    body.Write(brickInst.PosX);
                    body.Write(brickInst.PosY);
                    body.Write(brickInst.PosZ);
                    body.Write(brickInst.Code);
                    body.Write(brickInst.Rot);
                    if (brickInst.BrickForceScript != null)
                    {
                        body.Write((byte)brickInst.BrickForceScript.CmdList.Count);

                        if (brickInst.BrickForceScript.CmdList.Count > 0)
                        {
                            body.Write(brickInst.BrickForceScript.Alias);
                            body.Write(brickInst.BrickForceScript.EnableOnAwake);
                            body.Write(brickInst.BrickForceScript.VisibleOnAwake);
                            body.Write(brickInst.BrickForceScript.GetCommandString());
                        }
                    }
                    else
                    {
                        body.Write((byte)0);
                    }
                }
                Say(new MsgReference(21, body, client, sendType, matchData.channel, matchData, _doChunked: false));
            }
            if (debugSend)
                Debug.Log("SendCacheBrick with " + chunkCount + " chunks to: " + client.GetIdentifier());
        }

        public void SendCacheBrickDone(ClientReference client, MatchData matchData = null, SendType sendType = SendType.Unicast)
        {
            if (matchData == null)
            {
                matchData = client.matchData;
            }
            UserMap userMap = matchData.cachedMap;
            MsgBody body = new MsgBody();
            body.Write(0); // mapIndex
            body.Write(userMap.skybox);
            Say(new MsgReference(22, body, client, sendType, matchData.channel, matchData));
            if (debugSend)
                Debug.Log("SendCacheBrickDone for map " + userMap.map + " to: " + client.GetIdentifier());
        }

        public void SendCopyright(ClientReference client)
        {
            MsgBody body = new MsgBody();

            MatchData matchData = client.matchData;

            body.Write(matchData.masterSeq);
            body.Write(matchData.cachedUMI.Slot);

            Say(new MsgReference(53, body, client));

            if (debugSend)
                Debug.Log("SendCopyRight to: " + client.GetIdentifier());
        }

        public void SendPremiumItems(ClientReference client)
        {
            MsgBody body = new MsgBody();

            body.Write(2);
            body.Write("s20");
            body.Write("s21");

            Say(new MsgReference(492, body, client));
        }

        public void SendKick(ClientReference client, SendType sendType = SendType.Unicast)
        {
            MsgBody body = new MsgBody();

            body.Write(client.seq);

            Say(new MsgReference(89, body, client, sendType));
        }
        public void SendCannons(ClientReference client)
        {
            MatchData matchData = client.matchData;

            foreach (KeyValuePair<int, int> entry in matchData.usedCannons)
            {
                SendGetCannon(entry.Value, entry.Key, matchData, client, SendType.Unicast);
            }
        }

        public void SendTrains(ClientReference client)
        {
            MatchData matchData = client.matchData;

            foreach (KeyValuePair<int, int> entry in matchData.usedTrains)
            {
                SendGetTrain(entry.Value, entry.Key, matchData, client, SendType.Unicast);
            }
        }

        public void SendGetCannon(int seq, int brickSeq, MatchData matchData, ClientReference client = null, SendType sendType = SendType.BroadcastRoom)
        {
            MsgBody body = new MsgBody();

            body.Write(seq);
            body.Write(brickSeq);

            Say(new MsgReference(159, body, null, sendType, matchData.channel, matchData));

            if (debugSend)
            {
                if (sendType == SendType.BroadcastRoom)
                    Debug.Log("Broadcasted SendGetCannon for room no: " + matchData.room.No);

                else
                    Debug.Log("SendGetCannon to: " + client.GetIdentifier());
            }
        }

        public void SendEmptyCannon(int brickSeq, MatchData matchData)
        {
            MsgBody body = new MsgBody();

            body.Write(brickSeq);

            Say(new MsgReference(161, body, null, SendType.BroadcastRoom, matchData.channel, matchData));

            if (debugSend)
                Debug.Log("Broadcasted SendEmptyCannon for room no: " + matchData.room.No);
        }

        public void SendGetTrain(int seq, int trainId, MatchData matchData, ClientReference client = null, SendType sendType = SendType.BroadcastRoom)
        {
            MsgBody body = new MsgBody();

            body.Write(seq);
            body.Write(trainId);

            Say(new MsgReference(552, body, client, sendType, matchData.channel, matchData));

            if (debugSend)
            {
                if (sendType == SendType.BroadcastRoom)
                    Debug.Log("Broadcasted SendGetTrain for room no: " + matchData.room.No);

                else
                    Debug.Log("SendGetTrain to: " + client.GetIdentifier());
            }
        }

        public void SendEmptyTrain(int trainId, MatchData matchData)
        {
            MsgBody body = new MsgBody();

            body.Write(trainId);

            Say(new MsgReference(554, body, null, SendType.BroadcastRoom, matchData.channel, matchData));

            if (debugSend)
                Debug.Log("Broadcasted SendEmptyTrain for room no: " + matchData.room.No);
        }

        public void SendDisconnect(ClientReference client, SendType sendType = SendType.Unicast, string message = null)
        {
            MsgBody body = new MsgBody();
            if (message != null)
            {
                body.Write(message);
            }
            SayInstant(new MsgReference(ExtensionOpcodes.opDisconnectAck, body, client, sendType));
        }

        public void SendOpenDoor(int seq, MatchData matchData)
        {
            MsgBody body = new MsgBody();

            body.Write(seq);

            Say(new MsgReference(450, body, null, SendType.BroadcastRoom, matchData.channel, matchData));

            if (debugSend)
                Debug.Log("Broadcasted SendOpenDoor for room no: " + matchData.room.No);
        }

        public void SendWeaponChange(ClientReference client, long seq)
        {
            MsgBody body = new MsgBody();

            body.Write(0); //errorcode
            body.Write(0); //unused;
            body.Write(seq);

            Say(new MsgReference(415, body, client));

            if (debugSend)
                Debug.Log("SendWeaponChange to: " + client.GetIdentifier());
        }

        public void SendPlayerWeaponChange(ClientReference client, string prev, string next)
        {
            MsgBody body = new MsgBody();

            body.Write(client.seq);
            body.Write(prev);
            body.Write(next);

            Say(new MsgReference(416, body, client, SendType.BroadcastRoomExclusive, client.channel, client.matchData));

            if (debugSend)
                Debug.Log("Broadcasted SendPlayerWeaponChange for player: " + client.GetIdentifier());
        }
        public void SendInventory(ClientReference client)
        {
            client.inventory.UpdateActiveEquipment();
            SendItemList(client);
            SendShooterToolList(client);
            SendWeaponSlotList(client);
            //SendItemProperties(client);
            SendItemPimps(client);
            SendPremiumItems(client);
        }

        public void SendChat(ClientReference client, ChatText.CHAT_TYPE type, string text)
        {
            MsgBody body = new MsgBody();

            body.Write(client.seq);
            body.Write((byte)type);
            body.Write(client.name);
            body.Write(text);
            body.Write(Convert.ToBoolean(client.data.gm));

            Say(new MsgReference(25, body, null, SendType.BroadcastChannel, client.channel, client.matchData));

            if (debugSend)
                Debug.Log("Broadcasted SendChat");
        }

        public void SendRadioMsg(int seq, int category, int message, MatchData matchData)
        {
            MsgBody body = new MsgBody();
            body.Write(seq);
            body.Write(category);
            body.Write(message);

            Say(new MsgReference(96, body, null, SendType.BroadcastRoom, matchData.channel, matchData));

            if (debugSend)
                Debug.Log("Broadcasted SendRadioMsg for room no: " + matchData.room.No);
        }

        public void SendItemPimps(ClientReference client)
        {
            List<Item> weapons = client.inventory.equipment.FindAll(x => x.Template.type == TItem.TYPE.WEAPON);
            for (int i = 0; i < weapons.Count; i++)
            {
                SendItemPimp(client, weapons[i], PIMP.PROP_ATK_POW, 10);
                SendItemPimp(client, weapons[i], PIMP.PROP_ACCURACY, 10);
                SendItemPimp(client, weapons[i], PIMP.PROP_RECOIL, 10);
                SendItemPimp(client, weapons[i], PIMP.PROP_RPM, 10);
                SendItemPimp(client, weapons[i], PIMP.PROP_AMMO_MAX, 10);
                SendItemPimp(client, weapons[i], PIMP.PROP_ATTACK_SPEED, 10);
            }
        }

        public void SendItemPimp(ClientReference client, Item item, PIMP pimp, int grade)
        {
            try
            {
                if (!item.CanUpgradeAble())
                    return;
            }
            catch { return; }

            MsgBody body = new MsgBody();

            body.Write(item.Seq);
            body.Write((int)pimp);
            body.Write(grade);

            Say(new MsgReference(355, body, client));
        }

        public void SendItemProperties(ClientReference client)
        {
            MsgBody body = new MsgBody();

            List<Item> propertyItems = client.inventory.equipment.FindAll(x => x.Template.type == TItem.TYPE.ACCESSORY || x.Template.type == TItem.TYPE.CLOTH);
            body.Write(propertyItems.Count);
            for (int i = 0; i < propertyItems.Count; i++)
            {
                body.Write(propertyItems[i].Code);
                body.Write("ARMOR");
                body.Write(propertyItems[i].Template.type != TItem.TYPE.ACCESSORY || propertyItems[i].Template.slot == TItem.SLOT.HEAD ? 20 : 10);
                body.Write("");
                body.Write(0.2f);
            }

            Say(new MsgReference(491, body, client));

            if (debugSend)
                Debug.Log("SendItemProperties to: " + client.GetIdentifier());
        }

        public void SendSetShooterTool(ClientReference client, sbyte slot, long itemSeq)
        {
            MsgBody body = new MsgBody();

            body.Write(slot);
            body.Write(itemSeq);

            Say(new MsgReference(332, body, client));

            if (debugSend)
                Debug.Log("SendSetShooterTool to: " + client.GetIdentifier());
        }

        public void SendSetWeaponSlot(ClientReference client, int slot, long itemSeq)
        {
            MsgBody body = new MsgBody();

            body.Write(slot);
            body.Write(itemSeq);

            Say(new MsgReference(418, body, client));

            if (debugSend)
                Debug.Log("SendSetWeaponSlot to: " + client.GetIdentifier());
        }

        public void SendShooterToolList(ClientReference client)
        {
            MsgBody body = new MsgBody();

            body.Write(client.inventory.shooterTools.Length);
            for (int i = 0; i < client.inventory.shooterTools.Length; i++)
            {
                if (client.inventory.shooterTools[i] == null)
                {
                    body.Write((sbyte)i);
                    body.Write((long)-1);
                }

                else
                {
                    body.Write(client.inventory.shooterTools[i].toolSlot);
                    body.Write(client.inventory.shooterTools[i].Seq);
                }
            }

            Say(new MsgReference(462, body, client));

            if (debugSend)
                Debug.Log("SendShooterToolList to: " + client.GetIdentifier());
        }

        public void SendWeaponSlotList(ClientReference client)
        {
            MsgBody body = new MsgBody();

            body.Write(client.inventory.weaponChg.Length);
            for (int i = 0; i < client.inventory.weaponChg.Length; i++)
            {
                if (client.inventory.weaponChg[i] == null)
                {
                    body.Write(i);
                    body.Write((long)-1);
                }

                else
                {
                    body.Write((int)client.inventory.weaponChg[i].toolSlot);
                    body.Write(client.inventory.weaponChg[i].Seq);
                }
            }

            Say(new MsgReference(463, body, client));

            if (debugSend)
                Debug.Log("SendWeaponSlotList to: " + client.GetIdentifier());
        }

        public void SendEquip(ClientReference client, long itemSeq, string code)
        {
            MsgBody body = new MsgBody();

            body.Write(client.seq);
            body.Write(itemSeq);
            body.Write(code);

            Say(new MsgReference(36, body, client, SendType.Broadcast));

            if (debugSend)
                Debug.Log("Broadcasted SendEquip for client " + client.GetIdentifier() + " for room no: ");
        }

        public void SendUnequip(ClientReference client, long itemSeq, string code)
        {
            MsgBody body = new MsgBody();

            body.Write(client.seq);
            body.Write(itemSeq);
            body.Write(code);

            Say(new MsgReference(38, body, client, SendType.Broadcast));

            if (debugSend)
                Debug.Log("Broadcasted SendUnequip for client " + client.GetIdentifier() + " for room no: ");
        }

        public void SendInventoryRequest(ClientReference client)
        {
            MsgBody body = new MsgBody();

            body.Write(client.seq);

            Say(new MsgReference(ExtensionOpcodes.opInventoryReq, body, client));

            if (debugSend)
                Debug.Log("SendInventoryRequest to: " + client.GetIdentifier());
        }

        public void SendDestroyBrick(int brick, MatchData matchData)
        {
            MsgBody body = new MsgBody();

            body.Write(brick);

            Say(new MsgReference(77, body, null, SendType.BroadcastRoom, matchData.channel, matchData));

            if (debugSend)
                Debug.Log("Broadcasted SendDestroyBrick for brick " + brick + " for room no: " + matchData.room.No);
        }

        public void SendDestroyedBrick(ClientReference client, int brick, MatchData matchData, SendType sendType = SendType.Unicast)
        {
            MsgBody body = new MsgBody();

            body.Write(brick);

            Say(new MsgReference(78, body, client, sendType, matchData.channel, matchData));

            if (debugSend)
            {
                if (sendType == SendType.Unicast)
                    Debug.Log("SendDestroyedBrick to: " + client.GetIdentifier());
                else
                    Debug.Log("Broadcasted SendDestroyedBrick for brick for room no: " + matchData.room.No);
            }
        }

        public void SendKillCount(ClientReference client)
        {
            MatchData matchData = client.matchData;

            MsgBody body = new MsgBody();

            body.Write(client.seq);
            body.Write(client.kills);

            Say(new MsgReference(69, body, null, SendType.BroadcastRoom, client.channel, client.matchData));

            if (debugSend)
                Debug.Log("Broadcasted SendKillCount for client " + client.GetIdentifier() + " for room no: " + matchData.room.No);
        }

        public void SendDeathCount(ClientReference client)
        {
            MatchData matchData = client.matchData;

            MsgBody body = new MsgBody();

            body.Write(client.seq);
            body.Write(client.deaths);

            Say(new MsgReference(68, body, null, SendType.BroadcastRoom, client.channel, client.matchData));

            if (debugSend)
                Debug.Log("Broadcasted SendDeatchCount for client " + client.GetIdentifier() + " for room no: " + matchData.room.No);
        }

        public void SendAssistCount(ClientReference client)
        {
            MatchData matchData = client.matchData;

            MsgBody body = new MsgBody();

            body.Write(client.seq);
            body.Write(client.assists);
            body.Write(client.score);

            Say(new MsgReference(185, body, null, SendType.BroadcastRoom, client.channel, client.matchData));

            if (debugSend)
                Debug.Log("Broadcasted SendAssistCount for client " + client.GetIdentifier() + " for room no: " + matchData.room.No);
        }

        public void SendRoundScore(ClientReference client)
        {
            MatchData matchData = client.matchData;

            MsgBody body = new MsgBody();

            body.Write(client.seq);
            body.Write(client.score);

            Say(new MsgReference(300, body, null, SendType.BroadcastRoom, client.channel, client.matchData));

            if (debugSend)
                Debug.Log("Broadcasted SendRoundScore for client " + client.GetIdentifier() + " for room no: " + matchData.room.No);
        }

        public void SendKillLogEntry(KillLogEntry entry, MatchData matchData)
        {
            MsgBody body = new MsgBody();

            body.Write(entry.id);
            body.Write(entry.killerType);
            body.Write(entry.killer);
            body.Write(entry.victimType);
            body.Write(entry.victim);
            body.Write((int)entry.weaponBy);
            body.Write(entry.hitpart);

            Say(new MsgReference(45, body, null, SendType.BroadcastRoom, matchData.channel, matchData));

            if (debugSend)
                Debug.Log("Broadcasted SendKillLogEntry for room no: " + matchData.room.No);
        }

        public void SendIndividualScore(MatchData matchData)
        {
            MsgBody body = new MsgBody();

            body.Write(matchData.redScore);

            Say(new MsgReference(179, body, null, SendType.BroadcastRoom, matchData.channel, matchData));

            if (debugSend)
                Debug.Log("Broadcasted SendIndividualScore for room no: " + matchData.room.No);
        }

        public void SendTeamScore(MatchData matchData)
        {
            MsgBody body = new MsgBody();
            body.Write(matchData.redScore);
            body.Write(matchData.blueScore);

            Say(new MsgReference(67, body, null, SendType.BroadcastRoom, matchData.channel, matchData));

            if (debugSend)
                Debug.Log("Broadcasted SendTeamScore for room no: " + matchData.room.No);
        }

        public void SendMaster(ClientReference client, MatchData matchData)
        {
            MsgBody body = new MsgBody();

            body.Write(matchData.masterSeq);

            if (client == null)
            {
                Say(new MsgReference(31, body, null, SendType.BroadcastRoom, matchData.channel, matchData));

                if (debugSend)
                    Debug.Log("Broadcasted SendMaster for room no: " + matchData.room.No);
            }

            else
            {
                Say(new MsgReference(31, body, client));

                if (debugSend)
                    Debug.Log("SendMaster to: " + client.GetIdentifier());
            }
        }

        public void SendSlotLocks(ClientReference client)
        {
            MatchData matchData = client.matchData;
            for (sbyte i = 0; i < matchData.slots.Count; i++)
            {
                SendSlotLock(client, matchData, i);
            }

            if (debugSend)
                Debug.Log("SendSlots to: " + client.GetIdentifier());
        }

        public void SendSlotLock(ClientReference client, MatchData matchData, sbyte index, SendType sendType = SendType.Unicast)
        {
            MsgBody body = new MsgBody();

            body.Write(index);
            body.Write(Convert.ToSByte(matchData.slots[index].isLocked));
            Say(new MsgReference(86, body, client, sendType, matchData.channel, matchData));

            if (debugSend)
            {
                if (sendType == SendType.Unicast)
                    Debug.Log("SendSlotLock to: " + client.GetIdentifier());
                else
                    Debug.Log("Broadcasted SendSlotLock for room no " + matchData.room.No);
            }
        }

        public void SendRoomConfig(ClientReference client)
        {
            MsgBody body = new MsgBody();
            MatchData matchData = client.matchData;

            ROOM_TYPE roomType = matchData.room.type;

            body.Write(matchData.room.map);
            body.Write(matchData.room.CurMapAlias);

            if (roomType == ROOM_TYPE.MISSION)
            {
                body.Write(matchData.room.goal); // core HP
            } else
            {
                body.Write(matchData.room.weaponOption);
            }
            if (roomType == ROOM_TYPE.BND)
            {
                body.Write(PlayBuildAndDestroy.PackTimerOptions(matchData.buildPhaseTime, matchData.battlePhaseTime, matchData.repeat));
            } else
            {
                body.Write(matchData.room.timelimit);
            }
            body.Write(matchData.room.goal);
            body.Write(matchData.room.isBreakInto);
            body.Write(matchData.isBalance);
            body.Write(matchData.useBuildGun);
            body.Write("");         //password
            body.Write((byte)0);    //commented
            body.Write((int)matchData.room.Type);
            body.Write(matchData.room.isDropItem);
            body.Write(matchData.room.isWanted);

            Say(new MsgReference(92, body, client));
        }

        public void SendAddRoom(ClientReference client, MatchData matchData)
        {
            MsgBody body = new MsgBody();

            body.Write(matchData.room.No);
            body.Write((int)matchData.room.Type);
            body.Write(matchData.room.Title);
            body.Write(matchData.room.Locked);
            body.Write((int)matchData.room.Status);
            body.Write(matchData.room.CurPlayer);
            body.Write(matchData.room.MaxPlayer);
            body.Write(matchData.room.map);
            body.Write(matchData.room.CurMapAlias);
            body.Write(matchData.room.goal);
            body.Write(matchData.room.timelimit);
            body.Write(matchData.room.weaponOption);
            body.Write(matchData.room.ping);
            body.Write(matchData.room.score1);
            body.Write(matchData.room.score2);
            body.Write(matchData.room.CountryFilter);
            body.Write(matchData.room.isBreakInto);
            body.Write(matchData.room.isDropItem);
            body.Write(matchData.room.isWanted);
            body.Write(matchData.room.Squad);
            body.Write(matchData.room.SquadCounter);

            Say(new MsgReference(5, body, client, SendType.BroadcastChannel, matchData.channel, matchData));
            if (debugSend)
            {
                Debug.Log("SendAddRoom to channel: " + matchData.channel.channel.Name);
            }
        }

        public void SendUpdateRoom(MatchData matchData, ClientReference client = null)
        {
            MsgBody body = new MsgBody();

            body.Write(matchData.room.No);
            body.Write((int)matchData.room.Status);
            body.Write(matchData.room.CurPlayer);
            body.Write(matchData.room.MaxPlayer);
            body.Write(matchData.room.Locked);
            body.Write(matchData.room.map);
            body.Write(matchData.room.CurMapAlias);
            body.Write(matchData.room.goal);
            body.Write(matchData.room.timelimit);
            body.Write(matchData.room.weaponOption);
            body.Write(matchData.room.ping);
            body.Write(matchData.room.score1);
            body.Write(matchData.room.score2);
            body.Write(matchData.room.CountryFilter);
            body.Write(matchData.room.isBreakInto);
            body.Write((int)matchData.room.Type);
            body.Write(matchData.room.Title);
            body.Write(matchData.room.isDropItem);
            body.Write(matchData.room.isWanted);
            body.Write(matchData.room.Squad);
            body.Write(matchData.room.SquadCounter);

            if (client == null)
            {
                Say(new MsgReference(30, body, null, SendType.BroadcastChannel, matchData.channel, matchData));
                if (debugSend)
                {
                    Debug.Log("SendUpdateRoom to channel: " + matchData.channel.channel.Name);
                }
            } else
            {
                Say(new MsgReference(30, body, client, SendType.Unicast, matchData.channel, matchData));

                if (debugSend)
                {
                    Debug.Log("SendUpdateRoom to: " + client.GetIdentifier());
                }
            }

        }

        public void SendRoom(ClientReference client)
        {
            MsgBody body = new MsgBody();
            MatchData matchData = client.matchData;

            body.Write(matchData.room.No);
            body.Write((int)matchData.room.Type);
            body.Write(matchData.room.Title);
            body.Write(matchData.room.Locked);
            body.Write((int)matchData.room.Status);
            body.Write(matchData.room.CurPlayer);
            body.Write(matchData.room.MaxPlayer);
            if (matchData.room.type == ROOM_TYPE.BND)
            {
                if (matchData.room.Status == ROOM_STATUS.PLAYING)
                {
                    body.Write(matchData.room.map);
                }
                else
                {
                    body.Write(0);
                }
            }
            else
            {
                body.Write(matchData.room.map);
            }
            body.Write(matchData.room.CurMapAlias);
            body.Write(matchData.room.goal);
            body.Write(matchData.room.timelimit);
            body.Write(matchData.room.weaponOption);
            body.Write(matchData.room.ping);
            body.Write(matchData.room.score1);
            body.Write(matchData.room.score2);
            body.Write(matchData.room.CountryFilter);
            body.Write(matchData.room.isBreakInto);
            body.Write(matchData.room.isDropItem);
            body.Write(matchData.room.isWanted);
            body.Write(matchData.room.Squad);
            body.Write(matchData.room.SquadCounter);

            Say(new MsgReference(470, body, client, SendType.Unicast, matchData.channel, matchData));

            if (debugSend)
            {
                Debug.Log("SendRoom to: " + client.GetIdentifier());
            }

        }

        public void SendDeleteRoom(MatchData matchData, ChannelReference channel)
        {
            MsgBody body = new MsgBody();

            body.Write(matchData.room.No);

            Say(new MsgReference(6, body, null, SendType.BroadcastChannel, channel, matchData));

            if (debugSend)
                Debug.Log("Broadcasted SendDelRoom for room no: " + matchData.room.No);
        }

        public void SendRoomList(ClientReference client)
        {
            MsgBody body = new MsgBody();

            if (client.channel == null)
                body.Write(0); //count
            else
            {
                body.Write(client.channel.matches.Count); //count
                for (int i = 0; i < client.channel.matches.Count; i++)
                {
                    MatchData matchData = client.channel.matches[i];
                    body.Write(matchData.room.No);
                    body.Write((int)matchData.room.Type);
                    body.Write(matchData.room.Title);
                    body.Write(matchData.room.Locked);
                    body.Write((int)matchData.room.Status);
                    body.Write(matchData.room.CurPlayer);
                    body.Write(matchData.room.MaxPlayer);
                    body.Write(matchData.room.map);
                    body.Write(matchData.room.CurMapAlias);
                    body.Write(matchData.room.goal);
                    body.Write(matchData.room.timelimit);
                    body.Write(matchData.room.weaponOption);
                    body.Write(matchData.room.ping);
                    body.Write(matchData.room.score1);
                    body.Write(matchData.room.score2);
                    body.Write(matchData.room.CountryFilter);
                    body.Write(matchData.room.isBreakInto);
                    body.Write(matchData.room.isDropItem);
                    body.Write(matchData.room.isWanted);
                    body.Write(matchData.room.Squad);
                    body.Write(matchData.room.SquadCounter);
                }
            }

            Say(new MsgReference(468, body, client));

            if (debugSend)
                Debug.Log("SendRoomList to: " + client.GetIdentifier());
        }

        public void SendCreateRoom(ClientReference client, bool success = true)
        {
            MatchData matchData = client.matchData;

            MsgBody body = new MsgBody();

            body.Write((int)matchData.room.Type);
            body.Write(success ? matchData.room.No : -1);
            body.Write(matchData.room.Title);

            Say(new MsgReference(8, body, client));

            if (debugSend)
                Debug.Log("SendCreateRoom to: " + client.GetIdentifier());
        }

        public void SendJoin(ClientReference client)
        {
            MatchData matchData = client.matchData;
            MsgBody body = new MsgBody();

            body.Write(matchData.room.No);
            Say(new MsgReference(29, body, client));

            if (debugSend)
                Debug.Log("SendJoin to: " + client.GetIdentifier());
        }

        public void SendBreakInto(ClientReference client, int reply)
        {
            MsgBody body = new MsgBody();

            body.Write(reply);

            Say(new MsgReference(74, body, client));

            if (debugSend)
                Debug.Log("SendBreakInto to: " + client.GetIdentifier());
        }

        public void SendEnter(ClientReference client)
        {
            MatchData matchData = client.matchData;

            MsgBody body = new MsgBody();

            body.Write(client.slot.slotIndex);
            body.Write(client.seq);
            body.Write(client.name);
            body.Write(client.ip);
            body.Write(client.port); //port
            body.Write(client.ip);
            body.Write(client.port); //remotePort
            body.Write(client.inventory.equipmentString.Length);
            for (int i = 0; i < client.inventory.equipmentString.Length; i++)
            {
                body.Write(client.inventory.equipmentString[i]);
            }
            body.Write((int)client.status);
            body.Write(client.data.xp);
            body.Write(client.data.clanSeq);
            body.Write(client.data.clanName);
            body.Write(client.data.clanMark);
            body.Write(client.data.rank);
            body.Write((byte)1); //playerflag
            body.Write(client.inventory.weaponChgString.Length);
            for (int i = 0; i < client.inventory.weaponChgString.Length; i++)
            {
                body.Write(client.inventory.weaponChgString[i]);
            }
            body.Write(0); //drpItem count

            Say(new MsgReference(10, body, client, SendType.BroadcastRoomExclusive, matchData.channel, matchData));

            if (debugSend)
                Debug.Log("Broadcasted SendEnter for client " + client.GetIdentifier() + " for room no: " + matchData.room.No);
        }

        public void SendEnterSteam(ClientReference client)
        {
            MatchData matchData = client.matchData;

            MsgBody body = new MsgBody();

            body.Write(client.slot.slotIndex);
            body.Write(client.seq);
            body.Write(client.name);
            body.Write(client.steamID.m_SteamID);
            body.Write(client.inventory.equipmentString.Length);
            for (int i = 0; i < client.inventory.equipmentString.Length; i++)
            {
                body.Write(client.inventory.equipmentString[i]);
            }
            body.Write((int)client.status);
            body.Write(client.data.xp);
            body.Write(client.data.clanSeq);
            body.Write(client.data.clanName);
            body.Write(client.data.clanMark);
            body.Write(client.data.rank);
            body.Write((byte)1); //playerflag
            body.Write(client.inventory.weaponChgString.Length);
            for (int i = 0; i < client.inventory.weaponChgString.Length; i++)
            {
                body.Write(client.inventory.weaponChgString[i]);
            }
            body.Write(0); //drpItem count

            Say(new MsgReference(ExtensionOpcodes.opEnterSteamAck, body, client, SendType.BroadcastRoomExclusive, matchData.channel, matchData));

            if (debugSend)
                Debug.Log("Broadcasted SendEnterSteam for client " + client.GetIdentifier() + " for room no: " + matchData.room.No);
        }

        public void SendLeave(ClientReference client)
        {
            MatchData matchData = client.matchData;

            MsgBody body = new MsgBody();

            body.Write(client.seq);

            Say(new MsgReference(11, body, client, SendType.BroadcastRoom, matchData.channel, matchData));

            if (debugSend)
                Debug.Log("Broadcasted SendLeave for client " + client.GetIdentifier() + " for room no: " + matchData.room.No);
        }

        public void SendSlotData(MatchData matchData)
        {
            MsgBody body = new MsgBody();

            body.Write(matchData.clientList.Count);
            for (int i = 0; i < matchData.clientList.Count; i++)
            {
                ClientReference client = matchData.clientList[i];
                body.Write(client.slot.slotIndex);
                body.Write(client.seq);
                body.Write(client.name);
                body.Write(client.ip);
                body.Write(client.port); //port
                body.Write(client.ip);
                body.Write(client.port); //remotePort
                body.Write(client.inventory.equipmentString.Length);
                for (int j = 0; j < client.inventory.equipmentString.Length; j++)
                {
                    body.Write(client.inventory.equipmentString[j]);
                }
                body.Write((int)client.status);
                body.Write(client.data.xp);
                body.Write(client.data.clanSeq);
                body.Write(client.data.clanName);
                body.Write(client.data.clanMark);
                body.Write(client.data.rank);
                body.Write((byte)1); //playerflag
                body.Write(client.inventory.weaponChgString.Length);
                for (int j = 0; j < client.inventory.weaponChgString.Length; j++)
                {
                    body.Write(client.inventory.weaponChgString[j]);
                }
                body.Write(0); //drpItem count
            }

            Say(new MsgReference(ExtensionOpcodes.opSlotDataAck, body, null, SendType.BroadcastRoom, matchData.channel, matchData));
            Say(new MsgReference(ExtensionOpcodes.opSlotDataAck, body, null, SendType.BroadcastRoom, matchData.channel, matchData));

            if (debugSend)
                Debug.Log("Broadcasted SendSlotData for room no: " + matchData.room.No);
        }

        public void SendSlotDataSteam(MatchData matchData)
        {
            MsgBody body = new MsgBody();

            body.Write(matchData.clientList.Count);
            for (int i = 0; i < matchData.clientList.Count; i++)
            {
                ClientReference client = matchData.clientList[i];
                body.Write(client.slot.slotIndex);
                body.Write(client.seq);
                body.Write(client.name);
                body.Write(client.steamID.m_SteamID);
                body.Write(client.inventory.equipmentString.Length);
                for (int j = 0; j < client.inventory.equipmentString.Length; j++)
                {
                    body.Write(client.inventory.equipmentString[j]);
                }
                body.Write((int)client.status);
                body.Write(client.data.xp);
                body.Write(client.data.clanSeq);
                body.Write(client.data.clanName);
                body.Write(client.data.clanMark);
                body.Write(client.data.rank);
                body.Write((byte)1); //playerflag
                body.Write(client.inventory.weaponChgString.Length);
                for (int j = 0; j < client.inventory.weaponChgString.Length; j++)
                {
                    body.Write(client.inventory.weaponChgString[j]);
                }
                body.Write(0); //drpItem count
            }

            //Say(new MsgReference(ExtensionOpcodes.opSlotDataSteamAck, body, null, SendType.BroadcastRoom, matchData.channel, matchData));
            Say(new MsgReference(ExtensionOpcodes.opSlotDataSteamAck, body, null, SendType.BroadcastRoom, matchData.channel, matchData));

            if (debugSend)
                Debug.Log("Broadcasted SendSlotDataSteam for room no: " + matchData.room.No);
        }

        public void SendTeamChange(ClientReference client)
        {
            MatchData matchData = client.matchData;

            MsgBody body = new MsgBody();

            body.Write(client.seq);
            body.Write(0); //unused
            body.Write(client.slot.slotIndex);

            Say(new MsgReference(81, body, client, SendType.BroadcastRoom, matchData.channel, matchData));

            if (debugSend)
                Debug.Log("Broadcasted SendTeamChange for client " + client.GetIdentifier() + " for room no: " + matchData.room.No);
        }

        public void SendSetStatus(ClientReference client)
        {
            MatchData matchData = client.matchData;

            MsgBody body = new MsgBody();

            body.Write(client.seq);
            body.Write((int)client.status);

            Say(new MsgReference(48, body, null, SendType.BroadcastRoom, matchData.channel, matchData));

            if (debugSend)
                Debug.Log("Broadcasted SendSetStatus for client " + client.GetIdentifier() + " for room no: " + matchData.room.No);
        }

        public void SendStart(MatchData matchData)
        {
            MsgBody body = new MsgBody();

            body.Write(matchData.lobbyCountdownTime);

            Say(new MsgReference(50, body, null, SendType.BroadcastRoom, matchData.channel, matchData));

            if (debugSend)
                Debug.Log("Broadcasted SendStart for room no: " + matchData.room.No);
        }

        public void SendPostLoadInit(ClientReference client)
        {
            MsgBody body = new MsgBody();

            Say(new MsgReference(ExtensionOpcodes.opPostLoadInitAck, body, client));

            if (debugSend)
                Debug.Log("SendPostLoadInit to: " + client.GetIdentifier());
        }

        public void SendLoadComplete(ClientReference client)
        {
            MsgBody body = new MsgBody();

            body.Write(client.seq);
            Say(new MsgReference(43, body, null, SendType.BroadcastRoom, client.channel, client.matchData));

            if (debugSend)
                Debug.Log("Broadcasted SendLoadComplete for: " + client.GetIdentifier());
        }

        public void SendMatchCountdown(MatchData matchData)
        {
            MsgBody body = new MsgBody();

            body.Write(matchData.countdownTime);
            Say(new MsgReference(72, body, null, SendType.BroadcastRoom, matchData.channel, matchData));

            if (debugSend)
                Debug.Log("Broadcasted SendMatchCountdown for: " + matchData.countdownTime);
        }

        public void SendTimer(ClientReference client)
        {
            MatchData matchData = client.matchData;

            MsgBody body = new MsgBody();

            body.Write(matchData.remainTime);
            body.Write(matchData.playTime);
            Say(new MsgReference(66, body, client, SendType.BroadcastRoom, matchData.channel, matchData));

            if (debugSend)
                Debug.Log("SendTimer to: " + client.GetIdentifier());
        }

        public void SendRendezvousInfo(ClientReference client)
        {
            MsgBody body = new MsgBody();

            body.Write(0); //unused
            body.Write(client.ip);
            body.Write(client.port);

            Say(new MsgReference(320, body, client));

            if (debugSend)
                Debug.Log("SendRendezvousInfo to: " + client.GetIdentifier());
        }

        public void SendRendezvousInfoSteam(ClientReference client)
        {
            MsgBody body = new MsgBody();

            body.Write(client.steamID.m_SteamID);

            Say(new MsgReference(ExtensionOpcodes.opRendezvousInfoSteamAck, body, client));

            if (debugSend)
                Debug.Log("SendRendezvousInfoSteam to: " + client.GetIdentifier());
        }

        public void SendPlayerInitInfo(ClientReference client)
        {
            MsgBody body = new MsgBody();

            body.Write(client.data.xp);
            body.Write(client.data.tutorialed);
            body.Write(client.data.countryFilter);
            body.Write(client.data.tos);
            body.Write(client.data.extraSlots);
            body.Write(client.data.rank);
            body.Write(client.data.firstLoginFp);
            Say(new MsgReference(148, body, client));

            if (debugSend)
                Debug.Log("SendPlayerInitInfo to: " + client.GetIdentifier());

            body = new MsgBody();
            body.Write(client.data.qjModeMask);
            body.Write(client.data.qjOfficialMask);
            body.Write(client.data.qjCommonMask);

            Say(new MsgReference(417, body, client));

            if (debugSend)
                Debug.Log("SendPlayerOpt to: " + client.GetIdentifier());
        }

        public void SendChannels(ClientReference client)
        {
            MsgBody body = new MsgBody();

            body.Write(channelManager.channels.Count);
            foreach (ChannelReference channelRef in channelManager.channels)
            {
                body.Write(channelRef.channel.Id);
                body.Write(channelRef.channel.Mode);
                body.Write(channelRef.channel.Name);
                body.Write(channelRef.channel.Ip);
                body.Write(channelRef.channel.Port);
                body.Write(channelRef.channel.UserCount);
                body.Write(channelRef.channel.MaxUserCount);
                body.Write(channelRef.channel.Country);
                body.Write((byte)channelRef.channel.MinLvRank);
                body.Write((byte)channelRef.channel.MaxLvRank);
                body.Write((ushort)channelRef.channel.XpBonus);
                body.Write((ushort)channelRef.channel.FpBonus);
                body.Write(channelRef.channel.LimitStarRate);
            }

            /*body.Write(channels.Length);
			for (int i = 0; i < channels.Length; i++)
			{
				body.Write(channels[i].Id);
				body.Write(channels[i].Mode);
				body.Write(channels[i].Name);
				body.Write(channels[i].Ip);
				body.Write(channels[i].Port);
				body.Write(channels[i].UserCount);
				body.Write(channels[i].MaxUserCount);
				body.Write(channels[i].Country);
				body.Write((byte)channels[i].MinLvRank);
				body.Write((byte)channels[i].MaxLvRank);
				body.Write((ushort)channels[i].XpBonus);
				body.Write((ushort)channels[i].FpBonus);
				body.Write(channels[i].LimitStarRate);
			}*/
            Say(new MsgReference(141, body, client));

            if (debugSend)
                Debug.Log("SendChannels to: " + client.GetIdentifier());
        }

        public void SendCurChannel(ClientReference client, int curChannelId = 1)
        {
            MsgBody body = new MsgBody();

            body.Write(curChannelId);
            Say(new MsgReference(147, body, client));

            if (debugSend)
                Debug.Log("SendCurChannel to: " + client.GetIdentifier());
        }

        public void SendLogin(ClientReference client, int loginChannelId = 1)
        {
            MsgBody body = new MsgBody();

            body.Write(client.seq);
            body.Write(loginChannelId);
            Say(new MsgReference(2, body, client));

            if (debugSend)
                Debug.Log("SendLogin to: " + client.GetIdentifier());
        }

        public void SendPlayerInfo(ClientReference client)
        {
            MsgBody body = new MsgBody();

            body.Write(client.name);
            body.Write(client.data.xp);
            body.Write(client.data.forcePoints);
            body.Write(client.data.brickPoints);
            body.Write(client.data.tokens);
            body.Write(0);
            body.Write(client.data.coins);
            body.Write(client.data.starDust);
            body.Write(6);
            body.Write(5);
            body.Write(client.data.gm);
            body.Write(client.data.clanSeq);
            body.Write(client.data.clanName);
            body.Write(client.data.clanLv);
            body.Write(client.data.rank);
            body.Write(client.data.heavy);
            body.Write(client.data.assault);
            body.Write(client.data.sniper);
            body.Write(client.data.subMachine);
            body.Write(client.data.handGun);
            body.Write(client.data.melee);
            body.Write(client.data.special);
            Say(new MsgReference(27, body, client));

            if (debugSend)
                Debug.Log("SendPlayerInfo to: " + client.GetIdentifier());
        }

        public void SendItemList(ClientReference client)
        {
            MsgBody body = new MsgBody();

            body.Write(client.inventory.equipment.Count);
			for (int i = 0; i < client.inventory.equipment.Count; i++)
			{
				body.Write(client.inventory.equipment[i].Seq);
				body.Write(client.inventory.equipment[i].Code);
				body.Write((sbyte)client.inventory.equipment[i].Usage);
				body.Write(client.inventory.equipment[i].Amount);
				body.Write(client.inventory.equipment[i].IsPremium);
				body.Write(client.inventory.equipment[i].Durability);
			}

            Say(new MsgReference(464, body, client));

            if (debugSend)
                Debug.Log("SendItemList to: " + client.GetIdentifier());
        }

        public void SendUserList(ClientReference client, SendType sendType = SendType.Unicast)
        {
            MsgBody body = new MsgBody();

            body.Write(client.channel.clientList.Count);
            for (int i = 0; i < client.channel.clientList.Count; i++)
            {
                body.Write(client.channel.clientList[i].seq);
                body.Write(client.channel.clientList[i].name);
                body.Write(client.channel.clientList[i].data.xp);
                body.Write(client.channel.clientList[i].data.rank);

            }
            Say(new MsgReference(467, body, client, sendType));

            if (debugPing)
                Debug.Log("SendUserList to: " + client.GetIdentifier());
        }

        public void SendRoamout(ClientReference client, int src, SendType sendType = SendType.Unicast)
        {
            MsgBody body = new MsgBody();

            body.Write(src);
            Say(new MsgReference(144, body, client, sendType));

            if (debugSend)
                Debug.Log("SendRoamout to: " + client.GetIdentifier());
        }

        public void SendRoamin(ClientReference client, int dest, SendType sendType = SendType.Unicast)
        {
            MsgBody body = new MsgBody();

            body.Write(dest);
            Say(new MsgReference(146, body, client, sendType));

            if (debugSend)
                Debug.Log("SendRoamin to: " + client.GetIdentifier());
        }

        public void SendConnected(ClientReference client)
        {
            MsgBody body = new MsgBody();
            Say(new MsgReference(ExtensionOpcodes.opConnectedAck, body, client));

            if (debugSend)
                Debug.Log("SendConnected to: " + client.GetIdentifier());
        }

        public void SendAllDownloadedMaps(ClientReference client)
        {
            int chunkSize = 100;
            int chunkCount = Mathf.CeilToInt((float)regMaps.Count / (float)chunkSize);
            int processedCount = 0;

            for (int chunk = 0; chunk < chunkCount; chunk++)
            {
                int remaining = regMaps.Count - processedCount;
                if (remaining < chunkSize)
                    chunkSize = remaining;

                MsgBody body = new MsgBody();

                body.Write(-1); //page
                body.Write(chunkSize); //count
                for (int i = 0; i < chunkSize; i++, processedCount++)
                {
                    KeyValuePair<int, RegMap> entry = regMaps[processedCount];
                    body.Write(entry.Value.Map);
                    body.Write(entry.Value.Developer);
                    body.Write(entry.Value.Alias);
                    body.Write(entry.Value.ModeMask);
                    body.Write((byte)(Room.clanMatch | Room.official));
                    body.Write(entry.Value.tagMask);
                    body.Write(entry.Value.RegisteredDate.Year);
                    body.Write((sbyte)entry.Value.RegisteredDate.Month);
                    body.Write((sbyte)entry.Value.RegisteredDate.Day);
                    body.Write((sbyte)entry.Value.RegisteredDate.Hour);
                    body.Write((sbyte)entry.Value.RegisteredDate.Minute);
                    body.Write((sbyte)entry.Value.RegisteredDate.Second);
                    body.Write(entry.Value.DownloadFee);
                    body.Write(entry.Value.Release);
                    body.Write(entry.Value.LatestRelease);
                    body.Write(entry.Value.Likes);
                    body.Write(entry.Value.DisLikes);
                    body.Write(entry.Value.DownloadCount);
                }

                Say(new MsgReference(426, body, client));
            }

            if (debugSend)
                Debug.Log("SendAllDownloadedMaps to: " + client.GetIdentifier());
        }

        public void SendAllUserMaps(ClientReference client)
        {
            const int chunkSize = 200;

            int total = regMaps.Count;
            int chunkCount = Mathf.CeilToInt((float)total / chunkSize);

            Debug.LogWarning("RegMapCount: " + total);

            int processed = 0;

            for (int page = 0; page < chunkCount; page++)
            {
                int remaining = total - processed;
                int currentChunkSize = (remaining < chunkSize) ? remaining : chunkSize;

                MsgBody body = new MsgBody();

                body.Write(page);  
                body.Write(currentChunkSize);  

                for (int i = 0; i < currentChunkSize; i++, processed++)
                {
                    KeyValuePair<int, RegMap> entry = regMaps[processed];

                    body.Write(entry.Key); // slot
                    body.Write(entry.Value.Alias);
                    body.Write(-1); // brick count
                    body.Write(entry.Value.RegisteredDate.Year);
                    body.Write((sbyte)entry.Value.RegisteredDate.Month);
                    body.Write((sbyte)entry.Value.RegisteredDate.Day);
                    body.Write((sbyte)entry.Value.RegisteredDate.Hour);
                    body.Write((sbyte)entry.Value.RegisteredDate.Minute);
                    body.Write((sbyte)entry.Value.RegisteredDate.Second);

                    body.Write((sbyte)0); // premium
                }

                Say(new MsgReference(430, body, client));
            }

            if (debugSend)
                Debug.Log("SendAllUserMaps to: " + client.GetIdentifier());
        }


        public void SendUserMapSlots(ClientReference client)
        {
            const int firstId = 33;
            const int slotCount = 12;

            MsgBody body = new MsgBody();
            body.Write(1);            // page
            body.Write(slotCount);    // count

            for (int id = firstId; id < firstId + slotCount; id++)
            {
                // Default = empty slot
                string alias = "";
                int brickCount = -1;
                DateTime lastModified = DateTime.MinValue;
                sbyte premium = 0;

                // Try load from cache before sending
                var umi = new UserMapInfo(id, premium);
                if (umi.LoadCache())
                {
                    umi.VerifySavedData();

                    alias = umi.Alias;
                    brickCount = umi.BrickCount;
                    lastModified = umi.LastModified;
                    premium = umi.Premium;
                }

                body.Write(id);        // slot/id the client uses
                body.Write(alias);
                body.Write(brickCount);

                if (!string.IsNullOrEmpty(alias) && lastModified.Year > 1971)
                {
                    body.Write(lastModified.Year);
                    body.Write((sbyte)lastModified.Month);
                    body.Write((sbyte)lastModified.Day);
                    body.Write((sbyte)lastModified.Hour);
                    body.Write((sbyte)lastModified.Minute);
                    body.Write((sbyte)lastModified.Second);
                }
                else
                {
                    body.Write(0);
                    body.Write((sbyte)0);
                    body.Write((sbyte)0);
                    body.Write((sbyte)0);
                    body.Write((sbyte)0);
                    body.Write((sbyte)0);
                }

                body.Write(premium);
            }

            Say(new MsgReference(430, body, client));
        }

        private void HandleResetUserMapSlot(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out int slot);
            msgRef.msg._msg.Read(out long item);
            msgRef.msg._msg.Read(out string itemCode);

            if (slot < 33 || slot > 44)
            {
                SendResetAck(msgRef.client, result: 1, slot: slot);
                return;
            }

            bool ok = true;
            try
            {
                string cacheDir = Path.Combine(Application.dataPath, "Resources/Cache");

                string geom = Path.Combine(cacheDir, "downloaded" + slot + ".geometry");
                string umi = Path.Combine(cacheDir, "downloaded" + slot + ".umi.cache");

                if (File.Exists(geom)) File.Delete(geom);
                if (File.Exists(umi)) File.Delete(umi);
            }
            catch (Exception ex)
            {
                ok = false;
                Debug.LogError("HandleResetUserMapSlot: " + ex);
            }

            SendResetAck(msgRef.client, result: ok ? 0 : 1, slot: slot);
        }

        private void SendResetAck(ClientReference client, int result, int slot)
        {
            MsgBody body = new MsgBody();
            body.Write(result); // val
            body.Write(slot);   // val2
            Say(new MsgReference((int)MessageId.CS_RESET_USER_MAP_SLOTS_ACK, body, client));
        }


        public void SendDownloadedMaps(ClientReference client, int page)
        {
            MsgBody body = new MsgBody();

            const int mapsPerPage = 12;
            int offset = page * mapsPerPage;
            int remaining = regMaps.Count - offset;
            int count = remaining < mapsPerPage ? remaining : mapsPerPage;

            body.Write(page); //page
            body.Write(count); //count
            for (int i = offset; i < offset + count; i++)
            {
                KeyValuePair<int, RegMap> entry = regMaps[i];
                body.Write(entry.Value.Map);
                body.Write(entry.Value.Developer);
                body.Write(entry.Value.Alias);
                body.Write(entry.Value.ModeMask);
                body.Write((byte)(Room.clanMatch | Room.official));
                body.Write(entry.Value.tagMask);
                body.Write(entry.Value.RegisteredDate.Year);
                body.Write((sbyte)entry.Value.RegisteredDate.Month);
                body.Write((sbyte)entry.Value.RegisteredDate.Day);
                body.Write((sbyte)entry.Value.RegisteredDate.Hour);
                body.Write((sbyte)entry.Value.RegisteredDate.Minute);
                body.Write((sbyte)entry.Value.RegisteredDate.Second);
                body.Write(entry.Value.DownloadFee);
                body.Write(entry.Value.Release);
                body.Write(entry.Value.LatestRelease);
                body.Write(entry.Value.Likes);
                body.Write(entry.Value.DisLikes);
                body.Write(entry.Value.DownloadCount);
            }
            Say(new MsgReference(426, body, client));

            if (debugSend)
                Debug.Log("SendDownloadedMaps to: " + client.GetIdentifier());
        }

        public void SendRegisteredMaps(ClientReference client, int page)
        {
            MsgBody body = new MsgBody();

            const int mapsPerPage = 12;
            int offset = page * mapsPerPage;
            int remaining = regMaps.Count - offset;
            int count = remaining < mapsPerPage ? remaining : mapsPerPage;

            body.Write(page); //page
            body.Write(count); //count
            for (int i = offset; i < offset + count; i++)
            {
                KeyValuePair<int, RegMap> entry = regMaps[i];
                body.Write(entry.Value.Map);
                body.Write(entry.Value.Developer);
                body.Write(entry.Value.Alias);
                body.Write(entry.Value.ModeMask);
                body.Write((byte)(Room.clanMatch | Room.official));
                body.Write(entry.Value.tagMask);
                body.Write(entry.Value.RegisteredDate.Year);
                body.Write((sbyte)entry.Value.RegisteredDate.Month);
                body.Write((sbyte)entry.Value.RegisteredDate.Day);
                body.Write((sbyte)entry.Value.RegisteredDate.Hour);
                body.Write((sbyte)entry.Value.RegisteredDate.Minute);
                body.Write((sbyte)entry.Value.RegisteredDate.Second);
                body.Write(entry.Value.DownloadFee);
                body.Write(entry.Value.Release);
                body.Write(entry.Value.LatestRelease);
                body.Write(entry.Value.Likes);
                body.Write(entry.Value.DisLikes);
                body.Write(entry.Value.DownloadCount);
            }
            Say(new MsgReference(428, body, client));

            if (debugSend)
                Debug.Log("SendRegisteredMaps to: " + client.GetIdentifier());
        }

        public void SendUserMaps(ClientReference client, int page)
        {
            MsgBody body = new MsgBody();

            const int mapsPerPage = 12;
            int offset = page * mapsPerPage;
            int remaining = regMaps.Count - offset;
            int count = remaining < mapsPerPage ? remaining : mapsPerPage;

            body.Write(page); //page
            body.Write(count); //count
            for (int i = offset; i < offset + count; i++)
            {
                KeyValuePair<int, RegMap> entry = regMaps[i];
                body.Write(entry.Key); //slot
                body.Write(entry.Value.Alias);
                body.Write(10000); //brick count
                body.Write(entry.Value.RegisteredDate.Year);
                body.Write((sbyte)entry.Value.RegisteredDate.Month);
                body.Write((sbyte)entry.Value.RegisteredDate.Day);
                body.Write((sbyte)entry.Value.RegisteredDate.Hour);
                body.Write((sbyte)entry.Value.RegisteredDate.Minute);
                body.Write((sbyte)entry.Value.RegisteredDate.Second);
                body.Write((sbyte)0);
            }
            Say(new MsgReference(430, body, client));

            if (debugSend)
                Debug.Log("SendUserMaps to: " + client.GetIdentifier());
        }

        public void SendCustomMessage(string message, ClientReference client = null, SendType sendType = SendType.Broadcast)
        {
            MsgBody body = new MsgBody();

            body.Write(message);

            Say(new MsgReference(ExtensionOpcodes.opCustomMessageAck, body, client, sendType));
        }

        public void SendRespawnTicket(ClientReference client)
        {
            MsgBody body = new MsgBody();

            body.Write(UnityEngine.Random.Range(1, 64));

            Say(new MsgReference(64, body, client));

            if (debugSend)
                Debug.Log("SendRespawnTicket to: " + client.GetIdentifier());
        }

        private void HandleBeginChunkedBufferReceive(MsgReference msgRef)
        {
            if (!msgRef.client.chunkedBufferReceiver.Begin(msgRef.msg._msg))
            {
                SendDisconnect(msgRef.client, message: "Received invalid chunked buffer");
                msgRef.client.Disconnect(true);
            }
        }

        private void HandleChunkedBufferReceive(MsgReference msgRef)
        {
            if (!msgRef.client.chunkedBufferReceiver.ReceiveChunk(msgRef.msg._msg))
            {
                SendDisconnect(msgRef.client, message: "Received invalid chunked buffer");
                msgRef.client.Disconnect(true);
            }
        }

        private void HandleEndChunkedBufferReceive(MsgReference msgRef)
        {
            ushort packedOpcode;
            MsgBody packedBody;
            if (!msgRef.client.chunkedBufferReceiver.End(msgRef.msg._msg, out packedOpcode, out packedBody))
            {
                SendDisconnect(msgRef.client, message: "Received invalid chunked buffer");
                msgRef.client.Disconnect(true);
                return;
            }
            readQueue.Enqueue(new MsgReference(packedOpcode, packedBody, msgRef.client, _channelRef: msgRef.channelRef, _matchData: msgRef.matchData));
        }

        private void HandleGetBack2SpawnerRequest(MsgReference msgRef)
        {
            MatchData data = msgRef.matchData;
            MsgBody msg = new MsgBody();
            Say(new MsgReference(263, msg, msgRef.client, SendType.BroadcastRoom, data.channel, data));
        }

        private void HandleMatchRestartCountRequest(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out int count);
            MatchData data = msgRef.matchData;
            MsgBody msg = new MsgBody();
            msg.Write(count);
            Say(new MsgReference(265, msg, msgRef.client, SendType.BroadcastRoom, data.channel, data));
        }

        private void HandleMatchRestartRequest(MsgReference msgRef)
        {
            MatchData data = msgRef.matchData;
            MsgBody msg = new MsgBody();
            Say(new MsgReference(267, msg, msgRef.client, SendType.BroadcastRoom, data.channel, data));
        }

        private void HandleLineBrickRequest(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out long item);
            msgRef.msg._msg.Read(out string code);
            msgRef.msg._msg.Read(out byte brickIndex);
            msgRef.msg._msg.Read(out byte x);
            msgRef.msg._msg.Read(out byte y);
            msgRef.msg._msg.Read(out byte z);
            msgRef.msg._msg.Read(out byte rot);

            //Debug.LogWarning($"LINE_REQ item:{item} code:{code} brickIndex:{brickIndex} at({x},{y},{z}) rot:{rot}");

            if (msgRef?.matchData?.cachedMap == null)
            {
                Debug.LogError("LINE_REQ: matchData/cachedMap null");
                SendLineFail(msgRef, -6);
                return;
            }

            int playerSeq = msgRef.client.seq; 

            lock (dataLock)
            {
                var map = msgRef.matchData.cachedMap;

                if (map.GetByCoord(x, y, z) != null)
                {
                    // Success ACK but do not add (prevents the tool from stalling on overlaps)
                    Debug.LogWarning("Trying to replace Brick with line tool.");
                    SendLineAck(msgRef, playerSeq, /*newSeq*/ 0, /*template*/ 0, x, y, z, rot);
                    return;
                }

                Brick b = BrickManager.Instance.GetBrick(brickIndex); 
                if (b == null)
                {
                    Debug.LogWarning($"LINE_REQ: unknown brick index={brickIndex}");
                    SendLineFail(msgRef, -6);
                    return;
                }

                int newSeq = msgRef.matchData.GetNextBrickSeq();

                List<int> morphes = new List<int>(32);
                morphes.Clear();
                bool ok = map.AddBrickInst(newSeq, b.index, x, y, z, rot, ref morphes);

                if (!ok)
                {
                    Debug.LogError($"LINE_REQ: AddBrickInst failed newSeq={newSeq} template={b.index} at({x},{y},{z})");
                    SendLineFail(msgRef, -6);
                    return;
                }

                SendLineAck(msgRef, playerSeq, newSeq, b.index, x, y, z, rot);
            }
        }

        private void SendLineAck(MsgReference req, int playerSeq, int newSeq, byte template, byte x, byte y, byte z, byte rot)
        {
            MsgBody mb = new MsgBody();
            mb.Write(playerSeq);
            mb.Write(newSeq);
            mb.Write(template);
            mb.Write(x);
            mb.Write(y);
            mb.Write(z);
            mb.Write(rot);

            Say(new MsgReference((int)MessageId.CS_LINE_BRICK_ACK, mb, req.client, SendType.BroadcastRoom, req.matchData.channel, req.matchData));
        }

        private void SendLineFail(MsgReference req, int resultCode)
        {
            MsgBody mb = new MsgBody();
            mb.Write(resultCode);
            Say(new MsgReference((int)MessageId.CS_LINE_BRICK_FAIL_ACK, mb, req.client));
        }

        private void HandleBulkBrickRequest(MsgReference msgRef)
        {

            msgRef.msg._msg.Read(out ushort flag);
            msgRef.msg._msg.Read(out byte sourceIndex);
            msgRef.msg._msg.Read(out byte sourceRotation);
            msgRef.msg._msg.Read(out byte targetIndex);
            msgRef.msg._msg.Read(out byte targetRotation);
            msgRef.msg._msg.Read(out uint count);

            if (msgRef?.matchData?.cachedMap == null)
            {
                Debug.LogError("BULK_REQ: matchData/cachedMap null");
                SendBulkFail(msgRef, -1);
                return;
            }

            if (!flag.IsSet(OperationFlag.Delete))
            {
                Brick targetBrick = BrickManager.Instance.GetBrick(targetIndex);
                if (targetBrick == null)
                {
                    Debug.LogWarning($"BULK_REQ: unknown target brick index={targetIndex}");
                    SendBulkFail(msgRef, 0x0);
                    return;
                }
                if (targetBrick.maxInstancePerMap > 0)
                {
                    SendBulkFail(msgRef, 0x20);
                    return;
                }
            }
            if ((flag.IsSet(OperationFlag.OnlySource) && !flag.IsSet(OperationFlag.ExcludeSourceType)) || (!flag.IsSet(OperationFlag.OnlySource) && flag.IsSet(OperationFlag.ExcludeSourceType)))
            {
                Brick sourceBrick = BrickManager.Instance.GetBrick(sourceIndex);
                if (sourceBrick == null)
                {
                    Debug.LogWarning($"BULK_REQ: unknown source brick index={sourceIndex}");
                    SendBulkFail(msgRef, 0x1);
                    return;
                }
            }

            Debug.Log($"BULK_REQ: Trying to change {count} brick(s)");

            int playerSeq = msgRef.client.seq;

            // Pre-read coordinates (so if msg is malformed we fail before touching map)
            var xs = new byte[count];
            var ys = new byte[count];
            var zs = new byte[count];

            for (int i = 0; i < count; i++)
            {
                msgRef.msg._msg.Read(out xs[i]);
                msgRef.msg._msg.Read(out ys[i]);
                msgRef.msg._msg.Read(out zs[i]);
            }

            // Apply in one lock
            List<int> newSeqs = new List<int>((int) count);
            sbyte[] results = new sbyte[count];
            // -3 | Skipped                 | Uses no sequence
            // -2 | Failed                  | Uses no sequence
            // -1 | Replace partial success | Uses 1 sequence
            // 0  | Success                 | Uses 1 sequence
            // 1  | Replace success         | Uses 2 sequences
            // 2  | Success but unchanged   | Uses no sequence
            Array.Clear(results, 0, results.Length);

            sbyte result;
            lock (dataLock)
            {
                MyInfoManager.Instance.AuroraTemporarilyDisableBrickNetworkUpdates = true;
                try
                {
                    List<int> morphes = new List<int>(32);
                    var map = msgRef.matchData.cachedMap;
                    int brickSeq;
                    byte x, y, z;

                    for (int i = 0; i < count; i++)
                    {
                        x = xs[i];
                        y = ys[i]; 
                        z = zs[i];

                        BrickInst brickInst = map.GetByCoord(x, y, z);
                        if (brickInst != null)
                        {
                            if (flag.IsSet(OperationFlag.OnlySource))
                            {
                                if (flag.IsSet(OperationFlag.ExcludeSourceType) || brickInst.Template != sourceIndex || (flag.IsSet(OperationFlag.SourceWithRotation) && brickInst.Rot != sourceRotation))
                                {
                                    results[i] = -3; // Skipped cause it doesn't match
                                    continue;
                                }
                            } else if (flag.IsSet(OperationFlag.ExcludeSourceType))
                            {
                                if (brickInst.Template == sourceIndex && (!flag.IsSet(OperationFlag.SourceWithRotation) || brickInst.Rot == sourceRotation))
                                {
                                    results[i] = -3; // Skipped cause it matches
                                    continue;
                                }
                            }
                            if (!flag.IsSet(OperationFlag.Delete) && brickInst.Template == targetIndex && brickInst.Rot == targetRotation)
                            {
                                results[i] = 2; // Unchanged
                                continue;
                            }
                            morphes.Clear();
                            brickSeq = brickInst.Seq;
                            if (!map.DelBrickInst(brickSeq, ref morphes))
                            {
                                results[i] = -2; // Failed
                                continue;
                            }
                            newSeqs.Add(brickSeq);
                            if (flag.IsSet(OperationFlag.Delete))
                            {
                                results[i] = 0; // Brick deleted
                                continue;
                            }
                            // Deletion successful continue with adding replacement brick
                            result = 1;
                        } else
                        {
                            if (!flag.IsSet(OperationFlag.IncludeEmpty))
                            {
                                results[i] = -3; // Skipped
                                continue;
                            }
                            if (flag.IsSet(OperationFlag.Delete))
                            {
                                results[i] = 2; // Unchanged
                                continue;
                            }
                            result = 0;
                        }

                        brickSeq = msgRef.matchData.GetNextBrickSeq();
                        brickInst = map.AddBrickInst(brickSeq, targetIndex, x, y, z, 0, targetRotation);
                        if (brickInst == null)
                        {
                            results[i] = (sbyte) (-2 + result); // Failed, add result to account for partial replacement
                            continue;
                        }
                        newSeqs.Add(brickSeq);
                        results[i] = result; // Success
                    }
                } finally
                {
                    MyInfoManager.Instance.AuroraTemporarilyDisableBrickNetworkUpdates = false;
                }
            }

            // Broadcast one ACK to room
            MsgBody mb = new MsgBody();
            mb.Write(playerSeq);
            mb.Write(count);
            mb.Write(flag);
            mb.Write(targetIndex);
            mb.Write(targetRotation);

            int seqIdx = 0;
            for (int i = 0; i < count; i++)
            {
                mb.Write(xs[i]);
                mb.Write(ys[i]);
                mb.Write(zs[i]);
                result = results[i];
                mb.Write(result);
                if (result <= -2 || result == 2)
                {
                    continue;
                }
                mb.Write(newSeqs[seqIdx++]);
                if (result == 1)
                {
                    mb.Write(newSeqs[seqIdx++]);
                }
            }

            Say(new MsgReference(
                (int)ExtensionOpcodes.opBulkBrickAck,
                mb,
                msgRef.client,
                SendType.BroadcastRoom,
                msgRef.matchData.channel,
                msgRef.matchData
            ));
        }

        private void SendBulkFail(MsgReference req, int resultCode)
        {
            MsgBody mb = new MsgBody();
            mb.Write(resultCode);
            Say(new MsgReference((int)ExtensionOpcodes.opBulkBrickFailAck, mb, req.client));
        }

        private void HandleReplaceBrickRequest(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out long item);
            msgRef.msg._msg.Read(out string code);
            msgRef.msg._msg.Read(out int existingSeq);
            msgRef.msg._msg.Read(out byte brickIndex);
            msgRef.msg._msg.Read(out byte x);
            msgRef.msg._msg.Read(out byte y);
            msgRef.msg._msg.Read(out byte z);
            msgRef.msg._msg.Read(out byte rot);

            //Debug.LogWarning($"REPLACE_REQ item:{item} code:{code} brickIndex:{brickIndex} at({x},{y},{z}) rot:{rot}");

            if (msgRef?.matchData?.cachedMap == null)
            {
                Debug.LogError("HandleReplaceBrickRequest: matchData/cachedMap is null");
                SendReplaceFail(msgRef, -6);
                return;
            }

            int playerSeq = msgRef.client.seq;

            lock (dataLock)
            {
                var map = msgRef.matchData.cachedMap;

                BrickInst old = map.Get(existingSeq);
                if (old == null)
                {
                    Debug.LogError($"ReplaceBrick: existing seq not found: {existingSeq}");
                    SendReplaceFail(msgRef, -6);
                    return;
                }

                Brick newBrick = BrickManager.Instance.GetBrick(brickIndex);
                if (newBrick == null)
                {
                    Debug.LogWarning($"REPLACE_REQ: unknown brick index={brickIndex}");
                    SendReplaceFail(msgRef, -6);
                    return;
                }

                List<int> morphes = new List<int>();

                int newSeq = msgRef.matchData.GetNextBrickSeq();

                morphes.Clear();
                if (!map.DelBrickInst(existingSeq, ref morphes))
                {
                    Debug.LogError($"ReplaceBrick: DelBrickInst failed for seq={existingSeq}");
                    SendReplaceFail(msgRef, -6);
                    return;
                }

                morphes.Clear();
                if (!map.AddBrickInst(newSeq, brickIndex, old.PosX, old.PosY, old.PosZ, old.Rot, ref morphes))
                {
                    Debug.LogError($"ReplaceBrick: AddBrickInst failed newSeq={newSeq} template={brickIndex} at ({old.PosX},{old.PosY},{old.PosZ})");
                    SendReplaceFail(msgRef, -6);
                    return;
                }

                SendReplaceSuccess(msgRef, playerSeq, existingSeq, newSeq, brickIndex, old.PosX, old.PosY, old.PosZ, old.Rot);
            }
        }

        private void SendReplaceSuccess(MsgReference req, int playerSeq, int oldSeq, int newSeq, byte template, byte x, byte y, byte z, byte rot)
        {
            MsgBody msgBody = new MsgBody();
            msgBody.Write(playerSeq); // val
            msgBody.Write(oldSeq);    // val2
            msgBody.Write(newSeq);    // val3
            msgBody.Write(template);  // val4
            msgBody.Write(x);
            msgBody.Write(y);
            msgBody.Write(z);
            msgBody.Write(rot);

            Say(new MsgReference((int)MessageId.CS_REPLACE_BRICK_ACK, msgBody, req.client, SendType.BroadcastRoom, req.matchData.channel, req.matchData));
        }

        private void SendReplaceFail(MsgReference req, int resultCode)
        {
            // Client handler expects: msg.Read(out int val); ShowBuildErrorMessage(val); MoveNext(false)
            MsgBody msgBody = new MsgBody();
            msgBody.Write(resultCode);

            Say(new MsgReference((int)MessageId.CS_REPLACE_BRICK_FAIL_ACK, msgBody, req.client));
        }


        private void HandleMorphBrickRequest(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out int seq);
            msgRef.msg._msg.Read(out ushort code);

            var match = msgRef.matchData;
            if (match?.cachedMap == null)
                return;

            lock (dataLock)
            {
                BrickInst bi = match.cachedMap.Get(seq);
                if (bi == null)
                {
                    Debug.LogWarning($"MORPH_BRICK_REQ: unknown brick seq={seq}");
                    return;
                }
                bi.Code = code;
            }
        }

        private void HandleSaveMap(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out int slot);
            msgRef.msg._msg.Read(out byte[] textureBuffer);

            MatchData matchData = msgRef.matchData;

            DateTime time = DateTime.Now;

            // Generate ModeMask (if you still want it saved/used locally, keep it here; otherwise remove)
            ushort modeMask = GenerateModeMask(matchData.cachedMap.dic);

            // Load thumbnail from chunked buffer (local slot thumbnail)
            Texture2D thumbnail = new Texture2D(128, 128, TextureFormat.RGB24, mipmap: false);
            thumbnail.LoadImage(textureBuffer);
            thumbnail.Apply();
            if (debugSend) Debug.Log("Load Thumbnail (SaveMap)");

            // IMPORTANT: Do NOT touch RegMapManager here.
            // Only update the user map metadata (UMI) for this slot.
            // (Alias/BrickCount come from cachedUMI; adjust if you store alias elsewhere.)
            UserMapInfoManager.Instance.AddOrUpdate(
                slot,
                matchData.cachedUMI.Alias,
                matchData.cachedUMI.BrickCount,
                time,
                0 // premium
            );
            UserMapInfoManager.Instance.SetThumbnail(slot, thumbnail);
            UserMapInfoManager.Instance.CurMapName = matchData.cachedUMI.Alias;

            // Update current working map references WITHOUT registering
            matchData.cachedMap.map = slot;          // working map id stays "slot"
            matchData.cachedUMI.slot = slot;         // slot stays 1..12
            matchData.cachedUMI.regMap = null;       // NOT registered on save

            // Save files as user-slot files (local)
            // geometry: downloaded{slot}.geometry
            matchData.cachedMap.Save(slot, matchData.cachedMap.skybox);

            // if you have a cache save method for UMI on server-side:
            //matchData.cachedUMI.SaveCache();
            // If not, UserMapInfoManager probably handles it; otherwise add it there.

            MsgBody msgBody = new MsgBody();
            msgBody.Write(slot);
            msgBody.Write(0);
            Say(new MsgReference(40, msgBody, msgRef.client, SendType.BroadcastRoom, matchData.channel, matchData));
        }

        private void HandleChangeEditorPermissionRequest(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out int seq);
            msgRef.msg._msg.Read(out bool isEditor);

            MatchData matchData = msgRef.matchData;
            MsgBody msgBody = new MsgBody();

            msgBody.Write(seq);
            msgBody.Write(isEditor);
            Say(new MsgReference(305, msgBody, msgRef.client, SendType.BroadcastRoom, matchData.channel, matchData));
        }

        private void HandleCommonOpt(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out int opt);
            Debug.LogWarning("SaveCommonOpt: " + opt);
        }

        public static ushort GenerateModeMask(Dictionary<int, BrickInst> brickInstances)
        {
            ushort modeMask = 0;
            // Count occurrences of each Template
            var templateCounts = brickInstances
                .GroupBy(brick => brick.Value.Template)
                .ToDictionary(group => group.Key, group => group.Count());

            // Retrieve counts for specific templates
            int count23 = templateCounts.TryGetValue(23, out var c23) ? c23 : 0;
            int count22 = templateCounts.TryGetValue(22, out var c22) ? c22 : 0;
            int count24 = templateCounts.TryGetValue(24, out var c24) ? c24 : 0;
            int count121 = templateCounts.TryGetValue(121, out var c121) ? c121 : 0;
            int count122 = templateCounts.TryGetValue(122, out var c122) ? c122 : 0;
            int count123 = templateCounts.TryGetValue(123, out var c123) ? c123 : 0;
            int count124 = templateCounts.TryGetValue(124, out var c124) ? c124 : 0;
            int count134 = templateCounts.TryGetValue(134, out var c134) ? c134 : 0;
            int count135 = templateCounts.TryGetValue(135, out var c135) ? c135 : 0;
            int count136 = templateCounts.TryGetValue(136, out var c136) ? c136 : 0;
            int count181 = templateCounts.TryGetValue(181, out var c181) ? c181 : 0;

            // Calculate mode mask
            if (count23 >= 8 && count22 >= 8)
            {
                modeMask |= 1; // Team match mode
            }
            if (count24 >= 16)
            {
                modeMask |= 2; // Individual match mode
                modeMask |= 0x100; // Zombie mode
            }
            if (count23 >= 8 && count22 >= 8 && count121 >= 1 && count122 >= 1 && count123 >= 1)
            {
                modeMask |= 4; // CTF match mode
            }
            if (count124 >= 2 && count23 >= 8 && count22 >= 8)
            {
                modeMask |= 8; // Explosion match mode
            }
            if (count23 >= 8 && count22 >= 8 && count134 > 0 && count135 > 0 && count136 > 0)
            {
                modeMask |= 0x10; // Defense match mode
            }
            if (count24 >= 16 && count181 >= 1)
            {
                modeMask |= 0x80; // Escape mode
            }

            return modeMask;
        }

        private void HandleTCOpenRequest(MsgReference msgRef)
        {
            var chests = TreasureChestManager.Instance.ToArray();

            MsgBody msg = new MsgBody();

            // MUST BE FIRST: number of chests
            msg.Write(chests.Length);

            foreach (TcStatus tc in chests)
            {
                msg.Write(tc.Seq);
                msg.Write(tc.Index);
                msg.Write(tc.Max);
                msg.Write(tc.Cur);
                msg.Write(tc.Key);
                msg.Write(tc.MaxKey);
                msg.Write(tc.CoinPrice);
                msg.Write(tc.TokenPrice);
                msg.Write(tc.Alias);

                // number of unique item codes
                var items = tc.TcTItemToArray();
                var uniqueItems = items.GroupBy(i => i.code).ToArray();
                msg.Write(uniqueItems.Length);

                foreach (var group in uniqueItems)
                {
                    // group header
                    msg.Write(group.First().code);
                    msg.Write(group.Count());  // count (client expects this)

                    foreach (var item in group)
                    {  
                        // one entry
                        msg.Write(item.opt);
                        msg.Write((sbyte)(item.isKey ? 1 : 0));
                    }
                }
            }

            Say(new MsgReference(370, msg, msgRef.client, SendType.Unicast));
        }

        public void Handle_CS_ACCEPT_DAILY_MISSION_REQ(MsgReference msgRef)
        {
            MsgBody msg = new MsgBody();
            msg.Write(0);

            Say(new MsgReference(384, msg, msgRef.client, SendType.Unicast));
            Send_MISSION_ACK(msgRef);
        }

        public void Send_MISSION_ACK(MsgReference msgRef)
        {

            MsgBody msg = new MsgBody();

            msg.Write(1);
            msg.Write("MSSN_KILL_MELEE");
            msg.Write(50);
            msg.Write(0);
            msg.Write(false);
            msg.Write(4);

            Say(new MsgReference(381, msg, msgRef.client, SendType.Unicast));

            msg = new MsgBody();
            msg.Write(2);
            msg.Write("MSSN_WIN_TM");
            msg.Write(5);
            msg.Write(0);
            msg.Write(false);
            msg.Write(4);

            Say(new MsgReference(381, msg, msgRef.client, SendType.Unicast));

            msg = new MsgBody();
            msg.Write(5);
            msg.Write("MSSN_WIN_CTF");
            msg.Write(5);
            msg.Write(0);
            msg.Write(false);
            msg.Write(4);

            Say(new MsgReference(381, msg, msgRef.client, SendType.Unicast));
        }

        private void HandleBrickBatchDeleteRequest(MsgReference msgRef)
        {
            MatchData matchData = msgRef.matchData;

            if (debugHandle)
                Debug.Log("HandleDelBrickRequest from: " + msgRef.client.GetIdentifier());

            msgRef.msg._msg.Read(out int length);
            List<int> morphes = new List<int>();
            int[] sequences = new int[length];
            for (int i = 0; i < length; i++)
            {
                msgRef.msg._msg.Read(out int seq);
                sequences[i] = seq;
                matchData.cachedMap.DelBrickInst(seq, ref morphes);
            }
            SendBatchBrick(msgRef.client, sequences);
        }

        public void SendBatchBrick(ClientReference client, int[] sequences)
        {
            MatchData matchData = client.matchData;

            MsgBody body = new MsgBody();

            body.Write(client.seq);
            body.Write(sequences.Length);
            for (int i = 0; i < sequences.Length; i++)
            {
                body.Write(sequences[i]);
            }

            Say(new MsgReference(480, body, client, SendType.BroadcastRoom, matchData.channel, matchData));

            if (debugSend)
                Debug.Log("SendBatchDeleteBrick for room no " + matchData.room.No + " " + client.GetIdentifier());
        }

        public void HandleMissionPointRequest(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out int redPoint);
            msgRef.msg._msg.Read(out int bluePoint);
            MatchData matchData = msgRef.client.matchData;
            //Missing Server Logic Here
            MsgBody body = new MsgBody();
            Debug.Log("RedPoint: " + redPoint + " BluePoint: " + bluePoint);

            body.Write(redPoint);
            body.Write(bluePoint);

            Say(new MsgReference(509, body, msgRef.client, SendType.BroadcastRoom, matchData.channel, matchData));

            if (debugSend)
                Debug.Log("HandleMissionPointRequest for room no " + matchData.room.No + " " + msgRef.client.GetIdentifier());
        }

        public void HandleCoreHPReq(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out int redHp);
            msgRef.msg._msg.Read(out int blueHp);
            MatchData matchData = msgRef.client.matchData;
            MsgBody body = new MsgBody();
            Debug.Log("RedHp: " + redHp + " BlueHp: " + blueHp);
            //if hp <= 0 end match
            if (redHp <= 0 || blueHp <= 0)
            {
                matchData.EndMatch();
            }

            body.Write(redHp);
            body.Write(blueHp);

            Say(new MsgReference(181, body, msgRef.client, SendType.BroadcastRoom, matchData.channel, matchData));

            if (debugSend)
                Debug.Log("HandleCoreHPReq for room no " + matchData.room.No + " " + msgRef.client.GetIdentifier());
        }

        private void HandleInflictedDamage(MsgReference msgRef)
        {
            ClientReference client = msgRef.client;
            MatchData matchData = msgRef.matchData;

            // How many entries the client sends
            msgRef.msg._msg.Read(out int count);

            if (count <= 0)
                return;

            int totalDamage = 0;

            for (int i = 0; i < count; i++)
            {
                msgRef.msg._msg.Read(out int targetSeq);
                msgRef.msg._msg.Read(out int damage);

                // Ignore invalid data
                if (damage <= 0)
                    continue;

                // Damage must be applied to someone inside the same match
                ClientReference target = matchData.clientList.Find(x => x.seq == targetSeq);
                if (target == null)
                    continue;

                // Prevent cheating (client should never send huge values)
                if (damage > 200)
                    damage = 200;

                totalDamage += damage;
            }

            if (totalDamage <= 0)
                return;

            // -----------------------------------------
            //  REWARD CALCULATION (DAMAGE ONLY)
            // -----------------------------------------
            // Brick-Force rewarded small score per damage point.
            // Recommended balanced value:
            //
            // 1 score per ~7 damage.
            //
            // Damage reward formula:
            int reward = Mathf.Max(1, totalDamage / 7);

            // Add to overall player score
            client.score += reward;

            // Optionally: Debug log
            // Debug.Log($"[InflictedDamage] {client.name} inflicted {totalDamage} → +{reward} score");

            MsgBody msg = new MsgBody();
            msg.Write(client.seq);      // val  → player sequence ID
            msg.Write(client.score);    // val2 → updated score

            Say(new MsgReference(300, msg, msgRef.client, SendType.Unicast));
        }

        private void HandleCS_TC_LEAVE_REQ(MsgReference msgRef)
        {
            msgRef.client.lastOpenedChestSeq = -1;
            //update chest?
            //update inv
            ClientExtension.instance.inventory.Apply();
            ClientExtension.instance.inventory.Save();
            ClientExtension.instance.SendInventoryData();
        }

        private void HandleCS_TC_ENTER_REQ(MsgReference msgRef)
        {
            MsgBody req = msgRef.msg._msg;
            req.Read(out int chestSeq);

            TcStatus tc = TreasureChestManager.Instance.Get(chestSeq);
            msgRef.client.lastOpenedChestSeq = tc.Seq;
            if (tc == null)
            {
                MsgBody fail = new MsgBody();
                fail.Write(1);
                fail.Write(chestSeq);
                fail.Write(0);
                fail.Write(0);
                Say(new MsgReference(373, fail, msgRef.client, SendType.Unicast));
                return;
            }

            // Collect rare items (premium)
            List<int> rarePositions = TreasureChestManager.Instance.GetRareTiles(tc.Seq);

            // SUCCESS response
            MsgBody msg = new MsgBody();

            // 1) result code
            msg.Write(0);

            // 2) chest seq
            msg.Write(tc.Seq);

            // 3) bitmask length
            byte[] mask = TreasureChestManager.Instance.GetBitmask(tc.Seq);
            msg.Write(mask.Length);
            foreach (byte b in mask)
                msg.Write(b);

            // 5) number of rare items
            msg.Write(rarePositions.Count);

            // 6) rare tile indices
            foreach (int pos in rarePositions)
                msg.Write(pos);

            Say(new MsgReference(373, msg, msgRef.client, SendType.Unicast));
        }

        private void HandleCS_TC_OPEN_PRIZE_TAG_REQ(MsgReference msgRef)
        {
            MsgBody req = msgRef.msg._msg;

            req.Read(out int chestSeq);
            req.Read(out int index);     // board index?
            req.Read(out bool isCoin);

            TcStatus tc = TreasureChestManager.Instance.Get(chestSeq);
            if (tc == null)
            {
                // no such chest
                MsgBody fail = new MsgBody();
                fail.Write(-5L);     // matches TC_NO_SUCH_PRIZE
                fail.Write(chestSeq);
                fail.Write(index);
                fail.Write(0);
                fail.Write(false);
                fail.Write(false);
                Say(new MsgReference(376, fail, msgRef.client, SendType.Unicast));
                return;
            }

            // get rare tile positions
            var rareTiles = TreasureChestManager.Instance.GetRareTiles(tc.Seq);

            // check if clicked tile is rare
            bool clickedRare = rareTiles.Contains(index);

            TcTItem item;

            if (clickedRare)
            {
                // return the first rare item
                item = tc.GetFirstRare();

                // update chest (remove 1 rare)
                tc.Update(tc.Cur - 1, tc.Key - 1, tc.MaxKey);
            }
            else
            {
                // return a random normal
                var normals = tc.GetNormalArray();
                item = normals[UnityEngine.Random.Range(0, normals.Length)];

                // update chest (rare count unchanged)
                tc.Update(tc.Cur - 1, tc.Key, tc.MaxKey);
            }
            SendTookoff(msgRef.client, tc, index, item.isKey);
            SendChestUpdate(msgRef.client, tc);
            TreasureChestManager.Instance.OpenTile(tc.Seq, index);

            if (isCoin)
            {
                int coins = msgRef.client.data.coins = msgRef.client.data.coins - tc.CoinPrice;
                //check so user does not go negative
                MsgBody bodyUpdate = new MsgBody();
                bodyUpdate.Write(msgRef.client.data.forcePoints);
                bodyUpdate.Write(msgRef.client.data.brickPoints);
                bodyUpdate.Write(msgRef.client.data.tokens);
                bodyUpdate.Write(coins);
                bodyUpdate.Write(msgRef.client.data.starDust);
                Say(new MsgReference(102, bodyUpdate, msgRef.client, SendType.Unicast));
            }
            else
            {
                int tokens = msgRef.client.data.tokens = msgRef.client.data.tokens - tc.TokenPrice;
                //check negative
                MsgBody bodyUpdate = new MsgBody();
                bodyUpdate.Write(msgRef.client.data.forcePoints);
                bodyUpdate.Write(msgRef.client.data.brickPoints);
                bodyUpdate.Write(tokens);
                bodyUpdate.Write(msgRef.client.data.coins);
                bodyUpdate.Write(msgRef.client.data.starDust);
                Say(new MsgReference(102, bodyUpdate, msgRef.client, SendType.Unicast));
            }

            int amount = 1;
            // SEND ACK (client will call RECEIVE_PRIZE_REQ afterwards)
            long timestamp = DateTimeOffset.UtcNow.Ticks;
            MsgBody ack = new MsgBody();
            ack.Write(timestamp);   // error code /item
            ack.Write(chestSeq);   // unused by client but expected
            ack.Write(index);      // val3
            ack.Write(amount);     // val4
            ack.Write(item.isKey);     // val5 (wasKey)
            ack.Write(isCoin);   // val6

            Say(new MsgReference(376, ack, msgRef.client, SendType.Unicast));
        }


        private void HandleCS_TC_RECEIVE_PRIZE_REQ(MsgReference msgRef)
        {
            MsgBody req = msgRef.msg._msg;

            // The client passes the prize CODE here, not a unique seq
            req.Read(out long item);
            Debug.Log(item);
            req.Read(out int index);      // tile index
            Debug.Log(index);
            req.Read(out int amount);
            req.Read(out bool wasKey);
            req.Read(out bool freeCoin);

            // 1. Get chest containing this tile
            // The client knows the chest it opened; server must track last-entered chest per client.
            TcStatus tc = TreasureChestManager.Instance.Get(msgRef.client.lastOpenedChestSeq);
            if (tc == null)
            {
                MsgBody fail = new MsgBody();
                fail.Write(-2);
                Say(new MsgReference(380, fail, msgRef.client, SendType.Unicast));
                return;
            }
            
            //Update Board Here on TreasureChestManager
            SendChestUpdate(msgRef.client, tc);

            // 2. Find the TcTItem that matches the code
            TcTItem reward = tc.TcTItemToArray()[index];

            // 3. Load template from TItemManager
            TItem template = TItemManager.Instance.Get<TItem>(reward.code.ToString());
            if (template == null)
            {
                MsgBody fail = new MsgBody();
                fail.Write(-6);
                Say(new MsgReference(380, fail, msgRef.client, SendType.Unicast));
                return;
            }

            // 5. Build SUCCESS response packet
            MsgBody msg = new MsgBody();
            msg.Write(0);                 // val >= 0 = success
            msg.Write(item);              // seq (actually item code)
            msg.Write(template.code);              // item code again
            msg.Write((sbyte)Item.USAGE.UNEQUIP);
            msg.Write(amount);            // remain amount
            msg.Write(index);             // tile index
            msg.Write(amount);            // amount
            msg.Write(wasKey);            // rare item?
            msg.Write(reward.opt);        // durability (days)

            Say(new MsgReference(380, msg, msgRef.client, SendType.Unicast));

            // 6. Add item to player inventory
            msgRef.client.inventory.AddItem(template, false, reward.opt * 86400);
            //ClientExtension.instance.inventory.AddItem(template, false, reward.opt * 86400);
            //msgRef.client.myInfo.ReceivePrize(
              //  0,           // seq is ignored by your ReceivePrize implementation unless >0
             //   code,
             //   Item.USAGE.NOT_USING,
             //   amount,
              //  durability
            //);
        }
        private void SendChestUpdate(ClientReference client, TcStatus tc)
        {
            MsgBody body = new MsgBody();
            body.Write(tc.Seq);
            body.Write(tc.Cur);
            body.Write(tc.Key);
            body.Write(tc.MaxKey);

            // CS_TC_UPDATE_CHEST_ACK = 378
            Say(new MsgReference(378, body, client, SendType.Unicast));
        }

        private void SendChest(ClientReference client, TcStatus tc)
        {
            MsgBody msg = new MsgBody();

            msg.Write(tc.Seq);
            msg.Write(tc.Index);
            msg.Write(tc.Max);
            msg.Write(tc.Cur);
            msg.Write(tc.Key);
            msg.Write(tc.MaxKey);
            msg.Write(tc.CoinPrice);
            msg.Write(tc.TokenPrice);
            msg.Write(tc.Alias);

            // number of unique item codes
            msg.Write(1);
            var items = tc.TcTItemToArray();
            // group header
            msg.Write(items[0].code);
            msg.Write(items.Length);  // count (client expects this)

            foreach (var item in items)
            {
                // one entry
                msg.Write(item.opt);
                msg.Write((sbyte)(item.isKey ? 1 : 0));
            }
            Say(new MsgReference(375, msg, client, SendType.Unicast));
        }

        private void SendTookoff(ClientReference client, TcStatus tc, int index, bool wasKey)
        {
            MsgBody msg = new MsgBody();

            msg.Write(tc.Seq);
            msg.Write(index);
            msg.Write(wasKey);

            Say(new MsgReference(377, msg, client, SendType.Unicast));
        }

        private void HandleKickRequest(MsgReference msgRef)
        {
            //not tested/ server side remove?
            msgRef.msg._msg.Read(out int seq);
            //msgRef.matchData.RemoveClient(clientList.Find(client => client.seq == seq));
            MsgBody body = new MsgBody();
            body.Write(seq);
            Say(new MsgReference(89, body, msgRef.client, SendType.Unicast));
        }

        private void HandleChangeUserMapAliasRequest(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out int slot);
            msgRef.msg._msg.Read(out string newAlias);

            UserMapInfoManager.Instance.Get(slot).Alias = newAlias;
            bool ok = UserMapInfoManager.Instance.Get(slot).SaveCache();

            MsgBody body = new MsgBody();
            body.Write(ok ? 1 : 0); //success
            body.Write((sbyte)slot);
            body.Write(newAlias);

            Say(new MsgReference(55, body, msgRef.client, SendType.Unicast));
        }

        private void HandleVersionCheck(MsgReference msg)
        {
            ClientReference client = msg.client;
            if (client.isVersionSetUp)
            {
                return;
            }
            client.isVersionSetUp = true;
            if (client.isHost)
            {
                SayInstant(new MsgReference(ExtensionOpcodes.opVersionCheckAck, new MsgBody(), client));
                return;
            }

            MsgBody body = msg.msg._msg;
            body.Read(out int clientMajor);
            body.Read(out int clientMinor);
            body.Read(out int clientPatch);
            body.Read(out int clientRevision);

            if (clientMajor == -1)
            {
                // We are pretty sure the client has an unknown version if this is -1
                Debug.LogWarning($"Disconnecting client: Version check failed, client sent invalid version");
                SendDisconnect(client, message: $"Version mismatch detected, the host is using a newer version ({hostVersion})");
                msg.client.Disconnect(false);
                return;
            }

            Version clientVersion = new Version(clientMajor, clientMinor, clientPatch, clientRevision);
            int result = hostVersion.CompareTo(clientVersion);
            if (result != 0)
            {
                Debug.LogWarning($"Disconnecting client: Version check failed, expected {hostVersion} but got {clientVersion}");
                string relation = result < 0 ? "an older" : "a newer";
                SendDisconnect(client, message: $"Version mismatch detected, the host is using {relation} version ({hostVersion})");
                msg.client.Disconnect(false);
                return;
            }
            if (debugHandle)
            {
                Debug.Log($"Version check succeeded, got host version {hostVersion} and client version {clientVersion}");
            }
            SayInstant(new MsgReference(ExtensionOpcodes.opVersionCheckAck, new MsgBody(), client));
        }
    }
}