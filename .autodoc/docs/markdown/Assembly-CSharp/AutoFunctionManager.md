[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\AutoFunctionManager.cs)

The `AutoFunctionManager` class is responsible for managing a collection of `AutoFunction` objects. It provides methods to add, update, and delete these objects.

The `autoFunctionMap` variable is a dictionary that stores `AutoFunction` objects. The key is an integer hash code generated from the `AutoFunction` object, and the value is the `AutoFunction` object itself. This allows for efficient lookup and retrieval of `AutoFunction` objects.

The `AddAutoFunction` method adds an `AutoFunction` object to the `autoFunctionMap` dictionary. It takes an `AutoFunction` object as a parameter, generates a hash code for it, and adds it to the dictionary using the hash code as the key. It also performs a check to ensure that the number of `AutoFunction` objects in the dictionary does not exceed 100.

The `AddRepeatFunction` and `AddEndFunction` methods are convenience methods that create new `AutoFunction` objects and add them to the `autoFunctionMap` dictionary. They take different parameters to specify the behavior of the `AutoFunction` objects.

The `Update` method is called every frame and iterates through all the `AutoFunction` objects in the `autoFunctionMap` dictionary. It calls the `Update` method of each `AutoFunction` object, which updates its internal state and returns a boolean value indicating whether the `AutoFunction` has completed. If an `AutoFunction` has completed, its key is added to a list.

After iterating through all the `AutoFunction` objects, the `Update` method checks if there are any completed `AutoFunction` objects in the list. If there are, it calls the `DeleteAutoFunction` method to remove them from the `autoFunctionMap` dictionary.

The `DeleteAutoFunction` method removes an `AutoFunction` object from the `autoFunctionMap` dictionary. It takes an integer key as a parameter and checks if the key exists in the dictionary. If it does, it calls the `EndFunctionCall` method of the `AutoFunction` object to perform any necessary cleanup, and then removes the key-value pair from the dictionary.

The `DeleteOnly` method is a convenience method that removes an `AutoFunction` object from the `autoFunctionMap` dictionary without calling the `EndFunctionCall` method.

The `DeleteAllAutoFunction` method clears the `autoFunctionMap` dictionary, effectively removing all `AutoFunction` objects from it.

Overall, the `AutoFunctionManager` class provides a centralized way to manage and control the execution of `AutoFunction` objects in the larger project. It allows for adding, updating, and deleting `AutoFunction` objects, and ensures that the number of `AutoFunction` objects does not exceed a certain limit.
## Questions: 
 1. What is the purpose of the `AutoFunctionManager` class?
- The `AutoFunctionManager` class is responsible for managing a dictionary of `AutoFunction` objects.

2. What is the purpose of the `AddAutoFunction` method?
- The `AddAutoFunction` method adds an `AutoFunction` object to the `autoFunctionMap` dictionary and returns its hash code.

3. What is the purpose of the `Update` method?
- The `Update` method iterates through the `autoFunctionMap` dictionary and calls the `Update` method of each `AutoFunction` object. If an `AutoFunction` object returns true from its `Update` method, it is added to a list and then removed from the dictionary.