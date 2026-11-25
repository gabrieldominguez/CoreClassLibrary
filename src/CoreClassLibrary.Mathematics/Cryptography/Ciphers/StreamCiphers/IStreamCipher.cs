
namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.StreamCiphers;

public interface IStreamCipher
    {
    void Initialize();

    /// <summary>
    /// Encrypts the specified input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>
    /// The encrypted data.
    /// </returns>
    byte Encrypt(byte input);

    /// <summary>
    /// Decrypts the specified input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>
    /// The decrypted data.
    /// </returns>
    byte Decrypt(byte input);
    }
