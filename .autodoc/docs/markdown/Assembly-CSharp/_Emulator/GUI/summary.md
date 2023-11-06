[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/.autodoc\docs\json\Assembly-CSharp\_Emulator\GUI)

The `GUI` folder in the `Assembly-CSharp\_Emulator` directory of the Brick-Force project contains four C# files: `ConfigGUI.cs`, `DebugConsole.cs`, `InventoryGUI.cs`, and `MainGUI.cs`. Each file is responsible for a specific aspect of the game's graphical user interface (GUI).

`ConfigGUI.cs` manages a GUI window for configuring game settings. It extends Unity's `MonoBehaviour` class, allowing it to be attached to a game object and respond to events. The class has methods for updating the GUI and rendering it, as well as a method for drawing the contents of the GUI window. For example, it uses `GUILayout.HorizontalSlider` to create a slider for adjusting the `axisRatio` property of the `Config` class.

`DebugConsole.cs` displays debug logs and messages in a console window within the game. It also extends `MonoBehaviour` and defines a `Log` struct for log messages. The class registers a log callback function that is called whenever a new log message is received. The `OnGUI` method renders the console window and displays each log message.

`InventoryGUI.cs` displays and manages the inventory GUI in the game. It adjusts the size of the GUI elements based on the screen size and allows the player to show or hide the inventory GUI. The class has methods for drawing the icons of the items in the inventory and the items themselves. It also includes methods for updating, saving, and loading the inventory data.

`MainGUI.cs` manages the main GUI of the game, allowing the player to interact with various setup and host options. It checks for specific key presses to toggle the visibility of the setup and host GUI windows. The class creates a window for entering the host IP and provides buttons for hosting or joining a server. It also creates a host GUI window with buttons for different server-related actions.

These classes work together to provide a comprehensive and user-friendly interface for the Brick-Force game. They allow the player to configure settings, view debug logs, manage their inventory, and set up and manage a server.
