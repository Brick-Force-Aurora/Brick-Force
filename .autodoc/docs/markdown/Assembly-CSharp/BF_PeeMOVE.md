[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\BF_PeeMOVE.cs)

The code provided defines a struct called `BF_PeeMOVE` which represents a data structure for storing information related to a movement action in the Brick-Force project. 

The struct has three properties: `cc`, `isDead`, and `isRegularSend`. 

The `cc` property is a byte that represents the movement command. It uses a bitwise AND operation with the value `0x3F` to extract the lower 6 bits of the `bitvector1` field. The `get` accessor returns the extracted value, while the `set` accessor updates the `bitvector1` field by performing a bitwise OR operation with the provided value.

The `isDead` property is a boolean that indicates whether the movement action results in the character's death. It uses a bitwise AND operation with the value `0x40` to check the 7th bit of the `bitvector1` field. The `get` accessor divides the result by 64 and compares it to 1 to determine if the character is dead. The `set` accessor updates the `bitvector1` field by performing a bitwise OR operation with the appropriate value based on the provided boolean value.

The `isRegularSend` property is a boolean that indicates whether the movement action is a regular send. It uses a bitwise AND operation with the value `0x80` to check the 8th bit of the `bitvector1` field. The `get` accessor divides the result by 128 and compares it to 1 to determine if it is a regular send. The `set` accessor updates the `bitvector1` field by performing a bitwise OR operation with the appropriate value based on the provided boolean value.

This struct is likely used in the larger Brick-Force project to store and manipulate movement-related data for characters or objects. It provides a compact way to store multiple properties in a single byte field, using bitwise operations to extract and update specific bits. This can be useful for optimizing memory usage and improving performance in scenarios where a large number of movement actions need to be processed. 

Example usage:

```csharp
BF_PeeMOVE move = new BF_PeeMOVE();
move.cc = 10; // Set the movement command to 10
move.isDead = true; // Set the character as dead
move.isRegularSend = false; // Set the movement as not a regular send

Console.WriteLine(move.cc); // Output: 10
Console.WriteLine(move.isDead); // Output: True
Console.WriteLine(move.isRegularSend); // Output: False
```
## Questions: 
 1. What is the purpose of the `bitvector1` field in the `BF_PeeMOVE` struct?
- The `bitvector1` field is used to store multiple boolean flags in a single byte.

2. How does the `cc` property work?
- The `cc` property returns the lower 6 bits of `bitvector1`, effectively extracting a specific subset of flags.

3. What is the purpose of the `isDead` property?
- The `isDead` property is used to get or set the flag that indicates whether the object is dead.