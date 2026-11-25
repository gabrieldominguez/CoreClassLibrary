using System.Numerics;

namespace CoreClassLibrary.Mathematics.Statistics.Sequence;

public class CubicMeanSequenceStatistic<T>() : GeneralizedMeanSequenceStatistic<T>(3) where T : INumber<T>, IPowerFunctions<T>;
public class QuadraticMeanSequenceStatistic<T>() : GeneralizedMeanSequenceStatistic<T>(2) where T : INumber<T>, IPowerFunctions<T>;

/// <summary>
/// Calcula la media generica de los valores del arreglo.
/// </summary>
/// <see cref="https://en.wikipedia.org/wiki/Generalized_mean"/>
/// <see cref="https://en.wikipedia.org/wiki/Generalized_mean"/>
/// <see cref="https://en.wikipedia.org/wiki/Geometric_mean"/>
/// <see cref="https://en.wikipedia.org/wiki/Mean"/>
/// <see cref="https://en.wikipedia.org/wiki/Average"/>
public class GeneralizedMeanSequenceStatistic<T>(int p) : ISequenceStatistic<T> where T : INumber<T>, IPowerFunctions<T>
    {
    public T Compute(IEnumerable<T> values)
        {
        (int count, T sum) = values.Select(x => x.Pow(p)).CountAndSum<T>();
        return (sum / T.CreateChecked(count)).Pow(T.MultiplicativeIdentity / T.CreateChecked(p));
        }
    }
