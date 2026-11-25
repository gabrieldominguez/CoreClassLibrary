namespace CoreClassLibrary.Mathematics.Randomness.Generators;

/// <summary>
/// Marsaglia's Multiply With Carry (MWC) algorithm.
/// </summary>
/// <remarks>
/// http://en.wikipedia.org/wiki/Multiply-with-carry
/// http://www.codeproject.com/Articles/25172/Simple-Random-Number-Generation
/// </remarks>
public class MultiplyWithCarryRandom(uint w = 362436069, uint z = 521288629) : IDiscreteRandom
    {

    #region Constructors
    public MultiplyWithCarryRandom() : this(DateTime.Now.Ticks) { }

    public MultiplyWithCarryRandom(long seed): this((uint)(seed >> 16), (uint)(seed & uint.MaxValue)) { }
    #endregion

    /// <summary>
    /// Uses George Marsaglia's MWC algorithm to produce an unsigned integer.
    /// </summary>
    /// <returns>Random unsigned integer</returns>
    public uint DiscreteSample()
        => ((z = 36969 * (z & ushort.MaxValue) + (z >> 16)) << 16) + (w = 18000 * (w & ushort.MaxValue) + (w >> 16));
    }
