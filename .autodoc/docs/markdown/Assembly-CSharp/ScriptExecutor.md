[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\ScriptExecutor.cs)

The `ScriptExecutor` class is responsible for executing a sequence of script commands in the Brick-Force project. It contains a `Run` method that takes a `BfScript` object as a parameter and enqueues all the commands from the script into a `scriptCmdQueue`. 

The `ExecuteCommand` method is called internally by the `Update` method and is responsible for executing each command in the queue. It checks the type of the command and calls the corresponding method to perform the desired action. For example, if the command is of type "EnableScript", it calls the `EnableScript` method with the appropriate parameters.

The class also contains several methods that correspond to the different types of commands that can be executed. These methods perform actions such as enabling or disabling scripts, showing dialogs, playing sounds, giving weapons to the player, and setting missions.

For example, the `GiveWeapon` method finds the player object in the scene and retrieves the `LocalController` component attached to it. It then calls the `PickupFromTemplate` method of the `LocalController` component to give the player a weapon based on the provided weapon code. It also handles some additional logic related to displaying tutorial help dialogs.

The `Update` method is called every frame and checks if there are any commands in the queue. If there are, it dequeues the next command and calls the `ExecuteCommand` method to execute it. If there are no more commands in the queue and there are no active script dialogs or alarms, the `Update` method destroys the `ScriptExecutor` object.

Overall, the `ScriptExecutor` class provides a way to run a sequence of script commands in the Brick-Force project. It handles the execution of different types of commands and performs the necessary actions based on the command type. This class is likely used in the larger project to control scripted events, dialogues, and gameplay mechanics.
## Questions: 
 1. What is the purpose of the `Run` method in the `ScriptExecutor` class?
- The `Run` method initializes a queue of `ScriptCmd` objects and adds each `ScriptCmd` from the `CmdList` of the `BfScript` parameter to the queue.

2. What is the purpose of the `GiveWeapon` method in the `ScriptExecutor` class?
- The `GiveWeapon` method finds the game object with the name "Me" and retrieves the `LocalController` component from it. It then calls the `PickupFromTemplate` method of the `LocalController` component to give the player a weapon based on the `weaponCode` parameter.

3. What is the purpose of the `SetMission` method in the `ScriptExecutor` class?
- The `SetMission` method finds the game object with the name "Main" and retrieves the `MissionLog` component from it. It then calls the `SetMission` method of the `MissionLog` component to set the mission details based on the provided parameters.