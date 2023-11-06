[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/.autodoc\docs\json\Assembly-CSharp\Procurios.Public)

The `JSON.cs` file in the `Procurios.Public` folder is a C# implementation of a JSON parser and serializer. It provides functionality to convert JSON strings to objects and vice versa, which is essential for data interchange between the server and the web application in the Brick-Force project.

The `JSON` class in this file contains constants representing different types of JSON tokens. These constants are used to identify and handle different parts of a JSON string.

The `JsonDecode` method is used to parse a JSON string and convert it into an object. It uses a recursive approach, starting with the `ParseValue` method, which determines the type of the JSON value and calls the appropriate parsing method.

The `JsonEncode` method is used to serialize an object into a JSON string. It uses a `StringBuilder` to build the JSON string, calling the `SerializeValue` method to handle each value in the object.

There are also several helper methods for parsing and serializing specific types of JSON values. For example, `ParseObject`, `ParseArray`, `ParseString`, and `ParseNumber` are used by the parsing methods, while `SerializeObject`, `SerializeArray`, `SerializeString`, and `SerializeNumber` are used by the `SerializeValue` method.

Here's an example of how this code can be used:

```csharp
string json = "{\"name\":\"John\",\"age\":30,\"city\":\"New York\"}";

// Parsing a JSON string into an object
object obj = JSON.JsonDecode(json);
Console.WriteLine(obj); // Output: { name = John, age = 30, city = New York }

// Serializing an object into a JSON string
Hashtable hashtable = new Hashtable();
hashtable["name"] = "John";
hashtable["age"] = 30;
hashtable["city"] = "New York";
string jsonString = JSON.JsonEncode(hashtable);
Console.WriteLine(jsonString); // Output: {"name":"John","age":30,"city":"New York"}
```

In the Brick-Force project, this code can be used to parse API responses, serialize objects for storage or transmission, or convert JSON data into a more usable format within the application.
