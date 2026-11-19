#pragma warning disable IDE0056 // Use index operator

using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects;
using Xunit;

namespace CoreClassLibrary.Tests;

/// <summary>
/// Range Tests
/// </summary>
/// <remarks>
/// In all cases, the range operator for Array allocates an array to store the elements returned.
///
/// Memory<T>.ToArray() => Span<T>.ToArray();
///
/// Copies the contents from the memory into a new array.  This heap
/// allocates, so should generally be avoided, however it is sometimes
/// necessary to bridge the gap with APIs written in terms of arrays.
/// </remarks>
/// <see cref="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/member-access-operators#range-operator-"/>
/// <see cref="https://learn.microsoft.com/en-us/dotnet/csharp/tutorials/ranges-indexes"/>
/// <see cref="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/collection-expressions"/>
public class RangeTests(ITestOutputHelper output)
    {
    private void Write(int[] values, Range range)
        => output.WriteLine($"{range}:\t{string.Join(", ", values[range])}");

    [Fact]
    public void Range()
        { 
        Assert.Equal(3..5, new Range(3, 5));
        Assert.Equal(3..5, new Range(new Index(3), 5));
        Assert.Equal(3..5, new Range(new Index(3), new Index(5)));
        Assert.Equal(3..^5, new Range(new Index(3), new Index(5, true)));
        }

    [Fact]
    public void RangeTest1()
        {
        int[] numbers = [0, 10, 20, 30, 40, 50];

        int start = 1;
        int amountToTake = 3;

        this.Write(numbers, start..(start + amountToTake));
        Assert.Equal((int[])[10, 20, 30], numbers[start..(start + amountToTake)]);

        int margin = 1;
        this.Write(numbers, margin..^margin);
        Assert.Equal((int[])[10, 20, 30, 40], numbers[margin..^margin]);

        string line = "one two three";
        int amountToTakeFromEnd = 5;
        Range endIndices = ^amountToTakeFromEnd..^0;
        string end = line[endIndices];
        Assert.Equal("three", end);
        output.WriteLine(end);

        int amountToDrop = numbers.Length / 2;

        this.Write(numbers, amountToDrop..);
        Assert.Equal((int[])[30, 40, 50], numbers[amountToDrop..]);

        this.Write(numbers, ..^amountToDrop);
        Assert.Equal((int[])[0, 10, 20], numbers[..^amountToDrop]);

        this.Write(numbers, ..);
        Assert.Equal((int[])[0, 10, 20, 30, 40, 50], numbers[..]);
        }

    [Fact]
    public void RangeTest2()
        {
        int[] oneThroughTen = [ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 ];

        this.Write(oneThroughTen, ..);
        Assert.Equal((int[])[1, 2, 3, 4, 5, 6, 7, 8, 9, 10], oneThroughTen[..]);

        this.Write(oneThroughTen, ..3);
        Assert.Equal((int[])[1, 2, 3], oneThroughTen[..3]);

        this.Write(oneThroughTen, 2..);
        Assert.Equal((int[])[3, 4, 5, 6, 7, 8, 9, 10], oneThroughTen[2..]);

        this.Write(oneThroughTen, 3..5);
        Assert.Equal((int[])[4, 5], oneThroughTen[3..5]);

        this.Write(oneThroughTen, ^2..);
        Assert.Equal((int[])[9, 10], oneThroughTen[^2..]);

        this.Write(oneThroughTen, ..^3);
        Assert.Equal((int[])[1, 2, 3, 4, 5, 6, 7], oneThroughTen[..^3]);

        this.Write(oneThroughTen, 3..^4);
        Assert.Equal((int[])[4, 5, 6], oneThroughTen[3..^4]);

        this.Write(oneThroughTen, ^4..^2);
        Assert.Equal((int[])[7, 8], oneThroughTen[^4..^2]);
        }

    [Fact]
    public void RangeTest3()
        {
        string[] words = [
                        // index from start    index from end
            "The",      // 0                   ^9
            "quick",    // 1                   ^8
            "brown",    // 2                   ^7
            "fox",      // 3                   ^6
            "jumps",    // 4                   ^5
            "over",     // 5                   ^4
            "the",      // 6                   ^3
            "lazy",     // 7                   ^2
            "dog"       // 8                   ^1
        ];              // 9 (or words.Length) ^0

        Assert.Equal("dog", words[^1]);
        Assert.Equal((string[])[ "quick", "brown", "fox" ], words[1..4]);
        Assert.Equal((string[])[ "lazy" ], words[^2..^1]);
        Assert.Equal((string[])[ "lazy", "dog" ], words[^2..^0]);
        Assert.Equal((string[])[ "lazy", "dog" ], words[^2..]);
        Assert.Equal(words, words[..]);
        Assert.Equal((string[])[ "The", "quick", "brown", "fox" ], words[..4]);
        Assert.Equal((string[])[ "the", "lazy", "dog" ], words[6..]);
        }
    
    [Fact]
    public void RangeTest4()
        {
        int[] numbers = [..Enumerable.Range(0, 100)];
        int x = 12;
        int y = 25;
        int z = 36;

        Assert.Equal(numbers[^x], numbers[numbers.Length - x]);
        Assert.Equal(numbers[x..y].Length, y - x);

        output.WriteLine("numbers[x..y] and numbers[y..z] are consecutive and disjoint:");
        Assert.Equal(numbers[x..z], numbers[x..y].Concat(numbers[y..z]));
        Assert.Equal<int[]>(numbers[x..z], [..numbers[x..y], ..numbers[y..z]]);

        Span<int> x_y = numbers.AsSpan()[x..y];
        Span<int> y_z = numbers.AsSpan()[y..z];
        output.WriteLine($"\tnumbers[x..y] is {x_y[0]} through {x_y[^1]}, numbers[y..z] is {y_z[0]} through {y_z[^1]}");

        output.WriteLine("");

        output.WriteLine("numbers[x..^x] removes x elements at each end:");
        Span<int> x_x = numbers.AsSpan()[x..^x];
        output.WriteLine($"\tnumbers[x..^x] starts with {x_x[0]} and ends with {x_x[^1]}");

        output.WriteLine("");

        output.WriteLine("numbers[..x] means numbers[0..x] and numbers[x..] means numbers[x..^0]");
        Span<int> start_x = numbers.AsSpan()[..x];
        Span<int> zero_x = numbers.AsSpan()[0..x];
        output.WriteLine($"\t{start_x[0]}..{start_x[^1]} is the same as {zero_x[0]}..{zero_x[^1]}");
        Span<int> z_end = numbers.AsSpan()[z..];
        Span<int> z_zero = numbers.AsSpan()[z..^0];
        output.WriteLine($"\t{z_end[0]}..{z_end[^1]} is the same as {z_zero[0]}..{z_zero[^1]}");
        }
    
    [Fact]
    public void RangeTest5()
        {
        int[][] jagged = 
        [
           [0, 1, 2, 3, 4, 5, 6, 7, 8, 9],
           [10,11,12,13,14,15,16,17,18,19],
           [20,21,22,23,24,25,26,27,28,29],
           [30,31,32,33,34,35,36,37,38,39],
           [40,41,42,43,44,45,46,47,48,49],
           [50,51,52,53,54,55,56,57,58,59],
           [60,61,62,63,64,65,66,67,68,69],
           [70,71,72,73,74,75,76,77,78,79],
           [80,81,82,83,84,85,86,87,88,89],
           [90,91,92,93,94,95,96,97,98,99],
        ];

        List<int[]> list = [];
        int[][] selectedRows = jagged[3..^3];       
        foreach (int[] row in selectedRows)
            {
            int[] selectedColumns = row[2..^2];
            list.Add(selectedColumns);
            foreach (int cell in selectedColumns)
                {
                output.WriteLine($"{cell}, ");
                }
            }

        Assert.Equal(        
           [
           [30,31,32,33,34,35,36,37,38,39],
           [40,41,42,43,44,45,46,47,48,49],
           [50,51,52,53,54,55,56,57,58,59],
           [60,61,62,63,64,65,66,67,68,69],
           ], jagged[3..^3]);
        
        Assert.Equal([], jagged[3..^3][2..^2]);
        
        Assert.Equal(        
           [
           [32,33,34,35,36,37],
           [42,43,44,45,46,47],
           [52,53,54,55,56,57],
           [62,63,64,65,66,67],
           ], list);
    }

    [Fact]
    public void ArrayRange()
        {
        int[] array = [1, 2, 3, 4];
        Assert.Equal((int[])[1, 2], array[0..2]);
        Assert.Equal((int[])[3, 4], array[2..4]);
        Assert.Equal((int[])[3, 4], array[2..]);

        Assert.Throws<ArgumentOutOfRangeException>(() => array[2..5]);
        }

    [Fact]
    public void CopyArrayRange()
        {
        int[] array1 = [1, 2, 3, 4];
        int[] array2 = array1[..];   // Range operator COPY the array

        array1[0] = 5;

        Assert.Equal(5, array1[0]);
        Assert.Equal(1, array2[0]);
        }

    [Fact]
    public void CopyArrayRangeTest2()
        {
        // When taking a range from an array, the result is an array that is copied from the initial array, rather than referenced. Modifying values in the resulting array will not change values in the initial array.
        int[] arrayOfFiveItems = [1, 2, 3, 4, 5];

        int[] firstThreeItems = arrayOfFiveItems[..3]; // contains 1,2,3
        firstThreeItems[0] =  11; // now contains 11,2,3

        output.WriteLine(string.Join(",", firstThreeItems));
        Assert.Equal((int[])[11, 2, 3], firstThreeItems);
        Assert.Equal(stackalloc[] { 11, 2, 3 }, (ReadOnlySpan<int>) firstThreeItems);

        output.WriteLine(string.Join(",", arrayOfFiveItems));
        Assert.Equal((int[])[1, 2, 3, 4, 5], arrayOfFiveItems);
        Assert.Equal(stackalloc[] { 1, 2, 3, 4, 5 }, (ReadOnlySpan<int>) arrayOfFiveItems);
        }

    [Fact]
    public void SpanRange()
        {
        Span<int> span1 = [1, 2, 3, 4];
        Span<int> span2 = span1[..];   // Range operator NOT COPY the Span

        span1[0] = 5;

        Assert.Equal(5, span1[0]);
        Assert.Equal(5, span2[0]);
        }

    [Fact]
    public void ArraySpanRange()
        {
        int[] array = [1, 2, 3, 4];
        Span<int> span1 = array.AsSpan();
        Span<int> span2 = span1[..];   // Range operator NOT COPY the Span

        span1[0] = 5;

        Assert.Equal(5, span1[0]);
        Assert.Equal(5, span2[0]);
        Assert.Equal(5, array[0]);
        }
    
    [Fact]
    public void MemoryRange()
        {
        int[] array = [1, 2, 3, 4];
        Memory<int> memory1 = array.AsMemory();
        Memory<int> memory2 = memory1[..];   // Range operator NOT COPY the Memory

        memory1.Span[0] = 5;

        Assert.Equal(5, memory1.Span[0]);
        Assert.Equal(5, memory2.Span[0]);
        Assert.Equal(5, array[0]);
        }

    [Fact]
    public void AsSpan()
        {
        int[] array = [1, 2, 3, 4];
        Span<int> span = array.AsSpan();

        Assert.Equal([3, 4], array.AsSpan(2, 2));
        Assert.Equal([], span[2..2]);
        Assert.Equal([3], span[2..3]);
        Assert.Equal([3, 4], span[2..4]);

        Assert.Equal([2, 3], array.AsSpan(1, 2));
        Assert.Equal([2, 3], span[1..3]);
        }

    [Fact]
    public void ArrayReader()
        {
        MemoryReader<int> arrayReader = new MemoryReader<int>([ 2, 1, 5, 4, 3 ]);

        Assert.Equal(2, arrayReader.GetNextItem());
        Assert.Equal(1, arrayReader.GetNextItem());
        Assert.Equal([5, 4], arrayReader.GetNextMemory(2).Span);
        Assert.Equal(3, arrayReader.GetNextItem());
        }
    }