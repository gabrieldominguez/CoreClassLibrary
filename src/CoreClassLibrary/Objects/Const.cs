namespace CoreClassLibrary.Objects;

/// <summary>
/// Guarda un valor en la propiedad Value.
/// </summary>
/// <remarks>
/// La propiedad es mantenida aunque se intenten setear nuevos valores, los que son descartados.
/// </remarks>
public class Const<T>
    {
    private readonly Lock syncRoot = new Lock();
    private bool canSetValue = true;

    private T? value;
    public T? Value
        {
        get => this.value;
        set => this.SetValue(value);
        }

    /// <summary>
    /// Guarda el valor en la propiedad si todavia no tenia otro.
    /// </summary>
    /// <param name="value">Valor a guardar.</param>
    /// <returns>True si el valor fue guardado.</returns>
    public bool SetValue(T? value)
        {
        if (this.canSetValue)
            {
            lock (this.syncRoot)
                {
                if (this.canSetValue)
                    {
                    this.value = value;
                    this.canSetValue = false;
                    return true;
                    }
                }
            }
        return false;
        }
    }
