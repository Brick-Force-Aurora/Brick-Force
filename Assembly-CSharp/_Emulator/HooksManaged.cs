using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.IO;
using System.Net.Sockets;
using System.Net;
using static Brick;
using Steamworks;
using System.Linq;

namespace _Emulator
{
    class HooksManaged
    {
        static MethodInfo oP2PManagerHandshakeInfo = typeof(P2PManager).GetMethod("Handshake", BindingFlags.NonPublic | BindingFlags.Instance);
        static MethodInfo hP2PManagerHandshakeInfo = typeof(HooksManaged).GetMethod("hP2PManagerHandshake", BindingFlags.NonPublic | BindingFlags.Instance);
        static ManagedHook P2PManagerHandshakeHook;

		static MethodInfo oSockTcpGetSendKeyInfo = typeof(SockTcp).GetMethod("GetSendKey", BindingFlags.Public | BindingFlags.Instance);
		static MethodInfo hSockTcpGetSendKeyInfo = typeof(HooksManaged).GetMethod("hSockTcpGetSendKey", BindingFlags.Public | BindingFlags.Instance);
		static ManagedHook SockTcpGetSendKeyHook;

		static MethodInfo oSockTcpEnterAckInfo = typeof(SockTcp).GetMethod("HandleCS_ENTER_ACK", BindingFlags.NonPublic | BindingFlags.Instance);
		static MethodInfo hSockTcpEnterAckInfo = typeof(HooksManaged).GetMethod("hSockTcpEnterAck", BindingFlags.NonPublic | BindingFlags.Instance);
		static ManagedHook SockTcpEnterAckHook;

		static MethodInfo oSockTcpRendezvousInfoAckInfo = typeof(SockTcp).GetMethod("HandleCS_RENDEZVOUS_INFO_ACK", BindingFlags.NonPublic | BindingFlags.Instance);
		static MethodInfo hSockTcpRendezvousInfoAckInfo = typeof(HooksManaged).GetMethod("hSockTcpRendezvousInfoAck", BindingFlags.NonPublic | BindingFlags.Instance);
		static ManagedHook SockTcpRendezvousInfoAckHook;

		static MethodInfo oPimpManagerLoadInfo = typeof(PimpManager).GetMethod("Load", BindingFlags.Public | BindingFlags.Instance);
		static MethodInfo hPimpManagerLoadInfo = typeof(HooksManaged).GetMethod("hPimpManagerLoad", BindingFlags.Public | BindingFlags.Instance);
		static ManagedHook PimpManagerLoadHook;

		static MethodInfo oP2PManagerReliableSendInfo = typeof(P2PManager).GetMethod("ReliableSend", BindingFlags.NonPublic | BindingFlags.Instance);
		static MethodInfo hP2PManagerReliableSendInfo = typeof(HooksManaged).GetMethod("hP2PManagerReliableSend", BindingFlags.NonPublic | BindingFlags.Instance);
		static ManagedHook P2PManagerReliableSendHook;

		static MethodInfo oSockTcpKilllogReqInfo = typeof(SockTcp).GetMethod("SendCS_KILL_LOG_REQ", BindingFlags.Public | BindingFlags.Instance);
		static MethodInfo hSockTcpKilllogReqInfo = typeof(HooksManaged).GetMethod("hSockTcpKilllogReq", BindingFlags.Public | BindingFlags.Instance);
		static ManagedHook SockTcpKilllogReqHook;

		static MethodInfo oSockTcpKilllogAckInfo = typeof(SockTcp).GetMethod("HandleCS_KILL_LOG_ACK", BindingFlags.NonPublic | BindingFlags.Instance);
		static MethodInfo hSockTcpKilllogAckInfo = typeof(HooksManaged).GetMethod("hSockTcpKilllogAck", BindingFlags.NonPublic | BindingFlags.Instance);
		static ManagedHook SockTcpKilllogAckHook;

		static MethodInfo oSockTcpHandleItemListAckInfo = typeof(SockTcp).GetMethod("HandleCS_ITEM_LIST_ACK", BindingFlags.NonPublic | BindingFlags.Instance);
		static MethodInfo hSockTcpHandleItemListAckInfo = typeof(HooksManaged).GetMethod("hSockTcpHandleItemListAck", BindingFlags.NonPublic | BindingFlags.Instance);
		static ManagedHook SockTcpHandleItemListAckHook;

		static MethodInfo oSockTcpHandleShooterToolAckInfo = typeof(SockTcp).GetMethod("HandleCS_SHOOTER_TOOL_ACK", BindingFlags.NonPublic | BindingFlags.Instance);
		static MethodInfo hSockTcpHandleShooterToolAckInfo = typeof(HooksManaged).GetMethod("hSockTcpHandleShooterToolAck", BindingFlags.NonPublic | BindingFlags.Instance);
		static ManagedHook SockTcpHandleShooterToolAckHook;

