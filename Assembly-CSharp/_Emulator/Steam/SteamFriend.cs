using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Steamworks;

namespace _Emulator
{
    class SteamFriend : IComparable<SteamFriend>
    {
        public CSteamID steamID = CSteamID.Nil;
        public string name = string.Empty;
        public EPersonaState personaState = EPersonaState.k_EPersonaStateOffline;
        public string presenceStatus;
        public bool playingBrickForce = false;
        public bool isOnline = false;
        public string statusString = string.Empty;
        public string gameStateString = string.Empty;
        public CSteamID lobbyID = CSteamID.Nil;

        public SteamFriend(CSteamID _steamID)
        {
            steamID = _steamID;
            if (SteamManager.Initialized)
            {
                name = SteamFriends.GetFriendPersonaName(steamID);
                personaState = SteamFriends.GetFriendPersonaState(steamID);
                presenceStatus = SteamFriends.GetFriendRichPresence(steamID, "status");
                isOnline = personaState == EPersonaState.k_EPersonaStateOnline
                    || personaState == EPersonaState.k_EPersonaStateLookingToPlay
                    || personaState == EPersonaState.k_EPersonaStateLookingToTrade
                    || personaState == EPersonaState.k_EPersonaStateBusy;
                playingBrickForce = presenceStatus == SteamConstants.richPresenceStatusValue && isOnline;
                if (playingBrickForce)
                {
                    gameStateString = SteamFriends.GetFriendRichPresence(steamID, SteamConstants.richPresenceGameStateKey);
                    statusString = gameStateString;
                }
                else
                    statusString = SteamUtils.PersonaStateToString(personaState);

                var lobbyString = SteamFriends.GetFriendRichPresence(steamID, SteamConstants.richPresenceLobbyKey);
                if (lobbyString != null && lobbyString.Length > 0)
                {
                    try
                    {
                        lobbyID.m_SteamID = Convert.ToUInt64(lobbyString);
                    }
                    catch { }
                }
            }
        }

        public int CompareTo(SteamFriend other)
        {
            if (playingBrickForce && !other.playingBrickForce)
                return -1;

            if (playingBrickForce && other.playingBrickForce)
                return 0;

            if (!playingBrickForce && other.playingBrickForce)
                return 1;

            if (isOnline && !other.isOnline)
                return -1;

            if (isOnline && other.isOnline)
                return 0;

            if (!isOnline && other.isOnline)
                return 1;

            if (personaState > other.personaState)
                return -1;

            return 0;
        }
    }
}
