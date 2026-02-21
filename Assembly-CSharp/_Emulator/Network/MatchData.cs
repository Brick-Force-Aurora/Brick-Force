using _Emulator.Network.Gamemodes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Room;

namespace _Emulator
{
    public class MatchData
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

        //Killcount (CTF, Defusion)
        public int redKillCount;
        public int blueKillCount;

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

        //Zombie
        public List<int> humanPlayers;
        public List<int> zombiePlayers;
        public List<int> killedPlayers;
        public List<int> infectedPlayers;
        public bool roundInit;
        public int zombieCountdown;
        public int zombieRounds;
        public int zombieCurrentRound;
        public int zombieRoundsLeft;
        public int zombieTimePerRound;
        public double zombieDeltaTimer;
        public ZombieMatch.STEP zombieStatus;

        //Defusion
        public List<int> deadRedPlayers = new List<int>();
        public List<int> deadBluePlayers = new List<int>();

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
            deadRedPlayers.Clear();
            deadBluePlayers.Clear();

            for (int i = 0; i < redSlots.Count; i++)
                redSlots[i].isRed = true;

            redKillCount = 0;
            blueKillCount = 0;
            room = new Room(false, 0, "", Room.ROOM_TYPE.NONE, Room.ROOM_STATUS.WAITING, 0, 0, 0, "", 0, 0, 0, 0, 0, 0, 0, false, false, false, 0, 0);
            cachedMap = new UserMap();
            mapCached = false;
            roundInit = true;
            humanPlayers = new List<int>();
            zombiePlayers = new List<int>();
            killedPlayers = new List<int>();
            infectedPlayers = new List<int>();
            zombieCountdown = 0;
            zombieRounds = 0;
            zombieTimePerRound = 0;
            zombieRoundsLeft = 0;
            zombieDeltaTimer = 0;
            zombieCurrentRound = 1;
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
            redKillCount = 0;
            blueKillCount = 0;
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
                clientList[i].isZombie = false;
                clientList[i].score = 0;
            }
            roundInit = true;
            humanPlayers = new List<int>();
            zombiePlayers = new List<int>();
            killedPlayers = new List<int>();
            infectedPlayers = new List<int>();
            zombieCountdown = 0;
            zombieRounds = 0;
            zombieTimePerRound = 0;
            zombieRoundsLeft = 0;
            zombieDeltaTimer = 0;
            zombieCurrentRound = 1;
            deadRedPlayers.Clear();
            deadBluePlayers.Clear();
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
            roundInit = true;
            humanPlayers = new List<int>();
            zombiePlayers = new List<int>();
            killedPlayers = new List<int>();
            infectedPlayers = new List<int>();
            zombieCountdown = 0;
            zombieDeltaTimer = 0;
            zombieStatus = ZombieMatch.STEP.WAITING;
            foreach(ClientReference client in clientList)
            {
                client.isZombie = false;
            }
            deadRedPlayers.Clear();
            deadBluePlayers.Clear();
            if (room.Type == Room.ROOM_TYPE.ZOMBIE)
            {
                foreach (ClientReference client in clientList)
                {
                    //Does not work
                    ServerEmulator.instance.SendRespawnTicket(client);
                }

                if (ServerEmulator.instance.debugHandle)
                    UnityEngine.Debug.Log($"[ZombieSpawn] Reassigned {clientList.Count} random tickets for next round.");
            }
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

        public void CacheMapFromSlot(UserMapInfo umi)
        {
            if (umi == null)
            {
                Debug.LogError("CacheMapFromSlot: umi is null");
                return;
            }

            mapCached = true;

            cachedMap.Clear();

            // IMPORTANT: load local geometry file for this slot
            // This uses UserMap.Load(int mapIndex) which loads downloaded{mapIndex}.geometry
            if (!cachedMap.Load(umi.Slot))
            {
                Debug.LogWarning($"CacheMapFromSlot: No geometry for slot {umi.Slot}, starting empty map");
                // If you want: keep cachedMap empty and continue
            }

            cachedUMI = umi;

            // IMPORTANT: do NOT link any regMap here
            cachedUMI.AssignRegMap(null);
        }


        public void CacheMapGenerate(int slot, int landscapeIndex, int skyboxIndex, string alias)
        {
            mapCached = true;
            cachedMap.Clear();
            cachedMap = MapGenerator.instance.Generate(landscapeIndex, skyboxIndex);
            cachedMap.CacheDone(-1,  skyboxIndex); 

            DateTime time = DateTime.Now;

            cachedMap.map = slot; // 1..12
            cachedUMI = new UserMapInfo(slot, alias, cachedMap.dic.Keys.Count, time, 0);
        }

        public void SetMapDone()
        {
            mapCached = true;
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
            Room.ROOM_TYPE roomType = room.Type;
            if (roomType == Room.ROOM_TYPE.NONE || roomType == Room.ROOM_TYPE.NUM_TYPE)
            {
                roomType = Room.ROOM_TYPE.INDIVIDUAL;
            }
            ServerEmulator.instance.gameModes[(int)roomType].HandleMatchEnd(this);
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


            if (room.Type == ROOM_TYPE.MAP_EDITOR)
            {
                if (client.seq == masterSeq)
                {
                    ServerEmulator emulator = ServerEmulator.instance;
                    if (room.CurPlayer > 0)
                    {
                        ClientReference[] clients = clientList.ToArray();
                        foreach (ClientReference otherClient in clients)
                        {
                            emulator.SendKick(otherClient);
                            emulator.SendRoomList(otherClient);
                        }
                    }
                    emulator.SendDeleteRoom(this, channel);
                    channel.RemoveMatch(this);
                }
                return;
            }
            if (room.CurPlayer <= 0)
            {
                ServerEmulator.instance.SendDeleteRoom(this, channel);
                channel.RemoveMatch(this);
                return;
            }

            if (client.seq == masterSeq)
            {
                masterSeq = clientList[0].seq;
                ServerEmulator.instance.SendMaster(null, this);
            }
        }

        public void LockSlotsByMaxPlayers(int maxPlayers, Room.ROOM_TYPE roomType)
        {
            int totalSlots;
            bool isTeamMode;
            bool is8SlotLayout;

            // Determine mode rules
            is8SlotLayout = (roomType == Room.ROOM_TYPE.BUNGEE || roomType == Room.ROOM_TYPE.MISSION);
            totalSlots = is8SlotLayout ? 8 : 16;

            // DM/Zombie = NO TEAMS
            isTeamMode = !(roomType == Room.ROOM_TYPE.INDIVIDUAL || roomType == Room.ROOM_TYPE.ZOMBIE);

            // SPECIAL CASE: Deathmatch / Zombie → lock bottom-up
            if (!isTeamMode)
            {
                for (int i = totalSlots - 1; i >= maxPlayers; i--)
                    slots[i].ToggleLock(true);

                return;
            }

            // TEAM MODE (8-slot or 16-slot)
            int redIndex = is8SlotLayout ? 3 : 7;
            int blueIndex = is8SlotLayout ? 7 : 15;

            // Normal team-mode locking (alternating)
            for (int i = totalSlots - 1; i >= maxPlayers; i--)
            {
                bool odd = (i % 2 != 0);

                if (odd) // RED slot
                {
                    if (redIndex >= 0)
                        slots[redIndex].ToggleLock(true);
                    redIndex--;
                }
                else // BLUE slot
                {
                    if (blueIndex >= 0)
                        slots[blueIndex].ToggleLock(true);
                    blueIndex--;
                }
            }
        }

        public SlotData GetNextFreeSlot()
        {
            // 1) Deathmatch style (16 slots)
            if (room.Type == Room.ROOM_TYPE.INDIVIDUAL ||
                room.Type == Room.ROOM_TYPE.ZOMBIE)
            {
                return slots.Find(s => !s.isUsed && !s.isLocked);
            }

            // 2) Bungee or Mission — limit to 8 total players
            if (room.Type == Room.ROOM_TYPE.BUNGEE ||
                room.Type == Room.ROOM_TYPE.MISSION)
            {
                int totalUsed = slots.FindAll(s => s.isUsed).Count;
                if (totalUsed >= 8)
                    return null; // no more allowed

                // Try balance between teams
                int redCount = redSlots.FindAll(x => x.isUsed).Count;
                int blueCount = blueSlots.FindAll(x => x.isUsed).Count;

                // Pick team with fewer players
                if (blueCount >= redCount)
                {
                    var redFree = redSlots.Find(x => !x.isUsed && !x.isLocked);
                    if (redFree != null) return redFree;

                    return blueSlots.Find(x => !x.isUsed && !x.isLocked);
                }
                else
                {
                    var blueFree = blueSlots.Find(x => !x.isUsed && !x.isLocked);
                    if (blueFree != null) return blueFree;

                    return redSlots.Find(x => !x.isUsed && !x.isLocked);
                }
            }

            // 3) Normal team modes (max 16)
            {
                int redCount = redSlots.FindAll(x => x.isUsed).Count;
                int blueCount = blueSlots.FindAll(x => x.isUsed).Count;

                if (blueCount >= redCount)
                {
                    var redFree = redSlots.Find(x => !x.isUsed && !x.isLocked);
                    if (redFree != null) return redFree;

                    return blueSlots.Find(x => !x.isUsed && !x.isLocked);
                }
                else
                {
                    var blueFree = blueSlots.Find(x => !x.isUsed && !x.isLocked);
                    if (blueFree != null) return blueFree;

                    return redSlots.Find(x => !x.isUsed && !x.isLocked);
                }
            }
        }


        public SlotData GetNextFreeSlotOnOtherTeam(SlotData slot)
        {
            bool isLimited8 = (room.Type == Room.ROOM_TYPE.BUNGEE ||
                               room.Type == Room.ROOM_TYPE.MISSION);

            // Count globally
            int totalUsed = slots.FindAll(x => x.isUsed).Count;

            // If in 8-slot mode and already full, no switch possible
            if (isLimited8 && totalUsed >= 8)
                return null;

            // Player was on Red team (0–7), look for Blue
            if (slot.slotIndex < 8)
            {
                // Try Blue first
                SlotData freeBlue = blueSlots.Find(x => !x.isUsed && !x.isLocked);
                if (freeBlue != null)
                    return freeBlue;

                // If Blue full, fallback Red (only used in normal 16-slot modes)
                if (!isLimited8)
                    return redSlots.Find(x => !x.isUsed && !x.isLocked);

                return null;
            }
            else // Player was on Blue team (8–15), look for Red
            {
                SlotData freeRed = redSlots.Find(x => !x.isUsed && !x.isLocked);
                if (freeRed != null)
                    return freeRed;

                if (!isLimited8)
                    return blueSlots.Find(x => !x.isUsed && !x.isLocked);

                return null;
            }
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
