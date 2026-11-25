namespace CoreClassLibrary.Mathematics.IdGenerators;

public class IntIdGenerator(int seed) : IIdGenerator<int>
    {
    public IntIdGenerator() : this((int)DateTime.UtcNow.Ticks) { }

    public int NextId()
        => Interlocked.Increment(ref seed);
    }
