namespace CoreClassLibrary.Mathematics.Randomness.Generators;

/// <summary>
/// Xorshift random number generators form a class of pseudorandom number generators that was discovered by George Marsaglia
/// </summary>
/// <remarks>
/// They are a subclass of linear feedback shift registers,
/// http://en.wikipedia.org/wiki/Xorshift
///
/// Numerical Recipes: The Art of Scientific Computing (3rd ed.). New York: Cambridge University Press. ISBN 978-0-521-88068-8.
/// </remarks>
/// <remarks>
/// XorshiftRandom constructor with 4 seeds
/// </remarks>
/// <remarks>
/// The only stipulation stated for the xorshift RNG is that at least one of
/// the seeds x,y,z,w is non-zero.
/// </remarks>
public class XorshiftRandom(uint x, uint y, uint z, uint w) : IDiscreteRandom
    {
    public XorshiftRandom() : this((uint)DateTime.Now.Ticks) { }

    /// <summary>
    /// XorshiftRandom constructor with 1 seed
    /// </summary>
    /// <remarks>
    /// The only stipulation stated for the xorshift RNG is that at least one of
    /// the seeds x,y,z,w is non-zero. We fulfill that requirement by only allowing
    /// setting of the x seed
    /// </remarks>
    public XorshiftRandom(uint seed) : this(seed, 362436069, 521288629, 88675123) { }

    /// <summary>
    /// Generates a uint. Values returned are over the full range of a uint (uint.MinValue to uint.MaxValue, inclusive).
    /// </summary>
    public uint DiscreteSample()
        {
        uint t = x ^ (x << 11);
        x = y; y = z; z = w;
        return w = w ^ (w >> 19) ^ t ^ (t >> 8);
        }
    }
