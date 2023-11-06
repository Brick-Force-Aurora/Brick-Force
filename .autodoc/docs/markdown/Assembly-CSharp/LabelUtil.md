[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\LabelUtil.cs)

The `LabelUtil` class in the `Brick-Force` project provides utility methods for working with GUI labels in Unity. These methods allow for modifying the style, size, and position of labels, as well as calculating their length and size.

The `ToBold` method takes a style name as input and sets the font style of that style to bold. Similarly, the `ToNormal` method sets the font style of a style to normal.

The `PushSize` method takes a style name and a font size as input. It saves the current style and font size, and then sets the font size of the specified style to the given size. The `PopSize` method restores the previously saved font size.

The `CalcLength` method calculates the length of a given text string using a specified style. It returns a `Vector2` representing the width and height of the text.

The `CalcSize` method calculates the size of a given text string using a specified style and a maximum width. If the calculated width exceeds the maximum width, the method adjusts the height of the text to fit within the width. It returns a `Vector2` representing the adjusted width and height.

The `TextOut` methods are used to display text labels on the screen. They take various parameters such as position, text, style, text color, outline color, and alignment. The methods calculate the size of the text using the specified style, adjust the position based on the alignment, and then display the text label using the specified style, colors, and position. The `TextOut` method with a `width` parameter truncates the text if it exceeds the specified width, adding ellipsis at the end.

These utility methods can be used in the larger project to easily modify and display GUI labels with different styles, sizes, and positions. For example, they can be used to create dynamic UI elements such as buttons, tooltips, or information panels. The methods provide flexibility in customizing the appearance of labels and handling text that exceeds a certain width.
## Questions: 
 1. What is the purpose of the `LabelUtil` class?
- The `LabelUtil` class provides utility methods for working with GUI labels in Unity.

2. What does the `ToBold` method do?
- The `ToBold` method takes a style name as input and sets the font style of that style to bold.

3. What is the purpose of the `TextOut` method?
- The `TextOut` method is used to display text on the GUI. It takes various parameters such as position, text, style, and colors to customize the appearance of the text.