using System;
using UnityEngine;

public class Msg4Recv
{
	public enum MsgStatus
	{
		COMPLETE,
		INCOMPLETE,
		INVALID,
		OVERFLOW
	}

	private byte[] _buffer;

	private int _io;

	public MsgHdr _hdr;

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

	public Msg4Recv()
	{
		_io = 0;
		_hdr = new MsgHdr();
		_buffer = new byte[4092];
	}

	public Msg4Recv(byte[] src)
	{
		_io = 0;
		_hdr = new MsgHdr();
		_buffer = new byte[src.Length];
		Array.Copy(src, _buffer, src.Length);
	}

	private void ExpandBuffer()
	{
		byte[] array = new byte[_buffer.Length * 2];
		Array.Copy(_buffer, 0, array, 0, _buffer.Length);
		_buffer = array;
	}

	public MsgStatus GetStatus(byte recvKey)
	{
		if (_io >= _buffer.Length)
		{
			ExpandBuffer();
		}
		if (_io < 15)
		{
			return MsgStatus.INCOMPLETE;
		}
		_hdr.FromArray(_buffer);
		if (_io < 15 + _hdr._size)
		{
			return MsgStatus.INCOMPLETE;
		}
		if (recvKey == 255)
		{
			byte b = 0;
			for (int i = 0; i < _hdr._size; i++)
			{
				b = (byte)(b ^ _buffer[15 + i]);
			}
			if (b != _hdr._crc)
			{
				Debug.LogError("CRC Error id(" + _hdr._id + ") size(" + _hdr._size + ")");
				return MsgStatus.INVALID;
			}
		}
		return MsgStatus.COMPLETE;
	}

	public ushort GetId()
	{
		return _hdr._id;
	}

	public uint GetMeta()
	{
		return _hdr._meta;
	}

	public MsgBody Flush()
	{
		MsgBody result = new MsgBody(_buffer, 15, (int)_hdr._size);
		_io -= (int)(15 + _hdr._size);
		if (_io > 0)
		{
			Array.Copy(_buffer, 15 + _hdr._size, _buffer, 0L, _io);
		}
		return result;
	}
}
