using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _Emulator
{
    public class PlayEscape : IGameMode
    {
        private const int GoalScore = 100;

        private readonly ServerEmulator emulator;

        public PlayEscape(ServerEmulator emulator)
        {
            this.emulator = emulator;
        }

        public void RegisterNetworkHandlers(Action<MessageId, Action<MsgReference>> register, Action<ExtensionOpcodes, Action<MsgReference>> registerCustom)
        {
            register(MessageId.CS_ESCAPE_GOAL_REQ, HandleGoalRequest);
            register(MessageId.CS_ESCAPE_SCORE_REQ, HandleScoreRequest);
            register(MessageId.CS_ESCAPE_ACTIVE_PLAYER_REQ, HandleActivePlayerRequest);
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
            room.isDropItem = Convert.ToBoolean(parameters[7]);
            match.useBuildGun = false;
        }

        private void HandleGoalRequest(MsgReference msgRef)
        {
            MatchData matchData = msgRef.matchData;
            ClientReference client = msgRef.client;
            if (matchData == null)
                return;

            client.kills++;
            client.score += GoalScore;
            matchData.redScore++;

            MsgBody goal = new MsgBody();
            goal.Write(client.seq);
            goal.Write(client.kills);
            goal.Write(client.score);
            emulator.Say(new MsgReference((ushort)MessageId.CS_ESCAPE_GOAL_ACK, goal, client, SendType.BroadcastRoom, matchData.channel, matchData));
            SendScore(matchData, client, SendType.BroadcastRoom);

            if (emulator.debugHandle)
                ServerDebugger.Log($"HandleGoalRequest: {client.GetIdentifier()} reached the goal ({client.kills} goals, room {matchData.redScore}/{matchData.room.goal})");

            if (matchData.redScore >= matchData.room.goal)
                HandleMatchEnd(matchData);
        }

        private void HandleScoreRequest(MsgReference msgRef)
        {
            if (msgRef.matchData != null)
                SendScore(msgRef.matchData, msgRef.client, SendType.Unicast);
        }

        private void HandleActivePlayerRequest(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out bool active);
        }

        private void SendScore(MatchData matchData, ClientReference client, SendType sendType)
        {
            MsgBody body = new MsgBody();
            body.Write(matchData.redScore);
            emulator.Say(new MsgReference((ushort)MessageId.CS_ESCAPE_SCORE_ACK, body, client, sendType, matchData.channel, matchData));
        }

        public void HandleMatchEnd(MatchData matchData)
        {
            matchData.room.Status = Room.ROOM_STATUS.WAITING;
            SendEscapeMatchEnd(matchData);
            matchData.Reset();
            emulator.SendUpdateRoom(matchData);
        }

        private void SendEscapeMatchEnd(MatchData matchData)
        {
            MsgBody body = new MsgBody();
            body.Write(matchData.clientList.Count);
            foreach (ClientReference client in matchData.clientList)
            {
                body.Write(client.slot.isRed);
                body.Write(client.seq);
                body.Write(client.name);
                body.Write(client.kills);   // goals
                body.Write(client.deaths);
                body.Write(client.assists);
                body.Write(client.score);
                body.Write(0);              // points
                body.Write(0);              // xp
                body.Write(0);              // mission
                body.Write(client.data.xp);
                body.Write(client.data.xp);
                body.Write((long)0);        // buff
                body.Write(0);              // param
            }
            emulator.Say(new MsgReference((ushort)MessageId.CS_ESCAPE_MODE_END_ACK, body, null, SendType.BroadcastRoom, matchData.channel, matchData));

            if (emulator.debugSend)
                ServerDebugger.Log("Broadcasted SendEscapeMatchEnd for room no: " + matchData.room.No);
        }
    }
}