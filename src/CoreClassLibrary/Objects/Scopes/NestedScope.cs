namespace CoreClassLibrary.Objects.Scopes;

/// <summary>
/// Crea un NestedScope que admite anidamiento en el cual se puede acceder a una instancia por medio de la propiedad Current.
/// </summary>
/// <remarks>
/// Esta clase es sealed porque las clases derivadas compartirian el campo Instances
/// </remarks>
public sealed class NestedScope<T> : IDisposable where T : class
    {
    private static readonly AsyncLocal<Stack<T>?> instances = new AsyncLocal<Stack<T>?>();
    private static readonly Lock syncRoot = new Lock();

    public static T? Current => NestedScope<T>.instances.Value != null && NestedScope<T>.instances.Value.Count > 0 ? NestedScope<T>.instances.Value.Peek() : null;

    /// <summary>
    /// True si es el scope mas externo.
    /// </summary>
    public bool IsRoot { get; }
    private bool disposeInstance;
    public T? Instance { get; private set; }

    /// <summary>
    /// Crea un scope para la instancia.
    /// </summary>
    /// <remarks>
    /// Cuando el scope sea liberado, la instancia tambien lo será.
    /// </remarks>
    /// <param name="instance">Instancia a mantener dentro del RecursiveScope.</param>
    public NestedScope(T instance, bool disposeInstance = true)
        {
        if (NestedScope<T>.instances.Value == null)
            {
            lock (NestedScope<T>.syncRoot)
                {
                if (NestedScope<T>.instances.Value == null)
                    {
                    NestedScope<T>.instances.Value = new Stack<T>();
                    }
                }
            }

        this.IsRoot = NestedScope<T>.instances.Value.Count == 0;
        NestedScope<T>.instances.Value.Push(this.Instance = instance);
        this.disposeInstance = disposeInstance;
        }

    public void Dispose()
        {
        if (this.disposeInstance)
            {
            this.disposeInstance = false;
            (this.Instance as IDisposable)?.Dispose();
            }

        this.Instance = default;
        NestedScope<T>.instances.Value?.Pop();
        }
    }
