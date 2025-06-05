using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _Emulator.Network.Gamemodes
{
    internal static class DefenseGamemode
    {
        internal static void HandleMatchEnd(MatchData matchData)
        {
            matchData.room.Status = Room.ROOM_STATUS.WAITING;
            //SendMatchEnd(matchData);
            matchData.Reset();
            ServerEmulator.instance.SendRoom(null, matchData, SendType.BroadcastRoom);
        }
    }
}
