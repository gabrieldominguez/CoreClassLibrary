
using CoreClassLibrary.Mathematics.Randomness;

namespace CoreClassLibrary.Mathematics.Tests;

/// <summary>
/// Retorna los valores pasados en el constructor.
/// </summary>
/// <remarks>
/// Para hacer pruebas repetibles.
/// Una vez que se llego al último valor, vuelve al primero.
/// </remarks>
public class TestDiscreteRandom(CircularProvider<uint> circularProvider) : IDiscreteRandom
    {
    public TestDiscreteRandom(IReadOnlyList<uint> samples): this(new CircularProvider<uint>(samples)) { }

    public uint DiscreteSample()
        => circularProvider.GetNext();
    }

/// <summary>
/// Retorna los valores pasados en el constructor.
/// </summary>
/// <remarks>
/// Los valores pasados deben pertenecer al rango [0..1).
/// Para hacer pruebas repetibles.
/// Una vez que se llego al último valor, vuelve al primero.
/// </remarks>
public class TestRandom(CircularProvider<double> circularProvider) : IRandom
    {
    public TestRandom(IReadOnlyList<double> samples): this(new CircularProvider<double>(samples)) { }

    public double Sample()
        => circularProvider.GetNext();
    }

/// <summary>
/// Hace que NextInt(maxValue) retorne los valores pasados en el constructor.
/// </summary>
/// <remarks>
/// Para hacer pruebas repetibles.
/// Una vez que se llego al último valor, vuelve al primero.
/// </remarks>
public class TestIntRandom(IEnumerable<int> ints, int maxValue) : TestRandom(ints.Select(i => (double)i / maxValue).ToArray()), IRandom;
