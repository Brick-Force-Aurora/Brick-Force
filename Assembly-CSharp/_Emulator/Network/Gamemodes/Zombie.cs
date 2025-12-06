using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Debug = UnityEngine.Debug;

namespace _Emulator.Network.Gamemodes
{
    internal static class Zombie
    {
        internal static void HandleZombieStatusRequest(MsgReference msgRef)
        {
            if (msgRef.client.seq != msgRef.matchData.masterSeq)
            {
                return;
            }
            msgRef.msg._msg.Read(out int status);
            msgRef.msg._msg.Read(out int time);
            msgRef.msg._msg.Read(out int countDown);

            MatchData data = msgRef.matchData;
            MsgBody msg = new MsgBody();
            data.zombieStatus = (ZombieMatch.STEP)status;

            if (ServerEmulator.instance.debugHandle)
            {
                Debug.Log($"ZombieStatus: {status} Time: {time} Countdown: {countDown}");
            }

            switch ((ZombieMatch.STEP)status)
            {
                case ZombieMatch.STEP.WAITING:
                    Debug.Log("Waiting");
                    data.zombieCountdown = 0;
                    //status = 1;
                    break;
                case ZombieMatch.STEP.SET_POSITION:
                    // Countdown for setting positions
                    Debug.Log("SetPosition");
                    if (countDown > 0)
                    {
                        data.zombieCountdown = countDown - 1;
                    }
                    else
                    {
                        data.zombieCountdown = 10;
                        //status = 2;
                    }
                    break;
                case ZombieMatch.STEP.ZOMBIE:
                    Debug.Log("Zombie");
                    break;
                case ZombieMatch.STEP.ZOMBIE_PLAY:
                    Debug.Log("Zombie Play");
                    if (data.humanPlayers.Count == 0 || data.zombiePlayers.Count == 0)
                    {
                        if (!data.roundInit)
                        {
                            SendZombieRoundEnd(msgRef, data);
                        }
                    }
                    if (data.remainTime <= 0 && data.zombieRoundsLeft < 0)
                    {
                        data.EndMatch();
                        return;
                    }
                    //time = 0;\
                    if (data.zombieCountdown != 10)
                    {
                        data.zombieCountdown = 10;
                    }
                    break;
                default:
                    Debug.LogWarning("Unknown ZombieMatch.STEP received.");
                    break;
            }

            msg.Write(status);
            msg.Write(data.playTime);
            msg.Write(data.zombieCountdown);

            ServerEmulator.instance.Say(new MsgReference((ushort)MessageId.CS_ZOMBIE_STATUS_ACK, msg, msgRef.client, SendType.BroadcastRoom, data.channel, data));
        }

        internal static void HandleZombieObserverRequest(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out int seq); //MyInfoManager.Instance.Seq
            MatchData data = msgRef.client.matchData;
            data.zombiePlayers.Remove(seq);
            data.killedPlayers.Add(seq);
            if (ServerEmulator.instance.debugHandle)
            {
                Debug.LogWarning("ZombieObserver: Zombies:" + data.zombiePlayers.Count + " Humans: " + data.humanPlayers.Count);
            }
            if (data.zombiePlayers.Count == 0)
            {
                if (data.zombieRoundsLeft > 0)
                {
                    SendZombieRoundEnd(msgRef, data);
                }
                else
                {
                    data.EndMatch();
                }
            }
            // No Response
        }

