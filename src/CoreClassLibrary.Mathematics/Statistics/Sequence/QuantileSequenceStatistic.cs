using System.Numerics;

namespace CoreClassLibrary.Mathematics.Statistics.Sequence;

public class QuartileSequenceStatistic<T>(int n = 1) : QuantileSequenceStatistic<T>(4, n) where T : INumber<T>;

/// <summary>
/// Retorna el elemento del quantile especificado.
/// </summary>
/// <remarks>
/// El enumeracion es convertida a un arreglo y este es ordenado.
/// </remarks>
/// <see cref="https://en.wikipedia.org/wiki/Quantile"/>
/// <remarks>
/// Retorna el elemento del quantile especificado.
/// </remarks>
/// <param name="q">Cantidad de divisiones (quantiles).</param>
/// <param name="n">Numero de la division a retornar.</param>
public class QuantileSequenceStatistic<T>(int q = 2, int n = 1) : ArraySequenceStatistic<T> where T : INumber<T>
    {
    public override T Compute(T[] values)
        {
        Array.Sort(values);
        return values[n * values.Length / q];
        }
    }
