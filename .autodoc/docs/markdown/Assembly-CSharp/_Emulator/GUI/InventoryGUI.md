[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\_Emulator\GUI\InventoryGUI.cs)

The code provided is a part of the Brick-Force project and is located in the `InventoryGUI` class. This class is responsible for displaying and managing the inventory GUI (Graphical User Interface) in the game.

The `InventoryGUI` class extends the `MonoBehaviour` class from the Unity engine, which allows it to interact with the game objects and events. It contains several private variables, such as `hidden`, `ranGUI`, `sortedItems`, and various `Rect` objects, which define the positions and sizes of GUI elements.

The `FitToScreen` method is responsible for adjusting the size of the GUI elements based on the screen size. It is called whenever the screen size changes.

The `Update` method checks if the F5 key is pressed and toggles the `hidden` variable accordingly. This allows the player to show or hide the inventory GUI by pressing the F5 key.

The `OnGUI` method is called every frame to draw the inventory GUI. It first checks if the inventory is available and the client is connected. If so, it proceeds to draw the GUI elements, such as windows, buttons, and text fields, using the Unity GUI functions.

The `IconGUIWindow` method is responsible for drawing the icons of the items in the inventory. It uses a scroll view to display a list of icons, which can be sorted and filtered based on the `sortText` variable. The icons can be clicked to add the corresponding item to the inventory.

The `InventoryGUIWindow` method is responsible for drawing the items in the inventory. It uses a scroll view to display a grid of item icons. The icons can be clicked to equip or unequip the corresponding item.

Overall, this code provides the functionality to display and interact with the inventory GUI in the game. It allows the player to view, sort, and manage their inventory items. The code also includes methods to update, save, and load the inventory data.
## Questions: 
 1. What is the purpose of the `FitToScreen()` method and when is it called?
- The `FitToScreen()` method adjusts the size and position of GUI windows to fit the screen. It is called in the `OnGUI()` method.
2. What is the significance of the `hidden` variable and how is it used?
- The `hidden` variable is a boolean that determines whether the GUI windows should be displayed or not. It is used in the `OnGUI()` method to conditionally skip rendering the GUI if it is set to true.
3. What is the purpose of the `ranGUI` variable and how is it used?
- The `ranGUI` variable is a boolean that tracks whether the GUI has been rendered at least once. It is used in the `OnGUI()` method to ensure that certain operations are only performed once, such as sorting the items dictionary.