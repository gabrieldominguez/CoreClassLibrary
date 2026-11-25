namespace CoreClassLibrary.Mathematics.Randomness.Generators;

/// <summary>
/// Generador de números aleatorios por el método de congruencia aditiva.
/// </summary>
/// <remarks>
/// Método de congruencia aditiva.
/// Extraído del libro Algoritmos en C++ (Robert Sedgewick) - Cap. 35.
///
/// Parametros por Knuth:
/// Donald E. Knuth, The Art of Computer Programming, Volume 2, Section 3.2.1
/// </remarks>
public class AdditiveRandom : IRandom, IDiscreteRandom
    {
    private readonly uint T;
    private readonly long[] a;    // es long para que no de overflow la suma
    private readonly uint m;      // modulo
    private readonly uint b;
    private readonly uint c;

    private uint j;

    public AdditiveRandom() : this(TimeProvider.System) { }

    public AdditiveRandom(TimeProvider timeProvider) : this((uint)timeProvider.GetLocalNow().Ticks) { }

    public AdditiveRandom(uint seed, uint m = uint.MaxValue, uint t = 55, uint b = 23, uint c = 54): this(new CLinearRandom(seed), m, t, b, c) { }

    public AdditiveRandom(IDiscreteRandom random, uint m = uint.MaxValue, uint t = 55, uint b = 23, uint c = 54)
        {
        this.m = m;
        this.b = b;
        this.c = c;
        this.a = new long[T = t];

        for (j = 0; j < T; j++)
            {
            a[j] = random.DiscreteSample();
            }
        }

    public uint DiscreteSample()
        {
        j = (j + 1) % T;
        return (uint)(a[j] = (a[(j + b) % T] + a[(j + c) % T]) % m);
        }

    public double Sample() => (double)this.DiscreteSample() / m;
    }
