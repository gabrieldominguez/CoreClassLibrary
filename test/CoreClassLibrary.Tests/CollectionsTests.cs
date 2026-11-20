#pragma warning disable IDE0028 // Simplify collection initialization

using CoreClassLibrary.Collections;
using Xunit;

namespace CoreClassLibrary.Tests;

public class CollectionsTests
    {
    // ─── DuplicateSortedSet ──────────────────────────────────────────────────

    [Fact]
    public void DuplicateSortedSet_AllowsDuplicates()
        {
        DuplicateSortedSet<int> set = [1, 2, 2, 3];

        Assert.Equal(4, set.Count);
        }

    [Fact]
    public void DuplicateSortedSet_MaintainsSortedOrder()
        {
        DuplicateSortedSet<int> set = [3, 1, 2, 1];

        int[] items = [.. set];

        Assert.True(items[0] <= items[1]);
        Assert.True(items[1] <= items[2]);
        Assert.True(items[2] <= items[3]);
        }

    [Fact]
    public void DuplicateSortedSet_FromCollection()
        {
        DuplicateSortedSet<int> set = new DuplicateSortedSet<int>([5, 3, 3, 1]);

        Assert.Equal(4, set.Count);
        Assert.Equal(1, set.Min);
        Assert.Equal(5, set.Max);
        }

    [Fact]
    public void DuplicateSortedSet_Empty()
        {
        DuplicateSortedSet<int> set = [];

        Assert.Empty(set);
        }

    // ─── ReadOnlySortedDictionary ────────────────────────────────────────────

    [Fact]
    public void ReadOnlySortedDictionary_DefaultConstructor_IsEmpty()
        {
        ReadOnlySortedDictionary<string, int> dict = new();

        Assert.Empty(dict);
        Assert.Empty(dict.Keys);
        Assert.Empty(dict.Values);
        }

    [Fact]
    public void ReadOnlySortedDictionary_FromDictionary()
        {
        Dictionary<string, int> source = new() { ["b"] = 2, ["a"] = 1, ["c"] = 3 };
        ReadOnlySortedDictionary<string, int> dict = new(source);

        Assert.Equal(3, dict.Count);
        Assert.Equal(["a", "b", "c"], dict.Keys);
        }

    [Fact]
    public void ReadOnlySortedDictionary_Indexer_ReturnsValue()
        {
        ReadOnlySortedDictionary<string, int> dict = new(new Dictionary<string, int> { ["x"] = 42 });

        Assert.Equal(42, dict["x"]);
        }

    [Fact]
    public void ReadOnlySortedDictionary_ContainsKey()
        {
        ReadOnlySortedDictionary<string, int> dict = new(new Dictionary<string, int> { ["k"] = 1 });

        Assert.True(dict.ContainsKey("k"));
        Assert.False(dict.ContainsKey("missing"));
        }

    [Fact]
    public void ReadOnlySortedDictionary_TryGetValue()
        {
        ReadOnlySortedDictionary<string, int> dict = new(new Dictionary<string, int> { ["k"] = 99 });

        Assert.True(dict.TryGetValue("k", out int val));
        Assert.Equal(99, val);
        Assert.False(dict.TryGetValue("no", out int missing));
        Assert.Equal(0, missing);
        }

    [Fact]
    public void ReadOnlySortedDictionary_Enumerate()
        {
        ReadOnlySortedDictionary<int, string> dict = new(new Dictionary<int, string> { [2] = "B", [1] = "A" });

        KeyValuePair<int, string>[] pairs = [.. dict];

        Assert.Equal(1, pairs[0].Key);
        Assert.Equal(2, pairs[1].Key);
        }

    [Fact]
    public void ReadOnlySortedDictionary_WithComparer()
        {
        ReadOnlySortedDictionary<string, int> dict = new(StringComparer.OrdinalIgnoreCase);

        Assert.Empty(dict);
        }

    [Fact]
    public void ReadOnlySortedDictionary_WithDictionaryAndComparer()
        {
        Dictionary<string, int> source = new() { ["B"] = 2, ["a"] = 1 };
        ReadOnlySortedDictionary<string, int> dict = new(source, StringComparer.OrdinalIgnoreCase);

        Assert.Equal(2, dict.Count);
        }

    // ─── EmptyReadOnlyDictionary ─────────────────────────────────────────────

    [Fact]
    public void EmptyReadOnlyDictionary_IsEmpty()
        {
        IReadOnlyDictionary<string, int> dict = EmptyReadOnlyDictionary<string, int>.Value;

        Assert.Empty(dict);
        }

    [Fact]
    public void EmptyReadOnlyDictionary_SameInstance()
        {
        IReadOnlyDictionary<string, int> a = EmptyReadOnlyDictionary<string, int>.Value;
        IReadOnlyDictionary<string, int> b = EmptyReadOnlyDictionary<string, int>.Value;

        Assert.Same(a, b);
        }

    // ─── UndoRedoDataStack ───────────────────────────────────────────────────

    [Fact]
    public void UndoRedoDataStack_InvalidCapacityThrows()
        {
        Assert.Throws<ArgumentOutOfRangeException>(() => new UndoRedoDataStack<int>(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new UndoRedoDataStack<int>(-1));
        }

    [Fact]
    public void UndoRedoDataStack_DoSetsCurrentValue()
        {
        UndoRedoDataStack<int> stack = new(10);

        stack.Do(42);

        Assert.Equal(42, stack.Current);
        }

    [Fact]
    public void UndoRedoDataStack_CanUndo_AfterMultipleDos()
        {
        UndoRedoDataStack<int> stack = new(10);

        stack.Do(1);
        stack.Do(2);
        stack.Do(3);

        Assert.True(stack.CanUndo);
        }

    [Fact]
    public void UndoRedoDataStack_Undo_RestoresPreviousValue()
        {
        UndoRedoDataStack<int> stack = new(10);

        stack.Do(1);
        stack.Do(2);
        stack.Do(3);

        stack.Undo();

        Assert.Equal(2, stack.Current);
        }

    [Fact]
    public void UndoRedoDataStack_Undo_ThenRedo()
        {
        UndoRedoDataStack<int> stack = new(10);

        stack.Do(1);
        stack.Do(2);

        stack.Undo();
        Assert.Equal(1, stack.Current);
        Assert.True(stack.CanRedo);

        stack.Redo();
        Assert.Equal(2, stack.Current);
        }

    [Fact]
    public void UndoRedoDataStack_DoAfterUndo_ClearsRedo()
        {
        UndoRedoDataStack<int> stack = new(10);

        stack.Do(1);
        stack.Do(2);
        stack.Undo();

        stack.Do(3);

        Assert.False(stack.CanRedo);
        Assert.Equal(3, stack.Current);
        }

    [Fact]
    public void UndoRedoDataStack_Clear_ResetsEverything()
        {
        UndoRedoDataStack<int> stack = new(10);

        stack.Do(1);
        stack.Do(2);
        stack.Clear();

        Assert.False(stack.CanUndo);
        Assert.False(stack.CanRedo);
        Assert.Equal(default, stack.Current);
        }

    [Fact]
    public void UndoRedoDataStack_CurrentChangedEvent_Fires()
        {
        UndoRedoDataStack<int> stack = new(10);
        int eventCount = 0;
        stack.CurrentChanged += (_, _) => eventCount++;

        stack.Do(1);
        stack.Do(2);
        stack.Undo();

        Assert.Equal(3, eventCount);
        }

    [Fact]
    public void UndoRedoDataStack_UndoWhenEmpty_ReturnsDefault()
        {
        UndoRedoDataStack<int?> stack = new(10);

        int? result = stack.Undo();

        Assert.Equal(default, result);
        }

    [Fact]
    public void UndoRedoDataStack_RedoWhenEmpty_ReturnsDefault()
        {
        UndoRedoDataStack<int?> stack = new(10);

        int? result = stack.Redo();

        Assert.Equal(default, result);
        }

    [Fact]
    public void UndoRedoDataStack_SetCapacity()
        {
        UndoRedoDataStack<int> stack = new(10);

        stack.Do(1);
        stack.Do(2);
        stack.SetCapacity(5);

        Assert.True(stack.CanUndo);
        }

    [Fact]
    public void UndoRedoDataStack_SameSameValueNotPushedTwice()
        {
        UndoRedoDataStack<int> stack = new(10);

        stack.Do(1);
        stack.Do(1); // duplicate – should be ignored

        Assert.False(stack.CanUndo);
        }

    // ─── UndoRedoOperationStack ──────────────────────────────────────────────

    [Fact]
    public void UndoRedoOperationStack_InvalidCapacityThrows()
        {
        Assert.Throws<ArgumentOutOfRangeException>(() => new UndoRedoOperationStack<string>(0));
        }

    [Fact]
    public void UndoRedoOperationStack_DoThenUndo()
        {
        UndoRedoOperationStack<string> stack = new(10);

        stack.Do("A");
        string? result = stack.Undo();

        Assert.Equal("A", result);
        }

    [Fact]
    public void UndoRedoOperationStack_DoThenUndoThenRedo()
        {
        UndoRedoOperationStack<string> stack = new(10);

        stack.Do("A");
        stack.Do("B");

        stack.Undo();
        string? redone = stack.Redo();

        Assert.Equal("B", redone);
        }

    [Fact]
    public void UndoRedoOperationStack_CanUndo_CanRedo_Flags()
        {
        UndoRedoOperationStack<string> stack = new(10);

        Assert.False(stack.CanUndo);
        Assert.False(stack.CanRedo);

        stack.Do("A");
        Assert.True(stack.CanUndo);
        Assert.False(stack.CanRedo);

        stack.Undo();
        Assert.False(stack.CanUndo);
        Assert.True(stack.CanRedo);
        }

    [Fact]
    public void UndoRedoOperationStack_DoAfterUndo_ClearsRedo()
        {
        UndoRedoOperationStack<string> stack = new(10);

        stack.Do("A");
        stack.Undo();
        stack.Do("B");

        Assert.False(stack.CanRedo);
        }

    [Fact]
    public void UndoRedoOperationStack_Clear_FiresEvent()
        {
        UndoRedoOperationStack<string> stack = new(10);
        bool cleared = false;
        stack.Cleared += (_, _) => cleared = true;

        stack.Do("A");
        stack.Clear();

        Assert.True(cleared);
        Assert.False(stack.CanUndo);
        }

    [Fact]
    public void UndoRedoOperationStack_Did_Undid_Redid_EventsFire()
        {
        UndoRedoOperationStack<string> stack = new(10);
        List<string> log = [];

        stack.Did   += (_, e) => log.Add($"did:{e.Item}");
        stack.Undid += (_, e) => log.Add($"undid:{e.Item}");
        stack.Redid += (_, e) => log.Add($"redid:{e.Item}");

        stack.Do("X");
        stack.Undo();
        stack.Redo();

        Assert.Equal(["did:X", "undid:X", "redid:X"], log);
        }

    [Fact]
    public void UndoRedoOperationStack_UndoWhenEmpty_ReturnsDefault()
        {
        UndoRedoOperationStack<int?> stack = new(5);

        int? result = stack.Undo();

        Assert.Equal(default, result);
        }

    [Fact]
    public void UndoRedoOperationStack_RedoWhenEmpty_ReturnsDefault()
        {
        UndoRedoOperationStack<int> stack = new(5);
        stack.Do(1);

        stack.Undo();
        stack.Undo(); // already empty redo stack after first undo
        int? result = stack.Redo();

        // After one Undo there is one item in redo; second Undo is no-op.
        // After Redo the item is back in undo, redo is now empty.
        Assert.Equal(1, result);
        }

    [Fact]
    public void UndoRedoOperationStack_SetCapacity()
        {
        UndoRedoOperationStack<int> stack = new(10);

        stack.Do(1);
        stack.SetCapacity(5);

        Assert.True(stack.CanUndo);
        }

    [Fact]
    public void UndoRedoOperationStack_EventArgsDefaultItem()
        {
        UndoRedoOperationStack<string>.UndoRedoEventArgs args = new();

        Assert.Null(args.Item);
        }

    [Fact]
    public void UndoRedoOperationStack_EventArgsWithItem()
        {
        UndoRedoOperationStack<string>.UndoRedoEventArgs args = new("hello");

        Assert.Equal("hello", args.Item);
        }
    }
