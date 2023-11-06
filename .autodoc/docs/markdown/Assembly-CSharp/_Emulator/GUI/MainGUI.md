[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\_Emulator\GUI\MainGUI.cs)

The code provided is a part of the Brick-Force project and is located in the MainGUI.cs file. This code is responsible for managing the graphical user interface (GUI) of the game. It allows the player to interact with various setup and host options.

The MainGUI class is a MonoBehaviour class, which means it can be attached to a GameObject in the Unity game engine. It contains several public and private variables that control the visibility and position of GUI windows, as well as a custom message string.

The Update() method is called every frame and checks for specific key presses. If the F6 key is pressed, it toggles the visibility of the setup GUI window. If the F4 key is pressed, it toggles the visibility of the host GUI window. Additionally, the method calls the HandleReliableKillLog() method of the ClientExtension class.

The OnGUI() method is also called every frame and is responsible for rendering the GUI elements on the screen. If the setup GUI window is not hidden, it calls the GUILayout.Window() method to create a window with the title "Setup" and the specified dimensions. Inside the window, it displays a label for the host IP and a text field for the user to enter the IP address. It also provides two buttons: "Host" and "Join". Clicking the "Host" button hides the host GUI window, sets up the server using the ServerEmulator class, and loads the server using the ClientExtension class. Clicking the "Join" button also hides the host GUI window and loads the server.

If the host GUI window is not hidden and the server has been created, the method creates a host GUI window using the GUILayout.Window() method. Inside the window, it provides several buttons for different actions such as "Shutdown", "Reset", "Clear Buffers", and "Send Custom Message". It also displays a label for the connected clients and creates a button for each client in the ServerEmulator's clientList. Clicking a client button sends a disconnect message to the server.

Overall, this code provides a user interface for setting up and managing a server in the Brick-Force game. It allows the player to host or join a server, perform server-related actions, and interact with connected clients.
## Questions: 
 1. What is the purpose of the `Update()` method?
- The `Update()` method is used to handle input from the user and perform actions based on that input.

2. What is the purpose of the `OnGUI()` method?
- The `OnGUI()` method is responsible for rendering the graphical user interface (GUI) elements on the screen.

3. What does the `SendDisconnect()` method do?
- The `SendDisconnect()` method is used to send a disconnect message to a specific client in the `clientList` of the `ServerEmulator` instance.