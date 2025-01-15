![](https://i.imgur.com/fThs88a.png)

<div align="center">
  <a href="https://discord.gg/qktjAYsKwH">Join The Discord</a> |
  <a href="https://github.com/Brick-Force-Aurora/Brick-Force/releases/latest">Latest Release</a> |
  <a href="https://brick-force-aurora.github.io/Website/">Website</a> |
  <a href="https://www.youtube.com/watch?v=mslPRyCIKgo">Gameplay Video</a> |
  <a href="https://www.youtube.com/watch?v=OuJ-qxDsTrA">German Tutorial by Amorph (IP Setup)</a>
</div>

<p align="center">Open source Brick-Force server emulator project for everyone.</p>

## Features
- Play multiplayer over Steam (New!) or IP (VPN or LAN).
- Host matches from within the game.
- Load any original map file.
- Customize your inventory with the newly added ingame inventory editor.
- Use any item you want, synced with players in your network.
- Tracks team score, kills, assists, deaths and score during matches.
- Tracks usable and destroyable bricks like glass, crates, cannons and trains.
- Configure rooms and switch teams for easy match setup.
- Players sync movement, hits, shots, deaths, respawns and playerstates.
- Various bug fixes over the original game.

## How To Play (Steam) (Recommended)
<img width="962" alt="Steam_Setup_2" src="https://github.com/user-attachments/assets/bbd9c65f-6976-4d44-b69b-55d567fc2bb3" />
<img width="962" alt="SteamLobbies" src="https://github.com/user-attachments/assets/cb7e5722-86f5-4c14-bad1-4d7e40d073e0" />

- Download the newest [BfLauncher](https://github.com/Brick-Force-Aurora/Launcher/releases).
- Start it to install the game and receive the newest updates (can be disabled in launcher settings).
- Install and run [Steam](https://store.steampowered.com/), create or login to your Steam account.
- Install 'Spacewar' (Proxy for Steamworks API access) to your Steam Library by opening `steam://install/480/` in your browser while Steam is running.
- Run Brick-Force through BfLauncher.exe or BrickForce.exe. You can optionally add either to your Steam library as non-Steam game.
- Steam should show you as playing 'Spacewar'.
- To create a new lobby, enter a lobby name and hit 'Create' in the 'Setup (Steam)' menu.
- Lobby owner acts as the host for the game.
- Existing lobbies by others will appear in the 'Lobbies (Steam)' window, auto refreshes by default.
- To join an existing lobby with at least one open slot, right click a lobby entry and click 'Join'.
- If the host disconnects or leaves, the server and lobby get closed.
- Ingame player names are determined by the Steam names.
- While in a lobby, it can be left by clicking 'Close' in the 'Setup (Steam)' window (recommended way of leaving).
- 'Setup (Steam)' window also contains a list of players in the current lobby. Right clicking a player's name allows you to open the player's steam profile.
- After connecting, click Play to proceed to the main menu and create a room (others may need to refresh the room list in order to join).
- Make sure everyone has the files for the current map in their BrickForce_Data\Resources\Cache folder.
- Room master can start the match once every player is ready (you can also start alone).

![](https://i.imgur.com/6ncbt4O.png)
  
## How To Play (IP)
<img width="962" alt="IP" src="https://github.com/user-attachments/assets/51be5d18-ded1-44f2-972d-5e912131cf2e" />

- Download the newest [BfLauncher](https://github.com/Brick-Force-Aurora/Launcher/releases).
- Start it to install the game and receive the newest updates (can be disabled in launcher settings).
- Get into a network (VPN or LAN) with other players (recommended free VPN Service: Radmin).
- Make sure to allow any firewall exceptions that may pop up in the process.
- One player will act as host, make sure to put his network IP in the Host IP text field in the Setup (IP) menu after starting the game (default value can be set in Config).
- You can use localhost as Host IP (127.0.0.1) if you just want to test alone.
- Put your custom username into the regular login E-mail text field.
- The Host needs to click 'Host' in the 'Setup' menu, after that the other players need to click 'Join'.
- After connecting, click Play to proceed to the main menu and create a room (others may need to refresh the room list in order to join).
- Make sure everyone has the files for the current map in their BrickForce_Data\Resources\Cache folder.
- Room master can start the match once every player is ready (you can also start alone).

## Host Menu
<img width="962" alt="HostMenu" src="https://github.com/user-attachments/assets/74ec6773-115c-4a75-a83d-62888a9d6eb1" />

- Shutdown: Kills the current session and disconnects everyone back to the login screen.
- Reset: Sends everyone back to the main menu.
- End All Matches: Ends all currently running matches.
- Clear Buffers: Clears the server's write and read queues, use if the server gets stuck.
- Clients: All connected clients, right click on a client that that isn't host to disconnect them.

## Config Menu
<img width="962" alt="Config" src="https://github.com/user-attachments/assets/3a7505f0-96fa-41ff-9761-e6ac855b4ca8" />

- Save/Load: Save and load the config file.
- Theme Colour: Changes the primary menu colour. Click revert to reset it to default.
- DPI Aware: Scales the menu by your Windows scaling factor. Recommended for high resolution screens.
- Axis Ratio: Ratio of your vertical and horizontal mouse sensitivity while in-game. The original Brick-Force setting is 2.25.
- Crosshair Hue: Changes the colour of your in-game crosshair. Default is 120 (green).
- Max Connections: How many connections the server will accept.
- Auto Clear Dead Clients: Bandaid fix if clients somehow remain in the client list after disconnecting.
- One Client Per IP: Prevents duplicate clients from the same IP or Steam ID.
- Block All Connections: Server accepts no new clients.
- USK Textures: Activates censored paintball gun textures from the later versions of the game.
- Debug Handle: Log received server messages.
- Debug Send: Log sent server messages.
- Debug Ping: Log recurring server messages.
- Debug Steam: Log Steamworks API related messages.

## Paths
- Maps are located in Brick BrickForce_Data\Resources\Cache
- Assembly-CSharp.dll is located in BrickForce_Data\Managed

## Controls
- F5: Inventory Editor
- F6: Main Menu
- F8: Debug Console

## Limitations
- There are still a lot of bugs, if you encounter any, open an Issue or head to our Discord.
- Freefall, Defense and Defusion don't work.
- No ingame progression.
- No missions.
- No Pick'n'Win.
- No clans or friends.
- Map Manager doesn't work.
- Weapon upgrading doesn't work.
- Performance needs improvement.
- Temporary shop prices.
- No user information persists apart from inventory and config.
- Action panel untested.
- Build Mode: No Swappie or Streamliner.
- Item stats are mostly backup values loaded from disk and are different from Infernum servers.
- ~Most rare weapons and max up variants have empty stats and are therefore useless in game.~
- Game languages need to be english, otherwise you will get stuck in the loading screen.
- Can be changed under registry path HKEY_CURRENT_USER\SOFTWARE\EXE Games\BrickForce (set BfVoice & BfLanguage to 1 for english).

## How to develop
- Clone the repository.
- Install Visual Studio with '.NET desktop development' and '.NET Framework 3.5 development tools' (under individual components).
- Make sure all dependencies are compatible with .NET Framework 3.5 and Win32 x86.
- When importing new libraries, this often means changing their build targets and/or code.
- (Modified libraries should be forked in the organisation if the official release depends on them.)
- Open the solution file Brick-Force.sln in Visual Studio.
- Install the launcher and the game into the same folder where the Brick-Force repository folder is to resolve all dependencies.
- Change Build output path to Brick-Force_Data/Managed in project properties to match original game folder.
- If necessary reimport ICSharpCode.SharpZipLib.dll and EasyHook.dll as references.
- ~To Debug select the Brick-Force.exe as the external program.~ Regular debugging will crash due to Themida protection on BrickForce.exe by the original developer.

## Dependencies
- [Steamworks.NET.dll](https://github.com/rlabrecque/Steamworks.NET) (BrickForce_Data\Managed)
- [steam_api.dll](https://partner.steamgames.com/downloads/list) (Root)
- [ImGui.NET.dll](https://github.com/Brick-Force-Aurora/ImGui.NET) (BrickForce_Data\Managed)
- [cimgui.dll](https://github.com/Brick-Force-Aurora/cimgui) (Root)
- [EasyHook.dll](https://github.com/EasyHook/EasyHook) (BrickForce_Data\Managed)
- [EasyHook32.dll](https://github.com/EasyHook/EasyHook) (Root)
- [ICSharpCode.SharpZipLib.dll](https://github.com/icsharpcode/SharpZipLib) (BrickForce_Data\Managed)
- [d3d9helper.dll](https://github.com/Brick-Force-Aurora/d3d9helper) (Root)
- [Nunito-SemiBold.ttf](https://fonts.google.com/specimen/Nunito) (Root)

## Notes
- This is a non-commercial fan project and not associated with any of the companies originally involved in the development and publishing of the game.
- Code in _Emulator folder is newly added to the game.
- Other code is mostly reverse engineered (decompiled) from the original game and refined so it compiles as a VS project.
- Therefore commercial use of this project is not recommended.

## Troubleshooting

- **Stuck in "Downloading Once..":** Most likely something is faulty in your config. To fix this issue go to the Registry Editor and go to the path: `Computer\HKEY_CURRENT_USER\SOFTWARE\EXE Games\BrickForce` and delete the `EXE Games` Folder. Now restart the game and it should work
