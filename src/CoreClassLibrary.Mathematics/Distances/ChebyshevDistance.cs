using CoreClassLibrary.Extensions;
using System.Numerics;

namespace CoreClassLibrary.Mathematics.Distances;

/// <summary>
/// Chebyshev Distance
/// </summary>
/// <see cref="https://en.wikipedia.org/wiki/Chebyshev_distance"/>
public class ChebyshevDistance<T>: IDistance<T> where T : INumber<T>
    {
    public T Compute(IEnumerable<T> vector1, IEnumerable<T> vector2)
        => EnumerableExtensions.EnumerateInTuples(vector1, vector2)
            .Max(tuple => T.Abs(tuple.Item1 - tuple.Item2)) ?? T.AdditiveIdentity;
    }