		static MethodInfo oSockTcpHandleShooterToolListAckInfo = typeof(SockTcp).GetMethod("HandleCS_SHOOTER_TOOL_LIST_ACK", BindingFlags.NonPublic | BindingFlags.Instance);
		static MethodInfo hSockTcpHandleShooterToolListAckInfo = typeof(HooksManaged).GetMethod("hSockTcpHandleShooterToolListAck", BindingFlags.NonPublic | BindingFlags.Instance);
		static ManagedHook SockTcpHandleShooterToolListAckHook;

		static MethodInfo oSockTcpHandleWeaponSlotAckInfo = typeof(SockTcp).GetMethod("HandleCS_WEAPON_SLOT_ACK", BindingFlags.NonPublic | BindingFlags.Instance);
		static MethodInfo hSockTcpHandleWeaponSlotAckInfo = typeof(HooksManaged).GetMethod("hSockTcpHandleWeaponSlotAck", BindingFlags.NonPublic | BindingFlags.Instance);
		static ManagedHook SockTcpHandleWeaponSlotAckHook;

		static MethodInfo oSockTcpHandleWeaponSlotListAckInfo = typeof(SockTcp).GetMethod("HandleCS_WEAPON_SLOT_LIST_ACK", BindingFlags.NonPublic | BindingFlags.Instance);
		static MethodInfo hSockTcpHandleWeaponSlotListAckInfo = typeof(HooksManaged).GetMethod("hSockTcpHandleWeaponSlotListAck", BindingFlags.NonPublic | BindingFlags.Instance);
		static ManagedHook SockTcpHandleWeaponSlotListAckHook;

		static MethodInfo oSockTcpRegisterReqInfo = typeof(SockTcp).GetMethod("SendCS_REGISTER_REQ", BindingFlags.Public | BindingFlags.Instance);
		static MethodInfo hSockTcpRegisterReqInfo = typeof(HooksManaged).GetMethod("hSockTcpRegisterReq", BindingFlags.Public | BindingFlags.Instance);
		static ManagedHook SockTcpRegisterReqHook;

        static MethodInfo oSockTcpSaveMapReqInfo = typeof(SockTcp).GetMethod("SendCS_SAVE_REQ", BindingFlags.Public | BindingFlags.Instance);
        static MethodInfo hSockTcpSaveMapReqInfo = typeof(HooksManaged).GetMethod("hSockTcpSaveMapReq", BindingFlags.Public | BindingFlags.Instance);
        static ManagedHook SockTcpSaveMapReqHook;

        static MethodInfo oSockTcpSayInfo = typeof(SockTcp).GetMethod("Say", BindingFlags.Public | BindingFlags.Instance);
        static MethodInfo hSockTcpSayInfo = typeof(HooksManaged).GetMethod("hSockTcpSay", BindingFlags.Public | BindingFlags.Instance);
        static ManagedHook SockTcpSayHook;

        static MethodInfo oSockTcpIsConnectedInfo = typeof(SockTcp).GetMethod("IsConnected", BindingFlags.Public | BindingFlags.Instance);
        static MethodInfo hSockTcpIsConnectedInfo = typeof(HooksManaged).GetMethod("hSockTcpIsConnected", BindingFlags.Public | BindingFlags.Instance);
        static ManagedHook SockTcpIsConnectedHook;

        static MethodInfo oMyInfoManagerSetItemUsageInfo = typeof(MyInfoManager).GetMethod("SetItemUsage", BindingFlags.Public | BindingFlags.Instance);
		static MethodInfo hMyInfoManagerSetItemUsageInfo = typeof(HooksManaged).GetMethod("hMyInfoManagerSetItemUsage", BindingFlags.Public | BindingFlags.Instance);
		static ManagedHook MyInfoManagerSetItemUsageHook;

		static MethodInfo oApplicationQuitInfo = typeof(Application).GetMethod("Quit", BindingFlags.Public | BindingFlags.Static);
		static MethodInfo hApplicationQuitInfo = typeof(HooksManaged).GetMethod("hApplicationQuit", BindingFlags.Public | BindingFlags.Static);
		static ManagedHook ApplicationQuitHook;

        static MethodInfo oBuildOptionExitInfo = typeof(BuildOption).GetMethod("Exit", BindingFlags.Public | BindingFlags.Instance);
        static MethodInfo hBuildOptionExitInfo = typeof(HooksManaged).GetMethod("hBuildOptionExit", BindingFlags.Public | BindingFlags.Instance);
        static ManagedHook BuildOptionExitHook;

        static MethodInfo oP2PManagerSendPEER_RELIABLE_ACKInfo = typeof(P2PManager).GetMethod("SendPEER_RELIABLE_ACK", BindingFlags.NonPublic| BindingFlags.Instance);
        static MethodInfo hP2PManagerSendPEER_RELIABLE_ACKInfo = typeof(HooksManaged).GetMethod("hP2PManagerSendPEER_RELIABLE_ACK", BindingFlags.NonPublic | BindingFlags.Instance);
        static ManagedHook P2PManagerSendPEER_RELIABLE_ACKHook;

        static MethodInfo oP2PManagerSendReliableInfo = typeof(P2PManager).GetMethod("SendReliable", BindingFlags.NonPublic | BindingFlags.Instance);
        static MethodInfo hP2PManagerSendReliableInfo = typeof(HooksManaged).GetMethod("hP2PManagerSendReliable", BindingFlags.NonPublic | BindingFlags.Instance);
        static ManagedHook P2PManagerSendReliableHook;

