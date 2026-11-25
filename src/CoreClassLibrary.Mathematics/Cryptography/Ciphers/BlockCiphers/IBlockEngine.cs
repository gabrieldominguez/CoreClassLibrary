
namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers;

public interface IBlockEngine
    {
    void Initialize();

    /// <summary>
    /// Gets the size of the block in bytes.
    /// </summary>
    /// <value>
    /// The size of the block in bytes.
    /// </value>
    int BlockSize { get; }

    /// <summary>
    /// Process (Encrypts/Decrypts) the specified input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="offset">The zero-based offset in <paramref name="input"/> at which to begin encrypting.</param>
    /// <param name="length">The number of bytes to encrypt from <paramref name="input"/>.</param>
    void ProcessBlock(byte[] input, int inputOffset, byte[] output, int outputOffset);
    }
