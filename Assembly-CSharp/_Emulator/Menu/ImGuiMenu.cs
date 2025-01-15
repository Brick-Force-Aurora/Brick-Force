using ImGuiNET;
using Steamworks;
using Vector2 = System.Numerics.Vector2;

namespace _Emulator
{
    class ImGuiMenu
    {
        public static ImGuiMenu instance = new ImGuiMenu();
        public bool isVisible = true;
        private bool initialized = false;
        public float dpiScale = 1f;

        private string hostIpInput = string.Empty;
        private string createLobbyInput = "Lobby Name";
        private string customMessageInput = string.Empty;
        private bool createFriendsOnly = false;
        private int createMaxSlots = 16;

        void Initialize()
        {
            if (SteamManager.Initialized)
            {
                createLobbyInput = SteamFriends.GetPersonaName();
                if (createLobbyInput.EndsWith("s") || createLobbyInput.EndsWith("S"))
                    createLobbyInput += "' Lobby";
                else
                    createLobbyInput += "'s Lobby";

                initialized = true;
            }
        }

        public void Render()
        {
            if (!initialized)
                Initialize();

            if ((Import.GetAsyncKeyState(ImportTypes.Keys.F6) & 0x1) == 0x1)
                isVisible = !isVisible;

            if (isVisible)
            {
                RenderWindow();
            }
        }

        private unsafe void RenderWindow()
        {
            ImGui.SetNextWindowSizeConstraints(new Vector2(600.0f * dpiScale, 50.0f * dpiScale), new Vector2(600.0f * dpiScale, 500.0f * dpiScale));
            bool visible = isVisible;
            ImGui.Begin("Brick-Force Aurora", ref visible, ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoBringToFrontOnFocus);
            {
                ImGui.BeginTabBar("##MenuTabs");
                {
                    if (ImGui.BeginTabItem("Setup (Steam)"))
                    {
                        SetupSteamTab();
                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("Lobbies (Steam)"))
                    {
                        LobbiesSteamTab();
                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("Setup (IP)"))
                    {
                        SetupIPTab();
                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("Host"))
                    {
                        HostTab();
                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("Config"))
                    {
                        ConfigTab();
                        ImGui.EndTabItem();
                    }

                    ImGui.EndTabBar();
                }
                ImGui.End();
            }
            isVisible = visible;
        }

        private void SetupIPTab()
        {
            ImGui.InputTextWithHint("##HostIPInput", "Enter Host IP", ref hostIpInput, 16);
            ImGui.SameLine();
            if (ImGui.Button("Host"))
            {
                ServerEmulator.instance.SetupServer();
                ClientExtension.instance.LoadServer();
            }
            ImGui.SameLine();
            if (ImGui.Button("Join"))
            {
                ClientExtension.instance.LoadServer();
            }
        }

        private void SetupSteamTab()
        {
            if (!SteamManager.Initialized)
                return;

            if (!SteamLobbyManager.instance.IsInLobby())
            {
                ImGui.TextDisabled("Create Lobby");
                ImGui.PushItemWidth(ImGuiUtils.SplitRemainingWidth(2f));
                ImGui.InputTextWithHint("##LobbyNameInput", "Enter Lobby Name", ref createLobbyInput, 75);
                ImGui.SliderInt("Slots", ref createMaxSlots, 1, 16);
                ImGui.Checkbox("Friends Only", ref createFriendsOnly);
                if (ImGui.Button("Create"))
                    SteamLobbyManager.instance.CreateLobby(createLobbyInput, createFriendsOnly ? ELobbyType.k_ELobbyTypeFriendsOnly : ELobbyType.k_ELobbyTypePublic, createMaxSlots);
                ImGui.PopItemWidth();
            }

            else
            {
                ImGui.TextDisabled("Manage Lobby");

                lock (SteamLobbyManager.instance.currentLobbyLock)
                {
                    var lobby = SteamLobbyManager.instance.currentLobby;
                    ImGui.Text("Name: " + lobby.lobbyName);
                    ImGui.Text("Owner: " + lobby.ownerName + " | " + lobby.ownerSteamID);
                    ImGui.Columns(3, "##ManageLobbyColumns1", false);
                    {
                        ImGui.Text("Slots: " + lobby.GetSlotsString());
                        ImGui.NextColumn();
                        ImGui.Text("Mode: " + SteamLobbyManager.instance.gamemodeName);
                        ImGui.NextColumn();
                        ImGui.Text("Map: " + SteamLobbyManager.instance.mapName);
                        ImGui.EndColumns();
                    }
                    if (ImGui.Button(SteamLobbyManager.instance.IsUserOwner() ? "Close" : "Leave"))
                        SteamLobbyManager.instance.LeaveCurrentLobbyAndShutdown();

                    if (ImGui.BeginTable("##SteamMemberList", 3, ImGuiTableFlags.BordersInner | ImGuiTableFlags.SizingStretchSame | ImGuiTableFlags.ScrollY, new Vector2(0.0f, 300.0f * dpiScale)))
                    {
                        ImGui.TableSetupScrollFreeze(0, 1);

                        ImGui.TableSetupColumn("Name");
                        ImGui.TableSetupColumn("Steam ID");
                        ImGui.TableSetupColumn("Relationship");
                        ImGui.TableHeadersRow();

                        for (int i = 0; i < lobby.lobbyMembers.Count; i++)
                        {
                            ImGui.TableNextColumn();
                            var member = lobby.lobbyMembers[i];

                            ImGui.PushID("steam_member" + i);
                            bool isSelected = false;
                            ImGui.Selectable(member.name, ref isSelected, ImGuiSelectableFlags.SpanAllColumns);
                            ImGui.TableNextColumn();
                            ImGui.Selectable(member.steamID.ToString(), ref isSelected, ImGuiSelectableFlags.SpanAllColumns);
                            ImGui.TableNextColumn();
                            ImGui.Selectable(SteamUtils.FriendRelationshipToString(SteamFriends.GetFriendRelationship(member.steamID)), ref isSelected, ImGuiSelectableFlags.SpanAllColumns);

                            if (ImGui.BeginPopupContextItem())
                            {
                                SteamMemberPopup(member);
                                ImGui.EndPopup();
                            }

                            ImGui.PopID();
                        }

                        ImGui.EndTable();
                    }
                }
            }
        }

