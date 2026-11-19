#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.

using CoreClassLibrary.Extensions;
using CoreClassLibrary.RegularExpressions;
using System.Text.RegularExpressions;
using Xunit;

namespace CoreClassLibrary.Tests;

public class RegexTests
    {
    [Fact]
    public void ParseCommandLineArgs()
        {
        string[] args = ["hosting=console", "arg2=valor", "arg3=\"otro valor\"", "arg4=\"1=1\""];

        Assert.Equal("console", args.GetCommandLineArgValue("hosting"));
        Assert.Equal("valor", args.GetCommandLineArgValue("arg2"));
        Assert.Equal("otro valor", args.GetCommandLineArgValue("arg3"));
        Assert.Equal("1=1", args.GetCommandLineArgValue("arg4"));

        Dictionary<string, string> dic = args.ParseCommandLineArgs();

        Assert.Equal("console", dic["hosting"]);
        Assert.Equal("valor", dic["arg2"]);
        Assert.Equal("otro valor", dic["arg3"]);
        Assert.Equal("1=1", dic["arg4"]);
        }
    
    [Theory]
    [InlineData("\"F:\\Temp\\SampleWebApi.SelfHost.vshost.exe\\\" /files:*.* /install /a:all /b=\"all files\" /t:'single quote' /h")]
    [InlineData("\"F:\\Temp\\SampleWebApi.SelfHost.vshost.exe\\\" -files:*.* -install -a:all -b=\"all files\" -t:'single quote' -h")]
    [InlineData("\"F:\\Temp\\SampleWebApi.SelfHost.vshost.exe\\\" /files=*.* /install /a=all /b:\"all files\" /t='single quote' /h")]
    [InlineData("\"F:\\Temp\\SampleWebApi.SelfHost.vshost.exe\\\" /files=*.* -install -a=all -b:\"all files\" -t='single quote' -h")]
    [InlineData("\"F:\\Temp\\SampleWebApi.SelfHost.vshost.exe\\\" /files:\"*.*\" /install /a:\"all\" /b=\"all files\" /t:'single quote' /h")]
    [InlineData("\"F:\\Temp\\SampleWebApi.SelfHost.vshost.exe\\\" /files:'*.*' /install /a:'all' /b=\"all files\" /t:'single quote' /h")]
    [InlineData("\"F:\\Temp\\SampleWebApi.SelfHost.vshost.exe\\\" /files:*.* /install -a:all /b=\"all files\" -t:'single quote' /h")]
    [InlineData("\"F:\\Temp\\SampleWebApi.SelfHost.vshost.exe\\\" -files:*.* -install /a:all -b=\"all files\" /t:'single quote' -h")]
    public void ParseCommandLine(string commandline)
        {
        Dictionary<string, string?> dic = commandline.ParseCommandLine();

        //Assert.Null(dic["*.*"]);
        Assert.Null(dic["install"]);
        Assert.Equal("all", dic["a"]);
        Assert.Equal("all files", dic["b"]);
        Assert.Equal("single quote", dic["t"]);
        Assert.Null(dic["h"]);
        Assert.True(dic.ContainsKey("install"));
        Assert.True(dic.ContainsKey("h"));
        Assert.False(dic.ContainsKey("X"));
        }

    [Theory]
    [InlineData("", 0)]
    [InlineData("123", 1)]
    [InlineData("for", 1)]
    [InlineData("for,", 1)]
    [InlineData("    for   ", 1)]
    [InlineData("for more", 2)]
    [InlineData("for more ", 2)]
    [InlineData("for more information", 3)]
    [InlineData("for More information.", 3)]
    public void GetWords(string input, int count)
        => Assert.Equal(count, input.GetWords().Count());

    [Theory]
    [InlineData("", 1)]
    [InlineData("for", 1)]
    [InlineData("for\r", 2)]
    [InlineData("for\n ", 2)]
    [InlineData("for\r\n", 2)]
    [InlineData("for\rmore", 2)]
    [InlineData("for\nmore ", 2)]
    [InlineData("for\r\nmore", 2)]
    [InlineData("for\r\nmore\r\ninformation", 3)]
	[InlineData(@"1
		2
		3
		4
		5", 5)]
    public void GetLines(string input, int count)
        {
        // Assert.Equal(count, input.GetLines().Length);

        Regex.ValueSplitEnumerator enumerator = RegexExtensions.LineBreaks().EnumerateSplits(input);
        int c = 0;
        while (enumerator.MoveNext()) 
            {
            c++;
            }
        Assert.Equal(count, c);
        }
    
    [Fact]
    public void RegexGroupTest1()
        {
        Match match = Regex.Match("AAABBB", "(?<prefix>.*)-(?<sufix>.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        Assert.Equal("", match.Groups["prefix"].Value);
        Assert.Equal("", match.Groups["sufix"].Value);
        }

    [Fact]
    public void RegexGroupTest2()
        {
        Match match = Regex.Match("AAA-BBB", "(?<prefix>.*)-(?<sufix>.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        Assert.Equal("AAA", match.Groups["prefix"].Value);
        Assert.Equal("BBB", match.Groups["sufix"].Value);
        }

    [Fact]
    public void RegexGroupTest3()
        {
        Match match = Regex.Match("AAA-BBB-CCC", "(?<prefix>.*)-(?<infix>.*)-(?<sufix>.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        Assert.Equal("AAA", match.Groups["prefix"].Value);
        Assert.Equal("BBB", match.Groups["infix"].Value);
        Assert.Equal("CCC", match.Groups["sufix"].Value);
        }

    [Fact]
    public void RegexGroupTest4()
        {
        Match match = Regex.Match("AAA", "(?<A>A+)|(?<B>B+)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        Assert.True(match.Groups.ContainsKey("0"));
        Assert.True(match.Groups.ContainsKey("A"));
        Assert.True(match.Groups.ContainsKey("B"));
        Assert.False(match.Groups.ContainsKey("C"));
        Assert.True(match.Groups["A"].Success);
        Assert.False(match.Groups["B"].Success);
        Assert.True(match.Groups[0].Success);
        Assert.True(match.Groups[1].Success);
        Assert.False(match.Groups[2].Success);
        Assert.Equal(1, match.GetSuccessfulGroupIndex());
        }

    [Fact]
    public void RegexGroupTest5()
        {
        Match match = Regex.Match("AAA", "(?<1>A+)|(?<2>B+)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        Assert.True(match.Groups.ContainsKey("0"));
        Assert.True(match.Groups.ContainsKey("1"));
        Assert.True(match.Groups.ContainsKey("2"));
        Assert.False(match.Groups.ContainsKey("3"));
        Assert.True(match.Groups["1"].Success);
        Assert.False(match.Groups["2"].Success);
        Assert.True(match.Groups[0].Success);
        Assert.True(match.Groups[1].Success);
        Assert.False(match.Groups[2].Success);
        Assert.Equal(1, match.GetSuccessfulGroupIndex());
        }

    [Fact]
    public void RegexGroupTest6()
        {
        Match match = Regex.Match("AAA", "(A+)|(B+)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        Assert.True(match.Groups.ContainsKey("0"));
        Assert.True(match.Groups.ContainsKey("1"));
        Assert.True(match.Groups.ContainsKey("2"));
        Assert.False(match.Groups.ContainsKey("3"));
        Assert.True(match.Groups[0].Success);
        Assert.True(match.Groups[1].Success);
        Assert.False(match.Groups[2].Success);
        Assert.Equal(1, match.GetSuccessfulGroupIndex());
        }

    [Fact]
    public void RegexCombine()
        {
        Match match = Regex.Match("AAA", RegexExtensions.CombinePatterns(["A+", "B+"]), RegexOptions.Compiled | RegexOptions.CultureInvariant);
        Assert.True(match.Groups.ContainsKey("0"));
        Assert.True(match.Groups.ContainsKey("1"));
        Assert.True(match.Groups.ContainsKey("2"));
        Assert.False(match.Groups.ContainsKey("3"));
        Assert.True(match.Groups[0].Success);
        Assert.True(match.Groups[1].Success);
        Assert.False(match.Groups[2].Success);
        Assert.Equal(1, match.GetSuccessfulGroupIndex());
        }

    [Fact]
    public void GetSuccessfulGroupIndex()
        {
        Match match = Regex.Match("AAA", "(A+)|(B+)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        Assert.Equal(1, match.GetSuccessfulGroupIndex());
        }

    [Fact]
    public void TestRegex()
        {
        Assert.Matches("", "12");
        Assert.Matches("\\d{2}", "12");
        Assert.Matches("\\d{2}", "123");
        }
    }
