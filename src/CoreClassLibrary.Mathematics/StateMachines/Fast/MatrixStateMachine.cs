
namespace CoreClassLibrary.Mathematics.StateMachines.Fast;

public class MatrixStateMachine(StateTransitionMatrix transitionMatrix): IStateMachine<int>
    {
    public int State { get; set; }

    public IStateTransitionTable StateTransitionTable { get; } = transitionMatrix;

    public bool Process(int input)
        {
        SimpleTransition? transition = transitionMatrix.Transitions[this.State, input];

        if (transition == null)
            {
            return false;
            }

        transition.Action?.Invoke(input);
        this.State = transition.NextState;

        return true;
        }    
    }
