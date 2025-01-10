using System.Collections.Generic;
using System.Net.Sockets;
using Steamworks;

namespace _Emulator
{
    class ClientReference
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
        public byte[] buffer;
        public CSteamID steamID = CSteamID.Nil;
        public bool isSteam = false;
        public float lastHeartBeatTime;
        public string name;
        public int seq;
        public bool isLoaded;
        public bool isHost;
        public int kills = 0;
        public int deaths = 0;
        public int assists = 0;
        public int score = 0;
        public bool isZombie = false;
        public bool isBreakingInto;
        public float toleranceTime;
        public ClientStatus clientStatus;
        public BrickManDesc.STATUS status;
        public SlotData slot;
        public Inventory inventory;
        public DummyData data;
        public MatchData matchData;
        public ChannelReference channel;
        public ChunkedBuffer chunkedBuffer;

        private readonly object dataLock = new object();

        public ClientReference(Socket _socket, string _name = "", int _seq = -1)
        {
            lastHeartBeatTime = float.MaxValue;
            socket = _socket;
            name = _name;
            seq = _seq;
            clientStatus = ClientStatus.Invalid;
            status = BrickManDesc.STATUS.PLAYER_WAITING;
            data = new DummyData();
            ip = socket.RemoteEndPoint.ToString().Split(':')[0];
            isLoaded = false;
            isHost = ServerEmulator.instance.clientList.Count == 0;
            buffer = new byte[8192];
            toleranceTime = 0f;
            isSteam = false;
        }

        public ClientReference(CSteamID _steamID, string _name = "", int _seq = -1)
        {
            lastHeartBeatTime = float.MaxValue;
            steamID = _steamID;
            name = _name;
            seq = _seq;
            clientStatus = ClientStatus.Invalid;
            status = BrickManDesc.STATUS.PLAYER_WAITING;
            data = new DummyData();
            isLoaded = false;
            isHost = ServerEmulator.instance.clientList.Count == 0;
            buffer = new byte[8192];
            toleranceTime = 0f;
            isSteam = true;
        }

        public void Disconnect(bool send = true)
        {
            if (isSteam)
            {
            }
            else
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            lock (dataLock)
            {
                if (matchData != null)
                    matchData.RemoveClient(this);
                if (channel != null)
                    channel.RemoveClient(this);
                //ServerEmulator.instance.matchData.RemoveClient(this);
                ServerEmulator.instance.clientList.Remove(this);
            }
            if (send)
            {
                ServerEmulator.instance.SendLeave(this);
                if (isSteam)
                    ServerEmulator.instance.SendSlotDataSteam(matchData);
                else
                    ServerEmulator.instance.SendSlotData(matchData);
            }
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
    }
}
