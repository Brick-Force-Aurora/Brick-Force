[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\CodeStage.AntiCheat.Detectors\InjectionDetector.cs)

The code provided is a part of the Brick-Force project and is responsible for detecting code injection in the game. The InjectionDetector class is a MonoBehaviour that can be attached to a GameObject in the Unity scene. 

The purpose of this code is to monitor the game for any unauthorized code injection attempts. It does this by checking the assemblies that are currently loaded in the game and comparing them to a list of allowed assemblies. If an assembly is found that is not in the allowed list, it triggers the onInjectionDetected callback and takes appropriate action.

The code starts by defining a private class called AllowedAssembly, which represents an assembly that is allowed to be loaded in the game. It has two properties: name, which is the name of the assembly, and hashes, which is an array of integer hashes representing the assembly.

The InjectionDetector class has several public properties and methods that can be used to control its behavior. The autoDispose property determines whether the InjectionDetector GameObject should be automatically destroyed when an injection is detected. The keepAlive property determines whether the InjectionDetector should be kept alive when a new level is loaded. The onInjectionDetected property is a callback that is triggered when an injection is detected.

The code also defines a static instance of the InjectionDetector class, which can be accessed through the Instance property. This ensures that there is only one instance of the InjectionDetector in the game.

The code provides methods for starting and stopping the detection process. The StartDetection method starts the detection process by checking the currently loaded assemblies and comparing them to the allowed assemblies. If an unauthorized assembly is found, it triggers the onInjectionDetected callback. The StopMonitoring method stops the detection process by removing the event handler for new assembly loads.

The code also includes several private methods that are used internally. The Awake method checks if there is already an instance of the InjectionDetector and destroys itself if there is. It also checks if the InjectionDetector is placed correctly in the scene. The OnLevelWasLoaded method is called when a new level is loaded and disposes of the InjectionDetector if the keepAlive property is set to false. The OnDisable method stops the detection process when the InjectionDetector is disabled. The OnApplicationQuit method disposes of the InjectionDetector when the application is quit.

The LoadAndParseAllowedAssemblies method is responsible for loading and parsing the allowed assemblies from a text asset. It reads the text asset as a binary stream and decrypts the values using the ObscuredString.EncryptDecrypt method. It then splits the values into an array and creates an AllowedAssembly object for each value. The GetAssemblyHash method calculates a hash value for an assembly based on its name and public key token. The PublicKeyTokenToString method converts a byte array representing the public key token to a string.

In summary, the InjectionDetector class is a component that can be attached to a GameObject in the Unity scene to detect code injection attempts in the game. It monitors the currently loaded assemblies and compares them to a list of allowed assemblies. If an unauthorized assembly is found, it triggers a callback and takes appropriate action.
## Questions: 
 1. What is the purpose of the `InjectionDetector` class?
- The `InjectionDetector` class is used to detect injection of unauthorized assemblies into the project.

2. How does the `InjectionDetector` class determine if an assembly is allowed or not?
- The `InjectionDetector` class checks the name and hash of the loaded assembly against a list of allowed assemblies and their corresponding hashes.

3. What happens when an injection is detected?
- When an injection is detected, the `InjectionDetected` method is called, which triggers the `onInjectionDetected` callback if it is not null. If `autoDispose` is true, the `Dispose` method is called to destroy the `InjectionDetector` instance. Otherwise, the `StopMonitoringInternal` method is called to stop the detection process.