namespace CoreClassLibrary.Objects.Refs;

/// <summary>
/// Un ref que puede expirar
/// </summary>
/// <remarks>
/// Si al acceder al objeto este ha expirado se retorna null (y se desreferencia)
/// </remarks>
public class ExpirableRef<T>(Func<T, bool> isExpired) where T: class
    {
    public static ExpirableRef<T> Null { get; } = new ExpirableRef<T>(_ => true);

    private T? _value;
    public T? Value { get => this._value == null || isExpired(this._value)? (this._value = null): this._value; set => this._value = value; }
    }
