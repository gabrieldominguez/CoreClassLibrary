namespace CoreClassLibrary.Collections;

public static class EmptyReadOnlyDictionary<TKey, TValue> where TKey : notnull
    {
    public static readonly IReadOnlyDictionary<TKey, TValue> Value = new Dictionary<TKey, TValue>();
    }
