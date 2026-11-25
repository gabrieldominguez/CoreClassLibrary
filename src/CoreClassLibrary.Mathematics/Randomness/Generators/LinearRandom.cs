namespace CoreClassLibrary.Mathematics.Randomness.Generators;

#region Sedgewick
/// <summary>
/// Generador de números aleatorios por el método de congruencia lineal (D.Lehmer (1951)).
/// </summary>
/// <remarks>
/// Extraído del libro Algoritmos en C++ (Robert Sedgewick) - Cap. 35.
/// </remarks>
public class SedgewickLinearRandom : LinearRandom
    {
    private const int a = 31415821;
    private const int c = 1;
    private const uint m = 100000000;

    public SedgewickLinearRandom() : base(a, c, m) { }
    public SedgewickLinearRandom(uint seed) : base(seed, a, c, m) { }
    }
#endregion

#region Kernighan & Ritchie
/// <summary>
/// Generador de números aleatorios por el método de congruencia lineal.
/// </summary>
/// <remarks>
/// Parametros correspondientes a la biblioteca estandar de C,
/// extraídos del libro Kernighan & Ritchie - The C Programming Language
/// </remarks>
public class CLinearRandom : LinearRandom
    {
    private const int a = 1103515245;
    private const int c = 12345;
    private const uint m = 1U << 31;

    public CLinearRandom() : base(a, c, m) { }
    public CLinearRandom(uint seed) : base(seed, a, c, m) { }
    }
#endregion

/// <summary>
/// Generador de números aleatorios por el método de congruencia lineal.
/// </summary>
/// <remarks>
/// Método de congruencia lineal de D.Lehmer (1951).
/// Extraído del libro Algoritmos en C++ (Robert Sedgewick) - Cap. 35.
///
/// http://en.wikipedia.org/wiki/Linear_congruential_generator
///
/// Si c=0 entonces se denomina
/// http://en.wikipedia.org/wiki/Park%E2%80%93Miller_random_number_generator
///
/// Ver tambien el libro de Donald E. Knuth, The Art of Computer Programming, Volume 2, Section 3.2.1
/// </remarks>
/// <remarks>
/// Constructor.
/// </remarks>
/// <param name="seed">Semilla del generador.</param>
/// <param name="a">Constante multiplicadora.</param>
/// <param name="c">Constante sumadora.</param>
/// <param name="m">Modulo. (Valor maximo no incluido retornado por DiscreteSample)</param>
public class LinearRandom(uint seed, int a, int c, uint m) : IRandom, IDiscreteRandom
    {
    private long seed = seed; // es long para evitar el overflow al multiplicar por a      

    public LinearRandom(int a, int c) : this(a, c, uint.MaxValue) { }

    public LinearRandom(int a, int c, uint m) : this((uint)DateTime.Now.Ticks, a, c, m) { }

    public LinearRandom(uint seed, int a, int c): this(seed, a, c, uint.MaxValue) { }

    /// <summary>
    /// Retorna un entero entre [0..m)
    /// </summary>
    public uint DiscreteSample() => (uint)(seed = (seed * a + c) % m);

    /// <summary>
    /// Retorna un double entre [0..1)
    /// </summary>
    public double Sample() => (double)this.DiscreteSample() / m;
    }
