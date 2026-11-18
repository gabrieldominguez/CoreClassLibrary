namespace CoreClassLibrary.Objects.Refs;

/// <summary>
/// Agrega un nivel de indirección a un objecto
/// </summary>
/// <remarks>
/// Puede usarse para registrar un objeto como singleton y poder cambiarlo posteriormente.
/// Permite pasar por referencias value types cuando no se admita ref, por ej:
/// Error CS1628
/// Cannot use ref, out, or in parameter 'counter' inside an anonymous method, lambda expression, query expression, or local function.
/// </remarks>
/// <see cref="https://learn.microsoft.com/en-us/dotnet/communitytoolkit/high-performance/ref"/>
/// <see cref="https://github.com/CommunityToolkit/dotnet/blob/main/src/CommunityToolkit.HighPerformance/Ref%7BT%7D.cs"/>
public class ReadWrite<T>(T value)
    {
    public T Value { get; set; } = value;
    }