        private void LobbiesSteamTab()
        {
            if (!SteamManager.Initialized)
                return;

            lock (SteamLobbyManager.instance.listLock)
            {
                ImGui.Text("Num Lobbies: " + SteamLobbyManager.instance.list.Count.ToString());
                ImGui.Checkbox("Find Lobbies", ref SteamLobbyManager.instance.findLobbies);
                ImGui.SameLine();
                ImGui.Checkbox("Auto Clear Lobbies", ref SteamLobbyManager.instance.autoClearLobbies);
                ImGui.SameLine();
                if (ImGui.Button("Clear"))
                    SteamLobbyManager.instance.ClearLobbies();

                if (ImGui.BeginTable("##SteamLobbyList", 5, ImGuiTableFlags.BordersInner | ImGuiTableFlags.SizingStretchSame | ImGuiTableFlags.ScrollY, new Vector2(0.0f, 350.0f * dpiScale)))
                {
                    var list = SteamLobbyManager.instance.list;
                    ImGui.TableSetupScrollFreeze(0, 1);

                    ImGui.TableSetupColumn("Slots");
                    ImGui.TableSetupColumn("Mode");
                    ImGui.TableSetupColumn("Map");
                    ImGui.TableSetupColumn("Name");
                    ImGui.TableSetupColumn("Owner");
                    ImGui.TableHeadersRow();

                    for (int i = 0; i < list.Count; i++)
                    {
                        ImGui.TableNextColumn();
                        var lobby = list[i];

                        ImGui.PushID("steam_lobby" + i);
                        bool isSelected = false;
                        ImGui.Selectable(lobby.GetSlotsString(), ref isSelected, ImGuiSelectableFlags.SpanAllColumns);
                        ImGui.TableNextColumn();
                        ImGui.Selectable(lobby.gamemodeName, ref isSelected, ImGuiSelectableFlags.SpanAllColumns);
                        ImGui.TableNextColumn();
                        ImGui.Selectable(lobby.mapName, ref isSelected, ImGuiSelectableFlags.SpanAllColumns);
                        ImGui.TableNextColumn();
                        ImGui.Selectable(lobby.lobbyName, ref isSelected, ImGuiSelectableFlags.SpanAllColumns);
                        ImGui.TableNextColumn();
                        ImGui.Selectable(lobby.ownerName, ref isSelected, ImGuiSelectableFlags.SpanAllColumns);

                        if (ImGui.BeginPopupContextItem())
                        {
                            SteamLobbyPopup(lobby);
                            ImGui.EndPopup();
                        }

                        ImGui.PopID();
                    }

                    ImGui.EndTable();
                }
            }
        }

