
namespace CoreClassLibrary.Mathematics.IdGenerators;

public class ToStringIdGenerator<T>(IIdGenerator<T> generator) : IIdGenerator<string>
    {
    public string NextId()
        => generator.NextId()?.ToString() ?? throw new InvalidOperationException("Id is null");
    }
