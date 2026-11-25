namespace CoreClassLibrary.Mathematics.StateMachines.Versatile;

public record StateTransition<INPUT>(
    int CurrentState,
    Func<INPUT, bool> Predicate,
    Action<INPUT>? Action,
    int NextState,
    int Order = 0,
    string Label = "") : IComparable<StateTransition<INPUT>>
    {
    public StateTransition(int currentState, int nextState, int order = 0, string label = ""): 
        this(currentState, _ => true, nextState, order, label) { }

    public StateTransition(int currentState, Func<INPUT, bool> predicate, int nextState, int order = 0, string label = ""):
        this(currentState, predicate, null, nextState, order, label) { }

    public int CompareTo(StateTransition<INPUT>? other) 
        => this.Order.CompareTo(other?.Order);

    public void Write(TextWriter writer)
        {
        writer.Write(this.CurrentState);
        writer.Write("->");
        writer.Write(this.NextState);

        if (this.Label != "")
            {
            writer.Write(" [label=\" ");
            writer.Write(this.Label);
            writer.Write("\"]");
            }

        writer.Write("; ");
        }
    }

public record StateTransition<INPUT, TContext>(
    int CurrentState, 
    TContext Context, 
    Func<INPUT, TContext, bool> ContextPredicate, 
    Action<INPUT, TContext>? ContextAction, 
    int NextState, 
    int Order, 
    string Label = "") 
    : StateTransition<INPUT>
        (
        CurrentState,
        input => ContextPredicate(input, Context),
        ContextAction == null? null: input => ContextAction(input, Context), 
        NextState,
        Order, 
        Label        
        );

public static class TransitionExtensions
    {       
    public static StateTransition<INPUT, INPUT> CreateEquatableTransition<INPUT>(int currentState, INPUT input, int nextState, int order = 0, string label = "") where INPUT : IEquatable<INPUT>
        => new StateTransition<INPUT, INPUT>(currentState, input, (i, c) => i.Equals(c), null, nextState, order, label);

    public static StateTransition<INPUT, TContext> CreateEquatableTransition<INPUT, TContext>(int currentState, INPUT input, TContext context, Action<INPUT, TContext> action, int nextState, int order = 0, string label = "") where INPUT: IEquatable<INPUT>
        => new StateTransition<INPUT, TContext>(currentState, context, (i, _) => i.Equals(input), action, nextState, order, label);

    public static StateTransition<INPUT, TContext> CreateElseTransition<INPUT, TContext>(int currentState, TContext context, Action<INPUT, TContext> action, int nextState, string label = "")
        => new StateTransition<INPUT, TContext>(currentState, context, (_,_) => true, action, nextState, int.MaxValue, label);  
    }
