using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.IO;

namespace _Emulator
{
    class Hooks
    {
        static MethodInfo oP2PManagerHandshakeInfo = typeof(P2PManager).GetMethod("Handshake", BindingFlags.NonPublic | BindingFlags.Instance);
        static MethodInfo hP2PManagerHandshakeInfo = typeof(Hooks).GetMethod("hP2PManagerHandshake", BindingFlags.NonPublic | BindingFlags.Instance);
        static Hook P2PManagerHandshakeHook;

		static MethodInfo oSockTcpGetSendKeyInfo = typeof(SockTcp).GetMethod("GetSendKey", BindingFlags.NonPublic | BindingFlags.Instance);
		static MethodInfo hSockTcpGetSendKeyInfo = typeof(Hooks).GetMethod("hSockTcpGetSendKey", BindingFlags.NonPublic | BindingFlags.Instance);
		static Hook SockTcpGetSendKeyHook;

		static MethodInfo oSockTcpEnterAckInfo = typeof(SockTcp).GetMethod("HandleCS_ENTER_ACK", BindingFlags.NonPublic | BindingFlags.Instance);
		static MethodInfo hSockTcpEnterAckInfo = typeof(Hooks).GetMethod("hSockTcpEnterAck", BindingFlags.NonPublic | BindingFlags.Instance);
		static Hook SockTcpEnterAckHook;

		static MethodInfo oSockTcpRendezvousInfoAckInfo = typeof(SockTcp).GetMethod("HandleCS_RENDEZVOUS_INFO_ACK", BindingFlags.NonPublic | BindingFlags.Instance);
		static MethodInfo hSockTcpRendezvousInfoAckInfo = typeof(Hooks).GetMethod("hSockTcpRendezvousInfoAck", BindingFlags.NonPublic | BindingFlags.Instance);
		static Hook SockTcpRendezvousInfoAckHook;

		static MethodInfo oPimpManagerLoadInfo = typeof(PimpManager).GetMethod("Load", BindingFlags.Public | BindingFlags.Instance);
		static MethodInfo hPimpManagerLoadInfo = typeof(Hooks).GetMethod("hPimpManagerLoad", BindingFlags.Public | BindingFlags.Instance);
		static Hook PimpManagerLoadHook;

		static MethodInfo oP2PManagerReliableSendInfo = typeof(P2PManager).GetMethod("ReliableSend", BindingFlags.NonPublic | BindingFlags.Instance);
		static MethodInfo hP2PManagerReliableSendInfo = typeof(Hooks).GetMethod("hP2PManagerReliableSend", BindingFlags.NonPublic | BindingFlags.Instance);
		static Hook P2PManagerReliableSendHook;

		static MethodInfo oSockTcpKilllogReqInfo = typeof(SockTcp).GetMethod("SendCS_KILL_LOG_REQ", BindingFlags.Public | BindingFlags.Instance);
		static MethodInfo hSockTcpKilllogReqInfo = typeof(Hooks).GetMethod("hSockTcpKilllogReq", BindingFlags.Public | BindingFlags.Instance);
		static Hook SockTcpKilllogReqHook;

		static MethodInfo oSockTcpKilllogAckInfo = typeof(SockTcp).GetMethod("HandleCS_KILL_LOG_ACK", BindingFlags.NonPublic | BindingFlags.Instance);
		static MethodInfo hSockTcpKilllogAckInfo = typeof(Hooks).GetMethod("hSockTcpKilllogAck", BindingFlags.NonPublic | BindingFlags.Instance);
		static Hook SockTcpKilllogAckHook;

		static MethodInfo oSockTcpHandleItemListAckInfo = typeof(SockTcp).GetMethod("HandleCS_ITEM_LIST_ACK", BindingFlags.NonPublic | BindingFlags.Instance);
		static MethodInfo hSockTcpHandleItemListAckInfo = typeof(Hooks).GetMethod("hSockTcpHandleItemListAck", BindingFlags.NonPublic | BindingFlags.Instance);
		static Hook SockTcpHandleItemListAckHook;

