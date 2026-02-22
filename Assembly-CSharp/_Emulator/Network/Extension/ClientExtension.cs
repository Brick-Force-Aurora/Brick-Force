using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using _Emulator.Network;
using Steamworks;
using UnityEngine;
using static Room;
using static WindowSystemManager;
using Debug = UnityEngine.Debug;

namespace _Emulator
{
    public class ClientExtension : MonoBehaviour
    {
        public static ClientExtension instance;
        public string hostIP = "";
        public Inventory inventory;
        public bool clientConnected = false;
        public MsgBody lastKillLogMsg;
        public int lastKillLogId = -1;
        public float killLogRealiableTime = 0f;
        public bool isSteam = false;

        public float lastAnswer = 0f, connectedRequest = 0f;
        private volatile bool receivedAnswer = false, awaitingConnectedAnswer = false;

        public ChunkedBufferReceiver chunkedBufferReceiver = new ChunkedBufferReceiver();

        public readonly Version clientVersion;

        private readonly Dictionary<ushort, Action<MsgBody>> _handlers = new Dictionary<ushort, Action<MsgBody>>();

        private ClientExtension()
        {
            clientVersion = GetGithubVersionOrUnknown();
            Debug.Log($"GameVersion: {clientVersion}");
            RegisterHandlers();
        }

        private void RegisterHandlers()
        {
            Action<MessageId, Action<MsgBody>> register = (messageId, action) => _handlers[(ushort)messageId] = action;
            Action<ExtensionOpcodes, Action<MsgBody>> registerCustom = (messageId, action) => _handlers[(ushort)messageId] = action;

            registerCustom(ExtensionOpcodes.opConnectedAck, HandleConnected);
            registerCustom(ExtensionOpcodes.opVersionCheckAck, HandleVersionCheck);
            registerCustom(ExtensionOpcodes.opSlotDataAck, HandleReceiveSlotData);
            registerCustom(ExtensionOpcodes.opPostLoadInitAck, HandlePostLoadInit);
            registerCustom(ExtensionOpcodes.opInventoryReq, HandleRequestInventory);
            registerCustom(ExtensionOpcodes.opCustomMessageAck, HandleCustomMessage);
            registerCustom(ExtensionOpcodes.opDisconnectAck, HandleDisconnected);
            registerCustom(ExtensionOpcodes.opRendezvousInfoSteamAck, HandleRendezvousInfoSteam);
            registerCustom(ExtensionOpcodes.opEnterSteamAck, HandleEnterSteam);
            registerCustom(ExtensionOpcodes.opSlotDataSteamAck, HandleReceiveSlotDataSteam);
            registerCustom(ExtensionOpcodes.opBulkBrickAck, HandleBulkBrickAck);
            registerCustom(ExtensionOpcodes.opBulkBrickFailAck, HandleBulkBrickFailAck);
            registerCustom(ExtensionOpcodes.opBeginChunkedBufferReq, HandleBeginChunkedBufferReceive);
            registerCustom(ExtensionOpcodes.opChunkedBufferReq, HandleChunkedBufferReceive);
            registerCustom(ExtensionOpcodes.opEndChunkedBufferReq, HandleEndChunkedBufferReceive);
            registerCustom(ExtensionOpcodes.opAmIConnectedAck, HandleAmIConnected);
            registerCustom(ExtensionOpcodes.opRequestUserSlotMapAck, HandleRequestUserSlotMap);

            Type messageIdType = typeof(ExtensionOpcodes);
            foreach (ExtensionOpcodes id in Enum.GetValues(messageIdType))
            {
                ushort mapId = (ushort)id;
                if (_handlers.ContainsKey(mapId))
                {
                    continue;
                }
                string name = Enum.GetName(messageIdType, id);
                _handlers[mapId] = msgRef => Debug.LogWarning($"Server sent unhandled packet: {name} ({mapId})");
            }
        }

        public void LoadServer()
        {
            receivedAnswer = true;
            CSNetManager.Instance.BfServer = hostIP;
            CSNetManager.Instance.BfPort = 5000;
            GameObject gameObject = GameObject.Find("Main");
            if (null != gameObject)
            {
                //Debug.Log(gameObject);
                //gameObject.BroadcastMessage("OnRoundRobin");
                var login = gameObject.GetComponentInChildren<Login>();
                if (login != null)
                {
                    //login.loginStep = Login.LOGIN_STEP.WAITING_SEED;
                    //login.id = "";
                    login.BroadcastMessage("OnRoundRobin");
                }

            }
            ShopEmulator shop = new ShopEmulator();
            //shop.LoadAndSave();
            shop.ParseData();
        }

