namespace CoreClassLibrary.Objects.Ranges;

/// <summary>
/// Rango circular entre dos valores.
/// </summary>
/// <remarks>
/// Una rango dentro de un espacio cíclico, por ej. las horas, los dias de semana, los meses, los caracteres, etc.
/// 
/// Si Start es menor o igual a End es equivalente a Range.
/// Sino es el complemento dentro del espacio cíclico.
/// </remarks>
/// <example>
/// <![CDATA[
///     CircularRange<TimeSpan>(TimeSpan.Parse(20:00:00), TimeSpan.Parse(05:00:00))
///  es el complemento de 
///     CircularRange<TimeSpan>(TimeSpan.Parse(05:00:00), TimeSpan.Parse(20:00:00))
/// ]]>
/// </example>
public readonly record struct CircularRange<T> where T : IComparable<T>
    {
    private readonly Range<T> range;

    public CircularRange(T start, T end): this(new Range<T>(start, end)) { }

    public CircularRange(Range<T> range)
        {
        this.range = range;
        }

    public bool Contains(T value) 
        => this.range.Start.CompareTo(this.range.End) <= 0 ?
            this.range.Contains(value) :
            this.range.Start.CompareTo(value) <= 0 || value.CompareTo(this.range.End) <= 0;
    }
