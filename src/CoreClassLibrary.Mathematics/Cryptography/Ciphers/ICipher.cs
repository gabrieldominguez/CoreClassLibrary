
namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers;

public interface ICipher
    {
    void Initialize();

    /// <summary>
    /// Encrypts the specified input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="offset">The zero-based offset in <paramref name="input"/> at which to begin encrypting.</param>
    /// <param name="count">The number of bytes to encrypt from <paramref name="input"/>.</param>
    /// <returns>
    /// The encrypted data.
    /// </returns>
    byte[] Encrypt(byte[] input, int offset, int count);

    /// <summary>
    /// Decrypts the specified input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="offset">The zero-based offset in <paramref name="input"/> at which to begin decrypting.</param>
    /// <param name="count">The number of bytes to decrypt from <paramref name="input"/>.</param>
    /// <returns>
    /// The decrypted data.
    /// </returns>
    byte[] Decrypt(byte[] input, int offset, int count);
    }
