using System;

public class P2PMsg4Send
{
	public enum MsgStatus
	{
		COMPLETE,
		INCOMPLETE
	}

	private byte[] _buffer;

	private int _io;

	private ushort _meta;

	public byte[] Buffer => _buffer;

	public int Io
	{
		get
		{
			return _io;
		}
		set
		{
			_io = value;
		}
	}

	public ushort Meta => _meta;

	public byte _id;

	public P2PMsg4Send(byte id, ushort meta, byte src, byte dst, P2PMsgBody msgBody, byte sendKey)
	{
		ushort num = (ushort)msgBody.Offset;
		byte b = 0;
		if (sendKey == 255)
		{
			for (int i = 0; i < num; i++)
			{
				b = (byte)(b ^ msgBody.Buffer[i]);
			}
		}
		else
		{
			for (int j = 0; j < num; j++)
			{
				b = (byte)(b ^ msgBody.Buffer[j]);
				msgBody.Buffer[j] ^= sendKey;
			}
		}
		_id = id;
		_meta = meta;
		_io = 0;
		_buffer = new byte[8 + num];
		P2PMsgHdr p2PMsgHdr = new P2PMsgHdr(num, id, b, meta, src, dst);
		byte[] array = p2PMsgHdr.ToArray();
		Array.Copy(array, _buffer, array.Length);
		Array.Copy(msgBody.Buffer, 0, _buffer, array.Length, num);
	}

	public MsgStatus GetStatus()
	{
		if (_io >= _buffer.Length)
		{
			return MsgStatus.COMPLETE;
		}
		return MsgStatus.INCOMPLETE;
	}
}
