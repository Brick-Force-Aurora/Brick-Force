using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _Emulator.Network.Gamemodes
{
    internal static class Freefall
    {
        internal static void HandleMatchEnd(MatchData matchData)
        {
            matchData.room.Status = Room.ROOM_STATUS.WAITING;
            SendMatchEnd(matchData);
            matchData.Reset();
            ServerEmulator.instance.SendRoom(null, matchData, SendType.BroadcastRoom);
        }

        private static void SendMatchEnd(MatchData matchData)
        {
            MsgBody body = new MsgBody();

            body.Write(matchData.clientList.Count);
            for (int i = 0; i < matchData.clientList.Count; i++)
            {
                body.Write(matchData.clientList[i].slot.isRed);
                body.Write(matchData.clientList[i].seq);
                body.Write(matchData.clientList[i].name);
                body.Write(matchData.clientList[i].kills);
                body.Write(matchData.clientList[i].deaths);
                body.Write(matchData.clientList[i].assists);
                body.Write(matchData.clientList[i].score);
                body.Write(0); //points
                body.Write(0); //xp
                body.Write(0); //mission
                body.Write(matchData.clientList[i].data.xp);
                body.Write(matchData.clientList[i].data.xp);
                body.Write((long)0); //buff
            }
            ServerEmulator.instance.Say(new MsgReference(476, body, null, SendType.BroadcastRoom, matchData.channel, matchData));
        }
    }
}
