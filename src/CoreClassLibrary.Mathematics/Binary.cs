#pragma warning disable IDE0046 // Convert to conditional expression

using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Pooling;
using System.Globalization;
using System.Numerics;

namespace CoreClassLibrary.Mathematics;

public static class Binary
    {
    public static readonly int BitsPerByte = 8;

    public static TSelf RotateLeft<TSelf>(this TSelf value, int rotateAmount) where TSelf: IBinaryInteger<TSelf>
        => TSelf.RotateLeft(value, rotateAmount);

    public static TSelf RotateRight<TSelf>(this TSelf value, int rotateAmount) where TSelf: IBinaryInteger<TSelf>
        => TSelf.RotateRight(value, rotateAmount);

    /// <summary>
    /// Encodes the integer i into a 4-byte array, in big endian.
    /// </summary>
    public static void WriteBigEndianInt(uint value, byte[] buffer, int offset)
        {
        unchecked
            {
            //arr[offset] = (byte)(value >> 24);
            //arr[offset + 1] = (byte)(value >> 16);
            //arr[offset + 2] = (byte)(value >> 8);
            //arr[offset + 3] = (byte)value;

            for (int i = 0; i < 4; i++)
                {
                buffer[offset + i] = (byte)(value >> (Binary.BitsPerByte * (3 - i)));
                }
            }
        }

    /// <summary>
    /// Evalúa el bit seteados en number.
    /// </summary>
    /// <param name="number">Numero a testear.</param>
    /// <param name="bit">Bit a testear.</param>
    /// <remarks>
    /// Los bits se numeran del más significativo al menos :
    /// 00000000
    /// 01234567
    /// </remarks>
    /// <returns>Retorna true si el bit está en 1 en number.</returns>
    public static bool IsBitSettedHighFirst(this byte number, int bit)
        => number.IsMaskSetted((byte) BitOperations.RotateRight(128, bit)); // number.IsMaskSetted((byte)(128 >> bit));

    /// <summary>
    /// Evalúa el bit seteados en number.
    /// </summary>
    /// <param name="number">Numero a testear.</param>
    /// <param name="bit">Bit a testear.</param>
    /// <remarks>
    /// Los bits se numeran del menos significativo al más :
    /// 00000000
    /// 76543210
    /// </remarks>
    /// <returns>Retorna true si el bit está en 1 en number.</returns>
    public static bool IsBitSettedLowFirst(this byte number, int bit)
        => number.IsMaskSetted((byte) BitOperations.RotateLeft(1, bit)); // number.IsMaskSetted((byte)(1 << bit));


    /// <summary>
    /// Evalúa los bits seteados en number.
    /// </summary>
    /// <param name="number">Numero a testear.</param>
    /// <param name="mask">Bits a testear.</param>
    /// <returns>Retorna true si todos bits 1 de la máscara están en 1 en number.</returns>
    public static bool IsMaskSetted<TSelf>(this TSelf number, TSelf mask) where TSelf : IBinaryInteger<TSelf>
        => (number & mask) == mask;

    /// <summary>
    /// Enumera los bits del byte.
    /// </summary>
    /// <remarks>
    /// El orden de enumeración es empezando por los bits menos significativos hasta los más.
    /// </remarks>
    public static IEnumerable<bool> ToBitsLowFirst(this IEnumerable<byte> bytes)
        => bytes.SelectMany(Binary.ToBitsLowFirst);

    /// <summary>
    /// Enumera los bits del byte.
    /// </summary>
    /// <remarks>
    /// El orden de enumeración es empezando por los bits menos significativos hasta los más.
    /// </remarks>
    public static IEnumerable<bool> ToBitsLowFirst(this byte b)
        {
        for (int i = 0; i < Binary.BitsPerByte; i++)
            {
            yield return b.IsBitSettedLowFirst(i);    // (b >> i).IsMaskSetted(1);
            }
        }

    /// <summary>
    /// Enumera los bits del byte.
    /// </summary>
    /// <remarks>
    /// El orden de enumeración es empezando por los bits más significativos hasta los menos.
    /// </remarks>
    public static IEnumerable<bool> ToBitsHighFirst(this IEnumerable<byte> bytes)
        => bytes.SelectMany(Binary.ToBitsHighFirst);

    /// <summary>
    /// Enumera los bits del byte.
    /// </summary>
    /// <remarks>
    /// El orden de enumeración es empezando por los bits más significativos hasta los menos.
    /// </remarks>
    public static IEnumerable<bool> ToBitsHighFirst(this byte b)
        {
        for (int i = 0; i < Binary.BitsPerByte; i++)
            {
            yield return b.IsBitSettedHighFirst(i);     // (b << i).IsMaskSetted(0x80);
            }
        }

    /// <summary>
    /// Convierte una secuencia de bits en una de bytes, tomándolos de 8 en 8.
    /// </summary>
    /// <remarks>
    /// El orden de enumeración es empezando por los bits menos significativos hasta los más.
    /// </remarks>
    public static IEnumerable<byte> FromBitsLowFirst(this IEnumerable<bool> bits)
        {
        int i = 0;
        byte resultByte = 0;
        foreach (bool bit in bits)
            {
            if (bit)
                {
                resultByte |= (byte)(1 << i);
                }

            i++;

            if (i == Binary.BitsPerByte)
                {
                yield return resultByte;
                i = 0;
                resultByte = 0;
                }
            }
        }

    /// <summary>
    /// Convierte una secuencia de bits en una de bytes, tomándolos de 8 en 8.
    /// </summary>
    /// <remarks>
    /// El orden de enumeración es empezando por los bits más significativos hasta los menos.
    /// </remarks>
    public static IEnumerable<byte> FromBitsHighFirst(this IEnumerable<bool> bits)
        {
        int i = 0;
        byte resultByte = 0;
        foreach (bool bit in bits)
            {
            if (bit)
                {
                resultByte |= (byte)(1 << (7 - i));
                }

            i++;

            if (i == Binary.BitsPerByte)
                {
                yield return resultByte;
                i = 0;
                resultByte = 0;
                }
            }
        }

    public static IEnumerable<byte> GetBytesHighFirst(this long value)
        => ((UInt64) value).GetBytesHighFirst();

    /// <summary>
    /// Returns the specified 16-bit unsigned integer value as an array of bytes.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 2.</returns>
    public static IEnumerable<byte> GetBytesHighFirst(this UInt16 value)
        {
        yield return (byte)(value >> 8);
        yield return (byte) value; // yield return (byte)(value & 0xFF);
        }

    /// <summary>
    /// Returns the specified 32-bit unsigned integer value as an array of bytes.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 4.</returns>
    public static IEnumerable<byte> GetBytesHighFirst(this UInt32 value)
        {
        yield return (byte)(value >> 24);
        yield return (byte)(value >> 16);

        yield return (byte)(value >> 8);
        yield return (byte) value;
        }

    /// <summary>
    /// Returns the specified 64-bit unsigned integer value as an array of bytes.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 8.</returns>
    public static IEnumerable<byte> GetBytesHighFirst(this UInt64 value)
        {
        yield return (byte)(value >> 56);
        yield return (byte)(value >> 48);

        yield return (byte)(value >> 40);
        yield return (byte)(value >> 32);

        yield return (byte)(value >> 24);
        yield return (byte)(value >> 16);

        yield return (byte)(value >> 8);
        yield return (byte) value;
        }

    public static IEnumerable<byte> GetBytesLowFirst(this long value)
        => ((UInt64) value).GetBytesLowFirst();

    /// <summary>
    /// Returns the specified 16-bit unsigned integer value as an array of bytes.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 2.</returns>
    public static IEnumerable<byte> GetBytesLowFirst(this UInt16 value)
        {
        yield return (byte) value; // yield return (byte)(value & 0xFF);
        yield return (byte)(value >> 8);
        }

    /// <summary>
    /// Returns the specified 32-bit unsigned integer value as an array of bytes.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 4.</returns>
    public static IEnumerable<byte> GetBytesLowFirst(this UInt32 value)
        {
        yield return (byte) value;
        yield return (byte)(value >> 8);

        yield return (byte)(value >> 16);
        yield return (byte)(value >> 24);
        }

    /// <summary>
    /// Returns the specified 64-bit unsigned integer value as an array of bytes.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 8.</returns>
    public static IEnumerable<byte> GetBytesLowFirst(this UInt64 value)
        {
        yield return (byte) value;
        yield return (byte)(value >> 8);

        yield return (byte)(value >> 16);
        yield return (byte)(value >> 24);

        yield return (byte)(value >> 32);
        yield return (byte)(value >> 40);

        yield return (byte)(value >> 48);
        yield return (byte)(value >> 56);
        }


    /// <summary>
    /// Calcula xor sobre cada byte del arreglo.
    /// </summary>
    public static IEnumerable<TSelf> Xor<TSelf>(IEnumerable<TSelf> x, IEnumerable<TSelf> y) where TSelf: IBinaryInteger<TSelf>
        => EnumerableExtensions.EnumerateInTuples(x, y).Select(tuple => tuple.Item1 ^ tuple.Item2);

    public static string ToByteArrayLiteral(this IEnumerable<byte> bytes)
    //=> String.Concat("{ 0x", bytes.Select(b => b.ToHexString()).ToDelimitedString(", 0x"), " }");
        {
        using PooledStringBuilder builder = new PooledStringBuilder();
        return builder.StringBuilder
            .Append("{ 0x")
            .AppendDelimitedStrings(bytes.Select(b => b.ToHexString()), ", 0x")
            .Append(" }")
            .ToString();
        }

    /// <summary>
    /// Convierte el arreglo a una cadena de pares hexadecimales.
    /// </summary>
    /// <param name="bytes">Bytes a convertir.</param>
    /// <returns>Cadena con los valores hexadecimales.</returns>
    /// <see cref="http://stackoverflow.com/questions/311165/how-do-you-convert-byte-array-to-hexadecimal-string-and-vice-versa"/>
    /// <see cref="http://msdn.microsoft.com/en-us/library/dwhawy9k(v=vs.110).aspx#XFormatString"/>
    public static string ToHexString(this IEnumerable<byte> bytes)
        {
        using PooledStringBuilder builder = new PooledStringBuilder();
        return bytes
                    .Aggregate(builder.StringBuilder, (stringBuilder, b) => stringBuilder.Append($"{b:x2}"))
                    //.Select(b => b.ToString("x2")).Aggregate(builder.StringBuilder, (stringBuilder, s) => stringBuilder.AppendLine(s))
                    .ToString();
        }

    public static string ToHEXString(this IEnumerable<byte> bytes)
        {
        using PooledStringBuilder builder = new PooledStringBuilder();
        return bytes
                    .Aggregate(builder.StringBuilder, (stringBuilder, b) => stringBuilder.Append($"{b:X2}"))
                    //.Select(b => b.ToString("X2")).Aggregate(builder.StringBuilder, (stringBuilder, s) => stringBuilder.AppendLine(s))
                    .ToString();
        }

    /// <summary>
    /// Convierte el byte en una cadena hexadecimal.
    /// </summary>
    /// <see cref="http://msdn.microsoft.com/en-us/library/dwhawy9k(v=vs.110).aspx#XFormatString"/>
    public static string ToHexString(this byte b) => b.ToString("x2");
    public static string ToHEXString(this byte b) => b.ToString("X2");

    public static string ToHEXString(this UInt16 b) => b.ToString("X4");
    public static string ToHEXString(this UInt32 b) => b.ToString("X8");
    public static string ToHEXString(this UInt64 b) => b.ToString("X16");

    /// <summary>
    /// Convierte una cadena de pares hexadecimales a un arreglo de bytes.
    /// </summary>
    /// <param name="hexString">Cadena con los valores hexadecimales.</param>
    /// <returns>Bytes representados en la cadena.</returns>
    /// <see cref="http://stackoverflow.com/questions/311165/how-do-you-convert-byte-array-to-hexadecimal-string-and-vice-versa"/>
    public static byte[] FromHexString(this string hexString)
        {
        if (hexString.Length % 2 == 1)
            {
            throw new ArgumentException("Length must be even.", nameof(hexString));
            }

        int bufferLength = hexString.Length / 2;
        byte[] buffer = new byte[bufferLength];
        Span<char> hexPair = stackalloc char[2];
        using StringReader stringReader = new StringReader(hexString);

        for (int i = 0; i < bufferLength; i++)
            {
            stringReader.Read(hexPair);
            buffer[i] = Binary.FromHexPair(hexPair);
            }

        return buffer;
        }

    /// <summary>
    /// Convierte los dos primeros caracteres hexadecimales a un byte.
    /// </summary>
    public static byte FromHexPair(params char[] hexPair)
        => Binary.FromHexPair(new string(hexPair, 0, 2));

    public static byte FromHexPair(ReadOnlySpan<char> hexPair)
        {
        if (hexPair.Length != 2)
            {
            throw new ArgumentException("Length must be 2.", nameof(hexPair));
            }

        return byte.Parse(hexPair, NumberStyles.AllowHexSpecifier);
        }

    public static int FromHexPair(char highDigit, char lowDigit)
        => highDigit.FromHexDigit() * 16 + lowDigit.FromHexDigit();

    /// <summary>
    /// Convierte un par de caracteres hexadecimales a un byte.
    /// </summary>
    public static byte FromHexPair(this string hexPair)
        {
        if (hexPair.Length != 2)
            {
            throw new ArgumentException("Length must be 2.", nameof(hexPair));
            }

        //return Convert.ToByte(hexString, 16);
        return byte.Parse(hexPair, NumberStyles.AllowHexSpecifier);
        }

    public static bool IsHexDigit(this char c) 
        => c >= '0' && c <= '9' || c >= 'A' && c <= 'F' || c >= 'a' && c <= 'f';

    public static char ToHEXDigit(this int n) 
        //=> StringExtensions.HEXChars[n];
        => Binary.ToHexDigit(n, 'A');

    public static char ToHexDigit(this int n) 
        //=> StringExtensions.HexChars[n];
        => Binary.ToHexDigit(n, 'a');

    /// <see cref="HexConverter.ToCharLower"/>
    /// <see cref="HexConverter.ToCharUpper"/>
    private static char ToHexDigit(this int n, char a)
        {
        n &= 0xF;
        n += '0';

        if (n > '9')
            {
            n += (a - ('9' + 1));
            }

        return (char)n;
        }

    /// <see cref="HexConverter.FromChar"/>
    public static int FromHexDigit(this char c) 
        => c switch
            {
            >= '0' and <= '9' => c - '0',
            >= 'A' and <= 'F' => c - 'A' + 10,
            >= 'a' and <= 'f' => c - 'a' + 10,
            _ => throw new InvalidOperationException()
            };
    }
