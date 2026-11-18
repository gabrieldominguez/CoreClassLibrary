using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects;

namespace CoreClassLibrary.Validation;

/// <summary>
/// Ejemplo de implementación de una entidad con pk id.
/// </summary>
public class ValidationSampleIdEntity : IdEntity<ValidationSampleIdEntity>
    {
    [Required]
    public string Desc { get; private set; }

    public ValidationSampleIdEntity(
        int id,
        string desc
        ) 
        {
        this.Id = id;
        this.Desc = desc;
        }

    public override string ToString() =>
        new System.Text.StringBuilder().Append("SampleIdEntity { ")
            .Append("Id").Append('=').Append('"').Append(this.Id).Append('"').Append(' ')
            .Append('}').ToString();
    }

/// <summary>
/// Ejemplo de implementación de una entidad con pk compuesta.
/// </summary>
public class ValidationSampleEntity(
    int id1,
    long id2,
    string desc
        ) : Entity<ValidationSampleEntity, long>
    {
    [Required, Key, Column(Order = 1)]
    public int Id1 { get; private set; } = id1;

    [Required, Key, Column(Order = 2)]
    public long Id2 { get; private set; } = id2;

    [Required]
    public string Desc { get; private set; } = desc;

    public override IEnumerable<long> PrimaryKey
        => [ this.Id1, this.Id2 ];

    public override string ToString() =>
        new System.Text.StringBuilder().Append("SampleEntity { ")
            .Append("Id1").Append('=').Append('"').Append(this.Id1).Append('"').Append(' ')
            .Append("Id2").Append('=').Append('"').Append(this.Id2).Append('"').Append(' ')
            .Append('}').ToString();
    }

/// <summary>
/// Ejemplo de implementación POCO de una entidad con pk compuesta.
/// </summary>
public class ValidationSamplePoco(
    int id1,
    long id2,
    string desc
        ) : IEquatable<ValidationSamplePoco>, IComparable<ValidationSamplePoco>
    {
    [Required, Key, Column(Order = 1)]
    public int Id1 { get; private set; } = id1;

    [Required, Key, Column(Order = 2)]
    public long Id2 { get; private set; } = id2;

    [Required]
    public string Desc { get; private set; } = desc;

    public IEnumerable<long> PrimaryKey
        => [ this.Id1, this.Id2 ];

    public override int GetHashCode()
        => this.PrimaryKey.GetSequenceHashCode();

    public override bool Equals(object? other)
        => Object.ReferenceEquals(this, other) || this.Equals(other as ValidationSamplePoco);

    public bool Equals(ValidationSamplePoco? other)
        => other != null && this.PrimaryKey.SequenceEqual(other.PrimaryKey);

    public int CompareTo(ValidationSamplePoco? other)
        => other == null ?
            1 : // All instances are greater than null
            this.PrimaryKey.CompareTo(other.PrimaryKey);

    public override string ToString() =>
        new System.Text.StringBuilder().Append("SamplePoco { ")
            .Append("Id1").Append('=').Append('"').Append(this.Id1).Append('"').Append(' ')
            .Append("Id2").Append('=').Append('"').Append(this.Id2).Append('"').Append(' ')
            .Append('}').ToString();
    }
