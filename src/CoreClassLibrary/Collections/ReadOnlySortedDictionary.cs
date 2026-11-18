using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace CoreClassLibrary.Collections;

public class ReadOnlySortedDictionary<TKey, TValue>(SortedDictionary<TKey, TValue> sortedDictionary) : IReadOnlyDictionary<TKey, TValue> where TKey : notnull
    {
    public ReadOnlySortedDictionary()
        : this([]) { }
    
    public ReadOnlySortedDictionary(IComparer<TKey> comparer) 
        : this(new SortedDictionary<TKey, TValue>(comparer)) { }

    public ReadOnlySortedDictionary(IDictionary<TKey, TValue> dictionary)
        : this(new SortedDictionary<TKey, TValue>(dictionary)) { }

    public ReadOnlySortedDictionary(IDictionary<TKey, TValue> dictionary, IComparer<TKey> comparer)
        : this(new SortedDictionary<TKey, TValue>(dictionary, comparer)) { }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => sortedDictionary.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public int Count => sortedDictionary.Count;

    public bool ContainsKey(TKey key) => sortedDictionary.ContainsKey(key);

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) => sortedDictionary.TryGetValue(key, out value);

    public TValue this[TKey key] => sortedDictionary[key];

    public IEnumerable<TKey> Keys => sortedDictionary.Keys;

    public IEnumerable<TValue> Values => sortedDictionary.Values;
    }
