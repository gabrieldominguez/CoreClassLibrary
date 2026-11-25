using System.Numerics;

namespace CoreClassLibrary.Mathematics.Statistics.Sequence;

/// <summary>
/// Calcula la varianza de los valores del arreglo.
/// </summary>
/// <see cref="https://en.wikipedia.org/wiki/Standard_deviation"/>
public class VarianceSequenceStatistic<T> : ISequenceStatistic<T> where T : INumber<T>
    {
    public T Compute(IEnumerable<T> values)
        => values.ComputeMeanAndVariance().Variance;
    }
