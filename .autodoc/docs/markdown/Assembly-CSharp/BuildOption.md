[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\BuildOption.cs)

The `BuildOption` class in the Brick-Force project is responsible for managing various build options and configurations for the game. It contains several enums that define different options and settings for the game, such as XP mode, voice language, country filter, target server, season, and random box type.

The `target` variable represents the target server for the game, which can be set to different values from the `TARGET` enum. The `properties` array holds a list of `Property` objects, which contain specific properties and settings for each target server.

The class also has several boolean properties that determine certain conditions based on the target server. For example, the `IsRunup` property returns true if the target server is either the RUNUP_LIVE or RUNUP_LOCAL server. Similarly, the `MustAutoLogin` property returns true if the target server is the RUNUP_LIVE server.

The `Props` property returns the `Property` object corresponding to the current target server.

The `Instance` property is a singleton instance of the `BuildOption` class, ensuring that only one instance of the class exists at a time.

The class also contains several methods for managing the game, such as `AllowBuildGunInDestroyPhase`, which returns a boolean value indicating whether building guns is allowed during the destroy phase of the game.

The `Awake` method is called when the game object is initialized and sets up the necessary configurations. The `Update` method is empty and does not perform any actions.

The `Exit` method is responsible for quitting the game or loading the login scene based on the auto-login status. The `HardExit` method forcefully quits the game.

The `ResetSingletons` method clears various managers and resets the game state.

The `OnApplicationQuit` method is called when the application is about to quit and starts a new process for the FunPortal application if the target server is a Runup server.

The `IsDuplicateExcute` method checks if there is already an instance of the game running by creating a mutex and returning a boolean value indicating whether the game is a duplicate execution.

The `IsWindowsPlayerOrEditor` method checks if the application is running on a Windows player or editor.

The `OpenURL` method opens a URL in the default web browser.

The `TokensParameter` method returns a string containing tokens for the Infernum server, including the user ID and nickname.

Overall, the `BuildOption` class provides a centralized location for managing various build options and configurations for the Brick-Force game. It allows for easy customization and adaptation of the game based on different target servers and settings.
## Questions: 
 1. **What is the purpose of the `BuildOption` class?**
The `BuildOption` class is used to store various options and settings related to the build of the game.

2. **What is the significance of the `TARGET` enum?**
The `TARGET` enum represents different target servers or platforms that the game can be built for.

3. **What is the purpose of the `Props` property?**
The `Props` property returns the `Property` object associated with the current target server or platform.