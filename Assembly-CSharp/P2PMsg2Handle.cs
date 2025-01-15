using System.Net;
using Steamworks;

public class P2PMsg2Handle
{
	public ushort _id;

	public P2PMsgBody _msg;

	public IPEndPoint _recvFrom;

	public CSteamID _steamIDFrom;

	public ushort _meta;

	public byte _src;

	public byte _dst;

	public P2PMsg2Handle(ushort id, P2PMsgBody msg, IPEndPoint recvFrom, ushort meta, byte src, byte dst)
	{
		_id = id;
		_msg = msg;
		_recvFrom = recvFrom;
		_meta = meta;
		_src = src;
		_dst = dst;
	}

    public P2PMsg2Handle(ushort id, P2PMsgBody msg, CSteamID steamIDFrom, ushort meta, byte src, byte dst)
    {
        _id = id;
        _msg = msg;
        _steamIDFrom = steamIDFrom;
        _meta = meta;
        _src = src;
        _dst = dst;
    }
}
