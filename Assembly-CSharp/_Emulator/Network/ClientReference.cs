using System.Net.Sockets;

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
        public float lastHeartBeatTime;
        public string name;
        public int seq;
        public bool isLoaded;
        public bool isHost;
        public int kills = 0;
        public int deaths = 0;
        public int assists = 0;
        public int score = 0;
        public bool isBreakingInto;
        public ClientStatus clientStatus;
        public BrickManDesc.STATUS status;
        public SlotData slot;
        public Inventory inventory;
        public DummyData data;

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
        }

        public void Disconnect(bool send = true)
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            ServerEmulator.instance.matchData.RemoveClient(this);
            ServerEmulator.instance.clientList.Remove(this);
            if (send)
            {
                ServerEmulator.instance.SendLeave(this);
                ServerEmulator.instance.SendSlotData();
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
            return name + "-" + seq + "-" + ip;
        }
    }
}
