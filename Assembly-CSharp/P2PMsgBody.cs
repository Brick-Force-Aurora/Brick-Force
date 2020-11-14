using System;
using System.IO;
using System.Text;

public class P2PMsgBody
{
	private const int DEFAULT_BUFFER_SIZE = 1024;

	private int _offset;

	private byte[] _buffer;

	public int Offset
	{
		get
		{
			return _offset;
		}
		set
		{
			_offset = value;
		}
	}

	public byte[] Buffer => _buffer;

	public P2PMsgBody()
	{
		_offset = 0;
		_buffer = new byte[1024];
	}

	public P2PMsgBody(byte[] src, int offset, int length)
	{
		_offset = 0;
		_buffer = new byte[length];
		Array.Copy(src, offset, _buffer, 0, length);
	}

	private void ExpandBuffer()
	{
		byte[] array = new byte[_buffer.Length * 2];
		Array.Copy(_buffer, 0, array, 0, _buffer.Length);
		_buffer = array;
	}

	public void Decrypt(byte key)
	{
		if (key != 255)
		{
			for (int i = 0; i < _buffer.Length; i++)
			{
				_buffer[i] ^= key;
			}
		}
	}

	private bool Copy(byte[] src)
	{
		bool result = true;
		if (src.Length + _offset > _buffer.Length)
		{
			ExpandBuffer();
			result = false;
		}
		Array.Copy(src, 0, _buffer, _offset, src.Length);
		_offset += src.Length;
		return result;
	}

	public bool Write(string val)
	{
		byte[] bytes = Encoding.Unicode.GetBytes(val);
		if (!Write(bytes.Length))
		{
			return false;
		}
		return Copy(bytes);
	}

	public bool Write(int val)
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(val);
		return Copy(memoryStream.ToArray());
	}

	public bool Write(short val)
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(val);
		return Copy(memoryStream.ToArray());
	}

	public bool Write(float val)
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(val);
		return Copy(memoryStream.ToArray());
	}

	public bool Write(bool val)
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(val);
		return Copy(memoryStream.ToArray());
	}

	public bool Write(ushort val)
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(val);
		return Copy(memoryStream.ToArray());
	}

	public bool Write(uint val)
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(val);
		return Copy(memoryStream.ToArray());
	}

	public bool Write(sbyte val)
	{
		bool result = true;
		if (_offset + 1 > _buffer.Length)
		{
			ExpandBuffer();
			result = false;
		}
		_buffer[_offset++] = (byte)val;
		return result;
	}

	public bool Write(byte val)
	{
		bool result = true;
		if (_offset + 1 > _buffer.Length)
		{
			ExpandBuffer();
			result = false;
		}
		_buffer[_offset++] = val;
		return result;
	}

	public bool Write(long val)
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(val);
		return Copy(memoryStream.ToArray());
	}

	public bool Read(out string val)
	{
		val = string.Empty;
		if (!Read(out int val2))
		{
			return false;
		}
		val = Encoding.Unicode.GetString(_buffer, _offset, val2);
		_offset += val2;
		return true;
	}

	public bool Read(out int val)
	{
		val = 0;
		if (_offset + 4 > _buffer.Length)
		{
			return false;
		}
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(_buffer, _offset, 4);
		_offset += 4;
		memoryStream.Position = 0L;
		BinaryReader binaryReader = new BinaryReader(memoryStream);
		val = binaryReader.ReadInt32();
		return true;
	}

	public bool Read(out short val)
	{
		val = 0;
		if (_offset + 2 > _buffer.Length)
		{
			return false;
		}
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(_buffer, _offset, 2);
		_offset += 2;
		memoryStream.Position = 0L;
		BinaryReader binaryReader = new BinaryReader(memoryStream);
		val = binaryReader.ReadInt16();
		return true;
	}

	public bool Read(out float val)
	{
		val = 0f;
		if (_offset + 4 > _buffer.Length)
		{
			return false;
		}
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(_buffer, _offset, 4);
		_offset += 4;
		memoryStream.Position = 0L;
		BinaryReader binaryReader = new BinaryReader(memoryStream);
		val = binaryReader.ReadSingle();
		return true;
	}

	public bool Read(out bool val)
	{
		val = false;
		if (_offset + 1 > _buffer.Length)
		{
			return false;
		}
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(_buffer, _offset, 1);
		_offset++;
		memoryStream.Position = 0L;
		BinaryReader binaryReader = new BinaryReader(memoryStream);
		val = binaryReader.ReadBoolean();
		return true;
	}

	public bool Read(out ushort val)
	{
		val = 0;
		if (_offset + 2 > _buffer.Length)
		{
			return false;
		}
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(_buffer, _offset, 2);
		_offset += 2;
		memoryStream.Position = 0L;
		BinaryReader binaryReader = new BinaryReader(memoryStream);
		val = binaryReader.ReadUInt16();
		return true;
	}

	public bool Read(out uint val)
	{
		val = 0u;
		if (_offset + 4 > _buffer.Length)
		{
			return false;
		}
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(_buffer, _offset, 4);
		_offset += 4;
		memoryStream.Position = 0L;
		BinaryReader binaryReader = new BinaryReader(memoryStream);
		val = binaryReader.ReadUInt32();
		return true;
	}

	public bool Read(out sbyte val)
	{
		val = 0;
		if (_offset + 1 > _buffer.Length)
		{
			return false;
		}
		val = (sbyte)_buffer[_offset++];
		return true;
	}

	public bool Read(out byte val)
	{
		val = 0;
		if (_offset + 1 > _buffer.Length)
		{
			return false;
		}
		val = _buffer[_offset++];
		return true;
	}

	public bool Read(out long val)
	{
		val = 0L;
		if (_offset + 8 > _buffer.Length)
		{
			return false;
		}
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(_buffer, _offset, 8);
		_offset += 8;
		memoryStream.Position = 0L;
		BinaryReader binaryReader = new BinaryReader(memoryStream);
		val = binaryReader.ReadInt64();
		return true;
	}

	public byte[] GetFullPacketBuffer(byte id)
	{
		ushort num = (ushort)_offset;
		byte b = 0;
		for (int i = 0; i < num; i++)
		{
			b = (byte)(b ^ _buffer[i]);
		}
		byte[] array = new byte[8 + num];
		P2PMsgHdr p2PMsgHdr = new P2PMsgHdr(num, id, b, ushort.MaxValue, byte.MaxValue, byte.MaxValue);
		byte[] array2 = p2PMsgHdr.ToArray();
		Array.Copy(array2, array, array2.Length);
		Array.Copy(_buffer, 0, array, array2.Length, num);
		return array;
	}
}
