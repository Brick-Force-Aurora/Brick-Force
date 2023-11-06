[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\MapEditingInfo.cs)

The `MapEditingInfo` class is a script that is used for map editing in the Brick-Force project. It is attached to a game object in the Unity game engine. 

The class has several public variables that can be accessed and modified by other scripts or components. These variables include:
- `thumbnail`: a RenderTexture object that represents the thumbnail image of the map being edited.
- `width` and `height`: floats that represent the width and height of the map.
- `guiDepth`: an enum variable of type `GUIDepth.LAYER` that determines the GUI depth of the map editing interface.
- `constructor`: a Texture2D object that represents the constructor image of the map.
- `crdSize`: a Vector2 object that represents the size of the map editing interface.
- `crdConstructor`, `crdThumbnail`, and `crdThumbnailOutline`: Rect objects that define the positions and sizes of various elements in the map editing interface.

The class also has a method called `ThumbnailToPNG()` that converts the thumbnail RenderTexture to a byte array in PNG format. This method is used to save the thumbnail image of the map.

The class also overrides the `OnGUI()` method, which is a Unity callback method that is called when the GUI is being rendered. In this method, the script checks if the GUI is enabled and if the map editing interface should be displayed. If so, it sets the GUI skin, depth, and enables/disables the GUI based on the state of the dialog manager. It then retrieves the current user map info and checks if the BrickManager has loaded the map. If both conditions are met, it begins a GUI group and draws the constructor and thumbnail images on the screen using the specified positions and sizes. It also draws a box outline around the thumbnail image. Finally, it ends the GUI group and enables the GUI.

Overall, this script is responsible for managing the map editing interface and displaying the constructor and thumbnail images of the map being edited. It also provides a method to convert the thumbnail image to PNG format for saving purposes.
## Questions: 
 1. What is the purpose of the `ThumbnailToPNG()` method?
- The `ThumbnailToPNG()` method converts the `thumbnail` RenderTexture into a PNG byte array.

2. What is the significance of the `crdSize` and `crdConstructor` variables?
- The `crdSize` variable represents the size of a group in the GUI, while the `crdConstructor` variable represents the position and size of the constructor texture within that group.

3. What conditions need to be met for the GUI elements to be displayed in the `OnGUI()` method?
- The GUI elements will only be displayed if `MyInfoManager.Instance.isGuiOn` is true, `cur` is not null, and `BrickManager.Instance.IsLoaded` is true.