using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace _Emulator
{

    public class ChunkedBufferReceiver
    {
        public const ushort MIN_SIZE_FOR_CHUNKING = 4096;
        public const ushort MAX_CHUNKS = ushort.MaxValue;
        public const ushort MAX_CHUNK_LENGTH = 6144;
        public const uint MAX_BUFFER_LIMIT = MAX_CHUNKS * MAX_CHUNK_LENGTH;

        private class ReceivingBuffer
        {
            internal readonly ushort opcode;
            internal readonly byte subId;
            internal readonly byte[] data;
            internal readonly ushort chunkCount;
            internal readonly uint crc;
            internal ushort expectedChunkId;
            internal bool done = false;

            public ReceivingBuffer(ushort opcode, uint size, uint crc)
            {
                this.opcode = opcode;
                this.data = new byte[size];
                this.chunkCount = (ushort)Mathf.Ceil(size / (float)MAX_CHUNK_LENGTH);
                this.expectedChunkId = 0;
                this.crc = crc;
            }
        }

        private List<ReceivingBuffer> buffers = new List<ReceivingBuffer>();
        public bool IsServer { get; set; }
        public string Identifier { get
            {
                if (IsServer) return "Server";
                return "Client";
            }
        }

        private ReceivingBuffer GetFor(ushort opcode)
        {
            ReceivingBuffer buffer;
            for (int i = 0; i < buffers.Count; i++)
            {
                buffer = buffers[i];
                if (buffer.opcode == opcode)
                {
                    return buffer;
                }
            }
            return null;
        }

        public bool Begin(MsgBody input)
        {
            input.Read(out ushort opcode);
            input.Read(out uint dataSize);
            input.Read(out uint crc);

            if (dataSize > MAX_BUFFER_LIMIT)
            {
                Debug.LogWarning($"RECV ({Identifier}): Buffer of message {opcode} is too big");
                return false;
            }

            ReceivingBuffer buffer = GetFor(opcode);
            if (buffer != null)
            {
                Debug.LogWarning($"RECV ({Identifier}): There is already a buffer for message {opcode}");
                return false;
            }
            buffers.Add(new ReceivingBuffer(opcode, dataSize, crc));
            return true;
        }

        public bool ReceiveChunk(MsgBody input)
        {
            input.Read(out ushort opcode);
            input.Read(out ushort chunkId);
            input.Read(out byte[] data);
            ReceivingBuffer buffer = GetFor(opcode);
            if (buffer == null)
            {
                Debug.LogWarning($"RECV ({Identifier}): No buffer of message {opcode}");
                return false;
            }
            if (chunkId != buffer.expectedChunkId)
            {
                Debug.LogWarning($"RECV ({Identifier}): Expected chunk {buffer.expectedChunkId} but got chunk {chunkId} for buffer of message {opcode}");
                buffers.Remove(buffer);
                return false;
            }
            Array.Copy(data, 0, buffer.data, chunkId * MAX_CHUNK_LENGTH, data.Length);
            buffer.expectedChunkId = (ushort)(chunkId + 1);
            return true;
        }

        public bool End(MsgBody input, out ushort packedOpcode, out MsgBody packedBody)
        {
            input.Read(out ushort opcode);
            ReceivingBuffer buffer = GetFor(opcode);
            if (buffer == null)
            {
                Debug.LogWarning($"RECV ({Identifier}): No buffer of message {opcode}");
                packedOpcode = 0;
                packedBody = null;
                return false;
            }
            if (buffer.chunkCount != buffer.expectedChunkId)
            {
                Debug.LogWarning($"RECV ({Identifier}): Not all chunks were delivered for buffer of message {opcode}");
                packedOpcode = 0;
                packedBody = null;
                return false;
            }
            uint crc = CRC32.computeUnsigned(buffer.data);
            if (crc != buffer.crc)
            {
                Debug.LogWarning($"RECV ({Identifier}): CRC check failed, expected {buffer.crc} but got {crc} for buffer of message {opcode}");
                packedOpcode = 0;
                packedBody = null;
                return false;
            }
            buffers.Remove(buffer);
            packedOpcode = opcode;
            packedBody = new MsgBody(buffer.data, 0, buffer.data.Length);
            return true;
        }
    }
}
