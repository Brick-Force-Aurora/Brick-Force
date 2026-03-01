using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace _Emulator
{
    // Also known as Individual
    public class PlayDeathMatch : IGameMode
    {
        private readonly ServerEmulator emulator;

        public PlayDeathMatch(ServerEmulator emulator)
        {
            this.emulator = emulator;
        }

        public void RegisterNetworkHandlers(Action<MessageId, Action<MsgReference>> register, Action<ExtensionOpcodes, Action<MsgReference>> registerCustom)
        {

        }

        public void HandleRoomCreation(ClientReference clientRef, MatchData match, Room room, int[] parameters)
        {
            room.goal = parameters[0];
            room.timelimit = parameters[1];
            room.weaponOption = parameters[2];
            room.map = parameters[3];
            room.isBreakInto = Convert.ToBoolean(parameters[4]);
            match.isBalance = Convert.ToBoolean(parameters[5]);
            room.isWanted = Convert.ToBoolean(parameters[6]);
            room.isDropItem = /*Convert.ToBoolean(parameters[7]);*/ false;
            match.useBuildGun = false;
        }

        public void HandleMatchEnd(MatchData matchData)
        {
            matchData.room.Status = Room.ROOM_STATUS.WAITING;
            SendIndividualMatchEnd(matchData);
            matchData.Reset();
            emulator.SendUpdateRoom(matchData);
        }

        public void SendIndividualMatchEnd(MatchData matchData)
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
            emulator.Say(new MsgReference(180, body, null, SendType.BroadcastRoom, matchData.channel, matchData));

            if (emulator.debugSend)
                Debug.Log("Broadcasted SendIndivudalMatchEnd for room no: " + matchData.room.No);
        }
    }
}
