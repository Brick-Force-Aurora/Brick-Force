[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\Infection.cs)

The code provided defines a class called `Infection`. This class represents an infection event in the larger Brick-Force project. 

The `Infection` class has two private integer variables: `host` and `newZombie`. These variables store the IDs of the host and the newly infected zombie, respectively. 

The class also has two public properties: `Host` and `NewZombie`. These properties provide read-only access to the private variables `host` and `newZombie`, respectively. This means that other parts of the code can retrieve the values of these variables but cannot modify them directly. 

The `Infection` class has a constructor that takes two integer parameters: `_host` and `_newZombie`. These parameters are used to initialize the `host` and `newZombie` variables. 

The purpose of this code is to represent an infection event in the Brick-Force project. It provides a way to store and retrieve information about the host and the newly infected zombie. 

Here's an example of how this code might be used in the larger project:

```csharp
// Create a new infection event
Infection infection = new Infection(123, 456);

// Retrieve the host and new zombie IDs
int hostID = infection.Host;
int newZombieID = infection.NewZombie;

// Print the IDs
Console.WriteLine("Host ID: " + hostID);
Console.WriteLine("New Zombie ID: " + newZombieID);
```

In this example, we create a new `Infection` object with the host ID of 123 and the new zombie ID of 456. We then retrieve these IDs using the `Host` and `NewZombie` properties and print them to the console. This allows us to track and display information about infection events in the Brick-Force project.
## Questions: 
 1. **What is the purpose of the `Infection` class?**
The `Infection` class represents an infection event in the Brick-Force project, where a host becomes a new zombie. 

2. **What are the meanings of the `host` and `newZombie` variables?**
The `host` variable represents the original host before infection, while the `newZombie` variable represents the new zombie that resulted from the infection.

3. **What is the significance of the `Host` and `NewZombie` properties?**
The `Host` and `NewZombie` properties provide read-only access to the `host` and `newZombie` variables respectively, allowing other parts of the code to retrieve their values.