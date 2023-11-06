[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\ZombieVirus.cs)

The code provided is a class called `ZombieVirus` that represents a zombie virus in the larger project called Brick-Force. The purpose of this class is to manage the state and behavior of the zombie virus within the game.

The class has a private boolean variable called `active` which represents whether the zombie virus is currently active or not. It also has a public property called `Active` which allows other parts of the code to get and set the value of `active`.

The class has two additional public properties: `IsReallyActive` and `SpeedFactor`. 

The `IsReallyActive` property returns a boolean value based on the current state of the game. It checks if the current room type is a zombie room, if the zombie mode is enabled, and if the zombie virus is active. If all these conditions are true, then `IsReallyActive` returns `true`, otherwise it returns `false`.

The `SpeedFactor` property returns a float value that represents the speed factor of the zombie virus. If `IsReallyActive` is `false`, then the speed factor is 1.0f, otherwise it is 1.15f.

There is also a `MaxHpFactor` property that returns a float value representing the maximum health points factor of the zombie virus. If `IsReallyActive` is `false`, then the maximum health points factor is 0.0f, otherwise it is 3.0f.

The class also has a constructor that initializes the `active` variable to `false`.

In the larger project, this `ZombieVirus` class can be used to manage the behavior and attributes of the zombie virus. Other parts of the code can check the `Active` property to see if the zombie virus is active or not. They can also use the `IsReallyActive`, `SpeedFactor`, and `MaxHpFactor` properties to determine the behavior and attributes of the zombie virus in different situations.

For example, if the `IsReallyActive` property is `true`, it means that the zombie virus is active in a zombie room with the zombie mode enabled. In this case, the `SpeedFactor` property can be used to increase the speed of the zombie virus, and the `MaxHpFactor` property can be used to increase its maximum health points.

Overall, this `ZombieVirus` class provides a way to manage the state and behavior of the zombie virus in the Brick-Force project.
## Questions: 
 1. What is the purpose of the `active` variable and how is it used in the code?
- The `active` variable is a boolean that determines whether the zombie virus is active or not. It is used in the `IsReallyActive`, `SpeedFactor`, and `MaxHpFactor` properties to check if the zombie virus is active.

2. What conditions need to be met for the `IsReallyActive` property to return true?
- The `IsReallyActive` property will return true if the current room type is "ZOMBIE", the zombie mode is enabled, and the zombie virus is active.

3. How are the `SpeedFactor` and `MaxHpFactor` properties calculated?
- The `SpeedFactor` property is calculated as 1.15f if the zombie virus is active, and 1f otherwise. The `MaxHpFactor` property is calculated as 3f if the zombie virus is active, and 0f otherwise.