
using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Mathematics.IdGenerators;

/// <summary>
/// Generates an efficient id using the alphabet 0-9A-V
/// </summary>
/// <remarks>
/// The id is 13 characters length
/// </remarks>
/// <example>0HMMIQTF4J1U2</example>
/// <see href="https://github.com/aspnet/KestrelHttpServer/blob/6fde01a825cffc09998d3f8a49464f7fbe40f9c4/src/Kestrel.Core/Internal/Infrastructure/CorrelationIdGenerator.cs"/>
/// <see cref="http://www.nimaara.com/2018/10/10/generating-ids-in-csharp"/>
/// <see cref="https://stackoverflow.com/questions/50689315/how-is-httpcontext-traceidentifier-generated-in-net-core"/>
/// <see cref="https://en.wikipedia.org/wiki/Base32"/>
/// <see cref="Base64IdGenerator"/>
public class Base32IdGenerator(IIdGenerator<long> longGenerator) : IIdGenerator<string>
    {
    public string NextId()
        => Base32IdGenerator.EncodeId(longGenerator.NextId());

    public static string EncodeId(long id)
        {
        Span<char> buffer =
            [
            StringExtensions.Base32Chars[(int)(id >> 60) & 31],
            StringExtensions.Base32Chars[(int)(id >> 55) & 31],
            StringExtensions.Base32Chars[(int)(id >> 50) & 31],
            StringExtensions.Base32Chars[(int)(id >> 45) & 31],
            StringExtensions.Base32Chars[(int)(id >> 40) & 31],
            StringExtensions.Base32Chars[(int)(id >> 35) & 31],
            StringExtensions.Base32Chars[(int)(id >> 30) & 31],
            StringExtensions.Base32Chars[(int)(id >> 25) & 31],
            StringExtensions.Base32Chars[(int)(id >> 20) & 31],
            StringExtensions.Base32Chars[(int)(id >> 15) & 31],
            StringExtensions.Base32Chars[(int)(id >> 10) & 31],
            StringExtensions.Base32Chars[(int)(id >> 5) & 31],
            StringExtensions.Base32Chars[(int)id & 31],
            ];

        return buffer.ToString();
        }
    }
