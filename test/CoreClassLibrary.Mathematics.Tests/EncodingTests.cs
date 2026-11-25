
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Mathematics.Randomness;
using CoreClassLibrary.Mathematics.StateMachines;
using CoreClassLibrary.Mathematics.StateMachines.Versatile;
using CoreClassLibrary.Mathematics.StateMachines.Versatile.Encoding;
using CoreClassLibrary.Objects.Pooling;
using CoreClassLibrary.Tests;
using System.Globalization;
using Xunit;

namespace CoreClassLibrary.Mathematics.Tests;

public class EncodingTests(ITestOutputHelper outputHelper)
    {
    [Fact]
    public void Base64()
        {
        Assert.Equal("RGVtb01pY3Jvc2VydmljaW9zOmVlYzEzZjQwLTgwYjYtMGJjNS1lOTgzLTYyYTFjNzVlM2RkZA==", "DemoMicroservicios:eec13f40-80b6-0bc5-e983-62a1c75e3ddd".ToBase64String());
        }

    [Fact]
    public void EncodingMachineWrite()
        {
        using PooledStringBuilder outputBuilder = new PooledStringBuilder();
        using StringWriter outputWriter = new StringWriter(outputBuilder.StringBuilder, CultureInfo.InvariantCulture);
        StateTransitionTable<char> stateTable = AsciiEncoder.Create(outputWriter, StringExtensions.SymbolsChars).CreateEncodingTable();
        
        outputHelper.WriteLine(stateTable.Write());
        }

    [Theory]
    [InlineData("A@C", "A#40C", "@", '#')]
    [InlineData("", "", "@", '#')]
    [InlineData("A", "A", "@", '#')]
    [InlineData("ABC", "ABC", "@", '#')]
    [InlineData("A@B+C", "A#40B#2BC", "@+", '#')]
    [InlineData("A#C", "A#23C", "@", '#')]
    [InlineData("A##C", "A#23#23C", "@", '#')]
    [InlineData("#", "#23", "@", '#')]
    [InlineData("@", "#40", "@", '#')]
    [InlineData("#40", "#40", "@", '@')]
    [InlineData("@@", "#40#40", "@", '#')]
    [InlineData("#@@#", "#23#40#40#23", "@", '#')]
    public void EncodingMachine(string input, string expected, string encodingChars, char encoderChar)
        {
        using PooledStringBuilder outputBuilder = new PooledStringBuilder();
        using StringWriter outputWriter = new StringWriter(outputBuilder.StringBuilder, CultureInfo.InvariantCulture);
        
        StateTransitionTable<char> stateTable = AsciiEncoder.Create(outputWriter, encodingChars, encoderChar).CreateEncodingTable();
        StateMachine<char> stateMachine = new StateMachine<char>(stateTable);

        stateMachine.Process(input);

        outputHelper.Equal(expected, outputBuilder.ToString());
        }

    [Theory]
    [InlineData("A#40C", "A@C", '#')]
    [InlineData("", "", '#')]
    [InlineData("A", "A", '#')]
    [InlineData("ABC", "ABC", '#')]
    [InlineData("A#40B#2BC", "A@B+C", '#')]
    [InlineData("A#23C", "A#C", '#')]
    [InlineData("A#23#23C", "A##C", '#')]
    [InlineData("#23", "#", '#')]
    [InlineData("#40", "@", '#')]
    [InlineData("#40", "#40", '@')]
    [InlineData("#40#40", "@@", '#')]
    [InlineData("#23#40#40#23", "#@@#", '#')]
    [InlineData("A#4C", "AL", '#')]
    public void DecodingMachine(string input, string expected, char encoderChar)
        {
        using PooledStringBuilder outputBuilder = new PooledStringBuilder();
        using PooledStringBuilder keyBuilder = new PooledStringBuilder();
        using StringWriter outputWriter = new StringWriter(outputBuilder.StringBuilder, CultureInfo.InvariantCulture);

        StateTransitionTable<char> stateTable = new AsciiDecoder(outputWriter, encoderChar).CreateDecodingTable();
        StateMachine<char> stateMachine = new StateMachine<char>(stateTable);

        stateMachine.Process(input);

        outputHelper.Equal(expected, outputBuilder.ToString());
        }

    [Fact]
    public void EncodingDecodingMachine()
        {
        const char encoderChar = '#';

        using PooledStringBuilder encodingBuilder = new PooledStringBuilder();
        using StringWriter encodingWriter = new StringWriter(encodingBuilder.StringBuilder, CultureInfo.InvariantCulture);
        StateTransitionTable<char> encodingTable = AsciiEncoder.Create(encodingWriter, StringExtensions.SymbolsChars, encoderChar).CreateEncodingTable();
        outputHelper.WriteLine(encodingTable.Write());

        using PooledStringBuilder decodingBuilder = new PooledStringBuilder();
        using StringWriter decodingWriter = new StringWriter(decodingBuilder.StringBuilder, CultureInfo.InvariantCulture);
        StateTransitionTable<char> decodingTable = new AsciiDecoder(decodingWriter, encoderChar).CreateDecodingTable();
        outputHelper.WriteLine(decodingTable.Write(DecodingStates.Labels));

        string input = RandomWrapper.Shared.LoremIpsum(100);
        outputHelper.WriteLine(input);
        outputHelper.WriteLine("");

        StateMachine<char> encodingMachine = new StateMachine<char>(encodingTable);
        encodingMachine.Process(input);

        string output = encodingBuilder.ToString();
        outputHelper.WriteLine(output);

        StateMachine<char> decodingMachine = new StateMachine<char>(decodingTable);
        decodingMachine.Process(output);

        Assert.Equal(input, decodingBuilder.ToString());
        }
    }
