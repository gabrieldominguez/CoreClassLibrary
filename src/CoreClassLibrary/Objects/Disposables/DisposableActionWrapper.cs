namespace CoreClassLibrary.Objects.Disposables;

/// <summary>
/// Permite ejecutar una acción al terminar un bloque using
/// </summary>
/// <example>
/// using(new DisposableAction(...)) { ... }
/// </example>
public sealed class DisposableActionWrapper(Action action) : IDisposable
    {
    public void Dispose() => action();
    }

public sealed class DisposableAsyncActionWrapper(Func<ValueTask> action) : IAsyncDisposable
    {
    public ValueTask DisposeAsync() => action();
    }
