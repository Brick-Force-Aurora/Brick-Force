[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\ContextMenuManager.cs)

The `ContextMenuManager` class is responsible for managing the context menu in the Brick-Force project. It handles the creation, display, and interaction with the context menu.

The class has a reference to a `UserMenu` object, which represents the content of the context menu. The `isPopup` boolean variable is used to track whether the context menu is currently being displayed or not.

The class provides a static `Instance` property, which returns the singleton instance of the `ContextMenuManager`. This ensures that there is only one instance of the class throughout the project. The `Instance` property uses the `Object.FindObjectOfType` method to find an existing instance of the `ContextMenuManager` or create a new one if none exists.

The `Awake` method is called when the object is initialized and it uses the `Object.DontDestroyOnLoad` method to prevent the `ContextMenuManager` from being destroyed when a new scene is loaded.

The `Popup` method is used to display the context menu. It sets the `isPopup` variable to true and returns the `UserMenu` object. This allows other parts of the project to access and interact with the context menu.

The `OnGUI` method is called every frame and it is responsible for rendering the context menu. It checks if `isPopup` is true and then uses the `GUI.Window` method to display the context menu window. It also checks for clicks outside the context menu window using the `CheckClickOutside` method.

The `Clear` method is used to clear the context menu. It sets the `isPopup` variable to false.

The `CloseAll` method is used to close all open context menus. It calls the `OnClose` method of the `UserMenu` object and sets the `isPopup` variable to false.

The `Update` method is called every frame and it is responsible for updating the context menu. It checks if `isPopup` is true and then calls the `Update` method of the `UserMenu` object.

Overall, the `ContextMenuManager` class provides a centralized way to manage the context menu in the Brick-Force project. It handles the creation, display, and interaction with the context menu, ensuring that only one instance is active at a time. Other parts of the project can use the `Popup` method to display the context menu and interact with it through the `UserMenu` object.
## Questions: 
 1. What is the purpose of the `ContextMenuManager` class?
- The `ContextMenuManager` class manages the display and functionality of a context menu.

2. How does the `Popup` method work?
- The `Popup` method sets the `isPopup` variable to true and returns the `userMenu` object.

3. What is the purpose of the `CheckClickOutside` method?
- The `CheckClickOutside` method checks if the mouse click is outside the bounds of the `userMenu` and sets the `isPopup` variable to false if it is.