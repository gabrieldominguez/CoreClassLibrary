
namespace CoreClassLibrary.Mathematics.IdGenerators;

/// <summary>
/// Generates an id using the hexa alphabet 0-9A-F
/// </summary>
/// <remarks>
/// The id is 16 characters length
/// </remarks>
/// <example>08DADC715C2F3E88</example>
public class Hex16IdGenerator(IIdGenerator<long> longGenerator, string format = "x16") : IIdGenerator<string>
    {
    public string NextId()
        => this.EncodeId(longGenerator.NextId());

    public string EncodeId(long number)
        => number.ToString(format);
    }
