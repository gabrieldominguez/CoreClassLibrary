namespace CoreClassLibrary.Mathematics.Randomness;

/// <summary>
/// Wrapper de la clase Random del framework para usar la interface IRandom
/// </summary>
/// <remarks>
/// The current implementation of the Random class is based on Donald E. Knuth's subtractive random number generator algorithm.
/// For more information, see D. E. Knuth. "The Art of Computer Programming, volume 2: Seminumerical Algorithms". Addison-Wesley, Reading, MA, second edition, 1981.
/// </remarks>
public class RandomWrapper(Random random) : IRandom
    {
    public readonly static RandomWrapper Shared = new RandomWrapper(Random.Shared);

    public RandomWrapper() : this(new Random()) { }

    public RandomWrapper(int seed) : this(new Random(seed)) { }

    /// <summary>
    /// Generate a random double.
    /// </summary>
    /// <returns>A double [0..1)</returns>
    public double Sample() => random.NextDouble();
    }
