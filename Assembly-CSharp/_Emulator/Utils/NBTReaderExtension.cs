using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace _Emulator
{
    public static class NBTReaderExtension
    {

        public static KeyValuePair<string, CompoundTag> ReadAsNbt(this BinaryReader reader)
        {
            TagType type = (TagType) reader.ReadByte();
            if (type != TagType.COMPOUND)
            {
                throw new NotSupportedException("Expected 'COMPOUND' got something else.");
            }
            var name = reader.ReadNbtString();
            var compoundTag = reader.ReadCompoundTag();
            return new KeyValuePair<string, CompoundTag>(name, compoundTag);
        }

        private static ITag ReadTag(this BinaryReader reader, TagType type)
        {
            switch (type)
            {
                case TagType.END:
                    return EndTag.Instance;
                case TagType.BYTE:
                    return new ByteTag(reader.ReadByte());
                case TagType.SHORT:
                    return new ShortTag(reader.ReadShortBE());
                case TagType.INT:
                    return new IntTag(reader.ReadIntBE());
                case TagType.LONG:
                    return new LongTag(reader.ReadLongBE());
                case TagType.FLOAT:
                    return new FloatTag(reader.ReadFloatBE());
                case TagType.DOUBLE:
                    return new DoubleTag(reader.ReadDoubleBE());
                case TagType.STRING:
                    return new StringTag(reader.ReadNbtString());
                case TagType.BYTE_ARRAY:
                    return new ByteArrayTag(reader.ReadNbtByteArray());
                case TagType.INT_ARRAY:
                    return new IntArrayTag(reader.ReadNbtIntArray());
                case TagType.LONG_ARRAY:
                    return new LongArrayTag(reader.ReadNbtLongArray());
                case TagType.COMPOUND:
                    return reader.ReadCompoundTag();
                case TagType.LIST:
                    return reader.ReadListTag();
            }
            throw new NotSupportedException(type.ToString());
        }

        private static CompoundTag ReadCompoundTag(this BinaryReader reader)
        {
            CompoundTag tag = new CompoundTag();
            TagType type;
            while ((type = (TagType)reader.ReadByte()) != TagType.END)
            {
                var key = reader.ReadNbtString();
                tag[key] = reader.ReadTag(type);
            }
            return tag;
        }

        private static ListTag ReadListTag(this BinaryReader reader)
        {
            TagType type = (TagType)reader.ReadByte();
            int length = reader.ReadIntBE();
            if (length < 0)
            {
                throw new ArgumentException("List length can't be lower than 0");
            }
            if (length == 0)
            {
                return ListTag.Empty;
            }
            ListTag tag = new ListTag(type);
            for (int i = 0; i < length; i++)
            {
                tag.Add(ReadTag(reader, type));
            }
            return tag;
        }

        private static byte[] ReadNbtByteArray(this BinaryReader reader)
        {
            var length = reader.ReadIntBE();
            return reader.ReadBytes(length);
        }

        private static int[] ReadNbtIntArray(this BinaryReader reader)
        {
            var length = reader.ReadIntBE();
            int[] array = new int[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = reader.ReadIntBE();
            }
            return array;
        }

        private static long[] ReadNbtLongArray(this BinaryReader reader)
        {
            int length = reader.ReadIntBE();
            long[] array = new long[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = reader.ReadLongBE();
            }
            return array;
        }

        private static string ReadNbtString(this BinaryReader reader)
        {
            ushort length = (ushort)reader.ReadShortBE();
            var bytes = reader.ReadBytes(length);
            return Encoding.UTF8.GetString(bytes);
        }
    
    }
}
