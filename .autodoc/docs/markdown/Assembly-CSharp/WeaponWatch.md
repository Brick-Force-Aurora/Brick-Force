[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\WeaponWatch.cs)

The `WeaponWatch` class is responsible for tracking the amount of time that each weapon in the game is held by the player. This information is then used to calculate the ratio of time each weapon is held compared to the total time all weapons are held. 

The class contains a private `Dictionary<long, float>` variable called `weaponHeldTime`, which stores the total time each weapon has been held. The `deltaTime` variable keeps track of the time since the last update, and `deltaMax` determines the maximum time interval between updates.

The `Start()` method initializes the `deltaMax` and `deltaTime` variables, and creates a new instance of the `weaponHeldTime` dictionary.

The `OnSwapWeapon()` method is called when the player swaps weapons. It resets the `deltaMax` variable and calls the `UpdateWeaponHeldTime()` method.

The `UpdateWeaponHeldTime()` method retrieves all the `Weapon` components in the game, including inactive ones. It then iterates through each weapon and checks if it is a valid weapon type (melee, projectile, etc.). If it is, it retrieves the corresponding `Item` object using the `ItemSeq` property of the `WeaponFunction` component. If the `Item` is not null, it updates the `weaponHeldTime` dictionary with the time the weapon has been held.

After updating the `weaponHeldTime` dictionary, the method calculates the total time all weapons have been held by summing the values in the dictionary. It then creates a new dictionary called `dictionary3` and calculates the ratio of time each weapon has been held compared to the total time. Finally, it sends this information to the `CSNetManager` class using the `SendCS_WEAPON_HELD_RATIO_REQ()` method.

The `Update()` method is called every frame and updates the `deltaTime` variable. If the `deltaTime` exceeds the `deltaMax` value, it resets the `deltaTime` and increases the `deltaMax` value. It then calls the `UpdateWeaponHeldTime()` method.

Overall, this code tracks the amount of time each weapon is held by the player and calculates the ratio of time each weapon is held compared to the total time all weapons are held. This information can be used for various purposes in the larger project, such as balancing weapon usage, providing statistics to the player, or adjusting gameplay mechanics based on weapon popularity.
## Questions: 
 1. What is the purpose of the `WeaponWatch` class?
- The `WeaponWatch` class is responsible for tracking the amount of time that each weapon is held by the player.

2. What triggers the `OnSwapWeapon` method?
- The `OnSwapWeapon` method is triggered when the player swaps their current weapon for a different one.

3. What does the `Update` method do?
- The `Update` method is called every frame and updates the `deltaTime` and `deltaMax` variables. It also calls the `UpdateWeaponHeldTime` method if a certain amount of time has passed.