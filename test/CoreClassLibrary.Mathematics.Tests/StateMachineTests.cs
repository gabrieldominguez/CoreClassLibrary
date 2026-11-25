#pragma warning disable IDE0305 // Simplify collection initialization

using Microsoft.VisualStudio.TestPlatform.Utilities;
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Mathematics.StateMachines;
using CoreClassLibrary.Mathematics.StateMachines.Fast;
using CoreClassLibrary.Mathematics.StateMachines.Versatile;
using CoreClassLibrary.Mathematics.StateMachines.Versatile.Parsing;
using CoreClassLibrary.Objects.Pooling;
using System.Globalization;
using System.Web;
using Xunit;

namespace CoreClassLibrary.Mathematics.Tests;

public static class OutputExtensions
    {
    public static Task WriteAsync(this ITestOutputHelper output, IStateTransitionTable stateTable, string[]? stateLabels = null)
        => output.WriteDotAsync(stateTable.Write(stateLabels));

    public static async Task WriteDotAsync(this ITestOutputHelper output, string dot)
        {
        output.WriteLine(dot);
        output.WriteLine("");

        /// <see cref="https://github.com/ggerganov/dot-to-ascii"/>
        using HttpClient client = new HttpClient();
        output.WriteLine(HttpUtility.HtmlDecode(await client.GetStringAsync(String.Concat("https://dot-to-ascii.ggerganov.com/dot-to-ascii.php?src=", dot.Replace("+", "PLUS")))));   // &boxart=1 // dot-to-ascii doesn't handle '+' label
        }
    }

