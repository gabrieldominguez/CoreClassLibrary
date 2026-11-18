namespace CoreClassLibrary.Objects.Disposables;

/// <summary>
/// Null IDisposable object pattern
/// </summary>
/// <see cref="https://en.wikipedia.org/wiki/Null_object_pattern#C#"/>
public class NullDisposable : IDisposable, IAsyncDisposable
    {
    public static IDisposable Instance { get; } = new NullDisposable();

    public void Dispose()
        {
        GC.SuppressFinalize(this);
        }

    public ValueTask DisposeAsync()
        {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
        }
    }
