using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace _Emulator.Network
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

            public ReceivingBuffer(ushort opcode, byte subId, uint size, uint crc)
            {
                this.opcode = opcode;
                this.subId = subId;
                this.data = new byte[size];
                this.chunkCount = (ushort)Mathf.Ceil(size / 6144f);
                this.crc = crc;
                this.expectedChunkId = 0;
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

        private ReceivingBuffer GetFor(ushort opcode, byte subId, bool done = false)
        {
            ReceivingBuffer buffer;
            for (int i = 0; i < buffers.Count; i++)
            {
                buffer = buffers[i];
                if (buffer.opcode == opcode && buffer.subId == subId && buffer.done == done)
                {
                    return buffer;
                }
            }
            return null;
        }

        public int Begin(MsgBody input, ref MsgBody output)
        {
            input.Read(out ushort opcode);
            input.Read(out byte subId);
            input.Read(out uint dataSize);
            input.Read(out uint crc);

            if (dataSize > MAX_BUFFER_LIMIT)
            {
                Debug.LogWarning($"RECV ({Identifier}): Buffer of message {opcode} is too big");
                return -1;
            }

            ReceivingBuffer buffer = GetFor(opcode, subId);
            if (buffer != null)
            {
                Debug.LogWarning($"RECV ({Identifier}): There is already a buffer for message {opcode} with sub id {subId}");
                return -1;
            }
            buffer = new ReceivingBuffer(opcode, subId, dataSize, crc);
            buffers.Add(buffer);
            output.Write(opcode);
            output.Write(subId);
            return ExtensionOpcodes.opBeginChunkedBufferAck;
        }

        public int ReceiveChunk(MsgBody input, ref MsgBody output)
        {
            input.Read(out ushort opcode);
            input.Read(out byte subId);
            input.Read(out ushort chunkId);
            input.Read(out byte[] data);
            ReceivingBuffer buffer = GetFor(opcode, subId);
            if (buffer == null)
            {
                Debug.LogWarning($"RECV ({Identifier}): No buffer for message {opcode} with sub id {subId}");
                return -1;
            }
            Array.Copy(data, 0, buffer.data, chunkId * MAX_CHUNK_LENGTH, data.Length);
            output.Write(opcode);
            output.Write(subId);
            return ExtensionOpcodes.opChunkedBufferAck;
        }

        public int End(MsgBody input, ref MsgBody output, out ushort packedOpcode, out MsgBody packedBody)
        {
            input.Read(out ushort opcode);
            input.Read(out byte subId);
            ReceivingBuffer buffer = GetFor(opcode, subId);
            if (buffer == null)
            {
                Debug.LogWarning($"RECV ({Identifier}): No buffer for message {opcode} with sub id {subId}");
                packedOpcode = 0;
                packedBody = null;
                return -1;
            }
            buffer.done = true;
            uint crc = CRC32.computeUnsigned(buffer.data);
            output.Write(opcode);
            output.Write(subId);
            if (crc != buffer.crc)
            {
                packedOpcode = 0;
                packedBody = null;
                return ExtensionOpcodes.opEndChunkedBufferFailedAck;
            }
            packedOpcode = opcode;
            packedBody = new MsgBody(buffer.data, 0, buffer.data.Length);
            return ExtensionOpcodes.opEndChunkedBufferAck;
        }
    }
}
