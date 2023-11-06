[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\Timer.cs)

The `Timer` class is a script that is used to manage and display a countdown timer in the Brick-Force game. It is responsible for keeping track of the remaining time and updating the timer display accordingly.

The `Timer` class has several member variables that are used for various purposes. The `guiDepth` variable determines the depth at which the timer GUI is rendered. The `offset` variable specifies the position of the timer GUI on the screen. The `bkgnd` variable holds the background texture for the timer GUI. The `backChange` variable is an instance of the `UIImageSizeChange` class, which is used to animate the background of the timer GUI. The `play` variable keeps track of the number of times the timer has been played. The `playDelta` variable is used to control the frequency at which the timer updates. The `deltaTime` variable is used to track the time elapsed since the last timer update. The `remain` variable holds the remaining time in seconds. The `localController` variable is a reference to the `LocalController` component attached to the player character. The `expandArea` variable determines the size of the expanded area around the timer GUI.

The `Start` method is called when the script is initialized. It initializes the member variables, finds the player character object, and retrieves the `LocalController` component attached to it. It also sets the position and size of the background image and sets up the animation for the background.

The `OnGUI` method is called every frame to render the timer GUI. It checks if the GUI is enabled and if there are any active dialogs. It then calculates the minutes and seconds from the remaining time and displays them on the GUI. If the remaining time is less than 11 seconds, the background animation is played and the text color is set to red. Otherwise, the background image is displayed normally and the text color is set to yellow.

The `Update` method is called every frame to update the timer. It checks if the player is the master of the room, if the player is controllable, and if the brick manager is loaded. If these conditions are met, it updates the `playDelta` variable and sends a network message to update the timer on the server. If the current room type is not explosion or if the bomb is not installed, it updates the `deltaTime` variable and decreases the remaining time. If the remaining time reaches zero, it stops the timer.

The `OnPlayTime` and `OnTimer` methods are event handlers that are called when the play time and remaining time are received from the server. They update the `play` and `remain` variables if the received values are greater or smaller than the current values, respectively.

In summary, the `Timer` class is responsible for managing and displaying a countdown timer in the Brick-Force game. It updates the timer every frame, animates the background of the timer GUI, and sends network messages to synchronize the timer with the server. It also handles events related to the play time and remaining time received from the server.
## Questions: 
 1. What is the purpose of the `Timer` class?
- The `Timer` class is responsible for managing the countdown timer in the game.

2. What is the significance of the `TimeLimit` property?
- The `TimeLimit` property allows other classes to get and set the remaining time for the timer.

3. What is the purpose of the `OnGetBack2Spawner` method?
- The `OnGetBack2Spawner` method is called when the player returns to the spawner. It checks the current room type and updates the remaining time accordingly.