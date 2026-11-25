
namespace CoreClassLibrary.Mathematics.IdGenerators;

public class FormattedIntIdGenerator(IIdGenerator<int> intGenerator, string format = "") : IIdGenerator<string>
    {
    public string NextId()
        => intGenerator.NextId().ToString(format);
    }