        public void LoadServerSteam()
        {
            if (SteamManager.Initialized)
            {
                receivedAnswer = true;
                isSteam = true;
                GameObject gameObject = GameObject.Find("Main");
                if (null != gameObject)
                {
                    var login = gameObject.GetComponentInChildren<Login>();
                    if (login != null)
                    {
                        login.loginStep = Login.LOGIN_STEP.WAITING_SEED;
                        login.id = SteamFriends.GetPersonaName();
                        //SteamNetworkingManager.instance.SendInitMessageToHost();
                    }
                }

                ShopEmulator shop = new ShopEmulator();
                //shop.LoadAndSave();
                shop.ParseData();
            }
        }

        public void SendPacket(ushort id, MsgBody msgBody, bool doChunked)
        {
            if (isSteam)
            {
                SteamNetworkingManager.instance.SendMessageToHost(new Msg4Send(id, uint.MaxValue, uint.MaxValue, msgBody, CSNetManager.Instance.Sock.GetSendKey()));
                return;
            }

            SockTcp sock = CSNetManager.Instance.Sock;
            if (sock == null || sock._writeQueue == null)
            {
                return;
            }

            if (!doChunked || msgBody.Offset <= ChunkedBufferReceiver.MAX_CHUNK_LENGTH)
            {
                EnqueuePacket(sock, new Msg4Send(id, uint.MaxValue, uint.MaxValue, msgBody, sock.GetSendKey()));
                return;
            }

            msgBody.SendChunked(id, sock.GetSendKey(), "Client", (send) => EnqueuePacket(sock, send));
        }

