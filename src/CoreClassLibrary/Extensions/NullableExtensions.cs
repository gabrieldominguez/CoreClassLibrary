using System.Globalization;

namespace CoreClassLibrary.Extensions;

public static class NullableExtensions
    {
    public static bool ToBoolean(this bool? nullable) 
        => nullable ?? false;

    public static string ToStr(this string? nullable) 
        => nullable?.ToString() ?? "";

    public static string NullableToString<T>(this T? nullable) where T : struct 
        => nullable?.ToString() ?? "";

    public static string NullableToString(this int? nullable, string format) 
        => nullable?.ToString(format, CultureInfo.InvariantCulture) ?? "";

    public static string NullableToString(this long? nullable, string format) 
        => nullable?.ToString(format, CultureInfo.InvariantCulture) ?? "";

    public static string NullableToString(this DateTime? nullable, string format) 
        => nullable?.ToString(format, CultureInfo.InvariantCulture) ?? "";
    }
