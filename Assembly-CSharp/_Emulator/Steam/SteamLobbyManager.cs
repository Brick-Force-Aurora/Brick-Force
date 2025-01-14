using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Steamworks;
using UnityEngine;

namespace _Emulator
{
    [DisallowMultipleComponent]
    public class SteamLobbyManager : MonoBehaviour
    {
        public static SteamLobbyManager instance;
        public bool findLobbies = true;
        public bool autoClearLobbies = true;
        private bool inFindLobbies = false;
        public List<SteamLobbyInfo> list = new List<SteamLobbyInfo>();
        public SteamLobbyInfo currentLobby = null;
        public string gamemodeName = "";
        public string mapName = "";

        private string lobbyNameToCreate = "";

        public readonly object listLock = new object();
        public readonly object currentLobbyLock = new object();

        private const float findUpdateInterval = 2f;
        private float findUpdateTimer = 0f;

        private CallResult<LobbyCreated_t> m_CallResultLobbyCreated;
        private CallResult<LobbyEnter_t> m_CallResultLobbyEnter;
        private CallResult<LobbyMatchList_t> m_CallResultLobbyMatchList;

        private Callback<LobbyEnter_t> m_CallbackLobbyEnter;
        private Callback<LobbyDataUpdate_t> m_CallbackLobbyDataUpdate;
        private Callback<LobbyChatUpdate_t> m_CallbackLobbyChatUpdate;

        void OnEnable()
        {
            if (SteamManager.Initialized)
            {
                m_CallResultLobbyCreated = CallResult<LobbyCreated_t>.Create(OnLobbyCreated);
                m_CallResultLobbyEnter = CallResult<LobbyEnter_t>.Create(OnLobbyEnter);
                m_CallResultLobbyMatchList = CallResult<LobbyMatchList_t>.Create(OnLobbyMatchList);
                m_CallbackLobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
                m_CallbackLobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdate);
                m_CallbackLobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
            }
        }

        void Update()
        {
            findUpdateTimer += Time.deltaTime;
            if (findLobbies && !inFindLobbies && findUpdateTimer >= findUpdateInterval)
            {
                findUpdateTimer = 0f;
                FindLobbies();
            }

            UpdateCurrentMetadata();
        }

        private void UpdateCurrentMetadata()
        {
            if (SteamManager.Initialized && IsUserOwner())
            {
                lock (currentLobbyLock)
                {
                    if (currentLobby != null)
                    {
                        var result = ServerEmulator.instance.GetGamestateStrings(out string roomType, out string roomStatus, out string mapAlias);
                        var mode = result ? roomType + " (" + roomStatus + ")" : roomType;
                        var newMapName = mapAlias;
                        if (mode != gamemodeName)
                        {
                            SteamMatchmaking.SetLobbyData(currentLobby.steamID, SteamLobbyConstants.gamemodeKey, mode);
                            gamemodeName = mode;
                        }

                        if (newMapName != mapName)
                        {
                            SteamMatchmaking.SetLobbyData(currentLobby.steamID, SteamLobbyConstants.mapNameKey, newMapName);
                            mapName = newMapName;
                        }
                    }
                }
            }
        }

        public bool IsInLobby()
        {
            return IsCurrentMember(SteamUser.GetSteamID());
        }

        public bool IsCurrentMember(CSteamID steamID)
        {
            lock (currentLobbyLock)
            {
                return currentLobby != null && currentLobby.IsLobbyMember(steamID);
            }
        }

        public bool IsUserOwner()
        {
            return IsCurrentOwner(SteamUser.GetSteamID());
        }

        public bool IsCurrentOwner(CSteamID steamID)
        {
            lock (currentLobbyLock)
            {
                return currentLobby != null && currentLobby.ownerSteamID == steamID;
            }
        }

        public CSteamID GetCurrentOwner()
        {
            lock (currentLobbyLock)
            {
                if (currentLobby != null)
                    return currentLobby.ownerSteamID;
            }

            return CSteamID.Nil;
        }

        public void CreateLobby(string lobbyName, ELobbyType lobbyType = ELobbyType.k_ELobbyTypePublic, int maxNumMembers = 16)
        {
            if (SteamManager.Initialized)
            {
                LeaveCurrentLobby(); // Only one lobby at a time
                lobbyNameToCreate = lobbyName;
                SteamAPICall_t handle = SteamMatchmaking.CreateLobby(lobbyType, maxNumMembers);
                m_CallResultLobbyCreated.Set(handle);
            }
        }

