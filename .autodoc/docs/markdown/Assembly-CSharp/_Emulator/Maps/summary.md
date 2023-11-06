[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/.autodoc\docs\json\Assembly-CSharp\_Emulator\Maps)

The `MapGenerator.cs` file is a crucial component of the Brick-Force project, responsible for generating game maps. It contains the `MapGenerator` class, which is a singleton, ensuring only one instance of it exists. This class maintains a dictionary of landscape templates, with each template represented by a `Landscape` object. 

The `Landscape` class encapsulates properties such as bricks, ratios, size, and height. The bricks are an array of bytes representing different types of bricks, and ratios are an array of floats representing the distribution of each brick type. The `Landscape` constructor initializes these properties and calculates the distribution array, which determines the probability of selecting each brick type when generating the map.

The `MapGenerator` class has a method `GetHashIdForTime(DateTime time)`, which generates a unique hash ID based on the current time. This ID is used to ensure the uniqueness of each generated map. 

The class also contains a private method `GetNextTemplateByDistribution(Landscape landscape)`, which selects the next brick type based on the distribution array of the landscape. 

The `GenerateInternal` method generates a map based on a given landscape and skybox index. It creates a `UserMap` object, sets its properties, and iterates over each position in the map to add bricks using the `AddBrickInst` method. 

The `Generate` method is the main entry point for generating maps. It takes a landscape index and skybox index as input and returns a generated map based on the specified landscape and skybox. 

Here's an example of how the `Generate` method might be used:

```csharp
MapGenerator mapGenerator = MapGenerator.Instance;
UserMap generatedMap = mapGenerator.Generate(landscapeIndex, skyboxIndex);
```

In the larger project, this class interacts with other parts of the game that require map generation, such as game initialization or level changes. The generated maps are unique and based on predefined landscape templates and skybox indices, providing a diverse gaming experience.
