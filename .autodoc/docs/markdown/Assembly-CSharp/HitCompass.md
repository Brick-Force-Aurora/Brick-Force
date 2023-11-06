[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\HitCompass.cs)

The `HitCompass` class is a script that is part of the larger Brick-Force project. It is responsible for managing and displaying directional hit indicators on the player's screen.

The script contains several public variables that can be customized in the Unity editor. These variables include `guiDepth`, which determines the layer depth of the GUI elements, `compassColor`, which sets the color of the hit compass, `compassColorChild`, which sets the color of the hit compass for child objects, and `zeroColor`, which sets the color of the hit compass when the direction is zero.

The script also contains a private variable `lifeTime` which determines the duration for which the hit compass is displayed on the screen. Additionally, there is an array of `Texture2D` objects called `hitCompass`, which stores the different textures for the hit compass.

The `Start` method is called when the script is initialized. It initializes the `attackers` dictionary and finds the main camera in the scene. If the camera is found, it gets the `CameraController` component attached to it.

The `OnDirectionalHit` method is called when a directional hit occurs. It checks if the current room type is not an escape room. If it is not, it retrieves the attacker's game object using the `BrickManManager` class. If the attacker's game object is found, it checks if the attacker is already in the `attackers` dictionary. If it is, it resets the attacker's hit compass. If the attacker is not in the dictionary, it creates a new `DirAttacker` object and adds it to the `attackers` dictionary.

The `OnGUI` method is responsible for drawing the hit compasses on the screen. It checks if the GUI is enabled and if the current dialog is not modal. It then iterates through the `attackers` dictionary and calls the `Draw` method on each `DirAttacker` object. If the `Draw` method returns false, it adds the attacker's key to a list. After iterating through all the attackers, it removes the attackers in the list from the `attackers` dictionary. Finally, it resets the GUI settings to their original values.

The `Update` method is called every frame. It iterates through the `attackers` dictionary and calls the `Update` method on each `DirAttacker` object. If the `Update` method returns false, it adds the attacker's key to a list. After iterating through all the attackers, it removes the attackers in the list from the `attackers` dictionary.

In summary, the `HitCompass` script manages and displays hit compass indicators on the player's screen. It keeps track of attackers and their hit compasses, and updates and draws them accordingly. This script is likely used in the larger Brick-Force project to provide visual feedback to the player when they are being attacked from a certain direction.
## Questions: 
 1. What is the purpose of the `HitCompass` class?
- The `HitCompass` class is responsible for managing and displaying directional hit indicators on the screen.

2. What is the significance of the `attackers` dictionary?
- The `attackers` dictionary stores information about the attackers and their corresponding directional hit indicators.

3. What conditions need to be met for a directional hit indicator to be added to the `attackers` dictionary?
- A directional hit indicator will be added to the `attackers` dictionary if the current room type is not an escape room, the attacker exists in the game, and the player is not below 12 years old.