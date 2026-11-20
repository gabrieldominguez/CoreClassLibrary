using CoreClassLibrary.Extensions;
using Xunit;

namespace CoreClassLibrary.Tests;

public class CollectionExtensionsTests
    {
    // ─── CollectionExtensions ────────────────────────────────────────────────

    [Fact]
    public void AddUnique_AddsNewValue_ReturnsTrue()
        {
        List<int> list = [1, 2, 3];

        bool added = list.AddUnique(4);

        Assert.True(added);
        Assert.Contains(4, list);
        }

    [Fact]
    public void AddUnique_DuplicateValue_ReturnsFalse()
        {
        List<int> list = [1, 2, 3];

        bool added = list.AddUnique(2);

        Assert.False(added);
        Assert.Equal(3, list.Count);
        }

    [Fact]
    public void AddRangeUnique_ReturnsCountOfAddedItems()
        {
        List<int> list = [1, 2, 3];

        int count = list.AddRangeUnique([2, 4, 5]);

        Assert.Equal(2, count);
        Assert.Equal(5, list.Count);
        }

    [Fact]
    public void AddRangeUnique_NoneAdded_ReturnsZero()
        {
        List<int> list = [1, 2, 3];

        int count = list.AddRangeUnique([1, 2, 3]);

        Assert.Equal(0, count);
        Assert.Equal(3, list.Count);
        }

    [Fact]
    public void AddRange_AddsAllItems()
        {
        List<int> list = [1, 2];

        list.AddRange([3, 4, 5]);

        Assert.Equal([1, 2, 3, 4, 5], list);
        }

    [Fact]
    public void AddRange_EmptySource_NoChange()
        {
        List<int> list = [1, 2];

        list.AddRange([]);

        Assert.Equal(2, list.Count);
        }

    [Fact]
    public void Contains_InnerGroupContained_ReturnsTrue()
        {
        List<int> outer = [1, 2, 3, 4];
        List<int> inner = [2, 3];

        Assert.True(outer.Contains(inner));
        }

    [Fact]
    public void Contains_InnerGroupNotContained_ReturnsFalse()
        {
        List<int> outer = [1, 2, 3];
        List<int> inner = [2, 5];

        Assert.False(outer.Contains(inner));
        }

    [Fact]
    public void ContainsDistinct_InnerGroupWithDuplicates_IsContained()
        {
        List<int> outer = [1, 2, 3];
        List<int> inner = [2, 2, 3]; // 2 appears twice

        Assert.True(outer.ContainsDistinct(inner));
        }

    [Fact]
    public void ContainsDistinct_InnerGroupNotContained_ReturnsFalse()
        {
        List<int> outer = [1, 2, 3];
        List<int> inner = [2, 2, 9];

        Assert.False(outer.ContainsDistinct(inner));
        }

    [Fact]
    public void SymmetricDifference_ReturnsElementsInOnlyOneSet()
        {
        List<int> a = [1, 2, 3];
        List<int> b = [2, 3, 4];

        int[] diff = [.. a.SymmetricDifference(b).Order()];

        Assert.Equal([1, 4], diff);
        }

    [Fact]
    public void SymmetricDifference_SameSets_ReturnsEmpty()
        {
        List<int> a = [1, 2, 3];
        List<int> b = [1, 2, 3];

        Assert.Empty(a.SymmetricDifference(b));
        }

    [Fact]
    public void SymmetricDifference_DisjointSets_ReturnsAll()
        {
        List<int> a = [1, 2];
        List<int> b = [3, 4];

        int[] diff = [.. a.SymmetricDifference(b).Order()];

        Assert.Equal([1, 2, 3, 4], diff);
        }

    // ─── QueueStackExtensions ────────────────────────────────────────────────

    [Fact]
    public void TryDequeue_NonEmptyQueue_ReturnsFirstItem()
        {
        Queue<int> queue = new();
        queue.Enqueue(10);
        queue.Enqueue(20);

        int? result = queue.TryDequeue();

        Assert.Equal(10, result);
        Assert.Single(queue);
        }

    [Fact]
    public void TryDequeue_EmptyQueue_ReturnsDefault()
        {
        Queue<int> queue = new();

        int? result = queue.TryDequeue();

        Assert.Equal(default(int), result);
        }

    [Fact]
    public void TryDequeue_EmptyReferenceTypeQueue_ReturnsNull()
        {
        Queue<string> queue = new();

        string? result = queue.TryDequeue();

        Assert.Null(result);
        }

    [Fact]
    public void TryPeek_Queue_NonEmpty_ReturnsFirstWithoutRemoving()
        {
        Queue<int> queue = new();
        queue.Enqueue(5);
        queue.Enqueue(10);

        int? result = queue.TryPeek();

        Assert.Equal(5, result);
        Assert.Equal(2, queue.Count);
        }

    [Fact]
    public void TryPeek_Queue_Empty_ReturnsDefault()
        {
        Queue<int> queue = new();

        int? result = queue.TryPeek();

        Assert.Equal(default(int), result);
        }

    [Fact]
    public void TryPop_NonEmptyStack_ReturnsTopItem()
        {
        Stack<int> stack = new();
        stack.Push(1);
        stack.Push(2);

        int? result = stack.TryPop();

        Assert.Equal(2, result);
        Assert.Single(stack);
        }

    [Fact]
    public void TryPop_EmptyStack_ReturnsDefault()
        {
        Stack<int> stack = new();

        int? result = stack.TryPop();

        Assert.Equal(default(int), result);
        }

    [Fact]
    public void TryPeek_Stack_NonEmpty_ReturnsTopWithoutRemoving()
        {
        Stack<string> stack = new();
        stack.Push("first");
        stack.Push("second");

        string? result = stack.TryPeek();

        Assert.Equal("second", result);
        Assert.Equal(2, stack.Count);
        }

    [Fact]
    public void TryPeek_Stack_Empty_ReturnsNull()
        {
        Stack<string> stack = new();

        string? result = stack.TryPeek();

        Assert.Null(result);
        }
    }
