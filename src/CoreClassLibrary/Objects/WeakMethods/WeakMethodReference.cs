using CoreClassLibrary.Extensions;
using System.Reflection;

namespace CoreClassLibrary.Objects.WeakMethods;

/// <summary>
/// Mantiene una referencia a un metodo, permitiendo que el objeto al que pertenece 
/// pueda ser reclamado por el Garbage Collector.
/// </summary>
public class WeakMethodReference : IEquatable<WeakMethodReference>
    {
    private readonly MethodInfo method;
    private readonly WeakReference? reference;

    public WeakMethodReference(Delegate del): this(del.GetMethodInfo(), del.Target) { }

    public WeakMethodReference(MethodInfo method, object? reference = null)
        {
        if (!(this.method = method).IsStatic)
            {
            this.reference = new WeakReference(reference);
            }
        }

    /// <summary>
    /// true si la instancia a la que pertenece el método es accesible.
    /// </summary>
    /// <remarks>Para métodos estáticos siempre es true.</remarks>
    public bool IsAlive 
        => this.method.IsStatic || (this.reference?.IsAlive).ToBoolean();

    /// <summary>
    /// Instancia a la que pertenece el método.
    /// </summary>
    /// <remarks>
    /// null para métodos estáticos.
    /// </remarks>
    public object? Target 
        => this.method.IsStatic ? null : this.reference?.Target;

    /// <summary>
    /// Crea un delegado al método del tipo especificado.
    /// </summary>
    public T CreateDelegate<T>() where T : class 
        => (this.method.CreateDelegate(typeof(T), this.Target) as T)!;

    public override int GetHashCode()
        => this.method.GetHashCode();

    public override bool Equals(object? obj)
        => Object.ReferenceEquals(this, obj) || this.Equals(obj as WeakMethodReference);

    public bool Equals(WeakMethodReference? other)
        => other != null && this.method.Equals(other.method);
    }
