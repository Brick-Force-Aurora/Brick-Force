using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace _Emulator
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
    public interface SteamChannelHandler
    {
        void Enqueue(CSteamID source, byte[] message);
    }
}
