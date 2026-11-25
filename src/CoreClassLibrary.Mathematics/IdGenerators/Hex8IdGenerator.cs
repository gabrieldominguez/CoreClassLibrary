
using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Mathematics.IdGenerators;

/// <summary>
/// Generates an efficient id using the hexa alphabet 0-9A-F
/// </summary>
/// <remarks>
/// A faster version of int.ToString("X8")
/// The id is 8 characters length
/// </remarks>
/// <example>BC2D2C3D</example>
/// <see cref="https://github.com/aspnet/KestrelHttpServer/blob/0aff4a0440c2f393c0b98e9046a8e66e30a56cb0/src/Kestrel.Core/Internal/Infrastructure/StringUtilities.cs#L121"/>
public class Hex8IdGenerator(IIdGenerator<int> intGenerator) : IIdGenerator<string>
    {
    public string NextId()
        => Hex8IdGenerator.EncodeId(intGenerator.NextId());

    /// <summary>
    /// <remarks>A faster version of int.ToString("X8")</remarks>
    /// </summary>
    public static string EncodeId(int number)
        {
        Span<char> buffer = stackalloc char[8];

        buffer[7] = StringExtensions.HEXChars[number & 0xF];
        buffer[6] = StringExtensions.HEXChars[(number >> 4) & 0xF];
        buffer[5] = StringExtensions.HEXChars[(number >> 8) & 0xF];
        buffer[4] = StringExtensions.HEXChars[(number >> 12) & 0xF];
        buffer[3] = StringExtensions.HEXChars[(number >> 16) & 0xF];
        buffer[2] = StringExtensions.HEXChars[(number >> 20) & 0xF];
        buffer[1] = StringExtensions.HEXChars[(number >> 24) & 0xF];
        buffer[0] = StringExtensions.HEXChars[(number >> 28) & 0xF];

        return buffer.ToString();
        }
    }
