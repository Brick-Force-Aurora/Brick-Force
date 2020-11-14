using System;

public class Msg4Send
{
	public enum MsgStatus
	{
		COMPLETE,
		INCOMPLETE
	}

	private byte[] _buffer;

	private int _io;

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

	public Msg4Send(ushort id, uint meta, uint src, MsgBody msgBody, byte sendKey)
	{
		uint offset = (uint)msgBody.Offset;
		byte b = 0;
		if (sendKey == 255)
		{
			for (int i = 0; i < offset; i++)
			{
				b = (byte)(b ^ msgBody.Buffer[i]);
			}
		}
		else
		{
			for (int j = 0; j < offset; j++)
			{
				b = (byte)(b ^ msgBody.Buffer[j]);
				msgBody.Buffer[j] ^= sendKey;
			}
		}
		_io = 0;
		_buffer = new byte[15 + offset];
		MsgHdr msgHdr = new MsgHdr(offset, id, b, meta, src);
		byte[] array = msgHdr.ToArray();
		Array.Copy(array, _buffer, array.Length);
		Array.Copy(msgBody.Buffer, 0L, _buffer, array.Length, offset);
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
