using System.IO;

public class P2PMsgHdr
{
	public const int Size = 8;

	public ushort _size;

	public byte _id;

	public byte _crc;

	public ushort _meta;

	public byte _src;

	public byte _dst;

	public P2PMsgHdr()
	{
		_size = 0;
		_id = 0;
		_crc = 0;
		_meta = 0;
		_src = 0;
		_dst = 0;
	}

	public P2PMsgHdr(ushort size, byte id, byte crc, ushort meta, byte src, byte dst)
	{
		_size = size;
		_id = id;
		_crc = crc;
		_meta = meta;
		_src = src;
		_dst = dst;
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
		binaryWriter.Write(_dst);
		return memoryStream.ToArray();
	}

	public void FromArray(byte[] src)
	{
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(src, 0, src.Length);
		memoryStream.Position = 0L;
		BinaryReader binaryReader = new BinaryReader(memoryStream);
		_size = binaryReader.ReadUInt16();
		_id = binaryReader.ReadByte();
		_crc = binaryReader.ReadByte();
		_meta = binaryReader.ReadUInt16();
		_src = binaryReader.ReadByte();
		_dst = binaryReader.ReadByte();
	}
}
