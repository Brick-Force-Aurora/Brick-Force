using System.Net;

public class P2PMsg2Handle
{
	public ushort _id;

	public P2PMsgBody _msg;

	public IPEndPoint _recvFrom;

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
}
