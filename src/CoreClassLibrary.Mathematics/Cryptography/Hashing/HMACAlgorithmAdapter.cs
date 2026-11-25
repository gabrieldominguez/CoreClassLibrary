using System.Security.Cryptography;

namespace CoreClassLibrary.Mathematics.Cryptography.Hashing;

/// <summary>
/// Adaptador de IHashAlgorithm a KeyedHashAlgorithm.
/// </summary>
public sealed class HMACAlgorithmAdapter(HMACAlgorithm hmacAlgorithm) : KeyedHashAlgorithm
    {

    /// <summary>
    /// Gets the size, in bits, of the computed hash code.
    /// </summary>
    /// <returns>
    /// The size, in bits, of the computed hash code.
    /// </returns>
    public override int HashSize => hmacAlgorithm.HashSize;

    public override int InputBlockSize => hmacAlgorithm.BlockSize;
    public override int OutputBlockSize => hmacAlgorithm.BlockSize;

    public override byte[] Key
        {
        get => hmacAlgorithm.Key;
        set => hmacAlgorithm.Key = value;
        }

    /// <summary>
    /// Routes data written to the object into the hash algorithm for computing the hash.
    /// </summary>
    /// <param name="array">The input to compute the hash code for.</param>
    /// <param name="offset">The offset into the byte array from which to begin using data.</param>
    /// <param name="size">The number of bytes in the byte array to use as data.</param>
    protected override void HashCore(byte[] array, int offset, int size)
        => hmacAlgorithm.HashCore(array, offset, size);            

    /// <summary>
    /// Finalizes the hash computation after the last data is processed by the cryptographic stream object.
    /// </summary>
    /// <returns>
    /// The computed hash code.
    /// </returns>
    protected override byte[] HashFinal()
        => hmacAlgorithm.HashFinal();

    /// <summary>
    /// Initializes an implementation of the <see cref="HashAlgorithm"/> class.
    /// </summary>
    public override void Initialize()
        => hmacAlgorithm.Initialize();            
    }
