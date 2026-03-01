using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace _Emulator
{

    public class ImmediateSteamChannelHandler : SteamChannelHandler
    {

        private readonly object _lock = new object();
        private readonly Queue<Packet> packets = new Queue<Packet>();


        private readonly Action<CSteamID, byte[]> callback;

        public ImmediateSteamChannelHandler(Action<CSteamID, byte[]> callback)
        {
            this.callback = callback;
        }

        public void Enqueue(CSteamID source, byte[] message)
        {
            lock (_lock)
            {
                packets.Enqueue(new Packet(source, message));
            }
        }

        public void Dequeue()
        {
            Packet pkt;
            while (packets.Count > 0)
            {
                lock (_lock)
                {
                    pkt = packets.Dequeue();
                }
                callback(pkt.source, pkt.message);
            }
        }
    }
}
