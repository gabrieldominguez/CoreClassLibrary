
namespace CoreClassLibrary.Mathematics.Randomness;

/// <summary>
/// Adapta un IDiscreteRandom a IRandom
/// </summary>
public class RandomDiscreteAdapter(IDiscreteRandom discreteRandom) : IRandom
    {
    private const double m = uint.MaxValue + 1.0;

    public double Sample()
        => discreteRandom.DiscreteSample() / RandomDiscreteAdapter.m;
    }

/// <summary>
/// Adapta un IRandom a IDiscreteRandom
/// </summary>
public class DiscreteRandomAdapter(IRandom random) : IDiscreteRandom
    {
    public uint DiscreteSample()
        => random.Next<uint>();
    }
