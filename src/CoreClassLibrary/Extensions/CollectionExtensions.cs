namespace CoreClassLibrary.Extensions;

/// <summary>
/// Extension methods for all kind of Collections implementing the ICollection&lt;T&gt; interface
/// </summary>
public static class CollectionExtensions
    {
    /// <summary>
    /// Adds a value uniquely to a collection and returns a value whether the value was added or not.
    /// </summary>
    /// <typeparam name="T">The generic collection value type</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="value">The value to be added.</param>
    /// <returns>Indicates whether the value was added or not</returns>
    /// <example><code>
    /// list.AddUnique(1); // returns true;
    /// list.AddUnique(1); // returns false the second time;
    /// </code></example>
    public static bool AddUnique<T>(this ICollection<T> collection, T value)
        {
        if (!collection.Contains(value))
            {
            collection.Add(value);
            return true;
            }

        return false;
        }

    /// <summary>
    /// Adds a range of value uniquely to a collection and returns the amount of values added.
    /// </summary>
    /// <typeparam name="T">The generic collection value type.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="values">The values to be added.</param>
    /// <returns>The amount if values that were added.</returns>
    public static int AddRangeUnique<T>(this ICollection<T> collection, IEnumerable<T> values)
        => values.Count(collection.AddUnique);

    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> values)
        {
        foreach (T value in values)
            {
            collection.Add(value);
            }
        }

    /// <summary>
    /// Retorna true si el conjunto innerGroup esta contenido dentro de outerGroup
    /// </summary>
    /// <remarks>innerGroup puede contener items repetidos.</remarks>
    public static bool ContainsDistinct<T>(this ICollection<T> outerGroup, ICollection<T> innerGroup)
        => CollectionExtensions.Contains(outerGroup, innerGroup.Distinct().ToList());

    /// <summary>
    /// Retorna true si el conjunto innerGroup esta contenido dentro de outerGroup
    /// </summary>
    /// <remarks>innerGroup NO debe contener items repetidos.</remarks>
    public static bool Contains<T>(this ICollection<T> outerGroup, ICollection<T> innerGroup)
        => outerGroup.Intersect(innerGroup).Count() == innerGroup.Count;

    /// <summary>
    /// Retorna los elementos que pertenecen a alguno de los conjuntos iniciales, sin pertenecer a ambos a la vez.
    /// </summary>
    /// <remarks>
    /// En teoría de conjuntos, la diferencia simétrica de dos conjuntos es una operación que resulta en otro conjunto
    /// cuyos elementos son aquellos que pertenecen a alguno de los conjuntos iniciales, sin pertenecer a ambos a la vez.
    /// </remarks>
    /// <see cref="https://es.wikipedia.org/wiki/Diferencia_sim%C3%A9trica"/>
    public static IEnumerable<T> SymmetricDifference<T>(this ICollection<T> groupA, ICollection<T> groupB)
        => groupA.Except(groupB).Union(groupB.Except(groupA));
    }
