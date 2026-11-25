using System.Security.Cryptography;

namespace CoreClassLibrary.Mathematics.Cryptography.Hashing;

/// <summary>
/// Adaptador de IHashAlgorithm a HashAlgorithm.
/// </summary>
public sealed class HashAlgorithmAdapter(IHashAlgorithm hashAlgorithm) : HashAlgorithm
    {
    public override int HashSize => hashAlgorithm.HashSize;
    public override int InputBlockSize => hashAlgorithm.BlockSize;
    public override int OutputBlockSize => hashAlgorithm.BlockSize;

    /// <summary>
    /// Routes data written to the object into the hash algorithm for computing the hash.
    /// </summary>
    /// <param name="array">The input to compute the hash code for.</param>
    /// <param name="offset">The offset into the byte array from which to begin using data.</param>
    /// <param name="size">The number of bytes in the byte array to use as data.</param>
    protected override void HashCore(byte[] array, int offset, int size)
        =>hashAlgorithm.HashCore(array, offset, size);            

    /// <summary>
    /// Finalizes the hash computation after the last data is processed by the cryptographic stream object.
    /// </summary>
    /// <returns>
    /// The computed hash code.
    /// </returns>
    protected override byte[] HashFinal()
        => hashAlgorithm.HashFinal();

    /// <summary>
    /// Initializes an implementation of the <see cref="HashAlgorithm"/> class.
    /// </summary>
    public override void Initialize()
        => hashAlgorithm.Initialize();            
    }
