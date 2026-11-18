namespace CoreClassLibrary.Objects.Ranges;

public class OutOfRangeException<T>(string paramName, T actualValue, T minValue, T maxValue, string message) : ArgumentOutOfRangeException(paramName, actualValue, message)
    {
    public T Value { get; } = actualValue;
    public T MinValue { get; } = minValue;
    public T MaxValue { get; } = maxValue;

    public OutOfRangeException(string paramName, T actualValue, T minValue, T maxValue) : this(paramName, actualValue, minValue, maxValue, $"Value {actualValue:0.00} must be between {minValue:0.00} and {maxValue:0.00}") { }
    }
