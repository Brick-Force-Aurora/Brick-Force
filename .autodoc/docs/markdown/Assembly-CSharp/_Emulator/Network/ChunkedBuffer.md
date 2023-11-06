[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\_Emulator\Network\ChunkedBuffer.cs)

The code provided is a class called `ChunkedBuffer` that is used to handle and manage a buffer that is divided into chunks. 

The purpose of this class is to provide a way to write data into the buffer in chunks and keep track of the order of the chunks. It is designed to be used in a larger project called Brick-Force, although the specific details of how it fits into the project are not provided in the code snippet.

The `ChunkedBuffer` class has several properties and methods:

- `buffer`: A byte array that represents the buffer.
- `offset`: An unsigned integer that represents the current offset in the buffer.
- `crc`: An unsigned integer that represents the cyclic redundancy check (CRC) value of the buffer.
- `id`: A ushort (unsigned short) that represents the ID of the buffer.
- `lastChunk`: An integer that represents the ID of the last written chunk.
- `finished`: A boolean flag that indicates whether the buffer has been fully written.

The constructor of the `ChunkedBuffer` class takes three parameters: `_size`, `_crc`, and `_id`. It initializes the `buffer` property with a new byte array of size `_size`, sets the `offset` to 0, assigns the `_crc` value to the `crc` property, assigns the `_id` value to the `id` property, and sets the `lastChunk` to -1.

The `WriteNext` method is used to write the next chunk of data into the buffer. It takes two parameters: `next`, which is the byte array representing the next chunk of data, and `chunkId`, which is the ID of the current chunk. 

Inside the method, it first checks if the size of the `next` array plus the current `offset` exceeds the size of the `buffer`. If it does, it logs an error message and returns.

Next, it increments the `lastChunk` by 1 and checks if the `chunkId` matches the expected `lastChunk` value. If it doesn't match, it logs an error message and returns.

Finally, it copies the contents of the `next` array into the `buffer` starting from the current `offset`, and updates the `offset` by adding the length of the `next` array.

Overall, this code provides a way to write data into a buffer in chunks and keep track of the order of the chunks. It ensures that the chunks are written in the correct order and that the buffer does not exceed its size.
## Questions: 
 1. What is the purpose of the ChunkedBuffer class?
- The ChunkedBuffer class is used to store and manage a buffer that is divided into chunks.

2. What does the WriteNext method do?
- The WriteNext method appends the given byte array to the buffer, as long as the buffer has enough space. It also checks if the chunks are being written in the correct order.

3. What is the significance of the lastChunk variable?
- The lastChunk variable keeps track of the last chunk that was written to the buffer, and is used to check if the chunks are being written in the correct order.