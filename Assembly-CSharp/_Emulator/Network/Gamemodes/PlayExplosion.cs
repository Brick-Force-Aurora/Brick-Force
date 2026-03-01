using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Debug = UnityEngine.Debug;

namespace _Emulator
{
    //Also known as BlastMode / Defusion
    public class PlayExplosion : IGameMode
    {
        private readonly ServerEmulator emulator;

        public PlayExplosion(ServerEmulator emulator)
        {
            this.emulator = emulator;
        }

        public void RegisterNetworkHandlers(Action<MessageId, Action<MsgReference>> register, Action<ExtensionOpcodes, Action<MsgReference>> registerCustom)
        {
            register(MessageId.CS_BM_INSTALL_BOMB_REQ, HandleBombInstallRequest);
            register(MessageId.CS_BM_UNINSTALL_BOMB_REQ, HandleBombUninstallRequest);
            register(MessageId.CS_BM_BLAST_REQ, HandleBombBlastRequest);
            register(MessageId.CS_BLAST_MODE_SCORE_REQ, HandleScoreRequest);
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

        public void HandleBombInstallRequest(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out int bomb);
            msgRef.msg._msg.Read(out int x);
            msgRef.msg._msg.Read(out int y);
            msgRef.msg._msg.Read(out int z);
            msgRef.msg._msg.Read(out int nx);
            msgRef.msg._msg.Read(out int ny);
            msgRef.msg._msg.Read(out int nz);
            MatchData data = msgRef.matchData;
            MsgBody msg = new MsgBody();
            msg.Write(msgRef.client.seq); //Installer
            msg.Write(bomb); //target
            msg.Write(x);
            msg.Write(y);
            msg.Write(z);
            msg.Write(nx);
            msg.Write(ny);
            msg.Write(nz);
            ServerEmulator.instance.Say(new MsgReference((ushort)MessageId.CS_BM_INSTALL_BOMB_ACK, msg, msgRef.client, SendType.BroadcastRoom, data.channel, data));
        }
        public void HandleBombUninstallRequest(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out int bomb);
            MatchData data = msgRef.matchData;
            MsgBody msg = new MsgBody();
            data.ResetForNewRound();
            data.blueScore = data.blueScore + 1;
            msg.Write(bomb);
            msg.Write(0); //unused
            ServerEmulator.instance.Say(new MsgReference((ushort)MessageId.CS_BM_UNINSTALL_BOMB_ACK, msg, msgRef.client, SendType.BroadcastRoom, data.channel, data));
            if (data.blueScore >= data.room.goal)
            {
                data.EndMatch();
            }
            else
            {
                msg = new MsgBody();
                msg.Write(data.currentRound); //on Round End
                msg.Write(-1); //endCode4Red
                msg.Write(1); //endCode4Blue
                msg.Write(4); //roundCode
                ServerEmulator.instance.Say(new MsgReference((ushort)MessageId.CS_ROUND_END_ACK, msg, msgRef.client, SendType.BroadcastRoom, data.channel, data));
                data.ResetForNewRound();
            }
        }
        public void HandleBombBlastRequest(MsgReference msgRef)
        {
            MatchData data = msgRef.matchData;
            MsgBody msg = new MsgBody();
            data.ResetForNewRound();
            data.redScore = data.redScore + 1;
            //returns two values which are not used
            msg.Write(0);
            msg.Write(0);
            ServerEmulator.instance.Say(new MsgReference((ushort)MessageId.CS_BM_BLAST_ACK, msg, msgRef.client, SendType.BroadcastRoom, data.channel, data));
            //score limit?
            if (data.redScore >= data.room.goal)
            {
                data.EndMatch();
            }
            else
            {
                //start next round?
                msg = new MsgBody();
                msg.Write(data.currentRound); //on Round End
                msg.Write(1); //endCode4Red
                msg.Write(-1); //endCode4Blue
                msg.Write(3); //roundCode
                ServerEmulator.instance.Say(new MsgReference((ushort)MessageId.CS_ROUND_END_ACK, msg, msgRef.client, SendType.BroadcastRoom, data.channel, data));
                data.ResetForNewRound();
            }
        }
        public void HandleScoreRequest(MsgReference msgRef)
        {

        }

        public void HandleMatchEnd(MatchData matchData)
        {
            matchData.room.Status = Room.ROOM_STATUS.WAITING;
            SendMatchEnd(matchData);
            matchData.Reset();
            ServerEmulator.instance.SendUpdateRoom(matchData);
        }

        static void SendMatchEnd(MatchData matchData)
        {
            //fix this
            for (int team = 0; team < 2; team++)
            {
                MsgBody body = new MsgBody();

                body.Write(team == 0 ? matchData.GetWinningTeam() : (sbyte)-matchData.GetWinningTeam());
                body.Write(matchData.redScore); //RedScore
                body.Write(matchData.blueScore); //BlueScore
                body.Write(0); //RedMission
                body.Write(0); //BlueMission
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
                ServerEmulator.instance.Say(new MsgReference((ushort)MessageId.CS_BLAST_MODE_END_ACK, body, null, team == 0 ? SendType.BroadcastBlueTeam : SendType.BroadcastRedTeam, matchData.channel, matchData));
            }

            if (ServerEmulator.instance.debugSend)
                Debug.Log("Broadcasted SendDefusionMatchEnd for room no: " + matchData.room.No);
        }

        public void SendScore(MatchData matchData)
        {
            MsgBody body = new MsgBody();
            body.Write(matchData.redScore);
            body.Write(matchData.blueScore);

            ServerEmulator.instance.Say(new MsgReference(67, body, null, SendType.BroadcastRoom, matchData.channel, matchData));

            if (ServerEmulator.instance.debugSend)
                Debug.Log("Broadcasted SendDefusionScore for room no: " + matchData.room.No);
        }

        public void HandleRoundEnd(MsgReference msgRef, sbyte roundCode)
        {
            MatchData data = msgRef.matchData;
            MsgBody msg = new MsgBody();
            msg = new MsgBody();
            msg.Write(data.currentRound); //on Round End
            if (roundCode == 0)
            {
                msg.Write(1); //endCode4Red
                msg.Write(-1); //endCode4Blue
            } else
            {
                msg.Write(-1); //endCode4Red
                msg.Write(1); //endCode4Blue
            }
            msg.Write(roundCode); //roundCode
            ServerEmulator.instance.Say(new MsgReference((ushort)MessageId.CS_ROUND_END_ACK, msg, msgRef.client, SendType.BroadcastRoom, data.channel, data));
            SendScore(data);

            if (data.redScore >= data.room.goal || data.blueScore >= data.room.goal)
            {
                if (ServerEmulator.instance.debugHandle)
                {
                    Debug.LogWarning("Explosion match end: goal reached.");
                }
                data.EndMatch();
                return;
            }

            // Schedule round reset
            data.ResetForNewRound();
        }
    }
}
