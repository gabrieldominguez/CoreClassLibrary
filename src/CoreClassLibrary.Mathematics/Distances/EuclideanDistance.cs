using CoreClassLibrary.Extensions;
using System.Numerics;

namespace CoreClassLibrary.Mathematics.Distances;

/// <summary>
/// Euclidean Distance
/// </summary>
/// <see cref="https://en.wikipedia.org/wiki/Euclidean_distance"/>
/// <see cref="Maths.Distance"/>
public class EuclideanDistance<T>: IDistance<T> where T : INumber<T>, IRootFunctions<T>
    {
    public T Compute(IEnumerable<T> vector1, IEnumerable<T> vector2)
        => EuclideanDistance<T>.ComputeDistance(vector1, vector2);

    /// <summary>
    /// Vector Distance Funcion (0,0 - x,y)
    /// </summary>
    public static T ComputeDistance(T[] vector)
        => T.Sqrt(vector.Select(Maths.Pow2).Sum());
        //=> MathHelper.Distance(vector, Enumerable.Repeat(0.0, vector.Length).ToArray());

    public static T ComputeDistance(IEnumerable<T> vector1, IEnumerable<T> vector2)
        => T.Sqrt(EnumerableExtensions
            .EnumerateInTuples(vector1, vector2)
            .Select(tuple => (tuple.Item1 - tuple.Item2).Pow2())
            .Sum());
    }
