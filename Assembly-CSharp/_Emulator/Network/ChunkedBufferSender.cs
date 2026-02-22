using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using UnityEngine;

namespace _Emulator
{

    public static class ChunkedBufferSender
    {
        public static bool SendChunked(this MsgBody body, ushort opcode, byte sendKey, string identifier, Action<Msg4Send> sender)
        {
            int length = body.Offset;

            if (length > ChunkedBufferReceiver.MAX_BUFFER_LIMIT)
            {
                Debug.LogWarning($"SNDR ({identifier}): Buffer of message {opcode} is too big");
                return false;
            }

            byte[] data = body.Buffer;
            ushort chunkCount = (ushort)Mathf.Ceil(length / (float)ChunkedBufferReceiver.MAX_CHUNK_LENGTH);
            uint crc = CRC32.computeUnsigned(data, 0, length);

            MsgBody msgBody = new MsgBody();
            msgBody.Write(opcode);
            int startOffset = msgBody.Offset;

            msgBody.Write(length);
            msgBody.Write(crc);
            sender.Invoke(msgBody.NewMessage(ExtensionOpcodes.opBeginChunkedBufferReq, sendKey));

            int chunkLength, remainingLength = length;
            for (ushort chunkId = 0; chunkId < chunkCount; chunkId++)
            {
                body.Offset = startOffset;
                chunkLength = Math.Min(ChunkedBufferReceiver.MAX_CHUNK_LENGTH, remainingLength);
                msgBody.Write(chunkId);
                msgBody.Write(chunkLength);
                Array.Copy(data, chunkId * ChunkedBufferReceiver.MAX_CHUNK_LENGTH, data, body.Offset, chunkLength);
                body.Offset += chunkLength;
                remainingLength -= chunkLength;
                sender.Invoke(msgBody.NewMessage(ExtensionOpcodes.opChunkedBufferReq, sendKey));
            }

            body.Offset = startOffset;
            sender.Invoke(msgBody.NewMessage(ExtensionOpcodes.opEndChunkedBufferReq, sendKey));

            return true;
        }

        private static Msg4Send NewMessage(this MsgBody body, ExtensionOpcodes opcode, byte sendKey)
        {
            return new Msg4Send((ushort)opcode, uint.MaxValue, uint.MaxValue, body, sendKey);
        }
    }
}
