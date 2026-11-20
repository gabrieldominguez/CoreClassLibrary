using CoreClassLibrary.RegularExpressions;
using Xunit;

namespace CoreClassLibrary.Tests;

public class GrepTests
    {
    [Theory]
    // Exact matches
    [InlineData("hello", "hello", true)]
    [InlineData("hello", "a hello world", true)]
    [InlineData("hello", "hell", false)]

    // Beginning anchor ^
    [InlineData("^hello", "hello world", true)]
    [InlineData("^hello", "say hello", false)]

    // End anchor $
    [InlineData("world$", "hello world", true)]
    [InlineData("world$", "world!", false)]

    // Dot wildcard .
    [InlineData("h.llo", "hello", true)]
    [InlineData("h.llo", "hallo", true)]
    [InlineData("h.llo", "hllo", false)]

    // Star *
    [InlineData("ab*c", "ac", true)]
    [InlineData("ab*c", "abc", true)]
    [InlineData("ab*c", "abbbc", true)]
    [InlineData("ab*c", "abq", false)]
    [InlineData("a*", "", true)]
    [InlineData("a*", "a", true)]
    [InlineData("a*", "aaa", true)]
    [InlineData("a*", "b", true)]

    // Dot + star
    [InlineData("a.*c", "axyzc", true)]
    [InlineData("a.*c", "ac", true)]
    [InlineData("a.*c", "abd", false)]

    // Empty strings
    [InlineData("", "", true)]
    [InlineData("", "anything", true)]
    [InlineData("^$", "", true)]
    [InlineData("^$", "x", false)]
    public void Match_ReturnsExpectedResult(string pattern, string text, bool expected)
        {
        Assert.Equal(expected, Grep.Match(pattern, text));
        }
    }
