[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\ObscuredPrefsTest.cs)

The code provided is a part of the Brick-Force project and is responsible for saving and reading game data using PlayerPrefs and ObscuredPrefs. 

The `OnApplicationQuit` method is called when the application is about to quit. It deletes the keys for "money", "lifeBar", "playerName", "gameComplete", "demoLong", "demoDouble", "demoByteArray", and "demoVector3" from both PlayerPrefs and ObscuredPrefs. This ensures that the saved data is cleared when the game is closed.

The `Awake` method is called when the script instance is being loaded. It sets a new encryption key for ObscuredPrefs using the value provided in the `encryptionKey` variable. This encryption key is used to encrypt and decrypt the saved data, making it more secure than using PlayerPrefs.

The `SaveGame` method is responsible for saving the game data. It takes a boolean parameter `obscured` which determines whether to use ObscuredPrefs or PlayerPrefs for saving the data. If `obscured` is true, it saves the data using ObscuredPrefs by calling various methods like `SetInt`, `SetFloat`, `SetString`, etc. These methods store the data with encryption using the provided encryption key. If `obscured` is false, it saves the data using PlayerPrefs by calling methods like `SetInt`, `SetFloat`, `SetString`, etc. These methods store the data without encryption.

The `ReadSavedGame` method is responsible for reading the saved game data. It takes a boolean parameter `obscured` which determines whether to use ObscuredPrefs or PlayerPrefs for reading the data. If `obscured` is true, it reads the data using ObscuredPrefs by calling various methods like `GetInt`, `GetFloat`, `GetString`, etc. These methods retrieve the encrypted data and decrypt it using the encryption key. If `obscured` is false, it reads the data using PlayerPrefs by calling methods like `GetInt`, `GetFloat`, `GetString`, etc. These methods retrieve the data without decryption.

The retrieved data is then stored in the `gameData` variable, which can be accessed by other parts of the project. The format of the stored data is different depending on whether ObscuredPrefs or PlayerPrefs were used.

Overall, this code provides a way to save and read game data securely using encryption with ObscuredPrefs or without encryption with PlayerPrefs. It allows for the protection of sensitive game data from being easily tampered with by external sources.
## Questions: 
 1. What is the purpose of ObscuredPrefs and how does it differ from regular PlayerPrefs?
- ObscuredPrefs is a class that provides a way to securely store and retrieve player preferences in Unity. It differs from regular PlayerPrefs by encrypting the data to prevent easy tampering.

2. What happens when the game is saved with the `obscured` parameter set to true?
- When the game is saved with `obscured` set to true, the game data is saved using ObscuredPrefs, which encrypts the data before storing it.

3. How can the saved game data be accessed and displayed?
- The saved game data can be accessed and displayed by calling the `ReadSavedGame` method and passing in the `obscured` parameter. The method retrieves the saved data using either ObscuredPrefs or regular PlayerPrefs, depending on the value of `obscured`.