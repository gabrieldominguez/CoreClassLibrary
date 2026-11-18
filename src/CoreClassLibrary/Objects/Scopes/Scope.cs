namespace CoreClassLibrary.Objects.Scopes;

/// <summary>
/// Crea un scope en el cual se puede acceder a una instancia por medio de la propiedad Current.
/// </summary>
/// <remarks>
/// Esta clase no soporta anidamiento, si es necesario usar NestedScope.
/// Esta clase es sealed porque las clases derivadas compartirian el campo Instances
///
/// ThreadLocal no funciona con el ThreadPool (async/await), usar AsyncLocal.
/// </remarks>
/// <see cref="NestedScope{T}"/>
/// <see cref="https://stackoverflow.com/questions/43814346/why-does-asynclocalt-propagate-to-child-threads-where-as-threadlocalt-does-n"/>
/// <see cref="https://docs.microsoft.com/en-us/dotnet/api/system.threading.asynclocal-1"/>
public sealed class Scope<T> : IDisposable where T : class
    {
    private static readonly AsyncLocal<T?> instance = new AsyncLocal<T?>();
    public static T? Current => Scope<T>.instance.Value;

    private bool disposeInstance;
    public T? Instance { get; private set; }

    /// <summary>
    /// Crea un scope para la instancia.
    /// </summary>
    /// <remarks>
    /// Cuando el scope sea liberado, la instancia tambien lo será.
    /// </remarks>
    /// <param name="instance">Instancia a mantener dentro del scope.</param>
    public Scope(T instance, bool disposeInstance = true)
        {
        if (Scope<T>.Current != null)
            {
            throw new InvalidOperationException($"Scope<{typeof(T).Name}> is already set. Use NestedScope<{typeof(T).Name}> if you need to nest scopes.");
            }
        
        this.disposeInstance = disposeInstance;
        Scope<T>.instance.Value = this.Instance = instance;
        }

    public void Dispose()
        {
        if (this.disposeInstance)
            {
            this.disposeInstance = false;
            (this.Instance as IDisposable)?.Dispose();
            }

        Scope<T>.instance.Value = this.Instance = null;
        }
    }
