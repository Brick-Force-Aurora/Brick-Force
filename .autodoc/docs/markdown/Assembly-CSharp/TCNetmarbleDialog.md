[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\TCNetmarbleDialog.cs)

The code provided is a class called `TCNetmarbleDialog` that extends the `Dialog` class. This class is part of the larger Brick-Force project and is responsible for managing a specific dialog window in the game.

The purpose of this code is to handle the functionality and behavior of the TCNetmarble dialog window. This dialog window is used to display information and options related to purchasing tokens and coins in the game.

The `TCNetmarbleDialog` class contains various public fields that represent different UI elements in the dialog window, such as buttons, labels, and images. These fields are assigned in the Unity editor and are used to reference and manipulate these UI elements in the code.

The `InitDialog` method is called to initialize the dialog window with the current status of the TCNetmarble feature. It sets the text and visibility of various UI elements based on the current status. For example, if the coin price is 0, it hides the coin-related UI elements.

The `DoDialog` method is responsible for rendering and updating the dialog window. It draws all the UI elements, handles user input, and updates the visual effects. It also checks if the user clicks on the token button or coin button and performs the corresponding actions, such as displaying a confirmation dialog or showing a prize animation.

The `ReceivePrize` method is called when the player receives a prize from the TCNetmarble feature. It initializes and displays a roulette effect dialog to show the prize animation and details.

Overall, this code manages the functionality and behavior of the TCNetmarble dialog window in the Brick-Force game. It handles UI rendering, user input, and prize animations related to purchasing tokens and coins.
## Questions: 
 1. What is the purpose of the `InitDialog` method?
- The `InitDialog` method initializes the dialog by setting the appropriate values for the UI elements based on the current status of the `TcStatus` object.

2. What is the purpose of the `DoMain` method?
- The `DoMain` method handles the main functionality of the dialog, including drawing the UI elements, handling button clicks, and updating the dialog state.

3. What is the purpose of the `ReceivePrize` method?
- The `ReceivePrize` method is called when a prize is received. It initializes the `rouletteEffect` dialog with the received prize information.