
using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Objects;

public interface IEntity<Tkey>
    {
    IEnumerable<Tkey> PrimaryKey { get; }
    }

public class IdEntity<T> : Entity<T, int> where T : class, IEntity<int>
    {
    public int Id { get; set; }

    public override IEnumerable<int> PrimaryKey => [ this.Id ];
    }

public abstract class Entity<T> : Entity<T, object> where T : class, IEntity<object>;

public abstract class Entity<T, Tkey> : IEquatable<T>, IComparable<T>, IEntity<Tkey> where T : class, IEntity<Tkey>
    {
    public abstract IEnumerable<Tkey> PrimaryKey { get; }

    public override int GetHashCode()
        => this.PrimaryKey.GetSequenceHashCode();

    public override bool Equals(object? other)
        => Object.ReferenceEquals(this, other) || this.Equals(other as T);

    public bool Equals(T? other)
        => other != null && this.PrimaryKey.SequenceEqual(other.PrimaryKey);

    public int CompareTo(T? other)
        => other == null ?
            1 : // All instances are greater than null
            this.PrimaryKey.CompareTo(other.PrimaryKey);

    public override string ToString() =>
        new System.Text.StringBuilder()
            .Append(this.GetType().Name)
            .Append(' ')
            .Append('(')
            .Append(this.PrimaryKey.ToDelimitedString())
            .Append(')').ToString();
    }
