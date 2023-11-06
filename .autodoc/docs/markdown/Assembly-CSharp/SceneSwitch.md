[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\SceneSwitch.cs)

The code provided is a script called "SceneSwitch" that is used in the Brick-Force project. This script is responsible for handling the switching of scenes within the game. It contains various methods and variables that control the flow of the scene switching process.

The script starts by defining an enumeration called "STEP" which represents the different steps in the scene switching process. The possible steps are "DEPARTURE", "CHANGE_CHANNEL", "LOBBY", and "DESTINATION". These steps are used to keep track of the current state of the scene switching process.

The script also defines several constants that represent different error codes that can occur during the scene switching process. These error codes are used to handle different failure scenarios.

The script contains a public variable called "loadingImage" of type Texture, which represents the loading image that is displayed during the scene switching process. It also contains a variable called "logoSize" of type Vector2, which represents the size of the logo image that is displayed alongside the loading image.

The script contains several private variables, including "step" of type STEP, which represents the current step in the scene switching process, and various other variables used for GUI rendering.

The script contains several methods that handle different events during the scene switching process. The "OnGUI" method is responsible for rendering the loading image and logo on the screen. The "Start" method initializes the "step" variable to "DEPARTURE". The "OnJoin" method is called when a player successfully joins a room and handles the logic for switching to the appropriate scene based on the type of the room. The "OnRoamIn" method is called when a player successfully roams into a room and handles the logic for verifying the player's equipped slots and switching to the lobby scene. The "SendCS_JOIN_REQ" method is responsible for sending a join request to the server. The "UpdateJoinRoom" method checks if a join request has been sent and returns a boolean value indicating whether the join request was successful. The "OnLobby" method is called when the player is in the lobby and handles the logic for switching to the appropriate scene based on the destination level. The "OnSeed" method is called when the player successfully seeds into a room and handles the logic for clearing the channel user manager and roaming into the room. The "RoamIn" method is responsible for clearing the room and squad managers and sending a roam in request to the server. The "OnSuccess" method is called when the scene switching process is successful and loads the specified level. The "OnFail" method is called when the scene switching process fails and displays an error message. The "OnRoamOut" method is called when the player successfully roams out of a room and handles the logic for closing the socket connection and switching to the appropriate scene. The "Update" method is called every frame and handles the logic for the scene switching process based on the current step.

In summary, this script is responsible for handling the scene switching process in the Brick-Force game. It handles different events and error scenarios during the scene switching process and controls the flow of the switching process.
## Questions: 
 1. What is the purpose of the `OnGUI` method?
- The `OnGUI` method is responsible for rendering the loading screen and logo on the screen.

2. What does the `OnJoin` method do?
- The `OnJoin` method is called when a player successfully joins a room. It checks the type of the room and performs actions accordingly.

3. What is the purpose of the `UpdateJoinRoom` method?
- The `UpdateJoinRoom` method checks if the player has successfully joined a room and returns a boolean value indicating the status.