![](https://i.imgur.com/fThs88a.png)

[<p align="center">Join The Discord</p>](https://discord.gg/qktjAYsKwH)
[<p align="center">Latest Release</p>](https://github.com/Brick-Force-Aurora/Brick-Force/releases/latest)

[<p align="center">Gameplay Video</p>](https://www.youtube.com/watch?v=mslPRyCIKgo)

[<p align="center">Tutorial by Amorph (German)</p>](https://www.youtube.com/watch?v=OuJ-qxDsTrA)

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

- Download the newest [BfLauncher](https://github.com/Brick-Force-Aurora/Launcher/releases).
- Start it to install the game and receive the newest updates (can be disabled in launcher settings).
- Get into a network (VPN or LAN) with other players (recommended free VPN Service: Radmin).
- Make sure to allow any firewall exceptions that may pop up in the process.
- One player will act as host, make sure to put his network IP in the Host IP text field after starting the game (default value can be set in Config).
- You can use localhost as Host IP (127.0.0.1) if you just want to test alone.
- Put your custom username into the regular E-mail text field.
- Host needs to click Host Match, after that the other players click Join Match.
- After connecting, click Play to proceed to the main menu and create a room (others may need to refresh the room list in order to join).
- Make sure everyone has the files for the current map loaded.
- Room master can start the match once every player is ready (you can also start alone).


![](https://i.imgur.com/6ncbt4O.png)

## Inventory Editor
This is still available, but the Ingame Shop now works, items can only be bought permanently and all have the same price. Maybe sometime this could be updated.
To get Access to the full ingame shop, download the shop.txt.cooked and shopCategory.txt.cooked from the release and put them in BrickForce_Data/Resources/Template
![](https://i.imgur.com/teJ36Lz.png)

- Click on items in the left list to add them to the inventory.
- Click on items in the inventory to remove them.
- Hold CTRL while clicking in the inventory to equip items.
- Click Update Inventory to apply changes to your ingame inventory.
- Click Save Inventory to update and save the current inventory to Inventory.csv.

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

## Limitations
- Only TDM & Deathmtach are fully implemented currently, other modes should load but might not fully work.
- Future support for Build Mode is planned.
- Only one room per host to simplify server.
- Item stats are mostly backup values loaded from disk and are different from Infernum servers.
- ~Most rare weapons and max up variants have empty stats and are therefore useless in game.~
- Game languages need to be english, otherwise you will get stuck in the loading screen.
- Can be changed under registry path HKEY_CURRENT_USER\SOFTWARE\EXE Games\BrickForce (set BfVoice & BfLanguage to 1 for english).

## How to develop
- Install Visual Studio 2017
- Open the solution file Brick-Force.sln
- Install the Launcher and the Game into the same folder where the Brick-Force Repository Folder is to resolve all Dependencies
- Change Build Output Path to Brick-Force_Data/Managed in project properties to match original game folder
- If necessary reimport ICSharpCode.SharpZipLib.dll as reference
- To Debug select the Brick-Force.exe as the external Program

## Notes
- This is a non-commercial fan project and not associated with any of the companies originally involved in the development and publishing of the game.
- Code in _Emulator folder is newly added to the game.
- Other code is mostly reverse engineered (decompiled) from the original game and refined so it compiles as a VS project.
- Therefore commercial use of this project is not recommended.

## Troubleshooting

- **Stuck in "Downloading Once..":** Most likely something is faulty in your config. To fix this issue go to the Registry Editor and go to the path: `Computer\HKEY_CURRENT_USER\SOFTWARE\EXE Games\BrickForce` and delete the `EXE Games` Folder. Now restart the game and it should work
