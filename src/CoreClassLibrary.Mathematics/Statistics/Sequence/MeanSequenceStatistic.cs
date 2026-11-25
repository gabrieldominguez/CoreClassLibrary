using System.Numerics;

namespace CoreClassLibrary.Mathematics.Statistics.Sequence;

//public class MeanSequenceStatistic() : GeneralizedMeanSequenceStatistic(1);

/// <summary>
/// Calcula la media de los valores del arreglo.
/// </summary>
/// <see cref="https://en.wikipedia.org/wiki/Mean"/>
/// <see cref="https://en.wikipedia.org/wiki/Average"/>
public class MeanSequenceStatistic<T> : ISequenceStatistic<T> where T : INumber<T>
    {
    public T Compute(IEnumerable<T> values) => values.Mean();
    }
