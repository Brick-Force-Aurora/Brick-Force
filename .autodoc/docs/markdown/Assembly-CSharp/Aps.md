[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\Aps.cs)

The code provided is a part of the Brick-Force project and is contained in a file called "Aps.cs". This file defines a class called "Aps" that inherits from the "MonoBehaviour" class provided by the Unity engine.

The purpose of this code is to manage and provide access to data related to the APS (Advanced Player Statistics) system in the game. The APS system categorizes players into different types (represented by the "APS_TYPE" enum) such as "NONE", "CHINESE", and "KOREAN". Each APS type has a corresponding set of data stored in an array of "ApsData" objects.

The class has a private static instance variable "_instance" and a public static property "Instance" that provides a way to access the singleton instance of the "Aps" class. The "Instance" property uses the Unity method "Object.FindObjectOfType" to find an existing instance of the "Aps" class in the scene. If no instance is found, it logs an error message.

The "Awake" method is called when the object is initialized and it uses the "Object.DontDestroyOnLoad" method to prevent the "Aps" object from being destroyed when a new scene is loaded.

The class provides several public methods to interact with the APS data. The "SetLevel" method takes an APS type and a level as parameters and sets the current APS type and level. It then returns a warning message from the corresponding "ApsData" object if the level is within the range of warnings.

The "GetCurLevelIcon" method returns a texture icon for the current APS type and level. It takes a boolean parameter "flip" which determines whether to return the normal icon or a flipped version of it.

The "GetCurTooltip" method returns the tooltip text for the current APS type and level. It uses the "StringMgr.Instance.Get" method to retrieve the localized tooltip text from a string manager.

Overall, this code provides a centralized way to manage and access APS data in the game. It allows other parts of the project to retrieve information about APS types, levels, icons, and tooltips.
## Questions: 
 1. **Question:** What is the purpose of the `ApsData` array and how is it used in this code?
   - **Answer:** The `ApsData` array is used to store data related to different APS types. It is accessed to retrieve warnings, icons, and tooltips based on the current APS type and level.

2. **Question:** What is the significance of the `APS_TYPE` enum and how is it used in this code?
   - **Answer:** The `APS_TYPE` enum represents different types of APS. It is used to set the current APS type and retrieve data specific to that type, such as warnings, icons, and tooltips.

3. **Question:** What is the purpose of the `SetLevel` method and how does it determine the warning message to return?
   - **Answer:** The `SetLevel` method sets the current APS type and level, and returns a warning message based on the current APS type and level. It checks if the level is within the range of warnings available for the given APS type.