        private void EnqueuePacket(SockTcp sock, Msg4Send send)
        {
            if (sock._writeQueue.Count > 0)
            {
                sock._writeQueue.Enqueue(send);
                return;
            }
            sock._writeQueue.Enqueue(send);
            try
            {
                if (sock._sock != null)
                {
                    sock._sock.BeginSend(send.Buffer, 0, send.Buffer.Length, SocketFlags.None, sock.SendCallback, null);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error, " + ex.Message.ToString());
                sock.Close();
            }
        }

        void FixedUpdate()
        {
            if (!clientConnected)
                return;
            HandleClientTimeout();
        }

        private void HandleClientTimeout()
        {
            float time = Time.time;
            if (receivedAnswer)
            {
                receivedAnswer = false;
                lastAnswer = time;
                return;
            }
            if (time - lastAnswer <= 5f)
            {
                return;
            }
            if (!awaitingConnectedAnswer)
            {
                awaitingConnectedAnswer = true;
                connectedRequest = time;
                Say(ExtensionOpcodes.opAmIConnectedReq);
                return;
            }
            if (time - connectedRequest > 3f)
            {
                Disconnect("Disconnected from server:\nServer didn't respond for too long");
            }
        }

        private void Disconnect(string message = null)
        {
            bool custom = true;
            if (message == null)
            {
                custom = false;
                message = StringMgr.Instance.Get("NETWORK_BROKEN");
            }
            clientConnected = false;
            lastAnswer = 0;
            connectedRequest = 0;
            awaitingConnectedAnswer = false;
            receivedAnswer = false;
            if (!isSteam)
            {
                if (CSNetManager.Instance.Sock != null)
                    CSNetManager.Instance.Sock.Close();
                BuildOption.Instance.Exit();
                Actor.Instance.ShowDelayedMessage(message);
            }
            else
            {
                if (custom)
                {
                    SteamLobbyManager.instance.LeaveCurrentLobbyAndShutdown(message);
                }
                else
                {
                    SteamLobbyManager.instance.LeaveCurrentLobbyAndShutdown();
                }
            }
        }

        public void ReceiveSteam(CSteamID steamID, byte[] msg)
        {
            if (!isSteam)
                return;

            if (msg == null)
            {
                Debug.LogError("ReceiveSteam (Client): msg was null");
                return;
            }

            if (msg.Length < 15)
            {
                Debug.LogError("ReceiveSteam (Client): msg length was " + msg.Length);
                return;
            }

            if (CSNetManager.Instance.Sock == null)
            {
                CSNetManager.Instance.Sock = new SockTcp();
                CSNetManager.Instance.Sock.Init();
                Debug.LogError("ReceiveSteam (Client): Sock was null");
            }

            try
            {
                // Only receive from host
                if (SteamLobbyManager.instance.IsCurrentOwner(steamID))
                {
                    Msg4Recv recv = new Msg4Recv(msg);
                    recv._hdr.FromArray(recv.Buffer);
                    MsgBody msgBody = recv.Flush();
                    msgBody.Decrypt(CSNetManager.Instance.Sock.recvKey);

                    lock (CSNetManager.Instance.Sock)
                    {
                        CSNetManager.Instance.Sock._readQueue.Enqueue(new Msg2Handle(recv.GetId(), msgBody));
                    }
                }
            }

            catch (Exception ex)
            {
                Debug.LogError("ReceiveSteam (Client): " + ex.Message);
            }
        }

        public void Say(MessageId id, MsgBody msgBody = null, bool doChunked = true)
        {
            if (msgBody == null)
                msgBody = new MsgBody();
            CSNetManager.Instance.Sock.Say((ushort)id, msgBody, doChunked);
        }

        public void Say(ExtensionOpcodes id, MsgBody msgBody = null, bool doChunked = true)
        {
            if (msgBody == null)
                msgBody = new MsgBody();
            CSNetManager.Instance.Sock.Say((ushort)id, msgBody, doChunked);
        }

        public void Say(ushort id, MsgBody msgBody = null, bool doChunked = true)
        {
            if (msgBody == null)
                msgBody = new MsgBody();
            CSNetManager.Instance.Sock.Say(id, msgBody, doChunked);
        }

        public void UpdateLocalInventory()
        {
            inventory.UpdateActiveEquipment();
            inventory.Apply();
            SendInventoryData();
        }

        public void GetGamestateStrings(out string roomType, out string roomStatus, out string mapAlias, out string playerStatus)
        {
            roomType = BfUtils.RoomTypeToString(RoomManager.Instance.CurrentRoomType);
            roomStatus = BfUtils.RoomStatusToString(RoomManager.Instance.CurrentRoomStatus);
            if (RoomManager.Instance.CurrentRoomType == ROOM_TYPE.MAP_EDITOR)
                mapAlias = UserMapInfoManager.Instance.CurMapName != null ? UserMapInfoManager.Instance.CurMapName : "None";
            else
            {
                var map = RegMapManager.Instance.Get(RoomManager.Instance.CurMap);
                if (map != null)
                    mapAlias = map.Alias;
                else
                    mapAlias = "None";
            }

            var status = (BrickManDesc.STATUS)MyInfoManager.Instance.Status;
            playerStatus = BfUtils.BrickManStatusToString(status);
        }

        public void HandleReliableKillLog()
        {
            if (lastKillLogId != -1)
            {
                killLogRealiableTime += Time.deltaTime;
                if (killLogRealiableTime > 0.1f)
                {
                    killLogRealiableTime = 0f;
                    CSNetManager.Instance.Sock.Say(44, lastKillLogMsg);
                }
            }
        }

        public bool HandleMessage(Msg2Handle msg)
        {
            receivedAnswer = true;
            if (ServerEmulator.instance.debugSend)
                Debug.Log($"[Verbose/Client] Processing message ID: {msg._id}");
            if (!_handlers.ContainsKey(msg._id))
            {
                return false;
            }
            _handlers[msg._id].Invoke(msg._msg);
            return true;
        }

        private void HandleAmIConnected(MsgBody body)
        {
            connectedRequest = 0;
            awaitingConnectedAnswer = false;
        }

        private void HandleRequestUserSlotMap(MsgBody body)
        {
            body.Read(out int slot);
            UserMapInfo info = UserMapInfoManager.Instance.Get(slot);
            UserMap map = new UserMap();
            if (info == null || !map.Load(info.Slot))
            {
                Say(ExtensionOpcodes.opRequestUserSlotMapFailedReq);
                return;
            }
            SendCacheBrick(map);
            SendCacheBrickDone(map);
            body = new MsgBody();
            body.Write(info.BrickCount);
            Say(ExtensionOpcodes.opRequestUserSlotMapSuccessReq, body);
        }
        public void SendCacheBrick(UserMap userMap)
        {
            List<KeyValuePair<int, BrickInst>> brickList = userMap.dic.ToList();

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

                Say(21, body, doChunked: false);
            }
        }

        public void SendCacheBrickDone(UserMap userMap)
        {

            MsgBody body = new MsgBody();

            body.Write(-1);
            body.Write(userMap.skybox);

            Say(22, body, doChunked: false);
        }

