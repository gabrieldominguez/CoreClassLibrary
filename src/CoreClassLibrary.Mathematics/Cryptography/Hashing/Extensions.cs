
using CoreClassLibrary.Extensions;
using System.Text;

namespace CoreClassLibrary.Mathematics.Cryptography.Hashing;

public static class Extensions
    {
    public static string ComputeHash(this IHashAlgorithm hashAlgorithm, string data)
        => hashAlgorithm.ComputeHash(data, StringExtensions.UTF8Encoding);

    public static string ComputeHash(this IHashAlgorithm hashAlgorithm, string data, Encoding encoding)
        => hashAlgorithm.ComputeHash(encoding.GetBytes(data)).ToHexString();

    /// <summary>
    /// Computes the hash value for the input data.
    /// </summary>
    /// <param name="buffer">The input to compute the hash code for.</param>
    /// <returns>
    /// The computed hash code.
    /// </returns>
    public static byte[] ComputeHash(this IHashAlgorithm hashAlgorithm, byte[] buffer)
        {
        hashAlgorithm.HashCore(buffer, 0, buffer.Length);
        byte[] hash = hashAlgorithm.HashFinal();
        hashAlgorithm.Initialize();

        return hash;
        }
    }
