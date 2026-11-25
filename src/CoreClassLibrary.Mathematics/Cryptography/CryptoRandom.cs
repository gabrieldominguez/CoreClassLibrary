
using CoreClassLibrary.Mathematics.Randomness;
using System.Security.Cryptography;

namespace CoreClassLibrary.Mathematics.Cryptography;

/// <summary>
/// IRandom implementation of cryptographically strong random values generator.
/// </summary>
public sealed class CryptoRandom(RandomNumberGenerator randomNumberGenerator) : IDiscreteRandom
    {
    public uint DiscreteSample()
        {
        Span<byte> buffer = stackalloc byte[4];
        randomNumberGenerator.GetBytes(buffer);
        return BitConverter.ToUInt32(buffer);
        }
    }
