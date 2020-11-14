using System.IO;

public class MsgHdr
{
	public const int Size = 15;

	public uint _size;

	public ushort _id;

	public byte _crc;

	public uint _meta;

	public uint _src;

	public MsgHdr()
	{
		_size = 0u;
		_id = 0;
		_crc = 0;
		_meta = 0u;
		_src = 0u;
	}

	public MsgHdr(uint size, ushort id, byte crc, uint meta, uint src)
	{
		_size = size;
		_id = id;
		_crc = crc;
		_meta = meta;
		_src = src;
	}

	public byte[] ToArray()
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(_size);
		binaryWriter.Write(_id);
		binaryWriter.Write(_crc);
		binaryWriter.Write(_meta);
		binaryWriter.Write(_src);
		return memoryStream.ToArray();
	}

	public void FromArray(byte[] src)
	{
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(src, 0, src.Length);
		memoryStream.Position = 0L;
		BinaryReader binaryReader = new BinaryReader(memoryStream);
		_size = binaryReader.ReadUInt32();
		_id = binaryReader.ReadUInt16();
		_crc = binaryReader.ReadByte();
		_meta = binaryReader.ReadUInt32();
		_src = binaryReader.ReadUInt32();
	}
}
