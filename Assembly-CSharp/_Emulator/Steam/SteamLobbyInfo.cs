using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Steamworks;

namespace _Emulator
{
    public class SteamLobbyInfo
    {
        public CSteamID steamID = CSteamID.Nil;
        public CSteamID ownerSteamID = CSteamID.Nil;
        public string ownerName;
        public string lobbyName;
        public List<SteamLobbyMember> lobbyMembers = new List<SteamLobbyMember>();
        public Dictionary<string, string> data = new Dictionary<string, string>();
        public int numMembers = 0;
        public int maxNumMembers = 0;

        public SteamLobbyInfo(CSteamID steamIDLobby)
        {
            steamID = steamIDLobby;
            if (SteamManager.Initialized)
            {
                ownerSteamID = SteamMatchmaking.GetLobbyOwner(steamIDLobby);
                numMembers = SteamMatchmaking.GetNumLobbyMembers(steamIDLobby);
                maxNumMembers = SteamMatchmaking.GetLobbyMemberLimit(steamIDLobby);
                for (int i = 0; i < numMembers; i++)
                {
                    lobbyMembers.Add(new SteamLobbyMember(SteamMatchmaking.GetLobbyMemberByIndex(steamIDLobby, i), steamID));
                }

                var nData = SteamMatchmaking.GetLobbyDataCount(steamID);
                for (int j = 0; j < nData; j++)
                {
                    bool success = SteamMatchmaking.GetLobbyDataByIndex(steamID, j, out string key, 255, out string value, 8192);
                    if (success)
                    {
                        data.Add(key, value);
                    }
                }

                if (data.ContainsKey(SteamLobbyConstants.lobbyNameKey))
                    lobbyName = data[SteamLobbyConstants.lobbyNameKey];

                if (data.ContainsKey(SteamLobbyConstants.ownerNameKey))
                    ownerName = data[SteamLobbyConstants.ownerNameKey];
            }
        }

        public bool IsLobbyMember(CSteamID steamID)
        {
            return lobbyMembers.Find(x => x.steamID == steamID) != null;
        }

        public string GetMemberName(CSteamID steamID)
        {
            if (SteamManager.Initialized)
            {
                var member = lobbyMembers.Find(x => x.steamID == steamID);
                if (member != null)
                {
                    return member.name;
                }
            }

            return "";
        }

        public string GetSlotsString()
        {
            return numMembers + "/" + maxNumMembers;
        }

        public string GetDataString()
        {
            string result = "";
            foreach (var key in data.Keys)
            {
                result += key + ": " + data[key] + " | ";
            }

            return result;
        }

        public override string ToString()
        {
            return steamID + " | " + ownerSteamID + " | " + GetSlotsString() + " | " + GetDataString();
        }
    }
}
