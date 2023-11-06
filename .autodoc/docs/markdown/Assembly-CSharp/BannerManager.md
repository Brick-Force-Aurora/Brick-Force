[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\BannerManager.cs)

The `BannerManager` class in the Brick-Force project is responsible for managing and displaying banner ads. It contains methods for adding ads, retrieving ads, and clearing the ads.

The class has a private field `ads` which is a `SortedDictionary<int, Banner>`. This dictionary stores the ads, with the row number as the key and a `Banner` object as the value. The `Banner` class represents an individual ad and contains properties for the image path, action type, action parameter, banner texture, and a reference to the CDN (Content Delivery Network) object.

The class also has a private static field `_instance` and a public static property `Instance`. This follows the Singleton design pattern and ensures that there is only one instance of the `BannerManager` class throughout the project. The `Instance` property returns the singleton instance of the `BannerManager` class.

The `Awake` method is called when the `BannerManager` object is created. It initializes the `ads` dictionary and ensures that the object is not destroyed when a new scene is loaded.

The `Clear` method clears all the ads from the `ads` dictionary.

The `AddAd` method is used to add a new ad to the `ads` dictionary. It takes parameters for the row number, image path, action type, and action parameter. If an ad with the same row number already exists in the dictionary, the existing ad is updated with the new values. The method then calls the `GetBanner` method to retrieve the updated banner object and starts a coroutine `LoadBannerTexture` to load the banner texture asynchronously.

The `GetBnnr` method returns the banner texture for a given row number. It checks if the row number exists in the `ads` dictionary and returns the banner texture if it exists.

The `GetBanner` method returns the `Banner` object for a given row number. It checks if the row number exists in the `ads` dictionary and returns the `Banner` object if it exists.

The `Count` method returns the number of ads in the `ads` dictionary.

The `Start` and `Update` methods are empty and not used in this class.

The `LoadBannerTexture` method is a coroutine that loads the banner texture from a URL. It takes a `Banner` object as a parameter. It constructs the URL using the image path from the `Banner` object and starts a `WWW` request to download the image. It then creates a new `Texture2D` object with a size of 128x128 and loads the image data into it. If the image fails to load, the `Texture2D` object is set to null. Otherwise, the texture is applied and the `WWW` object is disposed.

Overall, the `BannerManager` class provides functionality for managing and displaying banner ads in the Brick-Force project. It allows for adding, retrieving, and clearing ads, as well as loading the banner textures asynchronously.
## Questions: 
 1. What is the purpose of the `BannerManager` class?
- The `BannerManager` class is responsible for managing banners in the game.

2. What is the purpose of the `ads` variable?
- The `ads` variable is a SortedDictionary that stores banners, with the key being the row number and the value being a `Banner` object.

3. What is the purpose of the `LoadBannerTexture` coroutine?
- The `LoadBannerTexture` coroutine is responsible for loading the banner texture from a URL and assigning it to the `Bnnr` property of a `Banner` object.