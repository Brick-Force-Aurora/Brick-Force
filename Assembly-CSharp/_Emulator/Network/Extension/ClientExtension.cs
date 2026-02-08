using System;
using System.IO;
using System.Text.RegularExpressions;
using Steamworks;
using UnityEngine;
using static Room;
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
            //Debug.Log(msg._id);
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

                case ExtensionOpcodes.opVersionCheckAck:
                    HandleVersionCheck(msg._msg);
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
            CSNetManager.Instance.Sock._heartbeat = true;
            GameObject gameObject = GameObject.Find("Main");
            if (null != gameObject)
            {
                gameObject.BroadcastMessage("OnSeed");
            }
            Core.SetBalancedItemProperties();
            SendVersionCheck();
        }

        private void SendVersionCheck()
        {
            string version = GetGithubVersionOrUnknown();

            MsgBody body = new MsgBody();
            body.Write(version);

            Say(ExtensionOpcodes.opVersionCheckReq, body);
        }

        private void HandleDisconnected(MsgBody msg)
        {
            clientConnected = false;
            if (!isSteam)
            {
                if (CSNetManager.Instance.Sock != null)
                    CSNetManager.Instance.Sock.Close();
                MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NETWORK_BROKEN"));
                BuildOption.Instance.Exit();
            }
            else
                SteamLobbyManager.instance.LeaveCurrentLobbyAndShutdown();
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

        private void HandleVersionCheck(MsgBody msg)
        {
            msg.Read(out string hostVer);
            msg.Read(out string clientVer);

            int result = CompareVersions(hostVer, clientVer);

            string relation =
                result > 0 ? "a newer" :
                result < 0 ? "an older" :
                "the same";

            MessageBoxMgr.Instance.AddMessage(
                $"Version mismatch detected, the host is using {relation} version ({hostVer}). "
            );
        }

        private int CompareVersions(string a, string b)
        {
            int aMaj, aMin, aPat, aRev;
            int bMaj, bMin, bPat, bRev;

            ParseVersion(a, out aMaj, out aMin, out aPat, out aRev);
            ParseVersion(b, out bMaj, out bMin, out bPat, out bRev);

            if (aMaj != bMaj) return aMaj.CompareTo(bMaj);
            if (aMin != bMin) return aMin.CompareTo(bMin);
            if (aPat != bPat) return aPat.CompareTo(bPat);

            // No -R is older than any -R
            if (aRev == -1 && bRev != -1) return -1;
            if (aRev != -1 && bRev == -1) return 1;

            return aRev.CompareTo(bRev);
        }

        private void ParseVersion(
            string version,
            out int major,
            out int minor,
            out int patch,
            out int revision)
        {
            major = minor = patch = 0;
            revision = -1; // -1 = no -R suffix

            string[] dashSplit = version.Split('-');
            string[] nums = dashSplit[0].Split('.');

            if (nums.Length > 0) int.TryParse(nums[0], out major);
            if (nums.Length > 1) int.TryParse(nums[1], out minor);
            if (nums.Length > 2) int.TryParse(nums[2], out patch);

            if (dashSplit.Length > 1 && dashSplit[1].StartsWith("R"))
            {
                int.TryParse(dashSplit[1].Substring(1), out revision);
            }
        }


        public static string GetGithubVersionOrUnknown()
        {
            try
            {
                string launcherPath = Path.GetFullPath(
                    Path.Combine(Application.dataPath, "../launcher.txt")
                );

                if (!File.Exists(launcherPath))
                {
                    Debug.LogWarning("launcher.txt not found at: " + launcherPath);
                    return "unknown";
                }

                string text = File.ReadAllText(launcherPath);

                // Beispiel: github-version=2.1.2
                var m = Regex.Match(text, @"(?im)^\s*github-version\s*=\s*([0-9A-Za-z\.\-_]+)\s*$");
                if (m.Success)
                {
                    var ver = m.Groups[1].Value.Trim();
                    Debug.Log("GameVersion: " + ver);
                    return ver;
                }

                Debug.LogWarning("github-version key not found in launcher.txt");
                return "unknown";
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Failed to read launcher version: " + ex.Message);
                return "unknown";
            }
        }
    }
}