		static MethodInfo oSockTcpHandleShooterToolAckInfo = typeof(SockTcp).GetMethod("HandleCS_SHOOTER_TOOL_ACK", BindingFlags.NonPublic | BindingFlags.Instance);
		static MethodInfo hSockTcpHandleShooterToolAckInfo = typeof(Hooks).GetMethod("hSockTcpHandleShooterToolAck", BindingFlags.NonPublic | BindingFlags.Instance);
		static Hook SockTcpHandleShooterToolAckHook;

		static MethodInfo oSockTcpHandleShooterToolListAckInfo = typeof(SockTcp).GetMethod("HandleCS_SHOOTER_TOOL_LIST_ACK", BindingFlags.NonPublic | BindingFlags.Instance);
		static MethodInfo hSockTcpHandleShooterToolListAckInfo = typeof(Hooks).GetMethod("hSockTcpHandleShooterToolListAck", BindingFlags.NonPublic | BindingFlags.Instance);
		static Hook SockTcpHandleShooterToolListAckHook;

		static MethodInfo oSockTcpHandleWeaponSlotAckInfo = typeof(SockTcp).GetMethod("HandleCS_WEAPON_SLOT_ACK", BindingFlags.NonPublic | BindingFlags.Instance);
		static MethodInfo hSockTcpHandleWeaponSlotAckInfo = typeof(Hooks).GetMethod("hSockTcpHandleWeaponSlotAck", BindingFlags.NonPublic | BindingFlags.Instance);
		static Hook SockTcpHandleWeaponSlotAckHook;

		static MethodInfo oSockTcpHandleWeaponSlotListAckInfo = typeof(SockTcp).GetMethod("HandleCS_WEAPON_SLOT_LIST_ACK", BindingFlags.NonPublic | BindingFlags.Instance);
		static MethodInfo hSockTcpHandleWeaponSlotListAckInfo = typeof(Hooks).GetMethod("hSockTcpHandleWeaponSlotListAck", BindingFlags.NonPublic | BindingFlags.Instance);
		static Hook SockTcpHandleWeaponSlotListAckHook;

		static MethodInfo oSockTcpRegisterReqInfo = typeof(SockTcp).GetMethod("SendCS_REGISTER_REQ", BindingFlags.Public | BindingFlags.Instance);
		static MethodInfo hSockTcpRegisterReqInfo = typeof(Hooks).GetMethod("hSockTcpRegisterReq", BindingFlags.Public | BindingFlags.Instance);
		static Hook SockTcpRegisterReqHook;

        static MethodInfo oSockTcpSaveMapReqInfo = typeof(SockTcp).GetMethod("SendCS_SAVE_REQ", BindingFlags.Public | BindingFlags.Instance);
        static MethodInfo hSockTcpSaveMapReqInfo = typeof(Hooks).GetMethod("hSockTcpSaveMapReq", BindingFlags.Public | BindingFlags.Instance);
        static Hook SockTcpSaveMapReqHook;

        static MethodInfo oMyInfoManagerSetItemUsageInfo = typeof(MyInfoManager).GetMethod("SetItemUsage", BindingFlags.Public | BindingFlags.Instance);
		static MethodInfo hMyInfoManagerSetItemUsageInfo = typeof(Hooks).GetMethod("hMyInfoManagerSetItemUsage", BindingFlags.Public | BindingFlags.Instance);
		static Hook MyInfoManagerSetItemUsageHook;

