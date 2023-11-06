[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\WeaponModifier.cs)

The `WeaponModifier` class is responsible for managing and modifying weapon properties in the Brick-Force project. It contains two dictionaries, `dic` and `dicEx`, which store instances of the `WpnMod` and `WpnModEx` classes respectively. 

The `WeaponModifier` class is a singleton, meaning that there can only be one instance of it in the project. This is enforced by the `Instance` property, which ensures that only one instance of `WeaponModifier` is created and accessed throughout the project. 

The `Get` method is used to retrieve a `WpnMod` object from the `dic` dictionary based on a given weapon ID. If the dictionary contains the weapon ID, the corresponding `WpnMod` object is returned. Otherwise, it returns null.

The `UpdateWpnMod` method is used to update the properties of a `WpnMod` object in the `dic` dictionary. It takes in various parameters that represent different weapon properties and updates the corresponding properties of the `WpnMod` object with the given weapon ID. If the dictionary does not contain the weapon ID, a new `WpnMod` object is created and added to the dictionary.

Similarly, the `GetEx` method is used to retrieve a `WpnModEx` object from the `dicEx` dictionary based on a given weapon ID. If the dictionary contains the weapon ID, the corresponding `WpnModEx` object is returned. Otherwise, it returns null.

The `UpdateWpnModEx` method is used to update the properties of a `WpnModEx` object in the `dicEx` dictionary. It takes in various parameters that represent different weapon properties and updates the corresponding properties of the `WpnModEx` object with the given weapon ID. If the dictionary does not contain the weapon ID, a new `WpnModEx` object is created and added to the dictionary.

The `Awake` method is called when the `WeaponModifier` object is created. It initializes the `dic` and `dicEx` dictionaries and ensures that the `WeaponModifier` object is not destroyed when a new scene is loaded.

The `Clear` method is used to clear the `dic` and `dicEx` dictionaries, removing all stored weapon properties.

Overall, the `WeaponModifier` class provides a centralized way to manage and modify weapon properties in the Brick-Force project. It allows for easy retrieval and updating of weapon properties based on weapon IDs.
## Questions: 
 1. What is the purpose of the `WeaponModifier` class?
- The `WeaponModifier` class is responsible for managing weapon modifications and providing methods to update and retrieve weapon modification data.

2. What is the purpose of the `UpdateWpnMod` method?
- The `UpdateWpnMod` method is used to update the weapon modification data for a specific weapon identified by its sequence number.

3. What is the purpose of the `Clear` method?
- The `Clear` method is used to clear the dictionary of weapon modifications, removing all stored data.