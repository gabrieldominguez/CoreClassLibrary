using CoreClassLibrary.Objects;
using Xunit;

namespace CoreClassLibrary.Tests;

public class ObjectsTests
    {
    // ─── AtomicCounter ───────────────────────────────────────────────────────

    [Fact]
    public void AtomicCounter_InitialValue_IsZero()
        {
        AtomicCounter counter = new();

        Assert.Equal(0, counter.Value);
        }

    [Fact]
    public void AtomicCounter_Increment_IncreasesValueByOne()
        {
        AtomicCounter counter = new();

        int result = counter.Increment();

        Assert.Equal(1, result);
        Assert.Equal(1, counter.Value);
        }

    [Fact]
    public void AtomicCounter_Decrement_DecreasesValueByOne()
        {
        AtomicCounter counter = new();
        counter.Increment();

        int result = counter.Decrement();

        Assert.Equal(0, result);
        Assert.Equal(0, counter.Value);
        }

    [Fact]
    public void AtomicCounter_Decrement_BelowZero_IsNegative()
        {
        AtomicCounter counter = new();

        counter.Decrement();

        Assert.Equal(-1, counter.Value);
        }

    [Fact]
    public void AtomicCounter_Reset_SetsValueToZero()
        {
        AtomicCounter counter = new();
        counter.Increment();
        counter.Increment();
        counter.Increment();

        counter.Reset();

        Assert.Equal(0, counter.Value);
        }

    [Fact]
    public void AtomicCounter_ValueSetter_SetsArbitraryValue()
        {
        AtomicCounter counter = new AtomicCounter()
            {
            Value = 42
            };

        Assert.Equal(42, counter.Value);
        }

    [Fact]
    public void AtomicCounter_MultipleIncrements_AccumulatesCorrectly()
        {
        AtomicCounter counter = new();

        for (int i = 0; i < 10; i++)
            counter.Increment();

        Assert.Equal(10, counter.Value);
        }

    [Fact]
    public void AtomicCounter_IncrementDecrement_ReturnsToZero()
        {
        AtomicCounter counter = new();

        counter.Increment();
        counter.Increment();
        counter.Decrement();
        counter.Decrement();

        Assert.Equal(0, counter.Value);
        }

    [Fact]
    public void AtomicCounter_ConcurrentIncrements_AreThreadSafe()
        {
        AtomicCounter counter = new();
        const int threadCount = 10;
        const int incrementsPerThread = 1000;

        Thread[] threads = new Thread[threadCount];
        for (int i = 0; i < threadCount; i++)
            threads[i] = new Thread(() =>
                {
                for (int j = 0; j < incrementsPerThread; j++)
                    counter.Increment();
                });

        foreach (Thread t in threads) t.Start();
        foreach (Thread t in threads) t.Join();

        Assert.Equal(threadCount * incrementsPerThread, counter.Value);
        }

    // ─── StringParser ────────────────────────────────────────────────────────

    [Fact]
    public void StringParser_GetNextChar_ReturnsFirstCharAndAdvances()
        {
        StringParser parser = new("ABC");

        char c = parser.GetNextChar();

        Assert.Equal('A', c);
        Assert.Equal('B', parser.PeekNextChar());
        }

    [Fact]
    public void StringParser_PeekNextChar_DoesNotAdvance()
        {
        StringParser parser = new("XYZ");

        char c1 = parser.PeekNextChar();
        char c2 = parser.PeekNextChar();

        Assert.Equal('X', c1);
        Assert.Equal('X', c2);
        }

    [Fact]
    public void StringParser_GetChar_AtIndex_SetsNextIndex()
        {
        StringParser parser = new("ABCDE");

        char c = parser.GetChar(2);

        Assert.Equal('C', c);
        Assert.Equal('D', parser.PeekNextChar());
        }

    [Fact]
    public void StringParser_GetNextString_ReturnsSubstringAndAdvances()
        {
        StringParser parser = new("Hello World");

        string s = parser.GetNextString(5);

        Assert.Equal("Hello", s);
        Assert.Equal(' ', parser.PeekNextChar());
        }

    [Fact]
    public void StringParser_PeekNextString_DoesNotAdvance()
        {
        StringParser parser = new("Hello World");

        string s1 = parser.PeekNextString(5);
        string s2 = parser.PeekNextString(5);

        Assert.Equal("Hello", s1);
        Assert.Equal("Hello", s2);
        }

    [Fact]
    public void StringParser_GetString_AtIndex_ReturnsCorrectSubstring()
        {
        StringParser parser = new("Hello World");

        string s = parser.GetString(6, 5);

        Assert.Equal("World", s);
        }

    [Fact]
    public void StringParser_SetNextString_SetsOutRef()
        {
        StringParser parser = new("Hello World");

        parser.SetNextString(5, out string result);

        Assert.Equal("Hello", result);
        }

    [Fact]
    public void StringParser_SetString_AtIndex_SetsOutRef()
        {
        StringParser parser = new("Hello World");

        parser.SetString(6, 5, out string result);

        Assert.Equal("World", result);
        }

    [Fact]
    public void StringParser_SetNextString_ReturnsParser_AllowsChaining()
        {
        StringParser parser = new("AABBCC");

        parser
            .SetNextString(2, out string part1)
            .SetNextString(2, out string part2)
            .SetNextString(2, out string part3);

        Assert.Equal("AA", part1);
        Assert.Equal("BB", part2);
        Assert.Equal("CC", part3);
        }

    [Fact]
    public void StringParser_SkipNextChar_AdvancesOne()
        {
        StringParser parser = new("ABC");

        parser.SkipNextChar();

        Assert.Equal('B', parser.PeekNextChar());
        }

    [Fact]
    public void StringParser_SkipNextString_AdvancesByLength()
        {
        StringParser parser = new("SKIP_REST");

        parser.SkipNextString(5);

        Assert.Equal('R', parser.PeekNextChar());
        }

    [Fact]
    public void StringParser_SkipNextChar_ReturnsParser_AllowsChaining()
        {
        StringParser parser = new("ABCDE");

        parser.SkipNextChar().SkipNextChar();

        Assert.Equal('C', parser.PeekNextChar());
        }

    [Fact]
    public void StringParser_StartIndex_BeginsAtOffset()
        {
        StringParser parser = new("Hello World", 6);

        string s = parser.GetNextString(5);

        Assert.Equal("World", s);
        }

    // ─── TypeNameHelper ──────────────────────────────────────────────────────

    [Fact]
    public void TypeNameHelper_NullItem_ReturnsNull()
        {
        string? result = TypeNameHelper.GetTypeDisplayName((object?)null);

        Assert.Null(result);
        }

    [Fact]
    public void TypeNameHelper_BuiltInType_ReturnsAlias()
        {
        string name = TypeNameHelper.GetTypeDisplayName(typeof(int), fullName: false);

        Assert.Equal("int", name);
        }

    [Fact]
    public void TypeNameHelper_BuiltInTypes_AllAliases()
        {
        Assert.Equal("bool",    TypeNameHelper.GetTypeDisplayName(typeof(bool),    fullName: false));
        Assert.Equal("string",  TypeNameHelper.GetTypeDisplayName(typeof(string),  fullName: false));
        Assert.Equal("long",    TypeNameHelper.GetTypeDisplayName(typeof(long),    fullName: false));
        Assert.Equal("double",  TypeNameHelper.GetTypeDisplayName(typeof(double),  fullName: false));
        Assert.Equal("decimal", TypeNameHelper.GetTypeDisplayName(typeof(decimal), fullName: false));
        Assert.Equal("object",  TypeNameHelper.GetTypeDisplayName(typeof(object),  fullName: false));
        }

    [Fact]
    public void TypeNameHelper_SimpleType_ShortName()
        {
        string name = TypeNameHelper.GetTypeDisplayName(typeof(Exception), fullName: false);

        Assert.Equal("Exception", name);
        }

    [Fact]
    public void TypeNameHelper_SimpleType_FullName()
        {
        string name = TypeNameHelper.GetTypeDisplayName(typeof(Exception), fullName: true);

        Assert.Equal("System.Exception", name);
        }

    [Fact]
    public void TypeNameHelper_GenericType_ShortName()
        {
        string name = TypeNameHelper.GetTypeDisplayName(typeof(List<int>), fullName: false);

        Assert.Equal("List<int>", name);
        }

    [Fact]
    public void TypeNameHelper_GenericType_FullName()
        {
        string name = TypeNameHelper.GetTypeDisplayName(typeof(List<int>), fullName: true);

        Assert.Equal("System.Collections.Generic.List<int>", name);
        }

    [Fact]
    public void TypeNameHelper_GenericType_TwoArgs_ShortName()
        {
        string name = TypeNameHelper.GetTypeDisplayName(typeof(Dictionary<string, int>), fullName: false);

        Assert.Equal("Dictionary<string, int>", name);
        }

    [Fact]
    public void TypeNameHelper_ArrayType_ShortName()
        {
        string name = TypeNameHelper.GetTypeDisplayName(typeof(int[]), fullName: false);

        Assert.Equal("int[]", name);
        }

    [Fact]
    public void TypeNameHelper_MultiDimensionalArray_ShortName()
        {
        string name = TypeNameHelper.GetTypeDisplayName(typeof(int[,]), fullName: false);

        Assert.Equal("int[,]", name);
        }

    [Fact]
    public void TypeNameHelper_FromObject_UsesRuntimeType()
        {
        string? name = TypeNameHelper.GetTypeDisplayName(42, fullName: false);

        Assert.Equal("int", name);
        }

    [Fact]
    public void TypeNameHelper_NoGenericParameters_OmitsAngleBrackets()
        {
        string name = TypeNameHelper.GetTypeDisplayName(typeof(List<int>), fullName: false, includeGenericParameters: false);

        Assert.Equal("List", name);
        }
    }