        private void HostTab()
        {
            if (ImGui.Button("Shutdown"))
                ServerEmulator.instance.ShutdownInit();
            ImGui.SameLine();
            if (ImGui.Button("Reset"))
                ServerEmulator.instance.Reset();
            ImGui.SameLine();
            if (ImGui.Button("End All Matches"))
                ServerEmulator.instance.EndAllMatches();
            ImGui.SameLine();
            if (ImGui.Button("Clear Buffers"))
                ServerEmulator.instance.ClearBuffers();

            ImGui.InputTextWithHint("##CustomMessageInput", "Enter Custom Message", ref customMessageInput, 75);

            ImGui.SameLine();
            if (ImGui.Button("Send Custom Message"))
            {
                ServerEmulator.instance.SendCustomMessage(customMessageInput);
            }

            ImGui.TextDisabled("Clients");
            if (ImGui.BeginTable("##HostClientsList", 4, ImGuiTableFlags.BordersInner | ImGuiTableFlags.SizingStretchSame | ImGuiTableFlags.ScrollY, new Vector2(0.0f, 350.0f * dpiScale)))
            {
                ImGui.TableSetupScrollFreeze(0, 1);

                ImGui.TableSetupColumn("Seq");
                ImGui.TableSetupColumn("Status");
                ImGui.TableSetupColumn("Name");
                ImGui.TableSetupColumn(ServerEmulator.instance.isSteam ? "Steam ID" : "IP");
                ImGui.TableHeadersRow();

                for (int i = 0; i < ServerEmulator.instance.clientList.Count; i++)
                {
                    ImGui.TableNextColumn();
                    var client = ServerEmulator.instance.clientList[i];
                    ImGui.PushID("host_client" + i);

                    bool isSelected = false;
                    ImGui.Selectable(client.seq.ToString(), ref isSelected, ImGuiSelectableFlags.SpanAllColumns);
                    ImGui.TableNextColumn();
                    ImGui.Selectable(BfUtils.BrickManStatusToString(client.status), ref isSelected, ImGuiSelectableFlags.SpanAllColumns);
                    ImGui.TableNextColumn();
                    ImGui.Selectable(client.name, ref isSelected, ImGuiSelectableFlags.SpanAllColumns);
                    ImGui.TableNextColumn();
                    ImGui.Selectable(client.isSteam ? client.steamID.ToString() : client.ip, ref isSelected, ImGuiSelectableFlags.SpanAllColumns);

                    if (ImGui.BeginPopupContextItem())
                    {
                        HostClientPopup(client);
                        ImGui.EndPopup();
                    }

                    ImGui.PopID();
                }

                ImGui.EndTable();
            }
        }

        private void ConfigTab()
        {
            if (ImGui.Button("Save"))
                Config.instance.SaveConfigToDisk();
            ImGui.SameLine();
            if (ImGui.Button("Load"))
                Config.instance.LoadConfigFromDisk();

            ImGui.Separator();

            ImGui.TextDisabled("Menu");
            ImGui.ColorEdit4("Theme Colour", ref Config.instance.themeColor, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoAlpha);
            ImGui.SameLine();
            if (ImGui.Button("Revert"))
                Config.instance.themeColor = Config.themeColorDefault;
            ImGui.Checkbox("DPI Aware", ref Config.instance.dpiAware);
            ImGui.Checkbox("Menu Blocks Input", ref Config.instance.menuBlocksInput);

            ImGui.Separator();

            ImGui.TextDisabled("Game");
            ImGui.SliderFloat("Axis Ratio", ref Config.instance.axisRatio, 1f, 2.25f, "%.2f");
            ImGui.SliderFloat("Crosshair Hue", ref Config.instance.crosshairHue, 0f, 360f, "%.2f");
            ImGui.Checkbox("USK Textures (Requires Restart)", ref Config.instance.uskTextures);

            ImGui.Separator();

            ImGui.TextDisabled("Host");
            ImGui.Checkbox("Only Host Can Create Rooms", ref Config.instance.onlyHostRooms);
            ImGui.SliderInt("Max Num Rooms", ref Config.instance.maxNumRooms, -1, 16);
            ImGui.SliderInt("Max Num Connections", ref Config.instance.maxConnections, 1, 16);
            ImGui.Checkbox("Auto Clear Dead Clients", ref Config.instance.autoClearDeadClients);
            ImGui.Checkbox("One Client Per IP", ref Config.instance.oneClientPerIP);
            ImGui.Checkbox("Block All Connections", ref Config.instance.blockConnections);

            ImGui.Separator();

            ImGui.TextDisabled("Debug");
            ImGui.Checkbox("Debug Handle", ref ServerEmulator.instance.debugHandle);
            ImGui.Checkbox("Debug Send", ref ServerEmulator.instance.debugSend);
            ImGui.Checkbox("Debug Ping", ref ServerEmulator.instance.debugPing);
            ImGui.Checkbox("Debug Steam", ref SteamManager.debug);
        }