        internal static void HandleZombieInfectionRequest(MsgReference msgRef)
        {
            MatchData data = msgRef.matchData;
            //Debug.LogWarning("InfectionRequest current status: " + data.zombieStatus);

            if (data.roundInit)
            {
                int numPlayers = data.clientList.Count;
                int numZombies;

                if (numPlayers <= 4)
                {
                    numZombies = 1;
                }
                else if (numPlayers <= 10)
                {
                    numZombies = 2;
                }
                else if (numPlayers <= 15)
                {
                    numZombies = 3;
                }
                else
                {
                    numZombies = 4;
                }

                // Clone the client list to select randomly
                List<ClientReference> shuffledClients = new List<ClientReference>(data.clientList);

                // Shuffle the list to ensure randomness
                for (int i = 0; i < shuffledClients.Count; i++)
                {
                    int randomIndex = UnityEngine.Random.Range(i, shuffledClients.Count);

                    // Manually swap elements
                    var temp = shuffledClients[i];
                    shuffledClients[i] = shuffledClients[randomIndex];
                    shuffledClients[randomIndex] = temp;
                }

                // Select the first `numZombies` clients as zombies
                data.zombiePlayers.Clear();
                for (int i = 0; i < numZombies; i++)
                {
                    data.clientList.Find(x => x.seq == shuffledClients[i].seq).isZombie = true;
                    data.zombiePlayers.Add(shuffledClients[i].seq);
                }

                // Add the remaining clients to the human players list
                data.humanPlayers.Clear();
                for (int i = numZombies; i < shuffledClients.Count; i++)
                {
                    data.humanPlayers.Add(shuffledClients[i].seq);
                }

                // Mark the round as initialized
                data.roundInit = false;

                if (ServerEmulator.instance.debugSend)
                {
                    Debug.Log($"Initialized round: {numZombies} zombies and {data.humanPlayers.Count} humans.");
                }
            }

            // Create a new message body for the response
            MsgBody msg = new MsgBody();

            // Add human IDs
            msg.Write(data.humanPlayers.Count); // Write the count of humans
            foreach (int humanId in data.humanPlayers)
            {
                msg.Write(humanId); // Write each human player ID
            }

            // Add zombie IDs
            msg.Write(data.zombiePlayers.Count); // Write the count of zombies
            foreach (int zombieId in data.zombiePlayers)
            {
                msg.Write(zombieId); // Write each zombie player ID
            }

            // Add IDs of players who died (killed)
            msg.Write(data.killedPlayers.Count); // Write the count of killed players
            foreach (int killedId in data.killedPlayers)
            {
                msg.Write(killedId); // Write each killed player ID
            }

            // Add IDs of players who became infected (turned into zombies)
            msg.Write(data.infectedPlayers.Count); // Write the count of infected players
            foreach (int infectedId in data.infectedPlayers)
            {
                msg.Write(infectedId); // Write each infected player ID
            }

            // Broadcast the response to all players in the room
            ServerEmulator.instance.Say(new MsgReference((ushort)MessageId.CS_ZOMBIE_INFECTION_ACK, msg, null, SendType.BroadcastRoom, data.channel, data));

            if (ServerEmulator.instance.debugSend)
            {
                Debug.Log("Zombie infection state updated and broadcasted.");
            }
        }

        internal static void HandleZombieInfectRequest(MsgReference msgRef)
        {
            msgRef.msg._msg.Read(out int brickMan);
            msgRef.msg._msg.Read(out int zombie);

            MatchData data = msgRef.matchData;
            // Update human and zombie player lists
            if (data.humanPlayers.Contains(brickMan))
            {
                data.humanPlayers.Remove(brickMan);
                data.zombiePlayers.Add(brickMan);
                ClientReference client = data.clientList[zombie];
                client.kills += 1;
            }

            if (data.humanPlayers.Count == 0)
            {
                SendZombieRoundEnd(msgRef, data);
            }

            MsgBody msg = new MsgBody();
            msg.Write(brickMan); // Infected player
            msg.Write(zombie);   // Infecting player
            ServerEmulator.instance.Say(new MsgReference((ushort)MessageId.CS_ZOMBIE_INFECT_ACK, msg, null, SendType.BroadcastRoom, data.channel, data));

            if (ServerEmulator.instance.debugSend)
            {
                Debug.Log($"Zombie Infection: {zombie} infected {brickMan}. Remaining humans: {data.humanPlayers.Count}, Remaining Zombies: {data.zombiePlayers.Count}");
            }
        }

