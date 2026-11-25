using System.Numerics;

namespace CoreClassLibrary.Mathematics.Statistics.Sequence;

/// <summary>
/// Calcula la desviacion estandar de los valores.
/// </summary>
/// <see cref="https://en.wikipedia.org/wiki/Standard_deviation"/>
public class StandardDeviationSequenceStatistic<T> : ISequenceStatistic<T> where T : INumber<T>, IRootFunctions<T>
    {
    public T Compute(IEnumerable<T> values)
        => values.ComputeMeanAndStandardDeviation().StandardDeviation;
    }
