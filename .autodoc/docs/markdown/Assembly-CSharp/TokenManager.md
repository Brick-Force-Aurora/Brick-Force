[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\TokenManager.cs)

The `TokenManager` class is a part of the Brick-Force project and is responsible for managing tokens. Tokens are objects that represent a specific type of item or currency within the game. This class provides functionality to set and retrieve the current token, as well as get the string representation of the token.

The class has a private static variable `_instance` which holds the singleton instance of the `TokenManager` class. The `Instance` property is a getter that returns the singleton instance. If the `_instance` variable is null, it tries to find an existing instance of the `TokenManager` class using `Object.FindObjectOfType`. If no instance is found, it logs an error message. This ensures that there is only one instance of the `TokenManager` class throughout the game.

The `tokens` variable is an array of `Token` objects. Each `Token` object represents a specific type of token in the game. The `currentToken` variable holds the currently selected token.

The `Awake` method is called when the object is initialized and it prevents the `TokenManager` object from being destroyed when a new scene is loaded using `Object.DontDestroyOnLoad(this)`. This ensures that the `TokenManager` persists across different scenes.

The `Start` method is called when the object is enabled and it sets the `currentToken` to the token specified by the `BuildOption.Instance.Props.TokenType` value. This allows the game to start with a specific token selected.

The `SetCurrentToken` method takes a `Token.TYPE` parameter and sets the `currentToken` to the token of that type. This allows the game to change the current token based on user input or game logic.

The `GetTokenString` method returns the string representation of the `currentToken` by calling `StringMgr.Instance.Get(currentToken.name)`. This suggests that there is a `StringMgr` class that manages string resources and the `Get` method retrieves the string associated with the given token name.

Overall, the `TokenManager` class provides functionality to manage tokens in the game, including setting the current token, retrieving the string representation of the token, and ensuring that there is only one instance of the `TokenManager` class throughout the game.
## Questions: 
 1. What is the purpose of the TokenManager class?
- The TokenManager class is responsible for managing tokens and providing access to the current token.

2. How does the TokenManager determine the current token?
- The current token is determined by the value of `BuildOption.Instance.Props.TokenType`, which is used as an index to access the corresponding token in the `tokens` array.

3. What is the purpose of the `GetTokenString()` method?
- The `GetTokenString()` method returns the localized string for the name of the current token, using the `StringMgr.Instance.Get()` method.