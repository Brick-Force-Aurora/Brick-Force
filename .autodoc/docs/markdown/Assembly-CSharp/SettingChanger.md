[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\SettingChanger.cs)

The `SettingChanger` class in the `Brick-Force` project is responsible for changing and applying various settings related to the game's display and graphics quality. 

The `OnSettingChange` method is called whenever a new setting is received. It takes a `SettingParam` object as a parameter, which contains the new settings. The method assigns the `SettingParam` object to the `sp` variable and then starts a coroutine called `ApplyChange`.

The `ApplyChange` coroutine is responsible for applying the new settings. It first checks if the width and height of the new settings are greater than or equal to the maximum width and height of the screen resolutions available. If they are, it sets the `fullscreen` variable to `true`. Then, it calls `Screen.SetResolution` to change the screen resolution to the new width, height, and fullscreen mode. It also saves the new width, height, and fullscreen mode to the player preferences using `PlayerPrefs.SetInt`.

Next, it sets the quality level of the game using `QualitySettings.SetQualityLevel` based on the `qualityLevel` property of the `SettingParam` object. It also saves the new quality level to the player preferences.

Finally, it sets the `sp` variable to `null` to indicate that the settings have been applied.

The `Start` and `Update` methods in this class are empty and do not have any functionality.

Overall, this code is a part of the larger Brick-Force project and is responsible for changing and applying display and graphics settings in the game. It uses the `SettingParam` object to receive new settings and applies them by changing the screen resolution, fullscreen mode, and quality level. The applied settings are also saved to the player preferences for future use.
## Questions: 
 1. What does the `SettingParam` class contain and how is it used in this code? The `SettingParam` class is used as a parameter in the `OnSettingChange` method and contains information about the desired screen resolution, fullscreen mode, and quality level.
   
2. What is the purpose of the `ApplyChange` coroutine? The `ApplyChange` coroutine sets the screen resolution, fullscreen mode, and quality level based on the values in the `SettingParam` object. It also saves these values to PlayerPrefs for future use.

3. Why is the `Update` method empty? The `Update` method is empty and does not contain any code. This suggests that there is no need for any continuous update or logic in this script.