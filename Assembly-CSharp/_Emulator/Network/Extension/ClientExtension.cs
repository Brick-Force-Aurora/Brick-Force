using System;
using Steamworks;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace _Emulator
{
    class ClientExtension
    {
        public static ClientExtension instance = new ClientExtension();
        public string hostIP = "";
        public Inventory inventory;
        public bool clientConnected = false;
        public MsgBody lastKillLogMsg;
        public int lastKillLogId = -1;
        public float killLogRealiableTime = 0f;
        public bool isSteam = false;

        public void LoadServer()
        {
            CSNetManager.Instance.BfServer = hostIP;
            CSNetManager.Instance.BfPort = 5000;
            GameObject gameObject = GameObject.Find("Main");
            if (null != gameObject)
            {
                gameObject.BroadcastMessage("OnRoundRobin");
            }
            ShopEmulator shop = new ShopEmulator();
            //shop.LoadAndSave();
            shop.ParseData();
        }

        public void LoadServerSteam()
        {
            if (SteamManager.Initialized)
            {
                isSteam = true;
                GameObject gameObject = GameObject.Find("Main");
                if (null != gameObject)
                {
                    var login = gameObject.GetComponentInChildren<Login>();
                    if (login != null)
                    {
                        login.loginStep = Login.LOGIN_STEP.WAITING_SEED;
                        login.id = SteamFriends.GetPersonaName();
                        SteamNetworkingManager.instance.SendInitMessageToHost();
                    }
                }

                ShopEmulator shop = new ShopEmulator();
                //shop.LoadAndSave();
                shop.ParseData();
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

        public void Say(ushort id, MsgBody msgBody)
        {
            CSNetManager.Instance.Sock.Say(id, msgBody);
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
            bool result = true;
            switch (msg._id)
            {
                case ExtensionOpcodes.opConnectedAck:
                    HandleConnected(msg._msg);
                    break;

                case ExtensionOpcodes.opSlotDataAck:
                    HandleReceiveSlotData(msg._msg);
                    break;

                case ExtensionOpcodes.opPostLoadInitAck:
                    HandlePostLoadInit(msg._msg);
                    break;

                case ExtensionOpcodes.opInventoryReq:
                    HandleRequestInventory(msg._msg);
                    break;

                case ExtensionOpcodes.opCustomMessageAck:
                    HandleCustomMessage(msg._msg);
                    break;

                case ExtensionOpcodes.opDisconnectAck:
                    HandleDisconnected(msg._msg);
                    break;

                case ExtensionOpcodes.opRendezvousInfoSteamAck:
                    HandleRendezvousInfoSteam(msg._msg);
                    break;

                case ExtensionOpcodes.opEnterSteamAck:
                    HandleEnterSteam(msg._msg);
                    break;

                case ExtensionOpcodes.opSlotDataSteamAck:
                    HandleReceiveSlotDataSteam(msg._msg);
                    break;

                default:
                    result = false;
                    break;
            }
            return result;
        }

        private void HandleConnected(MsgBody msg)
        {
            clientConnected = true;
            MainGUI.instance.setupHidden = true;
            GameObject gameObject = GameObject.Find("Main");
            if (null != gameObject)
            {
                gameObject.BroadcastMessage("OnSeed");
            }
        }

        private void HandleDisconnected(MsgBody msg)
        {
            if (CSNetManager.Instance.Sock != null)
                CSNetManager.Instance.Sock.Close();
            MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NETWORK_BROKEN"));
            BuildOption.Instance.Exit();
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
                body.Write(item.Usage.ToString());         // Write the item Usage
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

        public void SendBeginChunkedBuffer(ushort opcode, byte[] buffer)
        {
            const int maxBufferSize = 1000000000;
            if (buffer.Length > maxBufferSize)
            {
                Debug.LogWarning("ClientExtension.SendBeginChunkedBuffer: Buffer was " + buffer.Length + " bytes");
                return;
            }

            uint crc = CRC32.computeUnsigned(buffer);

            MsgBody body = new MsgBody();
            body.Write(opcode);
            body.Write(buffer.Length);
            body.Write(crc);

            Debug.LogWarning("Begin");
            Say(ExtensionOpcodes.opBeginChunkedBufferReq, body);
            SendChunkedBuffer(opcode, buffer);
            SendEndChunkedBuffer(opcode, crc);
        }

        public void SendChunkedBuffer(ushort opcode, byte[] buffer)
        {
            int chunkSize = 4096;
            int chunkCount = Mathf.CeilToInt((float)buffer.Length / (float)chunkSize);
            int processedCount = 0;

            Debug.Log(chunkSize + " " + buffer.Length + " " + chunkCount);
            for (int chunk = 0; chunk < chunkCount; chunk++)
            {
                int remaining = buffer.Length - processedCount;
                if (remaining < chunkSize)
                    chunkSize = remaining;

                MsgBody body = new MsgBody();

                byte[] next = new byte[chunkSize];
                Array.Copy(buffer, processedCount, next, 0, chunkSize);
                body.Write(opcode);
                body.Write(chunk);
                body.Write(next);
                processedCount += chunkSize;

                Debug.LogWarning("Send " + chunk + " " + chunkSize + " " + processedCount);
                Say(ExtensionOpcodes.opChunkedBufferReq, body);
            }
        }

        public void SendEndChunkedBuffer(ushort opcode, uint crc)
        {
            MsgBody body = new MsgBody();
            body.Write(opcode);

            Debug.LogWarning("End");
            Say(ExtensionOpcodes.opEndChunkedBufferReq, body);
        }
    }
}
