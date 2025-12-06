using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Debug = UnityEngine.Debug;

namespace _Emulator.Network.Gamemodes
{
    internal static class CTF
    {
        internal static void HandlePickFlagRequest(MsgReference msgRef)
        {
            MatchData data = msgRef.matchData;
            msgRef.msg._msg.Read(out int flag);
            if (ServerEmulator.instance.debugHandle)
                Debug.Log("HandlePickFlag from: " + msgRef.client.GetIdentifier() + " FlagId: " + flag);
            MsgBody msg = new MsgBody();
            // i think the response needs to be the plaxer seq which picked up the flag

            msg.Write(msgRef.client.seq);
            ServerEmulator.instance.Say(new MsgReference((ushort)MessageId.CS_CTF_PICK_FLAG_ACK, msg, msgRef.client, SendType.BroadcastRoom, data.channel, data));
        }

        internal static void HandleDropFlagRequest(MsgReference msgRef)
        {
            MatchData data = msgRef.matchData;
            msgRef.msg._msg.Read(out int x);
            msgRef.msg._msg.Read(out int y);
            msgRef.msg._msg.Read(out int z);
            if (ServerEmulator.instance.debugHandle)
                Debug.Log("HandleFlagDrop from: " + msgRef.client.GetIdentifier() + " Flag: x: " + x + " y: " + y + " z: " + z);
            MsgBody msg = new MsgBody();
            msg.Write(0); //unused
            msg.Write(0); // unused
            msg.Write(x);
            msg.Write(y);
            msg.Write(z);

            ServerEmulator.instance.Say(new MsgReference((ushort)MessageId.CS_CTF_DROP_FLAG_ACK, msg, msgRef.client, SendType.BroadcastRoom, data.channel, data));
        }

        internal static void HandleCaptureFlagRequest(MsgReference msgRef)
        {
            MatchData data = msgRef.matchData;
            msgRef.msg._msg.Read(out int flag);
            msgRef.msg._msg.Read(out bool opponent);
            if (msgRef.client.slot.slotIndex > 7)
            {
                data.blueScore++;
            }
            else
            {
                data.redScore++;
            }
            ServerEmulator.instance.SendTeamScore(data);
            data.ResetForNewRound();
            if (ServerEmulator.instance.debugHandle)
                Debug.Log("HandleCaptureFlag from: " + msgRef.client.GetIdentifier() + " FlagId: " + flag + " IsOpponent: " + opponent);
            MsgBody msg = new MsgBody();
            msg.Write(msgRef.client.seq); // Player sequence
            //Round only starts for one player (oponent?)

            ServerEmulator.instance.Say(new MsgReference((ushort)MessageId.CS_CTF_CAPTURE_FLAG_ACK, msg, msgRef.client, SendType.BroadcastRoom, data.channel, data));
        }

        internal static void HandleCTFScoreRequest(MsgReference msgRef)
        {
            MatchData data = msgRef.matchData;
            MsgBody msg = new MsgBody();
            msg.Write(data.redScore); // red score
            msg.Write(data.blueScore); // blue score
            if (ServerEmulator.instance.debugHandle)
                Debug.Log("HandleCTFScoreReq from: " + msgRef.client.GetIdentifier() + " RedScore: " + data.redScore + " BluScore: " + data.blueScore);

            ServerEmulator.instance.Say(new MsgReference((ushort)MessageId.CS_CTF_SCORE_ACK, msg, msgRef.client, SendType.BroadcastRoom, data.channel, data));
        }

        internal static void HandleFlagReturnRequest(MsgReference msgRef)
        {
            MatchData data = msgRef.matchData;
            msgRef.msg._msg.Read(out float x);
            msgRef.msg._msg.Read(out float y);
            msgRef.msg._msg.Read(out float z);
            if (ServerEmulator.instance.debugHandle)
                Debug.Log("HandleFlagReturn from: " + msgRef.client.GetIdentifier() + " Flag: x: " + x + " y: " + y + " z: " + z);
            MsgBody msg = new MsgBody();

            ServerEmulator.instance.Say(new MsgReference((ushort)MessageId.CS_CTF_FLAG_RETURN_ACK, msg, msgRef.client, SendType.BroadcastRoom, data.channel, data));
        }


        internal static void HandleCTFMatchEnd(MatchData matchData)
        {
            matchData.room.Status = Room.ROOM_STATUS.WAITING;
            SendCTFMatchEnd(matchData);
            matchData.Reset();
            ServerEmulator.instance.SendRoom(null, matchData, SendType.BroadcastRoom);
        }

        private static void SendCTFMatchEnd(MatchData matchData)
        {
            for (int team = 0; team < 2; team++)
            {
                MsgBody body = new MsgBody();

                body.Write(team == 0 ? matchData.GetWinningTeam() : (sbyte)-matchData.GetWinningTeam());
                body.Write(matchData.redScore); //RedScore
                body.Write(matchData.blueScore); //BlueScore
                body.Write(matchData.redKillCount); //RedTotalKill
                body.Write(matchData.blueKillCount); //BluTotalKill
                body.Write(matchData.blueKillCount); //RedTotalDeath
                body.Write(matchData.redKillCount); //BlueTotalDeath
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
                ServerEmulator.instance.Say(new MsgReference((ushort)MessageId.CS_CAPTURE_THE_FLAG_END_ACK, body, null, team == 0 ? SendType.BroadcastBlueTeam : SendType.BroadcastRedTeam));
            }

            if (ServerEmulator.instance.debugSend)
                Debug.Log("Broadcasted SendCTFMatchEnd for room no: " + matchData.room.No);
        }

    }
}
