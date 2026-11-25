
namespace CoreClassLibrary.Mathematics.Cryptography.Hashing;

/// <see cref="https://softwareengineering.stackexchange.com/questions/49550/which-hashing-algorithm-is-best-for-uniqueness-and-speed"/>
public interface IHashAlgorithm
    {
    /// <summary>
    /// Gets the size, in bits, of the computed hash code.
    /// </summary>
    /// <returns>
    /// The size, in bits, of the computed hash code.
    /// </returns>
    int HashSize { get; }

    /// <summary>
    /// Gets the block size.
    /// </summary>
    /// <returns>
    /// The block size.
    /// </returns>
    int BlockSize { get; }

    /// <summary>
    /// Resets an implementation of the <see cref="IHashProvider"/> to its initial state.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Routes data written to the object into the hash algorithm for computing the hash.
    /// </summary>
    /// <param name="array">The input to compute the hash code for.</param>
    /// <param name="offset">The offset into the byte array from which to begin using data.</param>
    /// <param name="size">The number of bytes in the byte array to use as data.</param>
    void HashCore(byte[] input, int offset, int size);

    /// <summary>
    /// Finalizes the hash computation after the last data is processed by the cryptographic stream object.
    /// </summary>
    /// <returns>
    /// The computed hash code.
    /// </returns>
    byte[] HashFinal();
    }
