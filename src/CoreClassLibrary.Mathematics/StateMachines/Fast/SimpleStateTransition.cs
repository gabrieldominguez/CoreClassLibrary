
namespace CoreClassLibrary.Mathematics.StateMachines.Fast;

public record SimpleTransition<TContext>(string Label, int NextState, TContext Context, Action<int, TContext> ContextAction):
    SimpleTransition(Label, NextState, input => ContextAction(input, Context));

public record SimpleTransition(string Label, int NextState, Action<int>? Action = null);

