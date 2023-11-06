[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\MatchEnder.cs)

The `MatchEnder` class in the Brick-Force project is responsible for handling the end of a match and displaying the appropriate UI elements and images. 

The class has several public variables that can be set in the Unity editor or through code. The `guiDepth` variable determines the depth of the GUI elements, the `wait` variable determines the amount of time to wait before loading the next level, and the `loadLevel` variable determines the name of the level to load after the match ends. The `endImage` variable is an array of Texture2D objects that represent the images to be displayed at the end of the match.

The class also has private variables such as `ending` and `deltaTime`. The `ending` variable keeps track of the current ending state of the match, while the `deltaTime` variable is used to measure the time elapsed since the match ended.

The `IsOverAll` property returns a boolean value indicating whether the match has ended or not.

The `OnMatchEnd` method is called when the match ends and takes a `code` parameter. It updates the `ending` variable based on the value of the `code` parameter and plays the appropriate audio based on the ending state. It also clears the vote and sets all player statuses to waiting.

The `OnGUI` method is responsible for displaying the end image on the screen. It checks if the GUI is enabled and if the match has ended. If both conditions are met, it sets the GUI skin, depth, and enables GUI interaction. It then draws the end image at the center of the screen using the `TextureUtil.DrawTexture` method.

The `Update` method is called every frame and checks if the match has ended and if the level is not currently being loaded. If both conditions are met, it increments the `deltaTime` variable by the time since the last frame. If the `deltaTime` exceeds the `wait` time and the game is not being shut down, it loads the next level specified by the `loadLevel` variable using the `Application.LoadLevel` method.

Overall, the `MatchEnder` class provides functionality for handling the end of a match, displaying end images, and loading the next level. It is an important component in the larger Brick-Force project as it contributes to the overall gameplay experience and progression.
## Questions: 
 1. What is the purpose of the `MatchEnder` class?
- The `MatchEnder` class is responsible for handling the end of a match and displaying the appropriate end image.

2. What is the significance of the `ending` variable?
- The `ending` variable is used to determine the outcome of the match. It is set based on the `code` parameter passed to the `OnMatchEnd` method.

3. What is the purpose of the `OnGUI` method?
- The `OnGUI` method is responsible for displaying the end image on the screen if the GUI is enabled and the match has ended.