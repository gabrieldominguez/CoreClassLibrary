
namespace CoreClassLibrary.Mathematics.StateMachines.Fast;

public record StateTransitionMatrix(SimpleTransition?[,] Transitions): IStateTransitionTable
    {
    public bool IsFinalState(int state)
        {
        for (int input = 0; input < this.Transitions.GetLength(1); input++)
            {
            if (this.Transitions[state, input] != null)
                {
                return false;
                }
            }
        return true;
        }

    public void Write(TextWriter writer, IReadOnlyList<string>? stateLabels = null)
        {
        writer.Write("digraph {");

        for (int state = 0; state < this.Transitions.GetLength(0); state++)
            {
            if(stateLabels != null)
                {
                writer.Write(state);
                writer.Write(" [label=\"");
                writer.Write(state);
                writer.Write("\\n");
                writer.Write(stateLabels[state]);
                writer.Write("\"]; ");
                }

            for (int input = 0; input < this.Transitions.GetLength(1); input++)
                {
                SimpleTransition? transition = this.Transitions[state, input];

                if (transition != null)
                    {
                    writer.Write(state);
                    writer.Write("->");
                    writer.Write(transition.NextState);

                    if (transition.Label != "")
                        {
                        writer.Write(" [label=\" ");
                        writer.Write(transition.Label);
                        writer.Write("\"]");
                        }

                    writer.Write("; ");
                    }
                }
            }

        writer.Write("}");
        }
    }