        private void HandleBeginChunkedBufferReceive(MsgBody body)
        {
            if (!chunkedBufferReceiver.Begin(body))
            {
                Disconnect("Received invalid chunked buffer from server");
            }
        }

        private void HandleChunkedBufferReceive(MsgBody body)
        {
            if (!chunkedBufferReceiver.ReceiveChunk(body))
            {
                Disconnect("Received invalid chunked buffer from server");
            }
        }

        private void HandleEndChunkedBufferReceive(MsgBody body)
        {
            ushort packedOpcode;
            MsgBody packedBody;
            if (!chunkedBufferReceiver.End(body, out packedOpcode, out packedBody))
            {
                Disconnect("Received invalid chunked buffer from server");
                return;
            }
            lock (CSNetManager.Instance.Sock)
            {
                CSNetManager.Instance.Sock._readQueue.Enqueue(new Msg2Handle(packedOpcode, packedBody));
            }

        }

        private void HandleConnected(MsgBody msg)
        {
            clientConnected = true;
            SendVersionCheck();
        }

        private void HandleVersionCheck(MsgBody msg)
        {
            MainGUI.instance.setupHidden = true;
            CSNetManager.Instance.Sock._heartbeat = true;
            GameObject gameObject = GameObject.Find("Main");
            if (null != gameObject)
            {
                gameObject.BroadcastMessage("OnSeed");
            }
            Core.SetBalancedItemProperties();
        }

        private void SendVersionCheck()
        {
            MsgBody body = new MsgBody();
            body.Write(clientVersion.Major);
            body.Write(clientVersion.Minor);
            body.Write(clientVersion.Patch);
            body.Write(clientVersion.Revision);

            Say(ExtensionOpcodes.opVersionCheckReq, body);
        }

        public uint SendBulkBrickRequest(ushort flag, byte sourceTemplate, byte sourceRotation, byte targetTemplate, byte targetRotation, byte[] coords)
        {
            if (coords.Length == 0)
            {
                // Cancel request cause no coordinates
                return 0;
            }

            MsgBody mb = new MsgBody();
            mb.Write(flag);
            mb.Write(sourceTemplate);
            mb.Write(sourceRotation);
            mb.Write(targetTemplate);
            mb.Write(targetRotation);

            float countRes = coords.Length / 3;
            uint count = (uint) Mathf.FloorToInt(countRes);
            if (count != countRes || count == 0)
            {
                // Cancel request cause invalid coordinate count
                return 0;
            }

            mb.Write(count);
            for (int i = 0; i < coords.Length; i++)
            {
                mb.Write(coords[i]);
            }

            Say(ExtensionOpcodes.opBulkBrickReq, mb);

            // Return changes count
            return count;
        }

        private void HandleBulkBrickAck(MsgBody msg)
        {
            // Possibly make this async then?
            msg.Read(out int playerSeq);
            msg.Read(out uint count);
            msg.Read(out ushort flag);
            msg.Read(out byte targetIndex);
            msg.Read(out byte targetRotation);

            if (!flag.IsSet(OperationFlag.Delete))
            {
                Brick targetBrick = BrickManager.Instance.GetBrick(targetIndex);
                if (targetBrick == null)
                {
                    if (playerSeq == MyInfoManager.Instance.Seq)
                    {
                        OperationProcessor.Instance.NextBulkOperation(0);
                    }
                    return;
                }
            }
            uint success = 0;
            MyInfoManager.Instance.AuroraTemporarilyDisableBrickNetworkUpdates = true;
            List<int> morphes = new List<int>((int)count), brickAddMorphes = new List<int>(2);
            List<BulkChange> changes = new List<BulkChange>(morphes.Count);
            try
            {
                int brickSeq;
                byte x, y, z;
                sbyte result;
                UserMap userMap = BrickManager.Instance.userMap;

                for (int i = 0; i < count; i++)
                {
                    brickAddMorphes.Clear();
                    msg.Read(out x);
                    msg.Read(out y);
                    msg.Read(out z);
                    msg.Read(out result);

                    if (result >= 0)
                    {
                        success++;
                    }
                    if (result <= -2 || result == 2)
                    {
                        continue;
                    }
                    if (flag.IsSet(OperationFlag.Delete))
                    {
                        if (result != 0)
                        {
                            // This should never happen
                            continue;
                        }
                        msg.Read(out brickSeq);
                        BrickManager.Instance.DeleteBrickBulk(brickSeq, ref morphes);
                        continue;
                    }
                    if (result == -1 || result == 1)
                    {
                        msg.Read(out brickSeq);
                        BrickManager.Instance.DeleteBrickBulk(brickSeq, ref morphes);
                        if (result == -1)
                        {
                            continue;
                        }
                    }
                    msg.Read(out brickSeq);
                    userMap.AddBrickInst(brickSeq, targetIndex, x, y, z, targetRotation, ref brickAddMorphes);
                    changes.Add(new BulkChange(brickSeq, targetIndex, targetRotation, userMap.GetMeshCode(brickSeq), x, y, z, brickAddMorphes.ToArray()));
                }
            }
            finally
            {
                BulkChangeProcessor.Instance.Push(ref morphes, ref changes);
                changes.Clear();
                morphes.Clear();
                MyInfoManager.Instance.AuroraTemporarilyDisableBrickNetworkUpdates = false;
                if (playerSeq == MyInfoManager.Instance.Seq)
                {
                    OperationProcessor.Instance.NextBulkOperation(success);
                }
            }
        }

