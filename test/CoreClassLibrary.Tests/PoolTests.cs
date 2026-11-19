using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Pooling;
using System.Buffers;
using System.Collections.Immutable;
using Xunit;

namespace CoreClassLibrary.Tests;

public class PoolTests(ITestOutputHelper output)
    {
    [Fact]
    public void Array()
        {
        const int length = 10;
        int[] array = new int[length];

        Assert.Equal(length, array.Length);

        array[9] = 5;

        Assert.Equal(5, array[9]);
        }
    
    [Fact]
    public void TestImmutableArray()
        {
        const int length = 10;
        int[] array = new int[length];
        ImmutableArray<int> immutableArray = ImmutableArray.Create(array);

        Assert.Equal(length, array.Length);
        Assert.Equal(length, immutableArray.Length);

        array[9] = 5;
        //immutableArray[8] = 5;

        Assert.Equal(5, array[9]);
        Assert.Equal(0, immutableArray[9]);
        }
    
    [Fact]
    public void Span()
        {
        const int length = 10;
        Span<int> stackArray = stackalloc int[length];

        Assert.Equal(length, stackArray.Length);

        stackArray[9] = 5;

        Assert.Equal(5, stackArray[9]);
        }

    [Fact]
    public void ReadOnlySpan()
        {
        const int length = 10;
        Span<int> stackArray = stackalloc int[length];
        ReadOnlySpan<int> readOnlyArray = stackArray;

        Assert.Equal(length, stackArray.Length);
        Assert.Equal(length, readOnlyArray.Length);

        stackArray[9] = 5;
        //readOnlyArray[8] = 5;

        Assert.Equal(5, stackArray[9]);
        Assert.Equal(5, readOnlyArray[9]);
        }

    /*
    [Fact]
    public void PooledArray()
        {
        const int length = 10;
        using PooledArray<int> pooledArray = new PooledArray<int>(length);

        Assert.True(length <= pooledArray.Array.Length);
        Assert.Equal(length, pooledArray.Memory.Length);
        Assert.Equal(length, pooledArray.Memory.Span.Length);

        pooledArray.Array[9] = 5;

        Assert.Equal(5, pooledArray.Array[9]);
        }
    */
    [Fact]
    public void PooledMemory()
        {
        const int length = 10;
        using PooledMemory<int> pooledMemory = new PooledMemory<int>(length);

        Assert.Equal(length, pooledMemory.Memory.Length);
        Assert.Equal(length, pooledMemory.Memory.Span.Length);

        pooledMemory.Memory.Span[9] = 5;

        Assert.Equal(5, pooledMemory.Memory.Span[9]);
        }

    [Fact]
    public void MemoryPoolOwner()
        {
        const int length = 10;
        using IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.Rent(length);
        
        Assert.True(length <= memoryOwner.Memory.Length);
        Assert.True(length <= memoryOwner.Memory.Span.Length);
        Assert.Equal(memoryOwner.Memory.Length, memoryOwner.Memory.Span.Length);

        memoryOwner.Memory.Span[9] = 5;

        Assert.Equal(5, memoryOwner.Memory.Span[9]);
        }

    
    [Fact]
    public void ReusedPooledMemory()
        {
        const int length = 10;
        using (PooledMemory<int> pooledMemory = new PooledMemory<int>(length))
            {
            output.WriteEnumerable<int>(pooledMemory.Memory.AsEnumerable());
            pooledMemory.Memory.Span.Fill(1);
            output.WriteEnumerable<int>(pooledMemory.Memory.AsEnumerable());
            Assert.True(IsUsed(pooledMemory.Memory));
            }

        using (PooledMemory<int> pooledMemory = new PooledMemory<int>(length))
            {
            output.WriteEnumerable<int>(pooledMemory.Memory.AsEnumerable());
            Assert.True(IsUsed(pooledMemory.Memory));
            }

        using (PooledMemory<int> pooledMemory = new PooledMemory<int>(length-1))
            {
            output.WriteEnumerable<int>(pooledMemory.Memory.AsEnumerable());
            Assert.True(IsUsed(pooledMemory.Memory));
            }

        using (PooledMemory<int> pooledMemory = new PooledMemory<int>(length+1))
            {
            output.WriteEnumerable<int>(pooledMemory.Memory.AsEnumerable());
            Assert.True(IsUsed(pooledMemory.Memory));
            }

        static bool IsUsed(Memory<int> memory)
            => memory.Span[0] != 0;
        }
    
    [Fact]
    public void ReusedPooledArray()
        {
        const int length = 10;
        using (PooledArray<int> pooledMemory = new PooledArray<int>(length))
            {
            output.WriteEnumerable<int>(pooledMemory.Memory.AsEnumerable());
            pooledMemory.Memory.Span.Fill(1);
            output.WriteEnumerable<int>(pooledMemory.Memory.AsEnumerable());
            Assert.True(IsUsed(pooledMemory.Memory));
            }

        using (PooledArray<int> pooledMemory = new PooledArray<int>(length))
            {
            output.WriteEnumerable<int>(pooledMemory.Memory.AsEnumerable());
            Assert.True(IsUsed(pooledMemory.Memory));
            }

        using (PooledArray<int> pooledMemory = new PooledArray<int>(length-1))
            {
            output.WriteEnumerable<int>(pooledMemory.Memory.AsEnumerable());
            Assert.True(IsUsed(pooledMemory.Memory));
            }

        using (PooledArray<int> pooledMemory = new PooledArray<int>(length+1))
            {
            output.WriteEnumerable<int>(pooledMemory.Memory.AsEnumerable());
            Assert.True(IsUsed(pooledMemory.Memory));
            }

        static bool IsUsed(Memory<int> memory)
            => memory.Span[0] == 1;
        }
    
    [Fact]
    public void PooledList()
        {
        using PooledList<int> pooledList = new PooledList<int>();

        Assert.Empty(pooledList.List);

        pooledList.List.Add(1);

        Assert.Single(pooledList.List);

        pooledList.List.Add(2);

        Assert.Equal(2, pooledList.List.Count);
        }

    [Fact]
    public void PooledStringBuilder()
        {
        using PooledStringBuilder builder = new PooledStringBuilder();
        
        Assert.Equal(0, builder.StringBuilder.Length);

        builder.StringBuilder.Append('a');

        Assert.Equal(1, builder.StringBuilder.Length);

        Assert.Equal('a', builder.StringBuilder[0]);

        Assert.Equal("a", builder.StringBuilder.ToString());
        }

    [Fact]
    public void DisposedPooledStringBuilder()
        {
        PooledStringBuilder builder = new PooledStringBuilder();
        
        builder.StringBuilder.Append('a');

        Assert.Equal("a", builder.StringBuilder.ToString());

        builder.Dispose();

        Assert.Equal("a", builder.StringBuilder.ToString());
        }

    [Fact]
    public void ValueListBuilder()
        {
        using ValueListBuilder<int> valueListBuilder = new ValueListBuilder<int>();
        
        Assert.Equal(0, valueListBuilder.Length);

        valueListBuilder.Append(1);

        Assert.Equal(1, valueListBuilder.Length);

        valueListBuilder.Append(2);

        Assert.Equal(2, valueListBuilder.Length);
        }
    
    [Fact]
    public void ValueStringBuilder()
        {
        using ValueStringBuilder valueStringBuilder = new ValueStringBuilder();
        
        Assert.Equal(0, valueStringBuilder.Length);

        valueStringBuilder.Append('a');

        Assert.Equal(1, valueStringBuilder.Length);

        Assert.Equal("a", valueStringBuilder.ToString());
        }
    
    [Fact]
    public void ValueStringBuilderToString()
        {
        using ValueStringBuilder valueStringBuilder = new ValueStringBuilder();
        
        valueStringBuilder.Append('a');

        Assert.Equal("a", valueStringBuilder.ToString());

        valueStringBuilder.Append('b');

        Assert.Equal("ab", valueStringBuilder.ToString());
        Assert.Equal("ab", valueStringBuilder.ToString());
        }
    
    [Fact]
    public void ValueStringBuilderSpan()
        {
        Span<char> span = stackalloc char[10];

        using ValueStringBuilder valueStringBuilder = new ValueStringBuilder(span);
        
        Assert.Equal(0, valueStringBuilder.Length);

        valueStringBuilder.Append('a');

        Assert.Equal(1, valueStringBuilder.Length);

        Assert.Equal("a", valueStringBuilder.ToString());
        }

    [Fact]
    public void ValueStringBuilderMemory()
        {
        using PooledMemory<char> pooledMemory = new PooledMemory<char>(10);
        using ValueStringBuilder valueStringBuilder = new ValueStringBuilder(pooledMemory.Memory.Span);
        
        Assert.Equal(0, valueStringBuilder.Length);

        valueStringBuilder.Append('a');

        Assert.Equal(1, valueStringBuilder.Length);

        Assert.Equal("a", valueStringBuilder.ToString());
        }
    }
