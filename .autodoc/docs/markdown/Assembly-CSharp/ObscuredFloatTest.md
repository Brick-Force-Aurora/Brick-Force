[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\ObscuredFloatTest.cs)

The code provided is a part of the Brick-Force project and it demonstrates the usage of the ObscuredFloat class from the CodeStage.AntiCheat.ObscuredTypes namespace. The purpose of this code is to showcase how to use the ObscuredFloat class to protect sensitive float values from being easily manipulated or cheated in a game.

The ObscuredFloatTest class inherits from MonoBehaviour and contains several variables and methods. Let's go through them one by one:

1. `healthBar` is a regular float variable that represents the health bar value. It is initially set to 11.4f.

2. `obscuredHealthBar` is an ObscuredFloat variable that also represents the health bar value. It is initially set to 11.4f. The ObscuredFloat class provides additional security measures to protect the value from being easily accessed or modified.

3. `useRegular` is a boolean variable that determines whether to use the regular float or the obscured float for manipulating the health bar value.

4. `cheatingDetected` is a boolean variable that is used to track if cheating has been detected.

The Start() method is called when the script is first initialized. It sets the crypto key for the ObscuredFloat class to 404, which is used for encryption and decryption of the obscured float values. It then sets the `healthBar` variable to 99.9f and assigns its value to `obscuredHealthBar`. The encrypted value of `obscuredHealthBar` is then logged to the console.

The OnCheatingDetected() method is a callback function that is called when cheating is detected. In this case, it simply sets the `cheatingDetected` variable to true.

The UseRegular() method is called when the regular float is being used. It sets the `useRegular` variable to true and modifies the `healthBar` value by adding a random range between -10f and 50f. It also sets the `obscuredHealthBar` to 11f and logs the modified `healthBar` value to the console.

The UseObscured() method is called when the obscured float is being used. It sets the `useRegular` variable to false and modifies the `obscuredHealthBar` value by adding a random range between -10f and 50f. It also sets the `healthBar` to 11f and logs the modified `obscuredHealthBar` value to the console.

Overall, this code demonstrates how to use the ObscuredFloat class to protect sensitive float values in a game. By using the ObscuredFloat class, the values are encrypted and stored in memory in an obscured form, making it difficult for cheaters to manipulate them. The code provides examples of how to use both the regular float and the obscured float for manipulating the health bar value, and it also showcases the detection of cheating through the OnCheatingDetected() callback function.
## Questions: 
 1. What is the purpose of the ObscuredFloat class and how does it work?
- The ObscuredFloat class is used to store sensitive float values in memory in an obscured manner. It encrypts the float value using a crypto key and provides methods for performing arithmetic operations on the obscured value.

2. How does the code detect cheating and what happens when cheating is detected?
- The code detects cheating by setting the `onCheatingDetected` delegate to the `OnCheatingDetected` method. When cheating is detected, the `cheatingDetected` variable is set to true.

3. What is the difference between using the regular float (`healthBar`) and the obscured float (`obscuredHealthBar`) in this code?
- The regular float (`healthBar`) is used for normal operations and can be directly modified. The obscured float (`obscuredHealthBar`) is used to store the obscured version of the health bar value and is modified using the ObscuredFloat class methods.