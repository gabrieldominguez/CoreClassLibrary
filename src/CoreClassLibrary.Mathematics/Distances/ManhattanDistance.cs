using CoreClassLibrary.Extensions;
using System.Numerics;

namespace CoreClassLibrary.Mathematics.Distances;

/// <summary>
/// Manhattan Distance
/// </summary>
/// <see cref="https://en.wikipedia.org/wiki/Taxicab_geometry"/>
/// <see cref="https://en.wikipedia.org/wiki/Minkowski_distance"/>
public class ManhattanDistance<T>: IDistance<T> where T : INumber<T>
    {
    public T Compute(IEnumerable<T> vector1, IEnumerable<T> vector2)
        => EnumerableExtensions.EnumerateInTuples(vector1, vector2)
            .Select(tuple => T.Abs(tuple.Item1 - tuple.Item2))
            .Sum();
    }
