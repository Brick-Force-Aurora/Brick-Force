[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\SystemInform.cs)

The `SystemInform` class in the Brick-Force project is responsible for displaying system messages to the user. It manages two types of system messages: messages that appear at the top of the screen and messages that appear in the center of the screen.

The class contains several private variables, including `crdLT` and `crdRB`, which define the coordinates of the top-left and bottom-right corners of the message display area. The `rcSysMessage` and `rcSysMessageCenter` variables are `Rect` objects that represent the position and size of the message display areas. The `statusMessageQ` and `statusMessageCenterQ` variables are queues that store the system messages to be displayed. The `SysMessage` and `SysMessageCenter` variables store the currently displayed messages. The `bReceived` and `bReceivedCenter` variables indicate whether new messages have been received. The `statusDelta` and `statusDeltaCenter` variables keep track of the time elapsed since the last message was displayed.

The class also has a reference to the `Lobby` class and a static instance of the `SystemInform` class.

The `Awake` method is called when the object is created and ensures that the object is not destroyed when a new scene is loaded.

The `SetToolbarSize` method sets the width of the toolbar.

The `SetCoord` method sets the position and size of the message display areas based on the width of the toolbar.

The `AddMessage` method adds a new message to the top message queue. If there are no messages currently being displayed, the `bReceived` flag is set to true.

The `AddMessageCenter` method adds a new message to the center message queue. If there is no message currently being displayed in the center, the `bReceivedCenter` flag is set to true. If there is already a message being displayed, the `statusDeltaCenter` variable is set to a value that will cause the message to be displayed for a shorter duration.

The `Start` method initializes the message queues.

The `VerifyLobby` method finds the `Lobby` object in the scene.

The `Update` method is called every frame. It updates the state of the system messages. If a new message has been received, it sets the appropriate variables and dequeues the message from the queue. If there is a message being displayed and it has not reached the end of the display area, it updates the `statusDelta` variable. If there is a message being displayed in the center and it has reached the end of its display duration, it dequeues the message from the queue.

The `OnGUI` method is responsible for rendering the system messages on the screen. It sets the GUI skin and depth, and then checks if there is a message to be displayed in the center. If there is, it calculates the position and opacity of the message based on the `statusDeltaCenter` variable and renders the message using different colors. Finally, it resets the GUI settings.

The `DoScrollMessage` method is responsible for rendering the top system message. It calculates the position of the message based on the `statusDelta` variable and the width of the toolbar, and then renders the message using the `LabelUtil.TextOut` method.

In summary, the `SystemInform` class manages the display of system messages in the Brick-Force project. It provides methods to add messages to the top and center message queues, and updates and renders the messages on the screen. The class also has a reference to the `Lobby` class and a static instance of itself for easy access.
## Questions: 
 1. What is the purpose of the `SystemInform` class?
- The `SystemInform` class is responsible for displaying system messages in the game.

2. What is the significance of the `SysMessage` and `SysMessageCenter` variables?
- The `SysMessage` variable stores the current system message to be displayed on the screen, while the `SysMessageCenter` variable stores the current center system message to be displayed on the screen.

3. What is the purpose of the `DoScrollMessage` method?
- The `DoScrollMessage` method is responsible for scrolling and displaying the system message on the screen.