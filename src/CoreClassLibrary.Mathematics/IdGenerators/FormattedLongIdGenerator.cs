
namespace CoreClassLibrary.Mathematics.IdGenerators;

public class FormattedLongIdGenerator(IIdGenerator<long> longGenerator, string format = "") : IIdGenerator<string>
    {
    public string NextId()
        => longGenerator.NextId().ToString(format);
    }
