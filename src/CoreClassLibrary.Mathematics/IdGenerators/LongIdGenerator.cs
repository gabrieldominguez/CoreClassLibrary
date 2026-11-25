namespace CoreClassLibrary.Mathematics.IdGenerators;

public class LongIdGenerator(long seed) : IIdGenerator<long>
    {
    public LongIdGenerator() : this(DateTime.UtcNow.Ticks) { }

    public long NextId()
        => Interlocked.Increment(ref seed);
    }
