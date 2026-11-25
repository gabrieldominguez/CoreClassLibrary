
using CoreClassLibrary.Collections;
using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Mathematics.StateMachines.Versatile;

/// <summary>
/// Simple state machine.
/// </summary>
/// <see cref="https://en.wikipedia.org/wiki/Finite-state_machine"/>
public class StateMachine<INPUT>(StateTransitionTable<INPUT> transitionTable): IStateMachine<INPUT>
    {
    public int State { get; set; }

    public IStateTransitionTable StateTransitionTable { get; } = transitionTable;

    public bool Process(INPUT input)
        {
        transitionTable.TransitionsDictionary.TryGetValue(this.State, out DuplicateSortedSet<StateTransition<INPUT>>? stateTransitions);
        StateTransition<INPUT>? transition = stateTransitions?.Where(input, (t, i) => t.Predicate(i)).FirstOrDefault();

        if(transition == null)
            {
            return false;
            }

        transition.Action?.Invoke(input);

        this.State = transition.NextState;

        return true;
        }
    }