        public static void SendZombieRoundEnd(MsgReference msgRef, MatchData data)
        {
            Debug.LogWarning($"Send RoundEnd client {msgRef.client.GetIdentifier()} data: {data.ToString()}");

            if (data.zombieCurrentRound >= data.room.goal)
            {
                data.EndMatch();
                return;
            }

            MsgBody msg = new MsgBody();
            msg.Write(data.zombieCurrentRound);
            if (data.humanPlayers.Count == 0)
            {
                //Zombies win
                msg.Write((sbyte)-1);
                msg.Write((sbyte)1);
                msg.Write(0); //roundcode (unused in zombie)
            }
            else
            {
                //Humans win
                msg.Write((sbyte)1);
                msg.Write((sbyte)-1);
                msg.Write(0); //roundcode (unused in zombie)
            }
            data.ResetForNewRound();
            data.zombieCurrentRound += 1;
            data.zombieRoundsLeft -= 1;

            HandleZombieScoreRequest(msgRef);

            ServerEmulator.instance.Say(new MsgReference((ushort)MessageId.CS_ROUND_END_ACK, msg, null, SendType.BroadcastRoom, data.channel, data));
        }

        internal static void HandleZombieScoreRequest(MsgReference msgRef)
        {
            // Ensure debug logging is only performed if debugSend is enabled
            if (ServerEmulator.instance.debugSend)
            {
                Debug.Log($"Zombie Score Request: Total Rounds: {msgRef.matchData.room.goal}, " +
                          $"Current Round: {msgRef.matchData.zombieCurrentRound}");
            }

            // Prepare the message body with total rounds and current round
            MsgBody msg = new MsgBody();
            msg.Write(msgRef.matchData.room.goal); // Total rounds
            msg.Write(msgRef.matchData.zombieCurrentRound); // Current round

            // Send the message as a broadcast
            ServerEmulator.instance.Say(new MsgReference((ushort)MessageId.CS_ZOMBIE_MODE_SCORE_ACK, msg, null, SendType.BroadcastRoom, msgRef.matchData.channel, msgRef.matchData));
        }

        internal static void HandleZombieMatchEnd(MatchData matchData)
        {
            matchData.room.Status = Room.ROOM_STATUS.WAITING;
            SendZombieMatchEnd(matchData);
            matchData.Reset();
            ServerEmulator.instance.SendRoom(null, matchData, SendType.BroadcastRoom);
        }

        private static void SendZombieMatchEnd(MatchData matchData)
        {
            MsgBody body = new MsgBody();

            body.Write(matchData.clientList.Count);
            for (int i = 0; i < matchData.clientList.Count; i++)
            {
                int points = (int)(matchData.clientList[i].score * 0.4);
                int xp = (int)(matchData.clientList[i].score * 0.5);
                body.Write(matchData.clientList[i].slot.isRed);
                body.Write(matchData.clientList[i].seq);
                body.Write(matchData.clientList[i].name);
                body.Write(0); // survival ensured
                body.Write(0); // zombie victory
                body.Write(matchData.clientList[i].assists);
                body.Write(matchData.clientList[i].score);
                body.Write(points); //points
                body.Write(xp); //xp
                body.Write(0); //mission
                body.Write(matchData.clientList[i].data.xp);
                body.Write(matchData.clientList[i].data.xp + xp);
                body.Write((long)0); //buff
            }

            ServerEmulator.instance.Say(new MsgReference((ushort)MessageId.CS_ZOMBIE_END_ACK, body, null, SendType.BroadcastRoom, matchData.channel, matchData));

            if (ServerEmulator.instance.debugSend)
                Debug.Log($"Broadcasted SendZombieMatchEnd for room no: {matchData.room.No}");
        }

        //ROUND END
    }
}
