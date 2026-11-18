namespace CoreClassLibrary.Objects.Pipes;

public static class PipeExtensions
    {
    public static TOutput? TryTransform<TInput, TOutput>(this IPipe<TInput, TOutput> pipe, TInput input)
        {
        try
            { 
            return pipe.Transform(input);
            }
        catch 
            {
            return default;
            }
        }

    public static Func<TInput, TOutput> ToFunc<TInput, TOutput>(this IPipe<TInput, TOutput> pipe)
        => pipe.Transform;
    }
