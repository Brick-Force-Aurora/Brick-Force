[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\BndWall.cs)

The `BndWall` class is a script that controls the hiding and showing of a wall object in the Brick-Force project. The purpose of this script is to provide functionality for hiding and showing the wall with a smooth animation.

The script contains several private variables, including `deltaTime`, `hideTime`, `showTime`, `hiding`, `showing`, `scaleShow`, and `scaleHide`. These variables are used to keep track of the time, animation durations, and the current state of the wall.

The `Start` method is called when the script is initialized. It sets the initial values for the `hiding` and `showing` variables, and calculates the `scaleHide` value based on the initial scale of the wall. It also adjusts the positions of the `probeTop` and `probeBottom` game objects.

The `Hiding` and `Showing` methods are responsible for animating the hiding and showing of the wall. They are called in the `Update` method. Inside these methods, the script checks if the wall is currently hiding or showing, and then updates the scale of the wall gradually over time using `Vector3.Lerp`. When the animation is complete, the script disables or enables the `MeshCollider` components of the wall and adjusts the positions of the `probeTop` and `probeBottom` game objects.

The `Hide` and `Show` methods are public methods that can be called from other scripts to initiate the hiding or showing of the wall. They take a boolean parameter `rightNow`, which determines whether the animation should start immediately or not. If `rightNow` is true, the `deltaTime` variable is set to the corresponding animation duration, otherwise it is set to 0. The `hiding` or `showing` variable is then set to true, which triggers the animation in the `Hiding` or `Showing` methods.

Overall, this script provides a way to hide and show a wall object in the Brick-Force project with smooth animations. It can be used to create dynamic and interactive environments where walls can be hidden or shown based on certain conditions or player actions.
## Questions: 
 1. What is the purpose of the `Hiding()` and `Showing()` methods?
- The `Hiding()` and `Showing()` methods are responsible for animating the hiding and showing of the object respectively. They update the scale of the object and enable/disable the MeshColliders accordingly.

2. What is the significance of the `probeTop` and `probeBottom` arrays?
- The `probeTop` and `probeBottom` arrays contain references to game objects that are used to position the top and bottom probes of the object. These probes are repositioned based on the scale of the object.

3. What is the purpose of the `Hide()` and `Show()` methods?
- The `Hide()` and `Show()` methods are used to initiate the hiding and showing of the object. They set the `deltaTime` variable to the appropriate value and set the `hiding` or `showing` flag to true, triggering the corresponding animation in the `Update()` method.