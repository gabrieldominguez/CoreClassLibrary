#pragma warning disable IDE0028 // Simplify collection initialization
#pragma warning disable IDE0305 // Simplify collection initialization
#pragma warning disable IDE0306 // Simplify collection initialization


using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Pooling;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace CoreClassLibrary.Extensions;

/// <summary>
/// Extensions to LINQ to Objects
/// </summary>
/// <see cref="https://github.com/morelinq/MoreLINQ"/>
/// <see cref="https://morelinq.github.io/"/>
public static class EnumerableExtensions
    {
    public static IEnumerable<TSource> WhereNotNull<TSource>(this IEnumerable<TSource?> source)
        => source.Where(element => element != null).Select(element => element!);

    public static Dictionary<TKey, IEnumerable<TElement>> ToDictionary<TKey, TElement>(this IEnumerable<IGrouping<TKey, TElement>> source) where TKey: notnull
        => source.ToDictionary(
            group => group.Key, 
            group => (IEnumerable<TElement>)group
            );

    public static Dictionary<TKey, TElement[]> ToArrayDictionary<TKey, TElement>(this IEnumerable<IGrouping<TKey, TElement>> source) where TKey: notnull
        => source.ToDictionary(
            group => group.Key, 
            group => group.ToArray()
            );

    public static TAccumulate Aggregate<TSource, TContext, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, TContext context, Func<TAccumulate, TSource, TContext, TAccumulate> func)
        {
        TAccumulate accumulate = seed;

        foreach (TSource element in source)
            {
            accumulate = func(accumulate, element, context);
            }

        return accumulate;
        }

    public static IEnumerable<TResult> Select<TSource, TContext, TResult>(this IEnumerable<TSource> source, TContext context, Func<TSource, TContext, TResult> selector)
        {
        foreach (TSource element in source)
            {
            yield return selector(element, context);
            }
        }

    public static IEnumerable<TResult> SelectMany<TSource, TContext, TResult>(this IEnumerable<TSource> source, TContext context, Func<TSource, TContext, IEnumerable<TResult>> selector)
        {
        foreach (TSource element in source)
            {
            foreach (TResult subItem in selector(element, context))
                {
                yield return subItem;
                }
            }
        }

    public static IEnumerable<TSource> Where<TSource, TContext>(this IEnumerable<TSource> source, TContext context, Func<TSource, TContext, bool> predicate)
        {
        foreach (TSource element in source)
            {
            if (predicate(element, context))
                {
                yield return element;
                }
            }
        }

    public static bool Any<TSource, TContext>(this IEnumerable<TSource> source, TContext context, Func<TSource, TContext, bool> predicate)
        {
        foreach (TSource element in source)
            {
            if(predicate(element, context))
                {
                return true;
                }
            }
        return false;
        }

    public static bool All<TSource, TContext>(this IEnumerable<TSource> source, TContext context, Func<TSource, TContext, bool> predicate)
        {
        foreach (TSource element in source)
            {
            if (!predicate(element, context))
                {
                return false;
                }
            }
        return true;
        }

    // Usar Enumerable.Append
    // public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> source, TSource element) => source.Concat(element.AsEnumerable());

    /// <summary>
    /// Returns all distinct elements of the given source, where "distinctness"
    /// is determined via a projection and the specified comparer for the projected type.
    /// </summary>
    /// <remarks>
    /// This operator uses deferred execution and streams the results, although
    /// a set of already-seen keys is retained. If a key is seen multiple times,
    /// only the first element with that key is returned.
    /// </remarks>
    /// <typeparam name="TSource">Type of the source sequence</typeparam>
    /// <typeparam name="TKey">Type of the projected element</typeparam>
    /// <context name="source">Source sequence</context>
    /// <context name="keySelector">Projection for determining "distinctness"</context>
    /// <context name="comparer">The equality comparer to use to determine whether or not keys are equal.
    /// If null, the default equality comparer for <c>TSource</c> is used.</context>
    /// <returns>A sequence consisting of distinct elements from the source sequence,
    /// comparing them by the specified key projection.</returns>
    /// <see cref="https://github.com/morelinq/MoreLINQ/blob/master/MoreLinq/DistinctBy.cs"/>
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer = null)
        {
        HashSet<TKey> knownKeys = new HashSet<TKey>(comparer);
        foreach (TSource element in source)
            {
            if (knownKeys.Add(keySelector(element)))
                {
                yield return element;
                }
            }
        }

    /// <summary>
    /// Emite un separador antes de cada element de la secuencia.
    /// </summary>
    public static IEnumerable<T> PreInterleave<T>(this IEnumerable<T> source, T separator)
        => source.SelectMany(separator, (element, sep) => (IEnumerable<T>)[ sep, element ]);

    /// <summary>
    /// Intercala un separator entre cada element de la secuencia.
    /// </summary>
    public static IEnumerable<T> Interleave<T>(this IEnumerable<T> source, T separator)
        {
        bool isFirst = true;

        foreach (T element in source)
            {
            if (isFirst)
                {
                isFirst = false;
                }
            else
                {
                yield return separator;
                }

            yield return element;
            }
        }

    /// <summary>
    /// Emite un separador luego de cada element de la secuencia.
    /// </summary>
    public static IEnumerable<T> PostInterleave<T>(this IEnumerable<T> source, T separator)
        => source.SelectMany(separator, EnumerableExtensions.AsEnumerable);

    public static void Dispose(this IEnumerable<IDisposable> disposables)
        {
        foreach (IDisposable disposable in disposables)
            {
            disposable.Dispose();
            }
        }

    /// <summary>
    /// Intercala las secuencias.
    /// </summary>
    /// <remarks>La secuencia termina cuando termina alguna de las pasadas por parámetro.</remarks>
    public static IEnumerable<T> InterleaveAll<T>(IEnumerable<IEnumerable<T>> sources)
        {
        // NullReferenceException
        // using PooledArray<IEnumerator<T>> enumerators = new PooledArray<IEnumerator<T>>(sources.Count);   // IReadOnlyCollection sources
        //sources.Select(s => s.GetEnumerator()).WriteTo(enumerators);
        IEnumerator<T>[] enumerators = sources.Select(s => s.GetEnumerator()).ToArray();
        try
            {
            while (enumerators.All(e => e.MoveNext()))
                {
                foreach (IEnumerator<T> enumerator in enumerators)
                    {
                    yield return enumerator.Current;
                    }
                }
            }
        finally
            {
            enumerators.Dispose();
            }
        }

    /// <summary>
    /// Intercala las secuencias.
    /// </summary>
    /// <remarks>La secuencia termina cuando terminan todas las pasadas por parámetro.</remarks>
    public static IEnumerable<T> InterleaveAny<T>(IEnumerable<IEnumerable<T>> sources)
        {
        // NullReferenceException
        //using PooledArray<IEnumerator<T>> enumerators = new PooledArray<IEnumerator<T>>(sources.Count);   // IReadOnlyCollection sources
        //sources.Select(s => s.GetEnumerator()).WriteTo(enumerators.Array);
        IEnumerator<T>[] enumerators = sources.Select(s => s.GetEnumerator()).ToArray();

        try
            {
            bool any;
            do
                {
                any = false;
                foreach (IEnumerator<T> enumerator in enumerators)
                    {
                    if (enumerator.MoveNext())
                        {
                        any = true;
                        yield return enumerator.Current;
                        }
                    }
                }
            while (any);
            }
        finally
            {
            enumerators.Dispose();
            }
        }

    /// <summary>
    /// Ejecuta una funcion una cantidad de veces y enumera los resultados.
    /// </summary>
    public static IEnumerable<T> Repeat<T>(Func<T> func, int count)
        {
        for (int i = 0; i < count; i++)
            {
            yield return func();
            }
        }

    /// <summary>
    /// Ejecuta una funcion una cantidad de veces pasandole por parametro el indice y enumera los resultados.
    /// </summary>
    /// <remarks>El indice comienza en 0.</remarks>
    public static IEnumerable<T> Repeat<T>(Func<int, T> func, int count)
        => Enumerable.Range(0, count).Select(func);

    /// <summary>
    /// Genera una secuencia utilizando una funcion a la que se le pasa el indice del elemento.
    /// </summary>
    /// <remarks>
    /// Generar hasta int.MaxValue o se producirá un excepción.
    /// </remarks>
    public static IEnumerable<TResult> RepeatWhile<TResult>(Func<int, TResult> generator, Func<int, bool> predicate)
        {
        int i = 0;
        while (predicate(i))
            {
            yield return generator(i++);
            }
        }

    /// <summary>
    /// Genera una secuencia utilizando una funcion.
    /// </summary>
    public static IEnumerable<TResult> RepeatWhile<TResult>(Func<TResult> generator, Func<bool> predicate)
        {
        while (predicate())
            {
            yield return generator();
            }
        }

    /// <summary>
    /// Genera una secuencia utilizando un valor inicial y una funcion a la que se le pasa el valor actual.
    /// </summary>
    public static IEnumerable<TResult> RepeatWhile<TResult>(TResult initial, Func<TResult, TResult> generator, Func<TResult, bool> predicate)
        {
        TResult current = initial;
        while (predicate(current))
            {
            yield return current;
            current = generator(current);
            }
        }

    /// <summary>
    /// Replica cada element de la secuencia de entrada
    /// </summary>
    public static IEnumerable<T> Replicate<T>(this IEnumerable<T> source, int count = 1)
        => EnumerableExtensions.Replicate(source, i => i, count);

    /// <summary>
    /// Replica cada element de la secuencia de entrada, permitiendo aplicar una funcion para obtener la copia.
    /// </summary>
    public static IEnumerable<T> Replicate<T>(this IEnumerable<T> source, Func<T, T> factory, int count = 1)
    //=> source.SelectMany(element => EnumerableExtensions.AsEnumerable(element).Concat(EnumerableExtensions.Repeat(() => factory(element), count)));
        {
        foreach (T element in source)
            {
            yield return element;

            for (int i = 0; i < count; i++)
                {
                yield return factory(element);
                }
            }
        }


    /// <summary>
    /// Genera una secuencia de números enteros en un intervalo especificado.
    /// </summary>
    /// <context name="from">Valor del primer entero de la secuencia.</context>
    /// <context name="to">Valor del último entero de la secuencia.</context>
    /// <returns>Secuencia generada.</returns>
    /// <exception cref="ArgumentOutOfRangeException">to es menor que from.</exception>
    public static IEnumerable<int> RangeFromTo(int from, int to)
        => Enumerable.Range(from, to - from + 1);

    /// <summary>
    /// Genera una serie (progresión o sucesión) aritmético-geométrica.
    /// </summary>
    /// <context name="a">Valor del primer entero de la secuencia.</context>
    /// <context name="r">Factor geométrico o razón.</context>
    /// <context name="d">Factor aritmético o diferencia.</context>
    /// <returns>Secuencia generada.</returns>
    /// <see cref="https://www.matesfacil.com/ESO/progresiones/ejercicios-resueltos-sucesiones.html"/>
    public static IEnumerable<int> Series(int a = 0, int r = 1, int d = 1, CancellationToken cancellationToken = default)
    //=> EnumerableExtensions.Series(n => r * n + d, a, cancellationToken);
        {
        while(!cancellationToken.IsCancellationRequested)
            {
            yield return a;
            a = r * a + d;
            }
        }

    /// <summary>
    /// Genera una serie (progresión o sucesión) aritmético-geométrica.
    /// </summary>
    /// <context name="next">Función que dado un valor retorna el próximo.</context>
    /// <context name="a">Valor del primer entero de la secuencia.</context>
    /// <returns>Secuencia generada.</returns>
    /// <see cref="https://vimeo.com/131409651"/>
    /// <see cref="https://vimeo.com/213571882"/>
    public static IEnumerable<int> Series(Func<int, int> next, int a = 0, CancellationToken cancellationToken = default)
        {
        while (!cancellationToken.IsCancellationRequested)
            {
            yield return a;
            a = next(a);
            }
        }

    /// <summary>
    /// Recorre las enumeraciones coordinadamente, enumerando arreglos con los items de cada una.
    /// </summary>
    /// <remarks>
    /// La enumeracion termina cuando alguno de los enumerables termina.
    /// </remarks>
    public static IEnumerable<T[]> EnumerateInArrays<T>(IEnumerable<T> source1, IEnumerable<T> source2)
        {
        using IEnumerator<T> enumerator1 = source1.GetEnumerator();
        using IEnumerator<T> enumerator2 = source2.GetEnumerator();
        while (enumerator1.MoveNext() && enumerator2.MoveNext())
            {
            yield return new T[] { enumerator1.Current, enumerator2.Current };
            }
        }

    /// <summary>
    /// Recorre las enumeraciones coordinadamente, enumerando tuplas con los items de cada una.
    /// </summary>
    /// <remarks>
    /// La enumeracion termina cuando alguno de los enumerables termina.
    /// </remarks>
    public static IEnumerable<(T1, T2)> EnumerateInTuples<T1, T2>(IEnumerable<T1> source1, IEnumerable<T2> source2)
        {
        using IEnumerator<T1> enumerator1 = source1.GetEnumerator();
        using IEnumerator<T2> enumerator2 = source2.GetEnumerator();
        while (enumerator1.MoveNext() && enumerator2.MoveNext())
            {
            yield return (enumerator1.Current, enumerator2.Current);
            }
        }

    /// <summary>
    /// Recorre las enumeraciones coordinadamente, enumerando tuplas con los items de cada una.
    /// </summary>
    /// <remarks>
    /// Cuando una de las enumeraciones termina se devuelve default en su lugar de la tupla.
    /// </remarks>
    public static IEnumerable<(T1?, T2?)> EnumerateInTuplesWithDefault<T1, T2>(IEnumerable<T1> source1, IEnumerable<T2> source2)
        {
        using IEnumerator<T1> enumerator1 = source1.GetEnumerator();
        using IEnumerator<T2> enumerator2 = source2.GetEnumerator();
        bool hasNext1 = enumerator1.MoveNext();
        bool hasNext2 = enumerator2.MoveNext();

        while (hasNext1 || hasNext2)
            {
            yield return (hasNext1 ? enumerator1.Current : default, hasNext2 ? enumerator2.Current : default);

            hasNext1 = enumerator1.MoveNext();
            hasNext2 = enumerator2.MoveNext();
            }
        }

    // Usar SequenceEqual
    // System.Linq.Enumerable.SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
    // System.Linq.Enumerable.SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)


    /*
    // Solo para secuencias con igual cantidad de elementos
    public static bool AreEquals<T>(IEnumerable<T> source1, IEnumerable<T> source2) where T : IEquatable<T>
        {
        EnumerableExtensions
            .EnumerateInTuples(source1, source2)
            .All(tuple => tuple.Item1.Equals(tuple.Item2));
        }
    */
    /*
    public static bool EqualTo<T>(this IEnumerable<T> source1, IEnumerable<T> source2) // where T : IEquatable<T> 
        => EnumerableExtensions.AreEquals<T>(source1, source2);

    public static bool AreEquals<T>(IEnumerable<T> source1, IEnumerable<T> source2) // where T : IEquatable<T> 
        => EnumerableExtensions.AreEquals<T>(EqualityComparer<T>.Default, source1, source2);

    /// <summary>
    /// Recorre las enumeraciones coordinadamente, y retorna true si todos los elementos son iguales.
    /// </summary>
    /// <remarks>
    /// La enumeracion termina al primer elemento distinto o cuando termina alguna.
    /// 
    /// Si una enumeracion contiene mas items que la otra, se devuelve false, aunque hasta el momento todos los enumerados hayan sido iguales.
    /// </remarks>
    public static bool AreEquals<T>(this IEqualityComparer<T> equalityComparer, IEnumerable<T> source1, IEnumerable<T> source2) // where T : IEquatable<T>
        {
        using (IEnumerator<T> enumerator1 = source1.GetEnumerator())
            {
            using (IEnumerator<T> enumerator2 = source2.GetEnumerator())
                {
                bool hasNext1 = enumerator1.MoveNext();
                bool hasNext2 = enumerator2.MoveNext();

                while (hasNext1 && hasNext2)
                    {
                    if (!equalityComparer.Equals(enumerator1.Current, enumerator2.Current))
                        {
                        return false;
                        }

                    hasNext1 = enumerator1.MoveNext();
                    hasNext2 = enumerator2.MoveNext();
                    }

                return !hasNext1 && !hasNext2;
                }
            }
        }
    */

    /*
    // Solo para secuencias con igual cantidad de elementos
    public static int CompareTo<T>(this IEnumerable<T> source1, IEnumerable<T> source2) where T : IComparable<T> 
        => EnumerableExtensions
            .EnumerateInTuples(source1, source2)
            .Where(tuple => !tuple.Item1.Equals(tuple.Item2))
            .Select(tuple => tuple.Item1.CompareTo(tuple.Item2))
            .FirstOrDefault();
    */
    public static int CompareTo<T>(this IEnumerable<T> source1, IEnumerable<T> source2) // where T : IComparable<T>
        => EnumerableExtensions.Compare<T>(source1, source2);

    public static int Compare<T>(IEnumerable<T> source1, IEnumerable<T> source2) // where T : IComparable<T>
        => EnumerableExtensions.Compare<T>(Comparer<T>.Default, source1, source2);

    /// <summary>
    /// Recorre las enumeraciones coordinadamente, y comparando los elementos.
    /// </summary>
    /// <remarks>
    /// La enumeracion termina al primer elemento distinto o cuando termina alguna.
    ///
    /// Si una enumeracion contiene mas items que la otra, y si hasta el momento todos los enumerados son iguales se considera mayor, por ej
    /// el orden seria:
    /// A
    /// AB
    /// AC
    /// ACB
    ///
    /// </remarks>
    public static int Compare<T>(this IComparer<T> comparer, IEnumerable<T> source1, IEnumerable<T> source2) // where T : IEquatable<T>
        {
        using IEnumerator<T> enumerator1 = source1.GetEnumerator();
        using IEnumerator<T> enumerator2 = source2.GetEnumerator();
        bool hasNext1 = enumerator1.MoveNext();
        bool hasNext2 = enumerator2.MoveNext();

        while (hasNext1 && hasNext2)
            {
            int comp = comparer.Compare(enumerator1.Current, enumerator2.Current);
            if (comp != 0)
                {
                return comp;
                }

            hasNext1 = enumerator1.MoveNext();
            hasNext2 = enumerator2.MoveNext();
            }

        if (hasNext1 && !hasNext2)
            {
            return 1;
            }

        if (!hasNext1 && hasNext2)
            {
            return -1;
            }

        return 0;   // !hasNext1 && !hasNext2
        }

    /// <summary>
    /// Determina si la secuencia está ordenada ascendentemente.
    /// </summary>
    /// <returns>True si la secuencia está ordenada ascendentemente.</returns>
    public static bool IsOrdered<T>(this IEnumerable<T> source) where T : IComparable<T>
        {
        T? last = default;

        foreach (T element in source)
            {
            if (Object.Equals(last, default) || last.CompareTo(element) <= 0)
                {
                last = element;
                }
            else
                {
                return false;
                }
            }

        return true;
        }

    /// <summary>
    /// Determina si la secuencia está ordenada descendentemente.
    /// </summary>
    /// <returns>True si la secuencia está ordenada descendentemente.</returns>
    public static bool IsDescendingOrdered<T>(this IEnumerable<T> source) where T : IComparable<T>
        {
        T? last = default;

        foreach (T element in source)
            {
            if (Object.Equals(last, default) || last.CompareTo(element) >= 0)
                {
                last = element;
                }
            else
                {
                return false;
                }
            }

        return true;
        }


    /// <summary>
    /// Calcula el producto cartesiano de todas las secuencias.
    /// </summary>
    /// <remarks>Equivale a Cross Join en SQL.</remarks>
    /// <see cref="http://blogs.msdn.com/b/ericlippert/archive/2010/06/28/computing-a-cartesian-product-with-linq.aspx"/>
    public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
        IEnumerable<IEnumerable<T>> emptyProduct = [[]];

        return sequences.Aggregate(
            emptyProduct,
            (accumulator, sequence) =>
                from accseq in accumulator
                from element in sequence
                select accseq.Concat([element]));
        }

    /// <summary>
    /// Calcula el producto cartesiano de dos secuencias.
    /// </summary>
    /// <remarks>Equivale a Cross Join en SQL.</remarks>
    /// <see cref="https://stackoverflow.com/questions/56547/how-do-you-perform-a-cross-join-with-linq-to-sql"/>
    public static IEnumerable<(T1, T2)> CrossJoin<T1, T2>(IEnumerable<T1> sequence1, IEnumerable<T2> sequence2)
    //=> sequence1.SelectMany(t1 => sequence2.Select(t2 => (t1, t2)));
        {
        foreach (T1 item1 in sequence1)
            {
            foreach (T2 item2 in sequence2)
                {
                yield return (item1, item2);
                }
            }
        }


    /// <summary>
    /// Saltea la cantidad de items especificada.
    /// </summary>
    /// <remarks>
    /// enumerator.Current queda apuntando al ultimo element salteado.
    /// </remarks>
    /// <returns>Una referencia al eBecas1 para una api fluida.</returns>
    public static IEnumerator<T> Skip<T>(this IEnumerator<T> enumerator, int count)
        {
        while (count-- > 0 && enumerator.MoveNext()) { }

        return enumerator;
        }

    /// <summary>
    /// Toma la cantidad de items especificada.
    /// </summary>
    /// <remarks>
    /// enumerator.Current queda apuntando al ultimo element tomado.
    /// </remarks>
    /// <returns>Items tomados.</returns>
    /// Idem        public static IEnumerator<T> Take<T>(this IEnumerator<T> enumerator, int count)
    public static IEnumerable<T> Take<T>(this IEnumerator<T> enumerator, int count)
        {
        while (count-- > 0 && enumerator.MoveNext())
            {
            yield return enumerator.Current;
            }
        }

    /// <summary>
    /// Toma todos los items.
    /// </summary>
    /// <returns>Items tomados.</returns>
    /// Idem        public static IEnumerator<T> TakeToEnd<T>(this IEnumerator<T> enumerator)
    public static IEnumerable<T> TakeToEnd<T>(this IEnumerator<T> enumerator)
        {
        while (enumerator.MoveNext())
            {
            yield return enumerator.Current;
            }
        }

    public static (T Min, T Max) MinMax<T>(this IEnumerable<T> source) where T : IComparable<T>
        => source.MinMaxOrDefault() ?? throw new InvalidOperationException("The source sequence is empty.");

    /// <summary>
    /// Obtiene el mínimo y máximo element de la secuencia.
    /// </summary>
    /// <remarks>
    /// La enumeración es enumerada una única vez.
    /// </remarks>
    /// <returns>
    /// Una tupla con el mínimo y máximo element de la secuencia, respectivamente.
    /// </returns>
    public static (T Min, T Max)? MinMaxOrDefault<T>(this IEnumerable<T> source) where T : IComparable<T>
        {
        T? min = default;
        T? max = default;
        bool isFirst = true;

        foreach (T current in source)
            {
            if (isFirst)
                {
                min = max = current;
                isFirst = false;
                }
            else
                {
                if (current.CompareTo(min) < 0)
                    {
                    min = current;
                    }

                if (current.CompareTo(max) > 0)
                    {
                    max = current;
                    }
                }
            }

        return isFirst ? null : (min!, max!);
        }

    /// <summary>
    /// Obtiene un subconjunto de una enumeración.
    /// </summary>
    /// <context name="source">Enumeracion.</context>
    /// <context name="start">Comienzo del subconjunto (base 0). </context>
    /// <context name="count">Cantidad de elementos del subconjunto.</context>
    /// <returns>Subconjunto de la enumeración.</returns>
    public static IEnumerable<T> Range<T>(this IEnumerable<T> source, int start, int count)
        => source.Skip(start).Take(count);

    /// <summary>
    /// Idem Enumerable.All() pero permite pasar el indice del elemento al predicado.
    /// </summary>
    public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        => source.Select(predicate).And();

    public static bool And(this IEnumerable<bool> source)
        => source.All(b => b);

    /// <summary>
    /// Devuelve true si el element buscado se encuentra en la secuencia.
    /// </summary>
    public static bool Any<TSource>(this IEnumerable<TSource> source, TSource searchedItem)
        => source.Any(searchedItem, (element, context) => element!.Equals(context));

    /// <summary>
    /// Idem Enumerable.Any() pero permite pasar el indice del elemento al predicado.
    /// </summary>
    public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        => source.Select(predicate).Or();

    public static bool Or(this IEnumerable<bool> source)
        => source.Any(b => b);

    /// <summary>
    /// Demora el retorno de cada elemento de la secuencia.
    /// </summary>
    /// <remarks>La espera bloquea el thread actual.</remarks>
    /// <see cref="Observable.Delay"/>
    public static IEnumerable<T> Delay<T>(this IEnumerable<T> source, TimeSpan delay)
        => EnumerableExtensions.Do(source, delay, (_, d) => Task.Delay(d).Wait());

    /// <summary>
    /// Ejecuta la enumeración, descartándola.
    /// </summary>
    public static void Discard<T>(this IEnumerable<T> source)
    //=> source.ForEach(_ => { });
        {
        foreach (T _ in source) { }
        }

    /// <summary>
    /// Realiza una acción para cada elemento de la secuencia.
    /// </summary>
    /// <remarks>
    /// Este metodo ejecuta la enumeración (a diferencia de Do que no lo hace).
    /// </remarks>
    /// <see cref="http://blogs.msdn.com/b/ericlippert/archive/2009/05/18/foreach-vs-foreach.aspx"/>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action, CancellationToken cancellationToken = default)
        {
        foreach (T element in source)
            {
            cancellationToken.ThrowIfCancellationRequested();
            action(element);
            }
        }

    public static void ForEach<T, TContext>(this IEnumerable<T> source, TContext context, Action<T, TContext> action, CancellationToken cancellationToken = default)
        {
        foreach (T element in source)
            {
            cancellationToken.ThrowIfCancellationRequested();
            action(element, context);
            }
        }

    public static async Task ForEachAsync<T>(this IAsyncEnumerable<T> source, Action<T> action, CancellationToken cancellationToken = default)
        {
        await foreach(T element in source)
            {
            cancellationToken.ThrowIfCancellationRequested();
            action(element);
            }
        }

    public static async Task ForEachAsync<T, TContext>(this IAsyncEnumerable<T> source, TContext context, Action<T, TContext> action, CancellationToken cancellationToken = default)
        {
        await foreach (T element in source)
            {
            cancellationToken.ThrowIfCancellationRequested();
            action(element, context);
            }
        }

    public static async Task ForEachAsync<T>(this IAsyncEnumerable<T> source, Func<T, CancellationToken, Task> actionAsync, CancellationToken cancellationToken = default)
        {
        await foreach(T element in source)
            {
            await actionAsync(element, cancellationToken);
            }
        }

    public static async Task ForEachAsync<T, TContext>(this IAsyncEnumerable<T> source, TContext context, Func<T, TContext, CancellationToken, Task> actionAsync, CancellationToken cancellationToken = default)
        {
        await foreach (T element in source)
            {
            await actionAsync(element, context, cancellationToken);
            }
        }

    /// <summary>
    /// Realiza una acción para cada elemento de la secuencia que es retornado.
    /// </summary>
    /// <remarks>
    /// Este metodo NO ejecuta la enumeración (a diferencia de ForEach que si lo hace).
    /// La acción es ejecutada ANTES de retornar el elemento.
    /// </remarks>
    /// <see cref="Observable.Do"/>
    public static IEnumerable<T> Do<T>(this IEnumerable<T> source, Action<T> action)
        {
        foreach(T element in source)
            {
            action(element);
            yield return element;
            }
        }

    public static IEnumerable<T> Do<T, TContext>(this IEnumerable<T> source, TContext context, Action<T, TContext> action)
        {
        foreach (T element in source)
            {
            action(element, context);
            yield return element;
            }
        }

    public static async IAsyncEnumerable<T> DoAsync<T>(this IAsyncEnumerable<T> source, Action<T> action, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
        await foreach(T element in source)
            {
            cancellationToken.ThrowIfCancellationRequested();
            action(element);
            yield return element;
            }
        }

    public static async IAsyncEnumerable<T> DoAsync<T, TContext>(this IAsyncEnumerable<T> source, TContext context, Action<T, TContext> action, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
        await foreach (T element in source)
            {
            cancellationToken.ThrowIfCancellationRequested();
            action(element, context);
            yield return element;
            }
        }

    public static async IAsyncEnumerable<T> DoAsync<T>(this IAsyncEnumerable<T> source, Func<T, CancellationToken, Task> actionAsync, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
        await foreach(T element in source)
            {
            await actionAsync(element, cancellationToken);
            yield return element;
            }
        }

    public static async IAsyncEnumerable<T> DoAsync<T, TContext>(this IAsyncEnumerable<T> source, TContext context, Func<T, TContext, CancellationToken, Task> actionAsync, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
        await foreach (T element in source)
            {
            await actionAsync(element, context, cancellationToken);
            yield return element;
            }
        }

    public static IEnumerable<TSource> SkipUntil<TSource>(this IEnumerable<TSource> source, TSource current) where TSource : IEquatable<TSource>
        => source.SkipUntil(current.Equals);

    /// <summary>
    /// Inverso de SkipWhile
    /// </summary>
    /// <remarks>
    /// Al contrario de MoreLinq/SkipUntil, el element para el cual el predicado se satisface no es salteado.
    /// </remarks>
    /// <see cref="https://github.com/morelinq/MoreLINQ/blob/master/MoreLinq/SkipUntil.cs"/>
    public static IEnumerable<TSource> SkipUntil<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    //=> source.SkipWhile(s => !predicate(s));
        {
        using IEnumerator<TSource> enumerator = source.GetEnumerator();
        do
            {
            if (!enumerator.MoveNext())
                {
                yield break;
                }
            }
        while (!predicate(enumerator.Current));

        do
            {
            yield return enumerator.Current;
            }
        while (enumerator.MoveNext());
        }

    public static IEnumerable<TSource> TakeUntil<TSource>(this IEnumerable<TSource> source, TSource current) where TSource : IEquatable<TSource>
        => source.TakeUntil(current.Equals);

    /// <summary>
    /// Inverso de TakeWhile.
    /// </summary>
    /// <remarks>
    /// Al contrario de MoreLinq/TakeUntil, el element para el cual el predicado se satisface no es tomado.
    /// </remarks>
    /// <see cref="https://github.com/morelinq/MoreLINQ/blob/master/MoreLinq/TakeUntil.cs"/>
    public static IEnumerable<TSource> TakeUntil<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    //=> source.TakeWhile(s => !predicate(s));
        {
        foreach (TSource element in source)
            {
            if (predicate(element))
                {
                yield break;
                }
            else
                {
                yield return element;
                }
            }
        }

    public static T GetPrevious<T>(this IEnumerable<T> source, T current) where T: IEquatable<T>
        => source.GetPrevious(current.Equals);

    /// <summary>
    /// Get last element.
    /// </summary>
    /// <see cref="http://www.herlitz.nu/2011/12/01/getting-the-previous-and-next-record-from-list-using-linq/"/>
    /// <see cref="https://social.msdn.microsoft.com/Forums/en-US/f4e246b5-deed-42d0-8773-5d12ff310dbd/next-element-in-linq-query?forum=csharplanguage"/>
    public static T GetPrevious<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        => source.TakeUntil(predicate).Last();

    public static T? GetPreviousOrDefault<T>(this IEnumerable<T> source, T current) where T : IEquatable<T>
        => source.GetPreviousOrDefault(current.Equals);

    public static T? GetPreviousOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        => source.TakeUntil(predicate).LastOrDefault();

    public static T GetNext<T>(this IEnumerable<T> source, T current) where T : IEquatable<T>
        => source.GetNext(current.Equals);

    /// <summary>
    /// Get next element.
    /// </summary>
    /// <see cref="http://www.herlitz.nu/2011/12/01/getting-the-previous-and-next-record-from-list-using-linq/"/>
    /// <see cref="https://social.msdn.microsoft.com/Forums/en-US/f4e246b5-deed-42d0-8773-5d12ff310dbd/next-element-in-linq-query?forum=csharplanguage"/>
    public static T GetNext<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        => source.GetNextOrDefault(predicate) ?? throw new InvalidOperationException("Sequence contains no elements.");

    public static T? GetNextOrDefault<T>(this IEnumerable<T> source, T current) where T : IEquatable<T>
        => source.GetNextOrDefault(current.Equals);

    public static T? GetNextOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        => source.SkipUntil(predicate).Skip(1).FirstOrDefault();

    /// <summary>
    /// Recorre la secuencia pasando a la funcion el elemento anterior, el actual y un indicador de si el elemento actual es el primero de la secuencia.
    /// </summary>
    /// <remarks>
    /// La primera vez que se llama a la función el elemento anterior es default y el indicador es True.
    ///
    /// Similar con Rx:
    /// Observable.ToObservable(source).Buffer(2, 1).Select(buffer => func(buffer));
    /// </remarks>
    /// <typeparam name="TSource">Tipo de datos de la secuencia de entrada.</typeparam>
    /// <typeparam name="TResult">Tipo de datos de la secuencia de salida.</typeparam>
    /// <context name="source">Secuencia a enumerar.</context>
    /// <context name="func">Función a ejecutar para cada elemento de la secuencia y cuyo resultado es enviado a la salida.</context>
    /// <![CDATA[
    /// Enumerable.Range(1, 3).SelectPrevious((previous, current, isFirst) =>
    ///    {
    ///    Trace.WriteLine(String.Concat("previous: ", previous));
    ///    Trace.WriteLine(String.Concat("current: ", current));
    ///    Trace.WriteLine(String.Concat("isFirst: ", isFirst));
    ///    Trace.WriteLine("----------------------------");
    ///    });
    /// ]]>
    /// <returns>Secuencia de elementos retornados por la funcion.</returns>
    public static IEnumerable<TResult> SelectPrevious<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource?, TSource, bool, TResult> func)
        {
        bool isFirst = true;
        TSource? previous = default;

        foreach (TSource current in source)
            {
            yield return func(previous, current, isFirst);

            previous = current;
            isFirst = false;
            }
        }

    /// <summary>
    /// Recorre la secuencia pasando a la funcion el elemento actual, el siguiente y un indicador de si el elemento actual es el ultimo de la secuencia.
    /// </summary>
    /// <remarks>
    /// La última vez que se llama a la función el elemento siguiente es default y el indicador es True.
    ///
    /// Similar con Rx:
    /// Observable.ToObservable(source).Buffer(2, 1).Select(buffer => func(buffer));
    /// </remarks>
    /// <typeparam name="TSource">Tipo de datos de la secuencia de entrada.</typeparam>
    /// <typeparam name="TResult">Tipo de datos de la secuencia de salida.</typeparam>
    /// <context name="source">Secuencia a enumerar.</context>
    /// <context name="func">Función a ejecutar para cada elemento de la secuencia y cuyo resultado es enviado a la salida.</context>
    /// <![CDATA[
    /// Enumerable.Range(1, 3).SelectNext((current, next, isPrevious) =>
    ///    {
    ///    Trace.WriteLine(String.Concat("current: ", current));
    ///    Trace.WriteLine(String.Concat("next: ", next));
    ///    Trace.WriteLine(String.Concat("isPrevious: ", isPrevious));
    ///
    ///    Trace.WriteLine("----------------------------");
    ///    });
    /// ]]>
    /// <returns>Secuencia de elementos retornados por la funcion.</returns>
    public static IEnumerable<TResult> SelectNext<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TSource?, bool, TResult> func)
        {
        TSource? current = default;

        bool isFirst = true;
        foreach (TSource next in source)
            {
            if (isFirst)
                {
                isFirst = false;
                }
            else
                {
                yield return func(current!, next, false);
                }

            current = next;
            }

        if (!isFirst)
            {
            yield return func(current!, default, true);
            }
        }

    /// <summary>
    /// Recorre la secuencia pasando a la funcion el elemento anterior, el actual, el siguiente, un indicador de si el elemento actual es el primero de la secuencia y otro si es el último.
    /// </summary>
    /// <remarks>
    /// El valor devuelto por la funcion es retornado.
    /// La primera vez que se llama a la función el elemento anterior es default y el indicador isFirst es True.
    /// La última vez que se llama a la función el elemento siguiente es default y el indicador isPrevious es True.
    ///
    /// Similar con Rx:
    /// Observable.ToObservable(source).Buffer(3, 1).Select(buffer => func(buffer));
    /// </remarks>
    /// <typeparam name="TSource">Tipo de datos de la secuencia de entrada.</typeparam>
    /// <typeparam name="TResult">Tipo de datos de la secuencia de salida.</typeparam>
    /// <context name="source">Secuencia a enumerar.</context>
    /// <context name="func">Función a ejecutar para cada elemento de la secuencia y cuyo resultado es enviado a la salida.</context>
    /// <example>
    /// <![CDATA[
    /// Enumerable.Range(1, 3).SelectPreviousNext((previous, current, next, isFirst, isPrevious) =>
    ///    {
    ///        Trace.WriteLine(String.Concat("previous: ", previous));
    ///        Trace.WriteLine(String.Concat("current: ", current));
    ///        Trace.WriteLine(String.Concat("next: ", next));
    ///        Trace.WriteLine(String.Concat("isFirst: ", isFirst));
    ///        Trace.WriteLine(String.Concat("isPrevious: ", isPrevious));
    ///
    ///        Trace.WriteLine("----------------------------");
    ///    });
    /// ]]>
    /// </example>
    /// <returns>Secuencia de elementos retornados por la funcion.</returns>
    public static IEnumerable<TResult> SelectPreviousNext<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource?, TSource, TSource?, bool, bool, TResult> func)
        {
        TSource? previous = default;
        TSource? current = default;

        bool isFirst = true;
        bool isFirstForUser = true;
        foreach (TSource next in source)
            {
            if (isFirst)
                {
                isFirst = false;
                }
            else
                {
                yield return func(previous, current!, next, isFirstForUser, false);

                isFirstForUser = false;
                previous = current;
                }

            current = next;
            }

        if (!isFirst)
            {
            yield return func(previous, current!, default, isFirstForUser, true);
            }
        }
    /// <summary>
    /// Recorre la secuencia pasando a la funcion el elemento anterior, el actual y un indicador de si el elemento actual es el primero de la secuencia.
    /// </summary>
    /// <remarks>
    /// La primera vez que se llama a la función el elemento anterior es default y el indicador es True.
    /// </remarks>
    /// <typeparam name="TSource">Tipo de datos de la secuencia.</typeparam>
    /// <context name="source">Secuencia a enumerar.</context>
    /// <context name="action">Función a ejecutar para cada elemento de la secuencia.</context>
    /// <![CDATA[
    /// Enumerable.Range(1, 3).DoPrevious((previous, current, isFirst) =>
    ///    {
    ///    Trace.WriteLine(String.Concat("previous: ", previous));
    ///    Trace.WriteLine(String.Concat("current: ", current));
    ///    Trace.WriteLine(String.Concat("isFirst: ", isFirst));
    ///    Trace.WriteLine("----------------------------");
    ///    });
    /// ]]>
    public static IEnumerable<TSource> DoPrevious<TSource>(this IEnumerable<TSource> source, Action<TSource?, TSource, bool> action)
        {
        bool isFirst = true;
        TSource? previous = default;

        foreach (TSource current in source)
            {
            action(previous, current, isFirst);
            yield return current;

            previous = current;
            isFirst = false;
            }
        }

    /// <summary>
    /// Recorre la secuencia pasando a la funcion el elemento actual, el siguiente y un indicador de si el elemento actual es el ultimo de la secuencia.
    /// </summary>
    /// <remarks>
    /// La última vez que se llama a la función el elemento siguiente es default y el indicador es True.
    /// </remarks>
    /// <typeparam name="TSource">Tipo de datos de la secuencia.</typeparam>
    /// <context name="source">Secuencia a enumerar.</context>
    /// <context name="action">Función a ejecutar para cada elemento de la secuencia.</context>
    /// <![CDATA[
    /// Enumerable.Range(1, 3).DoNext((current, next, isPrevious) =>
    ///    {
    ///    Trace.WriteLine(String.Concat("current: ", current));
    ///    Trace.WriteLine(String.Concat("next: ", next));
    ///    Trace.WriteLine(String.Concat("isPrevious: ", isPrevious));
    ///    Trace.WriteLine("----------------------------");
    ///    });
    /// ]]>
    public static IEnumerable<TSource> DoNext<TSource>(this IEnumerable<TSource> source, Action<TSource, TSource?, bool> action)
        {
        TSource? current = default;

        bool isFirst = true;
        foreach (TSource next in source)
            {
            if (isFirst)
                {
                isFirst = false;
                }
            else
                {
                action(current!, next, false);
                yield return current!;
                }

            current = next;
            }

        if (!isFirst)
            {
            action(current!, default, true);
            yield return current!;
            }
        }

    /// <summary>
    /// Recorre la secuencia pasando a la funcion el elemento anterior, el actual, el siguiente, un indicador de si el elemento actual es el primero de la secuencia y otro si es el último.
    /// </summary>
    /// <remarks>
    /// El valor devuelto por la funcion es asignado al acumulador.
    /// La primera vez que se llama a la función el elemento anterior es default y el indicador isFirst es True.
    /// La última vez que se llama a la función el elemento siguiente es default y el indicador isPrevious es True.
    /// </remarks>
    /// <typeparam name="TSource">Tipo de datos de la secuencia.</typeparam>
    /// <context name="source">Secuencia a enumerar.</context>
    /// <context name="action">Función a ejecutar para cada elemento de la secuencia.</context>
    /// <example>
    /// <![CDATA[
    /// Enumerable.Range(1, 3).DoPreviousNext((previous, current, next, isFirst, isPrevious) =>
    ///    {
    ///        Trace.WriteLine(String.Concat("previous: ", previous));
    ///        Trace.WriteLine(String.Concat("current: ", current));
    ///        Trace.WriteLine(String.Concat("next: ", next));
    ///        Trace.WriteLine(String.Concat("isFirst: ", isFirst));
    ///        Trace.WriteLine(String.Concat("isPrevious: ", isPrevious));
    ///        Trace.WriteLine("----------------------------");
    ///    });
    /// ]]>
    /// </example>
    public static IEnumerable<TSource> DoPreviousNext<TSource>(this IEnumerable<TSource> source, Action<TSource?, TSource, TSource?, bool, bool> action)
        {
        TSource? previous = default;
        TSource? current = default;

        bool isFirst = true;
        bool isFirstForUser = true;
        foreach (TSource next in source)
            {
            if (isFirst)
                {
                isFirst = false;
                }
            else
                {
                action(previous, current!, next, isFirstForUser, false);
                yield return current!;

                isFirstForUser = false;
                previous = current;
                }

            current = next;
            }

        if (!isFirst)
            {
            action(previous, current!, default, isFirstForUser, true);
            yield return current!;
            }
        }
    public static TSource AggregatePrevious<TSource>(this IEnumerable<TSource> source, Func<TSource?, TSource, TSource, bool, TSource> func, TSource seed)
        => EnumerableExtensions.AggregatePrevious<TSource, TSource>(source, func, seed);

    /// <summary>
    /// Recorre la secuencia pasando a la funcion el elemento anterior, el actual, un acumulador y un indicador de si el elemento actual es el primero de la secuencia.
    /// </summary>
    /// <remarks>
    /// El valor devuelto por la funcion es asignado al acumulador.
    /// La primera vez que se llama a la función el elemento anterior es default y el indicador es True.
    /// </remarks>
    /// <typeparam name="TSource">Tipo de datos de la secuencia.</typeparam>
    /// <typeparam name="TAccumulate">Tipo de datos del acumulador.</typeparam>
    /// <context name="source">Secuencia a enumerar.</context>
    /// <context name="func">Función a ejecutar para cada elemento de la secuencia.</context>
    /// <context name="seed">Valor inicial del acumulador</context>
    /// <example>
    /// <![CDATA[
    /// public static double Rosenbrock(IEnumerable<double> chromosome)
    ///     {
    ///     return chromosome.AggregatePrevious((previous, current, accumulate, isFirst) => isFirst ? 0.0 : accumulate + (1 - previous).Pow2() + 100 * (current - previous.Pow2()).Pow2());
    ///     }
    /// ]]>
    /// </example>
    /// <returns>Valor en el acumulador.</returns>
    public static TAccumulate AggregatePrevious<TSource, TAccumulate>(this IEnumerable<TSource> source, Func<TSource?, TSource, TAccumulate, bool, TAccumulate> func, TAccumulate seed)
        {
        bool isFirst = true;
        TSource? previous = default;
        TAccumulate accumulate = seed;

        foreach (TSource current in source)
            {
            accumulate = func(previous, current, accumulate, isFirst);
            previous = current;
            isFirst = false;
            }

        return accumulate;
        }

    public static TSource AggregateNext<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource?, TSource, bool, TSource> func, TSource seed)
        => EnumerableExtensions.AggregateNext<TSource, TSource>(source, func, seed);

    /// <summary>
    /// Recorre la secuencia pasando a la funcion el elemento actual, el siguiente, un acumulador y un indicador de si el elemento actual es el ultimo de la secuencia.
    /// </summary>
    /// <remarks>
    /// El valor devuelto por la funcion es asignado al acumulador.
    /// La última vez que se llama a la función el elemento siguiente es default y el indicador es True.
    /// </remarks>
    /// <typeparam name="TSource">Tipo de datos de la secuencia.</typeparam>
    /// <typeparam name="TAccumulate">Tipo de datos del acumulador.</typeparam>
    /// <context name="source">Secuencia a enumerar.</context>
    /// <context name="func">Función a ejecutar para cada elemento de la secuencia.</context>
    /// <context name="seed">Valor inicial del acumulador</context>
    /// <example>
    /// <![CDATA[
    /// public static double Rosenbrock(IEnumerable<double> chromosome)
    ///     {
    ///     return chromosome.AggregateNext((current, next, accumulate, isPrevious) => isPrevious? accumulate: accumulate + (1 - current).Pow2() + 100 * (next - current.Pow2()).Pow2());
    ///     }
    /// ]]>
    /// </example>
    /// <returns>Valor en el acumulador.</returns>
    public static TAccumulate AggregateNext<TSource, TAccumulate>(this IEnumerable<TSource> source, Func<TSource, TSource?, TAccumulate, bool, TAccumulate> func, TAccumulate seed)
        {
        TSource? current = default;
        TAccumulate accumulate = seed;

        bool isFirst = true;
        foreach (TSource next in source)
            {
            if (isFirst)
                {
                isFirst = false;
                }
            else
                {
                accumulate = func(current!, next, accumulate, false);
                }

            current = next;
            }

        return isFirst ? seed : func(current!, default, accumulate, true);
        }

    public static TSource AggregatePreviousNext<TSource>(this IEnumerable<TSource> source, Func<TSource?, TSource, TSource?, TSource, bool, bool, TSource> func, TSource seed)
        => EnumerableExtensions.AggregatePreviousNext<TSource, TSource>(source, func, seed);

    /// <summary>
    /// Recorre la secuencia pasando a la funcion el elemento anterior, el actual, el siguiente, un acumulador, un indicador de si el elemento actual es el primero de la secuencia y otro si es el último.
    /// </summary>
    /// <remarks>
    /// El valor devuelto por la funcion es asignado al acumulador.
    /// La primera vez que se llama a la función el elemento anterior es default y el indicador isFirst es True.
    /// La última vez que se llama a la función el elemento siguiente es default y el indicador isPrevious es True.
    /// </remarks>
    /// <typeparam name="TSource">Tipo de datos de la secuencia.</typeparam>
    /// <typeparam name="TAccumulate">Tipo de datos del acumulador.</typeparam>
    /// <context name="source">Secuencia a enumerar.</context>
    /// <context name="func">Función a ejecutar para cada elemento de la secuencia.</context>
    /// <context name="seed">Valor inicial del acumulador</context>
    /// <example>
    /// <![CDATA[
    /// Trace.WriteLine(Enumerable.Range(1, 3).AggregatePreviousNext((previous, current, next, accumulate, isFirst, isPrevious) =>
    ///     {
    ///     Trace.WriteLine(String.Concat("previous: ", previous));
    ///     Trace.WriteLine(String.Concat("current: ", current));
    ///     Trace.WriteLine(String.Concat("next: ", next));
    ///     Trace.WriteLine(String.Concat("accumulate: ", accumulate));
    ///     Trace.WriteLine(String.Concat("isFirst: ", isFirst));
    ///     Trace.WriteLine(String.Concat("isPrevious: ", isPrevious));
    ///     Trace.WriteLine(String.Concat("return: ", accumulate + current));
    ///     Trace.WriteLine("----------------------------");
    ///     return accumulate + current;
    ///     }));
    /// ]]>
    /// </example>
    /// <returns>Valor en el acumulador.</returns>
    public static TAccumulate AggregatePreviousNext<TSource, TAccumulate>(this IEnumerable<TSource> source, Func<TSource?, TSource, TSource?, TAccumulate, bool, bool, TAccumulate> func, TAccumulate seed)
        {
        TSource? previous = default;
        TSource? current = default;
        TAccumulate accumulate = seed;

        bool isFirst = true;
        bool isFirstForUser = true;
        foreach (TSource next in source)
            {
            if (isFirst)
                {
                isFirst = false;
                }
            else
                {
                accumulate = func(previous, current!, next, accumulate, isFirstForUser, false);
                isFirstForUser = false;
                previous = current;
                }

            current = next;
            }

        return isFirst ? seed : func(previous, current!, default, accumulate, isFirstForUser, true);
        }
    /*
    /// <summary>
    /// Enumera los elementos agregando el indice en forma de tupla.
    /// </summary>
    public static IEnumerable<IndexedElement<T>> Index<T>(this IEnumerable<T> source)
        => source.Select((value, index) => new IndexedElement<T>(index, value));
    
    public static IndexedElement<T> FirstIndexedTuple<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        => source.FirstIndexedTupleOrDefault(predicate) ?? throw new InvalidOperationException("Sequence contains no elements.");

    public static IndexedElement<T>? FirstIndexedTupleOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        //=> source.Index().First(tuple => predicate(tuple.Value)); //commented to avoid capture
        { 
        foreach (IndexedElement<T> tuple in source.Index())
            {
            if (predicate(tuple.Value))
                {
                return tuple;
                }
            }

        return null;
        }
    */
    /// <summary>
    /// Recorre la secuencia agrupando los elementos y enumerandolos en tuplas.
    /// </summary>
    /// <see cref="http://stackoverflow.com/questions/5239635/how-take-each-two-items-from-ienumerable-as-a-pair"/>
    public static IEnumerable<(T, T)> ToTuples<T>(this IEnumerable<T> source)
        => source
            .Index()
            .GroupBy(x => x.Index / 2)
            .Select(g => (g.ElementAt(0).Item, g.ElementAt(1).Item));

    /// <summary>
    /// Retorna los primeros dos elementos de la secuencia en una tupla.
    /// </summary>
    /// <remarks>
    /// Si la secuencia tiene menos de dos elementos retorna default para los items faltantes.
    /// </remarks>
    public static (T?, T?) FirstOrDefaultTuple<T>(this IEnumerable<T> source)
        {
        T? first = default;
        bool isFirst = true;

        foreach (T current in source)
            {
            if (isFirst)
                {
                first = current;
                isFirst = false;
                }
            else
                {
                return (first, current);
                }
            }

        return (first, default);
        }

    /// <summary>
    /// Retorna los primeros dos elementos de la secuencia en una tupla.
    /// </summary>
    /// <remarks>
    /// Si la secuencia tiene menos de dos elementos dispara una excepcion.
    /// </remarks>
    public static (T, T) FirstTuple<T>(this IEnumerable<T> source)
        {
        T? first = default;
        bool isFirst = true;

        foreach (T current in source)
            {
            if (isFirst)
                {
                first = current;
                isFirst = false;
                }
            else
                {
                return (first!, current);
                }
            }

        throw new InvalidOperationException("The source sequence has less than 2 items.");
        }

    /// <summary>
    /// Calcula un HashCode para la secuencia combinando los HashCode de los items
    /// SIN TENER EN CUENTA EL ORDEN DE LOS MISMOS, NI SI ESTAN DUPLICADOS.
    /// </summary>
    /// <remarks>
    /// Este método NO tiene en cuenta el orden de los items, ni si están duplicados,
    /// generando los mismos codigos para secuencias con elementos equivalentes (en cualquier orden y cantidad).
    /// </remarks>
    /// <example>
    ///     IEnumerable<string> list1 = new string[] { "a", null, "b" };
    ///     IEnumerable<string> list2 = new string[] { "a", null, "b", "b" };
    ///     IEnumerable<string> list3 = new string[] { "b", null, "a", null, "b" };
    ///     IEnumerable<string> list4 = new string[] { "b", null, "a", "b", "b" };
    ///
    ///     Assert.AreEqual(list1, list1);
    ///     Assert.AreNotEqual(list1, list2);
    ///     Assert.AreNotEqual(list1, list3);
    ///     Assert.AreNotEqual(list1, list4);
    ///
    ///     Assert.AreEqual(list1.GetSequenceHashCode(), list1.GetSequenceHashCode());
    ///     Assert.AreNotEqual(list1.GetSequenceHashCode(), list2.GetSequenceHashCode());
    ///     Assert.AreNotEqual(list1.GetSequenceHashCode(), list3.GetSequenceHashCode());
    ///     Assert.AreNotEqual(list1.GetSequenceHashCode(), list4.GetSequenceHashCode());
    ///     Assert.AreNotEqual(list2.GetSequenceHashCode(), list3.GetSequenceHashCode());
    ///
    ///     Trace.WriteLine(list1.GetSetHashCode());
    ///     Trace.WriteLine(list2.GetSetHashCode());
    ///     Trace.WriteLine(list3.GetSetHashCode());
    ///     Trace.WriteLine(list4.GetSetHashCode());
    ///
    ///     Assert.AreEqual(list1.GetSetHashCode(), list1.GetSetHashCode());
    ///     Assert.AreNotEqual(list1.GetSetHashCode(), list2.GetSetHashCode());
    ///     Assert.AreNotEqual(list1.GetSetHashCode(), list3.GetSetHashCode());
    ///
    ///     Assert.AreEqual(list1.GetSetHashCode(), list4.GetSetHashCode());
    ///     Assert.AreEqual(list2.GetSetHashCode(), list3.GetSetHashCode());
    /// </example>
    /// <see cref="http://stackoverflow.com/questions/7278136/create-hash-value-on-a-list/7278155#7278155"/>
    public static int GetSetHashCode<T>(this IEnumerable<T> source, int seed = 0)
        => source
            .Select(element => element?.GetHashCode() ?? 0)
            .Aggregate(seed, (total, itemHashCode) => total ^ itemHashCode);

    /// <summary>
    /// Calcula un HashCode para la secuencia combinando los HashCode de los items.
    /// </summary>
    /// <context name="source">Secuencia de items (cuyo orden afecta el resultado del calculo).</context>
    /// <context name="seed">Nro a usar como semilla del calculo (preferentemente un nro primo).</context>
    /// <context name="modifier">Nro primo a usar como modificador del hashcode de cada element (preferentemente un nro primo).</context>
    /// <returns>HashCode de la secuencia.</returns>
    /// <remarks>
    /// Este método tiene en cuenta el orden de los items, generando distintos codigos para secuencias con
    /// elementos equivalentes pero en distinto orden.
    ///
    /// La secuencia pueden contener elementos nulos.
    ///
    /// Este algoritmo es conocido como Bernstein Hash (o djb2) y es el usado por el compilador de C# para los tipos anonimos.
    ///
    /// Seed y modifier deberian ser co-primos.
    /// Algunos ejemplos de valores para seed y modifier:
    ///     17 y 23     del libro Effective Java (http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode)
    ///     17 y 0      records classes implementation (https://blog.cdemi.io/whats-coming-in-c-8-0-records/ o http://www.devsanon.com/c/c-8-is-introducing-records/?utm_source=csharpdigest&utm_medium=email&utm_campaign=featured)
    ///     23 y 37     Jon Skeet (https://stackoverflow.com/questions/508126/what-is-the-correct-implementation-for-gethashcode-for-entity-classes)
    ///     0 y 31      Java
    ///     0 y 33      del algoritmo original http://eternallyconfuzzled.com/tuts/algorithms/jsw_tut_hashing.aspx
    ///     487 y 31    del otra version del algoritmo original http://stackoverflow.com/questions/7278136/create-hash-value-on-a-list/7278155#7278155
    ///     5381 y 33   http://www.cse.yorku.ca/~oz/hash.html
    /// </remarks>
    /// <example>
    ///     IEnumerable<string> list1 = new string[] { "a", null, "b" };
    ///     IEnumerable<string> list2 = new string[] { "a", null, "b" };
    ///     IEnumerable<string> list3 = new string[] { "b", null, "a" };
    ///
    ///     Assert.AreEqual(list1, list1);
    ///     Assert.AreNotEqual(list1, list2);
    ///     Assert.AreNotEqual(list1, list3);
    ///
    ///     Assert.AreEqual(list1.GetSequenceHashCode(), list1.GetSequenceHashCode());
    ///     Assert.AreEqual(list1.GetSequenceHashCode(), list2.GetSequenceHashCode());
    ///     Assert.AreNotEqual(list1.GetSequenceHashCode(), list3.GetSequenceHashCode());
    ///
    ///     Assert.AreEqual(list1.GetSetHashCode(), list1.GetSetHashCode());
    ///     Assert.AreEqual(list1.GetSetHashCode(), list2.GetSetHashCode());
    ///     Assert.AreEqual(list1.GetSetHashCode(), list3.GetSetHashCode());
    /// </example>
    /// <see cref="http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode"/>
    /// <see cref="http://stackoverflow.com/questions/7278136/create-hash-value-on-a-list/7278155#7278155"/>
    /// <see cref="http://eternallyconfuzzled.com/tuts/algorithms/jsw_tut_hashing.aspx"/>
    /// <see cref="http://www.cse.yorku.ca/~oz/hash.html"/>
    /// <see cref="https://blogs.msdn.microsoft.com/ericlippert/2011/02/28/guidelines-and-rules-for-gethashcode/"/>
    /// <see cref="http://stackoverflow.com/questions/15174477/how-is-gethashcode-of-c-sharp-string-implemented"/>
    public static int GetSequenceHashCode<T>(this IEnumerable<T> source, int seed = 17, int modifier = 23)
        {
        unchecked // Overflow is fine, just wrap
            {
            return source
                .Select(element => element?.GetHashCode() ?? 0)
                .Aggregate(seed, modifier, (total, modif, itemHashCode) => total * modif + itemHashCode);
            }
        }

    public static string ToKeyValueString<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, string keyValueSeparator = " = ", string entrySeparator = ", ")
        {
        using PooledStringBuilder builder = new PooledStringBuilder();
        return builder.StringBuilder.AppendKeyValueString(source, keyValueSeparator, entrySeparator).ToString();
        }

    public static string ToDelimitedString<T>(this IEnumerable<T> source, string separator = ", ")
        => source.Select(element => element?.ToString() ?? "").ToDelimitedString(separator);

    /// <summary>
    /// Convierte una lista en un string representando el contenido.
    /// </summary>
    /// <context name="source">IEnumerable a convertir.</context>
    /// <context name="separator">Cadena a usar de separador de cada elemento.</context>
    /// <returns>String representando el contenido de la lista</returns>
    public static string ToDelimitedString(this IEnumerable<string> source, string separator = ", ")
        {
        using PooledStringBuilder builder = new PooledStringBuilder();
        return builder.StringBuilder.AppendDelimitedStrings(source, separator).ToString();
        }

    /// <summary>
    /// Returns a string that represents each items in the collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <context name="source"></context>
    /// <returns></returns>
    public static IEnumerable<string> ToEnumerableString<T>(this IEnumerable<T> source)
        => source.Select(element => Object.Equals(element, null) ? "null" : element.ToString())!;

    public static IEnumerable<T> Concat<T>(this T element, IEnumerable<T> source)
        => element.AsEnumerable().Concat(source);

    public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T element)
        => source.Concat([ element ]);

    /// <summary>
    /// Crea una enumeración con el elemento como único element.
    /// </summary>
    //public static IEnumerable<T> ToEnumerable<T>(this T element) => Enumerable.Repeat(obj, 1);
    /// <summary>
    /// Crea una enumeración con los elementos.
    /// </summary>
    //[Obsolete("Use collection expressions [ ..")]
    //public static IEnumerable<T> ToEnumerable<T>(params T[] items) => items;

    //[Obsolete("Use collection expressions [ .. ] when it makes sense.")]
    public static IEnumerable<T> AsEnumerable<T>(this T element)
        {
        yield return element;
        }

    /// <summary>
    /// Crea una enumeración con los elementos.
    /// </summary>
    //[Obsolete("Use collection expressions [ .. ] when it makes sense.")]
    public static IEnumerable<T> AsEnumerable<T>(T first, T second)
        {
        yield return first;
        yield return second;
        }

    //[Obsolete("Use collection expressions [ .. ] when it makes sense.")]
    public static IEnumerable<T> AsEnumerable<T>(T first, T second, T third)
        {
        yield return first;
        yield return second;
        yield return third;
        }

    public static IEnumerable<T> AsEnumerable<T>(this Memory<T> memory)
        => MemoryMarshal.ToEnumerable<T>(memory);

    /// <summary>
    /// Obtains the data as a list; if it is *already* a list, the original object is returned without
    /// any duplication; otherwise, ToList() is invoked.
    /// </summary>
    /// <typeparam name="T">The type of element in the list.</typeparam>
    /// <context name="source">The enumerable to return as a list.</context>
    /// <see cref="Dapper.SqlMapper.AsList()"/>
    //[Obsolete("Use collection expressions [ ..")]
    public static List<T> AsList<T>(this IEnumerable<T> source)
        => source is List<T> list ? list : [.. source]; // source.ToList();

    /// <summary>
    /// Obtains the data as an array; if it is *already* an array, the original object is returned without
    /// any duplication; otherwise, ToArray() is invoked.
    /// </summary>
    /// <typeparam name="T">The type of element in the list.</typeparam>
    /// <context name="source">The enumerable to return as a list.</context>
    /// <see cref="Dapper.SqlMapper.AsList()"/>
    //[Obsolete("Use collection expressions [ ..")]
    public static T[] AsArray<T>(this IEnumerable<T> source)
        => source is T[] v ? v : [.. source]; //source.ToArray();

    public static HashSet<T> AsHashSet<T>(this IEnumerable<T> source)
        => source is HashSet<T> set ? set : source.ToHashSet();

    public static ReadOnlyCollection<T> AsReadOnlyCollection<T>(this IEnumerable<T> source)
        => source is ReadOnlyCollection<T> collection ? collection : source.ToReadOnlyCollection();

    public static SortedSet<T> AsSortedSet<T>(this IEnumerable<T> source)
        => source is SortedSet<T> set ? set : source.ToSortedSet();

    public static Queue<T> AsQueue<T>(this IEnumerable<T> source)
        => source is Queue<T> queue ? queue : source.ToQueue();

    public static Stack<T> AsStack<T>(this IEnumerable<T> source)
        => source is Stack<T> stack ? stack : source.ToStack();

    public static LinkedList<T> AsLinkedList<T>(this IEnumerable<T> source)
        => source is LinkedList<T> list ? list : source.ToLinkedList();

    public static T[] ToArray<T>(this IEnumerable<T> source, int length)
        {
        T[] array = new T[length];
        source.WriteTo(array);
        return array;
        }

    public static IEnumerable<T> ToIEnumerable<T>(this IEnumerator<T> enumerator) 
        {
        while (enumerator.MoveNext()) 
            {
            yield return enumerator.Current;
            }
        }

    public static IEnumerable<T> ReadAsEnumerable<T>(this Memory<T> memory)
        {
        for (int i = 0; i < memory.Length; i++)
            {
            yield return memory.Span[i];
            }
        }

    /// <summary>
    /// Write the secuence to the Span.
    /// </summary>
    /// <remarks>
    /// Using an index variable is faster than using Enumerable.Index.
    /// 
    /// | Method               | Mean      | Error    | StdDev   | Ratio | RatioSD | Rank | Gen0       | Gen1      | Allocated    | Alloc Ratio |
    /// |--------------------- |----------:|---------:|---------:|------:|--------:|-----:|-----------:|----------:|-------------:|------------:|
    /// | ToArray              |  10.73 ms | 0.207 ms | 0.213 ms |  1.00 |    0.03 |    1 | 31734.3750 |  218.7500 | 195038.35 KB |       1.000 |
    /// | WriteTo with Index() | 441.89 ms | 1.829 ms | 1.527 ms | 41.19 |    0.80 |    6 |          - |         - |   1378.19 KB |       0.007 |
    /// | WriteTo with i       |  83.70 ms | 0.302 ms | 0.268 ms |  7.80 |    0.15 |    2 |          - |         - |    646.55 KB |       0.003 |
    /// </remarks>
    /// <returns>Number of items written.</returns>
    public static int WriteTo<T>(this IEnumerable<T> source, Span<T> span)
        {
        int i = 0;
        foreach (T item in source)
            {
            if (i == span.Length)
                {
                return i;
                }

            span[i++] = item;
            }

        return i;
        }

    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        => new HashSet<T>(source);

    public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> source)
        => new ReadOnlyCollection<T>([.. source]);

    public static SortedDictionary<TKey, TSource> ToSortedDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) where TKey : notnull
        => new SortedDictionary<TKey, TSource>(source.ToDictionary(keySelector));

    public static SortedSet<T> ToSortedSet<T>(this IEnumerable<T> source)
        => new SortedSet<T>(source);

    public static Queue<T> ToQueue<T>(this IEnumerable<T> source)
        => new Queue<T>(source);

    public static Stack<T> ToStack<T>(this IEnumerable<T> source)
        => new Stack<T>(source);

    public static LinkedList<T> ToLinkedList<T>(this IEnumerable<T> source)
        => new LinkedList<T>(source);

    /// <summary>
    /// Recorre un arbol de referencias retornando los nodos visitados.
    /// </summary>
    /// <context name="parent">Raiz del arbol.</context>
    /// <context name="getChildren">Función que dado un nodo retorna los hijos del mismo.</context>
    /// <returns>Todos los nodos visitados.</returns>
    public static IEnumerable<T> Traverse<T>(this T parent, Func<T, IEnumerable<T>> getChildren)
         => parent.AsEnumerable().Concat(getChildren(parent).SelectMany(getChildren, Traverse));

    /// <summary>
    /// Recorre el arbol y los pasa por parametro a una función especificada
    /// </summary>
    /// <remarks>Si el metodo devuelve false, se detiene la exploración.</remarks>
    /// <context name="element">Raiz del arbol a recorrer.</context>
    /// <context name="visitor">Metodo para procesar cada element.</context>
    /// <context name="recurse">Metodo que toma un element y devuelve la siguiente generacion a procesar.</context>
    /// <returns>True si se recorrieron todos los directorios del arbol (False si se detuvo antes). </returns>
    public static bool Walk<T>(this T element, Predicate<T> visitor, Func<T, IEnumerable<T>> recurse)
        => visitor(element) && recurse(element).All((visitor, recurse), (subItem, p) => subItem.Walk(p.visitor, p.recurse));

    /// <summary>
    /// Permite recorrer una lista enlazada circularmente hacia adelante.
    /// </summary>
    /// <returns>Retorna el próximo nodo o el primero si el actual es el último.</returns>
    public static LinkedListNode<T>? NextOrFirst<T>(this LinkedListNode<T> current)
        => current.Next ?? current.List?.First;

    /// <summary>
    /// Permite recorrer una lista enlazada circularmente hacia atras.
    /// </summary>
    /// <returns>Retorna el nodo anterior o el último si el actual es el primero.</returns>
    public static LinkedListNode<T>? PreviousOrLast<T>(this LinkedListNode<T> current)
        => current.Previous ?? current.List?.Last;

    /// <summary>
    /// Permite enumerar circularmente un enumerable.
    /// </summary>
    /// <remarks>
    /// El enumerable retornado es infinito.
    /// El enumerable pasado por parametro puede ser enumerado múltiples veces por lo que debe ser un objeto ya poblado
    /// como una lista o un arreglo.
    /// </remarks>
    /// <example>
    /// <![CDATA[
    ///     Usando foreach:
    ///
    ///             List<int> nums = new List<int>() { 1, 2, 3 };
    ///             int i = 0;
    ///             foreach (int num in nums.ToCircularEnumerable())
    ///                 {
    ///                 Trace.Write(num); Trace.Write(" - "); Trace.WriteLine(i);
    ///                 if (i++ > 9) break;    /// salida, de lo contrario el foreach es infinito
    ///                 }
    ///
    ///
    ///     Usando GetEnumerator():
    ///
    ///             List<int> nums = new List<int>() { 1, 2, 3 };
    ///             int i = 0;
    ///             using (IEnumerator<int> enumerator = nums.ToCircularEnumerable().GetEnumerator())
    ///                 {
    ///                 while (enumerator.MoveNext()) //Infinite loop
    ///                     {
    ///                     Trace.Write(enumerator.Current); Trace.Write(" - "); Trace.WriteLine(i);
    ///                     if (i++ > 9) break;
    ///                     }
    ///                 }
    ///
    /// ]]>
    /// </example>
    /// <returns>EBecas1 circular.</returns>
    public static IEnumerable<T> ToCircularEnumerable<T>(this IEnumerable<T> enumerable, CancellationToken cancellationToken = default)
        {
        while (!cancellationToken.IsCancellationRequested)
            {
            foreach (T element in enumerable)
                {
                yield return element;
                }
            }
        }

    /// <summary>
    /// Crea un StringBuilder a partir de una lista de bytes.
    /// </summary>
    /// <remarks>
    /// NO es lo mismo que usar System.Text.Encoding.GetString(bytes).ToCharArray();
    /// ya que las funciones de enconding eliminan los bytes que no pueden ser representados.
    /// </remarks>
    public static StringBuilder JoinBytes(this StringBuilder stringBuilder, IEnumerable<byte> bytes)
        => bytes
            .Select(b => (char)b)
            .Aggregate(stringBuilder, (stringBuilder, c) => stringBuilder.Append(c));

    public static ExpandoObject ToExpando<TSource>(this IEnumerable<TSource> source, Func<TSource, string> propertyNameSelector, Func<TSource, object> propertyValueSelector)
        => source.ToDictionary(propertyNameSelector, propertyValueSelector).ToExpando();

    public static void WriteToStream(this IEnumerable<string> source, StreamWriter streamWriter)
        {
        foreach (string element in source)
            {
            streamWriter.Write(element);
            streamWriter.WriteLine();
            }
        }

    public static async Task WriteToStreamAsync(this IAsyncEnumerable<string> source, StreamWriter streamWriter)
        {
        await foreach (string element in source)
            {
            await streamWriter.WriteAsync(element);
            await streamWriter.WriteLineAsync();
            }
        }

    public static void WriteToStream(this IEnumerable<char> source, StreamWriter streamWriter)
        {
        foreach (char element in source)
            {
            streamWriter.Write(element);
            streamWriter.WriteLine();
            }
        }

    public static async Task WriteToStreamAsync(this IAsyncEnumerable<char> source, StreamWriter streamWriter)
        {
        await foreach (char element in source)
            {
            await streamWriter.WriteAsync(element);
            await streamWriter.WriteLineAsync();
            }
        }

    /// <summary>
    /// Convierte los items de una lista en un string y los guarda en una linea de un archivo de texto.
    /// </summary>
    /// <context name="source">IEnumerable a convertir.</context>
    /// <context name="fileFullName">Nombre completo del archivo donde guardar los items.</context>
    /// <remarks>Si el archivo existe, los items se agregan al final del mismo.</remarks>
    /// <example>
    /// <![CDATA[
    ///     List<CeldaDeTarjeta> celdas = this.CrearLoteDeTarjetas(cantidad);
    ///
    ///     // CSV
    ///     celdas
    ///     .select(celda => String.Join(";", celda.NroSobre.ToString("0000000000"), celda.NroSerie, celda.Columna, celda.Fila, celda.Valor))
    ///     .ToTextFile(fileFullName);
    ///
    ///     // Fixed-length fields
    ///     celdas
    ///     .select(celda => String.Format("{0:-20}{1:-30}{2:-2}{3:-2}{4:-5}", celda.NroSobre, celda.NroSerie, celda.Columna, celda.Fila, celda.Valor))
    ///     .ToTextFile(fileFullName);
    ///
    /// ]]>
    /// </example>
    public static void AppendToTextFile(this IEnumerable<string> source, string fileFullName)
        {
        using StreamWriter streamWriter = File.AppendText(fileFullName);
        EnumerableExtensions.WriteToStream(source, streamWriter);
        }

    /// <summary>
    /// Convierte los items de una lista en un string y los guarda en una linea de un nuevo archivo de texto.
    /// </summary>
    /// <context name="source">IEnumerable a convertir.</context>
    /// <context name="fileFullName">Nombre completo del archivo donde guardar los items.</context>
    /// <remarks>Si el archivo existe, es sobreescrito.</remarks>
    public static void ToNewTextFile(this IEnumerable<string> source, string fileFullName)
        {
        using StreamWriter streamWriter = File.CreateText(fileFullName);
        EnumerableExtensions.WriteToStream(source, streamWriter);
        }

    public static async Task ToNewTextFileAsync(this IAsyncEnumerable<string> source, string fileFullName)
        {
        await using StreamWriter streamWriter = File.CreateText(fileFullName);
        await EnumerableExtensions.WriteToStreamAsync(source, streamWriter);
        }

    public static async Task ToTextFileAsync(this IAsyncEnumerable<string> source, string fileFullName, FileMode fileMode, FileAccess access, FileShare share, Encoding encoding)
        {
        await using FileStream fileStream = new FileStream(fileFullName, fileMode, access, share);
        await EnumerableExtensions.ToTextFileAsync(source, fileStream, encoding);
        }

    public static async Task ToTextFileAsync(this IAsyncEnumerable<string> source, FileStream fileStream, Encoding encoding)
        {
        await using StreamWriter streamWriter = new StreamWriter(fileStream, encoding, leaveOpen: true);
        await EnumerableExtensions.WriteToStreamAsync(source, streamWriter);
        }

    public static async Task ToTextFileAsync(this IAsyncEnumerable<char> source, string fileFullName, FileMode fileMode, FileAccess access, FileShare share, Encoding encoding)
        {
        await using FileStream fileStream = new FileStream(fileFullName, fileMode, access, share);
        await EnumerableExtensions.ToTextFileAsync(source, fileStream, encoding);
        }

    public static async Task ToTextFileAsync(this IAsyncEnumerable<char> source, FileStream fileStream, Encoding encoding)
        {
        await using StreamWriter streamWriter = new StreamWriter(fileStream, encoding, leaveOpen: true);
        await EnumerableExtensions.WriteToStreamAsync(source, streamWriter);
        }
    }
