
namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers;

public interface IEngine
    {
    void Initialize();

    /// <summary>
    /// Process (Encrypts/Decrypts) the specified input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="offset">The zero-based offset in <paramref name="input"/> at which to begin encrypting.</param>
    /// <param name="count">The number of bytes to encrypt from <paramref name="input"/>.</param>
    /// <returns>
    /// The encrypted/decrypted  data.
    /// </returns>
    byte[] Process(byte[] input, int offset, int count);
    }
