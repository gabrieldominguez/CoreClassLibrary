
namespace CoreClassLibrary.Mathematics.Randomness;

public class RandomProvider : IRandomProvider
    {
    public IRandom GetInstance() => new RandomWrapper();
    }
