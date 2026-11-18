using CoreClassLibrary.Objects.Ranges;
using System.Buffers;

namespace CoreClassLibrary.Extensions;

public static class ArrayExtensions
    {
    /// <summary>
    /// Crea un rango con los límites del arreglo.
    /// </summary>
    public static Range<int> GetBoundRange(this Array array, int dimension = 0)
        => new Range<int>(array.GetLowerBound(dimension), array.GetUpperBound(dimension));

    /// <summary>
    /// Crea una arreglo con los parametros.
    /// </summary>
    public static T[] CreateArray<T>(params T[] source)
        => source;

    public static void RentUseReturn<T>(this ArrayPool<T> arrayPool, int length, Action<T[]> action, bool clearArray = false)
        {
        T[] array = arrayPool.Rent(length);
        try
            {
            action(array);
            }
        finally
            {
            arrayPool.Return(array, clearArray);
            }
        }

    public static TResult RentUseReturn<T, TResult>(this ArrayPool<T> arrayPool, int length, Func<T[], TResult> func, bool clearArray = false)
        {
        T[] array = arrayPool.Rent(length);
        try
            {
            return func(array);
            }
        finally
            {
            arrayPool.Return(array, clearArray);
            }
        }

    /// <summary>
    /// Execute a transform delegate over the System.Array and then
    /// sets all elements in the System.Array to zero, to false, or to null, depending on the element type.
    /// </summary>
    /// <param name="array">The System.Array whose elements need to be cleared.</param>
    /// <param name="transform">The transform method to apply to the array.</param>
    public static T[] Clear<T>(this T[] array, Func<T[], T[]> transform)
        {
        T[] outArray = transform(array);
        array.Clear();
        return outArray;
        }

    /// <summary>
    /// Sets a range of elements in the Array to zero, to false, or to null, depending on the element type.
    /// </summary>
    /// <param name="array">The System.Array whose elements need to be cleared.</param>
    public static void Clear(this Array arr)
        => Array.Clear(arr, 0, arr.Length);

    public static T[] CloneArray<T>(this T[] array)
        => (T[]) array.Clone();

    public static IEnumerator<T> GetGenericEnumerator<T>(this T[] array)
        => Enumerable.AsEnumerable(array).GetEnumerator();
    }
