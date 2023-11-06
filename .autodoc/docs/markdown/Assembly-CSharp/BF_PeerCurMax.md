[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\BF_PeerCurMax.cs)

The code provided defines a struct called `BF_PeerCurMax`. This struct has two public properties: `cur` and `max`. 

The `cur` property is a getter and setter that operates on a bitvector called `bitvector1`. The getter returns the value of `bitvector1` after applying a bitwise AND operation with the hexadecimal value `0x3FF`. This operation effectively extracts the 10 least significant bits of `bitvector1`. The setter performs a bitwise OR operation between the input value and `bitvector1`, effectively setting the 10 least significant bits of `bitvector1` to the input value.

The `max` property is similar to the `cur` property, but with a different bitmask and scaling factor. The getter applies a bitwise AND operation with the hexadecimal value `0xFFC00` to `bitvector1`, effectively extracting the 10 bits between the 11th and 21st least significant bits. This value is then divided by 1024 to obtain the final result. The setter performs a multiplication of the input value by 1024 and then performs a bitwise OR operation with `bitvector1`, effectively setting the 10 bits between the 11th and 21st least significant bits of `bitvector1` to the scaled input value.

This struct seems to be used to store and manipulate two values, `cur` and `max`, which are represented as bitfields within a single 32-bit unsigned integer (`bitvector1`). The purpose of this struct is not clear from the provided code alone, but it could potentially be used to represent and manipulate current and maximum values in a game or simulation context. For example, it could be used to store and update the current and maximum health of a character in a game.
## Questions: 
 1. What is the purpose of the `bitvector1` field in the `BF_PeerCurMax` struct?
- The `bitvector1` field is used to store a bit vector that represents the current and maximum values.

2. How is the `cur` property calculated and what does it represent?
- The `cur` property is calculated by performing a bitwise AND operation between `bitvector1` and `0x3FF`, and it represents the current value.

3. How is the `max` property calculated and what does it represent?
- The `max` property is calculated by performing a bitwise AND operation between `bitvector1` and `0xFFC00`, dividing the result by 1024, and it represents the maximum value.