![](https://i.imgur.com/fThs88a.png)

<div align="center">
  <a href="https://discord.gg/qktjAYsKwH">Join The Discord</a> |
  <a href="https://github.com/Brick-Force-Aurora/Brick-Force/releases/latest">Latest Release</a> |
  <a href="https://brick-force-aurora.github.io/Website/">Website</a> |
  <a href="https://www.youtube.com/watch?v=mslPRyCIKgo">Gameplay Video</a> |
  <a href="https://www.youtube.com/watch?v=OuJ-qxDsTrA">Tutorial by Amorph (German)</a>
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
<img width="960" alt="Steam" src="https://github.com/user-attachments/assets/4391da58-aab0-4f1f-afb7-a35ca9431510" />

- Download the newest [BfLauncher](https://github.com/Brick-Force-Aurora/Launcher/releases).
- Start it to install the game and receive the newest updates (can be disabled in launcher settings).
- Install and run [Steam](https://store.steampowered.com/), create or login to your Steam account.
- Install 'Spacewar' (Proxy for SteamAPI access) to your Steam Library by opening `steam://install/480/` in your browser while Steam is running.
- Run Brick-Force through BfLauncher.exe or BrickForce.exe. You can optionally add either to your Steam library as non-Steam game.
- Steam should show you as playing 'Spacewar'.
- Click 'Steam' in the Setup menu, the Steam menus should open (can be toggled with F10).
- To create a new lobby, enter a lobby name and hit 'Create' in the 'Create Steam Lobby' menu.
- Lobby owner acts as the host for the game.
- Existing lobbies by others will appear in the 'Steam Lobby List' window, auto refreshes by default.
- To join an existing lobby with at least one open slot, hit 'Join' on the right of a lobby entry.
- If the host disconnects or leaves, the server and lobby get closed.
- Ingame player names are determined by the Steam names.
- While in a lobby, it can be left by clicking 'Leave' in the 'Manage Steam Lobby' window (recommended way of leaving).
- 'Manage Steam Lobby' window also contains a list of players in the current lobby. Clicking on a name will open the respective Steam profile.
- After connecting, click Play to proceed to the main menu and create a room (others may need to refresh the room list in order to join).
- Make sure everyone has the files for the current map in their BrickForce_Data\Resources\Cache folder.
- Room master can start the match once every player is ready (you can also start alone).

![](https://i.imgur.com/6ncbt4O.png)
  
## How To Play (IP)
![](https://i.imgur.com/OUqQ5dR.png)

- Download the newest [BfLauncher](https://github.com/Brick-Force-Aurora/Launcher/releases).
- Start it to install the game and receive the newest updates (can be disabled in launcher settings).
- Get into a network (VPN or LAN) with other players (recommended free VPN Service: Radmin).
- Make sure to allow any firewall exceptions that may pop up in the process.
- One player will act as host, make sure to put his network IP in the Host IP text field after starting the game (default value can be set in Config).
- You can use localhost as Host IP (127.0.0.1) if you just want to test alone.
- Put your custom username into the regular login E-mail text field.
- Host needs to click 'Host' in the 'Setup' menu, after that the other players need to click 'Join'.
- After connecting, click Play to proceed to the main menu and create a room (others may need to refresh the room list in order to join).
- Make sure everyone has the files for the current map in their BrickForce_Data\Resources\Cache folder.
- Room master can start the match once every player is ready (you can also start alone).

## Host Menu
![](https://i.imgur.com/zg6pEny.png)

- Shutdown: Kills the current session and sends everyone back to the main menu, can help when network is laggy.
- Reset: Sends everyone back to the main menu.
- End Match: Ends the match instantly.
- Clear Buffers: Clears the server's write and read queues, use if the server gets stuck.
- Clients: All connected clients, click on a client that isn't host to disconnect them.

## Paths
- Maps are located in Brick BrickForce_Data\Resources\Cache
- Assembly-CSharp.dll is located in BrickForce_Data\Managed

## Controls
- F4: Host Menu
- F5: Inventory Editor
- F6: Setup Menu
- F7: Config Menu
- F8: Debug Console
- F10: Steam Menu

## Limitations
- There are still a lot of bugs, if you encounter any, open an Issue or head to our Discord.
- Freefall, Defense and Defusion don't work.
- No ingame progression.
- No missions.
- No Pick'n'Win.
- No clans or friends.
- Map Manager does not work.
- Weapon upgrading does not work.
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
- Open the solution file Brick-Force.sln in Visual Studio.
- Install the launcher and the game into the same folder where the Brick-Force repository folder is to resolve all dependencies.
- Change Build output path to Brick-Force_Data/Managed in project properties to match original game folder.
- If necessary reimport ICSharpCode.SharpZipLib.dll as reference.
- ~To Debug select the Brick-Force.exe as the external program.~ Regular debugging will crash due to Themida protection on BrickForce.exe by the original developer.

## Notes
- This is a non-commercial fan project and not associated with any of the companies originally involved in the development and publishing of the game.
- Code in _Emulator folder is newly added to the game.
- Other code is mostly reverse engineered (decompiled) from the original game and refined so it compiles as a VS project.
- Therefore commercial use of this project is not recommended.

## Troubleshooting

- **Stuck in "Downloading Once..":** Most likely something is faulty in your config. To fix this issue go to the Registry Editor and go to the path: `Computer\HKEY_CURRENT_USER\SOFTWARE\EXE Games\BrickForce` and delete the `EXE Games` Folder. Now restart the game and it should work
