[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/.autodoc\docs\json\Assembly-CSharp\_Emulator)

The `Assembly-CSharp\_Emulator` directory of the Brick-Force project contains key components for the game's configuration, core initialization, graphical user interface (GUI), and map generation.

The `Config.cs` file defines the `Config` class, which manages and persists configuration settings for the project. It loads and saves these settings from a CSV file and applies changes to the settings in the appropriate places. For instance, it adjusts the camera's ySpeed based on the axis ratio, updates build options based on the `uskTextures` setting, and updates the crosshair color based on the `crosshairHue` setting.

The `Core.cs` file initializes the core components of the project and sets up the build configuration. It instantiates and assigns necessary components to the `coreObject` GameObject, including `MainGUI`, `InventoryGUI`, `ConfigGUI`, `DebugConsole`, and `ServerEmulator`. It also sets specific build options for the project.

The `GUI` subfolder contains four files responsible for different aspects of the game's GUI. `ConfigGUI.cs` manages a GUI window for configuring game settings, `DebugConsole.cs` displays debug logs and messages in a console window, `InventoryGUI.cs` displays and manages the inventory GUI, and `MainGUI.cs` manages the main GUI, allowing the player to interact with various setup and host options.

The `Maps` subfolder contains the `MapGenerator.cs` file, which generates game maps. It maintains a dictionary of landscape templates and generates a map based on a given landscape and skybox index. The generated maps are unique and based on predefined landscape templates and skybox indices, providing a diverse gaming experience.

Here's an example of how the `Generate` method might be used:

```csharp
MapGenerator mapGenerator = MapGenerator.Instance;
UserMap generatedMap = mapGenerator.Generate(landscapeIndex, skyboxIndex);
```

In the larger project, these components interact with each other and other parts of the game to provide a comprehensive and user-friendly interface, manage game settings, and generate diverse game maps.
