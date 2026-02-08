using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace _Emulator
{
    public static class ReaderExtension
    {

        public static short ReadShortBE(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(2);
            Array.Reverse(bytes);
            return BitConverter.ToInt16(bytes, 0);
        }

        public static int ReadIntBE(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(4);
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public static float ReadFloatBE(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(4);
            Array.Reverse(bytes);
            return BitConverter.ToSingle(bytes, 0);
        }

        public static long ReadLongBE(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(8);
            Array.Reverse(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }

        public static double ReadDoubleBE(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(8);
            Array.Reverse(bytes);
            return BitConverter.ToDouble(bytes, 0);
        }

    }
}
