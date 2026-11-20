#pragma warning disable IDE0028 // Simplify collection initialization

using CoreClassLibrary.Collections;
using Xunit;

namespace CoreClassLibrary.Tests;

public class RecentlyUsedListTests
    {
    [Fact]
    public void Indexer_ReturnsItemAtPosition()
        {
        RecentlyUsedList<string> list = new RecentlyUsedList<string>();
        list.Add("first");
        list.Add("second");

        Assert.Equal("first", list[1]);
        }

    [Theory]
    [InlineData(-1)]
    [InlineData(2)]
    public void Indexer_OutOfRange_Throws(int index)
        {
        RecentlyUsedList<string> list = new RecentlyUsedList<string>();
        list.Add("item");

        Assert.Throws<ArgumentOutOfRangeException>(() => list[index]);
        }

    [Fact]
    public void Contains_ReportsWhetherItemIsPresent()
        {
        RecentlyUsedList<string> list = new RecentlyUsedList<string>();
        list.Add("first");

        Assert.Contains("first", list);
        Assert.DoesNotContain("missing", list);
        }

    [Fact]
    public void Indexer_ReturnsItemsFromMostRecentToLeastRecent()
        {
        RecentlyUsedList<string> list = new RecentlyUsedList<string>();
        list.Add("first");
        list.Add("second");
        list.Add("third");

        Assert.Equal(3, list.Count);
        Assert.Equal("third", list[0]);
        Assert.Equal("second", list[1]);
        Assert.Equal("first", list[2]);
        }

    [Fact]
    public void Add_ExistingItem_MovesItToMostRecentWithoutDuplicating()
        {
        RecentlyUsedList<string> list = new RecentlyUsedList<string>();
        list.Add("first");
        list.Add("second");
        list.Add("first");

        Assert.Equal(2, list.Count);
        Assert.Equal("first", list[0]);
        Assert.Equal("second", list[1]);
        }
    }
