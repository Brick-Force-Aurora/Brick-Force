[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\WoundFx.cs)

The `WoundFx` class is responsible for managing and displaying visual effects related to wounds and damage in the game. It is part of the larger Brick-Force project.

The class has several public properties and fields that define various textures, colors, and audio clips used for the visual effects. These include `screenFx`, `screenFxChild`, `ToScreenFxClr`, `FromScreenFxClr`, `ToScreenFxClrChild`, `FromScreenFxClrChild`, `ToBloodClr`, `FromBloodClr`, `bloodMarks`, `piercedWounds`, `heartbeatSound`, and `growling`.

The class also has private fields and variables used for internal calculations and tracking. These include `screenFxClr`, `bloodMarkQ`, `localController`, `deltaTime`, `zombieFxClr`, `rcScreen`, `zombieDelta`, and `zombieDeltaGrowling`.

The `Start` method initializes the various fields and variables, including setting the initial values for `screenFxClr` and `zombieFxClr`. It also calls the `ResetZombieGrawling` method.

The `ClearScreen` method resets the screen effects and clears the `bloodMarkQ` queue.

The `OnRespawn` method is called when the player respawns and clears the screen effects.

The `Growling` method plays the growling audio clip.

The `OnGUI` method is responsible for rendering the screen effects and blood marks on the GUI. It sets the GUI skin, depth, and color, and then draws the screen effects and blood marks using the `TextureUtil.DrawTexture` method.

The `ApplyScreenFx` method updates the screen effects based on the current state of the game. It adjusts the `screenFxClr` color and updates the blood marks.

The `ResetZombieGrawling` method resets the zombie growling effect.

The `ApplyScreenFxForZombie` method updates the screen effects specifically for the zombie player. It adjusts the `zombieFxClr` color and plays the growling audio clip.

The `OnHit` method is called when the player is hit by a weapon. It adds a blood mark to the `bloodMarkQ` queue and updates the screen effects.

The `ApplyHeartbeat` method applies the heartbeat effect when the player's health is low. It plays the heartbeat audio clip and updates the screen effects.

The `Update` method is called every frame and applies the heartbeat and screen effects.

Overall, the `WoundFx` class manages and displays visual effects related to wounds and damage in the game. It handles the rendering of screen effects, blood marks, and audio clips. It also adjusts the effects based on the player's health and game state.
## Questions: 
 1. What is the purpose of the `WoundFx` class?
- The `WoundFx` class is responsible for managing visual effects related to wounds and blood in the game.

2. What are the different textures and colors used in the code?
- The code uses various textures (`screenFx`, `screenFxChild`, `bloodMarks`, `piercedWounds`) and colors (`ToScreenFxClr`, `FromScreenFxClr`, `ToScreenFxClrChild`, `FromScreenFxClrChild`, `ToBloodClr`, `FromBloodClr`, `ToZombieFxClr`, `FromZombieFxClr`) for different visual effects.

3. How are the screen effects and blood marks applied and updated?
- The screen effects and blood marks are applied and updated in the `ApplyScreenFx()` and `ApplyScreenFxForZombie()` methods respectively. These methods use color interpolation and time-based calculations to achieve the desired visual effects.