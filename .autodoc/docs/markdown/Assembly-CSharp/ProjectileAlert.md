[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\ProjectileAlert.cs)

The `ProjectileAlert` class is responsible for tracking and displaying projectiles in the game. It is a part of the larger Brick-Force project and is used to provide a radar-like functionality to the player.

The class has a private `Texture2D` variable `dirImage` which represents the direction image that will be displayed on the radar. It also has a private `CameraController` variable `cameraController` which is used to get the position of the camera.

The class has a private `Dictionary<int, PTT>` variable `mine` which stores information about the tracked projectiles. The `PTT` class is not provided in the code snippet, but it is assumed to store information about a tracked projectile, such as its weapon type, position, and range.

The `Start` method is called when the game starts. It finds the main camera in the scene and assigns it to the `cameraController` variable. If the camera cannot be found or the `CameraController` component cannot be obtained, an error message is logged.

The `Update` method is empty and does not contain any code.

The `RemoveMine` method is used to remove a tracked projectile from the `mine` dictionary. It takes an `index` parameter which represents the index of the projectile to be removed. If the `mine` dictionary is not null and contains the specified index, the projectile is removed from the dictionary.

The `TrackMine` method is used to track a projectile. It takes parameters such as the `index` of the projectile, the `weapon` type, the `pos` (position) of the projectile, and its `range`. If the `mine` dictionary is null, it is initialized. If the `mine` dictionary does not contain the specified index, a new `PTT` object is created and added to the dictionary. If the dictionary already contains the index, the position and range of the existing `PTT` object are updated.

The `Draw` method is responsible for drawing a tracked projectile on the radar. It takes parameters such as the `weapon` type, the `pos` (position) of the projectile, and its `range`. It uses the `weapon` type to get the corresponding `Texture2D` image from the `TItemManager` class. If the image is not null, it calculates the angle between the camera's forward direction and the direction towards the projectile. It then calculates the position and size of the image on the screen based on the angle and other factors. Finally, it draws the image, the direction image, and a label showing the distance to the projectile.

The `OnGUI` method is called every frame and is responsible for drawing the tracked projectiles on the radar. It iterates over the `mine` dictionary and calls the `Draw` method for each tracked projectile. It also iterates over the game objects in the scene and checks if they are hostile. If a hostile game object has components such as `GdgtGrenade`, `GdgtFlashBang`, or `GdgtXmasBomb`, it gets the weapon type and projectile information from these components and calls the `Draw` method for each projectile.

In summary, the `ProjectileAlert` class is used to track and display projectiles on the radar in the Brick-Force game. It provides methods to add and remove tracked projectiles, and it uses the `Draw` method to draw the projectiles on the screen. The class interacts with other game objects and components to obtain information about the projectiles and their positions.
## Questions: 
 1. What is the purpose of the `ProjectileAlert` class?
- The `ProjectileAlert` class is responsible for tracking and drawing projectiles on the radar.

2. What is the purpose of the `TrackMine` method?
- The `TrackMine` method is used to add or update a projectile's information in the `mine` dictionary.

3. What is the purpose of the `Draw` method?
- The `Draw` method is responsible for drawing a projectile on the radar based on its weapon, position, and range.