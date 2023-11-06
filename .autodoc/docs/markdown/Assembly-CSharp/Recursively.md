[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\Recursively.cs)

The code provided is a utility class called "Recursively" that contains three static methods: "SetLayer", "ChangeLayer", and "GetAllComponents". These methods are designed to perform recursive operations on a given parent object and its children.

The "SetLayer" method takes a Transform object and an integer representing a layer, and sets the layer of the parent object and all of its children to the specified layer. It uses a foreach loop to iterate through each child of the parent object and recursively calls the "SetLayer" method on each child. This method can be used to easily change the layer of a group of objects in a scene.

```csharp
Transform parentObject = // get reference to parent object
int layer = // specify the layer to set
Recursively.SetLayer(parentObject, layer);
```

The "ChangeLayer" method is similar to the "SetLayer" method, but it takes two additional parameters: "fromLayer" and "toLayer". It checks if the layer of the parent object matches the "fromLayer" parameter, and if so, it changes the layer to the "toLayer" parameter. Like the "SetLayer" method, it also recursively calls itself on each child of the parent object. This method can be used to selectively change the layer of specific objects in a scene.

```csharp
Transform parentObject = // get reference to parent object
int fromLayer = // specify the layer to change from
int toLayer = // specify the layer to change to
Recursively.ChangeLayer(parentObject, fromLayer, toLayer);
```

The "GetAllComponents" method takes a Transform object and a boolean parameter "includeInactive". It uses recursion to traverse up the parent hierarchy until it reaches the root object. Then, it calls the "GetComponentsInChildren" method on the root object, passing in the "includeInactive" parameter. This method returns an array of components of type T, where T is a generic type that extends the Component class. This method can be used to easily retrieve all components of a specific type from a parent object and its children.

```csharp
Transform parentObject = // get reference to parent object
bool includeInactive = // specify whether to include inactive objects
Component[] components = Recursively.GetAllComponents<Component>(parentObject, includeInactive);
```

Overall, this utility class provides convenient methods for performing recursive operations on a parent object and its children, such as setting the layer of objects, changing the layer of specific objects, and retrieving components of a specific type. These methods can be used in various scenarios within the larger Brick-Force project, such as managing object layers, modifying object properties, and accessing components for gameplay or rendering purposes.
## Questions: 
 1. **What does the `SetLayer` method do?**
The `SetLayer` method sets the layer of a given `Transform` and all of its child `Transforms` to a specified layer.

2. **What does the `ChangeLayer` method do?**
The `ChangeLayer` method changes the layer of a given `Transform` and all of its child `Transforms` from one layer to another.

3. **What does the `GetAllComponents` method do?**
The `GetAllComponents` method returns an array of all components of type `T` attached to a given `Transform` and its child `Transforms`, including inactive components if specified.