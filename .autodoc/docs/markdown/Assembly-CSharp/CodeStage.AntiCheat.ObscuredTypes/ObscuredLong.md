[View code on GitHub](https://github.com/TieHaxJan/Brick-Force/Assembly-CSharp\CodeStage.AntiCheat.ObscuredTypes\ObscuredLong.cs)

The code provided is a struct called `ObscuredLong` that is used to encrypt and decrypt long integer values. It is part of the `CodeStage.AntiCheat.ObscuredTypes` namespace.

The purpose of this code is to provide a way to store sensitive long integer values in an encrypted form, making it difficult for attackers to manipulate or cheat in the game. It achieves this by using a symmetric encryption algorithm, where the same key is used for both encryption and decryption.

The `ObscuredLong` struct has several private fields:
- `cryptoKey` is a static long variable that holds the encryption key. By default, it is set to 444442L, but it can be changed using the `SetNewCryptoKey` method.
- `currentCryptoKey` is a long variable that holds the current encryption key for the instance of `ObscuredLong`.
- `hiddenValue` is a long variable that holds the encrypted value.
- `fakeValue` is a long variable that holds the decrypted value, used for cheating detection.
- `inited` is a boolean variable that indicates whether the instance has been initialized.

The struct provides methods for encryption and decryption:
- `Encrypt` and `Decrypt` methods are used to encrypt and decrypt a long value using the current encryption key.
- `GetEncrypted` method returns the encrypted value of the instance. If the current encryption key is different from the global encryption key, it decrypts the value, re-encrypts it with the global encryption key, and updates the current encryption key.
- `SetEncrypted` method sets the encrypted value of the instance. If cheating is detected (the `onCheatingDetected` action is not null), it also sets the fake value to the decrypted value.
- `InternalDecrypt` method is used internally to decrypt the hidden value. If the instance has not been initialized, it initializes it with the global encryption key and encrypts a default value. It then decrypts the hidden value using the current encryption key and checks for cheating by comparing the decrypted value with the fake value.

The struct also overrides several methods:
- `Equals` methods are used to compare two instances of `ObscuredLong` for equality based on their hidden values.
- `GetHashCode` method returns the hash code of the decrypted value.
- `ToString` methods return the string representation of the decrypted value.

The struct also provides implicit conversion operators to convert between `ObscuredLong` and `long` types. It also provides overloaded increment and decrement operators to increment or decrement the hidden value.

Overall, this code provides a way to store and manipulate sensitive long integer values in an encrypted form, making it difficult for attackers to manipulate or cheat in the game. It also includes cheating detection by comparing the decrypted value with the fake value.
## Questions: 
 1. What is the purpose of the `ObscuredLong` struct?
- The `ObscuredLong` struct is used to store and manipulate encrypted long values.

2. How does the encryption and decryption process work?
- The encryption process is done using the XOR operation with a crypto key. The decryption process is the same as the encryption process.

3. What is the purpose of the `onCheatingDetected` action and how is it used?
- The `onCheatingDetected` action is used to detect cheating in the code. It is triggered when the decrypted value does not match the fake value, indicating that the value has been tampered with.