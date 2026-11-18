namespace CoreClassLibrary.Objects.Ranges;

public static class RangeExtensions
    {
    public static Range<int> ToIntRange(this Range range)
        => new Range<int>(range.Start.Value, range.End.Value-1);

    public static Range ToRange(this Range<int> range)
        => new Range(range.Start, range.End + 1);
    }
