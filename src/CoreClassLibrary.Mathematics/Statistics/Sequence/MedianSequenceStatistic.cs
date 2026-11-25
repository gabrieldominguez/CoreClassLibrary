
using System.Numerics;

namespace CoreClassLibrary.Mathematics.Statistics.Sequence;

/// <summary>
/// Calcula la mediana de los valores del arreglo.
/// </summary>
/// <see cref="https://en.wikipedia.org/wiki/Median"/>
public class MedianSequenceStatistic<T> : ArraySequenceStatistic<T> where T : INumber<T>
    {
    public override T Compute(T[] values)
        {
        if (values.Length.IsEven())
            {
            int upperMiddle = values.Length / 2;
            Array.Sort(values);
            return (values[upperMiddle - 1] + values[upperMiddle]) / T.CreateChecked(2);
            }
        else
            { 
            return new QuantileSequenceStatistic<T>(2, 1).Compute(values);
            }
        }
    }
