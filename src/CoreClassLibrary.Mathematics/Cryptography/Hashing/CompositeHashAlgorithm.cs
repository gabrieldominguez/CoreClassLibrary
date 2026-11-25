
namespace CoreClassLibrary.Mathematics.Cryptography.Hashing;

/// <summary>
/// Doble hashing.
/// </summary>
/// <remarks>
/// No reusar las referencias en IHashAlgorithm (hashAlgorithm1 != hashAlgorithm2).
/// </remarks>
/// <see cref="https://en.bitcoin.it/wiki/Protocol_documentation#Hashes"/>
/// <see cref="https://en.bitcoin.it/wiki/Hashcash#Double_Hash"/>
/// <see cref="https://bitcoin.stackexchange.com/questions/6037/why-are-hashes-in-the-bitcoin-protocol-typically-computed-twice-double-computed"/>
public class DoubleHashAlgorithm(IHashAlgorithm hashAlgorithm1, IHashAlgorithm hashAlgorithm2) : CompositeHashAlgorithm([hashAlgorithm1, hashAlgorithm2]);

/// <summary>
/// Permite aplicar hashing sucesivamente.
/// </summary>
/// <remarks>
/// No reusar las referencias en IHashAlgorithm[].
/// </remarks>
public class CompositeHashAlgorithm(IReadOnlyList<IHashAlgorithm> hashAlgorithms) : IHashAlgorithm
    {
    private byte[]? output;

    public int HashSize { get; } = hashAlgorithms[^1].HashSize;

    public int BlockSize { get; } = hashAlgorithms[^1].BlockSize;

    public void Initialize()
        {
        foreach (IHashAlgorithm hashAlgorithm in hashAlgorithms)
            {
            hashAlgorithm.Initialize();
            }
        }

    /// <summary>
    /// Routes data written to the object into the hash algorithm for computing the hash.
    /// </summary>
    /// <param name="array">The input to compute the hash code for.</param>
    /// <param name="offset">The offset into the byte array from which to begin using data.</param>
    /// <param name="size">The number of bytes in the byte array to use as data.</param>
    public void HashCore(byte[] array, int offset, int size)
        {
        hashAlgorithms[0].HashCore(array, offset, size);
        this.output = hashAlgorithms[0].HashFinal();

        for (int i = 1; i < hashAlgorithms.Count; i++)
            {
            hashAlgorithms[i].HashCore(this.output, 0, this.output.Length);
            this.output = hashAlgorithms[i].HashFinal();
            }
        }

    /// <summary>
    /// Finalizes the hash computation after the last data is processed by the cryptographic stream object.
    /// </summary>
    /// <returns>
    /// The computed hash code.
    /// </returns>
    public byte[] HashFinal()
        => this.output ?? throw new InvalidOperationException("Execute HashCore first.");
    }
