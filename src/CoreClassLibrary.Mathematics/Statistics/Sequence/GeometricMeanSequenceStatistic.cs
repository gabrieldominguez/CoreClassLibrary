using System.Numerics;

namespace CoreClassLibrary.Mathematics.Statistics.Sequence;

/// <summary>
/// Calcula la media geometrica de los valores del arreglo.
/// </summary>
/// <see cref="https://en.wikipedia.org/wiki/Geometric_mean"/>
/// <see cref="https://en.wikipedia.org/wiki/Mean"/>
/// <see cref="https://en.wikipedia.org/wiki/Average"/>
public class GeometricMeanSequenceStatistic<T> : ISequenceStatistic<T> where T : INumber<T>, IPowerFunctions<T>
    {
    public T Compute(IEnumerable<T> values)
        {
        (int count, T prod) = values.CountAndProd<T>();
        return prod.Pow(T.MultiplicativeIdentity / T.CreateChecked(count));
        }
    }
