
namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers;

public interface IBlockCipher
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
    /// Encrypts the specified region of the input byte array and copies the encrypted data to the specified region of the output byte array.
    /// </summary>
    /// <param name="input">The input data to encrypt.</param>
    /// <param name="inputOffset">The offset into the input byte array from which to begin using data.</param>
    /// <param name="inputCount">The number of bytes in the input byte array to use as data.</param>
    /// <param name="output">The output to which to write encrypted data.</param>
    /// <param name="outputOffset">The offset into the output byte array from which to begin writing data.</param>
    void EncryptBlock(byte[] input, int inputOffset, int inputCount, byte[] output, int outputOffset);

    /// <summary>
    /// Decrypts the specified region of the input byte array and copies the decrypted data to the specified region of the output byte array.
    /// </summary>
    /// <param name="input">The input data to decrypt.</param>
    /// <param name="inputOffset">The offset into the input byte array from which to begin using data.</param>
    /// <param name="inputCount">The number of bytes in the input byte array to use as data.</param>
    /// <param name="output">The output to which to write decrypted data.</param>
    /// <param name="outputOffset">The offset into the output byte array from which to begin writing data.</param>
    void DecryptBlock(byte[] input, int inputOffset, int inputCount, byte[] output, int outputOffset);
    }
