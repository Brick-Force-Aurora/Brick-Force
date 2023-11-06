[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\WordFilter.cs)

The `WordFilter` class is responsible for filtering and censoring inappropriate words in a text input. It is part of the larger Brick-Force project and is used to ensure that user-generated content within the game adheres to community guidelines and remains family-friendly.

The class contains several private variables, including `badWords`, `blind`, and `ignore`. `badWords` is a 2-dimensional array of strings that stores the list of inappropriate words categorized by language. `blind` is an array of characters that will be used to replace the inappropriate words in the filtered output. `ignore` is an array of characters that will be ignored during the filtering process.

The class also has a public boolean variable `displayReadString` that determines whether or not the filtered words will be logged to the console.

The class has a static instance `_instance` that ensures only one instance of the `WordFilter` class is created. The `Instance` property returns the instance of the `WordFilter` class.

The `Awake` method is called when the script instance is being loaded. It ensures that the `WordFilter` object is not destroyed when a new scene is loaded.

The `Load` method is responsible for loading the list of inappropriate words from either a local file or a web server, depending on the platform. If the platform is a web player, the `LoadFromWWW` coroutine is started, which downloads the word list from a specified URL. If the platform is not a web player, the `LoadFromLocalFileSystem` method is called, which loads the word list from a local file. The loaded word list is then parsed and stored in the `badWords` array.

The `Filter` method takes an input string and filters out any inappropriate words based on the current language option. It replaces the inappropriate words with a string of characters from the `blind` array. The filtered string is then returned.

The `IgnoreFilter` method is similar to the `Filter` method, but it also ignores certain characters specified in the `ignore` array. It removes these characters from the input string before filtering and then inserts them back into their original positions after filtering.

The `CheckBadword` method checks if the input string contains any inappropriate words without filtering them. It returns the first inappropriate word found, or an empty string if no inappropriate words are found.

Overall, the `WordFilter` class provides a way to filter and censor inappropriate words in user-generated content within the Brick-Force game. It ensures that the game remains family-friendly and adheres to community guidelines.
## Questions: 
 **Question 1:** What is the purpose of the `WordFilter` class?
    
**Answer:** The `WordFilter` class is responsible for filtering and censoring words in a given input string.

**Question 2:** How does the `WordFilter` class load the list of bad words?
    
**Answer:** The `WordFilter` class can load the list of bad words either from a web server or from the local file system, depending on the platform.

**Question 3:** How does the `WordFilter` class handle filtering and censoring words?
    
**Answer:** The `WordFilter` class replaces any occurrence of a bad word in the input string with a string of random characters from the `blind` array. It also has a method called `IgnoreFilter` that ignores certain characters from being censored.