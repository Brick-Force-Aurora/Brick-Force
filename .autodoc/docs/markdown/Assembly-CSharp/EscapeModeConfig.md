[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\EscapeModeConfig.cs)

The code provided is a class called `EscapeModeConfig` that is used in the larger Brick-Force project. This class is responsible for displaying and handling the GUI elements related to the escape mode configuration in the game.

The `EscapeModeConfig` class has several private fields that define the positions and dimensions of various GUI elements, such as thumbnails, buttons, and labels. These fields are used to position and size the GUI elements correctly on the screen.

The `OnGUI` method is the main entry point of this class and is responsible for rendering the GUI elements on the screen. It first checks if a thumbnail image is available for the current map. If a thumbnail is available, it is displayed using the `TextureUtil.DrawTexture` method. Depending on the properties of the map, such as its tag mask, different icons are also displayed using the same method.

The method then displays the alias and mode of the current room using the `LabelUtil.TextOut` method. It also calls the `DoOption` method to display additional options related to the room, such as time limit and arrival count. If the user is the master of the room, a configuration button is displayed, which opens a dialog to change the room configuration when clicked.

The `DoOption` method is responsible for displaying the additional options related to the room. It uses the `LabelUtil.TextOut` method to display the option labels and values. The values are retrieved from the `room` object passed as a parameter to the method.

The `ShowTooltip` method is used to display a tooltip when the user hovers over a GUI element. It uses the `LabelUtil.TextOut` method to display the tooltip message.

The `Start` method is empty and does not have any functionality.

In summary, the `EscapeModeConfig` class is responsible for rendering and handling the GUI elements related to the escape mode configuration in the Brick-Force game. It displays the map thumbnail, room alias, room mode, and additional options such as time limit and arrival count. It also provides a configuration button for the room master to change the room configuration.
## Questions: 
 **Question 1:** What is the purpose of the `OnGUI()` method?
    
**Answer:** The `OnGUI()` method is responsible for rendering the graphical user interface for the EscapeModeConfig class. It displays various textures, labels, and buttons based on the current state of the game.

**Question 2:** What does the `DoOption()` method do?
    
**Answer:** The `DoOption()` method is used to display and configure various options related to the game room, such as time limit, arrival count, break-in status, escape mode option, and item drop option.

**Question 3:** What is the purpose of the `ShowTooltip()` method?
    
**Answer:** The `ShowTooltip()` method is called when there is a tooltip message to be displayed. It renders the tooltip message as a label with a yellow color in the upper-left corner of the GUI window.