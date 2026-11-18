using CoreClassLibrary.Objects.Ranges;
using System.Collections.ObjectModel;

namespace CoreClassLibrary.Extensions;

public static class ListExtensions
    {
    /// <summary>
    /// Quita los ultimos elementos de la lista.
    /// </summary>
    public static void RemoveLast<T>(this List<T> list, int count = 1)
        => list.RemoveRange(list.Count - count, count);            

    /// <summary>
    /// Obtiene el ultimo item.
    /// </summary>
    public static T GetLast<T>(this IReadOnlyList<T> list)
        => list[^1];

    /// <summary>
    /// Retorna el elemento o default si el indice esta fuera del rango del span.
    /// </summary>
    public static T? GetValueOrDefault<T>(this ReadOnlySpan<T> span, int i)
        => 0 <= i && i < span.Length ? span[i] : default;

    /// <summary>
    /// Retorna el elemento o default si el indice esta fuera del rango de la lista.
    /// </summary>
    public static T? GetValueOrDefault<T>(this IReadOnlyList<T> list, int i)
        => 0 <= i && i < list.Count ? list[i] : default;

    public static ReadOnlyCollection<T> AsReadOnly<T>(this IList<T> list)
        => new ReadOnlyCollection<T>(list);

    /// <summary>
    /// Spread an enumerable into another array.
    /// </summary>
    /// <example>
    /// new List<int> { 1, 2, GetAnotherListOfInts(), 3}
    /// </example>
    /// <see cref="https://mariusgundersen.net/duck-extensions/"/>
    public static void Add<T>(this List<T> list, IEnumerable<T> items)
        => list.AddRange(items);

    /// <summary>
    /// Crea un rango desde 0 a la longitud - 1 del arreglo.
    /// </summary>
    public static Range<int> GetLengthRange<T>(this IReadOnlyList<T> list)
        => new Range<int>(0, list.Count-1);
    }