        public void JoinLobby(CSteamID steamIDlobby)
        {
            if (SteamManager.Initialized)
            {
                LeaveCurrentLobby(); // Only one lobby at a time
                SteamAPICall_t handle = SteamMatchmaking.JoinLobby(steamIDlobby);
                m_CallResultLobbyEnter.Set(handle);
            }
        }

        public void LeaveCurrentLobbyAndShutdown()
        {
            if (SteamManager.Initialized)
            {
                MessageBoxMgr.Instance.AddMessage("Steam Lobby Disconnected.");
                BuildOption.Instance.Exit();

                LeaveCurrentLobby();
            }
        }

        public void LeaveCurrentLobby()
        {
            lock (currentLobbyLock)
            {
                if (currentLobby != null)
                {
                    SteamMatchmaking.LeaveLobby(currentLobby.steamID);
                    currentLobby = null;
                }
            }
        }

        public void ClearLobbies()
        {
            list.Clear();
        }

        public void FindLobbies()
        {
            if (SteamManager.Initialized)
            {
                inFindLobbies = true;
                SteamMatchmaking.AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter.k_ELobbyDistanceFilterWorldwide);
                SteamMatchmaking.AddRequestLobbyListStringFilter(SteamLobbyConstants.gameIDKey, SteamLobbyConstants.gameIDValue, ELobbyComparison.k_ELobbyComparisonEqual);
                SteamAPICall_t handle = SteamMatchmaking.RequestLobbyList();
                m_CallResultLobbyMatchList.Set(handle);
            }
        }

        private void UpdateLobby(CSteamID lobbyID)
        {
            lock (currentLobbyLock)
            {
                if (currentLobby != null && currentLobby.steamID == lobbyID)
                    currentLobby = new SteamLobbyInfo(lobbyID);
            }

            lock (listLock)
            {
                var existingIndex = list.FindIndex(x => x.steamID == lobbyID);
                if (existingIndex != -1)
                    list[existingIndex] = new SteamLobbyInfo(lobbyID);
            }
        }

        void OnLobbyEnter(LobbyEnter_t pLobbyEnter, bool bIOFailure)
        {
            OnLobbyEnter(pLobbyEnter);
        }

        void OnLobbyEnter(LobbyEnter_t pLobbyEnter)
        {
            if (SteamManager.Initialized)
            {
                if (SteamManager.debug)
                    Debug.Log("Steam - OnLobbyEnter: " + pLobbyEnter.m_ulSteamIDLobby);

                CSteamID steamID = new CSteamID();
                steamID.m_SteamID = pLobbyEnter.m_ulSteamIDLobby;

                CSNetManager.Instance.Sock = new SockTcp();
                CSNetManager.Instance.Sock.Init();
                CSNetManager.Instance.SwitchAfter = new SockTcp();
                CSNetManager.Instance.SwitchAfter.Init();
                SteamNetworkingManager.instance.StartReceive();

                SteamMatchmaking.SetLobbyMemberData(steamID, SteamLobbyConstants.memberNameKey, SteamFriends.GetPersonaName());
                lock (currentLobbyLock)
                {
                    currentLobby = new SteamLobbyInfo(steamID);
                    if (currentLobby.ownerSteamID == SteamUser.GetSteamID())
                    {
                        ServerEmulator.instance.SetupServerSteam();
                        ServerEmulator.instance.AcceptSteam(SteamUser.GetSteamID());
                    }
                }

                ClientExtension.instance.LoadServerSteam();
            }
        }

        void OnLobbyDataUpdate(LobbyDataUpdate_t pLobbyDataUpdate)
        {
            if (SteamManager.Initialized)
            {
                if (SteamManager.debug)
                    Debug.Log("Steam - OnLobbyDataUpdate: " + pLobbyDataUpdate.m_ulSteamIDLobby + " | " + pLobbyDataUpdate.m_ulSteamIDMember);

                CSteamID lobbyID = new CSteamID();
                lobbyID.m_SteamID = pLobbyDataUpdate.m_ulSteamIDLobby;

                CSteamID steamID = new CSteamID();
                steamID.m_SteamID = pLobbyDataUpdate.m_ulSteamIDMember;

                UpdateLobby(lobbyID);
            }
        }

        void OnLobbyChatUpdate(LobbyChatUpdate_t pLobbyChatUpdate)
        {
            if (SteamManager.Initialized)
            {
                if (SteamManager.debug)
                    Debug.Log("Steam - OnLobbyChatUpdate: " + pLobbyChatUpdate.m_ulSteamIDMakingChange + " | " + pLobbyChatUpdate.m_ulSteamIDUserChanged + " | " + pLobbyChatUpdate.m_rgfChatMemberStateChange);

                CSteamID lobbyID = CSteamID.Nil;
                lobbyID.m_SteamID = pLobbyChatUpdate.m_ulSteamIDLobby;

                CSteamID steamIDChanged = CSteamID.Nil;
                steamIDChanged.m_SteamID = pLobbyChatUpdate.m_ulSteamIDUserChanged;
                bool isOwner = IsCurrentOwner(steamIDChanged);

                var state = pLobbyChatUpdate.m_rgfChatMemberStateChange;
                if ((state & (uint)EChatMemberStateChange.k_EChatMemberStateChangeLeft) == (uint)EChatMemberStateChange.k_EChatMemberStateChangeLeft
                    || (state & (uint)EChatMemberStateChange.k_EChatMemberStateChangeDisconnected) == (uint)EChatMemberStateChange.k_EChatMemberStateChangeDisconnected
                    || (state & (uint)EChatMemberStateChange.k_EChatMemberStateChangeKicked) == (uint)EChatMemberStateChange.k_EChatMemberStateChangeKicked
                    || (state & (uint)EChatMemberStateChange.k_EChatMemberStateChangeBanned) == (uint)EChatMemberStateChange.k_EChatMemberStateChangeBanned)
                {
                    if (isOwner || steamIDChanged == SteamUser.GetSteamID())
                    {
                        LeaveCurrentLobbyAndShutdown();
                    }
                }

                if ((state & (uint)EChatMemberStateChange.k_EChatMemberStateChangeEntered) == (uint)EChatMemberStateChange.k_EChatMemberStateChangeEntered)
                    ServerEmulator.instance.AcceptSteam(steamIDChanged);

                UpdateLobby(lobbyID);
            }
        }
        void OnLobbyCreated(LobbyCreated_t pLobbyCreated, bool bIOFailure)
        {
            if (SteamManager.Initialized)
            {
                if (SteamManager.debug)
                    Debug.Log("Steam - OnLobbyCreated: " + pLobbyCreated.m_ulSteamIDLobby + " | " + pLobbyCreated.m_eResult);
                if (pLobbyCreated.m_eResult == EResult.k_EResultOK)
                {
                    CSteamID steamID = new CSteamID();
                    steamID.m_SteamID = pLobbyCreated.m_ulSteamIDLobby;
                    SteamMatchmaking.SetLobbyData(steamID, SteamLobbyConstants.gameIDKey, SteamLobbyConstants.gameIDValue);
                    SteamMatchmaking.SetLobbyData(steamID, SteamLobbyConstants.lobbyNameKey, lobbyNameToCreate);
                    SteamMatchmaking.SetLobbyData(steamID, SteamLobbyConstants.ownerNameKey, SteamFriends.GetPersonaName());
                }
            }
        }

        void OnLobbyMatchList(LobbyMatchList_t pLobbyMatchList, bool bIOFailure)
        {
            if (SteamManager.debug)
                Debug.Log("Steam - OnLobbyMatchList: " + pLobbyMatchList.m_nLobbiesMatching + " lobbies ");

            lock (listLock)
            {
                if (autoClearLobbies)
                    ClearLobbies();

                for (int i = 0; i < pLobbyMatchList.m_nLobbiesMatching; i++)
                {
                    var steamID = SteamMatchmaking.GetLobbyByIndex(i);
                    var info = new SteamLobbyInfo(steamID);

                    if (info.data.ContainsKey(SteamLobbyConstants.gameIDKey) && info.data[SteamLobbyConstants.gameIDKey] == SteamLobbyConstants.gameIDValue)
                    {
                        if (SteamManager.debug)
                            Debug.Log("Steam - OnLobbyMatchList: Found matching lobby");

                        var existingIndex = list.FindIndex(x => x.steamID == steamID);
                        if (existingIndex != -1)
                            list[existingIndex] = info;
                        else
                            list.Add(info);
                    }
                }

                inFindLobbies = false;
            }
        }
    }
}
