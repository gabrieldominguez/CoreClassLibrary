
using CoreClassLibrary.Objects.Ranges;
using System.Numerics;

namespace CoreClassLibrary.Mathematics;

public static class RangeExtensions
    {
    public static TSelf GetLength<TSelf>(this Range<TSelf> range) where TSelf : INumber<TSelf>
        => range.End - range.Start + TSelf.One; // +1 porque range.End está contenido en el rango

    /// <summary>
    /// Genera un valor que esta dentro del rango.
    /// </summary>
    /// <remarks>
    /// Si el rango empieza en 0, es equivalente al sig modulo aritmetico:
    ///
    ///  x % n => Range(0, n-1).Modulo(x)
    /// </remarks>
    public static TSelf Modulo<TSelf>(this Range<TSelf> range, TSelf value) where TSelf : INumber<TSelf>
        => range.Start + value % range.GetLength();
    }
