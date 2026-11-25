
using System.Numerics;

namespace CoreClassLibrary.Mathematics.Distances;

/// <summary>
/// Calcula la distancia entre dos vectores.
/// </summary>
public interface IDistance<T> where T : INumber<T>
    {
    T Compute(IEnumerable<T> vector1, IEnumerable<T> vector2);
    }
