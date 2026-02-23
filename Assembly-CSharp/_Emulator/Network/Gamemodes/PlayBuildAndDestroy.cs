using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Debug = UnityEngine.Debug;

namespace _Emulator.Network.Gamemodes
{
    public class PlayBuildAndDestroy : IGameMode
    {
        private readonly ServerEmulator emulator;

        public PlayBuildAndDestroy(ServerEmulator emulator)
        {
            this.emulator = emulator;
        }

        public void RegisterNetworkHandlers(Action<MessageId, Action<MsgReference>> register, Action<ExtensionOpcodes, Action<MsgReference>> registerCustom)
        {
            register(MessageId.CS_BND_SCORE_REQ, HandleBNDScoreRequest);
            register(MessageId.CS_BND_SHIFT_PHASE_REQ, HandleBNDShiftPhaseRequest);
        }

        public void HandleRoomCreation(ClientReference clientRef, MatchData match, Room room, int[] parameters)
        {
            room.goal = parameters[0];
            room.timelimit = parameters[1];
            room.weaponOption = parameters[2];
            room.map = parameters[3];
            room.isBreakInto = Convert.ToBoolean(parameters[4]);
            match.isBalance = Convert.ToBoolean(parameters[5]);
            match.useBuildGun = Convert.ToBoolean(parameters[6]);
            room.isDropItem = Convert.ToBoolean(parameters[7]);

            UnpackTimerOption(room.timelimit, out int buildTime, out int battleTime, out int repeats);
            match.buildPhaseTime = buildTime;
            match.battlePhaseTime = battleTime;
            match.repeat = repeats;

            match.CacheMap(emulator.regMaps.Find(x => x.Value.Map == room.map).Value, new UserMapInfo(0, 0));
        }

        public void HandleBNDScoreRequest(MsgReference msgRef)
        {
            MatchData data = msgRef.matchData;
            MsgBody msg = new MsgBody();
            msg.Write(data.redScore); // red score
            msg.Write(data.blueScore); // blue score
            if (emulator.debugHandle)
                Debug.Log("HandleBNDScoreReq from: " + msgRef.client.GetIdentifier() + " RedScore: " + data.redScore + " BluScore: " + data.blueScore);

            emulator.Say(new MsgReference(MessageId.CS_BND_SCORE_ACK, msg, msgRef.client, SendType.BroadcastRoom, data.channel, data));
        }

        public void HandleBNDShiftPhaseRequest(MsgReference msgRef)
        {
            Debug.LogWarning("recieved Shift Phase Req");
            msgRef.msg._msg.Read(out int repeat);
            MatchData matchData = msgRef.client.matchData;
            Debug.LogWarning("matchData old repeat: " + matchData.repeat + " msg repeat: " + repeat);
            matchData.repeat = repeat;
            if (repeat <= 0)
            {
                matchData.EndMatch();
            }

            msgRef.msg._msg.Read(out bool isBuildPhase);
            Debug.LogWarning("matchData old isBuildPhase: " + matchData.isBuildPhase + " msg isBuild: " + isBuildPhase);
            matchData.isBuildPhase = isBuildPhase;
            SendShiftPhase(msgRef.client, repeat, isBuildPhase);
        }

        private void SendShiftPhase(ClientReference client, int repeat, bool isBuildPhase)
        {
            MatchData matchData = client.matchData;
            matchData.ResetForNewRound();

            MsgBody body = new MsgBody();

            body.Write(repeat);
            body.Write(isBuildPhase);

            emulator.Say(new MsgReference(MessageId.CS_BND_SHIFT_PHASE_ACK, body, client, SendType.BroadcastRoom, matchData.channel, matchData));
        }

        public void HandleMatchEnd(MatchData matchData)
        {
            matchData.room.Status = Room.ROOM_STATUS.WAITING;
            SendBNDMatchEnd(matchData);
            matchData.Reset();
            emulator.SendUpdateRoom(matchData);
        }
        public void SendBNDMatchEnd(MatchData matchData)
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
                emulator.Say(new MsgReference(MessageId.CS_BND_MODE_END_ACK, body, null, team == 0 ? SendType.BroadcastBlueTeam : SendType.BroadcastRedTeam, matchData.channel, matchData));
            }

            if (emulator.debugSend)
                Debug.Log("Broadcasted SendBNDMatchEnd for room no: " + matchData.room.No);
        }

        public void SendBnDStatus(ClientReference client)
        {
            MsgBody body = new MsgBody();
            body.Write(client.matchData.isBuildPhase);
            emulator.Say(new MsgReference(MessageId.CS_BND_STATUS_ACK, body, client));
        }
        public void SendBnDScore(MatchData matchData)
        {
            MsgBody body = new MsgBody();
            body.Write(matchData.redScore);
            body.Write(matchData.blueScore);

            emulator.Say(new MsgReference(MessageId.CS_BND_SCORE_ACK, body, null, SendType.BroadcastRoom, matchData.channel, matchData));

            if (emulator.debugSend)
                Debug.Log("Broadcasted SendTeamScore for room no: " + matchData.room.No);
        }

        public static int PackTimerOptions(int build, int battle, int rpt)
        {
            return (rpt & 0xFF) | (((battle / 60) & 0xFF) << 8) | (((build / 60) & 0xFF) << 16);
        }

        public static void UnpackTimerOption(int packed, out int build, out int battle, out int rpt)
        {
            // Extract `rpt` (last 8 bits)
            rpt = packed & 0xFF;

            // Extract `battle` (middle 8 bits)
            battle = (packed >> 8) & 0xFF;

            // Extract `build` (first 8 bits)
            build = (packed >> 16) & 0xFF;

            // Convert back to seconds
            build *= 60;
            battle *= 60;
        }

    }
}
