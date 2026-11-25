using CoreClassLibrary.Extensions;
using System.Collections;
using System.Collections.Concurrent;
using System.Dynamic;
using Xunit;

namespace CoreClassLibrary.Tests;

public class DictionaryExtensionsTests
    {
    // ─── GetOrDefault ─────────────────────────────────────────────────────────

    [Fact]
    public void GetOrDefault_ExistingKey_ReturnsValue()
        {
        IReadOnlyDictionary<string, int> dict = new Dictionary<string, int> { ["a"] = 1 };

        Assert.Equal(1, dict.GetOrDefault("a"));
        }

    [Fact]
    public void GetOrDefault_MissingKey_ReturnsDefault()
        {
        IReadOnlyDictionary<string, int> dict = new Dictionary<string, int>();

        Assert.Equal(0, dict.GetOrDefault("missing"));
        }

    [Fact]
    public void GetOrDefault_WithFallback_MissingKey_ReturnsFallback()
        {
        IReadOnlyDictionary<string, int> dict = new Dictionary<string, int>();

        Assert.Equal(99, dict.GetOrDefault("missing", 99));
        }

    [Fact]
    public void GetOrDefault_WithFallback_ExistingKey_ReturnsStoredValue()
        {
        IReadOnlyDictionary<string, int> dict = new Dictionary<string, int> { ["k"] = 7 };

        Assert.Equal(7, dict.GetOrDefault("k", 99));
        }

    // ─── GetOrAdd ────────────────────────────────────────────────────────────

    [Fact]
    public void GetOrAdd_Value_MissingKey_AddsAndReturns()
        {
        Dictionary<string, int> dict = [];

        int result = dict.GetOrAdd("k", 42);

        Assert.Equal(42, result);
        Assert.True(dict.ContainsKey("k"));
        }

    [Fact]
    public void GetOrAdd_Value_ExistingKey_ReturnsExisting()
        {
        Dictionary<string, int> dict = new() { ["k"] = 1 };

        int result = dict.GetOrAdd("k", 99);

        Assert.Equal(1, result);
        }

    [Fact]
    public void GetOrAdd_Factory_MissingKey_AddsAndReturns()
        {
        Dictionary<string, string> dict = [];

        string result = dict.GetOrAdd("k", key => key.ToUpper());

        Assert.Equal("K", result);
        Assert.True(dict.ContainsKey("k"));
        }

    [Fact]
    public void GetOrAdd_Factory_ExistingKey_ReturnsExisting()
        {
        Dictionary<string, string> dict = new() { ["k"] = "original" };

        string result = dict.GetOrAdd("k", key => "new");

        Assert.Equal("original", result);
        }

    // ─── AddOrUpdate ─────────────────────────────────────────────────────────

    [Fact]
    public void AddOrUpdate_Value_NewKey_AddsEntry()
        {
        Dictionary<string, int> dict = [];

        dict.AddOrUpdate("k", 5);

        Assert.Equal(5, dict["k"]);
        }

    [Fact]
    public void AddOrUpdate_Value_ExistingKey_UpdatesEntry()
        {
        Dictionary<string, int> dict = new() { ["k"] = 1 };

        dict.AddOrUpdate("k", 99);

        Assert.Equal(99, dict["k"]);
        }

    [Fact]
    public void AddOrUpdate_Factory_NewKey_AddsWithDefault()
        {
        Dictionary<string, int> dict = [];

        dict.AddOrUpdate("k", (key, old) => old + 10);

        Assert.Equal(10, dict["k"]);
        }

    [Fact]
    public void AddOrUpdate_ConcurrentDictionary_AddsOrReplaces()
        {
        ConcurrentDictionary<string, int> dict = new()
            {
            ["k"] = 1
            };

        int result = dict.AddOrUpdate("k", 99);

        Assert.Equal(99, result);
        }

    // ─── TryRemove ───────────────────────────────────────────────────────────

    [Fact]
    public void TryRemove_ExistingKey_RemovesAndReturnsTrue()
        {
        Dictionary<string, int> dict = new() { ["k"] = 1 };

        bool removed = dict.TryRemove("k");

        Assert.True(removed);
        Assert.False(dict.ContainsKey("k"));
        }

    [Fact]
    public void TryRemove_MissingKey_ReturnsFalse()
        {
        Dictionary<string, int> dict = [];

        bool removed = dict.TryRemove("missing");

        Assert.False(removed);
        }

    [Fact]
    public void TryRemove_ConcurrentDictionary_ExistingKey_ReturnsTrue()
        {
        ConcurrentDictionary<string, int> dict = new()
            {
            ["k"] = 1
            };

        bool removed = dict.TryRemove("k");

        Assert.True(removed);
        Assert.False(dict.ContainsKey("k"));
        }

    // ─── ContainsAnyKeys / ContainsAllKeys ───────────────────────────────────

    [Fact]
    public void ContainsAnyKeys_AtLeastOneMatch_ReturnsTrue()
        {
        IReadOnlyDictionary<string, int> dict = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };

        Assert.True(dict.ContainsAnyKeys(["b", "z"]));
        }

    [Fact]
    public void ContainsAnyKeys_NoMatch_ReturnsFalse()
        {
        IReadOnlyDictionary<string, int> dict = new Dictionary<string, int> { ["a"] = 1 };

        Assert.False(dict.ContainsAnyKeys(["x", "y"]));
        }

    [Fact]
    public void ContainsAllKeys_AllPresent_ReturnsTrue()
        {
        IReadOnlyDictionary<string, int> dict = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };

        Assert.True(dict.ContainsAllKeys(["a", "b"]));
        }

    [Fact]
    public void ContainsAllKeys_OneMissing_ReturnsFalse()
        {
        IReadOnlyDictionary<string, int> dict = new Dictionary<string, int> { ["a"] = 1 };

        Assert.False(dict.ContainsAllKeys(["a", "b"]));
        }

    // ─── ToKeyValuePair / ToDictionary (IDictionary) ─────────────────────────

    [Fact]
    public void ToKeyValuePair_ConvertsEntry()
        {
        DictionaryEntry entry = new("key", "value");

        KeyValuePair<object, object?> kvp = entry.ToKeyValuePair();

        Assert.Equal("key", kvp.Key);
        Assert.Equal("value", kvp.Value);
        }

    [Fact]
    public void ToDictionary_NonGeneric_ConvertsAllEntries()
        {
        Hashtable hashtable = new() { ["a"] = 1, ["b"] = 2 };

        IDictionary<object, object?> dict = hashtable.ToDictionary();

        Assert.Equal(2, dict.Count);
        }

    // ─── Add (IEnumerable<KVP>) ──────────────────────────────────────────────

    [Fact]
    public void Add_KeyValuePairs_AddsAllToTarget()
        {
        Dictionary<string, int> dict = [];
        IEnumerable<KeyValuePair<string, int>> pairs = [new("a", 1), new("b", 2)];

        dict.Add(pairs);

        Assert.Equal(2, dict.Count);
        Assert.Equal(1, dict["a"]);
        Assert.Equal(2, dict["b"]);
        }

    // ─── ToExpando ───────────────────────────────────────────────────────────

    [Fact]
    public void ToExpando_ConvertsToExpandoObject()
        {
        IReadOnlyDictionary<string, object> dict = new Dictionary<string, object> { ["Name"] = "Test", ["Value"] = 42 };

        ExpandoObject expando = dict.ToExpando();

        IDictionary<string, object?> expandoDict = expando;
        Assert.Equal("Test", expandoDict["Name"]);
        Assert.Equal(42, expandoDict["Value"]);
        }

    // ─── Type-keyed helpers ───────────────────────────────────────────────────

    [Fact]
    public void Add_TypeKey_And_Get_TypeKey()
        {
        Dictionary<object, object> dict = [];
        string item = "hello";

        dict.Add(item);
        string result = dict.Get<string>();

        Assert.Equal("hello", result);
        }

    [Fact]
    public void Contains_TypeKey_ExistingType_ReturnsTrue()
        {
        Dictionary<object, object> dict = [];
        dict.Add("hello");

        Assert.True(dict.Contains<string>());
        }

    [Fact]
    public void Contains_TypeKey_MissingType_ReturnsFalse()
        {
        Dictionary<object, object> dict = [];

        Assert.False(dict.Contains<string>());
        }

    [Fact]
    public void TryAdd_TypeKey_NullableDict_AddsEntry()
        {
        Dictionary<object, object?> dict = [];

        dict.TryAdd<string>("hello");

        Assert.Equal("hello", dict[typeof(string)]);
        }

    [Fact]
    public void TryGet_TypeKey_ExistingEntry_ReturnsValue()
        {
        Dictionary<object, object?> dict = [];
        dict[typeof(string)] = "world";

        string? result = dict.TryGet<string>();

        Assert.Equal("world", result);
        }

    [Fact]
    public void TryGet_TypeKey_MissingEntry_ReturnsNull()
        {
        Dictionary<object, object?> dict = [];
        dict[typeof(string)] = null;

        string? result = dict.TryGet<string>();

        Assert.Null(result);
        }

    [Fact]
    public void GetRequired_TypeKey_ExistingEntry_ReturnsValue()
        {
        Dictionary<object, object?> dict = [];
        dict[typeof(string)] = "required";

        string result = dict.GetRequired<string>();

        Assert.Equal("required", result);
        }

    [Fact]
    public void GetRequired_TypeKey_NullEntry_Throws()
        {
        Dictionary<object, object?> dict = [];
        dict[typeof(string)] = null;

        Assert.Throws<InvalidOperationException>(() => dict.GetRequired<string>());
        }

    // ─── ToArray (IDictionary) ────────────────────────────────────────────────

    [Fact]
    public void ToArray_NonGenericDictionary_ReturnsDictionaryEntries()
        {
        Hashtable hashtable = new() { ["x"] = 10 };

        DictionaryEntry[] entries = hashtable.ToArray();

        DictionaryEntry item = Assert.Single(entries);
        Assert.Equal("x", item.Key);
        Assert.Equal(10, item.Value);
        }
    }
