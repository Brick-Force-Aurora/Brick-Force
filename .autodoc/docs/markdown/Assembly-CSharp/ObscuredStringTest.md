[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\ObscuredStringTest.cs)

The code provided is a part of the Brick-Force project and is a script called "ObscuredStringTest". This script is used to demonstrate the functionality of the "ObscuredString" class from the "CodeStage.AntiCheat.ObscuredTypes" namespace.

The purpose of this code is to showcase how the "ObscuredString" class can be used to store sensitive string data in a more secure manner. It provides two methods, "UseRegular()" and "UseObscured()", which demonstrate the difference between using a regular string and an obscured string.

In the "Start()" method, the script sets a new crypto key for the "ObscuredString" class using the "SetNewCryptoKey()" method. It then assigns a value to the "cleanString" variable and logs the original string to the console. The "cleanString" is then assigned to the "obscuredString" variable, which automatically obscures the string using the assigned crypto key. Finally, both the "cleanString" and "obscuredString" variables are set to empty strings.

The "UseRegular()" method sets the "useRegular" flag to true and assigns a value to the "cleanString" variable. It also sets the "obscuredString" to an empty string. This method demonstrates that the value of the "cleanString" can be easily changed in memory.

The "UseObscured()" method sets the "useRegular" flag to false and assigns a value to the "obscuredString" variable. It also sets the "cleanString" to an empty string. This method demonstrates that the value of the "obscuredString" cannot be easily changed in memory.

Overall, this code is used to showcase the functionality of the "ObscuredString" class and how it can be used to store sensitive string data securely. It demonstrates the difference between using a regular string and an obscured string, highlighting the benefits of using the "ObscuredString" class in terms of data security.
## Questions: 
 1. What is the purpose of the ObscuredString class and how does it work?
- The ObscuredString class is used to store strings in an obscured form in memory. It uses a crypto key to encrypt and decrypt the string.

2. How does the UseRegular() method differ from the UseObscured() method?
- The UseRegular() method sets the useRegular variable to true and allows the cleanString to be easily changed in memory. The UseObscured() method sets the useRegular variable to false and prevents the obscuredString from being changed in memory.

3. What is the significance of the SetNewCryptoKey() method and how is it used?
- The SetNewCryptoKey() method is used to set a new crypto key for the ObscuredString class. It is called in the Start() method to set the crypto key to "I LOVE MY GIRL".