        private void HandleBulkBrickFailAck(MsgBody msg)
        {
            msg.Read(out int val);
            Debug.LogError("Failed to process Bulkbrick " + val);
            OperationProcessor.Instance.NextBulkOperation(0);
        }

        private void HandleDisconnected(MsgBody msg)
        {
            string message = StringMgr.Instance.Get("NETWORK_BROKEN");
            if (msg.Buffer.Length != 0)
            {
                msg.Read(out message);
            }
            Disconnect(message);
        }

        private void HandleRendezvousInfoSteam(MsgBody msg)
        {
            msg.Read(out ulong steamID64);
            P2PExtension.instance.BootupSteam();
        }

        private void HandleEnterSteam(MsgBody msg)
        {
            msg.Read(out int slot);
            msg.Read(out int seq);
            msg.Read(out string name);
            msg.Read(out ulong steamID64);
            msg.Read(out int arrayLength);
            string[] equip = new string[arrayLength];
            for (int j = 0; j < arrayLength; j++)
            {
                msg.Read(out equip[j]);
            }
            msg.Read(out int status);
            msg.Read(out int xp);
            msg.Read(out int clanSeq);
            msg.Read(out string clanName);
            msg.Read(out int clanMark);
            msg.Read(out int rank);
            msg.Read(out byte playerflag);
            msg.Read(out arrayLength);
            string[] weaponChg = (arrayLength > 0) ? new string[arrayLength] : null;
            for (int j = 0; j < arrayLength; j++)
            {
                msg.Read(out weaponChg[j]);
            }
            msg.Read(out arrayLength);
            string[] drpItem = (arrayLength > 0) ? new string[arrayLength] : null;
            for (int k = 0; k < arrayLength; k++)
            {
                msg.Read(out drpItem[k]);
            }
            BrickManManager.Instance.OnEnter(seq, name, equip, status, xp, clanSeq, clanName, clanMark, rank, weaponChg, drpItem);
            BrickManManager.Instance.GetDesc(seq).Slot = (sbyte)slot;
            if (seq != MyInfoManager.Instance.Seq)
            {
                CSteamID steamID = CSteamID.Nil;
                steamID.m_SteamID = steamID64;
                P2PExtension.instance.AddSteam(seq, steamID, playerflag);
                //P2PManager.Instance.Add(seq, ip, port, remoteIP, remotePort, playerflag);

                if (RoomManager.Instance.CurrentRoom >= 0)
                {
                    GameObject gameObject = GameObject.Find("Main");
                    if (null != gameObject)
                    {
                        gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, seq, name, StringMgr.Instance.Get("ENTERED")));
                    }
                }
            }

