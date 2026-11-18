#pragma warning disable IDE0028 // Simplify collection initialization

using CoreClassLibrary.Extensions;
using System.Collections.Concurrent;

namespace CoreClassLibrary.Extensions;

public static class ConcurrentDictionaryExtensions
    {       
    /// <summary>
    /// Obtiene el valor asociado a una clave. Si la clave no existe la da de alta con el valor default del tipo.
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="key">Clave del valor a recuperar.</param>
    /// <remarks>Este método está restringido a TValue que son por referencia.</remarks>
    /// <returns>El valor asociado a la clave.</returns>
    public static TValue GetOrAddDefault<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key) where TKey : notnull
        => dictionary.GetOrAdd(key, default(TValue)!);

    /// <summary>
    /// Obtiene el valor asociado a una clave. Si la clave no existe la da de alta con el valor construído con el constructor sin parámetros.
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="key">Clave del valor a recuperar.</param>
    /// <remarks>
    /// Este método está restringido a TValue que son por referencia.
    /// 
    /// El constructor de TValue puede ejecutarse más de una vez (aunque solo un resultado se agregara al diccionario).
    /// https://andrewlock.net/making-getoradd-on-concurrentdictionary-thread-safe-using-lazy/
    /// </remarks>
    /// <returns>El valor asociado a la clave.</returns>
    public static TValue GetOrAdd<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key)  where TKey : notnull where TValue : class, new()
        => dictionary.GetOrAdd(key, new TValue());

    public static ConcurrentDictionary<TKey, TSource> ToConcurrentDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) where TKey : notnull
        => new ConcurrentDictionary<TKey, TSource>(source.ToDictionary(keySelector));
    }
