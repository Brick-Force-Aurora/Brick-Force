using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Steamworks;

namespace _Emulator
{
    public class SteamGUI : MonoBehaviour
    {
        public static SteamGUI instance;
        public bool hidden = true;
        private Rect steamCreateLobbyGUIRect = new Rect(0f, 0f, 250f, 0f);
        private Rect steamManageLobbyGUIRect = new Rect(0f, 0f, 250f, 0f);
        private Rect steamLobbyListGUIRect = new Rect(0f, 0f, 500f, 0f);
        private float tableWidthSlots = 0f;
        private float tableWidthName = 0f;
        private float tableWidthOwner = 0f;
        private float tableWidthJoin = 0f;
        private float findLobbiesWidth = 150f;
        private float clearWidth = 150f;
        private float numLobbiesWidth = 150f;
        private string createLobbyName = "Lobby Name";
        private Vector2 lobbyListScrollPosition = Vector2.zero;
        private Vector2 lobbyMembersScrollPosition = Vector2.zero;
        private bool createFriendsOnly = false;
        private int createMaxNumMembers = 16;

        private void OnEnable()
        {
            if (SteamManager.Initialized)
            {
                createLobbyName = SteamFriends.GetPersonaName();
                if (createLobbyName.EndsWith("s") || createLobbyName.EndsWith("S"))
                    createLobbyName += "' Lobby";
                else
                    createLobbyName += "'s Lobby";
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F10))
                hidden = !hidden;
        }

        private void FitToScreen()
        {
            steamLobbyListGUIRect.height = Screen.height;
            steamLobbyListGUIRect.width = Screen.width - steamCreateLobbyGUIRect.width + 5f;
            steamLobbyListGUIRect.x = steamCreateLobbyGUIRect.width + 5f;
            steamManageLobbyGUIRect.y = steamCreateLobbyGUIRect.yMax + 5f;
            steamManageLobbyGUIRect.height = Screen.height - steamManageLobbyGUIRect.y;
            tableWidthSlots = steamLobbyListGUIRect.width * 0.075f;
            tableWidthName = steamLobbyListGUIRect.width * 0.4f;
            tableWidthOwner = steamLobbyListGUIRect.width * 0.3f;
            tableWidthJoin = steamLobbyListGUIRect.width * 0.1f;
        }

        private void OnGUI()
        {
            if (hidden)
                return;

            FitToScreen();

            steamCreateLobbyGUIRect = GUILayout.Window(105, steamCreateLobbyGUIRect, SteamCreateLobbyGUIWindow, "Create Steam Lobby");
            steamManageLobbyGUIRect = GUILayout.Window(106, steamManageLobbyGUIRect, SteamManageLobbyGUIWindow, "Manage Steam Lobby");
            steamLobbyListGUIRect = GUILayout.Window(107, steamLobbyListGUIRect, SteamLobbyListGUIWindow, "Steam Lobby List");
        }

        private void SteamCreateLobbyGUIWindow(int winID)
        {
            if (SteamManager.Initialized)
            {
                createLobbyName = GUILayout.TextField(createLobbyName, GUILayout.MaxWidth(230f));
                createFriendsOnly = GUILayout.Toggle(createFriendsOnly, "Friends Only");
                GUILayout.Label("Max Num Members: " + createMaxNumMembers);
                createMaxNumMembers = Mathf.FloorToInt(GUILayout.HorizontalSlider(createMaxNumMembers, 1, 16));
                if (GUILayout.Button("Create"))
                    SteamLobbyManager.instance.CreateLobby(createLobbyName, createFriendsOnly ? ELobbyType.k_ELobbyTypeFriendsOnly : ELobbyType.k_ELobbyTypePublic, createMaxNumMembers);
            }
        }

        private void SteamManageLobbyGUIWindow(int winID)
        {
            if (SteamManager.Initialized)
            {
                if (SteamLobbyManager.instance.IsInLobby())
                {
                    lock (SteamLobbyManager.instance.currentLobbyLock)
                    {
                        var lobby = SteamLobbyManager.instance.currentLobby;
                        GUILayout.Label(lobby.lobbyName);
                        GUILayout.Label(lobby.ownerName);
                        GUILayout.Label("Slots: " + lobby.GetSlotsString());
                        if (GUILayout.Button("Leave"))
                            SteamLobbyManager.instance.LeaveCurrentLobbyAndShutdown();

                        GUILayout.Label("");
                        GUILayout.Label("Members:");
                        lobbyMembersScrollPosition = GUILayout.BeginScrollView(lobbyMembersScrollPosition, false, true);
                        foreach (var member in SteamLobbyManager.instance.currentLobby.lobbyMembers)
                        {
                            if (GUILayout.Button(member.name))
                            {
                                Debug.Log("Overlay: " + SteamUtils.IsOverlayEnabled());
                                SteamFriends.ActivateGameOverlayToUser("steamid", member.steamID);
                            }
                        }
                        GUILayout.EndScrollView();
                    }
                }
                else
                    GUILayout.Label("");
            }
        }

        private void SteamLobbyListGUIWindow(int winID)
        {
            if (SteamManager.Initialized)
            {
                lock (SteamLobbyManager.instance.listLock)
                {
                    GUILayout.BeginHorizontal();
                    SteamLobbyManager.instance.findLobbies = GUILayout.Toggle(SteamLobbyManager.instance.findLobbies, "Find Lobbies", GUILayout.Width(findLobbiesWidth));
                    SteamLobbyManager.instance.autoClearLobbies = GUILayout.Toggle(SteamLobbyManager.instance.autoClearLobbies, "Auto Clear", GUILayout.Width(findLobbiesWidth));

                    GUILayout.Label("Num Lobbies: " + SteamLobbyManager.instance.list.Count.ToString(), GUILayout.Width(numLobbiesWidth));

                    if (GUILayout.Button("Clear", GUILayout.Width(clearWidth)))
                        SteamLobbyManager.instance.ClearLobbies();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Slots", GUILayout.Width(tableWidthSlots));
                    GUILayout.Label("Name", GUILayout.Width(tableWidthName));
                    GUILayout.Label("Owner", GUILayout.Width(tableWidthOwner));
                    GUILayout.Label("Join", GUILayout.Width(tableWidthJoin));
                    GUILayout.EndHorizontal();

                    lobbyListScrollPosition = GUILayout.BeginScrollView(lobbyListScrollPosition, false, true);
                    foreach (var info in SteamLobbyManager.instance.list)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(info.GetSlotsString(), GUILayout.Width(tableWidthSlots));
                        GUILayout.Label(info.lobbyName, GUILayout.Width(tableWidthName));
                        GUILayout.Label(info.ownerName, GUILayout.Width(tableWidthOwner));
                        if (GUILayout.Button("Join", GUILayout.Width(tableWidthJoin)))
                            SteamLobbyManager.instance.JoinLobby(info.steamID);

                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndScrollView();
                }
            }
        }
    }
}
