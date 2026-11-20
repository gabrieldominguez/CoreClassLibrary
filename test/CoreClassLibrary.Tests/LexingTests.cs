#pragma warning disable IDE0305 // Simplify collection initialization

using CoreClassLibrary.Extensions;
using CoreClassLibrary.RegularExpressions.Lexing;
using CoreClassLibrary.RegularExpressions.Lexing.Rules;
using Xunit;

namespace CoreClassLibrary.Tests;

public static class LexingTestsExtensions
    {
    public static void WriteAssert<T>(this ITestOutputHelper outputHelper, ILexer<T> lexer, int tokensCount)
        {
        Assert.Equal(tokensCount, outputHelper.Write(lexer));
        }

    public static int Write<T>(this ITestOutputHelper outputHelper, ILexer<T> lexer)
        {
        LexerMatch<T>[] lexerMatchs = lexer.GetMatches().ToArray();
        outputHelper.Write(lexerMatchs);
        return lexerMatchs.Length;
        }

    public static void Write<T>(this ITestOutputHelper outputHelper, IEnumerable<LexerMatch<T>> matches)
        {
        foreach (LexerMatch<T> match in matches)
            {
            outputHelper.WriteLine("{0}:{1} {2} '{3}'", match.LineNumber.ToString("0000"), match.Position.ToString("0000"), match.Match.Token?.ToString() ?? "<null>", match.Match.Value);
            }
        }
    }

