[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/.autodoc\docs\json\Assembly-CSharp\CodeStage.AntiCheat.Detectors)

The `InjectionDetector.cs` and `SpeedHackDetector.cs` files are part of the anti-cheat system in the Brick-Force project. They are designed to detect and respond to unauthorized code injections and speed hacks respectively.

`InjectionDetector.cs` is a MonoBehaviour script that can be attached to a GameObject in the Unity scene. It monitors the game for unauthorized code injection attempts by comparing the currently loaded assemblies to a list of allowed assemblies. If an unauthorized assembly is found, it triggers the `onInjectionDetected` callback and takes appropriate action. This class also provides methods for starting and stopping the detection process. For example, to start the detection process, you would call `InjectionDetector.Instance.StartDetection()`.

`SpeedHackDetector.cs` is another MonoBehaviour script that can be attached to a GameObject in the Unity scene. It monitors the player's movement speed and detects if they are using any speed hacks or cheats. If a speed hack is detected, it triggers a callback function and disposes of the `SpeedHackDetector` instance. To start the speed hack detection process, you would call `SpeedHackDetector.Instance.StartDetection(callback, checkInterval, maxFalsePositives)`.

These scripts can be used in conjunction with other parts of the anti-cheat system to ensure fair gameplay. For example, they could be used alongside a wall hack detector or an aimbot detector. They could also be used in combination with a system that bans or penalizes players who are detected using cheats. 

Here is an example of how you might use these scripts in your game:

```csharp
// Attach the InjectionDetector and SpeedHackDetector to a GameObject
InjectionDetector injectionDetector = gameObject.AddComponent<InjectionDetector>();
SpeedHackDetector speedHackDetector = gameObject.AddComponent<SpeedHackDetector>();

// Start the detection processes
injectionDetector.StartDetection();
speedHackDetector.StartDetection(OnSpeedHackDetected, 1.0f, 3);

// Define the callback function for when a speed hack is detected
void OnSpeedHackDetected()
{
    // Take appropriate action, such as banning the player or displaying a warning message
}
```

In summary, `InjectionDetector.cs` and `SpeedHackDetector.cs` are key components of the anti-cheat system in the Brick-Force project. They provide a way to detect and respond to code injections and speed hacks, helping to ensure fair gameplay.