        static MethodInfo oP2PManagerSayInfo = typeof(P2PManager).GetMethod("Say", BindingFlags.Public | BindingFlags.Instance);
        static MethodInfo hP2PManagerSayInfo = typeof(HooksManaged).GetMethod("hP2PManagerSay", BindingFlags.Public | BindingFlags.Instance);
        static ManagedHook P2PManagerSayHook;

        static MethodInfo oP2PManagerWhisperInfo = typeof(P2PManager).GetMethod("Whisper", BindingFlags.Public | BindingFlags.Instance);
        static MethodInfo hP2PManagerWhisperInfo = typeof(HooksManaged).GetMethod("hP2PManagerWhisper", BindingFlags.Public | BindingFlags.Instance);
        static ManagedHook P2PManagerWhisperHook;

        static MethodInfo oP2PManagerSendPEER_LEAVEInfo = typeof(P2PManager).GetMethod("SendPEER_LEAVE", BindingFlags.Public | BindingFlags.Instance);
        static MethodInfo hP2PManagerSendPEER_LEAVEInfo = typeof(HooksManaged).GetMethod("hP2PManagerSendPEER_LEAVE", BindingFlags.Public | BindingFlags.Instance);
        static ManagedHook P2PManagerSendPEER_LEAVEHook;

        static MethodInfo oLoadBrickMainStartInfo = typeof(LoadOthersMain).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance);
        static MethodInfo hLoadBrickMainStartInfo = typeof(HooksManaged).GetMethod("hLoadBrickMainStart", BindingFlags.Public | BindingFlags.Instance);
        static ManagedHook LoadBrickMainStartHook;

		static MethodInfo oScreenSetResolutionInfo = typeof(Screen).GetMethods(BindingFlags.Public | BindingFlags.Static).ToList().FindLast(x => x.Name == "SetResolution");
        static MethodInfo hScreenSetResolutionInfo = typeof(HooksManaged).GetMethod("hScreenSetResolution", BindingFlags.Public | BindingFlags.Static);
        static ManagedHook ScreenSetResolutionHook;

        private void hP2PManagerHandshake()
        {
            if (MyInfoManager.Instance.Status == 3 || MyInfoManager.Instance.Status == 4)
			{
                bool flag = false;
				P2PManager.Instance.handshakeTime += Time.deltaTime;
				if (P2PManager.Instance.handshakeTime > 0.1f)
				{
					P2PManager.Instance.handshakeTime = 0f;
					flag = true;
				}
				foreach (KeyValuePair<int, Peer> peer in P2PManager.Instance.dic)
				{
					BrickManDesc desc = BrickManManager.Instance.GetDesc(peer.Value.Seq);
					if (desc != null && peer.Value.P2pStatus == Peer.P2P_STATUS.NONE && (desc.Status == 3 || desc.Status == 4))
					{
						if (flag)
						{
							if (P2PExtension.instance.isSteam)
							{
								P2PExtension.instance.SendPeerPrivateHandSteam(peer.Value.steamID);
                                P2PExtension.instance.SendPeerPublicHandSteam(peer.Value.steamID);
                            }
                            else
                            {
                                P2PManager.Instance.SendPEER_PRIVATE_HAND(peer.Value.LocalIp, peer.Value.LocalPort);
                                if (P2PManager.Instance.OutputDebug)
                                {
                                    Debug.Log("SendPEER_PRIVATE_HAND to " + peer.Value.Seq.ToString());
                                    Debug.Log("addr: " + peer.Value.LocalIp);
                                    Debug.Log("port: " + peer.Value.LocalPort.ToString());
                                }
                                P2PManager.Instance.SendPEER_PUBLIC_HAND(peer.Value.RemoteIp, peer.Value.RemotePort);
                                if (P2PManager.Instance.OutputDebug)
                                {
                                    Debug.Log("SendPEER_PUBLIC_HAND to " + peer.Value.Seq.ToString());
                                    Debug.Log("addr: " + peer.Value.RemoteIp);
                                    Debug.Log("port: " + peer.Value.RemotePort.ToString());
                                }
                            }
						}
						peer.Value.Update();
					}
				}
			}
		}

		public byte hSockTcpGetSendKey()
		{
			return byte.MaxValue;
		}

