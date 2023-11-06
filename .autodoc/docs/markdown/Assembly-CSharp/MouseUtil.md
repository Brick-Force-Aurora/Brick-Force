[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\MouseUtil.cs)

The code provided is a utility class called `MouseUtil` that contains several static methods for handling mouse input in the context of a graphical user interface (GUI). 

The `ScreenToPixelPoint` method takes a `Vector2` representing a point on the screen and converts it to a pixel point. It does this by creating a new `Vector2` object with the same x-coordinate as the input point, but with the y-coordinate adjusted to be the difference between the screen height and the y-coordinate of the input point. This method is useful for converting mouse positions from screen coordinates to pixel coordinates.

The `MouseOver` method takes a `Rect` object representing a rectangular area on the screen and checks if the mouse position is within that area. It does this by first converting the mouse position to a pixel point using the `ScreenToPixelPoint` method. It then checks if the x-coordinate of the pixel point is outside the range of the rectangle's x-coordinates or if the y-coordinate of the pixel point is outside the range of the rectangle's y-coordinates. If either of these conditions is true, it returns `false`, indicating that the mouse is not over the rectangle. Otherwise, it returns `true`.

The `ClickInside` method is similar to `MouseOver`, but it also checks if the left mouse button, right mouse button, or middle mouse button is being pressed. It does this by checking if `Input.GetMouseButtonDown` returns `true` for any of these buttons. If any of the buttons are being pressed, it converts the mouse position to a GUI point using the `PixelToGUIPoint` method and checks if it is inside the rectangle. If the mouse is over the rectangle and one of the buttons is being pressed, it returns `true`. Otherwise, it returns `false`.

The `ClickOutside` method is the opposite of `ClickInside`. It checks if any of the mouse buttons are being pressed and if the mouse position is outside the rectangle. If both conditions are true, it returns `true`, indicating that the mouse click is happening outside the rectangle. Otherwise, it returns `false`.

These methods can be used in the larger project to handle mouse input for GUI elements. For example, `MouseOver` can be used to determine if the mouse is hovering over a button or other interactive element, while `ClickInside` and `ClickOutside` can be used to handle mouse clicks inside or outside a specific area. By providing these utility methods, the `MouseUtil` class simplifies the process of handling mouse input in the project.
## Questions: 
 1. **What does the `ScreenToPixelPoint` method do?**
The `ScreenToPixelPoint` method takes a screen point as input and returns a corresponding pixel point on the screen.

2. **What is the purpose of the `MouseOver` method?**
The `MouseOver` method checks if the mouse position is within the bounds of a given rectangle (`rc`) and returns `true` if it is, otherwise `false`.

3. **What is the difference between the `ClickInside` and `ClickOutside` methods?**
The `ClickInside` method checks if the left, right, or middle mouse buttons are pressed and if the mouse position is within the bounds of a given rectangle (`rc`), returning `true` if both conditions are met. On the other hand, the `ClickOutside` method checks if any mouse button is pressed and if the mouse position is outside the bounds of the given rectangle, returning `true` if both conditions are met.