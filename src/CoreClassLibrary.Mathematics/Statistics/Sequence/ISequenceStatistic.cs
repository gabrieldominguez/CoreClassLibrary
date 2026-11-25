using System.Numerics;

namespace CoreClassLibrary.Mathematics.Statistics.Sequence;

/// <summary>
/// Calcula una metrica estadística del arreglo.
/// </summary>
public interface ISequenceStatistic<T> where T : INumber<T>
    {
    T Compute(IEnumerable<T> values);
    }
