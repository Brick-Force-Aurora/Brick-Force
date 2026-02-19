using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using UnityEngine;

namespace _Emulator.Network
{
    public class ChunkedBufferSender
    {

        private class SendingBuffer
        {

            internal readonly ushort opcode;
            internal readonly byte[] data;
            internal readonly ushort chunkCount;
            internal readonly byte subId;
            internal readonly uint crc;
            internal ushort chunkId;
            internal byte tries = 3;

            public SendingBuffer(ushort opcode, byte subId, byte[] data)
            {
                this.opcode = opcode;
                this.subId = subId;
                this.data = data;
                this.chunkCount = (ushort) Mathf.Ceil(data.Length / 6144f);
                this.crc = CRC32.computeUnsigned(data);
            }
        }

        private List<SendingBuffer> buffers = new List<SendingBuffer>();
        public bool IsServer { get; set; }
        public string Identifier
        {
            get
            {
                if (IsServer) return "Server";
                return "Client";
            }
        }

        private SendingBuffer GetFor(ushort opcode, byte subId)
        {
            SendingBuffer buffer;
            for (int i = 0; i < buffers.Count; i++)
            {
                buffer = buffers[i];
                if (buffer.opcode == opcode && buffer.subId == subId)
                {
                    return buffer;
                }
            }
            return null;
        }

        private List<byte> IdsFor(ushort opcode)
        {
            List<byte> list = new List<byte>();
            SendingBuffer buffer;
            for (int i = 0; i < buffers.Count; i++)
            {
                buffer = buffers[i];
                if (buffer.opcode == opcode)
                {
                    list.Add(buffer.subId);
                }
            }
            return list;
        }

        public int Begin(ushort opcode, byte[] data, ref MsgBody output)
        {
            if (data.Length > ChunkedBufferReceiver.MAX_BUFFER_LIMIT)
            {
                Debug.LogWarning($"SNDR ({Identifier}): Buffer of message {opcode} is too big");
                return -1;
            }
            List<byte> bufList = IdsFor(opcode);
            if (bufList.Count == 255)
            {
                Debug.LogWarning($"SNDR ({Identifier}): There is too many buffers for message {opcode}");
                return -1;
            }
            byte subId = 0;
            for (; subId < 255; subId++)
            {
                if (!bufList.Contains(subId))
                {
                    break;
                }
            }
            SendingBuffer buffer = new SendingBuffer(opcode, subId, data);
            buffers.Add(buffer);
            output.Write(opcode);
            output.Write(subId);
            output.Write((uint) data.Length);
            output.Write(buffer.crc);
            return ExtensionOpcodes.opBeginChunkedBufferReq;
        }

        public int WriteChunk(MsgBody input, ref MsgBody output)
        {
            input.Read(out ushort opcode);
            input.Read(out byte subId);
            SendingBuffer buffer = GetFor(opcode, subId);
            if (buffer == null)
            {
                Debug.LogWarning($"SNDR ({Identifier}): No buffer for message {opcode} with sub id {subId}");
                return -1;
            }
            output.Write(opcode);
            output.Write(subId);
            if (buffer.chunkId == buffer.chunkCount)
            {
                return ExtensionOpcodes.opEndChunkedBufferReq;
            }
            ushort chunkId = buffer.chunkId++;
            output.Write(chunkId);
            byte[] data = new byte[Math.Min(buffer.data.Length - chunkId * ChunkedBufferReceiver.MAX_CHUNK_LENGTH, ChunkedBufferReceiver.MAX_CHUNK_LENGTH)];
            Array.Copy(buffer.data, chunkId * ChunkedBufferReceiver.MAX_CHUNK_LENGTH, data, 0, data.Length);
            output.Write(data);
            return ExtensionOpcodes.opChunkedBufferReq;
        }

        public int End(bool crcFailed, MsgBody input, ref MsgBody output)
        {
            input.Read(out ushort opcode);
            input.Read(out byte subId);
            SendingBuffer buffer = GetFor(opcode, subId);
            if (buffer == null)
            {
                Debug.LogWarning($"SNDR ({Identifier}): No buffer for message {opcode} with sub id {subId}");
                return -1;
            }
            if (crcFailed && buffer.tries != 0)
            {
                buffer.tries--;
                buffer.chunkId = 0;
                output.Write(opcode);
                output.Write(subId);
                output.Write((uint)buffer.data.Length);
                output.Write(buffer.crc);
                return ExtensionOpcodes.opBeginChunkedBufferReq;
            }
            buffers.Remove(buffer);
            return -1;
        }
    }
}