            else
                MyInfoManager.Instance.Slot = (sbyte)slot;
        }

        private void HandleReceiveSlotDataSteam(MsgBody msg)
        {
            msg.Read(out int count);
            for (int i = 0; i < count; i++)
            {
                msg.Read(out int slot);
                msg.Read(out int seq);
                msg.Read(out string name);
                msg.Read(out ulong steamID64);
                msg.Read(out int equipCount);
                string[] equipment = new string[equipCount];
                for (int j = 0; j < equipCount; j++)
                {
                    msg.Read(out equipment[j]);
                }
                msg.Read(out int status);
                msg.Read(out int xp);
                msg.Read(out int clanSeq);
                msg.Read(out string clanName);
                msg.Read(out int clanMark);
                msg.Read(out int rank);
                msg.Read(out byte playerflag);
                msg.Read(out equipCount);
                string[] wpnChg = (equipCount > 0) ? new string[equipCount] : null;
                for (int j = 0; j < equipCount; j++)
                    msg.Read(out wpnChg[j]);
                msg.Read(out equipCount);
                string[] drpItem = (equipCount > 0) ? new string[equipCount] : null;
                for (int k = 0; k < equipCount; k++)
                    msg.Read(out drpItem[k]);

                if (seq != MyInfoManager.Instance.Seq)
                {
                    CSteamID steamID = CSteamID.Nil;
                    steamID.m_SteamID = steamID64;
                    P2PExtension.instance.AddSteam(seq, steamID, playerflag);
                    //P2PManager.Instance.Add(seq, ip, port, remoteIp, remotePort, playerflag);
                    BrickManManager.Instance.OnEnter(seq, name, equipment, status, xp, clanSeq, clanName, clanMark, rank, equipment, equipment);
                    BrickManDesc desc = BrickManManager.Instance.GetDesc(seq);
                    desc.Slot = (sbyte)slot;
                }

                else
                    MyInfoManager.Instance.Slot = (sbyte)slot;
            }
        }

        private void HandleReceiveSlotData(MsgBody msg)
        {
            msg.Read(out int count);
            for (int i = 0; i < count; i++)
            {
                msg.Read(out int slot);
                msg.Read(out int seq);
                msg.Read(out string name);
                msg.Read(out string ip);
                msg.Read(out int port);
                msg.Read(out string remoteIp);
                msg.Read(out int remotePort);
                msg.Read(out int equipCount);
                string[] equipment = new string[equipCount];
                for (int j = 0; j < equipCount; j++)
                {
                    msg.Read(out equipment[j]);
                }
                msg.Read(out int status);
                msg.Read(out int xp);
                msg.Read(out int clanSeq);
                msg.Read(out string clanName);
                msg.Read(out int clanMark);
                msg.Read(out int rank);
                msg.Read(out byte playerflag);
                msg.Read(out equipCount);
                string[] wpnChg = (equipCount > 0) ? new string[equipCount] : null;
                for (int j = 0; j < equipCount; j++)
                    msg.Read(out wpnChg[j]);
                msg.Read(out equipCount);
                string[] drpItem = (equipCount > 0) ? new string[equipCount] : null;
                for (int k = 0; k < equipCount; k++)
                    msg.Read(out drpItem[k]);

                if (seq != MyInfoManager.Instance.Seq)
                {
                    P2PManager.Instance.Add(seq, ip, port, remoteIp, remotePort, playerflag);
                    BrickManManager.Instance.OnEnter(seq, name, equipment, status, xp, clanSeq, clanName, clanMark, rank, equipment, equipment);
                    BrickManDesc desc = BrickManManager.Instance.GetDesc(seq);
                    desc.Slot = (sbyte)slot;
                }

                else
                    MyInfoManager.Instance.Slot = (sbyte)slot;
            }
        }

        private void HandlePostLoadInit(MsgBody msg)
        {
            BrickManager.Instance.userMap.PostLoadInit();
        }

        private void HandleRequestInventory(MsgBody msg)
        {
            msg.Read(out int seq);
            inventory = new Inventory(seq, true);
            SendInventoryData();
        }

        private void HandleCustomMessage(MsgBody msg)
        {
            msg.Read(out string message);
            MessageBoxMgr.Instance.AddMessage(message);
        }

        public void SendInventoryData()
        {
            MsgBody body = new MsgBody();

            body.Write(inventory.equipment.Count);

            // Write each item's slot (category) and code
            foreach (var item in inventory.equipment)
            {
                body.Write(item.Code);                     // Write the item code
                body.Write((int)item.Usage);         // Write the item Usage
                body.Write(item.toolSlot);
            }

            // Send the data
            Say(ExtensionOpcodes.opInventoryAck, body);
        }

        public void SendDisconnect()
        {
            MsgBody body = new MsgBody();

            Say(ExtensionOpcodes.opDisconnectReq, body);
        }

        public static Version GetGithubVersionOrUnknown()
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
            catch (Exception ex)
            {
                Debug.LogWarning("Failed to read launcher version (launcher_data.dat): " + ex.Message);
                return Version.UNKNOWN;
            }
        }

    }
}
