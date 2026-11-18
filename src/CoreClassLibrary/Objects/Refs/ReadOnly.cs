namespace CoreClassLibrary.Objects.Refs;

/// <summary>
/// Permite pasar por referencias value types que no pueden ser modificados.
/// </summary>
/// <remarks>
/// Idem Ref pero no permite moficar el valor.
///
/// Para cuando no se admita ref, por ej:
/// Error CS1628
/// Cannot use ref, out, or in parameter 'counter' inside an anonymous method, lambda expression, query expression, or local function.
/// </remarks>
/// <see cref="https://learn.microsoft.com/en-us/dotnet/communitytoolkit/high-performance/ref#readonlyreft"/>
/// <see cref="https://github.com/CommunityToolkit/dotnet/blob/main/src/CommunityToolkit.HighPerformance/Ref%7BT%7D.cs"/>
public record ReadOnly<T>(T Value);
