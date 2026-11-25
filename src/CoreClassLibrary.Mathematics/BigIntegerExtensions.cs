
using System.Numerics;

namespace CoreClassLibrary.Mathematics;

public static class BigIntegerExtensions
    {
    /// <summary>
    /// Mods the inverse.
    /// </summary>
    /// <param name="bi">The bi.</param>
    /// <param name="modulus">The modulus.</param>
    public static BigInteger ModInverse(BigInteger bi, BigInteger modulus)
        {
        BigInteger a = modulus, b = bi % modulus;
        BigInteger p0 = 0, p1 = 1;

        while (!b.IsZero)
            {
            if (b.IsOne)
                {
                return p1;
                }

            p0 += a / b * p1;
            a %= b;

            if (a.IsZero)
                {
                break;
                }

            if (a.IsOne)
                {
                return modulus - p0;
                }

            p1 += b / a * p0;
            b %= a;
            }

        return BigInteger.Zero;
        }

    /// <summary>
    /// Returns positive remainder that results from division with two specified <see cref="BigInteger"/> values.
    /// </summary>
    /// <param name="dividend">The value to be divided.</param>
    /// <param name="divisor">The value to divide by.</param>
    /// <returns>
    /// Positive remainder that results from the division.
    /// </returns>
    public static BigInteger PositiveMod(BigInteger dividend, BigInteger divisor)
        {
        BigInteger result = dividend % divisor;
        if (result < BigInteger.Zero)
            {
            result += divisor;
            }

        return result;
        }
    }
