[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\ProjectileWrap.cs)

The code provided is a class called `ProjectileWrap` that is used to control the movement and behavior of a projectile in the game. 

The class has several public variables and methods that allow for the manipulation of the projectile's position, rotation, and range. 

The `ProjectileWrap` class has the following public variables:
- `targetPos`: a Vector3 that represents the target position of the projectile.
- `targetRot`: a Vector3 that represents the target rotation of the projectile.
- `projectile`: a reference to the GameObject that represents the projectile.
- `range`: a float that represents the range of the projectile.

The class also has a private variable `Elapsed` that keeps track of the elapsed time.

The constructor of the `ProjectileWrap` class takes a GameObject as a parameter and initializes the `targetPos` and `targetRot` variables with the position and rotation of the GameObject. It also sets the `projectile` variable to the passed GameObject and initializes the `range` variable to 0.

The `Fly` method is used to move the projectile towards its target position and rotation. It uses the `Lerp` function from the Unity engine to smoothly interpolate the position and rotation of the projectile towards the target position and rotation. The interpolation is done at a speed of 10 units per second multiplied by the time since the last frame.

The `Fly2` method is similar to the `Fly` method but also includes a check for the elapsed time. If the elapsed time is greater than 5 seconds, it sets the `overTime` variable to true. This method is likely used to handle projectiles that have a limited lifespan or behavior after a certain amount of time.

The `GetPos` method simply returns the current position of the projectile.

Overall, this code provides a way to control the movement and behavior of projectiles in the game. It allows for smooth interpolation of position and rotation, as well as handling of time-based behavior. This class can be used in the larger project to create and control various types of projectiles with different movement patterns and behaviors.
## Questions: 
 1. What is the purpose of the `Fly()` method?
- The `Fly()` method is responsible for moving the projectile towards its target position and rotating it towards its target rotation.

2. What is the purpose of the `Fly2()` method?
- The `Fly2()` method is similar to the `Fly()` method, but it also checks if a certain amount of time has elapsed and sets the `overTime` variable to true if it has.

3. What does the `GetPos()` method do?
- The `GetPos()` method returns the current position of the projectile.