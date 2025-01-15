using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _Emulator
{
    public enum SteamNetworkingChannel : int
    {
        Generic = 0,
        ToHost,
        ToClient,
        ToP2P
    }
}
