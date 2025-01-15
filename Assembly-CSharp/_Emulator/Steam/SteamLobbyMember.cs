using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Steamworks;

namespace _Emulator
{
    public class SteamLobbyMember
    {
        public CSteamID steamID = CSteamID.Nil;
        public CSteamID lobbyID = CSteamID.Nil;
        public string name = "";

        public SteamLobbyMember(CSteamID _steamID, CSteamID _lobbyID)
        {
            steamID = _steamID;
            lobbyID = _lobbyID;
            if (SteamManager.Initialized)
            {
                name = SteamMatchmaking.GetLobbyMemberData(lobbyID, steamID, SteamLobbyConstants.memberNameKey);
            }
        }
    }
}
