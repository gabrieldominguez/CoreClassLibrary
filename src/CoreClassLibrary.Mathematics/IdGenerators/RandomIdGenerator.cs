
using CoreClassLibrary.Mathematics.Randomness;

namespace CoreClassLibrary.Mathematics.IdGenerators;

public class RandomIdGenerator(IRandom random) : IIdGenerator<int>
    {
    public RandomIdGenerator(): this(RandomWrapper.Shared) { }

    public int NextId() => random.Next<int>();
    }
