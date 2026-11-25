
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Mathematics.Cryptography.Ciphers.StreamCiphers.Algorithms;

// https://github.com/sshnet/SSH.NET/tree/master/src/Renci.SshNet/Security/Cryptography
namespace CoreClassLibrary.Mathematics.Randomness.Generators;

/// <summary>
/// Generador de números aleatorios basado en el algoritmo ARC4.
/// </summary>
/// <remarks>
/// Several operating systems include arc4random, an API originating in OpenBSD providing access to a random number generator originally based on RC4. I
///
/// ARC4 (Alleged RC4) is an implementation of RC4 (Rivest’s Cipher version 4), a symmetric stream cipher designed by Ron Rivest in 1987.
/// The cipher started as a proprietary design, that was reverse engineered and anonymously posted on Usenet in 1994.
/// The company that owns RC4(RSA Data Inc.) never confirmed the correctness of the leaked algorithm.
///
/// </remarks>
/// <see cref="https://en.wikipedia.org/wiki/RC4"/>
public sealed class Arc4Random(Arc4ByteProvider arc4ByteProvider) : IDiscreteRandom
    {
    /// <summary>
    /// Create a Arc4Random with a datetime Now (8 bytes) as seed.
    /// </summary>
    public static Arc4Random CreateFromNow()
        => new Arc4Random(DateTime.Now);

    /// <summary>
    /// Create a Arc4Random with a new guid (16 bytes) as seed.
    /// </summary>
    public Arc4Random() : this(Guid.NewGuid()) { }

    /// <summary>
    /// Create a Arc4Random with a guid (16 bytes) as seed.
    /// </summary>
    public Arc4Random(Guid seed) : this(seed.ToByteArray()) { }

    /// <summary>
    /// Create a Arc4Random with a datetime (8 bytes) as seed.
    /// </summary>
    public Arc4Random(DateTime seed) : this(seed.Ticks) { }

    /// <summary>
    /// Create a Arc4Random with a long (8 bytes) as seed.
    /// </summary>
    public Arc4Random(long seed) : this(seed.GetBytesHighFirst()) { }

    public Arc4Random(IEnumerable<byte> seed) : this(seed.ToArray()) { }

    public Arc4Random(byte[] seed): this(new Arc4ByteProvider(seed, false)) { }

    public void Initialize()
        => arc4ByteProvider.Initialize();

    public uint DiscreteSample()
        => BitConverter.ToUInt32(
            [
            arc4ByteProvider.GetNext(),
            arc4ByteProvider.GetNext(),
            arc4ByteProvider.GetNext(),
            arc4ByteProvider.GetNext(),
            ]);
    }
