namespace _Emulator
{
	public enum SendType
	{
		Unicast,
		Broadcast,
		BroadcastRoom,
		BroadcastRoomExclusive,
		BroadcastRedTeam,
		BroadcastBlueTeam
	}
	class MsgReference
	{
		public Msg2Handle msg;
		public ClientReference client;
		public SendType sendType;

		public MsgReference(Msg2Handle _msg, ClientReference _client, SendType _sendType = SendType.Unicast)
		{
			msg = _msg;
			sendType = _sendType;
			if (_client != null)
				client = _client;
		}

		public MsgReference(ushort _id, MsgBody _msg, ClientReference _client, SendType _sendType = SendType.Unicast)
		{
			msg = new Msg2Handle(_id, _msg);
			sendType = _sendType;
			if (_client != null)
				client = _client;
		}
	}
}
