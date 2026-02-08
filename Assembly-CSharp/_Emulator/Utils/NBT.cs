using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace _Emulator
{
    internal static class Helper
    {
        public static bool IsNumeric(this TagType type)
        {
            switch (type)
            {
                case TagType.BYTE:
                case TagType.SHORT:
                case TagType.INT:
                case TagType.LONG:
                case TagType.FLOAT:
                case TagType.DOUBLE:
                    return true;
                default:
                    return false;
            }
        }
    }

    public enum TagType
    {
        NONE = -1,
        END = 0,
        BYTE = 1,
        SHORT = 2,
        INT = 3,
        LONG = 4,
        FLOAT = 5,
        DOUBLE = 6,
        BYTE_ARRAY = 7,
        STRING = 8,
        LIST = 9,
        COMPOUND = 10,
        INT_ARRAY = 11,
        LONG_ARRAY = 12
    }

    public interface ITag
    {

        TagType Type { get; }

        ITag Duplicate();
    }

    public interface INumericTag : ITag
    {
        byte AsByte();
        short AsShort();
        int AsInt();
        long AsLong();
        float AsFloat();
        double AsDouble();
    }

    public interface ITag<T> : ITag where T : ITag<T>
    {

        new T Duplicate();

    }

    public abstract class Tag<T> : ITag<T> where T : ITag<T>
    {
        public static TagType getType(byte id)
        {
            return (TagType)Enum.ToObject(typeof(TagType), id);
        }

        public abstract TagType Type { get; }

        public abstract T Duplicate();

        ITag ITag.Duplicate()
        {
            return Duplicate();
        }
    }

    public abstract class NumericTag<T> : Tag<T>, INumericTag where T : ITag<T>
    {
        public abstract byte AsByte();
        public abstract double AsDouble();
        public abstract float AsFloat();
        public abstract int AsInt();
        public abstract long AsLong();
        public abstract short AsShort();
    }

    public class EndTag : Tag<EndTag>
    {
        public static EndTag Instance => instance;
        private static readonly EndTag instance = new EndTag();
        private EndTag() { }
        public override TagType Type => TagType.END;

        public override EndTag Duplicate()
        {
            return this;
        }
    }

    public class CompoundTag : Tag<CompoundTag>, IEnumerable<KeyValuePair<string, ITag>>
    {
        public override TagType Type => TagType.COMPOUND;

        private readonly Dictionary<string, ITag> content = new Dictionary<string, ITag>();

        public ITag this[string key]
        {
            get => content[key];
            set => content[key] = value;
        }

        public IEnumerator<KeyValuePair<string, ITag>> GetEnumerator()
        {
            return content.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override CompoundTag Duplicate()
        {
            CompoundTag tag = new CompoundTag();
            foreach (KeyValuePair<string, ITag> pair in content)
            {
                tag.content[pair.Key] = pair.Value.Duplicate();
            }
            return tag;
        }

        public TagType GetType(string key)
        {
            var tag = content[key];
            if (tag == null)
            {
                return TagType.NONE;
            }
            return tag.Type;
        }

        public TagType GetListType(string key)
        {
            var tag = content[key];
            if (tag == null || tag.Type != TagType.LIST)
            {
                return TagType.NONE;
            }
            return (tag as ListTag).ElementType;
        }

        public bool IsEmpty()
        {
            return content.Count == 0;
        }

        public int Size()
        {
            return content.Count;
        }

        public bool Has(string key)
        {
            return content.ContainsKey(key);
        }

        public bool Has(string key, TagType type)
        {
            return GetType(key) == type;
        }

        public bool HasNumeric(string key)
        {
            return GetType(key).IsNumeric();
        }

        public bool HasList(string key, TagType type)
        {
            var tag = content[key];
            if (tag == null || tag.Type != TagType.LIST)
            {
                return false;
            }
            var listType = (tag as ListTag).ElementType;
            return listType == type || listType == TagType.END;
        }

        public byte GetByte(string key)
        {
            var tag = content[key];
            if (tag == null || !tag.Type.IsNumeric())
            {
                return 0;
            }
            return (tag as INumericTag).AsByte();
        }

        public short GetShort(string key)
        {
            var tag = content[key];
            if (tag == null || !tag.Type.IsNumeric())
            {
                return 0;
            }
            return (tag as INumericTag).AsShort();
        }

        public int GetInt(string key)
        {
            var tag = content[key];
            if (tag == null || !tag.Type.IsNumeric()) 
            { 
                return 0; 
            }
            return (tag as INumericTag).AsInt();
        }

        public long GetLong(string key)
        {
            var tag = content[key];
            if (tag == null || !tag.Type.IsNumeric())
            {
                return 0;
            }
            return (tag as INumericTag).AsLong();
        }

        public float GetFloat(string key)
        {
            var tag = content[key];
            if (tag == null || !tag.Type.IsNumeric())
            {
                return 0;
            }
            return (tag as INumericTag).AsFloat();
        }

        public double GetDouble(string key)
        {
            var tag = content[key];
            if (tag == null || !tag.Type.IsNumeric())
            {
                return 0;
            }
            return (tag as INumericTag).AsDouble();
        }
    }

    public class ListTag : Tag<ListTag>, IEnumerable<ITag>
    {
        public static ListTag Empty => empty;
        private static readonly ListTag empty = new ListTag(TagType.END);

        public override TagType Type => throw new NotImplementedException();
        public TagType ElementType => _elementType;

        private TagType _elementType = TagType.END;
        private List<ITag> content = new List<ITag>();

        public ListTag(TagType elementType)
        {
            _elementType = checkType(elementType);
        }

        private TagType checkType(TagType elementType)
        {
            if (elementType == TagType.NONE || (elementType == TagType.END && empty != null))
            {
                throw new ArgumentException("Can't create list of type " + elementType);
            }
            return elementType;
        }

        public override ListTag Duplicate()
        {
            if (this == empty)
            {
                return empty;
            }
            ListTag newTag = new ListTag(_elementType);
            for (int i = 0; i < content.Count; i++)
            {
                newTag.content[i] = content[i].Duplicate();
            }
            return newTag;
        }

        public bool IsEmpty()
        {
            return content.Count == 0;
        }

        public int Size()
        {
            return content.Count;
        }

        public T Get<T>(int index) where T : ITag<T>
        {
            var tag = content[index];
            if (tag is T castTag)
            {
                return castTag;
            }
            throw new ArgumentException("Unsupported tag type");
        }

        public void Add(ITag tag)
        {
            content.Add(checkTag(tag));
        }

        private ITag checkTag(ITag tag)
        {
            if (tag.Type != _elementType)
            {
                throw new ArgumentException("Expected tag of type " + _elementType + " but got tag of type " + tag.Type);
            }
            return tag;
        }

        public IEnumerator<ITag> GetEnumerator()
        {
            return content.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class StringTag : Tag<StringTag>
    {
        public override TagType Type => TagType.STRING;

        public string Value;
        public StringTag(string value)
        {
            Value = value;
        }

        public override StringTag Duplicate()
        {
            return new StringTag(Value);
        }
    }

    public class ByteTag : NumericTag<ByteTag>
    {
        public override TagType Type => TagType.BYTE;

        public byte Value;
        public ByteTag(byte value)
        {
            this.Value = value;
        }
        public override ByteTag Duplicate()
        {
            return new ByteTag(Value);
        }

        public override byte AsByte()
        {
            return Value;
        }

        public override double AsDouble()
        {
            return Value;
        }

        public override float AsFloat()
        {
            return Value;
        }

        public override int AsInt()
        {
            return Value;
        }

        public override long AsLong()
        {
            return Value;
        }

        public override short AsShort()
        {
            return Value;
        }

    }

    public class ShortTag : NumericTag<ShortTag>
    {
        public override TagType Type => TagType.SHORT;

        public short Value;
        public ShortTag(short value)
        {
            this.Value = value;
        }
        public override ShortTag Duplicate()
        {
            return new ShortTag(Value);
        }

        public override byte AsByte()
        {
            return (byte) Value;
        }

        public override double AsDouble()
        {
            return Value;
        }

        public override float AsFloat()
        {
            return Value;
        }

        public override int AsInt()
        {
            return Value;
        }

        public override long AsLong()
        {
            return Value;
        }

        public override short AsShort()
        {
            return Value;
        }

    }

    public class IntTag : NumericTag<IntTag>
    {
        public override TagType Type => TagType.INT;

        public int Value;
        public IntTag(int value)
        {
            this.Value = value;
        }
        public override IntTag Duplicate()
        {
            return new IntTag(Value);
        }

        public override byte AsByte()
        {
            return (byte) Value;
        }

        public override double AsDouble()
        {
            return Value;
        }

        public override float AsFloat()
        {
            return Value;
        }

        public override int AsInt()
        {
            return Value;
        }

        public override long AsLong()
        {
            return Value;
        }

        public override short AsShort()
        {
            return (short) Value;
        }

    }

    public class LongTag : NumericTag<LongTag>
    {
        public override TagType Type => TagType.LONG;

        public long Value;
        public LongTag(long value)
        {
            this.Value = value;
        }
        public override LongTag Duplicate()
        {
            return new LongTag(Value);
        }

        public override byte AsByte()
        {
            return (byte)Value;
        }

        public override double AsDouble()
        {
            return Value;
        }

        public override float AsFloat()
        {
            return Value;
        }

        public override int AsInt()
        {
            return (int) Value;
        }

        public override long AsLong()
        {
            return Value;
        }

        public override short AsShort()
        {
            return (short)Value;
        }

    }

    public class FloatTag : NumericTag<FloatTag>
    {
        public override TagType Type => TagType.FLOAT;

        public float Value;
        public FloatTag(float value)
        {
            this.Value = value;
        }
        public override FloatTag Duplicate()
        {
            return new FloatTag(Value);
        }

        public override byte AsByte()
        {
            return (byte)Value;
        }

        public override double AsDouble()
        {
            return Value;
        }

        public override float AsFloat()
        {
            return Value;
        }

        public override int AsInt()
        {
            return (int)Value;
        }

        public override long AsLong()
        {
            return (long)Value;
        }

        public override short AsShort()
        {
            return (short)Value;
        }

    }

    public class DoubleTag : NumericTag<DoubleTag>
    {
        public override TagType Type => TagType.FLOAT;

        public double Value;
        public DoubleTag(double value)
        {
            this.Value = value;
        }
        public override DoubleTag Duplicate()
        {
            return new DoubleTag(Value);
        }

        public override byte AsByte()
        {
            return (byte)Value;
        }

        public override double AsDouble()
        {
            return Value;
        }

        public override float AsFloat()
        {
            return (float)Value;
        }

        public override int AsInt()
        {
            return (int)Value;
        }

        public override long AsLong()
        {
            return (long)Value;
        }

        public override short AsShort()
        {
            return (short)Value;
        }

    }

    public class ByteArrayTag : Tag<ByteArrayTag>
    {
        public override TagType Type => TagType.INT_ARRAY;

        public byte[] Value;

        public ByteArrayTag(byte[] value)
        {
            this.Value = value;
        }

        public override ByteArrayTag Duplicate()
        {
            byte[] newArray = new byte[Value.Length];
            Array.Copy(Value, newArray, Value.Length);
            return new ByteArrayTag(newArray);
        }
    }

    public class IntArrayTag : Tag<IntArrayTag>
    {
        public override TagType Type => TagType.INT_ARRAY;

        public int[] Value;

        public IntArrayTag(int[] value) {
            this.Value = value;
        }

        public override IntArrayTag Duplicate()
        {
            int[] newArray = new int[Value.Length];
            Array.Copy(Value, newArray, Value.Length);
            return new IntArrayTag(newArray);
        }
    }

    public class LongArrayTag : Tag<LongArrayTag>
    {
        public override TagType Type => TagType.INT_ARRAY;

        public long[] Value;

        public LongArrayTag(long[] value)
        {
            this.Value = value;
        }

        public override LongArrayTag Duplicate()
        {
            long[] newArray = new long[Value.Length];
            Array.Copy(Value, newArray, Value.Length);
            return new LongArrayTag(newArray);
        }
    }

}
