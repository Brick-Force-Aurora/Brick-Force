[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\RegMapManager.cs)

The `RegMapManager` class is responsible for managing a collection of `RegMap` objects. It provides methods for adding, removing, and retrieving `RegMap` objects, as well as performing various operations on them.

The `RegMapManager` class has several private member variables, including three dictionaries: `dicRegMap`, `dicDownloaded`, and `dicDeleted`. These dictionaries store `RegMap` objects, with the keys being integers representing the map IDs. There is also a static instance of the `RegMapManager` class, `_instance`, which is used to access the singleton instance of the class.

The `RegMapManager` class provides several public methods for working with `RegMap` objects. The `ToArray` method returns an array of `RegMap` objects that are playable in a given room type, or with a given channel mode. The `IsDownloaded` and `IsDeleted` methods check if a map is present in the `dicDownloaded` or `dicDeleted` dictionaries, respectively. The `Clear` method clears all the dictionaries, and the `DownloadedClear` method clears only the `dicDownloaded` and `dicDeleted` dictionaries.

The `RegMapManager` class also provides methods for setting and getting various properties of `RegMap` objects. The `SetDownloadFirst` method sets a specific map as the first downloaded map, and the `SetDownload` method sets a map as downloaded or not downloaded. The `GetAlways` method retrieves a `RegMap` object with the specified map ID and sets its properties. The `SetThumbnail` method sets the thumbnail image for a specific map.

The `RegMapManager` class also includes some utility methods, such as `RequestRegMap`, which sends a request for map information to the server, and `Start`, which loads `RegMap` objects from cache files.

Overall, the `RegMapManager` class provides a centralized way to manage and manipulate `RegMap` objects in the larger Brick-Force project. It handles adding, removing, and retrieving `RegMap` objects, as well as performing operations on them, such as setting properties and requesting map information from the server.
## Questions: 
 1. What is the purpose of the `RegMapManager` class?
- The `RegMapManager` class is responsible for managing and manipulating dictionaries of `RegMap` objects.

2. What is the significance of the `ToArray` methods?
- The `ToArray` methods convert the `dicDownloaded` dictionary into an array of `RegMap` objects based on different criteria such as `roomType`, `page`, and `channelMode`.

3. What is the purpose of the `SetDownloadFirst` method?
- The `SetDownloadFirst` method sets a specific `RegMap` as the first element in the `dicDownloaded` dictionary, while moving the other elements down by one position.