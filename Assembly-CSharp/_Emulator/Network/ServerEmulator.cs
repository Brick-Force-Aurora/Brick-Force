using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace _Emulator
{
    class ServerEmulator : MonoBehaviour
    {
		public static ServerEmulator instance;
		private readonly object dataLock = new object();

		public List<ClientReference> clientList = new List<ClientReference>();
        private Socket serverSocket;
		private byte[] buffer = new byte[8192];
		private byte recvKey = byte.MaxValue;
		private byte sendKey = byte.MaxValue;
		private Queue<MsgReference> readQueue = new Queue<MsgReference>();
		private Queue<MsgReference> writeQueue = new Queue<MsgReference>();
		private int curSeq = 0;
		public bool debugHandle = false;
		public bool debugSend = false;
		public bool debugPing = false;
		public bool serverCreated = false;
		public MatchData matchData;
		private Channel defaultChannel;
		private float killLogTimer = 0f;
		private List<KeyValuePair<int, RegMap>> regMaps = new List<KeyValuePair<int, RegMap>>();
		private bool waitForShutDown = false;

		private void Start()
		{
			BuildOption.Instance.Props.UseP2pHolePunching = true;
			matchData = new MatchData();
			defaultChannel = new Channel(_id: 1, _mode: 2, _name: "Default", _ip: "", _port: 5000, _userCount: 1, _maxUserCount: 16, _country: 1, _minLvRank: 0, _maxLvRank: 66, _xpBonus: 0, _fpBonus: 0, _limitStarRate: 0);
		}

		public void SetupServer()
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
			regMaps = RegMapManager.Instance.dicRegMap.ToList();
			Debug.Log("Server created");
		}

		private void AcceptCallback(IAsyncResult result)
        {
			Socket clientSocket = serverSocket.EndAccept(result);
			clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), clientSocket);
			HandleClientAccepted(new ClientReference(clientSocket));
            serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

		private void ReceiveCallback(IAsyncResult result)
		{
			Socket clientSocket = (Socket)result.AsyncState;
			int numBytes = clientSocket.EndReceive(result);
			Msg4Recv recv = new Msg4Recv(buffer);
			recv._hdr.FromArray(recv.Buffer);
			MsgBody msgBody = recv.Flush();
			msgBody.Decrypt(recvKey);

			lock (dataLock)
			{
				ClientReference client = FindClientBySocket(clientSocket);
				if (numBytes <= 0)
					client.Disconnect(true);
				else
					readQueue.Enqueue(new MsgReference(new Msg2Handle(recv.GetId(), msgBody), client));
			}

			clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), clientSocket);
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

		private void BroadcastRoomMessage(MsgReference msgRef)
		{
			Msg4Send data = new Msg4Send(msgRef.msg._id, uint.MaxValue, uint.MaxValue, msgRef.msg._msg, sendKey);
			for (int i = 0; i < clientList.Count; i++)
			{
				if (clientList[i].clientStatus < ClientReference.ClientStatus.Room)
					continue;

				clientList[i].socket.BeginSend(data.Buffer, 0, data.Buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), clientList[i].socket);
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
			for (int i = 0; i < clientList.Count; i++)
			{
				if (clientList[i].socket == msgRef.client.socket || clientList[i].clientStatus < ClientReference.ClientStatus.Room)
					continue;

				clientList[i].socket.BeginSend(data.Buffer, 0, data.Buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), clientList[i].socket);
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
			matchData = new MatchData();
			curSeq = 0;
			serverCreated = false;
			SendDisconnect(null, SendType.Broadcast);
			waitForShutDown = true;
		}

		public void ShutdownFinally()
		{
			waitForShutDown = false;
			serverSocket.Shutdown(SocketShutdown.Both);
			serverSocket.Close();
			ClearBuffers();
			clientList.Clear();
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
			writeQueue.Enqueue(msg);
			SendMessages();
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
				HandleMessages();
				SendMessages();
			}
		}

		private void HandleMessages()
		{
			if (readQueue.Count < 1)
				return;

			MsgReference msgRef = readQueue.Peek();
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

				case 135:
					HandleP2PComplete(msgRef);
					break;

				case 137:
					HandleResultDoneRequest(msgRef);
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

				case 333:
					HandleSetShooterToolRequest(msgRef);
					break;

				case 334:
					HandleClearShooterTools(msgRef);
					break;

				case 337:
					HandleRegMapInfoRequest(msgRef);
					break;

				case 368:
					HandleWeaponHeldRatioRequest(msgRef);
					break;

				case 389:
					HandleDelegateMasterRequest(msgRef);
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

				case 447:
					HandleOpenDoorRequest(msgRef);
					break;

				case 448:
					HandleCloseDoorRequest(msgRef);
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

				default:
					if (debugHandle)
						Debug.LogWarning("Received unhandled message ID " + msgRef.msg._id + " from: " + msgRef.client.GetIdentifier());
					break;
			}

			readQueue.Dequeue();
		}

		private void SendMessages()
		{
			if (writeQueue.Count < 1)
				return;

			MsgReference msgRef = writeQueue.Peek();
			switch (msgRef.sendType)
			{
				case SendType.Unicast:
					UnicastMessage(msgRef);
					break;

				case SendType.Broadcast:
					BroadcastMessage(msgRef);
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

			writeQueue.Dequeue();
		}

		private void HandleClientAccepted(ClientReference client)
		{
			if (!clientList.Exists(x => x.socket == client.socket))
			{
				clientList.Add(client);
				SendConnected(client);
			}

			else
				Debug.LogWarning("HandleClientAccepted: Client " + client.socket.RemoteEndPoint.ToString() + " already exists in client list");
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

			SendPlayerInitInfo(msgRef.client);
			SendChannels(msgRef.client, new Channel[] { defaultChannel });
			SendCurChannel(msgRef.client, defaultChannel.Id);
			SendInventoryRequest(msgRef.client);
			SendLogin(msgRef.client, defaultChannel.Id);
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
			msgRef.msg._msg.Read(out int remainTime);
			msgRef.msg._msg.Read(out int playTime);

			if (msgRef.client.seq == matchData.masterSeq)
			{
				matchData.remainTime = remainTime;
				matchData.playTime = playTime;
			}

			if (debugPing)
				Debug.Log("HandleTimer from: " + msgRef.client.GetIdentifier());

			if (matchData.remainTime <= 0)
				HandleTeamMatchEnd();

			SendTimer(msgRef.client);
		}

		private void HandleMatchCountdown(MsgReference msgRef)
		{
			msgRef.msg._msg.Read(out int countdownTime);

			if (debugHandle)
				Debug.Log("HandleMatchCountdown from: " + msgRef.client.GetIdentifier());

			if (msgRef.client.seq == matchData.masterSeq)
			{
				matchData.countdownTime = countdownTime;
				if (matchData.countdownTime <= 0)
				{
					matchData.room.Status = Room.ROOM_STATUS.PLAYING;
					SendRoom(null, SendType.BroadcastRoom);
				}

				SendMatchCountdown();
			}
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

			SendPlayerInfo(msgRef.client);
			SendUserList(msgRef.client);
			SendDownloadedMaps(msgRef.client);
			SendRoamin(msgRef.client);

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

			SendDownloadedMaps(msgRef.client);
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

			if (roomNumber == matchData.room.No)
			{
				matchData.AddClient(msgRef.client);

				SendJoin(msgRef.client);
				SendRendezvousInfo(msgRef.client);
				SendMaster(msgRef.client);
				SendSlotLocks(msgRef.client);
				SendRoomConfig(msgRef.client);
				SendAddRoom(msgRef.client);
				SendEnter(msgRef.client);
				SendSlotData();
			}
		}

		private void HandleBreakIntoRequest(MsgReference msgRef)
		{
			if (debugHandle)
				Debug.Log("HandleBreakInto from: " + msgRef.client.GetIdentifier());

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
				SendTeamScore();
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
			matchData.RemoveClient(msgRef.client);

			if (debugHandle)
				Debug.Log("HandleLeave from: " + msgRef.client.GetIdentifier());

			SendLeave(msgRef.client);
			SendSetStatus(msgRef.client);

			if (matchData.room.CurPlayer <= 0)
			{
				SendDeleteRoom();
				matchData = new MatchData();
				return;
			}

			if (msgRef.client.seq == matchData.masterSeq)
			{
				matchData.masterSeq = matchData.clientList[0].seq;
				SendMaster(null);
			}
		}

		private void HandleCreateRoomRequest(MsgReference msgRef)
		{
			if (matchData.roomCreated)
			{
				SendCreateRoom(msgRef.client, false);
				return;
			}

			msgRef.msg._msg.Read(out int type);
			msgRef.msg._msg.Read(out string title);
			msgRef.msg._msg.Read(out bool isLocked);
			msgRef.msg._msg.Read(out string pswd);
			msgRef.msg._msg.Read(out int maxPlayer);
			msgRef.msg._msg.Read(out int goal);
			msgRef.msg._msg.Read(out int timelimit);
			msgRef.msg._msg.Read(out int weaponOption);
			msgRef.msg._msg.Read(out int map);
			msgRef.msg._msg.Read(out int breakinto);
			msgRef.msg._msg.Read(out int autobalance);
			msgRef.msg._msg.Read(out int wanted);
			msgRef.msg._msg.Read(out int drop);
			msgRef.msg._msg.Read(out string alias);
			msgRef.msg._msg.Read(out int master);

			matchData.room.Type = (Room.ROOM_TYPE)type;
			matchData.room.Title = title;
			matchData.room.Locked = isLocked;
			matchData.room.MaxPlayer = maxPlayer;
			matchData.room.goal = goal;
			matchData.room.timelimit = timelimit;
			matchData.room.weaponOption = weaponOption;
			matchData.room.map = map;
			matchData.room.isBreakInto = Convert.ToBoolean(breakinto);
			matchData.room.isWanted = Convert.ToBoolean(wanted);
			matchData.room.isDropItem = false;//Convert.ToBoolean(drop);
			matchData.isBalance = Convert.ToBoolean(autobalance);
			matchData.room.CurMapAlias = alias;
			matchData.masterSeq = msgRef.client.seq;
			matchData.LockSlotsByMaxPlayers(matchData.room.MaxPlayer);
			matchData.roomCreated = true;

			if (debugHandle)
				Debug.Log("HandleCreateRoom from: " + msgRef.client.GetIdentifier());

			matchData.AddClient(msgRef.client);
			SendRendezvousInfo(msgRef.client);
			SendMaster(msgRef.client);
			SendSlotLocks(msgRef.client);
			SendRoomConfig(msgRef.client);
			SendAddRoom(msgRef.client);
			SendCreateRoom(msgRef.client);
			SendEnter(msgRef.client);
			SendSlotData();
		}

		private void HandleRoomConfig(MsgReference msgRef)
		{
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

			SendRoomConfig(null);
		}

		private void HandleRoomRequest(MsgReference msgRef)
		{
			msgRef.msg._msg.Read(out int roomNumber);

			if (debugHandle)
				Debug.Log("HandleRoomRequest from: " + msgRef.client.GetIdentifier());

			if (roomNumber == matchData.room.No)
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
			msgRef.msg._msg.Read(out int nextStatus);

			if (msgRef.client.seq == matchData.masterSeq)
			{
				matchData.room.Status = (Room.ROOM_STATUS)nextStatus;
			}

			if (debugHandle)
				Debug.Log("HandleResumeRoomRequest from: " + msgRef.client.GetIdentifier());

			SendRoom(null, SendType.BroadcastRoom);
		}

		private void HandleTeamChangeRequest(MsgReference msgRef)
		{
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

				SendSlotLock(msgRef.client, slotNum, SendType.BroadcastRoom);
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
			SendRoom(null, SendType.BroadcastRoom);

			for (int i = 0; i < matchData.clientList.Count; i++)
			{
				matchData.clientList[i].status = BrickManDesc.STATUS.PLAYER_LOADING;
				matchData.clientList[i].clientStatus = ClientReference.ClientStatus.Match;
				SendSetStatus(matchData.clientList[i]);
			}

			SendStart();
		}

		private void HandleWeaponHeldRatioRequest(MsgReference msgRef)
		{
			msgRef.msg._msg.Read(out int count);
			for (int i = 0; i < count; i++)
			{
				msgRef.msg._msg.Read(out long key);
				msgRef.msg._msg.Read(out float value);
			}

			if (debugHandle)
				Debug.Log("HandleWeaponHeldRatioRequest from: " + msgRef.client.GetIdentifier());

			if (msgRef.client.status == BrickManDesc.STATUS.PLAYER_LOADING)
			{
				msgRef.client.status = BrickManDesc.STATUS.PLAYER_PLAYING;
				SendSetStatus(msgRef.client);
				SendPostLoadInit(msgRef.client);
			}

			if (msgRef.client.isBreakingInto)
			{
				msgRef.client.isBreakingInto = false;

				for (int i = 0; i < matchData.destroyedBricks.Count; i++)
					SendDestroyedBrick(msgRef.client, matchData.destroyedBricks[i]);

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

		private void HandleKillLogRequest(MsgReference msgRef)
		{
			if (killLogTimer < 0.2f)
				return;

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
			if (killer != victim) //TDM
			{

				if (victimClient.slot.slotIndex > 7)
					matchData.redScore++;
				else
					matchData.blueScore++;

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
			SendKillLogEntry(killLogEntry);
			SendTeamScore();

			if (matchData.blueScore >= matchData.room.goal || matchData.redScore >= matchData.room.goal)
				HandleTeamMatchEnd();
		}

		private void HandleTeamScoreRequest(MsgReference msgRef)
		{
			if (debugHandle)
				Debug.Log("HandleTeamScoreRequest from: " + msgRef.client.GetIdentifier());

			SendTeamScore();
		}

		private void HandleDestroyBrickRequest(MsgReference msgRef)
		{
			msgRef.msg._msg.Read(out int brick);

			if (debugHandle)
				Debug.Log("HandleDestroyBrickRequest from: " + msgRef.client.GetIdentifier());

			if (!(matchData.destroyedBricks.Exists(x => x == brick)))
			{
				matchData.destroyedBricks.Add(brick);
				SendDestroyBrick(brick);
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
				item.Usage = Item.USAGE.EQUIP;
				msgRef.client.inventory.GenerateActiveSlots();

				SendEquip(msgRef.client, item.Seq, item.Code);
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

			SendRadioMsg(seq, category, message);
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

		public void HandleTeamMatchEnd()
		{
			matchData.room.Status = Room.ROOM_STATUS.WAITING;
			SendTeamMatchEnd();
			matchData.Reset();
			SendRoom(null, SendType.BroadcastRoom);
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

			SendOpenDoor(seq);
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
			if (msgRef.client.seq == matchData.masterSeq)
			{
				msgRef.msg._msg.Read(out int newMaster);

				if (debugHandle)
					Debug.Log("HandleDelegateMasterRequest from: " + msgRef.client.GetIdentifier());

				matchData.masterSeq = newMaster;
				SendMaster(null);
			}
		}

		private void HandleGetCannonRequest(MsgReference msgRef)
		{
			msgRef.msg._msg.Read(out int brickSeq);

			if (debugHandle)
				Debug.Log("HandleGetCannonRequest from: " + msgRef.client.GetIdentifier());

			if (!matchData.usedCannons.ContainsKey(brickSeq))
			{
				matchData.usedCannons.Add(brickSeq, msgRef.client.seq);
				SendGetCannon(msgRef.client.seq, brickSeq);
				Debug.Log(matchData.usedCannons.Count);
			}
		}

		private void HandleEmptyCannonRequest(MsgReference msgRef)
		{
			msgRef.msg._msg.Read(out int brickSeq);

			if (debugHandle)
				Debug.Log("HandleGetCannonRequest from: " + msgRef.client.GetIdentifier());

			if (matchData.usedCannons.ContainsKey(brickSeq))
			{
				matchData.usedCannons.Remove(brickSeq);
				SendEmptyCannon(brickSeq);
			}
		}

		private void HandleGetTrainRequest(MsgReference msgRef)
		{
			msgRef.msg._msg.Read(out int brickSeq);
			msgRef.msg._msg.Read(out int trainId);

			if (debugHandle)
				Debug.Log("HandleGetTrainRequest from: " + msgRef.client.GetIdentifier());

			if (!matchData.usedTrains.ContainsKey(trainId))
			{
				matchData.usedTrains.Add(trainId, msgRef.client.seq);
				SendGetTrain(msgRef.client.seq, trainId);
			}
		}

		private void HandleEmptyTrainRequest(MsgReference msgRef)
		{
			msgRef.msg._msg.Read(out int trainId);

			if (debugHandle)
				Debug.Log("HandleEmptyTrainRequest from: " + msgRef.client.GetIdentifier());

			if (matchData.usedTrains.ContainsKey(trainId))
			{
				matchData.usedTrains.Remove(trainId);
				SendEmptyTrain(trainId);
			}
		}

		public void SendCannons(ClientReference client)
		{
			foreach(KeyValuePair<int, int> entry in matchData.usedCannons)
			{
				SendGetCannon(entry.Value, entry.Key, client, SendType.Unicast);
			}
		}

		public void SendTrains(ClientReference client)
		{
			foreach (KeyValuePair<int, int> entry in matchData.usedTrains)
			{
				SendGetTrain(entry.Value, entry.Key, client, SendType.Unicast);
			}
		}

		public void SendGetCannon(int seq, int brickSeq, ClientReference client = null, SendType sendType = SendType.BroadcastRoom)
		{
			MsgBody body = new MsgBody();

			body.Write(seq);
			body.Write(brickSeq);

			Say(new MsgReference(159, body, null, sendType));

			if (debugSend)
			{
				if (sendType == SendType.BroadcastRoom)
					Debug.Log("Broadcasted SendGetCannon for room no: " + matchData.room.No);

				else
					Debug.Log("SendGetCannon to: " + client.GetIdentifier());
			}
		}

		public void SendEmptyCannon(int brickSeq)
		{
			MsgBody body = new MsgBody();

			body.Write(brickSeq);

			Say(new MsgReference(161, body, null, SendType.BroadcastRoom));

			if (debugSend)
				Debug.Log("Broadcasted SendEmptyCannon for room no: " + matchData.room.No);
		}

		public void SendGetTrain(int seq, int trainId, ClientReference client = null, SendType sendType = SendType.BroadcastRoom)
		{
			MsgBody body = new MsgBody();

			body.Write(seq);
			body.Write(trainId);

			Say(new MsgReference(552, body, client, sendType));

			if (debugSend)
			{
				if (sendType == SendType.BroadcastRoom)
					Debug.Log("Broadcasted SendGetTrain for room no: " + matchData.room.No);

				else
					Debug.Log("SendGetTrain to: " + client.GetIdentifier());
			}
		}

		public void SendEmptyTrain(int trainId)
		{
			MsgBody body = new MsgBody();

			body.Write(trainId);

			Say(new MsgReference(554, body, null, SendType.BroadcastRoom));

			if (debugSend)
				Debug.Log("Broadcasted SendEmptyTrain for room no: " + matchData.room.No);
		}

		public void SendDisconnect(ClientReference client, SendType sendType = SendType.Unicast)
		{
			MsgBody body = new MsgBody();

			SayInstant(new MsgReference(ExtensionOpcodes.opDisconnectAck, body, client, sendType));
		}

		public void SendOpenDoor(int seq)
		{
			MsgBody body = new MsgBody();

			body.Write(seq);

			Say(new MsgReference(450, body, null, SendType.BroadcastRoom));

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

			Say(new MsgReference(416, body, client, SendType.BroadcastRoomExclusive));

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
		}

		public void SendTeamMatchEnd()
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

		public void SendChat(ClientReference client, ChatText.CHAT_TYPE type, string text)
		{
			MsgBody body = new MsgBody();

			body.Write(client.seq);
			body.Write((byte)type);
			body.Write(client.name);
			body.Write(text);
			body.Write(Convert.ToBoolean(client.data.gm));


			Say(new MsgReference(25, body, null, SendType.Broadcast));

			if (debugSend)
				Debug.Log("Broadcasted SendChat");
		}

		public void SendRadioMsg(int seq, int category, int message)
		{
			MsgBody body = new MsgBody();
			body.Write(seq);
			body.Write(category);
			body.Write(message);

			Say(new MsgReference(96, body, null, SendType.BroadcastRoom));

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

		public void SendEquip(ClientReference client, long itemSeq, string code)
		{
			MsgBody body = new MsgBody();

			body.Write(client.seq);
			body.Write(itemSeq);
			body.Write(code);

			Say(new MsgReference(36, body, client, SendType.Broadcast));

			if (debugSend)
				Debug.Log("Broadcasted SendEquip for client " + client.GetIdentifier() + " for room no: " + matchData.room.No);
		}

		public void SendUnequip(ClientReference client, long itemSeq, string code)
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

		public void SendDestroyBrick(int brick)
		{
			MsgBody body = new MsgBody();

			body.Write(brick);

			Say(new MsgReference(77, body, null, SendType.BroadcastRoom));

			if (debugSend)
				Debug.Log("Broadcasted SendDestroyBrick for brick " + brick+ " for room no: " + matchData.room.No);
		}

		public void SendDestroyedBrick(ClientReference client, int brick, SendType sendType = SendType.Unicast)
		{
			MsgBody body = new MsgBody();

			body.Write(brick);

			Say(new MsgReference(78, body, client, sendType));

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
			MsgBody body = new MsgBody();

			body.Write(client.seq);
			body.Write(client.kills);

			Say(new MsgReference(69, body, null, SendType.BroadcastRoom));

			if (debugSend)
				Debug.Log("Broadcasted SendKillCount for client " + client.GetIdentifier() + " for room no: " + matchData.room.No);
		}

		public void SendDeathCount(ClientReference client)
		{
			MsgBody body = new MsgBody();

			body.Write(client.seq);
			body.Write(client.deaths);

			Say(new MsgReference(68, body, null, SendType.BroadcastRoom));

			if (debugSend)
				Debug.Log("Broadcasted SendDeatchCount for client " + client.GetIdentifier() + " for room no: " + matchData.room.No);
		}

		public void SendAssistCount(ClientReference client)
		{
			MsgBody body = new MsgBody();

			body.Write(client.seq);
			body.Write(client.assists);
			body.Write(client.score);

			Say(new MsgReference(185, body, null, SendType.BroadcastRoom));

			if (debugSend)
				Debug.Log("Broadcasted SendAssistCount for client " + client.GetIdentifier() + " for room no: " + matchData.room.No);
		}

		public void SendRoundScore(ClientReference client)
		{
			MsgBody body = new MsgBody();

			body.Write(client.seq);
			body.Write(client.score);

			Say(new MsgReference(300, body, null, SendType.BroadcastRoom));

			if (debugSend)
				Debug.Log("Broadcasted SendRoundScore for client " + client.GetIdentifier() + " for room no: " + matchData.room.No);
		}

		public void SendKillLogEntry(KillLogEntry entry)
		{
			MsgBody body = new MsgBody();

			body.Write(entry.id);
			body.Write(entry.killerType);
			body.Write(entry.killer);
			body.Write(entry.victimType);
			body.Write(entry.victim);
			body.Write((int)entry.weaponBy);
			body.Write(entry.hitpart);

			Say(new MsgReference(45, body, null, SendType.BroadcastRoom));

			if (debugSend)
				Debug.Log("Broadcasted SendKillLogEntry for room no: " + matchData.room.No);
		}

		public void SendTeamScore()
		{
			MsgBody body = new MsgBody();
			body.Write(matchData.redScore);
			body.Write(matchData.blueScore);

			Say(new MsgReference(67, body, null, SendType.BroadcastRoom));

			if (debugSend)
				Debug.Log("Broadcasted SendTeamScore for room no: " + matchData.room.No);
		}

		public void SendMaster(ClientReference client)
		{
			MsgBody body = new MsgBody();

			body.Write(matchData.masterSeq);

			if (client == null)
			{
				Say(new MsgReference(31, body, null, SendType.BroadcastRoom));

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
			for (sbyte i = 0; i < matchData.slots.Count; i++)
			{
				SendSlotLock(client, i);
			}

			if (debugSend)
				Debug.Log("SendSlots to: " + client.GetIdentifier());
		}

		public void SendSlotLock(ClientReference client, sbyte index, SendType sendType = SendType.Unicast)
		{
			MsgBody body = new MsgBody();

			body.Write(index);
			body.Write(Convert.ToSByte(matchData.slots[index].isLocked));
			Say(new MsgReference(86, body, client, sendType));

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
				Say(new MsgReference(92, body, null, SendType.BroadcastRoom));

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

		public void SendAddRoom(ClientReference client)
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
				Say(new MsgReference(5, body, null, SendType.BroadcastRoom));

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

		public void SendRoom(ClientReference client, SendType sendType = SendType.Unicast)
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

			Say(new MsgReference(470, body, client, sendType));

			if (debugSend)
			{
				if (sendType == SendType.Unicast)
					Debug.Log("SendRoom to: " + client.GetIdentifier());

				else
					Debug.Log("Broadcasted SendRoom for room no: " + matchData.room.No);
			}

		}

		public void SendDeleteRoom()
		{
			MsgBody body = new MsgBody();

			body.Write(matchData.room.No);

			Say(new MsgReference(6, body, null, SendType.Broadcast));

			if (debugSend)
				Debug.Log("Broadcasted SendDelRoom for room no: " + matchData.room.No);
		}

		public void SendRoomList(ClientReference client)
		{
			MsgBody body = new MsgBody();

			body.Write(Convert.ToInt32(matchData.roomCreated)); //count
			if (matchData.roomCreated)
			{
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

			Say(new MsgReference(468, body, client));

			if (debugSend)
				Debug.Log("SendRoomList to: " + client.GetIdentifier());
		}

		public void SendCreateRoom(ClientReference client, bool success = true)
		{
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

			Say(new MsgReference(10, body, client, SendType.BroadcastRoomExclusive));

			if (debugSend)
				Debug.Log("Broadcasted SendEnter for client " + client.GetIdentifier() + " for room no: " + matchData.room.No);
		}

		public void SendLeave(ClientReference client)
		{
			MsgBody body = new MsgBody();

			body.Write(client.seq);

			Say(new MsgReference(11, body, client, SendType.BroadcastRoom));

			if (debugSend)
				Debug.Log("Broadcasted SendLeave for client " + client.GetIdentifier() + " for room no: " + matchData.room.No);
		}

		public void SendSlotData()
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

			Say(new MsgReference(ExtensionOpcodes.opSlotDataAck, body, null, SendType.BroadcastRoom));
			Say(new MsgReference(ExtensionOpcodes.opSlotDataAck, body, null, SendType.BroadcastRoom));

			if (debugSend)
				Debug.Log("Broadcasted SendSlotData for room no: " + matchData.room.No);
		}

		public void SendTeamChange(ClientReference client)
		{
			MsgBody body = new MsgBody();

			body.Write(client.seq);
			body.Write(0); //unused
			body.Write(client.slot.slotIndex);

			Say(new MsgReference(81, body, client, SendType.BroadcastRoom));

			if (debugSend)
				Debug.Log("Broadcasted SendTeamChange for client " + client.GetIdentifier() + " for room no: " + matchData.room.No);
		}

		public void SendSetStatus(ClientReference client)
		{
			MsgBody body = new MsgBody();

			body.Write(client.seq);
			body.Write((int)client.status);

			Say(new MsgReference(48, body, null, SendType.BroadcastRoom));

			if (debugSend)
				Debug.Log("Broadcasted SendSetStatus for client " + client.GetIdentifier() + " for room no: " + matchData.room.No);
		}

		public void SendStart()
		{
			MsgBody body = new MsgBody();

			body.Write(matchData.lobbyCountdownTime);

			Say(new MsgReference(50, body, null, SendType.BroadcastRoom));

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
			Say(new MsgReference(43, body, null, SendType.BroadcastRoom));

			if (debugSend)
				Debug.Log("Broadcasted SendLoadComplete for: " + client.GetIdentifier());
		}

		public void SendMatchCountdown()
		{
			MsgBody body = new MsgBody();

			body.Write(matchData.countdownTime);
			Say(new MsgReference(72, body, null, SendType.BroadcastRoom));

			if (debugSend)
				Debug.Log("Broadcasted SendMatchCountdown for: " + matchData.countdownTime);
		}

		public void SendTimer(ClientReference client)
		{
			MsgBody body = new MsgBody();

			body.Write(matchData.remainTime);
			body.Write(matchData.playTime);
			Say(new MsgReference(66, body, client, SendType.BroadcastRoom));

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

		public void SendChannels(ClientReference client, Channel[] channels)
		{
			MsgBody body = new MsgBody();

			body.Write(channels.Length);
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
			}
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

			body.Write(clientList.Count);
			for (int i = 0; i < clientList.Count; i++)
			{
				body.Write(clientList[i].seq);
				body.Write(clientList[i].name);
				body.Write(clientList[i].data.xp);
				body.Write(clientList[i].data.rank);

			}
			Say(new MsgReference(467, body, client, sendType));

			if (debugPing)
				Debug.Log("SendUserList to: " + client.GetIdentifier());
		}

		public void SendRoamin(ClientReference client, SendType sendType = SendType.Unicast)
		{
			MsgBody body = new MsgBody();

			body.Write(1);
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

		public void SendDownloadedMaps(ClientReference client, int offset = 0)
		{
			MsgBody body = new MsgBody();

			body.Write(Mathf.FloorToInt(offset / 100f)); //page
			body.Write(regMaps.Count - offset > 100 ? 100 : regMaps.Count);
			int i;
			bool recursiveFlag = false;
			for (i = offset; i < regMaps.Count; i++)
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

				if (i - offset == 100)
				{
					recursiveFlag = true;
					break;
				}
			}
			Say(new MsgReference(426, body, client));

			if (recursiveFlag)
				SendDownloadedMaps(client, i);

			if (debugSend)
				Debug.Log("SendDownloadedMaps to: " + client.GetIdentifier());
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
	}
}
