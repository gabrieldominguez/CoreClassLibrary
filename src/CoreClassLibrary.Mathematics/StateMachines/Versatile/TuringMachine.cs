
using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Mathematics.StateMachines.Versatile;

public enum Movement { Left = -1, NoMove = 0, Right = 1 }

/// <summary>
/// Simple Turing machine.
/// </summary>
/// <remarks>
/// Turing's paper ... contains, in essence, the invention of the modern computer and some of the programming techniques that accompanied it.
/// Minsky (1967), p. 104
/// </remarks>
/// <see cref="https://en.wikipedia.org/wiki/Turing_machine"/>
/// <see cref="https://plato.stanford.edu/entries/turing-machine/"/>
public class TuringMachine<INPUT> where INPUT: notnull, IEquatable<INPUT>
    {
    public record Transition(int CurrentState, INPUT ReadedSymbol, INPUT WrittenSymbol, Movement Movement, int NextState)
        { 
        public StateTransition<INPUT> CreateStateTransition(Tape tape)
            => TransitionExtensions.CreateEquatableTransition<INPUT, (Transition Transition, Tape Tape)>(
                        this.CurrentState,
                        this.ReadedSymbol,
                        (this, tape),
                        (_, context) =>
                            {
                            context.Tape.Write(context.Transition.WrittenSymbol);
                            context.Tape.Move(context.Transition.Movement);
                            },
                        this.NextState,
                        0,
                        String.Concat(this.ReadedSymbol, ", ", this.WrittenSymbol, ", ", this.Movement.ToString()[0])
                        );
        }

    public class Tape(INPUT[] array, int index = 0)
        {
        public void Move(Movement movement)
            => index += (int) movement;

        public INPUT Read()
            => array[index];

        public void Write(INPUT input)
            => array[index] = input;
        }

    public StateMachine<INPUT> StateMachine { get; }
    private readonly Tape tape;

    public TuringMachine(Tape tape, IEnumerable<TuringMachine<INPUT>.Transition> transitions)
        {
        this.StateMachine = new StateMachine<INPUT>(new StateTransitionTable<INPUT>(transitions.Select<Transition, Tape, StateTransition<INPUT>>(this.tape = tape, (transition, tape) => transition.CreateStateTransition(tape))));
        }

    public void RunUntilOutOfRange()
        {
        try
            {
            this.Run();
            }
        catch(IndexOutOfRangeException) { }
        }
    
    public void Run()
        {
        while(!this.StateMachine.IsFinalState())
            {
            this.Step();
            }
        }

    public bool Step()
        => this.StateMachine.Process(this.tape.Read());
    }
