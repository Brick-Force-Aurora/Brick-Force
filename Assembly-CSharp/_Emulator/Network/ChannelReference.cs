using System;
using System.Collections.Generic;
using System.Text;

namespace _Emulator
{
    class ChannelReference
    {
        public Channel channel;
        public List<MatchData> matches;
        public List<ClientReference> clientList;
        const int maxRoomNumber = 100;

        public ChannelReference(Channel _channel)
        {
            channel = _channel;
            matches = new List<MatchData>();
            clientList = new List<ClientReference>();
        }

        public MatchData GetMatchByRoomNumber(int roomNumber)
        {
            return matches.Find(x => x.room.No == roomNumber);
        }

        public int GetNextRoomNumber()
        {
            for (int i = 0; i < maxRoomNumber; i++)
            {
                if (matches.Find(x => x.room.No == i) == null)
                    return i;
            }

            return -1;
        }

        public void AddClient(ClientReference client)
        {
            if (client.channel != null)
                client.channel.RemoveClient(client);
            client.channel = this;
            clientList.Add(client);
            channel.UserCount = clientList.Count;
        }

        public void RemoveClient(ClientReference client)
        {
            client.channel = null;
            clientList.Remove(client);
            channel.UserCount = clientList.Count;
        }

        public MatchData AddNewMatch()
        {
            MatchData matchData = new MatchData();
            matchData.channel = this;
            matchData.roomCreated = true;
            matchData.room.no = GetNextRoomNumber();
            matches.Add(matchData);
            return matchData;
        }

        public void AddMatch(MatchData match)
        {
            match.channel = this;
            matches.Add(match);
        }

        public void RemoveMatch(MatchData match)
        {
            match.channel = null;
            matches.Remove(match);
        }

        public void Shutdown()
        {
            foreach (MatchData match in matches)
            {
                match.Shutdown();
                match.channel = null;
            }
            //clientList.Clear();
            //matches.Clear();
        }
    }
}
