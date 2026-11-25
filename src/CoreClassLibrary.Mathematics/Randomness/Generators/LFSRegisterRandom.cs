namespace CoreClassLibrary.Mathematics.Randomness.Generators;

/// <summary>
/// Galois Linear_feedback_shift_register
/// </summary>
/// <remarks>
/// 16 bits implementation.
/// http://en.wikipedia.org/wiki/Linear_feedback_shift_register
/// </remarks>
public class GaloisLFSRegisterRandom : LFSRegisterRandom
    {
    public GaloisLFSRegisterRandom() : base(16) { }

    public GaloisLFSRegisterRandom(uint seed) : base(seed, 16) { }

    public override uint DiscreteSample()
        {
        uint lsb = reg & 1;     /* Get lsb (i.e., the output bit). */
        reg >>= 1;               /* Shift register */
        if (lsb == 1)             /* Only apply toggle mask if output bit is 1. */
            {
            reg ^= 0xB400u;        /* Apply toggle mask, value has 1 at bits corresponding to taps, 0 elsewhere. */
            }
        return reg;
        }
    }

public class FibonacciLFSRegisterRandom : LFSRegisterRandom
    {
    public FibonacciLFSRegisterRandom(int n = 32) : base(n) { }

    public FibonacciLFSRegisterRandom(uint seed, int n = 32): base(seed, n) { }

    public override uint DiscreteSample() => reg = (((reg ^ (reg >> 2) ^ (reg >> 3) ^ (reg >> 5)) & 1) << (N - 1)) | (reg >> 1);
    }

/// <summary>
/// Generador de números aleatorios por el método de congruencia aditiva sobre un registro de retroalimentación lineal (Linear feedback shift register).
/// </summary>
/// <remarks>
/// Método de congruencia aditiva sobre un registro de retroalimentación.
/// Extraído del libro Algoritmos en C++ (Robert Sedgewick) - Cap. 35.
///
/// http://en.wikipedia.org/wiki/Linear_feedback_shift_register
/// </remarks>
public class LFSRegisterRandom : IRandom, IDiscreteRandom
    {
    public readonly int N;
    public readonly uint MaxValue;

    protected uint reg;

    public LFSRegisterRandom(int n = 32) : this((uint)DateTime.Now.Ticks, n) { }

    /// <summary>
    /// Construye el generador.
    /// </summary>
    /// <remarks>
    /// Para n menor a 30, el generador casi nunca pasará la prueba de chi-cuadrado.
    /// </remarks>
    /// <param name="seed">Semilla.</param>
    /// <param name="n">Cantidad de bits del registro.</param>
    public LFSRegisterRandom(uint seed, int n = 32)
        {
        if(n > 32)
            {
            throw new ArgumentOutOfRangeException(nameof(n), "Debe ser menor o igual a 32.");
            }
        if (seed == 0)
            {
            throw new ArgumentOutOfRangeException(nameof(seed), "La semilla no puede ser 0.");
            }

        this.reg = seed & (this.MaxValue = (uint)((1UL << (this.N = n)) - 1));
        }


    /// <summary>
    /// Retorna un numero en el rango [1, MaxValue]
    /// </summary>
    /// <remarks>Nunca retorna 0.</remarks>
    public virtual uint DiscreteSample() => reg = (((reg ^ (reg >> 1)) & 1) << (N - 1)) | (reg >> 1);

    /// <summary>
    /// Retorna un numero en el rango [0, 1)
    /// </summary>
    /// <remarks>
    /// Resta 1 al valor retornado por DiscreteSample, para incluir el 0 y excluir el 1.
    /// </remarks>
    public virtual double Sample() => (double)(this.DiscreteSample() - 1) / this.MaxValue;
    }
