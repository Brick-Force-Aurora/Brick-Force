[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\BungeeModeConfig.cs)

The code provided is a class called `BungeeModeConfig` that is used in the larger Brick-Force project. This class is responsible for configuring and displaying the settings and options for the Bungee game mode in the game.

The `BungeeModeConfig` class contains various properties and methods that handle the graphical user interface (GUI) elements and logic for displaying and interacting with the Bungee game mode settings.

The `OnGUI` method is the main entry point for rendering the GUI elements. It first checks if a thumbnail image is available for the current game map. If a thumbnail is available, it is displayed on the GUI. Additionally, if the map was registered on the current day, a "new map" icon is displayed. Depending on the map's tag mask, different icons are displayed to indicate special attributes of the map, such as glory, medals, or gold ribbons. If the map is flagged as an abuse map, an "abuse" icon is displayed.

The method also displays the alias of the current room and the game mode type. It then calls the `DoOption` method to display additional options for the Bungee game mode, such as the time limit and the number of kills required to win. Finally, if the user is the master of the room, a configuration button is displayed, which opens a dialog to change the room configuration.

The `DoOption` method is responsible for displaying the time limit and kill count options for the Bungee game mode. It calculates the positions of the GUI elements based on predefined coordinates and uses the `LabelUtil` class to render the text labels and `GUI.Box` to render the background boxes.

The `ShowTooltip` method is called when the user hovers over a GUI element and displays a tooltip with additional information about the element.

Overall, the `BungeeModeConfig` class provides the necessary functionality to configure and display the settings and options for the Bungee game mode in the Brick-Force project.
## Questions: 
 1. What is the purpose of the `OnGUI()` method?
- The `OnGUI()` method is responsible for rendering the graphical user interface for the Brick-Force game. It displays various textures and labels based on the current state of the game.

2. What does the `DoOption()` method do?
- The `DoOption()` method is used to display and update the options related to the current room in the game. It sets the time limit, bungee count, and break into options based on the values stored in the `Room` object.

3. What is the purpose of the `ShowTooltip()` method?
- The `ShowTooltip()` method is used to display a tooltip message when the user hovers over certain elements in the graphical user interface. It renders the tooltip message as a label with a yellow color.