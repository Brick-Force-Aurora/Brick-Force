[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\FeverFx.cs)

The `FeverFx` class is a script that is used to create a fever effect in the game. It is a part of the larger Brick-Force project. 

The purpose of this script is to display a visual effect on the screen when the player is in a fever state. The effect consists of two layers of textures that are gradually faded in and out. The script uses the `OnGUI` and `Update` methods to control the rendering of the effect.

The `screenFx` variable is an array of `Texture2D` objects that represent the textures to be displayed. The first texture in the array is used as the base layer, and the second texture is used as the second layer that fades in after a certain duration.

The `guiDepth` variable determines the rendering order of the GUI elements. It is set to `GUIDepth.LAYER.SCREEN_FX` by default.

The `localController` variable is a reference to the `LocalController` component attached to the same game object. It is used to check if the player is in a fever state.

The `deltaTime1` and `deltaTime2` variables are used to keep track of the time elapsed since the effect started. They are incremented in the `Update` method using `Time.deltaTime`.

The `FromColor` and `ToColor` variables represent the starting and ending colors of the textures. They are set to `Color.white` and `new Color(1f, 1f, 1f, 0f)` respectively.

The `screenColor1` and `screenColor2` variables represent the current colors of the textures. They are updated in the `Update` method using `Color.Lerp` to interpolate between the `FromColor` and `ToColor` values.

The `Start` method initializes the variables and gets a reference to the `LocalController` component.

The `reset` method resets the variables to their initial values.

The `OnGUI` method is called every frame to render the GUI elements. It checks if the player is in a fever state and if so, it sets the GUI skin, depth, and color. It then uses the `TextureUtil.DrawTexture` method to draw the textures on the screen.

The `Update` method is called every frame to update the colors of the textures. It checks if the player is in a fever state and if so, it updates the `deltaTime1` and `deltaTime2` variables and interpolates the colors accordingly.

Overall, this script is responsible for rendering the fever effect on the screen when the player is in a fever state. It uses two layers of textures that fade in and out over time to create the effect.
## Questions: 
 1. What is the purpose of the `FeverFx` class?
- The `FeverFx` class is responsible for handling the visual effects related to a fever action in the game.

2. What is the significance of the `screenFx` array?
- The `screenFx` array holds the textures used for the fever effect. The first element is always displayed, and the second element is displayed after a certain duration.

3. What is the role of the `localController` variable?
- The `localController` variable is used to access the `ActingFever` property, which determines whether the fever effect should be active or not.