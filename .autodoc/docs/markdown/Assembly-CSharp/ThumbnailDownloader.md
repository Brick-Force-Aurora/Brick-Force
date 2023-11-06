[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\ThumbnailDownloader.cs)

The `ThumbnailDownloader` class is responsible for managing the downloading and processing of thumbnails in the Brick-Force project. It contains methods for enqueueing thumbnail requests, dequeuing completed requests, and stacking received thumbnail data.

The class uses three main data structures: `dicRegMap`, `dicUserMap`, and `queue`. `dicRegMap` and `dicUserMap` are dictionaries that store `ThumbnailRequest` objects, with the request ID as the key. These dictionaries are used to keep track of the requests that have been enqueued. `queue` is a queue that stores the `ThumbnailRequest` objects in the order they were enqueued.

The `Awake()` method is called when the `ThumbnailDownloader` object is created. It initializes the `queue`, `dicRegMap`, and `dicUserMap` data structures, and sets `waitingForResponse` to false. It also ensures that the `ThumbnailDownloader` object is not destroyed when a new scene is loaded.

The `Update()` method is called every frame. It checks if there is a thumbnail request in the queue and if the `ThumbnailDownloader` is not currently waiting for a response. If both conditions are met, it dequeues the next request from the queue and sends a download request to the `CSNetManager` class. It sets `waitingForResponse` to true to indicate that it is waiting for a response.

The `Enqueue()` method is used to enqueue a new thumbnail request. It takes a boolean parameter `isUserMap` to indicate whether the request is for a user map or a regular map, and an integer parameter `id` to specify the ID of the map. It checks if the request is already in the corresponding dictionary (`dicUserMap` or `dicRegMap`) and returns if it is. Otherwise, it creates a new `ThumbnailRequest` object and enqueues it in the `queue`. It also adds the request to the corresponding dictionary.

The `Dequeue()` method is used to dequeue a completed thumbnail request. It takes a boolean parameter `success` to indicate whether the request was successful or not. It dequeues the next request from the `queue` and removes it from the corresponding dictionary (`dicUserMap` or `dicRegMap`) if the request was successful. It sets `waitingForResponse` to false to indicate that it is no longer waiting for a response.

The `Stack()` method is used to stack received thumbnail data. It takes a byte array `data2` as input. It checks if there is a thumbnail request in the `queue` and if the request's `ThumbnailBuffer` is null. If both conditions are met, it creates a new byte array with the same length as `data2` and copies the data from `data2` to the `ThumbnailBuffer`. If the `ThumbnailBuffer` is not null, it creates a new byte array with the combined length of the existing `ThumbnailBuffer` and `data2`, and copies the data from both arrays to the new `ThumbnailBuffer`.

Overall, the `ThumbnailDownloader` class provides a way to manage and process thumbnail requests in the Brick-Force project. It allows for enqueuing requests, dequeuing completed requests, and stacking received thumbnail data.
## Questions: 
 1. What is the purpose of the `ThumbnailDownloader` class?
- The `ThumbnailDownloader` class is responsible for downloading and managing thumbnail images.

2. What is the significance of the `queue` variable?
- The `queue` variable is used to store thumbnail requests in the order they are received, and the code processes them one by one.

3. What is the purpose of the `Stack` method?
- The `Stack` method is used to store the downloaded thumbnail image data in the `ThumbnailRequest` object.