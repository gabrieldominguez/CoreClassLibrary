using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Objects;

/// <summary>
/// Ejemplo de implementación de una entidad con pk id.
/// </summary>
public class SampleIdEntity : IdEntity<SampleIdEntity>
    {
    public string Desc { get; }

    public SampleIdEntity(
        int id,
        string desc
        )
        {
        this.Id = id;
        this.Desc = desc;
        }

    public override string ToString() =>
        new System.Text.StringBuilder().Append("SampleIdEntity { ").Append("Id = ").Append(this.Id).Append(" }").ToString();
    }

/// <summary>
/// Ejemplo de implementación de una entidad con pk compuesta.
/// </summary>
public class SampleEntity(
    int id1,
    long id2,
    string desc
        ) : Entity<SampleEntity, long>
    {
    public int Id1 { get; } = id1;
    public long Id2 { get; } = id2;

    public string Desc { get; } = desc;

    public override IEnumerable<long> PrimaryKey
        => [ this.Id1, this.Id2 ];

    public override string ToString() =>
        new System.Text.StringBuilder().Append("SampleEntity { ")
            .Append("Id1 = ").Append(this.Id1).Append(", ")
            .Append("Id2 = ").Append(this.Id2)
            .Append(" }").ToString();
    }

/// <summary>
/// Ejemplo de implementación POCO de una entidad con pk compuesta.
/// </summary>
public class SamplePoco(
    int id1,
    long id2,
    string desc
        ) : IEquatable<SamplePoco>, IComparable<SamplePoco>
    {
    public int Id1 { get; } = id1;
    public long Id2 { get; } = id2;

    public string Desc { get; } = desc;

    public IEnumerable<long> PrimaryKey
        => [ this.Id1, this.Id2 ];

    public override int GetHashCode()
        => this.PrimaryKey.GetSequenceHashCode();

    public override bool Equals(object? other)
        => Object.ReferenceEquals(this, other) || this.Equals(other as SamplePoco);

    public bool Equals(SamplePoco? other)
        => other != null && this.PrimaryKey.SequenceEqual(other.PrimaryKey);

    public int CompareTo(SamplePoco? other)
        => other == null ?
            1 : // All instances are greater than null
            this.PrimaryKey.CompareTo(other.PrimaryKey);

    public override string ToString() =>
        new System.Text.StringBuilder().Append("SamplePoco { ")
            .Append("Id1").Append('=').Append(this.Id1).Append(", ")
            .Append("Id2").Append('=').Append(this.Id2).Append(' ')
            .Append('}').ToString();
    }
