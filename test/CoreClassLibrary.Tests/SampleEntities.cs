#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace CoreClassLibrary.Tests.SampleClass
    {
    public class LogEntry : IEquatable<LogEntry>, IComparable<LogEntry>
        {
        public long Id { get; set; }

        public DateTime DateTime { get; set; }

        public int Level { get; set; }

        public double LevelDouble { get; set; }

        public decimal LevelDecimal { get; set; }

        public string Name { get; set; } = "";

        public override bool Equals(object? other)
            => Object.ReferenceEquals(this, other) || this.Equals(other as LogEntry);

        public override int GetHashCode()
            => this.Id.GetHashCode();

        public bool Equals(LogEntry? other)
            => other != null && this.Id.Equals(other.Id);

        public int CompareTo(LogEntry? other)
            => other == null ?
                1 : // All instances are greater than null
                this.Id.CompareTo(other.Id);

        public override string ToString()
            => new System.Text.StringBuilder("LogEntry { ")
                .Append("Id = ").Append(this.Id).Append(", ")
                .Append("DateTime = ").Append(this.DateTime).Append(", ")
                .Append("Level = ").Append(this.Level).Append(", ")
                .Append("LevelDouble = ").Append(this.LevelDouble).Append(", ")
                .Append("LevelDecimal = ").Append(this.LevelDecimal).Append(", ")
                .Append("Name = ").Append(this.Name)
                .Append(" }").ToString();
        }
    }

namespace CoreClassLibrary.Tests.SampleRecord
    {
    public record LogEntry(
        long Id,
        DateTime DateTime,
        int Level,
        double LevelDouble,
        decimal LevelDecimal,
        string Name
        );
    }