/// <summary>
/// Lexer tests
/// </summary>
/// <see cref="https://stackoverflow.com/questions/673113/poor-mans-lexer-for-c-sharp"/>
public class LexingTests(ITestOutputHelper output)
    {
    [Theory]
    [InlineData(@"( true one (two 456 -43.2 "" \"" quoted"" ))  ", 19)]
    [InlineData("1 + 2", 5)]
    [InlineData("true * false\r\n1 + 2", 12)]
    public void StringLexing(string input, int tokensCount)
        {
        output.WriteAssert(LexingTests.CreateStringLexer(input), tokensCount);
        }

    [Theory]
    [InlineData(@"( true one (two 456 -43.2 "" \"" quoted"" ))  ", 19)]
    [InlineData("1 + 2", 5)]
    [InlineData("true * false\r\n1 + 2", 10)]
    public void TextLexing(string input, int tokensCount)
        {       
        using StringReader reader = new StringReader(input);
        TextLexer<string> lexer = new TextLexer<string>(reader, LexingTests.CreateStringLexer);

        output.WriteAssert(lexer, tokensCount);
        }

    [Theory]
    [InlineData("#", 1)]
    public void TextLexingError(string input, int tokensCount)
        {       
        try
            {
            using StringReader reader = new StringReader(input);
            TextLexer<string> lexer = new TextLexer<string>(reader, LexingTests.CreateStringLexer);
            output.WriteAssert(lexer, tokensCount);
            }
        catch (InvalidOperationException e) when (e.Message.StartsWith("Sintax error"))
            {
            output.WriteLine(e.ToString());
            }
        }
    
    private static StringLexer<string> CreateStringLexer(string input)
        => new StringLexer<string>(
            input, 
            new CompositeRule<string>(
                [
                new RegexRule<string>(@"\s", "SPACE"),
                new RegexRule<string>(@"([""'])(?:\\\1|.)*?\1", "QUOTED-STRING"),
                new RegexRule<string>(@"[-+]?\d*\.\d+([eE][-+]?\d+)?", "FLOAT"),
                new RegexRule<string>(@"[-+]?\d+", "INT"),
                new RegexRule<string>(@"true", "TRUE"),
                new RegexRule<string>(@"false", "FALSE"),
                new RegexRule<string>(@"[*<>\?\-+/A-Za-z->!]+", "SYMBOL"),
                new RegexRule<string>(@"\.", "DOT"),
                new RegexRule<string>(@"\(", "LEFT"),
                new RegexRule<string>(@"\)", "RIGHT")
                ]));

    
    [Theory]
    [InlineData("1 + 2", 5)]
    [InlineData("-1 + 2", 6)]
    [InlineData("1 + 2 * 3", 9)]
    [InlineData("(1 + 2) * 3", 11)]
    [InlineData("(1 + 2) / 3", 11)]
    [InlineData("1 + 2.1 * 3", 11)]
    [InlineData("true + false", 5)]
    public void StringLexer(string input, int tokensCount)
        {
        output.WriteAssert(new StringLexer<string>(
            input, 
            new CompositeRule<string>(
                [
                new StringRule<string>(" ", "SPACE"),
                new StringRule<string>("1", "1"),
                new StringRule<string>("2", "2"),
                new StringRule<string>("3", "3"),
                new StringRule<string>("+", "ADDITION"),
                new StringRule<string>("-", "MINUS"),
                new StringRule<string>("*", "MULTIPLY"),
                new StringRule<string>("/", "DIVISION"),
                new StringRule<string>(".", "DOT"),
                new StringRule<string>("(", "LEFT"),
                new StringRule<string>(")", "RIGHT"),
                new StringRule<string>("true", "TRUE"),
                new StringRule<string>("false", "FALSE")
                ])), tokensCount);
        }

    [Theory]
    [InlineData(@"( true one (two 456 -43.2 ))  ", 17)]
    [InlineData("1 + 2", 5)]
    [InlineData("true * false\r\n1 + 2", 12)]    
    public void CompositeRegexLexer(string input, int tokensCount)
        {
        output.WriteAssert(new StringLexer<string>(
            input, 
            new CompositeRegexRule<string>(
                [
                new RegexRule<string>(@"\s", "SPACE"),
                new RegexRule<string>(@"[-+]?\d*\.\d+", "FLOAT"),
                new RegexRule<string>(@"[-+]?\d+", "INT"),
                new RegexRule<string>(@"true", "TRUE"),
                new RegexRule<string>(@"false", "FALSE"),
                new RegexRule<string>(@"[*<>\?\-+/A-Za-z->!]+", "SYMBOL"),
                new RegexRule<string>(@"\.", "DOT"),
                new RegexRule<string>(@"\(", "LEFT"),
                new RegexRule<string>(@"\)", "RIGHT")
                ])), tokensCount);
        }
    
    
    [Theory]
    [InlineData("1 + 2", 5)]
    [InlineData("-1 + 2", 6)]
    [InlineData("1 + 2 * 3", 9)]
    [InlineData("(1 + 2) * 3", 11)]
    [InlineData("(1 + 2) / 3", 11)]
    [InlineData("1 + 2.1 * 3", 11)]
    public void CharLexer(string input, int tokensCount)
        {
        output.WriteAssert(new StringLexer<string>(
            input, 
            new CompositeCharRule<string>(
                [
                new CharRule<string>(' ', "SPACE"),
                new CharRule<string>('1', "1"),
                new CharRule<string>('2', "2"),
                new CharRule<string>('3', "3"),
                new CharRule<string>('+', "ADDITION"),
                new CharRule<string>('-', "MINUS"),
                new CharRule<string>('*', "MULTIPLY"),
                new CharRule<string>('/', "DIVISION"),
                new CharRule<string>('.', "DOT"),
                new CharRule<string>('(', "LEFT"),
                new CharRule<string>(')', "RIGHT")
                ])), tokensCount);
        }
    /*
    [Theory]
    [InlineData("1 + 2", 5)]
    [InlineData("-1 + 2", 5)]
    [InlineData("1 + 2 * 3", 9)]
    [InlineData("(1 + 2) * 3", 11)]
    [InlineData("(1 + 2) / 3", 11)]
    [InlineData("1 + 2.1 * 3", 9)]
    public void ArithmeticLexing(string input, int tokensCount)
        {
        output.WriteAssert(new Lexer(input), tokensCount);
        }

    [Theory]
    [InlineData("1..")]
    [InlineData("#")]
    public void ArithmeticLexingError(string input)
        {
        try
            {
            output.Write(new Lexer(input));
            Assert.Fail();
            }
        catch (InvalidOperationException e) when (e.Message.StartsWith("Sintax error"))
            {
            output.WriteLine(e.ToString());
            }
        }
    */
    }
