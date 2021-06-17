![](https://i.imgur.com/fThs88a.png)

[<p align="center">Join The Discord</p>](https://discord.gg/qktjAYsKwH)
[<p align="center">Latest Release</p>](https://github.com/Brick-Force-Aurora/Brick-Force/releases/latest)

[<p align="center">Gameplay Video</p>](https://www.youtube.com/watch?v=mslPRyCIKgo)

[<p align="center">Tutorial by Amorph (German)</p>](https://www.youtube.com/watch?v=kc9XD6SWT6k)

<p align="center">Full rewrite of the original server emulator project with more features and better stability, now available for everyone!</p>

## Features
- Play multiplayer over VPN or LAN
- Host matches from within the game
- Load any original map file
- Customize your inventory with the newly added ingame inventory editor
- Use any item you want, all synced with players in your network
- Tracks team score, kills, assists, deaths and score during matches
- Tracks usable and destroyable bricks like glass, crates, cannons and trains
- Configure rooms and switch teams just like in the original for easy match setup
- Players sync movement, hits, shots, deaths, respawns and playerstates
- Various bug fixes over the original game

## How To Play
![](https://i.imgur.com/OUqQ5dR.png)

- Download the newest [BfLauncher](https://github.com/Brick-Force-Aurora/Launcher/releases)
- Start it to install the game and receive the newest updates (can be disabled in launcher settings)
- Get into a network (VPN or LAN) with other players (recommended free VPN: ZeroTier)
- Make sure to allow any firewall exceptions that may pop up in the process
- One player will act as host, make sure to put his network IP in the Host IP text field after starting the game (default value can be set in Config)
- You can use localhost as Host IP (127.0.0.1) if you just want to test alone
- Put your custom username into the regular E-mail text field
- Host needs to click Host Match, after that the other players click Join Match
- After connecting, click Play to proceed to the main menu and create a room (others may need to refresh the room list in order to join)
- Make sure everyone has the files for the current map loaded
- Room master can start the match once every player is ready (you can also start alone)


![](https://i.imgur.com/6ncbt4O.png)

## Inventory Editor
![](https://i.imgur.com/teJ36Lz.png)

- Click on items in the left list to add them to the inventory
- Click on items in the inventory to remove them
- Hold CTRL while clicking in the inventory to equip items
- Click Update Inventory to apply changes to your ingame inventory
- Click Save Inventory to update and save the current inventory to Inventory.csv

## Host Menu
![](https://i.imgur.com/zg6pEny.png)

- Shutdown: Kills the current session and sends everyone back to the main menu, can help when network is laggy
- End Match: Ends the match instantly
- Clear Buffers: Clears the server's write and read queues, use if the server gets stuck
- Debug Handle: Log incoming traffic
- Debug Send: Log outgoing traffic
- Debug Ping: Log frequent traffic
- Clients: All connected clients, click on a client that isn't host to disconnect them

## Paths
- Maps are located in Brick BrickForce_Data\Resources\Cache
- Assembly-CSharp.dll is located in BrickForce_Data\Managed

## Controls
- F4: Host Menu
- F5: Inventory Editor
- F6: Setup Menu
- F8: Debug Console

## Limitations
- Only TDM fully implemented currently, other modes should load but might not fully work
- Future support for Build Mode is planned
- Only one room per host to simplify server
- Item stats are mostly backup values loaded from disk and are different from Infernum servers
- Most rare weapons and max up variants have empty stats and are therefore useless in game
- Game languages need to be english, otherwise you will get stuck in the loading screen
- Can be changed under registry path HKEY_CURRENT_USER\SOFTWARE\EXE Games\BrickForce (set BfVoice & BfLanguage to 1 for english)

## Notes
- This is a non-commercial fan project and not associated with any of the companies originally involved in the development and publishing of the game
- Code in _Emulator folder is newly added to the game
- Other code is mostly reverse engineered (decompiled) from the original game and refined so it compiles as a VS project
- Therefore commercial use of this project is not recommended
