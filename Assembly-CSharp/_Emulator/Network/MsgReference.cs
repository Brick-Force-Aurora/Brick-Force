namespace _Emulator
{
	public enum SendType
	{
		Unicast,
		Broadcast,
		BroadcastChannel,
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
		public ChannelReference channelRef;
		public MatchData matchData;

		public MsgReference(Msg2Handle _msg, ClientReference _client, SendType _sendType = SendType.Unicast, ChannelReference _channelRef = null, MatchData _matchData = null)
		{
			msg = _msg;
			sendType = _sendType;
			if (_client != null)
				client = _client;
			channelRef = _channelRef;
			matchData = _matchData;
		}

		public MsgReference(ushort _id, MsgBody _msg, ClientReference _client, SendType _sendType = SendType.Unicast, ChannelReference _channelRef = null, MatchData _matchData = null)
		{
			msg = new Msg2Handle(_id, _msg);
			sendType = _sendType;
			if (_client != null)
				client = _client;
			channelRef = _channelRef;
			matchData = _matchData;
		}
	}
}
