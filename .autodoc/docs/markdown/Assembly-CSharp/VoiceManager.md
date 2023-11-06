[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\VoiceManager.cs)

The `VoiceManager` class is a script that manages audio clips in the Brick-Force project. It provides methods to add, retrieve, and play audio clips. 

The class has two dictionaries, `dic` and `dic2`, which are used to store audio clips. The `Add` and `Add2` methods are used to add audio clips to the dictionaries. The `Get` and `Get2` methods are used to retrieve audio clips from the dictionaries based on a given key. The keys are converted to lowercase before performing the dictionary lookup.

The `Play0`, `Play`, and `Play2` methods are used to play audio clips. The `Play0` method retrieves an audio clip using the `Get` method and plays it using the `PlaySound` method from the `GlobalVars` class. The `Play` method checks if the player is a "Yang" player using the `IsYang` property from the `MyInfoManager` class. If the player is a "Yang" player, it calls the `Play2` method to play the audio clip. Otherwise, it behaves the same as the `Play0` method. The `Play2` method retrieves an audio clip using the `Get2` method and plays it using the `PlaySound` method from the `GlobalVars` class.

The `Clear` and `Clear2` methods are used to clear the dictionaries. They check if the dictionaries are not null and have elements before clearing them.

The `Awake` method is called when the script is initialized. It initializes the dictionaries and ensures that the `VoiceManager` object is not destroyed when a new scene is loaded using the `DontDestroyOnLoad` method.

Overall, the `VoiceManager` class provides a centralized way to manage audio clips in the Brick-Force project. It allows for adding, retrieving, and playing audio clips, and provides separate dictionaries for different types of audio clips. The class also includes methods to clear the dictionaries and ensures that the `VoiceManager` object persists across scene changes.
## Questions: 
 1. What is the purpose of the `VoiceManager` class?
- The `VoiceManager` class is responsible for managing audio clips and playing them in the game.

2. What is the difference between `dic` and `dic2`?
- `dic` and `dic2` are both dictionaries that store audio clips, but they are separate instances and likely serve different purposes within the code.

3. What is the significance of the `Play0`, `Play`, and `Play2` methods?
- The `Play0`, `Play`, and `Play2` methods are used to play audio clips. `Play0` and `Play2` directly use the `Get` and `Get2` methods to retrieve the audio clip, while `Play` checks if a specific condition is met before playing the audio clip.