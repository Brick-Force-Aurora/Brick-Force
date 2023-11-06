[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\MissionManager.cs)

The `MissionManager` class is a component in the Brick-Force project that manages missions for the player. It keeps track of a dictionary of missions, where each mission is represented by an instance of the `Mission` class. The `MissionManager` class provides methods to update and manipulate the missions.

The `MissionManager` class has several properties and methods that are used to interact with the missions. 

The `HaveMission` property returns a boolean value indicating whether there are any missions currently active. It checks if the count of missions in the `dicMission` dictionary is greater than zero.

The `CanReceiveMission` property returns a boolean value indicating whether the player can receive a new mission. It checks if the `wait` variable is less than zero and the count of missions in the `dicMission` dictionary is less than or equal to zero.

The `CanCompleteMission` property returns a boolean value indicating whether there are any missions that can be completed. It iterates through each mission in the `dicMission` dictionary and checks if the `CanComplete` property of the mission is true.

The `Instance` property is a singleton implementation that returns the instance of the `MissionManager` class. It ensures that there is only one instance of the `MissionManager` class in the project.

The `Awake` method initializes the `dicMission` dictionary and ensures that the `MissionManager` object is not destroyed when a new scene is loaded.

The `Start` method initializes the `wait` variable to -1 and the `deltaTime` variable to 0.

The `Update` method is called every frame and updates the `deltaTime` variable. If the `deltaTime` is greater than 1 second, it decreases the `wait` variable by 1 if it is greater than or equal to 0.

The `Progress` method updates the progress of a specific mission. It takes in the mission index and the progress value and sets the progress of the corresponding mission in the `dicMission` dictionary.

The `UpdateAlways` method updates the progress and completion status of a mission. If the mission already exists in the `dicMission` dictionary, it updates the progress and completion status of the mission. If the mission does not exist, it creates a new mission and adds it to the `dicMission` dictionary.

The `SetMissionWait` method sets the `wait` variable to a specified value.

The `Del` method removes a mission from the `dicMission` dictionary based on its index.

The `Clear` method clears all missions from the `dicMission` dictionary.

The `CompletedCount` method returns the number of completed missions in the `dicMission` dictionary.

The `ToArray` method converts the missions in the `dicMission` dictionary to an array and returns it. The missions are sorted by their index using a `SortedList` before being converted to an array.

The `Complete` method completes missions based on a specified count. If the count is greater than or equal to 3, it clears all missions. Otherwise, it completes missions that can be completed.

Overall, the `MissionManager` class provides functionality to manage missions in the Brick-Force project. It allows for the creation, updating, and completion of missions, as well as providing information about the current state of the missions.
## Questions: 
 1. **What is the purpose of the MissionManager class?**
The MissionManager class is responsible for managing missions in the game. It keeps track of active missions, their progress, and allows for completing missions.

2. **What is the significance of the `wait` variable?**
The `wait` variable is used to determine if the MissionManager can receive new missions. If `wait` is less than 0, it means that the MissionManager can receive new missions. If `wait` is greater than or equal to 0, it means that the MissionManager is currently waiting and cannot receive new missions.

3. **What is the purpose of the `UpdateAlways` method?**
The `UpdateAlways` method is used to update the progress and completion status of a mission. If the mission with the specified index already exists in the `dicMission` dictionary, its progress and completion status are updated. If the mission does not exist, a new mission is added to the dictionary with the specified parameters.