
using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Mathematics.Cryptography.Hashing;

/// <summary>
/// Provides HMAC (keyed-hash message authentication code) algorithm implementation.
/// </summary>
/// <remarks>
/// In cryptography, a keyed-hash message authentication code (HMAC) is a specific type of message authentication code (MAC) 
/// involving a cryptographic hash function and a secret cryptographic key. 
/// It may be used to simultaneously verify both the data integrity and the authentication of a message, as with any MAC. 
/// Any cryptographic hash function, such as MD5 or SHA-1, may be used in the calculation of an HMAC;
/// the resulting MAC algorithm is termed HMAC-X, where X is the hash function used (e.g. HMAC-MD5 or HMAC-SHA1). 
/// </remarks>
/// <see cref="https://tools.ietf.org/html/rfc2104"/>
/// <see cref="https://tools.ietf.org/html/rfc2202"/>
/// <see cref="https://en.wikipedia.org/wiki/Hash-based_message_authentication_code"/>
/// <see cref="https://github.com/sshnet/SSH.NET/tree/master/src/Renci.SshNet/Security/Cryptography"/>
public class HMACAlgorithm: IHashAlgorithm
    {
    private readonly IHashAlgorithm hashAlgorithm;
    private byte[]? innerPadding;
    private byte[]? outerPadding;

    /// <summary>
    /// Holds value indicating whether the inner padding was already written.
    /// </summary>
    private bool innerPaddingWritten;

    /// <summary>
    /// Gets the size of the block.
    /// </summary>
    /// <value>
    /// The size of the block.
    /// </value>
    public int BlockSize => this.hashAlgorithm.BlockSize;

    /// <summary>
    /// Gets the size, in bits, of the computed hash code.
    /// </summary>
    /// <value>
    /// The size, in bits, of the computed hash code.
    /// </value>
    public int HashSize { get; }

    public HMACAlgorithm(IHashAlgorithm hashAlgorithm, byte[] key) : this(hashAlgorithm, key, hashAlgorithm.HashSize) { }

    /// <summary>
    /// Initializes a <see cref="HMACAlgorithm"/> with the specified hash algorithm and key.
    /// </summary>
    /// <param name="hashAlgorithm">The hash Algorithm.</param>
    /// <param name="key">The key.</param>
    /// <param name="hashSize">The size, in bits, of the computed hash code.</param>
    public HMACAlgorithm(IHashAlgorithm hashAlgorithm, byte[] key, int hashSize)
        {
        this.hashAlgorithm = hashAlgorithm;
        this.Key = key;
        this.HashSize = hashSize;
        }

    private byte[]? key;
    /// <summary>
    /// Gets or sets the key to use in the hash algorithm.
    /// </summary>
    /// <returns>
    /// The key to use in the hash algorithm.
    /// </returns>
    public byte[] Key
        {
        get
            {
            return this.key ?? throw new InvalidOperationException("Algorithm not initialized. Key not setted.");
            }
        set
            {
            byte[] shortenedKey = this.GetShortenedKey(value);

            this.innerPadding = new byte[BlockSize];
            this.outerPadding = new byte[BlockSize];

            // Compute inner and outer padding.
            for (int i = 0; i < shortenedKey.Length; i++)
                {
                this.innerPadding[i] = (byte)(0x36 ^ shortenedKey[i]);
                this.outerPadding[i] = (byte)(0x5C ^ shortenedKey[i]);
                }
            for (int i = shortenedKey.Length; i < BlockSize; i++)
                {
                this.innerPadding[i] = 0x36;
                this.outerPadding[i] = 0x5C;
                }

            this.key = shortenedKey.CloneArray();
            }
        }

    public void Initialize()
        {
        this.hashAlgorithm.Initialize();
        this.innerPaddingWritten = false;
        }

    /// <summary>
    /// Hashes the core.
    /// </summary>
    public void HashCore(byte[] rgb, int ib, int cb)
        {
        if (!this.innerPaddingWritten)
            {
            // write the inner padding
            this.hashAlgorithm.HashCore(
                this.innerPadding ?? throw new InvalidOperationException("Algorithm not initialized. InnerPadding not setted."), 
                0, 
                this.BlockSize);

            // ensure we only write inner padding once
            this.innerPaddingWritten = true;
            }

        this.hashAlgorithm.HashCore(rgb, ib, cb);
        }

    /// <summary>
    /// Finalizes the hash computation after the last data is processed by the cryptographic stream object.
    /// </summary>
    /// <returns>
    /// The computed hash code.
    /// </returns>
    public byte[] HashFinal()
        {
        // finalize the original hash
        byte[] hash = this.hashAlgorithm.ComputeHash(Array.Empty<byte>());

        // write the outer padding
        this.hashAlgorithm.HashCore(
            this.outerPadding ?? throw new InvalidOperationException("Algorithm not initialized. OuterPadding not setted."), 
            0, 
            this.BlockSize);

        // write the inner hash and finalize the hash
        hashAlgorithm.HashCore(hash, 0, hash.Length);            

        return this.GetTruncatedHash(hashAlgorithm.HashFinal());
        }

    private byte[] GetTruncatedHash(byte[] hash)
        {
        int hashSizeBytes = this.HashSize / 8;
        if (hash.Length == hashSizeBytes)
            {
            return hash;
            }

        byte[] truncatedHash = new byte[hashSizeBytes];
        Buffer.BlockCopy(hash, 0, truncatedHash, 0, hashSizeBytes);
        return truncatedHash;
        }

    /// <summary>
    /// Return a key that fits the <see cref="BlockSize"/> of the <see cref="HashAlgorithm"/>.
    /// </summary>
    /// <param name="key">The key to shorten, if necessary.</param>
    /// <returns>
    /// A hash of <paramref name="key"/> if <paramref name="key"/> is longer than the <see cref="BlockSize"/> of the
    /// <see cref="HashAlgorithm"/>; otherwise, <paramref name="key"/>.
    /// </returns>
    private byte[] GetShortenedKey(byte[] key) 
        => key.Length > this.BlockSize ? 
                this.hashAlgorithm.ComputeHash(key) : 
                key;
    }
