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
	public class MsgReference
	{
		public Msg2Handle msg;
		public ClientReference client;
		public SendType sendType;
		public ChannelReference channelRef;
		public MatchData matchData;
		public bool doChunked;

		public MsgReference(Msg2Handle _msg, ClientReference _client, SendType _sendType = SendType.Unicast, ChannelReference _channelRef = null, MatchData _matchData = null, bool _doChunked = true)
		{
			msg = _msg;
			sendType = _sendType;
			if (_client != null)
				client = _client;
			channelRef = _channelRef;
			matchData = _matchData;
			doChunked = _doChunked;
        }

        public MsgReference(MessageId _id, MsgBody _msg, ClientReference _client, SendType _sendType = SendType.Unicast, ChannelReference _channelRef = null, MatchData _matchData = null, bool _doChunked = true)
        {
            if (_msg == null)
                _msg = new MsgBody();
            msg = new Msg2Handle((ushort)_id, _msg);
            sendType = _sendType;
            if (_client != null)
                client = _client;
            channelRef = _channelRef;
            matchData = _matchData;
            doChunked = _doChunked;
        }
        public MsgReference(ExtensionOpcodes _id, MsgBody _msg, ClientReference _client, SendType _sendType = SendType.Unicast, ChannelReference _channelRef = null, MatchData _matchData = null, bool _doChunked = true)
        {
            if (_msg == null)
                _msg = new MsgBody();
            msg = new Msg2Handle((ushort)_id, _msg);
            sendType = _sendType;
            if (_client != null)
                client = _client;
            channelRef = _channelRef;
            matchData = _matchData;
            doChunked = _doChunked;
        }
        public MsgReference(ushort _id, MsgBody _msg, ClientReference _client, SendType _sendType = SendType.Unicast, ChannelReference _channelRef = null, MatchData _matchData = null, bool _doChunked = true)
		{
			if (_msg == null)
				_msg = new MsgBody();
			msg = new Msg2Handle(_id, _msg);
			sendType = _sendType;
			if (_client != null)
				client = _client;
			channelRef = _channelRef;
			matchData = _matchData;
			doChunked = _doChunked;
		}
	}
}
