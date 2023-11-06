[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\MemoManager.cs)

The `MemoManager` class is a singleton class that manages a collection of `Memo` objects. It provides methods to add, remove, and retrieve `Memo` objects from the collection. The purpose of this class is to provide a centralized location for managing and accessing `Memo` objects throughout the project.

The class has a private dictionary variable `dic` that stores `Memo` objects with a unique identifier (`long`) as the key. The `ToArray` method returns an array of all the `Memo` objects in the dictionary. The method iterates over the dictionary and adds each `Memo` object to a list. It then reverses the order of the list and converts it to an array before returning it.

The `GetOption` method takes a `long` parameter `seq` and returns the `option` value of the `Memo` object with the corresponding key in the dictionary. If the key is not found in the dictionary, it returns -1.

The `ClearPresent` method takes a `long` parameter `seq` and clears the `attached`, `option`, and `check` properties of the `Memo` object with the corresponding key in the dictionary.

The `Clear` method clears the dictionary by calling the `Clear` method on the dictionary object.

The `Add` method takes a `long` parameter `seq` and a `Memo` object `memo` and adds the `memo` object to the dictionary with the `seq` as the key. If the key already exists in the dictionary, it does not add the `memo` object.

The `Del` method takes a `long` parameter `seq` and removes the `Memo` object with the corresponding key from the dictionary.

The `HaveUnreadMemo` method checks if there are any `Memo` objects in the dictionary that have not been read. It iterates over the dictionary and returns `true` if it finds any unread `Memo` objects, otherwise it returns `false`.

The `GetUnreadMemoCount` method returns the count of unread `Memo` objects in the dictionary. It iterates over the dictionary and increments a counter for each unread `Memo` object.

The `GetMemoCountPercent` method returns the count of `Memo` objects in the dictionary as a percentage. If the dictionary is null, it returns 0.

Overall, the `MemoManager` class provides a centralized way to manage and access `Memo` objects in the larger project. It allows for adding, removing, and retrieving `Memo` objects, as well as checking for unread `Memo` objects and getting the count of `Memo` objects in the dictionary.
## Questions: 
 1. What is the purpose of the `MemoManager` class?
- The `MemoManager` class is responsible for managing a collection of `Memo` objects.

2. What is the significance of the `Instance` property?
- The `Instance` property provides a way to access the singleton instance of the `MemoManager` class.

3. What does the `ToArray` method do?
- The `ToArray` method converts the collection of `Memo` objects into an array and returns it.