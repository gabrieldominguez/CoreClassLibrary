using CoreClassLibrary.Extensions;
using System.Text;

namespace CoreClassLibrary.Extensions;

public static class ParallelQueryExtensions
    {
    #region All & Any
    /// <summary>
    /// Idem Enumerable.All() pero permite pasar el indice del elemento al predicado.
    /// </summary>
    public static bool All<TSource>(this ParallelQuery<TSource> source, Func<TSource, int, bool> predicate)
        => source.Select(predicate).All(b => b);

    /// <summary>
    /// Idem Enumerable.Any() pero permite pasar el indice del elemento al predicado.
    /// </summary>
    public static bool Any<TSource>(this ParallelQuery<TSource> source, Func<TSource, int, bool> predicate)
        => source.Select(predicate).Any(b => b);
    #endregion

    #region Do
    /// <summary>
    /// Realiza una acción para cada elemento de la lista que es retornado.
    /// </summary>
    /// <remarks>
    /// La acción es ejecutada ANTES de retornar el elemento.
    /// </remarks>
    /// <see cref="Observable.Do"/>
    public static ParallelQuery<T> Do<T>(this ParallelQuery<T> source, Action<T> action)
        => source.Select(item =>
            {
            action(item);
            return item;
            });
    #endregion

    #region Contains
    /// <summary>
    /// Retorna true si el conjunto innerGroup esta contenido dentro de outerGroup
    /// </summary>
    /// <remarks>innerGroup puede contener items repetidos.</remarks>
    public static bool ContainsDistinct<T>(this ParallelQuery<T> outerGroup, ParallelQuery<T> innerGroup)
        => ParallelQueryExtensions.Contains(outerGroup, innerGroup.Distinct());

    /// <summary>
    /// Retorna true si el conjunto innerGroup esta contenido dentro de outerGroup
    /// </summary>
    /// <remarks>innerGroup NO debe contener items repetidos.</remarks>
    public static bool Contains<T>(this ParallelQuery<T> outerGroup, ParallelQuery<T> innerGroup)
        => outerGroup.Intersect(innerGroup).Count() == innerGroup.Count();
    #endregion

    #region ToDelimitedString
    /// <summary>
    /// Convierte una lista en un string representando el contenido.
    /// </summary>
    /// <param name="source">IEnumerable a convertir.</param>
    /// <param name="separator">Cadena a usar de separador de cada elemento.</param>
    /// <returns>String representando el contenido de la lista</returns>
    /// <see cref="http://stackoverflow.com/questions/20469374/plinq-aggregate-extension-with-stringbuilder"/>
    public static string ToDelimitedString(this ParallelQuery<string> source, Func<StringBuilder> stringBuilderFactory, string separator = ", ")
        => source.Aggregate(
            stringBuilderFactory,                                                  // initialize thread (itemStringBuilder)
            (itemStringBuilder, item) => itemStringBuilder.Append(item),                // do this on each thread
            (totalStringBuilder, itemStringBuilder) =>                                  // aggregate results after all threads are done.
            {
                if (totalStringBuilder.Length > 0)
                    {
                    totalStringBuilder.Append(separator);
                    }

                return totalStringBuilder.Append(itemStringBuilder);
            },
            stringBuilder => stringBuilder.ToString()                                   // perform on the aggregated result.
            );
    #endregion

    #region Repeat
    /// <summary>
    /// Ejecuta una funcion una cantidad de veces y enumera los resultados.
    /// </summary>
    public static ParallelQuery<T> Repeat<T>(Func<T> func, int count)
        => ParallelQueryExtensions.Repeat(i => func(), count);

    /// <summary>
    /// Ejecuta una funcion una cantidad de veces pasandole por parametro el indice y enumera los resultados.
    /// </summary>
    /// <remarks>El indice comienza en 0.</remarks>
    public static ParallelQuery<T> Repeat<T>(Func<int, T> func, int count)
        => ParallelEnumerable.Range(0, count).Select(func);
    #endregion
    }
