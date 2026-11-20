using CoreClassLibrary.Extensions;
using CoreClassLibrary.Extensions.Paging;
using CoreClassLibrary.Objects.Ranges;
using System.Buffers;
using System.Text;
using Xunit;

namespace CoreClassLibrary.Tests;

public class ExtensionsTests
    {
    // ─── EnumExtensions ──────────────────────────────────────────────────────

    [Flags]
    private enum Perm { None = 0, Read = 1, Write = 2, Execute = 4 }

    [Fact]
    public void Enum_Append_AddsBit()
        {
        Perm p = Perm.Read;

        p = p.Append(Perm.Write);

        Assert.True(p.Has(Perm.Read));
        Assert.True(p.Has(Perm.Write));
        }

    [Fact]
    public void Enum_Remove_ClearsBit()
        {
        Perm p = Perm.Read.Append(Perm.Write);

        p = p.Remove(Perm.Read);

        Assert.False(p.Has(Perm.Read));
        Assert.True(p.Has(Perm.Write));
        }

    [Fact]
    public void Enum_Has_ReturnsTrueForPresentBit()
        {
        Perm p = Perm.Read.Append(Perm.Execute);

        Assert.True(p.Has(Perm.Read));
        Assert.True(p.Has(Perm.Execute));
        Assert.False(p.Has(Perm.Write));
        }

    [Fact]
    public void Enum_Has_CombinedFlag()
        {
        Perm p = Perm.Read.Append(Perm.Write).Append(Perm.Execute);
        Perm combined = Perm.Read.Append(Perm.Write);

        Assert.True(p.Has(combined));
        }

    [Fact]
    public void Enum_Parse_ReturnsCorrectValue()
        {
        Perm result = EnumExtensions.Parse<Perm>("Write");

        Assert.Equal(Perm.Write, result);
        }

    [Fact]
    public void Enum_Parse_CaseInsensitive()
        {
        Perm result = EnumExtensions.Parse<Perm>("write");

        Assert.Equal(Perm.Write, result);
        }

    [Fact]
    public void Enum_TryParse_ValidValue_ReturnsCorrect()
        {
        Perm result = EnumExtensions.TryParse<Perm>("Execute");

        Assert.Equal(Perm.Execute, result);
        }

    [Fact]
    public void Enum_TryParse_InvalidValue_ReturnsDefault()
        {
        Perm result = EnumExtensions.TryParse<Perm>("InvalidXYZ");

        Assert.Equal(default, result);
        }

    // ─── ListExtensions ──────────────────────────────────────────────────────

    [Fact]
    public void RemoveLast_DefaultCount_RemovesOneItem()
        {
        List<int> list = [1, 2, 3, 4];

        list.RemoveLast();

        Assert.Equal([1, 2, 3], list);
        }

    [Fact]
    public void RemoveLast_CountTwo_RemovesTwoItems()
        {
        List<int> list = [1, 2, 3, 4];

        list.RemoveLast(2);

        Assert.Equal([1, 2], list);
        }

    [Fact]
    public void GetLast_ReturnsLastElement()
        {
        IReadOnlyList<int> list = [10, 20, 30];

        Assert.Equal(30, list.GetLast());
        }

    [Fact]
    public void GetValueOrDefault_List_ValidIndex_ReturnsElement()
        {
        IReadOnlyList<string> list = ["a", "b", "c"];

        Assert.Equal("b", list.GetValueOrDefault(1));
        }

    [Fact]
    public void GetValueOrDefault_List_OutOfRange_ReturnsDefault()
        {
        IReadOnlyList<string> list = ["a", "b"];

        Assert.Null(list.GetValueOrDefault(5));
        Assert.Null(list.GetValueOrDefault(-1));
        }

    [Fact]
    public void GetValueOrDefault_Span_ValidIndex_ReturnsElement()
        {
        int[] arr = [10, 20, 30];
        ReadOnlySpan<int> span = arr;

        Assert.Equal(20, span.GetValueOrDefault(1));
        }

    [Fact]
    public void GetValueOrDefault_Span_OutOfRange_ReturnsDefault()
        {
        int[] arr = [10, 20];
        ReadOnlySpan<int> span = arr;

        Assert.Equal(0, span.GetValueOrDefault(10));
        Assert.Equal(0, span.GetValueOrDefault(-1));
        }

    [Fact]
    public void AsReadOnly_ReturnsReadOnlyCollection()
        {
        List<int> list = [1, 2, 3];

        System.Collections.ObjectModel.ReadOnlyCollection<int> ro = list.AsReadOnly();

        Assert.Equal(3, ro.Count);
        Assert.Equal(2, ro[1]);
        }

    [Fact]
    public void List_Add_Spread_AddsEnumerable()
        {
        List<int> list = [1, 2];

        list.Add((IEnumerable<int>)[3, 4, 5]);

        Assert.Equal([1, 2, 3, 4, 5], list);
        }

    [Fact]
    public void GetLengthRange_ReturnsZeroBasedRange()
        {
        IReadOnlyList<string> list = ["a", "b", "c"];

        Range<int> range = list.GetLengthRange();

        Assert.Equal(0, range.Start);
        Assert.Equal(2, range.End);
        }

    // ─── TupleExtensions ─────────────────────────────────────────────────────

    [Fact]
    public void Swap_ClassicTuple_SwapsItems()
        {
        Tuple<int, int> t = Tuple.Create(1, 2);

        Tuple<int, int> swapped = t.Swap();

        Assert.Equal(2, swapped.Item1);
        Assert.Equal(1, swapped.Item2);
        }

    [Fact]
    public void Swap_ValueTuple_SwapsItems()
        {
        (string a, string b) t = ("hello", "world");

        (string, string) swapped = t.Swap();

        Assert.Equal("world", swapped.Item1);
        Assert.Equal("hello", swapped.Item2);
        }

    [Fact]
    public void Enumerate_Tuple2_YieldsBothItems()
        {
        Tuple<int, int> t = Tuple.Create(10, 20);

        int[] items = [.. t.Enumerate()];

        Assert.Equal([10, 20], items);
        }

    [Fact]
    public void Enumerate_Tuple3_YieldsAllThreeItems()
        {
        Tuple<string, string, string> t = Tuple.Create("A", "B", "C");

        string[] items = [.. t.Enumerate()];

        Assert.Equal(["A", "B", "C"], items);
        }

    // ─── ArrayExtensions ─────────────────────────────────────────────────────

    [Fact]
    public void GetBoundRange_Returns0BasedRange()
        {
        int[] arr = new int[5];

        Range<int> range = arr.GetBoundRange();

        Assert.Equal(0, range.Start);
        Assert.Equal(4, range.End);
        }

    [Fact]
    public void CreateArray_ReturnsArrayFromParams()
        {
        int[] arr = ArrayExtensions.CreateArray(1, 2, 3);

        Assert.Equal([1, 2, 3], arr);
        }

    [Fact]
    public void Clear_ArrayExtension_SetsAllToDefault()
        {
        int[] arr = [1, 2, 3];

        arr.Clear();

        Assert.All(arr, x => Assert.Equal(0, x));
        }

    [Fact]
    public void Clear_WithTransform_AppliesTransformThenClears()
        {
        int[] arr = [1, 2, 3];

        int[] result = arr.Clear(a => [.. a.Select(x => x * 2)]);

        Assert.Equal([2, 4, 6], result);
        Assert.All(arr, x => Assert.Equal(0, x));
        }

    [Fact]
    public void CloneArray_ReturnsIndependentCopy()
        {
        int[] original = [1, 2, 3];

        int[] clone = original.CloneArray();
        clone[0] = 99;

        Assert.Equal(1, original[0]);
        Assert.Equal(99, clone[0]);
        }

    [Fact]
    public void GetGenericEnumerator_IteratesAllItems()
        {
        string[] arr = ["x", "y", "z"];

        List<string> collected = [];
        using IEnumerator<string> enumerator = arr.GetGenericEnumerator();
        while (enumerator.MoveNext())
            collected.Add(enumerator.Current);

        Assert.Equal(["x", "y", "z"], collected);
        }

    [Fact]
    public void RentUseReturn_Action_ReceivesArrayAndReturnsIt()
        {
        ArrayPool<int> pool = ArrayPool<int>.Shared;
        int length = 0;

        pool.RentUseReturn(16, arr => length = arr.Length);

        Assert.True(length >= 16);
        }

    [Fact]
    public void RentUseReturn_Func_ReturnsResult()
        {
        ArrayPool<int> pool = ArrayPool<int>.Shared;

        int result = pool.RentUseReturn(8, arr => arr.Length);

        Assert.True(result >= 8);
        }

    // ─── StringBuilderExtensions ─────────────────────────────────────────────

    [Fact]
    public void ToFormatString_ProducesFormatItem()
        {
        string result = "000".ToFormatString();

        Assert.Equal("{0:000}", result);
        }

    [Fact]
    public void ToFormatString_WithIndex_UsesIndex()
        {
        string result = "0.00".ToFormatString(2);

        Assert.Equal("{2:0.00}", result);
        }

    [Fact]
    public void RepeatAppend_String_RepeatsCorrectly()
        {
        string result = "AB".RepeatAppend(3);

        Assert.Equal("ABABAB", result);
        }

    [Fact]
    public void AppendIf_ConditionTrue_AppendsValue()
        {
        StringBuilder sb = new();

        sb.AppendIf(true, "yes");

        Assert.Equal("yes", sb.ToString());
        }

    [Fact]
    public void AppendIf_ConditionFalse_DoesNotAppend()
        {
        StringBuilder sb = new();

        sb.AppendIf(false, "no");

        Assert.Empty(sb.ToString());
        }

    [Fact]
    public void AppendIf_Factory_ConditionTrue_AppendsFactoryResult()
        {
        StringBuilder sb = new();

        sb.AppendIf(true, () => "computed");

        Assert.Equal("computed", sb.ToString());
        }

    [Fact]
    public void AppendLineIf_ConditionTrue_AppendsLineWithValue()
        {
        StringBuilder sb = new();

        sb.AppendLineIf(true, "line");

        Assert.StartsWith("line", sb.ToString());
        }

    [Fact]
    public void AppendLineIf_Factory_ConditionTrue_AppendsLine()
        {
        StringBuilder sb = new();

        sb.AppendLineIf(true, () => "computed");

        Assert.StartsWith("computed", sb.ToString());
        }

    [Fact]
    public void AppendLine_Object_AppendsWithNewline()
        {
        StringBuilder sb = new();

        sb.AppendLine((object)42);

        Assert.StartsWith("42", sb.ToString());
        }

    [Fact]
    public void AppendKeyValueString_FormatsCorrectly()
        {
        StringBuilder sb = new();
        Dictionary<string, int> dict = new() { ["a"] = 1, ["b"] = 2 };

        sb.AppendKeyValueString(dict);

        string result = sb.ToString();
        Assert.Contains("a = 1", result);
        Assert.Contains("b = 2", result);
        Assert.Contains(", ", result);
        }

    [Fact]
    public void AppendDelimitedStrings_JoinsWithSeparator()
        {
        StringBuilder sb = new();

        sb.AppendDelimitedStrings(["A", "B", "C"], "-");

        Assert.Equal("A-B-C", sb.ToString());
        }

    [Fact]
    public void AppendStrings_AppendsAll()
        {
        StringBuilder sb = new();

        sb.AppendStrings(["X", "Y", "Z"]);

        Assert.Equal("XYZ", sb.ToString());
        }

    [Fact]
    public void Remove_PositiveStartIndex_RemovesFromStart()
        {
        StringBuilder sb = new("ABCDEF");

        sb.Remove(2);

        Assert.Equal("AB", sb.ToString());
        }

    [Fact]
    public void Remove_NegativeStartIndex_RemovesFromEnd()
        {
        StringBuilder sb = new("ABCDEF");

        sb.Remove(-2);

        Assert.Equal("ABCD", sb.ToString());
        }

    [Fact]
    public void RemoveLast_PositiveStartIndex_RemovesRange()
        {
        StringBuilder sb = new("ABCDEF");

        sb.RemoveLast(1, 3);

        Assert.Equal("AEF", sb.ToString());
        }

    [Fact]
    public void RemoveLast_NegativeStartIndex_RemovesFromEnd()
        {
        StringBuilder sb = new("ABCDEF");

        sb.RemoveLast(-3, 2);

        Assert.Equal("ABCF", sb.ToString());
        }

    [Fact]
    public void RemoveLast_EmptyStringBuilder_ReturnsEmpty()
        {
        StringBuilder sb = new();

        sb.RemoveLast(0, 5);

        Assert.Empty(sb.ToString());
        }

    // ─── ObjectExtensions ────────────────────────────────────────────────────

    [Fact]
    public void Configure_AppliesAction()
        {
        List<int> list = [1, 2, 3];

        list.Configure(l => l.Add(4));

        Assert.Contains(4, list);
        }

    [Fact]
    public void Configure_MultipleActions_AllApplied()
        {
        List<int> list = [];

        list.Configure((IEnumerable<Action<List<int>>>)[l => l.Add(1), l => l.Add(2)]);

        Assert.Equal([1, 2], list);
        }

    [Fact]
    public void Configure_WithContext_PassesContext()
        {
        int[] result = [0];

        result.Configure(42, (ctx, arr) => arr[0] = ctx);

        Assert.Equal(42, result[0]);
        }

    [Fact]
    public void ConfigureIf_ConditionTrue_AppliesAction()
        {
        List<int> list = [];

        list.ConfigureIf(true, l => l.Add(99));

        Assert.Contains(99, list);
        }

    [Fact]
    public void ConfigureIf_ConditionFalse_SkipsAction()
        {
        List<int> list = [];

        list.ConfigureIf(false, l => l.Add(99));

        Assert.Empty(list);
        }

    [Fact]
    public void ConfigureIfNotNull_NullAction_SkipsWithoutThrow()
        {
        List<int> list = [1];

        list.ConfigureIfNotNull(null);

        Assert.Single(list);
        }

    [Fact]
    public void Swap_ExchangesValues()
        {
        int a = 1;
        int b = 2;

        ObjectExtensions.Swap(ref a, ref b);

        Assert.Equal(2, a);
        Assert.Equal(1, b);
        }

    [Fact]
    public void IsNullOrEmptyCollection_Null_ReturnsTrue()
        => Assert.True(ObjectExtensions.IsNullOrEmptyCollection<string>(null));

    [Fact]
    public void IsNullOrEmptyCollection_EmptyList_ReturnsTrue()
        => Assert.True(ObjectExtensions.IsNullOrEmptyCollection<List<int>>([]));

    [Fact]
    public void IsNullOrEmptyCollection_NonEmptyList_ReturnsFalse()
        => Assert.False(ObjectExtensions.IsNullOrEmptyCollection<List<int>>([1]));

    [Fact]
    public void GetLazy_NonNullStorage_ReturnsExistingValue()
        {
        string? storage = "existing";

        string result = ObjectExtensions.GetLazy(ref storage, () => "new");

        Assert.Equal("existing", result);
        }

    [Fact]
    public void Min_ReturnsSmaller()
        {
        Assert.Equal(3, ObjectExtensions.Min(3, 7));
        Assert.Equal(3, ObjectExtensions.Min(7, 3));
        }

    [Fact]
    public void Max_ReturnsLarger()
        {
        Assert.Equal(7, ObjectExtensions.Max(3, 7));
        Assert.Equal(7, ObjectExtensions.Max(7, 3));
        }

    [Fact]
    public void TryCastAndInvoke_SuccessfulCast_InvokesAction()
        {
        object obj = "hello";
        string? captured = null;

        obj.TryCastAndInvoke<string>(s => captured = s);

        Assert.Equal("hello", captured);
        }

    [Fact]
    public void TryCastAndInvoke_FailedCast_DoesNotInvoke()
        {
        object obj = 42;
        string? captured = null;

        obj.TryCastAndInvoke<string>(s => captured = s);

        Assert.Null(captured);
        }

    [Fact]
    public void CreateAndConfigure_Action_ReturnsConfiguredInstance()
        {
        List<int> result = ObjectExtensions.CreateAndConfigure<List<int>>(l => l.Add(1));

        Assert.Equal(1, Assert.Single(result));
        }

    [Fact]
    public void CreateAndConfigure_Actions_ReturnsConfiguredInstance()
        {
        List<int> result = ObjectExtensions.CreateAndConfigure<List<int>>(
            (IEnumerable<Action<List<int>>>)[l => l.Add(1), l => l.Add(2)]);

        Assert.Equal([1, 2], result);
        }

    [Fact]
    public void CreateFactory_CreatesInstance()
        {
        Func<object> factory = typeof(List<int>).CreateFactory();

        object instance = factory();

        Assert.IsType<List<int>>(instance);
        }

    [Fact]
    public void CreateInstance_SingleArg_CreatesInstance()
        {
        // Use a type with single-arg constructor — StringBuilder(int)
        StringBuilder sb = ObjectExtensions.CreateInstance<StringBuilder>(10);

        Assert.NotNull(sb);
        }

    // ─── PagingExtensions ────────────────────────────────────────────────────

    [Fact]
    public void Paginate_Enumerable_ReturnsCorrectPage()
        {
        IEnumerable<int> source = Enumerable.Range(1, 10);

        int[] page = [.. source.Paginate(1, 3)];

        Assert.Equal([4, 5, 6], page);
        }

    [Fact]
    public void Paginate_Enumerable_FirstPage()
        {
        IEnumerable<int> source = Enumerable.Range(1, 10);

        int[] page = [.. source.Paginate(0, 4)];

        Assert.Equal([1, 2, 3, 4], page);
        }

    [Fact]
    public void Paginate_Enumerable_PageSizeZero_ReturnsAll()
        {
        IEnumerable<int> source = Enumerable.Range(1, 5);

        int[] all = [.. source.Paginate(0, 0)];

        Assert.Equal([1, 2, 3, 4, 5], all);
        }

    [Fact]
    public void Paginate_Queryable_WithPageRequest_ReturnsPageResponse()
        {
        IQueryable<int> query = Enumerable.Range(1, 10).AsQueryable();
        PageRequest request = new(1, 3);

        PageResponse<int> response = query.Paginate(request);

        Assert.Equal(10, response.TotalCount);
        Assert.Equal([4, 5, 6], response.Items);
        }

    [Fact]
    public void Paginate_Queryable_NoPageRequest_ReturnsAll()
        {
        IQueryable<int> query = Enumerable.Range(1, 5).AsQueryable();

        PageResponse<int> response = query.Paginate();

        Assert.Equal(5, response.TotalCount);
        Assert.Equal([1, 2, 3, 4, 5], response.Items);
        }

    [Fact]
    public void PageRequest_PropertiesRoundtrip()
        {
        PageRequest req = new(2, 10);

        Assert.Equal(2, req.PageNumber);
        Assert.Equal(10, req.PageSize);
        }

    [Fact]
    public void PageResponse_PropertiesRoundtrip()
        {
        PageResponse<string> resp = new(3, ["a", "b", "c"]);

        Assert.Equal(3, resp.TotalCount);
        Assert.Equal(["a", "b", "c"], resp.Items);
        }
    }