		private void hSockTcpEnterAck(MsgBody msg)
		{
			msg.Read(out int slot);
			msg.Read(out int seq);
			msg.Read(out string val2);
			msg.Read(out string val3);
			msg.Read(out int val4);
			msg.Read(out string val5);
			msg.Read(out int val6);
			msg.Read(out int val7);
			string[] array = new string[val7];
			for (int j = 0; j < val7; j++)
			{
				msg.Read(out array[j]);
			}
			msg.Read(out int val8);
			msg.Read(out int val9);
			msg.Read(out int val10);
			msg.Read(out string val11);
			msg.Read(out int val12);
			msg.Read(out int val13);
			msg.Read(out byte val14);
			msg.Read(out val7);
			string[] array2 = (val7 > 0) ? new string[val7] : null;
			for (int j = 0; j < val7; j++)
			{
				msg.Read(out array2[j]);
			}
			msg.Read(out val7);
			string[] array3 = (val7 > 0) ? new string[val7] : null;
			for (int k = 0; k < val7; k++)
			{
				msg.Read(out array3[k]);
			}
			BrickManManager.Instance.OnEnter(seq, val2, array, val8, val9, val10, val11, val12, val13, array2, array3);
			BrickManManager.Instance.GetDesc(seq).Slot = (sbyte)slot;
			if (seq != MyInfoManager.Instance.Seq)
			{
				P2PManager.Instance.Add(seq, val3, val4, val5, val6, val14);

				if (RoomManager.Instance.CurrentRoom >= 0)
				{
					GameObject gameObject = GameObject.Find("Main");
					if (null != gameObject)
					{
						gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, seq, val2, StringMgr.Instance.Get("ENTERED")));
					}
				}
			}

			else
				MyInfoManager.Instance.Slot = (sbyte)slot;
		}

		private void hSockTcpRendezvousInfoAck(MsgBody msg)
		{
			msg.Read(out int val1);
			msg.Read(out string val2);
			msg.Read(out int val3);
			P2PManager.Instance.Bootup(val2, val3);
			P2PManager.Instance.rendezvousPointed = true;
		}

		public void hPimpManagerLoad()
		{
			PimpManager.Instance.LoadFromLocalFileSystem();
			PimpManager.Instance.updateValue((int)UPGRADE_CAT.HANDGUN, (int)PIMP.PROP_RPM, 9, 400);
			for (PIMP pimp = PIMP.PROP_ATK_POW; pimp < PIMP.PROP_MAX; pimp++)
			{
				for (int lv = 0; lv < 10; lv++)
				{
					PimpManager.Instance.updateValue((int)UPGRADE_CAT.OTHER, (int)pimp, lv, 0f);
				}
			}
		}

		private void hP2PManagerReliableSend(uint to, byte id, P2PMsgBody mb)
		{
			P2PManager.Instance.Say(id, mb);
		}

		public void hSockTcpKilllogReq(sbyte killerType, int killer, sbyte victimType, int victim, int weaponBy, int slot, int category, int hitpart, Dictionary<int, int> damageLog)
		{
			MsgBody msgBody = new MsgBody();
			int id = UnityEngine.Random.Range(0, int.MaxValue);

			msgBody.Write(id);
			msgBody.Write(killerType);
			msgBody.Write(killer);
			msgBody.Write(victimType);
			msgBody.Write(victim);
			msgBody.Write(weaponBy);
			msgBody.Write(slot);
			msgBody.Write(category);
			msgBody.Write(hitpart);
			if (damageLog == null)
			{
				msgBody.Write(0);
			}
			else
			{
				msgBody.Write(damageLog.Count);
				foreach (KeyValuePair<int, int> item in damageLog)
				{
					msgBody.Write(item.Key);
					msgBody.Write(item.Value);
				}
			}

			ClientExtension.instance.lastKillLogMsg = msgBody;
			ClientExtension.instance.lastKillLogId = id;

			CSNetManager.Instance.Sock.Say(44, msgBody);
		}

		private void hSockTcpKilllogAck(MsgBody msg)
		{
			msg.Read(out int id);
			if (id == ClientExtension.instance.lastKillLogId)
				ClientExtension.instance.lastKillLogId = -1;

			SockTcpKilllogAckHook.CallOriginal(CSNetManager.Instance.Sock, new object[] { msg });
		}

		public void hMyInfoManagerSetItemUsage(long seq, string code, Item.USAGE usage)
		{
			Item item = ClientExtension.instance.inventory.equipment.Find(x => x.Seq == seq);
			if (item != null)
				item.Usage = usage;

			MyInfoManagerSetItemUsageHook.CallOriginal(MyInfoManager.Instance, new object[] { seq, code, usage });
		}

		private void hSockTcpHandleItemListAck(MsgBody msg)
		{
			ClientExtension.instance.inventory.Apply();
		}

		private void hSockTcpHandleShooterToolAck(MsgBody msg)
		{
			msg.Read(out sbyte slot);
			msg.Read(out long seq);

			ClientExtension.instance.inventory.AddToolSlot(seq, slot);

			if (MyInfoManager.Instance.ShooterTools.Length > slot)
			{
				if (seq < 0)
				{
					MyInfoManager.Instance.ShooterTools[slot] = -1L;
				}
				else
				{
					Item itemBySequence = MyInfoManager.Instance.GetItemBySequence(seq);
					itemBySequence.toolSlot = slot;
					if (itemBySequence != null && itemBySequence.Template != null && itemBySequence.Template.type == TItem.TYPE.SPECIAL)
					{
						MyInfoManager.Instance.ShooterTools[slot] = seq;
					}
				}
			}
		}

		private void hSockTcpHandleShooterToolListAck(MsgBody msg)
		{
			msg.Read(out int count);
			for (int i = 0; i < count; i++)
			{
				msg.Read(out sbyte slot);
				msg.Read(out long seq);

				ClientExtension.instance.inventory.AddToolSlot(seq, slot);

				if (MyInfoManager.Instance.ShooterTools.Length > slot)
				{
					if (seq < 0)
					{
						MyInfoManager.Instance.ShooterTools[slot] = -1L;
					}
					else
					{
						Item itemBySequence = MyInfoManager.Instance.GetItemBySequence(seq);
						if (itemBySequence != null && itemBySequence.Template != null && itemBySequence.Template.type == TItem.TYPE.SPECIAL)
						{
							MyInfoManager.Instance.ShooterTools[slot] = seq;
						}
					}
				}
			}
		}

		private void hSockTcpHandleWeaponSlotAck(MsgBody msg)
		{
			msg.Read(out int slot);
			msg.Read(out long seq);
			if (MyInfoManager.Instance.WeaponSlots.Length > slot)
			{
				ClientExtension.instance.inventory.AddWeaponSlot(seq, (sbyte)slot);

				if (seq < 0)
				{
					MyInfoManager.Instance.WeaponSlots[slot] = -1L;
				}
				else
				{
					Item itemBySequence = MyInfoManager.Instance.GetItemBySequence(seq);
					if (itemBySequence != null && itemBySequence.Template != null && itemBySequence.Template.type == TItem.TYPE.WEAPON)
					{
						MyInfoManager.Instance.WeaponSlots[slot] = seq;
					}
				}
			}
		}

		private void hSockTcpHandleWeaponSlotListAck(MsgBody msg)
		{
			msg.Read(out int count);
			for (int i = 0; i < count; i++)
			{
				msg.Read(out int slot);
				msg.Read(out long seq);

				ClientExtension.instance.inventory.AddWeaponSlot(seq, (sbyte)slot);

				if (MyInfoManager.Instance.WeaponSlots.Length > slot)
				{
					if (seq < 0)
					{
						MyInfoManager.Instance.WeaponSlots[slot] = -1L;
					}
					else
					{
						Item itemBySequence = MyInfoManager.Instance.GetItemBySequence(seq);
						if (itemBySequence != null && itemBySequence.Template != null && itemBySequence.Template.type == TItem.TYPE.WEAPON)
						{
							MyInfoManager.Instance.WeaponSlots[slot] = seq;
						}
					}
				}
			}
		}

		public static void hApplicationQuit()
		{
			Debug.Log("Quit");
			if (ClientExtension.instance.isSteam)
			{
                SteamLobbyManager.instance.LeaveCurrentLobby();
            }

			if (ServerEmulator.instance.serverCreated)
			{
				ServerEmulator.instance.ShutdownInit();
				ServerEmulator.instance.ShutdownFinally();
			}
			else
			{
				CSNetManager.Instance.Sock.Close();
				P2PManager.Instance.Shutdown();
			}

            var hProcess = Import.GetCurrentProcess();
            Import.GetExitCodeProcess(hProcess, out uint exitCode);
            Import.TerminateProcess(hProcess, exitCode);

            HooksNative.Shutdown();
			//ApplicationQuitHook.CallOriginal(null, null);
		}

        public void hBuildOptionExit()
        {
            if (ClientExtension.instance.isSteam)
            {
                if (ServerEmulator.instance.serverCreated)
                {
                    try
                    {
                        ServerEmulator.instance.ShutdownInit();
                        ServerEmulator.instance.ShutdownFinally();
                    }
                    catch { }
                }

                else
                {
                    try
                    {
                        ClientExtension.instance.SendDisconnect();
                    }
                    catch { }
                }

                SteamNetworkingManager.instance.EndReceive();
                SteamLobbyManager.instance.LeaveCurrentLobby();
            }

            BuildOptionExitHook.CallOriginal(BuildOption.Instance, null);
        }

        public void hSockTcpRegisterReq(int slot, ushort modeMask, int regHow, int point, int downloadFee, byte[] thumbnail, string msgEval)
		{
			ClientExtension.instance.SendBeginChunkedBuffer(ExtensionOpcodes.opChunkedBufferThumbnailReq, thumbnail);

			MsgBody msgBody = new MsgBody();
			msgBody.Write(slot);
			msgBody.Write(modeMask);
			msgBody.Write(regHow);
			msgBody.Write(point);
			msgBody.Write(downloadFee);
			msgBody.Write(msgEval);
			CSNetManager.Instance.Sock.Say(51, msgBody);
		}

        public void hSockTcpSaveMapReq(int slot, byte[] thumbnail)
        {
            ClientExtension.instance.SendBeginChunkedBuffer(ExtensionOpcodes.opChunkedBufferThumbnailReq, thumbnail);

            MsgBody msgBody = new MsgBody();
            msgBody.Write(slot);
            CSNetManager.Instance.Sock.Say(39, msgBody);
        }

		public void hSockTcpSay(ushort id, MsgBody msgBody)
		{
            Msg4Send msg4Send = new Msg4Send(id, uint.MaxValue, uint.MaxValue, msgBody, CSNetManager.Instance.Sock.GetSendKey());

			if (ClientExtension.instance.isSteam)
			{
				SteamNetworkingManager.instance.SendMessageToHost(msg4Send);
			}

            else if (CSNetManager.Instance.Sock._writeQueue != null)
            {
                lock (this)
                {
                    if (CSNetManager.Instance.Sock._writeQueue.Count > 0)
                    {
                        CSNetManager.Instance.Sock._writeQueue.Enqueue(msg4Send);
                    }
                    else
                    {
                        CSNetManager.Instance.Sock._writeQueue.Enqueue(msg4Send);
                        try
                        {
							if (CSNetManager.Instance.Sock._sock != null)
                                CSNetManager.Instance.Sock._sock.BeginSend(msg4Send.Buffer, 0, msg4Send.Buffer.Length, SocketFlags.None, CSNetManager.Instance.Sock.SendCallback, null);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError("Error, " + ex.Message.ToString());
                            CSNetManager.Instance.Sock.Close();
                        }
                    }
                }
            }
        }

        public bool hSockTcpIsConnected()
        {
            if (ClientExtension.instance.isSteam)
                return true;

            if (CSNetManager.Instance.Sock._sock == null)
            {
                return false;
            }

            return CSNetManager.Instance.Sock._sock.Connected;
        }

        private void hP2PManagerSendPEER_RELIABLE_ACK(uint reliable)
        {
			if (P2PExtension.instance.isSteam)
			{
				P2PMsgBody p2PMsgBody = new P2PMsgBody();
				p2PMsgBody.Write(reliable);
                P2PManager.Instance.Say(27, p2PMsgBody);
			}

			else
			{
				P2PMsgBody p2PMsgBody = new P2PMsgBody();
				p2PMsgBody.Write(reliable);
				P2PMsg4Send p2PMsg4Send = new P2PMsg4Send(27, ushort.MaxValue, P2PManager.Seq2Slot((uint)MyInfoManager.Instance.Seq), byte.MaxValue, p2PMsgBody, byte.MaxValue);

				IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(P2PManager.Instance.rendezvousIp), P2PManager.Instance.rendezvousPort);
				if (iPEndPoint != null && p2PMsg4Send != null)
				{
					P2PManager.Instance.sock.SendTo(p2PMsg4Send.Buffer, p2PMsg4Send.Buffer.Length, SocketFlags.None, iPEndPoint);
				}
			}
        }

        private void hP2PManagerSendReliable()
        {
            if (P2PExtension.instance.isSteam)
            {
                //Debug.LogError("P2PManager.SendReliable during Steam P2P");
                return;
            }

            if (P2PManager.Instance.sock != null && P2PManager.Instance.queueReliable != null && P2PManager.Instance.queueReliable.Count > 0)
            {
                P2PMsg4Send p2PMsg4Send = P2PManager.Instance.queueReliable.Peek();
                IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(P2PManager.Instance.rendezvousIp), P2PManager.Instance.rendezvousPort);
                if (iPEndPoint != null && p2PMsg4Send != null)
                {
                    P2PManager.Instance.sock.SendTo(p2PMsg4Send.Buffer, p2PMsg4Send.Buffer.Length, SocketFlags.None, iPEndPoint);
                }
            }
        }

        public void hP2PManagerSay(byte id, P2PMsgBody mb)
        {
            try
            {
				if (P2PExtension.instance.isSteam)
				{
                    foreach (KeyValuePair<int, Peer> item in P2PManager.Instance.dic)
					{
                        if (item.Value.P2pStatus != 0)
						{
                            var p2PMsg4Send = new P2PMsg4Send(id, ushort.MaxValue, P2PManager.Seq2Slot((uint)MyInfoManager.Instance.Seq), P2PManager.Seq2Slot((uint)item.Key), mb, byte.MaxValue);
							SteamNetworkingManager.instance.SendMessageToPeer(item.Value.steamID, p2PMsg4Send);
                        }
					}
				}

				else
				{
					bool flag = false;
					foreach (KeyValuePair<int, Peer> item in P2PManager.Instance.dic)
					{
						if (item.Value.P2pStatus != 0)
						{
							P2PMsg4Send p2PMsg4Send = null;
							IPEndPoint iPEndPoint = null;
							if (item.Value.P2pStatus == Peer.P2P_STATUS.PRIVATE)
							{
								p2PMsg4Send = new P2PMsg4Send(id, ushort.MaxValue, P2PManager.Seq2Slot((uint)MyInfoManager.Instance.Seq), P2PManager.Seq2Slot((uint)item.Key), mb, byte.MaxValue);
								iPEndPoint = new IPEndPoint(IPAddress.Parse(item.Value.LocalIp), item.Value.LocalPort);
							}
							else if (item.Value.P2pStatus == Peer.P2P_STATUS.PUBLIC)
							{
								p2PMsg4Send = new P2PMsg4Send(id, ushort.MaxValue, P2PManager.Seq2Slot((uint)MyInfoManager.Instance.Seq), P2PManager.Seq2Slot((uint)item.Key), mb, byte.MaxValue);
								iPEndPoint = new IPEndPoint(IPAddress.Parse(item.Value.RemoteIp), item.Value.RemotePort);
							}
							else if (!flag)
							{
								flag = true;
								p2PMsg4Send = new P2PMsg4Send(id, ushort.MaxValue, P2PManager.Seq2Slot((uint)MyInfoManager.Instance.Seq), byte.MaxValue, mb, byte.MaxValue);
								iPEndPoint = new IPEndPoint(IPAddress.Parse(P2PManager.Instance.rendezvousIp), P2PManager.Instance.rendezvousPort);
							}
							if (iPEndPoint != null && p2PMsg4Send != null)
							{
								P2PManager.Instance.sock.SendTo(p2PMsg4Send.Buffer, p2PMsg4Send.Buffer.Length, SocketFlags.None, iPEndPoint);
							}
						}
					}
                }
            }
            catch (SocketException ex)
            {
                Debug.LogError("Error, " + ex.Message.ToString() + " : P2PManager.Say");
            }
        }

        public void hP2PManagerWhisper(int to, byte id, P2PMsgBody mb)
        {
            if (P2PManager.Instance.dic.ContainsKey(to) && P2PManager.Instance.dic[to].P2pStatus != 0)
            {
                try
                {
                    P2PMsg4Send p2PMsg4Send = new P2PMsg4Send(id, ushort.MaxValue, P2PManager.Seq2Slot((uint)MyInfoManager.Instance.Seq), P2PManager.Seq2Slot((uint)to), mb, byte.MaxValue);
					if (P2PExtension.instance.isSteam)
					{
                        SteamNetworkingManager.instance.SendMessageToPeer(P2PManager.Instance.dic[to].steamID, p2PMsg4Send);
                    }

					else
					{
						IPEndPoint iPEndPoint = null;
						iPEndPoint = ((P2PManager.Instance.dic[to].P2pStatus == Peer.P2P_STATUS.PRIVATE) ? new IPEndPoint(IPAddress.Parse(P2PManager.Instance.dic[to].LocalIp), P2PManager.Instance.dic[to].LocalPort) : ((P2PManager.Instance.dic[to].P2pStatus != Peer.P2P_STATUS.PUBLIC) ? new IPEndPoint(IPAddress.Parse(P2PManager.Instance.rendezvousIp), P2PManager.Instance.rendezvousPort) : new IPEndPoint(IPAddress.Parse(P2PManager.Instance.dic[to].RemoteIp), P2PManager.Instance.dic[to].RemotePort)));
						P2PManager.Instance.sock.SendTo(p2PMsg4Send.Buffer, p2PMsg4Send.Buffer.Length, SocketFlags.None, iPEndPoint);
					}
                }
                catch (SocketException ex)
                {
                    Debug.LogError("Error, " + ex.Message.ToString() + " : P2PManager.Whisper");
                }
            }
        }

		public void hP2PManagerSendPEER_LEAVE()
        {
            if (P2PExtension.instance.isSteam)
            {
                P2PMsgBody p2PMsgBody = new P2PMsgBody();
                p2PMsgBody.Write(MyInfoManager.Instance.Seq);
				P2PManager.Instance.Say(67, p2PMsgBody);
            }

            else if (P2PManager.Instance.sock != null)
            {
                try
                {
                    P2PMsgBody p2PMsgBody = new P2PMsgBody();
                    p2PMsgBody.Write(MyInfoManager.Instance.Seq);
                    P2PMsg4Send p2PMsg4Send = new P2PMsg4Send(67, ushort.MaxValue, P2PManager.Seq2Slot((uint)MyInfoManager.Instance.Seq), byte.MaxValue, p2PMsgBody, byte.MaxValue);
                    IPEndPoint remote_end = new IPEndPoint(IPAddress.Parse(P2PManager.Instance.rendezvousIp), P2PManager.Instance.rendezvousPort);
                    P2PManager.Instance.sock.SendTo(p2PMsg4Send.Buffer, p2PMsg4Send.Buffer.Length, SocketFlags.None, remote_end);
                }
                catch (SocketException ex)
                {
                    Debug.LogError("Error, " + ex.Message.ToString() + " : P2PManager.RandezvousPing");
                }
            }
        }

        public static void hScreenSetResolution(int width, int height, bool fullscreen)
        {
			lock (ImGuiBackend.instance.imguiLock)
			{
				// Reset and briefly disable ImGui to prevent it from crashing when changing resolutions.
				ImGuiBackend.instance.Shutdown(false, false);
				ScreenSetResolutionHook.CallOriginal(null, new object[] { width, height, fullscreen });
			}
        }

        public void hLoadBrickMainStart()
		{
			HooksNative.Initialize();
        }

        public static void Initialize()
        {
			P2PManagerHandshakeHook = new ManagedHook(oP2PManagerHandshakeInfo, hP2PManagerHandshakeInfo);
			P2PManagerHandshakeHook.ApplyHook();
			SockTcpGetSendKeyHook = new ManagedHook(oSockTcpGetSendKeyInfo, hSockTcpGetSendKeyInfo);
			SockTcpGetSendKeyHook.ApplyHook();
			SockTcpEnterAckHook = new ManagedHook(oSockTcpEnterAckInfo, hSockTcpEnterAckInfo);
			SockTcpEnterAckHook.ApplyHook();
			SockTcpRendezvousInfoAckHook = new ManagedHook(oSockTcpRendezvousInfoAckInfo, hSockTcpRendezvousInfoAckInfo);
			SockTcpRendezvousInfoAckHook.ApplyHook();
			PimpManagerLoadHook = new ManagedHook(oPimpManagerLoadInfo, hPimpManagerLoadInfo);
			PimpManagerLoadHook.ApplyHook();
			P2PManagerReliableSendHook = new ManagedHook(oP2PManagerReliableSendInfo, hP2PManagerReliableSendInfo);
			P2PManagerReliableSendHook.ApplyHook();
			SockTcpKilllogReqHook = new ManagedHook(oSockTcpKilllogReqInfo, hSockTcpKilllogReqInfo);
			SockTcpKilllogReqHook.ApplyHook();
			SockTcpKilllogAckHook = new ManagedHook(oSockTcpKilllogAckInfo, hSockTcpKilllogAckInfo);
			SockTcpKilllogAckHook.ApplyHook();
			SockTcpHandleItemListAckHook = new ManagedHook(oSockTcpHandleItemListAckInfo, hSockTcpHandleItemListAckInfo);
			SockTcpHandleItemListAckHook.ApplyHook();
			MyInfoManagerSetItemUsageHook = new ManagedHook(oMyInfoManagerSetItemUsageInfo, hMyInfoManagerSetItemUsageInfo);
			MyInfoManagerSetItemUsageHook.ApplyHook();
			SockTcpHandleShooterToolAckHook = new ManagedHook(oSockTcpHandleShooterToolAckInfo, hSockTcpHandleShooterToolAckInfo);
			SockTcpHandleShooterToolAckHook.ApplyHook();
			SockTcpHandleShooterToolListAckHook = new ManagedHook(oSockTcpHandleShooterToolListAckInfo, hSockTcpHandleShooterToolListAckInfo);
			SockTcpHandleShooterToolListAckHook.ApplyHook();
			SockTcpHandleWeaponSlotAckHook = new ManagedHook(oSockTcpHandleWeaponSlotAckInfo, hSockTcpHandleWeaponSlotAckInfo);
			SockTcpHandleWeaponSlotAckHook.ApplyHook();
			SockTcpHandleWeaponSlotListAckHook = new ManagedHook(oSockTcpHandleWeaponSlotListAckInfo, hSockTcpHandleWeaponSlotListAckInfo);
			SockTcpHandleWeaponSlotListAckHook.ApplyHook();
			SockTcpRegisterReqHook = new ManagedHook(oSockTcpRegisterReqInfo, hSockTcpRegisterReqInfo);
			SockTcpRegisterReqHook.ApplyHook();
            SockTcpSaveMapReqHook = new ManagedHook(oSockTcpSaveMapReqInfo, hSockTcpSaveMapReqInfo);
            SockTcpSaveMapReqHook.ApplyHook();
            SockTcpSayHook = new ManagedHook(oSockTcpSayInfo, hSockTcpSayInfo);
            SockTcpSayHook.ApplyHook();
            SockTcpIsConnectedHook = new ManagedHook(oSockTcpIsConnectedInfo, hSockTcpIsConnectedInfo);
            SockTcpIsConnectedHook.ApplyHook();
            ApplicationQuitHook = new ManagedHook(oApplicationQuitInfo, hApplicationQuitInfo);
			ApplicationQuitHook.ApplyHook();
            BuildOptionExitHook = new ManagedHook(oBuildOptionExitInfo, hBuildOptionExitInfo);
            BuildOptionExitHook.ApplyHook();
            P2PManagerSendPEER_RELIABLE_ACKHook = new ManagedHook(oP2PManagerSendPEER_RELIABLE_ACKInfo, hP2PManagerSendPEER_RELIABLE_ACKInfo);
            P2PManagerSendPEER_RELIABLE_ACKHook.ApplyHook();
            P2PManagerSendReliableHook = new ManagedHook(oP2PManagerSendReliableInfo, hP2PManagerSendReliableInfo);
            P2PManagerSendReliableHook.ApplyHook();
            P2PManagerSayHook = new ManagedHook(oP2PManagerSayInfo, hP2PManagerSayInfo);
            P2PManagerSayHook.ApplyHook();
            P2PManagerWhisperHook = new ManagedHook(oP2PManagerWhisperInfo, hP2PManagerWhisperInfo);
            P2PManagerWhisperHook.ApplyHook();
            P2PManagerSendPEER_LEAVEHook = new ManagedHook(oP2PManagerSendPEER_LEAVEInfo, hP2PManagerSendPEER_LEAVEInfo);
            P2PManagerSendPEER_LEAVEHook.ApplyHook();
            LoadBrickMainStartHook = new ManagedHook(oLoadBrickMainStartInfo, hLoadBrickMainStartInfo);
            LoadBrickMainStartHook.ApplyHook();
            ScreenSetResolutionHook = new ManagedHook(oScreenSetResolutionInfo, hScreenSetResolutionInfo);
            ScreenSetResolutionHook.ApplyHook();
        }
    }
}