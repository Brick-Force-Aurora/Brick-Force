using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace _Emulator
{

    public class QueuedSteamChannelHandler : SteamChannelHandler
    {

        private readonly ReaderWriterLockSlim chLock = new ReaderWriterLockSlim();
        private readonly Queue<Packet> packets = new Queue<Packet>();

        public void Enqueue(CSteamID source, byte[] message)
        {
            try
            {
                chLock.EnterWriteLock();
                packets.Enqueue(new Packet(source, message));
            } finally
            {
                chLock.ExitWriteLock();
            }
        }

        public void Dequeue(Action<CSteamID, byte[]> action)
        {
            try
            {
                chLock.EnterUpgradeableReadLock();
                if (packets.Count <= 0)
                    return;
                try
                {
                    chLock.EnterWriteLock();
                    Packet pkt;
                    while (packets.Count > 0)
                    {
                        pkt = packets.Dequeue();
                        action(pkt.source, pkt.message);
                    }
                } finally
                {
                    chLock.ExitWriteLock();
                }
            } finally
            {
                chLock.ExitUpgradeableReadLock();
            }
        }
    }
}
