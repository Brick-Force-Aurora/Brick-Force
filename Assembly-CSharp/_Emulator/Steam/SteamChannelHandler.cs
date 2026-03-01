using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace _Emulator.Steam
{

    public struct Packet
    {
        public CSteamID source;
        public byte[] message;

        public Packet(CSteamID source, byte[] message) : this()
        {
            this.source = source;
            this.message = message;
        }
    }

    public class SteamChannelHandler
    {

        public readonly SteamNetworkingChannel channel;
        private readonly ReaderWriterLock chLock = new ReaderWriterLock();
        private readonly Queue<Packet> packets = new Queue<Packet>();

        public SteamChannelHandler(SteamNetworkingChannel channel)
        {
            this.channel = channel;
        }

        public void Enqueue(CSteamID source, byte[] message)
        {
            chLock.AcquireWriterLock(250);
            try
            {
                packets.Enqueue(new Packet(source, message));
            } finally
            {
                chLock.ReleaseWriterLock();
            }
        }

        public void Dequeue(Action<CSteamID, byte[]> action)
        {
            chLock.AcquireReaderLock(250);
            try
            {
                if (packets.Count <= 0)
                    return;
                chLock.UpgradeToWriterLock(250);
                Packet pkt;
                while (packets.Count > 0)
                {
                    pkt = packets.Dequeue();
                    action(pkt.source, pkt.message);
                }
            } finally
            {
                chLock.ReleaseLock();
            }
        }
    }
}
