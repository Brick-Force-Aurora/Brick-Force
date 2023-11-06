[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\Wanted.cs)

The code provided is a part of the Brick-Force project and is contained within the "Wanted" class. The purpose of this code is to handle the display and functionality of the "Wanted" feature in the game.

The "Wanted" feature allows players to mark other players as "Wanted" and display their names on the screen. The code includes various variables and methods to control the appearance and behavior of the "Wanted" feature.

The code starts by defining an enum called "ACTION_STEP" which represents the different steps of the animation that occurs when a player is marked as "Wanted". The enum includes steps such as "MOVE_IN", "PAUSE", and "MOVE_OUT".

The class includes several public variables that can be set in the Unity editor, such as textures for the "Wanted" checkmark, background, and player icons. These variables allow for customization of the appearance of the "Wanted" feature.

The class also includes private variables to store the positions and sizes of various UI elements, as well as variables to control the timing and animation of the "Wanted" feature.

The "Start" method initializes some variables and checks if the current game mode is a team match or an individual match. It also sets the colors for the "Wanted" feature based on global variables.

The "Action" method is called when a player is marked as "Wanted". It sets the initial positions and sizes for the background and player icons, and starts the animation by setting the "actionStep" variable to "MOVE_IN".

The "Update" method is called every frame and handles the animation of the "Wanted" feature. It updates the positions and sizes of the UI elements based on the current action step and the elapsed time.

The "DrawWanted" method is responsible for drawing a single "Wanted" entry on the screen. It takes a nickname and a color as parameters and uses the "LabelUtil" and "TextureUtil" classes to draw the checkmark and the player's name.

The "DrawWantedList" method is called to draw the list of "Wanted" players on the screen. It retrieves the list of "Wanted" players from the "WantedManager" class and iterates over them to draw each entry using the "DrawWanted" method.

The "DrawIAMWanted" method is responsible for drawing the "I Am Wanted" icon on the screen if the current player is marked as "Wanted". It uses the "TextureUtil" class to draw the icon with a size that is interpolated based on the elapsed time.

The "DrawCenterAction" method is called to draw the background and player icons during the animation. It uses the "TextureUtil" class to draw the textures at the appropriate positions.

The "OnGUI" method is called to handle the GUI rendering. It checks if the "Wanted" feature is enabled and if the current player is in a "Wanted" room. If so, it calls the "DrawWantedList", "DrawIAMWanted", and "DrawCenterAction" methods to draw the UI elements on the screen.

The "VerifyLocalController" method is called to ensure that the "localController" variable is properly initialized. It finds the "Me" game object and retrieves the "LocalController" component from it.

The "OnSelectWanted" method is called when a player selects another player to mark as "Wanted". It checks if the selected player is the current player and if so, it starts the animation and enables the "Wanted" state in the "localController" component. Otherwise, it displays a system message with the name of the selected player.

In summary, this code handles the display and functionality of the "Wanted" feature in the game. It allows players to mark other players as "Wanted" and displays their names on the screen with an animation. The code includes methods to draw the UI elements, handle the animation, and respond to player interactions.
## Questions: 
 1. What is the purpose of the `Action()` method?
- The `Action()` method is used to initiate the animation of the wanted poster moving in and out of the screen.

2. What is the significance of the `wantedDelta` variable?
- The `wantedDelta` variable is used to control the speed of the animation of the wanted poster.

3. What is the purpose of the `VerifyLocalController()` method?
- The `VerifyLocalController()` method is used to check if the `localController` variable is null and assign it the `LocalController` component of the "Me" game object if it is not null.