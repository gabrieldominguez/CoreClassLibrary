
using CoreClassLibrary.Collections;
using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Mathematics.StateMachines.Versatile;

/// <summary>
/// State-transition table
/// </summary>
/// <see cref="https://en.wikipedia.org/wiki/State-transition_table"/>
public class StateTransitionTable<INPUT>: IStateTransitionTable
    {
    public Dictionary<int, DuplicateSortedSet<StateTransition<INPUT>>> TransitionsDictionary { get; } = [];

    public IEnumerable<StateTransition<INPUT>> Transitions => this.TransitionsDictionary.SelectMany(pair => pair.Value);

    public IEnumerable<StateTransition<INPUT>> GetTransitions(int state, int nextState)
        => this.TransitionsDictionary.TryGetValue(state, out DuplicateSortedSet<StateTransition<INPUT>>? stateTransitions) ?
                stateTransitions.Where(nextState, (t, s) => t.NextState == s) :
                [];

    public StateTransitionTable(IEnumerable<StateTransition<INPUT>> transitions)
        {
        foreach(StateTransition<INPUT> transition in transitions)
            { 
            if(!this.TransitionsDictionary.TryGetValue(transition.CurrentState, out DuplicateSortedSet<StateTransition<INPUT>>? stateTransitions))
                {
                this.TransitionsDictionary.Add(transition.CurrentState, stateTransitions = []);
                }

            stateTransitions.Add(transition);
            }
        }

    public bool IsFinalState(int state)
        => !this.TransitionsDictionary.ContainsKey(state);

    public void Write(TextWriter writer, IReadOnlyList<string>? stateLabels = null)
        {
        writer.Write("digraph {");

        if(stateLabels != null)
            {
            for (int state = 0; state < stateLabels.Count; state++)
                {
                writer.Write(state);
                writer.Write(" [label=\"");
                writer.Write(state);
                writer.Write("\\n");
                writer.Write(stateLabels[state]);
                writer.Write("\"]; ");
                }
            }

        foreach(StateTransition<INPUT> transition in this.Transitions)
            {
            transition.Write(writer);
            }

        writer.Write("}");
        }
    }
