
using CoreClassLibrary.Extensions;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Dynamic;

namespace CoreClassLibrary.Extensions
    {
    public static class DictionaryExtensions
        {
        public static IDictionary<object, object?> ToDictionary(this IDictionary dictionary)
            => new Dictionary<object, object?>().Add(dictionary);

        public static IDictionary<object, object?> Add(this IDictionary<object, object?> target, IDictionary source)
            { 
            foreach (DictionaryEntry entry in source)
                {
                target.Add(entry.ToKeyValuePair());
                }
            return target;
            }

        public static KeyValuePair<object, object?> ToKeyValuePair(this DictionaryEntry entry)
            => new KeyValuePair<object, object?>(entry.Key, entry.Value);

        /// <summary>
        /// Convierte un diccionario en un ExpandoObject.
        /// </summary>
        /// <see cref="http://theburningmonk.com/2011/05/idictionarystring-object-to-expandoobject-extension-method/"/>
        public static ExpandoObject ToExpando(this IReadOnlyDictionary<string, object> dictionary)
            {
            ExpandoObject expando = [];

            foreach (KeyValuePair<string, object> pair in dictionary)
                {
                ((IDictionary<string, object>)expando!).Add(pair);
                }

            return expando;
            }

        public static TValue AddOrUpdate<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, TValue value) where TKey : notnull
            => dictionary.AddOrUpdate(key, value, (_, _) => value);

        /// <summary>
        /// Guarda un valor asociado a una clave.
        /// </summary>
        /// <remarks>
        /// Si la clave ya existia, el valor anterior es reemplazado por el nuevo.
        /// </remarks>
        /// <param name="dictionary"></param>
        /// <param name="key">Clave del valor a guardar.</param>
        /// <param name="value">Valor a guardar.</param>
        /// <see cref="System.Collections.Concurrent.ConcurrentDictionary.AddOrUpdate(TKey, TValue, Func{TKey, TValue, TValue})"/>        
        public static TValue AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
            {
            if (!dictionary.ContainsKey(key))
                {
                lock (dictionary)
                    {
                    if (!dictionary.ContainsKey(key))
                        {
                        dictionary.Add(key, value);
                        return value;
                        }
                    }
                }

            return dictionary[key] = value;
            }
        
        /// <summary>
        /// Guarda un valor asociado a una clave.
        /// </summary>
        /// <remarks>
        /// Si la clave ya existia, el valor anterior es reemplazado por el retornado por factory.
        /// </remarks>
        /// <param name="dictionary"></param>
        /// <param name="key">Clave del valor a guardar.</param>
        /// <param name="factory">Proveedor del valor a guardar.</param>
        /// <see cref="System.Collections.Concurrent.ConcurrentDictionary.AddOrUpdate(TKey, Func{TKey, TValue}, Func{TKey, TValue, TValue})"/>
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue?, TValue> factory)
            {
            if (!dictionary.TryGetValue(key, out TValue? value))
                {
                lock (dictionary)
                    {
                    if (!dictionary.TryGetValue(key, out value))
                        {
                        dictionary.Add(key, factory(key, default));
                        return;
                        }
                    }
                }

            dictionary[key] = factory(key, value);
            }

        /// <summary>
        /// Obtiene el valor de una clave o un valor default si la clave no se encuentra.
        /// </summary>
        /// <see cref="System.Collections.Concurrent.ConcurrentDictionary.TryGetValue(TKey, out TValue)"/>
        public static TValue? GetOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
            => dictionary.TryGetValue(key, out TValue? value) ? value : default;

        public static TValue GetOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
            => dictionary.TryGetValue(key, out TValue? value) ? value : defaultValue;

        /// <summary>
        /// Obtiene el valor asociado a una clave. Si la clave no existe la da de alta con el valor por default pasado por parámetro.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key">Clave del valor a recuperar.</param>
        /// <param name="value">Valor a asignar si la clave no existe.</param>
        /// <returns>El valor asociado a la clave.</returns>
        /// <see cref="System.Collections.Concurrent.ConcurrentDictionary.GetOrAdd(TKey, TValue)"/>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
            {
            if (!dictionary.ContainsKey(key))
                {
                lock (dictionary)
                    {
                    if (!dictionary.ContainsKey(key))
                        {
                        dictionary.Add(key, value);
                        }
                    }
                }

            return dictionary[key];
            }

        /// <summary>
        /// Obtiene el valor asociado a una clave. Si la clave no existe la da de alta creandola con el delegado pasado por parametro.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key">Clave del valor a recuperar.</param>
        /// <param name="factory">Delegado para crear el valor a partir de la clave.</param>
        /// <returns>El valor asociado a la clave.</returns>
        /// <see cref="System.Collections.Concurrent.ConcurrentDictionary.GetOrAdd(TKey, Func{TKey, TValue})"/>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> factory)
            {
            if (!dictionary.ContainsKey(key))
                {
                lock (dictionary)
                    {
                    if (!dictionary.ContainsKey(key))
                        {
                        dictionary.Add(key, factory(key));
                        }
                    }
                }

            return dictionary[key];
            }

        /// <summary>
        /// Agregar una clave si no existía previamente.
        /// </summary>
        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> factory)
            {
            if (dictionary.ContainsKey(key))
                {
                lock (dictionary)
                    {
                    if (dictionary.ContainsKey(key))
                        {
                        dictionary.Add(key, factory(key));
                        return true;
                        }
                    }
                }

            return false;
            }
        
        /// <summary>
        /// Agregar una clave si no existía previamente.
        /// </summary>
        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
            {
            if (dictionary.ContainsKey(key))
                {
                lock (dictionary)
                    {
                    if (dictionary.ContainsKey(key))
                        {
                        dictionary.Add(key, value);
                        return true;
                        }
                    }
                }

            return false;
            }
            
        public static bool TryRemove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key) where TKey : notnull
            => dictionary.TryRemove(key, out _);

        /// <summary>
        /// Quita una clave sin retornar el valor.
        /// </summary>
        public static bool TryRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
            {
            if (dictionary.ContainsKey(key))
                {
                lock (dictionary)
                    {
                    if (dictionary.ContainsKey(key))
                        {
                        return dictionary.Remove(key);
                        }
                    }
                }

            return false;
            }

        /// <summary>
        /// Carga un arreglo de pares clave/valor en un diccionario.
        /// </summary>
        /// <see cref="https://mariusgundersen.net/duck-extensions/"/>
        public static void Add<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> pairs)
            {
            foreach (KeyValuePair<TKey, TValue> pair in pairs)
                {
                dictionary.Add(pair.Key, pair.Value);
                }
            }

        /// <summary>
        /// True si contiene al menos una de las claves.
        /// </summary>
        public static bool ContainsAnyKeys<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, IEnumerable<TKey> keys)
            => keys.Any(dictionary.ContainsKey);

        /// <summary>
        /// True si contiene todas las claves.
        /// </summary>
        public static bool ContainsAllKeys<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, IEnumerable<TKey> keys)
            => keys.All(dictionary.ContainsKey);

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<(TKey, TValue)> pairs) where TKey : notnull
            => pairs.ToDictionary(tuple => tuple.Item1, pair => pair.Item2);

        /// <summary>
        /// Convierte un IDictionary en un arreglo de DictionaryEntry.
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static DictionaryEntry[] ToArray(this IDictionary dic)
            {
            DictionaryEntry[] dicEntries = new DictionaryEntry[dic.Count];
            dic.CopyTo(dicEntries, 0);
            return dicEntries;
            }

        public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) where TKey : notnull
            => new ReadOnlyDictionary<TKey, TValue>(dictionary);

        public static void Add<T>(this IDictionary<object, object> dictionary, T item) where T: class
            => dictionary.Add(typeof(T), item);

        public static T Get<T>(this IDictionary<object, object> dictionary)
            => (T) dictionary[typeof(T)];

        public static void TryAdd<T>(this IDictionary<object, object?> dictionary, T item)
            => dictionary.Add(typeof(T), item);

        public static T? TryGet<T>(this IDictionary<object, object?> dictionary) where T: class
            => dictionary[typeof(T)] as T;

        public static T GetRequired<T>(this IDictionary<object, object?> dictionary) where T : class
            => dictionary[typeof(T)] as T ?? throw new InvalidOperationException($"Required item {typeof(T)} not found in dictionary or is null.");

        public static bool Contains<T>(this IDictionary<object, object> dictionary)
            => dictionary.ContainsKey(typeof(T));
        }
    }
