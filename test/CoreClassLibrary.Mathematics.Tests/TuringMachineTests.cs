
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Mathematics.StateMachines.Versatile;
using Xunit;

namespace CoreClassLibrary.Mathematics.Tests;

public class TuringMachineTests(ITestOutputHelper outputHelper)
    {
    private static TuringMachine<int> CreateUnaryNotationTuringMachine(int[] array)
        {
        TuringMachine<int> turingMachine =  new TuringMachine<int>(
            new TuringMachine<int>.Tape(array),
            [
            new TuringMachine<int>.Transition(1, 1, 0, Movement.Right, 2),
            new TuringMachine<int>.Transition(2, 0, 1, Movement.Left, 3),
            new TuringMachine<int>.Transition(2, 1, 1, Movement.Right, 2),
            new TuringMachine<int>.Transition(3, 0, 0, Movement.Right, 4),
            new TuringMachine<int>.Transition(3, 1, 1, Movement.Left, 3),
            new TuringMachine<int>.Transition(4, 1, 0, Movement.Right, 5),
            ]);

        turingMachine.StateMachine.State = 1;

        return turingMachine;
        }

    private static int UnaryNotationCount(int[] array)
        => array.Count(i => i == 1) - 1;

    [Fact]
    // https://plato.stanford.edu/entries/turing-machine/#SomeSimpExam
    public async Task UnaryNotationAddition()
        {
        int[] array = [1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0]; // 3 4

        Assert.Equal(3, TuringMachineTests.UnaryNotationCount(array[..4]));
        Assert.Equal(4, TuringMachineTests.UnaryNotationCount(array[4..]));

        TuringMachine<int> turingMachine = TuringMachineTests.CreateUnaryNotationTuringMachine(array);
        await outputHelper.WriteAsync(turingMachine.StateMachine.StateTransitionTable);
        turingMachine.Run();

        Assert.Equal(7, TuringMachineTests.UnaryNotationCount(array));
        Assert.Equal((int[])[0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0], array); 
        }

    [Fact]
    // https://plato.stanford.edu/entries/turing-machine/#SomeSimpExam
    public async Task UnaryNotationMultipleAddition()
        {
        int[] array = [1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0]; // 3 4 2

        Assert.Equal(3, TuringMachineTests.UnaryNotationCount(array[..4]));
        Assert.Equal(4, TuringMachineTests.UnaryNotationCount(array[4..10]));        
        Assert.Equal(2, TuringMachineTests.UnaryNotationCount(array[^4..]));

        TuringMachine<int> turingMachine = TuringMachineTests.CreateUnaryNotationTuringMachine(array);
        await outputHelper.WriteAsync(turingMachine.StateMachine.StateTransitionTable);
        turingMachine.Run();

        Assert.Equal(7, TuringMachineTests.UnaryNotationCount(array[..10]));
        Assert.Equal((int[])[0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0], array); 

        turingMachine.StateMachine.State = 1;
        turingMachine.Run();

        Assert.Equal(9, TuringMachineTests.UnaryNotationCount(array));
        Assert.Equal((int[])[0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0], array);         
        }

    [Fact]
    // https://en.wikipedia.org/wiki/Turing_machine_examples
    public async Task TuringFirst()
        {
        //int[] array = Enumerable.Repeat(0, 10).ToArray();
        int[] array = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];

        TuringMachine<int> turingMachine = new TuringMachine<int>(
            new TuringMachine<int>.Tape(array), 
            [
            new TuringMachine<int>.Transition(0, 0, 0, Movement.Right, 1),
            new TuringMachine<int>.Transition(0, 1, 0, Movement.Right, 1),
            new TuringMachine<int>.Transition(1, 0, 1, Movement.Right, 0),
            new TuringMachine<int>.Transition(1, 1, 1, Movement.Right, 0)
            ]);

        await outputHelper.WriteAsync(turingMachine.StateMachine.StateTransitionTable);
        turingMachine.RunUntilOutOfRange();

        Assert.Equal((int[])[0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0], array); 
        }

    private static TuringMachine<int> CreateCopyTuringMachine(int[] array)
        {
        TuringMachine<int> turingMachine = new TuringMachine<int>(
            new TuringMachine<int>.Tape(array),                    
            [
            new TuringMachine<int>.Transition(1, 0, 0, Movement.NoMove, 6),
            new TuringMachine<int>.Transition(1, 1, 0, Movement.Right, 2),
            new TuringMachine<int>.Transition(2, 0, 0, Movement.Right, 3),
            new TuringMachine<int>.Transition(2, 1, 1, Movement.Right, 2),
            new TuringMachine<int>.Transition(3, 0, 1, Movement.Left, 4),
            new TuringMachine<int>.Transition(3, 1, 1, Movement.Right, 3),
            new TuringMachine<int>.Transition(4, 0, 0, Movement.Left, 5),
            new TuringMachine<int>.Transition(4, 1, 1, Movement.Left, 4),
            new TuringMachine<int>.Transition(5, 0, 1, Movement.Right, 1),
            new TuringMachine<int>.Transition(5, 1, 1, Movement.Left, 5)
            ]);

        turingMachine.StateMachine.State = 1;

        return turingMachine;
        }

    [Fact]
    // https://en.wikipedia.org/wiki/Turing_machine_examples
    public async Task Copy()
        {
        int[] array = [1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0];

        TuringMachine<int> turingMachine = TuringMachineTests.CreateCopyTuringMachine(array);
        await outputHelper.WriteAsync(turingMachine.StateMachine.StateTransitionTable);
        turingMachine.Run();

        Assert.Equal((int[])[1, 1, 1, 0, 1, 1, 1, 0, 0, 0, 0], array); 
        }

    // https://en.wikipedia.org/wiki/Turing_machine_examples
    [Theory]
    [InlineData("0000000000", "0000000000")]
    [InlineData("1000000000", "1010000000")]
    [InlineData("1100000000", "1101100000")]
    [InlineData("1110000000", "1110111000")]
    [InlineData("1111000000", "1111011110")]
    public void Copy2(string input, string output)
        {
        int[] array = input.ToIntArray();

        TuringMachine<int> turingMachine = TuringMachineTests.CreateCopyTuringMachine(array);
        turingMachine.Run();

        Assert.Equal(output.ToIntArray(), array); 
        }

    [Fact]
    public async Task ClearWrite()
        {
        TuringMachine<int> turingMachine = new TuringMachine<int>(
            new TuringMachine<int>.Tape([]), 
            [
            new TuringMachine<int>.Transition(0, 0, 0, Movement.Right, 0),
            new TuringMachine<int>.Transition(0, 1, 0, Movement.Right, 0)
            ]);        
        
        await outputHelper.WriteAsync(turingMachine.StateMachine.StateTransitionTable);
        }

    [Theory]
    [InlineData("0000000000", "0000000000")]
    [InlineData("1000000000", "0000000000")]
    [InlineData("1100000000", "0000000000")]
    [InlineData("1110000000", "0000000000")]
    [InlineData("1111000000", "0000000000")]
    [InlineData("1111111111", "0000000000")]
    [InlineData("0101111111", "0000000000")]
    [InlineData("0010011111", "0000000000")]
    [InlineData("0001000111", "0000000000")]
    [InlineData("0000100001", "0000000000")]
    public void Clear(string input, string output)
        {
        int[] array = input.ToIntArray();

        TuringMachine<int> turingMachine = new TuringMachine<int>(
            new TuringMachine<int>.Tape(array), 
            [
            new TuringMachine<int>.Transition(0, 0, 0, Movement.Right, 0),
            new TuringMachine<int>.Transition(0, 1, 0, Movement.Right, 0)
            ]);        
        
        turingMachine.RunUntilOutOfRange();

        Assert.Equal(output.ToIntArray(), array); 
        }
    
    [Fact]
    public async Task NegateWrite()
        {
        TuringMachine<int> turingMachine = new TuringMachine<int>(
            new TuringMachine<int>.Tape([]), 
            [
            new TuringMachine<int>.Transition(0, 0, 1, Movement.Right, 0),
            new TuringMachine<int>.Transition(0, 1, 0, Movement.Right, 0)
            ]);        
        
        await outputHelper.WriteAsync(turingMachine.StateMachine.StateTransitionTable);
        }
    
    [Theory]
    [InlineData("0000000000", "1111111111")]
    [InlineData("1000000000", "0111111111")]
    [InlineData("1100000000", "0011111111")]
    [InlineData("1110000000", "0001111111")]
    [InlineData("1111000000", "0000111111")]
    [InlineData("1111111111", "0000000000")]
    [InlineData("0101111111", "1010000000")]
    [InlineData("0010011111", "1101100000")]
    [InlineData("0001000111", "1110111000")]
    [InlineData("0000100001", "1111011110")]
    public void Negate(string input, string output)
        {
        int[] array = input.ToIntArray();

        TuringMachine<int> turingMachine = new TuringMachine<int>(
            new TuringMachine<int>.Tape(array), 
            [
            new TuringMachine<int>.Transition(0, 0, 1, Movement.Right, 0),
            new TuringMachine<int>.Transition(0, 1, 0, Movement.Right, 0)
            ]);        

        turingMachine.RunUntilOutOfRange();

        Assert.Equal(output.ToIntArray(), array); 
        }

    private static TuringMachine<int> CreateIncrementalTuringMachine(int[] array)
        => new TuringMachine<int>(
            new TuringMachine<int>.Tape(array, array.Length-1), 
            [
            new TuringMachine<int>.Transition(0, 0, 1, Movement.NoMove, 1),
            new TuringMachine<int>.Transition(0, 1, 0, Movement.Left, 0)
            ]);  

    [Fact]
    public Task IncrementalWrite()
        => outputHelper.WriteAsync(TuringMachineTests.CreateIncrementalTuringMachine([]).StateMachine.StateTransitionTable);       

    [Theory]
    [InlineData("00000000", "00000001")]
    [InlineData("01111111", "10000000")]
    [InlineData("00111111", "01000000")]
    [InlineData("00011111", "00100000")]
    [InlineData("11110000", "11110001")]
    [InlineData("01011111", "01100000")]
    [InlineData("00100111", "00101000")]
    [InlineData("00010001", "00010010")]
    [InlineData("00001000", "00001001")]
    [InlineData("11111110", "11111111")]
    [InlineData("11111101", "11111110")]
    public void Incremental(string input, string output)
        {
        outputHelper.WriteLine(input);
        outputHelper.WriteLine(output);

        int[] array = input.ToIntArray();
        
        TuringMachine<int> turingMachine = TuringMachineTests.CreateIncrementalTuringMachine(array);
        turingMachine.Run();

        Assert.Equal(output.ToIntArray(), array); 
        }

    [Fact]    
    public void ExtensiveIncremental()
        {
        byte i = 0;
        while (i < 255)
            {
            this.Incremental(
                new String(i.ToBitsHighFirst().Select(b => b? '1': '0').ToArray()),
                new String((++i).ToBitsHighFirst().Select(b => b? '1': '0').ToArray()));
            
            outputHelper.WriteLine("--------");
            }
        }

    private static TuringMachine<int> CreateDecrementalTuringMachine(int[] array)
        => new TuringMachine<int>(
            new TuringMachine<int>.Tape(array, array.Length-1), 
            [
            new TuringMachine<int>.Transition(0, 1, 0, Movement.NoMove, 1),
            new TuringMachine<int>.Transition(0, 0, 1, Movement.Left, 0)
            ]);  

    [Fact]
    public Task DecrementalWrite()
        => outputHelper.WriteAsync(TuringMachineTests.CreateDecrementalTuringMachine([]).StateMachine.StateTransitionTable);

    [Theory]
    [InlineData("00000001", "00000000")]
    [InlineData("10000000", "01111111")]
    [InlineData("01000000", "00111111")]
    [InlineData("00100000", "00011111")]
    [InlineData("11110001", "11110000")]
    [InlineData("01100000", "01011111")]
    [InlineData("00101000", "00100111")]
    [InlineData("00010010", "00010001")]
    [InlineData("00001001", "00001000")]
    [InlineData("11111111", "11111110")]
    public void Decremental(string input, string output)
        {
        outputHelper.WriteLine(input);
        outputHelper.WriteLine(output);

        int[] array = input.ToIntArray();
        
        TuringMachine<int> turingMachine = TuringMachineTests.CreateDecrementalTuringMachine(array);
        turingMachine.Run();

        Assert.Equal(output.ToIntArray(), array); 
        }

    [Fact]    
    public void ExtensiveDecremental()
        {
        byte i = 255;
        while (i > 0)
            {
            this.Decremental(
                new String(i.ToBitsHighFirst().Select(b => b? '1': '0').ToArray()),
                new String((--i).ToBitsHighFirst().Select(b => b? '1': '0').ToArray()));
            
            outputHelper.WriteLine("--------");
            }
        }

    private static TuringMachine<int> CreateShiftTuringMachine(int[] array, Movement movement)
        => new TuringMachine<int>(
            new TuringMachine<int>.Tape(array, movement == Movement.Left? array.Length-1: 0), 
            [
            new TuringMachine<int>.Transition(0, 0, 0, movement, 0),
            new TuringMachine<int>.Transition(0, 1, 0, movement, 1), 
            new TuringMachine<int>.Transition(1, 0, 1, movement, 0),
            new TuringMachine<int>.Transition(1, 1, 1, movement, 1)
            ]);  

    [Fact]
    public Task ShiftLeftWrite()
        => outputHelper.WriteAsync(TuringMachineTests.CreateShiftTuringMachine([], Movement.Left).StateMachine.StateTransitionTable);

    [Theory]
    [InlineData("00000000", "00000000")]
    [InlineData("01111111", "11111110")]
    [InlineData("00111111", "01111110")]
    [InlineData("00011111", "00111110")]
    [InlineData("11110000", "11100000")]
    [InlineData("01011111", "10111110")]
    [InlineData("00100111", "01001110")]
    [InlineData("00010001", "00100010")]
    [InlineData("00001000", "00010000")]
    [InlineData("11111110", "11111100")]
    [InlineData("11111101", "11111010")]
    [InlineData("00000001", "00000010")]
    [InlineData("10000000", "00000000")]
    [InlineData("01000000", "10000000")]
    [InlineData("00100000", "01000000")]
    [InlineData("11110001", "11100010")]
    [InlineData("01100000", "11000000")]
    [InlineData("00101000", "01010000")]
    [InlineData("00010010", "00100100")]
    [InlineData("00001001", "00010010")]
    [InlineData("11111111", "11111110")]
    public void ShiftLeft(string input, string output)
        {
        outputHelper.WriteLine(input);
        outputHelper.WriteLine(output);

        int[] array = input.ToIntArray();
        
        TuringMachine<int> turingMachine = TuringMachineTests.CreateShiftTuringMachine(array, Movement.Left);
        turingMachine.RunUntilOutOfRange();

        Assert.Equal(output.ToIntArray(), array); 
        }

    [Fact]    
    public void ExtensiveShiftLeft()
        {
        int i = 0;
        while (i < 255)
            {
            this.ShiftLeft(
                new String(((byte)i).ToBitsHighFirst().Select(b => b? '1': '0').ToArray()),
                new String(((byte)(i++ * 2)).ToBitsHighFirst().Select(b => b? '1': '0').ToArray()));
            
            outputHelper.WriteLine("--------");
            }
        }
    
    [Fact]
    public Task ShiftRightWrite()
        => outputHelper.WriteAsync(TuringMachineTests.CreateShiftTuringMachine([], Movement.Right).StateMachine.StateTransitionTable);

    [Theory]
    [InlineData("00000000", "00000000")]
    [InlineData("01111111", "00111111")]
    [InlineData("00111111", "00011111")]
    [InlineData("00011111", "00001111")]
    [InlineData("11110000", "01111000")]
    [InlineData("01011111", "00101111")]
    [InlineData("00100111", "00010011")]
    [InlineData("00010001", "00001000")]
    [InlineData("00001000", "00000100")]
    [InlineData("11111110", "01111111")]
    [InlineData("11111101", "01111110")]
    [InlineData("00000001", "00000000")]
    [InlineData("10000000", "01000000")]
    [InlineData("01000000", "00100000")]
    [InlineData("00100000", "00010000")]
    [InlineData("11110001", "01111000")]
    [InlineData("01100000", "00110000")]
    [InlineData("00101000", "00010100")]
    [InlineData("00010010", "00001001")]
    [InlineData("00001001", "00000100")]
    [InlineData("11111111", "01111111")]
    public void ShiftRight(string input, string output)
        {
        outputHelper.WriteLine(input);
        outputHelper.WriteLine(output);

        int[] array = input.ToIntArray();
        
        TuringMachine<int> turingMachine = TuringMachineTests.CreateShiftTuringMachine(array, Movement.Right);
        turingMachine.RunUntilOutOfRange();

        Assert.Equal(output.ToIntArray(), array); 
        }

    [Fact]    
    public void ExtensiveShiftRight()
        {
        int i = 0;
        while (i < 255)
            {
            this.ShiftRight(
                new String(((byte)i).ToBitsHighFirst().Select(b => b? '1': '0').ToArray()),
                new String(((byte)(i++ / 2)).ToBitsHighFirst().Select(b => b? '1': '0').ToArray()));
            
            outputHelper.WriteLine("--------");
            }
        }
    }
