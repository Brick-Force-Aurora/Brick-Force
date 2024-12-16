using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Emulator
{
    class MatchData
    {
        public int countdownTime;
        public int remainTime;
        public int playTime;
        public int blueScore;
        public int redScore;
        public ClientReference highestKillsClient;
        public int lobbyCountdownTime;
        public int masterSeq;
        public bool isBalance;
        public bool roomCreated;
        public int lastKillLogId = -1;
        public List<ClientReference> clientList;
        public List<SlotData> slots;
        public List<SlotData> blueSlots;
        public List<SlotData> redSlots;
        public List<int> destroyedBricks;
        public Dictionary<int, int> usedCannons;
        public Dictionary<int, int> usedTrains;
        public List<KillLogEntry> killLog;
        public ChannelReference channel;
        public Room room;

        //CTF
        public int ctfRedKillCount;
        public int ctfBlueKillCount;

        //Build only
        public UserMap cachedMap;
        public UserMapInfo cachedUMI;
        public bool mapCached;

        //BND
        public bool isBuildPhase;    // Current phase: true = Build, false = Destroy
        public bool useBuildGun;
        public int repeat;      // Total number of Build-and-Destroy rounds
        public int currentRound;     // Current round number
        public int buildPhaseTime;   // Time (in seconds) for Build phase
        public int battlePhaseTime;  // Time (in seconds) for Destroy phase

        public MatchData()
        {
            countdownTime = 0;
            remainTime = 0;
            playTime = 0;
            blueScore = 0;
            redScore = 0;
            highestKillsClient = null;
            lobbyCountdownTime = 0;
            masterSeq = 0;
            isBalance = false;
            roomCreated = false;
            clientList = new List<ClientReference>();
            slots = new List<SlotData>();
            blueSlots = new List<SlotData>();
            redSlots = new List<SlotData>();
            killLog = new List<KillLogEntry>();
            destroyedBricks = new List<int>();
            usedCannons = new Dictionary<int, int>();
            usedTrains = new Dictionary<int, int>();
            for (int i = 0; i < 16; i++)
                slots.Add(new SlotData(i));
            List<List<SlotData>> split = Utils.SplitList<SlotData>(slots, 8);
            redSlots = split[0];
            blueSlots = split[1];
            isBuildPhase = true;
            repeat = 0;
            currentRound = 1;
            buildPhaseTime = 0;
            battlePhaseTime = 0;

            for (int i = 0; i < redSlots.Count; i++)
                redSlots[i].isRed = true;

            ctfRedKillCount = 0;
            ctfBlueKillCount = 0;
            room = new Room(false, 0, "", Room.ROOM_TYPE.TEAM_MATCH, Room.ROOM_STATUS.WAITING, 0, 0, 0, "", 0, 0, 0, 0, 0, 0, 0, false, false, false, 0, 0);
            cachedMap = new UserMap();
            mapCached = false;
        }

        public void Reset()
        {
            blueScore = 0;
            redScore = 0;
            highestKillsClient = null;
            destroyedBricks.Clear();
            usedCannons.Clear();
            usedTrains.Clear();
            killLog.Clear();
            ctfRedKillCount = 0;
            ctfBlueKillCount = 0;
            isBuildPhase = true;
            repeat = 0;
            currentRound = 1;
            buildPhaseTime = 0;
            battlePhaseTime = 0;
            room.Status = Room.ROOM_STATUS.WAITING;
            for (int i = 0; i < clientList.Count; i++)
            {
                clientList[i].clientStatus = ClientReference.ClientStatus.Room;
                clientList[i].deaths = 0;
                clientList[i].kills = 0;
                clientList[i].assists = 0;
                clientList[i].score = 0;
            }
        }

        // Method to reset data for a new round
        public void ResetForNewRound()
        {
            // Reset round-specific data
            remainTime = countdownTime; // assuming countdownTime is set to the desired round duration
            playTime = 0;
            destroyedBricks.Clear();
            usedCannons.Clear();
            usedTrains.Clear();
            killLog.Clear();
        }

        public void Shutdown()
        {
            foreach (ClientReference client in clientList)
            {
                client.matchData = null;
                client.DetachSlot();
                client.clientStatus = ClientReference.ClientStatus.Lobby;
                client.status = BrickManDesc.STATUS.PLAYER_WAITING;
                client.deaths = 0;
                client.kills = 0;
                client.assists = 0;
                client.score = 0;
                room.CurPlayer = clientList.Count;
            }

            Reset();
        }

        public void CacheMap(RegMap regMap, UserMapInfo umi)
        {
            if (regMap != null)
            {
                mapCached = true;
                cachedMap.Clear();
                cachedMap.Load(regMap.Map);
                cachedUMI = umi;
                cachedUMI.AssignRegMap(regMap);
                cachedUMI.Alias = cachedUMI.regMap.Alias;
            }

            else
                Debug.LogError("Couldn't cache map");
        }

        public void CacheMapGenerate(int landscapeIndex, int skyboxIndex, string alias)
        {
            mapCached = true;
            cachedMap.Clear();
            cachedMap = MapGenerator.instance.Generate(landscapeIndex, skyboxIndex);
            DateTime time = DateTime.Now;
            int hashId = MapGenerator.instance.GetHashIdForTime(time);
            cachedMap.map = hashId;
            cachedUMI = new UserMapInfo(hashId, alias, cachedMap.dic.Keys.Count, time, 0);
        }

        public int GetNextBrickSeq()
        {
            int seq = UnityEngine.Random.Range(0, int.MaxValue);
            while (cachedMap.dic.ContainsKey(seq))
                seq = UnityEngine.Random.Range(0, int.MaxValue);
            return seq;
        }

        public sbyte GetWinningTeam()
        {
            if (redScore == blueScore)
                return 0;

            else if (redScore > blueScore)
                return -1;

            else
                return 1;
        }

        public ClientReference GetHighestKillsClient()
        {
            ClientReference bestClient = null;
            foreach(ClientReference client in clientList)
            {
                if (bestClient == null)
                    bestClient = client;
                if (client.kills > bestClient.kills)
                    bestClient = client;
            }

            return bestClient;
        }

        public bool UpdateHighestKillsClient()
        {
            ClientReference bestClient = GetHighestKillsClient();
            if (bestClient != null)
            {
                highestKillsClient = bestClient;
                return true;
            }

            return false;
        }

        public void EndMatch()
        {
            switch (room.Type)
            {
                case Room.ROOM_TYPE.TEAM_MATCH:
                    ServerEmulator.instance.HandleTeamMatchEnd(this);
                    break;

                case Room.ROOM_TYPE.INDIVIDUAL:
                    ServerEmulator.instance.HandleIndividualMatchEnd(this);
                    break;

                case Room.ROOM_TYPE.CAPTURE_THE_FLAG:
                    ServerEmulator.instance.HandleCTFMatchEnd(this);
                    break;

                case Room.ROOM_TYPE.BND:
                    /*Debug.LogWarning("MatchDataEndMatch repeat:" + repeat + " remainTime: " + remainTime + " isBuildPhase: " + isBuildPhase);
                    if (repeat <= 0 && remainTime <0 && !isBuildPhase)
                    {
                        ServerEmulator.instance.HandleBNDMatchEnd(this);
                    }*/
                    ServerEmulator.instance.HandleBNDMatchEnd(this);
                    break;

                default:
                    ServerEmulator.instance.HandleIndividualMatchEnd(this);
                    break;
            }
        }

        public void AddClient(ClientReference client)
        {
            client.matchData = this;
            client.AssignSlot(GetNextFreeSlot());
            client.clientStatus = ClientReference.ClientStatus.Room;
            clientList.Add(client);
            room.CurPlayer = clientList.Count;
        }

        public void RemoveClient(ClientReference client)
        {
            client.matchData = null;
            client.DetachSlot();
            client.clientStatus = ClientReference.ClientStatus.Lobby;
            client.status = BrickManDesc.STATUS.PLAYER_WAITING;
            client.deaths = 0;
            client.kills = 0;
            client.assists = 0;
            client.score = 0;
            clientList.Remove(client);
            room.CurPlayer = clientList.Count;
        }

        public void LockSlotsByMaxPlayers(int maxPlayers)
        {
            int blueIndex = 15;
            int redIndex = 7;
            for (int i = slots.Count - 1; i >= maxPlayers; i--)
            {
                bool odd = i % 2 != 0;
                if (odd)
                {
                    slots[redIndex].ToggleLock(true);
                    redIndex--;
                }

                else
                {
                    slots[blueIndex].ToggleLock(true);
                    blueIndex--;
                }
            }
        }

        public SlotData GetNextFreeSlot()
        {
            int redCount = blueSlots.FindAll(x => x.isUsed).Count;
            int blueCount = redSlots.FindAll(x => x.isUsed).Count;

            if (blueCount >= redCount)
                return blueSlots.Find(x => !x.isUsed && !x.isLocked);
            else
                return redSlots.Find(x => !x.isUsed && !x.isLocked);
        }

        public SlotData GetNextFreeSlotOnOtherTeam(SlotData slot)
        {
            if (slot.slotIndex < 8)
                return blueSlots.Find(x => !x.isUsed && !x.isLocked);
            else
                return redSlots.Find(x => !x.isUsed && !x.isLocked);
        }

        public SlotData FindSlotByClient(ClientReference client)
        {
            SlotData slot = slots.Find(x => x.client == client);

            if (slot == null)
                Debug.LogError("FindSlotByClient: Could not find SlotData for client: " + client.GetIdentifier());

            return slot;
        }
    }
}
