using CoreClassLibrary.Extensions;
using System.Numerics;

namespace CoreClassLibrary.Mathematics.Statistics.Sequence;

/// <summary>
/// Retorna rango de la secuencia (max - min).
/// </summary>
public class RangeSequenceStatistic<T> : ISequenceStatistic<T> where T : INumber<T>
    {
    public T Compute(IEnumerable<T> values)
        {
        (T min, T max) = values.MinMax();
        return max - min;
        }
    }
