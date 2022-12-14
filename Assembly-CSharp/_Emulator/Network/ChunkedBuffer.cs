using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Debug = UnityEngine.Debug;

namespace _Emulator
{
    class ChunkedBuffer
    {
        public byte[] buffer;
        public uint offset;
        public uint crc;
        public ushort id;
        public int lastChunk;
        public bool finished;

        public ChunkedBuffer(uint _size, uint _crc, ushort _id)
        {
            buffer = new byte[_size];
            offset = 0;
            crc = _crc;
            id = _id;
            lastChunk = -1;
        }

        public void WriteNext(byte[] next, int chunkId)
        {
            if (next.Length + offset > buffer.Length)
            {
                Debug.LogError("ChunkedBuffer.WriteNext: next buffer too big");
                return;
            }
            lastChunk++;
            if (lastChunk != chunkId)
            {
                Debug.LogError("ChunkedBuffer.WriteNext: chunk order mismatch, was " + chunkId + ", expected " + lastChunk);
                return;
            }

            Array.Copy(next, 0, buffer, offset, next.Length);
            offset += Convert.ToUInt32(next.Length);
        }
    }
}
