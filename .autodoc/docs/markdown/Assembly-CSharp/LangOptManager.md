[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\LangOptManager.cs)

The `LangOptManager` class is responsible for managing language options and fonts in the Brick-Force project. It contains various properties, methods, and event handlers to handle language selection, font loading, and progress tracking.

The `LANG_OPT` enum represents the available language options in the game, such as Korean, English, Simplified Chinese, etc. The `langNames` array stores the names of these languages.

The `languages` array holds the textures for each language option, which can be used to display language flags or icons in the user interface.

The `levels` array stores the names of the game levels. This is used to track the progress of level streaming in the `Update` method.

The `skinHolders` array is not used in this code and its purpose is unclear.

The `fonts` array holds the fonts for each language option. The `fontNames` array stores the names of these fonts, and the `fontVersions` array stores the versions of the fonts.

The `toss` array contains some text strings related to the language options.

The `onceStreamed` boolean flag is used to track whether the levels have been streamed once.

The `streamingProgress` float variable stores the progress of level streaming.

The `LangOpt` property gets and sets the current language option. It also saves the selected language option in the `PlayerPrefs` for future use.

The `CurFont` property returns the font for the current language option.

The `Instance` property is a singleton pattern implementation that returns the instance of the `LangOptManager` class.

The `IsFontReady` property returns a boolean value indicating whether the font is ready for use.

The `GetAgbById` method returns a string based on the provided ID.

The `GetAgbCurrent` method returns a string based on the current language option.

The `GetLangName` method returns the name of the language based on the provided ID.

The `Awake` method initializes some variables and ensures that the `LangOptManager` object is not destroyed when a new scene is loaded.

The `SetFont` method sets the font for the current language option. If the font is already loaded, it updates the font in the `GUISkinFinder` class. Otherwise, it loads the font using the `AssetBundleLoadManager` class.

The `OnDestroy` method is empty and does not have any functionality.

The `AlreadyLoadedFont` method checks if a font for a specific language is already loaded and returns its index in the `fonts` array.

The `Start` method initializes the `langOpt` variable based on the default language option specified in the `BuildOption` class. If language selection is enabled, it checks if the selected language is supported and falls back to the default language if not. It also initializes the `fonts` array and loads the font for the current language option using the `AssetBundleLoadManager` class.

The `readyFont` method checks if the font is ready for use. If so, it updates the font in the `GUISkinFinder` class and sets the `isFontReady` flag to true.

The `Update` method checks if the font is ready and if the game is running in a web player. If so, it calculates the progress of level streaming based on the `levels` array and sets the `onceStreamed` flag if the streaming progress reaches 99.99999%.

The `OnGUI` method is responsible for displaying the loading progress if the game is running in a web player and the streaming progress is less than 99.99999%.

Overall, the `LangOptManager` class provides functionality for managing language options and fonts in the Brick-Force project. It allows the user to select a language, loads the corresponding font, and tracks the progress of level streaming.
## Questions: 
 1. What is the purpose of the `LangOptManager` class?
- The `LangOptManager` class manages language options and fonts for the game.
2. How does the `LangOptManager` class handle font loading?
- The `LangOptManager` class checks if the font for the selected language is already loaded. If not, it requests the font to be loaded from an asset bundle.
3. What is the purpose of the `readyFont()` method?
- The `readyFont()` method checks if the font has been loaded from the asset bundle and updates the GUI skin with the new font.