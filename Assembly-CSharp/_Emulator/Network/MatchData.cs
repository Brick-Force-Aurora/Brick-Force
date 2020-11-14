using System.Collections.Generic;
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
        public Room room;

        public MatchData()
        {
            countdownTime = 0;
            remainTime = 0;
            playTime = 0;
            blueScore = 0;
            redScore = 0;
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

            for (int i = 0; i < redSlots.Count; i++)
                redSlots[i].isRed = true;


            room = new Room(false, 0, "", Room.ROOM_TYPE.TEAM_MATCH, Room.ROOM_STATUS.WAITING, 0, 0, 0, "", 0, 0, 0, 0, 0, 0, 0, false, false, false, 0, 0);
        }

        public void Reset()
        {
            blueScore = 0;
            redScore = 0;
            destroyedBricks.Clear();
            usedCannons.Clear();
            usedTrains.Clear();
            killLog.Clear();
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

        public sbyte GetWinningTeam()
        {
            if (redScore == blueScore)
                return 0;

            else if (redScore > blueScore)
                return -1;

            else
                return 1;
        }

        public void AddClient(ClientReference client)
        {
            client.AssignSlot(GetNextFreeSlot());
            client.clientStatus = ClientReference.ClientStatus.Room;
            clientList.Add(client);
            room.CurPlayer = clientList.Count;
        }

        public void RemoveClient(ClientReference client)
        {
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