        private void HostClientPopup(ClientReference client)
        {
            ImGui.PushID("HostClientPopup");
            if (!client.isHost)
            {
                if (ImGui.MenuItem("Disconnect"))
                    ServerEmulator.instance.SendDisconnect(client);
                ImGui.Separator();
            }

            var seq = client.seq.ToString();
            if (ImGui.MenuItem(seq))
                ImGui.SetClipboardText(seq);
            ImGui.SameLine();
            ImGui.TextDisabled("(Seq)");

            var status = BfUtils.BrickManStatusToString(client.status);
            if (ImGui.MenuItem(status))
                ImGui.SetClipboardText(status);
            ImGui.SameLine();
            ImGui.TextDisabled("(Status)");

            if (ImGui.MenuItem(client.name))
                ImGui.SetClipboardText(client.name);
            ImGui.SameLine();
            ImGui.TextDisabled("(Name)");

            if (client.isSteam)
            {
                var steamID = client.steamID.ToString();
                if (ImGui.MenuItem(steamID))
                    ImGui.SetClipboardText(steamID);
                ImGui.SameLine();
                ImGui.TextDisabled("(Steam ID)");
            }

            else
            {
                if (ImGui.MenuItem(client.ip))
                    ImGui.SetClipboardText(client.ip);
                ImGui.SameLine();
                ImGui.TextDisabled("(IP)");
            }

            ImGui.PopID();
        }

        private void SteamLobbyPopup(SteamLobbyInfo lobby)
        {
            ImGui.PushID("SteamLobbyPopup");

            if (SteamManager.Initialized)
            {
                if (lobby.IsLobbyMember(SteamUser.GetSteamID()))
                {
                    if (ImGui.MenuItem(SteamLobbyManager.instance.IsUserOwner() ? "Close" : "Leave"))
                        SteamLobbyManager.instance.LeaveCurrentLobbyAndShutdown();
                }
                else
                {
                    if (ImGui.MenuItem("Join"))
                        SteamLobbyManager.instance.JoinLobby(lobby.steamID);
                }
            }

            ImGui.Separator();

            var slots = lobby.GetSlotsString();
            if (ImGui.MenuItem(slots))
                ImGui.SetClipboardText(slots);
            ImGui.SameLine();
            ImGui.TextDisabled("(Slots)");

            ImGui.PushID("##Mode");
            if (ImGui.MenuItem(lobby.gamemodeName))
                ImGui.SetClipboardText(lobby.gamemodeName);
            ImGui.SameLine();
            ImGui.TextDisabled("(Mode)");
            ImGui.PopID();

            if (ImGui.MenuItem(lobby.mapName))
                ImGui.SetClipboardText(lobby.mapName);
            ImGui.SameLine();
            ImGui.TextDisabled("(Map)");

            if (ImGui.MenuItem(lobby.lobbyName))
                ImGui.SetClipboardText(lobby.lobbyName);
            ImGui.SameLine();
            ImGui.TextDisabled("(Lobby Name)");

            if (ImGui.MenuItem(lobby.ownerName))
                ImGui.SetClipboardText(lobby.ownerName);
            ImGui.SameLine();
            ImGui.TextDisabled("(Owner Name)");

            if (ImGui.MenuItem(lobby.ownerSteamID.ToString()))
                ImGui.SetClipboardText(lobby.ownerSteamID.ToString());
            ImGui.SameLine();
            ImGui.TextDisabled("(Owner ID)");

            if (ImGui.MenuItem("Open Steam Profile"))
                SteamFriends.ActivateGameOverlayToUser("steamid", lobby.ownerSteamID);

            ImGui.PopID();
        }

        private void SteamMemberPopup(SteamLobbyMember member)
        {
            ImGui.PushID("SteamMemberPopup");
            if (ImGui.MenuItem("Open Steam Profile"))
                SteamFriends.ActivateGameOverlayToUser("steamid", member.steamID);

            if (ImGui.MenuItem(member.steamID.ToString()))
                ImGui.SetClipboardText(member.steamID.ToString());
            ImGui.SameLine();
            ImGui.TextDisabled("(Steam ID)");

            if (ImGui.MenuItem(member.name))
                ImGui.SetClipboardText(member.name);
            ImGui.SameLine();
            ImGui.TextDisabled("(Steam Name)");

            ImGui.PopID();
        }
    }
}
