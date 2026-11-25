using CoreClassLibrary.Extensions;
using System.Numerics;

namespace CoreClassLibrary.Mathematics.Statistics.Sequence;

/// <summary>
/// Para operaciones que necesitan enumerar la secuencia más de una vez, se proporciona un arreglo.
/// </summary>
public abstract class ArraySequenceStatistic<T>: ISequenceStatistic<T> where T : INumber<T>
    {
    public T Compute(IEnumerable<T> values)
        => this.Compute(values.AsArray());

    public abstract T Compute(T[] values);
    }
