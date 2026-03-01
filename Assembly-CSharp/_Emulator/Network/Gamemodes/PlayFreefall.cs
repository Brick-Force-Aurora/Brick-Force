using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace _Emulator
{
    // Also known as Bungee
    public class PlayFreefall : IGameMode
    {
        private readonly ServerEmulator emulator;

        public PlayFreefall(ServerEmulator emulator)
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
            match.CacheMap(emulator.regMaps.Find(x => x.Value.Map == room.map).Value, new UserMapInfo(0, 0));
            match.useBuildGun = true;
        }

        public void HandleMatchEnd(MatchData matchData)
        {
            matchData.room.Status = Room.ROOM_STATUS.WAITING;
            SendMatchEnd(matchData);
            matchData.Reset();
            ServerEmulator.instance.SendUpdateRoom(matchData);
        }

        public void HandleRoomCreation(MatchData matchData, Room room, int kills, int timeLimit, int map, bool breakInto)
        {
            room.goal = kills;
            room.timelimit = timeLimit;
            room.map = map;
            room.isBreakInto = breakInto;
        }

        private void SendMatchEnd(MatchData matchData)
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
                body.Write(matchData.clientList[i].kills); //param kill?
            }
            ServerEmulator.instance.Say(new MsgReference(476, body, null, SendType.BroadcastRoom, matchData.channel, matchData));
        }

        public void SendFreefallScore(MatchData matchData)
        {
            MsgBody body = new MsgBody();

            body.Write(matchData.redScore);

            ServerEmulator.instance.Say(new MsgReference(475, body, null, SendType.BroadcastRoom, matchData.channel, matchData));

            if (ServerEmulator.instance.debugSend)
                Debug.Log("Broadcasted SendFreefallScore for room no: " + matchData.room.No);
        }
    }
}
