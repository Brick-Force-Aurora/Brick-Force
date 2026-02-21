[![Contributors][contributors-shield]][contributors-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![Discord][discord-shield]][discord-url]

<br/>
<p align="center">
    <a href="https://brickforce-aurora.de/">
        <img src="https://raw.githubusercontent.com/Brick-Force-Aurora/Launcher/master/.github/assets/logo.png"/>
    </a>
    <h3 align="center">BrickForce Aurora</h3>
    <p align="center">
        <a href="https://github.com/Brick-Force-Aurora/Brick-Force/issues/new">Report Bug</a>
        ·
        <a href="https://github.com/Brick-Force-Aurora/Brick-Force/issues/new">Request Feature</a>
        ·
        <a href="https://brickforce-aurora.de/roadmap/">Roadmap</a>
        ·
        <a href="https://www.youtube.com/watch?v=mslPRyCIKgo">Gameplay Video</a>
        ·
        <a href="https://www.youtube.com/watch?v=fRpV5qkc_IM">Video Tutorial</a>
    </p>
</p>

<p align="center">Open source Brick-Force server emulator project for everyone.</p>

## Features
- Play multiplayer over Steam (New!) or IP (VPN or LAN).
- Host matches from within the game.
- Invite and join Steam friends.
- Load any original map file.
- Customize your inventory with the newly added ingame inventory editor.
- Use any item you want, synced with players in your lobby.
- Tracks team score, kills, assists, deaths and score during matches.
- Tracks usable and destroyable bricks like glass, crates, cannons and trains.
- Custom editing tools for Build Mode.
- Configure rooms and switch teams for easy match setup.
- Players sync movement, hits, shots, deaths, respawns and playerstates.
- Various bug fixes over the original game.

# How To Play (Steam) (Recommended)
<img width="962" alt="setup_steam" src="https://github.com/user-attachments/assets/5f67e42d-1a19-4bd8-97d8-03305cbf52e1" />
<img width="962" alt="lobbies_steam" src="https://github.com/user-attachments/assets/e14aaba1-b1d3-4584-83ba-cf1ce469762e" />

