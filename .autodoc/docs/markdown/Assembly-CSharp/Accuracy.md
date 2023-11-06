[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\Accuracy.cs)

The code provided is a class called "Accuracy" that is used to handle accuracy calculations for a game. It contains various properties and methods to control and manipulate accuracy values.

The class has several public properties that represent different aspects of accuracy, such as accurateMin, accurateMax, inaccurateMin, inaccurateMax, accurateSpread, accurateCenter, inaccurateSpread, inaccurateCenter, and moveInaccuracyFactor. These properties are used to define the range and spread of accuracy values.

The class also has private fields for inaccurate and accurate, which are used to store the current inaccurate and accurate values.

The class has a public method called "Init()" that initializes the accurate and inaccurate values to their minimum values.

There are two public methods called "MakeInaccurate()" and "MakeAccurate()" that are used to adjust the accurate and inaccurate values based on certain conditions. "MakeInaccurate()" takes a boolean parameter called "aimAccurateMore" and increases the accurate and inaccurate values by their respective spreads. If "aimAccurateMore" is true, it adjusts the maximum values of accurate and inaccurate based on their minimum values. "MakeAccurate()" takes a boolean parameter called "aimAccurate" and decreases the accurate and inaccurate values based on their respective centers. If "aimAccurate" is false, it multiplies the minimum values of accurate and inaccurate by the moveInaccuracyFactor.

The class also has a public method called "CalcDeflection()" that calculates a deflection vector based on the accuracy values. It generates random values for f and f2, and a random value for num between 0 and 100. If num is less than the accuracy value, it generates random values for num2 and num3 between 0 and accurate/2. Otherwise, it generates random values for num2 and num3 between accurate/2 and inaccurate/2. It then multiplies num2 and num3 by the sign of f and f2 respectively. Finally, it calculates a factor based on the screen width, height, and camera field of view, and returns a Vector2 representing the deflection.

Overall, this class is used to handle accuracy calculations for a game, allowing for adjustments to accuracy values and generating deflection vectors based on those values. It can be used in the larger project to determine the accuracy of player actions and calculate the resulting effects.
## Questions: 
 1. What is the purpose of the `Accuracy` class?
- The `Accuracy` class represents a set of properties and methods related to accuracy calculations in a game. 

2. What does the `Init()` method do?
- The `Init()` method initializes the `accurate` and `inaccurate` variables to their respective minimum values.

3. What does the `CalcDeflection()` method return?
- The `CalcDeflection()` method returns a `Vector2` representing the calculated deflection based on random values and the accuracy settings.