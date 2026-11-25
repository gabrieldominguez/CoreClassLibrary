
namespace CoreClassLibrary.Mathematics.StateMachines;

/// <summary>
/// Finite state machine
/// </summary>
/// <see cref="https://en.wikipedia.org/wiki/Finite-state_machine"/>
public interface IStateMachine<INPUT>
    {
    IStateTransitionTable StateTransitionTable { get; }

    int State { get; set; }

    /// <summary>
    /// Procesa la entrada.
    /// </summary>
    /// <param name="input"></param>
    /// <returns>True si se encontró una transición (aunque haya sido al mismo estado)</returns>
    bool Process(INPUT input);
    }

public static class StateMachineExtensions
    {
    public static bool IsFinalState<INPUT>(this IStateMachine<INPUT> machine)
        => machine.StateTransitionTable.IsFinalState(machine.State);

    /// <summary>
    /// Procesa la secuencia item por item.
    /// </summary>
    public static void Process<INPUT>(this IStateMachine<INPUT> machine, IEnumerable<INPUT> input)
        {
        foreach (INPUT item in input)
            {
            machine.Process(item);
            }
        }

    /// <summary>
    /// Procesa la secuencia item por item hasta el final de la misma o hasta que se alcanza un estado final.
    /// </summary>
    public static void ProcessUntilFinalState<INPUT>(this IStateMachine<INPUT> machine, IEnumerable<INPUT> input)
        {
        foreach (INPUT item in input)
            {
            machine.Process(item);

            if (machine.IsFinalState())
                {
                return;
                }
            }
        }
    }