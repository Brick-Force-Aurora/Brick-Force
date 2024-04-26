using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace _Emulator
{
    class ServerEmulator : MonoBehaviour
    {
		public static ServerEmulator instance;
		private readonly object dataLock = new object();
		public List<ClientReference> clientList = new List<ClientReference>();
        private Socket serverSocket;
		private byte recvKey = byte.MaxValue;
		private byte sendKey = byte.MaxValue;
		private Queue<MsgReference> readQueue = new Queue<MsgReference>();
		private Queue<MsgReference> writeQueue = new Queue<MsgReference>();
		private int curSeq = 0;
		public bool debugHandle = false;
		public bool debugSend = false;
		public bool debugPing = false;
		public bool serverCreated = false;
		//public MatchData matchData;
		//private Channel defaultChannel;
		public ChannelManager channelManager = new ChannelManager();
		private float killLogTimer = 0f;
		public List<KeyValuePair<int, RegMap>> regMaps = new List<KeyValuePair<int, RegMap>>();
		private bool waitForShutDown = false;

		private void Start()
		{
			//matchData = new MatchData();
		}

		public void SetupServer()
		{
			try
			{
				if (serverSocket == null)
				{
					serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					serverSocket.Bind(new IPEndPoint(IPAddress.Any, 5000));
				}
				serverSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
				serverSocket.Listen(16);
				serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
				serverCreated = true;
				Debug.Log("Server created");
			}

			catch (Exception ex)
			{
				Debug.LogError("SetupServer: " + ex.Message);
			}

			regMaps = RegMapManager.Instance.dicRegMap.ToList();
		}

		private void AcceptCallback(IAsyncResult result)
        {
			try
			{
				Socket clientSocket = serverSocket.EndAccept(result);

				ClientReference client = new ClientReference(clientSocket);
				if (!Config.instance.blockConnections)
				{
					if (HandleClientAccepted(client))
						clientSocket.BeginReceive(client.buffer, 0, client.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), client);
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
				int numBytes = client.socket.EndReceive(result);
				Msg4Recv recv = new Msg4Recv(client.buffer);
				recv._hdr.FromArray(recv.Buffer);
				MsgBody msgBody = recv.Flush();
				msgBody.Decrypt(recvKey);

				lock (dataLock)
				{
					if (numBytes <= 0)
						client.Disconnect(true);
					else
					{
						readQueue.Enqueue(new MsgReference(new Msg2Handle(recv.GetId(), msgBody), client, _channelRef: client.channel, _matchData: client.matchData));
						//HandleMessages();
					}
				}

				client.buffer = new byte[8192];
				if (numBytes > 0)
					client.socket.BeginReceive(client.buffer, 0, client.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), client);
			}

			catch(Exception ex)
			{
				Debug.LogError("ReceiveCallback: " + ex.Message);
			}
		}

		private void SendCallback(IAsyncResult result)
		{
			Socket clientSocket = (Socket)result.AsyncState;
			clientSocket.EndSend(result);
		}

		private void UnicastMessage(MsgReference msgRef)
		{
			Msg4Send data = new Msg4Send(msgRef.msg._id, uint.MaxValue, uint.MaxValue, msgRef.msg._msg, sendKey);
			msgRef.client.socket.BeginSend(data.Buffer, 0, data.Buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), msgRef.client.socket);
		}
		private void BroadcastMessage(MsgReference msgRef)
		{
			Msg4Send data = new Msg4Send(msgRef.msg._id, uint.MaxValue, uint.MaxValue, msgRef.msg._msg, sendKey);
			for (int i = 0; i < clientList.Count; i++)
			{
				clientList[i].socket.BeginSend(data.Buffer, 0, data.Buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), clientList[i].socket);
			}
		}

		private void BroadcastChannelMessage(MsgReference msgRef)
		{
			Msg4Send data = new Msg4Send(msgRef.msg._id, uint.MaxValue, uint.MaxValue, msgRef.msg._msg, sendKey);
			for (int i = 0; i < msgRef.channelRef.clientList.Count; i++)
			{
				msgRef.channelRef.clientList[i].socket.BeginSend(data.Buffer, 0, data.Buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), msgRef.channelRef.clientList[i].socket);
			}
		}

		private void BroadcastRoomMessage(MsgReference msgRef)
		{
			Msg4Send data = new Msg4Send(msgRef.msg._id, uint.MaxValue, uint.MaxValue, msgRef.msg._msg, sendKey);
			for (int i = 0; i < msgRef.matchData.clientList.Count; i++)
			{
				if (msgRef.matchData.clientList[i].clientStatus < ClientReference.ClientStatus.Room)
					continue;

				msgRef.matchData.clientList[i].socket.BeginSend(data.Buffer, 0, data.Buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), msgRef.matchData.clientList[i].socket);
			}
		}

		private void BroadcastRedTeamMessage(MsgReference msgRef)
		{
			Msg4Send data = new Msg4Send(msgRef.msg._id, uint.MaxValue, uint.MaxValue, msgRef.msg._msg, sendKey);
			for (int i = 0; i < clientList.Count; i++)
			{
				if (clientList[i].clientStatus < ClientReference.ClientStatus.Room || !clientList[i].slot.isRed)
					continue;

				clientList[i].socket.BeginSend(data.Buffer, 0, data.Buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), clientList[i].socket);
			}
		}

		private void BroadcastBlueTeamMessage(MsgReference msgRef)
		{
			Msg4Send data = new Msg4Send(msgRef.msg._id, uint.MaxValue, uint.MaxValue, msgRef.msg._msg, sendKey);
			for (int i = 0; i < clientList.Count; i++)
			{
				if (clientList[i].clientStatus < ClientReference.ClientStatus.Room || clientList[i].slot.isRed)
					continue;

				clientList[i].socket.BeginSend(data.Buffer, 0, data.Buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), clientList[i].socket);
			}
		}

		private void BroadcastRoomMessageExclusive(MsgReference msgRef)
		{
			Msg4Send data = new Msg4Send(msgRef.msg._id, uint.MaxValue, uint.MaxValue, msgRef.msg._msg, sendKey);
			for (int i = 0; i < msgRef.matchData.clientList.Count; i++)
			{
				if (msgRef.matchData.clientList[i].socket == msgRef.client.socket || msgRef.matchData.clientList[i].clientStatus < ClientReference.ClientStatus.Room)
					continue;

				msgRef.matchData.clientList[i].socket.BeginSend(data.Buffer, 0, data.Buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), msgRef.matchData.clientList[i].socket);
			}
		}

		private ClientReference FindClientBySocket(Socket clientSocket)
		{
			ClientReference client = clientList.Find(x => x.socket == clientSocket);
			if (client == null)
				Debug.LogError("FindClientBySocket: Could not find ClientReference for client: " + clientSocket.RemoteEndPoint.ToString());
			return client;

		}

		public void ShutdownInit()
		{
			channelManager.Shutdown();
			channelManager = new ChannelManager();
			//matchData = new MatchData();
			curSeq = 0;
			serverCreated = false;
			SendDisconnect(null, SendType.Broadcast);
			waitForShutDown = true;
		}

		public void ShutdownFinally()
		{
			lock (dataLock)
			{
				waitForShutDown = false;
				serverSocket.Shutdown(SocketShutdown.Both);
				serverSocket.Close();
				ClearBuffers();
				clientList.Clear();
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
				//SendMessages();
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
				if (waitForShutDown && clientList.Count == 0)
					ShutdownFinally();

				killLogTimer += Time.deltaTime;
				HandleDeadClients();
				HandleMessages();
				SendMessages();
			}
		}

		public void Reset()
		{
			ClearBuffers();
			channelManager.Shutdown();
			channelManager = new ChannelManager();
			//matchData.Reset();
			//matchData = new MatchData();
			foreach(ClientReference client in clientList)
			{
				SendChannels(client);
				SendKick(client);
				SendRoomList(client);
			}
			SendCustomMessage("Reset By Host");
		}

		private void HandleDeadClients()
		{
			if (!Config.instance.autoClearDeadClients)
				return;

			foreach (ClientReference client in clientList)
			{
				if (client.seq == -1)
				{
					client.toleranceTime += Time.deltaTime;
					if (client.toleranceTime >= 3f)
						client.Disconnect(false);
				}
			}
		}

		private void HandleMessages()
		{
			if (readQueue.Count < 1)
				return;

			MsgReference msgRef = readQueue.Peek();

			try
			{
				switch (msgRef.msg._id)
				{
					case 1:
						HandleLoginRequest(msgRef);
						break;

					case 3:
						HandleHeartbeat(msgRef);
						break;

					case 4:
						HandleRoomListRequest(msgRef);
						break;

					case 7:
						HandleCreateRoomRequest(msgRef);
						break;

					case 13:
						HandleAddBrickRequest(msgRef);
						break;

					case 15:
						HandleDelBrickRequest(msgRef);
						break;

                    case 19:
                        Debug.LogWarning("PaletteManagerRequest");
                        break;

					case 20:
						HandleCacheBrickRequest(msgRef);
						break;

					case 23:
						HandleLeave(msgRef);
						break;

					case 24:
						HandleChatRequest(msgRef);
						break;

					case 28:
						HandleJoinRequest(msgRef);
						break;

					case 32:
						HandleResumeRoomRequest(msgRef);
						break;

					case 35:
						HandleEquipRequest(msgRef);
						break;

                    case 37:
                        HandleUnequipRequest(msgRef);
                        break;

					case 42:
						HandleLoadComplete(msgRef);
						break;

					case 44:
						HandleKillLogRequest(msgRef);
						break;

					case 47:
						HandleSetStatusRequest(msgRef);
						break;

					case 49:
						HandleStartRequest(msgRef);
						break;

					case 51:
						HandleRegisterMapRequest(msgRef);
						break;

					case 63:
						HandleRespawnTicketRequest(msgRef);
						break;

					case 65:
						HandleTimer(msgRef);
						break;

					case 71:
						HandleMatchCountdown(msgRef);
						break;

					case 73:
						HandleBreakIntoRequest(msgRef);
						break;

					case 75:
						HandleTeamScoreRequest(msgRef);
						break;

					case 76:
						HandleDestroyBrickRequest(msgRef);
						break;

					case 80:
						HandleTeamChangeRequest(msgRef);
						break;

					case 85:
						HandleSlotLockRequest(msgRef);
						break;

					case 91:
						HandleRoomConfig(msgRef);
						break;

					case 93:
						HandleTeamChatRequest(msgRef);
						break;

					case 95:
						HandleRadioMsgRequest(msgRef);
						break;

                    case 121:
                        HandleBuyRequest(msgRef);
                        break;

					case 135:
						HandleP2PComplete(msgRef);
						break;

					case 137:
						HandleResultDoneRequest(msgRef);
						break;

					case 143:
						HandleRoamout(msgRef);
						break;

					case 145:
						HandleRoamin(msgRef);
						break;

					case 158:
						HandleGetCannonRequest(msgRef);
						break;

					case 160:
						HandleEmptyCannonRequest(msgRef);
						break;

                    case 262:
                        HandleGetBack2SpawnerRequest(msgRef);
                        break;

                    case 264:
                        HandleMatchRestartCountRequest(msgRef);
                        break;

                    case 266:
                        HandleMatchRestartRequest(msgRef);
                        break;

                    case 285:
                        HandlePickFlagRequest(msgRef);
                        break;

                    case 287:
                        HandleCaptureFlagRequest(msgRef);
                        break;

                    case 289: 
                        HandleDropFlagRequest(msgRef);
                        break;

                    case 295:
                        HandleCTFScoreRequest(msgRef);
                        break;

                    case 307:
                        HandleInitItemTermRequest(msgRef);
                        break;

                    case 324:
                        HandleBNDScoreRequest(msgRef);
                        break;

                    case 329:
                        msgRef.msg._msg.Read(out long item);
                        msgRef.msg._msg.Read(out string code);
                        Debug.LogWarning("UseConsumable Item: " + item + " code: " + code);
                        break;

					case 333:
						HandleSetShooterToolRequest(msgRef);
						break;

					case 334:
						HandleClearShooterTools(msgRef);
						break;

					case 337:
						HandleRegMapInfoRequest(msgRef);
						break;

                    case 345:
                        Debug.LogWarning("StackPointRequest");
                        break;

                    case 349:
                        HandleBNDShiftPhaseRequest(msgRef);
                        break;

                    case 366:
                        HandleFlagReturnRequest(msgRef);
                        break;

					case 368:
						HandleWeaponHeldRatioRequest(msgRef);
						break;

					case 389:
						HandleDelegateMasterRequest(msgRef);
						break;

                    case 399:
                        Debug.LogWarning("InflictedDamageRequest");
                        break;

                    case 414:
						HandleWeaponChangeRequest(msgRef);
						break;

					case 419:
						HandleSetWeaponSlotRequest(msgRef);
						break;

					case 420:
						HandleClearWeaponSlots(msgRef);
						break;

					case 425:
						HandleRequestDownloadedMaps(msgRef);
						break;

					case 427:
						HandleRequestRegisteredMaps(msgRef);
						break;

					case 429:
						HandleRequestUserMaps(msgRef);
						break;

                    case 431:
                        Debug.LogWarning("AllMapRequest:");
                        break;

					case 447:
						HandleOpenDoorRequest(msgRef);
						break;

					case 448:
						HandleCloseDoorRequest(msgRef);
						break;

                    case 460:
                        msgRef.msg._msg.Read(out int opt);
                        Debug.LogWarning("SaveCommonOpt: " + opt);
                        break;

					case 469:
						HandleRoomRequest(msgRef);
						break;

					case 478:
						HandleRequestUserList(msgRef);
						break;

					case 551:
						HandleGetTrainRequest(msgRef);
						break;

					case 553:
						HandleEmptyTrainRequest(msgRef);
						break;

					case ExtensionOpcodes.opInventoryAck:
						HandleInventoryCSV(msgRef);
						break;

					case ExtensionOpcodes.opDisconnectReq:
						HandleDisconnect(msgRef);
						break;

					case ExtensionOpcodes.opBeginChunkedBufferReq:
						HandleBeginChunkedBuffer(msgRef);
						break;

					case ExtensionOpcodes.opChunkedBufferReq:
						HandleChunkedBuffer(msgRef);
						break;

					case ExtensionOpcodes.opEndChunkedBufferReq:
						HandleEndChunkedBuffer(msgRef);
						break;

					default:
						if (debugHandle)
							Debug.LogWarning("Received unhandled message ID " + msgRef.msg._id + " from: " + msgRef.client.GetIdentifier());
						break;
				}
			}

			catch (Exception ex)
			{
				Debug.LogError("HandleMessages: " + ex.Message);
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
				if (!Config.instance.blockConnections && clientList.Count < Config.instance.maxConnections && !clientList.Exists(x => x.socket == client.socket) && (!Config.instance.oneClientPerIP || !clientList.Exists(x => x.ip == client.ip)))
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

		private void HandleHeartbeat(MsgReference msgRef)
		{
			msgRef.msg._msg.Read(out int gmFunction);
			if (Time.time - msgRef.client.lastHeartBeatTime > 3f)
				msgRef.client.Disconnect();

			else
				msgRef.client.lastHeartBeatTime = Time.time;
		}

		private void HandleLoginRequest(MsgReference msgRef)
		{
			msgRef.msg._msg.Read(out string id);
			msgRef.msg._msg.Read(out string pswd);
			msgRef.msg._msg.Read(out int major);
			msgRef.msg._msg.Read(out int minor);
			msgRef.msg._msg.Read(out string privateIpAddress);
			msgRef.msg._msg.Read(out string macAddress);

			msgRef.client.name = id;
			msgRef.client.seq = curSeq;
			msgRef.client.port = 6000 + msgRef.client.seq;
			curSeq++;

			ChannelReference channel = channelManager.GetDefaultChannel();
			channel.AddClient(msgRef.client);

			SendPlayerInitInfo(msgRef.client);
			SendChannels(msgRef.client);
			SendCurChannel(msgRef.client, channel.channel.Id);
			SendInventoryRequest(msgRef.client);
			SendLogin(msgRef.client, channel.channel.Id);
			SendPlayerInfo(msgRef.client);
			SendAllDownloadedMaps(msgRef.client);
			SendEmptyUserMap(msgRef.client);
			SendAllUserMaps(msgRef.client);
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
                Debug.LogWarning(matchData.repeat);
                if (matchData.repeat <= 0)
                {
                    matchData.EndMatch();
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
					SendRoom(null, matchData, SendType.BroadcastRoom);
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

			SendRegisteredMaps(msgRef.client, nextPage);
		}

		private void HandleRequestUserMaps(MsgReference msgRef)
		{
			msgRef.msg._msg.Read(out int page);

			if (debugHandle)
				Debug.Log("HandleRequestUserMaps from: " + msgRef.client.GetIdentifier());

			SendUserMaps(msgRef.client, page);
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
				SendRendezvousInfo(msgRef.client);
				SendMaster(msgRef.client, matchData);
				SendSlotLocks(msgRef.client);
				SendRoomConfig(msgRef.client, matchData);
				SendAddRoom(msgRef.client, matchData);
				SendEnter(msgRef.client);
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
				msgRef.client.isBreakingInto = true;
			}

			SendBreakInto(msgRef.client, reply);
		}

		private void HandleLeave(MsgReference msgRef)
		{
			MatchData matchData = msgRef.matchData;

			if (debugHandle)
				Debug.Log("HandleLeave from: " + msgRef.client.GetIdentifier());

			SendLeave(msgRef.client);
			SendSetStatus(msgRef.client);

			matchData.RemoveClient(msgRef.client);

			if (matchData.room.CurPlayer <= 0)
			{
				SendDeleteRoom(matchData, matchData.channel);
				msgRef.client.channel.RemoveMatch(matchData);
				//matchData = new MatchData();
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
			msgRef.msg._msg.Read(out int type);
			msgRef.msg._msg.Read(out string title);
			msgRef.msg._msg.Read(out bool isLocked);
			msgRef.msg._msg.Read(out string pswd);
			msgRef.msg._msg.Read(out int maxPlayer);
			msgRef.msg._msg.Read(out int param1);	//Play: goal			Build: isLoad
			msgRef.msg._msg.Read(out int param2);	//Play: timeLimit		Build: slot
			msgRef.msg._msg.Read(out int param3);	//Play: weaponOption	Build: brickCount:landscapeIndex
			msgRef.msg._msg.Read(out int param4);	//Play: map				Build: map:skyboxIndex
			msgRef.msg._msg.Read(out int param5);	//Play: breakInto		Build: premium
			msgRef.msg._msg.Read(out int param6);	//Play: isBalance		Build: N/A
			msgRef.msg._msg.Read(out int param7);	//Play: isWanted		Build: N/A
			msgRef.msg._msg.Read(out int param8);	//Play: isDrop			Build: N/A
			msgRef.msg._msg.Read(out string alias);
			msgRef.msg._msg.Read(out int master);

			MatchData matchData = msgRef.client.channel.AddNewMatch();

			matchData.room.Type = (Room.ROOM_TYPE)type;
			matchData.room.Title = title;
			matchData.room.Locked = isLocked;
			matchData.room.MaxPlayer = maxPlayer;
			matchData.room.CurMapAlias = alias;
			matchData.masterSeq = msgRef.client.seq;
			matchData.LockSlotsByMaxPlayers(matchData.room.MaxPlayer);
			matchData.roomCreated = true;

			if ((Room.ROOM_TYPE)type == Room.ROOM_TYPE.MAP_EDITOR)
			{
				if (param1 == 1)
					matchData.CacheMap(regMaps.Find(x => x.Value.Map == param2).Value, new UserMapInfo(param2, (sbyte)param5));
				else
					matchData.CacheMapGenerate(param3, param4, alias);
			}

			else
			{
				matchData.room.goal = param1;
				matchData.room.timelimit = param2;
				matchData.room.weaponOption = param3;
				matchData.room.map = param4;
				matchData.room.isBreakInto = Convert.ToBoolean(param5);
				matchData.room.isWanted = Convert.ToBoolean(param7);
				matchData.room.isDropItem = false;//Convert.ToBoolean(param8);
				matchData.isBalance = Convert.ToBoolean(param6);
			}

			if (debugHandle)
				Debug.Log("HandleCreateRoom from: " + msgRef.client.GetIdentifier());

			matchData.AddClient(msgRef.client);
			SendRendezvousInfo(msgRef.client);
			SendMaster(msgRef.client, matchData);
			SendSlotLocks(msgRef.client);
			SendRoomConfig(msgRef.client, matchData);
			SendAddRoom(msgRef.client, matchData);
			SendCreateRoom(msgRef.client);
			SendEnter(msgRef.client);
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
			matchData.room.isDropItem = false;//Convert.ToBoolean(itemPickup);
			matchData.room.CurMapAlias = whereAlias;
			matchData.room.Type = (Room.ROOM_TYPE)type;

			if (debugHandle)
				Debug.Log("HandleRoomConfig from: " + msgRef.client.GetIdentifier());

			SendRoomConfig(null, matchData);
		}

		private void HandleRoomRequest(MsgReference msgRef)
		{

			msgRef.msg._msg.Read(out int roomNumber);

			if (debugHandle)
				Debug.Log("HandleRoomRequest from: " + msgRef.client.GetIdentifier());

			MatchData matchData = msgRef.client.channel.GetMatchByRoomNumber(roomNumber);
			SendRoom(msgRef.client, matchData);
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

			SendRoom(null, matchData, SendType.BroadcastRoom);
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
			SendRoom(null, matchData, SendType.BroadcastRoom);

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
            int durability = int.MaxValue; //Durability int.MaxValue = Permanent
            MsgBody body = new MsgBody();

            TItem template = TItemManager.Instance.dic.FirstOrDefault(x => x.Value.code == code).Value;

            int seqSeed = msgRef.client.seq + 1;
            byte[] baseSeq = new byte[8];
            byte[] seed = System.Text.Encoding.UTF8.GetBytes(template.name);
            byte[] codeSeed = System.Text.Encoding.UTF8.GetBytes(template.code);
            for (int i = 0; i < seed.Length && i < 5; i++)
                baseSeq[i] = (byte)(seed[i] ^ seed[seed.Length - 1 - i]);

            for (int i = 0; i < 3; i++)
                baseSeq[i] ^= codeSeed[i];

            long itemSeq = BitConverter.ToInt64(baseSeq, 0) * seqSeed;

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
                    }
                    else
                    {
                        itemSeq = -3;
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
                    }
                    else
                    {
                        itemSeq = -3;
                    }
                    break;
                default:
                    break;
            }

            msgRef.client.inventory.AddItem(template);
            body.Write(itemSeq);
            body.Write(code);
            body.Write(remain);
            body.Write(premium);
            body.Write(durability);
            Say(new MsgReference(122, body, msgRef.client, SendType.Unicast));
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

			if (debugHandle)
				Debug.Log("HandleKillLogRequest from: " + msgRef.client.GetIdentifier());

			ClientReference victimClient = matchData.clientList.Find(x => x.seq == victim);
			victimClient.deaths++;
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
							HandleTeamMatchEnd(matchData);
						}
						break;

					case Room.ROOM_TYPE.INDIVIDUAL:
						matchData.redScore++;
						SendIndividualScore(matchData);

						if (matchData.redScore >= matchData.room.goal)
						{
							HandleIndividualMatchEnd(matchData);
						}
						break;

                    case Room.ROOM_TYPE.CAPTURE_THE_FLAG:
                        if (victimClient.slot.slotIndex > 7)
                            matchData.redScore++;
                        else
                            matchData.blueScore++;
                        SendTeamScore(matchData);
                        if (matchData.blueScore >= matchData.room.goal || matchData.redScore >= matchData.room.goal)
                        {
                            HandleCTFMatchEnd(matchData);
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

		private void HandleInventoryCSV(MsgReference msgRef)
		{
			List<string[]> rows = new List<string[]>();

			msgRef.msg._msg.Read(out int rowCount);
			for (int row = 0; row < rowCount; row++)
			{
				msgRef.msg._msg.Read(out int colCount);
				string[] rowData = new string[colCount];
				for (int col = 0; col < colCount; col++)
				{
					msgRef.msg._msg.Read(out string entry);
					rowData[col] = entry;
				}
				rows.Add(rowData);
			}

			msgRef.client.inventory = new Inventory(msgRef.client.seq, new CSVLoader(rows));

			if (debugHandle)
				Debug.Log("HandleInventoryCSV from: " + msgRef.client.GetIdentifier());

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
						SendUnequip(msgRef.client, oldItem.Seq, oldItem.Code, matchData);
					}
				}
				item.Usage = Item.USAGE.EQUIP;
				msgRef.client.inventory.GenerateActiveSlots();

				SendEquip(msgRef.client, item.Seq, item.Code, matchData);
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
                        SendUnequip(msgRef.client, currentItem.Seq, currentItem.Code, matchData);
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

			msgRef.client.inventory.GenerateActiveTools();
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

		public void HandleIndividualMatchEnd(MatchData matchData)
		{
			matchData.room.Status = Room.ROOM_STATUS.WAITING;
			SendIndividualMatchEnd(matchData);
			matchData.Reset();
			SendRoom(null, matchData, SendType.BroadcastRoom);
		}

		public void HandleTeamMatchEnd(MatchData matchData)
		{
			matchData.room.Status = Room.ROOM_STATUS.WAITING;
			SendTeamMatchEnd(matchData);
			matchData.Reset();
			SendRoom(null, matchData, SendType.BroadcastRoom);
		}

        public void HandleCTFMatchEnd(MatchData matchData)
        {
            matchData.room.Status = Room.ROOM_STATUS.WAITING;
            SendCTFMatchEnd(matchData);
            matchData.Reset();
            SendRoom(null, matchData, SendType.BroadcastRoom);
        }

        public void HandleBNDMatchEnd(MatchData matchData)
        {
            matchData.room.Status = Room.ROOM_STATUS.WAITING;
            SendBNDMatchEnd(matchData);
            matchData.Reset();
            SendRoom(null, matchData, SendType.BroadcastRoom);
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

			SendCacheBrick(msgRef.client);
			SendCacheBrickDone(msgRef.client);
		}

		private void HandleAddBrickRequest(MsgReference msgRef)
		{
			MatchData matchData = msgRef.matchData;

			if (debugHandle)
				Debug.Log("HandleAddBrickRequest from: " + msgRef.client.GetIdentifier());

			msgRef.msg._msg.Read(out byte brick);
			msgRef.msg._msg.Read(out byte x);
			msgRef.msg._msg.Read(out byte y);
			msgRef.msg._msg.Read(out byte z);
			msgRef.msg._msg.Read(out byte rot);

			int seq = matchData.GetNextBrickSeq();
			List<int> morphes = new List<int>();
			BrickInst brickInst = matchData.cachedMap.AddBrickInst(seq, brick, x, y, z, 0, rot);
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

			msgRef.msg._msg.Read(out int map);
			msgRef.msg._msg.Read(out ushort modeMask);
			msgRef.msg._msg.Read(out int regHow);
			msgRef.msg._msg.Read(out int point);
			msgRef.msg._msg.Read(out int downloadFee);
			msgRef.msg._msg.Read(out string msgEval);

			if (map == matchData.cachedMap.map)
			{
				Texture2D thumbnail = new Texture2D(1, 1);
				if (msgRef.client.chunkedBuffer.id == ExtensionOpcodes.opChunkedBufferThumbnailReq)
				{
					if (msgRef.client.chunkedBuffer.finished)
					{
						thumbnail.LoadImage(msgRef.client.chunkedBuffer.buffer);
						thumbnail.Apply();
						Debug.Log("Load Thumbnail");
					}
					else
						Debug.LogError("HandleRegisterMapRequest: ChunkedBuffer not finished");
				}

				DateTime time = DateTime.Now;
				int hashId = MapGenerator.instance.GetHashIdForTime(time);
				RegMap regMap = new RegMap(hashId, msgRef.client.name + "@Aurora", matchData.cachedUMI.Alias, time, modeMask, true, false, 0, 0, 0, 0, 0, 0, 0, false);
				regMap.Thumbnail = thumbnail;
				matchData.cachedMap.map = hashId;
				matchData.cachedUMI.regMap = regMap;
				matchData.cachedUMI.slot = hashId;

				matchData.cachedUMI.regMap.Save();
				matchData.cachedMap.Save(hashId, matchData.cachedMap.skybox);

				SendCustomMessage("Saved " + matchData.cachedUMI.regMap.Alias + " with ID " + hashId);
			}

			msgRef.client.chunkedBuffer = null;
		}

        public void HandleBNDScoreRequest(MsgReference msgRef)
        {
            Debug.LogWarning("ScoreRequest");
        }

        public void HandleBNDShiftPhaseRequest(MsgReference msgRef)
        {
            Debug.LogWarning("recieved Shift Phase Req");
            msgRef.msg._msg.Read(out int repeat);
            MatchData matchData = msgRef.client.matchData;
            matchData.repeat = repeat;
            msgRef.msg._msg.Read(out bool isBuildPhase);
            SendShiftPhase(msgRef.client, 1, isBuildPhase);
        }

        public void SendShiftPhase(ClientReference client, int repeat, bool isBuildPhase)
        {
            MatchData matchData = client.matchData;
            matchData.ResetForNewRound();

            MsgBody body = new MsgBody();

            body.Write(repeat);
            body.Write(isBuildPhase);

            Say(new MsgReference(344, body, client, SendType.BroadcastRoom, matchData.channel, matchData));
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

		public void SendCacheBrick(ClientReference client)
		{
			MatchData matchData = client.matchData;

			List<KeyValuePair<int, BrickInst>> brickList = new List<KeyValuePair<int, BrickInst>>();
			brickList = matchData.cachedMap.dic.ToList();

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
						body.Write((byte)0);
				}

				Say(new MsgReference(21, body, client, SendType.Unicast));
			}

			if (debugSend)
				Debug.Log("SendCacheBrick with " + chunkCount + " chunks to: " + client.GetIdentifier());
		}

		public void SendCacheBrickDone(ClientReference client)
		{
			MatchData matchData = client.matchData;

			MsgBody body = new MsgBody();

			body.Write(0); //mapIndex
			body.Write(matchData.cachedMap.skybox);

			Say(new MsgReference(22, body, client, SendType.Unicast));

			if (debugSend)
				Debug.Log("SendCacheBrickDone for map " + matchData.cachedMap.map + " to: " + client.GetIdentifier());
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

		public void SendDisconnect(ClientReference client, SendType sendType = SendType.Unicast)
		{
			MsgBody body = new MsgBody();

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
			SendItemList(client);
			SendShooterToolList(client);
			SendWeaponSlotList(client);
			SendItemProperties(client);
			SendItemPimps(client);
			SendPremiumItems(client);
		}
		public void SendIndividualMatchEnd(MatchData matchData)
		{
			MsgBody body = new MsgBody();

			body.Write(matchData.clientList.Count);
			for (int i = 0; i < matchData.clientList.Count; i++)
			{
				body.Write(matchData.clientList[i].slot.isRed);
				body.Write(matchData.clientList[i].seq);
				body.Write(matchData.clientList[i].name);
				body.Write(matchData.clientList[i].kills);
				body.Write(matchData.clientList[i].deaths);
				body.Write(matchData.clientList[i].assists);
				body.Write(matchData.clientList[i].score);
				body.Write(0); //points
				body.Write(0); //xp
				body.Write(0); //mission
				body.Write(matchData.clientList[i].data.xp);
				body.Write(matchData.clientList[i].data.xp);
				body.Write((long)0); //buff
			}
			Say(new MsgReference(180, body, null, SendType.BroadcastRoom, matchData.channel, matchData));

			if (debugSend)
				Debug.Log("Broadcasted SendIndivudalMatchEnd for room no: " + matchData.room.No);
		}

		public void SendTeamMatchEnd(MatchData matchData)
		{
			for (int team = 0; team < 2; team++)
			{
				MsgBody body = new MsgBody();

				body.Write(team == 0 ? matchData.GetWinningTeam() : (sbyte)-matchData.GetWinningTeam());
				body.Write(matchData.redScore);
				body.Write(matchData.blueScore);
				body.Write(matchData.blueScore);
				body.Write(matchData.redScore);
				body.Write(matchData.clientList.Count);
				for (int i = 0; i < matchData.clientList.Count; i++)
				{
					body.Write(matchData.clientList[i].slot.isRed);
					body.Write(matchData.clientList[i].seq);
					body.Write(matchData.clientList[i].name);
					body.Write(matchData.clientList[i].kills);
					body.Write(matchData.clientList[i].deaths);
					body.Write(matchData.clientList[i].assists);
					body.Write(matchData.clientList[i].score);
					body.Write(0); //points
					body.Write(0); //xp
					body.Write(0); //mission
					body.Write(matchData.clientList[i].data.xp);
					body.Write(matchData.clientList[i].data.xp);
					body.Write((long)0); //buff
				}
				Say(new MsgReference(70, body, null, team == 0 ? SendType.BroadcastBlueTeam : SendType.BroadcastRedTeam));
			}

			if (debugSend)
				Debug.Log("Broadcasted SendTeamMatchEnd for room no: " + matchData.room.No);
		}

        public void SendCTFMatchEnd(MatchData matchData)
        {
            for (int team = 0; team < 2; team++)
            {
                MsgBody body = new MsgBody();

                body.Write(team == 0 ? matchData.GetWinningTeam() : (sbyte)-matchData.GetWinningTeam());
                body.Write(matchData.redCaptures); //RedScore
                body.Write(matchData.blueCaptures); //BlueScore
                body.Write(matchData.blueScore); //RedTotalKill
                body.Write(matchData.redScore); //BluTotalKill
                body.Write(matchData.redScore); //RedTotalDeath
                body.Write(matchData.blueScore); //BlueTotalDeath
                body.Write(matchData.clientList.Count);
                for (int i = 0; i < matchData.clientList.Count; i++)
                {
                    body.Write(matchData.clientList[i].slot.isRed);
                    body.Write(matchData.clientList[i].seq);
                    body.Write(matchData.clientList[i].name);
                    body.Write(matchData.clientList[i].kills);
                    body.Write(matchData.clientList[i].deaths);
                    body.Write(matchData.clientList[i].assists);
                    body.Write(matchData.clientList[i].score);
                    body.Write(0); //points
                    body.Write(0); //xp
                    body.Write(0); //mission
                    body.Write(matchData.clientList[i].data.xp);
                    body.Write(matchData.clientList[i].data.xp);
                    body.Write((long)0); //buff
                }
                Say(new MsgReference(292, body, null, team == 0 ? SendType.BroadcastBlueTeam : SendType.BroadcastRedTeam));
            }

            if (debugSend)
                Debug.Log("Broadcasted SendCTFMatchEnd for room no: " + matchData.room.No);
        }

        public void SendBNDMatchEnd(MatchData matchData)
        {
            for (int team = 0; team < 2; team++)
            {
                MsgBody body = new MsgBody();

                body.Write(team == 0 ? matchData.GetWinningTeam() : (sbyte)-matchData.GetWinningTeam());
                body.Write(matchData.redCaptures); //RedScore
                body.Write(matchData.blueCaptures); //BlueScore
                body.Write(matchData.blueScore); //RedTotalKill
                body.Write(matchData.redScore); //BluTotalKill
                body.Write(matchData.redScore); //RedTotalDeath
                body.Write(matchData.blueScore); //BlueTotalDeath
                body.Write(matchData.clientList.Count);
                for (int i = 0; i < matchData.clientList.Count; i++)
                {
                    body.Write(matchData.clientList[i].slot.isRed);
                    body.Write(matchData.clientList[i].seq);
                    body.Write(matchData.clientList[i].name);
                    body.Write(matchData.clientList[i].kills);
                    body.Write(matchData.clientList[i].deaths);
                    body.Write(matchData.clientList[i].assists);
                    body.Write(matchData.clientList[i].score);
                    body.Write(0); //points
                    body.Write(0); //xp
                    body.Write(0); //mission
                    body.Write(matchData.clientList[i].data.xp);
                    body.Write(matchData.clientList[i].data.xp);
                    body.Write((long)0); //buff
                }
                Say(new MsgReference(338, body, null, team == 0 ? SendType.BroadcastBlueTeam : SendType.BroadcastRedTeam));
            }

            if (debugSend)
                Debug.Log("Broadcasted SendBNDMatchEnd for room no: " + matchData.room.No);
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
				SendItemPimp(client, weapons[i], PIMP.PROP_RPM, weapons[i].Template.upgradeCategory == TItem.UPGRADE_CATEGORY.HAND_GUN ? 10 : 10);
				SendItemPimp(client, weapons[i], PIMP.PROP_AMMO_MAX, 10);
				SendItemPimp(client, weapons[i], PIMP.PROP_ATTACK_SPEED, 10);
			}
		}

		public void SendItemPimp(ClientReference client, Item item, PIMP pimp, int grade)
		{
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

		public void SendEquip(ClientReference client, long itemSeq, string code, MatchData matchData)
		{
			MsgBody body = new MsgBody();

			body.Write(client.seq);
			body.Write(itemSeq);
			body.Write(code);

			Say(new MsgReference(36, body, client, SendType.Broadcast));

			if (debugSend)
				Debug.Log("Broadcasted SendEquip for client " + client.GetIdentifier() + " for room no: " + matchData.room.No);
		}

		public void SendUnequip(ClientReference client, long itemSeq, string code, MatchData matchData)
		{
			MsgBody body = new MsgBody();

			body.Write(client.seq);
			body.Write(itemSeq);
			body.Write(code);

			Say(new MsgReference(38, body, client, SendType.Broadcast));

			if (debugSend)
				Debug.Log("Broadcasted SendUnequip for client " + client.GetIdentifier() + " for room no: " + matchData.room.No);
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
				Debug.Log("Broadcasted SendDestroyBrick for brick " + brick+ " for room no: " + matchData.room.No);
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

		public void SendRoomConfig(ClientReference client, MatchData matchData)
		{
			MsgBody body = new MsgBody();

			body.Write(matchData.room.map);
			body.Write(matchData.room.CurMapAlias);
			body.Write(matchData.room.weaponOption);
			body.Write(matchData.room.timelimit);
			body.Write(matchData.room.goal);
			body.Write(matchData.room.isBreakInto);
			body.Write(matchData.isBalance);
			body.Write(false); //useBuildGun
			body.Write(""); //password
			body.Write((byte)0); //commented
			body.Write((int)matchData.room.Type);
			body.Write(matchData.room.isDropItem);
			body.Write(matchData.room.isWanted);

			if (client == null)
			{
				Say(new MsgReference(92, body, null, SendType.BroadcastRoom, matchData.channel, matchData));

				if (debugSend)
					Debug.Log("Broadcasted SendRoomConfig for room no: " + matchData.room.No);
			}

			else
			{
				Say(new MsgReference(92, body, client));

				if (debugSend)
					Debug.Log("SendRoomConfig to: " + client.GetIdentifier());
			}
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

			if (client == null)
			{
				Say(new MsgReference(5, body, null, SendType.BroadcastRoom, matchData.channel, matchData));

				if (debugSend)
					Debug.Log("Broadcasted SendAddRoom for room no: " + matchData.room.No);
			}

			else
			{
				Say(new MsgReference(5, body, client));

				if (debugSend)
					Debug.Log("SendAddRoom to: " + client.GetIdentifier());
			}
		}

		public void SendRoom(ClientReference client, MatchData matchData, SendType sendType = SendType.Unicast)
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

			Say(new MsgReference(470, body, client, sendType, matchData.channel, matchData));

			if (debugSend)
			{
				if (sendType == SendType.Unicast)
					Debug.Log("SendRoom to: " + client.GetIdentifier());

				else
					Debug.Log("Broadcasted SendRoom for room no: " + matchData.room.No);
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

			/*body.Write(client.inventory.equipment.Count);
			for (int i = 0; i < client.inventory.equipment.Count; i++)
			{
				body.Write(client.inventory.equipment[i].Seq);
				body.Write(client.inventory.equipment[i].Code);
				body.Write((sbyte)client.inventory.equipment[i].Usage);
				body.Write(client.inventory.equipment[i].Amount);
				body.Write(client.inventory.equipment[i].IsPremium);
				body.Write(client.inventory.equipment[i].Durability);
			}*/

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
			MsgBody body = new MsgBody();

			int chunkSize = 200;
			int chunkCount = Mathf.CeilToInt((float)regMaps.Count / (float)chunkSize);
			int processedCount = 0;

			for (int chunk = 0; chunk < chunkCount; chunk++)
			{
				int remaining = regMaps.Count - processedCount;
				if (remaining < chunkSize)
					chunkSize = remaining;

				body.Write(-1); //page
				body.Write(chunkSize); //count
				for (int i = 0; i < chunkSize; i++, processedCount++)
				{
					KeyValuePair<int, RegMap> entry = regMaps[processedCount];
					body.Write(entry.Value.Map); //slot
					body.Write(entry.Value.Alias);
					body.Write(-1); //brick count
					body.Write(entry.Value.RegisteredDate.Year);
					body.Write((sbyte)entry.Value.RegisteredDate.Month);
					body.Write((sbyte)entry.Value.RegisteredDate.Day);
					body.Write((sbyte)entry.Value.RegisteredDate.Hour);
					body.Write((sbyte)entry.Value.RegisteredDate.Minute);
					body.Write((sbyte)entry.Value.RegisteredDate.Second);
					body.Write((sbyte)0);
				}
				Say(new MsgReference(430, body, client));
			}

			if (debugSend)
				Debug.Log("SendAllUserMaps to: " + client.GetIdentifier());
		}

		public void SendEmptyUserMap(ClientReference client)
		{
			MsgBody body = new MsgBody();

			body.Write(-1); //page
			body.Write(1); //count
			body.Write(0); //slot
			body.Write("");
			body.Write(-1); //brick count
			body.Write(2000);
			body.Write((sbyte)0);
			body.Write((sbyte)0);
			body.Write((sbyte)0);
			body.Write((sbyte)0);
			body.Write((sbyte)0);
			body.Write((sbyte)0);

			Say(new MsgReference(430, body, client));

			if (debugSend)
				Debug.Log("SendEmptyUserMap to: " + client.GetIdentifier());
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
				body.Write(entry.Value.Map); //slot
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

		private void HandleBeginChunkedBuffer(MsgReference msgRef)
		{
			msgRef.msg._msg.Read(out ushort opcode);
			msgRef.msg._msg.Read(out int size);
			msgRef.msg._msg.Read(out uint crc);

			if (debugHandle)
				Debug.Log("HandleBeginChunkedBuffer from: " + msgRef.client.GetIdentifier());

			const int maxBufferSize = 1000000000;
			if (size > maxBufferSize)
			{
				Debug.LogWarning("ServerEmulator.HandleBeginChunkedBuffer: Buffer was " + size + " bytes");
				return;
			}

			if (msgRef.client.chunkedBuffer == null)
			{
				msgRef.client.chunkedBuffer = new ChunkedBuffer((uint)size, crc, opcode);
			}

			else
			{
				Debug.LogWarning("ServerEmulator.HandleBeginChunkedBuffer: ChunkedBuffer with id " + msgRef.client.chunkedBuffer.id + " is already assigned");
				return;
			}
		}

		private void HandleChunkedBuffer(MsgReference msgRef)
		{
			msgRef.msg._msg.Read(out ushort opcode);
			msgRef.msg._msg.Read(out int chunk);
			msgRef.msg._msg.Read(out byte[] next);

			if (msgRef.client.chunkedBuffer != null && msgRef.client.chunkedBuffer.id == opcode)
			{
				msgRef.client.chunkedBuffer.WriteNext(next, chunk);
			}
		}

		private void HandleEndChunkedBuffer(MsgReference msgRef)
		{

			if (debugHandle)
				Debug.Log("HandleEndChunkedBuffer from: " + msgRef.client.GetIdentifier());

			msgRef.msg._msg.Read(out ushort opcode);

			if (msgRef.client.chunkedBuffer != null)
			{
				msgRef.client.chunkedBuffer.finished = true;
				uint crc = CRC32.computeUnsigned(msgRef.client.chunkedBuffer.buffer);
				if (crc != (msgRef.client.chunkedBuffer.crc))
				{
					Debug.LogError("HandleEndChunkedBuffer: crc mismatch");
				}

				File.WriteAllBytes("test.png", msgRef.client.chunkedBuffer.buffer);
			}
		}

        private void HandlePickFlagRequest(MsgReference msgRef)
        {
            MatchData data = msgRef.matchData;
            msgRef.msg._msg.Read(out int flag);
            if (debugHandle)
                Debug.Log("HandlePickFlag from: " + msgRef.client.GetIdentifier() + " FlagId: " + flag);
            MsgBody msg = new MsgBody();
            // i think the response needs to be the plaxer seq which picked up the flag

            msg.Write(msgRef.client.seq);
            Say(new MsgReference(286, msg, msgRef.client, SendType.BroadcastRoom, data.channel, data));
        }

        private void HandleDropFlagRequest(MsgReference msgRef)
        {
            MatchData data = msgRef.matchData;
            msgRef.msg._msg.Read(out int x);
            msgRef.msg._msg.Read(out int y);
            msgRef.msg._msg.Read(out int z);
            if (debugHandle)
                Debug.Log("HandleFlagDrop from: " + msgRef.client.GetIdentifier() + " Flag: x: " + x + " y: " + y + " z: " + z);
            MsgBody msg = new MsgBody();
            msg.Write(0); //unused
            msg.Write(0); // unused
            msg.Write(x);
            msg.Write(y);
            msg.Write(z);

            Say(new MsgReference(290, msg, msgRef.client, SendType.BroadcastRoom, data.channel, data));
        }

        private void HandleCaptureFlagRequest(MsgReference msgRef)
        {
            MatchData data = msgRef.matchData;
            msgRef.msg._msg.Read(out int flag);
            msgRef.msg._msg.Read(out bool opponent);
            if (msgRef.client.slot.slotIndex > 7)
            {
                data.redCaptures++;
            } else
            {
                data.blueCaptures++;
            }
            if (debugHandle)
                Debug.Log("HandleCaptureFlag from: " + msgRef.client.GetIdentifier() + " FlagId: " + flag + " IsOpponent: " + opponent);
            MsgBody msg = new MsgBody();
            msg.Write(msgRef.client.seq); // Player sequence
            //Score is not counted
            //Round only starts for one player (oponent?)

            Say(new MsgReference(288, msg, msgRef.client, SendType.BroadcastRoom, data.channel, data));
        }

        private void HandleCTFScoreRequest(MsgReference msgRef)
        {
            MatchData data = msgRef.matchData;
            MsgBody msg = new MsgBody();
            msg.Write(data.redScore); // red score
            msg.Write(data.blueScore); // blue score
            if (debugHandle)
                Debug.Log("HandleCTFScoreReq from: " + msgRef.client.GetIdentifier() + " RedScore: " + data.redScore + " BluScore: " + data.blueScore);

            Say(new MsgReference(296, msg, msgRef.client, SendType.BroadcastRoom, data.channel, data));
        }

        private void HandleFlagReturnRequest(MsgReference msgRef)
        {
            MatchData data = msgRef.matchData;
            msgRef.msg._msg.Read(out float x);
            msgRef.msg._msg.Read(out float y);
            msgRef.msg._msg.Read(out float z);
            if (debugHandle)
                Debug.Log("HandleFlagReturn from: " + msgRef.client.GetIdentifier() + " Flag: x: " + x + " y: " + y + " z: " + z);
            MsgBody msg = new MsgBody();

            Say(new MsgReference(367, msg, msgRef.client, SendType.BroadcastRoom, data.channel, data));
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
    }
}
