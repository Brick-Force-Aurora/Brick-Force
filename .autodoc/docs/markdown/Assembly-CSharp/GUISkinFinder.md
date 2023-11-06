[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\GUISkinFinder.cs)

The `GUISkinFinder` class is responsible for managing the GUI skin used in the Brick-Force project. A GUI skin is a collection of graphical styles and settings that define the appearance of GUI elements in the game.

The `GUISkinFinder` class has a public `guiSkin` variable of type `GUISkin`, which represents the GUI skin used in the project. This variable can be set in the Unity editor or through code.

The class also has a private static array `builtinGUIStyleNames` that contains the names of the built-in GUI styles provided by Unity. These styles include "box", "button", "toggle", "label", and others.

The class has a static property `Instance` that provides a singleton instance of the `GUISkinFinder` class. This ensures that there is only one instance of the class throughout the project. The instance is obtained by finding an object of type `GUISkinFinder` in the scene. If no instance is found, an error message is logged.

The `Awake` method is called when the object is initialized. It uses `Object.DontDestroyOnLoad` to prevent the object from being destroyed when a new scene is loaded.

The `UpdateFont` method is used to update the font used in the GUI skin. It takes a `Font` parameter `curFont` and sets the `font` property of the `guiSkin` to `curFont`. It also updates the font of all the GUI styles in the `guiSkin` and the built-in GUI styles.

The `GetGUISkin` method simply returns the `guiSkin` variable.

The `LanguageChanged` method is called when the language in the game is changed. It calls the `SetFont` method of the `LangOptManager` class to update the font used for language options. It then finds the "Main" object in the scene and sends a message to its "OnLanguageChanged" method.

Overall, the `GUISkinFinder` class provides functionality to manage the GUI skin used in the Brick-Force project. It allows for updating the font used in the GUI skin and handles language changes in the game.
## Questions: 
 1. What is the purpose of the `GUISkinFinder` class?
- The `GUISkinFinder` class is responsible for finding and updating the GUI skin used in the game.

2. What is the significance of the `builtinGUIStyleNames` array?
- The `builtinGUIStyleNames` array contains the names of the built-in GUI styles that are used in the game.

3. What does the `UpdateFont` method do?
- The `UpdateFont` method updates the font used in the GUI skin and all the GUI styles associated with it.