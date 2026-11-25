namespace CoreClassLibrary.Objects.Pipes.Strings.GetPath;

/// <summary>
/// Retorna la parte correspondiente al directorio del path.
/// </summary>
public class GetDirectoryNamePipe : IPipe<string>
    {
    public string Transform(string input) => Path.GetDirectoryName(input) ?? "";
    }
