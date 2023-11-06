[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\GrbMain.cs)

The code provided is a script for the GrbMain class in the Brick-Force project. This class is responsible for handling the main menu screen of the game. 

The script starts by defining an enum called STEP, which represents the different steps or states that the main menu can be in. These steps include FADE_IN, WAIT, FADE_OUT, AUTO_LOGIN, AUTO_LOGIN_TO_RUNUP, and AUTO_LOGIN_TO_NETMARBLE.

The script also includes public variables for the grbLogo texture and the fade in, wait, and fade out times. These variables can be set in the Unity editor to customize the appearance and timing of the main menu.

The Awake() method is empty and does not contain any code.

The Start() method calls the FadeIn() method, which sets the step to FADE_IN and initializes the deltaTime variable to 0.

The OnGUI() method is responsible for drawing the main menu screen. It checks if the grbLogo texture is not null and then sets the GUI skin and GUI color based on the current step. The grbLogo texture is drawn at the center of the screen using the DrawTexture method from the TextureUtil class.

The FadeIn() method sets the step to FADE_IN and resets the deltaTime variable to 0.

The FadeOut() method sets the step to FADE_OUT and resets the deltaTime variable to 0.

The Wait() method sets the step to WAIT and resets the deltaTime variable to 0.

The MoveNext() method is called in the Update() method when the current step is FADE_OUT and the deltaTime exceeds the fadeOutTime. This method checks if certain levels can be loaded and then performs different actions based on the build options and tokens. For example, if the build option is set to runup, it extracts tokens from the web parameters and attempts to open a TCP connection. If the build option is not runup, it extracts a token and checks if it is empty. If it is empty, it loads the Login level. Otherwise, it sets the AutoLogin option and opens a TCP connection.

The Update() method is called every frame and updates the deltaTime variable. It also checks the current step and performs different actions based on the elapsed time and the current step. For example, if the current step is FADE_IN and the deltaTime exceeds the fadeInTime, it calls the Wait() method.

The OnRoundRobin() method is called when the round robin event occurs. It opens a TCP connection to the Brick-Force server.

The OnServiceFail() method is called when a service fails. It exits the game.

The OnSeed() method is called when the seed event occurs. It sends different requests to the server based on the current step.

The OnLoginFail() method is called when a login fails. It exits the game.

The OnLoginFailMessage() method is called when a login fail message is received. It displays an exit confirm dialog with the message.

In summary, this script handles the main menu screen of the Brick-Force game. It controls the fading in and out of the main menu logo, handles different steps and actions based on the elapsed time and build options, and communicates with the server for login and other requests.
## Questions: 
 1. What is the purpose of the `FadeIn`, `FadeOut`, and `Wait` methods?
- The `FadeIn` method sets the step to `FADE_IN` and resets the `deltaTime` to 0. The `FadeOut` method sets the step to `FADE_OUT` and resets the `deltaTime` to 0. The `Wait` method sets the step to `WAIT` and resets the `deltaTime` to 0. These methods are used to control the fading in and out of the GUI logo.

2. What is the purpose of the `MoveNext` method?
- The `MoveNext` method checks if certain levels can be loaded and performs different actions based on the conditions. It extracts values from parameters and sets the `step` accordingly. It also handles different scenarios for auto login.

3. What is the purpose of the `OnRoundRobin`, `OnServiceFail`, `OnSeed`, `OnLoginFail`, and `OnLoginFailMessage` methods?
- The `OnRoundRobin` method opens a new `SockTcp` connection to the Brick-Force server. The `OnServiceFail` method exits the application. The `OnSeed` method sends different requests based on the current `step`. The `OnLoginFail` method exits the application. The `OnLoginFailMessage` method displays an exit confirmation dialog with a specific message. These methods handle different events and actions based on the current state of the application.