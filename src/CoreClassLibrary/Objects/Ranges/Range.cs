#pragma warning disable IDE0046 // Convert to conditional expression
namespace CoreClassLibrary.Objects.Ranges;

/// <summary>
/// Rango entre dos valores.
/// </summary>
/// <remarks>
/// Se asume que Start es menor o igual a End y que ambos están contenidos en el rango.
/// </remarks>
public readonly record struct Range<X>(X Start, X End) where X : IComparable<X>
    {
    public bool Contains(Range<X> innerRange)
        => this.Contains(innerRange.Start) && this.Contains(innerRange.End);

    /// <summary>
    /// Retorna True si el parametro se encuentra dentro del rango.
    /// </summary>
    public bool Contains(X value)
        => this.Start.CompareTo(value) <= 0 && value.CompareTo(this.End) <= 0;

    /// <summary>
    /// Si el elemento se encuentra fuera del rango, retorna el limite correspondiente.
    /// </summary>
    public X BoundaryConstraint(X value)
        {
        if (value.CompareTo(this.Start) < 0)
            {
            return this.Start;
            }

        if (value.CompareTo(this.End) > 0)
            {
            return this.End;
            }

        return value;
        }

    /// <summary>
    /// Valida si el parametro se encuentra dentro del rango.
    /// </summary>
    public void ThrowIfNotContains(X value)
        {
        if (!this.Contains(value))
            {
            throw new OutOfRangeException<X>(nameof(value), value, this.Start, this.End);
            }
        }

    public (X, X) ToTuple()
        => (this.Start, this.End);

    public X[] ToVector()
        => [ this.Start, this.End ];

    public override string ToString()
        => String.Concat(this.Start.ToString(), "-", this.End.ToString());
    }