		static MethodInfo oApplicationQuitInfo = typeof(Application).GetMethod("Quit", BindingFlags.Public | BindingFlags.Static);
		static MethodInfo hApplicationQuitInfo = typeof(Hooks).GetMethod("hApplicationQuit", BindingFlags.Public | BindingFlags.Static);
		static Hook ApplicationQuitHook;

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
						peer.Value.Update();
					}
				}
			}
		}

		private byte hSockTcpGetSendKey()
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
			msg.Read(out int _);
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
			ApplicationQuitHook.CallOriginal(null, null);
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
            Debug.LogError("SAVEREQ");
            CSNetManager.Instance.Sock.Say(39, msgBody);
        }

        public static void Initialize()
        {
			P2PManagerHandshakeHook = new Hook(oP2PManagerHandshakeInfo, hP2PManagerHandshakeInfo);
			P2PManagerHandshakeHook.ApplyHook();
			SockTcpGetSendKeyHook = new Hook(oSockTcpGetSendKeyInfo, hSockTcpGetSendKeyInfo);
			SockTcpGetSendKeyHook.ApplyHook();
			SockTcpEnterAckHook = new Hook(oSockTcpEnterAckInfo, hSockTcpEnterAckInfo);
			SockTcpEnterAckHook.ApplyHook();
			SockTcpRendezvousInfoAckHook = new Hook(oSockTcpRendezvousInfoAckInfo, hSockTcpRendezvousInfoAckInfo);
			SockTcpRendezvousInfoAckHook.ApplyHook();
			PimpManagerLoadHook = new Hook(oPimpManagerLoadInfo, hPimpManagerLoadInfo);
			PimpManagerLoadHook.ApplyHook();
			P2PManagerReliableSendHook = new Hook(oP2PManagerReliableSendInfo, hP2PManagerReliableSendInfo);
			P2PManagerReliableSendHook.ApplyHook();
			SockTcpKilllogReqHook = new Hook(oSockTcpKilllogReqInfo, hSockTcpKilllogReqInfo);
			SockTcpKilllogReqHook.ApplyHook();
			SockTcpKilllogAckHook = new Hook(oSockTcpKilllogAckInfo, hSockTcpKilllogAckInfo);
			SockTcpKilllogAckHook.ApplyHook();
			SockTcpHandleItemListAckHook = new Hook(oSockTcpHandleItemListAckInfo, hSockTcpHandleItemListAckInfo);
			SockTcpHandleItemListAckHook.ApplyHook();
			MyInfoManagerSetItemUsageHook = new Hook(oMyInfoManagerSetItemUsageInfo, hMyInfoManagerSetItemUsageInfo);
			MyInfoManagerSetItemUsageHook.ApplyHook();
			SockTcpHandleShooterToolAckHook = new Hook(oSockTcpHandleShooterToolAckInfo, hSockTcpHandleShooterToolAckInfo);
			SockTcpHandleShooterToolAckHook.ApplyHook();
			SockTcpHandleShooterToolListAckHook = new Hook(oSockTcpHandleShooterToolListAckInfo, hSockTcpHandleShooterToolListAckInfo);
			SockTcpHandleShooterToolListAckHook.ApplyHook();
			SockTcpHandleWeaponSlotAckHook = new Hook(oSockTcpHandleWeaponSlotAckInfo, hSockTcpHandleWeaponSlotAckInfo);
			SockTcpHandleWeaponSlotAckHook.ApplyHook();
			SockTcpHandleWeaponSlotListAckHook = new Hook(oSockTcpHandleWeaponSlotListAckInfo, hSockTcpHandleWeaponSlotListAckInfo);
			SockTcpHandleWeaponSlotListAckHook.ApplyHook();
			SockTcpRegisterReqHook = new Hook(oSockTcpRegisterReqInfo, hSockTcpRegisterReqInfo);
			SockTcpRegisterReqHook.ApplyHook();
            SockTcpSaveMapReqHook = new Hook(oSockTcpSaveMapReqInfo, hSockTcpSaveMapReqInfo);
            SockTcpSaveMapReqHook.ApplyHook();
            ApplicationQuitHook = new Hook(oApplicationQuitInfo, hApplicationQuitInfo);
			ApplicationQuitHook.ApplyHook();
		}
    }
}