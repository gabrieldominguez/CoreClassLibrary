namespace CoreClassLibrary.Objects.Refs;

/// <summary>
/// Asocia un DateTime a un objecto
/// </summary>
/// <see cref="System.Reactive.Timestamped"/>
public record DateTimeStamped<T>(T Value, DateTimeOffset DateTimeStamp)
    {
    public DateTimeStamped(T value): this(value, TimeProvider.System) { }
    public DateTimeStamped(T value, TimeProvider timeProvider): this(value, timeProvider.GetLocalNow()) { }
    }
