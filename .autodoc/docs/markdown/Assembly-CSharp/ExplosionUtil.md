[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\ExplosionUtil.cs)

The code provided is a utility class called `ExplosionUtil` that contains several static methods for handling explosions in the game. This class is likely used in the larger Brick-Force project to calculate and apply damage to various game objects and entities when an explosion occurs.

The `CalcPowFrom` method calculates the power of an explosion at a given position based on the distance from the bomb's position, the damage of the bomb, and the explosion radius. It returns the calculated power as a float value.

The `CheckMyself` method checks if the player's character is within the explosion radius. If the player is within range, it calculates the damage based on the player's position and the explosion parameters. If the calculated damage is greater than 0, it calls the `GetHit` method on the player's `LocalController` component to apply the damage.

The `CheckBoxmen` method checks for nearby enemy players within the explosion radius. It calculates the damage for each enemy player based on their position and the explosion parameters. If the calculated damage is greater than 0, it updates the `accumDamaged` property of the enemy player's `PlayerProperty` component and sends a network message to inform other players about the damage.

The `CheckMonster` method checks for nearby monsters within the explosion radius. It calculates the damage for each monster based on their position and the explosion parameters. If the calculated damage is greater than 0, it calls the `Hit` method on the `MonManager` instance to apply the damage to the monster.

The `CheckDestructibles` method checks for nearby destructible bricks within the explosion radius. It calculates the damage for each brick based on its position and the explosion parameters. If the calculated damage reduces the brick's hit points to 0 or below, it sends a network message to destroy the brick. Otherwise, it sends a network message to update the brick's hit points.

The `CheckBoxmen`, `CheckMon`, and `CheckDestructibles` methods are helper methods used by the above methods to find the relevant game objects within the explosion radius.

Overall, this code provides a set of utility methods for handling explosions in the game, calculating damage, and applying it to various game objects and entities. It is likely used extensively throughout the Brick-Force project to handle explosions and their effects on the game world.
## Questions: 
 **Question 1:** What does the `CalcPowFrom` method do and how is it used?
- The `CalcPowFrom` method calculates the power of an explosion based on the distance between the bomb position and a given position. It is used to determine the damage caused by the explosion.

**Question 2:** What is the purpose of the `CheckMyself` method and how does it work?
- The `CheckMyself` method checks if the player's position is within the explosion radius and calculates the damage inflicted on the player. If the damage is greater than 0, it calls the `GetHit` method of the `LocalController` component to apply the damage to the player.

**Question 3:** What is the difference between the `CheckDestructibles` and `CheckMon` methods?
- The `CheckDestructibles` method checks for destructible bricks within the explosion radius and applies damage to them. The `CheckMon` method checks for monsters within the explosion radius and applies damage to them.