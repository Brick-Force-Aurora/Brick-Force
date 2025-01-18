using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Steamworks;
using UnityEngine;

namespace _Emulator
{
    [DisallowMultipleComponent]
    class SteamFriendsManager : MonoBehaviour
    {
        public static SteamFriendsManager instance;
        public List<SteamFriend> friends = new List<SteamFriend>();
        public readonly object friendsLock = new object();

        private string richPresenceGameState = string.Empty;
        private string oldRichPresenceGameState = string.Empty;
        private float richPresenceUpdateTimer = 1f;
        private const float richPresenceUpdateInterval = 1f;

        private Callback<PersonaStateChange_t> m_CallbackPersonaStateChange;
        private Callback<FriendRichPresenceUpdate_t> m_CallbackFriendRichPresenceUpdate;

        void OnEnable()
        {
            if (SteamManager.Initialized)
            {
                SteamFriends.SetRichPresence("status", SteamConstants.richPresenceStatusValue);

                m_CallbackPersonaStateChange = Callback<PersonaStateChange_t>.Create(OnPersonaStateChange);
                m_CallbackFriendRichPresenceUpdate = Callback<FriendRichPresenceUpdate_t>.Create(OnFriendRichPresenceUpdate);

                Refresh();
            }
        }

        void Update()
        {
            if (SteamManager.Initialized)
            {
                richPresenceUpdateTimer += Time.deltaTime;
                if (richPresenceUpdateTimer >= richPresenceUpdateInterval)
                {
                    richPresenceUpdateTimer = 0f;

                    if (RoomManager.Instance != null && RoomManager.Instance.CurrentRoomStatus != Room.ROOM_STATUS.NONE)
                    {
                        ClientExtension.instance.GetGamestateStrings(out string roomType, out string roomStatus, out string mapAlias, out string playerStatus);
                        richPresenceGameState = mapAlias.Length > 0 ? roomType + " (" + roomStatus + ") - " + mapAlias : roomType + " (" + roomStatus + ")";
                    }

                    else
                    {
                        if (SteamLobbyManager.instance.IsInLobby())
                            richPresenceGameState = "In Lobby";
                        else
                            richPresenceGameState = "Login";
                    }

                    if (richPresenceGameState != oldRichPresenceGameState)
                    {
                        oldRichPresenceGameState = richPresenceGameState;
                        SteamFriends.SetRichPresence(SteamConstants.richPresenceGameStateKey, richPresenceGameState);
                    }
                }
            }
        }

        public void UpdateFriendsData()
        {
            if (SteamManager.Initialized)
            {
                lock (friendsLock)
                {
                    foreach (var friend in friends)
                    {
                        SteamFriends.RequestUserInformation(friend.steamID, true);
                        SteamFriends.RequestFriendRichPresence(friend.steamID);
                    }
                }
            }
        }

        public void Refresh()
        {
            lock (friendsLock)
            {
                friends.Clear();
                var numFriends = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
                for (int i = 0; i < numFriends; i++)
                {
                    var friendID = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
                    var friend = new SteamFriend(friendID);
                    friends.Add(friend);
                }
                friends.Sort();
            }
        }

        public void ClearRichPresence()
        {
            if (SteamManager.Initialized)
            {
                SteamFriends.SetRichPresence("status", "");
                SteamFriends.SetRichPresence(SteamConstants.richPresenceGameStateKey, "");
                SteamFriends.SetRichPresence(SteamConstants.richPresenceLobbyKey, "");
            }
        }

        void OnPersonaStateChange(PersonaStateChange_t personaStateChange)
        {
            if (SteamManager.Initialized)
            {
                if (SteamManager.debug)
                {
                    var steamID = CSteamID.Nil;
                    steamID.m_SteamID = personaStateChange.m_ulSteamID;
                    Debug.Log("Steam - OnPersonaStateChange: " + personaStateChange.m_ulSteamID + " | " + personaStateChange.m_nChangeFlags + " | " + SteamFriends.GetFriendPersonaName(steamID));
                }

                lock (friendsLock)
                {
                    var friend = friends.Find(x => x.steamID.m_SteamID == personaStateChange.m_ulSteamID);
                    if (friend != null)
                    {
                        if (friends.Remove(friend))
                            friends.Add(new SteamFriend(friend.steamID));
                        friends.Sort();
                    }
                }
            }
        }

        void OnFriendRichPresenceUpdate(FriendRichPresenceUpdate_t friendRichPresenceUpdate)
        {
            if (SteamManager.Initialized)
            {
                if (SteamManager.debug)
                    Debug.Log("Steam OnFriendRichPresenceUpdate: " + friendRichPresenceUpdate.m_steamIDFriend + " | " + friendRichPresenceUpdate.m_nAppID + " | " + SteamFriends.GetFriendPersonaName(friendRichPresenceUpdate.m_steamIDFriend));

                lock (friendsLock)
                {
                    var friend = friends.Find(x => x.steamID == friendRichPresenceUpdate.m_steamIDFriend);
                    if (friend != null)
                    {
                        if (friends.Remove(friend))
                            friends.Add(new SteamFriend(friend.steamID));
                        friends.Sort();
                    }
                }
            }
        }
    }
}
