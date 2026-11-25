
using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Mathematics.IdGenerators;

/// <summary>
/// Generates an id using the base 64 alphabet
/// </summary>
/// <remarks>
/// The id is 12 characters length
/// </remarks>
/// <example>CNrcTr8t9/w=</example>
/// <see cref="https://base64.guru/learn/base64-characters"/>
/// <see cref="https://base64.guru/learn/base64-algorithm"/>
/// <see cref="https://en.wikipedia.org/wiki/Base64"/>
/// <see cref="Base32IdGenerator"/>
public class Base64IdGenerator(IIdGenerator<long> longGenerator) : IIdGenerator<string>
    {
    public string NextId()
        => Base64IdGenerator.EncodeId(longGenerator.NextId());

    public static string EncodeId(long id)
        {
        Span<byte> buffer = stackalloc byte[8];
        id.GetBytesHighFirst().WriteTo(buffer);
        return Convert.ToBase64String(buffer);
        }
    }
