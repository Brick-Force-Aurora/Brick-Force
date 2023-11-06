[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\_Emulator\Config.cs)

The code provided is a part of the Brick-Force project and is located in the `_Emulator` namespace. It defines a class called `Config` that is responsible for managing and loading/saving configuration settings for the project.

The `Config` class has several public properties that represent different configuration settings. These settings include the color of the crosshair, the hue of the crosshair color, whether to use USK textures, the axis ratio for camera movement, whether to allow only one client per IP, whether to block connections, whether to automatically clear dead clients, and the maximum number of connections.

The class also has a constructor that calls the `LoadConfigFromDisk` method to load the configuration settings from a CSV file located at the specified path. The `LoadConfigFromDisk` method uses a `CSVLoader` object to load the CSV file and then retrieves the values for each configuration setting from the loaded CSV data. It also applies some additional transformations to the loaded values, such as converting the crosshair color from RGB to HSV and calculating the crosshair hue.

The `Config` class also has a `SaveConfigToDisk` method that saves the current configuration settings to a CSV file located at the specified path. It uses the `CSVLoader` object to set the values for each configuration setting in the CSV data and then saves the CSV data to the file.

Additionally, the `Config` class has three methods: `ApplyAxisRatio`, `ApplyUskTextures`, and `ApplyCrosshairHue`. These methods are responsible for applying the changes made to the configuration settings. For example, the `ApplyAxisRatio` method adjusts the camera's ySpeed based on the axis ratio, the `ApplyUskTextures` method updates various build options based on the `uskTextures` setting, and the `ApplyCrosshairHue` method updates the crosshair color based on the `crosshairHue` setting.

Overall, the `Config` class provides a way to manage and persist configuration settings for the Brick-Force project. It allows the project to load and save these settings from a CSV file and apply the changes made to the settings in the appropriate places.
## Questions: 
 1. What is the purpose of the `Config` class?
- The `Config` class is responsible for loading and saving configuration settings from a CSV file.

2. What are the default values for the configuration settings?
- The default values for the configuration settings are as follows:
  - `crosshairColor` is set to `Color.green`
  - `crosshairHue` is set to `90f`
  - `uskTextures` is set to `false`
  - `axisRatio` is set to `2.25f`
  - `oneClientPerIP` is set to `true`
  - `blockConnections` is set to `false`
  - `autoClearDeadClients` is set to `false`
  - `maxConnections` is set to `16`

3. What is the purpose of the `ApplyAxisRatio()`, `ApplyUskTextures()`, and `ApplyCrosshairHue()` methods?
- The `ApplyAxisRatio()` method adjusts the camera's ySpeed based on the axisRatio value.
- The `ApplyUskTextures()` method updates various build options based on the uskTextures value.
- The `ApplyCrosshairHue()` method updates the crosshairColor based on the crosshairHue value.