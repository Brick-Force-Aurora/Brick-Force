[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\SystemMessage.cs)

The code provided is a class called "SystemMessage" that is used to display system messages in a graphical user interface (GUI) in the Brick-Force project. The purpose of this class is to handle the management and display of system messages in two different areas of the screen.

The class has several private variables, including coordinates for the top-left and bottom-right corners of the GUI elements, as well as rectangles to define the size and position of the GUI elements. It also has queues to store the system messages that need to be displayed.

The class has several public properties and methods that can be used to interact with the system messages. The "SetCoord" method is used to set the coordinates of the GUI elements based on the width of the screen. The "Start" method initializes the queues for the system messages. The "AddMessage" and "AddMessageCenter" methods are used to add new system messages to the respective queues.

The "OnGUI" method is responsible for rendering the GUI elements and displaying the system messages. It uses the "GUI" class to begin and end a GUI group for the first GUI element. It then checks if there is a system message to display and calculates the length of the message if needed. It then creates a rectangle to define the position of the message and uses the "GUI.Label" method to display the message.

The "Update" method is called every frame and is responsible for updating the system messages. It checks if there is a new system message to display and updates the status delta and length calculation variables accordingly. It also checks if the system message has reached the end of the GUI element and if so, it removes the message from the queue and resets the status delta and length calculation variables.

Overall, this class provides a way to manage and display system messages in the Brick-Force project. It allows for the addition of new messages, as well as the automatic scrolling and removal of messages when they reach the end of the GUI element.
## Questions: 
 1. What is the purpose of the `SystemMessage` class?
- The `SystemMessage` class is used to display system messages on the screen.

2. What is the significance of the `SysMsg` property?
- The `SysMsg` property returns the value of the `SysMessageCenter` field, which is the system message displayed at the center of the screen.

3. What is the purpose of the `Update` method?
- The `Update` method is responsible for updating the system messages and their display on the screen based on certain conditions and time intervals.