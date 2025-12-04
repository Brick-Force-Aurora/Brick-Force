using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace _Emulator.Network.Gamemodes
{
    internal static class DefenseGamemode
    {
        internal static void HandleMatchEnd(MatchData matchData)
        {
            matchData.room.Status = Room.ROOM_STATUS.WAITING;
            SendMatchEnd(matchData);
            matchData.Reset();
            ServerEmulator.instance.SendRoom(null, matchData, SendType.BroadcastRoom);
        }

        static void SendMatchEnd(MatchData matchData)
        {
            for (int team = 0; team < 2; team++)
            {
                MsgBody body = new MsgBody();

                body.Write(team == 0 ? matchData.GetWinningTeam() : (sbyte)-matchData.GetWinningTeam());
                body.Write(matchData.redScore); //RedScore
                body.Write(matchData.blueScore); //BlueScore
                body.Write(matchData.blueScore); //RedTotalKill
                body.Write(matchData.redScore); //BluTotalKill
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
                ServerEmulator.instance.Say(new MsgReference((ushort)MessageId.CS_MISSION_END_ACK, body, null, team == 0 ? SendType.BroadcastBlueTeam : SendType.BroadcastRedTeam));
            }

            if (ServerEmulator.instance.debugSend)
                Debug.Log("Broadcasted SendDefenseMatchEnd for room no: " + matchData.room.No);
        }
    }
}
