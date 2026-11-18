namespace CoreClassLibrary.Objects;

/// <summary>
/// Discriminated union implementation for two types
/// </summary>
/// <see cref="https://en.wikipedia.org/wiki/Tagged_union"/>
public readonly struct Union<T1, T2>
    {
    private readonly T1? value1;
    private readonly T2? value2;
    private readonly int index;

    public Union(T1 value) { this.value1 = value; this.index = 1; }
    public Union(T2 value) { this.value2 = value; this.index = 2; }

    public TResult Match<TResult>(Func<T1, TResult> f1, Func<T2, TResult> f2)
        => this.index switch
            {
            1 => f1(value1!),
            2 => f2(this.value2!),
            _ => throw new InvalidOperationException()
            };
    }