public class StateMachineTests(ITestOutputHelper output)
    {
    public static class EstadosTransferencia
        {
        public static readonly string[] Labels = [ "Por Autorizar", "En Ejecucion", "Cancelada", "Realizada", "Rechazada"];
        public const int PorAutorizar = 0;
        public const int EnEjecucion = 1;
        public const int Cancelada = 2;
        public const int Realizada = 3;
        public const int Rechazada = 4;
        }

    public static class EventosTransferencia
        {
        public const int Autorizar = 0;
        public const int Cancelar = 1;
        public const int Realizar = 2;
        public const int Rechazar = 3;
        }

    private static StateTransitionTable<int> CreateTransferenciaStateTable()
        => new StateTransitionTable<int>(
            [
            new StateTransition<int>(EstadosTransferencia.PorAutorizar, EstadosTransferencia.Cancelada, 0, "Cancelar"),
            new StateTransition<int>(EstadosTransferencia.PorAutorizar, EstadosTransferencia.EnEjecucion, 0, "Autorizar"),
            new StateTransition<int>(EstadosTransferencia.EnEjecucion, EstadosTransferencia.Realizada, 0, "Realizar"),
            new StateTransition<int>(EstadosTransferencia.EnEjecucion, EstadosTransferencia.Rechazada, 0, "Rechazar")
            ]);

    [Fact]
    public async Task StateTableTransition()
        {
        StateTransitionTable<int> stateTable = StateMachineTests.CreateTransferenciaStateTable();

        Assert.True(stateTable.GetTransitions(EstadosTransferencia.PorAutorizar, EstadosTransferencia.EnEjecucion).Any());
        Assert.False(stateTable.GetTransitions(EstadosTransferencia.PorAutorizar, EstadosTransferencia.Realizada).Any());

        await output.WriteAsync(stateTable, EstadosTransferencia.Labels);
        }

    [Fact]
    public async Task StateMachine()
        {
        StateTransitionTable<int> stateTable = new StateTransitionTable<int>(
            [
            TransitionExtensions.CreateEquatableTransition<int, ITestOutputHelper>(EstadosTransferencia.PorAutorizar, EventosTransferencia.Cancelar, output, (_, o) => o.WriteLine("Cancelada"), EstadosTransferencia.Cancelada, 0, "Cancelar"),
            TransitionExtensions.CreateEquatableTransition<int, ITestOutputHelper>(EstadosTransferencia.PorAutorizar, EventosTransferencia.Autorizar, output, (_, o) => o.WriteLine("EnEjecucion"), EstadosTransferencia.EnEjecucion, 0, "Autorizar"),
            TransitionExtensions.CreateEquatableTransition<int, ITestOutputHelper>(EstadosTransferencia.EnEjecucion, EventosTransferencia.Realizar, output, (_, o) => o.WriteLine("Realizada"), EstadosTransferencia.Realizada, 0, "Realizar"),
            TransitionExtensions.CreateEquatableTransition<int, ITestOutputHelper>(EstadosTransferencia.EnEjecucion, EventosTransferencia.Rechazar, output, (_, o) => o.WriteLine("Rechazada"), EstadosTransferencia.Rechazada, 0, "Rechazar")
            ]);

        Assert.False(stateTable.IsFinalState(EstadosTransferencia.PorAutorizar));
        Assert.False(stateTable.IsFinalState(EstadosTransferencia.EnEjecucion));
        Assert.True(stateTable.IsFinalState(EstadosTransferencia.Cancelada));
        Assert.True(stateTable.IsFinalState(EstadosTransferencia.Realizada));
        Assert.True(stateTable.IsFinalState(EstadosTransferencia.Rechazada));

        Assert.True(stateTable.GetTransitions(EstadosTransferencia.PorAutorizar, EstadosTransferencia.EnEjecucion).Any());
        Assert.False(stateTable.GetTransitions(EstadosTransferencia.PorAutorizar, EstadosTransferencia.Realizada).Any());

        await output.WriteAsync(stateTable, EstadosTransferencia.Labels);

        StateMachine<int> stateMachine = new StateMachine<int>(stateTable) { State = EstadosTransferencia.PorAutorizar };

        Assert.Equal(EstadosTransferencia.PorAutorizar, stateMachine.State);
        Assert.False(stateMachine.Process(EventosTransferencia.Realizar));
        Assert.Equal(EstadosTransferencia.PorAutorizar, stateMachine.State);
        Assert.True(stateMachine.Process(EventosTransferencia.Autorizar));
        Assert.Equal(EstadosTransferencia.EnEjecucion, stateMachine.State);
        Assert.False(stateMachine.Process(EventosTransferencia.Autorizar));
        Assert.Equal(EstadosTransferencia.EnEjecucion, stateMachine.State);
        Assert.True(stateMachine.Process(EventosTransferencia.Realizar));
        Assert.Equal(EstadosTransferencia.Realizada, stateMachine.State);

        Assert.True(stateMachine.IsFinalState());
        }

    [Fact]
    public async Task MatrixStateMachine()
        {
        StateTransitionMatrix stateMatrix = new StateTransitionMatrix(new SimpleTransition?[5, 4]);

        stateMatrix.Transitions[EstadosTransferencia.PorAutorizar, EventosTransferencia.Cancelar] = new SimpleTransition<ITestOutputHelper>("Cancelar", EstadosTransferencia.Cancelada, output, (_, o) => o.WriteLine("Cancelada"));
        stateMatrix.Transitions[EstadosTransferencia.PorAutorizar, EventosTransferencia.Autorizar] = new SimpleTransition<ITestOutputHelper>("Autorizar", EstadosTransferencia.EnEjecucion, output, (_, o) => o.WriteLine("EnEjecucion"));
        stateMatrix.Transitions[EstadosTransferencia.EnEjecucion, EventosTransferencia.Realizar] = new SimpleTransition<ITestOutputHelper>("Realizar", EstadosTransferencia.Realizada, output, (_, o) => o.WriteLine("Realizada"));
        stateMatrix.Transitions[EstadosTransferencia.EnEjecucion, EventosTransferencia.Rechazar] = new SimpleTransition<ITestOutputHelper>("Rechazar", EstadosTransferencia.Rechazada, output, (_, o) => o.WriteLine("Rechazada"));

        Assert.False(stateMatrix.IsFinalState(EstadosTransferencia.PorAutorizar));
        Assert.False(stateMatrix.IsFinalState(EstadosTransferencia.EnEjecucion));
        Assert.True(stateMatrix.IsFinalState(EstadosTransferencia.Cancelada));
        Assert.True(stateMatrix.IsFinalState(EstadosTransferencia.Realizada));
        Assert.True(stateMatrix.IsFinalState(EstadosTransferencia.Rechazada));

        await output.WriteAsync(stateMatrix, EstadosTransferencia.Labels);

        MatrixStateMachine stateMachine = new MatrixStateMachine(stateMatrix) { State = EstadosTransferencia.PorAutorizar };

        Assert.Equal(EstadosTransferencia.PorAutorizar, stateMachine.State);
        Assert.False(stateMachine.Process(EventosTransferencia.Realizar));
        Assert.Equal(EstadosTransferencia.PorAutorizar, stateMachine.State);
        Assert.True(stateMachine.Process(EventosTransferencia.Autorizar));
        Assert.Equal(EstadosTransferencia.EnEjecucion, stateMachine.State);
        Assert.False(stateMachine.Process(EventosTransferencia.Autorizar));
        Assert.Equal(EstadosTransferencia.EnEjecucion, stateMachine.State);
        Assert.True(stateMachine.Process(EventosTransferencia.Realizar));
        Assert.Equal(EstadosTransferencia.Realizada, stateMachine.State);

        Assert.True(stateMachine.IsFinalState());
        }

    [Fact]
    public async Task GetTextBetweenCharsStateTableWrite()
        {
        using PooledStringBuilder outputBuilder = new PooledStringBuilder();
        using StringWriter outputWriter = new StringWriter(outputBuilder.StringBuilder, CultureInfo.InvariantCulture);

        StateTransitionTable<char> stateTable = GetTextBetweenCharsTableFactory.CreateStateTransitionTable(outputWriter);
        await output.WriteAsync(stateTable);

        output.WriteLine(outputBuilder.ToString());
        }

    [Theory]
    [InlineData("aa[b]cc", '[', ']', "b")]
    [InlineData("aa[]cc", '[', ']', "")]
    [InlineData("[b]", '[', ']', "b")]
    [InlineData("abc", '[', ']', "")]
    [InlineData("", '[', ']', "")]
    [InlineData("aa[b]cc[dd]eee", '[', ']', "b")]
    [InlineData("aa[]cc[]ee", '[', ']', "")]
    [InlineData("aa[bcc", '[', ']', "bcc")]
    [InlineData("aa[cc]", '[', ']', "cc")]
    [InlineData("[b", '[', ']', "b")]
    [InlineData("abc[", '[', ']', "")]
    [InlineData("[]", '[', ']', "")]
    [InlineData("aa[bcc[dd]eee", '[', ']', "bcc[dd")]
    [InlineData("aa[]cc[123]ee", '[', ']', "")]
    public void GetTextBetweenCharsStateMachine(string input, char leftToken, char rightToken, string result)
        {
        using PooledStringBuilder outputBuilder = new PooledStringBuilder();
        using StringWriter outputWriter = new StringWriter(outputBuilder.StringBuilder, CultureInfo.InvariantCulture);

        StateMachine<char> stateMachine = new StateMachine<char>(GetTextBetweenCharsTableFactory.CreateStateTransitionTable(outputWriter, leftToken, rightToken));

        stateMachine.ProcessUntilFinalState(input);

        string outputString = outputBuilder.ToString();
        output.WriteLine(outputString);
        Assert.Equal(result, outputString);
        }

    [Theory]
    [InlineData("aa[b]cc", '[', ']', "b")]
    [InlineData("aa[]cc", '[', ']', "")]
    [InlineData("[b]", '[', ']', "b")]
    [InlineData("abc", '[', ']', "")]
    [InlineData("", '[', ']', "")]
    [InlineData("aa[b]cc[dd]eee", '[', ']', "b")]
    [InlineData("aa[]cc[]ee", '[', ']', "")]
    [InlineData("aa[bcc", '[', ']', "")]
    [InlineData("aa[cc]", '[', ']', "cc")]
    [InlineData("[b", '[', ']', "")]
    [InlineData("abc[", '[', ']', "")]
    [InlineData("[]", '[', ']', "")]
    [InlineData("aa[bcc[dd]eee", '[', ']', "bcc[dd")]
    [InlineData("aa[]cc[123]ee", '[', ']', "")]
    public void GetTextBetweenCharsStateMachine2(string input, char leftToken, char rightToken, string result)
        {
        using PooledStringBuilder outputBuilder = new PooledStringBuilder();
        using StringWriter outputWriter = new StringWriter(outputBuilder.StringBuilder, CultureInfo.InvariantCulture);

        StateMachine<char> stateMachine = new StateMachine<char>(GetTextBetweenCharsTableFactory.CreateStateTransitionTable(outputWriter, leftToken, rightToken));

        stateMachine.ProcessUntilFinalState(input);

        string outputString = stateMachine.IsFinalState()? outputBuilder.ToString(): "";
        output.WriteLine(outputString);
        Assert.Equal(result, outputString);
        }

    private class BinaryGapState
        { 
        public int Counter { get; set; } = 0;
        public int MaxValue { get; set; } = 0 ;
        }

    /// <summary>
    /// Binary Gap
    /// </summary>
    /// <remarks>
    /// A binary gap within a positive integer N is any maximal sequence of consecutive zeros that is surrounded by ones at both ends in the binary representation of N.
    /// </remarks>
    /// <see cref="https://app.codility.com/programmers/lessons/1-iterations/binary_gap/"/>
    [Theory]
    [InlineData(9, 2)]
    [InlineData(15, 0)]
    [InlineData(20, 1)]
    [InlineData(32, 0)]
    [InlineData(529, 4)]
    [InlineData(1041, 5)]
    public void BinaryGapCodility(UInt32 n, int gap)
        {
        BinaryGapState state = new BinaryGapState();

        StateMachine<bool> machine = new StateMachine<bool>(
            new StateTransitionTable<bool>(
                [
                TransitionExtensions.CreateEquatableTransition(0, false, 0),
                TransitionExtensions.CreateEquatableTransition(0, true, state, (c, s) => s.Counter = 0, 1),
                TransitionExtensions.CreateEquatableTransition(1, false, state, (c, s) => s.Counter++, 1),
                TransitionExtensions.CreateEquatableTransition(1, true, state, (c, s) => { s.MaxValue = Math.Max(s.MaxValue, s.Counter); s.Counter = 0; }, 1),
                ]));

        machine.Process(n.GetBytesHighFirst().ToBitsHighFirst());

        output.WriteLine(new String(n.GetBytesHighFirst().ToBitsHighFirst().Select(b => b? '1': '0').ToArray()));

        Assert.Equal(gap, state.MaxValue);
        }

    /// <summary>
    /// Binary Gap
    /// </summary>
    /// <remarks>
    /// Given a positive integer n, find and return the longest distance between any two adjacent 1's in the binary representation of n. If there are no two adjacent 1's, return 0.
    /// Two 1's are adjacent if there are only 0's separating them (possibly no 0's). The distance between two 1's is the absolute difference between their bit positions. For example, the two 1's in "1001" have a distance of 3.
    /// </remarks>
    /// <see cref="https://leetcode.com/problems/binary-gap/"/>
    [Theory]
    [InlineData(22, 2)]
    [InlineData(8, 0)]
    [InlineData(5, 2)]
    public void BinaryGapLeetCode(UInt32 n, int gap)
        {
        BinaryGapState state = new BinaryGapState();

        StateMachine<bool> machine = new StateMachine<bool>(
            new StateTransitionTable<bool>(
                [
                TransitionExtensions.CreateEquatableTransition(0, false, 0),
                TransitionExtensions.CreateEquatableTransition(0, true, state, (c, s) => s.Counter = 1, 1),
                TransitionExtensions.CreateEquatableTransition(1, false, state, (c, s) => s.Counter++, 1),
                TransitionExtensions.CreateEquatableTransition(1, true, state, (c, s) => { s.MaxValue = Math.Max(s.MaxValue, s.Counter); s.Counter = 1; }, 1),
                ]));

        machine.Process(n.GetBytesHighFirst().ToBitsHighFirst());

        output.WriteLine(new String(n.GetBytesHighFirst().ToBitsHighFirst().Select(b => b? '1': '0').ToArray()));

        Assert.Equal(gap, state.MaxValue);
        }
    }
