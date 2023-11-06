[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\Scope.cs)

The `Scope` class is a script that is used to handle the scope functionality in the Brick-Force project. It is attached to a game object in the scene and is responsible for managing the camera and crosshair when the player is aiming down the scope of a weapon.

The class has several public properties that can be accessed and modified by other scripts. These properties include `guiDepth`, `crossHair`, `blackOut`, `accuracy`, `fov`, `camSpeed`, `midstep`, `midfovs`, and `zoomKeep`. These properties control various aspects of the scope, such as the depth of the GUI, the crosshair texture, the blackout texture, the accuracy of the weapon, the field of view, the camera speed, and the zoom behavior.

The class also has several private variables that are used internally. These variables include `aiming`, `scoping`, `cam`, `fpCam`, `camCtrl`, and `cooldown`. These variables keep track of the current state of the scope, the camera objects in the scene, the camera controller, and the cooldown state.

The class has several methods that are used to handle different aspects of the scope functionality. These methods include `VerifyCamera()`, `DrawCrossHair()`, `Start()`, `Modify()`, `HandleFireEvent()`, `CalcDeflection()`, `Inaccurate()`, `Accurate()`, `SetAiming()`, `OnDisable()`, `ToggleScoping()`, `IsZooming()`, `ZoomIn()`, `ZoomOut()`, `SetupCamera()`, `OnGUI()`, and `Update()`.

The `VerifyCamera()` method is used to find and assign the camera objects in the scene. It returns true if the cameras are found and assigned successfully.

The `DrawCrossHair()` method is used to draw the crosshair texture on the screen when the player is aiming down the scope. It takes into account the position and size of the crosshair texture and the blackout texture.

The `Start()` method is called when the script is first initialized. It calls the `Modify()` method to modify the accuracy, field of view, and camera speed based on the weapon being used. It also initializes the cooldown, aiming, scoping, and accuracy.

The `Modify()` method is used to modify the accuracy, field of view, and camera speed based on the weapon being used. It retrieves the weapon function component and the weapon modifier component and updates the accuracy, field of view, and camera speed accordingly.

The `HandleFireEvent()` method is called when the player fires a weapon. It calls the `Inaccurate()` method to make the aim inaccurate and checks if the zoom should be kept or not. If the zoom should not be kept, it sets the cooldown to true and checks if the player is currently zooming. If the player is zooming, it sets the scoping to false, zooms out, and sets the aiming to true.

The `CalcDeflection()` method is used to calculate the deflection of the aim based on the current accuracy.

The `Inaccurate()` method is used to make the aim inaccurate. It takes a boolean parameter `aimAccurateMore` which determines if the aim should be made more accurate or not.

The `Accurate()` method is used to make the aim accurate. It takes a boolean parameter `aimAccurate` which determines if the aim should be made accurate or not.

The `SetAiming()` method is used to set the aiming state. It takes a boolean parameter `_aiming` which determines if the player is aiming or not.

The `OnDisable()` method is called when the script is disabled. If the player is currently zooming, it sets the scoping to false and zooms out.

The `ToggleScoping()` method is used to toggle the scoping state. It takes an optional boolean parameter `forceApply` which determines if the scoping state should be forced to apply. It checks if the midstep is greater than 0 and if the player is currently scoping or not. If the player is not scoping, it sets the current step to the midstep and returns true. If the player is scoping, it decreases the current step and checks if it is less than 0. If it is less than 0, it returns true. If the midstep is 0 or the flag is true or the forceApply parameter is true, it toggles the scoping state and calls the `SetupCamera()` method.

The `IsZooming()` method is used to check if the player is currently zooming. It returns true if the player is aiming and scoping.

The `ZoomIn()` method is used to zoom in the camera when the player is aiming and scoping. It sets the scope field of view of the camera based on the current step or the default field of view if the current step is 0. It disables the first-person camera and sets the camera speed factor.

The `ZoomOut()` method is used to zoom out the camera when the player is not aiming and scoping. It enables the first-person camera and resets the camera speed factor and scope field of view.

The `SetupCamera()` method is used to set up the camera based on the current scoping state. It verifies the camera objects, checks if the player is currently zooming, and calls the `ZoomIn()` or `ZoomOut()` method accordingly.

The `OnGUI()` method is used to draw the crosshair texture on the screen. It checks if the GUI is enabled and if the game is not in a modal state. It then calls the `DrawCrossHair()` method.

The `Update()` method is called every frame. It verifies the camera objects and checks if the cooldown is active. If the cooldown is active, it checks if the gun component is cooling down.
## Questions: 
 1. **Question:** What is the purpose of the `VerifyCamera()` method?
   - **Answer:** The `VerifyCamera()` method is used to find and assign the main camera and first-person camera objects in the scene, as well as the camera controller component. It ensures that these objects are not null before proceeding.

2. **Question:** What does the `HandleFireEvent()` method do?
   - **Answer:** The `HandleFireEvent()` method is responsible for handling the firing event. It calls the `Inaccurate()` method to make the aim inaccurate, and if `zoomKeep` is false, it sets `cooldown` to true and checks if the player is zooming. If the player is zooming, it sets `scoping` to false, zooms out, and sets the aiming state to true.

3. **Question:** What is the purpose of the `ToggleScoping()` method?
   - **Answer:** The `ToggleScoping()` method is used to toggle the scoping state. If `midstep` is greater than 0, it increments or decrements `curstep` based on the current scoping state. If `midstep` is 0 or the scoping state needs to be forced, it toggles the `scoping` state and calls the `SetupCamera()` method to update the camera settings accordingly. Finally, it returns the new scoping state.