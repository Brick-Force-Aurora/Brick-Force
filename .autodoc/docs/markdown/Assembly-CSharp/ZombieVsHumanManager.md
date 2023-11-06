[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\ZombieVsHumanManager.cs)

The `ZombieVsHumanManager` class is responsible for managing the state and behavior of zombies and humans in a game. It keeps track of the lists of zombies, humans, and dead players, and provides methods to add, remove, and check the status of players.

The class has a private static instance variable `_instance` and a public static property `Instance` that allows other classes to access the singleton instance of the `ZombieVsHumanManager`. The `Instance` property uses the `Object.FindObjectOfType` method to find and return the instance of the `ZombieVsHumanManager` in the scene.

The class also has a private boolean variable `respawnable` and a public property `AmIRespawnable` that allows other classes to get and set the respawnable status of the player.

The `Awake` method is called when the object is initialized and it uses `Object.DontDestroyOnLoad` to prevent the `ZombieVsHumanManager` object from being destroyed when a new scene is loaded.

The `SetupLocalDeath` method is called when a player dies and it determines if the player is respawnable based on the current room type and build options. If the player is a zombie and the zombie mode is enabled, the player is respawnable unless they were killed by a headshot.

The `ResetGameStuff` method is called to reset the lists of zombies, humans, and dead players, as well as the respawnable status.

The `AddZombie` method is called when a player becomes a zombie. It removes the player from the list of humans and adds them to the list of zombies if the current room type is zombie and the zombie mode is enabled.

The `DelZombie` method is called to remove a zombie from the list of zombies.

The `Die` method is called when a player dies. It removes the player from the lists of humans and zombies and adds them to the list of dead players if the current room type is zombie and the zombie mode is enabled.

The `AddHuman` method is called when a player becomes a human. It adds the player to the list of humans if the current room type is zombie and the zombie mode is enabled.

The `DelHuman` method is called to remove a human from the list of humans.

The `IsZombie` method checks if a player is a zombie by checking if their sequence number is in the list of zombies.

The `IsHuman` method checks if a player is a human by checking if their sequence number is in the list of humans.

The `GetZombieRatio` method calculates and returns the ratio of zombies to the maximum number of players. It ensures that the ratio is capped at 1.

The `GetHumanRatio` method calculates and returns the ratio of humans to the maximum number of players. It ensures that the ratio is capped at 1.

The `GetZombieCount` method returns the number of zombies.

The `GetHumanCount` method returns the number of humans.

The `Start` and `Update` methods are empty and do not have any functionality.

Overall, the `ZombieVsHumanManager` class provides a centralized way to manage the state and behavior of zombies and humans in the game. Other classes can access the singleton instance of the `ZombieVsHumanManager` to add, remove, and check the status of players.
## Questions: 
 1. What is the purpose of the `ZombieVsHumanManager` class?
- The `ZombieVsHumanManager` class manages the lists of zombies, humans, and dead players in a game, and provides methods for adding, deleting, and checking the status of players.

2. What is the significance of the `respawnable` variable?
- The `respawnable` variable determines whether a player is able to respawn after death. It is set based on the game mode and the type of player (zombie or human).

3. What is the purpose of the `SetupLocalDeath` method?
- The `SetupLocalDeath` method is called when a player dies and is responsible for determining if the player is a zombie or human, and whether they are able to respawn based on the game mode and other conditions.