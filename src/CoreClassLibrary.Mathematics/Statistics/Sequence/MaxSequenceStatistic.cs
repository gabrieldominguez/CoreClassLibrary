using System.Numerics;

namespace CoreClassLibrary.Mathematics.Statistics.Sequence;

/// <summary>
/// Retorna el valor máximo de la secuencia.
/// </summary>
/// <see cref="https://en.wikipedia.org/wiki/Mode_(statistics)"/>
public class MaxSequenceStatistic<T> : ISequenceStatistic<T> where T : INumber<T>
    {
    public T Compute(IEnumerable<T> values)
        => values.Max() ?? throw new InvalidOperationException("Sequence contains no elements");
    }
