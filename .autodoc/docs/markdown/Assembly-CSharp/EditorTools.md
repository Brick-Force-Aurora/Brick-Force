[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\EditorTools.cs)

The `EditorTools` class is a MonoBehaviour script that is part of the Brick-Force project. This script is responsible for managing the editor tools used in the game. It contains an array of `EditorToolScript` objects, which represent different types of editor tools. The purpose of this script is to initialize and update these editor tools, as well as handle their GUI representation.

The `Start` method is called when the script is first initialized. It retrieves the `BattleChat` component attached to the same GameObject as this script. It then sets the `desc` property of each `EditorToolScript` object in the `editorToolScripts` array by calling the `Get` method of the `ConsumableManager` class with the corresponding tool name as the argument. It also creates an array of `EditorTool` objects with the same length as the `editorToolScripts` array. For each `EditorToolScript` object, it checks the name of the tool and creates the corresponding `EditorTool` object. The `EditorTool` objects are initialized with the respective `EditorToolScript`, `BattleChat`, and `Item` objects.

The `Update` method is called every frame. It iterates through the `editorTool` array and calls the `Update` method of each `EditorTool` object. If the `Update` method returns true for any `EditorTool` object, it assigns that object to the `editorTool` variable.

The `OnGUI` method is responsible for rendering the GUI representation of the editor tools. It first checks if the GUI is enabled by checking the `isGuiOn` property of the `MyInfoManager` class. It then sets the GUI skin and depth, and disables the GUI if a modal dialog is active. It calculates the position and size of the GUI group based on the number of editor tools and their dimensions. It then begins the GUI group and iterates through the `editorTool` array. For each `EditorTool` object, it renders the tool's icon, hotkey, on/off status, and amount. The GUI group is then ended, and the GUI is enabled again and the original GUI skin is restored.

The `GetActiveEditorTool` method returns the name of the currently active editor tool. It iterates through the `editorTool` array and checks if the `IsActive` property of any `EditorTool` object is true. If it finds an active tool, it returns its name. Otherwise, it returns an empty string.

The `GetLineTool` method returns the `LineTool` object from the `editorTool` array if it is active and its name is "line_tool". It iterates through the `editorTool` array and checks if the `IsActive` property and name of any `EditorTool` object match the conditions. If it finds a matching tool, it casts it to a `LineTool` object and returns it. Otherwise, it returns null.

In summary, this script manages the editor tools used in the game by initializing and updating them, as well as rendering their GUI representation. It provides methods to retrieve the active editor tool and the line tool specifically.
## Questions: 
 1. What is the purpose of the `EditorTools` class?
- The `EditorTools` class is responsible for managing and displaying various editor tools in the game.

2. What is the purpose of the `GetActiveEditorTool()` method?
- The `GetActiveEditorTool()` method returns the name of the currently active editor tool.

3. What is the purpose of the `OnGUI()` method?
- The `OnGUI()` method is responsible for rendering the GUI elements for the editor tools, including icons, hotkeys, and status indicators.