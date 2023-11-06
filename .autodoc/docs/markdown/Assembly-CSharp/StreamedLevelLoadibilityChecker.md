[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\StreamedLevelLoadibilityChecker.cs)

The `StreamedLevelLoadibilityChecker` class is a script that checks if a set of levels can be streamed and loaded in the game. It is a part of the larger Brick-Force project and is used to ensure that the required levels are available and ready to be loaded.

The class has three public variables: `shouldBeStreamedLevel`, `outputDebugMessage`, and two empty methods `Start()` and `Update()`. 

The `shouldBeStreamedLevel` variable is an array of strings that represents the names of the levels that should be streamed and loaded. These level names are provided by the developer and can be set in the Unity editor. 

The `outputDebugMessage` variable is a boolean flag that determines whether debug messages should be displayed. If set to `true`, the script will log an error message using `Debug.LogError()` if a level cannot be loaded. 

The `CanStreamedLevelBeLoaded()` method is the main functionality of the script. It iterates through each level name in the `shouldBeStreamedLevel` array and checks if the level can be loaded using `Application.CanStreamedLevelBeLoaded()` method. If a level cannot be loaded, it logs an error message if `outputDebugMessage` is set to `true` and returns `false`. If all levels can be loaded, it returns `true`.

This script can be used in the larger Brick-Force project to ensure that all required levels are available and ready to be loaded before the game progresses. It can be attached to a game object in the Unity editor and the required level names can be set in the `shouldBeStreamedLevel` array. The script can then be called at the appropriate time to check if the levels are ready to be loaded. If any level is missing or not yet streamed, an error message will be logged and the game can handle this situation accordingly.

Example usage:

```csharp
StreamedLevelLoadibilityChecker levelChecker;

void Start()
{
    levelChecker = GetComponent<StreamedLevelLoadibilityChecker>();
    if (levelChecker.CanStreamedLevelBeLoaded())
    {
        // Load the levels
    }
    else
    {
        // Handle the error
    }
}
```

In this example, the `CanStreamedLevelBeLoaded()` method is called to check if the levels are ready to be loaded. If they are, the levels can be loaded. Otherwise, an error is handled.
## Questions: 
 1. **What is the purpose of the `shouldBeStreamedLevel` array?**
The `shouldBeStreamedLevel` array is used to store the names of the levels that should be streamed. 

2. **What does the `CanStreamedLevelBeLoaded` method do?**
The `CanStreamedLevelBeLoaded` method checks if all the levels in the `shouldBeStreamedLevel` array can be loaded. If any level cannot be loaded, it returns false. 

3. **What is the purpose of the `outputDebugMessage` variable?**
The `outputDebugMessage` variable determines whether or not debug messages should be printed when a level cannot be streamed.