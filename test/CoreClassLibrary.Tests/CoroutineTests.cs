
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Pooling;
using System.Text;
using Xunit;

namespace CoreClassLibrary.Tests;

/// <summary>
/// Coroutines sample
/// </summary>
/// <see cref="https://github.com/noseratio/coroutines-talk"/>
public class CoroutineTests
    {
    private static IEnumerable<int> CoroutineA(StringBuilder buffer)
        {
        for (int i = 0; i < StringExtensions.UpperCaseAlphaChars.Length; i++)
            {
            buffer.Append(StringExtensions.UpperCaseAlphaChars[i]);
            yield return i;
            }
        }

    private static IEnumerable<int> Coroutine1(StringBuilder buffer)
        {
        for (int i = 0; i < StringExtensions.NumericChars.Length; i++)
            {
            buffer.Append(StringExtensions.NumericChars[i]);
            yield return i;
            }
        }

    [Fact]
    public void CoroutineAll()
        {
        using PooledStringBuilder builder = new PooledStringBuilder();
        EnumerableExtensions.InterleaveAll([ CoroutineA(builder.StringBuilder), Coroutine1(builder.StringBuilder) ]).ForEach(s => { }, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal("A0B1C2D3E4F5G6H7I8J9K", builder.StringBuilder.ToString());
        }

    [Fact]
    public void CoroutineAny()
        {
        using PooledStringBuilder builder = new PooledStringBuilder();
        EnumerableExtensions.InterleaveAny([ CoroutineA(builder.StringBuilder), Coroutine1(builder.StringBuilder) ]).ForEach(s => { }, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal("A0B1C2D3E4F5G6H7I8J9KLMNOPQRSTUVWXYZ", builder.StringBuilder.ToString());
        }
    }
