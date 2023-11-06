[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\MissionLoadManager.cs)

The `MissionLoadManager` class is responsible for loading mission reward data from either a local file system or a web server. It also provides methods to change the reward values for specific missions.

The class has several private fields that store information about the rewards for each mission. These fields include arrays for storing whether each reward is in points or not (`reward1IsPoints`, `reward2IsPoints`, `reward3IsPoints`), and arrays for storing the counts of each reward (`reward1Counts`, `reward2Counts`, `reward3Counts`).

The class also has several public properties that allow access to these private fields.

The `Awake` method is called when the object is initialized and ensures that the object is not destroyed when a new scene is loaded.

The `Load` method is responsible for loading the mission reward data. It first checks if the game is running in a web player or not. If it is, it starts a coroutine called `LoadFromWWW` which loads the data from a web server. If it is not a web player, it calls the `LoadFromLocalFileSystem` method to load the data from a local file system.

The `LoadFromWWW` method uses the `WWW` class to download the mission reward data from a specified URL. It then uses a `MemoryStream` and a `BinaryReader` to read the downloaded data. The data is then parsed using a `CSVLoader` class, and the parsed values are stored in the appropriate fields.

The `LoadFromLocalFileSystem` method checks if the necessary directory for the resource files exists. If it does, it constructs the path to the mission reward file and uses the `CSVLoader` class to load and parse the data.

The `Parse` method takes a `CSVLoader` object as a parameter and uses it to parse the mission reward data. It iterates over each row in the data and extracts the values for each reward. The extracted values are then stored in the appropriate fields.

The `ChangeReward` method allows for changing the reward values for a specific mission. It takes three parameters: `step` (the mission step), `forcePoint` (the force point value), and `freeCoin` (the free coin value). It first checks if the target mission index is within the range of the reward arrays. If it is, it updates the corresponding reward values based on the provided parameters.

Overall, the `MissionLoadManager` class is responsible for loading and managing the mission reward data for the game. It provides methods to load the data from either a local file system or a web server, and allows for changing the reward values for specific missions.
## Questions: 
 1. What is the purpose of the `MissionLoadManager` class?
- The `MissionLoadManager` class is responsible for loading mission reward data from either a web server or the local file system.

2. How does the `MissionLoadManager` class handle loading mission reward data from a web server?
- The `LoadFromWWW` method uses a `WWW` object to download the mission reward data from a specified URL and then parses the data using a `CSVLoader` object.

3. How does the `MissionLoadManager` class handle loading mission reward data from the local file system?
- The `LoadFromLocalFileSystem` method checks if the necessary directory and file exist, and if so, it loads the data using a `CSVLoader` object. If the data is not found or fails to load, it logs an error message.