- Follow the instructions on how to install the [BrickForce Aurora Launcher](https://github.com/Brick-Force-Aurora/Launcher?tab=readme-ov-file#installation-).
- Start it to install the game and receive the newest updates (can be disabled in launcher settings).
- Install and run [Steam](https://store.steampowered.com/), create or login to your Steam account.
- Install 'Spacewar' (Proxy for Steamworks API access) to your Steam Library by opening `steam://install/480/` in your browser while Steam is running.
- Run Brick-Force through the launcher or BrickForce.exe. You can optionally add either to your Steam library as non-Steam game.
- Steam should show you as playing 'Spacewar'.
- To create a new lobby, enter a lobby name and hit 'Create' in the 'Setup (Steam)' menu.
- Lobby owner acts as the host for the game.
- Existing lobbies by others will appear in the 'Lobbies (Steam)' menu, auto refreshes by default.
- To join an existing lobby with at least one open slot, right click a lobby entry and click 'Join'.
- If the host disconnects or leaves, the server and lobby get closed.
- Ingame player names are determined by the Steam names.
- While in a lobby, it can be left by clicking 'Close' in the 'Setup (Steam)' window (recommended way of leaving).
- 'Setup (Steam)' window also contains a list of players in the current lobby. Right clicking a player's name allows you to open the player's steam profile.
- After connecting, click Play to proceed to the main menu and create a room (others may need to refresh the room list in order to join).
- Make sure everyone has the files for the current map in their BrickForce_Data\Resources\Cache folder.
- Room master can start the match once every player is ready (you can also start alone).

![](https://i.imgur.com/6ncbt4O.png)

## Playing With Steam Friends
- The 'Friends (Steam)' menu shows a list of all your Steam friends and sorts them by online status.
- Friends who are currently playing Brick-Force are at the top with green status text.
- You can invite friends to your current lobby or join their lobby by right-clicking their entry in the menu.
- Invites will trigger a clickable invite button in the Steam chat with the friend.
- To accept an invite, you need to be in-game already. Otherwise the regular Spacewar will be launched.

## How To Play (IP)
<img width="962" alt="setup_ip" src="https://github.com/user-attachments/assets/35aaab53-d0cd-4e8d-a934-403a512040af" />

- Follow the instructions on how to install the [BrickForce Aurora Launcher](https://github.com/Brick-Force-Aurora/Launcher?tab=readme-ov-file#installation-).
- Get into a network (VPN or LAN) with other players (recommended free VPN Service: Radmin).
- Make sure to allow any firewall exceptions that may pop up in the process.
- One player will act as host, make sure to put his network IP in the Host IP text field in the Setup (IP) menu after starting the game (default value can be set in Config).
- You can use localhost as Host IP (127.0.0.1) if you just want to test alone.
- Put your custom username into the regular login E-mail text field.
- The Host needs to click 'Host' in the 'Setup' menu, after that the other players need to click 'Join'.
- After connecting, click Play to proceed to the main menu and create a room (others may need to refresh the room list in order to join).
- Make sure everyone has the files for the current map in their BrickForce_Data\Resources\Cache folder.
- Room master can start the match once every player is ready (you can also start alone).
- [German Tutorial by Amorph (IP Setup)](https://www.youtube.com/watch?v=OuJ-qxDsTrA)

## Inventory Editing
<img width="962" alt="inventory" src="https://github.com/user-attachments/assets/6f33a60b-2ebd-45e2-86e1-aacfa2043cf8" />

- Inventory can be filled either with the Inventory Editor (F5) or the Shop (easier).
- Click an item in the list on the left in the Inventory Editor or purchase it in the shop to add it your inventory.
- You need to explicitly click 'Update Inventory' or 'Save Inventory' at the top of the Inventory Editor before you can equip an item.
- 'Save Inventory' is required to make the inventory persist. After equipping an item or setting up your action panel, you need to explicitly save if you want your equipment to persist across game restarts.
- You can equip itmes in the Inventory Editor by holding CTRL while clicking their icon. Clicking without CTRL will remove an item.

## Host Menu
<img width="962" alt="host" src="https://github.com/user-attachments/assets/fcbaf262-518d-4328-9e61-6d476054d6fa" />


- Shutdown: Kills the current session and disconnects everyone back to the login screen.
- Reset: Sends everyone back to the main menu.
- End All Matches: Ends all currently running matches.
- Clear Buffers: Clears the server's write and read queues, use if the server gets stuck.
- Clients: All connected clients, right click on a client that that isn't host to disconnect them.

## Config Menu
<img width="962" alt="config" src="https://github.com/user-attachments/assets/cddbc806-7428-4e1a-93e4-e63e4a72eca2" />


- Save/Load: Save and load the config file.
- Theme Colour: Changes the primary menu colour. Click revert to reset it to default.
- DPI Aware: Scales the menu by your Windows scaling factor. Recommended for high resolution screens.
- Menu Blocks Input: When the menu is open, no input will reach the game.
- Announce Lobby To Friends: Sets your current lobby info as rich presence and enables friends to join you directly.
- Axis Ratio: Ratio of your vertical and horizontal mouse sensitivity while in-game. The original Brick-Force setting is 2.25.
- Crosshair Hue: Changes the colour of your in-game crosshair. Default is 120 (green).
- USK Textures: Activates censored paintball gun textures from the later versions of the game.
- Enable VSync: Disabled by default.
- Limit FPS: -1 is no FPS limit and you can limit the FPS up to 400
- Only Host Can Create Rooms: Limits room creation to the frist connected client.
- Max Num Rooms: How many rooms can active at a time.
- Max Num Connections: How many connections the server will accept.
- Auto Clear Dead Clients: Bandaid fix if clients somehow remain in the client list after disconnecting.
- One Client Per IP: Prevents duplicate clients from the same IP or Steam ID.
- Block All Connections: Server accepts no new clients.
- Debug Handle: Log received server messages.
- Debug Send: Log sent server messages.
- Debug Ping: Log recurring server messages.
- Debug Steam: Log Steamworks API related messages.

## Performance View
<img width="481" alt="performance" src="https://raw.githubusercontent.com/Brick-Force-Aurora/Brick-Force/master/.github/assets/performancegraph.png"/>

- View the current fps (approx) and frame times
- Simulation Speed
- RAM usage
- Frame time history graph

## Paths
- Maps are located in Brick BrickForce_Data\Resources\Cache
- Assembly-CSharp.dll is located in BrickForce_Data\Managed

## Controls
- F5: Inventory Editor
- F6: Main Menu
- F8: Debug Console
- F9: Performance View

## BrickEdit
Activate using Keybind T (default) or the last Tool in the action panel during Build Mode

#### What is it?
BrickEdit is basically what you might know from Minecraft as WorldEdit.
It is a mass brick editing tool which you can use in Build Mode rooms.
Shows a preview of the area you are about to edit.
<img width="962" alt="BrickEditPreview" src="https://raw.githubusercontent.com/Brick-Force-Aurora/Brick-Force/master/.github/assets/brickeditpreview.png"/>

#### Commands
<sup>`(required) [optional] <variable>`</sup>

- `//set [<PALETTE_INDEX/BRICK_NAME>]` - Sets the provided brick in the current selection.
- `//replace (<PALETTE_INDEX/BRICK_NAME>) [<PALETTE_INDEX/BRICK_NAME>]` - Replaces the first provided brick with the second provided brick in the current sleection.
- `//del` - Deletes all bricks in the current selection.
- `//pos1` - Sets the first selection position.
- `//pos2` - Sets the second selection position.

## Limitations
- There are still a lot of bugs, if you encounter any, open an Issue or head to our Discord.
- No ingame progression.
- No clans or friends.
- Weapon upgrading doesn't work.
- Performance needs improvement.
- Temporary shop prices.
- No user information persists apart from inventory and config.
- Item stats are mostly backup values loaded from disk and are different from Infernum servers.
- Most rare weapons and max up variants have empty stats and are therefore useless in game.

## How to develop
- Clone the repository.
- Install Visual Studio with '.NET desktop development' and '.NET Framework 3.5 development tools' (under individual components).
- Make sure all dependencies are compatible with .NET Framework 3.5 and Win32 x86.
- When importing new libraries, this often means changing their build targets and/or code.
- (Modified libraries should be forked in the organisation if the official release depends on them.)
- Open the solution file Brick-Force.sln in Visual Studio.
- Install the launcher and the game into the same folder where the Brick-Force repository folder is to resolve all dependencies.
- Change Build output path to Brick-Force_Data/Managed in project properties to match original game folder.
- If necessary, reimport any missing assembly references.
- ~To Debug select the Brick-Force.exe as the external program.~ Regular debugging will crash due to Themida protection on BrickForce.exe by the original developer.

## Dependencies
- [Brick-Force Base (Install with the BrickForce Aurora Launcher)](https://github.com/Brick-Force-Aurora/Launcher) (Root)
- [Steamworks.NET.dll](https://github.com/rlabrecque/Steamworks.NET) (BrickForce_Data\Managed)
- [steam_api.dll](https://partner.steamgames.com/downloads/list) (Root)
- [ImGui.NET.dll](https://github.com/Brick-Force-Aurora/ImGui.NET) (BrickForce_Data\Managed)
- [cimgui.dll](https://github.com/Brick-Force-Aurora/cimgui) (Root)
- [EasyHook.dll](https://github.com/EasyHook/EasyHook) (BrickForce_Data\Managed)
- [EasyHook32.dll](https://github.com/EasyHook/EasyHook) (Root)
- [ICSharpCode.SharpZipLib.dll](https://github.com/icsharpcode/SharpZipLib) (BrickForce_Data\Managed)
- [LitJSON.dll](https://github.com/LitJSON/litjson) (BrickForce_Data\Managed)
- [d3d9helper.dll](https://github.com/Brick-Force-Aurora/d3d9helper) (Root)
- Font.ttf (Any font to use in the menu, release uses [Noto Sans JP Semi Bold](https://fonts.google.com/noto/specimen/Noto+Sans+JP). (Root)

## Notes
- This is a non-commercial fan project and not associated with any of the companies originally involved in the development and publishing of the game.
- Code in _Emulator folder is newly added to the game.
- Other code is mostly reverse engineered (decompiled) from the original game and refined so it compiles as a VS project.
- Therefore commercial use of this project is not recommended.

<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to be learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

<!-- LICENSE -->
## License

Distributed under the GPL-3.0 License. See `LICENSE` for more information.

<!-- MARKDOWN LINKS & IMAGES -->
[contributors-shield]: https://img.shields.io/github/contributors/Brick-Force-Aurora/Brick-Force.svg?style=flat-square&labelColor=%231D1F22&logo=devbox&logoColor=%230088cc&color=%230088cc
[contributors-url]: https://github.com/Brick-Force-Aurora/Brick-Force/graphs/contributors
[stars-shield]: https://img.shields.io/github/stars/Brick-Force-Aurora/Brick-Force.svg?style=flat-square&labelColor=%231D1F22&logo=reverbnation&logoColor=%23E3B341&color=%23E3B341
[stars-url]: https://github.com/Brick-Force-Aurora/Brick-Force/stargazers
[issues-shield]: https://img.shields.io/github/issues/Brick-Force-Aurora/Brick-Force.svg?style=flat-square&labelColor=%231D1F22&logo=buffer&logoColor=%230ec784&color=%230ec784
[issues-url]: https://github.com/Brick-Force-Aurora/Brick-Force/issues
[discord-shield]: https://img.shields.io/discord/777075012032004107?style=flat-square&logo=discord&logoColor=%235865f2&label=Discord&labelColor=%231D1F22&color=%23404eed
[discord-url]: https://discord.com/invite/npqB9f6xXZ
