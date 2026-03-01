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

        static MethodInfo oMyInfoManagerBuyItemInfo = typeof(MyInfoManager).GetMethod("BuyItem", BindingFlags.Public | BindingFlags.Instance);
        static MethodInfo hMyInfoManagerBuyItemInfo = typeof(HooksManaged).GetMethod("hMyInfoManagerBuyItem", BindingFlags.Public | BindingFlags.Instance);
        static ManagedHook MyInfoManagerBuyItemHook;

        static MethodInfo oSockTcpUserMapReq = typeof(SockTcp).GetMethod("SendCS_USER_MAP_REQ", BindingFlags.Public | BindingFlags.Instance);
        static MethodInfo hSockTcpUserMapReq = typeof(HooksManaged).GetMethod("hUserMapReq", BindingFlags.Public | BindingFlags.Instance);
        static ManagedHook SockTcpUserMapReqHook;

        static MethodInfo oSockTcpResetUserMapSlotReq= typeof(SockTcp).GetMethod("SendCS_RESET_USER_MAP_SLOTS_REQ", BindingFlags.Public | BindingFlags.Instance);
        static MethodInfo hSockTcpResetUserMapSlotReq = typeof(HooksManaged).GetMethod("hResetUserMapSlotReq", BindingFlags.Public | BindingFlags.Instance);
        static ManagedHook SockTcpResetUserMapSlotReqHook;

        static MethodInfo oSockTcpMyDownloadMapReq = typeof(SockTcp).GetMethod("SendCS_MY_DOWNLOAD_MAP_REQ", BindingFlags.Public | BindingFlags.Instance);
        static MethodInfo hSockTcpMyDownloadMapReq = typeof(HooksManaged).GetMethod("hMyDownloadMapReq", BindingFlags.Public | BindingFlags.Instance);
        static ManagedHook SockTcpMyDownloadMapReqHook;

        static MethodInfo oBndMatchStartLoadInfo = typeof(BndMatch).GetMethod("StartLoad", BindingFlags.NonPublic | BindingFlags.Instance);
        static MethodInfo hBndMatchStartLoadInfo = typeof(HooksManaged).GetMethod("hBndMatchStartLoad", BindingFlags.Public | BindingFlags.Instance);
        static ManagedHook BndMatchStartLoadHook;

        static MethodInfo oMapEditorStartLoadInfo = typeof(MapEditor).GetMethod("StartLoad", BindingFlags.NonPublic | BindingFlags.Instance);
        static MethodInfo hMapEditorStartLoadInfo = typeof(HooksManaged).GetMethod("hMapEditorStartLoad", BindingFlags.Public | BindingFlags.Instance);
        static ManagedHook MapEditorStartLoadHook;

        static MethodInfo oMapEditorOnLoadCompleteInfo = typeof(MapEditor).GetMethod("OnLoadComplete", BindingFlags.NonPublic | BindingFlags.Instance);
        static MethodInfo hMapEditorOnLoadCompleteInfo = typeof(HooksManaged).GetMethod("hMapEditorOnLoadComplete", BindingFlags.Public | BindingFlags.Instance);
        static ManagedHook MapEditorOnLoadCompleteHook;

        static MethodInfo oUserMapInfoManagerCreateBuildModeInfo = typeof(UserMapInfoManager).GetMethod("CreateBuildMode", BindingFlags.Public | BindingFlags.Instance);
        static MethodInfo hUserMapInfoManagerCreateBuildModeInfo = typeof(HooksManaged).GetMethod("hUserMapInfoManagerCreateBuildMode", BindingFlags.Public | BindingFlags.Instance);
        static ManagedHook UserMapInfoManagerCreateBuildModeHook;

        static MethodInfo oDoPagePanel = typeof(DownloadMapFrame).GetMethod("DoPagePanel", BindingFlags.NonPublic | BindingFlags.Instance);
        static MethodInfo hDoPagePanel = typeof(HooksManaged).GetMethod("hDownloadMapFrameDoPagePanel", BindingFlags.Public | BindingFlags.Static);
        static ManagedHook DoPagePanelHook;

        static MethodInfo oDownloadMapFrameOnGUI = typeof(DownloadMapFrame).GetMethod("OnGUI", BindingFlags.Public | BindingFlags.Instance);
        static MethodInfo hDownloadMapFrameOnGUI = typeof(HooksManaged).GetMethod("hDownloadMapOnGUI", BindingFlags.Public | BindingFlags.Static);
        static ManagedHook DownloadMapFrameOnGUIHook;

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

		public void hBndMatchStartLoad() 
		{ 
			GC.Collect();
			UserMap userMap = new UserMap();
			userMap.isLoaded = false;
			BrickManager.Instance.userMap = userMap;
            CSNetManager.Instance.Sock.SendCS_CACHE_BRICK_REQ();
        }
        public void hMapEditorStartLoad()
        {
            GC.Collect();
            UserMap userMap = new UserMap();
            userMap.isLoaded = false;
            BrickManager.Instance.userMap = userMap;
            CSNetManager.Instance.Sock.SendCS_CACHE_BRICK_REQ();
        }
        public void hMapEditorOnLoadComplete()
        {
            GameObject gameObject = GameObject.Find("Main");
            if (gameObject == null)
            {
                return;
            }
            MapEditor mapEditor = gameObject.GetComponent<MapEditor>();
            if (mapEditor == null)
            {
                return;
            }
            PaletteManager paletteManager = PaletteManager.Instance;
            if (paletteManager != null)
            {
                int[] pal = BrickCache.Instance.palette;
                PaletteManager.Instance.Setup(pal[0], pal[1], pal[2], pal[3], pal[4], pal[5], pal[6], pal[7], pal[8], pal[9]);
            }
            CSNetManager.Instance.Sock.SendCS_RESUME_ROOM_REQ(2);
            UserMap userMap = BrickManager.Instance.userMap;
            Vector3 position;
            if (userMap == null)
            {
                position = EditHelper.MAP_CENTER;
            } else
            {
                position = new Vector3(userMap.cenX, userMap.max.y + 5f, userMap.cenZ);
            }
            mapEditor.localController.Spawn(position, Rot.ToQuaternion((byte)UnityEngine.Random.Range(0, 4)));
            mapEditor.bLoaded = true;
            if (!MyInfoManager.Instance.GetCommonMask(MyInfoManager.COMMON_OPT.DONOT_MAPEDIT_GUIDE))
            {
                MapEditGuideDialog mapEditGuideDialog = (MapEditGuideDialog)DialogManager.Instance.GetDialogAlways(DialogManager.DIALOG_INDEX.BUILD_GUIDE);
                if (mapEditGuideDialog != null && !mapEditGuideDialog.DontShowThisMessageAgain)
                {
                    ((MapEditGuideDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.BUILD_GUIDE, exclusive: false))?.InitDialog();
                }
            }
        }

        public void hUserMapInfoManagerCreateBuildMode(int slot, string alias)
        {
            ClientExtension.instance.buildModeMapName = alias;
            UserMapInfoManager.Instance.CurSlot = slot;
            UserMapInfoManager.Instance.CurMapName = alias;
            UserMapInfoManager.Instance.dicRegMap.Clear();
            UserMapInfoManager.Instance.cacheRegMap.Clear();
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
			else if (ClientExtension.instance.clientConnected)
			{
                CSNetManager.Instance.Sock.Close();
                P2PManager.Instance.Shutdown();
			}

			SteamFriendsManager.instance.ClearRichPresence();

            HooksNative.Shutdown();

            var hProcess = Import.GetCurrentProcess();
            Import.GetExitCodeProcess(hProcess, out uint exitCode);
            Debug.Log("Terminate");
            Import.TerminateProcess(hProcess, exitCode);

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
            UserMapInfo umi = UserMapInfoManager.Instance.Get(slot);

            // Thumbnail for the registered map
            Texture2D thumbnailTex = new Texture2D(128, 128, TextureFormat.RGB24, mipmap: false);
            thumbnailTex.LoadImage(thumbnail);
            thumbnailTex.Apply();

            DateTime time = DateTime.Now;
            int hashId = MapGenerator.instance.GetHashIdForTime(time);

            // Create & register RegMap ONLY here
            RegMap regMap = new RegMap(
                hashId,
                ClientExtension.instance.name + "@Aurora",
                umi.Alias,
                time,
                modeMask,
                true, false,
                0, 0, 0, 0, 0, 0, 0,
                false
            );

            regMap.Thumbnail = thumbnailTex;

            RegMapManager.Instance.Add(regMap);
            RegMapManager.Instance.SetThumbnail(regMap.map, thumbnailTex);

            // Save registered files under the RegMap ID (separate from user slot file)
            regMap.Save();

            MsgBody body = new MsgBody();

            body.Write(umi.slot);
            body.Write((int)regMap.ModeMask);
            CSNetManager.Instance.Sock.HandleCS_REGISTER_ACK(body);
        }

        public void hUserMapReq(int page)
        {
            /*MsgBody msgBody = new MsgBody();
            msgBody.Write(page);
            Say(429, msgBody);*/
            const int firstId = 33;
            const int slotCount = 12;

            for (int id = firstId; id < firstId + slotCount; id++)
            {
                string alias = "";
                int brickCount = -1;
                DateTime lastModified = DateTime.MinValue;
                sbyte premium = 0;

                var umi = new UserMapInfo(id, premium);
                if (umi.LoadCache())
                {
                    umi.VerifySavedData();
                    alias = umi.Alias;
                    brickCount = umi.BrickCount;
                    lastModified = umi.LastModified;
                    premium = umi.Premium;
                }

                if (!string.IsNullOrEmpty(alias) && lastModified.Year > 1971)
                {
                    UserMapInfoManager.Instance.AddOrUpdate(id, alias, brickCount, lastModified, premium);
                }
                else
                {
                    UserMapInfoManager.Instance.AddOrUpdate(id, alias, brickCount, DateTime.MinValue, premium);
                }
            }
            return;
        }

        public void hResetUserMapSlotReq(int slot, long item, string itemCode)
        {
            /*MsgBody msgBody = new MsgBody();
            msgBody.Write(slot);
            msgBody.Write(item);
            msgBody.Write(itemCode);
            Say(405, msgBody);*/

            int result = 0;
            if (slot < 33 || slot > 44)
            {
                Actor.Instance.ShowDelayedMessage(StringMgr.Instance.Get("FAIL_TO_RESET_MAP_SLOT"));
            }
            else
            {
                try
                {
                    string cacheDir = Path.Combine(Application.dataPath, "Resources/Cache");

                    string geom = Path.Combine(cacheDir, "downloaded" + slot + ".geometry");
                    string umi = Path.Combine(cacheDir, "downloaded" + slot + ".umi.cache");

                    if (File.Exists(geom)) File.Delete(geom);
                    if (File.Exists(umi)) File.Delete(umi);
                    UserMapInfo userMapInfo = UserMapInfoManager.Instance.Get((byte)slot);
                    if (userMapInfo != null && userMapInfo.Alias.Length > 0)
                    {
                        string msg2 = string.Format(StringMgr.Instance.Get("RESET_MAP_SLOT_SUCCESS"), userMapInfo.Alias);
                        SystemMsgManager.Instance.ShowMessage(msg2);
                    }
                    UserMapInfoManager.Instance.Remove((byte)slot);
                    UserMapInfoManager.Instance.ValidateEmpty();
                }
                catch (Exception ex)
                {
                    result = 1;
                    Debug.LogError("Local ResetUserMapSlot failed: " + ex);
                }
            }
        }

        public void hMyDownloadMapReq(int prevPage, int nextPage, int indexer, ushort modeMask)
        {
            List<KeyValuePair<int, RegMap>> regMaps = RegMapManager.Instance.dicRegMap.ToList();
            DownloadMapFrame downloadMapFrame = null;
            GameObject gameObject = GameObject.Find("Main");
            if (null != gameObject)
            {
                Lobby component = gameObject.GetComponent<Lobby>();
                if (null != component)
                {
                    downloadMapFrame = component.myMapFrm.downloadMapFrm;
                }
            }
            if (downloadMapFrame != null && regMaps.Count > 0)
            {
                downloadMapFrame.BeginMapList(1);
            }

            foreach (var item in regMaps)
            {
                if (downloadMapFrame != null)
                {
                    if (item.Key == 0 && downloadMapFrame != null)
                    {
                        downloadMapFrame.firstIndexer = item.Value.Map;
                    }
                    if (item.Key == regMaps.Count - 1)
                    {
                        downloadMapFrame.lastIndexer = item.Value.Map;
                    }
                }
                RegMapManager.Instance.SetDownload(item.Value.Map, download: true);
            }
            downloadMapFrame?.EndMapList();
        }

        public static void hDownloadMapFrameDoPagePanel(DownloadMapFrame __instance, int length)
        {
            // no-op => no page UI, no page requests
        }

        static readonly BindingFlags BF = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        static readonly FieldInfo f_scrollPosition = typeof(DownloadMapFrame).GetField("scrollPosition", BF);
        static readonly FieldInfo f_chatView = typeof(DownloadMapFrame).GetField("chatView", BF);
        static readonly FieldInfo f_selected = typeof(DownloadMapFrame).GetField("selected", BF);
        static readonly FieldInfo f_lastClickTime = typeof(DownloadMapFrame).GetField("lastClickTime", BF);
        static readonly FieldInfo f_page = typeof(DownloadMapFrame).GetField("page", BF);
        static readonly FieldInfo f_crdRegMapRect = typeof(DownloadMapFrame).GetField("crdRegMapRect", BF);
        static readonly FieldInfo f_crdRegMapRectTemp = typeof(DownloadMapFrame).GetField("crdRegMapRectTemp", BF);
        static readonly FieldInfo f_crdMapSize = typeof(DownloadMapFrame).GetField("crdMapSize", BF);
        static readonly FieldInfo f_crdMapOffset = typeof(DownloadMapFrame).GetField("crdMapOffset", BF);
        static readonly FieldInfo f_crdAlias = typeof(DownloadMapFrame).GetField("crdAlias", BF);
        static readonly FieldInfo f_crdBtns = typeof(DownloadMapFrame).GetField("crdBtns", BF);
        static readonly FieldInfo f_crdBtns2 = typeof(DownloadMapFrame).GetField("crdBtns2", BF);
        static readonly FieldInfo f_doubleClickTimeout = typeof(DownloadMapFrame).GetField("doubleClickTimeout", BF);
        static readonly MethodInfo m_VerifyChatView = typeof(DownloadMapFrame).GetMethod("VerifyChatView", BF);
        static readonly MethodInfo m_GetFirstEmptyUserSlot = typeof(DownloadMapFrame).GetMethod("GetFirstEmptyUserSlot", BF);
        static readonly MethodInfo m_CopyRegMapToUserSlot = typeof(DownloadMapFrame).GetMethod("CopyRegMapToUserSlot", BF);

        private static _Emulator.RegMapQuickFilter regMapFilter = new _Emulator.RegMapQuickFilter();

        public static void hDownloadMapOnGUI(DownloadMapFrame __instance)
        {
            if (__instance == null) return;

            // -------- read needed fields --------
            var scrollPosition = (Vector2)(f_scrollPosition?.GetValue(__instance) ?? Vector2.zero);
            bool chatView = (bool)(f_chatView?.GetValue(__instance) ?? false);
            int selected = (int)(f_selected?.GetValue(__instance) ?? 0);
            float lastClick = (float)(f_lastClickTime?.GetValue(__instance) ?? 0f);
            float dblTimeout = (float)(f_doubleClickTimeout?.GetValue(__instance) ?? 0.2f);

            Rect crdRegMapRect = (Rect)(f_crdRegMapRect?.GetValue(__instance) ?? new Rect(264f, 142f, 732f, 515f));
            Rect crdRegMapRectTemp = (Rect)(f_crdRegMapRectTemp?.GetValue(__instance) ?? crdRegMapRect);
            Vector2 crdMapSize = (Vector2)(f_crdMapSize?.GetValue(__instance) ?? new Vector2(150f, 196f));
            Vector2 crdMapOffset = (Vector2)(f_crdMapOffset?.GetValue(__instance) ?? new Vector2(35f, 21f));
            Vector2 crdAlias = (Vector2)(f_crdAlias?.GetValue(__instance) ?? new Vector2(5f, 174f));

            var crdBtns = (Rect[])(f_crdBtns?.GetValue(__instance) ?? new Rect[0]);
            var crdBtns2 = (Rect[])(f_crdBtns2?.GetValue(__instance) ?? new Rect[0]);

            // -------- keep chat sizing behavior (from original) --------
            m_VerifyChatView?.Invoke(__instance, null);
            chatView = (bool)(f_chatView?.GetValue(__instance) ?? chatView);

            if (chatView)
                crdRegMapRect.height = 300f;
            else
                crdRegMapRect.height = crdRegMapRectTemp.height;

            // write back updated rect (since we modified height)
            f_crdRegMapRect?.SetValue(__instance, crdRegMapRect);

            // -------- get FULL reg list (no paging) --------
            // BEST: if you really have RegMapManager.Instance.ToArray(subTab) returning full list, use that.
            // e.g.: RegMap[] reg = RegMapManager.Instance.ToArray( /*tab*/ 0 );
            // Since subTab is private and you want mode selection disabled anyway, we just use ALL maps.
            RegMap[] reg = RegMapManager.Instance.dicRegMap.Values.ToArray();

            if (reg == null || reg.Length == 0)
                return;

            // -------- quick filter UI (position/sizing from your example) --------
            // Place it above the map grid, but keep the original rects.
            // Adjust these if you want; you said you have it set up already.
            bool changed = regMapFilter.Draw(
                new Vector2(800f, 90f),
                new Rect(800f, 100f, 210f, 26f)
            );

            if (changed || regMapFilter.Indices == null || regMapFilter.Indices.Length == 0)
            {
                regMapFilter.Rebuild(reg);
                selected = regMapFilter.ClampSelection(reg, selected);
            }

            int[] visible = regMapFilter.Indices;
            int visibleCount = (visible != null) ? visible.Length : 0;
            Debug.Log(visibleCount);
            if (visibleCount <= 0)
            {
                // nothing matches filter => still draw empty scroll area (optional)
                // Just persist selection + scroll and exit.
                f_selected?.SetValue(__instance, selected);
                f_scrollPosition?.SetValue(__instance, scrollPosition);
                f_lastClickTime?.SetValue(__instance, lastClick);
                return;
            }

            // -------- 4 columns like original (your DoRegMap example uses 3) --------
            const int COLS = 4;

            int rows = visibleCount / COLS;
            if (visibleCount % COLS > 0) rows++;

            Rect viewRect = new Rect(
                0f, 0f,
                crdMapSize.x * COLS + crdMapOffset.x * (COLS - 1),
                crdMapSize.y * rows
            );

            if (rows > 1)
                viewRect.height += crdMapOffset.y * (rows - 1);

            // -------- scroll view render --------
            scrollPosition = GUI.BeginScrollView(crdRegMapRect, scrollPosition, viewRect);

            bool openDetail = false; // double click behavior like original

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < COLS; c++)
                {
                    int idxVisible = COLS * r + c;
                    if (idxVisible >= visibleCount) continue;

                    int i = visible[idxVisible]; // index into reg[]

                    Rect rect = new Rect(
                        c * (crdMapSize.x + crdMapOffset.x),
                        r * (crdMapSize.y + crdMapOffset.y),
                        crdMapSize.x,
                        crdMapSize.y
                    );

                    Rect thumbRect = new Rect(rect.x, rect.y, rect.width, rect.width + 4f);

                    // thumbnail / blocked overlay (same as original)
                    Texture2D thumb = (reg[i].Thumbnail != null) ? reg[i].Thumbnail : __instance.nonAvailable;

                    if (!reg[i].Blocked)
                    {
                        TextureUtil.DrawTexture(thumbRect, thumb, ScaleMode.StretchToFill);
                    }
                    else
                    {
                        TextureUtil.DrawTexture(thumbRect, GlobalVars.Instance.iconBoxGray, ScaleMode.StretchToFill);

                        float w = GlobalVars.Instance.iconLockSlot.width;
                        float h = GlobalVars.Instance.iconLockSlot.height;
                        float x = thumbRect.x + thumbRect.width / 2f - w / 2f;
                        float y = thumbRect.y + thumbRect.height / 2f - h / 2f;
                        TextureUtil.DrawTexture(new Rect(x, y, w, h), GlobalVars.Instance.iconLockSlot, ScaleMode.StretchToFill);
                    }

                    // click handling (select + double click opens detail)
                    if (GlobalVars.Instance.MyButton(rect, string.Empty, "BoxMapSelectBorder"))
                    {
                        selected = i;

                        if (reg[selected].Blocked)
                            MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NOTICE_BLOCK_MAP"));

                        if (Time.time - lastClick > dblTimeout)
                        {
                            lastClick = Time.time;
                        }
                        else
                        {
                            openDetail = true;
                        }
                    }

                    // "new map"/tags/abuse icons (same as original)
                    DateTime d = reg[i].RegisteredDate;
                    if (d.Year == DateTime.Today.Year && d.Month == DateTime.Today.Month && d.Day == DateTime.Today.Day)
                    {
                        TextureUtil.DrawTexture(new Rect(rect.x, rect.y,
                            GlobalVars.Instance.iconNewmap.width,
                            GlobalVars.Instance.iconNewmap.height),
                            GlobalVars.Instance.iconNewmap, ScaleMode.StretchToFill);
                    }
                    else if ((reg[i].tagMask & 8) != 0)
                    {
                        TextureUtil.DrawTexture(new Rect(rect.x, rect.y,
                            GlobalVars.Instance.iconglory.width,
                            GlobalVars.Instance.iconglory.height),
                            GlobalVars.Instance.iconglory, ScaleMode.StretchToFill);
                    }
                    else if ((reg[i].tagMask & 4) != 0)
                    {
                        TextureUtil.DrawTexture(new Rect(rect.x, rect.y,
                            GlobalVars.Instance.iconMedal.width,
                            GlobalVars.Instance.iconMedal.height),
                            GlobalVars.Instance.iconMedal, ScaleMode.StretchToFill);
                    }
                    else if ((reg[i].tagMask & 2) != 0)
                    {
                        TextureUtil.DrawTexture(new Rect(rect.x, rect.y,
                            GlobalVars.Instance.icongoldRibbon.width,
                            GlobalVars.Instance.icongoldRibbon.height),
                            GlobalVars.Instance.icongoldRibbon, ScaleMode.StretchToFill);
                    }

                    if (reg[i].IsAbuseMap())
                    {
                        float x2 = rect.x + rect.width - GlobalVars.Instance.iconDeclare.width;
                        TextureUtil.DrawTexture(new Rect(x2, rect.y,
                            GlobalVars.Instance.iconDeclare.width,
                            GlobalVars.Instance.iconDeclare.height),
                            GlobalVars.Instance.iconDeclare, ScaleMode.StretchToFill);
                    }
                    LabelUtil.TextOut(
                        new Vector2(rect.x + crdAlias.x, rect.y + crdAlias.y - 18f),   // shift up a bit
                        reg[i].Developer ?? "",
                        "MiniLabel",
                        GlobalVars.Instance.txtMainColor,
                        GlobalVars.txtEmptyColor,
                        TextAnchor.UpperLeft
                    );
                    // alias label (same placement from original)
                    LabelUtil.TextOut(
                        new Vector2(rect.x + crdAlias.x, rect.y + crdAlias.y),
                        reg[i].Alias,
                        "MiniLabel",
                        GlobalVars.Instance.txtMainColor,
                        GlobalVars.txtEmptyColor,
                        TextAnchor.UpperLeft
                    );

                    // selection overlay
                    if (selected == i)
                        TextureUtil.DrawTexture(rect, __instance.selectedMapFrame, ScaleMode.StretchToFill);
                }
            }

            GUI.EndScrollView();

            // open detail dialog if double clicked
            if (openDetail && selected >= 0 && selected < reg.Length)
            {
                ((MapDetailDlg)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.MAP_DETAIL, exclusive: true))
                    ?.InitDialog(reg[selected]);
            }

            // -------- keep the bottom buttons (LOAD TO SLOT / CREATE ROOM / DELETE) --------
            if (selected >= 0 && selected < reg.Length)
            {
                // LOAD TO SLOT
                Rect rc = (crdBtns.Length > 0) ? new Rect(crdBtns[0]) : new Rect(300f, 714f, 139f, 38f);
                if (chatView && crdBtns2.Length > 0) rc = new Rect(crdBtns2[0]);

                GUI.enabled = !reg[selected].Blocked;
                GUIContent loadContent = new GUIContent("LOAD TO SLOT", GlobalVars.Instance.iconJoin);

                if (GlobalVars.Instance.MyButton3(rc, loadContent, "BtnAction"))
                {
                    int slot = -1;
                    if (m_GetFirstEmptyUserSlot != null)
                        slot = (int)m_GetFirstEmptyUserSlot.Invoke(__instance, null);

                    if (slot < 0)
                    {
                        MessageBoxMgr.Instance.AddMessage("No empty map slots available.");
                    }
                    else
                    {
                        bool ok = false;
                        if (m_CopyRegMapToUserSlot != null)
                            ok = (bool)m_CopyRegMapToUserSlot.Invoke(__instance, new object[] { reg[selected], slot });

                        if (ok)
                            SystemMsgManager.Instance.ShowMessage($"Loaded '{reg[selected].Alias}' into slot {slot}.");
                        else
                            MessageBoxMgr.Instance.AddMessage("Failed to load map into slot.");
                    }
                }

                // CREATE ROOM
                rc = (crdBtns.Length > 1) ? new Rect(crdBtns[1]) : new Rect(715f, 714f, 139f, 38f);
                if (chatView && crdBtns2.Length > 1) rc = new Rect(crdBtns2[1]);

                GUI.enabled = !reg[selected].Blocked;
                GUIContent content = new GUIContent(StringMgr.Instance.Get("CREATE_ROOM").ToUpper(), GlobalVars.Instance.iconJoin);

                if (ChannelManager.Instance.CurChannel.Mode != 3 && GlobalVars.Instance.MyButton3(rc, content, "BtnAction"))
                {
                    CreateRoomDialog dlg = (CreateRoomDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.CREATE_ROOM, exclusive: true);
                    if (dlg != null && !dlg.InitDialog4TeamMatch(reg[selected].Map, reg[selected].ModeMask))
                        DialogManager.Instance.Clear();
                }

                // DELETE
                GUI.enabled = true;
                rc = (crdBtns.Length > 2) ? new Rect(crdBtns[2]) : new Rect(859f, 714f, 139f, 38f);
                if (chatView && crdBtns2.Length > 2) rc = new Rect(crdBtns2[2]);

                GUIContent delContent = new GUIContent(StringMgr.Instance.Get("DELETE").ToUpper(), GlobalVars.Instance.iconGarbage);
                if (GlobalVars.Instance.MyButton3(rc, delContent, "BtnAction"))
                {
                    CSNetManager.Instance.Sock.SendCS_DEL_DOWNLOAD_MAP_REQ(reg[selected].Map);
                    selected = 0;
                }

                GUI.enabled = true;
            }

            // -------- write back state to instance --------
            f_selected?.SetValue(__instance, selected);
            f_scrollPosition?.SetValue(__instance, scrollPosition);
            f_lastClickTime?.SetValue(__instance, lastClick);

            // keep page stable so other code doesn't freak out (optional)
            f_page?.SetValue(__instance, 1);
        }

        public void hSockTcpSaveMapReq(int slot, byte[] thumbnail)
        {
            /*MsgBody msgBody = new MsgBody();
            msgBody.Write(slot);
			msgBody.Write(thumbnail);
            CSNetManager.Instance.Sock.Say(39, msgBody);*/
            UserMapInfoManager mapInfoMng = UserMapInfoManager.Instance;
            if (mapInfoMng.CurSlot != slot)
            {
                Actor.Instance.ShowDelayedMessage("Map slots don't align, can't save");
                return;
            }
            string mapName = ClientExtension.instance.buildModeMapName;
            if (RoomManager.Instance.Master != MyInfoManager.Instance.Seq)
            {
                Actor.Instance.ShowDelayedMessage(string.Format(StringMgr.Instance.Get("SAVE_FAIL"), mapName));
                return;
            }
            Texture2D thumb = new Texture2D(128, 128, TextureFormat.RGB24, mipmap: false);
            DateTime time = DateTime.Now;
            thumb.LoadImage(thumbnail);
            thumb.Apply();
            int brickCount = BrickManager.Instance.Count;
            mapInfoMng.AddOrUpdate(slot, mapName, brickCount, time, (sbyte) 0);
            mapInfoMng.SetThumbnail(slot, thumb);
            mapInfoMng.Get(slot).SaveCache();
            UserMap map = BrickManager.Instance.userMap;
            map.Save(slot, map.skybox);
            Actor.Instance.ShowDelayedMessage(string.Format(StringMgr.Instance.Get("SAVE_SUCCESS"), mapName));
            MyInfoManager.Instance.IsModified = false;
        }

		public void hSockTcpSay(ushort id, MsgBody msgBody, bool doChunked = true)
		{
            ClientExtension.instance.SendPacket(id, msgBody, doChunked);
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

        public void hMyInfoManagerBuyItem(long seq, string code, int remain, sbyte premium, int durability)
        {
            TItem tItem = TItemManager.Instance.Get<TItem>(code);
            if (tItem == null)
            {
                Debug.LogError("Fail to get item template for " + code);
            }
            else
            {
                Item.USAGE uSAGE = (!tItem.IsAmount && tItem.catType != 0 && remain >= 0) ? Item.USAGE.NOT_USING : Item.USAGE.UNEQUIP;
                if (MyInfoManager.Instance.inventory.ContainsKey(seq))
                {
                    MyInfoManager.Instance.inventory[seq].Buy(remain, uSAGE, durability);
                }
                else
                {
                    MyInfoManager.Instance.inventory.Add(seq, new Item(seq, tItem, code, uSAGE, remain, premium, durability));
                }

				// Local inventory hack
				ClientExtension.instance.inventory.AddItem(tItem, false, remain, uSAGE);
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
            MyInfoManagerBuyItemHook = new ManagedHook(oMyInfoManagerBuyItemInfo, hMyInfoManagerBuyItemInfo);
            MyInfoManagerBuyItemHook.ApplyHook();
            SockTcpUserMapReqHook = new ManagedHook(oSockTcpUserMapReq, hSockTcpUserMapReq);
            SockTcpUserMapReqHook.ApplyHook();
            SockTcpResetUserMapSlotReqHook = new ManagedHook(oSockTcpResetUserMapSlotReq, hSockTcpResetUserMapSlotReq);
            SockTcpResetUserMapSlotReqHook.ApplyHook();
            SockTcpMyDownloadMapReqHook = new ManagedHook(oSockTcpMyDownloadMapReq, hSockTcpMyDownloadMapReq);
            SockTcpMyDownloadMapReqHook.ApplyHook();
            BndMatchStartLoadHook = new ManagedHook(oBndMatchStartLoadInfo, hBndMatchStartLoadInfo);
            BndMatchStartLoadHook.ApplyHook();
            MapEditorStartLoadHook = new ManagedHook(oMapEditorStartLoadInfo, hMapEditorStartLoadInfo);
            MapEditorStartLoadHook.ApplyHook();
            MapEditorOnLoadCompleteHook = new ManagedHook(oMapEditorOnLoadCompleteInfo, hMapEditorOnLoadCompleteInfo);
            MapEditorOnLoadCompleteHook.ApplyHook();
            UserMapInfoManagerCreateBuildModeHook = new ManagedHook(oUserMapInfoManagerCreateBuildModeInfo, hUserMapInfoManagerCreateBuildModeInfo);
            UserMapInfoManagerCreateBuildModeHook.ApplyHook();
            DoPagePanelHook = new ManagedHook(oDoPagePanel, hDoPagePanel);
            DoPagePanelHook.ApplyHook();
            DownloadMapFrameOnGUIHook = new ManagedHook(oDownloadMapFrameOnGUI, hDownloadMapFrameOnGUI);
            DownloadMapFrameOnGUIHook.ApplyHook();
        }
    }
}